using System;
using System.Collections.Generic;

namespace Mural
{
	public class MultiplexSession : SynchronousSession, ILineConsumer
	{
		public MultiplexSession ()
		{
		}
		
		public override void Disconnect ()
		{
			Console.WriteLine("Multiplex session disconnecting.");
			while (Sessions.Count > 0)
			{
				SynchronousSession synchronousSession = Sessions[0] as SynchronousSession;
				if (synchronousSession != null)
				{
					synchronousSession.Disconnect();
				}
				Sessions.Remove(Sessions[0]);
			}
		}
		public override void SendLineToUser (string line)
		{
			foreach (IResponseConsumer session in Sessions)
			{
				session.SendLineToUser(line);	
			}
		}
		
		public void HandleDisconnectEvent (object sender, EventArgs args)
		{
			SynchronousSession senderSession = sender as SynchronousSession;
			Sessions.Remove(senderSession);
		}
		public void HandleLineReadyEvent (object sender, LineReadyEventArgs args)
		{
			OnRaiseLineReadyEvent(args);
		}
		
		public void AddSource (IResponseConsumer source)
		{
			Sessions.Add(source);
		}
		public void RemoveSource (IResponseConsumer source)
		{
			Sessions.Remove(source);
		}
		
		private List<IResponseConsumer> Sessions
		{
			get
			{
				if (_sessions == null)
				{
					_sessions = new List<IResponseConsumer>();	
				}
				return _sessions;
			}
		}
		
		private List<IResponseConsumer> _sessions;
	}
}

