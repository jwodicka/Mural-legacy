using System;

namespace Mural
{
	public class ResponseLineEventArgs : ResponseEventArgs
	{
		public ResponseLineEventArgs (string line)
		{
			_line = line;
		}
		
		public string Line {
			get {
				return _line;	
			}
		}
		
		public override string EventType {
			get {
				return "ResponseLine";
			}
		}
		
		private string _line;
	}
}

