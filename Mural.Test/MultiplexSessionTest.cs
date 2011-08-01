using System;
using NUnit.Framework;
using Mural;
using Moq;

namespace Mural.Test
{
	[TestFixture]
	public class MultiplexSessionTest
	{
		protected MultiplexSession _session;
		
		[SetUp]
		public void SetUpTest()
		{
			_session = new MultiplexSession();
		}
		
		[TearDown]
		public void TearDownTest()
		{
			_session = null;
		}
		
		/// <summary>
		/// MultiplexSessions should forward lines they recieve.
		/// </summary>
		[Test]
		public void ForwardsLineFromSourceToSink()
		{
			var source = _session.WithMockSource();
			var sink = _session.WithMockSink();
			
			var lineEvent = new LineReadyEventArgs("Line", "originIdentifier", null);
			
			// Raise a lineEvent from source
			source.Raise(responseConsumer => responseConsumer.RaiseUserEvent += null, lineEvent);
			
			// Verify that the handler on sink was called exactly once.
			sink.Verify(lineConsumer => lineConsumer.HandleUserEvent(_session, lineEvent), Times.Once());
		}
		
		/// <summary>
		/// MultiplexSessions should not forward disconnects they recieve.
		/// </summary>
		[Test]
		public void DoesNotForwardDisconnectFromSourceToSink()
		{
			var source = _session.WithMockSource();
			var sink = _session.WithMockSink();
			
			var disconnectEvent = new DisconnectEventArgs("originIdentifier");
				
			// Raise a disconnectEvent from source
			source.Raise(responseConsumer => responseConsumer.RaiseUserEvent += null, disconnectEvent);
			
			// Verify that the handler on sink was never called.
			sink.Verify(lineConsumer => lineConsumer.HandleUserEvent(_session, disconnectEvent), Times.Never());
		}
		
		/// <summary>
		/// MultiplexSessions should break their relationship with sources that send them a disconnect.
		/// </summary>
		// TODO: This test is applicable to all ILineConsumers - a source that sends a disconnect must 
		//  be detached as a result.
		[Test]
		public void DetachesFromSourceAfterDisconnect()
		{
			var source = _session.WithMockSource();
			var sink = _session.WithMockSink();
			
			var lineEvent = new LineReadyEventArgs("Line", "originIdentifier", null);
			var disconnectEvent = new DisconnectEventArgs("originIdentifier");
				
			// Raise a lineEvent from source
			source.Raise(responseConsumer => responseConsumer.RaiseUserEvent += null, lineEvent);
	
			// Verify that the handler on sink was called exactly once.
			sink.Verify(lineConsumer => lineConsumer.HandleUserEvent(_session, lineEvent), Times.Once());
			
			// Raise a disconnectEvent from source
			source.Raise(responseConsumer => responseConsumer.RaiseUserEvent += null, disconnectEvent);
						
			// Raise a lineEvent from source
			source.Raise(responseConsumer => responseConsumer.RaiseUserEvent += null, lineEvent);
	
			// Verify that the handler on sink was called exactly once, and not a second time.
			sink.Verify(lineConsumer => lineConsumer.HandleUserEvent(_session, lineEvent), Times.Once());
		}
	}
}

