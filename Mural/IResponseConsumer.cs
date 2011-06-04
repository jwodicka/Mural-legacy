using System;
namespace Mural
{
	/// <summary>
	/// The interface for objects which can send a reply up the message stream toward the user.
	/// </summary>
	public interface IResponseConsumer
	{
		void SendLineToUser(string line);
	}
}

