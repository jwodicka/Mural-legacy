using System;
using NUnit.Framework;
using Mural;
using Moq;

namespace Mural.Test
{
	[TestFixture]
	public class MultiplexSessionTest
	{
		[Test]
		public void ForwardsLineFromSourceToSink()
		{
			var multiplexSession = new MultiplexSession();
			var source = new Mock<IResponseConsumer>();
			var sink = new Mock<ILineConsumer>();
			
			var lineEvent = new LineReadyEventArgs("Line", "originIdentifier", null);
			
			multiplexSession.AddSource(source.Object);
			ConnectSourceToSink(multiplexSession, sink.Object);
			
			// Raise a lineEvent from source
			source.Raise(responseConsumer => responseConsumer.RaiseUserEvent += null, lineEvent);
			
			// Verify that the handler on sink was called exactly once.
			sink.Verify(lineConsumer => lineConsumer.HandleUserEvent(multiplexSession, lineEvent), Times.Once());
		}
		
		[Test]
		/// <summary>
		/// MultiplexSessions should not forward disconnects they recieve.
		/// </summary>
		public void DoesNotForwardDisconnectFromSourceToSink()
		{
			var multiplexSession = new MultiplexSession();
			var source = new Mock<IResponseConsumer>();
			var sink = new Mock<ILineConsumer>();
			
			var disconnectEvent = new DisconnectEventArgs("originIdentifier");
				
			multiplexSession.AddSource(source.Object);
			ConnectSourceToSink(multiplexSession, sink.Object);
					
			// Raise a disconnectEvent from source
			source.Raise(responseConsumer => responseConsumer.RaiseUserEvent += null, disconnectEvent);
			
			// Verify that the handler on sink was never called.
			sink.Verify(lineConsumer => lineConsumer.HandleUserEvent(multiplexSession, disconnectEvent), Times.Never());
		}
		
		[Test]
		/// <summary>
		/// MultiplexSessions should break their relationship with sources that send them a disconnect.
		/// </summary>
		// TODO: This test is applicable to all ILineConsumers - a source that sends a disconnect must 
		//  be detached as a result.
		public void DetachesFromSourceAfterDisconnect()
		{
			var multiplexSession = new MultiplexSession();
			var source = new Mock<IResponseConsumer>();
			var sink = new Mock<ILineConsumer>();
			
			var lineEvent = new LineReadyEventArgs("Line", "originIdentifier", null);
			var disconnectEvent = new DisconnectEventArgs("originIdentifier");
				
			multiplexSession.AddSource(source.Object);
			ConnectSourceToSink(multiplexSession, sink.Object);
			
					
			// Raise a lineEvent from source
			source.Raise(responseConsumer => responseConsumer.RaiseUserEvent += null, lineEvent);
	
			// Verify that the handler on sink was called exactly once.
			sink.Verify(lineConsumer => lineConsumer.HandleUserEvent(multiplexSession, lineEvent), Times.Once());
			
			// Raise a disconnectEvent from source
			source.Raise(responseConsumer => responseConsumer.RaiseUserEvent += null, disconnectEvent);
						
			// Raise a lineEvent from source
			source.Raise(responseConsumer => responseConsumer.RaiseUserEvent += null, lineEvent);
	
			// Verify that the handler on sink was called exactly once, and not a second time.
			sink.Verify(lineConsumer => lineConsumer.HandleUserEvent(multiplexSession, lineEvent), Times.Once());
		}
		
		private static void ConnectSourceToSink(IResponseConsumer source, ILineConsumer sink) {
			source.RaiseUserEvent += sink.HandleUserEvent;
			sink.RaiseResponseEvent += source.HandleResponseEvent;
		}
	}
}

