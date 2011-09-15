using System;
using System.Configuration;

namespace Mural
{
	// This is the definition for a single host element.
	// In the XML, it looks like:
	//   <host name="foo">
	//     ...
	//   </host>
	public class HostElement : ConfigurationElement
	{
		// This is pulling the name property
		[ConfigurationProperty("name")]
		public String Name
		{
			get
			{
				return (String)this["name"];
			}
		}
		
		// So, ConfigurationProperty is a decorator to indicate some property of this XML element.
		// In this case, though, the combination of "" as the name and IsDefaultCollection actually means
		//  that we're taking the set of all subtags within this element.
		// (The set of <port> tags, in this case.)
		// They correspond to our PortElementCollection class.
		[ConfigurationProperty("", IsDefaultCollection = true)]
		public PortElementCollection PortCollection
		{
			get
			{
				return (PortElementCollection)this[""];
			}
		}
	}

}

