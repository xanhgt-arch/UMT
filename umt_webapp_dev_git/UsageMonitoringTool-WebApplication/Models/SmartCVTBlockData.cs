using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsageMonitoringTool_WebApplication.Models
{
    [Table("mst_smartcvt_data")]
    public class SmartCVTBlockData
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
        public string PartNumber { get; set; }
        public string Diameter { get; set; }
        public string Centerline { get; set; }
        public string SmoothBlock { get; set; }
        public string ConvoluteBlock { get; set; }
        public string ComboBlock { get; set; }
        public string SmoothWithBead { get; set; }
        public string ComboWithBead { get; set; }
        public string ExpansionBlock { get; set; }
        public string PartialBlock { get; set; }        
        public string BlockPlacement { get; set; }
        public DateTime StopTime { get; set; }        
       
        public SmartCVTBlockData()
        {

        }
    }
}