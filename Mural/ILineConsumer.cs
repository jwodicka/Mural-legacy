using System;
namespace Mural
{
	/// <summary>
	/// The interface for objects which can handle lines raised by the end-user.
	/// </summary>
	public interface ILineConsumer
	{
		void HandleLineReadyEvent(object sender, LineReadyEventArgs args);
		void HandleDisconnectEvent(object sender, EventArgs args);
		
		void AddSource(IResponseConsumer source);
		void RemoveSource(IResponseConsumer source);
	}
}

