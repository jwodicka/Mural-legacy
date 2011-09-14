using System;

namespace Mural
{
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

