using System;

namespace Mural
{
	/// <summary>
	/// Interface for objects that identify themselves as authenticated to a particular character.
	/// </summary>
	public interface ICharacterAuthenticated
	{
		Character CharacterIdentity
		{
			get;
		}
	}
}

