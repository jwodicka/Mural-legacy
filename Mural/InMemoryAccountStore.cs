using System;
using System.Collections.Generic;
using log4net;
using Ninject;

namespace Mural
{
	public class InMemoryAccountStore : IAccountStore
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(InMemoryAccountStore));
		
		public InMemoryAccountStore ()
		{
			_accountStore = new Dictionary<string, string>();
			_accountStore.Add("orbus", "password");
			_accountStore.Add("mufi", "anotherPassword");
		}
				
		public Account GetAccount (string name, string password, AccountFactory factory)
		{
			_log.Debug("In GetAccount!");
			
			if (_accountStore.ContainsKey(name) && _accountStore[name] == password)
			{
				return factory.GetAccount(name);
			}
			else
			{
				return null;	
			}
		}
		
		public bool CreateAccount (Account account)
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
		
		public bool UpdateAccount (Account account, string password)
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

