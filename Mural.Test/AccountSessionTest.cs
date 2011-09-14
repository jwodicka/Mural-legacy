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
			Mock<ICharacterOwnershipIndex> index = new Mock<ICharacterOwnershipIndex>();
			_account = new Account(null, null, index.Object);
			_session = new AccountSession(_account);
		}
		
		[TearDown]
		public void TearDownTestObjects()
		{
			_account = null;
			_session = null;
		}
		
		/// <summary>
		/// It is an exception to attempt to construct an AccountSession with a null Account.
		/// </summary>
		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void CannotBeConstructedWithoutAccount ()
		{
			new AccountSession(null);	
		}
		
		/// <summary>
		/// Should be able to retrieve the account used to create this session.
		/// </summary>
		[Test]
		public void ReturnsAccountIdentity ()
		{
			AccountSession session = new AccountSession(_account);
			Assert.AreEqual(_account, session.AccountIdentity);
		}
		
		/// <summary>
		/// After adding a source, events should be passed through successfully in either
		/// direction to/from source/sink.
		/// </summary>
		[Test]
		public void CanAddSource()
		{
			Mock<IResponseConsumer> source = new Mock<IResponseConsumer>();
			_session.AddSource(source.Object);
			
			Mock<ILineConsumer> sink = _session.WithMockSink();
			
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
		
		/// <summary>
		/// After a source has been added, RemoveSource should successfully remove it.  After
		/// RemoveSource has been called, further events should not be passed either direction
		/// to the source that was removed.
		/// </summary>
		[Test]
		public void CanRemoveSource()
		{
			Mock<IResponseConsumer> source = _session.WithMockSource();
			Mock<ILineConsumer> sink = _session.WithMockSink();
			
			// Remove the source after having added it.
			_session.RemoveSource(source.Object);
			
			// Raise an event from the sink and verify the source is never notified.
			sink.Raise(lc => lc.RaiseResponseEvent += null, new ResponseLineEventArgs("A line."));
			source.Verify(rc => rc.HandleResponseEvent(It.IsAny<object>(), It.IsAny<ResponseEventArgs>()), Times.Never());
			
			// Raise an event from the source and verify the sink is never notified.
			source.Raise(rc => rc.RaiseUserEvent += null, new LineReadyEventArgs("Another line.", "An origin.", null));
			sink.Verify(lc => lc.HandleUserEvent(It.IsAny<object>(), It.IsAny<UserEventArgs>()), Times.Never());
		}
		
		/// <summary>
		/// ResponseLine events should be passed through, rewriting the event source
		/// to be the AccountSession.
		/// </summary>
		[Test]
		public void WillPassThroughResponseLineEvent()
		{
			Mock<IResponseConsumer> source = _session.WithMockSource();
			Mock<ILineConsumer> sink = _session.WithMockSink();
			
			ResponseLineEventArgs args = new ResponseLineEventArgs("A line.");
			sink.Raise(lc => lc.RaiseResponseEvent += null, args);
			source.Verify(rc => rc.HandleResponseEvent(_session, args), Times.Once());
		}
		
		/// <summary>
		/// RequestDisconnect events should be passed through, rewriting the event source
		/// to be the AccountSession.
		/// </summary>
		[Test]
		public void WillPassThroughRequestDisconnectEvent()
		{
			Mock<IResponseConsumer> source = _session.WithMockSource();
			Mock<ILineConsumer> sink = _session.WithMockSink();
			
			RequestDisconnectEventArgs args = new RequestDisconnectEventArgs();
			sink.Raise(lc => lc.RaiseResponseEvent += null, args);
			source.Verify(rc => rc.HandleResponseEvent(_session, args), Times.Once());
		}
		
		/// <summary>
		/// LineReady events should be passed through, rewriting the event source
		/// to be the AccountSession.
		/// </summary>
		[Test]
		public void WillPassThroughLineReadyEvent()
		{
			Mock<IResponseConsumer> source = _session.WithMockSource();
			Mock<ILineConsumer> sink = _session.WithMockSink();
			
			LineReadyEventArgs args = new LineReadyEventArgs("A line.", "An origin.", null);
			source.Raise(rc => rc.RaiseUserEvent += null, args);
			sink.Verify(lc => lc.HandleUserEvent(_session, args), Times.Once());
		}
		
		/// <summary>
		/// Disconnect events should be passed through the AccountSession, and the source should
		/// be removed during the processing of the event, so this will be the last event passed
		/// through successfully.
		/// </summary>
		[Test]
		public void WillPassThroughDisconnectEventAndRemoveSource()
		{
			Mock<IResponseConsumer> source = _session.WithMockSource();
			Mock<ILineConsumer> sink = _session.WithMockSink();
			
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

