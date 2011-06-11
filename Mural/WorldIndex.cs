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
			Index.Add("furrymuck", new ServerAddress("muck.furry.com", 8899));
		}
		
		public WorldRouter GetRouterForWorld(string worldName)
		{
			// At the moment, this assumes all worlds are telnet passthroughs.
			ServerAddress worldAddress = GetServerAddressForWorld(worldName);
			TelnetPassthrough passthroughSession = new TelnetPassthrough(worldAddress);
			return passthroughSession;
		}
		
		// At the moment, this thing doesn't know what to do with non-external worlds.
		// This should probably not be the permanent way this communicates.
		protected ServerAddress GetServerAddressForWorld (string worldName)
		{
			return Index[worldName];
		}
		
		protected Dictionary<string, ServerAddress> Index
		{
			get
			{
				if (_index == null)
				{
					_index = new Dictionary<string, ServerAddress>();	
				}
				return _index;
			}
		}
		
		private Dictionary<string, ServerAddress> _index;
	}
	
	public class ServerAddress
	{
		public ServerAddress(string hostname, int port)
		{
			Hostname = hostname;
			Port = port;
		}
		public string Hostname;
		public int Port;
	}
}

