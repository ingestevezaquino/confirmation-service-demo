﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSConfIPTV.Services.ClientResponse
{
    public class Facility
    {
        public string SubscriberNumber { get; set; } = null!;
        public string NodeAddress { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
        public string NodeName { get; set; } = null!;
    }
}
