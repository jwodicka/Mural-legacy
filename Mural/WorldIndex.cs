using System;
using System.Collections.Generic;

namespace Mural
{
	/// <summary>
	/// Given the name of a world, returns an object that routes lines to that world.
	/// </summary>
	public class WorldIndex
	{
		// At the moment, this is more of an index of connection-data, but it's probably
		// more appropriate as an ILineConsumer factory for line-routers that will connect
		// to the appropriate world.
		
		public WorldIndex ()
		{
			
		}
		
		public WorldRouter GetCharacterRouterForWorld(string characterName, string worldName)
		{
			if (!Worlds.ContainsKey(worldName))
			{
				World world = LocalWorldList.GetWorldByName(worldName); 
				if (world != null)
				{
					Worlds.Add(worldName, world);
				}
				else
				{
					throw new ArgumentException();
				}
			}
			return Worlds[worldName].GetRouterForCharacter(characterName); 
			
			// At the moment, this assumes all worlds are telnet passthroughs.
			//ServerAddress worldAddress = GetServerAddressForWorld(worldName);
			//TelnetPassthrough passthroughSession = new TelnetPassthrough(worldAddress);
			//return passthroughSession;
		}
		
		protected Dictionary<string, World> Worlds
		{
			get 
			{
				if (_worlds == null)
				{
					_worlds = new Dictionary<string, World>();	
				}
				return _worlds;
			}
		}
		
		protected WorldList LocalWorldList
		{
			get
			{
				if (_worldList == null)
				{
					_worldList = new HardcodedWorldList();	
				}
				return _worldList;
			}
		}
		
		private Dictionary<string, World> _worlds;
		private WorldList _worldList;
	}
}

