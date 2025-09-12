
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
    public class VesselMasterController : Controller
    {
        // GET: VesselMaster

        SCFSERPContext context = new SCFSERPContext();

        [Authorize(Roles = "VesselMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(context.vesselmasters.ToList());//Loading Grid
        }

        [Authorize(Roles = "VesselMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/VesselMaster/Form/" + id);

            //Response.Redirect("/VesselMaster/Form/" + id);
        }
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Export_Vessel(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.VSLCODE, d.VSLDESC, d.DISPSTATUS.ToString(), d.VSLID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //----------------------Initializing Form--------------------------//
        [Authorize(Roles = "VesselMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            VesselMaster tab = new VesselMaster();
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            tab.VSLID = 0;
            if (id != 0)//Edit Mode
            {
                tab = context.vesselmasters.Find(id);
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

        //public void savedata(VesselMaster tab)
        //{
        //    if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
        //    tab.LMUSRID = 1;
        //    tab.PRCSDATE = DateTime.Now;
        //    if ((tab.VSLID).ToString() != "0")
        //    {
        //        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
        //        context.SaveChanges();
        //    }
        //    else
        //    {
        //        context.vesselmasters.Add(tab);
        //        context.SaveChanges();
        //    }
        //    Response.Redirect("Index");
        //}
        
        //End of savedata


        [HttpPost]
        public JsonResult savedata(VesselMaster tab)
        {
            using (SCFSERPContext dataContext = new SCFSERPContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
                {
                    string status = "";
                    try
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

                        if ((tab.VSLID).ToString() != "0" || tab.VSLID != 0)
                        {

                            context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            status = "Update";
                            return Json(status, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            var query = context.vesselmasters.SqlQuery("SELECT *FROM VESSELMASTER WHERE VSLCODE='" + tab.VSLCODE + "' AND VSLDESC='" + tab.VSLDESC + "'").ToList<VesselMaster>();

                            if (query.Count != 0)
                            {
                                status = "Existing";
                                return Json(status, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                context.vesselmasters.Add(tab);
                                context.SaveChanges();

                                status = "Success";
                                return Json(status, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        status = "Error";
                        string Message = ex.Message.ToString();
                        return Json(status, Message, JsonRequestBehavior.AllowGet);
                        //Response.Write("Sorry!! An Error Occurred.... ");
                    }
                }
            }
        }

        //------------------------Delete Record----------//
        [Authorize(Roles = "VesselMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                VesselMaster vesselmaster = context.vesselmasters.Find(Convert.ToInt32(id));
                context.vesselmasters.Remove(vesselmaster);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//End of Delete
    }
}