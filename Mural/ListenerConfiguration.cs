using System;

namespace Mural
{
	public class ListenerConfiguration
	{
		// TODO: Getter/Setter, validation.
		public String host;
		
		// TODO: Getter/Setter, validation.
		public int port;
		
		// TODO: Getter/Setter, validation.
		public String type;
			
		public ListenerConfiguration ()
		{
			
		}
		
		public ListenerConfiguration (String host, int port, String type)
		{
			this.host = host;
			this.port = port;
			this.type = type;
		}
	}
}

