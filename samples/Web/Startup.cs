using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Solrevdev.InstagramBasicDisplay.Core;
using Solrevdev.InstagramBasicDisplay.Core.Instagram;

namespace Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<InstagramCredentials>(Configuration.GetSection("InstagramCredentials"));
            services.AddScoped<InstagramHttpClient>();
            services.AddScoped<InstagramApi>();
            services.AddHttpClient();

            services.AddRouting(option =>
            {
                option.AppendTrailingSlash = true;
                option.LowercaseUrls = true;
            });

            services.AddSession();

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
#pragma warning disable CA1822 // Mark members as static
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#pragma warning restore CA1822 // Mark members as static
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapRazorPages());
        }
    }
}
