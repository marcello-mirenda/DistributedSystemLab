using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RoutingSlipWebApp.Models;

namespace RoutingSlipWebApp.Services
{
    public class NewOrderCommand : ICommand
    {
        private readonly ILogger<ICommand> _logger;
        private readonly ServicesResolver _servicesResolver;

        public NewOrderCommand(ILogger<ICommand> logger, ServicesResolver servicesResolver)
        {
            _servicesResolver = servicesResolver;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            var newOrder = new Order
            {
                Id = Guid.NewGuid().ToString()
            };
            await _servicesResolver.Resolve<NewOrder>(newOrder).RunAsync();

            _logger.LogInformation($"{nameof(NewOrderCommand)} executed.");
        }
    }
}
