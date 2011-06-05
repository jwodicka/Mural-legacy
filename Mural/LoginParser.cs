using System;
using System.IO; // Referenced while we need to do path manipulation to get the DB location.

namespace Mural
{
	public class LoginParser : ILineConsumer
	{
		public LoginParser ()
		{
		}
		
		public void HandleLineReadyEvent(object sender, LineReadyEventArgs args) 
		{
			SynchronousSession session = sender as SynchronousSession;
			ParseLine(session, args.Line);
		}
		
		// Login parser doesn't care if you disconnect.
		public void HandleDisconnectEvent(object sender, EventArgs args)
		{
		}
		
		// Doesn't care about its sources.
		public void AddSource (IResponseConsumer source) {}
		public void RemoveSource (IResponseConsumer source) {}
		
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
							session.SendLineToUser("Character and world must be separated by an @.");	
						}
						else
						{
							string character = characterAndWorld.Substring(0, characterWorldSeparator);
							string world = characterAndWorld.Substring(characterWorldSeparator + 1);
							
							// Connect an AccountSession to the originating session
							AccountSession accountSession = new AccountSession(account);
							session.DisconnectLineConsumer(this);
							session.ConnectLineConsumer(accountSession);
							session.SendLineToUser(String.Format("Successfully logged in as {0}", account.Name));
							
							// Now try find a session for this character.
							CharacterSession characterSession =
								LocalCharacterSessionIndex.GetSessionForCharacter(character, world);
							
							// Connect this accountSession up to the characterSession
							accountSession.ConnectLineConsumer(characterSession);

						}
					}
					else
					{
						session.SendLineToUser("Incorrect username or password.");	
					}
				}
				else
				{
					session.SendLineToUser("Try \"connect username character@world password\" to connect.");
				}
				break;
			case "quit":
				session.SendLineToUser("Goodbye!");
				session.Disconnect();
				break;
			default:
				session.SendLineToUser("I didn't understand that.");
				break;
			}
		}
		
		private AccountStore _accountStore;
		private CharacterSessionIndex _characterSessionIndex;
	}
}

