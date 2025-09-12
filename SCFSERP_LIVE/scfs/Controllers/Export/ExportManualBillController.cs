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
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers.Export
{
    [SessionExpire]
    public class ExportManualBillController : Controller
    {
        //
        // GET: /ExportManualBill/
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        [Authorize(Roles = "ExportManualBillIndex")]
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
                Session["REGSTRID"] = "49";
            }
            //...........Bill type......//
            List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
            if (Convert.ToInt32(Session["TRANBTYPE"]) == 1)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "LI", Value = "1", Selected = true };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "DS", Value = "2", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);

            }
            else
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "LI", Value = "1", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "DS", Value = "2", Selected = true };
                selectedBILLYPE.Add(selectedItemGPTY);

            }
            ViewBag.TRANBTYPE = selectedBILLYPE;
            ////..........end
            //............Billed to....//
            List<SelectListItem> selectedregid_ = new List<SelectListItem>();
            
            if (Convert.ToInt32(Session["REGSTRID"]) == 49)
            {
                SelectListItem selectedItemreg_ = new SelectListItem { Text = "TAX INVOICE", Value = "49", Selected = true };
                selectedregid_.Add(selectedItemreg_);
                selectedItemreg_ = new SelectListItem { Text = "BILL OF SUPPLY", Value = "50", Selected = false };
                selectedregid_.Add(selectedItemreg_);
                ViewBag.REGSTRID = selectedregid_;
            }
            else if (Convert.ToInt32(Session["REGSTRID"]) == 50)
            {
                SelectListItem selectedItemreg_ = new SelectListItem { Text = "TAX INVOICE", Value = "49", Selected = false };
                selectedregid_.Add(selectedItemreg_);
                selectedItemreg_ = new SelectListItem { Text = "BILL OF SUPPLY", Value = "50", Selected = true };
                selectedregid_.Add(selectedItemreg_);
                ViewBag.REGSTRID = selectedregid_;
            }
            
            //.....end


            return View();
        }
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new  CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_ExportManualBill(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToInt32(Session["REGSTRID"]), 0, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANTIME.Value.ToString("hh:mm tt"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANNAMT.ToString(), d.GSTAMT.ToString(), d.ACKNO, d.DISPSTATUS, d.TRANMID.ToString() }).ToArray();
                //var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANTIME.Value.ToString("hh:mm tt"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANNAMT.ToString(), d.DISPSTATUS, d.TRANMID.ToString(), d.GSTAMT.ToString(), d.ACKNO }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //[Authorize(Roles = "ExportManualBillEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ExportManualBill/GSTForm/" + id);
        }

        //[Authorize(Roles = "ExportManualBillEdit")]
        public void MEdit(int id)
        {
            Response.Redirect("/ExportManualBill/MGSTForm/" + id);
        }

        [Authorize(Roles = "ExportManualBillCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            TransactionMaster tab = new TransactionMaster();
            TransactionMD vm = new TransactionMD();

            //..........................................Dropdown data.........................//

            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.BILLEDID = new SelectList(context.containersizemasters.Where(x => x.DISPSTATUS == 0 && x.CONTNRSID > 1), "CONTNRSID", "CONTNRSDESC");
            ViewBag.TRANDREFID = new SelectList(context.chargemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CHRGDESC), "CHRGID", "CHRGDESC");
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");

            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "LI", Value = "1", Selected = true };
            selectedBILLTYPE.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "DS", Value = "2", Selected = false };
            selectedBILLTYPE.Add(selectedItemDSP);
            ViewBag.TRANBTYPE = selectedBILLTYPE;
            //....end

            //............Billed to....//
            List<SelectListItem> selectedregid = new List<SelectListItem>();
            //SelectListItem selectedItemreg = new SelectListItem { Text = "EMPTY", Value = "21", Selected = false };
            //selectedregid.Add(selectedItemreg);
            //selectedItemreg = new SelectListItem { Text = "LOAD", Value = "22", Selected = false };
            //selectedregid.Add(selectedItemreg);
            //selectedItemreg = new SelectListItem { Text = "WGMT", Value = "23", Selected = true };
            //selectedregid.Add(selectedItemreg);
            //ViewBag.REGSTRID = selectedregid;
            SelectListItem selectedItemreg = new SelectListItem { Text = "TAX INVOICE", Value = "49", Selected = true };
            selectedregid.Add(selectedItemreg);
            selectedItemreg = new SelectListItem { Text = "BILL OF SUPPLY", Value = "50", Selected = false };
            selectedregid.Add(selectedItemreg);
            ViewBag.REGSTRID = selectedregid;
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

                ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC", tab.BANKMID);
                ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL", tab.TRANMODE);


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
                List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
                if (Convert.ToInt32(tab.TRANBTYPE) == 1)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LI", Value = "1", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);
                    selectedItemGPTY = new SelectListItem { Text = "DS", Value = "2", Selected = false };
                    selectedBILLYPE.Add(selectedItemGPTY);

                }
                else
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LI", Value = "1", Selected = false };
                    selectedBILLYPE.Add(selectedItemGPTY);
                    selectedItemGPTY = new SelectListItem { Text = "DS", Value = "2", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);

                }
                ViewBag.TRANBTYPE = selectedBILLYPE;
                ////..........end
                //............Billed to....//
                List<SelectListItem> selectedregid_ = new List<SelectListItem>();
                //if (Convert.ToInt32(tab.REGSTRID) == 22)
                //{
                //    SelectListItem selectedItemreg_ = new SelectListItem { Text = "EMPTY", Value = "21", Selected = false };
                //    selectedregid_.Add(selectedItemreg_);
                //    selectedItemreg_ = new SelectListItem { Text = "LOAD", Value = "22", Selected = true };
                //    selectedregid_.Add(selectedItemreg_);
                //    ViewBag.REGSTRID = selectedregid_;
                //    selectedItemreg_ = new SelectListItem { Text = "WGMT", Value = "23", Selected = false };
                //    selectedregid_.Add(selectedItemreg_);
                //}
                //else if (Convert.ToInt32(tab.REGSTRID) == 21)
                //{
                //    SelectListItem selectedItemreg_ = new SelectListItem { Text = "EMPTY", Value = "21", Selected = true };
                //    selectedregid_.Add(selectedItemreg_);
                //    selectedItemreg_ = new SelectListItem { Text = "LOAD", Value = "22", Selected = false };
                //    selectedregid_.Add(selectedItemreg_);
                //    ViewBag.REGSTRID = selectedregid_;
                //    selectedItemreg_ = new SelectListItem { Text = "WGMT", Value = "23", Selected = false };
                //    selectedregid_.Add(selectedItemreg_);
                //}
                if (Convert.ToInt32(tab.REGSTRID) == 49)
                {
                    SelectListItem selectedItemreg_ = new SelectListItem { Text = "TAX INVOICE", Value = "49", Selected = true };
                    selectedregid_.Add(selectedItemreg_);
                    selectedItemreg_ = new SelectListItem { Text = "BILL OF SUPPLY", Value = "50", Selected = false };
                    selectedregid_.Add(selectedItemreg_);
                    ViewBag.REGSTRID = selectedregid_;
                }
                else if (Convert.ToInt32(tab.REGSTRID) == 50)
                {
                    SelectListItem selectedItemreg_ = new SelectListItem { Text = "TAX INVOICE", Value = "49", Selected = false };
                    selectedregid_.Add(selectedItemreg_);
                    selectedItemreg_ = new SelectListItem { Text = "BILL OF SUPPLY", Value = "50", Selected = true };
                    selectedregid_.Add(selectedItemreg_);
                    ViewBag.REGSTRID = selectedregid_;
                }

                //.....end

                vm.masterdata = context.transactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.detaildata = context.transactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.costfactor = context.transactionmasterfactor.Where(det => det.TRANMID == id).ToList();

            }
            return View(vm);
        }


        //gst form
        [Authorize(Roles = "ExportManualBillCreate")]
        public ActionResult GSTForm(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            TransactionMaster tab = new TransactionMaster();
            TransactionMD vm = new TransactionMD();

            //..........................................Dropdown data.........................//
            ViewBag.ACHEADID = new SelectList(context.accountheadmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ACHEADDESC), "ACHEADID", "ACHEADDESC");
            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.BILLEDID = new SelectList(context.containersizemasters.Where(x => x.DISPSTATUS == 0 && x.CONTNRSID > 1), "CONTNRSID", "CONTNRSDESC");
            ViewBag.TRANDREFID = new SelectList(context.chargemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CHRGDESC), "CHRGID", "CHRGDESC");
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.CATEAID = new SelectList("");
            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "LI", Value = "1", Selected = true };
            selectedBILLTYPE.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "DS", Value = "2", Selected = false };
            selectedBILLTYPE.Add(selectedItemDSP);
            ViewBag.TRANBTYPE = selectedBILLTYPE;
            //....end

            //............Billed to....//
            List<SelectListItem> selectedregid = new List<SelectListItem>();
            //SelectListItem selectedItemreg = new SelectListItem { Text = "EMPTY", Value = "21", Selected = false };
            //selectedregid.Add(selectedItemreg);
            //selectedItemreg = new SelectListItem { Text = "LOAD", Value = "22", Selected = false };
            //selectedregid.Add(selectedItemreg);
            //selectedItemreg = new SelectListItem { Text = "WGMT", Value = "23", Selected = true };
            //selectedregid.Add(selectedItemreg);

            SelectListItem selectedItemreg = new SelectListItem { Text = "TAX INVOICE", Value = "49", Selected = false };
            selectedregid.Add(selectedItemreg);
            selectedItemreg = new SelectListItem { Text = "BILL OF SUPPLY", Value = "50", Selected = false };
            selectedregid.Add(selectedItemreg);
            ViewBag.REGSTRID = selectedregid;


            //.....end

            //........display status.........//
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemDISP = new SelectListItem { Text = "In Books", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItemDISP);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            //....end

            //.........s.Tax...//
            List<SelectListItem> selectedtaxlst = new List<SelectListItem>();
            SelectListItem selectedItemtax = new SelectListItem { Text = "Yes", Value = "0", Selected = true };
            selectedtaxlst.Add(selectedItemtax);
            selectedItemtax = new SelectListItem { Text = "No", Value = "1", Selected = false };
            selectedtaxlst.Add(selectedItemtax);
            ViewBag.STAX = selectedtaxlst;

            if (id != 0)//....Edit Mode
            {
                tab = context.transactionmaster.Find(id);//find selected record

                //...................................Selected dropdown value..................................//

                ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC", tab.BANKMID);
                ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL", tab.TRANMODE);

                var sqy = context.Database.SqlQuery<Category_Address_Details>("Select *from CATEGORY_ADDRESS_DETAIL WHERE CATEAID=" + tab.CATEAID).ToList();
                if (sqy.Count > 0)
                {
                    ViewBag.CATEAID = new SelectList(context.categoryaddressdetails.Where(x => x.CATEAID > 0), "CATEAID", "CATEATYPEDESC", tab.CATEAID);
                }
                else { ViewBag.CATEAID = new SelectList(""); }
                
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

                List<SelectListItem> selectedregide_ = new List<SelectListItem>();
                if (Convert.ToInt32(tab.REGSTRID) == 49)
                {
                    SelectListItem selectedItemrege1 = new SelectListItem { Text = "TAX INVOICE", Value = "49", Selected = true };
                    selectedregide_.Add(selectedItemrege1);
                    ViewBag.REGSTRID = selectedregide_;
                }
                else
                {
                    SelectListItem selectedItemrege1 = new SelectListItem { Text = "BILL OF SUPPLY", Value = "50", Selected = true };
                    selectedregide_.Add(selectedItemrege1);
                    ViewBag.REGSTRID = selectedregide_;
                }
                    
                //.................Stax.................//
                List<SelectListItem> selectedDISP1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.TRANTID) == 0)
                {
                    SelectListItem selectedItemDIS1 = new SelectListItem { Text = "No", Value = "1", Selected = false };
                    selectedDISP1.Add(selectedItemDIS1);
                    selectedItemDIS1 = new SelectListItem { Text = "Yes", Value = "0", Selected = true };
                    selectedDISP1.Add(selectedItemDIS1);
                    ViewBag.STAX = selectedDISP1;
                }
                else
                {
                    SelectListItem selectedItemDIS2 = new SelectListItem { Text = "No", Value = "1", Selected = true };
                    selectedDISP1.Add(selectedItemDIS2);
                    selectedItemDIS2 = new SelectListItem { Text = "Yes", Value = "0", Selected = false };
                    selectedDISP1.Add(selectedItemDIS2);
                    ViewBag.STAX = selectedDISP1;
                }//....end



                ////....................Bill type.................//
                List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
                if (Convert.ToInt32(tab.TRANBTYPE) == 1)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LI", Value = "1", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);
                    selectedItemGPTY = new SelectListItem { Text = "DS", Value = "2", Selected = false };
                    selectedBILLYPE.Add(selectedItemGPTY);

                }
                else
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LI", Value = "1", Selected = false };
                    selectedBILLYPE.Add(selectedItemGPTY);
                    selectedItemGPTY = new SelectListItem { Text = "DS", Value = "2", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);

                }
                ViewBag.TRANBTYPE = selectedBILLYPE;
                ////..........end
                //............Billed to....//
                List<SelectListItem> selectedregid_ = new List<SelectListItem>();
                //if (Convert.ToInt32(tab.REGSTRID) == 22)
                //{
                //    SelectListItem selectedItemreg_ = new SelectListItem { Text = "EMPTY", Value = "21", Selected = false };
                //    selectedregid_.Add(selectedItemreg_);
                //    selectedItemreg_ = new SelectListItem { Text = "LOAD", Value = "22", Selected = true };
                //    selectedregid_.Add(selectedItemreg_);
                //    ViewBag.REGSTRID = selectedregid_;
                //    selectedItemreg_ = new SelectListItem { Text = "WGMT", Value = "23", Selected = false };
                //    selectedregid_.Add(selectedItemreg_);
                //}
                //else if (Convert.ToInt32(tab.REGSTRID) == 21)
                //{
                //    SelectListItem selectedItemreg_ = new SelectListItem { Text = "EMPTY", Value = "21", Selected = true };
                //    selectedregid_.Add(selectedItemreg_);
                //    selectedItemreg_ = new SelectListItem { Text = "LOAD", Value = "22", Selected = false };
                //    selectedregid_.Add(selectedItemreg_);
                //    ViewBag.REGSTRID = selectedregid_;
                //    selectedItemreg_ = new SelectListItem { Text = "WGMT", Value = "23", Selected = false };
                //    selectedregid_.Add(selectedItemreg_);
                //}
                
                //.....end

                vm.masterdata = context.transactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.detaildata = context.transactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.costfactor = context.transactionmasterfactor.Where(det => det.TRANMID == id).ToList();
                vm.manualviewdata = context.Database.SqlQuery<pr_Export_Manual_Invoice_Ctrl_Flx_Assgn_Result>("pr_Export_Manual_Invoice_Ctrl_Flx_Assgn @PTRANMID=" + id + "").ToList();//........procedure  for edit mode details data

            }
            return View(vm);
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
                        transactionmaster.TRANTID = Convert.ToInt16(F_Form["STAX"]); //2;
                        transactionmaster.TRANLMID = 0;
                        transactionmaster.TRANLSID = 0;
                        transactionmaster.TRANLSNO = null;
                        transactionmaster.TRANLMNO = "";
                        transactionmaster.TRANLMDATE = DateTime.Now;
                        transactionmaster.TRANLSDATE = DateTime.Now;
                        transactionmaster.TRANNARTN = null;
                        var cusr = transactionmaster.CUSRID;
                        if (TRANMID.ToString() == "0" || cusr == null || cusr == "")
                            transactionmaster.CUSRID = Session["CUSRID"].ToString();
                        transactionmaster.LMUSRID = Session["CUSRID"].ToString();
                        transactionmaster.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
                        transactionmaster.PRCSDATE = DateTime.Now;
                        transactionmaster.TRANDATE = Convert.ToDateTime(F_Form["masterdata[0].TRANTIME"]).Date;
                        transactionmaster.TRANTIME = Convert.ToDateTime(F_Form["masterdata[0].TRANTIME"]);
                        transactionmaster.TRANREFID = Convert.ToInt32(F_Form["masterdata[0].TRANREFID"]);
                        transactionmaster.TRANREFNAME = F_Form["masterdata[0].TRANREFNAME"].ToString();
                        transactionmaster.LCATEID = 0;
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

                        string CATEAID = Convert.ToString(F_Form["CATEAID"]);
                        if (CATEAID == "" || CATEAID == null || CATEAID == "0")
                        { transactionmaster.CATEAID = 0; }
                        else { transactionmaster.CATEAID = Convert.ToInt32(CATEAID); }

                        string STATEID = Convert.ToString(F_Form["masterdata[0].STATEID"]);
                        if (STATEID == "" || STATEID == null || STATEID == "0")
                        { transactionmaster.STATEID = 0; }
                        else { transactionmaster.STATEID = Convert.ToInt32(STATEID); }

                        transactionmaster.CATEAGSTNO = Convert.ToString(F_Form["masterdata[0].CATEAGSTNO"]);
                        transactionmaster.TRANIMPADDR1 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR1"]);
                        transactionmaster.TRANIMPADDR2 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR2"]);
                        transactionmaster.TRANIMPADDR3 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR3"]);
                        transactionmaster.TRANIMPADDR4 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR4"]);

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
                        string bill = "";
                        if (Convert.ToInt32(F_Form["TRANBTYPE"]) == 1) bill = "LI";
                        else bill = "DS";
                        if (TRANMID == 0)
                        {
                            //transactionmaster.TRANNO = Convert.ToInt16(Autonumber.autonum("transactionmaster", "TRANNO", "TRANNO<>0 and TRANBTYPE=" + Convert.ToInt16(F_Form["TRANBTYPE"]) + " and REGSTRID=" + Convert.ToInt16(F_Form["REGSTRID"]) + " and SDPTID=2 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                            if (regsid == 49)
                                transactionmaster.TRANNO = Convert.ToInt32(auto_numbr_invoice.gstexportautonum("transactionmaster", "TRANNO", F_Form["REGSTRID"].ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString(), Convert.ToInt32(transactionmaster.TRANREFID)).ToString());
                            else
                                transactionmaster.TRANNO = Convert.ToInt32(auto_numbr_invoice.gstexportautonum_BS("transactionmaster", "TRANNO", F_Form["REGSTRID"].ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString(), Convert.ToInt32(transactionmaster.TRANREFID)).ToString());
                            int ano = transactionmaster.TRANNO;
                            //string format = "SUD/EXP/";
                            string format = "EXP/" ;
                            string btyp = auto_numbr_invoice.GetCateBillType(Convert.ToInt32(transactionmaster.TRANREFID)).ToString();
                            if (btyp == "")
                            {
                                format = format + Session["GPrxDesc"] + "/";
                            }
                            else
                                format = format.Replace("/", "") + Session["GPrxDesc"] + btyp;
                            string billformat = "";
                            switch (regsid)
                            {
                                //case 21: billformat = "STL/MNUL/EXP/"; break; //"EXP/LI/"; break;
                                //case 22: billformat = "STL/MNUL/EXP/"; break; //"EXP/" + bill + "/"; break;
                                //case 23: billformat = "STL/MNUL/EXP/"; break;

                                case 49: billformat = "STL/MNUL/EXP/"; break; //"EXP/LI/"; break;
                                case 50: billformat = "STL/MNUL/EXP/"; break; //"EXP/" + bill + "/"; break;
                                //case 23: billformat = "STL/MNUL/EXP/"; break;
                            }
                            string prfx = string.Format(format + "{0:D5}", ano);
                            string billprfx = string.Format(billformat + "{0:D5}", ano);
                            transactionmaster.TRANDNO = prfx.ToString();
                            transactionmaster.TRANBILLREFNO = billprfx.ToString();

                            //........end of autonumber
                            context.transactionmaster.Add(transactionmaster);
                            context.SaveChanges();
                            TRANMID = transactionmaster.TRANMID;
                        }
                        else
                        {
                            //int ano = transactionmaster.TRANNO;
                            ////string format = "SUD/EXP/";
                            //string format = "EXP/";
                            //string btyp = auto_numbr_invoice.GetCateBillType(Convert.ToInt32(transactionmaster.TRANREFID)).ToString();
                            //if (btyp == "")
                            //{
                            //    format = format + Session["GPrxDesc"] + "/";
                            //}
                            //else
                            //    format = format.Replace("/", "") + Session["GPrxDesc"] + btyp;
                            //string billformat = "";
                            //switch (regsid)
                            //{
                            //    //case 21: billformat = "STL/MNUL/EXP/"; break; //"EXP/LI/"; break;
                            //    //case 22: billformat = "STL/MNUL/EXP/"; break; //"EXP/" + bill + "/"; break;
                            //    //case 23: billformat = "STL/MNUL/EXP/"; break;

                            //    case 49: billformat = "STL/MNUL/EXP/"; break; //"EXP/LI/"; break;
                            //    case 50: billformat = "STL/MNUL/EXP/"; break; //"EXP/" + bill + "/"; break;
                            //   // case 23: billformat = "STL/MNUL/EXP/"; break;
                            //}
                            //string prfx = string.Format(format + "{0:D5}", ano);
                            //string billprfx = string.Format(billformat + "{0:D5}", ano);
                            //transactionmaster.TRANDNO = prfx.ToString();
                            //transactionmaster.TRANBILLREFNO = billprfx.ToString();
                            context.Entry(transactionmaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }

                        //-------------transaction Details
                        string[] F_TRANDID = F_Form.GetValues("TRANDID");
                        string[] TRANDREFNO = F_Form.GetValues("TRANDREFNO");
                        string[] TRANIDATE = F_Form.GetValues("TRANIDATE");
                        string[] TRANSDATE = F_Form.GetValues("TRANSDATE");
                        string[] TRANEDATE = F_Form.GetValues("TRANEDATE");
                        string[] TRANDNAMT = F_Form.GetValues("TRANDNAMT");
                        string[] TRANDGAMT = F_Form.GetValues("TRANDGAMT");
                        string[] ACHEADID;
                        string[] TRANDREFID;
                        string[] TRANDDESC;
                        string[] TRANDREFNAME;
                        if (F_Form.GetValues("detaildata[0].ACHEADID") == null)
                        {
                            ACHEADID = F_Form.GetValues("ACHEADID");
                        }
                        else 
                        {
                            ACHEADID = F_Form.GetValues("detaildata[0].ACHEADID");
                        }
                        if (F_Form.GetValues("detaildata[0].TRANDREFID") == null)
                        {
                           TRANDREFID = F_Form.GetValues("TRANDREFID");
                        }
                        else
                        {
                            TRANDREFID = F_Form.GetValues("detaildata[0].TRANDREFID");
                        }
                        if (F_Form.GetValues("detaildata[0].TRANDDESC") == null)
                        {
                            TRANDDESC = F_Form.GetValues("TRANDDESC");
                        }
                        else
                        {
                            TRANDDESC = F_Form.GetValues("detaildata[0].TRANDDESC");
                        }
                        if (F_Form.GetValues("detaildata[0].TRANDREFNAME") == null)
                        {
                            TRANDREFNAME = F_Form.GetValues("TRANDREFNAME");
                        }
                        else
                        {
                            TRANDREFNAME = F_Form.GetValues("detaildata[0].TRANDREFNAME");
                        }
                        string[] BILLEDID = F_Form.GetValues("detaildata[0].BILLEDID");
                        string[] TRANVHLFROM = F_Form.GetValues("TRANVHLFROM");
                        string[] TRANVHLTO = F_Form.GetValues("TRANVHLTO");
                        for (int count = 0; count < F_TRANDID.Count(); count++)
                        {
                            TRANDID = Convert.ToInt32(F_TRANDID[count]);

                            if (TRANDID != 0)
                            {
                                transactiondetail = context.transactiondetail.Find(TRANDID);
                            }
                            transactiondetail.TRANMID = transactionmaster.TRANMID;
                            transactiondetail.TRANDREFNO = (TRANDREFNO[count]).ToString();
                            transactiondetail.TRANDREFNAME = (TRANDREFNAME[count]).ToString();
                            transactiondetail.TRANDREFID = 0; // Convert.ToInt32(TRANDREFID[count]);
                            transactiondetail.TRANDDESC = (TRANDDESC[count]).ToString();
                            transactiondetail.ACHEADID = Convert.ToInt32(ACHEADID[count]);
                            //transactiondetail.TRANIDATE = Convert.ToDateTime(TRANIDATE[count]);
                            transactiondetail.TRANIDATE = DateTime.Now;
                            transactiondetail.TRANSDATE = DateTime.Now;
                            transactiondetail.TRANEDATE = DateTime.Now;
                            transactiondetail.TRANVDATE = DateTime.Now;
                            transactiondetail.TRANVHLFROM = ""; //(TRANVHLFROM[count]).ToString();
                            transactiondetail.TRANVHLTO = "";// (TRANVHLTO[count]).ToString();
                            transactiondetail.TRANDGAMT = Convert.ToDecimal(TRANDGAMT[count]);
                            transactiondetail.TRANDNAMT = Convert.ToDecimal(TRANDGAMT[count]);
                            transactiondetail.BILLEDID = 0;//Convert.ToInt32(BILLEDID[count]);
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
                        context.Database.ExecuteSqlCommand("DELETE FROM transactiondetail  WHERE TRANMID=" + TRANMID + " and  TRANDID NOT IN(" + DELIDS.Substring(1) + ")");
                        trans.Commit(); Response.Redirect("Index");
                    }
                    catch (Exception EX)
                    {
                        var msg = EX.InnerException;
                        trans.Rollback();
                        Response.Write("Sorry!!An Error Ocurred...");
                    }
                }
            }

        }

        [Authorize(Roles = "ExportManualBillCreate")]
        public ActionResult MGSTForm(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            TransactionMaster tab = new TransactionMaster();
            TransactionMD vm = new TransactionMD();

            //..........................................Dropdown data.........................//
            ViewBag.ACHEADID = new SelectList(context.accountheadmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ACHEADDESC), "ACHEADID", "ACHEADDESC");
            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.BILLEDID = new SelectList(context.containersizemasters.Where(x => x.DISPSTATUS == 0 && x.CONTNRSID > 1), "CONTNRSID", "CONTNRSDESC");
            ViewBag.TRANDREFID = new SelectList(context.chargemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CHRGDESC), "CHRGID", "CHRGDESC");
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.CATEAID = new SelectList("");
            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "LI", Value = "1", Selected = true };
            selectedBILLTYPE.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "DS", Value = "2", Selected = false };
            selectedBILLTYPE.Add(selectedItemDSP);
            ViewBag.TRANBTYPE = selectedBILLTYPE;
            //....end

            //............Billed to....//
            List<SelectListItem> selectedregid = new List<SelectListItem>();        

            SelectListItem selectedItemreg = new SelectListItem { Text = "TAX INVOICE", Value = "49", Selected = false };
            selectedregid.Add(selectedItemreg);
            selectedItemreg = new SelectListItem { Text = "BILL OF SUPPLY", Value = "50", Selected = false };
            selectedregid.Add(selectedItemreg);
            ViewBag.REGSTRID = selectedregid;

            //.....end

            //........display status.........//
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemDISP = new SelectListItem { Text = "In Books", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItemDISP);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            //....end

            //.........s.Tax...//
            List<SelectListItem> selectedtaxlst = new List<SelectListItem>();
            SelectListItem selectedItemtax = new SelectListItem { Text = "Yes", Value = "0", Selected = true };
            selectedtaxlst.Add(selectedItemtax);
            selectedItemtax = new SelectListItem { Text = "No", Value = "1", Selected = false };
            selectedtaxlst.Add(selectedItemtax);
            ViewBag.STAX = selectedtaxlst;

            if (id != 0)//....Edit Mode
            {
                tab = context.transactionmaster.Find(id);//find selected record

                //...................................Selected dropdown value..................................//

                ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC", tab.BANKMID);
                ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL", tab.TRANMODE);

                var sqy = context.Database.SqlQuery<Category_Address_Details>("Select *from CATEGORY_ADDRESS_DETAIL WHERE CATEAID=" + tab.CATEAID).ToList();
               
                if (sqy.Count > 0)
                {
                    
                    ViewBag.CATEAID = new SelectList(context.categoryaddressdetails.Where(x => x.CATEID == tab.TRANREFID), "CATEAID", "CATEATYPEDESC", tab.CATEAID);
                }
                else { ViewBag.CATEAID = new SelectList(""); }

                var starqy = context.Database.SqlQuery<StateMaster>("Select *from STATEMASTER where STATEID = " + tab.STATEID).ToList();
                if (starqy.Count > 0)
                {
                    ViewBag.STATEDESC = starqy[0].STATECODE + "  " + starqy[0].STATEDESC;
                    ViewBag.STATETYPE = starqy[0].STATETYPE;
                }


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

                List<SelectListItem> selectedregide_ = new List<SelectListItem>();
                if (Convert.ToInt32(tab.REGSTRID) == 49)
                {
                    SelectListItem selectedItemrege1 = new SelectListItem { Text = "TAX INVOICE", Value = "49", Selected = true };
                    selectedregide_.Add(selectedItemrege1);
                    ViewBag.REGSTRID = selectedregide_;
                }
                else
                {
                    SelectListItem selectedItemrege1 = new SelectListItem { Text = "BILL OF SUPPLY", Value = "50", Selected = true };
                    selectedregide_.Add(selectedItemrege1);
                    ViewBag.REGSTRID = selectedregide_;
                }

                //.................Stax.................//
                List<SelectListItem> selectedDISP1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.TRANTID) == 0)
                {
                    SelectListItem selectedItemDIS1 = new SelectListItem { Text = "No", Value = "1", Selected = false };
                    selectedDISP1.Add(selectedItemDIS1);
                    selectedItemDIS1 = new SelectListItem { Text = "Yes", Value = "0", Selected = true };
                    selectedDISP1.Add(selectedItemDIS1);
                    ViewBag.STAX = selectedDISP1;
                }
                else
                {
                    SelectListItem selectedItemDIS2 = new SelectListItem { Text = "No", Value = "1", Selected = true };
                    selectedDISP1.Add(selectedItemDIS2);
                    selectedItemDIS2 = new SelectListItem { Text = "Yes", Value = "0", Selected = false };
                    selectedDISP1.Add(selectedItemDIS2);
                    ViewBag.STAX = selectedDISP1;
                }//....end



                ////....................Bill type.................//
                List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
                if (Convert.ToInt32(tab.TRANBTYPE) == 1)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LI", Value = "1", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);
                    selectedItemGPTY = new SelectListItem { Text = "DS", Value = "2", Selected = false };
                    selectedBILLYPE.Add(selectedItemGPTY);

                }
                else
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LI", Value = "1", Selected = false };
                    selectedBILLYPE.Add(selectedItemGPTY);
                    selectedItemGPTY = new SelectListItem { Text = "DS", Value = "2", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);

                }
                ViewBag.TRANBTYPE = selectedBILLYPE;
               
                List<SelectListItem> selectedregid_ = new List<SelectListItem>();
              

                vm.masterdata = context.transactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.detaildata = context.transactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.costfactor = context.transactionmasterfactor.Where(det => det.TRANMID == id).ToList();
                vm.manualviewdata = context.Database.SqlQuery<pr_Export_Manual_Invoice_Ctrl_Flx_Assgn_Result>("pr_Export_Manual_Invoice_Ctrl_Flx_Assgn @PTRANMID=" + id + "").ToList();//........procedure  for edit mode details data

            }
            return View(vm);
        }

        public void msavedata(FormCollection F_Form)
        {
            using (SCFSERPContext dataContext = new SCFSERPContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
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
                        transactionmaster.TRANTID = Convert.ToInt16(F_Form["STAX"]); //2;
                        transactionmaster.TRANLMID = 0;
                        transactionmaster.TRANLSID = 0;
                        transactionmaster.TRANLSNO = null;
                        transactionmaster.TRANLMNO = "";
                        transactionmaster.TRANLMDATE = DateTime.Now;
                        transactionmaster.TRANLSDATE = DateTime.Now;
                        transactionmaster.TRANNARTN = null;
                        var cusr = transactionmaster.CUSRID;
                        if (TRANMID.ToString() == "0" || cusr == null || cusr == "")
                            transactionmaster.CUSRID = Session["CUSRID"].ToString();
                        transactionmaster.LMUSRID = Session["CUSRID"].ToString();
                        transactionmaster.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
                        transactionmaster.PRCSDATE = DateTime.Now;
                        transactionmaster.TRANDATE = Convert.ToDateTime(F_Form["masterdata[0].TRANTIME"]).Date;
                        transactionmaster.TRANTIME = Convert.ToDateTime(F_Form["masterdata[0].TRANTIME"]);
                        transactionmaster.TRANREFID = Convert.ToInt32(F_Form["masterdata[0].TRANREFID"]);
                        transactionmaster.TRANREFNAME = F_Form["masterdata[0].TRANREFNAME"].ToString();
                        transactionmaster.LCATEID = 0;
                        transactionmaster.TRANBTYPE = Convert.ToInt16(F_Form["TRANBTYPE"]);
                        transactionmaster.REGSTRID = Convert.ToInt16(F_Form["REGSTRID"]);
                        transactionmaster.TRANMODE = Convert.ToInt16(F_Form["TRANMODE"]);
                        transactionmaster.TRANMODEDETL = (F_Form["masterdata[0].TRANMODEDETL"]);
                        transactionmaster.TRANGAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANGAMT"]);
                        transactionmaster.TRANNAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANNAMT"]);
                        transactionmaster.TRANROAMT = 0; // Convert.ToDecimal(F_Form["masterdata[0].TRANROAMT"]);
                        transactionmaster.TRANREFAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANREFAMT"]);
                        transactionmaster.TRANRMKS = (F_Form["masterdata[0].TRANRMKS"]).ToString();
                        transactionmaster.TRANAMTWRDS = AmtInWrd.ConvertNumbertoWords(F_Form["masterdata[0].TRANNAMT"]);

                        transactionmaster.STRG_HSNCODE = ""; //F_Form["STRG_HSN_CODE"].ToString();
                        transactionmaster.HANDL_HSNCODE = "";// F_Form["HANDL_HSN_CODE"].ToString();

                        transactionmaster.STRG_TAXABLE_AMT = 0; //Convert.ToDecimal(F_Form["STRG_TAXABLE_AMT"]);
                        transactionmaster.HANDL_TAXABLE_AMT = 0; //Convert.ToDecimal(F_Form["HANDL_TAXABLE_AMT"]);

                        transactionmaster.STRG_CGST_EXPRN = 0; // Convert.ToDecimal(F_Form["STRG_CGST_EXPRN"]);
                        transactionmaster.STRG_SGST_EXPRN = 0; // Convert.ToDecimal(F_Form["STRG_SGST_EXPRN"]);
                        transactionmaster.STRG_IGST_EXPRN = 0; //Convert.ToDecimal(F_Form["STRG_IGST_EXPRN"]);
                        transactionmaster.STRG_CGST_AMT = 0; //Convert.ToDecimal(F_Form["STRG_CGST_AMT"]);
                        transactionmaster.STRG_SGST_AMT = 0; //Convert.ToDecimal(F_Form["STRG_SGST_AMT"]);
                        transactionmaster.STRG_IGST_AMT = 0; //Convert.ToDecimal(F_Form["STRG_IGST_AMT"]);

                        transactionmaster.HANDL_CGST_EXPRN = 0; //Convert.ToDecimal(F_Form["HANDL_CGST_EXPRN"]);
                        transactionmaster.HANDL_SGST_EXPRN = 0; //Convert.ToDecimal(F_Form["HANDL_SGST_EXPRN"]);
                        transactionmaster.HANDL_IGST_EXPRN = 0; //Convert.ToDecimal(F_Form["HANDL_IGST_EXPRN"]);
                        transactionmaster.HANDL_CGST_AMT = 0; //Convert.ToDecimal(F_Form["HANDL_CGST_AMT"]);
                        transactionmaster.HANDL_SGST_AMT = 0; //Convert.ToDecimal(F_Form["HANDL_SGST_AMT"]);
                        transactionmaster.HANDL_IGST_AMT = 0; //Convert.ToDecimal(F_Form["HANDL_IGST_AMT"]);

                        transactionmaster.TRANCGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANCGSTAMT"]);
                        transactionmaster.TRANSGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANSGSTAMT"]);
                        transactionmaster.TRANIGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANIGSTAMT"]);

                       

                        string CATEAID = Convert.ToString(F_Form["CATEAID"]);
                        if (CATEAID == "" || CATEAID == null || CATEAID == "0")
                        { transactionmaster.CATEAID = 0; }
                        else { transactionmaster.CATEAID = Convert.ToInt32(CATEAID); }

                        string STATEID = Convert.ToString(F_Form["masterdata[0].STATEID"]);
                        if (STATEID == "" || STATEID == null || STATEID == "0")
                        { transactionmaster.STATEID = 0; }
                        else { transactionmaster.STATEID = Convert.ToInt32(STATEID); }

                        transactionmaster.CATEAGSTNO = Convert.ToString(F_Form["masterdata[0].CATEAGSTNO"]);
                        transactionmaster.TRANIMPADDR1 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR1"]);
                        transactionmaster.TRANIMPADDR2 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR2"]);
                        transactionmaster.TRANIMPADDR3 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR3"]);
                        transactionmaster.TRANIMPADDR4 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR4"]);

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
                        string bill = "";
                        if (Convert.ToInt32(F_Form["TRANBTYPE"]) == 1) bill = "LI";
                        else bill = "DS";
                        if (TRANMID == 0)
                        {
                            //transactionmaster.TRANNO = Convert.ToInt16(Autonumber.autonum("transactionmaster", "TRANNO", "TRANNO<>0 and TRANBTYPE=" + Convert.ToInt16(F_Form["TRANBTYPE"]) + " and REGSTRID=" + Convert.ToInt16(F_Form["REGSTRID"]) + " and SDPTID=2 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                            if (regsid == 49)
                                transactionmaster.TRANNO = Convert.ToInt32(auto_numbr_invoice.gstexportautonum("transactionmaster", "TRANNO", F_Form["REGSTRID"].ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString(), Convert.ToInt32(transactionmaster.TRANREFID)).ToString());
                            else
                                transactionmaster.TRANNO = Convert.ToInt32(auto_numbr_invoice.gstexportautonum_BS("transactionmaster", "TRANNO", F_Form["REGSTRID"].ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString(), Convert.ToInt32(transactionmaster.TRANREFID)).ToString());
                            int ano = transactionmaster.TRANNO;
                            //string format = "SUD/EXP/";
                            string format = "EXP/" ;
                            string btyp = auto_numbr_invoice.GetCateBillType(Convert.ToInt32(transactionmaster.TRANREFID)).ToString();
                            if (btyp == "")
                            {
                                format = format + Session["GPrxDesc"] + "/";
                            }
                            else
                                format = format.Replace("/", "") + Session["GPrxDesc"] + btyp;
                            string billformat = "";
                            switch (regsid)
                            {
                                //case 21: billformat = "STL/MNUL/EXP/"; break; //"EXP/LI/"; break;
                                //case 22: billformat = "STL/MNUL/EXP/"; break; //"EXP/" + bill + "/"; break;
                                //case 23: billformat = "STL/MNUL/EXP/"; break;

                                case 49: billformat = "STL/MNUL/EXP/"; break; //"EXP/LI/"; break;
                                case 50: billformat = "STL/MNUL/EXP/"; break; //"EXP/" + bill + "/"; break;
                                                                              //case 23: billformat = "STL/MNUL/EXP/"; break;
                            }
                            string prfx = string.Format(format + "{0:D5}", ano);
                            string billprfx = string.Format(billformat + "{0:D5}", ano);
                            transactionmaster.TRANDNO = prfx.ToString();
                            transactionmaster.TRANBILLREFNO = billprfx.ToString();

                            //........end of autonumber
                            context.transactionmaster.Add(transactionmaster);
                            context.SaveChanges();
                            TRANMID = transactionmaster.TRANMID;
                        }
                        else
                        {
                            //int ano = transactionmaster.TRANNO;
                            ////string format = "SUD/EXP/";
                            //string format = "EXP/";
                            //string btyp = auto_numbr_invoice.GetCateBillType(Convert.ToInt32(transactionmaster.TRANREFID)).ToString();
                            //if (btyp == "")
                            //{
                            //    format = format + Session["GPrxDesc"] + "/";
                            //}
                            //else
                            //    format = format.Replace("/", "") + Session["GPrxDesc"] + btyp;
                            //string billformat = "";
                            //switch (regsid)
                            //{
                            //    //case 21: billformat = "STL/MNUL/EXP/"; break; //"EXP/LI/"; break;
                            //    //case 22: billformat = "STL/MNUL/EXP/"; break; //"EXP/" + bill + "/"; break;
                            //    //case 23: billformat = "STL/MNUL/EXP/"; break;

                            //    case 49: billformat = "STL/MNUL/EXP/"; break; //"EXP/LI/"; break;
                            //    case 50: billformat = "STL/MNUL/EXP/"; break; //"EXP/" + bill + "/"; break;
                            //                                                  // case 23: billformat = "STL/MNUL/EXP/"; break;
                            //}
                            //string prfx = string.Format(format + "{0:D5}", ano);
                            //string billprfx = string.Format(billformat + "{0:D5}", ano);
                            //transactionmaster.TRANDNO = prfx.ToString();
                            //transactionmaster.TRANBILLREFNO = billprfx.ToString();
                            context.Entry(transactionmaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }

                        //-------------transaction Details
                        string[] F_TRANDID = F_Form.GetValues("TRANDID");
                        string[] TRANDREFNO = F_Form.GetValues("TRANDREFNO");
                        string[] TRANIDATE = F_Form.GetValues("TRANIDATE");
                        string[] TRANSDATE = F_Form.GetValues("TRANSDATE");
                        string[] TRANEDATE = F_Form.GetValues("TRANEDATE");

                        string[] TRANDHSNCODE = F_Form.GetValues("TRANDHSNCODE");
                        string[] TRANDGST = F_Form.GetValues("TRANDGST");

                        string[] TRANDNAMT = F_Form.GetValues("TRANDNAMT");
                        string[] TRANDGAMT = F_Form.GetValues("TRANDGAMT");

                        string[] TRAND_STRG_CGST_EXPRN = F_Form.GetValues("TRAND_STRG_CGST_EXPRN");
                        string[] TRAND_STRG_SGST_EXPRN = F_Form.GetValues("TRAND_STRG_SGST_EXPRN");
                        string[] TRAND_STRG_IGST_EXPRN = F_Form.GetValues("TRAND_STRG_IGST_EXPRN");

                        string[] TRAND_HANDL_CGST_EXPRN = F_Form.GetValues("TRAND_HANDL_CGST_EXPRN");
                        string[] TRAND_HANDL_SGST_EXPRN = F_Form.GetValues("TRAND_HANDL_SGST_EXPRN");
                        string[] TRAND_HANDL_IGST_EXPRN = F_Form.GetValues("TRAND_HANDL_IGST_EXPRN");

                        string[] TRAND_STRG_CGST_AMT = F_Form.GetValues("TRAND_STRG_CGST_AMT");
                        string[] TRAND_STRG_SGST_AMT = F_Form.GetValues("TRAND_STRG_SGST_AMT");
                        string[] TRAND_STRG_IGST_AMT = F_Form.GetValues("TRAND_STRG_IGST_AMT");

                        string[] TRAND_HANDL_CGST_AMT = F_Form.GetValues("TRAND_HANDL_CGST_AMT");
                        string[] TRAND_HANDL_SGST_AMT = F_Form.GetValues("TRAND_HANDL_SGST_AMT");
                        string[] TRAND_HANDL_IGST_AMT = F_Form.GetValues("TRAND_HANDL_IGST_AMT");

                        string[] ACHEADID;
                        //string[] TRANDREFID;
                        string[] TRANDDESC;
                        string[] TRANDREFNAME = F_Form.GetValues("TRANDREFNAME");

                        if (F_Form.GetValues("detaildata[0].ACHEADID") == null)
                        {
                            ACHEADID = F_Form.GetValues("ACHEADID");
                        }
                        else
                        {
                            ACHEADID = F_Form.GetValues("detaildata[0].ACHEADID");
                        }

                        //if (F_Form.GetValues("detaildata[0].TRANDREFID") == null)
                        //{
                        //    TRANDREFID = F_Form.GetValues("TRANDREFID");
                        //}
                        //else
                        //{
                        //    TRANDREFID = F_Form.GetValues("detaildata[0].TRANDREFID");
                        //}

                        if (F_Form.GetValues("detaildata[0].TRANDDESC") == null)
                        {
                            TRANDDESC = F_Form.GetValues("TRANDDESC");
                        }
                        else
                        {
                            TRANDDESC = F_Form.GetValues("detaildata[0].TRANDDESC");
                        }
                        
                        string[] BILLEDID = F_Form.GetValues("detaildata[0].BILLEDID");
                        string[] TRANVHLFROM = F_Form.GetValues("TRANVHLFROM");
                        string[] TRANVHLTO = F_Form.GetValues("TRANVHLTO");

                        for (int count = 0; count < F_TRANDID.Count(); count++)
                        {
                            TRANDID = Convert.ToInt32(F_TRANDID[count]);

                            if (TRANDID != 0)
                            {
                                transactiondetail = context.transactiondetail.Find(TRANDID);
                            }
                            transactiondetail.TRANMID = transactionmaster.TRANMID;
                            transactiondetail.TRANDREFNO = (TRANDHSNCODE[count]).ToString();
                            transactiondetail.TRANDREFNAME = (TRANDREFNAME[count]).ToString();
                            transactiondetail.TRANDREFID = 0; // Convert.ToInt32(TRANDREFID[count]);
                            transactiondetail.TRANDDESC = (TRANDDESC[count]).ToString();
                            transactiondetail.ACHEADID = Convert.ToInt32(ACHEADID[count]);

                            transactiondetail.TRANIDATE = DateTime.Now;  //Convert.ToDateTime(TRANIDATE[count]);
                            transactiondetail.TRANSDATE = DateTime.Now;
                            transactiondetail.TRANEDATE = DateTime.Now;
                            transactiondetail.TRANVDATE = DateTime.Now;
                            transactiondetail.TRANVHLFROM = ""; //(TRANVHLFROM[count]).ToString();
                            transactiondetail.TRANVHLTO = "";// (TRANVHLTO[count]).ToString();

                            transactiondetail.TRANDQTY = Convert.ToDecimal(TRANDGST[count]);

                            transactiondetail.TRAND_STRG_CGST_EXPRN = Convert.ToDecimal(TRAND_STRG_CGST_EXPRN[count]);
                            transactiondetail.TRAND_STRG_SGST_EXPRN = Convert.ToDecimal(TRAND_STRG_SGST_EXPRN[count]);
                            transactiondetail.TRAND_STRG_IGST_EXPRN = Convert.ToDecimal(TRAND_STRG_IGST_EXPRN[count]);

                            transactiondetail.TRAND_HANDL_CGST_EXPRN = Convert.ToDecimal(TRAND_HANDL_CGST_EXPRN[count]);
                            transactiondetail.TRAND_HANDL_SGST_EXPRN = Convert.ToDecimal(TRAND_HANDL_SGST_EXPRN[count]);
                            transactiondetail.TRAND_HANDL_IGST_EXPRN = Convert.ToDecimal(TRAND_HANDL_IGST_EXPRN[count]);

                            transactiondetail.TRAND_STRG_CGST_AMT = Convert.ToDecimal(TRAND_STRG_CGST_AMT[count]);
                            transactiondetail.TRAND_STRG_SGST_AMT = Convert.ToDecimal(TRAND_STRG_SGST_AMT[count]);
                            transactiondetail.TRAND_STRG_IGST_AMT = Convert.ToDecimal(TRAND_STRG_IGST_AMT[count]);

                            transactiondetail.TRAND_HANDL_CGST_AMT = Convert.ToDecimal(TRAND_HANDL_CGST_AMT[count]);
                            transactiondetail.TRAND_HANDL_SGST_AMT = Convert.ToDecimal(TRAND_HANDL_SGST_AMT[count]);
                            transactiondetail.TRAND_HANDL_IGST_AMT = Convert.ToDecimal(TRAND_HANDL_IGST_AMT[count]);

                            transactiondetail.TRANDGAMT = Convert.ToDecimal(TRANDGAMT[count]);
                            transactiondetail.TRANDNAMT = Convert.ToDecimal(TRANDNAMT[count]);

                            transactiondetail.BILLEDID = 0;//Convert.ToInt32(BILLEDID[count]);
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
                        context.Database.ExecuteSqlCommand("DELETE FROM transactiondetail  WHERE TRANMID=" + TRANMID + " and  TRANDID NOT IN(" + DELIDS.Substring(1) + ")");
                        //context.Database.ExecuteSqlCommand("EXEC PR_EXPORT_MANUAL_BILL_TRANMASTER_UPD @PTRANMID =" + TRANMID);
                        trans.Commit();
                        TRANMID = transactionmaster.TRANMID;
                        context.Database.ExecuteSqlCommand("EXEC PR_EXPORT_MANUAL_BILL_TRANMASTER_UPD @PTRANMID =" + TRANMID);
                        Response.Redirect("Index");
                    }
                    catch (Exception EX)
                    {
                        var msg = EX.InnerException;
                        trans.Rollback();
                        Response.Write("Sorry!!An Error Ocurred...");
                    }
                }
            }

        }

        //--------Autocomplete CHA Name
        public JsonResult AutoCha(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct().OrderBy(X => X.CATENAME);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //---End 

        public JsonResult NewAutoCha(string term)
        {

            var result = (from r in context.VW_NEW_CATEGORY_STATETYPE_ASSGNs.Where(m => m.CATETID == 4 || m.CATETID == 2).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID, r.STATETYPE }).Distinct().OrderBy(X => X.CATENAME);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStateType(int? id = 0)
        {
            if (id > 0)
            {
                var result = context.Database.SqlQuery<StateMaster>("Select *from STATEMASTER WHERE STATEID=" + Convert.ToInt32(id)).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = "";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCharge(int id)//Get 
        {
            var result = context.Database.SqlQuery<ChargeMaster>("select * from ChargeMaster where CONTNRSID=" + id).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAmount(int id)//Get 
        {

            var result = context.Database.SqlQuery<ChargeMaster>("select * from ChargeMaster where CHRGID=" + id).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //---End 



        //......end

        //.............................storage ,handling,energy,fuel, and PTI amount...........//
        public JsonResult Bill_Detail(string id)
        {
            var param = id.Split('-');

            var TARIFFMID = Convert.ToInt32(param[0]);

            var CHRGETYPE = Convert.ToInt32(param[1]);
            var CONTNRSID = Convert.ToInt32(param[2]);
            var CHAID = Convert.ToInt32(param[3]);
            if (TARIFFMID == 4)/* INSTEAD OF SLABTID=6 ,,PARAM[4]*/
            {
                var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (" + Convert.ToInt32(param[4]) + ",14,15,16) and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + "and CHAID=" + CHAID).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (" + Convert.ToInt32(param[4]) + ",14,15,16) and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }


        }//.....end

        //............ratecardmaster.....................
        public JsonResult RATECARD(string id)
        {
            var param = id.Split('-');
            var TARIFFMID = Convert.ToInt32(param[0]);
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
            //   DbSqlQuery<CostFactorMaster> data = context.costfactormasters.SqlQuery("select * from costfactormaster  where CFID  in(2) order by CFID DESC");
            //DbSqlQuery<CostFactorMaster> data2 = context.costfactormasters.SqlQuery("select * from costfactormaster  where DISPSTATUS=0 and CFID  in(96,97) order by CFID");
            //data.Concat(data2);
            var data = context.Database.SqlQuery<CostFactorMaster>("select * from costfactormaster  where ((DISPSTATUS=0 and CFID  in(96,97)) or  CFID  in(2)) order by CFID").ToList();
            
            return Json(data, JsonRequestBehavior.AllowGet);

        }//....end

        //..........................Printview...
        [Authorize(Roles = "ExportManualBillPrint")]
        public void PrintView(string id)
        {
            var param = id.Split('~');

            var ids = Convert.ToInt32(param[0]);
            var gsttype = Convert.ToInt32(param[1]);

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "EXPORT_OSBILL", Convert.ToInt32(ids), Session["CUSRID"].ToString());
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

                // if (gsttype == 0)
                // { cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"]+  "EXPORT_OS_Detail.RPT"); }
                // else
                //{ cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"]+  "GST_EXPORT_OS_Detail.RPT"); }
                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "GST_EXPORT_OS_Detail.RPT");

                cryRpt.RecordSelectionFormula = "{VW_EXPORT_OS_DETAIL_RPT.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_OS_DETAIL_RPT.TRANMID} = " + ids;


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
                GC.Collect();
                stringbuilder.Clear();
            }

        }
        //end
        public JsonResult GetGSTRATE(string id)
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

        }
        //...............Delete Row.............
        [Authorize(Roles = "ExportManualBillDelete")]
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