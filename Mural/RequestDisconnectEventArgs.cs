using System;
using System.Collections.Generic;

namespace Mural
{
	public class RequestDisconnectEventArgs : ResponseEventArgs
	{
		public RequestDisconnectEventArgs ()
		{
			Type = RequestType.All;
		}
		
		public RequestDisconnectEventArgs (RequestType requestType, string identifier)
		{
			Type = requestType;
			Identifiers = new List<string>();
			Identifiers.Add(identifier);
		}
		
		public RequestDisconnectEventArgs (RequestType requestType, List<string> identifiers)
		{
			Type = requestType;
			Identifiers = identifiers;
		}
		
		public List<string> Identifiers { get; set; }
		
		public RequestType Type { get; set; }
		
		public override string EventType {
			get {
				return "RequestDisconnect";
			}
		}
		
		public enum RequestType {
			All, // Disconnect all sessions; ignore Identifiers
			Only, // Disconnect the sessions that are in the Identifiers list.
			Except // Disconnect all sessions unless they are in the Identifiers list.
		}
	}
}

