using System;
namespace Mural
{
	public abstract class SynchronousSession : IResponseConsumer
	{
		public abstract void SendLineToUser(string line);
		
		public abstract void Disconnect();
		
		public virtual void ConnectLineConsumer(ILineConsumer consumer)
		{
			this.RaiseLineReadyEvent += consumer.HandleLineReadyEvent;
			this.RaiseDisconnectEvent += consumer.HandleDisconnectEvent;
			
			consumer.AddSource(this);
		}
		
		public virtual void DisconnectLineConsumer(ILineConsumer consumer)
		{
			this.RaiseLineReadyEvent -= consumer.HandleLineReadyEvent;
			this.RaiseDisconnectEvent -= consumer.HandleDisconnectEvent;
			
			consumer.RemoveSource(this);
		}
		
		/// <summary>
		/// This event will be raised whenever there is a line available for processing.
		/// Subscribers to this event should return promptly; they will be called synchronously from a
		/// worker thread, and block additional parsing on this TelnetSession until they return.
		/// </summary>
		protected event EventHandler<LineReadyEventArgs> RaiseLineReadyEvent;
		
		protected void OnRaiseLineReadyEvent(LineReadyEventArgs args)
		{
			Console.WriteLine("LineReadyEvent raised!");
			// Make a temporary copy of the event to avoid the possibility of
			// a race condition if the last subscriber unsubscribes immediately
			// after the null check and before the event is raised.
			// This is modeled after: http://msdn.microsoft.com/en-us/library/w369ty8x.aspx
			// It might be nice to understand the details of what race condition this is preventing.
			EventHandler<LineReadyEventArgs> handler = RaiseLineReadyEvent;	
			
			// Event will be null if there are no subscribers
			if (handler != null)
			{
				handler(this, args);	
			}
		}
		
		protected event EventHandler RaiseDisconnectEvent;
		
		protected void OnRaiseDisconnectEvent()
		{
			Console.WriteLine("DisconnectEvent raised!");
			EventHandler handler = RaiseDisconnectEvent;
			
			if (handler != null)
			{
				handler(this, null); // No arguments are needed, so we pass null for the arguments.
			}
		}
	}
}

