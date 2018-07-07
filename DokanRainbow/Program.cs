namespace DokanRainbow
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
                    // Read configuration
                    dynamic jsonConfig = JsonConvert.DeserializeObject(r.ReadToEnd());

                    if (jsonConfig.ignoreHttpsInvalidCertificates.Value)
                    {
                        // Ignore if SSL certificate is not valid
                        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    }

                    // Setup tasks, so that drives can be mounted and run simultaneously
                    List<Task> drivesToMount = new List<Task>();
                    foreach (dynamic drive in jsonConfig.drives)
                    {
                        var rainbowDrive = new Rainbow(
                            (string) drive.url,
                            (string) drive.domain,
                            (string) drive.userName,
                            (string) drive.password,
                            (string) drive.databaseName,
                            (int) jsonConfig.cacheTimeSeconds);
                        drivesToMount.Add(Mount(rainbowDrive, (string) drive.mountPoint));
                    }
                    
                    // Only if all drive mounts are stopped, the program will be stopped
                    Task.WaitAll(drivesToMount.ToArray());

                    Console.WriteLine(@"Program terminated");
                }
            }
            catch (DokanException ex)
            {
                Console.WriteLine(@"Error: " + ex.Message);
            }
        }

        private async static Task Mount(Rainbow rainbow, string mountPoint)
        {
            await Task.Run(() => rainbow.Mount(mountPoint, DokanOptions.StderrOutput, 5));
        }
    }
}
