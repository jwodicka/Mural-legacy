using System;
using System.Collections.Generic;

namespace Mural
{
	// Needs to work no matter how many copies are instantiated. 
	public class HardcodedCharacterOwnershipIndex : ICharacterOwnershipIndex
	{
		public HardcodedCharacterOwnershipIndex ()
		{
			UserOwnership.Add("orbus", new List<string>());
			UserOwnership["orbus"].Add("orbus@furrymuck");
			
			UserOwnership.Add("mufi", new List<string>());
			UserOwnership["mufi"].Add("mufi@furrymuck");
		}
		
		public bool DoesUserOwnCharacter(string userName, string characterName, string worldName)
		{
			string characterIdentifier = String.Format("{0}@{1}", characterName, worldName);
			if (UserOwnership.ContainsKey(userName))
			{
				return (UserOwnership[userName] != null && 
					UserOwnership[userName].Contains(characterIdentifier));
			}
			else return false;
		}
		
		// This is an interim solution
		private Dictionary<string, List<string>> UserOwnership
		{
			get
			{
				if (_userOwnership == null)	
				{
					_userOwnership = new Dictionary<string, List<string>>();
				}
				return _userOwnership;
			}
		}
		
		// This is not an optimized storage solution by any means!
		private Dictionary<string, List<string>> _userOwnership;
	}
}

