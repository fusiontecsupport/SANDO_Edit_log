
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
    public class TariffMasterController : Controller
    {
        // GET: TariffMaster
        SCFSERPContext context = new SCFSERPContext();

        [Authorize(Roles = "TariffMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(context.tariffmasters.ToList());//Loading Grid
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                var data = e.pr_Search_Import_TariffMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                //var aaData = data.Select(d => new string[] { d.TARIFFMCODE, d.TARIFFMDESC, d.DISPSTATUS.ToString(), d.TARIFFMID.ToString() }).ToArray();
                var aaData = data.Select(d => new string[] { d.TARIFFMCODE, d.TariffType, d.TARIFFMDESC, d.DISPSTATUS.ToString(), d.TARIFFMID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "TariffMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/TariffMaster/Form/" + id);

            //Response.Redirect("/TariffMaster/Form/" + id);
        }

        [Authorize(Roles = "TariffMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            TariffMaster tab = new TariffMaster();
            tab.TARIFFMID = 0;
            ViewBag.SDPTID = new SelectList(context.softdepartmentmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SDPTNAME), "SDPTID", "SDPTNAME");
            ViewBag.TGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            if (id != 0)//Edit Mode
            {
                tab = context.tariffmasters.Find(id);
                ViewBag.TGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC", tab.TGID);
                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
                ViewBag.SDPTID = new SelectList(context.softdepartmentmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SDPTNAME), "SDPTID", "SDPTNAME", tab.SDPTID);
            }
            return View(tab);
        }//End of Form
        //--------------------------Insert or Modify data------------------------//
        //public void savedata(TariffMaster tab)
        //{
        //    if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
        //    tab.LMUSRID = 1;
        //    tab.PRCSDATE = DateTime.Now;
        //    if ((tab.TARIFFMID).ToString() != "0")
        //    {
        //        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
        //        context.SaveChanges();
        //    }
        //    else
        //    {
        //        context.tariffmasters.Add(tab);
        //        context.SaveChanges();
        //    }
        //    Response.Redirect("Index");
        //}

        [HttpPost]
        public JsonResult savedata(TariffMaster tab)
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
            if ((tab.TARIFFMID).ToString() != "0")
            {

                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                status = "Update";
                return Json(status, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var query = context.tariffmasters.SqlQuery("SELECT *FROM TARIFFMASTER WHERE TARIFFMDESC='" + tab.TARIFFMDESC + "' AND TARIFFMCODE='" + tab.TARIFFMCODE + "'").ToList<TariffMaster>();


                if (query.Count != 0)
                {
                    status = "Existing";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    context.tariffmasters.Add(tab);
                    context.SaveChanges();

                    status = "Success";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
            }
        }




        //End of savedata
        //------------------------Delete Record----------//
        [Authorize(Roles = "TariffMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                TariffMaster tariffmasters = context.tariffmasters.Find(Convert.ToInt32(id));
                context.tariffmasters.Remove(tariffmasters);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//End of Delete


    }
}