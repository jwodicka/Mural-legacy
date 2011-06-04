using System;
namespace Mural
{
	public class Character
	{
		public Character (string name, string world)
		{
			_name = name;
			_world = world;
		}
		
		public override bool Equals (object obj)
		{
			Character other = obj as Character;
			return other != null &&
				Name == other.Name &&
				World == other.World;
		}
		
		public override int GetHashCode ()
		{
			// This should take advantage of the distribution of the default
			// string GetHashCode, and only be somewhat painful for efficiency.
			// It could be better, but it's good enough for now.
			return String.Format("{0}{1}", Name, World).GetHashCode();
		}
		
		public string Name
		{
			get
			{
				return _name;	
			}
		}
		public string World
		{
			get
			{
				return _world;	
			}
		}
		
		private string _name;
		private string _world;
	}
}

