
using scfs.Data;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System;
using System.Configuration;

namespace scfs_erp.Controllers.Masters
{
    public class GoDownMasterController : Controller
    {
        // GET: GoDownMaster

        SCFSERPContext context = new SCFSERPContext();
        [Authorize(Roles = "GoDownMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(context.godownmasters.ToList());//Loading Grid
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_GoDownMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.GDWNCODE, d.GDWNDESC, d.DISPSTATUS.ToString(), d.GDWNID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "GoDownMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/GoDownMaster/Form/" + id);

            //Response.Redirect("/GoDownMaster/Form/" + id);
        }

        //----------------------Initializing Form--------------------------//
        [Authorize(Roles = "GoDownMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GodownMaster tab = new GodownMaster();
            tab.GDWNID = 0;
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            if (id != 0)//Edit Mode
            {
                tab = context.godownmasters.Find(id);
                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
            }
            return View(tab);
        }//End of form
        //--------------------------Insert or Modify data------------------------//
        //public void savedata(GodownMaster tab)
        //{
        //    if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
        //    tab.LMUSRID = 1;
        //    tab.PRCSDATE = DateTime.Now;
        //    if ((tab.GDWNID).ToString() != "0")
        //    {
        //        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
        //        context.SaveChanges();
        //    }
        //    else
        //    {
        //        context.godownmasters.Add(tab);
        //        context.SaveChanges();
        //    }
        //    Response.Redirect("Index");
        //}

        [HttpPost]
        public JsonResult savedata(GodownMaster tab)
        {
            if (tab.CUSRID == "" || tab.CUSRID == null)
            {
                if (Session["CUSRID"] != null)
                {
                    tab.CUSRID = Session["CUSRID"].ToString();
                }
                else { tab.CUSRID = "0"; }
            }
            tab.LMUSRID = 1;
            tab.PRCSDATE = DateTime.Now;

            string status = "";

            if ((tab.GDWNID).ToString() != "0")
            {

                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                status = "Update";
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = context.godownmasters.SqlQuery("SELECT *FROM GODOWNMASTER WHERE GDWNDESC='" + tab.GDWNDESC + "' AND GDWNCODE='" + tab.GDWNCODE + "'").ToList<GodownMaster>();

                if (query.Count != 0)
                {
                    status = "Existing";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    context.godownmasters.Add(tab);
                    context.SaveChanges();

                    status = "Success";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
            }
            //Response.Redirect("Index");
        }

        //End of savedata



        //------------------------Delete Record----------//
        [Authorize(Roles = "GoDownMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                GodownMaster godownmasters = context.godownmasters.Find(Convert.ToInt32(id));
                context.godownmasters.Remove(godownmasters);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//End of Delete

    }
}