using System;
using System.Collections.Generic;

namespace UsageMonitoringTool_WebApplication.Models
{
    public class BasicAppData
    {
        public List<string> funcNames { get; set; }
        public List< Dictionary<string, dynamic>> data { get; set; }
    }
}