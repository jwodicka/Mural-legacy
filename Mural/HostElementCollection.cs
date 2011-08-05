using System;
using System.Configuration;

namespace Mural
{
	// This is a collection of multiple ConfigurationElements.
	// The decorator here tells us that, in case we missed the subclassing.
	// Additionally, it says that:
	//  - The elements contained in this collection are of type HostElement
	//  - The tag in the XML we should recognize as an element to add is "host"
	[ConfigurationCollection(typeof(HostElement), AddItemName = "host")]
	public class HostElementCollection : ConfigurationElementCollection
	{
		// When we make a new element, make it a HostElement! This gets called automagically.
		protected override System.Configuration.ConfigurationElement CreateNewElement()
		{
			return new HostElement();
		}
		
		// This is where we define what the unique key for this collection is. In this case, it's the name.
		// Two hosts cannot have the same hostname.
		// If they do in the XML, the parser will throw an exception when it discovers that.
		protected override object GetElementKey(System.Configuration.ConfigurationElement element) 
		{
			return ((HostElement)element).Name;
		}
		
		new public HostElement this [String name]
		{
			get {
				return (HostElement)BaseGet(name);
			}
		}
		
	}
}

