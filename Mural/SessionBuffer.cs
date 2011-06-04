using System;
using System.Collections.Generic;

namespace Mural
{
	public class SessionBuffer : IResponseConsumer, ILineConsumer
	{
		public SessionBuffer ()
		{
		}
		
		public void AddSource (IResponseConsumer source) {}
		public void RemoveSource (IResponseConsumer source) {}
		
		public void HandleLineReadyEvent (object sender, LineReadyEventArgs args)
		{
			// For now, ignore lines sent by the user.
		}
		
		// Right now, we don't care if we get disconnected from. 
		// In the future, we might use this opportunity to clean up?
		public void HandleDisconnectEvent (object sender, EventArgs args) {}
		public void SendLineToUser (string line)
		{
			Buffer.Add(line);
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
		
		private List<string> _buffer;		
	}
}

