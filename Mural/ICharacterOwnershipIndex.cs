using System;

namespace Mural
{
	public interface ICharacterOwnershipIndex
	{
		bool DoesUserOwnCharacter(string userName, string characterName, string worldName);
	}
}

