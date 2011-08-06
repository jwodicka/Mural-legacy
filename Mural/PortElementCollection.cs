using System;
using System.Configuration;

namespace Mural
{
	// This is a collection of multiple ConfigurationElements.
	// The decorator here tells us that, in case we missed the subclassing.
	// Additionally, it says that:
	//  - The elements contained in this collection are of type PortElement
	//  - The tag in the XML we should recognize as an element to add is "port"
	[ConfigurationCollection(typeof(PortElement), AddItemName = "port")]
	public class PortElementCollection : ConfigurationElementCollection
	{
		// When we make a new element, make it a PortElement! This gets called automagically.
		protected override System.Configuration.ConfigurationElement CreateNewElement()
		{
			return new PortElement();
		}
		
		// This is where we define what the unique key for this collection is. In this case, it's the number.
		// Two ports cannot have the same number.
		// If they do in the XML, the parser will throw an exception when it discovers that.
		// Of course, ports are only required to be unique within the containing hostname, not globally.
		protected override object GetElementKey(System.Configuration.ConfigurationElement element) 
		{
			return ((PortElement)element).Number;
		}
	}

}

