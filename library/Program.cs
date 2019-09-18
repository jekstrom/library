using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace library
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options => {
                    options.Listen(IPAddress.Any, 8081);
                    options.Listen(IPAddress.Loopback, 8443, listenOptions =>
                    {
                        listenOptions.UseHttps("cert.pfx", "password");
                    });
                })
                .UseStartup<Startup>();
    }
}
