using System;
using Mono.Data;
using Mono.Data.Sqlite;
using log4net;

namespace Mural
{
	public class SQLiteAccountStore : IAccountStore
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(SQLiteAccountStore));

		public SQLiteAccountStore (string accountFilePath)
		{
			_log.Debug("Constructing SQLiteAccountStore");
			string connectionString = String.Format("Data Source={0},version=3", accountFilePath);
			_connection = new SqliteConnection(connectionString);
			_connection.Open();
			try
			{
				// Ensure that there is a table for users.
				string commandText = "create table if not exists Accounts (Name string, Password string)";
				using (SqliteCommand command = new SqliteCommand(commandText, _connection))
				{
					command.ExecuteNonQuery();	
				}
				
				commandText = "select count(*) from Accounts";
				using (SqliteCommand command = new SqliteCommand(commandText, _connection))
				{
					_log.DebugFormat("Initializing SQLiteAccountStore, {0} accounts known.",
						command.ExecuteScalar());
				}
			}
			finally
			{
				_connection.Close();
			}
		}
		
		public Account GetAccount (string name, string password, AccountFactory factory)
		{
			string commandText = "select Name from Accounts where Name=@name and Password=@password";
			
			_connection.Open();
			try
			{
				using (SqliteCommand command = new SqliteCommand(commandText, _connection))
				{
					command.Parameters.AddWithValue("@name", name);
					command.Parameters.AddWithValue("@password", password);
					using (SqliteDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							return factory.GetAccount(reader.GetString(0));
						}
						else
						{
							return null;	
						}
					}
				}
			}
			finally
			{
				_connection.Close();
			}
		}
		
		public bool CreateAccount (Account account)
		{
			_connection.Open();
			try 
			{
				string commandText =
					"select count(*) from Accounts where Name=@name";
				int numberOfAccounts;
				using (SqliteCommand command = new SqliteCommand(commandText, _connection))
				{
					command.Parameters.AddWithValue("@name", account.Name);
					string queryResult = command.ExecuteScalar().ToString();
					_log.DebugFormat("Result: {0}", queryResult);
					numberOfAccounts = Int32.Parse(queryResult);
				}
				
				if (numberOfAccounts > 0) 
				{
					// Cannot create an account that already exists!
					return false;	
				}
				else
				{
					commandText = "insert into Accounts (Name, Password) VALUES (@name, @password)";
					using (SqliteCommand command = new SqliteCommand(commandText, _connection))
					{
						command.Parameters.AddWithValue("@name", account.Name);
						command.Parameters.AddWithValue("@password", account.Password);
						command.ExecuteNonQuery();
						return true;
					}
				}
			}
			finally
			{
				_connection.Close();
			}
		}
		
		public bool UpdateAccount (Account account, string password)
		{
			_connection.Open();
			try
			{
				if (account.Password == null)
				{
					throw new ArgumentException("Right now, this method can only be used to update password. Sorry.");	
				}
				string commandText = "select count(*) from Accounts where Name=@name and Password=@password";
				int numberOfAccounts;
				using (SqliteCommand command = new SqliteCommand(commandText, _connection))
				{
					command.Parameters.AddWithValue("@name", account.Name);
					command.Parameters.AddWithValue("@password", account.Password);
					numberOfAccounts = (int) command.ExecuteScalar();	
				}
				
				if (numberOfAccounts < 1) 
				{
					// This name and password doesn't indicate an existing account
					return false;	
				}
				else
				{
					commandText = "update Accounts set Password=@newpassword WHERE Name=@name AND Password=@oldpassword";
					using (SqliteCommand command = new SqliteCommand(commandText, _connection))
					{
						command.Parameters.AddWithValue("@name", account.Name);
						command.Parameters.AddWithValue("@oldpassword", password);
						command.Parameters.AddWithValue("@newpassword", account.Password);
						command.ExecuteNonQuery();
						return true;
					}
				}
			}
			finally
			{
				_connection.Close();
			}
		} 
		
		private SqliteConnection _connection;
	}
}

