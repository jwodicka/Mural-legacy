using System;

namespace Mural
{
	public class RemoteSSLWorld : World
	{	
		public RemoteSSLWorld (string name, string arguments)
		{
			_name = name;
			string[] args = arguments.Split(' ');
			if (args.Length != 2)
			{
				throw new ArgumentException();	
			}
			string remoteHostName = args[0];
			int remotePort = Int32.Parse(args[1]);
			
			_remoteServer = new ServerAddress(remoteHostName, remotePort);
		}
		
		public override string Name {
			get 
			{
				return _name;
			}
		}
		
		public override WorldRouter GetRouterForCharacter (string characterName)
		{
			return new TelnetPassthrough(_remoteServer);
		}
	
		private string _name;
		private ServerAddress _remoteServer;
	}
}

