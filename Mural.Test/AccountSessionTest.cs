using System;
using NUnit.Framework;
using Mural;
using Moq;

namespace Mural.Test
{
	[TestFixture]
	public class AccountSessionTest
	{
		[Test, Category("SadPath"), ExpectedException(typeof (ArgumentNullException))]
		public void CannotBeConstructedWithoutAccount ()
		{
			new AccountSession(null);	
		}
		
		#region Basic happy path setup.
		Account _account;
		AccountSession _session;
		
		[SetUp, Category("HappyPath")]
		public void SetUpTestObjects()
		{
			_account = new Account();
			_session = new AccountSession(_account);
		}
		
		[TearDown, Category("HappyPath")]
		public void TearDownTestObjects()
		{
			_account = null;
			_session = null;
		}
		#endregion
		
		[Test, Category("HappyPath")]
		public void ReturnsAccountIdentity ()
		{
			Assert.AreEqual(_account, _session.AccountIdentity);
		}
		
		[Test, Category("HappyPath")]
		public void CanAddSynchronousSource ()
		{
			MockResponseConsumer source = new MockResponseConsumer();
			_session.AddSource(source);
			Assert.Contains(new EventHandler<UserEventArgs>(_session.HandleUserEvent), source.UserEventHandlers);
		}

		[Test, Category("HappyPath")]
		public void CanRemoveSource ()
		{
			MockResponseConsumer source = new MockResponseConsumer();
			_session.AddSource(source);
			Assert.Contains(new EventHandler<UserEventArgs>(_session.HandleUserEvent), source.UserEventHandlers);
			_session.RemoveSource(source);
			Assert.IsEmpty(source.UserEventHandlers);
		}
		
		[Test, Category("HappyPath")]
		public void WillPassThroughResponseLineEvent()
		{
			ResponseEventArgs responseArgs = null;
			_session.RaiseResponseEvent += (sender, e) => { responseArgs = (ResponseEventArgs) e; };
			
			const string receivedLine = "This is a line.";
			_session.HandleResponseEvent(null, new ResponseLineEventArgs(receivedLine));
			
			Assert.IsNotNull(responseArgs);
			Assert.IsInstanceOfType(typeof(ResponseLineEventArgs), responseArgs);
			Assert.AreEqual(receivedLine, ((ResponseLineEventArgs)responseArgs).Line);
		}
		
		[Test, Category("HappyPath")]
		public void WillPassThroughLineReadyEvent()
		{
			UserEventArgs userArgs = null;
			_session.RaiseUserEvent += (sender, e) => { userArgs = (UserEventArgs) e; };
			
			const string line = "This is a line.";
			const string origin = "Origin of the line.";
			_session.HandleUserEvent(null, new LineReadyEventArgs(line, origin, null));
			
			Assert.IsNotNull(userArgs);
			Assert.IsInstanceOfType(typeof(LineReadyEventArgs), userArgs);
			Assert.AreEqual(line, ((LineReadyEventArgs)userArgs).Line);
			Assert.AreEqual(origin, ((LineReadyEventArgs)userArgs).OriginIdentifier);
		}
		
		[Test, Category("HappyPath")]
		public void WillPassThroughDisconnectEvent()
		{
			MockResponseConsumer source = new MockResponseConsumer();

			UserEventArgs userArgs = null;
			_session.RaiseUserEvent += (sender, e) => { userArgs = (UserEventArgs) e; };
			
			const string origin = "Origin of the disconnect.";
			_session.HandleUserEvent(source, new DisconnectEventArgs(origin));
			
			Assert.IsNotNull(userArgs);
			Assert.IsInstanceOfType(typeof(DisconnectEventArgs), userArgs);
			Assert.AreEqual(origin, userArgs.OriginIdentifier);
		}
		
		[Test, Category("HappyPath")]
		public void WillRemoveSourceWhenProcessingDisconnectEvent()
		{
			MockResponseConsumer source = new MockResponseConsumer();

			_session.AddSource(source);
			Assert.Contains(new EventHandler<UserEventArgs>(_session.HandleUserEvent), source.UserEventHandlers);
			
			const string origin = "Origin of the disconnect.";
			_session.HandleUserEvent(source, new DisconnectEventArgs(origin));
			
			//Source should have been removed when the disconnect message was handled.
			Assert.IsEmpty(source.UserEventHandlers);
		}
	}
}

