using System;

namespace Mural
{
	/// <summary>
	/// A persistent object maintained for each world the server is aware of.
	/// Subclasses of World are responsible for maintaining any state the world needs, and
	/// creating WorldRouters for each character who connects to the world.
	/// </summary>
	public abstract class World
	{
		public abstract string Name 
		{
			get;
		}
		
		public abstract WorldRouter GetRouterForCharacter(string characterName);
	}
}

