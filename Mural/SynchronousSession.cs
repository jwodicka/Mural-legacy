using System;
namespace Mural
{
	public abstract class SynchronousSession : IResponseConsumer
	{
		public abstract void HandleResponseEvent (object sender, ResponseEventArgs args);	
		
		/// <summary>
		/// A unique identifier that can be used to refer to this object when examining the
		/// stream of message handlers.
		/// </summary>
		public virtual string Identifier
		{
			get
			{
				if (_identifier == null)
				{
					_identifier = Guid.NewGuid().ToString();
				}
				return _identifier;
			}
		}
		
		/// <summary>
		/// This event will be raised whenever there is user event available for processing.
		/// Subscribers to this event should return promptly; they will be called synchronously from a
		/// worker thread, and block additional parsing on this TelnetSession until they return.
		/// </summary>
		public event EventHandler<UserEventArgs> RaiseUserEvent;
		
		protected void OnRaiseUserEvent(UserEventArgs args)
		{
			// Make a temporary copy of the event to avoid the possibility of
			// a race condition if the last subscriber unsubscribes immediately
			// after the null check and before the event is raised.
			// This is modeled after: http://msdn.microsoft.com/en-us/library/w369ty8x.aspx
			// TODO: It might be nice to understand the details of what race condition this is preventing.
			EventHandler<UserEventArgs> handler = RaiseUserEvent;	
			
			// Event will be null if there are no subscribers
			if (handler != null)
			{
				handler(this, args);	
			}	
		}
		
		protected void OnRaiseLineReadyEvent(LineReadyEventArgs args)
		{
			OnRaiseUserEvent(args);
		}
		
		protected void OnRaiseDisconnectEvent()
		{
			OnRaiseUserEvent(new DisconnectEventArgs(this.Identifier));
		}
		
		private string _identifier = null;
	}
}

