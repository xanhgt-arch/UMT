using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsageMonitoringTool_WebApplication.Models
{
    [Table("mst_tool_usage")]
    public class ToolTrackingDetails
    {
        [Key]
        public long SrNo { get; set; }
        public string ApplicationName { get; set; }
        public string Functionality { get; set; }
        public DateTime StartTime { get; set; }
        public string UserID { get; set; }
        public string MachineID { get; set; }
        public DateTime StopTime { get; set; }
        public string Region { get; set; }
        public string CadTool { get; set; }
        public string Status { get; set; }
        public string Domain { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ProductLine { get; set; }
        public bool? IsProd { get; set; }
        public bool? IsVDI { get; set; }
        public string CustomerName { get; set; }
        public string CadLaunchPath { get; set; }
        public ToolTrackingDetails()
        {

        }
    }
}