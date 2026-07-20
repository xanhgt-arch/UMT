using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UsageMonitoringTool_WebApplication.Models
{
    public class BasicChartData
    {
        public string title { get; set; }
        public long value { get; set; }
        public string customToolTip { get; set; }
    }
}