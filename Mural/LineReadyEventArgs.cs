using System;
namespace Mural
{
	public class LineReadyEventArgs : EventArgs 
	{
		public LineReadyEventArgs(string line, SynchronousSession origin) 
		{
			_line = line;
			_origin = origin;
		}
		
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
		
		public SynchronousSession Origin
		{
			get
			{
				return _origin;	
			}
			set
			{
				_origin = value;	
			}
		}
		
		private string _line;
		private SynchronousSession _origin;
	}
}

