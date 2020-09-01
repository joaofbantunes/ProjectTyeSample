using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var db = scope
                    .ServiceProvider
                    .GetRequiredService<GreetingsDbContext>();
                db
                    .Database
                    .EnsureCreated();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    services.AddDbContext<GreetingsDbContext>(options => options.UseNpgsql(hostContext.Configuration.GetConnectionString("postgres")));
                });
    }
}
