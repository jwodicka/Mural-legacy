using System;
using Ninject;

namespace Mural
{
	public class AccountFactory
	{
		private ICharacterOwnership _index;
		
		[Inject]
		public AccountFactory (ICharacterOwnership index)
		{
			_index = index;
		}
		
		public Account GetAccount(string name)
		{
			return GetAccount(name, null);
		}
		
		public Account GetAccount(string name, string password)
		{
			return new Account(name, password, _index);
		}
	}
}

