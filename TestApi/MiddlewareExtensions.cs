using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;

namespace TestApi;

public static class MiddlewareExtensions
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Test API",
                Description = "ASP.NET Core Web API for testing puropses",
                //TermsOfService = new Uri("https://example.com/terms"),
                //Contact = new OpenApiContact
                //{
                //    Name = "Example Contact",
                //    Url = new Uri("https://example.com/contact")
                //},
                //License = new OpenApiLicense
                //{
                //    Name = "Example License",
                //    Url = new Uri("https://example.com/license")
                //}
            });
    
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        return services;
    }

    public static IWebHostBuilder ConfigureKestrel(this IWebHostBuilder webHost)
    {
        webHost.ConfigureKestrel(o =>
        {
            o.AddServerHeader = false;
            o.ConfigureHttpsDefaults(o =>
            {
                o.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
                o.OnAuthenticate = (context, sslOptions) =>
                {
                    sslOptions.CipherSuitesPolicy = new CipherSuitesPolicy(
                        new[]
                        {
                            // https://ciphersuite.info/cs/?singlepage=true&page=1&sort=desc&tls=tls12&security=recommended
                            
                            // TLS 1.3
                            TlsCipherSuite.TLS_AES_128_GCM_SHA256,
                            TlsCipherSuite.TLS_AES_256_GCM_SHA384,
                            TlsCipherSuite.TLS_CHACHA20_POLY1305_SHA256,
                            // TLS 1.2
                            TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
                            TlsCipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256,
                            TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384,
                            TlsCipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384,
                            TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256,
                            TlsCipherSuite.TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256
                        });
                };

            });
            o.ConfigureEndpointDefaults(o =>
            {
                o.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                o.UseHttps();
            });
        });

        return webHost;
    }

    public static IServiceCollection ConfigureHsts(this IServiceCollection services)
    {
        services.AddHsts(o =>
        {
            o.Preload = true;
            o.IncludeSubDomains = true;
            o.MaxAge = TimeSpan.FromDays(30);
        });

        return services;
    }

    public static IApplicationBuilder UseSwagger(this IApplicationBuilder app)
    {
        SwaggerBuilderExtensions.UseSwagger(app);
        app.UseSwaggerUI(o =>
        {
            o.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            o.RoutePrefix = string.Empty;
        });

        return app;
    }

    public static IApplicationBuilder UseSecurityCookies(this IApplicationBuilder app)
    {
        app.Use((context, next) =>
        {
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Add("Content-Security-Policy", "default-src 'self';");
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            return next.Invoke();
        });
        return app;
    }

    public static IApplicationBuilder UseDisableCaching(this IApplicationBuilder app)
    {
        app.Use((context, next) =>
        {
            context.Response.Headers.Expires = new StringValues("0");
            return next.Invoke();
        });
        return app;
    }
}

