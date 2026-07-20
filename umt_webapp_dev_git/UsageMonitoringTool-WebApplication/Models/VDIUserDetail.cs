using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UsageMonitoringTool_WebApplication.Models
{
    [Table("mst_vdi_user_detail")]
    public class VDIUserDetail
    {
        [Key]
        public long SrNo { get; set; }
        public String UserId { get; set; }
        public String Region { get; set; }
        public String Domain { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}