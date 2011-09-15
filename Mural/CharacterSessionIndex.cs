using System;
using System.Collections.Generic;
using Ninject;

namespace Mural
{
	public class CharacterSessionIndex
	{
		private CharacterFactory _characterFactory;
		private WorldIndex _localWorldIndex;
		private Dictionary<string, CharacterSession> _index;
		
		[Inject]
		public CharacterSessionIndex (WorldIndex worldIndex, CharacterFactory characterFactory)
		{
			_localWorldIndex = worldIndex;
			_characterFactory = characterFactory;
			_index = new Dictionary<string, CharacterSession>();
		}
		
		public CharacterSession GetSessionForCharacter(string userName, string characterName, string worldName)
		{
			// First, look for a session - a session is unique by character and world, but not player.
			// (Why? Because it's possible for a character to be used by multiple players, even simultaneously.
			//  This isn't the default case, but we want to support it in the architecture.)
			string key = string.Format("{0}@{1}", characterName, worldName);
			if (_index.ContainsKey(key))
			{
				// This character session exists. We should return it if this user has rights to it.
				
				CharacterSession characterSession = _index[key];
				Character character = characterSession.CharacterIdentity;
				
				if (character.CanBeAccessedByUser(userName))
				{
					// Return the existing CharacterSession object
					return characterSession;
				}
				else
				{
					// This is only semi-exceptional. The caller should probably have a test-method it can use
					// first to find out whether the exception is likely to occur.
					throw new Exception("User does not have permission to access this character.");	
				}
			}
			else
			{
				// Initialize the character for this session so we can test for rights.
				Character character = _characterFactory.GetCharacter(characterName, worldName);
				
				if (character.CanBeAccessedByUser(userName))
				{
					// Construct a new CharacterSession
					CharacterSession characterSession = new CharacterSession(character);
					
					// Add it to the local index
					_index.Add(key, characterSession);
					
					// Connect it to the relevant world
					WorldRouter worldRouter = _localWorldIndex.GetCharacterRouterForWorld(characterName, worldName);
					worldRouter.AddSource(characterSession);
					
					// Establish a SessionBuffer for it
					SessionBuffer buffer = new SessionBuffer();
					// Connect the buffer both downstream and upstream.
					worldRouter.AddSource(buffer); // We'll buffer the responses we get from the worldRouter
					buffer.AddSource(characterSession); // Technically, we never buffer lines going this way..
					
					characterSession.Buffer = buffer; // Attach it to the object so we can find it later for recall.
					worldRouter.Buffer = buffer;
					
					// Start the passthrough
					worldRouter.Connect();
					
					// Return it
					return characterSession;
				}
				else
				{
					// This is only semi-exceptional. The caller should probably have a test-method it can use
					// first to find out whether the exception is likely to occur.
					throw new Exception("User does not have permission to access this character.");	
				}
			}
		}
	}
}

