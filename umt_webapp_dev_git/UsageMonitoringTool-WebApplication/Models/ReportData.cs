using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UsageMonitoringTool_WebApplication.Models
{
    public class ReportData
    {
        public string ApplicationName { get; set; }
        public string Functionality { get; set; }
        public string cadTool { get; set; }
        public string Domain { get; set; }
        public string UserID { get; set; }
        public string MachineID { get; set; }
        public string ProductLine { get; set; }
        public string Status { get; set; }
        public string Region { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public bool? isProd { get; set; }
        public bool? isVDI { get; set; }
        public string CustomerName { get; set; }

    }
}