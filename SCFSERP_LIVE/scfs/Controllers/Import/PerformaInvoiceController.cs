using scfs.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
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

namespace scfs_erp.Controllers.Import
{
    [SessionExpire]
    public class PerformaInvoiceController : Controller
    {
        //
        // GET: /PerformaInvoice/
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        [Authorize(Roles = "ImportPerformaInvoiceIndex")]
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
                Session["TRANBTYPE"] = "1";
                Session["REGSTRID"] = "1";
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
            else
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = true };
                selectedBILLYPE.Add(selectedItemGPTY);

            }
            ViewBag.TRANBTYPE = selectedBILLYPE;
            //....end

            //............Billed to....//
            ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == 1 || x.REGSTRID == 2 || x.REGSTRID == 6), "REGSTRID", "REGSTRDESC", Convert.ToInt32(Session["REGSTRID"]));
            //.....end


            DateTime sd = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;

            DateTime ed = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;
            return View();
            // return View(context.performatransactionmaster.Where(x => x.TRANDATE >= sd).Where(x => x.TRANDATE <= ed).ToList());
        }//...End of index grid

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Import_Performa_Invoice(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToInt32(Session["TRANBTYPE"]), Convert.ToInt32(Session["REGSTRID"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANTIME.Value.ToString("hh:mm tt"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANNAMT.ToString(), d.GSTAMT.ToString(), d.NOC.ToString(), d.dono.ToString(), d.DISPSTATUS, d.TRANMID.ToString() }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "ImportPerformaInvoiceEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/PerformaInvoice/GSTForm/" + id);
        }
        [Authorize(Roles = "ImportPerformaInvoiceEdit")]
        public void DOEdit(int id)
        {
            Response.Redirect("/PerformaInvoice/DOGSTForm/" + id);
        }
        //......................Form data....................//
        [Authorize(Roles = "ImportPerformaInvoiceCreate")]
        public ActionResult Form(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            Performa_TransactionMaster tab = new Performa_TransactionMaster();
            Performa_TransactionMD vm = new Performa_TransactionMD();

            //..........................................Dropdown data.........................//
            ViewBag.CHACATEAID = new SelectList("");
            ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME");
            // ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.SBMDATE = DateTime.Now;
            
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.TRANLMID = new SelectList("");
            ViewBag.TRANOTYPE = new SelectList(context.ImportDestuffSlipOperation, "OPRTYPE", "OPRTYPEDESC");
            ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");

            ViewBag.TARIFFMID = new SelectList("");
            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
            selectedBILLTYPE.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
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

            ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == 1 || x.REGSTRID == 2 || x.REGSTRID == 6), "REGSTRID", "REGSTRDESC");
            //.....end

            //........display status.........//
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemDISP = new SelectListItem { Text = "In Books", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItemDISP);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            //....end

            if (id != 0)//....Edit Mode
            {
                tab = context.performatransactionmaster.Find(id);//find selected record

                //...................................Selected dropdown value..................................//
                ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME", tab.LCATEID);
                ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC", tab.BANKMID);
                ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL", tab.TRANMODE);
                ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == tab.REGSTRID), "REGSTRID", "REGSTRDESC", tab.REGSTRID);

                int chaid = Convert.ToInt32(tab.TRANREFID);
                int chaaid = Convert.ToInt32(tab.CHACATEAID);
                ViewBag.CHACATEAID = new SelectList(context.categoryaddressdetails.Where(m => m.CATEID == chaid).OrderBy(x => x.CATEATYPEDESC), "CATEAID", "CATEATYPEDESC", chaaid).ToList();

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
                else
                {

                    //selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                    //  selectedBILLYPE.Add(selectedItemGPTY);
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);

                }
                ViewBag.TRANBTYPE = selectedBILLYPE;
                //..........end

                vm.perfmasterdata = context.performatransactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.perfdetaildata = context.performatransactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.perfcostfactor = context.performatransactionmasterfactor.Where(det => det.TRANMID == id).ToList();
                vm.perfimpinvcedata = context.Database.SqlQuery<pr_Import_Performa_Invoice_IGMNO_Grid_Assgn_Result>("EXEC pr_Import_Performa_Invoice_IGMNO_Grid_Assgn @PIGMNO='-',@PLNO='-',@PTRANMID=" + id + "").ToList();//........procedure  for edit mode details data

                var billemid = Convert.ToInt32(vm.perfimpinvcedata[0].BILLEMID);
                var sealcnt = context.Database.SqlQuery<int>("select NOC from VW_IMPORT_BILL_OPEN_SHEET_SEAL_COUNT_ASSGN where BILLEMID=" + billemid).ToList();
                if (sealcnt.Count > 0)
                    ViewBag.NOC = sealcnt[0];
                else
                    ViewBag.NOC = 0;

                ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
                var tariffmid = Convert.ToInt32(vm.perfdetaildata[0].TARIFFMID);
                var sql = context.Database.SqlQuery<int>("select TGID from TariffMaster where TARIFFMID=" + tariffmid).ToList();
                ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC", Convert.ToInt32(sql[0]));
            }

            return View(vm);
        }//........End of form


        // GST FORM DATA

        [Authorize(Roles = "ImportPerformaInvoiceCreate")]
        public ActionResult GSTForm(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            Performa_TransactionMaster tab = new Performa_TransactionMaster();
            Performa_TransactionMD vm = new Performa_TransactionMD();

            //..........................................Dropdown data.........................//
            ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME");
            // ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.SBMDATE = DateTime.Now;
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.TRANLMID = new SelectList("");

            ViewBag.TRANOTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC");
            //ViewBag.TRANOTYPE = new SelectList(context.ImportDestuffSlipOperation, "OPRTYPE", "OPRTYPEDESC");
            ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");

            ViewBag.TARIFFMID = new SelectList("");
            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
            selectedBILLTYPE.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
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

            ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == 1 || x.REGSTRID == 2 || x.REGSTRID == 6), "REGSTRID", "REGSTRDESC");
            //.....end

            //........display status.........//
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemDISP = new SelectListItem { Text = "In Books", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItemDISP);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            //....end
            ViewBag.CHACATEAID = new SelectList("");
            
            if (id != 0)//....Edit Mode
            {
                tab = context.performatransactionmaster.Find(id);//find selected record

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
                int chaid = Convert.ToInt32(tab.TRANREFID);
                int chaaid = Convert.ToInt32(tab.CHACATEAID);
                ViewBag.CHACATEAID = new SelectList(context.categoryaddressdetails.Where(m => m.CATEID == chaid).OrderBy(x => x.CATEATYPEDESC), "CATEAID", "CATEATYPEDESC", chaaid).ToList();


                //....................Bill type.................//
                List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
                if (Convert.ToInt32(tab.TRANBTYPE) == 1)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);
                    // selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
                    // selectedBILLYPE.Add(selectedItemGPTY);

                }
                else
                {

                    //selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                    //  selectedBILLYPE.Add(selectedItemGPTY);
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);

                }
                ViewBag.TRANBTYPE = selectedBILLYPE;
                //..........end

                vm.perfmasterdata = context.performatransactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.perfdetaildata = context.performatransactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.perfcostfactor = context.performatransactionmasterfactor.Where(det => det.TRANMID == id).ToList();
                string cqry = "EXEC pr_Import_Performa_Invoice_IGMNO_Grid_Assgn @PIGMNO='-',@PLNO='-',@PTRANMID=" + id + "";
                vm.perfimpinvcedata = context.Database.SqlQuery<pr_Import_Performa_Invoice_IGMNO_Grid_Assgn_Result>(cqry).ToList();//........procedure  for edit mode details data
                var billemid = 0;
                if (vm.perfimpinvcedata.Count>0)
                    billemid =Convert.ToInt32(vm.perfimpinvcedata[0].BILLEMID);
                var sealcnt = context.Database.SqlQuery<int>("select NOC from VW_IMPORT_BILL_OPEN_SHEET_SEAL_COUNT_ASSGN where BILLEMID=" + billemid).ToList();
                if (sealcnt.Count > 0)
                    ViewBag.NOC = sealcnt[0];
                else
                    ViewBag.NOC = 0;

                ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
                var tariffmid = Convert.ToInt32(vm.perfdetaildata[0].TARIFFMID);
                var sql = context.Database.SqlQuery<int>("select TGID from TariffMaster where TARIFFMID=" + tariffmid).ToList();
                ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC", Convert.ToInt32(sql[0]));
            }

            return View(vm);
        }//........End of form

        [Authorize(Roles = "ImportPerformaInvoiceCreate")]
        public ActionResult DOGSTForm(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            Performa_TransactionMaster tab = new Performa_TransactionMaster();
            Performa_TransactionMD vm = new Performa_TransactionMD();

            //..........................................Dropdown data.........................//
            ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME");
            // ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.SBMDATE = DateTime.Now;
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.TRANLMID = new SelectList("");


            ViewBag.TRANOTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC");
            //ViewBag.TRANOTYPE = new SelectList(context.ImportDestuffSlipOperation, "OPRTYPE", "OPRTYPEDESC");
            ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");

            ViewBag.TARIFFMID = new SelectList("");
            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
            selectedBILLTYPE.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
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

            ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == 1 || x.REGSTRID == 2 || x.REGSTRID == 6), "REGSTRID", "REGSTRDESC");
            //.....end

            //........display status.........//
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemDISP = new SelectListItem { Text = "In Books", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItemDISP);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            //....end

            if (id != 0)//....Edit Mode
            {
                tab = context.performatransactionmaster.Find(id);//find selected record

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
                else
                {

                    //selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                    //  selectedBILLYPE.Add(selectedItemGPTY);
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);

                }
                ViewBag.TRANBTYPE = selectedBILLYPE;
                //..........end

                vm.perfmasterdata = context.performatransactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.perfdetaildata = context.performatransactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.perfcostfactor = context.performatransactionmasterfactor.Where(det => det.TRANMID == id).ToList();
                vm.perfimpinvcedata = context.Database.SqlQuery<pr_Import_Performa_Invoice_IGMNO_Grid_Assgn_Result>("EXEC pr_Import_Performa_Invoice_IGMNO_Grid_Assgn @PIGMNO='-',@PLNO='-',@PTRANMID=" + id + "").ToList();//........procedure  for edit mode details data
                var billemid = Convert.ToInt32(vm.perfimpinvcedata[0].BILLEMID);
                var sealcnt = context.Database.SqlQuery<int>("select NOC from VW_IMPORT_BILL_OPEN_SHEET_SEAL_COUNT_ASSGN where BILLEMID=" + billemid).ToList();
                if (sealcnt.Count > 0)
                    ViewBag.NOC = sealcnt[0];
                else
                    ViewBag.NOC = 0;

                ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
                var tariffmid = Convert.ToInt32(vm.perfdetaildata[0].TARIFFMID);
                var sql = context.Database.SqlQuery<int>("select TGID from TariffMaster where TARIFFMID=" + tariffmid).ToList();
                ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC", Convert.ToInt32(sql[0]));
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

                        Performa_TransactionMaster performatransactionmaster = new Performa_TransactionMaster();
                        Performa_TransactionDetail performatransactiondetail = new Performa_TransactionDetail();
                        //-------Getting Primarykey field--------
                        Int32 TRANMID = Convert.ToInt32(F_Form["perfmasterdata[0].TRANMID"]);
                        Int32 TRANDID = 0;
                        string DELIDS = "";
                        //-----End


                        if (TRANMID != 0)
                        {
                            performatransactionmaster = context.performatransactionmaster.Find(TRANMID);
                        }

                        //...........performatransaction master.............//
                        performatransactionmaster.TRANMID = TRANMID;
                        performatransactionmaster.COMPYID = Convert.ToInt32(Session["compyid"]);
                        performatransactionmaster.SDPTID = 1;
                        performatransactionmaster.TRANTID = 2;
                        performatransactionmaster.TRANLMID = Convert.ToInt32(F_Form["TRANLMID"]);
                        performatransactionmaster.TRANLSID = Convert.ToInt16(F_Form["STAX"]); //0;
                        performatransactionmaster.TRANLSNO = null;
                        performatransactionmaster.TRANLMNO = F_Form["TRANLMNO"].ToString();
                        if (F_Form["TRANLMDATE"] == "")
                            performatransactionmaster.TRANLMDATE = DateTime.Now;
                        else
                            performatransactionmaster.TRANLMDATE = Convert.ToDateTime(F_Form["TRANLMDATE"]).Date; 
                        performatransactionmaster.TRANLSDATE = DateTime.Now;
                        performatransactionmaster.TRANNARTN = null;
                        if (TRANMID == 0)
                            performatransactionmaster.CUSRID = Session["CUSRID"].ToString();
                        performatransactionmaster.LMUSRID = Session["CUSRID"].ToString();
                        performatransactionmaster.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
                        performatransactionmaster.PRCSDATE = DateTime.Now;
                        performatransactionmaster.TRANDATE = Convert.ToDateTime(F_Form["perfmasterdata[0].TRANTIME"]).Date;

                        if (performatransactionmaster.TRANDATE > Convert.ToDateTime(todayd))
                        {
                            performatransactionmaster.TRANDATE = Convert.ToDateTime(todayd);
                        }

                        performatransactionmaster.TRANTIME = Convert.ToDateTime(F_Form["perfmasterdata[0].TRANTIME"]);

                        if (performatransactionmaster.TRANTIME > Convert.ToDateTime(todaydt))
                        {
                            performatransactionmaster.TRANTIME = Convert.ToDateTime(todaydt);
                        }

                        performatransactionmaster.TRANREFID = Convert.ToInt32(F_Form["perfmasterdata[0].TRANREFID"]);//chaid
                        if (F_Form["perfmasterdata[0].TRANREFNAME"] == null )
                            performatransactionmaster.TRANREFNAME = "";
                        else
                            performatransactionmaster.TRANREFNAME = F_Form["perfmasterdata[0].TRANREFNAME"].ToString();//chaname
                        performatransactionmaster.LCATEID = Convert.ToInt32(F_Form["LCATEID"]);
                        performatransactionmaster.TRANBTYPE = Convert.ToInt16(F_Form["TRANBTYPE"]);
                        performatransactionmaster.REGSTRID = Convert.ToInt16(F_Form["REGSTRID"]);
                        performatransactionmaster.TRANMODE = Convert.ToInt16(F_Form["TRANMODE"]);
                        performatransactionmaster.TRANMODEDETL = (F_Form["perfmasterdata[0].TRANMODEDETL"]);
                        performatransactionmaster.TRANGAMT = Convert.ToDecimal(F_Form["perfmasterdata[0].TRANGAMT"]);
                        performatransactionmaster.TRANNAMT = Convert.ToDecimal(F_Form["perfmasterdata[0].TRANNAMT"]);
                        performatransactionmaster.TRANROAMT = Convert.ToDecimal(F_Form["perfmasterdata[0].TRANROAMT"]);
                        performatransactionmaster.TRANREFAMT = Convert.ToDecimal(F_Form["perfmasterdata[0].TRANREFAMT"]);
                        performatransactionmaster.TRANRMKS = (F_Form["perfmasterdata[0].TRANRMKS"]).ToString();
                        performatransactionmaster.TRANAMTWRDS = AmtInWrd.ConvertNumbertoWords(F_Form["perfmasterdata[0].TRANNAMT"]);
                        performatransactionmaster.TRANINOC1 = Convert.ToDecimal(F_Form["perfmasterdata[0].TRANINOC1"]);
                        performatransactionmaster.TRANINOC2 = Convert.ToDecimal(F_Form["perfmasterdata[0].TRANINOC2"]);
                        performatransactionmaster.TRANSAMT = Convert.ToDecimal(F_Form["TRANSAMT"]);
                        performatransactionmaster.TRANAAMT = Convert.ToDecimal(F_Form["TRANAAMT"]);
                        performatransactionmaster.TRANHAMT = Convert.ToDecimal(F_Form["TRANHAMT"]);
                        performatransactionmaster.TRANEAMT = Convert.ToDecimal(F_Form["TRANEAMT"]);
                        performatransactionmaster.TRANFAMT = Convert.ToDecimal(F_Form["TRANFAMT"]);
                        performatransactionmaster.TRANTCAMT = Convert.ToDecimal(F_Form["TRANTCAMT"]);
                        performatransactionmaster.TRAN_COVID_DISC_AMT = Convert.ToDecimal(F_Form["perfmasterdata[0].TRAN_COVID_DISC_AMT"]);

                        performatransactionmaster.STRG_HSNCODE = F_Form["STRG_HSN_CODE"].ToString();
                        performatransactionmaster.HANDL_HSNCODE = F_Form["HANDL_HSN_CODE"].ToString();

                        performatransactionmaster.STRG_TAXABLE_AMT = Convert.ToDecimal(F_Form["STRG_TAXABLE_AMT"]);
                        performatransactionmaster.HANDL_TAXABLE_AMT = Convert.ToDecimal(F_Form["HANDL_TAXABLE_AMT"]);

                        performatransactionmaster.STRG_CGST_EXPRN = Convert.ToDecimal(F_Form["STRG_CGST_EXPRN"]);
                        performatransactionmaster.STRG_SGST_EXPRN = Convert.ToDecimal(F_Form["STRG_SGST_EXPRN"]);
                        performatransactionmaster.STRG_IGST_EXPRN = Convert.ToDecimal(F_Form["STRG_IGST_EXPRN"]);
                        performatransactionmaster.STRG_CGST_AMT = Convert.ToDecimal(F_Form["STRG_CGST_AMT"]);
                        performatransactionmaster.STRG_SGST_AMT = Convert.ToDecimal(F_Form["STRG_SGST_AMT"]);
                        performatransactionmaster.STRG_IGST_AMT = Convert.ToDecimal(F_Form["STRG_IGST_AMT"]);

                        performatransactionmaster.HANDL_CGST_EXPRN = Convert.ToDecimal(F_Form["HANDL_CGST_EXPRN"]);
                        performatransactionmaster.HANDL_SGST_EXPRN = Convert.ToDecimal(F_Form["HANDL_SGST_EXPRN"]);
                        performatransactionmaster.HANDL_IGST_EXPRN = Convert.ToDecimal(F_Form["HANDL_IGST_EXPRN"]);
                        performatransactionmaster.HANDL_CGST_AMT = Convert.ToDecimal(F_Form["HANDL_CGST_AMT"]);
                        performatransactionmaster.HANDL_SGST_AMT = Convert.ToDecimal(F_Form["HANDL_SGST_AMT"]);
                        performatransactionmaster.HANDL_IGST_AMT = Convert.ToDecimal(F_Form["HANDL_IGST_AMT"]);

                        performatransactionmaster.TRANCGSTAMT = Convert.ToDecimal(F_Form["perfmasterdata[0].TRANCGSTAMT"]);
                        performatransactionmaster.TRANSGSTAMT = Convert.ToDecimal(F_Form["perfmasterdata[0].TRANSGSTAMT"]);
                        performatransactionmaster.TRANIGSTAMT = Convert.ToDecimal(F_Form["perfmasterdata[0].TRANIGSTAMT"]);

                        string TRANADONAMT = Convert.ToString(F_Form["perfmasterdata[0].TRANADONAMT"]);
                        if (TRANADONAMT == null || TRANADONAMT == "")
                        { TRANADONAMT = "0"; }

                        performatransactionmaster.TRANADONAMT = Convert.ToDecimal(TRANADONAMT);
                        performatransactionmaster.TRANNARTN = Convert.ToString(F_Form["perfmasterdata[0].TRANNARTN"]);

                        string tranmode = Convert.ToString(F_Form["TRANMODE"]);
                        //var tranmode = Convert.ToInt16(F_Form["TRANMODE"]);
                        if (tranmode != "2" && tranmode != "3")
                        {
                            performatransactionmaster.TRANREFNO = "";
                            performatransactionmaster.TRANREFBNAME = "";
                            performatransactionmaster.BANKMID = 0;
                            performatransactionmaster.TRANREFDATE = DateTime.Now;
                        }
                        else
                        {
                            performatransactionmaster.TRANREFNO = (F_Form["perfmasterdata[0].TRANREFNO"]).ToString();
                            performatransactionmaster.TRANREFBNAME = (F_Form["perfmasterdata[0].TRANREFBNAME"]).ToString();
                            performatransactionmaster.BANKMID = Convert.ToInt32(F_Form["BANKMID"]);
                            performatransactionmaster.TRANREFDATE = Convert.ToDateTime(F_Form["perfmasterdata[0].TRANREFDATE"]).Date;

                        }
                        string CATEAID = Convert.ToString(F_Form["CHACATEAID"]);
                        if (CATEAID != "" || CATEAID != null)
                        {
                            performatransactionmaster.CHACATEAID = Convert.ToInt32(CATEAID);
                        }
                        else
                        {
                            performatransactionmaster.CHACATEAID = 0;
                        }
                        string CHASTATEID = Convert.ToString(F_Form["perfmasterdata[0].CHASTATEID"]);
                        if (CHASTATEID != "" || CHASTATEID != null)
                        {
                            performatransactionmaster.CHASTATEID = Convert.ToInt32(CHASTATEID);
                        }
                        else
                        {
                            performatransactionmaster.CHASTATEID = 0;
                        }
                        performatransactionmaster.CHACATEAGSTNO = Convert.ToString(F_Form["perfmasterdata[0].CHACATEAGSTNO"]);
                        performatransactionmaster.CHAADDR1 = Convert.ToString(F_Form["perfmasterdata[0].CHAADDR1"]);
                        performatransactionmaster.CHAADDR2 = Convert.ToString(F_Form["perfmasterdata[0].CHAADDR2"]);
                        performatransactionmaster.CHAADDR3 = Convert.ToString(F_Form["perfmasterdata[0].CHAADDR3"]);
                        performatransactionmaster.CHAADDR4 = Convert.ToString(F_Form["perfmasterdata[0].CHAADDR4"]);

                        //.................Autonumber............//
                        var regsid = Convert.ToInt32(F_Form["REGSTRID"]);
                        var btype = Convert.ToInt32(F_Form["TRANBTYPE"]);
                        string format = "";
                        string billformat = "";

                        if (TRANMID == 0)
                        {

                            performatransactionmaster.TRANNO = Convert.ToInt32(Autonumber.performaautonum("Performa_TransactionMaster", "TRANNO", regsid.ToString(), Session["compyid"].ToString(), btype.ToString(), Convert.ToInt32(performatransactionmaster.TRANREFID)).ToString());

                            
                            int ano = performatransactionmaster.TRANNO;
                            if (regsid == 1 && btype == 1)
                            {
                                billformat = "STL/LD/CU/";
                            }
                            else if (regsid == 1 && btype == 2)
                            {
                                billformat = "STL/DS/CU/";
                            }

                            else if (regsid == 2 && btype == 1)
                            {
                                billformat = "STL/LD/CH/";
                            }

                            else if (regsid == 2 && btype == 2)
                            {
                                billformat = "STL/DS/CH/";
                            }
                            else if (regsid == 6 && btype == 1)
                            {
                                billformat = "ZB/LD/";
                            }
                            else if (regsid == 6 && btype == 2)
                            {
                                billformat = "ZB/DS/";
                            }
                            //........end of autonumber
                            //format = "SUD/IMP/";
                            format = "IMP/" + Session["GPrxDesc"] + "/";
                            string prfx = string.Format(format + "{0:D5}", ano);
                            string billprfx = string.Format(billformat + "{0:D5}", ano);
                            performatransactionmaster.TRANDNO = prfx.ToString();
                            performatransactionmaster.TRANBILLREFNO = billprfx.ToString();
                            context.performatransactionmaster.Add(performatransactionmaster);
                            context.SaveChanges();
                            TRANMID = performatransactiondetail.TRANMID;
                        }
                        else
                        {
                            //performatransactionmaster.REGSTRID = Convert.ToInt16(F_Form["perfmasterdata[0].REGSTRID"]);
                            //performatransactionmaster.TRANMODE = Convert.ToInt16(F_Form["TRANMODE"]);

                            //performatransactionmaster.TRANNO = Convert.ToInt32(Autonumber.performaautonum("Performa_TransactionMaster", "TRANNO", regsid.ToString(), Session["compyid"].ToString(), btype.ToString(), Convert.ToInt32(performatransactionmaster.TRANREFID)).ToString());


                            //int ano = performatransactionmaster.TRANNO;
                            //if (regsid == 1 && btype == 1)
                            //{
                            //    billformat = "STL/LD/CU/";
                            //}
                            //else if (regsid == 1 && btype == 2)
                            //{
                            //    billformat = "STL/DS/CU/";
                            //}

                            //else if (regsid == 2 && btype == 1)
                            //{
                            //    billformat = "STL/LD/CH/";
                            //}

                            //else if (regsid == 2 && btype == 2)
                            //{
                            //    billformat = "STL/DS/CH/";
                            //}
                            //else if (regsid == 6 && btype == 1)
                            //{
                            //    billformat = "ZB/LD/";
                            //}
                            //else if (regsid == 6 && btype == 2)
                            //{
                            //    billformat = "ZB/DS/";
                            //}
                            ////........end of autonumber
                            ////format = "SUD/IMP/";
                            //format = "IMP/" + Session["GPrxDesc"] + "/";
                            //string prfx = string.Format(format + "{0:D5}", ano);
                            //string billprfx = string.Format(billformat + "{0:D5}", ano);
                            //performatransactionmaster.TRANDNO = prfx.ToString();
                            //performatransactionmaster.TRANBILLREFNO = billprfx.ToString();

                            context.Entry(performatransactionmaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }


                        //-------------performatransaction Details
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
                        string[] TRANDADONAMT = F_Form.GetValues("TRANDADONAMT");
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
                        string[] NOD = F_Form.GetValues("NOD");//days
                        string[] TRANDRATE = F_Form.GetValues("TRANDRATE");
                        string[] BILLEDID = F_Form.GetValues("BILLEDID"); string[] F_BILLEMID = F_Form.GetValues("BILLEMID"); string[] TRANDWGHT = F_Form.GetValues("TRANDWGHT");
                        string[] TRANOTYPE = F_Form.GetValues("perfdetaildata[0].TRANOTYPE");
                        string[] TRAND_COVID_DISC_AMT = F_Form.GetValues("TRAND_COVID_DISC_AMT");

                        var BILLEMID = 0;
                        for (int count = 0; count < F_TRANDID.Count(); count++)
                        {
                            if (boolSTFDIDS[count] == "true")
                            {
                                TRANDID = Convert.ToInt32(F_TRANDID[count]);
                                if (F_BILLEMID[count] == null || F_BILLEMID[count] == "")
                                    BILLEMID = 0;
                                else
                                    BILLEMID = Convert.ToInt32(F_BILLEMID[count]);
                                //  var boolSTFDID = Convert.ToString(boolSTFDIDS[count]);
                                if (TRANDID != 0)
                                {
                                    performatransactiondetail = context.performatransactiondetail.Find(TRANDID);
                                }
                                performatransactiondetail.TRANMID = performatransactionmaster.TRANMID;
                                performatransactiondetail.TRANDREFNO = (TRANDREFNO[count]).ToString();
                                performatransactiondetail.TRANDREFNAME = (TRANDREFNAME[count]).ToString();
                                if (TRANDREFID[count] == "" || TRANDREFID[count] == null)
                                    performatransactiondetail.TRANDREFID = 0;
                                else
                                    performatransactiondetail.TRANDREFID = Convert.ToInt32(TRANDREFID[count]);//GIDID
                                performatransactiondetail.TRANIDATE = Convert.ToDateTime(TRANIDATE[count]);
                                performatransactiondetail.TRANSDATE = Convert.ToDateTime(TRANSDATE[count]);
                                performatransactiondetail.TRANEDATE = Convert.ToDateTime(TRANEDATE[count]);
                                if (F_Form["perfdetaildata[0].TRANVDATE"] != "")
                                    performatransactiondetail.TRANVDATE = Convert.ToDateTime(F_Form["perfdetaildata[0].TRANVDATE"]);
                                else
                                    performatransactiondetail.TRANVDATE = DateTime.Now.Date;
                                performatransactiondetail.TRANDSAMT = Convert.ToDecimal(TRANDSAMT[count]);
                                performatransactiondetail.TRANDHAMT = Convert.ToDecimal(TRANDHAMT[count]);
                                performatransactiondetail.TRANDEAMT = Convert.ToDecimal(TRANDEAMT[count]);
                                performatransactiondetail.TRANDFAMT = Convert.ToDecimal(TRANDFAMT[count]);
                                performatransactiondetail.TRANDAAMT = Convert.ToDecimal(TRANDAAMT[count]);
                                performatransactiondetail.TRANDADONAMT = Convert.ToDecimal(TRANDADONAMT[count]);
                                performatransactiondetail.TRANDNAMT = Convert.ToDecimal(TRANDNAMT[count]); 
                                //  performatransactiondetail.TRANDNOP = Convert.ToDecimal(TRANDNOP[count]);
                                performatransactiondetail.TRANDQTY = Convert.ToDecimal(NOD[count]);//NO.OF DAYS
                                performatransactiondetail.TARIFFMID = Convert.ToInt32(F_Form["perfdetaildata[0].TARIFFMID"]);
                                //  performatransactiondetail.TRANDRATE = 0;
                                performatransactiondetail.TRANDRATE = Convert.ToDecimal(TRANDRATE[count]);//TRANSPORT CHARGE
                                performatransactiondetail.TRANOTYPE = Convert.ToInt16(F_Form["perfdetaildata[0].TRANOTYPE"]);
                                performatransactiondetail.TRANDGAMT = Convert.ToDecimal(TRANDNAMT[count]);
                                if (BILLEDID[count] == "0" || BILLEDID[count] == "" || BILLEDID[count] == null)
                                    performatransactiondetail.BILLEDID = 0;
                                else
                                    performatransactiondetail.BILLEDID = Convert.ToInt32(BILLEDID[count]);
                                performatransactiondetail.RCOL1 = Convert.ToDecimal(RCOL1[count]);
                                performatransactiondetail.RCOL2 = Convert.ToDecimal(RCOL2[count]);
                                performatransactiondetail.RCOL3 = Convert.ToDecimal(RCOL3[count]);
                                performatransactiondetail.RCOL4 = Convert.ToDecimal(RCOL4[count]);
                                performatransactiondetail.RCOL5 = Convert.ToDecimal(RCOL5[count]);
                                performatransactiondetail.RCOL6 = Convert.ToDecimal(RCOL6[count]);
                                performatransactiondetail.RCOL7 = 0;
                                performatransactiondetail.RAMT1 = Convert.ToDecimal(RAMT1[count]);
                                performatransactiondetail.RAMT2 = Convert.ToDecimal(RAMT2[count]);
                                performatransactiondetail.RAMT3 = Convert.ToDecimal(RAMT3[count]);
                                performatransactiondetail.RAMT4 = Convert.ToDecimal(RAMT4[count]);
                                performatransactiondetail.RAMT5 = Convert.ToDecimal(RAMT5[count]);
                                performatransactiondetail.RAMT6 = Convert.ToDecimal(RAMT6[count]);
                                performatransactiondetail.RCAMT1 = Convert.ToDecimal(RCAMT1[count]);
                                performatransactiondetail.RCAMT2 = Convert.ToDecimal(RCAMT2[count]);
                                performatransactiondetail.RCAMT3 = Convert.ToDecimal(RCAMT3[count]);
                                performatransactiondetail.RCAMT4 = Convert.ToDecimal(RCAMT4[count]);
                                performatransactiondetail.RCAMT5 = Convert.ToDecimal(RCAMT5[count]);
                                performatransactiondetail.RCAMT6 = Convert.ToDecimal(RCAMT6[count]);
                                performatransactiondetail.SLABTID = 0;
                                performatransactiondetail.TRANYTYPE = 0;
                                performatransactiondetail.TRANDWGHT = Convert.ToDecimal(TRANDWGHT[count]);
                                performatransactiondetail.TRANDAID = 0;
                                performatransactiondetail.SBDID = 0;
                                performatransactiondetail.TRAND_COVID_DISC_AMT = Convert.ToDecimal(TRAND_COVID_DISC_AMT[count]);

                                if (Convert.ToInt32(TRANDID) == 0)
                                {
                                    context.performatransactiondetail.Add(performatransactiondetail);
                                    context.SaveChanges();
                                    TRANDID = performatransactiondetail.TRANDID;
                                }
                                else
                                {
                                    performatransactiondetail.TRANDID = TRANDID;
                                    context.Entry(performatransactiondetail).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                }//..............end
                                DELIDS = DELIDS + "," + TRANDID.ToString();
                            }
                        }
                        
                        if(BILLEMID >= 0)
                            //context.Database.ExecuteSqlCommand("UPDATE BILLENTRYMASTER SET DPAIDNO='" + F_Form["DPAIDNO"] + "',DPAIDAMT='" + Convert.ToDecimal(F_Form["DPAIDAMT"]) + "'  WHERE BILLEMID=" + BILLEMID);
                        //-------delete performatransaction master factor-------//
                        context.Database.ExecuteSqlCommand("DELETE FROM PERFORMA_TRANSACTIONMASTERFACTOR WHERE tranmid=" + TRANMID);

                        //performatransaction Type Master-------//

                        Performa_TransactionMasterFactor performatransactionmasterfactors = new Performa_TransactionMasterFactor();
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

                                performatransactionmasterfactors.TRANMID = performatransactionmaster.TRANMID;
                                performatransactionmasterfactors.DORDRID = Convert.ToInt16(DORDRID[count2]);
                                performatransactionmasterfactors.DEDMODE = DEDMODE[count2].ToString();
                                performatransactionmasterfactors.DEDVALUE = Convert.ToDecimal(DEDVALUE[count2]);
                                performatransactionmasterfactors.DEDTYPE = Convert.ToInt16(DEDTYPE[count2]);
                                performatransactionmasterfactors.DEDEXPRN = Convert.ToDecimal(DEDEXPRN[count2]);
                                performatransactionmasterfactors.CFID = Convert.ToInt32(TAX1[count2]);
                                performatransactionmasterfactors.DEDCFDESC = CFDESC[count2];
                                performatransactionmasterfactors.DEDNOS = Convert.ToDecimal(DEDNOS[count2]);
                                performatransactionmasterfactors.CFOPTN = 0;
                                performatransactionmasterfactors.DEDORDR = 0;
                                context.performatransactionmasterfactor.Add(performatransactionmasterfactors);
                                context.SaveChanges();
                            }
                        }
                        context.Database.ExecuteSqlCommand("DELETE FROM Performa_TransactionDetail  WHERE TRANMID=" + TRANMID + " and  TRANDID NOT IN(" + DELIDS.Substring(1) + ")");
                        //  Response.Redirect("Index");
                        trans.Commit(); Response.Redirect("Index");
                    }
                    catch (Exception ex)
                    {
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
            //DbSqlQuery<CostFactorMaster> data2 = context.costfactormasters.SqlQuery("select * from costfactormaster  where DISPSTATUS=0 and CFID  in(2,96,97) ");
            //foreach (var cost in data2)
            //{

            //    first_id = cost.CFID.ToString();




            //    //if (i == 0)
            //    //{
            //    first = cost.CFDESC;
            //    f_order = cost.DORDRID.ToString();
            //    f_expr = cost.CFEXPR.ToString();
            //    if (cost.CFMODE != 0)
            //        mod = "selected";
            //    if (cost.CFTYPE != 0)
            //        expr = "selected";
            //    else expr = "";
            //    //   }

            //    // html = html + "<option value='" + cost.CFID + "'>" + cost.CFDESC + "</option>";
            //    html = html + "<tr class='item-row'><Td> <button href='#'   type='button' onclick='del_factor(this)' class='btn btn-danger dfact btn-xs'><i class=glyphicon-minus></i> </button>  </td> <td><input type=text name=TAX id='TAX'  class='hide TAX' value='" + cost.CFID + "'><input type=text name=CFDESC id='CFDESC' class='hidden CFDESC' value='" + cost.CFDESC + "'>" + cost.CFDESC + "";
            //    html = html + "</td> <tD class='col-lg-1' > <select id='CFTYPE' name='CFTYPE' class='CFTYPE' onchange='totalonchange(this)'><option value='0' >Value </option> <option value='1' " + expr + "  >  %</option> </select></td> <td class='col-lg-1'><input type='text' id='DEDNOS' class='DEDNOS' name='DEDNOS' value='0' onchange='totalonchange(this)'  style='width:50px'></td><td class='col-lg-1' > <input onchange='totalonchange(this)' type=text value='" + cost.CFEXPR + "' class='CFEXPR' name='CFEXPR' id='CFEXPR'> </td><td><select onchange='totalonchange(this)' class='CFMODE' id='CFMODE' name='CFMODE'> <option value='0' >  +</option><option value='1' " + mod + " >-</option> </select><input type='text' id='DORDRID'   value='" + cost.DORDRID + "' class='DORDRID' style='display:none'  name='DORDRID' >  <input  type=text value='0' style='display:none' name=TRANMFID id='TRANMFID' class='TRANMFID' >";
            //    html = html + "<input  type=text value='0' style='display:none' name=DEDORDR id='DEDORDR' class='DEDORDR' ><input  type=text value='0' style='display:none' name=TMPCFVAL id='TMPCFVAL' class='TMPCFVAL' ></td><td><input  type=text value='' name=CFAMOUNT id='CFAMOUNT' class='CFAMOUNT' readonly='readonly'> </td>  </TD></tr>";

            //    i++;

            //    //do something with cust
            //}
            return html;


        }
        //--------Autocomplete CHA Name
        public JsonResult AutoCha(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DODetail(int id)
        {
            var query = context.Database.SqlQuery<pr_Import_DO_Invoice_IGMNO_Grid_Assgn_Result>("EXEC pr_Import_DO_Invoice_IGMNO_Grid_Assgn @PDONO=" + id).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public string Detail(string id)
        {
            int tariffmid = 0;
            var param = id.Split('~');
            var igmno = (param[0]); var gplno = param[1]; var TRANMID = Convert.ToInt32(param[2]);
            if (param[3] != "") { tariffmid = Convert.ToInt32((param[3])); } else { tariffmid = 0; };
            context.Database.CommandTimeout = 0;
            var query = context.Database.SqlQuery<pr_Import_Performa_Invoice_IGMNO_Grid_Assgn_Result>("EXEC pr_Import_Performa_Invoice_IGMNO_Grid_Assgn @PIGMNO='" + igmno + "',@PLNO='" + gplno + "',@PTRANMID=" + TRANMID).ToList();

            var tabl = "";
            var count = 0;

            foreach (var rslt in query)
            {
                var st = ""; var bt = "";

                if (rslt.TRANDID != 0) { st = "checked"; bt = "true"; }
                else { bt = "false"; st = ""; }

                if (rslt.GODATE == null) rslt.GODATE = DateTime.Now.Date;
                if (rslt.GOTIME == null) rslt.GOTIME = DateTime.Now;
                if (tariffmid > 6)
                {
                    tabl = tabl + "<tr><td class='hidden1'><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS checked='" + bt + "' onchange=total() style='width:30px'>";
                    tabl = tabl + "<input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value='" + bt + "'></td><td>";
                  
                    tabl = tabl + "<input type=text id=TRANDREFNO value='" + rslt.BOENO + "'  class='TRANDREFNO' readonly='readonly' name=TRANDREFNO style='width:56px'></td>";
                    tabl = tabl + "<td class='col-lg-0'><input type='text' id='TRANDREFNAME' value='" + rslt.CONTNRNO + "'  class='TRANDREFNAME' readonly='readonly' name='TRANDREFNAME' ></td>";
                    tabl = tabl + "<td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class=SIZE name=SIZE style='width:40px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANIDATE value='" + rslt.GIDATE.Value.ToString("dd/MM/yyyy") + "' class=TRANIDATE name=TRANIDATE style='width:70px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANSDATE value='" + rslt.GIDATE.Value.ToString("dd/MM/yyyy") + "' class=TRANSDATE name=TRANSDATE readonly='readonly' style='width:70px'></td><td>";
                    tabl = tabl + "<input type=text id='TRANEDATE' value='" + rslt.GODATE.Value.ToString("dd/MM/yyyy") + "' class='TRANEDATE datepicker' name='TRANEDATE' style='width:70px' onchange='calculation()'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDWGHT value=" + rslt.WGHT + " class=TRANDWGHT name=TRANDWGHT readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDSAMT value='0' class=TRANDSAMT name=TRANDSAMT style='width:70px' readonly='readonly'>";
                    tabl = tabl + "<input type=text id=TRAND_COVID_DISC_AMT value='0' class='TRAND_COVID_DISC_AMT hide' name=TRAND_COVID_DISC_AMT style='width:70px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDHAMT value='0' class=TRANDHAMT name=TRANDHAMT readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td class='hide'><input type=text value='0' id=TRANDEAMT class=TRANDEAMT name=TRANDEAMT  readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text value='0' id=TRANDFAMT class=TRANDFAMT name=TRANDFAMT  readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDAAMT value='0' class=TRANDAAMT name=TRANDAAMT   readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDADONAMT value='0' class=TRANDADONAMT name=TRANDADONAMT   style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text value='0' id=TRANDNAMT class=TRANDNAMT name=TRANDNAMT readonly='readonly' style='width:70px'></td>";
                    if(rslt.DODATE == null)
                        tabl = tabl + "<td class=hide><input type=text id=TRANDVDATE class=TRANDVDATE name=TRANDVDATE value='" + DateTime.Now.Date.ToString("dd/MM/yyyy") + "'>";
                    else
                        tabl = tabl + "<td class=hide><input type=text id=TRANDVDATE class=TRANDVDATE name=TRANDVDATE value='" + rslt.DODATE.Value.ToString("dd/MM/yyyy") + "'>";
                    tabl = tabl + "<input type=text id=F_DPAIDNO value='" + rslt.DPAIDNO + "'  class='F_DPAIDNO' name=F_DPAIDNO >";
                    tabl = tabl + "<input type=text id=F_DPAIDAMT value='" + rslt.DPAIDAMT + "'  class=F_DPAIDAMT name=F_DPAIDAMT ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=CONTNRTID value=0  class=CONTNRTID name=CONTNRTID >";
                    tabl = tabl + "<input type=text id=CONTNRSID value='" + rslt.CONTNRSID + "'  class=CONTNRSID name=CONTNRSID >";
                    tabl = tabl + "<input type=text id=TRANDREFID value='" + rslt.GIDID + "'  class=TRANDREFID name=TRANDREFID >";
                    tabl = tabl + "<input type=text id=BILLEDID value='" + rslt.BILLEDID + "'  class=BILLEDID name=BILLEDID >";
                    tabl = tabl + "<input type=text id=BILLEMID value='" + rslt.BILLEMID + "'  class=BILLEMID name=BILLEMID >";
                    tabl = tabl + "<input type=text id=F_TARIFFMID value='" + rslt.TARIFFMID + "'  class=F_TARIFFMID name=F_TARIFFMID >";
                    tabl = tabl + "<input type=text id=F_TRANDOTYPE value='" + rslt.TRANDOTYPE + "'  class=F_TRANDOTYPE name=F_TRANDOTYPE >";
                    tabl = tabl + "<input type=text id=F_CHANAME value='" + rslt.TRANREFNAME + "'  class=F_CHANAME name=F_CHANAME >";
                    tabl = tabl + "<input type=text id=F_STMRNAME value='" + rslt.STMRNAME + "'  class=F_STMRNAME name=F_STMRNAME >";
                    tabl = tabl + "<input type=text id=F_CHAID value='" + rslt.TRANREFID + "'  class=F_CHAID name=F_CHAID >";
                    tabl = tabl + "<input type=text id=F_STMRID value='" + rslt.STMRID + "'  class=F_STMRID name=F_STMRID ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=TRANDID value=0  class=TRANDID name=TRANDID ></td>";
                    tabl = tabl + "<td class='hide'><input type=text id=days value=0  class=days name=days >";
                    tabl = tabl + "<input type=text id=NOD value='" + rslt.NOD + "'  class=NOD name=NOD >";
                    tabl = tabl + "<input type=text id=TRANDRATE value='" + rslt.TRANDRATE + "'  class=TRANDRATE name=TRANDRATE ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=RAMT7 value=" + Convert.ToDecimal(rslt.RAMT7) + "  class=RAMT7 name=RAMT7 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT1 value=" + Convert.ToDecimal(rslt.RAMT1) + "  class=RAMT1 name=RAMT1 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT2 value=" + Convert.ToDecimal(rslt.RAMT2) + "  class=RAMT2 name=RAMT2 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT3 value=" + Convert.ToDecimal(rslt.RAMT3) + "  class=RAMT3 name=RAMT3 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT4 value=" + Convert.ToDecimal(rslt.RAMT4) + "  class=RAMT4 name=RAMT4 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT5 value=" + Convert.ToDecimal(rslt.RAMT5) + "  class=RAMT5 name=RAMT5 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT6 value=" + Convert.ToDecimal(rslt.RAMT6) + " class=RAMT6 name=RAMT6 style='display:none' ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=SLABMIN6 value='0'  class=SLABMIN6 name=SLABMIN6 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMAX6 value='0'  class=SLABMAX6 name=SLABMAX6 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMIN value='0'  class=SLABMIN name=SLABMIN style='display:none' >";
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
                    tabl = tabl + "<input type=text id=SLABMAX5 value='0'  class=SLABMAX5 name=SLABMAX5 style='display:none' > </td>";
                    tabl = tabl + "<td class=hide> <input type=text id=RCAMT7 value=value=" + Convert.ToDecimal(rslt.RCAMT7) + "  class=RCAMT7 name=RCAMT7 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT1 value=value=" + Convert.ToDecimal(rslt.RCAMT1) + "  class=RCAMT1 name=RCAMT1 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT2 value=" + Convert.ToDecimal(rslt.RCAMT2) + "  class=RCAMT2 name=RCAMT2 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT3 value=" + Convert.ToDecimal(rslt.RCAMT3) + "  class=RCAMT3 name=RCAMT3 style='display:none'>";
                    tabl = tabl + "<input type=text id=RCAMT4 value=" + Convert.ToDecimal(rslt.RCAMT4) + "  class=RCAMT4 name=RCAMT4 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT5 value=" + Convert.ToDecimal(rslt.RCAMT5) + "  class=RCAMT5 name=RCAMT5 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT6 value=" + Convert.ToDecimal(rslt.RCAMT6) + "  class=RCAMT6 name=RCAMT6 style='display:none' ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=RCOL7 value=" + Convert.ToDecimal(rslt.RCOL7) + "  class=RCOL7 name=RCOL7 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL1 value=" + Convert.ToDecimal(rslt.RCOL1) + "  class=RCOL1 name=RCOL1 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL2 value=" + Convert.ToDecimal(rslt.RCOL2) + " class=RCOL2 name=RCOL2 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL3 value=" + Convert.ToDecimal(rslt.RCOL3) + "  class=RCOL3 name=RCOL3 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL4 value=" + Convert.ToDecimal(rslt.RCOL4) + "  class=RCOL4 name=RCOL4 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL5 value=" + Convert.ToDecimal(rslt.RCOL5) + " class=RCOL5 name=RCOL5 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL6 value=" + Convert.ToDecimal(rslt.RCOL6) + "  class=RCOL6 name=RCOL6 style='display:none' ></td>";
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
                else
                {
                    //tabl = tabl + "<tr><td class='hidden1'><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS value='' " + count + "  onchange=total() style='width:30px'>";
                    //tabl = tabl + "<input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value=''></td><td>";

                    tabl = tabl + "<tr><td class='hidden1'><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS value='" + st + "' checked='" + bt + "' onchange=total() style='width:30px'>";
                    tabl = tabl + "<input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value='" + bt + "'></td><td>";


                    tabl = tabl + "<input type=text id=TRANDREFNO value='" + rslt.BOENO + "'  class='TRANDREFNO' readonly='readonly' name=TRANDREFNO style='width:56px'></td>";
                    tabl = tabl + "<td class='col-lg-0'><input type='text' id='TRANDREFNAME' value='" + rslt.CONTNRNO + "'  class='TRANDREFNAME' readonly='readonly' name='TRANDREFNAME' ></td>";
                    tabl = tabl + "<td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class=SIZE name=SIZE style='width:40px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANIDATE value='" + rslt.GIDATE.Value.ToString("dd/MM/yyyy") + "' class=TRANIDATE name=TRANIDATE style='width:70px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANSDATE value='" + rslt.GIDATE.Value.ToString("dd/MM/yyyy") + "' class=TRANSDATE name=TRANSDATE readonly='readonly' style='width:70px'></td><td>";
                    tabl = tabl + "<input type=text id='TRANEDATE' value='" + rslt.GODATE.Value.ToString("dd/MM/yyyy") + "' class='TRANEDATE datepicker' name='TRANEDATE' style='width:70px' onchange='calculation()'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDWGHT value=" + rslt.WGHT + " class=TRANDWGHT name=TRANDWGHT readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDSAMT value='0' class=TRANDSAMT name=TRANDSAMT style='width:70px' readonly='readonly'>";
                    tabl = tabl + "<input type=text id=TRAND_COVID_DISC_AMT value='0' class='TRAND_COVID_DISC_AMT hide' name=TRAND_COVID_DISC_AMT style='width:70px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDHAMT value='0' class=TRANDHAMT name=TRANDHAMT readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td class='hide'><input type=text value='0' id=TRANDEAMT class=TRANDEAMT name=TRANDEAMT  readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text value='0' id=TRANDFAMT class=TRANDFAMT name=TRANDFAMT  readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDAAMT value='0' class=TRANDAAMT name=TRANDAAMT   readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDADONAMT  value='0' class=TRANDADONAMT name=TRANDADONAMT   style='width:70px' onchange=total()></td>";
                    tabl = tabl + "<td><input type=text value='0' id=TRANDNAMT class=TRANDNAMT name=TRANDNAMT readonly='readonly' style='width:70px'></td>";
                    if (rslt.DODATE == null)
                    {
                        tabl = tabl + "<td class=hide><input type=text id=TRANDVDATE class=TRANDVDATE name=TRANDVDATE value=''>";
                    }
                    else
                    {
                        tabl = tabl + "<td class=hide><input type=text id=TRANDVDATE class=TRANDVDATE name=TRANDVDATE value='" + rslt.DODATE.Value.ToString("dd/MM/yyyy") + "'>";
                    }
                    
                    tabl = tabl + "<input type=text id=F_DPAIDNO value='" + rslt.DPAIDNO + "'  class='F_DPAIDNO' name=F_DPAIDNO >";
                    tabl = tabl + "<input type=text id=F_DPAIDAMT value=" + rslt.DPAIDAMT + "  class=F_DPAIDAMT name=F_DPAIDAMT ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=CONTNRTID value=0  class=CONTNRTID name=CONTNRTID >";
                    tabl = tabl + "<input type=text id=CONTNRSID value=" + rslt.CONTNRSID + "  class=CONTNRSID name=CONTNRSID >";
                    tabl = tabl + "<input type=text id=TRANDREFID value='" + rslt.GIDID + "'  class=TRANDREFID name=TRANDREFID >";
                    tabl = tabl + "<input type=text id=BILLEDID value='" + rslt.BILLEDID + "'  class=BILLEDID name=BILLEDID >";
                    tabl = tabl + "<input type=text id=BILLEMID value='" + rslt.BILLEMID + "'  class=BILLEMID name=BILLEMID >";
                    tabl = tabl + "<input type=text id=F_TARIFFMID value='" + rslt.TARIFFMID + "'  class=F_TARIFFMID name=F_TARIFFMID >";
                    tabl = tabl + "<input type=text id=F_TRANDOTYPE value=" + rslt.TRANDOTYPE + "  class=F_TRANDOTYPE name=F_TRANDOTYPE >";
                    tabl = tabl + "<input type=text id=F_CHANAME value='" + rslt.TRANREFNAME + "'  class=F_CHANAME name=F_CHANAME >";
                    tabl = tabl + "<input type=text id=F_STMRNAME value='" + rslt.STMRNAME + "'  class=F_STMRNAME name=F_STMRNAME >";
                    tabl = tabl + "<input type=text id=F_CHAID value=" + rslt.TRANREFID + "  class=F_CHAID name=F_CHAID >";
                    tabl = tabl + "<input type=text id=F_STMRID value=" + rslt.STMRID + "  class=F_STMRID name=F_STMRID ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=TRANDID value=0  class=TRANDID name=TRANDID ></td>";
                    tabl = tabl + "<td class='hide'><input type=text id=days value=0  class=days name=days >";
                    tabl = tabl + "<input type=text id=NOD value='" + rslt.NOD + "'  class=NOD name=NOD >";
                    tabl = tabl + "<input type=text id=TRANDRATE value='" + rslt.TRANDRATE + "'  class=TRANDRATE name=TRANDRATE ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=RAMT7 value=" + Convert.ToDecimal(rslt.RAMT7) + "  class=RAMT7 name=RAMT7 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT1 value=" + Convert.ToDecimal(rslt.RAMT1) + "  class=RAMT1 name=RAMT1 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT2 value=" + Convert.ToDecimal(rslt.RAMT2) + "  class=RAMT2 name=RAMT2 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT3 value=" + Convert.ToDecimal(rslt.RAMT3) + "  class=RAMT3 name=RAMT3 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT4 value=" + Convert.ToDecimal(rslt.RAMT4) + "  class=RAMT4 name=RAMT4 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT5 value=" + Convert.ToDecimal(rslt.RAMT5) + "  class=RAMT5 name=RAMT5 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT6 value=" + Convert.ToDecimal(rslt.RAMT6) + " class=RAMT6 name=RAMT6 style='display:none' ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=SLABMIN6 value='0'  class=SLABMIN6 name=SLABMIN6 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMAX6 value='0'  class=SLABMAX6 name=SLABMAX6 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMIN value='0'  class=SLABMIN name=SLABMIN style='display:none' >";
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
                    tabl = tabl + "<input type=text id=SLABMAX5 value='0'  class=SLABMAX5 name=SLABMAX5 style='display:none' > </td>";
                    tabl = tabl + "<td class=hide> <input type=text id=RCAMT7 value=value=" + Convert.ToDecimal(rslt.RCAMT7) + "  class=RCAMT7 name=RCAMT7 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT1 value=value=" + Convert.ToDecimal(rslt.RCAMT1) + "  class=RCAMT1 name=RCAMT1 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT2 value=" + Convert.ToDecimal(rslt.RCAMT2) + "  class=RCAMT2 name=RCAMT2 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT3 value=" + Convert.ToDecimal(rslt.RCAMT3) + "  class=RCAMT3 name=RCAMT3 style='display:none'>";
                    tabl = tabl + "<input type=text id=RCAMT4 value=" + Convert.ToDecimal(rslt.RCAMT4) + "  class=RCAMT4 name=RCAMT4 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT5 value=" + Convert.ToDecimal(rslt.RCAMT5) + "  class=RCAMT5 name=RCAMT5 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT6 value=" + Convert.ToDecimal(rslt.RCAMT6) + "  class=RCAMT6 name=RCAMT6 style='display:none' ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=RCOL7 value=" + Convert.ToDecimal(rslt.RCOL7) + "  class=RCOL7 name=RCOL7 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL1 value=" + Convert.ToDecimal(rslt.RCOL1) + "  class=RCOL1 name=RCOL1 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL2 value=" + Convert.ToDecimal(rslt.RCOL2) + " class=RCOL2 name=RCOL2 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL3 value=" + Convert.ToDecimal(rslt.RCOL3) + "  class=RCOL3 name=RCOL3 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL4 value=" + Convert.ToDecimal(rslt.RCOL4) + "  class=RCOL4 name=RCOL4 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL5 value=" + Convert.ToDecimal(rslt.RCOL5) + " class=RCOL5 name=RCOL5 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL6 value=" + Convert.ToDecimal(rslt.RCOL6) + "  class=RCOL6 name=RCOL6 style='display:none' ></td>";
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

        public string Detail2(string id)
        {
            int tariffmid = 0;
            var param = id.Split('~');
            var igmno = (param[0]); var gplno = param[1]; var TRANMID = Convert.ToInt32(param[2]);
            if (param[3] != "") { tariffmid = Convert.ToInt32((param[3])); } else { tariffmid = 0; };

            var query = context.Database.SqlQuery<pr_Import_Performa_Invoice_IGMNO_Grid_Assgn_Result>("EXEC pr_Import_Performa_Invoice_IGMNO_Grid_Assgn @PIGMNO='" + igmno + "',@PLNO='" + gplno + "',@PTRANMID=" + TRANMID).ToList();

            var tabl = "";
            var count = 0;

            foreach (var rslt in query)
            {
                var st = ""; var bt = "";

                if (rslt.TRANDID != 0) { st = "checked"; bt = "true"; }
                else { bt = "false"; st = ""; }

                if (rslt.GODATE == null) rslt.GODATE = DateTime.Now.Date;
                if (rslt.GOTIME == null) rslt.GOTIME = DateTime.Now;
                if (tariffmid > 6)
                {
                    tabl = tabl + "<tr><td class='hidden1'><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS checked='" + bt + "' onchange=total() style='width:30px'>";
                    tabl = tabl + "<input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value='" + bt + "'></td><td>";

                    tabl = tabl + "<input type=text id=TRANDREFNO value='" + rslt.BOENO + "'  class='TRANDREFNO' readonly='readonly' name=TRANDREFNO style='width:56px'></td>";
                    tabl = tabl + "<td class='col-lg-0'><input type='text' id='TRANDREFNAME' value='" + rslt.CONTNRNO + "'  class='TRANDREFNAME' readonly='readonly' name='TRANDREFNAME' ></td>";
                    tabl = tabl + "<td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class=SIZE name=SIZE style='width:40px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANIDATE value='" + rslt.GIDATE.Value.ToString("dd/MM/yyyy") + "' class=TRANIDATE name=TRANIDATE style='width:70px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANSDATE value='" + rslt.GIDATE.Value.ToString("dd/MM/yyyy") + "' class=TRANSDATE name=TRANSDATE readonly='readonly' style='width:70px'></td><td>";
                    tabl = tabl + "<input type=text id='TRANEDATE' value='" + rslt.GODATE.Value.ToString("dd/MM/yyyy") + "' class='TRANEDATE datepicker' name='TRANEDATE' style='width:70px' onchange='calculation()'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDWGHT value=" + rslt.WGHT + " class=TRANDWGHT name=TRANDWGHT readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDSAMT value='0' class=TRANDSAMT name=TRANDSAMT style='width:70px' readonly='readonly'>";
                    tabl = tabl + "<input type=text id=TRAND_COVID_DISC_AMT value='0' class='TRAND_COVID_DISC_AMT hide' name=TRAND_COVID_DISC_AMT style='width:70px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDHAMT value='0' class=TRANDHAMT name=TRANDHAMT readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td class='hide'><input type=text value='0' id=TRANDEAMT class=TRANDEAMT name=TRANDEAMT  readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text value='0' id=TRANDFAMT class=TRANDFAMT name=TRANDFAMT  readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDAAMT value='0' class=TRANDAAMT name=TRANDAAMT   readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDADONAMT value='0' class=TRANDADONAMT name=TRANDADONAMT   style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text value='0' id=TRANDNAMT class=TRANDNAMT name=TRANDNAMT readonly='readonly' style='width:70px'></td>";
                    tabl = tabl + "<td class=hide><input type=text id=TRANDVDATE class=TRANDVDATE name=TRANDVDATE value='" + rslt.DODATE.Value.ToString("dd/MM/yyyy") + "'>";
                    tabl = tabl + "<input type=text id=F_DPAIDNO value='" + rslt.DPAIDNO + "'  class='F_DPAIDNO' name=F_DPAIDNO >";
                    tabl = tabl + "<input type=text id=F_DPAIDAMT value='" + rslt.DPAIDAMT + "'  class=F_DPAIDAMT name=F_DPAIDAMT ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=CONTNRTID value=0  class=CONTNRTID name=CONTNRTID >";
                    tabl = tabl + "<input type=text id=CONTNRSID value='" + rslt.CONTNRSID + "'  class=CONTNRSID name=CONTNRSID >";
                    tabl = tabl + "<input type=text id=TRANDREFID value='" + rslt.GIDID + "'  class=TRANDREFID name=TRANDREFID >";
                    tabl = tabl + "<input type=text id=BILLEDID value='" + rslt.BILLEDID + "'  class=BILLEDID name=BILLEDID >";
                    tabl = tabl + "<input type=text id=BILLEMID value='" + rslt.BILLEMID + "'  class=BILLEMID name=BILLEMID >";
                    tabl = tabl + "<input type=text id=F_TARIFFMID value='" + rslt.TARIFFMID + "'  class=F_TARIFFMID name=F_TARIFFMID >";
                    tabl = tabl + "<input type=text id=F_TRANDOTYPE value='" + rslt.TRANDOTYPE + "'  class=F_TRANDOTYPE name=F_TRANDOTYPE >";
                    tabl = tabl + "<input type=text id=F_CHANAME value='" + rslt.TRANREFNAME + "'  class=F_CHANAME name=F_CHANAME >";
                    tabl = tabl + "<input type=text id=F_STMRNAME value='" + rslt.STMRNAME + "'  class=F_STMRNAME name=F_STMRNAME >";
                    tabl = tabl + "<input type=text id=F_CHAID value='" + rslt.TRANREFID + "'  class=F_CHAID name=F_CHAID >";
                    tabl = tabl + "<input type=text id=F_STMRID value='" + rslt.STMRID + "'  class=F_STMRID name=F_STMRID ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=TRANDID value=0  class=TRANDID name=TRANDID ></td>";
                    tabl = tabl + "<td class='hide'><input type=text id=days value=0  class=days name=days >";
                    tabl = tabl + "<input type=text id=NOD value='" + rslt.NOD + "'  class=NOD name=NOD >";
                    tabl = tabl + "<input type=text id=TRANDRATE value='" + rslt.TRANDRATE + "'  class=TRANDRATE name=TRANDRATE ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=RAMT7 value=" + Convert.ToDecimal(rslt.RAMT7) + "  class=RAMT7 name=RAMT7 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT1 value=" + Convert.ToDecimal(rslt.RAMT1) + "  class=RAMT1 name=RAMT1 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT2 value=" + Convert.ToDecimal(rslt.RAMT2) + "  class=RAMT2 name=RAMT2 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT3 value=" + Convert.ToDecimal(rslt.RAMT3) + "  class=RAMT3 name=RAMT3 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT4 value=" + Convert.ToDecimal(rslt.RAMT4) + "  class=RAMT4 name=RAMT4 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT5 value=" + Convert.ToDecimal(rslt.RAMT5) + "  class=RAMT5 name=RAMT5 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT6 value=" + Convert.ToDecimal(rslt.RAMT6) + " class=RAMT6 name=RAMT6 style='display:none' ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=SLABMIN6 value='0'  class=SLABMIN6 name=SLABMIN6 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMAX6 value='0'  class=SLABMAX6 name=SLABMAX6 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMIN value='0'  class=SLABMIN name=SLABMIN style='display:none' >";
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
                    tabl = tabl + "<input type=text id=SLABMAX5 value='0'  class=SLABMAX5 name=SLABMAX5 style='display:none' > </td>";
                    tabl = tabl + "<td class=hide> <input type=text id=RCAMT7 value=value=" + Convert.ToDecimal(rslt.RCAMT7) + "  class=RCAMT7 name=RCAMT7 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT1 value=value=" + Convert.ToDecimal(rslt.RCAMT1) + "  class=RCAMT1 name=RCAMT1 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT2 value=" + Convert.ToDecimal(rslt.RCAMT2) + "  class=RCAMT2 name=RCAMT2 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT3 value=" + Convert.ToDecimal(rslt.RCAMT3) + "  class=RCAMT3 name=RCAMT3 style='display:none'>";
                    tabl = tabl + "<input type=text id=RCAMT4 value=" + Convert.ToDecimal(rslt.RCAMT4) + "  class=RCAMT4 name=RCAMT4 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT5 value=" + Convert.ToDecimal(rslt.RCAMT5) + "  class=RCAMT5 name=RCAMT5 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT6 value=" + Convert.ToDecimal(rslt.RCAMT6) + "  class=RCAMT6 name=RCAMT6 style='display:none' ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=RCOL7 value=" + Convert.ToDecimal(rslt.RCOL7) + "  class=RCOL7 name=RCOL7 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL1 value=" + Convert.ToDecimal(rslt.RCOL1) + "  class=RCOL1 name=RCOL1 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL2 value=" + Convert.ToDecimal(rslt.RCOL2) + " class=RCOL2 name=RCOL2 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL3 value=" + Convert.ToDecimal(rslt.RCOL3) + "  class=RCOL3 name=RCOL3 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL4 value=" + Convert.ToDecimal(rslt.RCOL4) + "  class=RCOL4 name=RCOL4 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL5 value=" + Convert.ToDecimal(rslt.RCOL5) + " class=RCOL5 name=RCOL5 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL6 value=" + Convert.ToDecimal(rslt.RCOL6) + "  class=RCOL6 name=RCOL6 style='display:none' ></td>";
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
                else
                {
                    //tabl = tabl + "<tr><td class='hidden1'><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS value='' " + count + "  onchange=total() style='width:30px'>";
                    //tabl = tabl + "<input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value=''></td><td>";

                    tabl = tabl + "<tr><td class='hidden1'><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS value='" + st + "' checked='" + bt + "' onchange=total() style='width:30px'>";
                    tabl = tabl + "<input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value='" + bt + "'></td><td>";

                    tabl = tabl + "<input type=text id=TRANDREFNO value='" + rslt.BOENO + "'  class='TRANDREFNO' readonly='readonly' name=TRANDREFNO style='width:56px'></td>";
                    tabl = tabl + "<td class='col-lg-0'><input type='text' id='TRANDREFNAME' value='" + rslt.CONTNRNO + "'  class='TRANDREFNAME' readonly='readonly' name='TRANDREFNAME' ></td>";
                    tabl = tabl + "<td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class=SIZE name=SIZE style='width:40px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANIDATE value='" + rslt.GIDATE.Value.ToString("dd/MM/yyyy") + "' class=TRANIDATE name=TRANIDATE style='width:70px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANSDATE value='" + rslt.GIDATE.Value.ToString("dd/MM/yyyy") + "' class=TRANSDATE name=TRANSDATE readonly='readonly' style='width:70px'></td><td>";
                    tabl = tabl + "<input type=text id='TRANEDATE' value='" + rslt.GODATE.Value.ToString("dd/MM/yyyy") + "' class='TRANEDATE datepicker' name='TRANEDATE' style='width:70px' onchange='calculation()'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDWGHT value=" + rslt.WGHT + " class=TRANDWGHT name=TRANDWGHT readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDSAMT value='0' class=TRANDSAMT name=TRANDSAMT style='width:70px' readonly='readonly'>";
                    tabl = tabl + "<input type=text id=TRAND_COVID_DISC_AMT value='0' class='TRAND_COVID_DISC_AMT hide' name=TRAND_COVID_DISC_AMT style='width:70px' readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDHAMT value='0' class=TRANDHAMT name=TRANDHAMT readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td class='hide'><input type=text value='0' id=TRANDEAMT class=TRANDEAMT name=TRANDEAMT  readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text value='0' id=TRANDFAMT class=TRANDFAMT name=TRANDFAMT  readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDAAMT value='0' class=TRANDAAMT name=TRANDAAMT   readonly=readonly style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDADONAMT  value='0' class=TRANDADONAMT name=TRANDADONAMT   style='width:70px' onchange=total()></td>";
                    tabl = tabl + "<td><input type=text value='0' id=TRANDNAMT class=TRANDNAMT name=TRANDNAMT readonly='readonly' style='width:70px'></td>";
                    if (rslt.DODATE == null)
                    {
                        tabl = tabl + "<td class=hide><input type=text id=TRANDVDATE class=TRANDVDATE name=TRANDVDATE value=''>";
                    }
                    else
                    {
                        tabl = tabl + "<td class=hide><input type=text id=TRANDVDATE class=TRANDVDATE name=TRANDVDATE value='" + rslt.DODATE.Value.ToString("dd/MM/yyyy") + "'>";
                    }

                    tabl = tabl + "<input type=text id=F_DPAIDNO value='" + rslt.DPAIDNO + "'  class='F_DPAIDNO' name=F_DPAIDNO >";
                    tabl = tabl + "<input type=text id=F_DPAIDAMT value=" + rslt.DPAIDAMT + "  class=F_DPAIDAMT name=F_DPAIDAMT ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=CONTNRTID value=0  class=CONTNRTID name=CONTNRTID >";
                    tabl = tabl + "<input type=text id=CONTNRSID value=" + rslt.CONTNRSID + "  class=CONTNRSID name=CONTNRSID >";
                    tabl = tabl + "<input type=text id=TRANDREFID value='" + rslt.GIDID + "'  class=TRANDREFID name=TRANDREFID >";
                    tabl = tabl + "<input type=text id=BILLEDID value='" + rslt.BILLEDID + "'  class=BILLEDID name=BILLEDID >";
                    tabl = tabl + "<input type=text id=BILLEMID value='" + rslt.BILLEMID + "'  class=BILLEMID name=BILLEMID >";
                    tabl = tabl + "<input type=text id=F_TARIFFMID value='" + rslt.TARIFFMID + "'  class=F_TARIFFMID name=F_TARIFFMID >";
                    tabl = tabl + "<input type=text id=F_TRANDOTYPE value=" + rslt.TRANDOTYPE + "  class=F_TRANDOTYPE name=F_TRANDOTYPE >";
                    tabl = tabl + "<input type=text id=F_CHANAME value='" + rslt.TRANREFNAME + "'  class=F_CHANAME name=F_CHANAME >";
                    tabl = tabl + "<input type=text id=F_STMRNAME value='" + rslt.STMRNAME + "'  class=F_STMRNAME name=F_STMRNAME >";
                    tabl = tabl + "<input type=text id=F_CHAID value=" + rslt.TRANREFID + "  class=F_CHAID name=F_CHAID >";
                    tabl = tabl + "<input type=text id=F_STMRID value=" + rslt.STMRID + "  class=F_STMRID name=F_STMRID ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=TRANDID value=0  class=TRANDID name=TRANDID ></td>";
                    tabl = tabl + "<td class='hide'><input type=text id=days value=0  class=days name=days >";
                    tabl = tabl + "<input type=text id=NOD value='" + rslt.NOD + "'  class=NOD name=NOD >";
                    tabl = tabl + "<input type=text id=TRANDRATE value='" + rslt.TRANDRATE + "'  class=TRANDRATE name=TRANDRATE ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=RAMT7 value=" + Convert.ToDecimal(rslt.RAMT7) + "  class=RAMT7 name=RAMT7 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT1 value=" + Convert.ToDecimal(rslt.RAMT1) + "  class=RAMT1 name=RAMT1 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT2 value=" + Convert.ToDecimal(rslt.RAMT2) + "  class=RAMT2 name=RAMT2 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT3 value=" + Convert.ToDecimal(rslt.RAMT3) + "  class=RAMT3 name=RAMT3 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT4 value=" + Convert.ToDecimal(rslt.RAMT4) + "  class=RAMT4 name=RAMT4 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT5 value=" + Convert.ToDecimal(rslt.RAMT5) + "  class=RAMT5 name=RAMT5 style='display:none' >";
                    tabl = tabl + "<input type=text id=RAMT6 value=" + Convert.ToDecimal(rslt.RAMT6) + " class=RAMT6 name=RAMT6 style='display:none' ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=SLABMIN6 value='0'  class=SLABMIN6 name=SLABMIN6 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMAX6 value='0'  class=SLABMAX6 name=SLABMAX6 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMIN value='0'  class=SLABMIN name=SLABMIN style='display:none' >";
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
                    tabl = tabl + "<input type=text id=SLABMAX5 value='0'  class=SLABMAX5 name=SLABMAX5 style='display:none' > </td>";
                    tabl = tabl + "<td class=hide> <input type=text id=RCAMT7 value=value=" + Convert.ToDecimal(rslt.RCAMT7) + "  class=RCAMT7 name=RCAMT7 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT1 value=value=" + Convert.ToDecimal(rslt.RCAMT1) + "  class=RCAMT1 name=RCAMT1 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT2 value=" + Convert.ToDecimal(rslt.RCAMT2) + "  class=RCAMT2 name=RCAMT2 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT3 value=" + Convert.ToDecimal(rslt.RCAMT3) + "  class=RCAMT3 name=RCAMT3 style='display:none'>";
                    tabl = tabl + "<input type=text id=RCAMT4 value=" + Convert.ToDecimal(rslt.RCAMT4) + "  class=RCAMT4 name=RCAMT4 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT5 value=" + Convert.ToDecimal(rslt.RCAMT5) + "  class=RCAMT5 name=RCAMT5 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT6 value=" + Convert.ToDecimal(rslt.RCAMT6) + "  class=RCAMT6 name=RCAMT6 style='display:none' ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=RCOL7 value=" + Convert.ToDecimal(rslt.RCOL7) + "  class=RCOL7 name=RCOL7 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL1 value=" + Convert.ToDecimal(rslt.RCOL1) + "  class=RCOL1 name=RCOL1 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL2 value=" + Convert.ToDecimal(rslt.RCOL2) + " class=RCOL2 name=RCOL2 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL3 value=" + Convert.ToDecimal(rslt.RCOL3) + "  class=RCOL3 name=RCOL3 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL4 value=" + Convert.ToDecimal(rslt.RCOL4) + "  class=RCOL4 name=RCOL4 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL5 value=" + Convert.ToDecimal(rslt.RCOL5) + " class=RCOL5 name=RCOL5 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL6 value=" + Convert.ToDecimal(rslt.RCOL6) + "  class=RCOL6 name=RCOL6 style='display:none' ></td>";
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

        //.............................storage ,handling,energy,fuel, and PTI amount...........//
        public JsonResult Bill_Detail(string id)
        {
            var param = id.Split('-');

            var TARIFFMID = 0;
            var CONTNRSID = 0;
            var STMRID = 0;
            string WGHT = "0";
            var CHRGETYPE = 0;
            var htype = 0;
            var handlng = 0;
            if (param.Length>0)
            {
                if (param[0] != "" && param[0] != "null" && param[0] != null) { TARIFFMID = Convert.ToInt32(param[0]); } else { TARIFFMID = 0; };
                
                if (param[1] != "null" && param[1] != "")
                    CHRGETYPE = Convert.ToInt32(param[1]);
                if (param[2] != "null" && param[2] != "") { CONTNRSID = Convert.ToInt32(param[2]); } else { CONTNRSID = 0; };  //var CONTNRSID = Convert.ToInt32(param[2]);
                if (param[3] != "null" && param[3] != "") { STMRID = Convert.ToInt32(param[3]); } else { STMRID = 0; };  //var STMRID = Convert.ToInt32(param[3]);/* INSTEAD OF SLABTID=5 ,,PARAM[5]*/
                                                                                                   //if (param[5] != "") { WGHT = Convert.ToInt32(param[5]); } else { WGHT = 0; };
                if (param[5] != "null" && param[5] != "") { WGHT = param[5]; } else { WGHT = "0"; };
                if (param[1] != "null" && param[1] != "") { handlng = Convert.ToInt32(param[1]); }
                if (param[4] != "null" && param[4] != null && param[4] != "") { htype = Convert.ToInt32(param[4]); } else { htype = 1; }

            }

            WGHT = WGHT.Replace(',', '.');
            //Response.Write(strqty);
            decimal aqty = Convert.ToDecimal(WGHT);

            
            //if (param[1] == "1") { handlng = 3; }
            //if (param[1] == "2") { handlng = 4; }
              if (TARIFFMID == 4)
            {
                //var query = context.Database.SqlQuery<VW_IMPORT_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_IMPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (" + handlng + ",14,15,16)and HTYPE=" + Convert.ToInt32(param[4]) + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + "and STMRID=" + STMRID).ToList();
                var query = context.Database.SqlQuery<VW_IMPORT_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_IMPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (" + handlng + ",14,15,16)and HTYPE=" + Convert.ToInt32(htype) + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + "and STMRID=" + STMRID).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var query = context.Database.SqlQuery<VW_IMPORT_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_IMPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (" + handlng + ",14,15,16)and HTYPE=" + Convert.ToInt32(param[4]) + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and (SLABMIN <= " + WGHT + " or SLABMIN >= " + WGHT + ")").ToList();
                //var query = context.Database.SqlQuery<PR_NEW_IMPORT_RATECARDMASTER_FLX_ASSGN_Result>("EXEC PR_NEW_IMPORT_RATECARDMASTER_FLX_ASSGN @PTARIFFMID=" + TARIFFMID + ", @PSLABTID=" + handlng + ", @PHTYPE=" + Convert.ToInt32(param[4]) + ", @PCHRGETYPE = " + CHRGETYPE + ", @PCONTNRSID = " + CONTNRSID + ", @PSLABMIN = " + aqty).ToList();
                var query = context.Database.SqlQuery<PR_NEW_IMPORT_RATECARDMASTER_FLX_ASSGN_Result>("EXEC PR_NEW_IMPORT_RATECARDMASTER_FLX_ASSGN @PTARIFFMID=" + TARIFFMID + ", @PSLABTID=" + handlng + ", @PHTYPE=" + Convert.ToInt32(htype) + ", @PCHRGETYPE = " + CHRGETYPE + ", @PCONTNRSID = " + CONTNRSID + ", @PSLABMIN = " + aqty).ToList();
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
                var query = context.Database.SqlQuery<VW_IMPORT_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_IMPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (" + handlng + ",14,15,16)and HTYPE=" + Convert.ToInt32(param[4]) + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + "and STMRID=" + STMRID).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var query = context.Database.SqlQuery<VW_IMPORT_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_IMPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (" + handlng + ",14,15,16)and HTYPE=" + Convert.ToInt32(param[4]) + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and (SLABMIN <= " + WGHT + " or SLABMIN >= " + WGHT + ")").ToList();
                var query = context.Database.SqlQuery<PR_NEW_IMPORT_RATECARDMASTER_FLX_ASSGN_Result>("EXEC PR_NEW_IMPORT_RATECARDMASTER_FLX_ASSGN @PTARIFFMID=" + TARIFFMID + ", @PSLABTID=" + handlng + ", @PHTYPE=" + Convert.ToInt32(param[4]) + ", @PCHRGETYPE = " + CHRGETYPE + ", @PCONTNRSID = " + CONTNRSID + ", @PSLABMIN = " + WGHT).ToList();
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

            if (param[0] != "") { TARIFFMID = Convert.ToInt32(param[0]); } else { TARIFFMID = 0; };
            var CHRGETYPE = Convert.ToInt32(param[1]);
            if (param[2] != "")
            { CONTNRSID = Convert.ToInt32(param[2]); }
            else
            { CONTNRSID = 0; }

            if (param[3] != "")
            { STMRID = Convert.ToInt32(param[3]); }
            else
            { STMRID = 0; }

            //if (param[6] != "")
            //{ WGHT = Convert.ToInt32(param[6]); }
            //else
            //{ WGHT = 0; }


            var SLABMIN = Convert.ToInt32(param[4]);
            if (TARIFFMID == 4)
            {
                var query = context.Database.SqlQuery<VW_IMPORT_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_IMPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID=2 and HTYPE=" + Convert.ToInt32(param[5]) + " and SDTYPE=1 and SLABMIN <= " + SLABMIN + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and STMRID=" + STMRID + " order by SLABMIN").ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var query = context.Database.SqlQuery<VW_IMPORT_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_IMPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID=2 and HTYPE=" + Convert.ToInt32(param[5]) + " and SDTYPE=1 and SLABMIN <= " + SLABMIN + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " order by SLABMIN").ToList();

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
            var xcovidsdate = Convert.ToDateTime(Session["COVIDSDATE"]).ToString("dd/MM/yyyy").Split('-');

            
            if (xcovidsdate.Length == 1)
                xcovidsdate = Convert.ToDateTime(Session["COVIDSDATE"]).ToString("dd/MM/yyyy").Split('/');
            var zcovidsdate = xcovidsdate[1] + '-' + xcovidsdate[0] + '-' + xcovidsdate[2];

            var xcovidedate = Convert.ToDateTime(Session["COVIDEDATE"]).ToString("dd/MM/yyyy").Split('-');
            if (xcovidedate.Length == 1)
                xcovidedate = Convert.ToDateTime(Session["COVIDEDATE"]).ToString("dd/MM/yyyy").Split('/');
            var zcovidedate = xcovidedate[1] + '-' + xcovidedate[0] + '-' + xcovidedate[2];
                        

            using (var e = new CFSImportEntities())
            {
                //var query = context.Database.SqlQuery<z_pr_New_Import_Covid_Slab_Assgn_Result>("z_pr_New_Import_Covid_Slab_Assgn @PKUSRID = '" + Session["CUSRID"] + "', @PSDATE = '" + zsdate + "', @PEDATE = '" + zedate + "', @PTARIFFMID = " + ztariffmid + ", @PSTMRID = " + zstmrmid + ", @PCHRGETYPE = " + zchrgtype + ", @PSLABTID = 2, @PSLABMIN = 0, @PCONTNRSID = " + zcontnrsid + ", @PSLABHTYPE = 0, @PCHRGDATE = '" + zchrgdate + "' @PCDate1 = '" + zcovidsdate + "', @PCDate2 = '" + zcovidedate + "'").ToList();
                var query = context.Database.SqlQuery<z_pr_New_Import_Covid_Slab_Assgn_Result>("z_pr_New_Import_Covid_Slab_Assgn @PKUSRID = '" + Session["CUSRID"].ToString() + "', @PSDATE = '" + zsdate + "', @PEDATE = '" + zedate + "', @PTARIFFMID = " + ztariffmid + ", @PSTMRID = " + zstmrmid + ", @PCHRGETYPE = " + zchrgtype + ", @PSLABTID = 2, @PSLABMIN = 0, @PCONTNRSID = " + zcontnrsid + ", @PSLABHTYPE = " + zotype + ", @PCHRGDATE = '" + zchrgdate + "', @PCDate1 = '" + zcovidsdate + "', @PCDate2 = '" + zcovidedate + "'").ToList();
                //var a = 1;
                return Json(query, JsonRequestBehavior.AllowGet);
            }
        } //...end

        public JsonResult GetHandlingAmt(string id)
        {
            var param = id.Split('-');
            // var TARIFFMID = Convert.ToInt32(param[0]);
            var TARIFFMID = 0;
            var CHRGETYPE = 0;
            var CONTNRSID = 0;
            var STMRID = 0;
            var WGHT = "0";
            var handlng = 0;
            var htyp = 0;
            var sdtype = 0;
            if(param.Length > 0)
            {
                if (param[0] != "" && param[0] != "null") { TARIFFMID = Convert.ToInt32(param[0]); } else { TARIFFMID = 0; };
                if (param[1] != "" && param[1] != "null")
                    CHRGETYPE = Convert.ToInt32(param[1]);
                if (param[2] != "" && param[2] != "null")
                    CONTNRSID = Convert.ToInt32(param[2]);
                if (param[3] != "" && param[3] != "null")
                    STMRID = Convert.ToInt32(param[3]);
                if (param[4] != "" && param[4] != "null")
                    htyp = Convert.ToInt32(param[4]);
                if (param[5] != "" && param[5] != "null")
                    sdtype = Convert.ToInt32(param[5]);
                if (param[1] == "1") { handlng = 3; }
                if (param[1] == "2") { handlng = 4; }
                if (param[6] != "") { WGHT = param[6]; } else { WGHT = "0"; };
            }
            WGHT = WGHT.Replace(',', '.');
            //Response.Write(strqty);
            decimal aqty = Convert.ToDecimal(WGHT);

            if (TARIFFMID == 4)
            {
                var query = context.Database.SqlQuery<VW_IMPORT_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_IMPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (" + handlng + ") and HTYPE=" + htyp + " and SDTYPE=" + sdtype + " and  CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and STMRID=" + STMRID + " order by SLABMIN").ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var tqry = "select SLABAMT,SLABMIN,SLABMAX from VW_IMPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID = " + TARIFFMID + " and SLABTID in (" + handlng + ") and HTYPE = " + htyp + " and SDTYPE = " + sdtype + "  and CHRGETYPE = " + CHRGETYPE + " and CONTNRSID = " + CONTNRSID + " and ((" + aqty + " >= slabmin and " + aqty + " <= slabmax and SLABMAX <> 0) or (" + aqty + " >= slabmin and SLABMAX = 0))   order by SLABMIN";

                var query = context.Database.SqlQuery<VW_IMPORT_RATECARDMASTER_FLX_ASSGN>(tqry).ToList();

                //var query = context.Database.SqlQuery<VW_IMPORT_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_IMPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID in (" + handlng + ") and HTYPE=" + Convert.ToInt32(param[4]) + " and SDTYPE=" + Convert.ToInt32(param[5]) + "  and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " order by SLABMIN").ToList();

                return Json(query, JsonRequestBehavior.AllowGet);


            }

        } //...end


        //............ratecardmaster.....................
        public JsonResult GetImportGSTRATE(string id)
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
                var query = context.Database.SqlQuery<VW_IMPORT_SLABTYPE_HSN_DETAIL_ASSGN>("select HSNCODE,CGSTEXPRN,SGSTEXPRN,IGSTEXPRN from VW_IMPORT_SLABTYPE_HSN_DETAIL_ASSGN where SLABTID=" + SlabTId + " order by HSNCODE").ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var query = context.Database.SqlQuery<VW_IMPORT_SLABTYPE_HSN_DETAIL_ASSGN>("select HSNCODE,ACGSTEXPRN as CGSTEXPRN,ASGSTEXPRN as SGSTEXPRN,AIGSTEXPRN as IGSTEXPRN from VW_IMPORT_SLABTYPE_HSN_DETAIL_ASSGN where SLABTID=" + SlabTId + " order by HSNCODE").ToList();

                return Json(query, JsonRequestBehavior.AllowGet);


            }

        } //...end

        //............ratecardmaster.....................
        public JsonResult GetStateType(string id)
        {
            var stateid = 0;
            if (id != "")
            { stateid = Convert.ToInt32(id); }
            else
            { stateid = 0; }

            var query = context.Database.SqlQuery<StateMaster>("select * From StateMaster where StateId =" + stateid).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);

        } //...end

        //..........TARIFFTMID get function....................
        public JsonResult TARIFFTMID(int id)
        {
            var query = context.Database.SqlQuery<int>("select TARIFFTMID from exporttariffmaster where TARIFFMID=" + id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        } //........end
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

        //..........................Printview...
        [Authorize(Roles = "ImportPerformaInvoicePrint")]
        public void PrintView(string id)
        {

            var param = id.Split(';');
            // Response.Write(@"10.10.5.5"); Response.End();
            //  ........delete TMPRPT...//

            var ids = Convert.ToInt32(param[0]);
            var rpttype = Convert.ToInt32(param[1]);
            var gsttype = Convert.ToInt32(param[2]);
            var billedto = Convert.ToInt32(param[3]);
            var strHead = param[4].ToString();
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "IMPORTPERFORMAINVOICE", Convert.ToInt32(ids), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;



                // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");


                //........Get TRANPCOUNT...//
                var Query = context.Database.SqlQuery<int>("select TRANPCOUNT FROM PERFORMA_TRANSACTIONMASTER where TRANMID=" + ids).ToList();
                var PCNT = 0;

                if (Query.Count() != 0) { PCNT = Query[0]; }
                var TRANPCOUNT = ++PCNT;
                // Response.Write(++PCNT);
                // Response.End();

                context.Database.ExecuteSqlCommand("UPDATE PERFORMA_TRANSACTIONMASTER SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + ids);

                string RptNamePath = "";
                gsttype = 1;
                //switch (billedto)
                //{
                //    case 1:
                //        if (rpttype == 0)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_rpt_IMP.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_import_Invoice_rpt_imp.RPT"; }
                //        else if (rpttype == 1)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_Group_rpt_IMP.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_import_Invoice_Group_rpt_IMP.RPT"; }

                //        break;

                //    default:

                //        if (rpttype == 0)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_rpt.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_import_Invoice_rpt.RPT"; }
                //        else if (rpttype == 1)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_Group_rpt.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_import_Invoice_Group_rpt.RPT"; }

                //        break;
                //}
                //cryRpt.Load(RptNamePath);

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "GST_Performa_Invoice_Rpt.RPT");

                
                cryRpt.RecordSelectionFormula = "{VW_TRANSACTION_PERFORMA_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_TRANSACTION_PERFORMA_CRY_PRINT_ASSGN.TRANMID} = " + ids;



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
                stringbuilder.Clear();
            }

        }
        //public void PrintView(string id)
        //{

        //    var param = id.Split(';');
        //    // Response.Write(@"10.10.5.5"); Response.End();
        //    //  ........delete TMPRPT...//

        //    var ids = Convert.ToInt32(param[0]);
        //    var rpttype = Convert.ToInt32(param[1]); 
        //    context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
        //    var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "IMPORTPERFORMAINVOICE", Convert.ToInt32(ids), Session["CUSRID"].ToString());
        //    if (TMPRPT_IDS == "Successfully Added")
        //    {
        //        ReportDocument cryRpt = new ReportDocument();
        //        TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
        //        TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
        //        ConnectionInfo crConnectionInfo = new ConnectionInfo();
        //        Tables CrTables;



        //        // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");


        //        //........Get TRANPCOUNT...//
        //        var Query = context.Database.SqlQuery<int>("select TRANPCOUNT FROM PERFORMA_TRANSACTIONMASTER where TRANMID=" + ids).ToList();
        //        var PCNT = 0;

        //        if (Query.Count() != 0) { PCNT = Query[0]; }
        //        var TRANPCOUNT = ++PCNT;
        //        // Response.Write(++PCNT);
        //        // Response.End();

        //        context.Database.ExecuteSqlCommand("UPDATE PERFORMA_TRANSACTIONMASTER SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + ids);

        //        if (rpttype==0)
        //        cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_rpt.RPT");
        //        else if (rpttype == 1)
        //            cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_Group_rpt.RPT");

        //        cryRpt.RecordSelectionFormula = "{VW_performatransaction_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_performatransaction_CRY_PRINT_ASSGN.TRANMID} = " + ids;



        //        crConnectionInfo.ServerName = stringbuilder.DataSource;
        //        crConnectionInfo.DatabaseName = stringbuilder.InitialCatalog;
        //        crConnectionInfo.UserID = stringbuilder.UserID;
        //        crConnectionInfo.Password = stringbuilder.Password;

        //        CrTables = cryRpt.Database.Tables;
        //        foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
        //        {
        //            crtableLogoninfo = CrTable.LogOnInfo;
        //            crtableLogoninfo.ConnectionInfo = crConnectionInfo;
        //            CrTable.ApplyLogOnInfo(crtableLogoninfo);
        //        }


        //        cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
        //        cryRpt.Dispose();
        //        cryRpt.Close();
        //    }

        //}
        //end

        //bform start

        [Authorize(Roles = "ImportPerformaInvoiceNameUpdate")]
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
            var query = context.Database.SqlQuery<Performa_TransactionMaster>("select *  FROM PERFORMA_TRANSACTIONMASTER where TRANMID=" + ids).ToList();
            if (query[0].TRANCSNAME != null)
            {
                ViewBag.TRANCSNAME = query[0].TRANCSNAME;
                ViewBag.TRANIMPADDR1 = query[0].TRANIMPADDR1;
                ViewBag.TRANIMPADDR2 = query[0].TRANIMPADDR2;
                ViewBag.TRANIMPADDR3 = query[0].TRANIMPADDR3;
                ViewBag.TRANIMPADDR4 = query[0].TRANIMPADDR4;
            }
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
            return View();
        }
        //bform end

        //public ActionResult AForm(string id)
        //{
        //    if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

        //    var param = id.Split('~');

        //    var ids = Convert.ToInt32(param[0]);
        //    var gstamt = Convert.ToInt32(param[1]);

        //    ViewBag.id = ids;
        //    ViewBag.FGSTAMT = gstamt;
        //    //var query = context.Database.SqlQuery<Performa_TransactionMaster>("select *  FROM PERFORMA_TRANSACTIONMASTER where TRANMID=" + ids).ToList();
        //    //if (query[0].TRANCSNAME != null)
        //    //{
        //    //    ViewBag.TRANCSNAME = query[0].TRANCSNAME;
        //    //    ViewBag.TRANIMPADDR1 = query[0].TRANIMPADDR1;
        //    //    ViewBag.TRANIMPADDR2 = query[0].TRANIMPADDR2;
        //    //    ViewBag.TRANIMPADDR3 = query[0].TRANIMPADDR3;
        //    //    ViewBag.TRANIMPADDR4 = query[0].TRANIMPADDR4;
        //    //}
        //    //else
        //    //{
        //    //    var chaid = Convert.ToInt32(query[0].TRANREFID);
        //    //    var sql = context.Database.SqlQuery<CategoryMaster>("select * from CategoryMaster where CATEID=" + chaid).ToList();
        //    //    ViewBag.TRANCSNAME = sql[0].CATENAME;
        //    //    ViewBag.TRANIMPADDR1 = sql[0].CATEADDR1;
        //    //    ViewBag.TRANIMPADDR2 = sql[0].CATEADDR2;
        //    //    ViewBag.TRANIMPADDR3 = sql[0].CATEADDR3;
        //    //    ViewBag.TRANIMPADDR4 = sql[0].CATEADDR4;
        //    //}

        //    //var query = ""; // rajesh to check context.Database.SqlQuery<pr_Import_Invoice_Print_Customer_Detail_Assgn_Result>("EXEC pr_Import_Invoice_Print_Customer_Detail_Assgn @PTranMID = " + ids).ToList();
        //    //if (query[0].OSBILLREFNAME != null)
        //    //{
        //    //    ViewBag.TRANCSNAME = query[0].OSBILLREFNAME;
        //    //    ViewBag.TRANIMPADDR1 = query[0].CATEADDR1;
        //    //    ViewBag.TRANIMPADDR2 = query[0].CATEADDR2;
        //    //    ViewBag.TRANIMPADDR3 = query[0].CATEADDR3;
        //    //    ViewBag.TRANIMPADDR4 = query[0].CATEADDR4;
        //    //    ViewBag.GSTNO = query[0].CATEGSTNO;
        //    //}
        //    return View();
        //}

        public JsonResult AFormAddr(string id)
        {

            var param = id.Split('~');

            var ids = Convert.ToInt32(param[0]);
            var billedto = Convert.ToInt32(param[1]);

            if (billedto == 0)
            {
                var query = context.Database.SqlQuery<VW_IMPORT_PERFORMA_TRANSACTION_ADDRESS_DETAIL_ASSGN>("select TRANCSNAME,TRANIMPADDR1,TRANIMPADDR2,TRANIMPADDR3,TRANIMPADDR4,CHACATEGSTNO from VW_IMPORT_PERFORMA_TRANSACTION_ADDRESS_DETAIL_ASSGN where TRANMID=" + ids).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = context.Database.SqlQuery<VW_IMPORT_PERFORMA_TRANSACTION_ADDRESS_DETAIL_ASSGN>("select IMPRTNAME,IMPCATEADDR1,IMPCATEADDR2,IMPCATEADDR3,IMPCATEADDR4,IMPCATEGSTNO from VW_IMPORT_PERFORMA_TRANSACTION_ADDRESS_DETAIL_ASSGN where TRANMID=" + ids).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);

            }

        }


        public JsonResult AFormCategoryAddr(string id)
        {

            var param = id.Split('~');

            var ids = Convert.ToInt32(param[0]);
            var billedto = Convert.ToInt32(param[1]);

            if (billedto == 0)
            {
                var query = context.Database.SqlQuery<CategoryMaster>("select TRANCSNAME,TRANIMPADDR1,TRANIMPADDR2,TRANIMPADDR3,TRANIMPADDR4,CHACATEGSTNO from CategoryMaster where CATETID =" + ids).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = context.Database.SqlQuery<VW_IMPORT_PERFORMA_TRANSACTION_ADDRESS_DETAIL_ASSGN>("select IMPRTNAME,IMPCATEADDR1,IMPCATEADDR2,IMPCATEADDR3,IMPCATEADDR4,IMPCATEGSTNO from VW_IMPORT_PERFORMA_TRANSACTION_ADDRESS_DETAIL_ASSGN where TRANMID=" + ids).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);

            }

        }




        public void UpdateBillingName(FormCollection F_Form)
        {
            Performa_TransactionMaster performatransactionmaster = new Performa_TransactionMaster();
            Int32 TRANMID = Convert.ToInt32(F_Form["FTRANMID"]);
            Int32 OSBILLREFID = Convert.ToInt32(F_Form["OSBILLREFID"]);
            string OSBILLREFNAME = F_Form["OSBILLREFNAME"];
            Int16 OSBILLEDTO = Convert.ToInt16(F_Form["OSBILLEDTO"]);

            if (TRANMID != 0)
            {
                performatransactionmaster = context.performatransactionmaster.Find(TRANMID);
            }

            context.Entry(performatransactionmaster).Entity.TRANREFID = Convert.ToInt32(F_Form["OSBILLREFID"]);
            context.Entry(performatransactionmaster).Entity.TRANREFNAME = Convert.ToString(F_Form["OSBILLREFNAME"]);
            context.Entry(performatransactionmaster).Entity.TRANCSNAME = Convert.ToString(F_Form["OSBILLREFNAME"]);
            //context.Entry(performatransactionmaster).Entity.TRANIMPADDR2 = Convert.ToString(F_Form["TRANIMPADDR2"]);
            //context.Entry(performatransactionmaster).Entity.TRANIMPADDR3 = Convert.ToString(F_Form["TRANIMPADDR3"]);
            //context.Entry(performatransactionmaster).Entity.TRANIMPADDR4 = Convert.ToString(F_Form["TRANIMPADDR4"]);
            context.SaveChanges();

            InvoiceNameUpdate(TRANMID, OSBILLREFID, OSBILLEDTO, OSBILLREFNAME);

            Response.Write("Saved");
        }/*END*/



        public ActionResult AForm(string id)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            var param = id.Split('~');

            var ids = Convert.ToInt32(param[0]);
            var gstamt = Convert.ToInt32(param[1]);

            ViewBag.id = ids;
            ViewBag.FGSTAMT = gstamt;

            
            var query = context.Database.SqlQuery<pr_Performa_Invoice_Print_Customer_Detail_Assgn_Result>("EXEC pr_Performa_Invoice_Print_Customer_Detail_Assgn @PTranMID = " + ids).ToList();
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


        public void InvoiceNameUpdate(int tranmid, int osbillrefid, Int16 osbilledto, string osbillrefname)
        {
            try
            {
                var Query = context.Database.SqlQuery<int>("select OSMID from ZW_IMPORT_INVOICE_BILLNAME_UPDATE_DETAIL_CHECK_ASSGN where TRANMID=" + tranmid).ToList();
                if (Query.Count() > 0)
                {
                    var osmid = Query[0];
                    context.Database.ExecuteSqlCommand("pr_Import_Invoice_Billing_Name_Update_Assgn @PTranMId = " + tranmid + ", @POSBillRefId  = " + osbillrefid + ", @PBilledto = " + osbilledto + ", @POSMId = " + osmid + " , @POSBillRefName='" + osbillrefname + "'");
                    // Response.Write("Saved Successfully");
                }
            }
            catch (Exception e) { Response.Write(e.Message); }
        }


        public void SaveAddress(FormCollection F_Form)
        {
            Performa_TransactionMaster performatransactionmaster = new Performa_TransactionMaster();
            Int32 TRANMID = Convert.ToInt32(F_Form["FTRANMID"]);

            if (TRANMID != 0)
            {
                performatransactionmaster = context.performatransactionmaster.Find(TRANMID);
            }

            context.Entry(performatransactionmaster).Entity.TRANCSNAME = Convert.ToString(F_Form["TRANCSNAME"]);
            context.Entry(performatransactionmaster).Entity.TRANIMPADDR1 = Convert.ToString(F_Form["TRANIMPADDR1"]);
            context.Entry(performatransactionmaster).Entity.TRANIMPADDR2 = Convert.ToString(F_Form["TRANIMPADDR2"]);
            context.Entry(performatransactionmaster).Entity.TRANIMPADDR3 = Convert.ToString(F_Form["TRANIMPADDR3"]);
            context.Entry(performatransactionmaster).Entity.TRANIMPADDR4 = Convert.ToString(F_Form["TRANIMPADDR4"]);
            context.SaveChanges();
            Response.Write("Saved");
        }/*END*/
        /*PRINT DETAIL*/
        public ActionResult CForm(int id)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ViewBag.id = id;
            var query = context.Database.SqlQuery<Performa_TransactionMaster>("select *  FROM PERFORMA_TRANSACTIONMASTER where TRANMID=" + id).ToList();

            var chaid = Convert.ToInt32(query[0].TRANREFID);
            var sql = context.Database.SqlQuery<CategoryMaster>("select * from CategoryMaster where CATEID=" + chaid).ToList();
            ViewBag.CATEMAIL = sql[0].CATEMAIL;

            var TmpAStr = "<table width='539' border='0' cellpadding='0' cellspacing='0' style='background:#FFCC66;border:1px solid #663300'><tr><Td colspan='2'  style='background:#663300;color:#FFFFFF;font-weight:700;text-align:center;padding:5px'>Import</Td> </tr> <tr>";
            TmpAStr = TmpAStr + " <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left'width='208' >Invoice No. </th> <td style='padding:4px;border-bottom:1px solid #663300'  width='331'>" + query[0].TRANDNO + "</td> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Date </th>";
            TmpAStr = TmpAStr + " <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANDATE + "</td> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Bill Amount </th> <td style='padding:4px;border-bottom:1px solid #663300' >[[PInvAmt]]</td> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >CHA Name </th>";
            TmpAStr = TmpAStr + " <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANREFNAME + "</td> </tr> </table> <br> <br> <table width='539' border='0' cellpadding='0' cellspacing='0' style='background:#FFCC66;border:1px solid #663300 '> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left'width='208' >SUDHARSHAN LOGISTICS PRIVATE LIMITED</th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' ># 592, ENNORE EXPRESS HIGH ROAD,</th>";
            TmpAStr = TmpAStr + " </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >CHENNAI - 600 057. </th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Phone No. : 6545 5252 / 2573 3447 / 2573 3762</th> </tr> <tr> <th  style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >E-Mail Id : chennaicfs@sancotrans.com</th> </tr> </table>";

            ViewBag.SUB = "Import Invoice No." + query[0].TRANDNO;
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
                var query = context.Database.SqlQuery<Performa_TransactionMaster>("select *  FROM PERFORMA_TRANSACTIONMASTER where TRANMID=" + Convert.ToInt32(mysbfrm["FTRANMID"])).ToList();
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
                message.Attachments.Add(new Attachment("E:\\CFS\\" + Session["CUSRID"] + "\\ImportInv\\" + query[0].TRANNO + ".pdf"));
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
        //public void Contact(FormCollection mysbfrm)
        //{

        //        var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
        //        var message = new MailMessage();
        //        message.To.Add(new MailAddress("name@gmail.com")); //replace with valid value
        //        message.Subject = "Your email subject";
        //        message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
        //        message.IsBodyHtml = true;
        //        if (model.Upload != null && model.Upload.ContentLength > 0)
        //        {
        //            message.Attachments.Add(new Attachment(model.Upload.InputStream, Path.GetFileName(model.Upload.FileName)));
        //        }
        //        using (var smtp = new SmtpClient())
        //        {
        //            await smtp.SendMailAsync(message);
        //           // return RedirectToAction("Sent");
        //        }

        //}




        [Authorize(Roles = "ImportPerformaInvoicePrint")]
        public void APrintView(string id)
        {
            // Response.Write(@"10.10.5.5"); Response.End();
            //  ........delete TMPRPT...//
            var param = id.Split(';');

            var ids = Convert.ToInt32(param[0]); var rpttype = Convert.ToInt32(param[1]);

            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "IMPORTPERFORMAINVOICE", Convert.ToInt32(ids), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;



                // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");


                //........Get TRANPCOUNT...//
                var Query = context.Database.SqlQuery<Performa_TransactionMaster>("select * FROM PERFORMA_TRANSACTIONMASTER where TRANMID=" + ids).ToList();
                var PCNT = 0;

                if (Query.Count() != 0) { PCNT = Convert.ToInt32(Query[0].TRANPCOUNT); }
                var TRANPCOUNT = ++PCNT;
                // Response.Write(++PCNT);
                // Response.End();

                context.Database.ExecuteSqlCommand("UPDATE PERFORMA_TRANSACTIONMASTER SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + ids);

                if (rpttype == 0)
                    cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_rpt_E01.RPT");
                else cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_group_rpt_E01.RPT");

                cryRpt.RecordSelectionFormula = "{VW_performatransaction_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_performatransaction_CRY_PRINT_ASSGN.TRANMID} = " + ids;



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
                string path = "E:\\CFS\\" + Session["CUSRID"] + "\\ImportInv";
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
                stringbuilder.Clear();
            }

        }
        //end
        //...............Delete Row.............
        [Authorize(Roles = "ImportPerformaInvoiceDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);

            if (temp.Equals("PROCEED"))
            {
                Performa_TransactionMaster performatransactionmaster = context.performatransactionmaster.Find(Convert.ToInt32(id));
                context.performatransactionmaster.Remove(performatransactionmaster);
                context.SaveChanges();
                Response.Write("Deleted successfully...");
            }
            else
                Response.Write(temp);

        }//-----End of Delete Row
    }
}