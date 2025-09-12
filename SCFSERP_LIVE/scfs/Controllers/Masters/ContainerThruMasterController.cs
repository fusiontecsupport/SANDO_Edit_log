
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
    public class ContainerThruMasterController : Controller
    {
        SCFSERPContext context = new SCFSERPContext();

        // GET: ContainerThruMaster
        [Authorize(Roles = "ContainerThruMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(context.containerthrumasters.ToList());//Loading Grid
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_ContainerThruMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.CONTNRFCODE, d.CONTNRFDESC, d.DISPSTATUS, d.CONTNRFID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "ContainerThruMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/ContainerThruMaster/Form/" + id);

            //Response.Redirect("/ContainerThruMaster/Form/" + id);
        }

        //-----------------------Initializing Form------------------------//
        [Authorize(Roles = "ContainerThruMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ContainerThruMaster tab = new ContainerThruMaster();
            tab.CONTNRFID = 0;

            List<SelectListItem> selectedCONTNRFTYPE = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "IN", Value = "0", Selected = true };
            selectedCONTNRFTYPE.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "OUT", Value = "1", Selected = false };
            selectedCONTNRFTYPE.Add(selectedItem);
            ViewBag.CONTNRFTYPE = selectedCONTNRFTYPE;

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem1);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            if (id != 0)//Edit Mode
            {
                tab = context.containerthrumasters.Find(id);
                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem3 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem3);
                    selectedItem3 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem3);

                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }

                List<SelectListItem> selectedCONTNRFTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.CONTNRFTYPE) == 1)
                {
                    SelectListItem selectedItem3 = new SelectListItem { Text = "IN", Value = "0", Selected = false };
                    selectedCONTNRFTYPE1.Add(selectedItem3);
                    selectedItem3 = new SelectListItem { Text = "OUT", Value = "1", Selected = true };
                    selectedCONTNRFTYPE1.Add(selectedItem3);

                    ViewBag.CONTNRFTYPE = selectedCONTNRFTYPE1;
                }
            }
            return View(tab);
        }//End of Form


        //---------------------Insert or Modify data------------------//
        //public void savedata(ContainerThruMaster tab)
        //{
        //    if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
        //    tab.LMUSRID = 1;
        //    tab.PRCSDATE = DateTime.Now;
        //    if ((tab.CONTNRFID).ToString() != "0")
        //    {
        //        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
        //        context.SaveChanges();
        //    }
        //    else
        //    {
        //        context.containerthrumasters.Add(tab);
        //        context.SaveChanges();
        //    }
        //    Response.Redirect("Index");
        //}

        #region Insert or Modify data       
        [HttpPost]
        public JsonResult savedata(ContainerThruMaster tab)
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
            if ((tab.CONTNRFID).ToString() != "0")
            {

                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                status = "Update";
                return Json(status, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var query = context.containerthrumasters.SqlQuery("SELECT *FROM CONTAINERTHRUMASTER  WHERE CONTNRFDESC='" + tab.CONTNRFDESC + "' AND CONTNRFCODE='" + tab.CONTNRFCODE + "'").ToList<ContainerThruMaster>();

                if (query.Count != 0)
                {
                    status = "Existing";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    context.containerthrumasters.Add(tab);
                    context.SaveChanges();

                    status = "Success";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion


        //End of savedata


        //------------------------Delete Record----------//
        [Authorize(Roles = "ContainerThruMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                ContainerThruMaster containerthrumasters = context.containerthrumasters.Find(Convert.ToInt32(id));
                context.containerthrumasters.Remove(containerthrumasters);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        //End of Delete

    }
}