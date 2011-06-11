using System;
using System.Collections.Generic;

namespace Mural
{
	public class CharacterSessionIndex
	{
		public CharacterSessionIndex ()
		{
		}
		
		public CharacterSession GetSessionForCharacter(string name, string world)
		{
			string key = string.Format("{0}@{1}", name, world);
			if (Index.ContainsKey(key))
			{
				// Return the existing CharacterSession object
				return Index[key];
			}
			else
			{
				// Construct a new CharacterSession
				Character character = new Character(name, world);
				CharacterSession characterSession = new CharacterSession(character);
				
				// Add it to the local index
				Index.Add(key, characterSession);
				
				// Connect it to the relevant world
				WorldRouter worldRouter = LocalWorldIndex.GetRouterForWorld(world);
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

