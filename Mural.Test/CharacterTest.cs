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
			
			Character TestCharacter = new Character(name, world, null);
			Assert.AreEqual(TestCharacter.Name, name);
			Assert.AreEqual(TestCharacter.World, world);
		}
		
		[Test]
		public void CaseOfCharacterNameIsPreserved() {
			Character TestCharacter = new Character("tEsTnAme", "World", null);
			
			Assert.AreEqual(TestCharacter.Name, "tEsTnAme");
		}
		
		[Test]
		public void CaseOfCharacterWorldsIsPreserved() {
			Character TestCharacter = new Character("TestName", "wOrLd", null);
			
			Assert.AreEqual(TestCharacter.World, "wOrLd");
			
		}
		
		[Test]
		public void CharacterNamesAreCaseInsensitiveWhenTestingEquality() {
			Character TestCharacter1 = new Character("Name", "World", null);
			Character TestCharacter2 = new Character("name", "World", null);
			
			Assert.AreEqual(TestCharacter1, TestCharacter2);
		}
		
		[Test]
		public void CharacterWorldsAreCaseInsensitiveWhenTestingEquality() {
			Character TestCharacter1 = new Character("Name", "World", null);
			Character TestCharacter2 = new Character("Name", "world", null);
			Assert.AreEqual(TestCharacter1, TestCharacter2);
		}
		
		[Test]
		public void IdenticalCharactersAreEqual() {
			String name = "TestName";
			String world = "TestWorld";
			
			Character TestCharacter1 = new Character(name, world, null);
			Character TestCharacter2 = new Character(name, world, null);
			Assert.AreEqual(TestCharacter1, TestCharacter2);
		}
		
		[Test]
		public void CharactersAreNotEqualWithDifferentNameAndSameWorld() {
			Character TestCharacter1 = new Character("Name1", "World", null);
			Character TestCharacter2 = new Character("Name2", "World", null);
			Assert.AreNotEqual(TestCharacter1, TestCharacter2);
		}
		
		[Test]
		public void CharactersAreNotEqualWithSameNameAndDifferentWorld() {
			Character TestCharacter1 = new Character("Name", "World1", null);
			Character TestCharacter2 = new Character("Name", "World2", null);
			Assert.AreNotEqual(TestCharacter1, TestCharacter2);
		}
		
		[Test]
		public void HashCodePersists() {
			Character TestCharacter = new Character("Name", "World", null);
			Assert.AreEqual(TestCharacter.GetHashCode(), TestCharacter.GetHashCode());
		}
		
		[Test]
		public void IdenticalCharactersHaveIdenticalHashCodes() {
			String name = "TestName";
			String world = "TestWorld";
			
			Character TestCharacter1 = new Character(name, world, null);
			Character TestCharacter2 = new Character(name, world, null);
			Assert.AreEqual(TestCharacter1.GetHashCode(), TestCharacter2.GetHashCode());
		}
		
		[Test]
		public void HashCodesAreCharacterNameCaseInsensitive() {
			Character TestCharacter1 = new Character("Name", "World", null);
			Character TestCharacter2 = new Character("name", "World", null);
			
			Assert.AreEqual(TestCharacter1.GetHashCode(), TestCharacter2.GetHashCode());
		}
		
		[Test]
		public void HashCodesAreCharacterWorldInsensitive() {
			Character TestCharacter1 = new Character("Name", "World", null);
			Character TestCharacter2 = new Character("Name", "world", null);
			Assert.AreEqual(TestCharacter1.GetHashCode(), TestCharacter2.GetHashCode());
		}
		
		[Test]
		public void DifferentCharactersHaveDifferentHashCodes() {
			Character TestCharacter1 = new Character("Name1", "World", null);
			Character TestCharacter2 = new Character("Name2", "World", null);
			Assert.AreNotEqual(TestCharacter1, TestCharacter2);
			Assert.AreNotEqual(TestCharacter1.GetHashCode(), TestCharacter2.GetHashCode());
		}
	}
}

