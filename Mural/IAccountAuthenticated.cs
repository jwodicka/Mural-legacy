using System;

namespace Mural
{
	/// <summary>
	/// Interface for objects that identify themselves as authenticated to a particular account.
	/// </summary>
	public interface IAccountAuthenticated
	{
		Account AccountIdentity 
		{
			get;
		}
	}
}

