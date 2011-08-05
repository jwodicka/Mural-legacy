using System;
using System.Configuration;

namespace Mural
{
	[ConfigurationCollection(typeof(HostElement), AddItemName = "host")]
	public class HostElementCollection : ConfigurationElementCollection
	{
		protected override System.Configuration.ConfigurationElement CreateNewElement()
		{
			return new HostElement();
		}
		
		protected override object GetElementKey(System.Configuration.ConfigurationElement element) 
		{
			return ((HostElement)element).Name;
		}
		// TODO: Uncomment this once we're done debugging.
/*		new public HostElement this [String name]
		{
			get {
				return (HostElement)BaseGet(name);
			}
		}
		*/
	}
}

