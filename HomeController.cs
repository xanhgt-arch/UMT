using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UsageMonitoringTool_WebApplication.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace UsageMonitoringTool_WebApplication.Controllers
{
    public class HomeController : Controller
    {       

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string userId = User.Identity.Name.Split('\\').Last();

            ViewBag.IsAdmin = IsAdminUser(userId);

            base.OnActionExecuting(filterContext);
        }

        public ActionResult Index()
        {
            List<SelectListItem> lstAppName = new List<SelectListItem>();
            lstAppName.Add(new SelectListItem() { Text = "All", Value = "All", Selected = true });
            using (var ctx = new UMTContext())
            {
                var appData = ctx.ToolTracking.Select(x => x.ApplicationName.ToUpper()).Distinct();
                foreach (string app in appData)
                {
                    lstAppName.Add(new SelectListItem() { Text = app, Value = app });
                }
            }
            ViewBag.listItems = lstAppName;
            string ADGroupName = System.Configuration.ConfigurationManager.AppSettings["AdGroupName"];
            bool validuser = User.IsInRole(ADGroupName);

            if (validuser)
            {
                return View();
            }
            else
            {
                return View("Error");
            }
        }

        public ActionResult About()
        {
            string ADGroupName = System.Configuration.ConfigurationManager.AppSettings["AdGroupName"];
            bool validuser = User.IsInRole(ADGroupName);

            List<SelectListItem> lstAppName = new List<SelectListItem>();
            List<SelectListItem> lstDomainNames = new List<SelectListItem>();
            List<SelectListItem> lstRegions = new List<SelectListItem>();
            lstAppName.Add(new SelectListItem() { Text = "All", Value = "All", Selected = true });
            //lstDomainNames.Add(new SelectListItem() { Text = "All", Value = "All", Selected = true });
            using (var ctx = new UMTContext())
            {
                var appData = ctx.ToolTracking.Select(x => x.ApplicationName.ToUpper()).Distinct();
                foreach (string app in appData)
                {
                    lstAppName.Add(new SelectListItem() { Text = app, Value = app });
                }
                var domaindata =ctx.DomainDetails.Select(x => x.Domain.ToUpper()).Distinct().ToList();
                foreach (string domain in domaindata)
                {
                    lstDomainNames.Add(new SelectListItem() { Text = domain, Value = domain });
                }
                var regiondata = ctx.DomainDetails.Select(x => x.Region.ToUpper()).Distinct().ToList();
                foreach (string region in regiondata)
                {
                    lstRegions.Add(new SelectListItem() { Text = region, Value = region });
                }
            }
            ViewBag.listItems = lstAppName;
            ViewBag.listitems2 = lstDomainNames;
            ViewBag.listitems3 = lstRegions;
            if (validuser)
            {
                return View();
            }
            else
            {
                return View("Error");
            }
        }
        

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult VDIUserDetails()
        {
            List<VDIUserDetail> res = new List<VDIUserDetail>();
            using (var ctx = new UMTContext())
            {
                res = ctx.VDIUserDetail.OrderBy(x => x.UserId).ToList();
            }
            return View(res);
        }

        public ActionResult EditVDIUserDetails(long id)
        {
            VDIUserDetail res;
            using (var ctx = new UMTContext())
            {
                res = ctx.VDIUserDetail.Where(c => c.SrNo == id).FirstOrDefault();
            }
            return View(res);
        }

        public ActionResult DetailVDIUserDetails(long id)
        {
            VDIUserDetail res;
            using (var ctx = new UMTContext())
            {
                res = ctx.VDIUserDetail.Where(c => c.SrNo == id).FirstOrDefault();
            }
            return View(res);
        }

        public ActionResult DeleteVDIUserDetails(long id)
        {
            VDIUserDetail res;
            using (var ctx = new UMTContext())
            {
                res = ctx.VDIUserDetail.Where(c => c.SrNo == id).FirstOrDefault();
                ctx.VDIUserDetail.Remove(res);
                ctx.SaveChanges();
            }
            return RedirectToAction("VDIUserDetails");
        }

        public ActionResult CreateVDIUserDetails()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateVDIUserDetails(VDIUserDetail details)
        {
            using (var ctx = new UMTContext())
            {
                string userName = User.Identity.Name.Split('\\').Last();
                if (IsAdminUser(userName))
                {
                    details.CreatedBy = "ADMIN";
                    details.ModifiedBy = "ADMIN";
                }
                else
                {
                    details.CreatedBy = User.Identity.Name;
                    details.ModifiedBy = User.Identity.Name;
                }
                details.CreatedDate = DateTime.Now;
                details.ModifiedDate = DateTime.Now;
                ctx.VDIUserDetail.Add(details);
                ctx.SaveChanges();
            }
            return RedirectToAction("VDIUserDetails");
        }

        [HttpPost]
        public ActionResult EditVDIUserDetails(VDIUserDetail details)
        {
            VDIUserDetail res;
            using (var ctx = new UMTContext())
            {
                res = ctx.VDIUserDetail.Where(c => c.SrNo == details .SrNo ).FirstOrDefault();
                res.ModifiedDate = DateTime.Now;
                res.ModifiedBy = User.Identity.Name;
                res.UserId = details.UserId;
                res.Domain = details.Domain;
                res.Region = details.Region;
                ctx.SaveChanges();
            }
            return RedirectToAction("VDIUserDetails");
        }
        public ActionResult DomainDetails()
        {
            List<DomainDetails> res = new List<DomainDetails>();
            List<DomainDetails> finalResView = new List<DomainDetails>();
            using (var ctx = new UMTContext())
            {
                //res = ctx.DomainDetails.OrderBy(x => x.SrNo).ToList();
                res = ctx.DomainDetails.OrderByDescending(x => x.SrNo).ToList();
                foreach(var dom in res)
                {
                    dom.CreatedBy = dom.CreatedBy.Split('\\').First();
                    dom.ModifiedBy = dom.ModifiedBy.Split('\\').First();
                    finalResView.Add(dom);
                }

            }
            return View(finalResView);
        }
        public ActionResult ManageAdmin()
        {
            List<CooperAdmins> res = new List<CooperAdmins>();
            using (var ctx = new UMTContext())
            {
                //res = ctx.DomainDetails.OrderBy(x => x.SrNo).ToList();
                res = ctx.CooperAdmins.OrderByDescending(x => x.SrNo).ToList();
            }
            return View(res);
        }
        public ActionResult EditDomainDetails(long id)
        {
            using (var ctx = new UMTContext())
            {
                var model = ctx.DomainDetails.FirstOrDefault(x => x.SrNo == id);
                if (model == null)
                    return HttpNotFound();

                // Regions
                var regions = ctx.DomainDetails
                    .Select(x => x.Region.ToUpper())
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                var regionList = regions
                    .Select(r => new SelectListItem
                    {
                        Text = r,
                        Value = r,
                        Selected = (r == model.Region.ToUpper())
                    })
                    .ToList();

                regionList.Insert(0, new SelectListItem { Text = "Add New", Value = "ADD_NEW" });

                // Domains for the selected region
                var domains = ctx.DomainDetails
                    .Where(x => x.Region == model.Region)
                    .Select(x => x.Domain.ToUpper())
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                var domainList = domains
                    .Select(d => new SelectListItem
                    {
                        Text = d,
                        Value = d,
                        Selected = (d == model.Domain.ToUpper())
                    })
                    .ToList();

                domainList.Insert(0, new SelectListItem { Text = "Add New", Value = "ADD_NEW" });

                ViewBag.RegionList = regionList;
                ViewBag.DomainList = domainList;

                return View(model);
            }
        }
        

        public ActionResult DetailDomainDetails(long id)
        {
            DomainDetails res;
            using (var ctx = new UMTContext())
            {
                res = ctx.DomainDetails.Where(c => c.SrNo == id).FirstOrDefault();
            }
            return View(res);
        }

        public ActionResult DeleteDomainDetails(long id)
        {            
            DomainDetails res;
            using (var ctx = new UMTContext())
            {
                string userName = User.Identity.Name.Split('\\').Last();
                if (!IsAdminUser(userName))
                {
                    TempData["AccessDenied"] = "You do not have access to do this action.";
                    return RedirectToAction("DomainDetails");
                }
                res = ctx.DomainDetails.Where(c => c.SrNo == id).FirstOrDefault();
                ctx.DomainDetails.Remove(res);
                ctx.SaveChanges();
            }
            return RedirectToAction("DomainDetails");
        }
        public ActionResult RemoveAdmin(long id)
        {
            CooperAdmins res;
            using (var ctx = new UMTContext())
            {
                string userId = User.Identity.Name.Split('\\').Last();                
                res = ctx.CooperAdmins.Where(c => c.SrNo == id).FirstOrDefault();
                if (userId.ToUpper()==res.UserId.ToUpper())
                {
                    TempData["Error"] = "You can not remove your UserID";
                    return RedirectToAction("ManageAdmin");
                }
                ctx.CooperAdmins.Remove(res);
                ctx.SaveChanges();
            }
            return RedirectToAction("ManageAdmin");
        }

        public ActionResult CreateDomainDetails()
        {
            string userName = User.Identity.Name.Split('\\').Last();
            if (!IsAdminUser(userName))
            {
                TempData["AccessDenied"] = "You do not have access to do this action.";
                return RedirectToAction("DomainDetails");
            }
            using (var ctx = new UMTContext())
            {
                var regions = ctx.DomainDetails
                    .Select(x => x.Region.ToUpper())
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                var regionList = regions
                    .Select(r => new SelectListItem { Text = r, Value = r })
                    .ToList();

                regionList.Add(new SelectListItem { Text = "Add New", Value = "ADD_NEW" });
                regionList.Reverse();
                ViewBag.RegionList = regionList;
            }           

            return View();
        }

        [HttpPost]
        public ActionResult CreateDomainDetails(DomainDetails details, string NewRegion, string NewDomain)
        {
            using (var ctx = new UMTContext())
            {
                if (details.Region == "ADD_NEW")
                    details.Region = NewRegion;

                if (details.Domain == "ADD_NEW")
                    details.Domain = NewDomain;                
                if (string.IsNullOrEmpty(details.UserId) || string.IsNullOrEmpty(details.Domain)|| string.IsNullOrEmpty(details.Region))
                {                    
                    return RedirectToAction("CreateDomainDetails");
                }
                if (!ModelState.IsValid)
                    return RedirectToAction("CreateDomainDetails");

                bool exists = ctx.DomainDetails.Any(x => x.UserId == details.UserId);
                if (exists)
                {
                    TempData["Error"] = "User already exists";
                    return RedirectToAction("CreateDomainDetails");
                }
                bool userExists = ctx.ToolTracking.Any(x => x.UserID == details.UserId);
                if (!userExists)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction("CreateDomainDetails");
                }

                details.CreatedDate = DateTime.Now;
                details.ModifiedDate = DateTime.Now;
                string userName = User.Identity.Name.Split('\\').Last();
                details.CreatedBy = "ADMIN\\"+userName;
                details.ModifiedBy = "ADMIN\\"+userName;
                ctx.DomainDetails.Add(details);
                ctx.SaveChanges();

                UpdateDomainUsage(ctx, details.UserId);
            }

            return RedirectToAction("DomainDetails");
        }
        [HttpPost]
        public ActionResult AddNewAdmin(string userId)
        {
            using (var ctx = new UMTContext())
            {
                
                if (string.IsNullOrEmpty(userId) )
                {
                    TempData["Error"] = "Please Enter UserID";
                    return RedirectToAction("ManageAdmin");
                }
                if (!ModelState.IsValid)
                    return RedirectToAction("ManageAdmin");

                bool exists = ctx.CooperAdmins.Any(x => x.UserId == userId);
                if (exists)
                {
                    TempData["Error"] = "User already exists";
                    return RedirectToAction("ManageAdmin");
                }
                string currentUser = User.Identity.Name.Split('\\').Last();
                ctx.CooperAdmins.Add(new CooperAdmins
                {
                    UserId = userId,
                    AddedBy = currentUser,
                    AddedOn = DateTime.Now
            }); ;
                ctx.SaveChanges();
            }
            return RedirectToAction("ManageAdmin");
        }

        [HttpPost]
        public ActionResult EditDomainDetails(DomainDetails details, string NewRegion, string NewDomain)
         {
            DomainDetails res;
            using (var ctx = new UMTContext())
            {


                if (details.Region == "ADD_NEW")
                    details.Region = NewRegion;

                if (details.Domain == "ADD_NEW")
                    details.Domain = NewDomain;

                
                var regions = ctx.DomainDetails
                    .Select(x => x.Region.ToUpper())
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                var regionList = regions
                    .Select(r => new SelectListItem
                    {
                        Text = r,
                        Value = r,
                        Selected = (r == details.Region)
                    })
                    .ToList();

                regionList.Insert(0, new SelectListItem
                {
                    Text = "Add New",
                    Value = "ADD_NEW"
                });

                
                var domainList = new List<SelectListItem>();

                if (!string.IsNullOrEmpty(details.Region))
                {
                    var domains = ctx.DomainDetails
                        .Where(x => x.Region == details.Region)
                        .Select(x => x.Domain.ToUpper())
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();

                    domainList = domains
                        .Select(d => new SelectListItem
                        {
                            Text = d,
                            Value = d,
                            Selected = (d == details.Domain)
                        })
                        .ToList();
                }

                domainList.Insert(0, new SelectListItem
                {
                    Text = "Add New",
                    Value = "ADD_NEW"
                });

                ViewBag.RegionList = regionList;
                ViewBag.DomainList = domainList;

                
                if (string.IsNullOrEmpty(details.Region))
                    ModelState.AddModelError("Region", "Region is required");

                if (string.IsNullOrEmpty(details.Domain))
                    ModelState.AddModelError("Domain", "Domain is required");
                if (string.IsNullOrEmpty(details.UserId))
                    ModelState.AddModelError("UserId", "UserId is required");

                if (!ModelState.IsValid)
                {                    
                    return View(details);
                }

                res = ctx.DomainDetails.Where(c => c.SrNo == details.SrNo).FirstOrDefault();
                res.ModifiedDate = DateTime.Now;
                string userName = User.Identity.Name.Split('\\').Last();
                if (IsAdminUser(userName))
                {
                    details.ModifiedBy = "ADMIN\\"+userName;
                    res.ModifiedBy = "ADMIN\\" + userName;
                }
                else
                {                    
                    details.ModifiedBy = User.Identity.Name;
                    res.ModifiedBy = User.Identity.Name;
                }
                res.UserId = details.UserId;
                res.Domain = details.Domain;
                res.Region = details.Region;
                ctx.SaveChanges();
                UpdateDomainUsage(ctx, details.UserId);
            }
            return RedirectToAction("DomainDetails");
        }

        //    private void UpdateDomainUsage(UMTContext ctx, string userId)
        //    {
        //        string sql = @"
        //    SET SQL_SAFE_UPDATES = 0;

        //    UPDATE mst_tool_usage tu
        //    JOIN mst_domain_details dd
        //        ON dd.UserID = tu.UserID
        //    SET tu.Domain = dd.Domain
        //    WHERE
        //        tu.UserID = @userId
        //        AND (tu.Domain IS NULL OR tu.Domain <> dd.Domain)
        //        AND tu.StartTime >= NOW() - INTERVAL 365 DAY;

        //    SET SQL_SAFE_UPDATES = 1;
        //";

        //        var param = new MySql.Data.MySqlClient.MySqlParameter( "@userId", userId);
        //        ctx.Database.ExecuteSqlCommand(sql, param);
        //    }
        private void UpdateDomainUsage(UMTContext ctx, string userId)
        {
            string sql = @"
        SET SQL_SAFE_UPDATES = 0;

        UPDATE mst_tool_usage tu
        JOIN mst_domain_details dd
            ON dd.UserID = tu.UserID
        SET
            tu.Domain = CASE
                WHEN tu.Domain IS NULL OR tu.Domain <> dd.Domain
                THEN dd.Domain
                ELSE tu.Domain
            END,
            tu.Region = CASE
                WHEN tu.Region IS NULL OR tu.Region <> dd.Region
                THEN dd.Region
                ELSE tu.Region
            END
        WHERE
            tu.UserID = @userId
            AND tu.StartTime >= NOW() - INTERVAL 365 DAY
            AND (
                tu.Domain IS NULL OR tu.Domain <> dd.Domain
                OR
                tu.Region IS NULL OR tu.Region <> dd.Region
            );

        SET SQL_SAFE_UPDATES = 1;
    ";

            var param = new MySql.Data.MySqlClient.MySqlParameter("@userId", userId);
            ctx.Database.ExecuteSqlCommand(sql, param);
        }
        public JsonResult GetDomainsByRegion(string region)
        {
            using (var ctx = new UMTContext())
            {
                var domains = ctx.DomainDetails
                    .Where(x => x.Region == region)
                    .Select(x => x.Domain.ToUpper())
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                var list = domains
                    .Select(d => new SelectListItem { Text = d, Value = d })
                    .ToList();

                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetDomainsByRegions(List<string> regions)
        {
            using (var ctx = new UMTContext())
            {
                var domains = ctx.DomainDetails
                    .Where(x => regions.Contains(x.Region))
                    .Select(x => x.Domain)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                return Json(domains.Select(d => new SelectListItem
                {
                    Text = d.ToUpper(),
                    Value = d
                }).ToList());
            }
        }
        [HttpGet]
        public JsonResult GetUserIdSuggestions(string term)
        {
            using (var ctx = new UMTContext())
            {
                var users = ctx.ToolTracking
                    .Where(x => x.UserID.Contains(term))   // ✅ CONTAINS (not StartsWith)
                    .Select(x => x.UserID)
                    .Distinct()
                    .Take(20)
                    .ToList();

                return Json(users, JsonRequestBehavior.AllowGet);
            }
        }

        private bool IsAdminUser(string userName)
        {
            using (var ctx = new UMTContext())
            {
                return ctx.CooperAdmins
                          .Any(a => a.UserId.Equals(userName,
                               StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}