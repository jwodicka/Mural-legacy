using System;
using System.Collections.Generic;
using log4net;

namespace Mural
{
	public class InMemoryAccountStore : AccountStore
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(InMemoryAccountStore));

		public InMemoryAccountStore ()
		{
			_accountStore = new Dictionary<string, string>();
			_accountStore.Add("orbus", "password");
			_accountStore.Add("mufi", "anotherPassword");
		}
				
		public override Account GetAccount (string name, string password)
		{
			_log.Debug("In GetAccount!");
			
			if (_accountStore.ContainsKey(name) && _accountStore[name] == password)
			{
				Account account = new Account();
				account.Name = name;
				return account;
			}
			else
			{
				return null;	
			}
		}
		
		public override bool CreateAccount (Account account)
		{
			_log.Debug("In CreateAccount!");
			
			if(_accountStore.ContainsKey(account.Name))
			{
				return false;	
			}
			else
			{
				_accountStore.Add(account.Name, account.Password);
				return true;
			}
		}
		
		public override bool UpdateAccount (Account account, string password)
		{
			_log.Debug("In UpdateAccount!");
			
			if (account.Password == null)
			{
				throw new ArgumentException("Right now, this method can only be used to update password. Sorry.");	
			}
			
			if(_accountStore.ContainsKey(account.Name) && _accountStore[account.Name] == password)
			{
				_accountStore[account.Name] = account.Password;
				return true;
			}
			else
			{
				return false;	
			}
		} 
		
		Dictionary<string, string> _accountStore;
	}
}

