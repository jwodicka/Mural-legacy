using System;

namespace Mural
{
	public class ListenerConfiguration
	{
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
			
		public ListenerConfiguration ()
		{
			
		}
		
		public ListenerConfiguration (String host, int port, String type)
		{
			this.Host = host;
			this.Port = port;
			this.Type = type;
		}
	}
}

