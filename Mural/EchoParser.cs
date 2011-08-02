using System;
using log4net;

namespace Mural
{
	public class EchoParser : BasicLineConsumer
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(EchoParser));

		public EchoParser ()
		{
		}
		
		public override void HandleUserEvent(object sender, UserEventArgs args) 
		{
			switch (args.EventType)
			{
			case "LineReady":
				var lineArgs = args as LineReadyEventArgs;
				string line = lineArgs.Line.Trim();
				HandleLineReadyEvent(sender, line);
				break;
			case "Disconnect":
				// Break the listening relationship with the disconnected sender.
				RemoveSource(sender as IResponseConsumer);
				break;
			default:
				throw new NotImplementedException(String.Format("Unsupported EventType: {0}", args.EventType));
			}
		}
		
		private void HandleLineReadyEvent(object sender, string line) 
		{
			_log.DebugFormat("Echoing: {0}", line);
			
			var session = sender as IResponseConsumer;
			if (session == null)
			{
				_log.ErrorFormat("Got a LineReadyEvent from something that was not an IResponseConsumer.");
			}
			else
			{
				if (line.ToLower() == "quit")
				{
					SendLineToUser("Goodbye! (Goodbye!)");
					SendGlobalDisconnectRequestToUser();
					
					// TODO: I think this will be pushed back up the chain as a consequence of the above request, and 
					// there's no need to remove this explicitly. But I'm not certain, and it should be safe to call 
					// RemoveSource an extra time. 
					RemoveSource(session);
				}
				else
				{
					// Send this message back to the originating session.
					SendLineToUser(line);
				}
			}
		}		
	}
}

