using scfs_erp.Context;
using scfs_erp.Models;
using scfs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers.Cargo
{
    public class ImportCargoInController : Controller
    {
        // GET: ImportCargoIn

        #region Context Declaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region Index Form
        [Authorize(Roles = "ImportCargoInIndex")]
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
                    Session["EDATE"] = Request.Form.Get("to");
                }

            }

            DateTime sd = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;

            DateTime ed = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;

            return View();
        }
        #endregion

        #region Get Ajax Data
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {

            using (var e = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Import_CargoIn(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(Session["compyid"]));

                var aaData = data.Select(d => new string[] { d.GIDATE.Value.ToString("dd/MM/yyyy"), d.GIDNO, d.CONTNRNO, d.CONTNRSID, d.STMRNAME, d.VHLNO, d.DISPSTATUS, d.GIDID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

    }
}