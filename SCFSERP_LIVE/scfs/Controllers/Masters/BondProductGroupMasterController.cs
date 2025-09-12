
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
    public class BondProductGroupMasterController : Controller
    {
        // GET: BondProductGroupMaster
        BondContext context = new BondContext();
        //[Authorize(Roles = "BondProductMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(context.bondproductgroupmasters.ToList());//Loading Grid
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new BondEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_SearchBondProductGroup(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.PRDTGCODE, d.PRDTGDESC, d.DISPSTATUS.ToString(), d.PRDTGID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //[Authorize(Roles = "BondProductMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/BondProductGroupMaster/Form/" + id);

            //Response.Redirect("/BondProductGroupMaster/Form/" + id);
        }
        //----------------------Initializing Form--------------------------//
        //[Authorize(Roles = "BondProductMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            BondProductGroupMaster tab = new BondProductGroupMaster();
            tab.PRDTGID = 0;
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            if (id != 0)//Edit Mode
            {
                tab = context.bondproductgroupmasters.Find(id);
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
        //--------------------------Insert or Modify data------------------------//

        [HttpPost]
        public JsonResult savedata(BondProductGroupMaster tab)
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
            if ((tab.PRDTGID).ToString() != "0")
            {
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                status = "Update";
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = context.bondproductgroupmasters.SqlQuery("SELECT *FROM BondProductGROUPMASTER WHERE PRDTGDESC='" + tab.PRDTGDESC + "' AND PRDTGCODE='" + tab.PRDTGCODE + "'").ToList<BondProductGroupMaster>();

                if (query.Count != 0)
                {
                    status = "Existing";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    context.bondproductgroupmasters.Add(tab);
                    context.SaveChanges();

                    status = "Success";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
            }
        }

        //public void savedata(BondProductGroupMaster tab)
        //{
        //    if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
        //    tab.LMUSRID = 1;
        //    tab.PRCSDATE = DateTime.Now;
        //    if ((tab.PRDTGID).ToString() != "0")
        //    {
        //        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
        //        context.SaveChanges();
        //    }
        //    else
        //    {
        //        context.bondproductgroupmasters.Add(tab);
        //        context.SaveChanges();
        //    }
        //    Response.Redirect("Index");
        //}
        //End of savedata
        //------------------------Delete Record----------//
        [Authorize(Roles = "BondProductMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                BondProductGroupMaster bondproductgroupmasters = context.bondproductgroupmasters.Find(Convert.ToInt32(id));
                context.bondproductgroupmasters.Remove(bondproductgroupmasters);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//End of Delete
    }
}
