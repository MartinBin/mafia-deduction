using Mafia_server;
using Mafia_server.Observer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mafia_server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Completely disable ASP.NET Core logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
            builder.Logging.SetMinimumLevel(LogLevel.None);
            builder.Services.AddSingleton<GameManager>();
            builder.Services.AddSignalR();

            var commandSubject = new CommandSubject();
            builder.Services.AddSingleton(commandSubject);
            
            builder.Services.AddSingleton<GameHub>(sp =>
            {
                var gameManager = sp.GetRequiredService<GameManager>();
                var hubContext = sp.GetRequiredService<IHubContext<GameHub>>();
                return new GameHub(gameManager, commandSubject, hubContext);
            });
            var app = builder.Build();

            // Disable the development exception page
            app.UseExceptionHandler("/Error");

            app.MapHub<GameHub>("/gamehub");

            Globals.serverIsRunning = true;
            Logger.Initialize(ConsoleColor.Green);
            Logger.getInstance.Log(LogType.Info,"Server started. Press Ctrl+C to shut down.");
            //Console.WriteLine("Server started. Press Ctrl+C to shut down.");

            var terminalCommandHandler = new TerminalCommandHandler(commandSubject);
            var terminalTask = terminalCommandHandler.ReadCommandsFromTerminalAsync();
            
            await Task.WhenAny(app.RunAsync(), terminalTask);
        }
    }
}