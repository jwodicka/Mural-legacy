using System;
using System.Collections.Generic;
using log4net;

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
		
		public override bool CreateWorld (string name, string worldType, string arguments)
		{
			// Right now, this doesn't attempt to check if any of the types already have a world
			// by this name. Be aware of that, but it may not be worth fixing; this code is not
			// liable to be final-version since it doesn't save out its state.
			switch(worldType)
			{
			case "Mural.RemoteSSLWorld":
				SSLPassthroughIndex.Add(name, arguments);
				return true;
			default:
				_log.ErrorFormat("Couldn't construct a world of type {0}", worldType);
				return false;
			}
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
		
		private static readonly ILog _log = LogManager.GetLogger(typeof(HardcodedWorldList));
	}

}

