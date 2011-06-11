using System;

namespace Mural
{
	public abstract class WorldRouter : ILineConsumer
	{
		public abstract bool Connect();
		
		public abstract void HandleLineReadyEvent(object sender, LineReadyEventArgs args);
		public abstract void HandleDisconnectEvent(object sender, EventArgs args);
		
		public void AddSource (IResponseConsumer source) 
		{
			if (_source != null)
			{
				throw new Exception("A WorldRouter can only have one source.");	
			}
			else
			{
				SynchronousSession synchronousSource = source as SynchronousSession;
				if (synchronousSource == null)
				{
					throw new Exception("A WorldRouter must have a SynchronousSource as a source.");	
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
		
		protected SynchronousSession Source
		{
			get
			{
				return _source;	
			}
		}
		
		private SynchronousSession _source;
	}
}

