using System;
using System.Net;
using System.IO;
using log4net;
using log4net.Config;
using Ninject;
using Ninject.Modules;
using System.Configuration;
using System.Collections.Generic;
using System.Threading;

namespace Mural
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			// This method is currently proxying for the layer of the program that hooks up components to one another.
			// Each module should be unaware of the others. This is the only module that needs to know the lay of the
			// entire system.
			
			// Configure log4net based off the App.config
			XmlConfigurator.Configure();
			
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
			
			// TODO: Support binding on different addresses, configurable?
			//string localHostName = Dns.GetHostName();
			string localHostName = "localhost";
			
			// Grabs the IoC kernel to instantiate dependency-injected objects.
			MuralModule module = new MuralModule();
			IKernel kernel = new StandardKernel(module);
			
			ILineConsumer defaultParser = kernel.Get<LoginParser>();
			ISystemMessageProvider messageProvider = new HardcodedSystemMessageProvider();
			
			// This gets a list populated by ReadPortConfiguration with all the host/ports we know about from the config file.
			List<ListenerConfiguration> connectionList = ReadPortConfiguration();
			
			// Assume we failed to set up a listener unless we observe otherwise.
			bool listenerOn = false;
			
			// For each endpoint, attempt to set it up.
			foreach(ListenerConfiguration config in connectionList) 
			{
				IPHostEntry ipHostInfo = Dns.GetHostEntry(config.Host);
				
				// Gets the IP Address associated with the host given.
				// Assuming that it was an actual IP Address, there should only be one.
				// If it was a hostname, there might be several. We take the first; if something else is needed,
				//  then it should have been specified in the config file. That's what it's there for.
				IPAddress ipAddress = ipHostInfo.AddressList[0];
				
				_log.DebugFormat("Using net configuration: {0}:{1} ({2}). Listener type: {3}", 
					config.Host, config.Port, ipAddress.ToString(), config.Type);
								
				// TODO: Make connection type an enum so this can be a switch statement.
				// Or, possibly, a different refactor as part of making this support multiple listeners.
				switch(config.Type) 
				{
				case "telnet" :
					TelnetListener telnetListener = 
						new TelnetListener(defaultParser, messageProvider, ipAddress, config.Port);
					try {
						telnetListener.StartListenerLoop();
						listenerOn = true;
					} catch(System.Net.Sockets.SocketException e) {
						_log.Error(e.Message);
					}
					break;
				default:
					_log.Warn(String.Format("No listener of type {0}.", config.Type));
					break;
				}
			}
			
			// If we actually managed to set up at least one listener:
			if (listenerOn)
			{
				// Wait for someone to call _endProgram.Set(). Block until that happens.
				_endProgram.Reset();
				_endProgram.WaitOne();
			}
			
			_log.Debug("Reached the end of the program.");
		}

		private static List<ListenerConfiguration> ReadPortConfiguration()
		{
			List<ListenerConfiguration> connectionList = new List<ListenerConfiguration>();
			try {
				// Pull values for Port Configuration as defined in App.config
				PortConfigurationSection PortConfig = (PortConfigurationSection)ConfigurationManager.GetSection("hosts");
				
				foreach(HostElement host in PortConfig.HostCollection) {
					foreach(PortElement port in host.PortCollection) {
						// ListenerConfiguration is essentially a struct with name, number, and type.
						connectionList.Add(new ListenerConfiguration(host.Name, port.Number, port.Type));
					}
				}
			} catch(ConfigurationException e) {
				// This will cause a ConfigurationException for a duplicate host or port number.
				// There may be other error conditions that cause ConfigurationExceptions.
				// Regardless, if we can't read our configuration, we are sad and confused pandas.
				// Log as an error, then exit.
				_log.ErrorFormat(e.Message);
				Environment.Exit(1);
			}
			
			if(connectionList.Count == 0) {
				// There was nothing in the XML for connections!
				// TODO: A fancy configuration step.
				
				_log.Warn("No configuration provided! Please provide one in App.config");
				Environment.Exit(0);
			}
			return connectionList;
		}
		
		private static ManualResetEvent _endProgram = new ManualResetEvent(false);
		
		private static readonly ILog _log = LogManager.GetLogger(typeof(MainClass));
	}
}

