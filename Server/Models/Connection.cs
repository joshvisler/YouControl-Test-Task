using Server.Constants;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Server.Models
{
    public class Connection
    {
        public Guid Id { get; }
        public ConcurrentStack<int> Commands { get; }
        public string Ip { get; }

        public Connection(Guid connectionId, string ip)
        {
            Id = connectionId;
            Ip = ip;
            Commands = new ConcurrentStack<int>();
        }

        public void AddCommand(int command)
        {
            Commands.Push(command);
        }

        public override string ToString()
        {
            try
            {
                var sum = Commands.Sum();
                return $"IP: {Ip} Sum: {sum}";
            }
            catch (OverflowException)
            {
                return $"IP: {Ip} Sum: {int.MaxValue}";
            }
        }
    }
}
