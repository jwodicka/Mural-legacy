using System;
using System.Net;
using log4net;
using log4net.Config;
using System.Configuration;
using System.Collections.Generic;

namespace Mural
{
	class MainClass
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(MainClass));
		
		public static void Main (string[] args)
		{
			// This method is currently proxying for the layer of the program that hooks up components to one another.
			// Each module should be unaware of the others. This is the only module that needs to know the lay of the
			// entire system.
			
			// Configure log4net based off the App.config
			XmlConfigurator.Configure();
			
			// TODO: Replace the logic here that connects components with an IoC system.
			// Probably use Ninject for this purpose: http://ninject.org/
			
			// This gets a list populated by ReadPortConfiguration with all the host/ports we know about from the config file.
			List<ListenerConfiguration> connectionList = ReadPortConfiguration();
			
			// We currently do not have the architecture in place to launch multiple listeners.
			// Therefore, we are going to use whatever the first item in the connectionList.
			// TODO: Support multiple listeners.
			// Once we have the architectural support for multiple listeners, we'll want to iterate 
			//  through the connectionList and set them all up.
			// Right now, this foreach is abusing the fact that the StartListenerLoop on a TelnetListener doesn't return.
			// It will keep trying until it finds one of type telnet, then pause there. Forever.
			foreach(ListenerConfiguration config in connectionList) {
				IPHostEntry ipHostInfo = Dns.GetHostEntry(config.Host);
				
				// Gets the IP Address associated with the host given.
				// Assuming that it was an actual IP Address, there should only be one.
				// If it was a hostname, there might be several. We take the first; if something else is needed,
				//  then it should have been specified in the config file. That's what it's there for.
				IPAddress ipAddress = ipHostInfo.AddressList[0];
				
				_log.DebugFormat("Using net configuration: {0}:{1} ({2}). Listener type: {3}", 
					config.Host, config.Port, ipAddress.ToString(), config.Type);
				
				ILineConsumer defaultParser = new LoginParser(); //new RedirectingParser();
				
				// TODO: Make connection type an enum so this can be a switch statement.
				// Or, possibly, a different refactor as part of making this support multiple listeners.
				switch(config.Type) 
				{
				case "telnet" :
					TelnetListener telnetListener = new TelnetListener(defaultParser, ipAddress, config.Port);
					// TODO: What does it look like to end the program politely? 
					// Who handles that, and how do we terminate the listener loops?
					// Do we support connection-draining?
					try {
						telnetListener.StartListenerLoop(); // This method doesn't return under normal circumstances.
					} catch(System.Net.Sockets.SocketException e) {
						_log.Error(e.Message);
					}
					break;
					break;
				default:
					_log.Warn(String.Format("No listener of type {0}.", config.Type));
					break;
				}
			}
			
			_log.Debug("Reached the end of the program.");
		}

		public List<ListenerConfiguration> ReadPortConfiguration ()
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
	}
}

