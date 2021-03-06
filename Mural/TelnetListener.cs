using System;
using System.Net;
using System.Net.Sockets;
using log4net;

namespace Mural
{
	public class TelnetListener
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(TelnetListener));
		
		public TelnetListener(
			ILineConsumer defaultParser, 
			ISystemMessageProvider systemMessageProvider, 
			IPAddress ipAddress, 
			int port)
		{
			this._defaultParser = defaultParser;
			this._systemMessageProvider = systemMessageProvider;
			this._ipAddress = ipAddress;
			this._port = port;
		}
				
		public void StartListenerLoop()
		{
			// For now, this is based on the MSDN sample at: http://msdn.microsoft.com/en-us/library/5w7b7x5f.aspx
			
			IPEndPoint localEndPoint = new IPEndPoint(_ipAddress, _port); 
			
			int backlogSize = 10; // The maximum number of connections that can be pending while the socket is listening.
			// In normal MU*-style usage, this backlog should never get very large; MU* servers generally rely on a relatively
			// small number of connections, each of which can be responded to promptly and spends most of its time idle.
			// This should be set in the config file, really.
			
			_log.DebugFormat("Mural server listening on {0}", localEndPoint.ToString());
			
			Socket listener = new Socket(localEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			
			try {
				listener.Bind(localEndPoint);
				listener.Listen(backlogSize);
				
				_log.Debug("Waiting to accept connection.");
					listener.BeginAccept(
						new AsyncCallback(this.acceptCallback), 
					    listener);
				
			} catch ( Exception e ) {
				// This error-handling is not ready for prime-time.
				_log.Error(e.ToString());
			}
		}
		
		private void acceptCallback(IAsyncResult asyncResult)
		{
			Socket listener = (Socket) asyncResult.AsyncState;
			Socket handler = listener.EndAccept(asyncResult);
			// At this point, we have "handler", which is a socket connected to the end user.
			
			_log.Debug("Establishing session.");
			
			// Create a new TelnetSession to handle this connection
			TelnetSession session = new TelnetSession(handler);
			
			// Hook the session up to the default parser for this system.
			_defaultParser.AddSource(session);
			
			// Start the TelnetSession running.
			session.BeginRecieve();
			
			// Get the login message and transmit it to the user.
			foreach(string line in _systemMessageProvider.GetMessage("login", "terminal.telnet.plaintext"))
			{
				// Rather than hook up an event to this listener, 
				// raise the event once per line, then unhook the event,
				// we are directly invoking the event handler.
				session.HandleResponseEvent(this, new ResponseLineEventArgs(line));
			}
			
			// Enqueue an additional asynchronous accept.
			_log.Debug("Waiting to accept connection.");
			listener.BeginAccept(
				new AsyncCallback(this.acceptCallback), 
			    listener);
		}
					
		private ILineConsumer _defaultParser;
		private ISystemMessageProvider _systemMessageProvider;
		
		private IPAddress _ipAddress;
		private int _port;
	}
}

