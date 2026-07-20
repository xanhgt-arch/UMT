using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsageMonitoringTool_WebApplication.Models
{
    [Table("mst_profilechecker_data")]
    public class ProfileCheckerData
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
        public string OverlapCheck { get; set; }
        public string DoubleLineCheck { get; set; }
        public string GapCheck { get; set; }
        public string PlaneCheck { get; set; }
        public string SplineCheck { get; set; }
        public string TangencyCheck { get; set; }
        public string TinySegmentCheck { get; set; }        
        public DateTime StopTime { get; set; }

        public ProfileCheckerData()

        {

        }
    }
}