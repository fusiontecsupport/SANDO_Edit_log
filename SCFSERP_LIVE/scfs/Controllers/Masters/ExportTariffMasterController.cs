
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
using System.Data.Entity.Infrastructure;

namespace scfs_erp.Controllers.Masters
{
    public class ExportTariffMasterController : Controller
    {
        // GET: ExportTariffMaster

        SCFSERPContext context = new SCFSERPContext();

        //[Authorize(Roles = "ExportTariffMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(context.exporttariffmaster.ToList());//Loading Grid
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Export_TariffMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.TARIFFMCODE, d.TARIFFMDESC, d.DISPSTATUS, d.TARIFFMID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //[Authorize(Roles = "ExportTariffMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/ExportTariffMaster/Form/" + id);

            //Response.Redirect("/ExportTariffMaster/Form/" + id);
        }

        //----------------------Initializing Form--------------------------//
        //[Authorize(Roles = "ExportTariffMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ExportTariffMaster tab = new ExportTariffMaster();
            tab.TARIFFMID = 0;

            ViewBag.TARIFFTMID = new SelectList(context.exporttarifftypemasters.Where(x => x.DISPSTATUS == 0), "TARIFFTMID", "TARIFFTMDESC");
            ViewBag.TGID = new SelectList(context.ExportTariffGroupMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;


            if (id != 0)//Edit Mode
            {
                tab = context.exporttariffmaster.Find(id);
                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
                ViewBag.TGID = new SelectList(context.ExportTariffGroupMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC", tab.TGID);
                ViewBag.TARIFFTMID = new SelectList(context.exporttarifftypemasters.Where(x => x.DISPSTATUS == 0), "TARIFFTMID", "TARIFFTMDESC", tab.TARIFFTMID);
            }
            return View(tab);
        }//End of Form



        //[Authorize(Roles = "ExportTariffMasterCreate")]
        public ActionResult TForm(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ExportTariffOperationMaster tab = new ExportTariffOperationMaster();
            ExportTariffOPDetails vm = new ExportTariffOPDetails();
            ViewBag.EOPTID = new SelectList(context.Export_OperationTypeMaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.EOPTDESC), "EOPTID", "EOPTDESC");
            ViewBag.id = id;

            var query = context.Database.SqlQuery<ExportTariffOperationMaster>("select * from ExportTariffOperationMaster where TARIFFMID=" + id).ToList();
            if (query.Count() > 0)
            {
                vm.tariffoperation = context.ExportTariffOperationMaster.Where(x => x.TARIFFMID == id).ToList();
            }

            return View(vm);
        }



        public void TSavedata(FormCollection sfrm)//...Schedule save
        {
            int TARIFFMID = Convert.ToInt32(sfrm["TARIFFMID"]);
            int ETID = 0;
            string S_DELIDS = "";

            ExportTariffOperationMaster ExportTariffOperationMaster = new ExportTariffOperationMaster();
            string[] T_ETID = sfrm.GetValues("ETID");
            string[] EOPTID = sfrm.GetValues("tariffoperation[0].EOPTID");


            for (int row = 0; row < T_ETID.Count(); row++)
            {
                ETID = Convert.ToInt32(T_ETID[row]);

                //if (ETID == -1)
                //{
                //    context.Database.ExecuteSqlCommand("DELETE FROM ExportTariffOperationMaster  WHERE TARIFFMID=" + TARIFFMID + "");
                //}
                //else
                //{
                if (ETID != 0) ExportTariffOperationMaster = context.ExportTariffOperationMaster.Find(ETID);

                ExportTariffOperationMaster.TARIFFMID = TARIFFMID;
                ExportTariffOperationMaster.EOPTID = Convert.ToInt32(EOPTID[row]);


                if (ETID == 0)
                {
                    context.ExportTariffOperationMaster.Add(ExportTariffOperationMaster);
                    context.SaveChanges();
                    ETID = ExportTariffOperationMaster.ETID;
                }
                else
                {
                    context.Entry(ExportTariffOperationMaster).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                // }
                S_DELIDS = S_DELIDS + "," + ETID.ToString();
            }
            context.Database.ExecuteSqlCommand("DELETE FROM ExportTariffOperationMaster  WHERE TARIFFMID=" + TARIFFMID + " and  ETID NOT IN(" + S_DELIDS.Substring(1) + ")");
            Response.Write("saved");
        }
        //---------------------Insert or Modify data------------------//
        //public void savedata(ExportTariffMaster tab)
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
        //        context.exporttariffmaster.Add(tab);
        //        context.SaveChanges();
        //    }
        //    Response.Redirect("Index");
        //}

        [HttpPost]
        public JsonResult savedata(ExportTariffMaster tab)
        {
            using (SCFSERPContext dataContext = new SCFSERPContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
                {
                    string status = "";
                    try
                    {


                        if ((tab.CUSRID == "" || tab.CUSRID == null) && ((tab.TARIFFMID).ToString() == "0"))
                        {
                            if (Session["CUSRID"] != null)
                            {
                                tab.CUSRID = Session["CUSRID"].ToString();
                            }
                            else { tab.CUSRID = "0"; }
                        }
                        tab.LMUSRID = Session["CUSRID"].ToString();
                        tab.PRCSDATE = DateTime.Now;

                        if ((tab.TARIFFMID).ToString() != "0")
                        {

                            context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            status = "Update";
                            return Json(status, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            var query = context.exporttariffmaster.SqlQuery("SELECT *FROM EXPORTTARIFFMASTER  WHERE TARIFFMDESC='" + tab.TARIFFMDESC + "' AND TARIFFMCODE='" + tab.TARIFFMCODE + "'").ToList<ExportTariffMaster>();

                            if (query.Count != 0)
                            {
                                status = "Existing";
                                return Json(status, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                context.exporttariffmaster.Add(tab);
                                context.SaveChanges();

                                status = "Success";
                                return Json(status, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    
                    catch (DbUpdateConcurrencyException ex1)
                    {
                        trans.Rollback();

                        status = "Error";
                        //ex1.Entries.Single().Reload();
                        string Message = ex1.Message.ToString();
                        return Json(status, Message, JsonRequestBehavior.AllowGet);
                        //Response.Write("Sorry!! An Error Occurred.... ");
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


        //End of savedata


        //------------------------Delete Record----------//
        [Authorize(Roles = "ExportTariffMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                ExportTariffMaster exporttariffmaster = context.exporttariffmaster.Find(Convert.ToInt32(id));
                context.exporttariffmaster.Remove(exporttariffmaster);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        //End of Delete
    }
}