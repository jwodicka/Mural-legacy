using System;

namespace Mural
{
	public abstract class WorldList
	{
		public abstract World GetWorldByName(string worldName);
		
		public abstract bool CreateWorld(string name, string type, string arguments);
	}	
}

