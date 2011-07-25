using System;
namespace Mural
{
	/// <summary>
	/// The interface for objects which can handle messages from the service toward the user
	/// </summary>
	public interface IResponseConsumer
	{
		// The ability to recieve messages from the service for the user
		void HandleResponseEvent(object sender, ResponseEventArgs args);
		
		// The ability to raise messages from the user to the service
		event EventHandler<UserEventArgs> RaiseUserEvent;
	}
}

