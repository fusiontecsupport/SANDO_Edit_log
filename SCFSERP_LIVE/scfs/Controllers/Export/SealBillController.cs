using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using scfs_erp;
using scfs_erp.Context;

using scfs.Data;

using scfs_erp.Helper;
using scfs_erp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using static scfs_erp.Models.EInvoice;

namespace scfs_erp.Controllers.Export
{
    [SessionExpire]
    public class SealBillController : Controller
    {
        //
        // GET: /SealBill/
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

        [Authorize(Roles = "ExportSealBillIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            if (string.IsNullOrEmpty(Session["SDATE"] as string))
            {

                Session["SDATE"] = DateTime.Now.ToString("dd-MM-yyyy");
                Session["EDATE"] = DateTime.Now.ToString("dd-MM-yyyy");
            }
            else
            {
                if (Request.Form.Get("from") != null)
                {
                    Session["SDATE"] = Request.Form.Get("from");
                    Session["EDATE"] = Request.Form.Get("to");
                }

            }
            if (Request.Form.Get("TRANBTYPE") != null)
            {
                Session["TRANBTYPE"] = Request.Form.Get("TRANBTYPE");
                Session["REGSTRID"] = Request.Form.Get("REGSTRID");
            }
            else
            {
                Session["TRANBTYPE"] = "3";
                Session["REGSTRID"] = "16";
            }
            //...........Bill type......//
            List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
            SelectListItem selectedItemGPTY = new SelectListItem { Text = "SEAL", Value = "3", Selected = true };
            selectedBILLYPE.Add(selectedItemGPTY);
            ViewBag.TRANBTYPE = selectedBILLYPE;
            //....end

            //............Billed to....//
            //ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID != 1).Where(x => x.REGSTRID != 2).Where(x => x.REGSTRID != 6).Where(x => x.REGSTRID != 46).Where(x => x.REGSTRID != 47).Where(x => x.REGSTRID != 48).Where(x => x.REGSTRID != 51).Where(x => x.REGSTRID != 52).Where(x => x.REGSTRID != 53), "REGSTRID", "REGSTRDESC");
            ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == 16 || x.REGSTRID == 17 || x.REGSTRID == 18), "REGSTRID", "REGSTRDESC", Convert.ToInt32(Session["REGSTRID"]));
            //.....end


            return View();
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Export_Seal_Billing(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToInt32(Session["TRANBTYPE"]), Convert.ToInt32(Session["REGSTRID"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANTIME.Value.ToString("hh:mm tt"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANNAMT.ToString(), d.ACKNO.ToString(), d.DISPSTATUS, d.GSTAMT.ToString(), d.TRANMID.ToString() }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        //{
        //    using (var e = new CFSExportEntities())
        //    {
        //        var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
        //        var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

        //        var data = e.pr_Search_Export_Seal_Billing(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
        //            totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToInt32(Session["TRANBTYPE"]), Convert.ToInt32(Session["REGSTRID"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
        //        var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANTIME.Value.ToString("hh:mm tt"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANNAMT.ToString(), d.ACKNO.ToString(), d.DISPSTATUS,  d.GSTAMT.ToString(), d.TRANMID.ToString() }).ToArray();
        //        return Json(new
        //        {
        //            sEcho = param.sEcho,
        //            aaData = aaData,
        //            iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
        //            iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        [Authorize(Roles = "ExportSealBillEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/SealBill/GSTForm/" + id);
        }
        [Authorize(Roles = "ExportSealBillCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            TransactionMaster tab = new TransactionMaster();
            TransactionMD vm = new TransactionMD();

            //..........................................Dropdown data.........................//
            ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6 && m.DISPSTATUS == 0).OrderBy(m => m.CATENAME), "CATEID", "CATENAME");
            ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(m => m.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", "TARIFFTMID");
            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.SBMDATE = DateTime.Now;
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.TRANLMID = new SelectList("");
            ViewBag.TARIFFGID = new SelectList(context.ExportTariffGroupMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");
            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "SEAL", Value = "3", Selected = true };
            selectedBILLTYPE.Add(selectedItemDSP);
            //selectedItemDSP = new SelectListItem { Text = "GRT", Value = "2", Selected = false };
            //selectedBILLTYPE.Add(selectedItemDSP);
            ViewBag.TRANBTYPE = selectedBILLTYPE;
            //....end

            //............Billed to....//
            //ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID != 1).Where(x => x.REGSTRID != 2).Where(x => x.REGSTRID != 6).Where(x => x.REGSTRID != 46).Where(x => x.REGSTRID != 47).Where(x => x.REGSTRID != 48).Where(x => x.REGSTRID != 51).Where(x => x.REGSTRID != 52).Where(x => x.REGSTRID != 53), "REGSTRID", "REGSTRDESC");
            ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == 16 || x.REGSTRID == 17 || x.REGSTRID == 18), "REGSTRID", "REGSTRDESC", Convert.ToInt32(Session["REGSTRID"]));
            //.....end

            //........display status.........//
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemDISP = new SelectListItem { Text = "In Books", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItemDISP);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            //....end
            if (id != 0)//....Edit Mode
            {
                tab = context.transactionmaster.Find(id);//find selected record

                //...................................Selected dropdown value..................................//
                ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME", tab.LCATEID);
                ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC", tab.BANKMID);
                ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL", tab.TRANMODE);
                ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == tab.REGSTRID), "REGSTRID", "REGSTRDESC", tab.REGSTRID);


                /*....GETTNG HANDLING SLABTYPE ..?*/

                var result = context.Database.SqlQuery<Nullable<int>>("select SLABTID from StuffingMaster where STFMID=" + tab.TRANLMID).ToList();
                if (result.Count == 0) ViewBag.SLABTID = 0;
                else ViewBag.SLABTID = Convert.ToInt32(result[0]);
                /*END*/
                //.................Display status.................//
                List<SelectListItem> selectedDISP = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItemDIS = new SelectListItem { Text = "In Books", Value = "0", Selected = false };
                    selectedDISP.Add(selectedItemDIS);
                    selectedItemDIS = new SelectListItem { Text = "Cancelled", Value = "1", Selected = true };
                    selectedDISP.Add(selectedItemDIS);
                    ViewBag.DISPSTATUS = selectedDISP;
                }
                else
                {
                    SelectListItem selectedItemDIS = new SelectListItem { Text = "In Books", Value = "0", Selected = true };
                    selectedDISP.Add(selectedItemDIS);
                    selectedItemDIS = new SelectListItem { Text = "Cancelled", Value = "1", Selected = false };
                    selectedDISP.Add(selectedItemDIS);
                    ViewBag.DISPSTATUS = selectedDISP;
                }//....end

                ////....................Bill type.................//
                //List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
                //if (Convert.ToInt32(tab.TRANBTYPE) == 1)
                //{
                //    SelectListItem selectedItemGPTY = new SelectListItem { Text = "STUFF", Value = "1", Selected = true };
                //    selectedBILLYPE.Add(selectedItemGPTY);
                //    selectedItemGPTY = new SelectListItem { Text = "GRT", Value = "2", Selected = false };
                //    selectedBILLYPE.Add(selectedItemGPTY);

                //}
                //else
                //{
                //    SelectListItem selectedItemGPTY = new SelectListItem { Text = "STUFF", Value = "1", Selected = false };
                //    selectedBILLYPE.Add(selectedItemGPTY);
                //    selectedItemGPTY = new SelectListItem { Text = "GRT", Value = "2", Selected = true };
                //    selectedBILLYPE.Add(selectedItemGPTY);

                //}
                //ViewBag.TRANBTYPE = selectedBILLYPE;
                ////..........end

                vm.masterdata = context.transactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.detaildata = context.transactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.costfactor = context.transactionmasterfactor.Where(det => det.TRANMID == id).ToList();
                vm.viewdata = context.Database.SqlQuery<pr_Export_Invoice_Stuff_Flx_Assgn_Result>("pr_Export_Invoice_Stuff_Flx_Assgn @PSTFMID=" + tab.TRANLMID + ",@PEDATE='" + tab.TRANDATE.ToString("MM/dd/yyyy") + "',@PTRANMID=" + id + "").ToList();//........procedure  for edit mode details data
                var tariffmid = Convert.ToInt32(vm.detaildata[0].TARIFFMID);
                var qry = context.Database.SqlQuery<int>("select TGID from EXPORTTARIFFMASTER WHERE TARIFFMID=" + tariffmid).ToList();

                ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", vm.detaildata[0].TARIFFMID);
                ViewBag.TARIFFGID = new SelectList(context.ExportTariffGroupMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC", qry[0]);
            }
            return View(vm);
        }



        //GST FORM

        [Authorize(Roles = "ExportSealBillCreate")]
        public ActionResult GSTForm(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            TransactionMaster tab = new TransactionMaster();
            TransactionMD vm = new TransactionMD();

            //..........................................Dropdown data.........................//
            ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6 && m.DISPSTATUS == 0).OrderBy(m => m.CATENAME), "CATEID", "CATENAME");
            ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(m => m.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", "TARIFFTMID");
            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.SBMDATE = DateTime.Now;
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.TRANLMID = new SelectList("");
            ViewBag.TARIFFGID = new SelectList(context.ExportTariffGroupMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");
            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "SEAL", Value = "3", Selected = true };
            selectedBILLTYPE.Add(selectedItemDSP);
            //selectedItemDSP = new SelectListItem { Text = "GRT", Value = "2", Selected = false };
            //selectedBILLTYPE.Add(selectedItemDSP);
            ViewBag.TRANBTYPE = selectedBILLTYPE;
            //....end

            //............Billed to....//
            //ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID != 1).Where(x => x.REGSTRID != 2).Where(x => x.REGSTRID != 6).Where(x => x.REGSTRID != 46).Where(x => x.REGSTRID != 47).Where(x => x.REGSTRID != 48).Where(x => x.REGSTRID != 51).Where(x => x.REGSTRID != 52).Where(x => x.REGSTRID != 53), "REGSTRID", "REGSTRDESC");
            ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == 16 || x.REGSTRID == 17 || x.REGSTRID == 18), "REGSTRID", "REGSTRDESC", Convert.ToInt32(Session["REGSTRID"]));
            //.....end

            //........display status.........//
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemDISP = new SelectListItem { Text = "In Books", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItemDISP);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            //....end
            if (id != 0)//....Edit Mode
            {
                tab = context.transactionmaster.Find(id);//find selected record

                //...................................Selected dropdown value..................................//
                ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME", tab.LCATEID);
                ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC", tab.BANKMID);
                ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL", tab.TRANMODE);
                ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == tab.REGSTRID), "REGSTRID", "REGSTRDESC", tab.REGSTRID);



                /*....GETTNG HANDLING SLABTYPE ..?*/

                var result = context.Database.SqlQuery<Nullable<int>>("select SLABTID from StuffingMaster where STFMID=" + tab.TRANLMID).ToList();
                if (result.Count == 0) ViewBag.SLABTID = 0;
                else ViewBag.SLABTID = Convert.ToInt32(result[0]);
                /*END*/
                //.................Display status.................//
                List<SelectListItem> selectedDISP = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItemDIS = new SelectListItem { Text = "In Books", Value = "0", Selected = false };
                    selectedDISP.Add(selectedItemDIS);
                    selectedItemDIS = new SelectListItem { Text = "Cancelled", Value = "1", Selected = true };
                    selectedDISP.Add(selectedItemDIS);
                    ViewBag.DISPSTATUS = selectedDISP;
                }
                else
                {
                    SelectListItem selectedItemDIS = new SelectListItem { Text = "In Books", Value = "0", Selected = true };
                    selectedDISP.Add(selectedItemDIS);
                    selectedItemDIS = new SelectListItem { Text = "Cancelled", Value = "1", Selected = false };
                    selectedDISP.Add(selectedItemDIS);
                    ViewBag.DISPSTATUS = selectedDISP;
                }//....end

                ////....................Bill type.................//
                //List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
                //if (Convert.ToInt32(tab.TRANBTYPE) == 1)
                //{
                //    SelectListItem selectedItemGPTY = new SelectListItem { Text = "STUFF", Value = "1", Selected = true };
                //    selectedBILLYPE.Add(selectedItemGPTY);
                //    selectedItemGPTY = new SelectListItem { Text = "GRT", Value = "2", Selected = false };
                //    selectedBILLYPE.Add(selectedItemGPTY);

                //}
                //else
                //{
                //    SelectListItem selectedItemGPTY = new SelectListItem { Text = "STUFF", Value = "1", Selected = false };
                //    selectedBILLYPE.Add(selectedItemGPTY);
                //    selectedItemGPTY = new SelectListItem { Text = "GRT", Value = "2", Selected = true };
                //    selectedBILLYPE.Add(selectedItemGPTY);

                //}
                //ViewBag.TRANBTYPE = selectedBILLYPE;
                ////..........end

                vm.masterdata = context.transactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.detaildata = context.transactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.costfactor = context.transactionmasterfactor.Where(det => det.TRANMID == id).ToList();
                vm.viewdata = context.Database.SqlQuery<pr_Export_Invoice_Stuff_Flx_Assgn_Result>("pr_Export_Invoice_Stuff_Flx_Assgn @PSTFMID=" + tab.TRANLMID + ",@PEDATE='" + tab.TRANDATE.ToString("MM/dd/yyyy") + "',@PTRANMID=" + id + "").ToList();//........procedure  for edit mode details data
                var tariffmid = Convert.ToInt32(vm.detaildata[0].TARIFFMID);
                var qry = context.Database.SqlQuery<int>("select TGID frOm EXPORTTARIFFMASTER WHERE TARIFFMID=" + tariffmid).ToList();

                ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", vm.detaildata[0].TARIFFMID);
                ViewBag.TARIFFGID = new SelectList(context.ExportTariffGroupMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC", qry[0]);
            }
            return View(vm);
        }
        /*PRINT DETAIL*/
        public ActionResult CForm(string id)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            var param = id.Split('~');

            var ids = Convert.ToInt32(param[0]);
            var gstamt = Convert.ToInt32(param[1]);

            ViewBag.id = ids;
            ViewBag.FGSTAMT = gstamt;

            var query = context.Database.SqlQuery<TransactionMaster>("select * from TransactionMaster where TRANMID=" + ids).ToList();
            //if (query[0].TRANCSNAME != null)
            //{
            //    ViewBag.TRANCSNAME = query[0].TRANCSNAME;
            //    ViewBag.TRANIMPADDR1 = query[0].TRANIMPADDR1;
            //    ViewBag.TRANIMPADDR2 = query[0].TRANIMPADDR2;
            //    ViewBag.TRANIMPADDR3 = query[0].TRANIMPADDR3;
            //    ViewBag.TRANIMPADDR4 = query[0].TRANIMPADDR4;
            //}
            //else
            //{
            //    var chaid = Convert.ToInt32(query[0].TRANREFID);
            //    var sql = context.Database.SqlQuery<CategoryMaster>("select * from CategoryMaster where CATEID=" + chaid).ToList();
            //    ViewBag.TRANCSNAME = sql[0].CATENAME;
            //    ViewBag.TRANIMPADDR1 = sql[0].CATEADDR1;
            //    ViewBag.TRANIMPADDR2 = sql[0].CATEADDR2;
            //    ViewBag.TRANIMPADDR3 = sql[0].CATEADDR3;
            //    ViewBag.TRANIMPADDR4 = sql[0].CATEADDR4;
            //}            
            if (query[0].TRANCSNAME != null)
            {
                ViewBag.TRANCSNAME = query[0].TRANCSNAME;
                ViewBag.TRANIMPADDR1 = query[0].TRANIMPADDR1;
                ViewBag.TRANIMPADDR2 = query[0].TRANIMPADDR2;
                ViewBag.TRANIMPADDR3 = query[0].TRANIMPADDR3;
                ViewBag.TRANIMPADDR4 = query[0].TRANIMPADDR4;
                ViewBag.OSBILLGSTNO = query[0].CATEAGSTNO;
                ViewBag.STATEID = query[0].STATEID;
                ViewBag.OCATEAID = new SelectList("");
            }
            else
            {
                ViewBag.TRANCSNAME = query[0].TRANREFNAME;
                ViewBag.TRANIMPADDR1 = query[0].TRANIMPADDR1;
                ViewBag.TRANIMPADDR2 = query[0].TRANIMPADDR2;
                ViewBag.TRANIMPADDR3 = query[0].TRANIMPADDR3;
                ViewBag.TRANIMPADDR4 = query[0].TRANIMPADDR4;
                ViewBag.OSBILLGSTNO = query[0].CATEAGSTNO;
                ViewBag.STATEID = query[0].STATEID;
                ViewBag.OCATEAID = new SelectList("");
            }

            return View();
        }
        //.................Insert/update values into database.............//
        [HttpPost]
        public ActionResult SaveAddress(FormCollection tab)
        {
            string status = "";
            using (var trans = context.Database.BeginTransaction())
            {
                try
                {
                    string TRANMID = Convert.ToString(tab["TRANMID"]);
                    string STFMID = Convert.ToString(tab["STFMID"]);

                    string TRANREFNAME = Convert.ToString(tab["TRANREFNAME"]);
                    string TRANREFID = Convert.ToString(tab["TRANREFID"]);

                    string CATEAID = Convert.ToString(tab["CATEAID"]);
                    string CATEAGSTNO = Convert.ToString(tab["CATEAGSTNO"]);
                    string STATEID = Convert.ToString(tab["STATEID"]);
                    string TRANIMPADDR1 = Convert.ToString(tab["TRANIMPADDR1"]);
                    string TRANIMPADDR2 = Convert.ToString(tab["TRANIMPADDR2"]);
                    string TRANIMPADDR3 = Convert.ToString(tab["TRANIMPADDR3"]);
                    string TRANIMPADDR4 = Convert.ToString(tab["TRANIMPADDR4"]);

                    if (STFMID != "" || STFMID != null || STFMID != "0")
                    {
                        string uquery = " UPDATE STUFFINGMASTER SET STFBILLREFNAME = '" + Convert.ToString(TRANREFNAME) + "', STFBILLREFID = " + Convert.ToInt32(TRANREFID) + ",";
                        uquery += " STFBCATEAID = " + Convert.ToInt32(CATEAID) + ", STFBCHAGSTNO = '" + Convert.ToString(CATEAGSTNO) + "',";
                        uquery += " STFBCHASTATEID = " + Convert.ToInt32(STATEID) + ", ";
                        uquery += " STFBCHAADDR1 = '" + Convert.ToString(TRANIMPADDR1) + "',";
                        uquery += " STFBCHAADDR2 = '" + Convert.ToString(TRANIMPADDR2) + "',";
                        uquery += " STFBCHAADDR3 = '" + Convert.ToString(TRANIMPADDR3) + "',";
                        uquery += " STFBCHAADDR4 = '" + Convert.ToString(TRANIMPADDR4) + "' ";
                        uquery += "  WHERE STFMID =" + Convert.ToInt32(STFMID) + " ";
                        context.Database.ExecuteSqlCommand(uquery);

                    }


                    if (TRANMID != "" || TRANMID != null || TRANMID != "0")
                    {
                        string uquery = " UPDATE TRANSACTIONMASTER SET TRANREFNAME = '" + Convert.ToString(TRANREFNAME) + "', TRANREFID = " + Convert.ToInt32(TRANREFID) + ",";
                        uquery += " CATEAID = " + Convert.ToInt32(CATEAID) + ", CATEAGSTNO = '" + Convert.ToString(CATEAGSTNO) + "',";
                        uquery += " STATEID = " + Convert.ToInt32(STATEID) + ", ";
                        uquery += " TRANIMPADDR1 = '" + Convert.ToString(TRANIMPADDR1) + "',";
                        uquery += " TRANIMPADDR2 = '" + Convert.ToString(TRANIMPADDR2) + "',";
                        uquery += " TRANIMPADDR3 = '" + Convert.ToString(TRANIMPADDR3) + "',";
                        uquery += " TRANIMPADDR4 = '" + Convert.ToString(TRANIMPADDR4) + "' ";
                        uquery += "  WHERE TRANMID =" + Convert.ToInt32(TRANMID) + " ";
                        context.Database.ExecuteSqlCommand(uquery);

                    }

                    status = "saved";
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    status = ex.Message.ToString();
                    trans.Rollback();
                }
            }

            return Json(status, JsonRequestBehavior.AllowGet);

        }/*END*/


        public JsonResult AFormAddr(string id)
        {

            var param = id.Split('~');

            var ids = Convert.ToInt32(param[0]);
            var billedto = Convert.ToInt32(param[1]);

            if (billedto == 0)
            {
                var query = context.Database.SqlQuery<VW_EXPORT_TRANSACTION_ADDRESS_DETAIL_ASSGN>("select TRANCSNAME,TRANIMPADDR1,TRANIMPADDR2,TRANIMPADDR3,TRANIMPADDR4,CHACATEGSTNO from VW_EXPORT_TRANSACTION_ADDRESS_DETAIL_ASSGN where TRANMID=" + ids).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = context.Database.SqlQuery<VW_EXPORT_TRANSACTION_ADDRESS_DETAIL_ASSGN>("select EXPRTNAME,EXPRTCATEADDR1,EXPRTCATEADDR2,EXPRTCATEADDR3,EXPRTCATEADDR4,EXPRTCATEGSTNO from VW_EXPORT_TRANSACTION_ADDRESS_DETAIL_ASSGN where TRANMID=" + ids).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);

            }

        }




        //.................Insert/update values into database.............//
        public void savedata(FormCollection F_Form)
        {
            using (SCFSERPContext dataContext = new SCFSERPContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        string todaydt = Convert.ToString(DateTime.Now);
                        string todayd = Convert.ToString(DateTime.Now.Date);

                        TransactionMaster transactionmaster = new TransactionMaster();
                        TransactionDetail transactiondetail = new TransactionDetail();
                        //-------Getting Primarykey field--------
                        Int32 TRANMID = Convert.ToInt32(F_Form["masterdata[0].TRANMID"]);
                        Int32 TRANDID = 0;
                        string DELIDS = "";
                        //-----End


                        if (TRANMID != 0)
                        {
                            transactionmaster = context.transactionmaster.Find(TRANMID);
                        }

                        //...........transaction master.............//
                        transactionmaster.TRANMID = TRANMID;
                        transactionmaster.COMPYID = Convert.ToInt32(Session["compyid"]);
                        transactionmaster.SDPTID = 2;
                        transactionmaster.TRANTID = 2;
                        transactionmaster.TRANLMID = Convert.ToInt32(F_Form["TRANLMID"]);
                        transactionmaster.TRANLSID = 0;
                        transactionmaster.TRANLSNO = null;
                        transactionmaster.TRANLMNO = F_Form["masterdata[0].TRANLMNO"].ToString();
                        transactionmaster.TRANLMDATE = DateTime.Now;
                        transactionmaster.TRANLSDATE = DateTime.Now;
                        transactionmaster.TRANNARTN = null;
                        transactionmaster.CUSRID = Session["CUSRID"].ToString();
                        if (TRANMID == 0)
                        {
                       	 transactionmaster.CUSRID = Session["CUSRID"].ToString();
                            
                        }
                        transactionmaster.LMUSRID = Session["CUSRID"].ToString();
                        transactionmaster.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
                        transactionmaster.PRCSDATE = DateTime.Now;
                                               

                        string indate = Convert.ToString(F_Form["masterdata[0].TRANDATE"]);
                        string intime = Convert.ToString(F_Form["masterdata[0].TRANTIME"]);

                        if (indate != null || indate != "")
                        {
                            transactionmaster.TRANDATE = Convert.ToDateTime(indate).Date;
                        }
                        else { transactionmaster.TRANDATE = DateTime.Now.Date; }

                        if (transactionmaster.TRANDATE > Convert.ToDateTime(todayd))
                        {
                            transactionmaster.TRANDATE = Convert.ToDateTime(todayd);
                        }

                        if ((intime != null || intime != "") && ((indate != null || indate != "")))
                        {
                            if ((intime.Contains(' ')) && (indate.Contains(' ')))
                            {
                                var in_time = intime.Split(' ');
                                var in_date = indate.Split(' ');

                                if ((in_time[1].Contains(':')) && (in_date[0].Contains('/')))
                                {

                                    var in_time1 = in_time[1].Split(':');
                                    var in_date1 = in_date[0].Split('/');

                                    string in_datetime = in_date1[2] + "-" + in_date1[1] + "-" + in_date1[0] + "  " + in_time1[0] + ":" + in_time1[1] + ":" + in_time1[2];

                                    transactionmaster.TRANTIME = Convert.ToDateTime(in_datetime);
                                }
                                else { transactionmaster.TRANTIME = DateTime.Now; }
                            }
                            else
                            {
                                var in_time = intime;
                                var in_date = indate;

                                if ((in_time.Contains(':')) && (in_date.Contains('/')))
                                {

                                    var in_time1 = in_time.Split(':');
                                    var in_date1 = in_date.Split('/');

                                    string in_datetime = in_date1[2] + "-" + in_date1[1] + "-" + in_date1[0] + "  " + in_time1[0] + ":" + in_time1[1] + ":" + in_time1[2];

                                    transactionmaster.TRANTIME = Convert.ToDateTime(in_datetime);
                                }
                                else { transactionmaster.TRANTIME = DateTime.Now; }

                            }
                        }
                        else { transactionmaster.TRANTIME = DateTime.Now; }

                        if (transactionmaster.TRANTIME > Convert.ToDateTime(todaydt))
                        {
                            transactionmaster.TRANTIME = Convert.ToDateTime(todaydt);
                        }

                        //transactionmaster.TRANDATE = Convert.ToDateTime(F_Form["masterdata[0].TRANTIME"]).Date;
                        //transactionmaster.TRANTIME = Convert.ToDateTime(F_Form["masterdata[0].TRANTIME"]);

                        transactionmaster.TRANREFID = Convert.ToInt32(F_Form["masterdata[0].TRANREFID"]);
                        transactionmaster.TRANREFNAME = F_Form["masterdata[0].TRANREFNAME"].ToString();
                        transactionmaster.LCATEID = Convert.ToInt32(F_Form["LCATEID"]);
                        transactionmaster.TRANBTYPE = Convert.ToInt16(F_Form["TRANBTYPE"]);
                        transactionmaster.REGSTRID = Convert.ToInt16(F_Form["REGSTRID"]);
                        transactionmaster.TRANMODE = Convert.ToInt16(F_Form["TRANMODE"]);
                        transactionmaster.TRANMODEDETL = (F_Form["masterdata[0].TRANMODEDETL"]);
                        transactionmaster.TRANGAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANGAMT"]);
                        transactionmaster.TRANNAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANNAMT"]);
                        transactionmaster.TRANROAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANROAMT"]);
                        transactionmaster.TRANREFAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANREFAMT"]);
                        transactionmaster.TRANRMKS = (F_Form["masterdata[0].TRANRMKS"]).ToString();
                        transactionmaster.TRANAMTWRDS = AmtInWrd.ConvertNumbertoWords(F_Form["masterdata[0].TRANNAMT"]);

                        if (F_Form["TRANSAMT"].Length != 0)
                        { transactionmaster.TRANSAMT = Convert.ToDecimal(F_Form["TRANSAMT"]); }
                        else
                        { transactionmaster.TRANSAMT = 0; }

                        if (F_Form["TRANPAMT"].Length != 0)
                        { transactionmaster.TRANPAMT = Convert.ToDecimal(F_Form["TRANPAMT"]); }
                        else
                        { transactionmaster.TRANPAMT = 0; }

                        if (F_Form["TRANHAMT"].Length != 0)
                        { transactionmaster.TRANHAMT = Convert.ToDecimal(F_Form["TRANHAMT"]); }
                        else
                        { transactionmaster.TRANHAMT = 0; }

                        if (F_Form["TRANEAMT"].Length != 0)
                        { transactionmaster.TRANEAMT = Convert.ToDecimal(F_Form["TRANEAMT"]); }
                        else
                        { transactionmaster.TRANEAMT = 0; }

                        if (F_Form["TRANFAMT"].Length != 0)
                        { transactionmaster.TRANFAMT = Convert.ToDecimal(F_Form["TRANFAMT"]); }
                        else
                        { transactionmaster.TRANFAMT = 0; }

                        //transactionmaster.TRANSAMT = Convert.ToDecimal(F_Form["TRANSAMT"]);
                        //transactionmaster.TRANPAMT = Convert.ToDecimal(F_Form["TRANPAMT"]);
                        //transactionmaster.TRANHAMT = Convert.ToDecimal(F_Form["TRANHAMT"]);
                        //transactionmaster.TRANEAMT = Convert.ToDecimal(F_Form["TRANEAMT"]);
                        //transactionmaster.TRANFAMT = Convert.ToDecimal(F_Form["TRANFAMT"]);
                        if (F_Form["TRANTCAMT"].Length != 0)
                        { transactionmaster.TRANTCAMT = Convert.ToDecimal(F_Form["TRANTCAMT"]); }
                        else
                        { transactionmaster.TRANTCAMT = 0; }

                        transactionmaster.STRG_HSNCODE = F_Form["STRG_HSN_CODE"].ToString();
                        transactionmaster.HANDL_HSNCODE = F_Form["HANDL_HSN_CODE"].ToString();

                        transactionmaster.STRG_TAXABLE_AMT = Convert.ToDecimal(F_Form["STRG_TAXABLE_AMT"]);
                        transactionmaster.HANDL_TAXABLE_AMT = Convert.ToDecimal(F_Form["HANDL_TAXABLE_AMT"]);

                        transactionmaster.STRG_CGST_EXPRN = Convert.ToDecimal(F_Form["STRG_CGST_EXPRN"]);
                        transactionmaster.STRG_SGST_EXPRN = Convert.ToDecimal(F_Form["STRG_SGST_EXPRN"]);
                        transactionmaster.STRG_IGST_EXPRN = Convert.ToDecimal(F_Form["STRG_IGST_EXPRN"]);
                        transactionmaster.STRG_CGST_AMT = Convert.ToDecimal(F_Form["STRG_CGST_AMT"]);
                        transactionmaster.STRG_SGST_AMT = Convert.ToDecimal(F_Form["STRG_SGST_AMT"]);
                        transactionmaster.STRG_IGST_AMT = Convert.ToDecimal(F_Form["STRG_IGST_AMT"]);

                        transactionmaster.HANDL_CGST_EXPRN = Convert.ToDecimal(F_Form["HANDL_CGST_EXPRN"]);
                        transactionmaster.HANDL_SGST_EXPRN = Convert.ToDecimal(F_Form["HANDL_SGST_EXPRN"]);
                        transactionmaster.HANDL_IGST_EXPRN = Convert.ToDecimal(F_Form["HANDL_IGST_EXPRN"]);
                        transactionmaster.HANDL_CGST_AMT = Convert.ToDecimal(F_Form["HANDL_CGST_AMT"]);
                        transactionmaster.HANDL_SGST_AMT = Convert.ToDecimal(F_Form["HANDL_SGST_AMT"]);
                        transactionmaster.HANDL_IGST_AMT = Convert.ToDecimal(F_Form["HANDL_IGST_AMT"]);

                        transactionmaster.TRANCGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANCGSTAMT"]);
                        transactionmaster.TRANSGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANSGSTAMT"]);
                        transactionmaster.TRANIGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANIGSTAMT"]);

                        transactionmaster.TRANNARTN = Convert.ToString(F_Form["masterdata[0].TRANNARTN"]);
                        var tranmode = Convert.ToInt16(F_Form["TRANMODE"]);
                        if (tranmode != 2 && tranmode != 3)
                        {
                            transactionmaster.TRANREFNO = "";
                            transactionmaster.TRANREFBNAME = "";
                            transactionmaster.BANKMID = 0;
                            transactionmaster.TRANREFDATE = DateTime.Now;
                        }
                        else
                        {
                            transactionmaster.TRANREFNO = (F_Form["masterdata[0].TRANREFNO"]).ToString();
                            transactionmaster.TRANREFBNAME = (F_Form["masterdata[0].TRANREFBNAME"]).ToString();
                            transactionmaster.BANKMID = Convert.ToInt32(F_Form["BANKMID"]);
                            transactionmaster.TRANREFDATE = Convert.ToDateTime(F_Form["masterdata[0].TRANREFDATE"]).Date;

                        }


                        //.................Autonumber............//
                        var regsid = Convert.ToInt32(F_Form["REGSTRID"]);
                        var btype = Convert.ToInt32(F_Form["TRANBTYPE"]);
                        if (TRANMID == 0)
                        {
                            transactionmaster.TRANNO = Convert.ToInt32(auto_numbr_invoice.gstexportautonum("transactionmaster", "TRANNO", F_Form["REGSTRID"].ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString(), Convert.ToInt32(transactionmaster.TRANREFID)).ToString());
                            //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.gstexportautonum("transactionmaster", "TRANNO", F_Form["REGSTRID"].ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString()).ToString());
                            int ano = transactionmaster.TRANNO;
                            //string format = "SUD/EXP/";
                            string format = "";
                            string btyp = auto_numbr_invoice.GetCateBillType(Convert.ToInt32(transactionmaster.TRANREFID)).ToString();
                            if (btyp == "")
                            {
                                format = "EXP/" + Session["GPrxDesc"] + "/";
                            }
                            else
                                format = "EXP" + Session["GPrxDesc"] + btyp;
                            string billformat = "";
                            switch (regsid)
                            {
                                case 16: billformat = "STL/SL/CU/"; break;
                                case 17: billformat = "STL/SL/CH/"; break;
                                case 18: billformat = "ZB/SEAL/"; break;

                            }
                            string prfx = string.Format(format + "{0:D5}", ano);
                            string billprfx = string.Format(billformat + "{0:D5}", ano);
                            transactionmaster.TRANDNO = prfx.ToString();
                            transactionmaster.TRANBILLREFNO = billprfx.ToString();

                            //........end of autonumber
                            context.transactionmaster.Add(transactionmaster);
                            context.SaveChanges();
                            TRANMID = transactiondetail.TRANMID;
                        }
                        else
                        {
                            //transactionmaster.REGSTRID = Convert.ToInt16(F_Form["masterdata[0].REGSTRID"]);
                            // transactionmaster.TRANMODE = Convert.ToInt16(F_Form["TRANMODE"]);
                            context.Entry(transactionmaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }


                        //-------------transaction Details
                        string[] F_TRANDID = F_Form.GetValues("TRANDID");
                        string[] TRANDREFNO = F_Form.GetValues("TRANDREFNO");
                        string[] TRANIDATE = F_Form.GetValues("TRANIDATE");
                        string[] TRANSDATE = F_Form.GetValues("TRANSDATE");
                        string[] TRANEDATE = F_Form.GetValues("TRANEDATE");
                        string[] STFDIDS = F_Form.GetValues("STFDIDS");
                        string[] STFDID = F_Form.GetValues("STFDID");
                        string[] boolSTFDIDS = F_Form.GetValues("boolSTFDIDS");
                        string[] TRANDHAMT = F_Form.GetValues("TRANDHAMT");
                        string[] TRANDEAMT = F_Form.GetValues("TRANDEAMT");
                        string[] TRANDFAMT = F_Form.GetValues("TRANDFAMT");
                        string[] TRANDPAMT = F_Form.GetValues("TRANDPAMT");
                        string[] TRANDNAMT = F_Form.GetValues("TRANDNAMT");
                        string[] TRANDSAMT = F_Form.GetValues("TRANDSAMT");
                        string[] TRANDNOP = F_Form.GetValues("TRANDNOP");
                        string[] TRANDQTY = F_Form.GetValues("TRANDQTY");
                        string[] TRANDREFID = F_Form.GetValues("TRANDREFID");
                        string[] RAMT1 = F_Form.GetValues("RAMT1");
                        string[] RAMT2 = F_Form.GetValues("RAMT2");
                        string[] RAMT3 = F_Form.GetValues("RAMT3");
                        string[] RAMT4 = F_Form.GetValues("RAMT4");
                        string[] RAMT5 = F_Form.GetValues("RAMT5");
                        string[] RAMT6 = F_Form.GetValues("RAMT6");
                        string[] RCAMT1 = F_Form.GetValues("RCAMT1");
                        string[] RCAMT2 = F_Form.GetValues("RCAMT2");
                        string[] RCAMT3 = F_Form.GetValues("RCAMT3");
                        string[] RCAMT4 = F_Form.GetValues("RCAMT4");
                        string[] RCAMT5 = F_Form.GetValues("RCAMT5");
                        string[] RCAMT6 = F_Form.GetValues("RCAMT6");
                        string[] RCOL1 = F_Form.GetValues("RCOL1");
                        string[] RCOL2 = F_Form.GetValues("RCOL2");
                        string[] RCOL3 = F_Form.GetValues("RCOL3");
                        string[] RCOL4 = F_Form.GetValues("RCOL4");
                        string[] RCOL5 = F_Form.GetValues("RCOL5");
                        string[] RCOL6 = F_Form.GetValues("RCOL6");
                        string[] days = F_Form.GetValues("days");
                        string[] TRAND_COVID_DISC_AMT = F_Form.GetValues("TRAND_COVID_DISC_AMT");

                        for (int count = 0; count < boolSTFDIDS.Count(); count++)
                        {
                            if (boolSTFDIDS[count] == "true")
                            {
                                TRANDID = Convert.ToInt32(F_TRANDID[count]);
                                var boolSTFDID = Convert.ToString(boolSTFDIDS[count]);
                                if (TRANDID != 0 && boolSTFDID == "true")
                                {
                                    transactiondetail = context.transactiondetail.Find(TRANDID);
                                }
                                transactiondetail.TRANMID = transactionmaster.TRANMID;
                                transactiondetail.TRANDREFNO = (TRANDREFNO[count]).ToString();
                                transactiondetail.TRANDREFNAME = (TRANDREFNO[count]).ToString();
                                transactiondetail.TRANDREFID = Convert.ToInt32(TRANDREFID[count]);
                                transactiondetail.TRANIDATE = Convert.ToDateTime(TRANIDATE[count]);
                                transactiondetail.TRANSDATE = Convert.ToDateTime(TRANSDATE[count]);
                                transactiondetail.TRANEDATE = Convert.ToDateTime(TRANEDATE[count]);
                                transactiondetail.TRANVDATE = DateTime.Now;
                                transactiondetail.TRANDSAMT = Convert.ToDecimal(TRANDSAMT[count]);
                                transactiondetail.TRANDHAMT = Convert.ToDecimal(TRANDHAMT[count]);
                                transactiondetail.TRANDEAMT = Convert.ToDecimal(TRANDEAMT[count]);
                                transactiondetail.TRANDFAMT = Convert.ToDecimal(TRANDFAMT[count]);
                                transactiondetail.TRANDPAMT = Convert.ToDecimal(TRANDPAMT[count]);
                                transactiondetail.TRANDNAMT = Convert.ToDecimal(TRANDNAMT[count]);
                                transactiondetail.TRANDNOP = Convert.ToDecimal(TRANDNOP[count]);
                                transactiondetail.TRANDQTY = Convert.ToDecimal(TRANDQTY[count]);
                                transactiondetail.TARIFFMID = Convert.ToInt32(F_Form["TARIFFMID"]);
                                transactiondetail.TRANDRATE = 0;
                                transactiondetail.STFDID = Convert.ToInt32(STFDID[count]);
                                transactiondetail.TRANDGAMT = Convert.ToDecimal(TRANDNAMT[count]);
                                transactiondetail.BILLEDID = 0;
                                transactiondetail.RCOL1 = Convert.ToDecimal(RCOL1[count]);
                                transactiondetail.RCOL2 = Convert.ToDecimal(RCOL2[count]);
                                transactiondetail.RCOL3 = Convert.ToDecimal(RCOL3[count]);
                                transactiondetail.RCOL4 = 0;
                                transactiondetail.RCOL5 = 0;
                                transactiondetail.RCOL6 = 0;
                                transactiondetail.RCOL7 = 0;
                                transactiondetail.RAMT1 = Convert.ToDecimal(RAMT1[count]);
                                transactiondetail.RAMT2 = Convert.ToDecimal(RAMT2[count]);
                                transactiondetail.RAMT3 = Convert.ToDecimal(RAMT3[count]);
                                transactiondetail.RAMT4 = 0;// Convert.ToDecimal(RAMT4[count]);
                                transactiondetail.RAMT5 = 0;// Convert.ToDecimal(RAMT5[count]);
                                transactiondetail.RAMT6 = 0;// Convert.ToDecimal(RAMT6[count]);
                                transactiondetail.RCAMT1 = Convert.ToDecimal(RCAMT1[count]);
                                transactiondetail.RCAMT2 = Convert.ToDecimal(RCAMT2[count]);
                                transactiondetail.RCAMT3 = Convert.ToDecimal(RCAMT3[count]);
                                transactiondetail.RCAMT4 = 0;
                                transactiondetail.RCAMT5 = 0;
                                transactiondetail.RCAMT6 = 0;
                                //transactiondetail.RCAMT4 = Convert.ToDecimal(RCAMT4[count]);
                                //transactiondetail.RCAMT5 = Convert.ToDecimal(RCAMT5[count]);
                                //transactiondetail.RCAMT6 = Convert.ToDecimal(RCAMT6[count]);
                                transactiondetail.SLABTID = 0;
                                transactiondetail.TRANYTYPE = 0;
                                transactiondetail.TRANDWGHT = 0;
                                transactiondetail.TRANDAID = 0;
                                transactiondetail.SBDID = 0;
                                transactiondetail.TRAND_COVID_DISC_AMT = Convert.ToDecimal(TRAND_COVID_DISC_AMT[count]);


                                if (Convert.ToInt32(TRANDID) == 0)
                                {
                                    context.transactiondetail.Add(transactiondetail);
                                    context.SaveChanges();
                                    TRANDID = transactiondetail.TRANDID;
                                }
                                else
                                {
                                    transactiondetail.TRANDID = TRANDID;
                                    context.Entry(transactiondetail).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                }//..............end
                                DELIDS = DELIDS + "," + TRANDID.ToString();
                            }
                        }

                        //-------delete transaction master factor-------//
                        context.Database.ExecuteSqlCommand("DELETE FROM transactionmasterfactor WHERE tranmid=" + TRANMID);

                        //Transaction Type Master-------//

                        TransactionMasterFactor transactionmasterfactors = new TransactionMasterFactor();
                        string[] DEDEXPRN = F_Form.GetValues("CFEXPR");
                        string[] TAX1 = F_Form.GetValues("TAX");
                        string[] DEDMODE = F_Form.GetValues("CFMODE");
                        string[] DEDTYPE = F_Form.GetValues("CFTYPE");
                        string[] DORDRID = F_Form.GetValues("DORDRID");
                        string[] DEDNOS = F_Form.GetValues("DEDNOS");
                        string[] DEDVALUE = F_Form.GetValues("CFAMOUNT");
                        string[] CFAMOUNT = F_Form.GetValues("CFAMOUNT");
                        string[] CFDESC = F_Form.GetValues("CFDESC");

                        if (CFDESC != null)//if (DORDRID != null)
                        {
                            for (int count2 = 0; count2 < CFDESC.Count(); count2++)
                            {

                                transactionmasterfactors.TRANMID = transactionmaster.TRANMID;
                                transactionmasterfactors.DORDRID = Convert.ToInt16(DORDRID[count2]);
                                transactionmasterfactors.DEDMODE = DEDMODE[count2].ToString();
                                transactionmasterfactors.DEDVALUE = Convert.ToDecimal(DEDVALUE[count2]);
                                transactionmasterfactors.DEDTYPE = Convert.ToInt16(DEDTYPE[count2]);
                                transactionmasterfactors.DEDEXPRN = Convert.ToDecimal(DEDEXPRN[count2]);
                                transactionmasterfactors.CFID = Convert.ToInt32(TAX1[count2]);
                                transactionmasterfactors.DEDCFDESC = CFDESC[count2];
                                transactionmasterfactors.DEDNOS = Convert.ToDecimal(DEDNOS[count2]);
                                transactionmasterfactors.CFOPTN = 0;
                                transactionmasterfactors.DEDORDR = 0;
                                context.transactionmasterfactor.Add(transactionmasterfactors);
                                context.SaveChanges();
                            }
                        }
                        context.Database.ExecuteSqlCommand("DELETE FROM transactiondetail  WHERE TRANMID=" + TRANMID + " and  TRANDID NOT IN(" + DELIDS.Substring(1) + ")");
                        trans.Commit();
                    }
                    catch (SqlException ex)
                    {
                        trans.Rollback();
                        throw ex;
                        // Response.Write("Sorry!!An Error Ocurred...");
                    }
                }
            }
            Response.Redirect("Index");
        }

        public ActionResult BForm(string id)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            var ids = Convert.ToInt32(id);
            //var gstamt = Convert.ToInt32(param[1]);

            //BILLED TO
            //.........s.Tax...//
            List<SelectListItem> selectedtaxlst1 = new List<SelectListItem>();
            SelectListItem selectedItemtax1 = new SelectListItem { Text = "CHA", Value = "0", Selected = true };
            selectedtaxlst1.Add(selectedItemtax1);
            ViewBag.BILLEDTO = selectedtaxlst1;


            ViewBag.TRANMID = ids;
            ViewBag.FGSTAMT = 0;//gstamt;

            ViewBag.TRANMID = ids;
            ViewBag.FGSTAMT = 0;//gstamt;
            var query = context.Database.SqlQuery<TransactionDetail>("select top 1 * from TransactionDetail where TRANMID=" + ids).ToList();
            if (query[0].TRANDREFID > 0)
            {
                var query1 = context.Database.SqlQuery<StuffingDetail>("select  top 1  * from STUFFINGDETAIL where GIDID =" + query[0].TRANDREFID).ToList();

                if (query1[0].STFMID > 0)
                {
                    var query2 = context.Database.SqlQuery<StuffingMaster>("select  top 1  * from STUFFINGMASTER where STFMID =" + query1[0].STFMID).ToList();

                    if (query2.Count > 0)
                    {
                        ViewBag.TRANMID = query[0].TRANMID;
                        ViewBag.STFMID = query1[0].STFMID;
                        ViewBag.TRANREFNAME = query2[0].STFBILLREFNAME;
                        ViewBag.TRANREFID = query2[0].STFBILLREFID;
                        ViewBag.CATEAGSTNO = query2[0].STFBCHAGSTNO;
                        ViewBag.STATEID = query2[0].STFBCHASTATEID;
                        ViewBag.TRANIMPADDR1 = query2[0].STFBCHAADDR1;
                        ViewBag.TRANIMPADDR2 = query2[0].STFBCHAADDR2;
                        ViewBag.TRANIMPADDR3 = query2[0].STFBCHAADDR3;
                        ViewBag.TRANIMPADDR4 = query2[0].STFBCHAADDR4;
                        ViewBag.CUENT_CATEAID = query2[0].STFBCATEAID;

                        int stateid = query2[0].STFBCHASTATEID;

                        var starqy = context.Database.SqlQuery<StateMaster>("Select *from STATEMASTER where STATEID = " + stateid).ToList();
                        if (starqy.Count > 0)
                        {
                            ViewBag.STATEDESC = starqy[0].STATECODE + "  " + starqy[0].STATEDESC;
                            ViewBag.STATETYPE = starqy[0].STATETYPE;
                        }

                        int chaid = Convert.ToInt32(query2[0].STFBILLREFID);
                        int chaaid = Convert.ToInt32(query2[0].STFBCATEAID);

                        var adds = context.Database.SqlQuery<Category_Address_Details>("Select *From CATEGORY_ADDRESS_DETAIL Where CATEAID  = " + chaaid).ToList();

                        if (adds.Count > 0)
                        {
                            ViewBag.CATEAID = new SelectList(context.categoryaddressdetails.Where(m => m.CATEID == chaid).OrderBy(x => x.CATEATYPEDESC), "CATEAID", "CATEATYPEDESC", chaaid).ToList();
                        }
                        else
                        {
                            ViewBag.CATEAID = new SelectList("");
                        }

                    }
                }
            }

            return View();
        }
        //--------Autocomplete CHA Name
        public JsonResult AutoCha(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //---End 

        public JsonResult GetOPTYP(int id)//Get Operation type for stuff
        {
            var result = context.Database.SqlQuery<VW_EXPORT_INVOICE_STUFFING_DETAIL_CTRL_ASSGN>("select * from VW_EXPORT_INVOICE_STUFFING_DETAIL_CTRL_ASSGN where STFMID=" + id).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTariff(string id)//Get tariff based on grp and operatn
        {
            //var param = id.Split(';');
            //var eoptid = Convert.ToInt32(param[0]);
            var tgid = Convert.ToInt32(id);//Convert.ToInt32(param[1]);
            // var result = context.Database.SqlQuery<VW_EXPORT_SEAL_INV_TARIFF_ASSGN>("select * from VW_EXPORT_SEAL_INV_TARIFF_ASSGN where EOPTID=" + eoptid + " and TGID=" + tgid).ToList();
            //var result = context.Database.SqlQuery<VW_EXPORT_INVOICE_OPERATION_WISE_TARIFF_ASSGN>("select * from VW_EXPORT_INVOICE_OPERATION_WISE_TARIFF_ASSGN where EOPTID=" + eoptid + " and TGID=" + tgid).ToList();
            var result = context.Database.SqlQuery<ExportTariffMaster>("select * from ExportTariffMaster where TGID=" + tgid + "").ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //---End 

        //.............procedure to display detail table...............

        //public string Detail(int CHAID)
        //{


        //    var query = context.Database.SqlQuery<PROC_EXPORT_INVOICE_SBILL_CONTAINER_NO_GRID_ASSGN_Result>("EXEC PROC_EXPORT_INVOICE_SBILL_CONTAINER_NO_GRID_ASSGN @PCHAID=" + CHAID).ToList();


        //        var tabl = " <div class='panel-heading navbar-inverse' style=color:white>Stuffing Bill Details</div><Table id=TDETAIL class='table table-striped table-bordered bootstrap-datatable'> <thead><tr><th></th><th>Container No</th><th>Size</th><th>In Date</th> <th>Storage Date</th><th>Charge Date</th><th>Storage</th><th>Handling</th><th>Energy</th><th>Fuel</th><th>PTI</th><th>Total</th></tr> </thead>";
        //        var count = 0;

        //        foreach(var rslt in query)
        //        {
        //            tabl = tabl + "<tbody><tr><td><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS value='' " + count + "  onchange=total() style='width:30px'><input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value=''></td><td class=hide><input type=text id=STFDID value=" + rslt.STFDID + "  class=STFDID name=STFDID></td> <td><input type=text id=TRANDREFNO value=" + rslt.CONTNRNO + "  class=TRANDREFNO readonly='readonly' name=TRANDREFNO style='width:110px'></td><td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class=SIZE name=SIZE style='width:40px' readonly='readonly'></td><td ><input type=text id=TRANIDATE value='" + rslt.GIDATE + "' class=TRANIDATE name=TRANIDATE readonly='readonly'></td><td ><input type=text id=TRANSDATE value='" + rslt.SBDDATE + "' class=TRANSDATE name=TRANSDATE readonly='readonly'></td><td><input type=text id=TRANEDATE value='" + rslt.GODATE + "' class=TRANEDATE name=TRANEDATE style='width:135px' onchange='calculation()'></td><td><input type=text id=TRANDSAMT value='0' class=TRANDSAMT name=TRANDSAMT style='width:75px' readonly='readonly'></td><td><input type=text id=TRANDHAMT value='0' class=TRANDHAMT name=TRANDHAMT readonly=readonly style='width:75px'></td><td><input type=text value='0' id=TRANDEAMT class=TRANDEAMT name=TRANDEAMT style=width:45px readonly=readonly ></td><td><input type=text value='0' id=TRANDFAMT class=TRANDFAMT name=TRANDFAMT style=width:45px readonly=readonly ></td><td><input type=text id=TRANDPAMT value='0' class=TRANDPAMT name=TRANDPAMT  style='width:45px' readonly=readonly ></td><td><input type=text value='0' id=TRANDNAMT class=TRANDNAMT name=TRANDNAMT readonly='readonly' style=width:100px> <td class=hide><input type=text id=TRANDNOP class=TRANDNOP name=TRANDNOP value=" + rslt.STFDNOP + "><input type=text id=TRANDQTY value=" + rslt.STFDQTY + "  class=TRANDQTY name=TRANDQTY ></td>  <td class=hide><input type=text id=CONTNRTID value=" + rslt.CONTNRTID + "  class=CONTNRTID name=CONTNRTID ><input type=text id=CONTNRSID value=" + rslt.CONTNRSID + "  class=CONTNRSID name=CONTNRSID ><input type=text id=TRANDREFID value=" + rslt.GIDID + "  class=TRANDREFID name=TRANDREFID ></td><td class=hide><input type=text id=TRANDID value=0  class=TRANDID name=TRANDID ></td><td class='hide'><input type=text id=days value=0  class=days name=days ></td><td class=hide><input type=text id=RAMT1 value='0'  class=RAMT1 name=RAMT1 style='display:none1' ><input type=text id=RAMT2 value='0'  class=RAMT2 name=RAMT2 style='display:none1' ><input type=text id=RAMT3 value='0'  class=RAMT3 name=RAMT3 style='display:none1' ><input type=text id=RAMT4 value='0'  class=RAMT4 name=RAMT4 style='display:none1' ><input type=text id=RAMT5 value='0'  class=RAMT5 name=RAMT5 style='display:none1' ><input type=text id=RAMT6 value='0'  class=RAMT6 name=RAMT6 style='display:none1' ></td><td class=hide><input type=text id=SLABMIN value='0'  class=SLABMIN name=SLABMIN style='display:none1' ><input type=text id=SLABMAX value='0'  class=SLABMAX name=SLABMAX style='display:none1' ><input type=text id=SLABMIN1 value='0'  class=SLABMIN1 name=SLABMIN1 style='display:none1' ><input type=text id=SLABMAX1 value='0'  class=SLABMAX1 name=SLABMAX1 style='display:none1' ><input type=text id=SLABMIN2 value='0'  class=SLABMIN2 name=SLABMIN2 style='display:none1' ><input type=text id=SLABMAX2 value='0'  class=SLABMAX2 name=SLABMAX2 style='display:none1' > <input type=text id=SLABMIN3 value='0'  class=SLABMIN3 name=SLABMIN3 style='display:none1' ><input type=text id=SLABMAX3 value='0'  class=SLABMAX3 name=SLABMAX3 style='display:none1' >  <input type=text id=SLABMIN4 value='0'  class=SLABMIN4 name=SLABMIN4 style='display:none1' ><input type=text id=SLABMAX4 value='0'  class=SLABMAX4 name=SLABMAX4 style='display:none1' > <input type=text id=SLABMIN5 value='0'  class=SLABMIN5 name=SLABMIN5 style='display:none1' ><input type=text id=SLABMAX5 value='0'  class=SLABMAX5 name=SLABMAX5 style='display:none1' > </td><td class=hide> <input type=text id=RCAMT1 value=0  class=RCAMT1 name=RCAMT1 style='display:none1' ><input type=text id=RCAMT2 value=0  class=RCAMT2 name=RCAMT2 style='display:none1' ><input type=text id=RCAMT3 value=0  class=RCAMT3 name=RCAMT3 style='display:none1'><input type=text id=RCAMT4 value='0'  class=RCAMT4 name=RCAMT4 style='display:none1' ><input type=text id=RCAMT5 value='0'  class=RCAMT5 name=RCAMT5 style='display:none1' ><input type=text id=RCAMT6 value='0'  class=RCAMT6 name=RCAMT6 style='display:none1' ></td><td class=hide><input type=text id=RCOL1 value='0'  class=RCOL1 name=RCOL1 style='display:none1' ><input type=text id=RCOL2 value='0'  class=RCOL2 name=RCOL2 style='display:none1' ><input type=text id=RCOL3 value='0'  class=RCOL3 name=RCOL3 style='display:none1' ><input type=text id=RCOL4 value='0'  class=RCOL4 name=RCOL4 style='display:none1' ><input type=text id=RCOL5 value='0'  class=RCOL5 name=RCOL5 style='display:none1' ><input type=text id=RCOL6 value='0'  class=RCOL6 name=RCOL6 style='display:none1' ></td></tr></tbody>";
        //            count++;
        //        }
        //        tabl = tabl + "</Table>";

        //        return tabl;

        //}

        public string Detail(string ids)
        {
            var param = ids.Split('~');
            var STFMID = 0;
            if (Convert.ToString(param[0]) == "")
            { STFMID = 0; }
            else { STFMID = Convert.ToInt32(param[0]); }

            //var STFMID = Convert.ToInt32(param[0]);
            var TRANMID = Convert.ToInt32(param[2]);
            //var EDATE = Convert.ToDateTime(param[1]);
            var EDATE = param[1].Split('-');
            var edt = EDATE[2] + '-' + EDATE[1] + "-" + EDATE[0];

            var query = context.Database.SqlQuery<pr_Export_Invoice_Stuff_Flx_Assgn_Result>("EXEC pr_Export_Invoice_Stuff_Flx_Assgn @PSTFMID=" + STFMID + ",@PEDATE='" + edt + "',@PTRANMID=" + TRANMID).ToList(); //EDATE.ToString("MM/dd/yyyy")


            var tabl = " <div class='panel-heading navbar-inverse' style=color:white>Seal Bill Details</div>";
            tabl = tabl + "<Table id=TDETAIL class='table table-striped table-bordered bootstrap-datatable'>";
            tabl = tabl + "<thead><tr><th><input type='checkbox' id='CHCK_ALL' name='CHCK_ALL' class='CHCK_ALL' onchange='checkall()' style='width:30px'/></th>";
            tabl = tabl + "<th>Container No</th><th>Size</th><th>In Date</th> <th>Storage Date</th>";
            tabl = tabl + "<th>Charge Date</th><th>Storage</th>";
            tabl = tabl + "<th>Handling</th><th>Energy</th>";
            tabl = tabl + "<th>Fuel</th><th>PTI</th><th>Total</th></tr> </thead>";
            var count = 0;

            foreach (var rslt in query)
            {
                var st = ""; var bt = "";

                if (rslt.TRANDID != 0) { st = "checked"; bt = "true"; }
                else { bt = "false"; st = ""; }


                tabl = tabl + "<tbody><tr><td><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS checked='" + bt + "'   onchange=total() style='width:30px'>";
                tabl = tabl + "<input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value='" + bt + "'></td>";
                tabl = tabl + "<td class=hide><input type=text id=STFDID value=" + rslt.STFDID + "  class=STFDID name=STFDID></td>";
                tabl = tabl + "<td><input type=text id=TRANDREFNO value=" + rslt.CONTNRNO + "  class=TRANDREFNO readonly='readonly' name=TRANDREFNO style='width:110px'></td>";
                tabl = tabl + "<td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class=SIZE name=SIZE style='width:40px' readonly='readonly'></td>";
                tabl = tabl + "<td ><input type=text id=TRANIDATE value='" + rslt.GIDATE + "' class=TRANIDATE name=TRANIDATE readonly='readonly'></td>";
                tabl = tabl + "<td ><input type=text id=TRANSDATE value='" + rslt.SBDDATE + "' class=TRANSDATE name=TRANSDATE readonly='readonly'></td>";
                tabl = tabl + "<td><input type=text id=TRANEDATE value='" + rslt.GODATE + "' class='datepicker TRANEDATE' name=TRANEDATE style='width:135px' onchange='calculation()'></td>";
                tabl = tabl + "<td><input type=text id=TRANDSAMT value='0' class=TRANDSAMT name=TRANDSAMT style='width:75px' readonly='readonly'>";
                tabl = tabl + "<input type=text id=TRAND_COVID_DISC_AMT value='0' class='TRAND_COVID_DISC_AMT hide' name=TRAND_COVID_DISC_AMT style='width:75px' readonly='readonly'></td>";
                tabl = tabl + "<td><input type=text id=TRANDHAMT value='0' class=TRANDHAMT name=TRANDHAMT readonly=readonly style='width:75px'></td>";
                tabl = tabl + "<td><input type=text value='0' id=TRANDEAMT class=TRANDEAMT name=TRANDEAMT style=width:45px readonly=readonly ></td>";
                tabl = tabl + "<td><input type=text value='0' id=TRANDFAMT class=TRANDFAMT name=TRANDFAMT style=width:45px readonly=readonly ></td>";
                tabl = tabl + "<td><input type=text id=TRANDPAMT value='0' class=TRANDPAMT name=TRANDPAMT  style='width:45px' readonly=readonly ></td>";
                tabl = tabl + "<td><input type=text value='0' id=TRANDNAMT class=TRANDNAMT name=TRANDNAMT readonly='readonly' style=width:100px></td>";
                tabl = tabl + "<td class=hide><input type=text id=TRANDNOP class=TRANDNOP name=TRANDNOP value=" + rslt.STFDNOP + ">";
                tabl = tabl + "<input type=text id=TRANDQTY value=" + rslt.STFDQTY + "  class=TRANDQTY name=TRANDQTY ></td>";
                tabl = tabl + "<td class=hide><input type=text id=CONTNRTID value=0  class=CONTNRTID name=CONTNRTID >";
                tabl = tabl + "<input type=text id=CONTNRSID value=" + rslt.CONTNRSID + "  class=CONTNRSID name=CONTNRSID >";
                tabl = tabl + "<input type=text id=TRANDREFID value=" + rslt.GIDID + "  class=TRANDREFID name=TRANDREFID ></td>";
                tabl = tabl + "<td class=hide><input type=text id=TRANDID value=0  class=TRANDID name=TRANDID ></td>";
                tabl = tabl + "<td class='hide'><input type=text id=days value=0  class=days name=days ></td>";
                tabl = tabl + "<td class=hide><input type=text id=RAMT1 value='0'  class=RAMT1 name=RAMT1 style='display:none' >";
                tabl = tabl + "<input type=text id=RAMT2 value='0'  class=RAMT2 name=RAMT2 style='display:none' >";
                tabl = tabl + "<input type=text id=RAMT3 value='0'  class=RAMT3 name=RAMT3 style='display:none' >";
                tabl = tabl + "<input type=text id=RAMT4 value='0'  class=RAMT4 name=RAMT4 style='display:none' >";
                tabl = tabl + "<input type=text id=RAMT5 value='0'  class=RAMT5 name=RAMT5 style='display:none' >";
                tabl = tabl + "<input type=text id=RAMT6 value='0'  class=RAMT6 name=RAMT6 style='display:none' >";
                tabl = tabl + "<input type=text id=RAMT7 value='0'  class=RAMT7 name=RAMT7 style='display:none' ></td>";
                tabl = tabl + "<td class=hide><input type=text id=SLABMIN value='0'  class=SLABMIN name=SLABMIN style='display:none' >";
                tabl = tabl + "<input type=text id=SLABMAX value='0'  class=SLABMAX name=SLABMAX style='display:none' >";
                tabl = tabl + "<input type=text id=SLABMIN1 value='0'  class=SLABMIN1 name=SLABMIN1 style='display:none' >";
                tabl = tabl + "<input type=text id=SLABMAX1 value='0'  class=SLABMAX1 name=SLABMAX1 style='display:none' >";
                tabl = tabl + "<input type=text id=SLABMIN2 value='0'  class=SLABMIN2 name=SLABMIN2 style='display:none' >";
                tabl = tabl + "<input type=text id=SLABMAX2 value='0'  class=SLABMAX2 name=SLABMAX2 style='display:none' >";
                tabl = tabl + "<input type=text id=SLABMIN3 value='0'  class=SLABMIN3 name=SLABMIN3 style='display:none' >";
                tabl = tabl + "<input type=text id=SLABMAX3 value='0'  class=SLABMAX3 name=SLABMAX3 style='display:none' >";
                tabl = tabl + "<input type=text id=SLABMIN4 value='0'  class=SLABMIN4 name=SLABMIN4 style='display:none' >";
                tabl = tabl + "<input type=text id=SLABMAX4 value='0'  class=SLABMAX4 name=SLABMAX4 style='display:none' >";
                tabl = tabl + "<input type=text id=SLABMIN5 value='0'  class=SLABMIN5 name=SLABMIN5 style='display:none' >";
                tabl = tabl + "<input type=text id=SLABMAX5 value='0'  class=SLABMAX5 name=SLABMAX5 style='display:none' >";
                tabl = tabl + "<input type=text id=SLABMIN6 value='0'  class=SLABMIN6 name=SLABMIN6 style='display:none' >";
                tabl = tabl + "<input type=text id=SLABMAX6 value='0'  class=SLABMAX6 name=SLABMAX6 style='display:none' > </td>";
                tabl = tabl + "<td class=hide> <input type=text id=RCAMT1 value=0  class=RCAMT1 name=RCAMT1 style='display:none' >";
                tabl = tabl + "<input type=text id=RCAMT2 value=0  class=RCAMT2 name=RCAMT2 style='display:none' >";
                tabl = tabl + "<input type=text id=RCAMT3 value=0  class=RCAMT3 name=RCAMT3 style='display:none'>";
                tabl = tabl + "<input type=text id=RCAMT4 value='0'  class=RCAMT4 name=RCAMT4 style='display:none' >";
                tabl = tabl + "<input type=text id=RCAMT5 value='0'  class=RCAMT5 name=RCAMT5 style='display:none' >";
                tabl = tabl + "<input type=text id=RCAMT6 value='0'  class=RCAMT6 name=RCAMT6 style='display:none' >";
                tabl = tabl + "<input type=text id=RCAMT7 value='0'  class=RCAMT7 name=RCAMT7 style='display:none' ></td>";
                tabl = tabl + "<td class=hide><input type=text id=RCOL1 value='0'  class=RCOL1 name=RCOL1 style='display:none' >";
                tabl = tabl + "<input type=text id=RCOL2 value='0'  class=RCOL2 name=RCOL2 style='display:none' >";
                tabl = tabl + "<input type=text id=RCOL3 value='0'  class=RCOL3 name=RCOL3 style='display:none' >";
                tabl = tabl + "<input type=text id=RCOL4 value='0'  class=RCOL4 name=RCOL4 style='display:none' >";
                tabl = tabl + "<input type=text id=RCOL5 value='0'  class=RCOL5 name=RCOL5 style='display:none' >";
                tabl = tabl + "<input type=text id=RCOL6 value='0'  class=RCOL6 name=RCOL6 style='display:none' >";
                tabl = tabl + "<input type=text id=RCOL7 value='0'  class=RCOL7 name=RCOL7 style='display:none' >";
                tabl = tabl + "<input type='text' value=" + rslt.STATETYPE + " id='F_STATETYPE' class='F_STATETYPE' name='F_STATETYPE'></td></tr></tbody>";
                count++;
            }
            tabl = tabl + "</Table>";

            return tabl;

        }

        //......end


        public JsonResult GetExportGSTRATE(string id)
        {
            var param = id.Split('~');
            var StateType = 0;
            if (param[0] != "")
            { StateType = Convert.ToInt32(param[0]); }
            else
            { StateType = 0; }

            var SlabTId = Convert.ToInt32(param[1]);
            if (StateType == 0)
            {
                var query = context.Database.SqlQuery<VW_EXPORT_SLABTYPE_HSN_DETAIL_ASSGN>("select HSNCODE,CGSTEXPRN,SGSTEXPRN,IGSTEXPRN from VW_EXPORT_SLABTYPE_HSN_DETAIL_ASSGN where SLABTID=" + SlabTId + " order by HSNCODE").ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var query = context.Database.SqlQuery<VW_EXPORT_SLABTYPE_HSN_DETAIL_ASSGN>("select HSNCODE,ACGSTEXPRN as CGSTEXPRN,ASGSTEXPRN as SGSTEXPRN,AIGSTEXPRN as IGSTEXPRN from VW_EXPORT_SLABTYPE_HSN_DETAIL_ASSGN where SLABTID=" + SlabTId + " order by HSNCODE").ToList();

                return Json(query, JsonRequestBehavior.AllowGet);


            }

        } //...end


        //.............................storage ,handling,energy,fuel, and PTI amount...........//
        public JsonResult Bill_Detail(string id)
        {
            var param = id.Split('-');

            var TARIFFMID = 0;
            if (Convert.ToString(param[0]) == "")
            { TARIFFMID = 0; }
            else { TARIFFMID = Convert.ToInt32(param[0]); }

            //var TARIFFMID = Convert.ToInt32(param[0]);

            var CHRGETYPE = Convert.ToInt32(param[1]);
            var CONTNRSID = Convert.ToInt32(param[2]);
            var CHAID = Convert.ToInt32(param[3]);
            var EOPTID = Convert.ToInt32(param[4]);
            if (TARIFFMID == 4)/* INSTEAD OF SLABTID=6 ,,PARAM[4]*/
            {
                var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (6,7,14,15,16) and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and CHAID=" + CHAID + " and (EOPTID=" + EOPTID + " or (EOPTID=0 and SLABTID <> 6))" ).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var qry = "select SLABTID,SLABAMT from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (6,7,14,15,16) and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and (EOPTID=" + EOPTID + " or (EOPTID=0 and SLABTID <> 6))";
                var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>(qry).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }


        }//.....end

        //............ratecardmaster.....................
        public JsonResult RATECARD(string id)
        {
            var param = id.Split('-');
            var TARIFFMID = 0;
            if (Convert.ToString(param[0]) == "")
            { TARIFFMID = 0; }
            else { TARIFFMID = Convert.ToInt32(param[0]); }

            //var TARIFFMID = Convert.ToInt32(param[0]);
            var CHRGETYPE = Convert.ToInt32(param[1]);
            var CONTNRSID = Convert.ToInt32(param[2]);
            var CHAID = Convert.ToInt32(param[3]);
            var SLABMIN = Convert.ToInt32(param[4]);
            if (TARIFFMID == 4)
            {
                var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID=7 and HTYPE=0 and SDTYPE=1 and SLABMIN <= " + SLABMIN + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and CHAID=" + CHAID + " order by SLABMIN").ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID=7 and HTYPE=0 and SDTYPE=1 and SLABMIN <= " + SLABMIN + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " order by SLABMIN").ToList();

                return Json(query, JsonRequestBehavior.AllowGet);


            }

        } //...end

        //..........TARIFFTMID get function....................
        public JsonResult TARIFFTMID(int id)
        {
            var query = context.Database.SqlQuery<int>("select TARIFFTMID from exporttariffmaster where TARIFFMID=" + id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        } //........end



        //...............seal detail No...//
        public JsonResult GetSealNo(int id)
        {
            var query = context.Database.SqlQuery<pr_Export_Invoice_Seal_No_Assgn_Result>("EXEC pr_Export_Invoice_Seal_No_Assgn @PCHAID=" + id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public JsonResult datediff(string id)
        {
            var param = id.Split('-');

            var SDATE = Convert.ToInt32(param[0]);

            var EDATE = Convert.ToInt32(param[1]);
            var Start = Convert.ToDateTime(SDATE);
            var End = Convert.ToDateTime(EDATE);
            Response.Write(SDATE + "------" + EDATE);
            Response.End();
            var DateDiff = End.Subtract(Start).Days;
            return Json(DateDiff, JsonRequestBehavior.AllowGet);
        }

        //...........cost factor with default value
        public JsonResult DefaultCF()
        {
            DbSqlQuery<CostFactorMaster> data = context.costfactormasters.SqlQuery("select * from costfactormaster  where CFID  in(2) order by CFID DESC");
            DbSqlQuery<CostFactorMaster> data2 = context.costfactormasters.SqlQuery("select * from costfactormaster  where DISPSTATUS=0 and CFID  in(96,97) order by CFID");
            return Json(data.Concat(data2), JsonRequestBehavior.AllowGet);

        }//....end

        //..........................Printview...
        [Authorize(Roles = "ExportSealBillPrint")]
        public void PrintView(string id)
        {

            var param = id.Split(';');

            var ids = Convert.ToInt32(param[0]);
            var gsttype = Convert.ToInt32(param[1]);
            var billedto = Convert.ToInt32(param[2]);

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "SEALINVOICE", Convert.ToInt32(ids), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;



                // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");


                //........Get TRANPCOUNT...//
                var Query = context.Database.SqlQuery<int>("select TRANPCOUNT from transactionmaster where TRANMID=" + ids).ToList();
                var PCNT = 0;

                if (Query.Count() != 0) { PCNT = Query[0]; }
                var TRANPCOUNT = ++PCNT;
                // Response.Write(++PCNT);
                // Response.End();

                context.Database.ExecuteSqlCommand("UPDATE transactionmaster SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + ids);
                gsttype = 1;
                switch (billedto)
                {
                    case 1:
                        if (gsttype == 0)
                        { cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Export_Seal_Invoice_Exp.RPT"); }
                        else
                        { cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "GST_Export_Seal_Invoice_Exp.RPT"); }

                        break;

                    default:
                        if (gsttype == 0)
                        { cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Export_Seal_Invoice.RPT"); }
                        else
                        { cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "GST_Export_Seal_Invoice.RPT"); }

                        break;
                }




                cryRpt.RecordSelectionFormula = "{VW_EXPORT_STUFF_INVOICE_PRINT.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_STUFF_INVOICE_PRINT.TRANMID} = " + ids;



                crConnectionInfo.ServerName = stringbuilder.DataSource;
                crConnectionInfo.DatabaseName = stringbuilder.InitialCatalog;
                crConnectionInfo.UserID = stringbuilder.UserID;
                crConnectionInfo.Password = stringbuilder.Password;

                CrTables = cryRpt.Database.Tables;
                foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
                {
                    crtableLogoninfo = CrTable.LogOnInfo;
                    crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                    CrTable.ApplyLogOnInfo(crtableLogoninfo);
                }


                cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                cryRpt.Dispose();
                cryRpt.Close();
            }

        }
        //end
        public ActionResult AForm(string id)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            var param = id.Split('~');

            var ids = Convert.ToInt32(param[0]);
            var gstamt = Convert.ToInt32(param[1]);

            ViewBag.id = ids;
            ViewBag.FGSTAMT = gstamt;

            var query = context.Database.SqlQuery<pr_Invoice_Print_Customer_Detail_Assgn_Result>("EXEC pr_Invoice_Print_Customer_Detail_Assgn @PTranMID = " + ids).ToList();
            if (query[0].OSBILLREFNAME != null)
            {
                ViewBag.TRANCSNAME = query[0].OSBILLREFNAME;
                ViewBag.TRANBILLREFID = query[0].TRANREFID;
                ViewBag.TRANIMPADDR1 = query[0].CATEADDR1;
                ViewBag.TRANIMPADDR2 = query[0].CATEADDR2;
                ViewBag.TRANIMPADDR3 = query[0].CATEADDR3;
                ViewBag.TRANIMPADDR4 = query[0].CATEADDR4;
                ViewBag.CATEMAIL = query[0].CATEMAIL;
                ViewBag.CATEPHN1 = query[0].CATEPHN1;
                ViewBag.GSTNO = query[0].CATEGSTNO;
            }
            return View();
        }


        [HttpPost]
        public JsonResult UpdateEMailMobile(FormCollection F_Form)
        {
            string CATEID = F_Form["TRANBILLREFID"].ToString();
            string CATEMAIL = F_Form["CATEMAIL"].ToString();
            string CATEPHN1 = F_Form["CATEPHN1"].ToString();
            try
            {
                var Query = context.Database.SqlQuery<CategoryMaster>("select * from CATEGORYMASTER where CateID=" + Convert.ToInt32(CATEID)).ToList();
                if (Query.Count() > 0)
                {
                    context.Database.ExecuteSqlCommand("Update CATEGORYMASTER set CATEMAIL  = '" + CATEMAIL + "', CATEPHN1 = '" + CATEPHN1 + "' where CateID = " + Convert.ToInt32(CATEID));
                    //Response.Write("Saved");
                    var sts = "Saved";
                    return Json(sts, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var sts = "Not Saved";
                    return Json(sts, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult MForm(int id)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ViewBag.id = id;
            var query = context.Database.SqlQuery<TransactionMaster>("select * from TransactionMaster where TRANMID=" + id).ToList();

            var chaid = Convert.ToInt32(query[0].TRANREFID);
            var sql = context.Database.SqlQuery<CategoryMaster>("select * from CategoryMaster where CATEID=" + chaid).ToList();
            ViewBag.CATEMAIL = sql[0].CATEMAIL;

            var TmpAStr = "<table width='539' border='0' cellpadding='0' cellspacing='0' style='background:#FFCC66;border:1px solid #663300'><tr><Td colspan='2'  style='background:#663300;color:#FFFFFF;font-weight:700;text-align:center;padding:5px'>Import</Td> </tr> <tr>";
            TmpAStr = TmpAStr + " <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left'width='208' >Invoice No. </th> <td style='padding:4px;border-bottom:1px solid #663300'  width='331'>" + query[0].TRANDNO + "</td> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Date </th>";
            TmpAStr = TmpAStr + " <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANDATE + "</td> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Bill Amount </th> <td style='padding:4px;border-bottom:1px solid #663300' >[[PInvAmt]]</td> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >CHA Name </th>";
            TmpAStr = TmpAStr + " <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANREFNAME + "</td> </tr> </table> <br> <br> <table width='539' border='0' cellpadding='0' cellspacing='0' style='background:#FFCC66;border:1px solid #663300 '> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left'width='208' >SUDHARSHAN LOGISTICS PRIVATE LIMITED</th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' ># 592, ENNORE EXPRESS HIGH ROAD,</th>";
            TmpAStr = TmpAStr + " </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >CHENNAI - 600 057. </th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Phone No. : 6545 5252 / 2573 3447 / 2573 3762</th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >E-Mail Id : chennaicfs@sancotrans.com</th> </tr> </table>";

            ViewBag.SUB = "Export Invoice No." + query[0].TRANDNO;
            //ViewBag.MSG = TmpAStr;
            //ViewBag.TRANIMPADDR2 = sql[0].CATEADDR2;
            //ViewBag.TRANIMPADDR3 = sql[0].CATEADDR3;
            //ViewBag.TRANIMPADDR4 = sql[0].CATEADDR4;

            return View();
        }
        [HttpPost]
        public void Contact(FormCollection mysbfrm)
        {
            try
            {
                var query = context.Database.SqlQuery<TransactionMaster>("select * from TransactionMaster where TRANMID=" + Convert.ToInt32(mysbfrm["FTRANMID"])).ToList();
                var TmpAStr = "<table width='539' border='0' cellpadding='0' cellspacing='0' style='background:#FFCC66;border:1px solid #663300'><tr><Td colspan='2'  style='background:#663300;color:#FFFFFF;font-weight:700;text-align:center;padding:5px'>Export</Td> </tr> <tr>";
                TmpAStr = TmpAStr + " <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left'width='208' >Invoice No. </th> <td style='padding:4px;border-bottom:1px solid #663300'  width='331'>" + query[0].TRANDNO + "</td> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Date </th>";
                TmpAStr = TmpAStr + " <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANDATE + "</td> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Bill Amount </th> <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANNAMT + "</td> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >CHA Name </th>";
                TmpAStr = TmpAStr + " <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANCSNAME  /*query[0].TRANREFNAME*/  + "</td> </tr> </table> <br> <br> <table width='539' border='0' cellpadding='0' cellspacing='0' style='background:#FFCC66;border:1px solid #663300 '> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left'width='208' >SUDHARSHAN LOGISTICS PRIVATE LIMITED</th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >New No. 41, Redhills High Road, Andarkuppam, New Manali,</th>";
                TmpAStr = TmpAStr + " </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >CHENNAI - 600 103. </th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Phone No. : 7144 9000 </th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >E-Mail Id : billcfs@sudharsan.co</th> </tr> </table>";

                var body = TmpAStr;
                var message = new MailMessage();
                message.To.Add(new MailAddress(mysbfrm["CATEMAIL"].ToString()));  // replace with valid value 
                if (mysbfrm["CATEMAILCC"].ToString() != "")
                    message.CC.Add(new MailAddress(address: mysbfrm["CATEMAILCC"].ToString(), displayName: Session["COMPNAME"].ToString()));
                //  message.CC.Add(new MailAddress(address: "dinesh@fusiontec.com", displayName: Session["COMPNAME"].ToString()));
                //  if (mysbfrm["CATEMAILBCC"].ToString() != "")
                message.Bcc.Add(new MailAddress(address: "ssathya@sancotrans.com", displayName: Session["COMPNAME"].ToString()));
                // message.Bcc.Add(new MailAddress(address: "edp@sancotrans.com", displayName: Session["COMPNAME"].ToString()));
                message.Bcc.Add(new MailAddress(address: "ramaswamy@sancotrans.com", displayName: Session["COMPNAME"].ToString()));
                message.Bcc.Add(new MailAddress(address: "billcfs@sudharsan.co", displayName: Session["COMPNAME"].ToString()));
                message.Bcc.Add(new MailAddress(address: "chennaicfs@sudharsan.co", displayName: Session["COMPNAME"].ToString()));
                message.ReplyToList.Add(new MailAddress("billcfs@sudharsan.co"));

                message.From = new MailAddress(address: "billcfs@sudharsan.co", displayName: Session["COMPNAME"].ToString());  // replace with valid value
                message.Subject = mysbfrm["TMPSUB"].ToString();

                message.Body = TmpAStr;
                message.IsBodyHtml = true;
                message.Attachments.Add(new Attachment("F:\\CFS\\" + Session["CUSRID"] + "\\ExportInv\\" + query[0].TRANNO + ".pdf"));
                using (var smtp = new SmtpClient())
                {
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential("billcfs@sudharsan.co", "Cfs2billing@24");

                    //  smtp.Host = "smtp.gmail.com";
                    smtp.Host = "mail.sudharsan.co";
                    smtp.Port = 25;
                    // smtp.Port = 587;
                    smtp.EnableSsl = false;
                    smtp.Send(message);

                }

                Response.Write("Success");
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
        //..........................Printview...
        [Authorize(Roles = "ExportSealBillPrint")]
        public void APrintView(int id)
        {

            var ids = id;
            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "SEALINVOICE", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;



                // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");


                //........Get TRANPCOUNT...//
                var Query = context.Database.SqlQuery<TransactionMaster>("select * from transactionmaster where TRANMID=" + id).ToList();
                var PCNT = 0;

                if (Query.Count() != 0) { PCNT = Convert.ToInt32(Query[0].TRANPCOUNT); }
                var TRANPCOUNT = ++PCNT;
                // Response.Write(++PCNT);
                // Response.End();

                context.Database.ExecuteSqlCommand("UPDATE transactionmaster SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + id);


                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Export_Seal_Invoice_E01.RPT");

                cryRpt.RecordSelectionFormula = "{VW_EXPORT_STUFF_INVOICE_PRINT.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_STUFF_INVOICE_PRINT.TRANMID} = " + id;



                crConnectionInfo.ServerName = stringbuilder.DataSource;
                crConnectionInfo.DatabaseName = stringbuilder.InitialCatalog;
                crConnectionInfo.UserID = stringbuilder.UserID;
                crConnectionInfo.Password = stringbuilder.Password;

                CrTables = cryRpt.Database.Tables;
                foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
                {
                    crtableLogoninfo = CrTable.LogOnInfo;
                    crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                    CrTable.ApplyLogOnInfo(crtableLogoninfo);
                }

                //string path = Session["EXCLPATH"] + "ExportInv";
                ////  string path = Server.MapPath(fromDate);
                //if (!(Directory.Exists(path)))
                //{
                //    Directory.CreateDirectory(path);
                //}
                //cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, path + "\\" + Query[0].TRANNO + ".pdf");
                cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                cryRpt.Dispose();
                cryRpt.Close();
            }

        }
        //end
        //...............Delete Row.............
        [Authorize(Roles = "ExportSealBillDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);

            if (temp.Equals("PROCEED"))
            {
                TransactionMaster transactionmaster = context.transactionmaster.Find(Convert.ToInt32(id));
                context.transactionmaster.Remove(transactionmaster);
                context.SaveChanges();
                Response.Write("Deleted successfully...");
            }
            else
                Response.Write(temp);

        }//-----End of Delete Row
    }//----------End of Class
}//-------------End of namespace