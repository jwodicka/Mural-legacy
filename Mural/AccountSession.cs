using System;

namespace Mural
{
	/// <summary>
	/// A session that is authenticated as belonging to a specified Account.
	/// </summary>
	public class AccountSession : SynchronousSession, ILineConsumer, IAccountAuthenticated
	{
		/// <summary>
		/// Construct a new <see cref="Mural.AccountSession"/> that authenticates as the given
		/// <see cref="Account"/>. 
		/// </summary>
		/// <param name='identity'>
		/// The <see cref="Account"/> that this session is authenticated as. 
		/// </param>
		public AccountSession (Account identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("AccountSession must have a non-null account.");	
			}
			_identity = identity;
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
		
		public override void HandleResponseEvent (object sender, ResponseEventArgs args)
		{
			OnRaiseResponseEvent(args);
		}
		
		/// <summary>
		/// The <see cref="Account"/> that this session is authenticated as.
		/// </summary>
		/// <value>
		/// The <see cref="Account"/> that this session is authenticated as.
		/// </value>
		public Account AccountIdentity 
		{
			get 
			{
				return _identity;
			}
		}

		// Just pass the event through.
		// TODO: Annotate the event with the account.
		public void HandleUserEvent (object sender, UserEventArgs args)
		{
			if (args.EventType == "Disconnect")
			{
				// Break the listening relationship with the disconnected sender.
				RemoveSource(sender as IResponseConsumer);	
			}
			
			// Reraise the event, whether or not it's a disconnection.
			OnRaiseUserEvent(args);
		}
		
		/// <summary>
		/// Event listener for LineReadyEvents.
		/// 
		/// Reraises the event as coming from this account.
		/// </summary>
		/// <param name='sender'>
		/// The sender of the event. Under normal usage, should correspond to the source that this
		/// object is aware of.
		/// </param>
		/// <param name='args'>
		/// The event arguments, containing the line that is ready.
		/// </param>
		public void HandleLineReadyEvent (object sender, LineReadyEventArgs args)
		{
			OnRaiseLineReadyEvent(args);
		}
		
		public void HandleDisconnectEvent (object sender, EventArgs args)
		{
			OnRaiseDisconnectEvent();
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
		
						
		/// <summary>
		/// Private backing variable for AccountIdentity.
		/// </summary>
		private Account _identity;
	}
}

