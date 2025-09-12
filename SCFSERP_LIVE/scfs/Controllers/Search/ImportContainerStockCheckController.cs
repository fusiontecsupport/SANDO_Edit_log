using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using scfs.Data;
using scfs_erp;
using scfs_erp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace scfs.Controllers.Search
{
    [SessionExpire]
    public class ImportContainerStockCheckController : Controller
    {
        // GET: ImportContainerStockCheck
        SCFSERPContext context = new SCFSERPContext();
        //[Authorize(Roles = "ContainerStockCheckView")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            if (string.IsNullOrEmpty(Session["SDATE"] as string))
            {

                Session["SDATE"] = DateTime.Now.ToString("dd-MM-yyyy");
                Session["EDATE"] = DateTime.Now.ToString("dd-MM-yyyy");
            }
            else
            {
                if (Request.Form.Get("from") != null)
                {
                    Session["SDATE"] = Request.Form.Get("from");
                    Session["EDATE"] = DateTime.Now.ToString("dd-MM-yyyy");
                }
            }
            DateTime sd = Convert.ToDateTime(Session["SDATE"]).Date;
            DateTime ed = Convert.ToDateTime(Session["EDATE"]).Date;

            var query = context.Database.SqlQuery<pr_Import_Container_Stock_Assgn_Result>("EXEC pr_Import_Container_Stock_Assgn @PSDATE='" + sd.ToString("MM-dd-yyyy") + "'").ToList();
            return View(query);
        }


    }
}