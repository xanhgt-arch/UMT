using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UsageMonitoringTool_WebApplication.Models
{
    public class SectionManagerReportData
    {
        public string UserID { get; set; }
        public DateTime StartTime { get; set; }
        public string Time { get; set; }
        public string Region { get; set; }
        public string MachineID { get; set; }
        public string Domain { get; set; }
        public string CadTool { get; set; }
        public string Productline { get; set; }
        public string IsProd { get; set; }
        public string IsVDI { get; set; }
        public string ProductionDrive { get; set; }
        public string Functionality { get; set; }
        public string ViewCount { get; set; }
        public DateTime StopTime { get; set; }
    }
}