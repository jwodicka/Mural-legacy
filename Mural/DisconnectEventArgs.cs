using System;

namespace Mural
{
	public class DisconnectEventArgs : UserEventArgs
	{
		public DisconnectEventArgs(string originIdentifier)
			: base(originIdentifier)
		{
		}
		
		public override string EventType 
		{
			get
			{
				return "Disconnect";	
			}
		}
	}
}

