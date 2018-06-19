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
                
                var master = new Rainbow("https://localhost/", "sitecore", "admin", "c", "master");
                var web = new Rainbow("https://localhost/", "sitecore", "admin", "c", "web");
                var core = new Rainbow("https://localhost/", "sitecore", "admin", "c", "core");

                Task.WaitAll(Mount(master, "s:\\"), Mount(web, "t:\\"), Mount(core, "u:\\"));

                Console.WriteLine(@"Success");
            }
            catch (DokanException ex)
            {
                Console.WriteLine(@"Error: " + ex.Message);
            }
        }

        private async static Task Mount(Rainbow rainbow, string mountPoint)
        {
            await Task.Run(() => rainbow.Mount(mountPoint, DokanOptions.DebugMode, 5));
        }
    }
}
