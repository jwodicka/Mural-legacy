using System;
using System.Configuration;

namespace Mural
{
	// This is the definition for a single port element.
	// In the XML, it looks like:
	//   <port number="1234" type="foo" />
	public class PortElement : ConfigurationElement
	{
		// A port has a number property...
		[ConfigurationProperty("number")]
		public int Number
		{
			get
			{
				return (int)this["number"];
			}
		}
		
		// ...and a type property.
		// TODO: Validation. This can only support a limited set of types.
		// We probably want an enum here.
		// It can be:
		//   HTTP
		//   Telnet
		//   SSH
		//   ...Jabber? SMTP? etc.
		// For now, it's just a string.
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

