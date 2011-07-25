using System;
using System.IO; // Referenced while we need to do path manipulation to get the DB location.

namespace Mural
{
	public class LoginParser : BasicLineConsumer
	{
		public LoginParser ()
		{
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
						
		protected AccountStore LocalAccountStore
		{
			get
			{
				if (_accountStore == null)
				{
					//_accountStore = new InMemoryAccountStore();
					string defaultAccountFile = Path.Combine("DefaultDB", "account.db");
					_accountStore = new SQLiteAccountStore(defaultAccountFile);
				}
				return _accountStore;
			}
		}
		
		protected CharacterSessionIndex LocalCharacterSessionIndex
		{
			get
			{
				if (_characterSessionIndex == null)
				{
					_characterSessionIndex = new CharacterSessionIndex();	
				}
				return _characterSessionIndex;
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
				Console.WriteLine("Attempting to connect");
				if (command.Length == 4)
				{
					string playerName = command[1].ToLower().Trim();
					string characterAndWorld = command[2].Trim();
					string password = command[3].Trim();
					
					Account account = LocalAccountStore.GetAccount(playerName, password);
					
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
									LocalCharacterSessionIndex.GetSessionForCharacter(playerName, character, world);
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
		
		private AccountStore _accountStore;
		private CharacterSessionIndex _characterSessionIndex;
	}
}

