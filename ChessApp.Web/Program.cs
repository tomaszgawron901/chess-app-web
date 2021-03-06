
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ChessApp.Web.Services;
using ChessApp.Web.Controllers;

namespace ChessApp.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddScoped<IStringLocalizer<App>, StringLocalizer<App>>();

            builder.Services.AddScoped<GameHubService>();
            builder.Services.AddScoped<JSInteropService>();
            builder.Services.AddTransient<GameRoomController>();

            await builder.Build().RunAsync();
        }
    }
}
