using System;
using System.Configuration;

namespace Mural
{
	public class PortConfigurationSection : ConfigurationSection
	{
		[ConfigurationProperty("", IsDefaultCollection = true)]
		public HostElementCollection HostCollection
		{
			get
			{
				return (HostElementCollection)this[""];
			}
		}
	}
}
