using System;
using System.Collections.Generic;

namespace Mural
{
	public class CharacterSessionIndex
	{
		public CharacterSessionIndex ()
		{
		}
		
		public CharacterSession GetSessionForCharacter(string userName, string characterName, string worldName)
		{
			// First, look for a session - a session is unique by character and world, but not player.
			// (Why? Because it's possible for a character to be used by multiple players, even simultaneously.
			//  This isn't the default case, but we want to support it in the architecture.)
			string key = string.Format("{0}@{1}", characterName, worldName);
			if (Index.ContainsKey(key))
			{
				// This character session exists. We should return it if this user has rights to it.
				
				CharacterSession characterSession = Index[key];
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
				// Construct the character for this session so we can test for rights.
				Character character = new Character(characterName, worldName);
				
				if (character.CanBeAccessedByUser(userName))
				{
					// Construct a new CharacterSession
					CharacterSession characterSession = new CharacterSession(character);
					
					// Add it to the local index
					Index.Add(key, characterSession);
					
					// Connect it to the relevant world
					WorldRouter worldRouter = LocalWorldIndex.GetRouterForWorld(worldName);
					characterSession.ConnectLineConsumer(worldRouter);
					
					// Establish a SessionBuffer for it
					SessionBuffer buffer = new SessionBuffer();
					// Connect the buffer both downstream and upstream.
					characterSession.ConnectLineConsumer(buffer); // Technically, we never buffer lines going this way..
					characterSession.AddSource(buffer); // These are the ones we buffer.
					characterSession.Buffer = buffer; // Attach it to the object so we can find it later for recall.
					
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
		
		private Dictionary<string, CharacterSession> Index
		{
			get
			{
				if (_index == null)
				{
					_index = new Dictionary<string, CharacterSession>();	
				}
				return _index;
			}
		}
		
		private WorldIndex LocalWorldIndex
		{
			get
			{
				if (_worldIndex == null)
				{
					_worldIndex = new WorldIndex();
				}
				return _worldIndex;
			}
		}
		
		private WorldIndex _worldIndex;
		private Dictionary<string, CharacterSession> _index;
	}
}

