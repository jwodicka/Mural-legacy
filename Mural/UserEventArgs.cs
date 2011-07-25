using System;

namespace Mural
{
    /// <summary>
    /// The arguments for events raised from the user side of the data pipeline.
    /// </summary>
	public abstract class UserEventArgs : EventArgs
	{
		public UserEventArgs (string originIdentifier)
		{
			OriginIdentifier = originIdentifier;
		}
		
		public UserEventArgs (string originIdentifier, ResponseDelegate responseHandler)
		{
			OriginIdentifier = originIdentifier;
			_responseHandler = responseHandler;
		}
		
		/// <summary>
		/// Property overridden in child classes to indicate what event type they are.
		/// </summary>
		// TODO: Again, this probably ought to be an enum. See ResponseEventArgs for my thinking.
		abstract public string EventType { get; }
		
		/// <summary>
		/// A string that uniquely identifies the endpoint that originated this session.
		/// </summary>
		public string OriginIdentifier { get; set; }
		
		/// <summary>
		/// Delegate for handling error messages encountered in the processing of this event.
		/// Should make an attempt to return the errorMessage to the user, at the endpoint
		/// most closely associated with the command. 
		/// </summary>
		/// <remarks>
		/// The ErrorDelegate EXPLICTLY does not guarantee that the message will be delivered.
		/// Messages that should reach the user reliably should be routed through the usual message
		/// pipeline, which will reach all appropriate endpoints.
		/// </remarks>
		public delegate void ResponseDelegate (string errorMessage);
		
		/// <summary>
		/// The ResponseDelegate to use in handling errors.
		/// </summary>
		public ResponseDelegate Respond { 
			get
			{
				return _responseHandler;	
			}
		}
		
		private ResponseDelegate _responseHandler;
	}
}

