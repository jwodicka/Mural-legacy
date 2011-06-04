using System;
namespace Mural
{
	public class RedirectingParser : ILineConsumer
	{
		public RedirectingParser ()
		{
		}
		
		public void HandleLineReadyEvent(object sender, LineReadyEventArgs args) 
		{
			string line = args.Line;
			Console.WriteLine("Redirector parsing: {0}", line);
			
			string[] command = line.Split(' ');
			
			SynchronousSession session = sender as SynchronousSession;
			if (session == null)
			{
				Console.Error.WriteLine("Got a LineReadyEvent from something that was not a SynchronousSession.");
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
					session.SendLineToUser("Switching to echo mode!");	
					RedirectToParser(session, ReusableEchoParser);
					break;
				case "login":
					if (account == null)
					{
						session.SendLineToUser("Switching to login mode!");	
						RedirectToParser(session, ReusableLoginParser);
					}
					else
					{
						session.SendLineToUser(String.Format("Already logged in as {0}!", account.Name));
					}
					break;
				case "connect":
				case "con":
					if (account == null)
					{
						session.SendLineToUser("Please log in before you try to make outbound connections.");	
					}
					else
					{
						int port;
						if (command.Length != 3 || !Int32.TryParse(command[2], out port))
						{
							session.SendLineToUser("Connect syntax:");
							session.SendLineToUser("  connect <hostname> <port>");
						}
						else
						{
							string hostname = command[1];
							session.SendLineToUser(String.Format("Connecting to {0}, port {1}", hostname, port));
							TelnetPassthrough passthroughParser = new TelnetPassthrough(hostname, port);
							if (passthroughParser.Connect())
							{
								session.SendLineToUser("Connected!");
								RedirectToParser(session, passthroughParser);
							}
							else
							{
								session.SendLineToUser("Connection failed.");	
							}
						}
					}
					break;
				case "quit":
					session.SendLineToUser("Goodbye!");
					session.Disconnect();
					break;
				default:
					session.SendLineToUser(String.Format("I don't have a parser named {0}!", command[0]));
					break;
				}
			}
		}
		
		// The redirecting parser does nothing when the user disconnects.
		public void HandleDisconnectEvent(object sender, EventArgs args)
		{
		}
		
		// Doesn't care about its sources
		public void AddSource (IResponseConsumer source) {}
		public void RemoveSource (IResponseConsumer source) {}
		
		private void RedirectToParser(SynchronousSession session, ILineConsumer parser)
		{
			// Add the new parser, then remove this as a line consumer.
			session.ConnectLineConsumer(parser);
			session.DisconnectLineConsumer(this);
		}
		
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

