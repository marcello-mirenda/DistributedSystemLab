﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingSlipWebApp.Services
{
    public interface ICommand
    {
        Task ExecuteAsync();
    }
}
