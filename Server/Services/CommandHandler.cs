using Server.Constants;
using Server.Models;
using System;
using System.Linq;

namespace Server.Services
{
    public class CommandHandler
    {
        private readonly SocketManager _socketManager;

        public CommandHandler(SocketManager socketManager)
        {
            _socketManager = socketManager;
        }

        public string Handle(Connection conection, string command)
        {
            if (command == GlobalConstants.LIST_COMMAND)
            {
                return string.Join("\r\n", _socketManager.GetAllSockets().Select(x => x.ToString()));
            }

            int number;

            if (int.TryParse(command, out number))
            {
                conection.AddCommand(number);

                try
                {
                    return $"{conection.Commands.Sum()}";
                } catch (OverflowException)
                {
                    return $"{GlobalConstants.NUMBER_LIMIT_ERROR} {int.MaxValue}";
                }

            } else
            {
                return GlobalConstants.PLS_ENTER_NUMBER_OR_COMMAND;
            }
        }
    }
}
