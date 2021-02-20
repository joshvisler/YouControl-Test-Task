using Server.Constants;
using Server.Services;
using System;
using Microsoft.Extensions.Configuration;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

            var port = GlobalConstants.DEFAULT_PORT;

            try
            {
                
                port = int.Parse(Configuration.GetSection("port").Value);
            } catch
            {

            }

            var socketManager = new SocketManager();
            var commandHandler = new CommandHandler(socketManager);
            var socketServer = new CustomSocketServer(commandHandler, socketManager, port);
            socketServer.Start();
        }
    }
}
