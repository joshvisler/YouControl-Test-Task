using Server.Constants;
using Server.Models;
using SockNet.ServerSocket;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Services
{
    public class CustomSocketServer
    {
        private readonly SocketServer _socketServer;
        private readonly CommandHandler _commandHandler;
        private readonly SocketManager _socketManager;

        public CustomSocketServer(CommandHandler commandHandler, SocketManager socketManager, int port)
        {
            _commandHandler = commandHandler;
            _socketManager = socketManager;
            _socketServer = new SocketServer();
            _socketServer.InitializeSocketServer("127.0.0.1", port);
            _socketServer.SetReaderBufferBytes(1024);
        }

        public void Start()
        {
            _socketServer.StartListening();

            bool openServer = true;
            Timer timer = new Timer(CheckSocketsAsync, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            Console.WriteLine($"Server Started {_socketServer.Ip}:{_socketServer.Port}!");

            while (openServer)
            {
                if (_socketServer.IsNewData())
                {
                    var data = _socketServer.GetData();
                    if (data.Value.Length > 0)
                    {
                        Task.Run(() => Handle(data));
                    }
                }
            }

            _socketServer.CloseServer();
        }

        public void Stop()
        {
            _socketServer.CloseServer();
        }

        public Task SendAsync(TcpClient client, string message)
        {
            return Task.Run(async() => 
            {
                await _socketServer.ResponseToClient(client, $"{message}\r\n");
            });
        }

        private void CheckSocketsAsync(object obj)
        {
            var clients = _socketServer.GetClients();
            var pingTasks = new List<Task>();

            
            foreach (var client in clients)
            {
                pingTasks.Add(_socketServer.ResponseToClient(client.Value, " "));
            }

            try
            {
                Task.WaitAll(pingTasks.ToArray());
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            _socketManager.CheckConnections(clients);
        }

        private void Handle(KeyValuePair<TcpClient, byte[]> data)
        {
            Connection conection;
            var client = _socketServer.GetCurrentClient();

            if (_socketManager.IsNewConnection(_socketServer.GetCurrentClientId(), client.Client, out conection))
            {
                SendAsync(data.Key, GlobalConstants.HELLO_MESSAGE);
                Console.WriteLine($"New user connected! {conection.Ip}");
            } else
            {
                var command = Encoding.UTF8.GetString(data.Value);

                if (command == "\r\n")
                {
                    return;
                }

                var message = _commandHandler.Handle(conection, command);
                SendAsync(client, message);
            }
        }
    }
}
