using Mafia_server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mafia_server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Completely disable ASP.NET Core logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
            builder.Logging.SetMinimumLevel(LogLevel.None);

            builder.Services.AddSignalR();

            var app = builder.Build();

            // Disable the development exception page
            app.UseExceptionHandler("/Error");

            app.MapHub<GameHub>("/gamehub");

            Globals.serverIsRunning = true;
            Logger.Initialize(ConsoleColor.Green);

            Console.WriteLine("Server started. Press Ctrl+C to shut down.");

            app.Run();
        }
    }
}