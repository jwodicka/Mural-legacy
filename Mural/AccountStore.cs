using System;
namespace Mural
{
	public abstract class AccountStore
	{
		public AccountStore()
		{
		}
		
		public abstract Account GetAccount(string name, string password, AccountFactory factory);
		
		public abstract bool CreateAccount(Account account);
		
		public abstract bool UpdateAccount(Account account, string password);
	}
}

