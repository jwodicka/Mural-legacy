using System;
using NUnit.Framework;
using Mural;
using Moq;

namespace Mural.Test
{
	[TestFixture]	
	public class CharacterTest
	{
		[Test]
		public void CanCreateCharacter() {
			String name = "TestName";
			String world = "TestWorld";
			
			Character TestCharacter = new Character(name, world);
			Assert.AreEqual(TestCharacter.Name, name);
			Assert.AreEqual(TestCharacter.World, world);
		}
		
		[Test]
		public void CaseOfCharacterNameIsPreserved() {
			Character TestCharacter = new Character("tEsTnAme", "World");
			
			Assert.AreEqual(TestCharacter.Name, "tEsTnAme");
		}
		
		[Test]
		public void CaseOfCharacterWorldsIsPreserved() {
			Character TestCharacter = new Character("TestName", "wOrLd");
			
			Assert.AreEqual(TestCharacter.World, "wOrLd");
			
		}
		
		[Test]
		public void CharacterNamesAreCaseInsensitiveWhenTestingEquality() {
			Character TestCharacter1 = new Character("Name", "World");
			Character TestCharacter2 = new Character("name", "World");
			
			Assert.IsTrue(TestCharacter1.Equals(TestCharacter2));
		}
		
		[Test]
		public void CharacterWorldsAreCaseInsensitiveWhenTestingEquality() {
			Character TestCharacter1 = new Character("Name", "World");
			Character TestCharacter2 = new Character("Name", "world");
			
			Assert.IsTrue(TestCharacter1.Equals(TestCharacter2));
		}
		
		[Test]
		public void IdenticalCharactersAreEqual() {
			String name = "TestName";
			String world = "TestWorld";
			
			Character TestCharacter1 = new Character(name, world);
			Character TestCharacter2 = new Character(name, world);
			Assert.IsTrue(TestCharacter1.Equals(TestCharacter2));
		}
		
		[Test]
		public void CharactersAreNotEqualWithDifferentNameAndSameWorld() {
			Character TestCharacter1 = new Character("Name1", "World");
			Character TestCharacter2 = new Character("Name2", "World");
			Assert.IsFalse(TestCharacter1.Equals(TestCharacter2));
		}
		
		[Test]
		public void CharactersAreNotEqualWithSameNameAndDifferentWorld() {
			Character TestCharacter1 = new Character("Name", "World1");
			Character TestCharacter2 = new Character("Name", "World2");
			Assert.IsFalse(TestCharacter1.Equals(TestCharacter2));
		}
		
		[Test]
		public void HashCodePersists() {
			Character TestCharacter = new Character("Name", "World");
			Assert.AreEqual(TestCharacter.GetHashCode(), TestCharacter.GetHashCode());
		}
		
		public void IdenticalCharactersHaveIdenticalHashCodes() {
			String name = "TestName";
			String world = "TestWorld";
			
			Character TestCharacter1 = new Character(name, world);
			Character TestCharacter2 = new Character(name, world);
			Assert.AreEqual(TestCharacter1.GetHashCode(), TestCharacter2.GetHashCode());
		}
		
		public void DifferentCharactersHaveDifferentHashCodes() {
			Character TestCharacter1 = new Character("Name1", "World");
			Character TestCharacter2 = new Character("Name2", "World");
			Assert.IsFalse(TestCharacter1.Equals(TestCharacter2));
			Assert.AreNotEqual(TestCharacter1.GetHashCode(), TestCharacter2.GetHashCode());
		}
	}
}

