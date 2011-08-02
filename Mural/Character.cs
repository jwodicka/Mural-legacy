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
		public Character (string name, string world)
		{
			_name = name;
			_world = world;
		}
		
		public override bool Equals (object obj)
		{
			Character other = obj as Character;
			return other != null &&
				Name.ToLower() == other.Name.ToLower() &&
				World.ToLower() == other.World.ToLower();
		}
		
		public override int GetHashCode ()
		{
			// This should take advantage of the distribution of the default
			// string GetHashCode, and only be somewhat painful for efficiency.
			// It could be better, but it's good enough for now.
			return String.Format("{0}{1}", Name.ToLower(), World.ToLower()).GetHashCode();
		}
		
		public bool CanBeAccessedByUser(string userName)
		{
			// This is a quick naive implementation. We probably want to actually persist the
			// ownership index longer.
			// TODO: Replace this with an IoC construct.
			CharacterOwnershipIndex ownershipIndex = new CharacterOwnershipIndex();
			return ownershipIndex.DoesUserOwnCharacter(userName, Name, World);	
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

