using System;
using System.IO;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Mural
{
	public class SslSession : TelnetSession
	{
		public SslSession (Socket sessionSocket)
			: base(sessionSocket)
		{
			_secureStream = new SslStream(
				base.ConnectionStream, 
				false, 
				new RemoteCertificateValidationCallback(IgnoreCertificateErrorsCallback));
		}
		
		public override void BeginRecieve ()
		{
			// This will be called repeatedly. We only want to authenticate the first time.
			if (_secureStream.IsAuthenticated == false)
			{
				try {
					_secureStream.AuthenticateAsClient("muck.furry.com", 
						null, System.Security.Authentication.SslProtocols.Default, false);
				} catch (Exception) {
					Console.WriteLine("Exception authenticating:");
					throw; // Rethrow preserving context!
				}
			}
			base.BeginRecieve();
		}
		

		// This code is scaring the crap out of me. Why is the cert not
		// accepted by default?
		static bool IgnoreCertificateErrorsCallback(object sender,
	        X509Certificate certificate,
	        X509Chain chain,
	        SslPolicyErrors sslPolicyErrors)
	    {
	        if (sslPolicyErrors != SslPolicyErrors.None)
	        {
	 
	            Console.WriteLine("IgnoreCertificateErrorsCallback: {0}", sslPolicyErrors);
	            //you should implement different logic here...
	 
	            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
	            {
	                foreach (X509ChainStatus chainStatus in chain.ChainStatus)
	                {
	                    Console.WriteLine("\t" + chainStatus.Status);
	                }
	            }
	        }
	 
	        //returning true tells the SslStream object you don't care about any errors.
	        return true;
	    }
		
		protected override Stream ConnectionStream 
		{
			get {
				return _secureStream;
			}
		}
		
		
		
		private SslStream _secureStream;
	}
}

