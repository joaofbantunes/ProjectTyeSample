using Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using static Api.Greeter;

namespace Frontend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages(options => options.RootDirectory = "/Features");

            // not ideal, but didn't want to waste too much time messing with trusting certificates and whatnot
            System.AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            services.AddGrpcClient<GreeterClient>(options =>
            {
                options.Address = Configuration.GetServiceUri("api");
            });

            services.AddDbContext<GreetingsDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("postgres")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapFallback(ctx =>
                {
                    ctx.Response.Redirect("/Greetings/List");
                    return Task.CompletedTask;
                });
            });
        }
    }
}
