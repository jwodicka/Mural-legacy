using System;
using log4net;

namespace Mural
{
	public class RedirectingParser : BasicLineConsumer
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(RedirectingParser));

		public RedirectingParser ()
		{
		}
		
		public override void HandleUserEvent(object sender, UserEventArgs args) 
		{
			switch (args.EventType)
			{
			case "LineReady":
				var lineArgs = args as LineReadyEventArgs;
				HandleLineReadyEvent(sender, lineArgs.Line);
				break;
			case "Disconnect":
				// Break the listening relationship with the disconnected sender.
				RemoveSource(sender as IResponseConsumer);
				break;
			default:
				throw new NotImplementedException(String.Format("Unsupported EventType: {0}", args.EventType));
			}
		}
		
		private void HandleLineReadyEvent(object sender, string line) 
		{
			_log.DebugFormat("Redirector parsing: {0}", line);
			
			string[] command = line.Split(' ');
			
			SynchronousSession session = sender as SynchronousSession;
			if (session == null)
			{
				_log.ErrorFormat("Got a LineReadyEvent from something that was not a SynchronousSession.");
			}
			else
			{
				AccountSession accountSession = session as AccountSession;
				Account account = null;
				if (accountSession != null)
				{
					account = accountSession.AccountIdentity;	
				}
				switch (command[0].ToLower().Trim())
				{
				case "echo":
					SendLineToUser("Switching to echo mode!");	
					RedirectToParser(session, ReusableEchoParser);
					break;
				case "login":
					if (account == null)
					{
						SendLineToUser("Switching to login mode!");	
						RedirectToParser(session, ReusableLoginParser);
					}
					else
					{
						SendLineToUser(String.Format("Already logged in as {0}!", account.Name));
					}
					break;
				case "connect":
				case "con":
					if (account == null)
					{
						SendLineToUser("Please log in before you try to make outbound connections.");	
					}
					else
					{
						int port;
						if (command.Length != 3 || !Int32.TryParse(command[2], out port))
						{
							SendLineToUser("Connect syntax:");
							SendLineToUser("  connect <hostname> <port>");
						}
						else
						{
							string hostname = command[1];
							SendLineToUser(String.Format("Connecting to {0}, port {1}", hostname, port));
							TelnetPassthrough passthroughParser = new TelnetPassthrough(hostname, port);
							if (passthroughParser.Connect())
							{
								SendLineToUser("Connected!");
								RedirectToParser(session, passthroughParser);
							}
							else
							{
								SendLineToUser("Connection failed.");	
							}
						}
					}
					break;
				case "quit":
					SendLineToUser("Goodbye!");
					SendGlobalDisconnectRequestToUser();
					break;
				default:
					SendLineToUser(String.Format("I don't have a parser named {0}!", command[0]));
					break;
				}
			}
		}
				
		private void RedirectToParser(SynchronousSession session, ILineConsumer parser)
		{
			// Add the new parser, then remove this session as a source
			parser.AddSource(session);
			this.RemoveSource(session);
		}
		
		// TODO: The construction here should be replaced by an IoC construct for better testability. Ninject, perhaps?
		EchoParser ReusableEchoParser
		{
			get
			{
				if (_echoParser == null)
				{
					_echoParser = new EchoParser();	
				}
				return _echoParser;
			}
		}
		LoginParser ReusableLoginParser
		{
			get
			{
				if (_loginParser == null)
				{
					_loginParser = new LoginParser();	
				}
				return _loginParser;
			}
		}
		
		private EchoParser _echoParser;
		private LoginParser _loginParser;
	}
}

