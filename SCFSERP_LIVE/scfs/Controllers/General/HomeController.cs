using scfs.Data;
using scfs_erp.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace scfs_erp.Controllers
{
    public class HomeController : Controller
    {
        SCFSERPContext context = new SCFSERPContext();

        [Authorize]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            
                if (Request.Form.Get("txtfromdate") != null)
                {
                    Session["SDATE"] = Request.Form.Get("txtfromdate");
                    Session["EDATE"] = Request.Form.Get("txttodate");
                }
                else
                {
                    Session["SDATE"] = DateTime.Now.ToString("dd-MM-yyyy");
                    Session["EDATE"] = DateTime.Now.ToString("dd-MM-yyyy");
                }
            
            DateTime fromdate = Convert.ToDateTime(Session["SDATE"]).Date;
            DateTime todate = Convert.ToDateTime(Session["EDATE"]).Date;

            
                       
            TotalContainerDetails(fromdate, todate);
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult TotalContainerDetails(DateTime fromdate, DateTime todate)
        {
            string fdate = ""; string tdate = ""; int sdptid = 1;
            if (fromdate == null)
            {
                fromdate = DateTime.Now.Date;
                fdate = Convert.ToString(fromdate);
            }
            else
            {
                string infdate = Convert.ToString(fromdate);               
                var in_date = infdate.Split(' ');
                var in_date1 = in_date[0].Split('/');
                fdate = Convert.ToString(in_date1[2] + "-" + in_date1[1] + "-" + in_date1[0]);
            }
            if (todate == null)
            {
                todate = DateTime.Now.Date;
                tdate = Convert.ToString(todate);
            }
            else
            {
                string intdate = Convert.ToString(todate);
                
                var in_date1 = intdate.Split(' ');
                var in_date2 = in_date1[0].Split('/');
                tdate = Convert.ToString(in_date2[2] + "-" + in_date2[1] + "-" + in_date2[0]);

            }

            Session["SDATE"] = Convert.ToDateTime(fromdate).Date;
            Session["EDATE"] = Convert.ToDateTime(todate).Date;
            context.Database.CommandTimeout = 0;
            var result = context.Database.SqlQuery<PR_IMPORT_DASHBOARD_DETAILS_Result>("EXEC PR_IMPORT_DASHBOARD_DETAILS @PFDT='" + fdate + "',@PTDT='" + tdate + "',@PSDPTID=" + 1).ToList();

            foreach (var rslt in result)
            {
                if ((rslt.Sno == 1) && (rslt.Descriptn == "IMPORT - GATEIN"))
                {
                    @ViewBag.Total20 = rslt.c_20;
                    @ViewBag.Total40 = rslt.c_40;
                    @ViewBag.Total45 = rslt.c_45;                    
                    @ViewBag.TotalTues = rslt.c_tues;

                    Session["GI20"]= rslt.c_20;
                    Session["GI40"] = rslt.c_40;
                    Session["GI45"] = rslt.c_45;
                    Session["GITU"] = rslt.c_tues;
                }

                if((rslt.Sno == 2) && (rslt.Descriptn == "IMPORT - GATEOUT"))
                {
                    @ViewBag.GOTotal20 = rslt.c_20;
                    @ViewBag.GOTotal40 = rslt.c_40;
                    @ViewBag.GOTotal45 = rslt.c_45;
                    @ViewBag.GOTotalTues = rslt.c_tues;


                    Session["GO20"] = rslt.c_20;
                    Session["GO40"] = rslt.c_40;
                    Session["GO45"] = rslt.c_45;
                    Session["GOTU"] = rslt.c_tues;
                }

                              
            }
            sdptid = 2;
            var result1 = context.Database.SqlQuery<PR_EXPORT_DASHBOARD_DETAILS_Result>("EXEC PR_EXPORT_DASHBOARD_DETAILS @PFDT='" + fdate + "',@PTDT='" + tdate + "',@PSDPTID=" + 2).ToList();

            foreach (var rslt in result1)
            {
                if ((rslt.Sno == 3) && (rslt.Descriptn == "EXPORT - GATEIN"))
                {
                    @ViewBag.ETotal20 = rslt.c_20;
                    @ViewBag.ETotal40 = rslt.c_40;
                    @ViewBag.ETotal45 = rslt.c_45;
                    @ViewBag.ETotalTues = rslt.c_tues;

                    Session["EGI20"] = rslt.c_20;
                    Session["EGI40"] = rslt.c_40;
                    Session["EGI45"] = rslt.c_45;
                    Session["EGITU"] = rslt.c_tues;
                }

                if ((rslt.Sno == 4) && (rslt.Descriptn == "EXPORT - GATEOUT"))
                {
                    @ViewBag.EGOTotal20 = rslt.c_20;
                    @ViewBag.EGOTotal40 = rslt.c_40;
                    @ViewBag.EGOTotal45 = rslt.c_45;
                    @ViewBag.EGOTotalTues = rslt.c_tues;


                    Session["EGO20"] = rslt.c_20;
                    Session["EGO40"] = rslt.c_40;
                    Session["EGO45"] = rslt.c_45;
                    Session["EGOTU"] = rslt.c_tues;
                }
                

            }
           
            ViewBag.CDate = Convert.ToDateTime(Session["EDATE"]).Date.ToString("dd-MMM-yyyy");// DateTime.Now.ToString("dd-MMM-yyyy");
            return Json(result, JsonRequestBehavior.AllowGet);
           
        }

        public ActionResult Sess_Exp()
        {
            ViewData["SessionTimeout"] = Session.Timeout;
            ViewData["SessionWillExpireOn"] = DateTime.Now.AddMinutes(Session.Timeout);
            return View();
        }
    }
}