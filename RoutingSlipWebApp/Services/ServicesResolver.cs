using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoutingSlipWebApp.Models;

namespace RoutingSlipWebApp.Services
{
    public class ServicesResolver
    {
        private readonly Dictionary<Type, Type> _services;

        public ServicesResolver()
        {
            _services = new Dictionary<Type, Type>
            {
                { typeof(NewOrder), typeof(NewOrderService) }
            };
        }

        public IService Resolve<T>(object data)
        {
            var serviceType = _services[typeof(T)];
            var ctor = serviceType.GetConstructor(new Type[] { data.GetType() });
            return ctor.Invoke(new object[] { data }) as IService;
        }
    }
}
