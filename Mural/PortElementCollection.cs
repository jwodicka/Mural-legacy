using System;
using System.Configuration;

namespace Mural
{
	// TODO: KIll this. ... or not?
	[ConfigurationCollection(typeof(PortElement), AddItemName = "port")]
	public class PortElementCollection : ConfigurationElementCollection
	{
		protected override System.Configuration.ConfigurationElement CreateNewElement()
		{
			return new PortElement();
		}
		
		protected override object GetElementKey(System.Configuration.ConfigurationElement element) 
		{
			return ((PortElement)element).Number;
		}
	}

}

