
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
    public class EmptyDamageCodeMasterController : Controller
    {
        // GET: EmptyDamageCodeMaster

        SCFSERPContext context = new SCFSERPContext();
        [Authorize(Roles = "EmptyDamageCodeMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(context.emptydamagecodemasters.ToList());//Loading Grid
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Empty_DamageCodeMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.DMGCODE, d.DMGDESC, d.DISPSTATUS, d.DMGID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "EmptyDamageCodeMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/EmptyDamageCodeMaster/Form/" + id);

            //Response.Redirect("/EmptyDamageCodeMaster/Form/" + id);
        }


        [Authorize(Roles = "EmptyDamageCodeMasterCreate")]
        //-------------Initializing Form-------------//
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            EmptyDamageCodeMaster tab = new EmptyDamageCodeMaster();
            tab.DMGID = 0;
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            if (id != 0)//Edit Mode
            {
                tab = context.emptydamagecodemasters.Find(id);

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
        }//End of Form
        //---------------------Insert or Modify data------------------//
        //public void savedata(EmptyDamageCodeMaster tab)
        //{
        //    if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
        //    tab.LMUSRID = 1;
        //    tab.PRCSDATE = DateTime.Now;
        //    if ((tab.DMGID).ToString() != "0")
        //    {
        //        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
        //        context.SaveChanges();
        //    }
        //    else
        //    {
        //        context.emptydamagecodemasters.Add(tab);
        //        context.SaveChanges();
        //    }
        //    Response.Redirect("Index");
        //}
        //End of savedata

        [HttpPost]
        public JsonResult savedata(EmptyDamageCodeMaster tab)
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
            if ((tab.DMGID).ToString() != "0")
            {

                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                status = "Update";
                return Json(status, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var query = context.emptydamagecodemasters.SqlQuery("SELECT *FROM EMPTY_DAMAGECODEMASTER WHERE DMGDESC='" + tab.DMGDESC + "' AND DMGCODE='" + tab.DMGCODE + "'").ToList<EmptyDamageCodeMaster>();

                if (query.Count != 0)
                {
                    status = "Existing";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    context.emptydamagecodemasters.Add(tab);
                    context.SaveChanges();

                    status = "Success";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
            }
        }

        //------------------------Delete Record----------//
        [Authorize(Roles = "EmptyDamageCodeMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                EmptyDamageCodeMaster emptydamagecodemasters = context.emptydamagecodemasters.Find(Convert.ToInt32(id));
                context.emptydamagecodemasters.Remove(emptydamagecodemasters);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//End of Delete

    }
}