using System;
using System.Configuration;

namespace Mural
{
	// The ConfigurationSection corresponds to a blob of XML in the App.config.
	// It is registered in the configsections, and used to parse that part of the XML.
	// PortConfigurationSection is what we use to store the hostnames and ports being used by Mural.
	public class PortConfigurationSection : ConfigurationSection
	{
		// So, ConfigurationProperty is a decorator to indicate some property of this XML element.
		// In this case, though, the combination of "" as the name and IsDefaultCollection actually means
		//  that we're taking the set of all subtags within this element.
		// (The set of <host> tags, in this case.)
		// They correspond to our HostElementCollection class.
		[ConfigurationProperty("", IsDefaultCollection = true)]
		public HostElementCollection HostCollection
		{
			// The getter for our collection of tags.
			get
			{
				return (HostElementCollection)this[""];
			}
			// TODO: write a setter for automated config.
		}
	}
}
