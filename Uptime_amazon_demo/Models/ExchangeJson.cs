using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uptime_demo.Models
{
    public class ExchangeJson
    {
        public string Disclaimer { get; set; }
        public string License { get; set; }
        public string Timestamp { get; set; }
        public string Base { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}