using System;
using log4net;
using Ninject;

namespace Mural
{
	public class LoginParser : BasicLineConsumer
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(LoginParser));
		
		private IAccountStore _accountStore;
		private AccountFactory _accountFactory;
		private CharacterSessionIndex _characterSessionIndex;
		
		[Inject]
		public LoginParser (IAccountStore accountStore, AccountFactory accountFactory, CharacterSessionIndex characterSessionIndex)
		{
			_accountStore = accountStore;
			_accountFactory = accountFactory;
			_characterSessionIndex = characterSessionIndex;
		}
		
		public override void HandleUserEvent(object sender, UserEventArgs args) 
		{
			switch (args.EventType)
			{
			case "LineReady":
				var session = sender as SynchronousSession;
				var lineArgs = args as LineReadyEventArgs;
				ParseLine(session, lineArgs.Line);
				break;
			case "Disconnect":
				// Break the listening relationship with the disconnected sender.
				RemoveSource(sender as IResponseConsumer);
				break;
			default:
				throw new NotImplementedException(String.Format("Unsupported EventType: {0}", args.EventType));
			}
		}
						
		protected void ParseLine (SynchronousSession session, string line)
		{
			string[] command = line.Split(' ');
			switch(command[0].ToLower().Trim())
			{
			case "connect":
			case "connec":
			case "conne":
			case "conn":
			case "con":
			case "co":
			case "c":
				_log.Debug("Attempting to connect");
				if (command.Length == 4)
				{
					string playerName = command[1].ToLower().Trim();
					string characterAndWorld = command[2].Trim();
					string password = command[3].Trim();
					
					Account account = _accountStore.GetAccount(playerName, password, _accountFactory);
					
					// If we successfully authenticated the player:
					if (account != null)
					{
						int characterWorldSeparator = characterAndWorld.LastIndexOf('@');
						if (characterWorldSeparator < 0)
						{
							SendLineToUser("Character and world must be separated by an @.");	
						}
						else
						{
							string character = characterAndWorld.Substring(0, characterWorldSeparator);
							string world = characterAndWorld.Substring(characterWorldSeparator + 1);
							
							// Connect an AccountSession to the originating session
							AccountSession accountSession = new AccountSession(account);
							accountSession.AddSource(session);
							SendLineToUser(String.Format("Successfully logged in as {0}", account.Name));
							
							// Now try to find a session for this character.
							try 
							{
								CharacterSession characterSession =
									_characterSessionIndex.GetSessionForCharacter(playerName, character, world);
								// Connect this accountSession up to the characterSession
								characterSession.AddSource(accountSession);
							}
							catch (Exception e)
							{
								if (e.Message == "User does not have permission to access this character.")
								{
									SendLineToUser("That character doesn't exist, or you don't have permission for it.");
									SendGlobalDisconnectRequestToUser(); // Why disconnect? Because right now, we don't have a good
																  // parser scenario for "logged into account but not character."
								}
							}
							// Finally, disconnect this session from the parser. Wait until the last moment, so that 
							// we can reply to the user in case of errors.
							this.RemoveSource(session);
						}
					}
					else
					{
						SendLineToUser("Incorrect username or password.");	
					}
				}
				else
				{
					SendLineToUser("Try \"connect username character@world password\" to connect.");
				}
				break;
			case "quit":
			case "//quit":
				SendLineToUser("Goodbye!");
				SendGlobalDisconnectRequestToUser();
				break;
			default:
				SendLineToUser("I didn't understand that.");
				break;
			}
		}
	}
}

