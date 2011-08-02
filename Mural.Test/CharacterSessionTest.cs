using System;
using NUnit.Framework;
using Mural;
using Moq;

namespace Mural.Test
{
	[TestFixture]
	public class CharacterSessionTest
	{
		[Test]
		public void CharacterIdentityIsStoredOnCreation() {
			Character TestCharacter = new Character("Name", "World");
			CharacterSession TestCharacterSession = new CharacterSession(TestCharacter);
			
			Assert.IsTrue(TestCharacterSession.CharacterIdentity.Equals(TestCharacter));
		}
		
	}
}

