using System;
using System.Collections.Generic;

namespace Mural
{
	public abstract class WorldList
	{
		public abstract World GetWorldByName(string worldName);
	}
	
	public class HardcodedWorldList : WorldList
	{
		private Dictionary<string, string> _sslPassthroughIndex;
		
		public HardcodedWorldList()
		{
			_sslPassthroughIndex = new Dictionary<string, string>();
			_sslPassthroughIndex.Add("furrymuck", "muck.furry.com 8899");	
		}
		
		public override World GetWorldByName (string worldName)
		{
			if (_sslPassthroughIndex.ContainsKey(worldName))
			{
				return new RemoteSSLWorld(worldName, _sslPassthroughIndex[worldName]);
			}
			// No worlds found.
			return null;
		}
	}
}

