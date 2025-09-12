
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
    public class ExportTariffGroupMasterController : Controller
    {
        // GET: ExportTariffGroupMaster

        SCFSERPContext context = new SCFSERPContext();

        [Authorize(Roles = "ExportTariffGroupMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(context.ExportTariffGroupMasters.ToList());//---Loading Grid
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Export_TariffGroupMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.TGCODE, d.TGDESC, d.DISPSTATUS.ToString(), d.TGID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "ExportTariffGroupMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/ExportTariffGroupMaster/Form/" + id);

            //Response.Redirect("/ExportTariffGroupMaster/Form/" + id);
        }


        //----------------Initializing Form-----------------------//
        [Authorize(Roles = "ExportTariffGroupMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ExportTariffGroupMaster tab = new ExportTariffGroupMaster();

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            tab.TGID = 0;
            if (id != 0)
            {
                tab = context.ExportTariffGroupMasters.Find(id);

                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem3 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem3);
                    selectedItem3 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem3);

                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }

            }
            return View(tab);
        }//End of Form

        //-------------------Insert or Modify data-------------//
        //public void savedata(ExportTariffGroupMaster tab)
        //{
        //    if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
        //    tab.LMUSRID = 1;
        //    tab.PRCSDATE = DateTime.Now;


        //    if ((tab.TGID).ToString() != "0")
        //    {
        //        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
        //        context.SaveChanges();
        //    }
        //    else
        //    {
        //        context.ExportTariffGroupMasters.Add(tab);
        //        context.SaveChanges();

        //    }
        //    Response.Redirect("Index");
        //}
        
        //--End of Savedata

        [HttpPost]
        public JsonResult savedata(ExportTariffGroupMaster tab)
        {
            using (SCFSERPContext dataContext = new SCFSERPContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
                {
                    string status = "";
                    try
                    {
                        if ((tab.CUSRID == "" || tab.CUSRID == null) && ((tab.TGID).ToString() == "0"))
                        {
                            if (Session["CUSRID"] != null)
                            {
                                tab.CUSRID = Session["CUSRID"].ToString();
                            }
                            else { tab.CUSRID = "0"; }
                        }
                        tab.LMUSRID = Session["CUSRID"].ToString();
                        tab.PRCSDATE = DateTime.Now;

                        if ((tab.TGID).ToString() != "0")
                        {

                            context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            status = "Update";
                            return Json(status, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            var query = context.ExportTariffGroupMasters.SqlQuery("SELECT *FROM EXPORTTARIFFGROUPMASTER WHERE TGDESC='" + tab.TGDESC + "' AND TGCODE='" + tab.TGCODE + "'").ToList<ExportTariffGroupMaster>();

                            if (query.Count != 0)
                            {
                                status = "Existing";
                                return Json(status, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                context.ExportTariffGroupMasters.Add(tab);
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


        //---------------Delete Record-------------//
        [Authorize(Roles = "ExportTariffGroupMasterDelete")]
        public void Del()
        {
            using (SCFSERPContext dataContext = new SCFSERPContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        String id = Request.Form.Get("id");
                        String fld = Request.Form.Get("fld");
                        String temp = Delete_fun.delete_check1(fld, id);
                        if (temp.Equals("PROCEED"))
                        {
                            ExportTariffGroupMaster ExportTariffGroupMasters = context.ExportTariffGroupMasters.Find(Convert.ToInt32(id));
                            context.ExportTariffGroupMasters.Remove(ExportTariffGroupMasters);
                            context.SaveChanges();
                            Response.Write("Deleted Successfully ...");

                        }
                        else
                            Response.Write(temp); trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                        trans.Rollback();
                        Response.Write("Sorry!! An Error Occurred.... ");
                    }
                }
            }
        }//--End of Delete

    }
}