using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsageMonitoringTool_WebApplication.Models
{
    [Table("mst_3dtrim_data")]
    public class TrimData
    {
        [Key]
        public long SrNo { get; set; }
        public string UserID { get; set; }
        public DateTime StartTime { get; set; }
        public string Region { get; set; }
        public string MachineID { get; set; }
        public string Domain { get; set; }
        public string CadTool { get; set; }
        public string ProductLine { get; set; }
        public string IsProd { get; set; }
        public string IsVDI { get; set; }
        public string ProductionDrive { get; set; }        
        public string BodyCount { get; set; }
        public DateTime StopTime { get; set; }

        public TrimData()

        {

        }
    }
}