﻿using System;
using System.Collections.Generic;
using System.Text;

namespace InvoicingFuncApp
{
    public class StaleStatus
    {
        public string Id { get; set; }
        public string PartitionKey { get; set; }
        public string Status { get; set; }
    }
}
