using System;

namespace Mural
{
	public interface ICharacterOwnership
	{
		bool DoesUserOwnCharacter(string userName, string characterName, string worldName);
	}
}

