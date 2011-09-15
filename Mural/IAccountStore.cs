using System;
namespace Mural
{
	public interface IAccountStore
	{
		Account GetAccount(string name, string password, AccountFactory factory);
		bool CreateAccount(Account account);
		bool UpdateAccount(Account account, string password);
	}
}

