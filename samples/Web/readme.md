## Razor Pages Example

### Facebook and Instagram Setup

First make sure you have the `client_id`, `client_secret` and `redirect_url` configured and noted down, for more information go to the [Facebook and Instagram Setup](../../docs/facebook-and-instagram-setup.md#facebook-and-instagram-setup) page.

![](https://i.imgur.com/JN2fppT.png)

![](https://i.imgur.com/bhJybs4.png)

### Update configuration with Instagram Credentials

**appsettings.json**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "InstagramCredentials": {
    "Name": "friendly name or your app name can go here - this is passed to Instagram as the user-agent",
    "ClientId": "client-id",
    "ClientSecret": "client-secret",
    "RedirectUrl": "https://localhost:5001/auth/oauth"
  }
}
```
### Startup.cs

Add references to your startup.

```cs
services.AddScoped<Solrevdev.InstagramBasicDisplay.Core.InstagramApi>();
services.AddScoped<Solrevdev.InstagramBasicDisplay.Core.InstagramHttpClient>();
services.Configure<InstagramCredentials>(configuration.GetSection("InstagramCredentials"));
```

A full example:

```cs
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

```


### Update Facebook app to ensure localhost:5001 is valid redirect_url

Ensure `https://localhost:5001/auth/oauth` is a valid `redirect_url`

![](https://i.imgur.com/bhJybs4.png)

### Start kestrel web server

**First navigate to the directory this `readme.md` file is in.**

```bash
cd path/to/samples/Web/
```

**Run `dotnet` on port 5001**

```powershell
dotnet run
```

or

```powershell
dotnet watch run
```

### Open page

e.g `https://localhost:5001/` which will serve the razor page from the samples/Web folder over https which is required by the Facebook app.
