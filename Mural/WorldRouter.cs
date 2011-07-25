using System;

namespace Mural
{
	public abstract class WorldRouter : BasicLineConsumer
	{
		public abstract bool Connect();
		
		public SessionBuffer Buffer { get; set; }
	}
}

