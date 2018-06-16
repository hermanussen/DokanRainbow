using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokanRainbow
{
    using System.Net;
    using DokanNet;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Ignore if SSL certificate is not valid
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                
                var rainbow = new Rainbow("https://localhost/", "sitecore", "admin", "c");
                rainbow.Mount("n:\\", DokanOptions.DebugMode, 5);

                Console.WriteLine(@"Success");
            }
            catch (DokanException ex)
            {
                Console.WriteLine(@"Error: " + ex.Message);
            }
        }
    }
}
