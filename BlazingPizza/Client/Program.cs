using BlazingPizza.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlazingPizza.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddScoped<OrderState>();
            builder.Services.AddApiAuthorization<PizzaAuthenticationState>(options => 
            {
                options.AuthenticationPaths.LogOutSucceededPath = "";
            });
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-MX");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-MX");

            builder.Services.AddHttpClient<OrdersClient>(httpclient => httpclient.BaseAddress
            = new(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
            await builder.Build().RunAsync();
        }
    }
}
