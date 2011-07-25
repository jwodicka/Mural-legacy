using System;

namespace Mural
{
	public abstract class BasicLineConsumer : ILineConsumer
	{
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
			
		public abstract void HandleUserEvent (object sender, UserEventArgs args);
		
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
		
		protected void SendLineToUser(string line)
		{
			var responseEvent = new ResponseLineEventArgs(line);
			OnRaiseResponseEvent(responseEvent);
		}
		
		protected void SendGlobalDisconnectRequestToUser()
		{
			var responseEvent = new RequestDisconnectEventArgs();
			OnRaiseResponseEvent(responseEvent);
		}	
		
		protected void SendDisconnectRequestByIdentifierToUser(string identifier)
		{
			var responseEvent = new RequestDisconnectEventArgs(RequestDisconnectEventArgs.RequestType.Only, identifier);
			OnRaiseResponseEvent(responseEvent);
		}
	}
}

