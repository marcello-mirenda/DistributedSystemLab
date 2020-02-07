using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RoutingSlipWebApp.Services
{
    public class DefaultCommand : ICommand
    {
        private readonly ILogger<ICommand> _logger;

        public DefaultCommand(ILogger<ICommand> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync()
        {
            _logger.LogInformation($"{nameof(DefaultCommand)} executed.");
            return Task.FromResult(0);
        }
    }
}
