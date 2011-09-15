using System;
using Mono.Data;
using Mono.Data.Sqlite;
using log4net;

namespace Mural
{
	public class SQLiteWorldList : WorldList
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(SQLiteAccountStore));

		public SQLiteWorldList (string worldFilePath)
		{
			_log.Debug("Constructing SQLiteAccountStore");
			string connectionString = String.Format("Data Source={0},version=3", worldFilePath);
			_connection = new SqliteConnection(connectionString);
			_connection.Open();
			try
			{
				// Ensure that there is a table for worlds.
				string commandText = "create table if not exists Worlds (Name string, Type string, Arguments string)";
				using (SqliteCommand command = new SqliteCommand(commandText, _connection))
				{
					command.ExecuteNonQuery();	
				}
				
				commandText = "select count(*) from Worlds";
				using (SqliteCommand command = new SqliteCommand(commandText, _connection))
				{
					_log.DebugFormat("Initializing SQLiteWorldList, {0} worlds known.",
						command.ExecuteScalar());
				}
			}
			finally
			{
				_connection.Close();
			}
		}
		
		public override World GetWorldByName (string worldName)
		{
			string commandText = "select Name, Type, Arguments from Worlds where Name=@name";
			
			_connection.Open();
			try
			{
				using (SqliteCommand command = new SqliteCommand(commandText, _connection))
				{
					command.Parameters.AddWithValue("@name", worldName);
					using (SqliteDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							string name = reader.GetString(0);
							string type = reader.GetString(1);
							string arguments = reader.GetString(2);
							
							return ConstructWorld(name, type, arguments);
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
		
		public bool CreateWorld (string name, string type, string arguments)
		{
			_connection.Open();
			try 
			{
				string commandText = "select count(*) from Worlds where Name=@name";
				int numberOfWorlds;
				using (SqliteCommand command = new SqliteCommand(commandText, _connection))
				{
					command.Parameters.AddWithValue("@name", name);
					string queryResult = command.ExecuteScalar().ToString();
					_log.DebugFormat("Result: {0}", queryResult);
					numberOfWorlds = Int32.Parse(queryResult);
				}
				
				if (numberOfWorlds > 0) 
				{
					// Cannot create a world that already exists!
					return false;	
				}
				else
				{
					commandText = "insert into Worlds (Name, Type, Arguments) VALUES (@name, @type, @arguments)";
					using (SqliteCommand command = new SqliteCommand(commandText, _connection))
					{
						command.Parameters.AddWithValue("@name", name);
						command.Parameters.AddWithValue("@type", type);
						command.Parameters.AddWithValue("@arguments", arguments);
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
		
		private World ConstructWorld(string name, string worldType, string arguments)
		{
			// This would be neat if it did something that could locate the relevant
			// object to construct at runtime, by type. But for now, this method knows
			// too much about the options.
			
			switch (worldType)
			{
			case "Mural.RemoteSSLWorld":
				return new RemoteSSLWorld(name, arguments);
			default:
				_log.ErrorFormat("Couldn't construct a world of type {0}", worldType);
				throw new ArgumentException(String.Format("Unknown world type: {0}", worldType), "type");
			}			
		}
		
		private SqliteConnection _connection;
	}
}

