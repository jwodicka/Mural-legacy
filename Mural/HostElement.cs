using System;
using System.Configuration;

namespace Mural
{
	public class HostElement : ConfigurationElement
	{
		[ConfigurationProperty("name")]
		public String Name
		{
			get
			{
				return (String)this["name"];
			}
		}
		
		[ConfigurationProperty("", IsDefaultCollection = true)]
		public PortElementCollection PortCollection
		{
			get
			{
				return (PortElementCollection)this[""];
			}
		}
		
		// TODO: Uncomment this once we're done debugging.
/*		public PortElement this [int number]
		{
			get {
				return (PortElement)BaseGet(number);
			}
		} */
	}

}

