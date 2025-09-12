using scfs_erp.Context;
using scfs_erp.Helper;
using scfs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using scfs_erp.Models;
using System.Configuration;

namespace scfs_erp.Controllers.Masters
{
    public class DesignationTypeMasterController : Controller
    {
        // GET: DesignationTypeMaster
        SCFSERPContext context = new SCFSERPContext();
        SCFSERPEntities db = new SCFSERPEntities();

        // GET: /DesignationTypeMaster/
        [Authorize(Roles = "DesignationTypeMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(context.designationtypemasters.ToList());//Loading Grid
        }
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_SearchDesignationTypeMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.DSGNTCODE, d.DSGNTDESC, d.DISPSTATUS, d.DSGNTID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "DesignationTypeMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/DesignationTypeMaster/Form/" + id);

            //Response.Redirect("/DesignationTypeMaster/Form/" + id);
        }
        //-------------Initializing Form-------------//
        [Authorize(Roles = "DesignationTypeMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            DesignationTypeMaster tab = new DesignationTypeMaster();
            tab.DSGNTID = 0;
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            if (id != 0)//Edit Form
            {
                tab = context.designationtypemasters.Find(id);
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
        public void savedata(DesignationTypeMaster tab)
        {
           
            if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
            tab.LMUSRID = 1;
            tab.PRCSDATE = DateTime.Now;
            if (ModelState.IsValid)
            {
                if ((tab.DSGNTID).ToString() != "0")
                {
                    context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                else
                {
                    context.designationtypemasters.Add(tab);
                    context.SaveChanges();
                }
                Response.Redirect("Index");
            }
        }//End of savedata
        //------------------------Delete Record----------//
        [Authorize(Roles = "DesignationTypeMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                DesignationTypeMaster designationtypemasters = context.designationtypemasters.Find(Convert.ToInt32(id));
                context.designationtypemasters.Remove(designationtypemasters);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//End of Delete

    }
}
