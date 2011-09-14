using System;
using Ninject;

namespace Mural
{
	public class CharacterFactory
	{
		private ICharacterOwnershipIndex _index;
		
		[Inject]
		public CharacterFactory (ICharacterOwnershipIndex index)
		{
			_index = index;
		}
		
		public Character GetCharacter(string characterName, string worldName)
		{
			return new Character(characterName, worldName, _index);
		}
	}
}

