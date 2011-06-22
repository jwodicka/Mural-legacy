using System;

namespace Mural
{
	public class Account
	{
		public Account()
		{
		}
		
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		public string Password 
			// This will not be set when Account is returned; users should set it when they need to provide
			// a password during Create or Update.
		{
			get
			{
				return _password;
			}
			set
			{
				_password = value;	
			}
		}
		
		public bool CanAccessCharacter(string characterName, string worldName)
		{
			// This is a quick naive implementation. We probably want to actually persist the
			// ownership index longer.
			CharacterOwnershipIndex ownershipIndex = new CharacterOwnershipIndex();
			return ownershipIndex.DoesUserOwnCharacter(this.Name, characterName, worldName);
		}
		
		private string _name;
		private string _password;
	}
}

