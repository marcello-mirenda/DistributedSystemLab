using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RoutingSlipWebApp.Services
{
    public class CommandResolver
    {
        private readonly ILogger<CommandResolver> _logger;

        private readonly Dictionary<string, Func<ICommand>> _dictionary = new Dictionary<string, Func<ICommand>>();

        public CommandResolver(ILogger<CommandResolver> logger, ILogger<ICommand> commandLogger, ServicesResolver servicesResolver)
        {
            _logger = logger;
            _dictionary.Add("Default", () => new DefaultCommand(commandLogger));
            _dictionary.Add($"{nameof(NewOrderCommand).Replace("Command", "")}", () => new NewOrderCommand(commandLogger, servicesResolver));
        }

        public ICommand Lookup(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                return _dictionary["Default"]();
            }

            return _dictionary[cmd]();
        }
    }
}
