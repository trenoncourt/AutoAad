using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoAad.Api.Extensions;
using Flurl.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AutoAad.Api
{
    public class Program
    {
        private static AppSettings _appSettings;
        private static Token _token;
        
        public static void Main()
        {
            BuildWebHost().Run();
        }

        public static IWebHost BuildWebHost()
        {
            return new WebHostBuilder()
                .UseKestrel(options => options.AddServerHeader = false)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                    _appSettings = config.Build().Get<AppSettings>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        logging.AddConsole();
                    }
                })
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                    if (_appSettings.Cors.Enabled)
                    {
                        services.AddCors();
                    }
                })
                .Configure(app =>
                {
                    app.ConfigureCors(_appSettings);
                    app.Run(async context =>
                    {
                        if (!context.Request.Path.HasValue)
                        {
                            context.Response.StatusCode = 404;
                            return;
                        }

                        if (_token == null || _token.ExpireDate <= DateTime.UtcNow)
                        {
                            // Get the token if first time or expired
                            // TODO add redis store instead of static variable
                            HttpResponseMessage tknResponse = await $"{_appSettings.AzureAd.Instance}{_appSettings.AzureAd.Domain}/oauth2/v2.0/token"
                                .AllowHttpStatus("4xx")
                                .SendUrlEncodedAsync(HttpMethod.Post, new
                                {
                                    grant_type = "client_credentials",
                                    client_id = _appSettings.AzureAd.ClientId,
                                    client_secret = _appSettings.AzureAd.ClientSecret,
                                    scope = $"{_appSettings.AzureAd.Resource}.default"
                                }).ConfigureAwait(false);

                            if (!tknResponse.IsSuccessStatusCode)
                            { // If the response is not success return the stream.
                                context.Response.ContentType = "application/json";
                                context.Response.StatusCode = (int)tknResponse.StatusCode;
                                Stream responseBufferStream = await tknResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                                await responseBufferStream.CopyToAsync(context.Response.Body).ConfigureAwait(false);
                                return;
                            }

                            using (Stream stream = await tknResponse.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            {
                                using (var sr = new StreamReader(stream))
                                {
                                    string json = await sr.ReadLineAsync().ConfigureAwait(false);
                                    TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);
                                    _token = new Token { AccessToken = tokenResponse.AccessToken, ExpireDate = DateTime.UtcNow.AddMinutes(tokenResponse.ExpireInMinutes) };
                                }
                            }
                        }
                        
                        HttpResponseMessage resourceResponse = await $"{_appSettings.AzureAd.Resource}{_appSettings.AzureAd.GraphVersion}{context.Request.Path.Value}{context.Request.QueryString.Value ?? ""}"
                            .WithOAuthBearerToken(_token.AccessToken)
                            .GetAsync()
                            .ConfigureAwait(false);
                        
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int)resourceResponse.StatusCode;
                        Stream resourceResponseBufferStream = await resourceResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                        await resourceResponseBufferStream.CopyToAsync(context.Response.Body).ConfigureAwait(false);
                        return;
                    });
                })
                .Build();
            
            
        }
    }
}
