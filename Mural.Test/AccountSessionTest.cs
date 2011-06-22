using System;
using NUnit.Framework;
using Mural;
using Moq;

namespace Mural.Test
{
	[TestFixture]
	public class AccountSessionTest
	{
		[Test]
		public void ReturnsAccountIdentity ()
		{
			Account account = new Account();
			AccountSession accountSession = new AccountSession(account);
			
			Assert.AreEqual(account, accountSession.AccountIdentity);
		}
		
		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void CannotBeConstructedWithoutAccount ()
		{
			new AccountSession(null);	
		}
		
		[Test]
		public void CanAddSynchronousSource ()
		{
			Account account = new Account();
			AccountSession accountSession = new AccountSession(account);
			
			IResponseConsumer source = new Mock<SynchronousSession>().Object;
			
			accountSession.AddSource(source);
		}
		
		[Test]
		[ExpectedException(typeof(Exception))]
		public void CannotAddNonSynchronousSource ()
		{
			Account account = new Account();
			AccountSession accountSession = new AccountSession(account);
			
			IResponseConsumer source = new Mock<IResponseConsumer>().Object;
			
			accountSession.AddSource(source);
		}
		
		[Test]
		[ExpectedException(typeof(Exception))]
		public void CannotAddMultipleSources ()
		{
			Account account = new Account();
			AccountSession accountSession = new AccountSession(account);
			
			IResponseConsumer source1 = new Mock<SynchronousSession>().Object;
			IResponseConsumer source2 = new Mock<SynchronousSession>().Object;
			
			accountSession.AddSource(source1);
			accountSession.AddSource(source2);
		}
		
		[Test]
		public void CanRemoveSource ()
		{
			// TODO: Improve this test case once we have mocks firing events, to confirm
			//		 that the source has been removed!
			Account account = new Account();
			AccountSession accountSession = new AccountSession(account);
			
			IResponseConsumer source = new Mock<SynchronousSession>().Object;
			
			accountSession.AddSource(source);
			
			accountSession.RemoveSource(source);
		}
	}
}

