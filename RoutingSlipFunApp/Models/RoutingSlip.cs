using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingSlipFunApp.Models
{
    public class RoutingSlip<T>
    {
        public List<Step> Steps { get; set; }

        public T Data { get; set; }
    }
}
