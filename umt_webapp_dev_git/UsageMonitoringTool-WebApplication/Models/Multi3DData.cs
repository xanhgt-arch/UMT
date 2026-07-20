using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsageMonitoringTool_WebApplication.Models
{
    [Table("mst_multi3d_data")]
    public class Multi3DData
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
        public string Functionality { get; set; }
        public string ObjectCount { get; set; }        
        public DateTime StopTime { get; set; }

        public Multi3DData()

        {

        }
    }
}