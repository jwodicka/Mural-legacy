using System;
using Mono.Data;
using Mono.Data.Sqlite;

namespace Mural
{
	public class SQLiteAccountStore : AccountStore
	{
		public SQLiteAccountStore (string accountFilePath)
		{
			Console.WriteLine("Constructing SQLiteAccountStore");
			string connectionString = String.Format("Data Source=file:{0},version=3", accountFilePath);
			_connection = new SqliteConnection(connectionString);
			
			// Ensure that there is a table for users.
			string commandText = "create table if not exists Accounts (Name string, Password string)";
			using (SqliteCommand command = new SqliteCommand(commandText, _connection))
			{
				command.ExecuteNonQuery();	
			}
			
			commandText = "select count(*) from Accounts";
			using (SqliteCommand command = new SqliteCommand(commandText, _connection))
			{
				Console.WriteLine(String.Format("Initializing SQLiteAccountStore, {0} accounts known.",
					command.ExecuteScalar()));
			}
		}
		
		public override Account GetAccount (string name, string password)
		{
			Console.WriteLine("In GetAccount!");
			// Holy SQL(ite) injection attack, Batman! This code must not outlive the demo.
			string commandText = string.Format(
            	"select Name from Accounts where Name = {0} and Password = {1}",
               	name, password);
			using (SqliteCommand command = new SqliteCommand(commandText, _connection))
			{
				using (SqliteDataReader reader = command.ExecuteReader())
				{
					if (reader.Read())
					{
						Account account = new Account();
						account.Name = reader.GetString(0);
						return account;
					}
					else
					{
						return null;	
					}
				}
			}
		}
		
		public override bool CreateAccount (Account account)
		{
			string commandText = string.Format(
				"select count(*) from Accounts where Name = {0}",
				account.Name);
			int numberOfAccounts;
			using (SqliteCommand command = new SqliteCommand(commandText, _connection))
			{
				numberOfAccounts = (int) command.ExecuteScalar();	
			}
			
			if (numberOfAccounts > 0) 
			{
				// Cannot create an account that already exists!
				return false;	
			}
			else
			{
				commandText = string.Format(
					"insert into Accounts (Name, Password) VALUES ({0}, {1})",                       
					account.Name, account.Password);
				using (SqliteCommand command = new SqliteCommand(commandText, _connection))
				{
					command.ExecuteNonQuery();
					return true;
				}
			}
		}
		
		public override bool UpdateAccount (Account account, string password)
		{
			if (account.Password == null)
			{
				throw new ArgumentException("Right now, this method can only be used to update password. Sorry.");	
			}
			string commandText = string.Format(
            	"select count(*) from Accounts where Name = {0} and Password = {1}",
               	account.Name, password);
			int numberOfAccounts;
			using (SqliteCommand command = new SqliteCommand(commandText, _connection))
			{
				numberOfAccounts = (int) command.ExecuteScalar();	
			}
			
			if (numberOfAccounts < 1) 
			{
				// This name and password doesn't indicate an existing account
				return false;	
			}
			else
			{
				commandText = string.Format(
					"update Accounts set Password = {1} WHERE Name = {0} AND Password = {2})",                       
					account.Name, account.Password, password);
				using (SqliteCommand command = new SqliteCommand(commandText, _connection))
				{
					command.ExecuteNonQuery();
					return true;
				}
			}
		} 
		
		private SqliteConnection _connection;
	}
}

