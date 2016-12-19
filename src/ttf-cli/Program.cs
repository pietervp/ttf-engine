using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Ttf.Server.Core;

namespace Ttf.Server.SelfHosted
{
    public class Program
    {
        public static void Main()
        {
            // http://tidtilforsikring.dk/jobs/assignment.pdf
            var builder = new ConfigurationBuilder()
                                   .AddJsonFile("appconfig.json", optional: false, reloadOnChange: true)
                                   .AddEnvironmentVariables();

            TtfStartup.ConfigureMapper(builder.Build());

            var host = new WebHostBuilder()
                   .UseContentRoot(Directory.GetCurrentDirectory())
                   .UseKestrel()
                   .UseStartup<Startup>()
                   .UseUrls(TtfStartup.AppConfig["url-endpoint"])
                   .Build();

            host.Run();
        }
   }
}
