using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MySql.Data.EntityFramework;
using System.Data.Common;

namespace UsageMonitoringTool_WebApplication.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class UMTContext:DbContext
    {
        public DbSet<ToolTrackingDetails> ToolTracking { get; set; }
        public DbSet<VDIUserDetail> VDIUserDetail { get; set; }
        public DbSet<SmartCVTBlockData> SmartCVTTracking { get; set; }
        public DbSet<PointChartData> PointChartTracking { get; set; }
        public DbSet<ProfileCheckerData> ProfileCheckerTracking { get; set; }
        public DbSet<NamingToolData> NamingToolTracking { get; set; }
        public DbSet<Multi3DData> Multi3DTracking { get; set; }

        public DbSet<SectionManagerData> SectionManagerTracking { get; set; }
        public DbSet<TrimData> TrimTracking { get; set; }
        public DbSet<DomainDetails> DomainDetails { get; set; }
        public DbSet<CooperAdmins> CooperAdmins { get; set; }

        public UMTContext(): base("UsageMonitoringContext")
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           // modelBuilder.Entity<UMTContext>().MapToStoredProcedures();
        }
    }
}
