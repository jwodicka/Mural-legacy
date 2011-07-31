using System;
using System.Net;

namespace Mural
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			// This method is currently proxying for the layer of the program that hooks up components to one another.
			// Each module should be unaware of the others. This is the only module that needs to know the lay of the
			// entire system.
			
			// TODO: Replace the logic here that connects components with an IoC system.
			// Probably use Ninject for this purpose: http://ninject.org/
			
			// TODO: Figure out how Mural actually becomes aware of the IP addresses it serves,
			// and the ports it should listen on. Perhaps the standard .NET .config XML files?
			
			// This whole next bit is hackish: It gets an IP address (probably!) that _should_ work for this machine.
			// No promises are made that it does or it will. It is certain to fail miserably on machines that host
			//  multiple domains, or have multiple IP addresses, or are behind load balancers, or are interesting in any
			//  of a number of other ways. It has fascinating issues on WinBoxen with IPv6 installed.
			// That is to say:
			// THIS IS NOT PRODUCTION CODE.
			// Replace this with a combination of defaulting as elegantly as possible to let simple users get it
			//  mostly-right when they try to run Mural on their local boxen, and reading from config files so that
			//  serious installers of Mural can set it all up in config and have it work.
			String localHostName = Dns.GetHostName();
			
			localHostName = "localhost";
			
			IPHostEntry ipHostInfo = Dns.GetHostEntry(localHostName);
			// Gets the first IP Address associated with this machine.
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			int port = 8888;
			
			// TODO: Let's get a decent logger in here, shall we?
			Console.WriteLine("Autodetected net configuration: {0} ({1})", localHostName, ipAddress.ToString());
			
			ILineConsumer defaultParser = new LoginParser(); //new RedirectingParser();
			
			TelnetListener telnetListener = new TelnetListener(defaultParser, ipAddress, port);
			// TODO: What does it look like to end the program politely? 
			// Who handles that, and how do we terminate the listener loops?
			// Do we support connection-draining?
			telnetListener.StartListenerLoop(); // This method doesn't return under normal circumstances.
			
			Console.WriteLine("Reached the end of the program.");
		}
	}
}

