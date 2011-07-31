using System;
using System.Collections.Generic;

namespace Mural.Test
{
	public struct MockResponseConsumerEventRecord
	{
		public object Sender;
		public ResponseEventArgs Args;
	}
	
	public class MockResponseConsumer : IResponseConsumer
	{
		public readonly List<MockResponseConsumerEventRecord> HandledResponseEvents = new List<MockResponseConsumerEventRecord>();
		public readonly List<EventHandler<UserEventArgs>> UserEventHandlers = new List<EventHandler<UserEventArgs>>();
		
		// The ability to recieve messages from the service for the user
		public void HandleResponseEvent(object sender, ResponseEventArgs args)
		{
			HandledResponseEvents.Add(new MockResponseConsumerEventRecord {Sender = sender, Args = args});
		}
		
		// The ability to raise messages from the user to the service
		public event EventHandler<UserEventArgs> RaiseUserEvent
		{
			add
			{
				UserEventHandlers.Add((EventHandler<UserEventArgs>)value);
			}
			remove
			{
				UserEventHandlers.Remove((EventHandler<UserEventArgs>)value);
			}
		}
	}
}

