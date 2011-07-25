using System;
namespace Mural
{
	/// <summary>
	/// The interface for objects which can handle lines raised by the end-user.
	/// </summary>
	public interface ILineConsumer
	{	 		
		// This is the listener for all UserEvents. 
		// All events raised from the user side of the event-stream are UserEvents.
		// They are distinguished by which subclass of UserEventArgs they raise.
		void HandleUserEvent(object sender, UserEventArgs args);
		
		// All objects that can handle userEvents need to be able to raise reponse events,
		// because the messaging pipeline is bidirectional.
		event EventHandler<ResponseEventArgs> RaiseResponseEvent;
		
		// This pair of methods currently attach sources to ILineConsumer sinks.
		// Sources must be IResponseConsumers, so that messages can be handed back
		// up the chain to them.
		void AddSource(IResponseConsumer source);
		void RemoveSource(IResponseConsumer source);
	}
}

