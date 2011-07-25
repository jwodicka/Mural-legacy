using System;
using System.Collections.Generic;

namespace Mural
{
	public class MultiplexSession : SynchronousSession, ILineConsumer
	{
		public MultiplexSession ()
		{
		}
		
		public event EventHandler<ResponseEventArgs> RaiseResponseEvent;
		
		protected void OnRaiseResponseEvent(ResponseEventArgs args)
		{
			// Make a temporary copy of the event to avoid the possibility of
			// a race condition if the last subscriber unsubscribes immediately
			// after the null check and before the event is raised.
			// This is modeled after: http://msdn.microsoft.com/en-us/library/w369ty8x.aspx
			// TODO: It might be nice to understand the details of what race condition this is preventing.
			EventHandler<ResponseEventArgs> handler = RaiseResponseEvent;	
			
			// Event will be null if there are no subscribers
			if (handler != null)
			{
				handler(this, args);	
			}	
		}
		
		// Just reraise all events you get. The multiplexing will just work as a consequence of having multiple
		// upstream listeners.
		public override void HandleResponseEvent (object sender, ResponseEventArgs args)
		{
			OnRaiseResponseEvent(args);
		}
				
		public void HandleUserEvent (object sender, UserEventArgs args)
		{
			switch (args.EventType)
			{
			case "LineReady":
				OnRaiseLineReadyEvent(args as LineReadyEventArgs);
				break;
			case "Disconnect":
				// Break the listening relationship with the disconnected sender.
				RemoveSource(sender as IResponseConsumer);
				break;
			default:
				throw new NotImplementedException(String.Format("Unsupported EventType: {0}", args.EventType));
			}
		}
		
		public void AddSource (IResponseConsumer source)
		{
			this.RaiseResponseEvent += source.HandleResponseEvent;
			source.RaiseUserEvent += this.HandleUserEvent;
		}
		
		public void RemoveSource (IResponseConsumer source)
		{
			this.RaiseResponseEvent -= source.HandleResponseEvent;
			source.RaiseUserEvent -= this.HandleUserEvent;	
		}
	}
}

