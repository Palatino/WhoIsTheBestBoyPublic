using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Services;
using BestBoyClient.Services.IServices;
using BestBoyClient.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BestBoyClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("Auth0", options.ProviderOptions);
                options.ProviderOptions.ResponseType = "code";
            }).AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>();

            builder.Services.AddMudServices();

            //Register HtppClient which includes the authorization token
            builder.Services.AddHttpClient("AuthenticatedClient",
            client => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("BaseAPIUrl")))
           .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
           .ConfigureHandler(
            authorizedUrls: new[] { builder.Configuration.GetValue<string>("BaseAPIUrl")}
            ));

            //Register client withou authorization token for the public endpoints
            builder.Services.AddHttpClient("AnonymousClient",
            client => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("BaseAPIUrl")));


            
            builder.Services.AddScoped<IDogService, DogService>();



            await builder.Build().RunAsync();
        }
    }
}
