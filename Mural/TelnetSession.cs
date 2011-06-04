using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Mural
{
	/// <summary>
	/// A TelnetSession manages the lifespan of a telnet connection. It should be constructed on the <see cref="Socket"/>
	/// immediately after the listener establishes it (or immediately after it's connected on an outbound socket).
	/// 
	/// TelnetSession is a line-oriented reader.
	/// Call BeginRecieve to start its asynchronous listening for new data. When a complete line is available,
	/// TelnetSession will raise its LineReadyEvent. To get the data from this socket, you should register as 
	/// a listener for LineReadyEvent.
	/// </summary>
	public class TelnetSession : SynchronousSession
	{
		/// <summary>
		/// Constructs a new TelnetSession that handles data coming in on the provided socket.
		/// After constructing a TelnetSession, call BeginRecieve to start asynchronously recieving.
		/// </summary>
		/// <param name="sessionSocket">
		/// A <see cref="Socket"/> to recieve data for. It should be connected, but not yet recieving.
		/// </param>
		public TelnetSession(Socket sessionSocket)
		{
			_sessionSocket = sessionSocket;
			_sessionStream = new NetworkStream(sessionSocket);
		}
		
		/// <summary>
		/// Start the asynchronous reading for this TelnetSession. This method is non-blocking.
		/// </summary>
		public virtual void BeginRecieve()
		{
			ConnectionStream.BeginRead(
				_buffer, 
				0, // Offset into the buffer - we don't offset! 
				_bufferLength, 
				new AsyncCallback(TelnetSession.RecieveText),
				this);
		}
		
		public override void SendLineToUser(string line)
		{
			// Right now, this is a synchronous send. This should be made asynchronous before too long.
			byte[] buffer = Encoding.ASCII.GetBytes(line);
			ConnectionStream.Write(buffer, 0, buffer.Length);
			
			// This is a hackish way of sending a newline after the line!
			buffer = Encoding.ASCII.GetBytes("\r\n");
			ConnectionStream.Write(buffer, 0, buffer.Length);
		}
		
		public override void Disconnect()
		{
			ConnectionStream.Close();
			_sessionSocket.Disconnect(false); // You must disconnect the underlying socket explicitly.
			
			Console.WriteLine("Session disconnected by server.");
			OnRaiseDisconnectEvent();
		}
		
		private static void RecieveText(IAsyncResult asyncResult)
		{
			TelnetSession state = (TelnetSession) asyncResult.AsyncState;
			Stream stream = state.ConnectionStream;		
			
			int bytesRead = stream.EndRead(asyncResult);
			
			if (bytesRead > 0) 
			{
				state._lineBuilder.Append(Encoding.ASCII.GetString(state._buffer, 0, bytesRead));
				state.ProcessCompleteLines();
				state.BeginRecieve();
			}
			else
			{
				Console.WriteLine("Session disconnected by remote.");
				// Raise the disconnection event.
				state.OnRaiseDisconnectEvent();
				
				stream.Close(); // Close this stream.
				state._sessionSocket.Disconnect(false); // You must disconnect the underlying socket explicitly.
			}
		}
		
		/// <summary>
		/// Look at the buffer, removing completed lines and raising a LineReadyEvent for each.
		/// </summary>
		private void ProcessCompleteLines()
		{
			// Not the most efficient way of doing this, but we're going for clarity in the first version.
			// We can make this faster / more memory-efficient if the string allocations and searches here turn
			// out to have a meaningful effect on perf.
			
			// We can't search a lineBuilder, so we work on strings in here.
			// The memory ramifications of this make me sad.
			string input = _lineBuilder.ToString(); 
			
			int newlinePosition = input.IndexOfAny(_newlines);
			while (newlinePosition >= 0)
			{
				// We don't include the line break in the string we store.
				string completeLine = input.Substring(0, newlinePosition);
				
				// Raise an event that the input line is ready to be parsed.
				Console.WriteLine("Input line: {0}", completeLine);
				LineReadyEventArgs eventArgs = new LineReadyEventArgs(completeLine, this);
				OnRaiseLineReadyEvent(eventArgs);
				
				input = input.Substring(newlinePosition + 1);
				newlinePosition = input.IndexOfAny(_newlines);
			}
			
			_lineBuilder = new StringBuilder(input);
		}
		
		protected virtual Stream ConnectionStream
		{
			get
			{
				return this._sessionStream;	
			}
		}
		
		/// <summary>
		/// Array of characters we recognize as line-separators. The actual separator we seem to be getting is \r\n.
		/// </summary>
		private char[] _newlines = {'\n'};
		
		private const int _bufferLength = 1024; // Does this want to be configurable?
		private byte[] _buffer = new byte[_bufferLength];
		
		// We maintain a reference to the underlying socket so that we can disconnect it 
		// when the session is closed.
		private Socket _sessionSocket;
		private NetworkStream _sessionStream;
		
		private StringBuilder _lineBuilder = new StringBuilder();
	}

}

