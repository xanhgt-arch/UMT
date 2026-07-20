using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UsageMonitoringTool_WebApplication.Models
{
    public class FuncData
    {
        public string ApplicationName { get; set; }
        public string[] FuncTitle { get; set; }
        public long[] FuncValue { get; set; }
        public bool IsSelected { get; set; }
    }
}