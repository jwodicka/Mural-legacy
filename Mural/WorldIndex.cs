using System;
using System.Collections.Generic;
using Ninject;

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
		
		private Dictionary<string, World> _worlds;
		private WorldList _localWorldList;
		
		[Inject]
		public WorldIndex (WorldList worldList)
		{
			_localWorldList = worldList;
			_worlds = new Dictionary<string, World>();
		}
		
		public WorldRouter GetCharacterRouterForWorld(string characterName, string worldName)
		{
			if (!_worlds.ContainsKey(worldName))
			{
				World world = _localWorldList.GetWorldByName(worldName); 
				if (world != null)
				{
					_worlds.Add(worldName, world);
				}
				else
				{
					throw new ArgumentException();
				}
			}
			return _worlds[worldName].GetRouterForCharacter(characterName); 
			
			// At the moment, this assumes all worlds are telnet passthroughs.
			//ServerAddress worldAddress = GetServerAddressForWorld(worldName);
			//TelnetPassthrough passthroughSession = new TelnetPassthrough(worldAddress);
			//return passthroughSession;
		}
	}
}

