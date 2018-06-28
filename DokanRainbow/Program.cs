using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokanRainbow
{
    using System.IO;
    using System.Net;
    using DokanNet;
    using Newtonsoft.Json;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (StreamReader r = new StreamReader("drives.json"))
                {
                    dynamic drives = JsonConvert.DeserializeObject(r.ReadToEnd());

                    // Ignore if SSL certificate is not valid
                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                    List<Task> drivesToMount = new List<Task>();
                    foreach (dynamic drive in drives.drives)
                    {
                        var rainbowDrive = new Rainbow(
                            drive.url?.ToString(),
                            drive.domain?.ToString(),
                            drive.userName?.ToString(),
                            drive.password?.ToString(),
                            drive.databaseName?.ToString());
                        drivesToMount.Add(Mount(rainbowDrive, drive.mountPoint));
                    }
                    
                    Task.WaitAll(drivesToMount.ToArray());

                    Console.WriteLine(@"Success");
                }
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
