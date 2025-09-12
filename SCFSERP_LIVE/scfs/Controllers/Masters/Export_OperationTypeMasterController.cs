
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
    public class Export_OperationTypeMasterController : Controller
    {
        // GET: Export_OperationTypeMaster

        SCFSERPContext context = new SCFSERPContext();

        [Authorize(Roles = "ExportOperationTypeMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View();
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Export_Operation_TypeMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.EOPTCODE, d.EOPTDESC, d.DISPSTATUS.ToString(), d.EOPTID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "ExportOperationTypeMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/Export_OperationTypeMaster/Form/" + id);

            //Response.Redirect("/Export_OperationTypeMaster/Form/" + id);
        }

        //----------------Initializing Form-----------------------//
        [Authorize(Roles = "ExportOperationTypeMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            Export_OperationTypeMaster tab = new Export_OperationTypeMaster();
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            tab.EOPTID = 0;
            if (id != 0)
            {
                tab = context.Export_OperationTypeMaster.Find(id);

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

        //public void savedata(Export_OperationTypeMaster export_operationtypemaster)
        //{
        //    using (SCFSERPContext dataContext = new SCFSERPContext())
        //    {
        //        using (var trans = dataContext.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                export_operationtypemaster.CUSRID = Session["CUSRID"].ToString();
        //                export_operationtypemaster.LMUSRID = 1;
        //                export_operationtypemaster.PRCSDATE = DateTime.Now;
        //                if (export_operationtypemaster.EOPTID != 0)
        //                {
        //                    dataContext.Entry(export_operationtypemaster).State = System.Data.Entity.EntityState.Modified;
        //                    dataContext.SaveChanges();
        //                }
        //                else
        //                {
        //                    dataContext.Export_OperationTypeMasters.Add(export_operationtypemaster);
        //                    dataContext.SaveChanges();
        //                }
        //                trans.Commit(); Response.Redirect("Index");
        //            }
        //            catch (Exception ex)
        //            {
        //                trans.Rollback();
        //                Response.Write("Sorry!! An Error Occurred.... ");
        //            }
        //        }
        //    }

        //}

        [HttpPost]
        public JsonResult savedata(Export_OperationTypeMaster export_operationtypemaster)
        {
            using (SCFSERPContext dataContext = new SCFSERPContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
                {
                    string status = "";
                    try
                    {
                        
                        if ((export_operationtypemaster.CUSRID == "" || export_operationtypemaster.CUSRID == null) && ((export_operationtypemaster.EOPTID).ToString() == "0"))
                        {
                            if (Session["CUSRID"] != null)
                            {
                                export_operationtypemaster.CUSRID = Session["CUSRID"].ToString();
                            }
                            else { export_operationtypemaster.CUSRID = "0"; }
                        }
                        if ((export_operationtypemaster.EOPTID).ToString() != "0")
                        {
                            export_operationtypemaster.LMUSRID = Session["CUSRID"].ToString();
                        }
                            
                        export_operationtypemaster.PRCSDATE = DateTime.Now;
                        
                        if ((export_operationtypemaster.EOPTID).ToString() != "0")
                        {

                            context.Entry(export_operationtypemaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            status = "Update";
                            return Json(status, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            var query = context.Export_OperationTypeMaster.SqlQuery("SELECT *FROM EXPORT_OPERATIONTYPEMASTER WHERE EOPTDESC='" + export_operationtypemaster.EOPTDESC + "' AND EOPTCODE='" + export_operationtypemaster.EOPTCODE + "'").ToList<Export_OperationTypeMaster>();

                            if (query.Count != 0)
                            {
                                status = "Existing";
                                return Json(status, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                context.Export_OperationTypeMaster.Add(export_operationtypemaster);
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
        [Authorize(Roles = "ExportOperationTypeMasterDelete")]
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
                            Export_OperationTypeMaster Export_OperationTypeMaster = context.Export_OperationTypeMaster.Find(Convert.ToInt32(id));
                            context.Export_OperationTypeMaster.Remove(Export_OperationTypeMaster);
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