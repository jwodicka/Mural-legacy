using System;
namespace Mural
{
	/// <summary>
	/// A character is a specific, public identity on a specific world / service.
	/// 
	/// The combination of character name and world name uniquely identify a character; for Mural's
	/// purposes, no two characters on the same world may have the same name, but characters on
	/// two different worlds may have the same name.
	/// </summary>
	public class Character
	{
		// Used to determine ownership rights for this Character
		private ICharacterOwnershipIndex _index;
		
		public string Name { get; set; }
		public string World { get; set; }
		
		public Character (string name, string world, ICharacterOwnershipIndex index)
		{
			Name = name;
			World = world;
			_index = index;
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
		
		public bool CanBeAccessedByUser(string userName)
		{
			return _index.DoesUserOwnCharacter(userName, Name, World);
		}
	}
}

