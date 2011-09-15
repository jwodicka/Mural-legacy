using System;
using System.Collections.Generic;
using log4net;

namespace Mural
{
	public class HardcodedSystemMessageProvider : ISystemMessageProvider
	{
		public HardcodedSystemMessageProvider ()
		{
			_messages.Add("login", new Dictionary<string, List<string>>());
			_messages["login"].Add("", new List<string>());
			_messages["login"][""].Add(@"____    _____ ___  ___ ______    _____  ___    ");
			_messages["login"][""].Add(@"|   \  /    | | |  | | |  __ \  /  _  \ | |    ");
			_messages["login"][""].Add(@"| |\ \/  /| | | |  | | | |__| | | |_| | | |    ");
			_messages["login"][""].Add(@"| | \   / | | | |  | | |  _  _| | ___ | | |    ");
			_messages["login"][""].Add(@"| |  \_/  | | \ \__/ / | | \ \  | | | | | |___ ");
			_messages["login"][""].Add(@"|_|       |_|  \____/  |_|  \_\ |_| |_| |_____|");
			_messages["login"][""].Add(@"");
			_messages["login"][""].Add(@"To connect, type: connect <user> <character>@<world> <password>");
			_messages["login"][""].Add(@"");
		}
		
		public List<string> GetMessage (string messageType, string endpointType)
		{
			if (!_messages.ContainsKey(messageType))
			{
				string error = String.Format("Unknown message type of {0}", messageType);
				_log.Error(error);
				var reply = new List<string>();
				reply.Add(String.Format("Error: {0}", error));
				return reply;
			}
			while (!_messages[messageType].ContainsKey(endpointType) &&
					endpointType.Contains("."))
			{
				endpointType = endpointType.Substring(0, endpointType.LastIndexOf('.'));
				_log.DebugFormat("EndpointType trimmed to: {0}", endpointType);
			}
			if(_messages[messageType].ContainsKey(endpointType))
			{
				return _messages[messageType][endpointType];	
			}
			else if (_messages[messageType].ContainsKey(""))
			{
				return _messages[messageType][""];	
			}
			else
			{
				string error = String.Format("Message type {0} has no default message.", messageType);
				_log.Error(error);
				var reply = new List<string>();
				reply.Add(String.Format("Error: {0}", error));
				return reply;
			}	
		}
		
		private Dictionary<string, Dictionary<string, List<string>>> _messages =
			new Dictionary<string, Dictionary<string, List<string>>>();
		
		private static readonly ILog _log = LogManager.GetLogger(typeof(TelnetSession));
	}
}

