
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

namespace scfs_erp.Controllers.Masters
{
    public class EndorsementChargeTypeController : Controller
    {
        // GET: EndorsementChargeType

        SCFSERPContext context = new SCFSERPContext();

        //[Authorize(Roles = "EndorsementChargeTypeIndex")]
        public ActionResult Index()
        {
            //if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View();
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Export_Endorsement_ChargeType_Master(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.ECTCODE, d.ECTDESC, d.DISPSTATUS.ToString(), d.ECTID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }


        //[Authorize(Roles = "EndorsementChargeTypeEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/EndorsementChargeType/Form/" + id);

            //Response.Redirect("/EndorsementChargeType/Form/" + id);
        }

        //----------------Initializing Form-----------------------//
        //[Authorize(Roles = "EndorsementChargeTypeCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            EndorsementChargeTypeMaster tab = new EndorsementChargeTypeMaster();
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            tab.ECTID = 0;
            if (id != 0)
            {
                tab = context.EndorsementChargeTypeMasters.Find(id);

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

        //public void savedata(EndorsementChargeTypeMaster EndorsementChargeTypeMaster)
        //{
        //    using (SCFSERPContext dataContext = new SCFSERPContext())
        //    {
        //        using (var trans = dataContext.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                EndorsementChargeTypeMaster.CUSRID = Session["CUSRID"].ToString();
        //                EndorsementChargeTypeMaster.LMUSRID = 1;
        //                EndorsementChargeTypeMaster.PRCSDATE = DateTime.Now;
        //                EndorsementChargeTypeMaster.DISPORDER = 0;
        //                if (EndorsementChargeTypeMaster.ECTID != 0)
        //                {
        //                    dataContext.Entry(EndorsementChargeTypeMaster).State = System.Data.Entity.EntityState.Modified;
        //                    dataContext.SaveChanges();
        //                }
        //                else
        //                {
        //                    dataContext.EndorsementChargeTypeMasters.Add(EndorsementChargeTypeMaster);
        //                    dataContext.SaveChanges();
        //                }
        //                //dataContext.ExportEndorsementChargeTypeMaster.Add(ExportEndorsementChargeTypeMaster);
        //                //dataContext.SaveChanges();
        //                trans.Commit(); Response.Redirect("Index");
        //            }
        //            catch (Exception ex)
        //            {
        //                trans.Rollback();
        //                Response.Write(ex + "Sorry!! An Error Occurred.... ");
        //            }
        //        }
        //    }
        //}

        [HttpPost]
        public JsonResult savedata(EndorsementChargeTypeMaster tab)
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

                        if ((tab.ECTID).ToString() != "0" || tab.ECTID != 0)
                        {

                            context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            status = "Update";
                            return Json(status, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            var query = context.EndorsementChargeTypeMasters.SqlQuery("SELECT *FROM ENDORSEMENT_CHARGE_TYPE_MASTER WHERE ECTCODE='" + tab.ECTCODE + "' AND ECTDESC='" + tab.ECTDESC + "'").ToList<EndorsementChargeTypeMaster>();

                            if (query.Count != 0)
                            {
                                status = "Existing";
                                return Json(status, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                context.EndorsementChargeTypeMasters.Add(tab);
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
        //[Authorize(Roles = "EndorsementChargeTypeDelete")]
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
                            EndorsementChargeTypeMaster EndorsementChargeTypeMaster = context.EndorsementChargeTypeMasters.Find(Convert.ToInt32(id));
                            context.EndorsementChargeTypeMasters.Remove(EndorsementChargeTypeMaster);
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
        }
        //--End of Delete
    }
}