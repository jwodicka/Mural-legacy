using System;
using NUnit.Framework;
using Mural;
using Moq;

namespace Mural.Test
{
	[TestFixture]
	public class AccountSessionTest
	{
		Account _account;
		AccountSession _session;
		
		[SetUp]
		public void SetUpTestObjects()
		{
			_account = new Account();
			_session = new AccountSession(_account);
		}
		
		[TearDown]
		public void TearDownTestObjects()
		{
			_account = null;
			_session = null;
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void CannotBeConstructedWithoutAccount ()
		{
			new AccountSession(null);	
		}
		
		[Test]
		public void ReturnsAccountIdentity ()
		{
			AccountSession session = new AccountSession(_account);
			Assert.AreEqual(_account, session.AccountIdentity);
		}

		protected Mock<IResponseConsumer> HookUpMockSource()
		{
			Mock<IResponseConsumer> source = new Mock<IResponseConsumer>();
			_session.AddSource(source.Object);
			return source;
		}
		
		protected Mock<ILineConsumer> HookUpMockSink()
		{
			Mock<ILineConsumer> sink = new Mock<ILineConsumer>();
			_session.RaiseUserEvent += sink.Object.HandleUserEvent;
			sink.Object.RaiseResponseEvent += _session.HandleResponseEvent;
			return sink;
		}
		
		[Test]
		public void CanAddSource ()
		{
			Mock<IResponseConsumer> source = new Mock<IResponseConsumer>();
			_session.AddSource(source.Object);
			
			Mock<ILineConsumer> sink = HookUpMockSink();
			
			// Raise an event from the sink and verify it's passed to the source with
			// the event source rewritten to the AccountSession.
			ResponseEventArgs sinkArgs = new ResponseLineEventArgs("A line.");
			sink.Raise(lc => lc.RaiseResponseEvent += null, sinkArgs);
			source.Verify(rc => rc.HandleResponseEvent(_session, sinkArgs), Times.Once());
			
			// Raise an event from the source and verify it's passed to the sink with
			// the event source rewritten to the AccountSession.
			UserEventArgs sourceArgs = new LineReadyEventArgs("Another line.", "An origin.", null);
			source.Raise(rc => rc.RaiseUserEvent += null, sourceArgs);
			sink.Verify(lc => lc.HandleUserEvent(_session, sourceArgs), Times.Once());
		}
		
		[Test]
		public void CanRemoveSource ()
		{
			Mock<IResponseConsumer> source = HookUpMockSource();
			Mock<ILineConsumer> sink = HookUpMockSink();
			
			// Remove the source after having added it.
			_session.RemoveSource(source.Object);
			
			// Raise an event from the sink and verify the source is never notified.
			sink.Raise(lc => lc.RaiseResponseEvent += null, new ResponseLineEventArgs("A line."));
			source.Verify(rc => rc.HandleResponseEvent(It.IsAny<object>(), It.IsAny<ResponseEventArgs>()), Times.Never());
			
			// Raise an event from the source and verify the sink is never notified.
			source.Raise(rc => rc.RaiseUserEvent += null, new LineReadyEventArgs("Another line.", "An origin.", null));
			sink.Verify(lc => lc.HandleUserEvent(It.IsAny<object>(), It.IsAny<UserEventArgs>()), Times.Never());
		}
		
		[Test]
		public void WillPassThroughResponseLineEvent()
		{
			Mock<IResponseConsumer> source = HookUpMockSource();
			Mock<ILineConsumer> sink = HookUpMockSink();
			
			ResponseLineEventArgs args = new ResponseLineEventArgs("A line.");
			sink.Raise(lc => lc.RaiseResponseEvent += null, args);
			source.Verify(rc => rc.HandleResponseEvent(_session, args), Times.Once());
		}
		
		[Test]
		public void WillPassThroughRequestDisconnectEvent()
		{
			Mock<IResponseConsumer> source = HookUpMockSource();
			Mock<ILineConsumer> sink = HookUpMockSink();
			
			RequestDisconnectEventArgs args = new RequestDisconnectEventArgs();
			sink.Raise(lc => lc.RaiseResponseEvent += null, args);
			source.Verify(rc => rc.HandleResponseEvent(_session, args), Times.Once());
		}
		
		[Test]
		public void WillPassThroughLineReadyEvent()
		{
			Mock<IResponseConsumer> source = HookUpMockSource();
			Mock<ILineConsumer> sink = HookUpMockSink();
			
			LineReadyEventArgs args = new LineReadyEventArgs("A line.", "An origin.", null);
			source.Raise(rc => rc.RaiseUserEvent += null, args);
			sink.Verify(lc => lc.HandleUserEvent(_session, args), Times.Once());
		}
		
		[Test]
		public void WillPassThroughDisconnectEventAndRemoveSource()
		{
			Mock<IResponseConsumer> source = HookUpMockSource();
			Mock<ILineConsumer> sink = HookUpMockSink();
			
			DisconnectEventArgs args = new DisconnectEventArgs("An origin.");
			source.Raise(rc => rc.RaiseUserEvent += null, args);
			sink.Verify(lc => lc.HandleUserEvent(_session, args), Times.Once());
			
			// Try to send events either way to verify we disconnected the source.
			source.Raise(rc => rc.RaiseUserEvent += null, new LineReadyEventArgs("A line.", "An origin.", null));
			sink.Verify(lc => lc.HandleUserEvent(It.IsAny<object>(), It.IsAny<LineReadyEventArgs>()), Times.Never());
			sink.Raise(lc => lc.RaiseResponseEvent += null, new ResponseLineEventArgs("A line."));
			source.Verify(rc => rc.HandleResponseEvent(It.IsAny<object>(), It.IsAny<ResponseLineEventArgs>()), Times.Never());
		}
	}
}

