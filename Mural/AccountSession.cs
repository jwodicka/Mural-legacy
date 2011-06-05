using System;

namespace Mural
{
	/// <summary>
	/// A session that is authenticated as belonging to a specified Account.
	/// </summary>
	/// <exception cref='Exception'>
	/// Represents errors that occur during application execution.
	/// </exception>
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
			_identity = identity;
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
		
		/// <summary>
		/// Event listener for DiconnectEvents.
		/// 
		/// Reraises the event as coming from this account.
		/// </summary>
		/// <param name='sender'>
		/// The sender of the event. Under normal usage, should correspond to the source that this
		/// object is aware of.
		/// </param>
		/// <param name='args'>
		/// The event arguments, which cannot contain any data by default. Expected to be null.
		/// </param>
		public void HandleDisconnectEvent (object sender, EventArgs args)
		{
			OnRaiseDisconnectEvent();
		}
		
		public override void SendLineToUser (string line)
		{
			_source.SendLineToUser(line);
		}
		
		public override void Disconnect ()
		{
			_source.Disconnect();
		}
		
		/// <summary>
		/// Adds a SynchronousSession as a message origin for this session. Will fail if there is already 
		/// a source for this session. 
		/// </summary>
		/// <param name='source'>
		/// The upstream source of messages.
		/// 
		/// AccountSession will reject source if it is not a SynchronousSession.
		/// (Why? Because it needs the ability to disconnect the upstream session if it is disconnected.
		/// But maybe it can deal with only forwarding disconnect messages to the source if we can cast 
		/// source into a SynchronousSession? Or maybe AccountSession should come in Sync and Async 
		/// flavors?)
		/// </param>
		/// <exception cref='Exception'>
		/// Thrown if there is already a source connected, or if the source is not valid.
		/// </exception>
		public void AddSource (IResponseConsumer source) 
		{
			if (_source != null)
			{
				throw new Exception("AccountSession can only have one source.");	
			}
			else
			{
				SynchronousSession synchronousSource = source as SynchronousSession;
				if (synchronousSource == null)
				{
					throw new Exception("AccountSession must have a SynchronousSource as a source.");	
				}
				else
				{
					_source = synchronousSource;
				}
			}
		}
		
		/// <summary>
		/// Removes the message origin for this session. Will do nothing if the object handed to it is 
		/// not the source for this session.
		/// </summary>
		/// <param name='source'>
		/// The upstream source of messages. Will be removed if and only if it is currently the source 
		/// for this session.
		/// </param>
		public void RemoveSource (IResponseConsumer source)
		{
			if (_source == source)
			{
				_source = null;
			}
		}
		
		/// <summary>
		/// Private backing variable for AccountIdentity.
		/// </summary>
		private Account _identity;
		/// <summary>
		/// The message origin for this session. Manipulated by AddSource and RemoveSource, used to route
		/// messages upstream from this session.
		/// </summary>
		private SynchronousSession _source;
	}
}

