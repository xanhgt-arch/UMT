using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsageMonitoringTool_WebApplication.Models
{
    [Table("mst_pointchart_data")]
    public class PointChartData
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
        public string ChartType { get; set; }
        public string PointCount { get; set; }
        public string ViewCount { get; set; }
        public string LeaderCount { get; set; }
        public string BendAngle { get; set; }
        public string CuffLength { get; set; }        
        public string SegmentLength { get; set; }
        public string EndForm { get; set; }
        public string HoseDetails { get; set; }
        public string ReverseDirection { get; set; }
        public DateTime StopTime { get; set; }

        public PointChartData()        
       
      {

        }
    }
}