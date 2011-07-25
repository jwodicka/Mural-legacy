using System;

namespace Mural
{
	/// <summary>
	/// The CharacterSession is the aggregate session for one character / world intersection.
	/// It is a MultiplexSession, so it can have multiple (or no!) sessions upstream of it, one
	/// per user connection.
	/// </summary>
	public class CharacterSession : MultiplexSession, ICharacterAuthenticated
	{
		public CharacterSession (Character character)
		{
			_character = character;
		}
		
		public Character CharacterIdentity
		{
			get
			{
				return _character;	
			}
		}
		
		public SessionBuffer Buffer { get; set; }
		
		private Character _character;
	}
}

