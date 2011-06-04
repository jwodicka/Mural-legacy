using System;
namespace Mural
{
	public class EchoParser : ILineConsumer
	{
		public EchoParser ()
		{
		}
		
		public void HandleLineReadyEvent(object sender, LineReadyEventArgs args) 
		{
			string line = args.Line.Trim();
			Console.WriteLine("Echoing: {0}", line);
			
			SynchronousSession session = sender as SynchronousSession;
			if (session == null)
			{
				Console.Error.WriteLine("Got a LineReadyEvent from something that was not a SynchronousSession.");
			}
			else
			{
				if (line.ToLower() == "quit")
				{
					session.SendLineToUser("Goodbye! (Goodbye!)");
					session.DisconnectLineConsumer(this);
					session.Disconnect();
				}
				
				// Send this message back to the originating session.
				session.SendLineToUser(line);
			}
		}
		
		// The echo parser does nothing when the user disconnects.
		public void HandleDisconnectEvent(object sender, EventArgs args)
		{
		}
		
		// Doesn't care about its sources
		public void AddSource (IResponseConsumer source) {}
		public void RemoveSource (IResponseConsumer source) {}
	}
}

