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
    public class StuffingBillController : Controller
    {
        //
        // GET: /StuffingBill/
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

        //[Authorize(Roles = "ExportStuffingBillIndex")]
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
            else { 
             Session["TRANBTYPE"] = "1";
             Session["REGSTRID"] = "16";
            }

            if (Session["Group"].ToString() == "Exports")
            {
                ViewBag.aaa = "hide";
            }
            else
            {
                ViewBag.aaa = "hide1";
            }

            //...........Bill type......//
            List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
            if (Convert.ToInt32(Session["TRANBTYPE"]) == 1)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "STUFF", Value = "1", Selected = true };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "GRT", Value = "2", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);

            }
            else
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "STUFF", Value = "1", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "GRT", Value = "2", Selected = true };
                selectedBILLYPE.Add(selectedItemGPTY);

            }
            ViewBag.TRANBTYPE = selectedBILLYPE;
            //....end

            //............Billed to....//
            ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == 16 || x.REGSTRID == 17 || x.REGSTRID == 18), "REGSTRID", "REGSTRDESC", Convert.ToInt32(Session["REGSTRID"]));
            //.....end


            DateTime sd = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;

            DateTime ed = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;
            return View();
           // return View(context.transactionmaster.Where(x => x.TRANDATE >= sd).Where(x => x.TRANDATE <= ed).ToList());
        }//...End of index grid

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Export_Stuff_Billing(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToInt32(Session["TRANBTYPE"]), Convert.ToInt32(Session["REGSTRID"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANTIME.Value.ToString("hh:mm tt"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANNAMT.ToString(), d.ACKNO, d.DISPSTATUS, d.GSTAMT.ToString(), d.TRANMID.ToString() }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //[Authorize(Roles = "ExportStuffingBillEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/StuffingBill/GSTForm/" + id);

            //Response.Redirect("/StuffingBill/GSTForm/" + id);
        }
        //......................Form data....................//
        //......................Form data....................//

        //[Authorize(Roles = "ExportStuffingBillCreate")]
        public ActionResult Form(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            TransactionMaster tab = new TransactionMaster();
            TransactionMD vm = new TransactionMD();

            //..........................................Dropdown data.........................//
            ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME");
            ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", "TARIFFTMID");
            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.SBMDATE = DateTime.Now;
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.TRANLMID = new SelectList("");
            ViewBag.TARIFFGID = new SelectList(context.ExportTariffGroupMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");
            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "STUFF", Value = "1", Selected = true };
            selectedBILLTYPE.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "GRT", Value = "2", Selected = false };
            selectedBILLTYPE.Add(selectedItemDSP);
            ViewBag.TRANBTYPE = selectedBILLTYPE;
            //....end

            //............Billed to....//
            //List<SelectListItem> selectedBILLEDTO = new List<SelectListItem>();
            //SelectListItem selectedItemBILLEDTO = new SelectListItem { Text = "CUSTOMER", Value = "16", Selected = true };
            //selectedBILLEDTO.Add(selectedItemBILLEDTO);
            //selectedItemBILLEDTO = new SelectListItem { Text = "NOTIONAL", Value = "17", Selected = false };
            //selectedBILLEDTO.Add(selectedItemBILLEDTO);
            //selectedItemBILLEDTO = new SelectListItem { Text = "ZERO BILL", Value = "18", Selected = false };
            //selectedBILLEDTO.Add(selectedItemBILLEDTO);
            //ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID != 1).Where(x => x.REGSTRID != 2).Where(x => x.REGSTRID != 6).Where(x => x.REGSTRID != 46).Where(x => x.REGSTRID != 47).Where(x => x.REGSTRID != 48), "REGSTRID", "REGSTRDESC");
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

                var result1 = context.Database.SqlQuery<Nullable<int>>("select statetype from statemaster where stateid =" + tab.STATEID).ToList();
                if (result1.Count == 0) ViewBag.STATETYPE = 0;
                else ViewBag.STATETYPE = Convert.ToInt32(result1[0]);

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

                //....................Bill type.................//
                List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
                if (Convert.ToInt32(tab.TRANBTYPE) == 1)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "STUFF", Value = "1", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);
                    // selectedItemGPTY = new SelectListItem { Text = "GRT", Value = "2", Selected = false };
                    //  selectedBILLYPE.Add(selectedItemGPTY);

                }
                else
                {

                    //selectedItemGPTY = new SelectListItem { Text = "STUFF", Value = "1", Selected = false };
                    // selectedBILLYPE.Add(selectedItemGPTY);
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "GRT", Value = "2", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);

                }
                ViewBag.TRANBTYPE = selectedBILLYPE;
                //..........end

                vm.masterdata = context.transactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.detaildata = context.transactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.costfactor = context.transactionmasterfactor.Where(det => det.TRANMID == id).ToList();
                vm.viewdata = context.Database.SqlQuery<pr_Export_Invoice_Stuff_Flx_Assgn_Result>("pr_Export_Invoice_Stuff_Flx_Assgn @PSTFMID=" + tab.TRANLMID + ",@PEDATE='" + tab.TRANDATE.ToString("MM/dd/yyyy") + "',@PTRANMID=" + id + "").ToList();//........procedure  for edit mode details data

                var tariffmid = Convert.ToInt32(vm.detaildata[0].TARIFFMID);

                ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", vm.detaildata[0].TARIFFMID);
                //  var sql=context.exporttariffmaster.Where(x => x.TARIFFMID == tariffmid).Select(x => x.TGID).ToList();
                var qry = context.Database.SqlQuery<int>("select TGID frOm EXPORTTARIFFMASTER WHERE TARIFFMID=" + tariffmid).ToList();

                ViewBag.TARIFFGID = new SelectList(context.ExportTariffGroupMasters.Where(x => x.DISPSTATUS == 0), "TGID", "TGDESC", qry[0]);

            }

            return View(vm);
        }//........End of form

        /*PRINT DETAIL*/


        //[Authorize(Roles = "ExportStuffingBillCreate")]
        public ActionResult GSTForm(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            TransactionMaster tab = new TransactionMaster();
            TransactionMD vm = new TransactionMD();

            //..........................................Dropdown data.........................//
            ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME");
            ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", "TARIFFTMID");
            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.SBMDATE = DateTime.Now;
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.TRANLMID = new SelectList("");
            ViewBag.TARIFFGID = new SelectList(context.ExportTariffGroupMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");
            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "STUFF", Value = "1", Selected = true };
            selectedBILLTYPE.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "GRT", Value = "2", Selected = false };
            selectedBILLTYPE.Add(selectedItemDSP);
            ViewBag.TRANBTYPE = selectedBILLTYPE;
            //....end

            //............Billed to....//
            //List<SelectListItem> selectedBILLEDTO = new List<SelectListItem>();
            //SelectListItem selectedItemBILLEDTO = new SelectListItem { Text = "CUSTOMER", Value = "16", Selected = true };
            //selectedBILLEDTO.Add(selectedItemBILLEDTO);
            //selectedItemBILLEDTO = new SelectListItem { Text = "NOTIONAL", Value = "17", Selected = false };
            //selectedBILLEDTO.Add(selectedItemBILLEDTO);
            //selectedItemBILLEDTO = new SelectListItem { Text = "ZERO BILL", Value = "18", Selected = false };
            //selectedBILLEDTO.Add(selectedItemBILLEDTO);
            //ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID != 1).Where(x => x.REGSTRID != 2).Where(x => x.REGSTRID != 6).Where(x => x.REGSTRID != 46).Where(x => x.REGSTRID != 47).Where(x => x.REGSTRID != 48), "REGSTRID", "REGSTRDESC");
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

                var result = context.Database.SqlQuery<Nullable<int>>("select EOPTID from StuffingMaster where STFMID=" + tab.TRANLMID).ToList();
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

                //....................Bill type.................//
                List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
                if (Convert.ToInt32(tab.TRANBTYPE) == 1)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "STUFF", Value = "1", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);
                    // selectedItemGPTY = new SelectListItem { Text = "GRT", Value = "2", Selected = false };
                    //  selectedBILLYPE.Add(selectedItemGPTY);

                }
                else
                {

                    //selectedItemGPTY = new SelectListItem { Text = "STUFF", Value = "1", Selected = false };
                    // selectedBILLYPE.Add(selectedItemGPTY);
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "GRT", Value = "2", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);

                }
                ViewBag.TRANBTYPE = selectedBILLYPE;
                //..........end

                vm.masterdata = context.transactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.detaildata = context.transactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.costfactor = context.transactionmasterfactor.Where(det => det.TRANMID == id).ToList();
                vm.viewdata = context.Database.SqlQuery<pr_Export_Invoice_Stuff_Flx_Assgn_Result>("pr_Export_Invoice_Stuff_Flx_Assgn @PSTFMID=" + tab.TRANLMID + ",@PEDATE='" + tab.TRANDATE.ToString("MM/dd/yyyy") + "',@PTRANMID=" + id + "").ToList();//........procedure  for edit mode details data

                var tariffmid = Convert.ToInt32(vm.detaildata[0].TARIFFMID);

                ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", vm.detaildata[0].TARIFFMID);
                //  var sql=context.exporttariffmaster.Where(x => x.TARIFFMID == tariffmid).Select(x => x.TGID).ToList();
                var qry = context.Database.SqlQuery<int>("select TGID frOm EXPORTTARIFFMASTER WHERE TARIFFMID=" + tariffmid).ToList();

                ViewBag.TARIFFGID = new SelectList(context.ExportTariffGroupMasters.Where(x => x.DISPSTATUS == 0), "TGID", "TGDESC", qry[0]);

            }

            return View(vm);
        }//........End of form
        /*PRINT DETAIL*/
        public ActionResult CForm(string id)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            var param = id.Split('~');

            var ids = Convert.ToInt32(param[0]);
            var gstamt = Convert.ToInt32(param[1]);

            ViewBag.id = ids;
            ViewBag.FGSTAMT = gstamt;

            //var query = context.Database.SqlQuery<TransactionMaster>("select * from TransactionMaster where TRANMID=" + ids).ToList();
            var query = context.Database.SqlQuery<VW_EXPORT_STUFFING_BILL_PRINT_ADDRESS_DETAIL_ASSGN>("select * from VW_EXPORT_STUFFING_BILL_PRINT_ADDRESS_DETAIL_ASSGN where TRANMID=" + ids).ToList();
            if (query[0].TRANCSNAME != null)
            {
                ViewBag.TRANCSNAME = query[0].TRANCSNAME;
                ViewBag.TRANIMPADDR1 = query[0].TRANIMPADDR1;
                ViewBag.TRANIMPADDR2 = query[0].TRANIMPADDR2;
                ViewBag.TRANIMPADDR3 = query[0].TRANIMPADDR3;
                ViewBag.TRANIMPADDR4 = query[0].TRANIMPADDR4;
                ViewBag.GSTNO = query[0].TRANGSTNO;
            }
            else
            {
                var chaid = Convert.ToInt32(query[0].TRANREFID);
                var sql0 = context.Database.SqlQuery<CategoryMaster>("select * from CATEGORYMASTER where CATEID=" + chaid).ToList(); 
                var chaaid = Convert.ToInt32(query[0].CATEAID);
                var sql = context.Database.SqlQuery<Category_Address_Details>("select * from CATEGORY_ADDRESS_DETAIL where CATEID=" + chaid + " AND CATEAID="+ chaaid).ToList();
                ViewBag.TRANCSNAME = sql0[0].CATENAME;
                ViewBag.TRANIMPADDR1 = sql[0].CATEAADDR1;
                ViewBag.TRANIMPADDR2 = sql[0].CATEAADDR2;
                ViewBag.TRANIMPADDR3 = sql[0].CATEAADDR3;
                ViewBag.TRANIMPADDR4 = sql[0].CATEAADDR4;
                ViewBag.GSTNO = sql[0].CATEAGSTNO;
            }
            return View();
        }
        //.................Insert/update values into database.............//

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
        public string Detail(string ids)
        {
            var param = ids.Split('~');

            var STFMID = 0;

            if (Convert.ToString(param[0]) == "")
            { STFMID = 0; }
            else { STFMID = Convert.ToInt32(param[0]); }


            //var STFMID = Convert.ToInt32(param[0]); 
            var EDATE = param[1].Split('-');
            
            if (EDATE[1].Length == 1)
                EDATE[1] = "0" + EDATE[1];

            var edt = EDATE[2] + '-' + EDATE[1] + "-"+ EDATE[0];
            var TRANMID = Convert.ToInt32(param[2]);
            var qry = "EXEC pr_Export_Invoice_Stuff_Flx_Assgn @PSTFMID=" + STFMID + ",@PEDATE='" + edt + "',@PTRANMID=" + TRANMID;
            var query = context.Database.SqlQuery<pr_Export_Invoice_Stuff_Flx_Assgn_Result>(qry).ToList();


            var tabl = " <div class='panel-heading navbar-inverse' style=color:white>Stuffing Bill Details</div><Table id=TDETAIL class='table table-striped table-bordered bootstrap-datatable'> <thead><tr><th><input type='checkbox' id='CHCK_ALL' name='CHCK_ALL' class='CHCK_ALL' onchange='checkall()' style='width:30px'/></th><th>Container No</th><th>Size</th><th>In Date</th> <th>Storage Date</th><th>Charge Date</th><th>Storage</th><th>Handling</th><th>Energy</th><th>Fuel</th><th>PTI</th><th>Total</th></tr> </thead>";
            var count = 0;

            foreach (var rslt in query)
            {
                var st = ""; var bt = "";

                if (rslt.TRANDID != 0) { st = "checked"; bt = "true"; }
                else { bt = "false"; st = ""; }
                              

                tabl = tabl + "<tbody><tr>";
                tabl = tabl + "<td><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS checked='" + bt + "'   onchange=total() style='width:30px'>";
                tabl = tabl + "<input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value='" + bt + "'></td>";
                tabl = tabl + "<td class=hide><input type=text id=STFDID value=" + rslt.STFDID + "  class=STFDID name=STFDID></td> ";
                tabl = tabl + "<td><input type=text id=TRANDREFNO value=" + rslt.CONTNRNO + "  class=TRANDREFNO readonly='readonly' name=TRANDREFNO style='width:110px'></td>";
                tabl = tabl + "<td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class=SIZE name=SIZE style='width:40px' readonly='readonly'></td>";
                tabl = tabl + "<td ><input type=text id=TRANIDATE value='" + rslt.GIDATE + "' class=TRANIDATE name=TRANIDATE readonly='readonly'></td>";
                tabl = tabl + "<td ><input type=text id=TRANSDATE value='" + rslt.SBDDATE + "' class=TRANSDATE name=TRANSDATE readonly='readonly'></td>";
                tabl = tabl + "<td><input type=text id='TRANEDATE' value='" + rslt.GODATE + "' class='TRANEDATE datepicker' name='TRANEDATE' style='width:135px' onchange='calculation()'></td>";
                tabl = tabl + "<td><input type=text id=TRANDSAMT value='0' class=TRANDSAMT name=TRANDSAMT style='width:75px' readonly='readonly'>";
                tabl = tabl + "<input type=text id=TRAND_COVID_DISC_AMT value='0' class=TRAND_COVID_DISC_AMT name=TRAND_COVID_DISC_AMT style='width:75px' readonly='readonly'></td>";
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
                tabl = tabl + "<input type=text id=RAMT7 value='0'  class=RAMT7 name=RAMT7 style='display:none1' ></td>";
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
                tabl = tabl + "<input type=text id=SLABMAX6 value='0'  class=SLABMAX6 name=SLABMAX6 style='display:none' ></td>";
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
        [HttpPost]
        public void Contact(FormCollection mysbfrm)
        {
            try
            {
                var query = context.Database.SqlQuery<TransactionMaster>("select * from TransactionMaster where TRANMID=" + Convert.ToInt32(mysbfrm["FTRANMID"])).ToList();
                var TmpAStr = "<table width='539' border='0' cellpadding='0' cellspacing='0' style='background:#FFCC66;border:1px solid #663300'><tr><Td colspan='2'  style='background:#663300;color:#FFFFFF;font-weight:700;text-align:center;padding:5px'>Export</Td> </tr> <tr>";
                TmpAStr = TmpAStr + " <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left'width='208' >Invoice No. </th> <td style='padding:4px;border-bottom:1px solid #663300'  width='331'>" + query[0].TRANDNO + "</td> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Date </th>";
                TmpAStr = TmpAStr + " <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANDATE + "</td> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Bill Amount </th> <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANNAMT + "</td> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >CHA Name </th>";
                TmpAStr = TmpAStr + " <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANCSNAME  /*query[0].TRANREFNAME*/ + "</td> </tr> </table> <br> <br> <table width='539' border='0' cellpadding='0' cellspacing='0' style='background:#FFCC66;border:1px solid #663300 '> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left'width='208' >SUDHARSHAN LOGISTICS PRIVATE LIMITED</th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >New No. 41, Redhills High Road, Andarkuppam, New Manali,</th>";
                TmpAStr = TmpAStr + " </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >CHENNAI - 600 103. </th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Phone No. : 7144 9000 </th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >E-Mail Id : billcfs@sudharsan.co</th> </tr> </table>";

                string fpath = "F:\\CFS\\" + Session["CUSRID"] + "\\ExportInv\\" + query[0].TRANNO + ".pdf";
                if (System.IO.File.Exists(fpath))
                {
                    var body = TmpAStr;
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(mysbfrm["CATEMAIL"].ToString()));  // replace with valid value 
                    if (mysbfrm["CATEMAILCC"].ToString() != "")
                        message.CC.Add(new MailAddress(address: mysbfrm["CATEMAILCC"].ToString(), displayName: Session["COMPNAME"].ToString()));
                    //  message.CC.Add(new MailAddress(address: "dinesh@fusiontec.com", displayName: Session["COMPNAME"].ToString()));
                    // if (mysbfrm["CATEMAILBCC"].ToString() != "")

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
                    // message.Attachments.Add(new Attachment("E:\\CFS\\" + Session["CUSRID"] + "\\ExportInv\\" + query[0].TRANNO + ".pdf"));
                    message.Attachments.Add(new Attachment(fpath));
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
                else
                {
                    Response.Write("File does not exist.");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
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
            /* INSTEAD OF SLABTID=5 ,,PARAM[5]*/
            if (TARIFFMID == 4)
            {
                //var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (" + Convert.ToInt32(param[4]) + ",14,15,16) and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + "and CHAID=" + CHAID).ToList();
                var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (2,5,14,15,16) and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and CHAID=" + CHAID + " and (EOPTID=" + EOPTID + " or (EOPTID=0 and SLABTID <> 5))").ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (2,5,14,15,16) and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and (EOPTID=" + EOPTID + " or (EOPTID=0 and SLABTID <> 5))").ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }


        }//.....end

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
                var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID=2 and HTYPE=0 and SDTYPE=1 and SLABMIN <= " + SLABMIN + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and CHAID=" + CHAID + " order by SLABMIN").ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID=2 and HTYPE=0 and SDTYPE=1 and SLABMIN <= " + SLABMIN + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " order by SLABMIN").ToList();

                return Json(query, JsonRequestBehavior.AllowGet);


            }

        } //...end

        public JsonResult COVIDRATECARD(string id)
        {
            var param = id.Split('~');
            //
            var zsdate = param[0];
            var zedate = param[1];
            var ztariffmid = 0;

            if ((param[2]) != "") { ztariffmid = Convert.ToInt32(param[2]); }

            var zstmrmid = Convert.ToInt32(param[3]);
            var zchrgtype = Convert.ToInt32(param[4]);
            var zcontnrsid = Convert.ToInt32(param[5]);
            var zotype = Convert.ToInt32(param[7]);
            var zchrgdate = param[6];
            var xcovidsdate = Convert.ToDateTime(Session["COVIDSDATE"]).ToString("dd/MM/yyyy").Split('/');
            var zcovidsdate = xcovidsdate[1] + '-' + xcovidsdate[0] + '-' + xcovidsdate[2];

            var xcovidedate = Convert.ToDateTime(Session["COVIDEDATE"]).ToString("dd/MM/yyyy").Split('/');
            var zcovidedate = xcovidedate[1] + '-' + xcovidedate[0] + '-' + xcovidedate[2];

            using (var e = new CFSExportEntities())
            {
                //var query = context.Database.SqlQuery<z_pr_New_Import_Covid_Slab_Assgn_Result>("z_pr_New_Import_Covid_Slab_Assgn @PKUSRID = '" + Session["CUSRID"] + "', @PSDATE = '" + zsdate + "', @PEDATE = '" + zedate + "', @PTARIFFMID = " + ztariffmid + ", @PSTMRID = " + zstmrmid + ", @PCHRGETYPE = " + zchrgtype + ", @PSLABTID = 2, @PSLABMIN = 0, @PCONTNRSID = " + zcontnrsid + ", @PSLABHTYPE = 0, @PCHRGDATE = '" + zchrgdate + "' @PCDate1 = '" + zcovidsdate + "', @PCDate2 = '" + zcovidedate + "'").ToList();
                var query = context.Database.SqlQuery<z_pr_New_Export_Covid_Slab_Assgn_Result>("z_pr_New_Export_Covid_Slab_Assgn @PKUSRID = '" + Session["CUSRID"].ToString() + "', @PSDATE = '" + zsdate + "', @PEDATE = '" + zedate + "', @PTARIFFMID = " + ztariffmid + ", @PSTMRID = " + zstmrmid + ", @PCHRGETYPE = " + zchrgtype + ", @PSLABTID = 2, @PSLABMIN = 0, @PCONTNRSID = " + zcontnrsid + ", @PSLABHTYPE = " + zotype + ", @PCHRGDATE = '" + zchrgdate + "', @PCDate1 = '" + zcovidsdate + "', @PCDate2 = '" + zcovidedate + "'").ToList();
                //var a = 1;
                return Json(query, JsonRequestBehavior.AllowGet);
            }




        } //...end


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

        public void savedata(FormCollection F_Form)
        {
            using (SCFSERPContext context = new SCFSERPContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
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
                        transactionmaster.LMUSRID = Session["CUSRID"].ToString();
                        if (TRANMID == 0)
                        {
                            transactionmaster.CUSRID = Session["CUSRID"].ToString();
                        }
                        transactionmaster.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
                        transactionmaster.PRCSDATE = DateTime.Now;

                        string todaydt = Convert.ToString(DateTime.Now);
                        string todayd = Convert.ToString(DateTime.Now.Date);

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

                        transactionmaster.TRANSAMT = Convert.ToDecimal(F_Form["TRANSAMT"]);
                        transactionmaster.TRANPAMT = Convert.ToDecimal(F_Form["TRANPAMT"]);
                        transactionmaster.TRANHAMT = Convert.ToDecimal(F_Form["TRANHAMT"]);
                        transactionmaster.TRANEAMT = Convert.ToDecimal(F_Form["TRANEAMT"]);
                        transactionmaster.TRANFAMT = Convert.ToDecimal(F_Form["TRANFAMT"]);
                        if (F_Form["TRANTCAMT"].Length != 0)
                        { transactionmaster.TRANTCAMT = Convert.ToDecimal(F_Form["TRANTCAMT"]); }
                        else { transactionmaster.TRANTCAMT = 0; }

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

                        transactionmaster.TRAN_COVID_DISC_AMT = Convert.ToDecimal(F_Form["masterdata[0].TRAN_COVID_DISC_AMT"]);

                        transactionmaster.TRANNARTN = Convert.ToString(F_Form["masterdata[0].TRANNARTN"]);
                        var tranmode = Convert.ToInt16(F_Form["TRANMODE"]);
                        if (tranmode != 2 && tranmode != 3)
                        {
                            transactionmaster.TRANREFNO = "";
                            transactionmaster.TRANREFBNAME = "";
                            transactionmaster.BANKMID = 0;
                            transactionmaster.TRANREFDATE = Convert.ToDateTime(DateTime.Now).Date;
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
                        //string format = "SUD/EXP/";
                        string format = ""; //"EXP/" + Session["GPrxDesc"] + "/";
                        string btyp = auto_numbr_invoice.GetCateBillType(Convert.ToInt32(transactionmaster.TRANREFID)).ToString();
                        if (btyp == "")
                        {
                            format = "EXP/" + Session["GPrxDesc"] + "/";
                        }
                        else
                            format = "EXP" + Session["GPrxDesc"] + btyp;
                        string billformat = "";
                        string prfx = "";
                        string billprfx = "";
                        int ano = 0;



                        if (TRANMID == 0)
                        {
                            //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.gstexportautonum("transactionmaster", "TRANNO", "16", Session["compyid"].ToString(), "1").ToString());

                            if (regsid == 16 && btype == 1)
                            {

                                //ano = transactionmaster.TRANNO;
                                billformat = "STL/ST/CU/";
                                //prfx = string.Format(format + "{0:D5}", ano);
                               // transactionmaster.TRANDNO = prfx.ToString();
                            }
                            else if (regsid == 16 && btype == 2)
                            {
                                //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.gstexportautonum("transactionmaster", "TRANNO", "16", Session["compyid"].ToString(), "2").ToString());

                                //ano = transactionmaster.TRANNO;
                                billformat = "GRT/CUS/";
                                //prfx = string.Format(format + "{0:D5}", ano);
                               // transactionmaster.TRANDNO = prfx.ToString();
                            }

                            else if (regsid == 17 && btype == 1)
                            {
                                //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.gstexportautonum("transactionmaster", "TRANNO", "17", Session["compyid"].ToString(), "1").ToString());
                                //ano = transactionmaster.TRANNO;
                                billformat = "STL/ST/CH/";
                                //prfx = string.Format(format + "{0:D5}", ano);
                                //transactionmaster.TRANDNO = prfx.ToString();
                            }

                            else if (regsid == 17 && btype == 2)
                            {
                                //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.gstexportautonum("transactionmaster", "TRANNO", "17", Session["compyid"].ToString(), "2").ToString());
                                //ano = transactionmaster.TRANNO;
                                billformat = "GRT/CHA/";
                                //prfx = string.Format(format + "{0:D5}", ano);
                                //transactionmaster.TRANDNO = prfx.ToString();
                            }
                            else if (regsid == 18 && btype == 1)
                            {
                                //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.gstexportautonum("transactionmaster", "TRANNO", "18", Session["compyid"].ToString(), "1").ToString());
                                //ano = transactionmaster.TRANNO;
                                billformat = "ZB/STUFF/";
                                //prfx = string.Format(format + "{0:D5}", ano);
                                //transactionmaster.TRANDNO = prfx.ToString();
                            }
                            else if (regsid == 18 && btype == 2)
                            {
                                //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.gstexportautonum("transactionmaster", "TRANNO", "18", Session["compyid"].ToString(), "2").ToString());
                                //ano = transactionmaster.TRANNO;
                                billformat = "ZB/GRT/";
                                //prfx = string.Format(format + "{0:D5}", ano);
                                //transactionmaster.TRANDNO = prfx.ToString();
                            }
                            //........end of autonumber
                            //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.gstexportautonum("transactionmaster", "TRANNO", "0", Session["compyid"].ToString(), btype.ToString()).ToString());
                            transactionmaster.TRANNO = Convert.ToInt32(auto_numbr_invoice.gstexportautonum("transactionmaster", "TRANNO", F_Form["REGSTRID"].ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString(), Convert.ToInt32(transactionmaster.TRANREFID)).ToString());
                            ano = transactionmaster.TRANNO;
                            prfx = string.Format(format + "{0:D5}", ano);
                            billprfx = string.Format(billformat + "{0:D5}", ano);
                            transactionmaster.TRANDNO = prfx.ToString();
                            transactionmaster.TRANBILLREFNO = billprfx.ToString();
                            context.transactionmaster.Add(transactionmaster);
                            context.SaveChanges();
                            TRANMID = transactiondetail.TRANMID;
                        }
                        else
                        {
                            //transactionmaster.REGSTRID = Convert.ToInt16(F_Form["masterdata[0].REGSTRID"]);
                            //transactionmaster.TRANMODE = Convert.ToInt16(F_Form["TRANMODE"]);
                            context.Entry(transactionmaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                           // TRANMID = transactiondetail.TRANMID;
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

                        // var transamt = 0.00; var traneamt = 0; var tranfamt = 0; var tranpamt = 0; var tranhamt = 0; var tranaamt = 0; var trantcamt = 0;
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
                                transactiondetail.RCOL4 = 0;// Convert.ToDecimal(RCOL4[count]);
                                transactiondetail.RCOL5 = 0;// Convert.ToDecimal(RCOL5[count]);
                                transactiondetail.RCOL6 = 0;// Convert.ToDecimal(RCOL6[count]);
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
                                transactiondetail.RCAMT4 = 0;// Convert.ToDecimal(RCAMT4[count]);
                                transactiondetail.RCAMT5 = 0;// Convert.ToDecimal(RCAMT5[count]);
                                transactiondetail.RCAMT6 = 0;// Convert.ToDecimal(RCAMT6[count]);
                                transactiondetail.SLABTID = 0;
                                transactiondetail.TRANYTYPE = 0;
                                transactiondetail.TRANDWGHT = 0;
                                transactiondetail.TRANDAID = 0;
                                transactiondetail.SBDID = 0;
                                transactiondetail.TRAND_COVID_DISC_AMT = Convert.ToDecimal(TRAND_COVID_DISC_AMT[count]);
                                //transamt = transamt + Convert.ToDecimal(TRANDSAMT[count]);//
                                //traneamt = traneamt + Convert.ToInt32(TRANDEAMT[count]);
                                //tranfamt = tranfamt + Convert.ToInt32(TRANDFAMT[count]);
                                //tranhamt = tranhamt + Convert.ToInt32(TRANDHAMT[count]);
                                //tranpamt = tranpamt + Convert.ToInt32(TRANDPAMT[count]);
                                //trantcamt = trantcamt + Convert.ToInt32(F_Form["TRANTCAMT"]);
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


                                //,..............update master..........//
                              //  context.Database.ExecuteSqlCommand("UPDATE TRANSACTIONMASTER SET TRANSAMT=" + Convert.ToDecimal(transamt) + ",TRANHAMT=" + Convert.ToDecimal(tranhamt) + ",TRANEAMT=" + Convert.ToDecimal(traneamt) + ",TRANFAMT=" + Convert.ToDecimal(tranfamt) + ",TRANPAMT=" + Convert.ToDecimal(tranpamt) + ",TRANTCAMT=" + Convert.ToDecimal(trantcamt) + " WHERE TRANMID="+TRANMID+"");

                                DELIDS = DELIDS + "," + TRANDID.ToString();
                               // traneamt = traneamt + Convert.ToInt32(TRANDSAMT[count]);
                            }
                        }

                        //context.Entry(transactionmaster).State = System.Data.Entity.EntityState.Modified;
                        //context.Entry(transactionmaster).State = System.Data.Entity.EntityState.Modified;
                        //context.Entry(transactionmaster).State = System.Data.Entity.EntityState.Modified;
                        //context.Entry(transactionmaster).State = System.Data.Entity.EntityState.Modified;




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
                        trans.Commit(); Response.Redirect("Index");
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        trans.Rollback();                        
                        //Response.Write("Sorry!!An Error Ocurred...");
                        Response.Redirect("/Error/AccessDenied");
                    }
                }
            }

        }

        
        //--------Autocomplete CHA Name
        public JsonResult AutoCha(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }





        ////.............procedure to display detail table...............

        //public string Detail(int CHAID)
        //{


        //    var query = context.Database.SqlQuery<PROC_EXPORT_INVOICE_SBILL_CONTAINER_NO_GRID_ASSGN_Result>("EXEC PROC_EXPORT_INVOICE_SBILL_CONTAINER_NO_GRID_ASSGN @PCHAID=" + CHAID).ToList();


        //    var tabl = " <div class='panel-heading navbar-inverse' style=color:white>Stuffing Bill Details</div><Table id=TDETAIL class='table table-striped table-bordered bootstrap-datatable'> <thead><tr><th></th><th>Container No</th><th>Size</th><th>In Date</th> <th>Storage Date</th><th>Charge Date</th><th>Storage</th><th>Handling</th><th>Energy</th><th>Fuel</th><th>PTI</th><th>Total</th></tr> </thead>";
        //    var count = 0;

        //    foreach (var rslt in query)
        //    {
        //        tabl = tabl + "<tbody><tr><td><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS value='' " + count + "  onchange=total() style='width:30px'><input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value=''></td><td class=hide><input type=text id=STFDID value=" + rslt.STFDID + "  class=STFDID name=STFDID></td> <td><input type=text id=TRANDREFNO value=" + rslt.CONTNRNO + "  class=TRANDREFNO readonly='readonly' name=TRANDREFNO style='width:110px'></td><td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class=SIZE name=SIZE style='width:40px' readonly='readonly'></td><td ><input type=text id=TRANIDATE value='" + rslt.GIDATE + "' class=TRANIDATE name=TRANIDATE readonly='readonly'></td><td ><input type=text id=TRANSDATE value='" + rslt.SBDDATE + "' class=TRANSDATE name=TRANSDATE readonly='readonly'></td><td><input type=text id=TRANEDATE value='" + rslt.GODATE + "' class=TRANEDATE name=TRANEDATE style='width:135px' onchange='calculation()'></td><td><input type=text id=TRANDSAMT value='0' class=TRANDSAMT name=TRANDSAMT style='width:75px' readonly='readonly'></td><td><input type=text id=TRANDHAMT value='0' class=TRANDHAMT name=TRANDHAMT readonly=readonly style='width:75px'></td><td><input type=text value='0' id=TRANDEAMT class=TRANDEAMT name=TRANDEAMT style=width:45px readonly=readonly ></td><td><input type=text value='0' id=TRANDFAMT class=TRANDFAMT name=TRANDFAMT style=width:45px readonly=readonly ></td><td><input type=text id=TRANDPAMT value='0' class=TRANDPAMT name=TRANDPAMT  style='width:45px' readonly=readonly ></td><td><input type=text value='0' id=TRANDNAMT class=TRANDNAMT name=TRANDNAMT readonly='readonly' style=width:100px> <td class=hide><input type=text id=TRANDNOP class=TRANDNOP name=TRANDNOP value=" + rslt.STFDNOP + "><input type=text id=TRANDQTY value=" + rslt.STFDQTY + "  class=TRANDQTY name=TRANDQTY ></td>  <td class=hide><input type=text id=CONTNRTID value=" + rslt.CONTNRTID + "  class=CONTNRTID name=CONTNRTID ><input type=text id=CONTNRSID value=" + rslt.CONTNRSID + "  class=CONTNRSID name=CONTNRSID ><input type=text id=TRANDREFID value=" + rslt.GIDID + "  class=TRANDREFID name=TRANDREFID ></td><td class=hide><input type=text id=TRANDID value=0  class=TRANDID name=TRANDID ></td><td class='hide'><input type=text id=days value=0  class=days name=days ></td><td class=hide><input type=text id=RAMT1 value='0'  class=RAMT1 name=RAMT1 style='display:none1' ><input type=text id=RAMT2 value='0'  class=RAMT2 name=RAMT2 style='display:none1' ><input type=text id=RAMT3 value='0'  class=RAMT3 name=RAMT3 style='display:none1' ><input type=text id=RAMT4 value='0'  class=RAMT4 name=RAMT4 style='display:none1' ><input type=text id=RAMT5 value='0'  class=RAMT5 name=RAMT5 style='display:none1' ><input type=text id=RAMT6 value='0'  class=RAMT6 name=RAMT6 style='display:none1' ></td><td class=hide><input type=text id=SLABMIN value='0'  class=SLABMIN name=SLABMIN style='display:none1' ><input type=text id=SLABMAX value='0'  class=SLABMAX name=SLABMAX style='display:none1' ><input type=text id=SLABMIN1 value='0'  class=SLABMIN1 name=SLABMIN1 style='display:none1' ><input type=text id=SLABMAX1 value='0'  class=SLABMAX1 name=SLABMAX1 style='display:none1' ><input type=text id=SLABMIN2 value='0'  class=SLABMIN2 name=SLABMIN2 style='display:none1' ><input type=text id=SLABMAX2 value='0'  class=SLABMAX2 name=SLABMAX2 style='display:none1' > <input type=text id=SLABMIN3 value='0'  class=SLABMIN3 name=SLABMIN3 style='display:none1' ><input type=text id=SLABMAX3 value='0'  class=SLABMAX3 name=SLABMAX3 style='display:none1' >  <input type=text id=SLABMIN4 value='0'  class=SLABMIN4 name=SLABMIN4 style='display:none1' ><input type=text id=SLABMAX4 value='0'  class=SLABMAX4 name=SLABMAX4 style='display:none1' > <input type=text id=SLABMIN5 value='0'  class=SLABMIN5 name=SLABMIN5 style='display:none1' ><input type=text id=SLABMAX5 value='0'  class=SLABMAX5 name=SLABMAX5 style='display:none1' > </td><td class=hide> <input type=text id=RCAMT1 value=0  class=RCAMT1 name=RCAMT1 style='display:none1' ><input type=text id=RCAMT2 value=0  class=RCAMT2 name=RCAMT2 style='display:none1' ><input type=text id=RCAMT3 value=0  class=RCAMT3 name=RCAMT3 style='display:none1'><input type=text id=RCAMT4 value='0'  class=RCAMT4 name=RCAMT4 style='display:none1' ><input type=text id=RCAMT5 value='0'  class=RCAMT5 name=RCAMT5 style='display:none1' ><input type=text id=RCAMT6 value='0'  class=RCAMT6 name=RCAMT6 style='display:none1' ></td><td class=hide><input type=text id=RCOL1 value='0'  class=RCOL1 name=RCOL1 style='display:none1' ><input type=text id=RCOL2 value='0'  class=RCOL2 name=RCOL2 style='display:none1' ><input type=text id=RCOL3 value='0'  class=RCOL3 name=RCOL3 style='display:none1' ><input type=text id=RCOL4 value='0'  class=RCOL4 name=RCOL4 style='display:none1' ><input type=text id=RCOL5 value='0'  class=RCOL5 name=RCOL5 style='display:none1' ><input type=text id=RCOL6 value='0'  class=RCOL6 name=RCOL6 style='display:none1' ></td></tr></tbody>";
        //        count++;
        //    }
        //    tabl = tabl + "</Table>";

        //    return tabl;

        //}






        public JsonResult datediff(string id)
        {
            var param = id.Split('-');

            var SDATE = Convert.ToInt32(param[0]);

            var EDATE = Convert.ToInt32(param[1]);
            var Start=Convert.ToDateTime(SDATE);
            var End=Convert.ToDateTime(EDATE);
            Response.Write(SDATE + "------" + EDATE);
            Response.End();
            var DateDiff = End.Subtract(Start).Days;
            return Json(DateDiff, JsonRequestBehavior.AllowGet);
        }

        //...........cost factor with default value




        //..........................Printview...
        [Authorize(Roles = "ExportStuffingBillPrint")]
        public void oldPrintView(string id)
        {
            // Response.Write(@"10.10.5.5"); Response.End();
            //  ........delete TMPRPT...//

            var param = id.Split(';');

            var ids = Convert.ToInt32(param[0]);
            var gsttype = Convert.ToInt32(param[1]);
            var billedto = Convert.ToInt32(param[2]);
            var strHead = param[3].ToString();

            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "STUFFINVOICE", Convert.ToInt32(ids), Session["CUSRID"].ToString());
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
                
                switch (billedto)
                {
                    case 1:
                        if (gsttype == 0)
                        { cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] +  "Export_Stuff_Invoice_Exp.RPT"); }
                        else
                        { cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] +  "GST_Export_Stuff_Invoice_Exp.RPT"); }

                        break;

                    default:
                        if (gsttype == 0)
                        { cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] +  "Export_Stuff_Invoice.RPT"); }
                        else
                        { cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] +  "GST_Export_Stuff_Invoice.RPT"); }

                        break;
                }



                cryRpt.RecordSelectionFormula = "{VW_EXPORT_STUFF_INVOICE_PRINT.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_STUFF_INVOICE_PRINT.TRANMID} = " + ids;


                string paramName = "@FStuffHead";

                for (int i = 0; i < cryRpt.DataDefinition.FormulaFields.Count; i++)
                    if (cryRpt.DataDefinition.FormulaFields[i].FormulaName == "{" + paramName + "}")
                        cryRpt.DataDefinition.FormulaFields[i].Text = "'" + strHead + "'";


                crConnectionInfo.ServerName = stringbuilder.DataSource;
                crConnectionInfo.DatabaseName = stringbuilder.InitialCatalog;
                crConnectionInfo.UserID= "ftec";
               crConnectionInfo.Password= "ftec";

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
        //..........................Printview...
        [Authorize(Roles = "ExportStuffingBillPrint")]
        public void PrintView(string id)
        {
            // Response.Write(@"10.10.5.5"); Response.End();
            //  ........delete TMPRPT...//

            var param = id.Split(';');

            var ids = Convert.ToInt32(param[0]);
            var gsttype = Convert.ToInt32(param[1]);
            var billedto = Convert.ToInt32(param[2]);
            var strHead = param[3].ToString();

            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "STUFFINVOICE", Convert.ToInt32(ids), Session["CUSRID"].ToString());
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
                        { cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] +  "Export_Stuff_Invoice_Exp.RPT"); }
                        else
                        { cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] +  "GST_Export_Stuff_Invoice_Exp.RPT"); }

                        break;

                    default:
                        if (gsttype == 0)
                        { cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] +  "Export_Stuff_Invoice.RPT"); }
                        else
                        { cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] +  "GST_Export_Stuff_Invoice.RPT"); }

                        break;
                }



                cryRpt.RecordSelectionFormula = "{VW_EXPORT_STUFF_INVOICE_PRINT.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_STUFF_INVOICE_PRINT.TRANMID} = " + ids;


                string paramName = "@FStuffHead";

                for (int i = 0; i < cryRpt.DataDefinition.FormulaFields.Count; i++)
                    if (cryRpt.DataDefinition.FormulaFields[i].FormulaName == "{" + paramName + "}")
                        cryRpt.DataDefinition.FormulaFields[i].Text = "'" + strHead + "'";


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
            TmpAStr = TmpAStr + " <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANCSNAME  /*query[0].TRANREFNAME*/ + "</td> </tr> </table> <br> <br> <table width='539' border='0' cellpadding='0' cellspacing='0' style='background:#FFCC66;border:1px solid #663300 '> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left'width='208' >SUDHARSHAN LOGISTICS PRIVATE LIMITED</th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' ># 592, ENNORE EXPRESS HIGH ROAD,</th>";
            TmpAStr = TmpAStr + " </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >CHENNAI - 600 057. </th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Phone No. : 6545 5252 / 2573 3447 / 2573 3762</th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >E-Mail Id : chennaicfs@sancotrans.com</th> </tr> </table>";

            ViewBag.SUB = "Export Invoice No." + query[0].TRANDNO;
            //ViewBag.MSG = TmpAStr;
            //ViewBag.TRANIMPADDR2 = sql[0].CATEADDR2;
            //ViewBag.TRANIMPADDR3 = sql[0].CATEADDR3;
            //ViewBag.TRANIMPADDR4 = sql[0].CATEADDR4;

            return View();
        }
        
        [Authorize(Roles = "ExportStuffingBillPrint")]
        public void APrintView(int? id = 0)
        {
            // Response.Write(@"10.10.5.5"); Response.End();
            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "STUFFINVOICE", Convert.ToInt32(id), Session["CUSRID"].ToString());
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


                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] +  "Export_Stuff_Invoice_E01.RPT");

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
                string path = Server.MapPath("~/Invoice/");

                if (!(Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);
                }
                try
                {
                    cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, path + "\\" + Query[0].TRANNO + ".pdf");
                    cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message);
                }
                
                //    cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");

               
                cryRpt.Dispose();
                cryRpt.Close();
            }

        }
        //end
        //...............Delete Row.............
        [Authorize(Roles = "ExportStuffingBillDelete")]
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

        private List<ItemList> GetItemList(int id)
        {
            SqlDataReader reader = null;
            string _connStr = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(_connStr);

            SqlCommand sqlCmd = new SqlCommand("pr_EInvoice_Export_Transaction_Detail_Assgn", myConnection);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@PTranMID", id);
            sqlCmd.Connection = myConnection;
            myConnection.Open();
            reader = sqlCmd.ExecuteReader();

            List<ItemList> ItemList = new List<ItemList>();

            while (reader.Read())
            {

                ItemList.Add(new ItemList
                {
                    SlNo = 1,
                    PrdDesc = reader["PrdDesc"].ToString(),
                    IsServc = "Y",
                    HsnCd = reader["HsnCd"].ToString(),
                    Barcde = "123456",
                    Qty = 1,
                    FreeQty = 0,
                    Unit = reader["UnitCode"].ToString(),
                    UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                    TotAmt = Convert.ToDecimal(reader["TotAmt"]),
                    Discount = 0,
                    PreTaxVal = 1,
                    AssAmt = Convert.ToDecimal(reader["AssAmt"]),
                    GstRt = Convert.ToDecimal(reader["GstRt"]),
                    IgstAmt = Convert.ToDecimal(reader["IgstAmt"]),
                    CgstAmt = Convert.ToDecimal(reader["CgstAmt"]),
                    SgstAmt = Convert.ToDecimal(reader["SgstAmt"]),
                    CesRt = 0,
                    CesAmt = 0,
                    CesNonAdvlAmt = 0,
                    StateCesRt = 0,
                    StateCesAmt = 0,
                    StateCesNonAdvlAmt = 0,
                    OthChrg = 0,
                    TotItemVal = Convert.ToDecimal(reader["TotItemVal"])
                    //OrdLineRef = "",
                    //OrgCntry = "",
                    //PrdSlNo = ""
                });
            }


            return ItemList;
        }

        public JsonResult GetOPTYP(int id)//Get Operation type for stuff
        {
            var result = context.Database.SqlQuery<VW_EXPORT_INVOICE_STUFFING_DETAIL_CTRL_ASSGN>("select * from VW_EXPORT_INVOICE_STUFFING_DETAIL_CTRL_ASSGN where STFMID=" + id).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTariff(string id)//Get tariff based on grp and operatn
        {
            //var param = id.Split(';');
            //var eoptid = Convert.ToInt32(param[0]);
            var tgid = Convert.ToInt32(id);  //Convert.ToInt32(param[1]);
            //var result = context.Database.SqlQuery<VW_EXPORT_STUFF_INV_TARIFF_ASSGN>("select * from VW_EXPORT_STUFF_INV_TARIFF_ASSGN where EOPTID=" + eoptid+" and TGID="+tgid).ToList();
            //var result = context.Database.SqlQuery<VW_EXPORT_INVOICE_OPERATION_WISE_TARIFF_ASSGN> ("select * from VW_EXPORT_INVOICE_OPERATION_WISE_TARIFF_ASSGN where EOPTID=" + eoptid + " and TGID=" + tgid).ToList();
            //var result = context.Database.SqlQuery<VW_EXPORT_INVOICE_OPERATION_WISE_TARIFF_ASSGN>("select * from VW_EXPORT_INVOICE_OPERATION_WISE_TARIFF_ASSGN where TGID=" + tgid).ToList();
            var result = context.Database.SqlQuery<ExportTariffMaster>("select * from ExportTariffMaster where TGID=" + tgid + "").ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //...............Stuffing No...//
        public JsonResult GetStuffNo(int id)
        {
            var query = context.Database.SqlQuery<pr_Export_Invoice_Stuff_No_Assgn_Result>("EXEC pr_Export_Invoice_Stuff_No_Assgn   @PCHAID=" + id + ", @PSTFTID=0").ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        // [Authorize(Roles = "ExportEInvoice")]
       
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

        //public ActionResult CInvoice(int id = 0)/*10rs.reminder*/
        //{

        //    SqlDataReader reader = null;
        //    SqlDataReader Sreader = null;
        //    string _connStr = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        //    SqlConnection myConnection = new SqlConnection(_connStr);

        //    string _SconnStr = ConfigurationManager.ConnectionStrings["SCFSERPContext"].ConnectionString;
        //    SqlConnection SmyConnection = new SqlConnection(_SconnStr);

        //    var tranmid = id;// Convert.ToInt32(Request.Form.Get("id"));// Convert.ToInt32(ids);

        //    SqlCommand sqlCmd = new SqlCommand();
        //    sqlCmd.CommandType = CommandType.Text;
        //    sqlCmd.CommandText = "Select * from Z_EXPORT_EINVOICE_DETAILS Where TRANMID = " + tranmid;
        //    sqlCmd.Connection = myConnection;
        //    myConnection.Open();
        //    reader = sqlCmd.ExecuteReader();

        //    string stringjson = "";

        //    decimal strgamt = 0;
        //    decimal strg_cgst_amt = 0;
        //    decimal strg_sgst_amt = 0;
        //    decimal strg_igst_amt = 0;

        //    decimal handlamt = 0;
        //    decimal handl_cgst_amt = 0;
        //    decimal handl_sgst_amt = 0;
        //    decimal handl_igst_amt = 0;

        //    decimal cgst_amt = 0;
        //    decimal sgst_amt = 0;
        //    decimal igst_amt = 0;


        //    while (reader.Read())
        //    {
        //        strgamt = Convert.ToDecimal(reader["STRG_TAXABLE_AMT"]);
        //        strg_cgst_amt = Convert.ToDecimal(reader["STRG_CGST_AMT"]);
        //        strg_sgst_amt = Convert.ToDecimal(reader["STRG_SGST_AMT"]);
        //        strg_igst_amt = Convert.ToDecimal(reader["STRG_IGST_AMT"]);

        //        handlamt = Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]);
        //        handl_cgst_amt = Convert.ToDecimal(reader["HANDL_CGST_AMT"]);
        //        handl_sgst_amt = Convert.ToDecimal(reader["HANDL_SGST_AMT"]);
        //        handl_igst_amt = Convert.ToDecimal(reader["HANDL_IGST_AMT"]);

        //        cgst_amt = Convert.ToDecimal(reader["CGST_AMT"]);
        //        sgst_amt = Convert.ToDecimal(reader["SGST_AMT"]);
        //        igst_amt = Convert.ToDecimal(reader["IGST_AMT"]);

        //        var response = new Response()
        //        {
        //            Version = "1.1",

        //            TranDtls = new TranDtls()
        //            {
        //                TaxSch = "GST",
        //                SupTyp = "B2B",
        //                RegRev = "N",
        //                EcmGstin = null,
        //                IgstOnIntra = "N"
        //            },

        //            DocDtls = new DocDtls()
        //            {
        //                Typ = "INV",
        //                No = reader["TRANDNO"].ToString(),
        //                Dt = Convert.ToDateTime(reader["TRANDATE"]).Date.ToString("dd/MM/yyyy")
        //            },

        //            SellerDtls = new SellerDtls()
        //            {
        //                Gstin = reader["COMPGSTNO"].ToString(),
        //                LglNm = reader["COMPNAME"].ToString(),
        //                Addr1 = reader["COMPADDR1"].ToString(),
        //                Addr2 = reader["COMPADDR2"].ToString(),
        //                Loc = reader["COMPLOCTDESC"].ToString(),
        //                Pin = Convert.ToInt32(reader["COMPPINCODE"]),
        //                Stcd = reader["COMPSTATECODE"].ToString(),
        //                Ph = reader["COMPPHN1"].ToString(),
        //                Em = reader["COMPMAIL"].ToString()
        //            },

        //            BuyerDtls = new BuyerDtls()
        //            {
        //                Gstin = reader["CATEBGSTNO"].ToString(),
        //                LglNm = reader["TRANREFNAME"].ToString(),
        //                Pos = reader["STATECODE"].ToString(),
        //                Addr1 = reader["TRANIMPADDR1"].ToString(),
        //                Addr2 = reader["TRANIMPADDR2"].ToString(),
        //                Loc = reader["TRANIMPADDR3"].ToString(),
        //                Pin = Convert.ToInt32(reader["TRANIMPADDR4"]),
        //                Stcd = reader["STATECODE"].ToString(),
        //                Ph = reader["CATEPHN1"].ToString(),
        //                Em = null// reader["CATEMAIL"].ToString()
        //            },

        //            ValDtls = new ValDtls()
        //            {
        //                AssVal = strgamt + handlamt,// Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]),
        //                CesVal = 0,
        //                CgstVal = cgst_amt,// Convert.ToDecimal(reader["HANDL_CGST_AMT"]),
        //                IgstVal = igst_amt,// Convert.ToDecimal(reader["HANDL_IGST_AMT"]),
        //                OthChrg = 0,
        //                SgstVal = sgst_amt,// Convert.ToDecimal(reader["HANDL_sGST_AMT"]),
        //                Discount = 0,
        //                StCesVal = 0,
        //                RndOffAmt = 0,
        //                TotInvVal = Convert.ToDecimal(reader["TRANNAMT"]),
        //                TotItemValSum = strgamt + handlamt,//Convert.ToDecimal(reader["TOTALITEMVAL"])
        //            },

        //            ItemList = GetItemList(tranmid),
        //            //ItemList = new List<ItemList>()
        //            //{
        //            //    new ItemList()
        //            //    {
        //            //        SlNo = 1,
        //            //        PrdDesc = "Handling",
        //            //        IsServc = "Y",
        //            //        HsnCd = reader["HANDL_HSNCODE"].ToString(),
        //            //        Barcde = "123456",
        //            //        Qty = 1,
        //            //        FreeQty = 0,
        //            //        Unit = reader["UNITCODE"].ToString(),
        //            //        UnitPrice = Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]),
        //            //        TotAmt = Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]),
        //            //        Discount = 0,
        //            //        PreTaxVal = 1,
        //            //        AssAmt = Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]),
        //            //        GstRt = 18,
        //            //        IgstAmt =Convert.ToDecimal(reader["HANDL_IGST_AMT"]),
        //            //        CgstAmt = Convert.ToDecimal(reader["HANDL_CGST_AMT"]),
        //            //        SgstAmt = Convert.ToDecimal(reader["HANDL_SGST_AMT"]),
        //            //        CesRt = 0,
        //            //        CesAmt = 0,
        //            //        CesNonAdvlAmt = 0,
        //            //        StateCesRt = 0,
        //            //        StateCesAmt = 0,
        //            //        StateCesNonAdvlAmt = 0,
        //            //        OthChrg = 0,
        //            //        TotItemVal = Convert.ToDecimal(reader["TOTALITEMVAL"])
        //            //        //OrdLineRef = "",
        //            //        //OrgCntry = "",
        //            //        //PrdSlNo = ""
        //            //    },

        //            //    new ItemList()
        //            //    {
        //            //        SlNo = 2,
        //            //        PrdDesc = "Handling",
        //            //        IsServc = "Y    ",
        //            //        HsnCd = reader["HANDL_HSNCODE"].ToString(),
        //            //        Barcde = "123456",
        //            //        Qty = 1,
        //            //        FreeQty = 0,
        //            //        Unit = reader["UNITCODE"].ToString(),
        //            //        UnitPrice = Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]),
        //            //        TotAmt = Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]),
        //            //        Discount = 0,
        //            //        PreTaxVal = 1,
        //            //        AssAmt = Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]),
        //            //        GstRt = 18,
        //            //        IgstAmt =Convert.ToDecimal(reader["HANDL_IGST_AMT"]),
        //            //        CgstAmt = Convert.ToDecimal(reader["HANDL_CGST_AMT"]),
        //            //        SgstAmt = Convert.ToDecimal(reader["HANDL_SGST_AMT"]),
        //            //        CesRt = 0,
        //            //        CesAmt = 0,
        //            //        CesNonAdvlAmt = 0,
        //            //        StateCesRt = 0,
        //            //        StateCesAmt = 0,
        //            //        StateCesNonAdvlAmt = 0,
        //            //        OthChrg = 0,
        //            //        TotItemVal = Convert.ToDecimal(reader["TOTALITEMVAL"])
        //            //        //OrdLineRef = "",
        //            //        //OrgCntry = "",
        //            //        //PrdSlNo = ""
        //            //    },


        //            //}

        //        };

        //        stringjson = JsonConvert.SerializeObject(response);
        //        //update
        //        string result = "";
        //        DataTable dt = new DataTable();
        //        SqlCommand SsqlCmd = new SqlCommand();
        //        SsqlCmd.CommandType = CommandType.Text;
        //        SsqlCmd.CommandText = "Select * from ETRANSACTIONMASTER Where TRANMID = " + tranmid;
        //        SsqlCmd.Connection = SmyConnection;
        //        SmyConnection.Open();
        //        // Sreader = SsqlCmd.ExecuteReader();
        //        SqlDataAdapter Sqladapter = new SqlDataAdapter(SsqlCmd);
        //        Sqladapter.Fill(dt);
        //        //dt.Load(Sreader);
        //        // int numRows = dt.Rows.Count;



        //        if (dt.Rows.Count > 0)
        //        {

        //            foreach (DataRow row in dt.Rows)
        //            {
        //                SqlConnection ZmyConnection = new SqlConnection(_SconnStr);
        //                SqlCommand cmd = new SqlCommand("ETransaction_Update_Assgn", ZmyConnection);
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                //cmd.Parameters.AddWithValue("@CustomerID", 0);    
        //                cmd.Parameters.AddWithValue("@PTranMID", tranmid);
        //                cmd.Parameters.AddWithValue("@PEINVDESC", stringjson);
        //                cmd.Parameters.AddWithValue("@PECINVDESC", row["ECINVDESC"].ToString());
        //                ZmyConnection.Open();
        //                //result = cmd.ExecuteScalar().ToString();
        //                ZmyConnection.Close();
        //            }

        //            //while (Sreader.Read()) 
        //            //{
        //            //    SqlCommand cmd = new SqlCommand("ETransaction_Update_Assgn", SmyConnection);
        //            //    cmd.CommandType = CommandType.StoredProcedure;
        //            //    //cmd.Parameters.AddWithValue("@CustomerID", 0);    
        //            //    cmd.Parameters.AddWithValue("@PTranMID", tranmid);
        //            //    cmd.Parameters.AddWithValue("@PEINVDESC", stringjson);
        //            //    cmd.Parameters.AddWithValue("@PECINVDESC", Sreader["ECINVDESC"].ToString());
        //            //    SmyConnection.Open();
        //            //    result = cmd.ExecuteScalar().ToString();
        //            //    SmyConnection.Close();
        //            //}

        //        }
        //        else
        //        {
        //            SqlConnection ZmyConnection = new SqlConnection(_SconnStr);
        //            SqlCommand cmd = new SqlCommand("ETransaction_Insert_Assgn", ZmyConnection);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            //cmd.Parameters.AddWithValue("@CustomerID", 0);    
        //            cmd.Parameters.AddWithValue("@PTranMID", tranmid);
        //            cmd.Parameters.AddWithValue("@PEINVDESC", stringjson);
        //            cmd.Parameters.AddWithValue("@PECINVDESC", "");
        //            ZmyConnection.Open();
        //            cmd.ExecuteNonQuery();
        //            ZmyConnection.Close();
        //            //result = cmd.ExecuteNonQuery().ToString();
        //        }


        //        //update

        //    }

        //    //  var strPostData = "https://www.fusiontec.com/ebill2/check.php?ids=" + stringjson;





        //    // string name = stuff.Irn;// responseString.Substring(2);// stuff.Name;
        //    // string address = stuff.Address.City;


        //    //context.Database.ExecuteSqlCommand("insert into SMS_STATUS_INFO(kusrid,optnstr,rptid,LastModified,MobileNumber)select '" + Session["CUSRID"] + "','" + responseString + "'," + sql[i].STUDENT_ID + ",'" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "','" + sql[i].STUDENT_PHNNO + "'");



        //    SmyConnection.Close();
        //    myConnection.Close();

        //    return Content(stringjson);

        //    //Response.Write(msg);

        //    //var sterm = term.TrimEnd(',');

        //    ////  var textsms = "Dear Student, we are pleased to confirm that your scholarship amount has been transferred by NEFT directly to your bank account. Please verify receipt from your bank. Thereafter you must check your email, login using the link provided and acknowledge receipt of the payment, positively within 15 days from today's date. If you fail to acknowledge online your scholarship will be cancelled immediately and you will receive no further payments.";
        //    //var textsms = GetSMStext(4);
        //    ////var sql = context.Database.SqlQuery<Student_Detail>("select * from Student_Detail where STUDENT_ID in (" + sterm + ")").ToList();Session["FSTAGEID"].ToString()
        //    //var sql = context.Database.SqlQuery<Student_Detail>("select * from Student_Detail where STUDENT_ID not in (" + sterm + ") and STAGEID = " + Convert.ToInt32(Session["FSTAGEID"]) + " and CATEID = " + Convert.ToInt32(Session["FCATEID"]) + " and DISPSTATUS=0 and CYRID not in(4,7)").ToList();
        //    //for (int i = 0; i < sql.Count; i++)
        //    //{
        //    //    try
        //    //    {
        //    //        var sentack = CheckAndUpdate.CheckCondition("STUDENT_PAYMENT_DETAIL", "SENT_ACK", "STUDENT_ID=" + sql[i].STUDENT_ID + "");
        //    //        if (sentack != "1.00")
        //    //            context.Database.ExecuteSqlCommand("UPDATE STUDENT_PAYMENT_DETAIL SET RS_10_ACK='No',SENT_ACK='0',SMS_SENT=1,LINKSENT_DATE='" + DateTime.Now.Date.ToString("MM/dd/yyyy") + "' WHERE STUDENT_ID =" + sql[i].STUDENT_ID + "");
        //    //        else
        //    //            context.Database.ExecuteSqlCommand("UPDATE STUDENT_PAYMENT_DETAIL SET SMS_SENT=1,LINKSENT_DATE='" + DateTime.Now.Date.ToString("MM/dd/yyyy") + "' WHERE STUDENT_ID =" + sql[i].STUDENT_ID + "");
        //    //        //var strPostData = "http://api.msg91.com/api/sendhttp.php?authkey=71405A7Yy0Qqi53ff0539&mobiles=" + sql[i].STUDENT_PHNNO + "&message=" + textsms + "&sender=MVDSAF&route=4&response=json";
        //    //        //HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(strPostData);
        //    //        //HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
        //    //        //System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
        //    //        //// string responseString = respStreamReader.ReadToEnd();
        //    //        //string responseString = respStreamReader.ReadLine().Substring(46).Substring(0, 7);
        //    //        //context.Database.ExecuteSqlCommand("insert into SMS_STATUS_INFO(kusrid,optnstr,rptid,LastModified,MobileNumber)select '" + Session["CUSRID"] + "','" + responseString + "'," + sql[i].STUDENT_ID + ",'" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "','" + sql[i].STUDENT_PHNNO + "'");
        //    //        //Response.Write("updated Succesfully");
        //    //    }
        //    //    catch (Exception e)
        //    //    {
        //    //        context.Database.ExecuteSqlCommand("insert into SMS_STATUS_INFO(kusrid,optnstr,rptid,LastModified,MobileNumber)select '" + Session["CUSRID"] + "','" + e.Message + "'," + sql[i].STUDENT_ID + ",'" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "','" + sql[i].STUDENT_PHNNO + "'");
        //    //        Response.Write("Sorry Error Occurred While Processing.Contact Admin.");
        //    //    }
        //    //}
        //    //Response.Write("updated Succesfully");
        //}
        //public void UInvoice(int id = 0)/*10rs.reminder*/
        //{
        //    SqlDataReader reader = null;
        //    SqlDataReader Sreader = null;
        //    string _connStr = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        //    SqlConnection myConnection = new SqlConnection(_connStr);

        //    var tranmid = id;// Convert.ToInt32(Request.Form.Get("id"));// Convert.ToInt32(ids);

        //    var strPostData = "https://www.fusiontec.com/ebill2/einvoice.php?ids=" + id;
        //    HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(strPostData);
        //    HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
        //    System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());

        //    // string responseString = respStreamReader.ReadToEnd();
        //    string responseString = respStreamReader.ReadLine();//.Substring(46).Substring(0, 7);

        //    responseString = responseString.Replace("<br>", "~");
        //    responseString = responseString.Replace("=>", "!");
        //    var param = responseString.Split('~');

        //    var status = 0;
        //    string zirnno = "";// param[2].ToString();
        //    string zackdt = "";//param[3].ToString();
        //    string zackno = "";//param[4].ToString();
        //    string imgUrl = "";

        //    string msg = "";


        //    if (param[0] != "") { status = (Convert.ToInt32(param[0].Substring(9))); } else { status = 0; }
        //    if (param[1] != "") { msg = param[1].Substring(10); } else { msg = ""; }

        //    if (status == 1)
        //    {
        //        if (param[2] != "") { zirnno = param[2].Substring(6); } else { zirnno = ""; }
        //        if (param[3] != "") { zackdt = param[3].Substring(8); } else { zackdt = ""; }
        //        if (param[4] != "") { zackno = param[4].Substring(8); } else { zackno = ""; }
        //        if (param[14] != "") { imgUrl = param[14].ToString(); } else { imgUrl = ""; }

        //        SqlConnection GmyConnection = new SqlConnection(_connStr);
        //        SqlCommand cmd = new SqlCommand("pr_IRN_Transaction_Update_Assgn", GmyConnection);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        //cmd.Parameters.AddWithValue("@CustomerID", 0);    
        //        cmd.Parameters.AddWithValue("@PTranMID", tranmid);
        //        cmd.Parameters.AddWithValue("@PIRNNO", zirnno);
        //        cmd.Parameters.AddWithValue("@PACKNO", zackno);
        //        cmd.Parameters.AddWithValue("@PACKDT", Convert.ToDateTime(zackdt));
        //        GmyConnection.Open();
        //        cmd.ExecuteNonQuery();
        //        GmyConnection.Close();

        //        //string remoteFileUrl = "https://my.gstzen.in/" + imgUrl;
        //        string remoteFileUrl = "https://fusiontec.com//ebill2//images//qrcode.png";
        //        string localFileName = tranmid.ToString() + ".png";

        //        string path = Server.MapPath("~/QrCode");


        //        WebClient webClient = new WebClient();
        //        webClient.DownloadFile(remoteFileUrl, path + "\\" + localFileName);

        //        SqlConnection XmyConnection = new SqlConnection(_connStr);
        //        SqlCommand Xcmd = new SqlCommand("pr_Transaction_QrCode_Path_Update_Assgn", XmyConnection);
        //        Xcmd.CommandType = CommandType.StoredProcedure;
        //        //cmd.Parameters.AddWithValue("@CustomerID", 0);    
        //        Xcmd.Parameters.AddWithValue("@PTranMID", tranmid);
        //        Xcmd.Parameters.AddWithValue("@PPath", path + "\\" + localFileName);
        //        XmyConnection.Open();
        //        Xcmd.ExecuteNonQuery();
        //        //result = cmd.ExecuteScalar().ToString();
        //        XmyConnection.Close();

        //        msg = "Uploaded Succesfully";

        //    }
        //    else
        //    {
        //        //msg = "";

        //    }



        //    Response.Write(msg);

        //}


    }//----------End of Class
}//-------------End of namespace