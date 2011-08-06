using System;

namespace Mural
{
	/// ListenerConfiguration holds the data required to set up a single Listener of some type:
	///   - a hostname, a port, and a connection type (telnet, ssl, http, etc.)
	public class ListenerConfiguration
	{
		public ListenerConfiguration ()
		{
		}
		
		public ListenerConfiguration (String host, int port, String type)
		{
			this.Host = host;
			this.Port = port;
			this.Type = type;
		}
		public String Host
		{
			get;
			private set;
		}
		
		public int Port
		{
			get;
			private set;
		}
		
		public String Type
		{
			get;
			private set;
		}
	}			
}

