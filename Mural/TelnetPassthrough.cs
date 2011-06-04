using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Mural
{
	public class TelnetPassthrough : ILineConsumer
	{
		public TelnetPassthrough(string remoteHostName, int remotePort)
		{
			_remoteHostName = remoteHostName;
			_remotePort = remotePort;
		}
		public TelnetPassthrough(ServerAddress serverAddress)
		{
			_remoteHostName = serverAddress.Hostname;
			_remotePort = serverAddress.Port;
		}
		
		// This should be a little more cautious about what happens if it gets called when it's already connected.
		// Also, this is full of synchronous network calls. So very not-ready-for-prime-time.
		public bool Connect()
		{
			Socket outboundSocket = null;
			
			IPHostEntry hostEntry = null;

        	// Get host related information.
        	hostEntry = Dns.GetHostEntry(_remoteHostName);

	        // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
	        // an exception that occurs when the host IP Address is not compatible with the address family
	        // (typical in the IPv6 case).
	        foreach(IPAddress address in hostEntry.AddressList)
	        {
	            IPEndPoint endPoint = new IPEndPoint(address, _remotePort);
	            Socket tempSocket = 
	                new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
	
	            tempSocket.Connect(endPoint);
	
	            if(tempSocket.Connected)
	            {
	                outboundSocket = tempSocket;
	                break;
	            }
	            else
	            {
	                continue;
	            }
	        }
			
			if (outboundSocket == null)
			{
				return false;
			}
			else 
			{
				//_outboundSession = new TelnetSession(outboundSocket);
				_outboundSession = new SslSession(outboundSocket); // Test SSLed connection.
				_outboundSession.ConnectLineConsumer(this.MyOutboundSessionLineConsumer);
				_outboundSession.BeginRecieve();
				return true;
			}
		}
		
		public void HandleLineReadyEvent(object sender, LineReadyEventArgs args) 
		{
			SynchronousSession senderSession = sender as SynchronousSession;
			
			string line = args.Line;
			if (line.StartsWith(_commandPrefix))
			{
				ProcessLineAsCommand(args.Origin, line.Substring(_commandPrefix.Length));	
			}
			else
			{
				SendLineToOutboundSession(senderSession, line);	
			}
		}
		
		public void HandleDisconnectEvent(object sender, EventArgs args)
		{
			SynchronousSession session = sender as SynchronousSession;
			session.DisconnectLineConsumer(this);
			
			DisconnectRemote();
		}
		
		public void AddSource (IResponseConsumer source) 
		{
			if (_source != null)
			{
				throw new Exception("TelnetPassthrough can only have one source.");	
			}
			else
			{
				SynchronousSession synchronousSource = source as SynchronousSession;
				if (synchronousSource == null)
				{
					throw new Exception("TelnetPassthrough must have a SynchronousSource as a source.");	
				}
				else
				{
					_source = synchronousSource;
				}
			}
		}
		public void RemoveSource (IResponseConsumer source)
		{
			if (_source == source)
			{
				_source = null;
			}
		}
		
		private OutboundSessionLineConsumer _outboundSessionLineConsumer;
		private OutboundSessionLineConsumer MyOutboundSessionLineConsumer
		{
			get
			{
				if (_outboundSessionLineConsumer == null)
				{
					_outboundSessionLineConsumer = new OutboundSessionLineConsumer(this);	
				}
				return _outboundSessionLineConsumer;
			}
		}
		
		// Internal class that the outbound session sends its lines to.
		private class OutboundSessionLineConsumer : ILineConsumer
		{
			public OutboundSessionLineConsumer(TelnetPassthrough parent)
			{
				_parent = parent;
			}
			
			// Distribute the incoming line to all of the sessions connected to this parser.
			public void HandleLineReadyEvent(object sender, LineReadyEventArgs args) 
			{
				_parent.SendLineToUser(args.Line);	
			}
			public void HandleDisconnectEvent(object sender, EventArgs args)
			{
				_parent.SendLineToUser("Remote server disconnected.");
				_parent._outboundSession.DisconnectLineConsumer(this);
				_parent._outboundSession = null;
			}	
			
			// Doesn't care about its sources.
			public void AddSource (IResponseConsumer source) {}
			public void RemoveSource (IResponseConsumer source) {}
			
			private TelnetPassthrough _parent;
		}
		
		private void SendLineToUser(string line)
		{
			_source.SendLineToUser(line);
		}
		
		private void ProcessLineAsCommand(SynchronousSession origin, string line)
		{
			// If we start with the commandPrefix twice in a row, the user is escaping
			// a line that starts with the commandPrefix.
			if (line.StartsWith(_commandPrefix))
			{
				SendLineToOutboundSession(origin, line);	
			}
			else
			{
				string[] args = line.Split(' ');
				switch (args[0].Trim().ToLower())
				{
				case "quit":
					_source.SendLineToUser("Goodbye!");
					_source.Disconnect();
					_source.DisconnectLineConsumer(this);
					DisconnectRemote();
					break;
				case "detach":
					origin.SendLineToUser("Disconnecting this session, but staying logged in.");
					origin.Disconnect();
					break;
				case "recall":
					int linesToRecall = 10;
					if (args.Length > 1)
					{
						int.TryParse(args[1], out linesToRecall);
					}
					CharacterSession session = _source as CharacterSession;
					if (session != null)
					{
						if (session.Buffer != null)
						{
							int linesInBuffer = session.Buffer.Buffer.Count;
							int actualLinesToRecall = Math.Min(linesInBuffer, linesToRecall);
							for (int i = linesInBuffer - actualLinesToRecall; i < linesInBuffer; i++)
							{
								origin.SendLineToUser(session.Buffer.Buffer[i]);	
							}
						}
						else
						{
							origin.SendLineToUser("This session isn't buffered.");	
						}
					}
					else
					{
						origin.SendLineToUser("Huh. There isn't a CharacterSession upstream of me. That's weird. This bit of code is kind of cheesy, though.");	
					}
					break;
				case "help":
				default:
					origin.SendLineToUser(String.Format("{0}help - display this help listing", _commandPrefix));
					origin.SendLineToUser(String.Format("{0}quit - disconnect from the local and remote server", _commandPrefix));
					origin.SendLineToUser(String.Format("{0}detach - disconnect from the local and server, but remain connected", _commandPrefix));
					origin.SendLineToUser(String.Format("{0}{0} - start a line with {0} twice to send a line that starts with {0} to the remote server", _commandPrefix));
					break;
				}
			}
		}
		
		private void DisconnectRemote()
		{
			Console.WriteLine("Disconnecting remote.");
			if (_outboundSession != null)
			{
				Console.WriteLine("Outbound session is present; disconnecting it.");
				_outboundSession.Disconnect();
				_outboundSession = null;
			}	
		}
		
		private void SendLineToOutboundSession(SynchronousSession origin, string line)
		{
			if (_outboundSession != null)
			{
				_outboundSession.SendLineToUser(line);	
			}
			else 
			{
				origin.SendLineToUser("The redirector isn't connected.");
			}
		}
		
		private string _commandPrefix = "//";
		
		private SynchronousSession _source;
		private string _remoteHostName;
		private int _remotePort;
		
		private TelnetSession _outboundSession;
	}
}

