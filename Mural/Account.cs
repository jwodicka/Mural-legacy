using System;


namespace Mural
{
	public class Account
	{
		private ICharacterOwnershipIndex _index;
		
		public string Name { get; set; }
		public string Password { get; set; }
		
		public Account(string name, string password, ICharacterOwnershipIndex index)
		{
			_index = index;
			Name = name;
			Password = password;
		}
		
		public bool CanAccessCharacter(string characterName, string worldName)
		{
			return _index.DoesUserOwnCharacter(Name, characterName, worldName);
		}
	}
}

