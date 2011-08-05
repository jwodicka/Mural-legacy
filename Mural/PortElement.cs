using System;
using System.Configuration;

namespace Mural
{
	public class PortElement : ConfigurationElement
	{
		[ConfigurationProperty("number")]
		public int Number
		{
			get
			{
				return (int)this["number"];
			}
		}
		
		[ConfigurationProperty("type")]
		public String Type
		{
			get
			{
				return (String)this["type"];
			}
		}
		
	}

}

