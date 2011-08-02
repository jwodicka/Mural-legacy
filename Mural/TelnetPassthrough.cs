using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using log4net;

namespace Mural
{
	public class TelnetPassthrough : WorldRouter
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(TelnetPassthrough));

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
		
		// TODO: This should be a little more cautious about what happens if it gets called when it's already connected.
		// Also, this is full of synchronous network calls. So very not-ready-for-prime-time.
		public override bool Connect()
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
				// TODO: The Telnet / SSL distinction in here is hardcoded. Fix it. (Probably by creating an SSLPassthrough)
				//_outboundSession = new TelnetSession(outboundSocket);
				_outboundSession = new SslSession(outboundSocket); // Test SSLed connection.
				
				// Add the outbound session as a source to the OutboundSessionLineConsumer.
				this.MyOutboundSessionLineConsumer.AddSource(_outboundSession);
				_outboundSession.BeginRecieve();
				return true;
			}
		}
		
		public override void HandleUserEvent (object sender, UserEventArgs args)
		{
			switch (args.EventType)
			{
			case "LineReady":
				var lineArgs = args as LineReadyEventArgs;
				HandleLineReadyEvent(sender, lineArgs);
				break;
			case "Disconnect":
				var disconnectArgs = args as DisconnectEventArgs;
				HandleDisconnectEvent(sender, disconnectArgs);
				break;
			default:
				throw new NotImplementedException(String.Format("Unsupported EventType: {0}", args.EventType));
			}
		}
		
		public void HandleLineReadyEvent(object sender, LineReadyEventArgs args) 
		{
			string line = args.Line;
			if (line.StartsWith(_commandPrefix))
			{
				ProcessLineAsCommand(args, line.Substring(_commandPrefix.Length));	
			}
			else
			{
				SendLineToOutboundSession(args, line);	
			}
		}
		
		public void HandleDisconnectEvent(object sender, EventArgs args)
		{
			// Break the listening relationship with the disconnected sender.
			RemoveSource(sender as IResponseConsumer);
			
			// Request that the remote session be disconnected.
			DisconnectRemote();
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
		// TODO: It's kind of hacky that this considers the remote server a "user."
		//  On the other hand, it allows reuse of the socket-management objects.
		private class OutboundSessionLineConsumer : BasicLineConsumer
		{
			public OutboundSessionLineConsumer(TelnetPassthrough parent)
			{
				_parent = parent;
			}
			
			// This method recieves lines from the remote server;
			// from the perspective of this object, the remote server is its "user."
			public override void HandleUserEvent (object sender, UserEventArgs args)
			{
				switch (args.EventType)
				{
				case "LineReady":
					var lineArgs = args as LineReadyEventArgs;
					// The parent's user is the actual end-user to whom lines should
					// be routed.
					_parent.SendLineToUser(lineArgs.Line);	
					break;
				case "Disconnect":
					// The parent's user is the actual end-user.
					_parent.SendLineToUser("Remote server disconnected.");
					this.RemoveSource(_parent._outboundSession);
					_parent._outboundSession = null;
					break;
				default:
					throw new NotImplementedException(String.Format("Unsupported EventType: {0}", args.EventType));
				}
			}	
						
			public void RequestDisconnect()
			{
				// The remote server is the "user" here - we are asking the connection
				// to the remote session to disconnect.
				SendGlobalDisconnectRequestToUser();	
			}
			
			public void SendLine(string line)
			{
				// The remote server is the "user" here - we are sending this line
				// to the remote session.
				SendLineToUser(line);	
			}
			
			private TelnetPassthrough _parent;
		}
				
		private void ProcessLineAsCommand(LineReadyEventArgs lineArgs, string line)
		{
			// If we start with the commandPrefix twice in a row, the user is escaping
			// a line that starts with the commandPrefix.
			if (line.StartsWith(_commandPrefix))
			{
				SendLineToOutboundSession(lineArgs, line);	
			}
			else
			{
				string[] args = line.Split(' ');
				switch (args[0].Trim().ToLower())
				{
				case "quit":
					SendLineToUser("Goodbye!");
					SendGlobalDisconnectRequestToUser();
					
					// This will not "disconnect" any logger / multiplexer upstream of this service;
					// it will remain attached to this service, but any synchronous endpoint will be
					// asked to disconnect.
					
					DisconnectRemote();
					break;
				case "detach":
					lineArgs.Respond("Disconnecting this session, but staying logged in.");
					// Request that the session the user sent this command from disconnect itself.
					SendDisconnectRequestByIdentifierToUser(lineArgs.OriginIdentifier);
					break;
				case "recall":
					int linesToRecall = 10;
					if (args.Length > 1)
					{
						int.TryParse(args[1], out linesToRecall);
					}
					if (Buffer != null)
					
					{
						int linesInBuffer = Buffer.Buffer.Count;
						int actualLinesToRecall = Math.Min(linesInBuffer, linesToRecall);
						for (int i = linesInBuffer - actualLinesToRecall; i < linesInBuffer; i++)
						{
							lineArgs.Respond(Buffer.Buffer[i]);	
						}
					}
					else
					{
						lineArgs.Respond("This session isn't buffered.");	
					}
					break;
				case "help":
				default:
					lineArgs.Respond(String.Format("{0}help - display this help listing", _commandPrefix));
					lineArgs.Respond(String.Format("{0}quit - disconnect from the local and remote server", _commandPrefix));
					lineArgs.Respond(String.Format("{0}detach - disconnect from the local and server, but remain connected", _commandPrefix));
					lineArgs.Respond(String.Format("{0}{0} - start a line with {0} twice to send a line that starts with {0} to the remote server", _commandPrefix));
					break;
				}
			}
		}
		
		private void DisconnectRemote()
		{
			_log.Debug("Disconnecting remote.");
			if (_outboundSession != null)
			{
				_log.Debug("Outbound session is present; disconnecting it.");
				MyOutboundSessionLineConsumer.RequestDisconnect();
				MyOutboundSessionLineConsumer.RemoveSource(_outboundSession);
				_outboundSession = null;
			}	
		}
		
		private void SendLineToOutboundSession(LineReadyEventArgs lineArgs, string line)
		{
			if (_outboundSession != null)
			{
				MyOutboundSessionLineConsumer.SendLine(line);
			}
			else 
			{
				lineArgs.Respond("The redirector isn't connected.");
			}
		}
		
		private string _commandPrefix = "//";
		
		private string _remoteHostName;
		private int _remotePort;
		
		private TelnetSession _outboundSession;
	}
}

