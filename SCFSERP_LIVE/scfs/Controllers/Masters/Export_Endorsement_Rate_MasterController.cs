
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
    public class Export_Endorsement_Rate_MasterController : Controller
    {
        // GET: Export_Endorsement_Rate_Master

        SCFSERPContext context = new SCFSERPContext();

        [Authorize(Roles = "ExportEndorsementRateMasterIndex")]

        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            //if (Request.Form.Get("ECMID") != null)
            //{
            //    Session["ECMID"] = Request.Form.Get("ECMID");
            //    Session["ECTID"] = Request.Form.Get("ECTID");               
            //}
            //else
            //{
            //    Session["ECMID"] = 1;
            //    Session["ECTID"] = 2;                
            //}

            //ViewBag.ECTID = new SelectList(context.EndorsementChargeTypeMasters.OrderBy(x => x.ECTDESC), "ECTID", "ECTDESC", Session["ECTID"]);
            //ViewBag.ECMID = new SelectList(context.EndorsementChargeMasters.Where(X => X.ECMID == 2).OrderBy(X => X.ECMDESC), "ECMID", "ECMDESC", Session["ECMID"]);
            
            return View();
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Export_Endorsement_Rate_Master(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.ECTDESC, d.ECMDESC, d.ECRDATE.Value.ToString("dd/MM/yyyy"), d.CONTNRSCODE, d.CHRGDON, d.ECRMIN.ToString(), d.ECRAMT.ToString(), d.ECRID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "ExportEndorsementRateMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/Export_Endorsement_Rate_Master/Form/" + id);

            //Response.Redirect("/Export_Endorsement_Rate_Master/Form/" + id);
        }

        //----------------------Initializing Form--------------------------//
        [Authorize(Roles = "ExportEndorsementRateMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            Export_Endorsement_Rate_Master tab = new Export_Endorsement_Rate_Master();
            tab.ECRID = 0;
            tab.ECRDATE = DateTime.Now;
            //-------------------------------------Dropdown List---------------------------------------//
            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1), "CONTNRSID", "CONTNRSDESC");
            ViewBag.ECTID = new SelectList(context.EndorsementChargeTypeMasters.OrderBy(x => x.ECTDESC), "ECTID", "ECTDESC");
            ViewBag.ECMID = new SelectList(context.EndorsementChargeMasters.Where(X => X.ECMID == 2).OrderBy(X => X.ECMDESC), "ECMID", "ECMDESC");
            // ViewBag.CHRGDON = new SelectList(context.ExportEndorsementChargeMaster, "CHRGDON", "CONTAINER");

            //--------------------------CHRGETYPE--------
            List<SelectListItem> selectedCHRGETYPE = new List<SelectListItem>();
            SelectListItem selectedItem_C = new SelectListItem { Text = "STUFF", Value = "1", Selected = true };
            selectedCHRGETYPE.Add(selectedItem_C);
            selectedItem_C = new SelectListItem { Text = "SEAL", Value = "2", Selected = false };
            selectedCHRGETYPE.Add(selectedItem_C);
            ViewBag.CHRGETYPE = selectedCHRGETYPE;
            //-----------------CHRGDON----
            List<SelectListItem> selectedCHRGDON = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "CONTAINER", Value = "0", Selected = true };
            selectedCHRGDON.Add(selectedItem);
            ViewBag.CHRGDON = selectedCHRGDON;

            if (id != 0)//Edit Mode
            {
                tab = context.Export_Endorsement_Rate_Master.Find(id);
                EndorsementChargeMaster ectid = context.EndorsementChargeMasters.Find(tab.ECMID);
                //--------------------------------Selected values in Dropdown List-----------------------------//
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.ECTID = new SelectList(context.EndorsementChargeTypeMasters.OrderBy(x => x.ECTDESC), "ECTID", "ECTDESC", ectid.ECTID);
                ViewBag.ECMID = new SelectList(context.EndorsementChargeMasters.OrderBy(X => X.ECMDESC), "ECMID", "ECMDESC", tab.ECMID);
                //    ViewBag.CHRGDON = new SelectList(context.ExportEndorsementChargeMaster, "CHRGDON", "CONTAINER");


                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 0)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "0", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "1", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
                //---------------------SDTYPE-------------------------------------//
                List<SelectListItem> selectedCHRGETYPE_ = new List<SelectListItem>();
                if (Convert.ToInt32(tab.CHRGETYPE) == 2)
                {
                    SelectListItem selectedItemC = new SelectListItem { Text = "STUFF", Value = "1", Selected = false };
                    selectedCHRGETYPE_.Add(selectedItemC);
                    selectedItemC = new SelectListItem { Text = "SEAL", Value = "2", Selected = true };
                    selectedCHRGETYPE_.Add(selectedItemC);
                    ViewBag.CHRGETYPE = selectedCHRGETYPE_;
                }
            }
            return View(tab);
        }
        //End of Form

        //public void savedata(Export_Endorsement_Rate_Master tab)
        //{
        //    using (SCFSERPContext dataContext = new SCFSERPContext())
        //    {
        //        using (var trans = dataContext.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                tab.COMPYID = Convert.ToInt32(Session["compyid"]);
        //                tab.CUSRID = Session["cusrid"].ToString();
        //                tab.LMUSRID = 1;
        //                tab.DISPSTATUS = 0;
        //                tab.PRCSDATE = DateTime.Now;
        //                tab.ECRMAX = 0;
        //                if (tab.ECRID.ToString() == "0")
        //                {
        //                    context.Export_Endorsement_Rate_Master.Add(tab);
        //                    context.SaveChanges();
        //                }
        //                else
        //                {
        //                    context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
        //                    context.SaveChanges();
        //                }
        //                trans.Commit();
        //                Response.Redirect("Index");
        //            }
        //            catch
        //            {
        //                trans.Rollback();
        //                Response.Write("Sorry!! An Error Occurred.... ");
        //            }
        //        }
        //    }
        //}

        [HttpPost]
        public JsonResult savedata(Export_Endorsement_Rate_Master tab)
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

                        if (tab.COMPYID == 0|| (tab.COMPYID).ToString() == "0")
                        {
                            if (Session["CUSRID"] != null)
                            {
                                tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                            }
                            else { tab.COMPYID = Convert.ToInt32(Session["compyid"]); }
                        }

                        tab.LMUSRID = 1;
                        tab.PRCSDATE = DateTime.Now;

                        if ((tab.ECRID).ToString() != "0" || tab.ECRID != 0)
                        {

                            context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            status = "Update";
                            return Json(status, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            var query = context.Export_Endorsement_Rate_Master.SqlQuery("SELECT *FROM EXPORT_ENDORSEMENT_RATE_MASTER WHERE CONTNRSID=" + tab.CONTNRSID + " AND ECRMIN=" + tab.ECRMIN + " AND ECRAMT=" + tab.ECRAMT + " ").ToList<Export_Endorsement_Rate_Master>();

                            if (query.Count != 0)
                            {
                                status = "Existing";
                                return Json(status, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                context.Export_Endorsement_Rate_Master.Add(tab);
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

        public JsonResult GetCharge(int id)//...........Endorsement charge 
        {
            var result = (from cha in context.EndorsementChargeMasters.Where(u => u.ECTID == id)
                          select new { cha.ECMID, cha.ECMDESC }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        //------------------------Delete Record----------//
        [Authorize(Roles = "ExportEndorsementRateMasterDelete")]
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
                            Export_Endorsement_Rate_Master Export_Endorsement_Rate_Master = context.Export_Endorsement_Rate_Master.Find(Convert.ToInt32(id));
                            context.Export_Endorsement_Rate_Master.Remove(Export_Endorsement_Rate_Master);
                            context.SaveChanges();
                            Response.Write("Deleted Successfully ...");
                        }
                        else
                            Response.Write(temp);
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
        
        //End of Delete


    }
}