using System;

namespace Mural
{
	/// <summary>
    /// The arguments for events raised from the service side of the data pipeline.
    /// </summary>
	public abstract class ResponseEventArgs : EventArgs
	{
		public ResponseEventArgs ()
		{
		}
		
		/// <summary>
		/// Property overridden in child classes to indicate what event type they are.
		/// </summary>
		// TODO: Seriously, this ought to be an enum. All these string comparisons are silly.
		// (We can also infer the value of this using runtime type information, but I think
		// that might not be the sanest thing to do. My gut says it's going to be less efficient,
		// and it's likely to involve code that looks a little like wizardry.)
		abstract public string EventType { get; }	
	}
}	

