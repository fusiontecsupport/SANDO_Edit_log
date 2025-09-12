using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;

using scfs.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using scfs_erp;

namespace scfs_erp.Controllers.NonPnr
{
    [SessionExpire]
    public class NonPnrInvoiceController : Controller
    {
        // GET: NonPnrInvoice

        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);


        [Authorize(Roles = "NonPnrInvoiceIndex")]
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
                Session["TRANBTYPE"] = "0";
                Session["REGSTRID"] = "51";
            }


            //...........Bill type......//
            List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
            if (Convert.ToInt32(Session["TRANBTYPE"]) == 1)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);

            }
            else if (Convert.ToInt32(Session["TRANBTYPE"]) == 2)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = true };
                selectedBILLYPE.Add(selectedItemGPTY);

            }
            else
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "Please Select Bill Type", Value = "0", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);
            }
            ViewBag.TRANBTYPE = selectedBILLYPE;
            //....end

            //............Billed to....//
            ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == 51 || x.REGSTRID == 52 || x.REGSTRID == 53), "REGSTRID", "REGSTRDESC", Convert.ToInt32(Session["REGSTRID"]));
            //.....end


            DateTime sd = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;

            DateTime ed = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;
            return View();
          
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_NonPnr_Invoice(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToInt32(Session["TRANBTYPE"]), Convert.ToInt32(Session["REGSTRID"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANTIME.Value.ToString("hh:mm tt"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANNAMT.ToString(), d.NOC.ToString(), d.DISPSTATUS, d.TRANNAMT.ToString(), d.TRANMID.ToString() }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "NonPnrInvoiceEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/NonPnrInvoice/GSTForm/" + id);

            //Response.Redirect("/NonPnrInvoice/GSTForm/" + id);
        }

        [Authorize(Roles = "NonPnrInvoicePrint")]
        public void View(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/NonPnrInvoice/GSTFormView/" + id);

            //Response.Redirect("/NonPnrInvoice/GSTForm/" + id);
        }

        //......................Form data....................//
        [Authorize(Roles = "NonPnrInvoiceCreate")]
        public ActionResult Form(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            TransactionMaster tab = new TransactionMaster();
            TransactionMD vm = new TransactionMD();

            //..........................................Dropdown data.........................//
            ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME");
            // ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.SBMDATE = DateTime.Now;
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.TRANLMID = new SelectList("");
            //code commented and modified by Yamuna on 21-07-2021 <S>
            //ViewBag.TRANOTYPE = new SelectList(context.ImportDestuffSlipOperation, "OPRTYPE", "OPRTYPEDESC");
            ViewBag.TRANOTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC");
            //code commented and modified by Yamuna on 21-07-2021 <E>
            ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");

            ViewBag.TARIFFMID = new SelectList("");
            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
            selectedBILLTYPE.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
            selectedBILLTYPE.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "Please Select Bill Type", Value = "0", Selected = false };
            selectedBILLTYPE.Add(selectedItemDSP);
            ViewBag.TRANBTYPE = selectedBILLTYPE;
            //....end

            //.........s.Tax...//
            List<SelectListItem> selectedtaxlst = new List<SelectListItem>();
            SelectListItem selectedItemtax = new SelectListItem { Text = "YES", Value = "1", Selected = true };
            selectedtaxlst.Add(selectedItemtax);
            selectedItemtax = new SelectListItem { Text = "NO", Value = "0", Selected = false };
            selectedtaxlst.Add(selectedItemtax);
            ViewBag.STAX = selectedtaxlst;

            ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == 51 || x.REGSTRID == 52 || x.REGSTRID == 53), "REGSTRID", "REGSTRDESC");
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
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);
                    // selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
                    // selectedBILLYPE.Add(selectedItemGPTY);

                }
                else if (Convert.ToInt32(tab.TRANBTYPE) == 2)
                {

                    //selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                    //  selectedBILLYPE.Add(selectedItemGPTY);
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);

                }
                else
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "Please Select Bill Type", Value = "0", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);
                }
                ViewBag.TRANBTYPE = selectedBILLYPE;
                //..........end

                vm.masterdata = context.transactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.detaildata = context.transactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.costfactor = context.transactionmasterfactor.Where(det => det.TRANMID == id).ToList();
                vm.nonpnrinvcedata = context.Database.SqlQuery<pr_NonPnr_Invoice_IGMNO_Grid_Assgn_Result>("pr_NonPnr_Invoice_IGMNO_Grid_Assgn @PIGMNO='-',@PLNO='-',@PTRANMID=" + id + "").ToList();//........procedure  for edit mode details data

                //var sealcnt = context.Database.SqlQuery<int>("select NOC from VW_IMPORT_BILL_OPEN_SHEET_SEAL_COUNT_ASSGN where BILLEMID=" + billemid).ToList();
                //if (sealcnt.Count > 0)
                //    ViewBag.NOC = sealcnt[0];
                //else
                //    ViewBag.NOC = 0;

                ViewBag.NOC = 0;

                ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.SDPTID == 9 && x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
                var tariffmid = Convert.ToInt32(vm.detaildata[0].TARIFFMID);
                var sql = context.Database.SqlQuery<int>("select TGID from TariffMaster where SDPTID = 9 AND TARIFFMID=" + tariffmid).ToList();
                ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC", Convert.ToInt32(sql[0]));
            }

            return View(vm);
        }//........End of form

        // GST FORM DATA

        [Authorize(Roles = "NonPnrInvoiceCreate")]
        public ActionResult GSTForm(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            TransactionMaster tab = new TransactionMaster();
            TransactionMD vm = new TransactionMD();

            //..........................................Dropdown data.........................//
            ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME");
            // ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.SBMDATE = DateTime.Now;
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.TRANLMID = new SelectList("");
            //ViewBag.TRANOTYPE = new SelectList(context.ImportDestuffSlipOperation, "OPRTYPE", "OPRTYPEDESC");
            ViewBag.TRANOTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC");
            ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");

            ViewBag.TARIFFMID = new SelectList("");
            ViewBag.CATEAID = new SelectList("");            
            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
            //selectedBILLTYPE.Add(selectedItemDSP);
            //selectedItemDSP = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
            //selectedBILLTYPE.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "Please Select Bill Type", Value = "0", Selected = false };
            selectedBILLTYPE.Add(selectedItemDSP);
            ViewBag.TRANBTYPE = selectedBILLTYPE;
            //....end

            //.........s.Tax...//
            List<SelectListItem> selectedtaxlst = new List<SelectListItem>();
            SelectListItem selectedItemtax = new SelectListItem { Text = "Yes", Value = "1", Selected = false };
            selectedtaxlst.Add(selectedItemtax);
            selectedItemtax = new SelectListItem { Text = "No", Value = "0", Selected = true };
            selectedtaxlst.Add(selectedItemtax);
            ViewBag.STAX = selectedtaxlst;

            ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == 51 || x.REGSTRID == 52 || x.REGSTRID == 53), "REGSTRID", "REGSTRDESC");
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
                ViewBag.CATEAID = new SelectList(context.categoryaddressdetails.Where(m => m.CATEID == tab.TRANREFID).OrderBy(x => x.CATEATYPEDESC), "CATEAID", "CATEATYPEDESC", tab.CATEAID);
                //code added by Yamuna on 21-07-2021 <S>
                ViewBag.TRANOTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC");
                //code added by Yamuna on 21-07-2021 <S>
                

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

                List<SelectListItem> selectedTaxDISP = new List<SelectListItem>();
                if (Convert.ToInt32(tab.TRANLSID) == 1)
                {
                    SelectListItem selectedTaxDIS = new SelectListItem { Text = "Yes", Value = "1", Selected = true };
                    selectedTaxDISP.Add(selectedTaxDIS);
                    //selectedTaxDIS = new SelectListItem { Text = "Cancelled", Value = "1", Selected = true };
                    // selectedTaxDISP.Add(selectedTaxDIS);
                    ViewBag.STAX = selectedTaxDISP;
                }
                else
                {
                    SelectListItem selectedTaxDIS = new SelectListItem { Text = "No", Value = "0", Selected = true };
                    selectedTaxDISP.Add(selectedTaxDIS);
                    //selectedTaxDIS = new SelectListItem { Text = "Cancelled", Value = "1", Selected = false };
                    //selectedTaxDISP.Add(selectedTaxDIS);
                    ViewBag.STAX = selectedTaxDISP;
                }//....end


                //....................Bill type.................//
                List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
                if (Convert.ToInt32(tab.TRANBTYPE) == 1)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);
                    // selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
                    // selectedBILLYPE.Add(selectedItemGPTY);

                }
                else if (Convert.ToInt32(tab.TRANBTYPE) == 2)
                {

                    //selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                    //  selectedBILLYPE.Add(selectedItemGPTY);
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);

                }
                else
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "Please Select Bill Type", Value = "0", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);
                }
                ViewBag.TRANBTYPE = selectedBILLYPE;
                //..........end

                vm.masterdata = context.transactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.detaildata = context.transactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.costfactor = context.transactionmasterfactor.Where(det => det.TRANMID == id).ToList();
                vm.nonpnrinvcedata = context.Database.SqlQuery<pr_NonPnr_Invoice_IGMNO_Grid_Assgn_Result>("pr_NonPnr_Invoice_IGMNO_Grid_Assgn @PIGMNO='-',@PLNO='-',@PTRANMID=" + id + "").ToList();//........procedure  for edit mode details data

                //var billemid = Convert.ToInt32(vm.nonpnrinvcedata[0].BILLEMID);
                //var sealcnt = context.Database.SqlQuery<int>("select NOC from VW_IMPORT_BILL_OPEN_SHEET_SEAL_COUNT_ASSGN where BILLEMID=" + billemid).ToList();
                //if (sealcnt.Count > 0)
                //    ViewBag.NOC = sealcnt[0];
                //else
                //    ViewBag.NOC = 0;

                ViewBag.NOC = 0;

                ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.SDPTID == 9 && x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
                var tariffmid = Convert.ToInt32(vm.detaildata[0].TARIFFMID);
                var sql = context.Database.SqlQuery<int>("select TGID from TariffMaster where SDPTID = 9 AND TARIFFMID=" + tariffmid).ToList();
                if (sql.Count > 0)
                {
                    ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC", Convert.ToInt32(sql[0]));
                }
                else
                {
                    ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");
                }                
            }

            return View(vm);
        }//........End of form

        [Authorize(Roles = "NonPnrInvoicePrint")]
        public ActionResult GSTFormView(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            TransactionMaster tab = new TransactionMaster();
            TransactionMD vm = new TransactionMD();

            //..........................................Dropdown data.........................//
            ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME");
            // ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.SBMDATE = DateTime.Now;
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.TRANLMID = new SelectList("");
            //ViewBag.TRANOTYPE = new SelectList(context.ImportDestuffSlipOperation, "OPRTYPE", "OPRTYPEDESC");
            ViewBag.TRANOTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC");
            ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");

            ViewBag.TARIFFMID = new SelectList("");
            ViewBag.CATEAID = new SelectList("");
            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
            //selectedBILLTYPE.Add(selectedItemDSP);
            //selectedItemDSP = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
            //selectedBILLTYPE.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "Please Select Bill Type", Value = "0", Selected = false };
            selectedBILLTYPE.Add(selectedItemDSP);
            ViewBag.TRANBTYPE = selectedBILLTYPE;
            //....end

            //.........s.Tax...//
            List<SelectListItem> selectedtaxlst = new List<SelectListItem>();
            SelectListItem selectedItemtax = new SelectListItem { Text = "Yes", Value = "1", Selected = false };
            selectedtaxlst.Add(selectedItemtax);
            selectedItemtax = new SelectListItem { Text = "No", Value = "0", Selected = true };
            selectedtaxlst.Add(selectedItemtax);
            ViewBag.STAX = selectedtaxlst;

            ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == 51 || x.REGSTRID == 52 || x.REGSTRID == 53), "REGSTRID", "REGSTRDESC");
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
                ViewBag.CATEAID = new SelectList(context.categoryaddressdetails.Where(m => m.CATEID == tab.TRANREFID).OrderBy(x => x.CATEATYPEDESC), "CATEAID", "CATEATYPEDESC", tab.CATEAID);
                //code added by Yamuna on 21-07-2021 <S>
                ViewBag.TRANOTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC");
                //code added by Yamuna on 21-07-2021 <S>


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

                List<SelectListItem> selectedTaxDISP = new List<SelectListItem>();
                if (Convert.ToInt32(tab.TRANLSID) == 1)
                {
                    SelectListItem selectedTaxDIS = new SelectListItem { Text = "Yes", Value = "1", Selected = true };
                    selectedTaxDISP.Add(selectedTaxDIS);
                    //selectedTaxDIS = new SelectListItem { Text = "Cancelled", Value = "1", Selected = true };
                    // selectedTaxDISP.Add(selectedTaxDIS);
                    ViewBag.STAX = selectedTaxDISP;
                }
                else
                {
                    SelectListItem selectedTaxDIS = new SelectListItem { Text = "No", Value = "0", Selected = true };
                    selectedTaxDISP.Add(selectedTaxDIS);
                    //selectedTaxDIS = new SelectListItem { Text = "Cancelled", Value = "1", Selected = false };
                    //selectedTaxDISP.Add(selectedTaxDIS);
                    ViewBag.STAX = selectedTaxDISP;
                }//....end


                //....................Bill type.................//
                List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
                if (Convert.ToInt32(tab.TRANBTYPE) == 1)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);
                    // selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
                    // selectedBILLYPE.Add(selectedItemGPTY);

                }
                else if (Convert.ToInt32(tab.TRANBTYPE) == 2)
                {

                    //selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                    //  selectedBILLYPE.Add(selectedItemGPTY);
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);

                }
                else
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "Please Select Bill Type", Value = "0", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);
                }
                ViewBag.TRANBTYPE = selectedBILLYPE;
                //..........end

                vm.masterdata = context.transactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.detaildata = context.transactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.costfactor = context.transactionmasterfactor.Where(det => det.TRANMID == id).ToList();
                vm.nonpnrinvcedata = context.Database.SqlQuery<pr_NonPnr_Invoice_IGMNO_Grid_Assgn_Result>("pr_NonPnr_Invoice_IGMNO_Grid_Assgn @PIGMNO='-',@PLNO='-',@PTRANMID=" + id + "").ToList();//........procedure  for edit mode details data

                //var billemid = Convert.ToInt32(vm.nonpnrinvcedata[0].BILLEMID);
                //var sealcnt = context.Database.SqlQuery<int>("select NOC from VW_IMPORT_BILL_OPEN_SHEET_SEAL_COUNT_ASSGN where BILLEMID=" + billemid).ToList();
                //if (sealcnt.Count > 0)
                //    ViewBag.NOC = sealcnt[0];
                //else
                //    ViewBag.NOC = 0;

                ViewBag.NOC = 0;

                ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.SDPTID == 9 && x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
                var tariffmid = Convert.ToInt32(vm.detaildata[0].TARIFFMID);
                var sql = context.Database.SqlQuery<int>("select TGID from TariffMaster where SDPTID = 9 AND TARIFFMID=" + tariffmid).ToList();
                if (sql.Count > 0)
                {
                    ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC", Convert.ToInt32(sql[0]));
                }
                else
                {
                    ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");
                }
            }

            return View(vm);
        }//........End of form

        //.................Insert/update values into database.............//
        public void savedata(FormCollection F_Form)
        {
            using (SCFSERPContext context = new SCFSERPContext())
            {
                using (var trans = context.Database.BeginTransaction())
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
                        transactionmaster.SDPTID = 9;
                        //code commented and modified by Yamuna on 21-07-2021 <S>
                        //transactionmaster.TRANTID = 2;
                        transactionmaster.TRANTID = Convert.ToInt16(F_Form["TRANOTYPE"]);
                        //code commented and modified by Yamuna on 21-07-2021 <S>
                        // transactionmaster.TRANLMID = Convert.ToInt32(F_Form["TRANLMID"]);
                        transactionmaster.TRANLSID = Convert.ToInt16(F_Form["STAX"]); //0;
                        transactionmaster.TRANLSNO = null;
                        // transactionmaster.TRANLMNO = F_Form["masterdata[0].TRANLMNO"].ToString();
                        transactionmaster.TRANLMDATE = DateTime.Now;
                        transactionmaster.TRANLSDATE = DateTime.Now;
                        transactionmaster.TRANNARTN = null;
                        
                        if (TRANMID == 0)
                        {
                            transactionmaster.CUSRID = Session["CUSRID"].ToString();
                        }
                        transactionmaster.LMUSRID = Session["CUSRID"].ToString(); //transactionmaster.LMUSRID = 1;
                        transactionmaster.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
                        transactionmaster.PRCSDATE = DateTime.Now;

                        string indate = Convert.ToString(F_Form["masterdata[0].TRANDATE"]);

                        if (indate != null || indate != "")
                        {
                            transactionmaster.TRANDATE = Convert.ToDateTime(indate).Date;
                        }
                        else { transactionmaster.TRANDATE = DateTime.Now.Date; }

                        if (transactionmaster.TRANDATE > Convert.ToDateTime(todayd))
                        {
                            transactionmaster.TRANDATE = Convert.ToDateTime(todayd);
                        }

                        string intime = Convert.ToString(F_Form["masterdata[0].TRANTIME"]);

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
                            else { transactionmaster.TRANTIME = DateTime.Now; }
                        }
                        else { transactionmaster.TRANTIME = DateTime.Now; }

                        if (transactionmaster.TRANTIME > Convert.ToDateTime(todaydt))
                        {
                            transactionmaster.TRANTIME = Convert.ToDateTime(todaydt);
                        }

                        //transactionmaster.TRANDATE = Convert.ToDateTime(F_Form["masterdata[0].TRANTIME"]).Date;
                        //transactionmaster.TRANTIME = Convert.ToDateTime(F_Form["masterdata[0].TRANTIME"]);

                        transactionmaster.TRANREFID = Convert.ToInt32(F_Form["masterdata[0].TRANREFID"]);//chaid
                        transactionmaster.TRANREFNAME = F_Form["masterdata[0].TRANREFNAME"].ToString();//chaname
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
                        transactionmaster.TRANINOC1 = Convert.ToDecimal(F_Form["masterdata[0].TRANINOC1"]);
                        transactionmaster.TRANINOC2 = Convert.ToDecimal(F_Form["masterdata[0].TRANINOC2"]);
                        transactionmaster.TRANSAMT = Convert.ToDecimal(F_Form["TRANSAMT"]);
                        transactionmaster.TRANAAMT = Convert.ToDecimal(F_Form["TRANAAMT"]);
                        transactionmaster.TRANHAMT = Convert.ToDecimal(F_Form["TRANHAMT"]);
                        transactionmaster.TRANEAMT = Convert.ToDecimal(F_Form["TRANEAMT"]);
                        transactionmaster.TRANFAMT = Convert.ToDecimal(F_Form["TRANFAMT"]);
                        transactionmaster.TRANTCAMT = Convert.ToDecimal(F_Form["TRANTCAMT"]);
                        transactionmaster.TRAN_COVID_DISC_AMT = Convert.ToDecimal(F_Form["masterdata[0].TRAN_COVID_DISC_AMT"]);

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
                        transactionmaster.TRANCSNAME = null;                        
                        string CATEAID = Convert.ToString(F_Form["CATEAID"]);
                        if (CATEAID != "" || CATEAID != null || CATEAID != "0")
                        {
                            transactionmaster.CATEAID = Convert.ToInt32(CATEAID);
                        }
                        else { transactionmaster.CATEAID = 0; }

                        string STATEID = Convert.ToString(F_Form["masterdata[0].STATEID"]);
                        if (STATEID != "" || STATEID != null || STATEID != "0")
                        {
                            transactionmaster.STATEID = Convert.ToInt32(STATEID);
                        }
                        else { transactionmaster.STATEID = 0; }

                        transactionmaster.CATEAGSTNO = Convert.ToString(F_Form["masterdata[0].CATEAGSTNO"]);
                        transactionmaster.TRANIMPADDR1 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR1"]);
                        transactionmaster.TRANIMPADDR2 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR2"]);
                        transactionmaster.TRANIMPADDR3 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR3"]);
                        transactionmaster.TRANIMPADDR4 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR4"]);

                        transactionmaster.TALLYSTAT = 0;
                        //transactionmaster.IRNNO = "";
                        //transactionmaster.ACKNO = "";
                        //transactionmaster.ACKDT = DateTime.Now;
                        transactionmaster.QRCODEPATH = "";



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
                        string format = "";
                        string billformat = "";

                        if (TRANMID == 0)
                        {
                            transactionmaster.TRANNO = Convert.ToInt32(auto_numbr_invoice.npnrgstautonum("transactionmaster", "TRANNO", regsid.ToString(), Session["compyid"].ToString(), btype.ToString(), Convert.ToInt32(transactionmaster.TRANREFID)).ToString());
                            //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.npnrgstautonum("transactionmaster", "TRANNO", regsid.ToString(), Session["compyid"].ToString(), btype.ToString()).ToString());
                            int ano = transactionmaster.TRANNO;
                            if (regsid == 51 && btype == 1)
                            {
                                billformat = "LD/CUS/";
                            }
                            else if (regsid == 51 && btype == 2)
                            {
                                billformat = "DS/CUS/";
                            }

                            else if (regsid == 52 && btype == 1)
                            {
                                billformat = "LD/CH/";
                            }

                            else if (regsid == 52 && btype == 2)
                            {
                                billformat = "DS/CH/";
                            }
                            else if (regsid == 53 && btype == 1)
                            {
                                billformat = "ZB/LD/";
                            }
                            else if (regsid == 53 && btype == 2)
                            {
                                billformat = "ZB/DS/";
                            }
                            //........end of autonumber
                            //format = "SUD/IMP/";
                            format = "IMP/" ;
                            string btyp = auto_numbr_invoice.GetCateBillType(Convert.ToInt32(transactionmaster.TRANREFID)).ToString();
                            if (btyp == "")
                            {
                                format = format + Session["GPrxDesc"] + "/";
                            }
                            else
                                format = format.Replace("/", "") + Session["GPrxDesc"] + btyp;
                            string prfx = string.Format(format + "{0:D5}", ano);
                            string billprfx = string.Format(billformat + "{0:D5}", ano);
                            transactionmaster.TRANDNO = prfx.ToString();
                            // NEW COLUMN ADDED BY RAJESH ON 23-JUL-2021
                            transactionmaster.TRANBILLREFNO = billprfx.ToString();
                            // NEW COLUMN ADDED BY RAJESH ON 23-JUL-2021
                            context.transactionmaster.Add(transactionmaster);
                            context.SaveChanges();
                            //TRANMID = transactiondetail.TRANMID;
                        }
                        else
                        {
                            //transactionmaster.REGSTRID = Convert.ToInt16(F_Form["masterdata[0].REGSTRID"]);
                            //transactionmaster.TRANMODE = Convert.ToInt16(F_Form["TRANMODE"]);
                            context.Entry(transactionmaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }


                        //-------------transaction Details
                        string[] F_TRANDID = F_Form.GetValues("TRANDID");
                        string[] TRANDREFNO = F_Form.GetValues("TRANDREFNO");
                        string[] TRANDREFNAME = F_Form.GetValues("TRANDREFNAME");
                        string[] TRANIDATE = F_Form.GetValues("TRANIDATE");
                        string[] TRANSDATE = F_Form.GetValues("TRANSDATE");
                        string[] TRANEDATE = F_Form.GetValues("TRANEDATE");
                        string[] STFDIDS = F_Form.GetValues("STFDIDS");
                        string[] STFDID = F_Form.GetValues("STFDID");
                        string[] boolSTFDIDS = F_Form.GetValues("boolSTFDIDS");
                        string[] TRANDHAMT = F_Form.GetValues("TRANDHAMT");
                        string[] TRANDEAMT = F_Form.GetValues("TRANDEAMT");
                        string[] TRANDFAMT = F_Form.GetValues("TRANDFAMT");
                        string[] TRANDAAMT = F_Form.GetValues("TRANDAAMT");
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
                        string[] RAMT7 = F_Form.GetValues("RAMT7");
                        string[] RCAMT1 = F_Form.GetValues("RCAMT1");
                        string[] RCAMT2 = F_Form.GetValues("RCAMT2");
                        string[] RCAMT3 = F_Form.GetValues("RCAMT3");
                        string[] RCAMT4 = F_Form.GetValues("RCAMT4");
                        string[] RCAMT5 = F_Form.GetValues("RCAMT5");
                        string[] RCAMT6 = F_Form.GetValues("RCAMT6");
                        string[] RCAMT7 = F_Form.GetValues("RCAMT7");
                        string[] RCOL1 = F_Form.GetValues("RCOL1");
                        string[] RCOL2 = F_Form.GetValues("RCOL2");
                        string[] RCOL3 = F_Form.GetValues("RCOL3");
                        string[] RCOL4 = F_Form.GetValues("RCOL4");
                        string[] RCOL5 = F_Form.GetValues("RCOL5");
                        string[] RCOL6 = F_Form.GetValues("RCOL6");
                        string[] days = F_Form.GetValues("days");
                        string[] NOD = F_Form.GetValues("NOD");//days
                        string[] TRANDRATE = F_Form.GetValues("TRANDRATE");
                        string[] BILLEDID = F_Form.GetValues("TRANDREFID"); string[] F_BILLEMID = F_Form.GetValues("TRANDREFID"); string[] TRANDWGHT = F_Form.GetValues("TRANDWGHT");
                        string[] TRANOTYPE = F_Form.GetValues("detaildata[0].TRANOTYPE");
                        string[] TRAND_COVID_DISC_AMT = F_Form.GetValues("TRAND_COVID_DISC_AMT");

                        var BILLEMID = 0;
                        for (int count = 0; count < F_TRANDID.Count(); count++)
                        {
                            if (boolSTFDIDS[count] == "true")
                            {
                                TRANDID = Convert.ToInt32(F_TRANDID[count]); BILLEMID = Convert.ToInt32(F_BILLEMID[count]);
                                //  var boolSTFDID = Convert.ToString(boolSTFDIDS[count]);
                                if (TRANDID != 0)
                                {
                                    transactiondetail = context.transactiondetail.Find(TRANDID);
                                }
                                transactiondetail.TRANMID = transactionmaster.TRANMID;
                                transactiondetail.TRANDREFNO = (TRANDREFNO[count]).ToString();
                                transactiondetail.TRANDREFNAME = (TRANDREFNAME[count]).ToString();
                                transactiondetail.TRANDREFID = Convert.ToInt32(TRANDREFID[count]);//GIDID
                                transactiondetail.TRANIDATE = Convert.ToDateTime(TRANIDATE[count]);
                                transactiondetail.TRANSDATE = Convert.ToDateTime(TRANSDATE[count]);
                                transactiondetail.TRANEDATE = Convert.ToDateTime(TRANEDATE[count]);
                                transactiondetail.TRANVDATE = Convert.ToDateTime(F_Form["detaildata[0].TRANVDATE"]);
                                transactiondetail.TRANDSAMT = Convert.ToDecimal(TRANDSAMT[count]);
                                transactiondetail.TRANDHAMT = Convert.ToDecimal(TRANDHAMT[count]);
                                transactiondetail.TRANDEAMT = Convert.ToDecimal(TRANDEAMT[count]);
                                transactiondetail.TRANDFAMT = Convert.ToDecimal(TRANDFAMT[count]);
                                transactiondetail.TRANDAAMT = Convert.ToDecimal(TRANDAAMT[count]);
                                transactiondetail.TRANDNAMT = Convert.ToDecimal(TRANDNAMT[count]);
                                //  transactiondetail.TRANDNOP = Convert.ToDecimal(TRANDNOP[count]);
                                transactiondetail.TRANDQTY = Convert.ToDecimal(NOD[count]);//NO.OF DAYS
                                transactiondetail.TARIFFMID = Convert.ToInt32(F_Form["detaildata[0].TARIFFMID"]);
                                //  transactiondetail.TRANDRATE = 0;
                                transactiondetail.TRANDRATE = Convert.ToDecimal(TRANDRATE[count]);//TRANSPORT CHARGE
                                transactiondetail.TRANOTYPE = Convert.ToInt16(F_Form["detaildata[0].TRANOTYPE"]);
                                transactiondetail.TRANDGAMT = Convert.ToDecimal(TRANDNAMT[count]);
                                transactiondetail.BILLEDID = Convert.ToInt32(TRANDREFID[count]);
                                transactiondetail.RCOL1 = Convert.ToDecimal(RCOL1[count]);
                                transactiondetail.RCOL2 = Convert.ToDecimal(RCOL2[count]);
                                transactiondetail.RCOL3 = Convert.ToDecimal(RCOL3[count]);
                                transactiondetail.RCOL4 = Convert.ToDecimal(RCOL4[count]);
                                transactiondetail.RCOL5 = Convert.ToDecimal(RCOL5[count]);
                                transactiondetail.RCOL6 = Convert.ToDecimal(RCOL6[count]);
                                transactiondetail.RCOL7 = 0;
                                transactiondetail.RAMT1 = Convert.ToDecimal(RAMT1[count]);
                                transactiondetail.RAMT2 = Convert.ToDecimal(RAMT2[count]);
                                transactiondetail.RAMT3 = Convert.ToDecimal(RAMT3[count]);
                                transactiondetail.RAMT4 = Convert.ToDecimal(RAMT4[count]);
                                transactiondetail.RAMT5 = Convert.ToDecimal(RAMT5[count]);
                                transactiondetail.RAMT6 = Convert.ToDecimal(RAMT6[count]);
                                transactiondetail.RAMT7 = Convert.ToDecimal(RAMT7[count]);
                                transactiondetail.RCAMT1 = Convert.ToDecimal(RCAMT1[count]);
                                transactiondetail.RCAMT2 = Convert.ToDecimal(RCAMT2[count]);
                                transactiondetail.RCAMT3 = Convert.ToDecimal(RCAMT3[count]);
                                transactiondetail.RCAMT4 = Convert.ToDecimal(RCAMT4[count]);
                                transactiondetail.RCAMT5 = Convert.ToDecimal(RCAMT5[count]);
                                transactiondetail.RCAMT6 = Convert.ToDecimal(RCAMT6[count]);
                                transactiondetail.RCAMT7 = Convert.ToDecimal(RCAMT7[count]);
                                transactiondetail.SLABTID = 0;
                                transactiondetail.TRANYTYPE = 0;
                                transactiondetail.TRANDWGHT = Convert.ToDecimal(TRANDWGHT[count]);
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
                        //context.Database.ExecuteSqlCommand("UPDATE BILLENTRYMASTER SET DPAIDNO='" + F_Form["DPAIDNO"] + "',DPAIDAMT='" + Convert.ToDecimal(F_Form["DPAIDAMT"]) + "'  WHERE BILLEMID=" + BILLEMID);

                        context.Database.ExecuteSqlCommand("DELETE FROM transactiondetail  WHERE TRANMID=" + TRANMID + " and  TRANDID NOT IN(" + DELIDS.Substring(1) + ")");

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

                        if (DORDRID != null)
                        {
                            for (int count2 = 0; count2 < DORDRID.Count(); count2++)
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
                       
                        //  Response.Redirect("Index");
                        trans.Commit(); 
                        Response.Redirect("Index");
                    }
                    catch (Exception ex)
                    {
                        //string Message = "" + ex.Message.ToString() + "";
                        trans.Rollback();
                        Response.Write("Sorry!!An Error Ocurred..." + ex.Message);
                        // Response.Redirect("/Error/AccessDenied");
                    }
                }
            }

        }

        //...........cost factor with default value
        public JsonResult DefaultCF()
        {
            DbSqlQuery<CostFactorMaster> data = context.costfactormasters.SqlQuery("select * from costfactormaster  where CFID  in(2,5,6,77,65,90,4) order by CFID DESC");
            DbSqlQuery<CostFactorMaster> data2 = context.costfactormasters.SqlQuery("select * from costfactormaster  where DISPSTATUS=0 and CFID  in(96,97) order by CFID");
            return Json(data.Concat(data2), JsonRequestBehavior.AllowGet);

        }//....end

        public string defCostFactor()
        {


            DbSqlQuery<CostFactorMaster> data = context.costfactormasters.SqlQuery("select * from costfactormaster  where CFID  in(5,6,77,65,90)");


            string html = "";
            //<select id='TAX' class='TAX' name='TAX' onchange='sel_text(this,&quot;CFDESC&quot;);'  > ";

            string first = "";
            string f_order = "";
            string f_expr = "";
            string mod = "";
            string expr = "";
            string first_id = "0";

            int i = 0;

            foreach (var cost in data)
            {

                first_id = cost.CFID.ToString();




                //if (i == 0)
                //{
                first = cost.CFDESC;
                f_order = cost.DORDRID.ToString();
                f_expr = cost.CFEXPR.ToString();
                if (cost.CFMODE != 0)
                    mod = "selected";
                if (cost.CFTYPE != 0)
                    expr = "selected";
                else expr = "";
                //   }

                // html = html + "<option value='" + cost.CFID + "'>" + cost.CFDESC + "</option>";
                html = html + "<tr class='item-row'><Td> <button href='#'   type='button' onclick='del_factor(this)' class='btn btn-danger dfact btn-xs'><i class=glyphicon-minus></i> </button>  </td> <td><input type=text name=TAX id='TAX'  class='hide TAX' value='" + cost.CFID + "'><input type=text name=CFDESC id='CFDESC' class='hide CFDESC' value='" + cost.CFDESC + "'>" + cost.CFDESC + "";
                html = html + "</td> <tD class='col-lg-1' > <select id='CFTYPE' name='CFTYPE' class='CFTYPE' onchange='totalonchange(this)'> <option value='0' >Value </option><option value='1' " + expr + "  >  %</option> </select></td> <td class='col-lg-1'><input type='text' id='DEDNOS' class='DEDNOS' name='DEDNOS' onchange='totalonchange(this)' value='0' style='width:50px'></td><td class='col-lg-1' > <input onchange='totalonchange(this)' type=text value='" + cost.CFEXPR + "' class='CFEXPR' name='CFEXPR' id='CFEXPR'> </td><td><select onchange='totalonchange(this)' class='CFMODE' id='CFMODE' name='CFMODE'> <option value='0' >  +</option><option value='1' " + mod + " >-</option> </select><input type='text' id='DORDRID'   value='" + cost.DORDRID + "' class='DORDRID' style='display:none'  name='DORDRID' >  <input  type=text value='0' style='display:none' name=TRANMFID id='TRANMFID' class='TRANMFID' >";
                html = html + "<input  type=text value='0' style='display:none' name=DEDORDR id='DEDORDR' class='DEDORDR' ><input  type=text value='0' style='display:none' name=TMPCFVAL id='TMPCFVAL' class='TMPCFVAL' ></td><td><input  type=text value='' name=CFAMOUNT id='CFAMOUNT' class='CFAMOUNT' readonly='readonly'> </td>  </TD></tr>";

                i++;

                //do something with cust
            }
            DbSqlQuery<CostFactorMaster> data1 = context.costfactormasters.SqlQuery("select * from costfactormaster  where CFID  in(4)");
            foreach (var cost in data1)
            {

                first_id = cost.CFID.ToString();




                //if (i == 0)
                //{
                first = cost.CFDESC;
                f_order = cost.DORDRID.ToString();
                f_expr = cost.CFEXPR.ToString();
                if (cost.CFMODE != 0)
                    mod = "selected";
                if (cost.CFTYPE != 0)
                    expr = "selected";
                else expr = "";
                //   }

                // html = html + "<option value='" + cost.CFID + "'>" + cost.CFDESC + "</option>";
                html = html + "<tr class='item-row'><Td> <button href='#'   type='button' onclick='del_factor(this)' class='btn btn-danger dfact btn-xs'><i class=glyphicon-minus></i> </button>  </td> <td><input type=text name=TAX id='TAX'  class='hide TAX' value='" + cost.CFID + "'><input type=text name=CFDESC id='CFDESC' class='hide CFDESC' value='" + cost.CFDESC + "'>" + cost.CFDESC + "";
                html = html + "</td> <tD class='col-lg-1' > <select id='CFTYPE' name='CFTYPE' class='CFTYPE' onchange='totalonchange(this)'><option value='0'  >Value </option>  <option value='1'  " + expr + " >  %</option></select></td> <td class='col-lg-1'><input type='text' id='DEDNOS' class='DEDNOS' name='DEDNOS' value='0' onchange='totalonchange(this)' style='width:50px'></td><td class='col-lg-1' > <input onchange='totalonchange(this)' type=text value='" + cost.CFEXPR + "' class='CFEXPR' name='CFEXPR' id='CFEXPR'> </td><td><select onchange='totalonchange(this)' class='CFMODE' id='CFMODE' name='CFMODE'> <option value='0' >  +</option><option value='1' " + mod + " >-</option> </select><input type='text' id='DORDRID'   value='" + cost.DORDRID + "' class='DORDRID' style='display:none'  name='DORDRID' >  <input  type=text value='0' style='display:none' name=TRANMFID id='TRANMFID' class='TRANMFID' >";
                html = html + "<input  type=text value='0' style='display:none' name=DEDORDR id='DEDORDR' class='DEDORDR' ><input  type=text value='0' style='display:none' name=TMPCFVAL id='TMPCFVAL' class='TMPCFVAL' ></td><td><input  type=text value='' name=CFAMOUNT id='CFAMOUNT' class='CFAMOUNT' readonly='readonly'> </td>  </TD></tr>";


                //do something with cust
            }
            DbSqlQuery<CostFactorMaster> data2 = context.costfactormasters.SqlQuery("select * from costfactormaster  where DISPSTATUS=0 and CFID  in(2,96,97) ");
            foreach (var cost in data2)
            {

                first_id = cost.CFID.ToString();

                //if (i == 0)
                //{
                first = cost.CFDESC;
                f_order = cost.DORDRID.ToString();
                f_expr = cost.CFEXPR.ToString();
                if (cost.CFMODE != 0)
                    mod = "selected";
                if (cost.CFTYPE != 0)
                    expr = "selected";
                else expr = "";
                //   }

                // html = html + "<option value='" + cost.CFID + "'>" + cost.CFDESC + "</option>";
                html = html + "<tr class='item-row'><Td> <button href='#'   type='button' onclick='del_factor(this)' class='btn btn-danger dfact btn-xs'><i class=glyphicon-minus></i> </button>  </td> <td><input type=text name=TAX id='TAX'  class='hide TAX' value='" + cost.CFID + "'><input type=text name=CFDESC id='CFDESC' class='hidden CFDESC' value='" + cost.CFDESC + "'>" + cost.CFDESC + "";
                html = html + "</td> <tD class='col-lg-1' > <select id='CFTYPE' name='CFTYPE' class='CFTYPE' onchange='totalonchange(this)'><option value='0' >Value </option> <option value='1' " + expr + "  >  %</option> </select></td> <td class='col-lg-1'><input type='text' id='DEDNOS' class='DEDNOS' name='DEDNOS' value='0' onchange='totalonchange(this)'  style='width:50px'></td><td class='col-lg-1' > <input onchange='totalonchange(this)' type=text value='" + cost.CFEXPR + "' class='CFEXPR' name='CFEXPR' id='CFEXPR'> </td><td><select onchange='totalonchange(this)' class='CFMODE' id='CFMODE' name='CFMODE'> <option value='0' >  +</option><option value='1' " + mod + " >-</option> </select><input type='text' id='DORDRID'   value='" + cost.DORDRID + "' class='DORDRID' style='display:none'  name='DORDRID' >  <input  type=text value='0' style='display:none' name=TRANMFID id='TRANMFID' class='TRANMFID' >";
                html = html + "<input  type=text value='0' style='display:none' name=DEDORDR id='DEDORDR' class='DEDORDR' ><input  type=text value='0' style='display:none' name=TMPCFVAL id='TMPCFVAL' class='TMPCFVAL' ></td><td><input  type=text value='' name=CFAMOUNT id='CFAMOUNT' class='CFAMOUNT' readonly='readonly'> </td>  </TD></tr>";

                i++;

                //do something with cust
            }
            return html;


        }

        //gst def costfactor
        public string gstdefCostFactor()
        {


            //DbSqlQuery<CostFactorMaster> data = context.costfactormasters.SqlQuery("select * from costfactormaster  where CFID  in(5,6,77,65,90)");
            DbSqlQuery<CostFactorMaster> data = context.costfactormasters.SqlQuery("select * from costfactormaster  where CFID  in(6,77,65,90,126,127)");


            string html = "";
            //<select id='TAX' class='TAX' name='TAX' onchange='sel_text(this,&quot;CFDESC&quot;);'  > ";

            string first = "";
            string f_order = "";
            string f_expr = "";
            string mod = "";
            string expr = "";
            string first_id = "0";

            int i = 0;

            foreach (var cost in data)
            {

                first_id = cost.CFID.ToString();

                //if (i == 0)
                //{
                first = cost.CFDESC;
                f_order = cost.DORDRID.ToString();
                f_expr = cost.CFEXPR.ToString();
                if (cost.CFMODE != 0)
                    mod = "selected";
                if (cost.CFTYPE != 0)
                    expr = "selected";
                else expr = "";
                //   }

                // html = html + "<option value='" + cost.CFID + "'>" + cost.CFDESC + "</option>";
                html = html + "<tr class='item-row'><Td> <button href='#'   type='button' onclick='del_factor(this)' class='btn btn-danger dfact btn-xs'><i class=glyphicon-minus></i> </button>  </td> <td><input type=text name=TAX id='TAX'  class='hide TAX' value='" + cost.CFID + "'><input type=text name=CFDESC id='CFDESC' class='hide CFDESC' value='" + cost.CFDESC + "'>" + cost.CFDESC + "";
                html = html + "</td> <tD class='col-lg-1' > <select id='CFTYPE' name='CFTYPE' class='CFTYPE' onchange='totalonchange(this)'> <option value='0' >Value </option><option value='1' " + expr + "  >  %</option> </select></td> <td class='col-lg-1'><input type='text' id='DEDNOS' class='DEDNOS' name='DEDNOS' onchange='totalonchange(this)' value='0' style='width:50px'></td><td class='col-lg-1' > <input onchange='totalonchange(this)' type=text value='" + cost.CFEXPR + "' class='CFEXPR' name='CFEXPR' id='CFEXPR'> </td><td><select onchange='totalonchange(this)' class='CFMODE' id='CFMODE' name='CFMODE'> <option value='0' >  +</option><option value='1' " + mod + " >-</option> </select><input type='text' id='DORDRID'   value='" + cost.DORDRID + "' class='DORDRID' style='display:none'  name='DORDRID' >  <input  type=text value='0' style='display:none' name=TRANMFID id='TRANMFID' class='TRANMFID' >";
                html = html + "<input  type=text value='0' style='display:none' name=DEDORDR id='DEDORDR' class='DEDORDR' ><input  type=text value='0' style='display:none' name=TMPCFVAL id='TMPCFVAL' class='TMPCFVAL' ></td><td><input  type=text value='' name=CFAMOUNT id='CFAMOUNT' class='CFAMOUNT' readonly='readonly'> </td>  </TD></tr>";

                i++;

                //do something with cust
            }
            DbSqlQuery<CostFactorMaster> data1 = context.costfactormasters.SqlQuery("select * from costfactormaster  where CFID  in(4)");
            foreach (var cost in data1)
            {

                first_id = cost.CFID.ToString();

                //if (i == 0)
                //{
                first = cost.CFDESC;
                f_order = cost.DORDRID.ToString();
                f_expr = cost.CFEXPR.ToString();
                if (cost.CFMODE != 0)
                    mod = "selected";
                if (cost.CFTYPE != 0)
                    expr = "selected";
                else expr = "";
                //   }

                // html = html + "<option value='" + cost.CFID + "'>" + cost.CFDESC + "</option>";
                html = html + "<tr class='item-row'><Td> <button href='#'   type='button' onclick='del_factor(this)' class='btn btn-danger dfact btn-xs'><i class=glyphicon-minus></i> </button>  </td> <td><input type=text name=TAX id='TAX'  class='hide TAX' value='" + cost.CFID + "'><input type=text name=CFDESC id='CFDESC' class='hide CFDESC' value='" + cost.CFDESC + "'>" + cost.CFDESC + "";
                html = html + "</td> <tD class='col-lg-1' > <select id='CFTYPE' name='CFTYPE' class='CFTYPE' onchange='totalonchange(this)'><option value='0'  >Value </option>  <option value='1'  " + expr + " >  %</option></select></td> <td class='col-lg-1'><input type='text' id='DEDNOS' class='DEDNOS' name='DEDNOS' value='0' onchange='totalonchange(this)' style='width:50px'></td><td class='col-lg-1' > <input onchange='totalonchange(this)' type=text value='" + cost.CFEXPR + "' class='CFEXPR' name='CFEXPR' id='CFEXPR'> </td><td><select onchange='totalonchange(this)' class='CFMODE' id='CFMODE' name='CFMODE'> <option value='0' >  +</option><option value='1' " + mod + " >-</option> </select><input type='text' id='DORDRID'   value='" + cost.DORDRID + "' class='DORDRID' style='display:none'  name='DORDRID' >  <input  type=text value='0' style='display:none' name=TRANMFID id='TRANMFID' class='TRANMFID' >";
                html = html + "<input  type=text value='0' style='display:none' name=DEDORDR id='DEDORDR' class='DEDORDR' ><input  type=text value='0' style='display:none' name=TMPCFVAL id='TMPCFVAL' class='TMPCFVAL' ></td><td><input  type=text value='' name=CFAMOUNT id='CFAMOUNT' class='CFAMOUNT' readonly='readonly'> </td>  </TD></tr>";


                //do something with cust
            }
            
            return html;

        }

        public JsonResult GetTariff(int id)
        {
            var data = context.Database.SqlQuery<TariffMaster>("Select * from TariffMaster where SDPTID = 9 AND DISPSTATUS=0 and TGID=" + id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCount(int id)/*OTL charges DEDNOS count*/
        {
            var query = context.Database.SqlQuery<int>("select NOC from VW_IMPORT_BILL_OPEN_SHEET_SEAL_COUNT_ASSGN where BILLEMID=" + id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        } //........end


        public JsonResult GetTransportCharge(int id)/*handling amt in category master*/
        {
            var query = context.Database.SqlQuery<CategoryMaster>("select * from CategoryMaster where CATEID=" + id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        } //........end

        #region Autocomplete CHA Name  
        //--------Autocomplete CHA Name        
        public JsonResult AutoChaname(string term)
        {
            var result = (from r in context.categorymasters.Where(x => x.CATETID == 4 && x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).OrderBy(x => x.CATENAME).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public JsonResult LoadBillType(string id)
        {
            
            var param = id.Split('~');
            var igmno = (param[0]); var gplno = param[1]; var TRANMID = Convert.ToInt32(param[2]);
            
            var result = context.Database.SqlQuery<pr_NonPnr_Invoice_IGMNO_BillType_Load_Result>("EXEC pr_NonPnr_Invoice_IGMNO_BillType_Load @PIGMNO='" + igmno + "',@PLNO='" + gplno + "',@PTRANMID=" + TRANMID).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public string Detail(string id)
        {
            int tariffmid = 0;
            var param = id.Split('~');
            var igmno = (param[0]); var gplno = param[1]; var TRANMID = Convert.ToInt32(param[2]);
            if (param[3] != "Please select"|| param[3] != "") { tariffmid = Convert.ToInt32((param[3])); } else { tariffmid = 0; };
            var qry = "EXEC pr_NonPnr_Invoice_IGMNO_Grid_Assgn @PIGMNO='" + igmno + "',@PLNO='" + gplno + "',@PTRANMID=" + TRANMID;
            var query = context.Database.SqlQuery<pr_NonPnr_Invoice_IGMNO_Grid_Assgn_Result>(qry).ToList();

            var tabl = "";
            var count = 0;

            foreach (var rslt in query)
            {

                if (rslt.GODATE == null) rslt.GODATE = DateTime.Now.Date;
                if (rslt.GOTIME == null) rslt.GOTIME = DateTime.Now;
                if (rslt.GPCSTYPE == null) rslt.GPCSTYPE = 0;
                if (rslt.GPLBTYPE == null) rslt.GPLBTYPE = 0;
                if (rslt.GPCSAMT == null) rslt.GPCSAMT = 0;
                if (rslt.GPLBAMT == null) rslt.GPLBAMT = 0;
                if (rslt.STRGDATE == null) rslt.STRGDATE = DateTime.Now.Date;

                if (tariffmid > 6)
                {
                    tabl = tabl + "<tr><td class='hidden1'><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS value='' " + count + "  onchange=total() style='width:30px'><input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value=''></td>";
                    tabl = tabl + "<td><input type=text id=TRANDREFNO value=" + rslt.BOENO + "  class='TRANDREFNO' readonly='readonly' name=TRANDREFNO style='width:56px'></td>";
                    tabl = tabl + "<td class='col-lg-0'><input type='text' id='TRANDREFNAME' value='" + rslt.CONTNRNO + "'  class='TRANDREFNAME' readonly='readonly' name='TRANDREFNAME' ></td>";
                    tabl = tabl + "<td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class=SIZE name=SIZE style='width:40px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANIDATE value='" + rslt.GIDATE.Value.ToString("dd/MM/yyyy") + "' class=TRANIDATE name=TRANIDATE style='width:70px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANSDATE value='" + rslt.GIDATE.Value.ToString("dd/MM/yyyy") + "' class=TRANSDATE name=TRANSDATE readonly='readonly' style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id='TRANEDATE' value='" + rslt.GODATE.Value.ToString("dd/MM/yyyy") + "' class='TRANEDATE datepicker' name='TRANEDATE' style='width:70px' onchange='calculation()'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDWGHT value=" + rslt.WGHT + " class=TRANDWGHT name=TRANDWGHT readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDSAMT value=" + rslt.TRANDSAMT + " class=TRANDSAMT name=TRANDSAMT style='width:70px' readonly='readonly'>";
                    tabl = tabl + "<input type=text id=TRAND_COVID_DISC_AMT value=" + rslt.TRAND_COVID_DISC_AMT + " class='TRAND_COVID_DISC_AMT hide' name=TRAND_COVID_DISC_AMT style='width:70px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDHAMT value=" + rslt.TRANDHAMT + " class=TRANDHAMT name=TRANDHAMT readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text value=" + rslt.TRANDEAMT + " id=TRANDEAMT class=TRANDEAMT name=TRANDEAMT  readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text value=" + rslt.TRANDFAMT + " id=TRANDFAMT class=TRANDFAMT name=TRANDFAMT  readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDAAMT value=" + rslt.TRANDAAMT + " class=TRANDAAMT name=TRANDAAMT   readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text value='0' id=TRANDNAMT class=TRANDNAMT name=TRANDNAMT readonly='readonly' style='width:70px'>";
                    //tabl = tabl + "<input type=text id=TRANDVDATE class='TRANDVDATE hide' name=TRANDVDATE value='" + rslt.DODATE.Value.ToString("dd/MM/yyyy") + "'>";
                    tabl = tabl + "<input type=text id=F_DPAIDNO value='" + rslt.DPAIDNO + "'  class='F_DPAIDNO hide' name=F_DPAIDNO >";
                    tabl = tabl + "<input type=text id=F_DPAIDAMT value=" + rslt.DPAIDAMT + "  class='F_DPAIDAMT hide' name=F_DPAIDAMT ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=CONTNRTID value=0  class=CONTNRTID name=CONTNRTID >";
                    tabl = tabl + "<input type=text id=CONTNRSID value=" + rslt.CONTNRSID + "  class=CONTNRSID name=CONTNRSID >";
                    tabl = tabl + "<input type=text id=TRANDREFID value=" + rslt.GIDID + "  class=TRANDREFID name=TRANDREFID >";
                    //tabl = tabl + "<input type=text id=BILLEDID value=" + rslt.BILLEDID + "  class=BILLEDID name=BILLEDID >";
                    //tabl = tabl + "<input type=text id=BILLEMID value=" + rslt.BILLEMID + "  class=BILLEMID name=BILLEMID >";
                    tabl = tabl + "<input type=text id=F_TARIFFMID value=" + rslt.TARIFFMID + "  class=F_TARIFFMID name=F_TARIFFMID >";
                    tabl = tabl + "<input type=text id=F_TRANDOTYPE value=" + rslt.TRANDOTYPE + "  class=F_TRANDOTYPE name=F_TRANDOTYPE >";
                    tabl = tabl + "<input type=text id=F_CHANAME value='" + rslt.TRANREFNAME + "'  class=F_CHANAME name=F_CHANAME >";
                    tabl = tabl + "<input type=text id=F_STMRNAME value='" + rslt.STMRNAME + "'  class=F_STMRNAME name=F_STMRNAME >";
                    tabl = tabl + "<input type=text id=F_CHAID value=" + rslt.TRANREFID + "  class=F_CHAID name=F_CHAID >";
                    tabl = tabl + "<input type=text id=F_STMRID value=" + rslt.STMRID + "  class=F_STMRID name=F_STMRID ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=TRANDID value=0  class=TRANDID name=TRANDID ></td>";
                    tabl = tabl + "<td class='hide'><input type=text id=days value=0  class=days name=days ><input type=text id=NOD value='" + rslt.NOD + "'  class=NOD name=NOD >";
                    tabl = tabl + "<input type=text id=TRANDRATE value='" + rslt.TRANDRATE + "'  class=TRANDRATE name=TRANDRATE ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=RAMT7 value=" + Convert.ToDecimal(rslt.RAMT7) + "  class=RAMT7 name=RAMT7 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RAMT1 value=" + Convert.ToDecimal(rslt.RAMT1) + "  class=RAMT1 name=RAMT1 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RAMT2 value=" + Convert.ToDecimal(rslt.RAMT2) + "  class=RAMT2 name=RAMT2 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RAMT3 value=" + Convert.ToDecimal(rslt.RAMT3) + "  class=RAMT3 name=RAMT3 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RAMT4 value=" + Convert.ToDecimal(rslt.RAMT4) + "  class=RAMT4 name=RAMT4 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RAMT5 value=" + Convert.ToDecimal(rslt.RAMT5) + "  class=RAMT5 name=RAMT5 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RAMT6 value=" + Convert.ToDecimal(rslt.RAMT6) + " class=RAMT6 name=RAMT6 style='display:none1' ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=SLABMIN6 value='0'  class=SLABMIN6 name=SLABMIN6 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMAX6 value='0'  class=SLABMAX6 name=SLABMAX6 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMIN value='0'  class=SLABMIN name=SLABMIN style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMAX value='0'  class=SLABMAX name=SLABMAX style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMIN1 value='0'  class=SLABMIN1 name=SLABMIN1 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMAX1 value='0'  class=SLABMAX1 name=SLABMAX1 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMIN2 value='0'  class=SLABMIN2 name=SLABMIN2 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMAX2 value='0'  class=SLABMAX2 name=SLABMAX2 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMIN3 value='0'  class=SLABMIN3 name=SLABMIN3 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMAX3 value='0'  class=SLABMAX3 name=SLABMAX3 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMIN4 value='0'  class=SLABMIN4 name=SLABMIN4 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMAX4 value='0'  class=SLABMAX4 name=SLABMAX4 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMIN5 value='0'  class=SLABMIN5 name=SLABMIN5 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMAX5 value='0'  class=SLABMAX5 name=SLABMAX5 style='display:none1' ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=RCAMT7 value=" + Convert.ToDecimal(rslt.RCAMT7) + "  class=RCAMT7 name=RCAMT7 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCAMT1 value=" + Convert.ToDecimal(rslt.RCAMT1) + "  class=RCAMT1 name=RCAMT1 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCAMT2 value=" + Convert.ToDecimal(rslt.RCAMT2) + "  class=RCAMT2 name=RCAMT2 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCAMT3 value=" + Convert.ToDecimal(rslt.RCAMT3) + "  class=RCAMT3 name=RCAMT3 style='display:none1'>";
                    tabl = tabl + "<input type=text id=RCAMT4 value=" + Convert.ToDecimal(rslt.RCAMT4) + "  class=RCAMT4 name=RCAMT4 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCAMT5 value=" + Convert.ToDecimal(rslt.RCAMT5) + "  class=RCAMT5 name=RCAMT5 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCAMT6 value=" + Convert.ToDecimal(rslt.RCAMT6) + "  class=RCAMT6 name=RCAMT6 style='display:none1' ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=RCOL7 value=" + Convert.ToDecimal(rslt.RCOL7) + "  class=RCOL7 name=RCOL7 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCOL1 value=" + Convert.ToDecimal(rslt.RCOL1) + "  class=RCOL1 name=RCOL1 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCOL2 value=" + Convert.ToDecimal(rslt.RCOL2) + " class=RCOL2 name=RCOL2 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCOL3 value=" + Convert.ToDecimal(rslt.RCOL3) + "  class=RCOL3 name=RCOL3 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCOL4 value=" + Convert.ToDecimal(rslt.RCOL4) + "  class=RCOL4 name=RCOL4 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCOL5 value=" + Convert.ToDecimal(rslt.RCOL5) + " class=RCOL5 name=RCOL5 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCOL6 value=" + Convert.ToDecimal(rslt.RCOL6) + "  class=RCOL6 name=RCOL6 style='display:none1' ></td>";
                    tabl = tabl + "<td class='hide'><input type='text' value=" + rslt.GPAAMT + " id='GPAAMT' class='GPAAMT' name='GPAAMT'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPEAMT + " id='GPEAMT' class='GPEAMT' name='GPEAMT'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPETYPE + " id='GPETYPE' class='GPETYPE' name='GPETYPE'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPCSTYPE + " id='GPCSTYPE' class='GPCSTYPE' name='GPCSTYPE'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPCSAMT + " id='GPCSAMT' class='GPCSAMT' name='GPCSAMT'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPLBTYPE + " id='GPLBTYPE' class='GPLBTYPE' name='GPLBTYPE'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPLBAMT + " id='GPLBAMT' class='GPLBAMT' name='GPLBAMT'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPSTYPE + " id='GPSTYPE' class='GPSTYPE' name='GPSTYPE'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPWTYPE + " id='GPWTYPE' class='GPWTYPE' name='GPWTYPE'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPSCNTYPE + " id='GPSCNTYPE' class='GPSCNTYPE' name='GPSCNTYPE'>";
                    tabl = tabl + "<input type='text' value=" + rslt.PRDTGID + " id='F_PRDTGID' class='F_PRDTGID' name='F_PRDTGID'>";
                    tabl = tabl + "<input type='text' value=" + rslt.STATETYPE + " id='F_STATETYPE' class='F_STATETYPE' name='F_STATETYPE'></td>";
                    tabl = tabl + "</tr>";
                }
                else
                {
                    tabl = tabl + "<tr><td class='hidden1'><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS value='' " + count + "  onchange=total() style='width:30px'><input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value=''></td>";
                    tabl = tabl + "<td><input type=text id=TRANDREFNO value=" + rslt.BOENO + "  class='TRANDREFNO' readonly='readonly' name=TRANDREFNO style='width:56px'></td>";
                    tabl = tabl + "<td class='col-lg-0'><input type='text' id='TRANDREFNAME' value='" + rslt.CONTNRNO + "'  class='TRANDREFNAME' readonly='readonly' name='TRANDREFNAME' ></td>";
                    tabl = tabl + "<td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class=SIZE name=SIZE style='width:40px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANIDATE value='" + rslt.GIDATE.Value.ToString("dd/MM/yyyy") + "' class=TRANIDATE name=TRANIDATE style='width:70px' readonly='readonly'></td>";
                    tabl = tabl + "<td ><input type=text id=TRANSDATE value='" + rslt.STRGDATE.Value.ToString("dd/MM/yyyy") + "' class=TRANSDATE name=TRANSDATE readonly='readonly' style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id='TRANEDATE' value='" + rslt.GODATE.Value.ToString("dd/MM/yyyy") + "' class='TRANEDATE datepicker' name='TRANEDATE' style='width:70px' onchange='calculation()'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDWGHT value=" + rslt.WGHT + " class=TRANDWGHT name=TRANDWGHT readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDSAMT value=" + rslt.TRANDSAMT + " class=TRANDSAMT name=TRANDSAMT style='width:70px' readonly='readonly'>";
                    tabl = tabl + "<input type=text id=TRAND_COVID_DISC_AMT value='0' class='TRAND_COVID_DISC_AMT hide' name=TRAND_COVID_DISC_AMT style='width:70px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDHAMT value='0' class=TRANDHAMT name=TRANDHAMT readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text value='0' id=TRANDEAMT class=TRANDEAMT name=TRANDEAMT  readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text value='0'  id=TRANDFAMT class=TRANDFAMT name=TRANDFAMT  readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDAAMT value='0' class=TRANDAAMT name=TRANDAAMT   readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text value='0' id=TRANDNAMT class=TRANDNAMT name=TRANDNAMT readonly='readonly' style='width:70px'>";
                    tabl = tabl + "<td class=hide>";
                    //tabl = tabl + "<input type=text id=TRANDVDATE class=TRANDVDATE name=TRANDVDATE value='" + rslt.DODATE.Value.ToString("dd/MM/yyyy") + "'>";
                    tabl = tabl + "<input type=text id=F_DPAIDNO value='" + rslt.DPAIDNO + "'  class='F_DPAIDNO' name=F_DPAIDNO >";
                    tabl = tabl + "<input type=text id=F_DPAIDAMT value=" + rslt.DPAIDAMT + "  class=F_DPAIDAMT name=F_DPAIDAMT ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=CONTNRTID value=0  class=CONTNRTID name=CONTNRTID >";
                    tabl = tabl + "<input type=text id=CONTNRSID value=" + rslt.CONTNRSID + "  class=CONTNRSID name=CONTNRSID >";
                    tabl = tabl + "<input type=text id=TRANDREFID value=" + rslt.GIDID + "  class=TRANDREFID name=TRANDREFID >";
                    //tabl = tabl + "<input type=text id=BILLEDID value=" + rslt.BILLEDID + "  class=BILLEDID name=BILLEDID >";
                    //tabl = tabl + "<input type=text id=BILLEMID value=" + rslt.BILLEMID + "  class=BILLEMID name=BILLEMID >";
                    tabl = tabl + "<input type=text id=F_TARIFFMID value=" + rslt.TARIFFMID + "  class=F_TARIFFMID name=F_TARIFFMID >";
                    tabl = tabl + "<input type=text id=F_TRANDOTYPE value=" + rslt.TRANDOTYPE + "  class=F_TRANDOTYPE name=F_TRANDOTYPE >";
                    tabl = tabl + "<input type=text id=F_CHANAME value='" + rslt.TRANREFNAME + "'  class=F_CHANAME name=F_CHANAME >";
                    tabl = tabl + "<input type=text id=F_STMRNAME value='" + rslt.STMRNAME + "'  class=F_STMRNAME name=F_STMRNAME >";
                    tabl = tabl + "<input type=text id=F_CHAID value=" + rslt.TRANREFID + "  class=F_CHAID name=F_CHAID >";
                    tabl = tabl + "<input type=text id=F_STMRID value=" + rslt.STMRID + "  class=F_STMRID name=F_STMRID ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=TRANDID value=0  class=TRANDID name=TRANDID ></td><td class='hide'>";
                    tabl = tabl + "<input type=text id=days value=0  class=days name=days ><input type=text id=NOD value='" + rslt.NOD + "'  class=NOD name=NOD >";
                    tabl = tabl + "<input type=text id=TRANDRATE value='" + rslt.TRANDRATE + "'  class=TRANDRATE name=TRANDRATE ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=RAMT7 value=" + Convert.ToDecimal(rslt.RAMT7) + "  class=RAMT7 name=RAMT7 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RAMT1 value=" + Convert.ToDecimal(rslt.RAMT1) + "  class=RAMT1 name=RAMT1 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RAMT2 value=" + Convert.ToDecimal(rslt.RAMT2) + "  class=RAMT2 name=RAMT2 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RAMT3 value=" + Convert.ToDecimal(rslt.RAMT3) + "  class=RAMT3 name=RAMT3 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RAMT4 value=" + Convert.ToDecimal(rslt.RAMT4) + "  class=RAMT4 name=RAMT4 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RAMT5 value=" + Convert.ToDecimal(rslt.RAMT5) + "  class=RAMT5 name=RAMT5 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RAMT6 value=" + Convert.ToDecimal(rslt.RAMT6) + " class=RAMT6 name=RAMT6 style='display:none1' ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=SLABMIN6 value='0'  class=SLABMIN6 name=SLABMIN6 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMAX6 value='0'  class=SLABMAX6 name=SLABMAX6 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMIN value='0'  class=SLABMIN name=SLABMIN style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMAX value='0'  class=SLABMAX name=SLABMAX style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMIN1 value='0'  class=SLABMIN1 name=SLABMIN1 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMAX1 value='0'  class=SLABMAX1 name=SLABMAX1 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMIN2 value='0'  class=SLABMIN2 name=SLABMIN2 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMAX2 value='0'  class=SLABMAX2 name=SLABMAX2 style='display:none1' > ";
                    tabl = tabl + "<input type=text id=SLABMIN3 value='0'  class=SLABMIN3 name=SLABMIN3 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMAX3 value='0'  class=SLABMAX3 name=SLABMAX3 style='display:none1' > ";
                    tabl = tabl + "<input type=text id=SLABMIN4 value='0'  class=SLABMIN4 name=SLABMIN4 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMAX4 value='0'  class=SLABMAX4 name=SLABMAX4 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMIN5 value='0'  class=SLABMIN5 name=SLABMIN5 style='display:none1' >";
                    tabl = tabl + "<input type=text id=SLABMAX5 value='0'  class=SLABMAX5 name=SLABMAX5 style='display:none1' > ";
                    tabl = tabl + "</td><td class=hide><input type=text id=RCAMT7 value=" + Convert.ToDecimal(rslt.RCAMT7) + "  class=RCAMT7 name=RCAMT7 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCAMT1 value=" + Convert.ToDecimal(rslt.RCAMT1) + "  class=RCAMT1 name=RCAMT1 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCAMT2 value=" + Convert.ToDecimal(rslt.RCAMT2) + "  class=RCAMT2 name=RCAMT2 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCAMT3 value=" + Convert.ToDecimal(rslt.RCAMT3) + "  class=RCAMT3 name=RCAMT3 style='display:none1'>";
                    tabl = tabl + "<input type=text id=RCAMT4 value=" + Convert.ToDecimal(rslt.RCAMT4) + "  class=RCAMT4 name=RCAMT4 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCAMT5 value=" + Convert.ToDecimal(rslt.RCAMT5) + "  class=RCAMT5 name=RCAMT5 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCAMT6 value=" + Convert.ToDecimal(rslt.RCAMT6) + "  class=RCAMT6 name=RCAMT6 style='display:none1' ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=RCOL7 value=" + Convert.ToDecimal(rslt.RCOL7) + "  class=RCOL7 name=RCOL7 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCOL1 value=" + Convert.ToDecimal(rslt.RCOL1) + "  class=RCOL1 name=RCOL1 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCOL2 value=" + Convert.ToDecimal(rslt.RCOL2) + " class=RCOL2 name=RCOL2 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCOL3 value=" + Convert.ToDecimal(rslt.RCOL3) + "  class=RCOL3 name=RCOL3 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCOL4 value=" + Convert.ToDecimal(rslt.RCOL4) + "  class=RCOL4 name=RCOL4 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCOL5 value=" + Convert.ToDecimal(rslt.RCOL5) + " class=RCOL5 name=RCOL5 style='display:none1' >";
                    tabl = tabl + "<input type=text id=RCOL6 value=" + Convert.ToDecimal(rslt.RCOL6) + "  class=RCOL6 name=RCOL6 style='display:none1' ></td>";
                    tabl = tabl + "<td class='hide'><input type='text' value=" + rslt.GPAAMT + " id='GPAAMT' class='GPAAMT' name='GPAAMT'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPEAMT + " id='GPEAMT' class='GPEAMT' name='GPEAMT'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPETYPE + " id='GPETYPE' class='GPETYPE' name='GPETYPE'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPCSTYPE + " id='GPCSTYPE' class='GPCSTYPE' name='GPCSTYPE'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPCSAMT + " id='GPCSAMT' class='GPCSAMT' name='GPCSAMT'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPLBTYPE + " id='GPLBTYPE' class='GPLBTYPE' name='GPLBTYPE'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPLBAMT + " id='GPLBAMT' class='GPLBAMT' name='GPLBAMT'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPSTYPE + " id='GPSTYPE' class='GPSTYPE' name='GPSTYPE'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPWTYPE + " id='GPWTYPE' class='GPWTYPE' name='GPWTYPE'>";
                    tabl = tabl + "<input type='text' value=" + rslt.GPSCNTYPE + " id='GPSCNTYPE' class='GPSCNTYPE' name='GPSCNTYPE'>";
                    tabl = tabl + "<input type='text' value=" + rslt.PRDTGID + " id='F_PRDTGID' class='F_PRDTGID' name='F_PRDTGID'>";
                    tabl = tabl + "<input type='text' value=" + rslt.STATETYPE + " id='F_STATETYPE' class='F_STATETYPE' name='F_STATETYPE'></td></tr>";
                }

                count++;
            }
            tabl = tabl + "</Table>";

            return tabl;

        }

        //..........................Printview...
        [Authorize(Roles = "NonPnrInvoicePrint")]
        public void PrintView(string id)
        {
            if (id.Contains(';'))
            {
                var param = id.Split(';');
                // Response.Write(@"10.10.5.5"); Response.End();
                //  ........delete TMPRPT...//

                var ids = Convert.ToInt32(param[0]);
                var rpttype = Convert.ToInt32(param[1]);
                var gsttype = Convert.ToInt32(param[2]);
                var billedto = Convert.ToInt32(param[3]);
                //var strHead = param[4].ToString();
            }
            var strHead = "Non-Pnr Charges";
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "NONPNRINVOICE", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;



                // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");


                //........Get TRANPCOUNT...//
                var Query = context.Database.SqlQuery<int>("select TRANPCOUNT from transactionmaster where TRANMID=" + Convert.ToInt32(id)).ToList();
                var PCNT = 0;

                if (Query.Count() != 0) { PCNT = Query[0]; }
                var TRANPCOUNT = ++PCNT;
                // Response.Write(++PCNT);
                // Response.End();

                context.Database.ExecuteSqlCommand("UPDATE transactionmaster SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + Convert.ToInt32(id));

                string RptNamePath = "";
                //gsttype = 1;
                //switch (billedto)
                //{
                //    case 1:
                //        if (rpttype == 0)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"]+  "NonPnr_Invoice_rpt_IMP.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] +  "GST_NonPnr_Invoice_Rpt.RPT"; }
                //        else if (rpttype == 1)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] +  "NonPnr_Invoice_Group_rpt_IMP.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] +  "GST_NonPnr_Invoice_Group_rpt_IMP.RPT"; }

                //        break;

                //    default:

                //        if (rpttype == 0)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] +  "NonPnr_Invoice_rpt.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] +  "GST_NonPnr_Invoice_Rpt.RPT"; }
                //        else if (rpttype == 1)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] +  "NonPnr_Invoice_Group_rpt.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] +  "GST_NonPnr_Invoice_Group_rpt.RPT"; }

                //        break;
                //}

                RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_NonPnr_Invoice_Rpt.RPT";
                // cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] +  "NonPnr_Invoice_Group_rpt.RPT");

                cryRpt.Load(RptNamePath);
                cryRpt.RecordSelectionFormula = "{VW_TRANSACTION_NONPNR_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_TRANSACTION_NONPNR_CRY_PRINT_ASSGN.TRANMID} = " + Convert.ToInt32(id);



                string paramName = "@FTHANDLING";

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
                cryRpt.Close(); 
                cryRpt.Dispose();                
                GC.Collect();
            }

        }

        [Authorize(Roles = "NonPnrInvoiceNameUpdate")]
        public ActionResult BForm(string id)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            var param = id.Split('~');

            var ids = Convert.ToInt32(param[0]);
            var gstamt = Convert.ToInt32(param[1]);

            //BILLED TO
            //.........s.Tax...//
            List<SelectListItem> selectedtaxlst1 = new List<SelectListItem>();
            SelectListItem selectedItemtax1 = new SelectListItem { Text = "IMPORTER", Value = "1", Selected = false };
            selectedtaxlst1.Add(selectedItemtax1);
            selectedItemtax1 = new SelectListItem { Text = "CHA", Value = "0", Selected = true };
            selectedtaxlst1.Add(selectedItemtax1);
            ViewBag.BILLEDTO = selectedtaxlst1;

            ViewBag.id = ids;
            ViewBag.FGSTAMT = gstamt;
            var query = context.Database.SqlQuery<TransactionMaster>("select * from TransactionMaster where TRANMID=" + ids).ToList();
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
        //bform end

        public ActionResult AForm(string id)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            var param = id.Split('~');

            var ids = Convert.ToInt32(param[0]);
            var gstamt = Convert.ToDecimal(param[1]);

            ViewBag.id = ids;
            ViewBag.FGSTAMT = gstamt;
            //var query = context.Database.SqlQuery<TransactionMaster>("select * from TransactionMaster where TRANMID=" + ids).ToList();
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
            //    ViewBag.TRANIMPADDR1 = sql[0].CATEADDR;
            //    ViewBag.TRANIMPADDR2 = sql[0].CATEMAIL;
            //    ViewBag.TRANIMPADDR3 = sql[0].CATEPHN1;
            //    ViewBag.TRANIMPADDR4 = sql[0].CATEPHN3;
            //}
            var query = context.Database.SqlQuery<pr_NonPnr_Invoice_Print_Customer_Detail_Assgn_Result>("EXEC pr_NonPnr_Invoice_Print_Customer_Detail_Assgn @PTranMID = " + ids).ToList();
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

        public JsonResult AFormAddr(string id)
        {

            var param = id.Split('~');

            var ids = Convert.ToInt32(param[0]);
            var billedto = Convert.ToInt32(param[1]);

            if (billedto == 0)
            {
                var query = context.Database.SqlQuery<VW_NONPNR_TRANSACTION_ADDRESS_DETAIL_ASSGN>("select TRANCSNAME,TRANIMPADDR1,TRANIMPADDR2,TRANIMPADDR3,TRANIMPADDR4,CHACATEGSTNO from VW_NONPNR_TRANSACTION_ADDRESS_DETAIL_ASSGN where TRANMID=" + ids).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = context.Database.SqlQuery<VW_NONPNR_TRANSACTION_ADDRESS_DETAIL_ASSGN>("select IMPRTNAME,IMPCATEADDR1,IMPCATEADDR2,IMPCATEADDR3,IMPCATEADDR4,IMPCATEGSTNO from VW_NONPNR_TRANSACTION_ADDRESS_DETAIL_ASSGN where TRANMID=" + ids).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);

            }

        }

        #region Get Category address Details

        public JsonResult GetCategoryAddressDetails(int CATEID)
        {
            if (CATEID > 0)
            {
                var result = context.Database.SqlQuery<pr_NonPnr_Invoice_CategoryAddresDetails_Result>("EXEC pr_NonPnr_Invoice_CategoryAddresDetails @CATEID =" + Convert.ToInt32(CATEID)).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }


        }

        public JsonResult GetCatAddressDetail(int CATEAID)
        {
            if (CATEAID > 0)
            {
                var result = (from r in context.categoryaddressdetails
                              where r.CATEAID == CATEAID
                              select new { r.STATEID, r.CATEAGSTNO, r.CATEAADDR1, r.CATEAADDR2, r.CATEAADDR3, r.CATEAADDR4 }).Distinct();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }


        }

        public JsonResult GetCatLocationDetail(int CATEID)
        {
            if (CATEID > 0)
            {

                var result = (from r in context.categoryaddressdetails
                              where r.CATEID == CATEID
                              select new { r.CATEAID, r.CATEATYPEDESC }).Distinct();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }


        }

        public JsonResult Chanamemaster(string CATENAME)
        {

            if (CATENAME != null || CATENAME != "")
            {
                var result = (from r in context.tariffmasters.Where(x => x.TGID == 4 && x.DISPSTATUS == 0)
                              where r.TARIFFMDESC.ToLower().Contains(CATENAME.ToLower())
                              select new { r.TARIFFMID }).Distinct();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }


        }


        #endregion 


        public JsonResult AFormCategoryAddr(string id)
        {

            var param = id.Split('~');

            var ids = Convert.ToInt32(param[0]);
            //var billedto = Convert.ToInt32(param[1]);

            //if (billedto == 0)
            //{
            //    var query = context.Database.SqlQuery<CategoryMaster>("select TRANCSNAME,TRANIMPADDR1,TRANIMPADDR2,TRANIMPADDR3,TRANIMPADDR4,CHACATEGSTNO from CategoryMaster where CATETID =" + ids).ToList();
            //    return Json(query, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            var query = context.Database.SqlQuery<VW_NONPNR_TRANSACTION_ADDRESS_DETAIL_ASSGN>("select IMPRTNAME,IMPCATEADDR1,IMPCATEADDR2,IMPCATEADDR3,IMPCATEADDR4,IMPCATEGSTNO from VW_NONPNR_TRANSACTION_ADDRESS_DETAIL_ASSGN where TRANMID=" + ids).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);

            //}

        }


        public void UpdateBillingName(FormCollection F_Form)
        {
            TransactionMaster transactionmaster = new TransactionMaster();
            Int32 TRANMID = Convert.ToInt32(F_Form["FTRANMID"]);
            Int32 OSBILLREFID = Convert.ToInt32(F_Form["OSBILLREFID"]);
            string OSBILLREFNAME = F_Form["OSBILLREFNAME"];
            Int16 OSBILLEDTO = Convert.ToInt16(F_Form["OSBILLEDTO"]);

            if (TRANMID != 0)
            {
                transactionmaster = context.transactionmaster.Find(TRANMID);
            }

            context.Entry(transactionmaster).Entity.TRANREFID = Convert.ToInt32(F_Form["OSBILLREFID"]);
            context.Entry(transactionmaster).Entity.TRANREFNAME = Convert.ToString(F_Form["OSBILLREFNAME"]);
            context.Entry(transactionmaster).Entity.TRANCSNAME = Convert.ToString(F_Form["OSBILLREFNAME"]);
            context.Entry(transactionmaster).Entity.TRANIMPADDR1 = Convert.ToString(F_Form["TRANIMPADDR1"]);
            context.Entry(transactionmaster).Entity.TRANIMPADDR2 = Convert.ToString(F_Form["TRANIMPADDR2"]);
            context.Entry(transactionmaster).Entity.TRANIMPADDR3 = Convert.ToString(F_Form["TRANIMPADDR3"]);
            context.Entry(transactionmaster).Entity.TRANIMPADDR4 = Convert.ToString(F_Form["TRANIMPADDR4"]);
            context.Entry(transactionmaster).Entity.CATEAGSTNO = Convert.ToString(F_Form["OSBILLGSTNO"]);
            context.Entry(transactionmaster).Entity.CATEAID = Convert.ToInt32(F_Form["OCATEAID"]);
            context.Entry(transactionmaster).Entity.STATEID = Convert.ToInt32(F_Form["STATEID"]);
            context.SaveChanges();

            //InvoiceNameUpdate(TRANMID, OSBILLREFID, OSBILLEDTO, OSBILLREFNAME);

            Response.Write("Saved");
        }/*END*/

        //public void InvoiceNameUpdate(int tranmid, int osbillrefid, Int16 osbilledto, string osbillrefname)
        //{
        //    try
        //    {
        //        var Query = context.Database.SqlQuery<int>("select OSMID from ZW_IMPORT_INVOICE_BILLNAME_UPDATE_DETAIL_CHECK_ASSGN where TRANMID=" + tranmid).ToList();
        //        if (Query.Count() > 0)
        //        {
        //            var osmid = Query[0];
        //            context.Database.ExecuteSqlCommand("pr_Import_Invoice_Billing_Name_Update_Assgn @PTranMId = " + tranmid + ", @POSBillRefId  = " + osbillrefid + ", @PBilledto = " + osbilledto + ", @POSMId = " + osmid + " , @POSBillRefName='" + osbillrefname + "'");
        //            // Response.Write("Saved Successfully");
        //        }
        //    }
        //    catch (Exception e) { Response.Write(e.Message); }
        //}

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
            catch (Exception e) {
                return Json(e.Message, JsonRequestBehavior.AllowGet); }
        }


        public void SaveAddress(FormCollection F_Form)
        {
            TransactionMaster transactionmaster = new TransactionMaster();
            Int32 TRANMID = Convert.ToInt32(F_Form["FTRANMID"]);

            if (TRANMID != 0)
            {
                transactionmaster = context.transactionmaster.Find(TRANMID);
            }

            context.Entry(transactionmaster).Entity.TRANCSNAME = Convert.ToString(F_Form["TRANCSNAME"]);
            context.Entry(transactionmaster).Entity.TRANIMPADDR1 = Convert.ToString(F_Form["TRANIMPADDR1"]);
            context.Entry(transactionmaster).Entity.TRANIMPADDR2 = Convert.ToString(F_Form["TRANIMPADDR2"]);
            context.Entry(transactionmaster).Entity.TRANIMPADDR3 = Convert.ToString(F_Form["TRANIMPADDR3"]);
            context.Entry(transactionmaster).Entity.TRANIMPADDR4 = Convert.ToString(F_Form["TRANIMPADDR4"]);
            context.SaveChanges();
            Response.Write("Saved");
        }/*END*/
        /*PRINT DETAIL*/


        public ActionResult CForm(int id)
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

            ViewBag.SUB = "NonPnr Invoice No." + query[0].TRANDNO;
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
                var TmpAStr = "<table width='539' border='0' cellpadding='0' cellspacing='0' style='background:#FFCC66;border:1px solid #663300'><tr><Td colspan='2'  style='background:#663300;color:#FFFFFF;font-weight:700;text-align:center;padding:5px'>Import</Td> </tr> <tr>";
                TmpAStr = TmpAStr + " <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left'width='208' >Invoice No. </th> <td style='padding:4px;border-bottom:1px solid #663300'  width='331'>" + query[0].TRANDNO + "</td> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Date </th>";
                TmpAStr = TmpAStr + " <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANDATE + "</td> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Bill Amount </th> <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANNAMT + "</td> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >CHA Name </th>";
                TmpAStr = TmpAStr + " <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANCSNAME + "</td> </tr> </table> <br> <br> <table width='539' border='0' cellpadding='0' cellspacing='0' style='background:#FFCC66;border:1px solid #663300 '> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left'width='208' >SUDHARSHAN LOGISTICS PRIVATE LIMITED</th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >New No. 41, Redhills High Road, Andarkuppam, New Manali,</th>";
                TmpAStr = TmpAStr + " </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >CHENNAI - 600 103. </th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Phone No. : 7144 9000 </th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >E-Mail Id : billcfs@sudharsan.co</th> </tr> </table>";

                var body = TmpAStr;
                var message = new MailMessage();
                message.To.Add(new MailAddress(mysbfrm["CATEMAIL"].ToString()));  // replace with valid value 
                if (mysbfrm["CATEMAILCC"].ToString() != "")
                    message.CC.Add(new MailAddress(address: mysbfrm["CATEMAILCC"].ToString(), displayName: Session["COMPNAME"].ToString()));
                //   message.CC.Add(new MailAddress(address: "dinesh@fusiontec.com", displayName: Session["COMPNAME"].ToString()));
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
                message.Attachments.Add(new Attachment("E:\\SCFS\\" + Session["CUSRID"] + "\\NonPnrInv\\" + query[0].TRANNO + ".pdf"));
                using (var smtp = new SmtpClient())
                {
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential("billcfs@sudharsan.co", "Cfs2billing@24");//Billcfs@963

                    // smtp.Host = "smtp.gmail.com";
                    smtp.Host = "mail.sudharsan.co";
                    smtp.Port = 587;
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


        //.............................storage ,handling,energy,fuel, and PTI amount...........//
        public JsonResult Bill_Detail(string id)
        {
            var param = id.Split('-');

            var CHRGETYPE = 0;
            var TARIFFMID = 0;
            var CONTNRSID = 0;
            var STMRID = 0;
            var HTYPE = 0;
            var handlng = 0;
            string WGHT = "0";
            if (param[0] != "") { TARIFFMID = Convert.ToInt32(param[0]); } else { TARIFFMID = 0; };
            if (param[1] != "") { CHRGETYPE = Convert.ToInt32(param[1]); } else { CHRGETYPE = 0; };
            if (param[2] != "") { CONTNRSID = Convert.ToInt32(param[2]); } else { CONTNRSID = 0; };  //var CONTNRSID = Convert.ToInt32(param[2]);
            if (param[3] != "") { STMRID = Convert.ToInt32(param[3]); } else { STMRID = 0; };  //var STMRID = Convert.ToInt32(param[3]);/* INSTEAD OF SLABTID=5 ,,PARAM[5]*/
            //if (param[5] != "") { WGHT = Convert.ToInt32(param[5]); } else { WGHT = 0; };
            if (param[5] != "") { WGHT = param[5]; } else { WGHT = "0"; };


            WGHT = WGHT.Replace(',', '.');
            //Response.Write(strqty);
            decimal aqty = Convert.ToDecimal(WGHT);

            if (CHRGETYPE == 1)
            {
                handlng = 2;
                if (param[4] != "") { HTYPE = Convert.ToInt32(param[4]); } else { HTYPE = 0; };
            }

            if (CHRGETYPE == 2)
            {
                handlng = 2;
                if (param[4] != "") { HTYPE = Convert.ToInt32(param[4]); } else { HTYPE = 0; };
            }

            //if (param[1] == "2") { handlng = 4; }

            if (TARIFFMID == 4)
            {
                var query = context.Database.SqlQuery<VW_NONPNR_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_NONPNR_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (" + handlng + ",14,15,16)and HTYPE=" + HTYPE + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + "and STMRID=" + STMRID).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var query = context.Database.SqlQuery<VW_NONPNR_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_NONPNR_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (" + handlng + ",14,15,16)and HTYPE=" + Convert.ToInt32(param[4]) + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and (SLABMIN <= " + WGHT + " or SLABMIN >= " + WGHT + ")").ToList();
                var query = context.Database.SqlQuery<PR_NEW_NONPNR_RATECARDMASTER_FLX_ASSGN_Result>("EXEC PR_NEW_NONPNR_RATECARDMASTER_FLX_ASSGN @PTARIFFMID=" + TARIFFMID + ", @PSLABTID=" + handlng + ", @PHTYPE=" + HTYPE + ", @PCHRGETYPE = " + CHRGETYPE + ", @PCONTNRSID = " + CONTNRSID + ", @PSLABMIN = " + aqty).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }


        }//.....end

        //

        public JsonResult Calc_Bill_Detail(string id)
        {
            var param = id.Split('-');

            var TARIFFMID = 0;
            var CONTNRSID = 0;
            var STMRID = 0;
            var WGHT = 0;

            if (param[0] != "") { TARIFFMID = Convert.ToInt32(param[0]); } else { TARIFFMID = 0; };

            var CHRGETYPE = Convert.ToInt32(param[1]);
            if (param[2] != "") { CONTNRSID = Convert.ToInt32(param[2]); } else { CONTNRSID = 0; };  //var CONTNRSID = Convert.ToInt32(param[2]);
            if (param[3] != "") { STMRID = Convert.ToInt32(param[3]); } else { STMRID = 0; };  //var STMRID = Convert.ToInt32(param[3]);/* INSTEAD OF SLABTID=5 ,,PARAM[5]*/
            if (param[5] != "") { WGHT = Convert.ToInt32(param[5]); } else { WGHT = 0; };

            if (WGHT > 0)
            { WGHT = WGHT / 1000; }

            var handlng = 0;
            if (param[1] == "1") { handlng = 3; }
            if (param[1] == "2") { handlng = 4; }

            if (TARIFFMID == 4)
            {
                var query = context.Database.SqlQuery<VW_NONPNR_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_NONPNR_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (" + handlng + ",14,15,16)and HTYPE=" + Convert.ToInt32(param[4]) + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + "and STMRID=" + STMRID).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var query = context.Database.SqlQuery<VW_NONPNR_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_NONPNR_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (" + handlng + ",14,15,16)and HTYPE=" + Convert.ToInt32(param[4]) + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and (SLABMIN <= " + WGHT + " or SLABMIN >= " + WGHT + ")").ToList();
                var query = context.Database.SqlQuery<PR_NEW_NONPNR_RATECARDMASTER_FLX_ASSGN_Result>("EXEC PR_NEW_NONPNR_RATECARDMASTER_FLX_ASSGN @PTARIFFMID=" + TARIFFMID + ", @PSLABTID=" + handlng + ", @PHTYPE=" + Convert.ToInt32(param[4]) + ", @PCHRGETYPE = " + CHRGETYPE + ", @PCONTNRSID = " + CONTNRSID + ", @PSLABMIN = " + WGHT).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }


        }//.....end
        //

        //............ratecardmaster.....................
        public JsonResult RATECARD(string id)
        {
            var param = id.Split('-');
            //var TARIFFMID = Convert.ToInt32(param[0]);
            var TARIFFMID = 0;
            var CONTNRSID = 0;
            var STMRID = 0;
            //var WGHT = 0;
            var CHRGETYPE = 0;
            var HTYPE = 0;
            var SLABMIN = 0;

            if (param[0] != "") { TARIFFMID = Convert.ToInt32(param[0]); } else { TARIFFMID = 0; };
            if (param[1] != "") { CHRGETYPE = Convert.ToInt32(param[1]); } else { CHRGETYPE = 0; };
            if (param[2] != "") { CONTNRSID = Convert.ToInt32(param[2]); } else { CONTNRSID = 0; };
            if (param[3] != "") { STMRID = Convert.ToInt32(param[3]); } else { STMRID = 0; };
            if (param[4] != "") { SLABMIN = Convert.ToInt32(param[4]); } else { SLABMIN = 0; };
            if (param[5] != "") { HTYPE = Convert.ToInt32(param[5]); } else { HTYPE = 0; };



            //if (param[6] != "")
            //{ WGHT = Convert.ToInt32(param[6]); }
            //else
            //{ WGHT = 0; }


            if (TARIFFMID == 4)
            {
                var query = context.Database.SqlQuery<VW_NONPNR_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_NONPNR_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and HTYPE=" + HTYPE + "  and SLABMIN <= " + SLABMIN + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and STMRID=" + STMRID + " order by SLABMIN").ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = context.Database.SqlQuery<VW_NONPNR_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_NONPNR_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and HTYPE=" + HTYPE + "  and SLABMIN <= " + SLABMIN + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " order by SLABMIN").ToList();

                return Json(query, JsonRequestBehavior.AllowGet);
            }

            //var SLABMIN = Convert.ToInt32(param[4]);
            //if (TARIFFMID == 4)
            //{
            //    var query = context.Database.SqlQuery<VW_NONPNR_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_NONPNR_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID=2 and HTYPE=" + Convert.ToInt32(param[5]) + " and SDTYPE=1 and SLABMIN <= " + SLABMIN + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and STMRID=" + STMRID + " order by SLABMIN").ToList();
            //    return Json(query, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{

            //    var query = context.Database.SqlQuery<VW_NONPNR_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_NONPNR_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID=2 and HTYPE=" + Convert.ToInt32(param[5]) + " and SDTYPE=1 and SLABMIN <= " + SLABMIN + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " order by SLABMIN").ToList();

            //    return Json(query, JsonRequestBehavior.AllowGet);


            //}

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
            //var xcovidsdate = Convert.ToDateTime(Session["COVIDSDATE"]).ToString("dd/MM/yyyy").Split('-');
            //var zcovidsdate = xcovidsdate[1] + '-' + xcovidsdate[0] + '-' + xcovidsdate[2];

            var xcovidsdate = Convert.ToDateTime(Session["COVIDSDATE"]).ToString("dd-MM-yyyy").Split('-');            
            var zcovidsdate = xcovidsdate[2] + '-' + xcovidsdate[1] + '-' + xcovidsdate[0];

            //var xcovidedate = Convert.ToDateTime(Session["COVIDEDATE"]).ToString("dd/MM/yyyy").Split('-');
            //var zcovidedate = xcovidedate[1] + '-' + xcovidedate[0] + '-' + xcovidedate[2];

            var xcovidedate = Convert.ToDateTime(Session["COVIDEDATE"]).ToString("dd-MM-yyyy").Split('-');
            var zcovidedate = xcovidedate[2] + '-' + xcovidedate[1] + '-' + xcovidedate[0];

            using (var e = new SCFSERPEntities())
            {
                //var query = context.Database.SqlQuery<z_pr_New_Import_Covid_Slab_Assgn_Result>("z_pr_New_Import_Covid_Slab_Assgn @PKUSRID = '" + Session["CUSRID"] + "', @PSDATE = '" + zsdate + "', @PEDATE = '" + zedate + "', @PTARIFFMID = " + ztariffmid + ", @PSTMRID = " + zstmrmid + ", @PCHRGETYPE = " + zchrgtype + ", @PSLABTID = 2, @PSLABMIN = 0, @PCONTNRSID = " + zcontnrsid + ", @PSLABHTYPE = 0, @PCHRGDATE = '" + zchrgdate + "' @PCDate1 = '" + zcovidsdate + "', @PCDate2 = '" + zcovidedate + "'").ToList();
                var query = context.Database.SqlQuery<z_pr_New_Import_Covid_Slab_Assgn_Result>("z_pr_New_Import_Covid_Slab_Assgn @PKUSRID = '" + Session["CUSRID"].ToString() + "', @PSDATE = '" + zsdate + "', @PEDATE = '" + zedate + "', @PTARIFFMID = " + ztariffmid + ", @PSTMRID = " + zstmrmid + ", @PCHRGETYPE = " + zchrgtype + ", @PSLABTID = 2, @PSLABMIN = 0, @PCONTNRSID = " + zcontnrsid + ", @PSLABHTYPE = " + zotype + ", @PCHRGDATE = '" + zchrgdate + "', @PCDate1 = '" + zcovidsdate + "', @PCDate2 = '" + zcovidedate + "'").ToList();
                
                return Json(query, JsonRequestBehavior.AllowGet);
                
            }




        } //...end

        public JsonResult GetHandlingAmt(string id)
        {
            var param = id.Split('-');
            // var TARIFFMID = Convert.ToInt32(param[0]);
            var TARIFFMID = 0;
            if (param[0] != "") { TARIFFMID = Convert.ToInt32(param[0]); } else { TARIFFMID = 0; };
            var CHRGETYPE = Convert.ToInt32(param[1]);
            var CONTNRSID = Convert.ToInt32(param[2]);
            var STMRID = Convert.ToInt32(param[3]);
            var handlng = 0;
            if (param[1] == "1") { handlng = 3; }
            if (param[1] == "2") { handlng = 4; }

            var WGHT = "0";

            if (param[6] != "") { WGHT = param[6]; } else { WGHT = "0"; };

            WGHT = WGHT.Replace(',', '.');
            //Response.Write(strqty);
            decimal aqty = Convert.ToDecimal(WGHT);

            //if (TARIFFMID == 4)
            //{
            //    var query = context.Database.SqlQuery<VW_NONPNR_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_NONPNR_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID in (" + handlng + ") and HTYPE=" + Convert.ToInt32(param[4]) + " and SDTYPE=" + Convert.ToInt32(param[5]) + " and  CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and STMRID=" + STMRID + " order by SLABMIN").ToList();
            //    return Json(query, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{

            //    var query = context.Database.SqlQuery<VW_NONPNR_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_NONPNR_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID in (" + handlng + ") and HTYPE=" + Convert.ToInt32(param[4]) + " and SDTYPE=" + Convert.ToInt32(param[5]) + "  and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " order by SLABMIN").ToList();

            //    return Json(query, JsonRequestBehavior.AllowGet);

            //}


            if (TARIFFMID == 4)
            {
                var query = context.Database.SqlQuery<VW_IMPORT_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_NONPNR_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID in (" + handlng + ") and HTYPE=" + Convert.ToInt32(param[4]) + " and SDTYPE=" + Convert.ToInt32(param[5]) + " and  CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and STMRID=" + STMRID + " order by SLABMIN").ToList();
                //var query = context.Database.SqlQuery<VW_NONPNR_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_NONPNR_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID in (" + handlng + ") and HTYPE=" + Convert.ToInt32(param[4]) + " and  CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and STMRID=" + STMRID + " order by SLABMIN").ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var qry = "select SLABAMT,SLABMIN,SLABMAX from VW_NONPNR_RATECARDMASTER_FLX_ASSGN where TARIFFMID = " + TARIFFMID + "and SLABTID in (" + handlng + ") and HTYPE = " + Convert.ToInt32(param[4]) + " and SDTYPE = " + Convert.ToInt32(param[5]) + "  and CHRGETYPE = " + CHRGETYPE + " and CONTNRSID = " + CONTNRSID + " and ((" + aqty + " >= slabmin and " + aqty + " <= slabmax and SLABMAX <> 0) or (" + aqty + " >= slabmin and SLABMAX = 0))   order by SLABMIN";
                //var qry = "select SLABAMT,SLABMIN,SLABMAX from VW_NONPNR_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID in (" + handlng + ") and HTYPE=" + Convert.ToInt32(param[4]) + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " order by SLABMIN";
                    var query = context.Database.SqlQuery<VW_NONPNR_RATECARDMASTER_FLX_ASSGN>(qry).ToList();

                return Json(query, JsonRequestBehavior.AllowGet);
            }

        } //...end


        //............ratecardmaster.....................
        public JsonResult GetImportGSTRATE(string id)
        {
            var param = id.Split('~');
            var SlabTId = 0;
            var StateType = 0;
            var tariffmid = 0;

            if (param[0] != "") { StateType = Convert.ToInt32(param[0]); } else { StateType = 0; }
            if (param[1] != "") { SlabTId = Convert.ToInt32(param[1]); } else { SlabTId = 0; }
            if (param[2] != "") { tariffmid = Convert.ToInt32(param[2]); } else { tariffmid = 0; }

            //var SlabTId = Convert.ToInt32(param[1]);
            if (StateType == 0)
            {
                var query = context.Database.SqlQuery<VW_IMPORT_SLABTYPE_HSN_DETAIL_ASSGN>("select top 1 HSNCODE,CGSTEXPRN,SGSTEXPRN,IGSTEXPRN from VW_IMPORT_SLABTYPE_HSN_DETAIL_ASSGN where SLABTID=" + SlabTId + " and TARIFFMID = " + tariffmid + " order by HSNCODE").ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var query = context.Database.SqlQuery<VW_IMPORT_SLABTYPE_HSN_DETAIL_ASSGN>("select HSNCODE,ACGSTEXPRN as CGSTEXPRN,ASGSTEXPRN as SGSTEXPRN,AIGSTEXPRN as IGSTEXPRN from VW_IMPORT_SLABTYPE_HSN_DETAIL_ASSGN where SLABTID=" + SlabTId + " and TARIFFMID = " + tariffmid + " order by HSNCODE").ToList();

                return Json(query, JsonRequestBehavior.AllowGet);


            }

        } //...end

        [Authorize(Roles = "NonPnrInvoicePrint")]
        public void APrintView(string id)
        {
            // Response.Write(@"10.10.5.5"); Response.End();
            //  ........delete TMPRPT...//
            var param = id.Split(';');

            var ids = Convert.ToInt32(param[0]); var rpttype = Convert.ToInt32(param[1]);

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
                var Query = context.Database.SqlQuery<TransactionMaster>("select * from transactionmaster where TRANMID=" + ids).ToList();
                var PCNT = 0;

                if (Query.Count() != 0) { PCNT = Convert.ToInt32(Query[0].TRANPCOUNT); }
                var TRANPCOUNT = ++PCNT;
                // Response.Write(++PCNT);
                // Response.End();

                context.Database.ExecuteSqlCommand("UPDATE transactionmaster SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + ids);

                if (rpttype == 0)
                    cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] +  "GST_NonPnr_Invoice_rpt_E01.RPT");
                else cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] +  "GST_NonPnr_Invoice_group_rpt_E01.RPT");

                cryRpt.RecordSelectionFormula = "{VW_TRANSACTION_NONPNR_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_TRANSACTION_NONPNR_CRY_PRINT_ASSGN.TRANMID} = " + ids;



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
                string path = "E:\\SCFS\\" + Session["CUSRID"] + "\\NonPnrInv";
                if (!(Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);
                }
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, path + "\\" + Query[0].TRANNO + ".pdf");
                //  cryRpt.SaveAs(path+ "\\"+Query[0].TRANNO+".pdf");
                //   cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                cryRpt.Close(); 
                cryRpt.Dispose();                
                GC.Collect();
            }

        }
        //end

        //...............Delete Row.............
        [Authorize(Roles = "NonPnrInvoiceDelete")]
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
    }
}