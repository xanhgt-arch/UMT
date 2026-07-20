using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UsageMonitoringTool_WebApplication.Models;
using System.Xml;
using Microsoft.VisualBasic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace UsageMonitoringTool_WebApplication.Controllers
{
    public class ChartsController : Controller
    {
        // GET: Charts
        public ActionResult Index()
        {
            var x = User.IsInRole("USERS");

            using (var ctx = new UMTContext())
            {
                var data = ctx.ToolTracking.ToList();
            }
            return View();
        }


        private static readonly Dictionary<string, string> DomainMapping =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "ATIBAIA",    "Atibaia" },
                { "ASIA",       "Asia" },
                { "AUBURN",     "AUBURN" },
                { "BATTIPAGLIA","Battipaglia" },
                { "CHONGQING",  "Chongqing" },
                { "COVENTRY",   "Coventry" },
                { "CSAID",      "CSAID" },
                { "ITALY",      "Italy" },
                { "KOREA",      "Korea" },
                { "KUNSHAN",    "Kunshan" },
                { "LINDAU",     "Lindau" },
                { "MANNHEIM",   "Mannheim" },
                { "MERGON",     "Mergon" },
                { "MTC",        "MTC" },
                { "MYCORP",     "MYCORP" },
                { "NORTHVILLE", "Northville" },
                { "POLAND",     "Poland" },
                { "SERBIA",     "Serbia" },
                { "SHANGHAI",   "Shanghai" },
                { "SPAIN",      "Spain" },
                { "TATA",       "TATA" },      // Change to "TATA" if it must remain uppercase
                { "VITRE",      "Vitre" },
                { "YOKOHAMA",   "Yokohama" },

            };


        public ContentResult Rawdata(string[] Pc, string[] Region, string[] CadApps, string[] App,string[] Hardware,string[] Domain, DateTime fromdate, DateTime todate)
     {


            using (var ctx = new UMTContext())
            {
                try
                {
                    string selVal_Pc = string.Join(",", Pc).Replace(",,", ",");
                    string selVal_App = string.Join(",", App).Replace(",,",",");
                    string selVal_CadApps = string.Join(",", CadApps).Replace(",,", ",");
                    string selVal_Region = string.Join(",", Region).Replace(",,", ",");
                    string selVal_Hardware = string.Join(",", Hardware).Replace(",,", ",");
                    string selVal_Domain= string.Join(",", Domain).Replace(",,", ",");

                    var data = ctx.ToolTracking.Where(c => c.SrNo > 0);                    
                    List<ToolTrackingDetails> lstAppFilter = new List<ToolTrackingDetails>();
                    List<ToolTrackingDetails> lstPcFilter = new List<ToolTrackingDetails>();
                    List<ToolTrackingDetails> lstRegionFilter = new List<ToolTrackingDetails>();
                    List<ToolTrackingDetails> lstCadAppsFilter = new List<ToolTrackingDetails>();
                    List<ToolTrackingDetails> lstHardwareFilter = new List<ToolTrackingDetails>();
                    List<ToolTrackingDetails> lstDomainFilter = new List<ToolTrackingDetails>();
                    List<ToolTrackingDetails> lstDateFilter = new List<ToolTrackingDetails>();
                    bool nullValue = false;
                    bool anyfilter = false;
                    List<string> lst_app = new List<string>();
                    List<string> lst_Pc = new List<string>();
                    List<string> lst_Region = new List<string>();
                    List<string> lst_Domain = new List<string>();
                    List<string> lst_Hardware = new List<string>();
                    List<string> lst_CadApps = new List<string>();

                    if (!selVal_App.Contains("ALL"))
                    {
                        anyfilter = true;
                        string[] sv_App = selVal_App.Split(',');
                        lst_app = sv_App.ToList();
                        foreach (var x in lst_app)
                        {
                            lstAppFilter.AddRange(data.Where(c => c.ApplicationName == x));
                        }
                        if (lstAppFilter.Count == 0)
                        {
                            nullValue = true;
                        }
                    }
                    if (!selVal_Pc.Contains("ALL") && nullValue == false)
                    {
                        anyfilter = true;
                        string[] sv_Pc = selVal_Pc.Split(',');
                        lst_Pc = sv_Pc.ToList();
                        foreach (var x in lst_Pc)
                        {
                            if (lstAppFilter.Count == 0)
                            {
                                if (x.ToUpper() == "FLUIDS")
                                {
                                    lstPcFilter.AddRange(data.Where(c => c.ProductLine == "FTS" || c.ProductLine == "FBD" || c.ProductLine == "FLUIDS"));
                                }
                                else
                                {
                                    lstPcFilter.AddRange(data.Where(c => c.ProductLine == x));
                                }
                            }
                            else
                            {
                                if (x.ToUpper() == "FLUIDS")
                                {
                                    lstPcFilter.AddRange(lstAppFilter.Where(c => c.ProductLine == "FTS" || c.ProductLine == "FBD" || c.ProductLine == "FLUIDS"));
                                }
                                else
                                {
                                    lstPcFilter.AddRange(lstAppFilter.Where(c => c.ProductLine == x));
                                }
                            }
                        }
                        if (lstPcFilter.Count == 0)
                        {
                            nullValue = true;
                        }
                    }
                    else if (lstAppFilter.Count > 0)
                    {
                        lstPcFilter = lstAppFilter;
                    }
                    if (!selVal_Region.Contains("ALL") && nullValue == false)
                    {
                        anyfilter = true;
                        string[] sv_Region = selVal_Region.Split(',');
                        lst_Region = sv_Region.ToList();
                        foreach (var x in lst_Region)
                        {
                            if (lstPcFilter.Count == 0)
                            {
                                lstRegionFilter.AddRange(data.Where(c => c.Region == x));
                            }
                            else
                            {
                                lstRegionFilter.AddRange(lstPcFilter.Where(c => c.Region == x));
                            }
                        }
                        if (lstRegionFilter.Count == 0)
                        {
                            nullValue = true;
                        }
                    }
                    else if (lstPcFilter.Count > 0)
                    {
                        lstRegionFilter = lstPcFilter;
                    }
                    if (!selVal_CadApps.Contains("ALL") && nullValue == false)
                    {
                        anyfilter = true;
                        string[] sv_CadApps = selVal_CadApps.Split(',');
                        lst_CadApps = sv_CadApps.ToList();
                        foreach (var x in lst_CadApps)
                        {
                            if (lstRegionFilter.Count == 0)
                            {
                                lstCadAppsFilter.AddRange(data.Where(c => c.CadTool == x));
                            }
                            else
                            {
                                lstCadAppsFilter.AddRange(lstRegionFilter.Where(c => c.CadTool == x));
                            }
                        }
                        if (lstCadAppsFilter.Count == 0)
                        {
                            nullValue = true;
                        }
                    }
                    else if (lstRegionFilter.Count > 0)
                    {
                        lstCadAppsFilter = lstRegionFilter;
                    }
                    if (!selVal_Hardware.Contains("ALL") && nullValue == false)
                    {
                        anyfilter = true;
                        string[] sv_Hardware = selVal_Hardware.Split(',');
                        lst_Hardware = sv_Hardware.ToList();
                        foreach (var x in lst_Hardware)
                        {
                            if (lstCadAppsFilter.Count == 0)
                            {
                                if (x.ToUpper() == "VDI")
                                {
                                    lstHardwareFilter.AddRange(data.Where(c => c.IsVDI == true));
                                }
                                if (x.ToUpper() == "NON-VDI")
                                {
                                    lstHardwareFilter.AddRange(data.Where(c => c.IsVDI == false));
                                }
                            }
                            else
                            {
                                if (x.ToUpper() == "VDI")
                                {
                                    lstHardwareFilter.AddRange(lstCadAppsFilter.Where(c => c.IsVDI == true));
                                }
                                if (x.ToUpper() == "NON-VDI")
                                {
                                    lstHardwareFilter.AddRange(lstCadAppsFilter.Where(c => c.IsVDI == false));
                                }
                            }
                        }
                    }
                    else if (lstCadAppsFilter.Count > 0)
                    {
                        lstHardwareFilter = lstCadAppsFilter;
                    }
                    if (!selVal_Domain.Contains("ALL") && nullValue == false)
                    {
                        anyfilter = true;
                        string[] sv_Domain = selVal_Domain.Split(',');
                        lst_Domain = sv_Domain.ToList();
                        foreach (var x in lst_Domain)
                        {
                            if (lstHardwareFilter.Count == 0)
                            {
                                lstDomainFilter.AddRange(data.Where(c => c.Domain == x));
                                //if (x.ToUpper() == "TATA")
                                //{
                                //    lstDomainFilter.AddRange(data.Where(c => c.Domain == "MYCORP"));
                                //}
                                //else if (x.ToUpper() == "COOPER")
                                //{
                                //    lstDomainFilter.AddRange(data.Where(c => c.Domain == "CSAID" || c.Domain == "AUBURN" || c.Domain == "ASIA"));
                                //}
                                //else if (x.ToUpper() == "MERGON")
                                //{
                                //    lstDomainFilter.AddRange(data.Where(c => c.Domain == "MERGON"));
                                //}
                                //else if (x.ToUpper() == "MTC")
                                //{
                                //    lstDomainFilter.AddRange(data.Where(c => c.Domain == "MTC"));
                                //}
                            }
                            else
                            {
                                lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == x));
                                //if (x.ToUpper() == "TATA")
                                //{
                                //    lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == "MYCORP"));
                                //}
                                //else if (x.ToUpper() == "COOPER")
                                //{
                                //    lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == "CSAID" || c.Domain == "AUBURN" || c.Domain == "ASIA"));
                                //}
                                //else if (x.ToUpper() == "MERGON")
                                //{
                                //    lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == "MERGON"));
                                //}
                                //else if (x.ToUpper() == "MTC")
                                //{
                                //    lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == "MTC"));
                                //}
                            }
                        }
                        if (lstDomainFilter.Count == 0)
                        {
                            nullValue = true;
                        }
                    }
                    else if (lstHardwareFilter.Count > 0)
                    {
                        lstDomainFilter = lstHardwareFilter;
                    }
                    if (fromdate > Convert.ToDateTime("2019-01-01") && todate > Convert.ToDateTime("2019-01-01") && nullValue == false)
                    {
                        anyfilter = true;
                        if (lstDomainFilter.Count == 0)
                        {
                            todate = todate.AddDays(1);
                            lstDateFilter.AddRange(data.Where(c => c.StartTime > fromdate && c.StartTime <= todate));
                        }
                        else
                        {
                            todate = todate.AddDays(1);
                            lstDateFilter.AddRange(lstDomainFilter.Where(c => c.StartTime > fromdate && c.StartTime <= todate));
                        }
                    }

                    var tempList =anyfilter==false? data.ToList(): lstDateFilter.ToList();
                    var sortedByDate = tempList.OrderBy(d => d.StartTime).ToList();
                    List<ReportData> data2 = sortedByDate.Select(x => new ReportData { ApplicationName = x.ApplicationName, Functionality=x.Functionality, cadTool = x.CadTool, Domain = x.Domain, UserID = x.UserID, MachineID = x.MachineID, ProductLine = x.ProductLine,
                        Status = x.Status, Region = x.Region, StartTime = Convert.ToDateTime(x.StartTime), StopTime = Convert.ToDateTime(x.StopTime),
                        isProd = x.IsProd, isVDI = x.IsVDI, CustomerName = x.CustomerName
                    }).ToList();  
                    List<ReportData> FinalList = new List<ReportData>();
                    string[] removelist = { "COMBO BLOCK", "SMOOTH BLOCK", "CONVOLUTE BLOCK", "BLOCK CREATION", "EXECUTION", "CREATE BLOCK", "SMOOTH BORE" };
                                     
                    string xmlpath = "";
                    string newname = string.Empty;
                    string[] oldApps = { "CLIPS AND ADAPTORS", "HIT", "FLANGE THICKNESS","TEST_APP" };
                    XmlDocument xmlfile = new XmlDocument();
                    System.Xml.XmlElement nodesfrmConfig;
                    xmlpath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                    if (System.IO.File.Exists(xmlpath))
                    {
                        xmlfile.Load(xmlpath);
                    }                    
                    string woSpace = string.Empty;                    
                    foreach (var b in data2)
                    {
                        woSpace = b.ApplicationName.Replace(" ", "").Replace("_", "").ToUpper();
                        nodesfrmConfig = xmlfile.SelectSingleNode("/configuration/myCustomElement") as XmlElement;
                        XmlNodeList elemlst = nodesfrmConfig.GetElementsByTagName(woSpace);                        
                        if(elemlst != null && elemlst.Count > 0 && elemlst.Item(0).InnerText == "SMART CVT" && removelist.Contains(b.Functionality.ToUpper()))
                        {
                            continue;
                        }
                        if(oldApps.Contains(b.ApplicationName.ToUpper()))
                        {
                            continue;
                        }
                        if (elemlst != null && elemlst.Count > 0)
                        {
                            b.ApplicationName = elemlst.Item(0).InnerText;
                            b.Functionality = b.Functionality.ToUpper();
                        }
                        else
                        {
                            b.ApplicationName = b.ApplicationName.ToUpper();
                            b.Functionality = b.Functionality.ToUpper();
                        }
                        //Change Fluid
                        if (b.ProductLine=="FTS"||b.ProductLine=="FBD")
                        {
                            b.ProductLine = "FLUIDS";
                        }

                        //Domain normalization
                        if (!string.IsNullOrWhiteSpace(b.Domain))
                        {
                            string domainKey = b.Domain.Trim().ToUpper();

                            if (DomainMapping.ContainsKey(domainKey))
                            {
                                b.Domain = DomainMapping[domainKey];
                            }
                            else
                            {
                                b.Domain = domainKey; // fallback
                            }
                        }

                        woSpace = b.ApplicationName.Replace(" ", "").Replace("_", "").ToUpper();


                        FinalList.Add(b);                       

                    }



                    var serializer = new JavaScriptSerializer();             
                    serializer.MaxJsonLength = Int32.MaxValue;                                    
                    var result = new ContentResult
                    {
                        Content = serializer.Serialize(FinalList),
                        ContentType = "application/json"
                    };
                    return result;

              //      return Json(data2, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    var serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    var d = ex.ToString();
                    return new ContentResult {
                        Content = serializer.Serialize(d),
                        ContentType = "application/json"
                };
                    //return Json(d, JsonRequestBehavior.AllowGet);
                }
            }
     }

        public ContentResult AppData(string App, string[] CadApps,string[] DataCategory, DateTime fromdate, DateTime todate)
        {


            using (var ctx = new UMTContext())
            {
                try
                {
                    string Dummy = "NotSelected";
                    //bool anyfilter = false;
                    List<SmartCVTReportData> SmartCVTData = new List<SmartCVTReportData>();
                    List<PointChartReportData> PointChartData = new List<PointChartReportData>();
                    List<ProfileCheckerReportData> ProfileCheckerData = new List<ProfileCheckerReportData>();
                    List<NamingToolReportData> NamingToolData = new List<NamingToolReportData>();
                    List<Multi3DReportData> Multi3DData = new List<Multi3DReportData>();
                    List<SectionManagerReportData> SectionManagerData = new List<SectionManagerReportData>();
                    List<TrimReportData> TrimData = new List<TrimReportData>();
                    //List<SmartCVTBlockData> SmartCVTData1 = new List<SmartCVTBlockData>();
                    //List<SmartCVTBlockData> SmartCVTData2 = new List<SmartCVTBlockData>();                    
                    if (App=="SMART CVT")
                    {
                        var data = ctx.SmartCVTTracking.Where(c => c.SrNo > 0);
                        if (fromdate > Convert.ToDateTime("2023-01-01"))
                        {
                            data = data.Where(c => c.StartTime > fromdate);
                        }                        
                        if (todate > Convert.ToDateTime("2023-01-01"))
                        {
                            todate = todate.AddDays(1);
                            data = data.Where(c => c.StartTime <= todate);
                        }     
                        if (CadApps.Length!=3)
                        {
                            string selCad = CadApps[0];
                            data = data.Where(c => c.CadTool.ToUpper() == selCad.ToUpper());
                        }
                        if (DataCategory.Length != 3)
                        {
                            string selData = DataCategory[0];
                            if (selData.ToUpper()=="PRODUCTION DATA")
                            {
                                data = data.Where(c => c.ProductionDrive.ToUpper() == "TRUE");
                            }
                            if (selData.ToUpper()=="TEST DATA")
                            {
                                data = data.Where(c => c.ProductionDrive.ToUpper() == "FALSE");
                            }                            
                        }
                        var tempList =  data.ToList();
                        List<SmartCVTReportData> data2 = tempList.Select(x => new SmartCVTReportData
                        {
                            UserID = x.UserID,
                            StartTime = Convert.ToDateTime(x.StartTime),
                            Time = x.StartTime.Hour.ToString() + ":" + x.StartTime.Minute.ToString() + ":" + x.StartTime.Second.ToString(),
                            Region = x.Region,
                            MachineID = x.MachineID,
                            Domain=x.Domain,
                            CadTool = x.CadTool,
                            ProductLine=x.ProductLine,
                            IsProd=x.IsProd,
                            IsVDI=x.IsVDI,
                            PartNumber = x.PartNumber,
                            Diameter = x.Diameter,
                            Centerline = x.Centerline,
                            SmoothBlock = x.SmoothBlock,
                            ConvoluteBlock = x.ConvoluteBlock,
                            ComboBlock = x.ComboBlock,
                            SmoothWithBead = x.SmoothWithBead,
                            ComboWithBead = x.ComboWithBead,
                            ExpansionBlock = x.ExpansionBlock,
                            PartialBlock = x.PartialBlock,
                            BlockPlacement = x.BlockPlacement,
                            ProductionDrive=x.ProductionDrive,
                            StopTime = Convert.ToDateTime(x.StopTime)
                            
                        }).ToList();
                        SmartCVTData = data2;
                    }
                    else if(App== "POINT CHART")
                    {
                        var data = ctx.PointChartTracking.Where(c => c.SrNo > 0);
                        if (fromdate > Convert.ToDateTime("2023-01-01"))
                        {
                            data = data.Where(c => c.StartTime > fromdate);
                        }
                        if (todate > Convert.ToDateTime("2023-01-01"))
                        {
                            todate = todate.AddDays(1);
                            data = data.Where(c => c.StartTime <= todate);
                        }
                        if (CadApps.Length != 3)
                        {
                            string selCad = CadApps[0];
                            data = data.Where(c => c.CadTool.ToUpper() == selCad.ToUpper());
                        }
                        if (DataCategory.Length != 3)
                        {
                            string selData = DataCategory[0];
                            if (selData.ToUpper() == "PRODUCTION DATA")
                            {
                                data = data.Where(c => c.ProductionDrive.ToUpper() == "TRUE");
                            }
                            if (selData.ToUpper() == "TEST DATA")
                            {
                                data = data.Where(c => c.ProductionDrive.ToUpper() == "FALSE");
                            }
                        }
                        var tempList = data.ToList();
                        List<PointChartReportData> data2 = tempList.Select(x => new PointChartReportData
                        {
                            UserID = x.UserID,
                            StartTime = Convert.ToDateTime(x.StartTime),
                            Time = x.StartTime.Hour.ToString() + ":" + x.StartTime.Minute.ToString() + ":" + x.StartTime.Second.ToString(),
                            Region = x.Region,
                            MachineID = x.MachineID,
                            Domain = x.Domain,
                            CadTool = x.CadTool,
                            Productline = x.ProductLine,
                            IsProd = x.IsProd,
                            IsVDI = x.IsVDI,
                            ProductionDrive = x.ProductionDrive,
                            Functionality = x.Functionality,
                            ChartType = x.ChartType,
                            PointCount = x.PointCount,
                            ViewCount = x.ViewCount,
                            LeaderCount = x.LeaderCount,
                            BendAngle = x.BendAngle,
                            CuffLength = x.CuffLength,
                            SegmentLength = x.SegmentLength,
                            EndForm = x.EndForm,
                            HoseDetails = x.HoseDetails,
                            ReverseDirection = x.ReverseDirection,
                            StopTime = Convert.ToDateTime(x.StopTime)

                        }).ToList();
                        PointChartData = data2;
                    }
                    else if (App == "PROFILE CHECKER")
                    {
                        var data = ctx.ProfileCheckerTracking.Where(c => c.SrNo > 0);
                        if (fromdate > Convert.ToDateTime("2023-01-01"))
                        {
                            data = data.Where(c => c.StartTime > fromdate);
                        }
                        if (todate > Convert.ToDateTime("2023-01-01"))
                        {
                            todate = todate.AddDays(1);
                            data = data.Where(c => c.StartTime <= todate);
                        }
                        if (CadApps.Length != 3)
                        {
                            string selCad = CadApps[0];
                            data = data.Where(c => c.CadTool.ToUpper() == selCad.ToUpper());
                        }
                        if (DataCategory.Length != 3)
                        {
                            string selData = DataCategory[0];
                            if (selData.ToUpper() == "PRODUCTION DATA")
                            {
                                data = data.Where(c => c.ProductionDrive.ToUpper() == "TRUE");
                            }
                            if (selData.ToUpper() == "TEST DATA")
                            {
                                data = data.Where(c => c.ProductionDrive.ToUpper() == "FALSE");
                            }
                        }
                        var tempList = data.ToList();
                        List<ProfileCheckerReportData> data2 = tempList.Select(x => new ProfileCheckerReportData
                        {
                            UserID = x.UserID,
                            StartTime = Convert.ToDateTime(x.StartTime),
                            Time = x.StartTime.Hour.ToString() + ":" + x.StartTime.Minute.ToString() + ":" + x.StartTime.Second.ToString(),
                            Region = x.Region,
                            MachineID = x.MachineID,
                            Domain = x.Domain,
                            CadTool = x.CadTool,
                            Productline = x.ProductLine,
                            IsProd = x.IsProd,
                            IsVDI = x.IsVDI,
                            ProductionDrive = x.ProductionDrive,
                            OverlapCheck = x.OverlapCheck,
                            DoubleLineCheck = x.DoubleLineCheck,
                            GapCheck = x.GapCheck,
                            PlaneCheck = x.PlaneCheck,
                            SplineCheck = x.SplineCheck,
                            TangencyCheck = x.TangencyCheck,
                            TinySegmentCheck = x.TinySegmentCheck,                           
                            StopTime = Convert.ToDateTime(x.StopTime)

                        }).ToList();
                        ProfileCheckerData = data2;
                    }
                    else if (App == "FILE MANAGER")
                    {
                        var data = ctx.NamingToolTracking.Where(c => c.SrNo > 0);
                        if (fromdate > Convert.ToDateTime("2023-01-01"))
                        {
                            data = data.Where(c => c.StartTime > fromdate);
                        }
                        if (todate > Convert.ToDateTime("2023-01-01"))
                        {
                            todate = todate.AddDays(1);
                            data = data.Where(c => c.StartTime <= todate);
                        }
                        if (CadApps.Length != 3)
                        {
                            string selCad = CadApps[0];
                            data = data.Where(c => c.CadTool.ToUpper() == selCad.ToUpper());
                        }
                        if (DataCategory.Length != 3)
                        {
                            string selData = DataCategory[0];
                            if (selData.ToUpper() == "PRODUCTION DATA")
                            {
                                data = data.Where(c => c.ProductionDrive.ToUpper() == "TRUE");
                            }
                            if (selData.ToUpper() == "TEST DATA")
                            {
                                data = data.Where(c => c.ProductionDrive.ToUpper() == "FALSE");
                            }
                        }
                        var tempList = data.ToList();
                        List<NamingToolReportData> data2 = tempList.Select(x => new NamingToolReportData
                        {
                            UserID = x.UserID,
                            StartTime = Convert.ToDateTime(x.StartTime),
                            Time = x.StartTime.Hour.ToString() + ":" + x.StartTime.Minute.ToString() + ":" + x.StartTime.Second.ToString(),
                            Region = x.Region,
                            MachineID = x.MachineID,
                            Domain = x.Domain,
                            CadTool = x.CadTool,
                            Productline = x.ProductLine,
                            IsProd = x.IsProd,
                            IsVDI = x.IsVDI,
                            ProductionDrive = x.ProductionDrive,
                            Functionality = x.Functionality,
                            TotalFiles = x.TotalFiles,
                            Success = x.Success,
                            Failed = x.Failed,                            
                            StopTime = Convert.ToDateTime(x.StopTime)

                        }).ToList();
                        NamingToolData = data2;
                    }
                    else if (App == "MULTI 3D")
                    {
                        var data = ctx.Multi3DTracking.Where(c => c.SrNo > 0);
                        if (fromdate > Convert.ToDateTime("2023-01-01"))
                        {
                            data = data.Where(c => c.StartTime > fromdate);
                        }
                        if (todate > Convert.ToDateTime("2023-01-01"))
                        {
                            todate = todate.AddDays(1);
                            data = data.Where(c => c.StartTime <= todate);
                        }
                        if (CadApps.Length != 3)
                        {
                            string selCad = CadApps[0];
                            data = data.Where(c => c.CadTool.ToUpper() == selCad.ToUpper());
                        }
                        if (DataCategory.Length != 3)
                        {
                            string selData = DataCategory[0];
                            if (selData.ToUpper() == "PRODUCTION DATA")
                            {
                                data = data.Where(c => c.ProductionDrive.ToUpper() == "TRUE");
                            }
                            if (selData.ToUpper() == "TEST DATA")
                            {
                                data = data.Where(c => c.ProductionDrive.ToUpper() == "FALSE");
                            }
                        }
                        var tempList = data.ToList();
                        List<Multi3DReportData> data2 = tempList.Select(x => new Multi3DReportData
                        {
                            UserID = x.UserID,
                            StartTime = Convert.ToDateTime(x.StartTime),
                            Time = x.StartTime.Hour.ToString() + ":" + x.StartTime.Minute.ToString() + ":" + x.StartTime.Second.ToString(),
                            Region = x.Region,
                            MachineID = x.MachineID,
                            Domain = x.Domain,
                            CadTool = x.CadTool,
                            Productline = x.ProductLine,
                            IsProd = x.IsProd,
                            IsVDI = x.IsVDI,
                            ProductionDrive = x.ProductionDrive,
                            Functionality = x.Functionality,
                            ObjectCount = x.ObjectCount,                            
                            StopTime = Convert.ToDateTime(x.StopTime)

                        }).ToList();
                        Multi3DData = data2;
                    }
                    else if (App == "SECTION MANAGER")
                    {
                        var data = ctx.SectionManagerTracking.Where(c => c.SrNo > 0);
                        if (fromdate > Convert.ToDateTime("2023-01-01"))
                        {
                            data = data.Where(c => c.StartTime > fromdate);
                        }
                        if (todate > Convert.ToDateTime("2023-01-01"))
                        {
                            todate = todate.AddDays(1);
                            data = data.Where(c => c.StartTime <= todate);
                        }
                        if (CadApps.Length != 3)
                        {
                            string selCad = CadApps[0];
                            data = data.Where(c => c.CadTool.ToUpper() == selCad.ToUpper());
                        }
                        if (DataCategory.Length != 3)
                        {
                            string selData = DataCategory[0];
                            if (selData.ToUpper() == "PRODUCTION DATA")
                            {
                                data = data.Where(c => c.ProductionDrive.ToUpper() == "TRUE");
                            }
                            if (selData.ToUpper() == "TEST DATA")
                            {
                                data = data.Where(c => c.ProductionDrive.ToUpper() == "FALSE");
                            }
                        }
                        var tempList = data.ToList();
                        List<SectionManagerReportData> data2 = tempList.Select(x => new SectionManagerReportData
                        {
                            UserID = x.UserID,
                            StartTime = Convert.ToDateTime(x.StartTime),
                            Time = x.StartTime.Hour.ToString() + ":" + x.StartTime.Minute.ToString() + ":" + x.StartTime.Second.ToString(),
                            Region = x.Region,
                            MachineID = x.MachineID,
                            Domain = x.Domain,
                            CadTool = x.CadTool,
                            Productline = x.ProductLine,
                            IsProd = x.IsProd,
                            IsVDI = x.IsVDI,
                            ProductionDrive = x.ProductionDrive,
                            Functionality = x.Functionality,
                            ViewCount = x.ViewCount,
                            StopTime = Convert.ToDateTime(x.StopTime)

                        }).ToList();
                        SectionManagerData = data2;
                    }
                    else if (App == "3D TRIM")
                    {
                        var data = ctx.TrimTracking.Where(c => c.SrNo > 0);
                        if (fromdate > Convert.ToDateTime("2023-01-01"))
                        {
                            data = data.Where(c => c.StartTime > fromdate);
                        }
                        if (todate > Convert.ToDateTime("2023-01-01"))
                        {
                            todate = todate.AddDays(1);
                            data = data.Where(c => c.StartTime <= todate);
                        }
                        if (CadApps.Length != 3)
                        {
                            string selCad = CadApps[0];
                            data = data.Where(c => c.CadTool.ToUpper() == selCad.ToUpper());
                        }
                        if (DataCategory.Length != 3)
                        {
                            string selData = DataCategory[0];
                            if (selData.ToUpper() == "PRODUCTION DATA")
                            {
                                data = data.Where(c => c.ProductionDrive.ToUpper() == "TRUE");
                            }
                            if (selData.ToUpper() == "TEST DATA")
                            {
                                data = data.Where(c => c.ProductionDrive.ToUpper() == "FALSE");
                            }
                        }
                        var tempList = data.ToList();
                        List<TrimReportData> data2 = tempList.Select(x => new TrimReportData
                        {
                            UserID = x.UserID,
                            StartTime = Convert.ToDateTime(x.StartTime),
                            Time = x.StartTime.Hour.ToString() + ":" + x.StartTime.Minute.ToString() + ":" + x.StartTime.Second.ToString(),
                            Region = x.Region,
                            MachineID = x.MachineID,
                            Domain = x.Domain,
                            CadTool = x.CadTool,
                            Productline = x.ProductLine,
                            IsProd = x.IsProd,
                            IsVDI = x.IsVDI,
                            ProductionDrive = x.ProductionDrive,                            
                            BodyCount = x.BodyCount,
                            StopTime = Convert.ToDateTime(x.StopTime)

                        }).ToList();
                        TrimData = data2;
                    }
                    List<SmartCVTReportData> FinalListSmartCVT = new List<SmartCVTReportData>();
                    List<PointChartReportData> FinalListPointChart = new List<PointChartReportData>();
                    List<ProfileCheckerReportData> FinalListProfileChecker = new List<ProfileCheckerReportData>();
                    List<NamingToolReportData> FinalListNamingTool = new List<NamingToolReportData>();
                    List<Multi3DReportData> FinalListMulti3D = new List<Multi3DReportData>();
                    List<SectionManagerReportData> FinalListSectionManager = new List<SectionManagerReportData>();
                    List<TrimReportData> FinalListTrim = new List<TrimReportData>();
                    if (App=="SMART CVT")
                    {
                        foreach (var b in SmartCVTData)
                        {

                            b.BlockPlacement = b.BlockPlacement.Replace("-", ",").Replace("*", "/").ToUpper();
                            b.BlockPlacement = b.BlockPlacement.Replace("B", "b");
                            //var split = b.Time.Split(':');
                            FinalListSmartCVT.Add(b);

                        }
                    }
                    else if(App=="POINT CHART")
                    {
                        FinalListPointChart = PointChartData;                  
                        
                    }
                    else if (App == "PROFILE CHECKER")
                    {
                        FinalListProfileChecker = ProfileCheckerData;

                    }
                    else if (App == "FILE MANAGER")
                    {
                        FinalListNamingTool = NamingToolData;

                    }
                    else if (App == "MULTI 3D")
                    {
                        FinalListMulti3D = Multi3DData;

                    }
                    else if (App == "SECTION MANAGER")
                    {
                        FinalListSectionManager = SectionManagerData;

                    }
                    else if (App == "3D TRIM")
                    {
                        FinalListTrim = TrimData;

                    }
                    //var data2 = data.Select(x => new { x.ApplicationName, x.Functionality , x.CadTool, x.Domain, x.UserID, x.MachineID, x.ProductLine, x.Status, x.Region, x.StartTime, x.StopTime, x.IsProd, x.IsVDI }).ToList();

                    // var data3 = data2.Select(x => new ToolTrackingDetails { ApplicationName = x.ApplicationName, CadTool = x.CadTool, Domain = x.Domain, UserID = x.UserID, MachineID = x.MachineID, ProductLine = x.ProductLine, Status = x.Status, Region = x.Region, StartTime = Convert.ToDateTime(x.StartTime), StopTime = Convert.ToDateTime(x.StopTime) }).ToList();


                    var serializer = new JavaScriptSerializer();


                    serializer.MaxJsonLength = Int32.MaxValue;
                    ContentResult DummyContent = new ContentResult() ;
                    if(App=="SMART CVT")
                    {
                        var result = new ContentResult
                        {
                            Content = serializer.Serialize(FinalListSmartCVT),
                            ContentType = "application/json"
                        };
                        return result;
                    }
                    else if(App=="POINT CHART")
                    {
                        var result = new ContentResult
                        {
                            Content = serializer.Serialize(FinalListPointChart),
                            ContentType = "application/json"
                        };
                        return result; 
                    }
                    else if (App == "PROFILE CHECKER")
                    {
                        var result = new ContentResult
                        {
                            Content = serializer.Serialize(FinalListProfileChecker),
                            ContentType = "application/json"
                        };
                        return result;
                    }
                    else if (App == "FILE MANAGER")
                    {
                        var result = new ContentResult
                        {
                            Content = serializer.Serialize(FinalListNamingTool),
                            ContentType = "application/json"
                        };
                        return result;
                    }
                    else if (App == "MULTI 3D")
                    {
                        var result = new ContentResult
                        {
                            Content = serializer.Serialize(FinalListMulti3D),
                            ContentType = "application/json"
                        };
                        return result;
                    }
                    else if (App == "SECTION MANAGER")
                    {
                        var result = new ContentResult
                        {
                            Content = serializer.Serialize(FinalListSectionManager),
                            ContentType = "application/json"
                        };
                        return result;
                    }
                    else if (App == "3D TRIM")
                    {
                        var result = new ContentResult
                        {
                            Content = serializer.Serialize(FinalListTrim),
                            ContentType = "application/json"
                        };
                        return result;
                    }
                    return DummyContent;
                    //      return Json(data2, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    var serializer = new JavaScriptSerializer();


                    serializer.MaxJsonLength = Int32.MaxValue;
                    var d = ex.ToString();
                    return new ContentResult
                    {
                        Content = serializer.Serialize(d),
                        ContentType = "application/json"
                    };
                    //return Json(d, JsonRequestBehavior.AllowGet);
                }
            }
        }


        public JsonResult CADApplications()
            //Cad utilization matrix when page load
        {
            using (var ctx = new UMTContext())
            {
                var data = ctx.ToolTracking.Where(c => c.IsProd == true && c.Status == "Success")
                    .GroupBy(x => x.CadTool).Select(g => new { title = g.Key, value = g.Count() }).ToList();
                List<String> appNames = ctx.ToolTracking.Where(x => x.IsProd == true && x.Status == "Success").Select(x => x.ApplicationName.Trim().ToUpper())
                    .Distinct().OrderBy(x => x).ToList();
                List<string> dataIn = new List<string>();
                List<BasicChartData> kebStats = new List<BasicChartData>();
                List<BasicChartData> tempkebStats = new List<BasicChartData>();
                string xmlpath = "";
                XmlDocument xmlfile = new XmlDocument();                
                string[] removelist = { "COMBO BLOCK", "SMOOTH BLOCK", "CONVOLUTE BLOCK", "BLOCK CREATION", "EXECUTION", "CREATE BLOCK", "SMOOTH BORE" };
                System.Xml.XmlElement nodesfrmConfig;
                xmlpath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                if (System.IO.File.Exists(xmlpath))
                {
                    xmlfile.Load(xmlpath);
                }
                string formattedStr2 = string.Empty;
                String newname = string.Empty;
                string[] oldApps = { "CLIPS AND ADAPTORS", "HIT", "FLANGE THICKNESS" };
                for (int i = 0; i < 2; i++)
                {
                    BasicChartData ObjKBEappdata = new BasicChartData();
                    ObjKBEappdata.title = GetCADSoftware(i + 1);
                    dataIn.Add(ObjKBEappdata.title);
                    foreach (var app in appNames)
                    {
                        formattedStr2 = app.Replace(" ", "").Replace("_", "").ToUpper();
                        nodesfrmConfig = xmlfile.SelectSingleNode("/configuration/myCustomElement") as XmlElement;
                        XmlNodeList elemlst = nodesfrmConfig.GetElementsByTagName(formattedStr2);
                        //System.Xml.XmlNodeList elemlist = xmlfile.GetElementsByTagName(formattedStr2);
                        if (elemlst != null && elemlst.Count > 0)
                        {
                            newname = elemlst.Item(0).InnerText;
                        }
                        else
                        {
                            newname = app;
                        }
                        string aName = app.ToUpper();
                        var fRes = ctx.ToolTracking.Where(x => x.CadTool== ObjKBEappdata.title && x.ApplicationName == aName && x.IsProd == true && x.Status == "Success").
                           GroupBy(x => x.Functionality).Select(g => new { funcTitle = g.Key, funcVal = g.Count(), })
                           .OrderBy(x => x.funcTitle).ToList();
                        foreach (var fR in fRes)
                        {
                            if (newname == "FTS PMC" || newname == "FBD PMC")
                            {
                                if (fR.funcTitle.ToUpper() == "VALIDATION")
                                {
                                    ObjKBEappdata.value = ObjKBEappdata.value + fR.funcVal;
                                }
                            }
                            if (oldApps.Contains(newname.ToUpper()))
                            {
                                ObjKBEappdata.value = ObjKBEappdata.value + fR.funcVal;
                            }
                            if (newname == "SMART CVT")
                            {
                                if (removelist.Contains(fR.funcTitle.ToUpper()))
                                {
                                    ObjKBEappdata.value = ObjKBEappdata.value + fR.funcVal;
                                    continue;
                                }
                            }
                        }
                    }
                    tempkebStats.Add(ObjKBEappdata);
                }
                bool existdata = false;
                for (int i = 0; i < 2; i++)
                {
                    BasicChartData ObjKBEappdata = new BasicChartData();
                    ObjKBEappdata.title = GetCADSoftware(i + 1);

                    foreach (var x in data)
                    {
                        if(x.title=="SOLIDWORKS")
                        {
                            continue;
                        }
                        if (x.title == GetCADSoftware(i + 1))
                        {
                            if(tempkebStats.Count>0)
                            {
                                foreach(var exDet in tempkebStats)
                                {
                                    if(exDet.title==x.title)
                                    {
                                        if(exDet.value>0)
                                        {
                                            ObjKBEappdata.value = x.value - exDet.value;
                                            existdata = true;
                                            break;
                                        }
                                        else
                                        {
                                            ObjKBEappdata.value = x.value;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }                                
    
                            }                            
                        }
                        else
                        {
                            
                            ObjKBEappdata.value = 0;
                            if (dataIn.Contains(ObjKBEappdata.title))
                            {
                                continue;
                            }                           

                        }
                        if(existdata==true)
                        {
                            break;
                        }
                        
                    }
                    kebStats.Add(ObjKBEappdata);                    

                }
                return Json(kebStats, JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult KBEApplications()
            //application utilization matrix when page load
        {
            
            using (var ctx = new UMTContext())
            {
                //BasicAppData data = new BasicAppData();
                List<String> appNames = ctx.ToolTracking.Where(x => x.IsProd == true && x.Status =="Success").Select(x => x.ApplicationName.Trim().ToUpper())
                    .Distinct().OrderBy(x => x).ToList();
                
                var data = ctx.ToolTracking.Where(c => c.StartTime.Year == DateTime.Now.Year && c.IsProd == true && c.Status == "Success")
                    .GroupBy(x => x.ApplicationName).Select(g => new { title = g.Key.ToUpper(), value = g.Count() }).ToList();

                List<BasicChartData> kebStats = new List<BasicChartData>();             
                string xmlpath = "";
                string xmlLogFile = string.Empty;
                XmlDocument xmlfile = new XmlDocument();
                System.Xml.XmlElement nodesfrmConfig;
                bool nodesd = false;
                xmlpath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                nodesd = true;
                if (System.IO.File.Exists(xmlpath))
                {
                    xmlfile.Load(xmlpath);                    
                }                
                string formattedStr = string.Empty;
                String newname = string.Empty;
                string[] oldApps = { "CLIPSANDADAPTORS","HIT","FLANGETHICKNESS"};
                foreach (var x in data)
                {
                    bool existdata = false;
                    if (oldApps.Contains(x.title.Replace(" ","").ToUpper()))
                    {
                        continue;
                    }
                    BasicChartData ObjKBEappdata = new BasicChartData();
                    //if (x.title == GetKBEAppName(i + 1))
                    //{
                    ObjKBEappdata.title = x.title;
                    ObjKBEappdata.value = x.value;

                    formattedStr = ObjKBEappdata.title.Replace(" ", "").Replace("_","").ToUpper();
                    nodesfrmConfig = xmlfile.SelectSingleNode("/configuration/myCustomElement") as XmlElement;
                    XmlNodeList elemlst = nodesfrmConfig.GetElementsByTagName(formattedStr);
                    if (elemlst != null && elemlst.Count > 0)
                    {
                        newname = elemlst.Item(0).InnerText;
                        ObjKBEappdata.title = newname;
                    }
                    else
                    {
                        ObjKBEappdata.title = x.title;
                    }
                    //if (nodesd ==true)
                    //{
                        

                    //}
                    //else
                    //{
                    //    System.Xml.XmlNodeList elemlist = xmlfile.GetElementsByTagName(formattedStr);
                    //    if (elemlist != null && elemlist.Count > 0)
                    //    {
                    //        newname = elemlist.Item(0).InnerText;
                    //        ObjKBEappdata.title = newname;
                    //    }
                    //    else
                    //    {
                    //        ObjKBEappdata.title = x.title;
                    //    }
                    //}                    
                    //if (elemlist != null && elemlist.Count > 0)
                    //{
                    //    newname = elemlist.Item(0).InnerText;
                    //    ObjKBEappdata.title = newname;
                    //}
                    //else
                    //{
                    //    ObjKBEappdata.title = x.title;
                    //}
                    if (kebStats.Count>0)
                        {
                            int index = 0;
                            foreach (var b in kebStats)
                            {
                            if (b.title == ObjKBEappdata.title)
                            {
                                kebStats[index].value = kebStats[index].value + ObjKBEappdata.value;
                                existdata = true;                                
                            }
                            index = index + 1;
                            }
                        }                                                                 
                                        
                    if (existdata ==false)
                    {
                        kebStats.Add(ObjKBEappdata);
                    }
                    
                }                
                string formattedStr2 = string.Empty;                              
                foreach (String app in appNames)
                {
                    if(oldApps.Contains(app.Replace(" ","").Replace("_","").ToUpper()))
                    {
                        continue;
                    }
                    formattedStr2 = app.Replace(" ", "").Replace("_", "").ToUpper();
                    nodesfrmConfig = xmlfile.SelectSingleNode("/configuration/myCustomElement") as XmlElement;
                    XmlNodeList elemlst = nodesfrmConfig.GetElementsByTagName(formattedStr2);                    
                    if (elemlst != null && elemlst.Count > 0)
                    {
                        newname = elemlst.Item(0).InnerText;
                    }
                    else
                    {
                        newname = app;
                    }

                    List<BasicChartData> d = kebStats.Where(x => x.title == newname).ToList();
                    if (d.Count == 0)
                    {
                        BasicChartData newData = new BasicChartData() { title = newname, value = 0 };
                        newData.customToolTip = "";
                        kebStats.Add(newData);
                    }
                    else
                    {
                        string tooltip = d[0].customToolTip;
                        long TotalFunc = d[0].value;
                        string aNmae =app.ToUpper();
                        var fRes = ctx.ToolTracking.Where(x => x.ApplicationName == aNmae && x.IsProd == true &&x.Status=="Success"
                        && x.StartTime.Year == DateTime.Now.Year).
                            GroupBy(x => x.Functionality).Select(g => new { funcTitle = g.Key, funcVal = g.Count() })
                            .OrderBy(x => x.funcTitle).ToList();
                        string[] removelist = { "COMBO BLOCK", "SMOOTH BLOCK", "CONVOLUTE BLOCK", "BLOCK CREATION", "EXECUTION","CREATE BLOCK","SMOOTH BORE" };
                        foreach (var function in fRes)
                        {
                            if(newname == "FTS PMC" || newname == "FBD PMC")
                            {
                                if (function.funcTitle.ToUpper()=="VALIDATION")
                                {
                                    TotalFunc = TotalFunc - function.funcVal;
                                }                                
                            }
                            if(newname=="SMART CVT")
                            {
                                if (removelist.Contains(function.funcTitle.ToUpper()))
                                {
                                    TotalFunc = TotalFunc - function.funcVal;
                                    continue;
                                }
                            }                            
                            if ((!string.IsNullOrEmpty(tooltip)) && tooltip.ToUpper().Contains(function.funcTitle))
                            {
                                int functionalPos = tooltip.IndexOf(function.funcTitle);
                                int actualPos = functionalPos +function.funcTitle.ToCharArray().Length+ 8; //till end of bold] 8 positions counted
                                string tempSubStr = tooltip.Substring(actualPos);
                                string existingActualValueStr = tempSubStr.Split('[')[0];
                                int.TryParse(existingActualValueStr, out int actualValueInt);
                                int updatedFunCount = actualValueInt + function.funcVal;
                                tooltip=tooltip.Replace(function.funcTitle + ": [bold]" + existingActualValueStr, function.funcTitle + ": [bold]" + updatedFunCount.ToString());
                            }
                            else
                            {
                                tooltip += function.funcTitle + ": [bold]" + function.funcVal.ToString() + "[/]\n";
                            }                            

                        }
                        d[0].customToolTip = tooltip;
                        d[0].value = TotalFunc;
                        
                    }                    
                }                
                //foreach (BasicChartData thisData in kebStats)
                //{
                //    if (string.IsNullOrEmpty(thisData.customToolTip))
                //    {
                //        continue;
                //    }
                //    else
                //    {
                //        thisData.customToolTip = thisData.customToolTip.ToUpper();
                //    }
                //}
                List<BasicChartData> resData = kebStats.OrderBy(x => x.title).ToList();
                return Json(resData, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult MonthlyUtilization()
            //utilization over the year when page load in reports tab
        {
            using (var ctx = new UMTContext())
            {
                var data2 = ctx.ToolTracking
                          .Where(c => c.StartTime.Year == DateTime.Now.Year && c.IsProd == true && c.Status == "Success")
                          .Select(c => new { c.StartTime.Year, c.StartTime.Month })
                          .GroupBy(c => new { c.Month })
                          .Select(grouped => new { value = grouped.Count(), title = grouped.Key.Month }).ToList();
                //var data2 = ctx.ToolTracking.Where(c => c.StartTime.Year == DateTime.Now.Year && c.IsProd == true && c.Status =="Success")
                //    .Select(c =>new{ Month = c.StartTime.Month })
                //    .GroupBy(c => c.Month)
                //    .Select(grouped =>new{ value= grouped.Count(),title = grouped.Key}).ToList();
                //var tData = ctx.ToolTracking.Where(x => x.Domain == "MYCORP").ToList();
                var data = data2.Select(x => new { title = Getmonth(x.title), value = x.value }).ToList();
                
                string[] removelist = { "COMBO BLOCK", "SMOOTH BLOCK", "CONVOLUTE BLOCK", "BLOCK CREATION", "EXECUTION", "CREATE BLOCK", "SMOOTH BORE" };

                //Newtonsoft.Json.Linq.JArray MonthlyData = new Newtonsoft.Json.Linq.JArray();
                System.Xml.XmlElement nodesfrmConfig;
                string xmlpath = "";
                string[] oldApps = { "CLIPS AND ADAPTORS", "HIT", "FLANGE THICKNESS" };
                XmlDocument xmlfile = new XmlDocument();
                xmlpath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                if (System.IO.File.Exists(xmlpath))
                {
                    xmlfile.Load(xmlpath);
                }
                string formattedStr2 = string.Empty;
                string newname = string.Empty;
                
                List<MonthlyData> lstMonthlyData = new List<MonthlyData>();
                List<MonthlyData> templstMonthlyData = new List<MonthlyData>();
                for (int i = 0; i < data.Count(); i++)
                {                   
                    MonthlyData ObjOneMonthdata = new MonthlyData();
                    ObjOneMonthdata.title = data[i].title;
                    ObjOneMonthdata.value = 0;
                    int monthID = Getmonthindex(ObjOneMonthdata.title);
                    List<String> appNames = ctx.ToolTracking.Where(x => x.StartTime.Year == DateTime.Now.Year &&x.StartTime.Month==monthID && x.IsProd == true && x.Status == "Success").Select(x => x.ApplicationName.Trim().ToUpper())
                    .Distinct().OrderBy(x => x).ToList();
                    foreach (var app in appNames)
                    {

                        formattedStr2 = app.Replace(" ", "").Replace("_", "").ToUpper();
                        nodesfrmConfig = xmlfile.SelectSingleNode("/configuration/myCustomElement") as XmlElement;
                        XmlNodeList elemlst = nodesfrmConfig.GetElementsByTagName(formattedStr2);                        
                        if (elemlst != null && elemlst.Count > 0)
                        {
                            newname = elemlst.Item(0).InnerText;
                        }
                        else
                        {
                            newname = app;
                        }
                        string aName = app.ToUpper();
                        var fRes = ctx.ToolTracking.Where(x => x.ApplicationName == aName && x.IsProd == true && x.Status == "Success"
                       && x.StartTime.Year == DateTime.Now.Year && x.StartTime.Month == monthID).
                           GroupBy(x => x.Functionality).Select(g => new { funcTitle = g.Key, funcVal = g.Count(), })
                           .OrderBy(x => x.funcTitle).ToList();
                        if(fRes.Count>0&&(newname == "FTS PMC" || newname == "FBD PMC"|| oldApps.Contains(newname.ToUpper())|| newname == "SMART CVT"))
                            foreach (var fR in fRes)
                            {
                                if (newname == "FTS PMC" || newname == "FBD PMC")
                                {
                                    if (fR.funcTitle.ToUpper() == "VALIDATION")
                                    {
                                        ObjOneMonthdata.value = ObjOneMonthdata.value + fR.funcVal;
                                        continue;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                if (oldApps.Contains(newname.ToUpper()))
                                {
                                    ObjOneMonthdata.value = ObjOneMonthdata.value + fR.funcVal;
                                    continue;
                                }
                                if (newname == "SMART CVT")
                                {
                                    if (removelist.Contains(fR.funcTitle.ToUpper()))
                                    {
                                        ObjOneMonthdata.value = ObjOneMonthdata.value + fR.funcVal;
                                        continue;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                            }


                    }
                    templstMonthlyData.Add(ObjOneMonthdata);
                }

                for (int i = 0; i < 12; i++)
                {
                    MonthlyData ObjOneMonthdata = new MonthlyData();
                    ObjOneMonthdata.title = Getmonth(i + 1);
                    int value = 0;
                    bool validmonth = false;
                    bool existMon = false;
                    foreach (var item in data)
                    {
                        if (item.title == ObjOneMonthdata.title)//copy 984 line
                        {
                            if (templstMonthlyData.Count > 0)
                            {
                                foreach (var exDet in templstMonthlyData)
                                {
                                    if (exDet.title == item.title)
                                    {
                                        value = item.value - exDet.value;
                                        validmonth = true;
                                        existMon = true;
                                        break;
                                    }

                                }
                                break;
                            }


                        }

                    }


                    if (validmonth)
                    {
                        ObjOneMonthdata.value = value;
                    }
                    else
                    {
                        ObjOneMonthdata.value = 0;
                    }
                    //if (existMon)
                    //{
                    //    lstMonthlyData[i].value = ObjOneMonthdata.value;
                    //    continue;
                    //}
                    lstMonthlyData.Add(ObjOneMonthdata);
                }
                return Json(lstMonthlyData, JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult MonthlyUtilizationHome()
            //monthly utilization over the year when page load in home tab
        {
            try
            {
                using (var ctx = new UMTContext())
                {
                    var data2 = ctx.ToolTracking
                              .Where(c => c.StartTime.Year == DateTime.Now.Year && c.IsProd == true&&c.Status == "Success")
                              .Select(c => new { c.StartTime.Year, c.StartTime.Month })
                              .GroupBy(c => new { c.Month })
                              .Select(grouped => new { value = grouped.Count(), title = grouped.Key.Month }).ToList();                    
                    string[] removelist = { "COMBO BLOCK", "SMOOTH BLOCK", "CONVOLUTE BLOCK", "BLOCK CREATION", "EXECUTION", "CREATE BLOCK", "SMOOTH BORE" };
                    var data = data2.Select(x => new { title = Getmonth(x.title), value = x.value }).ToList();
                    string xmlpath = "";
                    string[] oldApps = { "CLIPS AND ADAPTORS", "HIT", "FLANGE THICKNESS" };
                    XmlDocument xmlfile = new XmlDocument();
                    System.Xml.XmlElement nodesfrmConfig;
                    xmlpath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                    if (System.IO.File.Exists(xmlpath))
                    {
                        xmlfile.Load(xmlpath);
                    }
                    string formattedStr2 = string.Empty;
                    string newname = string.Empty;                    
                    List<MonthlyData> lstMonthlyData = new List<MonthlyData>();
                    List<MonthlyData> templstMonthlyData = new List<MonthlyData>();
                    //int totLoopCount = appNames.Count();                    
                    for (int i = 0; i < data.Count(); i++)
                    {
                        MonthlyData ObjOneMonthdata = new MonthlyData();
                        ObjOneMonthdata.title = data[i].title;
                        ObjOneMonthdata.value = 0;
                        int monthID = Getmonthindex(ObjOneMonthdata.title);
                        List<String> appNames = ctx.ToolTracking.Where(x => x.StartTime.Year == DateTime.Now.Year&&x.StartTime.Month==monthID && x.IsProd == true && x.Status == "Success").Select(x => x.ApplicationName.Trim().ToUpper())
                    .Distinct().OrderBy(x => x).ToList();
                        foreach (var app in appNames)
                        {
                            
                            formattedStr2 = app.Replace(" ", "").Replace("_", "").ToUpper();
                            nodesfrmConfig = xmlfile.SelectSingleNode("/configuration/myCustomElement") as XmlElement;
                            XmlNodeList elemlst = nodesfrmConfig.GetElementsByTagName(formattedStr2);
                            //System.Xml.XmlNodeList elemlist = xmlfile.GetElementsByTagName(formattedStr2);
                            if (elemlst != null && elemlst.Count > 0)
                            {
                                newname = elemlst.Item(0).InnerText;
                            }
                            else
                            {
                                newname = app;
                            }
                            string aName = app.ToUpper();
                            var fRes = ctx.ToolTracking.Where(x => x.ApplicationName == aName && x.IsProd == true && x.Status == "Success"
                           && x.StartTime.Year == DateTime.Now.Year && x.StartTime.Month == monthID).
                               GroupBy(x => x.Functionality).Select(g => new { funcTitle = g.Key, funcVal = g.Count(), })
                               .OrderBy(x => x.funcTitle).ToList();  
                            if(fRes.Count>0&&(newname == "FTS PMC" || newname == "FBD PMC"|| oldApps.Contains(newname.ToUpper())|| newname == "SMART CVT"))
                            {
                                foreach (var fR in fRes)
                                {
                                    if (newname == "FTS PMC" || newname == "FBD PMC")
                                    {
                                        if (fR.funcTitle.ToUpper() == "VALIDATION")
                                        {
                                            ObjOneMonthdata.value = ObjOneMonthdata.value + fR.funcVal;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    if (oldApps.Contains(newname.ToUpper()))
                                    {
                                        ObjOneMonthdata.value = ObjOneMonthdata.value + fR.funcVal;
                                    }
                                    if (newname == "SMART CVT")
                                    {
                                        if (removelist.Contains(fR.funcTitle.ToUpper()))
                                        {
                                            ObjOneMonthdata.value = ObjOneMonthdata.value + fR.funcVal;
                                            continue;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }



                        }
                        templstMonthlyData.Add(ObjOneMonthdata);
                    }

                    for (int i = 0; i < 12; i++)
                    {
                        MonthlyData ObjOneMonthdata = new MonthlyData();
                        ObjOneMonthdata.title = Getmonth(i + 1);
                        int value = 0;
                        bool validmonth = false;
                        bool existMon = false;
                        foreach (var item in data)
                        {
                            if (item.title == ObjOneMonthdata.title)
                            {
                                if(templstMonthlyData.Count>0)
                                {
                                    foreach (var exDet in templstMonthlyData)
                                    {
                                        if(exDet.title==item.title)
                                        {
                                            value = item.value - exDet.value;
                                            validmonth = true;
                                            existMon = true;
                                            break;
                                        }
                                        
                                    }
                                    break;
                                }
                                
                                
                            }

                        }


                        if (validmonth)
                        {
                            ObjOneMonthdata.value = value;
                        }
                        else
                        {
                            ObjOneMonthdata.value = 0;
                        }
                        //if(existMon)
                        //{
                        //    lstMonthlyData[i].value = ObjOneMonthdata.value; 
                        //    continue;
                        //}
                        lstMonthlyData.Add(ObjOneMonthdata);
                    }
                    return Json(lstMonthlyData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult PercentageUtilization()
            //percentage utilization over the year when page load
        {
            using (var ctx = new UMTContext())
            {
                var data = ctx.ToolTracking.Where(c => c.StartTime.Year == DateTime.Now.Year&&c.IsProd == true&&c.Status == "Success")
                          .GroupBy(c => c.ApplicationName)
                          .Select(grouped => new { value = grouped.Count(), title = grouped.Key.ToUpper() }).ToList();
                List<String> appNames = ctx.ToolTracking.Where(x => x.IsProd == true && x.Status == "Success").Select(x => x.ApplicationName.Trim().ToUpper())
                    .Distinct().OrderBy(x => x).ToList();
                long totalCount = ctx.ToolTracking.Where(c=> c.StartTime.Year == DateTime.Now.Year && c.IsProd == true && c.Status == "Success").Count();
                long smartCvtCount = 0;
                long FTSpmcValidcount = 0;
                long FBDpmcValidCount = 0;
                string xmlpath = "";
                XmlDocument xmlfile = new XmlDocument();                
                string[] oldApps = { "CLIPSANDADAPTORS", "HIT", "FLANGETHICKNESS" };
                System.Xml.XmlElement nodesfrmConfig;
                xmlpath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                if (System.IO.File.Exists(xmlpath))
                {
                    xmlfile.Load(xmlpath);
                }
                string formattedStr2 = string.Empty;
                string newname = string.Empty;                
                foreach (var app in appNames)
                {
                    formattedStr2 = app.Replace(" ", "").Replace("_", "").ToUpper();
                    nodesfrmConfig = xmlfile.SelectSingleNode("/configuration/myCustomElement") as XmlElement;
                    XmlNodeList elemlst = nodesfrmConfig.GetElementsByTagName(formattedStr2);
                    //System.Xml.XmlNodeList elemlist = xmlfile.GetElementsByTagName(formattedStr2);
                    if (elemlst != null && elemlst.Count > 0)
                    {
                        newname = elemlst.Item(0).InnerText;
                    }
                    else
                    {
                        newname = app;
                    }
                    string aName = app.ToUpper();
                    var fRes = ctx.ToolTracking.Where(x => x.ApplicationName == aName && x.IsProd == true && x.Status == "Success"
                        && x.StartTime.Year == DateTime.Now.Year).
                            GroupBy(x => x.Functionality).Select(g => new { funcTitle = g.Key, funcVal = g.Count() })
                            .OrderBy(x => x.funcTitle).ToList();
                    string[] removelist = { "COMBO BLOCK", "SMOOTH BLOCK", "CONVOLUTE BLOCK", "BLOCK CREATION", "EXECUTION","SMOOTH BORE","CREATE BLOCK" };
                    foreach (var fR in fRes)
                    {
                        if (newname == "FTS PMC")
                        {
                            if (fR.funcTitle.ToUpper() == "VALIDATION")
                            {
                                totalCount = totalCount - fR.funcVal;
                                FTSpmcValidcount = FTSpmcValidcount + fR.funcVal;
                                continue;
                            }
                        }
                        if(newname=="FBD PMC")
                        {
                            if (fR.funcTitle.ToUpper() == "VALIDATION")
                            {
                                totalCount = totalCount - fR.funcVal;
                                FBDpmcValidCount = FBDpmcValidCount + fR.funcVal;
                                continue;
                            }
                        }
                        if (oldApps.Contains(app.ToUpper()))
                        {
                            totalCount = totalCount - fR.funcVal;
                            continue;
                        }
                        if(newname=="SMART CVT")
                        {
                            if(removelist.Contains(fR.funcTitle.ToUpper()))
                            {
                                totalCount = totalCount - fR.funcVal;
                                smartCvtCount =smartCvtCount + fR.funcVal;
                                continue;
                            }
                                
                        }
                    }
                }
                List<BasicChartDatadecimal> kebStats = new List<BasicChartDatadecimal>();                
                string formattedStr = string.Empty;
                String newname1 = string.Empty;
                foreach (var item in data)
                {
                    bool existdata = false;
                    if (oldApps.Contains(item.title.ToUpper()))
                    {                        
                        continue;
                    }
                    BasicChartDatadecimal ObjKBEappdata = new BasicChartDatadecimal();
                    ObjKBEappdata.title = item.title;
                    //ObjKBEappdata.value = (decimal)item.value * 100 / totalCount;
                    ObjKBEappdata.actual = item.value;
                    formattedStr = ObjKBEappdata.title.Replace(" ", "").Replace("_", "").ToUpper();
                    nodesfrmConfig = xmlfile.SelectSingleNode("/configuration/myCustomElement") as XmlElement;
                    XmlNodeList elemlst = nodesfrmConfig.GetElementsByTagName(formattedStr);
                    //System.Xml.XmlNodeList elemlist = xmlfile.GetElementsByTagName(formattedStr);
                    if (elemlst != null && elemlst.Count > 0)
                    {
                        newname1 = elemlst.Item(0).InnerText;
                        ObjKBEappdata.title = newname1;
                    }
                    else
                    {
                        ObjKBEappdata.title = item.title;
                    }
                    if (kebStats.Count > 0)
                    {
                        int index = 0;
                        foreach (var b in kebStats)
                        {
                            if (b.title == ObjKBEappdata.title)
                            {
                                //kebStats[index].value = kebStats[index].value + ObjKBEappdata.value;
                                kebStats[index].actual = kebStats[index].actual + ObjKBEappdata.actual;
                                existdata = true;
                            }
                            index = index + 1;
                        }
                    }
                    if (existdata == false)
                    {                        
                        kebStats.Add(ObjKBEappdata);
                    }
                    
                
                }
                //int indexFinal = 0;
                foreach(var fData in kebStats)
                {
                    if(fData.title =="FTS PMC")
                    {
                        fData.actual = fData.actual - FTSpmcValidcount;
                        fData.value = (decimal)fData.actual * 100 / totalCount;
                    }
                    if(fData.title == "FBD PMC")
                    {
                        fData.actual = fData.actual - FBDpmcValidCount;
                        fData.value = (decimal)fData.actual * 100 / totalCount;
                    }
                    if(fData.title=="SMART CVT")
                    {
                        fData.actual = fData.actual - smartCvtCount;
                        fData.value  = (decimal)fData.actual * 100 / totalCount;
                    }
                    fData.value = (decimal)fData.actual * 100 / totalCount;
                }

                return Json(kebStats, JsonRequestBehavior.AllowGet);
            }

        }

        

        [HttpPost]
        public JsonResult KBEApplicationsFilter(string PC, string AUM_Region, string CAD, string Hardware, string Domain,
            DateTime fromdate, DateTime todate,string App)
            //application utilization matrix when filter apply
        {         
            
            using (var ctx = new UMTContext())
            {
                bool smtCVTSel = false;
                string[] smtCVT = { "SMART CVT", "SMART CVT DESIGN TOOL", "SMOOTH BLOCK", "CONVOLUTE BLOCK", "COMBO BLOCK", "SMART CVT NX" };
                var data = ctx.ToolTracking.Where(c => c.SrNo > 0 && c.IsProd == true&&c.Status == "Success");
               // List<ToolTrackingDetails> allData = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstAppFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstPcFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstRegionFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstCadAppsFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstHardwareFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstDomainFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstDateFilter = new List<ToolTrackingDetails>();

                bool anyfilter = false;
                bool nullValue = false;
                string[] selVal_AUM_AppArr = Request.Form.GetValues("AumApps");
                string selVal_AUM_App = selVal_AUM_AppArr[0].ToUpper();

                string[] selVal_AUM_PcArr = Request.Form.GetValues("AumPc");
                string selVal_AUM_Pc = selVal_AUM_PcArr[0].ToUpper();

                string[] selVal_AUM_RegionArr = Request.Form.GetValues("AumRegion");
                string selVal_AUM_Region = selVal_AUM_RegionArr[0].ToUpper();

                string[] selVal_AUM_CadAppsArr = Request.Form.GetValues("AumCadApps");
                string selVal_AUM_CadApps = selVal_AUM_CadAppsArr[0].ToUpper();

                string[] selVal_AUM_HardwareArr = Request.Form.GetValues("AumHardware");
                string selVal_AUM_Hardware = selVal_AUM_HardwareArr[0].ToUpper();

                string[] selVal_AUM_DomainArr = Request.Form.GetValues("AumDomain");
                string selVal_AUM_Domain = selVal_AUM_DomainArr[0].ToUpper();
                bool notSelect = false;
                if (selVal_AUM_Pc == "" || selVal_AUM_Region == "" || selVal_AUM_App == "" || selVal_AUM_Hardware == "" || selVal_AUM_Domain == ""||selVal_AUM_CadApps=="")
                {
                    notSelect = true;
                }

                List<string> lst_app = new List<string> ();
                List<string> lst_Pc = new List<string> ();
                List<string> lst_Region = new List<string>();
                List<string> lst_Domain = new List<string> ();
                List<string> lst_Hardware = new List<string>();
                List<string> lst_CadApps = new List<string> ();

                if (!selVal_AUM_App.Contains("ALL") && selVal_AUM_App != "" && notSelect == false)
                {
                    anyfilter = true;
                    string[] sv_App = selVal_AUM_App.Split(',');
                    lst_app = sv_App.ToList();
                    foreach (var x in lst_app)
                    {
                        lstAppFilter.AddRange(data.Where(c => c.ApplicationName.ToUpper() == x.ToUpper() && c.IsProd == true && c.Status == "Success"));
                    }
                    if (lstAppFilter.Count > 0)
                    {
                        nullValue = true;
                    }
                }
                if (!selVal_AUM_Pc.Contains("ALL") && selVal_AUM_Pc != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_Pc = selVal_AUM_Pc.Split(',');
                    lst_Pc = sv_Pc.ToList();
                    foreach (var x in lst_Pc)
                    {
                        if (lstAppFilter.Count == 0)
                        {
                            if (x.ToUpper() == "FLUIDS")
                            {
                                lstPcFilter.AddRange(data.Where(c => c.ProductLine == "FTS" || c.ProductLine == "FBD" || c.ProductLine == "FLUIDS"));
                            }
                            else
                            {
                                lstPcFilter.AddRange(data.Where(c => c.ProductLine == x));
                            }
                        }
                        else
                        {
                            if (x.ToUpper() == "FLUIDS")
                            {
                                lstPcFilter.AddRange(lstAppFilter.Where(c => c.ProductLine == "FTS" || c.ProductLine == "FBD" || c.ProductLine == "FLUIDS"));
                            }
                            else
                            {
                                lstPcFilter.AddRange(lstAppFilter.Where(c => c.ProductLine == x));
                            }
                        }
                    }
                    if (lstPcFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                else if (lstAppFilter.Count > 0)
                {
                    lstPcFilter = lstAppFilter;
                }
                if (!selVal_AUM_Region.Contains("ALL") && selVal_AUM_Region != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_Region = selVal_AUM_Region.Split(',');
                    lst_Region = sv_Region.ToList();
                    foreach (var x in lst_Region)
                    {
                        if (lstPcFilter.Count == 0)
                        {
                            lstRegionFilter.AddRange(data.Where(c => c.Region == x));
                        }
                        else
                        {
                            lstRegionFilter.AddRange(lstPcFilter.Where(c => c.Region == x));
                        }
                    }
                    if (lstRegionFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                else if (lstPcFilter.Count > 0)
                {
                    lstRegionFilter = lstPcFilter;
                }
                if (!selVal_AUM_CadApps.Contains("ALL") && selVal_AUM_CadApps != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_CadApps = selVal_AUM_CadApps.Split(',');
                    lst_CadApps = sv_CadApps.ToList();
                    foreach (var x in lst_CadApps)
                    {
                        if (lstRegionFilter.Count == 0)
                        {
                            lstCadAppsFilter.AddRange(data.Where(c => c.CadTool == x));
                        }
                        else
                        {
                            lstCadAppsFilter.AddRange(lstRegionFilter.Where(c => c.CadTool == x));
                        }
                    }
                    if (lstCadAppsFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                else if (lstRegionFilter.Count > 0)
                {
                    lstCadAppsFilter = lstRegionFilter;
                }
                if (!selVal_AUM_Hardware.Contains("ALL") && selVal_AUM_Hardware != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_Hardware = selVal_AUM_Hardware.Split(',');
                    lst_Hardware = sv_Hardware.ToList();
                    foreach (var x in lst_Hardware)
                    {
                        if (lstCadAppsFilter.Count == 0)
                        {
                            if (x.ToUpper() == "VDI")
                            {
                                lstHardwareFilter.AddRange(data.Where(c => c.IsVDI == true));
                            }
                            if (x.ToUpper() == "NON-VDI")
                            {
                                lstHardwareFilter.AddRange(data.Where(c => c.IsVDI == false));
                            }
                        }
                        else
                        {
                            if (x.ToUpper() == "VDI")
                            {
                                lstHardwareFilter.AddRange(lstCadAppsFilter.Where(c => c.IsVDI == true));
                            }
                            if (x.ToUpper() == "NON-VDI")
                            {
                                lstHardwareFilter.AddRange(lstCadAppsFilter.Where(c => c.IsVDI == false));
                            }
                        }
                    }
                    if (lstHardwareFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                else if (lstCadAppsFilter.Count > 0)
                {
                    lstHardwareFilter = lstCadAppsFilter;
                }
                if (!selVal_AUM_Domain.Contains("ALL") && selVal_AUM_Domain != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_Domain = selVal_AUM_Domain.Split(',');
                    lst_Domain = sv_Domain.ToList();
                    foreach (var x in lst_Domain)
                    {
                        if (lstHardwareFilter.Count == 0)
                        {
                            lstDomainFilter.AddRange(data.Where(c => c.Domain == x));
                            //if (x.ToUpper() == "TATA")
                            //{
                            //    lstDomainFilter.AddRange(data.Where(c => c.Domain == "MYCORP"));
                            //}
                            //else if (x.ToUpper() == "COOPER")
                            //{
                            //    lstDomainFilter.AddRange(data.Where(c => c.Domain == "CSAID" || c.Domain == "AUBURN" || c.Domain == "ASIA"));
                            //}
                            //else if (x.ToUpper() == "MERGON")
                            //{
                            //    lstDomainFilter.AddRange(data.Where(c => c.Domain == "MERGON"));
                            //}
                            //else if (x.ToUpper() == "MTC")
                            //{
                            //    lstDomainFilter.AddRange(data.Where(c => c.Domain == "MTC"));
                            //}
                        }
                        else
                        {
                            lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain ==x));
                            //if (x.ToUpper() == "TATA")
                            //{
                            //    lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == "MYCORP"));
                            //}
                            //else if (x.ToUpper() == "COOPER")
                            //{
                            //    lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == "CSAID" || c.Domain == "AUBURN" || c.Domain == "ASIA"));
                            //}
                            //else if (x.ToUpper() == "MERGON")
                            //{
                            //    lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == "MERGON"));
                            //}
                            //else if (x.ToUpper() == "MTC")
                            //{
                            //    lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == "MTC"));
                            //}
                        }
                    }
                    if (lstDomainFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                else if (lstHardwareFilter.Count > 0)
                {
                    lstDomainFilter = lstHardwareFilter;
                }
                if (fromdate > Convert.ToDateTime("2019-01-01") && todate > Convert.ToDateTime("2019-01-01") && nullValue == false)
                {
                    anyfilter = true;
                    if (lstDomainFilter.Count == 0)
                    {
                        todate = todate.AddDays(1);
                        lstDateFilter.AddRange(data.Where(c => c.StartTime > fromdate && c.StartTime <= todate));
                    }
                    else
                    {
                        todate = todate.AddDays(1);
                        lstDateFilter.AddRange(lstDomainFilter.Where(c => c.StartTime > fromdate && c.StartTime <= todate));
                    }

                }

                var data2 = anyfilter==false ? data.GroupBy(x => x.ApplicationName).Select(g => new { title = g.Key.ToUpper(), value = g.Count() }).ToList(): lstDateFilter.GroupBy(x => x.ApplicationName).Select(g => new { title = g.Key.ToUpper(), value = g.Count() }).ToList();
                List<String> appNames = ctx.ToolTracking.Where(x => x.IsProd == true&&x.Status=="Success").Select(x => x.ApplicationName.Trim().ToUpper()).Distinct().OrderBy(x => x).ToList();
                List<BasicChartData> kebStats = new List<BasicChartData>();
                string xmlpath = "";
                XmlDocument xmlfile = new XmlDocument();
                bool existdata = false;
                System.Xml.XmlElement nodesfrmConfig;
                xmlpath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                if (System.IO.File.Exists(xmlpath))
                {
                    xmlfile.Load(xmlpath);
                }
                string formattedStr = string.Empty;
                String newname = string.Empty;
                string[] oldApps = { "CLIPSANDADAPTORS", "HIT", "FLANGETHICKNESS" };
                foreach (var x in data2)
                {
                    if (oldApps.Contains(x.title.Replace(" ", "").Replace("_", "").ToUpper()))
                    {
                        continue;
                    }
                    BasicChartData ObjKBEappdata = new BasicChartData();                    
                    ObjKBEappdata.title = x.title;
                    ObjKBEappdata.value = x.value;                    
                    formattedStr  = ObjKBEappdata.title.Replace(" ", "").Replace("_","").ToUpper();                   
                    bool existData = false;
                    xmlfile.Load(xmlpath);
                    nodesfrmConfig = xmlfile.SelectSingleNode("/configuration/myCustomElement") as XmlElement;
                    XmlNodeList elemlst = nodesfrmConfig.GetElementsByTagName(formattedStr);
                    //System.Xml.XmlNodeList elemlist = xmlfile.GetElementsByTagName(formattedStr);
                    if (elemlst != null && elemlst.Count > 0)
                    {
                        newname = elemlst.Item(0).InnerText;
                        ObjKBEappdata.title = newname;
                    }
                    else
                    {
                        ObjKBEappdata.title = x.title; ;
                    }
                    if (kebStats.Count > 0)
                        {
                            int index = 0; 
                            foreach (var b in kebStats)
                            {
                                if (b.title == ObjKBEappdata.title)
                                {
                                    kebStats[index].value = kebStats[index].value + ObjKBEappdata.value;
                                    existData = true;
                                }
                                index = index + 1;
                            }                       
                            
                        }                    
                    if(existData== false)
                    {
                        kebStats.Add(ObjKBEappdata);
                    }
                    
                }
                string formattedStr2 = string.Empty;
                foreach (String app in appNames)
                {
                    if (oldApps.Contains(app.Replace(" ", "").Replace("_", "").ToUpper()))
                    {
                        continue;
                    }
                    formattedStr2 = app.Replace(" ", "").Replace("_", "").ToUpper();
                    nodesfrmConfig = xmlfile.SelectSingleNode("/configuration/myCustomElement") as XmlElement;
                    XmlNodeList elemlst = nodesfrmConfig.GetElementsByTagName(formattedStr2);
                    //System.Xml.XmlNodeList elemlist = xmlfile.GetElementsByTagName(formattedStr2);
                    if (elemlst != null && elemlst.Count > 0)
                    {
                        newname = elemlst.Item(0).InnerText;
                    }
                    else
                    {
                        newname = app;
                    }
                    List<BasicChartData> d = kebStats.Where(x => x.title == newname).ToList();
                    if (d.Count == 0)
                    {
                        BasicChartData newData = new BasicChartData() { title = newname, value = 0};
                        newData.customToolTip = "";
                        kebStats.Add(newData);
                    }
                    else
                    {
                        string tooltip = d[0].customToolTip;
                        long TotalFunc = d[0].value;
                        string aNmae = app.ToUpper();
                        var fRes =anyfilter==false? data.Where(x => x.ApplicationName.ToUpper() == aNmae && x.IsProd == true&&x.Status== "Success" && x.StartTime > fromdate && x.StartTime <= todate).
                            GroupBy(x => x.Functionality).Select(g => new { funcTitle = g.Key, funcVal = g.Count() })
                            .OrderBy(x => x.funcTitle).ToList(): lstDateFilter.Where(x => x.ApplicationName.ToUpper() == aNmae && x.IsProd == true && x.Status == "Success" && x.StartTime > fromdate && x.StartTime <= todate).
                            GroupBy(x => x.Functionality).Select(g => new { funcTitle = g.Key, funcVal = g.Count() })
                            .OrderBy(x => x.funcTitle).ToList();
                        //valStr = d[0].customToolTip;
                        string[] removelistforSMTCVT = { "COMBO BLOCK", "SMOOTH BLOCK", "CONVOLUTE BLOCK", "BLOCK CREATION", "EXECUTION","CREATE BLOCK","SMOOTH BORE" };
                        foreach (var function in fRes)
                        {
                            if (newname == "FTS PMC"||newname =="FBD PMC")
                            {
                                if (function.funcTitle.ToUpper() == "VALIDATION")
                                {
                                    TotalFunc = TotalFunc - function.funcVal;
                                }
                            }
                            if (newname == "SMART CVT")
                            {
                                if (removelistforSMTCVT.Contains(function.funcTitle.ToUpper()))
                                {
                                    TotalFunc = TotalFunc - function.funcVal;
                                    continue;
                                }
                            }
                            if ((!string.IsNullOrEmpty(tooltip)) && tooltip.ToUpper().Contains(function.funcTitle.ToUpper()))
                            {
                                int functionalPos = tooltip.IndexOf(function.funcTitle.ToUpper());
                                int actualPos = functionalPos + function.funcTitle.ToUpper().ToCharArray().Length + 8; //till end of bold] 8 positions counted
                                string tempSubStr = tooltip.Substring(actualPos);
                                string existingActualValueStr = tempSubStr.Split('[')[0];
                                int.TryParse(existingActualValueStr, out int actualValueInt);
                                int updatedFunCount = actualValueInt + function.funcVal;
                                if(function.funcTitle=="")
                                {
                                    tooltip+=function.funcTitle.ToUpper()+ ": [bold]" + function.funcVal.ToString()+ "[/]\n";
                                }
                                tooltip = tooltip.Replace(function.funcTitle.ToUpper() + ": [bold]" + existingActualValueStr, function.funcTitle.ToUpper() + ": [bold]" + updatedFunCount.ToString());
                            }
                            else
                            {
                                tooltip += function.funcTitle.ToUpper() + ": [bold]" + function.funcVal.ToString() + "[/]\n";
                            }
                        }
                        d[0].customToolTip = tooltip;
                        d[0].value = TotalFunc;
                    }
                }                

               List<BasicChartData> resData = kebStats.OrderBy(x => x.title).ToList();
                return Json(resData, JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult CADToolFilter(string PC, string Region, string App, string Hardware, string Domain,
            DateTime fromdate, DateTime todate)
            //cad utilzation matrix when filter apply
        {
            using (var ctx = new UMTContext())
            {
                bool smtCVTSel = false;
                bool pmcSel = false;
                bool oldappsSel = false;
                string[] smtCVT = { "SMART CVT", "SMART CVT DESIGN TOOL", "SMOOTH BLOCK", "CONVOLUTE BLOCK", "COMBO BLOCK", "SMART CVT NX","STICK MODEL" };
                string[] pmcLst = {"FTS PMC","FBD PMC","FTS PMC VALIDATION","FTS PMC EXECUTION","FBD PMC VALIDATION","FBD PMC EXECUTION",
                                    "SEALING PMC VALIDATION","SEALING PMC EXECUTION"};
                string[] oldApps = { "CLIPS AND ADAPTORS", "HIT", "FLANGE THICKNESS" };
                var data = ctx.ToolTracking.Where(c => c.SrNo > 0 && c.IsProd == true&&c.Status == "Success");
                List<ToolTrackingDetails> lstAppFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstPcFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstRegionFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstHardwareFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstDomainFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstDateFilter = new List<ToolTrackingDetails>();

                bool nullValue = false;
                bool anyfilter = false;

                string[] selVal_AppArr = Request.Form.GetValues("CumApps");
                string selVal_App = selVal_AppArr[0].ToUpper();

                string[] selVal_PcArr = Request.Form.GetValues("CumPc");
                string selVal_Pc = selVal_PcArr[0].ToUpper();

                string[] selVal_RegionArr = Request.Form.GetValues("CumRegion");
                string selVal_Region = selVal_RegionArr[0].ToUpper();                

                string[] selVal_HardwareArr = Request.Form.GetValues("CumHardware");
                string selVal_Hardware = selVal_HardwareArr[0].ToUpper();

                string[] selVal_DomainArr = Request.Form.GetValues("CumDomain");
                string selVal_Domain = selVal_DomainArr[0].ToUpper();
                bool notSelect = false;
                if (selVal_Pc == "" || selVal_Region == "" ||  selVal_App == "" || selVal_Hardware == "" || selVal_Domain == "")
                {
                    notSelect = true;
                }

                List<string> lst_app = new List<string>();
                List<string> lst_Pc = new List<string>();
                List<string> lst_Region = new List<string>();
                List<string> lst_Domain = new List<string>();
                List<string> lst_Hardware = new List<string>();
                List<string> lst_CadApps = new List<string>();

                if (!selVal_App.Contains("ALL") && selVal_App != "" && notSelect == false)
                {
                    anyfilter = true;
                    string[] sv_App = selVal_App.Split(',');
                    lst_app = sv_App.ToList();
                    foreach (var x in lst_app)
                    {
                        if (smtCVT.Contains(x.ToUpper()))
                        {
                            smtCVTSel = true;
                        }
                        else if (pmcLst.Contains(x.ToUpper()))
                        {
                            pmcSel = true;
                        }
                        else if (oldApps.Contains(x.ToUpper()))
                        {
                            oldappsSel = true;
                        }
                        lstAppFilter.AddRange(data.Where(c => c.ApplicationName == x && c.IsProd == true && c.Status == "Success"));
                    }
                    if (lstAppFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                if (!selVal_Pc.Contains("ALL") && selVal_Pc != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_Pc = selVal_Pc.Split(',');
                    lst_Pc = sv_Pc.ToList();
                    foreach (var x in lst_Pc)
                    {
                        if (lstAppFilter.Count == 0)
                        {
                            if (x.ToUpper() == "FLUIDS")
                            {
                                lstPcFilter.AddRange(data.Where(c => c.ProductLine == "FTS" || c.ProductLine == "FBD" || c.ProductLine == "FLUIDS"));
                            }
                            else
                            {
                                lstPcFilter.AddRange(data.Where(c => c.ProductLine == x));
                            }
                        }
                        else
                        {
                            if (x.ToUpper() == "FLUIDS")
                            {
                                lstPcFilter.AddRange(lstAppFilter.Where(c => c.ProductLine == "FTS" || c.ProductLine == "FBD" || c.ProductLine == "FLUIDS"));
                            }
                            else
                            {
                                lstPcFilter.AddRange(lstAppFilter.Where(c => c.ProductLine == x));
                            }
                        }
                    }
                    if (lstPcFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                else if (lstAppFilter.Count > 0)
                {
                    lstPcFilter = lstAppFilter;
                }
                if (!selVal_Region.Contains("ALL") && selVal_Region != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_Region = selVal_Region.Split(',');
                    lst_Region = sv_Region.ToList();
                    foreach (var x in lst_Region)
                    {
                        if (lstPcFilter.Count == 0)
                        {
                            lstRegionFilter.AddRange(data.Where(c => c.Region == x));
                        }
                        else
                        {
                            lstRegionFilter.AddRange(lstPcFilter.Where(c => c.Region == x));
                        }
                    }
                    if (lstRegionFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                else if (lstPcFilter.Count > 0)
                {
                    lstRegionFilter = lstPcFilter;
                }
                if (!selVal_Hardware.Contains("ALL") && selVal_Hardware != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_Hardware = selVal_Hardware.Split(',');
                    lst_Hardware = sv_Hardware.ToList();
                    foreach (var x in lst_Hardware)
                    {
                        if (lstRegionFilter.Count == 0)
                        {
                            if (x.ToUpper() == "VDI")
                            {
                                lstHardwareFilter.AddRange(data.Where(c => c.IsVDI == true));
                            }
                            if (x.ToUpper() == "NON-VDI")
                            {
                                lstHardwareFilter.AddRange(data.Where(c => c.IsVDI == false));
                            }
                        }
                        else
                        {
                            if (x.ToUpper() == "VDI")
                            {
                                lstHardwareFilter.AddRange(lstRegionFilter.Where(c => c.IsVDI == true));
                            }
                            if (x.ToUpper() == "NON-VDI")
                            {
                                lstHardwareFilter.AddRange(lstRegionFilter.Where(c => c.IsVDI == false));
                            }
                        }
                    }
                    if (lstHardwareFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                else if (lstRegionFilter.Count > 0)
                {
                    lstHardwareFilter = lstRegionFilter;
                }
                if (!selVal_Domain.Contains("ALL") && selVal_Domain != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_Domain = selVal_Domain.Split(',');
                    lst_Domain = sv_Domain.ToList();
                    foreach (var x in lst_Domain)
                    {
                        if (lstHardwareFilter.Count == 0)
                        {
                            lstDomainFilter.AddRange(data.Where(c => c.Domain == x));
                            //if (x.ToUpper() == "TATA")
                            //{
                            //    lstDomainFilter.AddRange(data.Where(c => c.Domain == "MYCORP"));
                            //}
                            //else if (x.ToUpper() == "COOPER")
                            //{
                            //    lstDomainFilter.AddRange(data.Where(c => c.Domain == "CSAID" || c.Domain == "AUBURN" || c.Domain == "ASIA"));
                            //}
                            //else if (x.ToUpper() == "MERGON")
                            //{
                            //    lstDomainFilter.AddRange(data.Where(c => c.Domain == "MERGON"));
                            //}
                            //else if (x.ToUpper() == "MTC")
                            //{
                            //    lstDomainFilter.AddRange(data.Where(c => c.Domain == "MTC"));
                            //}
                        }
                        else
                        {
                            lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == x));
                            //if (x.ToUpper() == "TATA")
                            //{
                            //    lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == "MYCORP"));
                            //}
                            //else if (x.ToUpper() == "COOPER")
                            //{
                            //    lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == "CSAID" || c.Domain == "AUBURN" || c.Domain == "ASIA"));
                            //}
                            //else if (x.ToUpper() == "MERGON")
                            //{
                            //    lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == "MERGON"));
                            //}
                            //else if (x.ToUpper() == "MTC")
                            //{
                            //    lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == "MTC"));
                            //}
                        }
                    }
                    if (lstDomainFilter.Count == 0)
                    {
                        nullValue = true;
                    }

                }
                else if (lstHardwareFilter.Count > 0)
                {
                    lstDomainFilter = lstHardwareFilter;
                }
                if (fromdate > Convert.ToDateTime("2019-01-01") && todate > Convert.ToDateTime("2019-01-01") && nullValue == false)
                {
                    anyfilter = true;
                    if (lstDomainFilter.Count == 0)
                    {
                        todate = todate.AddDays(1);
                        lstDateFilter.AddRange(data.Where(c => c.StartTime > fromdate && c.StartTime <= todate));
                    }
                    else
                    {
                        todate = todate.AddDays(1);
                        lstDateFilter.AddRange(lstDomainFilter.Where(c => c.StartTime > fromdate && c.StartTime <= todate));
                    }
                }

                var data2 = anyfilter==false ? data.GroupBy(x => x.CadTool).Select(g => new { title = g.Key, value = g.Count() }).ToList() : lstDateFilter.GroupBy(x => x.CadTool).Select(g => new { title = g.Key, value = g.Count() }).ToList();
                List<String> appNames = ctx.ToolTracking.Where(x => x.IsProd == true && x.Status == "Success" && x.StartTime > fromdate && x.StartTime <= todate).Select(x => x.ApplicationName.Trim().ToUpper())
                    .Distinct().OrderBy(x => x).ToList();
                List<string> dataIn = new List<string>();
                List<string> dataIn2 = new List<string>();
                foreach (var dta in data2)
                {
                    dataIn2.Add(dta.title);
                }
                List<BasicChartData> kebStats = new List<BasicChartData>();
                List<BasicChartData> tempkebStats = new List<BasicChartData>();
                string xmlpath = "";
                XmlDocument xmlfile = new XmlDocument();
                string[] removelist = { "COMBO BLOCK", "SMOOTH BLOCK", "CONVOLUTE BLOCK", "BLOCK CREATION", "EXECUTION", "CREATE BLOCK", "SMOOTH BORE" };
                System.Xml.XmlElement nodesfrmConfig;
                xmlpath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

                if (System.IO.File.Exists(xmlpath))
                {
                    xmlfile.Load(xmlpath);
                }

                string formattedStr2 = string.Empty;
                String newname = string.Empty;                
                if (smtCVTSel != false || selVal_App.Contains("ALL")||pmcSel!=false || selVal_App == "")
                {
                    for (int i = 0; i < 2; i++)
                    {
                        BasicChartData ObjKBEappdata = new BasicChartData();
                        ObjKBEappdata.title = GetCADSoftware(i + 1);
                        dataIn.Add(ObjKBEappdata.title);
                        foreach (var app in appNames)
                        {
                            formattedStr2 = app.Replace(" ", "").Replace("_", "").ToUpper();
                            nodesfrmConfig = xmlfile.SelectSingleNode("/configuration/myCustomElement") as XmlElement;
                            XmlNodeList elemlst = nodesfrmConfig.GetElementsByTagName(formattedStr2);
                            //System.Xml.XmlNodeList elemlist = xmlfile.GetElementsByTagName(formattedStr2);
                            if (elemlst != null && elemlst.Count > 0)
                            {
                                newname = elemlst.Item(0).InnerText;
                            }
                            else
                            {
                                newname = app;
                            }
                            string aName = app;
                            var fRes =anyfilter==false? ctx.ToolTracking.Where(x => x.CadTool == ObjKBEappdata.title && x.ApplicationName == aName && x.IsProd == true && x.Status == "Success" && x.StartTime > fromdate && x.StartTime <= todate).
                               GroupBy(x => x.Functionality).Select(g => new { funcTitle = g.Key, funcVal = g.Count(), })
                               .OrderBy(x => x.funcTitle).ToList(): lstDateFilter.Where(x => x.CadTool == ObjKBEappdata.title && x.ApplicationName == aName && x.IsProd == true && x.Status == "Success" && x.StartTime > fromdate && x.StartTime <= todate).
                               GroupBy(x => x.Functionality).Select(g => new { funcTitle = g.Key, funcVal = g.Count(), })
                               .OrderBy(x => x.funcTitle).ToList();
                            foreach (var fR in fRes)
                            {
                                if (pmcSel == true&&selVal_App.Contains(aName) || selVal_App.Contains("ALL")|| selVal_App == "")
                                {
                                    if (newname == "FTS PMC" || newname == "FBD PMC")
                                    {
                                        if (fR.funcTitle.ToUpper() == "VALIDATION")
                                        {
                                            ObjKBEappdata.value = ObjKBEappdata.value + fR.funcVal;
                                        }
                                    }
                                }
                                if (oldappsSel == true || selVal_App.Contains("ALL")|| selVal_App == "")
                                {
                                    if (oldApps.Contains(newname.ToUpper()))
                                    {
                                        ObjKBEappdata.value = ObjKBEappdata.value + fR.funcVal;
                                    }
                                }
                                if (smtCVTSel == true || selVal_App.Contains("ALL")||selVal_App == "")
                                {
                                    if (newname == "SMART CVT")
                                    {
                                        if (removelist.Contains(fR.funcTitle.ToUpper()))
                                        {
                                            ObjKBEappdata.value = ObjKBEappdata.value + fR.funcVal;
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                        tempkebStats.Add(ObjKBEappdata);
                    }
                }
                
                bool existdata = false;
                //List<BasicChartData> kebStats = new List<BasicChartData>();

                for (int i = 0; i < 2; i++)
                {
                    if(data2.Count>0)
                    {
                        BasicChartData ObjKBEappdata = new BasicChartData();
                        ObjKBEappdata.title = GetCADSoftware(i + 1);

                        foreach (var x in data2)
                        {
                            if (x.title == "SOLIDWORKS")
                            {
                                continue;
                            }
                            if (x.title == GetCADSoftware(i + 1))
                            {
                                if (tempkebStats.Count > 0)
                                {
                                    foreach (var exDet in tempkebStats)
                                    {
                                        if (exDet.title == x.title)
                                        {
                                            if (exDet.value > 0)
                                            {
                                                ObjKBEappdata.value = x.value - exDet.value;
                                                existdata = true;
                                                break;
                                            }
                                            else
                                            {
                                                ObjKBEappdata.value = x.value;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    break;

                                }
                                else
                                {
                                    ObjKBEappdata.value = x.value;
                                    break;
                                }
                            }
                            else
                            {
                                ObjKBEappdata.value = 0;
                                if (dataIn.Contains(ObjKBEappdata.title) || dataIn2.Contains(ObjKBEappdata.title))
                                {
                                    continue;
                                }
                                //kebStats.Add(ObjKBEappdata);  commented by ibrahim point chart issue in CAD utilization graph 02/05/2023
                                break;

                            }
                            if (existdata == true)
                            {
                                break;
                            }

                        }                        
                       kebStats.Add(ObjKBEappdata);

                    }
                    else
                    {
                        BasicChartData ObjKBEappdata = new BasicChartData();
                        ObjKBEappdata.title = GetCADSoftware(i + 1);
                        ObjKBEappdata.value = 0;
                        kebStats.Add(ObjKBEappdata);
                    }

                }
                return Json(kebStats, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult MonthlyUtilizationFilter(string APP, string PC, string Region, string CAD, string Hardware, string Domain,
            DateTime fromdate, DateTime todate)
            //utilization over the year when filter apply in reports tab
        {

            using (var ctx = new UMTContext())
            {
                bool smtCVTSel = false;
                string[] smtCVT = { "SMART CVT", "SMART CVT DESIGN TOOL", "SMOOTH BLOCK", "CONVOLUTE BLOCK", "COMBO BLOCK", "SMART CVT NX" };
                string[] pmcLst = {"FTS PMC","FBD PMC","FTS PMC VALIDATION","FTS PMC EXECUTION","FBD PMC VALIDATION","FBD PMC EXECUTION",
                                    "SEALING PMC VALIDATION","SEALING PMC EXECUTION"};
                string[] oldApps = { "CLIPS AND ADAPTORS", "HIT", "FLANGE THICKNESS" };
                bool oldAppsSel = false;
                bool pmcSel = false;
                var data2 = ctx.ToolTracking.Where(c =>c.IsProd == true&& c.Status == "Success");
                // c.StartTime.Year == DateTime.Now.Year && 
                List<ToolTrackingDetails> lstAppFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstPcFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstRegionFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstCadAppsFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstHardwareFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstDomainFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstDateFilter = new List<ToolTrackingDetails>();

                bool anyfilter = false;
                bool nullValue = false;
                string[] selVal_AppArr = Request.Form.GetValues("UoyApps");
                string selVal_App = selVal_AppArr[0].ToUpper();

                string[] selVal_PcArr = Request.Form.GetValues("UoyPc");
                string selVal_Pc = selVal_PcArr[0].ToUpper();

                string[] selVal_RegionArr = Request.Form.GetValues("UoyRegion");
                string selVal_Region = selVal_RegionArr[0].ToUpper();

                string[] selVal_CadAppsArr = Request.Form.GetValues("UoyCadApps");
                string selVal_CadApps = selVal_CadAppsArr[0].ToUpper();

                string[] selVal_HardwareArr = Request.Form.GetValues("UoyHardware");
                string selVal_Hardware = selVal_HardwareArr[0].ToUpper();

                string[] selVal_DomainArr = Request.Form.GetValues("UoyDomain");
                string selVal_Domain = selVal_DomainArr[0].ToUpper();

                bool notSelect = false;
                if (selVal_Pc == "" || selVal_Region == "" || selVal_CadApps == ""|| selVal_App == "" || selVal_Hardware == "" || selVal_Domain == "")
                {
                    notSelect = true;
                }

                List<string> lst_app = new List<string>();
                List<string> lst_Pc = new List<string>();
                List<string> lst_Region = new List<string>();
                List<string> lst_Domain = new List<string>();
                List<string> lst_Hardware = new List<string>();
                List<string> lst_CadApps = new List<string>();

                if (!selVal_App.Contains("ALL") && selVal_App != "" && notSelect == false)
                {
                    anyfilter = true;
                    string[] sv_App = selVal_App.Split(',');
                    lst_app = sv_App.ToList();
                    foreach (var x in lst_app)
                    {
                        if (smtCVT.Contains(x.ToUpper()))
                        {
                            smtCVTSel = true;
                        }
                        else if (pmcLst.Contains(x.ToUpper()))
                        {
                            pmcSel = true;
                        }
                        else if (oldApps.Contains(x.ToUpper()))
                        {
                            oldAppsSel = true;
                        }                        
                        lstAppFilter.AddRange(data2.Where(c => c.ApplicationName == x && c.IsProd == true && c.Status == "Success"));
                    }
                    if (lstAppFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                if (!selVal_Pc.Contains("ALL") && selVal_Pc != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_Pc = selVal_Pc.Split(',');
                    lst_Pc = sv_Pc.ToList();
                    foreach (var x in lst_Pc)
                    {
                        if (lstAppFilter.Count == 0)
                        {
                            if (x.ToUpper() == "FLUIDS")
                            {
                                lstPcFilter.AddRange(data2.Where(c => c.ProductLine == "FTS" || c.ProductLine == "FBD" || c.ProductLine == "FLUIDS"));
                            }
                            else
                            {
                                lstPcFilter.AddRange(data2.Where(c => c.ProductLine == x));
                            }
                        }
                        else
                        {
                            if (x.ToUpper() == "FLUIDS")
                            {
                                lstPcFilter.AddRange(lstAppFilter.Where(c => c.ProductLine == "FTS" || c.ProductLine == "FBD" || c.ProductLine == "FLUIDS"));
                            }
                            else
                            {
                                lstPcFilter.AddRange(lstAppFilter.Where(c => c.ProductLine == x));
                            }
                        }                        
                    }
                    if (lstPcFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                else if (lstAppFilter.Count > 0)
                {
                    lstPcFilter = lstAppFilter;
                }
                if (!selVal_Region.Contains("ALL") && selVal_Region != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_Region = selVal_Region.Split(',');
                    lst_Region = sv_Region.ToList();
                    foreach (var x in lst_Region)
                    {
                        if (lstPcFilter.Count == 0)
                        {
                            lstRegionFilter.AddRange(data2.Where(c => c.Region == x));
                        }
                        else
                        {
                            lstRegionFilter.AddRange(lstPcFilter.Where(c => c.Region == x));
                        }                      

                    }
                    if (lstRegionFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                else if (lstPcFilter.Count > 0)
                {
                    lstRegionFilter = lstPcFilter;
                }
                if (!selVal_CadApps.Contains("ALL") && selVal_CadApps != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_CadApps = selVal_CadApps.Split(',');
                    lst_CadApps = sv_CadApps.ToList();
                    foreach (var x in lst_CadApps)
                    {
                        if (lstRegionFilter.Count == 0)
                        {
                            lstCadAppsFilter.AddRange(data2.Where(c => c.CadTool == x));
                        }
                        else
                        {
                            lstCadAppsFilter.AddRange(lstRegionFilter.Where(c => c.CadTool == x));
                        }                        
                    }
                    if (lstCadAppsFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                else if (lstRegionFilter.Count > 0)
                {
                    lstCadAppsFilter = lstRegionFilter;
                }
                if (!selVal_Hardware.Contains("ALL") && selVal_Hardware != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_Hardware = selVal_Hardware.Split(',');
                    lst_Hardware = sv_Hardware.ToList();
                    foreach (var x in lst_Hardware)
                    {
                        if (lstCadAppsFilter.Count == 0)
                        {
                            if (x.ToUpper() == "VDI")
                            {
                                lstHardwareFilter.AddRange(data2.Where(c => c.IsVDI == true));
                            }
                            if (x.ToUpper() == "NON-VDI")
                            {
                                lstHardwareFilter.AddRange(data2.Where(c => c.IsVDI == false));
                            }
                        }
                        else
                        {
                            if (x.ToUpper() == "VDI")
                            {
                                lstHardwareFilter.AddRange(lstCadAppsFilter.Where(c => c.IsVDI == true));
                            }
                            if (x.ToUpper() == "NON-VDI")
                            {
                                lstHardwareFilter.AddRange(lstCadAppsFilter.Where(c => c.IsVDI == false));
                            }
                        }                        
                    }
                    if (lstHardwareFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                else if (lstCadAppsFilter.Count > 0)
                {
                    lstHardwareFilter = lstCadAppsFilter;
                }
                if (!selVal_Domain.Contains("ALL") && selVal_Domain != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_Domain = selVal_Domain.Split(',');
                    lst_Domain = sv_Domain.ToList();                    
                    foreach (var x in lst_Domain)
                    {
                        if (lstHardwareFilter.Count == 0)
                        {
                            lstDomainFilter.AddRange(data2.Where(c => c.Domain == x));
                            //if (x.ToUpper() == "TATA")
                            //{
                            //    lstDomainFilter.AddRange(data2.Where(c => c.Domain == "MYCORP"));
                            //}
                            //else if (x.ToUpper() == "COOPER")
                            //{
                            //    lstDomainFilter.AddRange(data2.Where(c => c.Domain == "CSAID" || c.Domain == "AUBURN" || c.Domain == "ASIA"));
                            //}
                            //else if (x.ToUpper() == "MERGON")
                            //{
                            //    lstDomainFilter.AddRange(data2.Where(c => c.Domain == "MERGON"));
                            //}
                            //else if (x.ToUpper() == "MTC")
                            //{
                            //    lstDomainFilter.AddRange(data2.Where(c => c.Domain == "MTC"));
                            //}
                        }
                        else
                        {
                            lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == x));
                            //if (x.ToUpper() == "TATA")
                            //{
                            //    lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == "MYCORP"));
                            //}
                            //else if (x.ToUpper() == "COOPER")
                            //{
                            //    lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == "CSAID" || c.Domain == "AUBURN" || c.Domain == "ASIA"));
                            //}
                            //else if (x.ToUpper() == "MERGON")
                            //{
                            //    lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == "MERGON"));
                            //}
                            //else if (x.ToUpper() == "MTC")
                            //{
                            //    lstDomainFilter.AddRange(lstHardwareFilter.Where(c => c.Domain == "MTC"));
                            //}
                        }                                              
                    }
                    if (lstDomainFilter.Count == 0)
                    {
                        nullValue = true;
                    }

                }
                else if (lstHardwareFilter.Count > 0)
                {
                    lstDomainFilter = lstHardwareFilter;
                }
                if (fromdate > Convert.ToDateTime("2019-01-01") && todate > Convert.ToDateTime("2019-01-01") && nullValue == false)
                {
                    anyfilter = true;
                    if (lstDomainFilter.Count == 0)
                    {
                        todate = todate.AddDays(1);
                        lstDateFilter.AddRange(data2.Where(c => c.StartTime > fromdate && c.StartTime <= todate));
                    }
                    else
                    {
                        todate = todate.AddDays(1);
                        lstDateFilter.AddRange(lstDomainFilter.Where(c => c.StartTime > fromdate && c.StartTime <= todate));
                    }

                }

                var data4 = anyfilter==false ? data2.Select(c => new { c.StartTime.Year, c.StartTime.Month })
                          .GroupBy(c => new { c.Month })
                          .Select(grouped => new { value = grouped.Count(), title = grouped.Key.Month }).OrderBy(x => x.title).ToList() : lstDateFilter.Select(c => new { c.StartTime.Year, c.StartTime.Month })
                          .GroupBy(c => new { c.Month })
                          .Select(grouped => new { value = grouped.Count(), title = grouped.Key.Month }).OrderBy(x => x.title).ToList();


                var data = data4.Select(x => new { title = Getmonth(x.title), value = x.value }).ToList();
                List<String> appNames = ctx.ToolTracking.Where(x => x.IsProd == true && x.Status == "Success" && x.StartTime > fromdate && x.StartTime <= todate).Select(x => x.ApplicationName.Trim().ToUpper())
                    .Distinct().OrderBy(x => x).ToList();
                string[] removelist = { "COMBO BLOCK", "SMOOTH BLOCK", "CONVOLUTE BLOCK", "BLOCK CREATION", "EXECUTION", "CREATE BLOCK", "SMOOTH BORE" };
                List<MonthlyData> lstMonthlyData = new List<MonthlyData>();
                List<MonthlyData> templstMonthlyData = new List<MonthlyData>();
                string xmlpath = "";
                //string[] oldApps = { "CLIPS AND ADAPTORS", "HIT", "FLANGE THICKNESS" };
                List<string> dataIn = new List<string>();
                List<string> dataIn2 = new List<string>();
                foreach(var dta in data)
                {
                    dataIn2.Add(dta.title);
                }
                XmlDocument xmlfile = new XmlDocument();
                bool existdata = false;
                bool SmartCVT = false;
                System.Xml.XmlElement nodesfrmConfig;
                xmlpath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                if (System.IO.File.Exists(xmlpath))
                {
                    xmlfile.Load(xmlpath);
                }
                string formattedStr2 = string.Empty;
                string newname = string.Empty;
                if(smtCVTSel!=false|| selVal_App.Contains("ALL")||selVal_App==""||oldAppsSel!=false||pmcSel!=false)
                {
                    for (int i = 0; i < data.Count(); i++)
                    {
                        dataIn.Add(data[i].title);
                        MonthlyData ObjOneMonthdata = new MonthlyData();
                        ObjOneMonthdata.title = data[i].title;
                        ObjOneMonthdata.value = 0;
                        int monthID = Getmonthindex(ObjOneMonthdata.title);
                        foreach (var app in appNames)
                        {

                            formattedStr2 = app.Replace(" ", "").Replace("_", "").ToUpper();
                            nodesfrmConfig = xmlfile.SelectSingleNode("/configuration/myCustomElement") as XmlElement;
                            XmlNodeList elemlst = nodesfrmConfig.GetElementsByTagName(formattedStr2);
                            //System.Xml.XmlNodeList elemlist = xmlfile.GetElementsByTagName(formattedStr2);
                            if (elemlst != null && elemlst.Count > 0)
                            {
                                newname = elemlst.Item(0).InnerText;
                            }
                            else
                            {
                                newname = app;
                            }
                            string aName = app.ToUpper();
                            //todate = todate.AddDays(1);
                            var fRes = anyfilter==false? ctx.ToolTracking.Where(x => x.ApplicationName.ToUpper() == aName && x.IsProd == true && x.Status == "Success"
                           && x.StartTime > fromdate && x.StartTime <= todate && x.StartTime.Month == monthID).
                               GroupBy(x => x.Functionality).Select(g => new { funcTitle = g.Key, funcVal = g.Count(), })
                               .OrderBy(x => x.funcTitle).ToList(): lstDateFilter.Where(x => x.ApplicationName.ToUpper() == aName && x.IsProd == true && x.Status == "Success"
                            && x.StartTime > fromdate && x.StartTime <= todate && x.StartTime.Month == monthID).
                               GroupBy(x => x.Functionality).Select(g => new { funcTitle = g.Key, funcVal = g.Count(), })
                               .OrderBy(x => x.funcTitle).ToList();
                            foreach (var fR in fRes)
                            {
                                if (pmcSel == true && selVal_App.Contains(aName)|| selVal_App.Contains("ALL") || selVal_App == "")
                                {
                                    if (newname == "FTS PMC" || newname == "FBD PMC")
                                    {
                                        if (fR.funcTitle.ToUpper() == "VALIDATION")
                                        {
                                            ObjOneMonthdata.value = ObjOneMonthdata.value + fR.funcVal;
                                        }
                                    }
                                } 
                                if(oldAppsSel == true || selVal_App.Contains("ALL") || selVal_App == "")
                                {
                                    if (oldApps.Contains(newname.ToUpper()))
                                    {
                                        ObjOneMonthdata.value = ObjOneMonthdata.value + fR.funcVal;
                                    }
                                }
                                if(smtCVTSel == true || selVal_App.Contains("ALL") || selVal_App == "")
                                {
                                    if (newname == "SMART CVT")
                                    {
                                        if (removelist.Contains(fR.funcTitle.ToUpper()))
                                        {
                                            ObjOneMonthdata.value = ObjOneMonthdata.value + fR.funcVal;
                                            SmartCVT = true;
                                            continue;
                                        }
                                    }
                                }                                
                            }
                        }
                        //lstMonthlyData.Add(ObjOneMonthdata);
                        templstMonthlyData.Add(ObjOneMonthdata);
                    }
                }
                

                for (int i = 0; i < 12; i++)
                {   
                    if(data.Count>0)
                    {
                        foreach (var x in data)
                        {
                            MonthlyData ObjOneMonthdata = new MonthlyData();
                            ObjOneMonthdata.title = Getmonth(i + 1);
                            if (x.title == Getmonth(i + 1))
                            {

                                if (templstMonthlyData.Count > 0)
                                {
                                    foreach (var exDet in templstMonthlyData)
                                    {
                                        if (exDet.title == x.title)
                                        {
                                            if (exDet.value > 0)
                                            {
                                                ObjOneMonthdata.value = x.value - exDet.value;
                                                break;
                                            }
                                            else
                                            {
                                                ObjOneMonthdata.value = x.value;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                }

                            }
                            else
                            {
                                ObjOneMonthdata.value = 0;
                                if (dataIn.Contains(ObjOneMonthdata.title) || dataIn2.Contains(ObjOneMonthdata.title))
                                {
                                    continue;
                                }
                                lstMonthlyData.Add(ObjOneMonthdata);
                                break;
                            }
                            bool selApp = false;
                            if (smtCVTSel == false && !selVal_App.Contains("ALL")&&selVal_App!=""&&pmcSel==false&&oldAppsSel==false)
                            {
                                ObjOneMonthdata.value = x.value;
                                selApp = true;
                            }
                            lstMonthlyData.Add(ObjOneMonthdata);
                            break;

                        }
                    }
                    else
                    {
                        MonthlyData ObjOneMonthdata = new MonthlyData();
                        ObjOneMonthdata.title = Getmonth(i + 1);
                        ObjOneMonthdata.value = 0;
                        lstMonthlyData.Add(ObjOneMonthdata);
                    }
                    
                }
                return Json(lstMonthlyData, JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult IndexMonthlyUtilizationFilter(DateTime fromDate, DateTime todate)
            //monthly utilization over the year when filter apply in home tab
        {

            using (var ctx = new UMTContext())
            {
                var data2 = ctx.ToolTracking.Where(c =>  c.IsProd == true&& c.Status == "Success");
                //c.StartTime.Year == DateTime.Now.Year &&

                if (fromDate > Convert.ToDateTime("2019-01-01"))
                {
                    data2 = data2.Where(c => c.StartTime > fromDate);
                }
                if (todate > Convert.ToDateTime("2019-01-01"))
                {
                    todate = todate.AddDays(1);
                    data2 = data2.Where(c => c.StartTime <= todate);
                }

                var data3 = data2.Select(c => new { c.StartTime.Year, c.StartTime.Month })
                          .GroupBy(c => new { c.Month })
                          .Select(grouped => new { value = grouped.Count(), title = grouped.Key.Month }).OrderBy(c=>c.title ).ToList();


                var data = data3.Select(x => new { title = Getmonth(x.title), value = x.value }).ToList();
                
                List<String> appNames = ctx.ToolTracking.Where(x => x.IsProd == true && x.Status == "Success"&&x.StartTime > fromDate && x.StartTime <= todate).Select(x => x.ApplicationName.Trim().ToUpper())
                    .Distinct().OrderBy(x => x).ToList();
                string[] removelist = { "COMBO BLOCK", "SMOOTH BLOCK", "CONVOLUTE BLOCK", "BLOCK CREATION", "EXECUTION", "CREATE BLOCK", "SMOOTH BORE" };
                List<MonthlyData> lstMonthlyData = new List<MonthlyData>();
                List<MonthlyData> templstMonthlyData = new List<MonthlyData>();
                string xmlpath = "";
                string[] oldApps = { "CLIPS AND ADAPTORS", "HIT", "FLANGE THICKNESS" };
                List<string >dataIn = new List<string> ();
                XmlDocument xmlfile = new XmlDocument();
                bool existdata = false;
                bool SmartCVT = false;
                System.Xml.XmlElement nodesfrmConfig;
                xmlpath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                if (System.IO.File.Exists(xmlpath))
                {
                    xmlfile.Load(xmlpath);
                }
                string formattedStr2 = string.Empty;
                string newname = string.Empty;
                for (int i = 0; i < data.Count(); i++)
                {
                    dataIn.Add(data[i].title);
                    MonthlyData ObjOneMonthdata = new MonthlyData();
                    ObjOneMonthdata.title = data[i].title;                   
                    ObjOneMonthdata.value = 0;
                    int monthID = Getmonthindex(ObjOneMonthdata.title);
                    foreach (var app in appNames)
                    {

                        formattedStr2 = app.Replace(" ", "").Replace("_", "").ToUpper();
                        nodesfrmConfig = xmlfile.SelectSingleNode("/configuration/myCustomElement") as XmlElement;
                        XmlNodeList elemlst = nodesfrmConfig.GetElementsByTagName(formattedStr2);
                        //System.Xml.XmlNodeList elemlist = xmlfile.GetElementsByTagName(formattedStr2);
                        if (elemlst != null && elemlst.Count > 0)
                        {
                            newname = elemlst.Item(0).InnerText;
                        }
                        else
                        {
                            newname = app;
                        }
                        string aName = app.ToUpper();
                        //todate = todate.AddDays(1);
                        var fRes = ctx.ToolTracking.Where(x => x.ApplicationName == aName && x.IsProd == true && x.Status == "Success"
                       && x.StartTime > fromDate && x.StartTime <= todate && x.StartTime.Month == monthID).
                           GroupBy(x => x.Functionality).Select(g => new { funcTitle = g.Key, funcVal = g.Count(), })
                           .OrderBy(x => x.funcTitle).ToList();
                        foreach (var fR in fRes)
                        {
                            if (newname == "FTS PMC" || newname == "FBD PMC")
                            {
                                if (fR.funcTitle.ToUpper() == "VALIDATION")
                                {
                                    ObjOneMonthdata.value = ObjOneMonthdata.value + fR.funcVal;
                                }
                            }
                            if (oldApps.Contains(newname.ToUpper()))
                            {
                                ObjOneMonthdata.value = ObjOneMonthdata.value + fR.funcVal;
                            }
                            if (newname == "SMART CVT")
                            {
                                if (removelist.Contains(fR.funcTitle.ToUpper()))
                                {
                                    ObjOneMonthdata.value = ObjOneMonthdata.value + fR.funcVal;
                                    SmartCVT = true;
                                    continue;
                                }
                            }
                        }


                    }
                    //lstMonthlyData.Add(ObjOneMonthdata);
                    templstMonthlyData.Add(ObjOneMonthdata);
                }
                for (int i = 0; i < 12; i++)
                {                   
                    
                    int loopIndex = 0;
                    foreach (var x in data)
                    {
                        loopIndex = loopIndex + 1;
                        MonthlyData ObjOneMonthdata = new MonthlyData();
                        ObjOneMonthdata.title = Getmonth(i + 1);
                        if (x.title == Getmonth(i + 1))
                        { 
                            
                            if (templstMonthlyData.Count > 0)
                            {
                                foreach (var exDet in templstMonthlyData)
                                {
                                    if(exDet.title == x.title) 
                                    {
                                        if (exDet.value > 0)
                                        {
                                            ObjOneMonthdata.value = x.value - exDet.value;                                            
                                            break;
                                        }
                                        else
                                        {
                                            ObjOneMonthdata.value = x.value;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    
                                    

                                }
                            }                           
                            
                        }
                        else
                        {
                            ObjOneMonthdata.value = 0;
                            if(dataIn.Contains(ObjOneMonthdata.title))
                            {
                                continue;
                            }
                            lstMonthlyData.Add(ObjOneMonthdata);
                            break;
                        }

                        lstMonthlyData.Add(ObjOneMonthdata);
                        break;

                    }
                    
                }
                return Json(lstMonthlyData, JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult PercentageUtilizationFilter(string PC, string Region,string Domain, string CAD, DateTime fromdate, DateTime todate)
            //percentage utilization
        {

            using (var ctx = new UMTContext())
            {
                var data = ctx.ToolTracking.Where(c =>c.IsProd == true&& c.Status == "Success");
                List<ToolTrackingDetails> lstPcFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstRegionFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstDomainFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstCadAppsFilter = new List<ToolTrackingDetails>();
                List<ToolTrackingDetails> lstDateFilter = new List<ToolTrackingDetails>();

                bool anyfilter = false;
                bool notSelect = false;
                bool nullValue = false;
                string[] selVal_PcArr = Request.Form.GetValues("PumPc");
                string selVal_Pc = selVal_PcArr[0].ToUpper();

                string[] selVal_RegionArr = Request.Form.GetValues("PumRegion");
                string selVal_Region = selVal_RegionArr[0].ToUpper();

                string[] selVal_DomainArr = Request.Form.GetValues("PumDomain");
                string selVal_Domain = selVal_DomainArr[0].ToUpper();

                string[] selVal_CadAppsArr = Request.Form.GetValues("PumCadApps");
                string selVal_CadApps = selVal_CadAppsArr[0].ToUpper(); 
                
                if(selVal_Pc==""|| selVal_Region == "" || selVal_Domain == "" || selVal_CadApps == "")
                {
                    notSelect = true;
                }


                
                List<string> lst_Pc = new List<string>();
                List<string> lst_Region = new List<string>();
                List<string> lst_Domain = new List<string>();
                List<string> lst_CadApps = new List<string>();

                if (!selVal_Pc.Contains("ALL") && selVal_Pc != "" && notSelect == false)
                {
                    anyfilter = true;
                    string[] sv_Pc = selVal_Pc.Split(',');
                    lst_Pc = sv_Pc.ToList();
                    foreach (var x in lst_Pc)
                    {
                        if (x.ToUpper() == "FLUIDS")
                        {
                            lstPcFilter.AddRange(data.Where(c => c.ProductLine == "FTS" || c.ProductLine == "FBD" || c.ProductLine == "FLUIDS"));
                        }
                        else
                        {
                            lstPcFilter.AddRange(data.Where(c => c.ProductLine == x));
                        }
                    }
                    if (lstPcFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                if (!selVal_Region.Contains("ALL") && selVal_Region != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_Region = selVal_Region.Split(',');
                    lst_Region = sv_Region.ToList();
                    foreach (var x in lst_Region)
                    {
                        if (lstPcFilter.Count == 0)
                        {
                            lstRegionFilter.AddRange(data.Where(c => c.Region == x));
                        }
                        else
                        {
                            lstRegionFilter.AddRange(lstPcFilter.Where(c => c.Region == x));
                        }
                    }
                    if (lstRegionFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                else if (lstPcFilter.Count > 0)
                {
                    lstRegionFilter = lstPcFilter;
                }
                if (!selVal_Domain.Contains("ALL") && selVal_Domain != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_Domain = selVal_Domain.Split(',');
                    lst_Domain = sv_Domain.ToList();
                    foreach (var x in lst_Domain)
                    {
                        if (lstRegionFilter.Count == 0)
                        {
                            lstDomainFilter.AddRange(data.Where(c => c.Domain == x));
                        }
                        else
                        {
                            lstDomainFilter.AddRange(lstRegionFilter.Where(c => c.Domain == x));
                        }
                    }
                    if (lstDomainFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                else if (lstRegionFilter.Count > 0)
                {
                    lstDomainFilter = lstRegionFilter;
                }
                if (!selVal_CadApps.Contains("ALL") && selVal_CadApps != "" && notSelect == false && nullValue == false)
                {
                    anyfilter = true;
                    string[] sv_CadApps = selVal_CadApps.Split(',');
                    lst_CadApps = sv_CadApps.ToList();
                    foreach (var x in lst_CadApps)
                    {
                        if (lstDomainFilter.Count == 0)
                        {
                            lstCadAppsFilter.AddRange(data.Where(c => c.CadTool == x));
                        }
                        else
                        {
                            lstCadAppsFilter.AddRange(lstDomainFilter.Where(c => c.CadTool == x));
                        }
                    }
                    if (lstCadAppsFilter.Count == 0)
                    {
                        nullValue = true;
                    }
                }
                else if (lstDomainFilter.Count > 0)
                {
                    lstCadAppsFilter = lstDomainFilter;
                }
                if (fromdate > Convert.ToDateTime("2019-01-01") && todate > Convert.ToDateTime("2019-01-01") && nullValue == false)
                {
                    anyfilter = true;
                    if (lstCadAppsFilter.Count == 0)
                    {
                        todate = todate.AddDays(1);
                        lstDateFilter.AddRange(data.Where(c => c.StartTime > fromdate && c.StartTime <= todate));
                    }
                    else
                    {
                        todate = todate.AddDays(1);
                        lstDateFilter.AddRange(lstCadAppsFilter.Where(c => c.StartTime > fromdate && c.StartTime <= todate));
                    }

                }

                List<String> appNames = ctx.ToolTracking.Where(x => x.IsProd == true && x.Status == "Success").Select(x => x.ApplicationName.Trim().ToUpper())
                    .Distinct().OrderBy(x => x).ToList();

                long totalCount =anyfilter==false? data.Count():lstDateFilter.Count();
                long FTSpmcValidCount = 0;
                long FBDpmcValidCount = 0;

                var data2 =anyfilter==false? data
                          .GroupBy(c => c.ApplicationName)
                          .Select(grouped => new { value = grouped.Count(), title = grouped.Key.ToUpper() }).ToList(): lstDateFilter
                          .GroupBy(c => c.ApplicationName)
                          .Select(grouped => new { value = grouped.Count(), title = grouped.Key.ToUpper() }).ToList();
                long smartCvtCount = 0;
                string xmlpath = "";
                string[] oldApps = { "CLIPS AND ADAPTORS", "HIT", "FLANGE THICKNESS" };
                XmlDocument xmlfile = new XmlDocument();
                bool existdata = false;
                System.Xml.XmlElement nodesfrmConfig;
                xmlpath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                if (System.IO.File.Exists(xmlpath))
                {
                    xmlfile.Load(xmlpath);
                }
                string formattedStr2 = string.Empty;
                string newname = string.Empty;
                foreach (var app in appNames)
                {
                    formattedStr2 = app.Replace(" ", "").Replace("_", "").ToUpper();
                    nodesfrmConfig = xmlfile.SelectSingleNode("/configuration/myCustomElement") as XmlElement;
                    XmlNodeList elemlst = nodesfrmConfig.GetElementsByTagName(formattedStr2);
                    //System.Xml.XmlNodeList elemlist = xmlfile.GetElementsByTagName(formattedStr2);
                    if (elemlst != null && elemlst.Count > 0)
                    {
                        newname = elemlst.Item(0).InnerText;
                    }
                    else
                    {
                        newname = app;
                    }
                    string aName = app.ToUpper();
                    var fRes =anyfilter==false? data.Where(x => x.ApplicationName.ToUpper() == aName && x.IsProd == true && x.Status == "Success"&& x.StartTime > fromdate && x.StartTime <= todate).
                            GroupBy(x => x.Functionality).Select(g => new { funcTitle = g.Key, funcVal = g.Count() })
                            .OrderBy(x => x.funcTitle).ToList(): lstDateFilter.Where(x => x.ApplicationName.ToUpper() == aName && x.IsProd == true && x.Status == "Success" && x.StartTime > fromdate && x.StartTime <= todate).
                            GroupBy(x => x.Functionality).Select(g => new { funcTitle = g.Key, funcVal = g.Count() })
                            .OrderBy(x => x.funcTitle).ToList();
                    string[] removelist = { "COMBO BLOCK", "SMOOTH BLOCK", "CONVOLUTE BLOCK", "BLOCK CREATION", "EXECUTION","CREATE BLOCK","SMOOTH BORE"};
                    foreach (var fR in fRes)
                    {
                        if (newname == "FTS PMC" )
                        {
                            if (fR.funcTitle.ToUpper() == "VALIDATION")
                            {
                                totalCount = totalCount - fR.funcVal;
                                FTSpmcValidCount = FTSpmcValidCount + fR.funcVal;
                                continue;
                            }
                        }
                        if(newname == "FBD PMC")
                        {
                            if (fR.funcTitle.ToUpper() == "VALIDATION")
                            {
                                totalCount = totalCount - fR.funcVal;
                                FBDpmcValidCount = FBDpmcValidCount + fR.funcVal;
                                continue;
                            }
                        }
                        if (oldApps.Contains(app.ToUpper()))
                        {
                            totalCount = totalCount - fR.funcVal;
                            continue;
                        }
                        if (newname == "SMART CVT")
                        {
                            if (removelist.Contains(fR.funcTitle.ToUpper()))
                            {
                                totalCount = totalCount - fR.funcVal;
                                smartCvtCount = smartCvtCount + fR.funcVal;
                                continue;
                            }

                        }
                    }
                }

                List<BasicChartDatadecimal> kebStats = new List<BasicChartDatadecimal>();
                string formattedStr1 = string.Empty;
                string newname1 = string.Empty;
                foreach (var item in data2)
                {
                    if(oldApps.Contains(item.title.ToUpper()))
                    {
                        continue;
                    }
                    BasicChartDatadecimal ObjKBEappdata = new BasicChartDatadecimal();
                    ObjKBEappdata.title = item.title;
                    //ObjKBEappdata.value = item.value * 100 / totalCount;
                    ObjKBEappdata.actual = item.value;
                    formattedStr1 = ObjKBEappdata.title.Replace(" ", "").Replace("_", "").ToUpper();
                    nodesfrmConfig = xmlfile.SelectSingleNode("/configuration/myCustomElement") as XmlElement;
                    XmlNodeList elemlst = nodesfrmConfig.GetElementsByTagName(formattedStr1);
                    //System.Xml.XmlNodeList elemlist = xmlfile.GetElementsByTagName(formattedStr1);
                    if (elemlst != null && elemlst.Count > 0)
                    {
                        newname1 = elemlst.Item(0).InnerText;
                        ObjKBEappdata.title = newname1;
                    }
                    if (kebStats.Count > 0)
                    {
                        int index = 0;
                        foreach (var b in kebStats)
                        {
                            if (b.title == ObjKBEappdata.title)
                            {
                                //kebStats[index].value = kebStats[index].value + ObjKBEappdata.value;
                                kebStats[index].actual = kebStats[index].actual + ObjKBEappdata.actual;
                                existdata = true;
                            }
                            index = index + 1;
                        }
                    }
                    if (existdata == false)
                    {
                        kebStats.Add(ObjKBEappdata);
                    }
                    existdata = false;
                }
                foreach (var fData in kebStats)
                {
                    if(fData.title =="FTS PMC")
                    {
                        fData.actual = fData.actual - FTSpmcValidCount;
                        fData.value = (decimal)fData.actual * 100 / totalCount;
                    }
                    if(fData.title == "FBD PMC")
                    {
                        fData.actual = fData.actual - FBDpmcValidCount;
                        fData.value = (decimal)fData.actual * 100 / totalCount;
                    }
                    if (fData.title == "SMART CVT")
                    {
                        fData.actual = fData.actual - smartCvtCount;
                        fData.value = (decimal)fData.actual * 100 / totalCount;
                    }
                    fData.value = (decimal)fData.actual * 100 / totalCount;
                }
                return Json(kebStats, JsonRequestBehavior.AllowGet);
            }

        }



        public string GetCADSoftware(int KBEindex)
        {
            string AppName = "";
            if (KBEindex == 1)
            {
                AppName = "CATIA";
            }
            if (KBEindex == 2)
            {
                AppName = "NX";
            }
            //if (KBEindex == 3)
            //{
            //    AppName = "SOLIDWORKS";
            //}
            return AppName;
        }

        public string Getmonth(int monthid)
        {
            string Month = "";
            if (monthid == 1)
            {
                Month = "JAN";
            }
            if (monthid == 2)
            {
                Month = "FEB";
            }
            if (monthid == 3)
            {
                Month = "MAR";
            }
            if (monthid == 4)
            {
                Month = "APR";
            }
            if (monthid == 5)
            {
                Month = "MAY";
            }
            if (monthid == 6)
            {
                Month = "JUN";
            }
            if (monthid == 7)
            {
                Month = "JUL";
            }
            if (monthid == 8)
            {
                Month = "AUG";
            }
            if (monthid == 9)
            {
                Month = "SEP";
            }
            if (monthid == 10)
            {
                Month = "OCT";
            }
            if (monthid == 11)
            {
                Month = "NOV";
            }
            if (monthid == 12)
            {
                Month = "DEC";
            }
            return Month;
        }

        public string[] SelectedValues(string formId)
        {
            List<string> selectedValues = new List<string>();

            int thisFormCount = Request.Form[formId].Count();


            return selectedValues.ToArray();
        }

        public int Getmonthindex(string monthname)
        {
            int Month = 0;
            if (monthname == "JAN")
            {
                Month = 1;
            }
            if (monthname == "FEB")
            {
                Month =2;
            }
            if (monthname == "MAR")
            {
                Month = 3;
            }
            if (monthname == "APR")
            {
                Month = 4;
            }
            if (monthname == "MAY")
            {
                Month = 5;
            }
            if (monthname == "JUN")
            {
                Month = 6;
            }
            if (monthname == "JUL")
            {
                Month = 7;
            }
            if (monthname == "AUG")
            {
                Month = 8;
            }
            if (monthname == "SEP")
            {
                Month = 9;
            }
            if (monthname == "OCT")
            {
                Month = 10;
            }
            if (monthname == "NOV")
            {
                Month = 11;
            }
            if (monthname == "DEC")
            {
                Month = 12;
            }
            return Month;
        }        
    }
}