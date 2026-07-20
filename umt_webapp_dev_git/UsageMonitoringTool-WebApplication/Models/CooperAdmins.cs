using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UsageMonitoringTool_WebApplication.Models
{
    [Table("mst_cooper_admins")]
    public class CooperAdmins
    {
        [Key]
        public long SrNo { get; set; }
        public String UserId { get; set; } 
        public String AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
    }
}