using System;
namespace Mural
{
	public class AccountSession : SynchronousSession, ILineConsumer, IAccountAuthenticated
	{
		public AccountSession (Account identity)
		{
			_identity = identity;
		}
		
		public Account AccountIdentity 
		{
			get 
			{
				return _identity;
			}
		}
		
		// Reraise the event as coming from this account.
		public void HandleLineReadyEvent (object sender, LineReadyEventArgs args)
		{
			OnRaiseLineReadyEvent(args);
		}
		
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
		public void RemoveSource (IResponseConsumer source)
		{
			if (_source == source)
			{
				_source = null;
			}
		}
		
		private Account _identity;
		private SynchronousSession _source;
	}
}

