using System;
using System.Collections.Generic;

namespace Mural
{
	public class SessionBuffer : IResponseConsumer, ILineConsumer
	{
		public SessionBuffer ()
		{
		}
		
		public event EventHandler<UserEventArgs> RaiseUserEvent;
		public event EventHandler<ResponseEventArgs> RaiseResponseEvent;
		
		public void HandleResponseEvent (object sender, ResponseEventArgs args)
		{
			if (args.EventType == "ResponseLine")
			{
				Buffer.Add((args as ResponseLineEventArgs).Line);	
			}
		}
		
		public void HandleUserEvent (object sender, UserEventArgs args)
		{
			switch (args.EventType)
			{
			case "LineReady":
				// For now, ignore lines sent by the user.
				break;
			case "Disconnect":
				// Break the listening relationship with the disconnected sender.
				// We remain connected to the world, and recieve ResponseEvents from it.
				RemoveSource(sender as IResponseConsumer);
				break;
			default:
				throw new NotImplementedException(String.Format("Unsupported EventType: {0}", args.EventType));
			}
		}
		
		// For now, you can read this buffer. Expect that will change; 
		//  users should probably get a read-only view of this stuff.
		public List<string> Buffer
		{
			get
			{
				if (_buffer == null)
				{
					_buffer = new List<string>();	
				}
				return _buffer;
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
		
		private List<string> _buffer;		
	}
}

