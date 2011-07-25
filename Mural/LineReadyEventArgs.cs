using System;
namespace Mural
{
	public class LineReadyEventArgs : UserEventArgs 
	{
		public LineReadyEventArgs(string line, string originIdentifier, ResponseDelegate responseHandler) 
			: base(originIdentifier, responseHandler)
		{
			_line = line;
		}
		
		/// <summary>
		/// Gets or sets the line of input associated with this event.
		/// </summary>
		public string Line 
		{
			get
			{
				return _line;	
			}
			set
			{
				_line = value;	
			}
		}
		
		public override string EventType 
		{
			get
			{
				return "LineReady";	
			}
		}
		
		private string _line;
	}
}

