
using scfs.Data;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using scfs_erp;

namespace scfs_erp.Controllers.Export
{
    [SessionExpire]
    public class ExportSlabTypeController : Controller
    {
        // GET: ExportSlabType

        SCFSERPContext context = new SCFSERPContext();

        [Authorize(Roles = "ExportSlabTypeMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(context.exportslabtypemaster.ToList());//Loading Grid
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Export_SlabTypeMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.SLABTCODE, d.SLABTDESC, d.HSNCODE, d.DISPSTATUS, d.SLABTID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "ExportSlabTypeMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/ExportSlabType/Form/" + id);

            //Response.Redirect("/ExportSlabType/Form/" + id);
        }

        //----------------------Initializing Form--------------------------//
        [Authorize(Roles = "ExportSlabTypeMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ExportSlabTypeMaster tab = new ExportSlabTypeMaster();
            tab.SLABTID = 0;

            ViewBag.HSNID = new SelectList(context.HSNCodeMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.HSNCODE), "HSNID", "HSNCODE");

            List<SelectListItem> selectedITEMLT = new List<SelectListItem>();
            SelectListItem selectedItemS = new SelectListItem { Text = "STORAGE", Value = "1", Selected = false };
            selectedITEMLT.Add(selectedItemS);
            selectedItemS = new SelectListItem { Text = "HANDLING", Value = "2", Selected = true };
            selectedITEMLT.Add(selectedItemS);
            selectedItemS = new SelectListItem { Text = "OTHERS", Value = "3", Selected = false };
            selectedITEMLT.Add(selectedItemS);
            ViewBag.SLABSTYPE = selectedITEMLT;

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            if (id != 0)//Edit Mode
            {
                tab = context.exportslabtypemaster.Find(id);

                ViewBag.HSNID = new SelectList(context.HSNCodeMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.HSNCODE), "HSNID", "HSNCODE", tab.HSNID);

                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }

                List<SelectListItem> selectedITEMLT_ = new List<SelectListItem>();
                if (Convert.ToInt32(tab.SLABSTYPE) == 1)
                {
                    SelectListItem selectedItemS_ = new SelectListItem { Text = "STORAGE", Value = "1", Selected = true };
                    selectedITEMLT_.Add(selectedItemS_);
                    selectedItemS_ = new SelectListItem { Text = "HANDLING", Value = "2", Selected = false };
                    selectedITEMLT_.Add(selectedItemS_);
                    selectedItemS_ = new SelectListItem { Text = "OTHERS", Value = "3", Selected = false };
                    selectedITEMLT_.Add(selectedItemS_); ViewBag.SLABSTYPE = selectedITEMLT_;
                }
                else if (Convert.ToInt32(tab.SLABSTYPE) == 2)
                {
                    SelectListItem selectedItemS_ = new SelectListItem { Text = "STORAGE", Value = "1", Selected = false };
                    selectedITEMLT_.Add(selectedItemS_);
                    selectedItemS_ = new SelectListItem { Text = "HANDLING", Value = "2", Selected = true};
                    selectedITEMLT_.Add(selectedItemS_);
                    selectedItemS_ = new SelectListItem { Text = "OTHERS", Value = "3", Selected = false};
                    selectedITEMLT_.Add(selectedItemS_); ViewBag.SLABSTYPE = selectedITEMLT_;
                }
 		else
                {
                    SelectListItem selectedItemS_ = new SelectListItem { Text = "STORAGE", Value = "1", Selected = false };
                    selectedITEMLT_.Add(selectedItemS_);
                    selectedItemS_ = new SelectListItem { Text = "HANDLING", Value = "2", Selected = false };
                    selectedITEMLT_.Add(selectedItemS_);
                    selectedItemS_ = new SelectListItem { Text = "OTHERS", Value = "3", Selected = true };
                    selectedITEMLT_.Add(selectedItemS_); ViewBag.SLABSTYPE = selectedITEMLT_;
                }

            }
            return View(tab);
        }
        //End of Form

        //--------------------------Insert or Modify data------------------------//
        //public void savedata(ExportSlabTypeMaster tab)
        //{
        //    if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
        //    tab.LMUSRID = 1;
        //    tab.PRCSDATE = DateTime.Now;
        //    if ((tab.SLABTID).ToString() != "0")
        //    {
        //        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
        //        context.SaveChanges();
        //    }
        //    else
        //    {
        //        context.exportslabtypemaster.Add(tab);
        //        context.SaveChanges();
        //    }
        //    Response.Redirect("Index");
        //}
        //End of savedata

        [HttpPost]
        public JsonResult savedata(ExportSlabTypeMaster tab)
        {
            using (SCFSERPContext dataContext = new SCFSERPContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
                {
                    string status = "";
                    try
                    {

                        if ((tab.CUSRID == "" || tab.CUSRID == null) && ((tab.SLABTID).ToString() == "0"))
                        {
                            if (Session["CUSRID"] != null)
                            {
                                tab.CUSRID = Session["CUSRID"].ToString();
                            }
                            else { tab.CUSRID = "0"; }
                        }
                        tab.LMUSRID = Session["CUSRID"].ToString();                         
                        tab.PRCSDATE = DateTime.Now;

                        if ((tab.SLABTID).ToString() != "0")
                        {

                            context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            status = "Update";
                            return Json(status, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            var query = context.exportslabtypemaster.SqlQuery("SELECT *FROM EXPORTSLABTYPEMASTER WHERE SLABTDESC='" + tab.SLABTDESC + "' AND SLABTCODE='" + tab.SLABTCODE + "'").ToList<ExportSlabTypeMaster>();

                            if (query.Count != 0)
                            {
                                status = "Existing";
                                return Json(status, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                context.exportslabtypemaster.Add(tab);
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
        [Authorize(Roles = "ExportSlabTypeMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");

            String temp = Delete_fun.delete_check1(fld, id);

            if (temp.Equals("PROCEED"))
            {

                ExportSlabTypeMaster exportslabtypemaster = context.exportslabtypemaster.Find(Convert.ToInt32(id));
                context.exportslabtypemaster.Remove(exportslabtypemaster);
                context.SaveChanges();

                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//End of Delete

    }
}