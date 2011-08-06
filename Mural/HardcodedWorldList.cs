using System;
using System.Collections.Generic;

namespace Mural
{
	public class HardcodedWorldList : WorldList
	{
		public HardcodedWorldList()
		{
			SSLPassthroughIndex.Add("furrymuck", "muck.furry.com 8899");	
		}
		
		public override World GetWorldByName (string worldName)
		{
			if (SSLPassthroughIndex.ContainsKey(worldName))
			{
				return new RemoteSSLWorld(worldName, SSLPassthroughIndex[worldName]);
			}
			// No worlds found.
			return null;
		}
		
		protected Dictionary<string, string> SSLPassthroughIndex
		{
			get
			{
				if (_sslPassthroughIndex == null)
				{
					_sslPassthroughIndex = new Dictionary<string, string>();	
				}
				return _sslPassthroughIndex;
			}
		}
		
		private Dictionary<string, string> _sslPassthroughIndex;
	}

}

