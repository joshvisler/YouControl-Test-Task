using Server.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server.Services
{
    public class SocketManager
    {
        private List<Connection> _connections = new List<Connection>();

        public void Add(Connection connection)
        {
            _connections.Add(connection);
            Console.WriteLine("WebSocketServerConnectionManager-> AddSocket: WebSocket added with ID: " + connection.Id);
        }

        public bool IsNewConnection(Guid clientId, Socket socket, out Connection connection)
        {
            connection = _connections.FirstOrDefault(x => x.Id == clientId);
            
            var isNew = connection == null;

            if (isNew)
            {
                var ip = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
                connection = new Connection(clientId, ip);
                Add(connection);
            }

            return isNew;
        }

        public List<Connection> GetAllSockets()
        {
            return _connections;
        }

        public Task RemoveAsync(Guid connectionId)
        {
            return Task.Factory.StartNew(() =>
            {
                var connection = _connections.FirstOrDefault(x => x.Id == connectionId);

                if (connection != null)
                {
                    _connections.Remove(connection);
                    Console.WriteLine($"Connection Closed {connection.Ip}");
                }
            });
        }

        public void CheckConnections(ConcurrentDictionary<Guid, TcpClient> clients)
        {
            var connections = clients.Where(x => !x.Value.Client.Connected);
            var removeTasks = new List<Task>();

            foreach (var connection in connections)
            {
                removeTasks.Add(RemoveAsync(connection.Key));

                TcpClient client;
                clients.Remove(connection.Key, out client);
                connection.Value.Dispose();
            }

            Task.WaitAll(removeTasks.ToArray());
        }
    }
}
