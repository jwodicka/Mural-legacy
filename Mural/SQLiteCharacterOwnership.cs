using System;
using Mono.Data;
using Mono.Data.Sqlite;
using log4net;

namespace Mural
{
	public class SQLiteCharacterOwnership : ICharacterOwnership
	{
		public SQLiteCharacterOwnership (string characterFilePath)
		{
			_log.Debug("Constructing SQLiteCharacterOwnership");
			string connectionString = String.Format("Data Source={0},version=3", characterFilePath);
			_connection = new SqliteConnection(connectionString);
			_connection.Open();
			try
			{
				// Ensure that there is a table for characters.
				string commandText = "create table if not exists Characters (User string, Character string, World string)";
				using (SqliteCommand command = new SqliteCommand(commandText, _connection))
				{
					command.ExecuteNonQuery();	
				}
				
				commandText = "select count(*) from Characters";
				using (SqliteCommand command = new SqliteCommand(commandText, _connection))
				{
					_log.DebugFormat("Initializing SQLiteCharacterOwnership, {0} entries known.",
						command.ExecuteScalar());
				}
			}
			finally
			{
				_connection.Close();
			}
		}
		
		public bool DoesUserOwnCharacter (string userName, string characterName, string worldName)
		{
			string commandText = 
				"select 1 from Characters where User=@username and Character=@charactername and World=@worldname";
			
			_connection.Open();
			try
			{
				using (SqliteCommand command = new SqliteCommand(commandText, _connection))
				{
					command.Parameters.AddWithValue("@username", userName);
					command.Parameters.AddWithValue("@charactername", characterName);
					command.Parameters.AddWithValue("@worldname", worldName);

					using (SqliteDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							return true;
						}
						else
						{
							return false;	
						}
					}
				}
			}
			finally
			{
				_connection.Close();
			}
		}
		
		private SqliteConnection _connection;
		
		private static readonly ILog _log = LogManager.GetLogger(typeof(SQLiteCharacterOwnership));
	}
}

