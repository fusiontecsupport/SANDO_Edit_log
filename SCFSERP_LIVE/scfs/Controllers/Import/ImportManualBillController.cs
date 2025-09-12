using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using scfs_erp.Context;
using scfs.Data;
using scfs_erp.Helper;
using scfs_erp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static scfs_erp.Models.EInvoice;

namespace scfs_erp.Controllers.Import
{
    [SessionExpire]
    public class ImportManualBillController : Controller
    {
        // GET: /ImportManualBill/ 

        #region context declaration
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        #endregion

        #region Index FORM
        [Authorize(Roles = "ImportManualBillIndex")]
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
            if (Request.Form.Get("from") != null)
            {
                Session["TRANBTYPE"] = Request.Form.Get("TRANBTYPE");
                Session["REGSTRID"] = Request.Form.Get("REGSTRID");
            }
            else
            {
                Session["TRANBTYPE"] = "0";
                Session["REGSTRID"] = "15";
            }

            if (Session["Group"].ToString() == "Imports")
            {
                ViewBag.aaa = "hide";
            }
            else
            {
                ViewBag.aaa = "hide1";
            }

            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
           
            if (Convert.ToInt32(Session["TRANBTYPE"]) == 0)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = true };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "AUCTION", Value = "7", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "NOTIONAL", Value = "8", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
            }
            else if (Convert.ToInt32(Session["TRANBTYPE"]) == 5)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = true };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "AUCTION", Value = "7", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "NOTIONAL", Value = "8", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);

            }
            else if (Convert.ToInt32(Session["TRANBTYPE"]) == 7)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "AUCTION", Value = "7", Selected = true };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "NOTIONAL", Value = "8", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);

            }
            else if (Convert.ToInt32(Session["TRANBTYPE"]) == 8)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "AUCTION", Value = "7", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "NOTIONAL", Value = "8", Selected = true };
                selectedBILLTYPE.Add(selectedItemGPTY);

            }
            else
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = true };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "AUCTION", Value = "7", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "NOTIONAL", Value = "8", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);


            }
            ViewBag.TRANBTYPE = selectedBILLTYPE;
            //............Billed to....//

            ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == 15 || x.REGSTRID == 37), "REGSTRID", "REGSTRDESC", Convert.ToInt32(Session["REGSTRID"]));
            //.....end


            DateTime sd = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;

            DateTime ed = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;
            return View();
            // return View(context.transactionmaster.Where(x => x.TRANDATE >= sd).Where(x => x.TRANDATE <= ed).ToList());
        }//...End of index grid
        #endregion

        #region GetAjaxData
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_ImportManualBill(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToInt32(Session["REGSTRID"]), Convert.ToInt32(Session["TRANBTYPE"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                //var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANTIME.Value.ToString("hh:mm tt"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANNAMT.ToString(), d.ACKNO , d.DISPSTATUS, d.TRANMID.ToString(),   d.GSTAMT.ToString()  }).ToArray();
                var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANTIME.Value.ToString("hh:mm tt"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANNAMT.ToString(), d.GSTAMT.ToString(), d.ACKNO, d.DISPSTATUS, d.TRANMID.ToString()  }).ToArray();                
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GST FORM Edit
        [Authorize(Roles = "ImportManualBillEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            //Response.Redirect("" + strPath + "/ImportManualBill/GSTForm/" + id);

            Response.Redirect("" + strPath + "/ImportManualBill/MGSTForm/" + id);

            //Response.Redirect("/ImportManualBill/GSTForm/" + id);
        }
        #endregion

        #region MGST FORM EDIT
        //[Authorize(Roles = "ImportManualBillEdit")]
      
        public void MEdit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/ImportManualBill/MGSTForm/" + id);

            //Response.Redirect("/ImportManualBill/GSTForm/" + id);
        }
        #endregion

        #region GST FORM
        [Authorize(Roles = "ImportManualBillCreate")]
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
            //SelectListItem selectedItemDSP = new SelectListItem { Text = "LI", Value = "1", Selected = true };
            //selectedBILLTYPE.Add(selectedItemDSP);
            //selectedItemDSP = new SelectListItem { Text = "DS", Value = "2", Selected = false };
            //selectedBILLTYPE.Add(selectedItemDSP);
            //ViewBag.TRANBTYPE = selectedBILLTYPE;

            if (Convert.ToInt32(Session["TRANBTYPE"]) == 0)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = true };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "AUCTION", Value = "7", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);

            }
            else if (Convert.ToInt32(Session["TRANBTYPE"]) == 5)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = true };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "AUCTION", Value = "7", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);

            }
            else if (Convert.ToInt32(Session["TRANBTYPE"]) == 7)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "AUCTION", Value = "7", Selected = true };
                selectedBILLTYPE.Add(selectedItemGPTY);

            }
            else
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = true };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "7", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);


            }
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

            SelectListItem selectedItemreg = new SelectListItem { Text = "TAX INVOICE", Value = "15", Selected = false };
            selectedregid.Add(selectedItemreg);
            selectedItemreg = new SelectListItem { Text = "BILL OF SUPPLY", Value = "37", Selected = false };
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
                if (Convert.ToInt32(tab.REGSTRID) == 15)
                {
                    SelectListItem selectedItemrege1 = new SelectListItem { Text = "TAX INVOICE", Value = "15", Selected = true };
                    selectedregide_.Add(selectedItemrege1);
                    ViewBag.REGSTRID = selectedregide_;
                }
                else
                {
                    SelectListItem selectedItemrege1 = new SelectListItem { Text = "BILL OF SUPPLY", Value = "37", Selected = true };
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
                List<SelectListItem> selectedBILLTYPE1 = new List<SelectListItem>();
                //if (Convert.ToInt32(tab.TRANBTYPE) == 1)
                //{
                //    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LI", Value = "1", Selected = true };
                //    selectedBILLYPE.Add(selectedItemGPTY);
                //    selectedItemGPTY = new SelectListItem { Text = "DS", Value = "2", Selected = false };
                //    selectedBILLYPE.Add(selectedItemGPTY);

                //}
                //else
                //{
                //    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LI", Value = "1", Selected = false };
                //    selectedBILLYPE.Add(selectedItemGPTY);
                //    selectedItemGPTY = new SelectListItem { Text = "DS", Value = "2", Selected = true };
                //    selectedBILLYPE.Add(selectedItemGPTY);

                //}
                //ViewBag.TRANBTYPE = selectedBILLYPE;
                if (Convert.ToInt32(tab.TRANBTYPE) == 0)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = true };
                    selectedBILLTYPE1.Add(selectedItemGPTY);

                }
                else if (Convert.ToInt32(tab.TRANBTYPE) == 5)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = true };
                    selectedBILLTYPE1.Add(selectedItemGPTY);

                }
                else if (Convert.ToInt32(tab.TRANBTYPE) == 7)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "AUCTION", Value = "7", Selected = true };
                    selectedBILLTYPE1.Add(selectedItemGPTY);

                }
                else
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = true };
                    selectedBILLTYPE1.Add(selectedItemGPTY);


                }
                ViewBag.TRANBTYPE = selectedBILLTYPE1;
                ////..........end
                //............Billed to....//
                //List<SelectListItem> selectedregid_ = new List<SelectListItem>();
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
                vm.impmanualviewdata = context.Database.SqlQuery<pr_Import_Manual_Invoice_Ctrl_Flx_Assgn_Result>("pr_Import_Manual_Invoice_Ctrl_Flx_Assgn @PTRANMID=" + id + "").ToList();//........procedure  for edit mode details data

            }
            return View(vm);
        }
        #endregion

        #region MGST FORM
        [Authorize(Roles = "ImportManualBillCreate")]
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
            ViewBag.TCATEAID = new SelectList("");
            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
            if (Convert.ToInt32(Session["TRANBTYPE"]) == 0)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = true };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "AUCTION", Value = "7", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "NOTIONAL", Value = "8", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
            }
            else if (Convert.ToInt32(Session["TRANBTYPE"]) == 5)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = true };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "AUCTION", Value = "7", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "NOTIONAL", Value = "8", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
            }
            else if (Convert.ToInt32(Session["TRANBTYPE"]) == 7)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "AUCTION", Value = "7", Selected = true };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "NOTIONAL", Value = "8", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
            }
            else if (Convert.ToInt32(Session["TRANBTYPE"]) == 8)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "AUCTION", Value = "7", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "NOTIONAL", Value = "8", Selected = true };
                selectedBILLTYPE.Add(selectedItemGPTY);
            }
            else
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = true };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "AUCTION", Value = "7", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "NOTIONAL", Value = "8", Selected = false };
                selectedBILLTYPE.Add(selectedItemGPTY);
            }
            ViewBag.TRANBTYPE = selectedBILLTYPE;
            //....end

            //............Billed to....//
            List<SelectListItem> selectedregid = new List<SelectListItem>();
            SelectListItem selectedItemreg = new SelectListItem { Text = "TAX INVOICE", Value = "15", Selected = false };
            selectedregid.Add(selectedItemreg);
            selectedItemreg = new SelectListItem { Text = "BILL OF SUPPLY", Value = "37", Selected = false };
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

                var sqy = context.Database.SqlQuery<Category_Address_Details>("Select *from CATEGORY_ADDRESS_DETAIL WHERE CATEAID = " + tab.CATEAID).ToList();
                var starqy = context.Database.SqlQuery<StateMaster>("Select *from STATEMASTER where STATEID = " + tab.STATEID).ToList();
                if (starqy.Count > 0)
                {
                    ViewBag.STATEDESC = starqy[0].STATECODE + "  " + starqy[0].STATEDESC;
                    ViewBag.STATETYPE = starqy[0].STATETYPE;
                }

                if (sqy.Count > 0)
                {
                    ViewBag.CATEAID = new SelectList(context.categoryaddressdetails.Where(x => x.CATEID == tab.TRANREFID), "CATEAID", "CATEATYPEDESC", tab.CATEAID);
                }
                else { ViewBag.CATEAID = new SelectList(""); }


                var zsqy = context.Database.SqlQuery<Category_Address_Details>("Select *from CATEGORY_ADDRESS_DETAIL WHERE CATEAID = " + tab.TCATEAID).ToList();
                if (zsqy.Count > 0)
                {
                    ViewBag.TCATEAID = new SelectList(context.categoryaddressdetails.Where(x => x.CATEID == tab.TRANTALLYCHAID), "CATEAID", "CATEATYPEDESC", tab.TCATEAID);
                }
                else { ViewBag.TCATEAID = new SelectList(""); }

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
                if (Convert.ToInt32(tab.REGSTRID) == 15)
                {
                    SelectListItem selectedItemrege1 = new SelectListItem { Text = "TAX INVOICE", Value = "15", Selected = true };
                    selectedregide_.Add(selectedItemrege1);
                    ViewBag.REGSTRID = selectedregide_;
                }
                else
                {
                    SelectListItem selectedItemrege1 = new SelectListItem { Text = "BILL OF SUPPLY", Value = "37", Selected = true };
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
                List<SelectListItem> selectedBILLTYPE0 = new List<SelectListItem>();

                if (Convert.ToInt32(tab.TRANBTYPE) == 0)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = true };
                    selectedBILLTYPE0.Add(selectedItemGPTY);
                }
                else if (Convert.ToInt32(tab.TRANBTYPE) == 5)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = true };
                    selectedBILLTYPE0.Add(selectedItemGPTY);
                }
                else if (Convert.ToInt32(tab.TRANBTYPE) == 7)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "AUCTION", Value = "7", Selected = true };
                    selectedBILLTYPE0.Add(selectedItemGPTY);
                }
                else if (Convert.ToInt32(tab.TRANBTYPE) == 8)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "NOTIONAL", Value = "8", Selected = true };
                    selectedBILLTYPE0.Add(selectedItemGPTY);
                }
                else
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = true };
                    selectedBILLTYPE0.Add(selectedItemGPTY);
                }
                ViewBag.TRANBTYPE = selectedBILLTYPE0;

                vm.masterdata = context.transactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.detaildata = context.transactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.costfactor = context.transactionmasterfactor.Where(det => det.TRANMID == id).ToList();
                vm.impmanualviewdata = context.Database.SqlQuery<pr_Import_Manual_Invoice_Ctrl_Flx_Assgn_Result>("pr_Import_Manual_Invoice_Ctrl_Flx_Assgn @PTRANMID=" + id + "").ToList();//........procedure  for edit mode details data

            }
            return View(vm);
        }
        #endregion

        #region GST FORM Insert/update values into database
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
                        transactionmaster.SDPTID = 1;
                        transactionmaster.TRANTID = Convert.ToInt16(F_Form["STAX"]);
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
                        var bill = "";
                        //if (Convert.ToInt32(F_Form["TRANBTYPE"]) == 1) bill = "LI";
                        //else bill = "DS";
                        if (Convert.ToInt32(F_Form["TRANBTYPE"]) == 0)
                            bill = "GEN";
                        else if (Convert.ToInt32(F_Form["TRANBTYPE"]) == 5)
                            bill = "MNR";
                        else if (Convert.ToInt32(F_Form["TRANBTYPE"]) == 7)
                            bill = "ACT";
                        else if (Convert.ToInt32(F_Form["TRANBTYPE"]) == 8)
                            bill = "NTL";
                        else bill = "O&M";
                        if (TRANMID == 0)
                        {
                            //transactionmaster.TRANNO = Convert.ToInt16(Autonumber.autonum("transactionmaster", "TRANNO", "TRANNO<>0 and TRANBTYPE=" + Convert.ToInt16(F_Form["TRANBTYPE"]) + " and REGSTRID=" + Convert.ToInt16(F_Form["REGSTRID"]) + " and SDPTID=2 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                            if (regsid == 15)
                                transactionmaster.TRANNO = Convert.ToInt32(auto_numbr_invoice.gstautonum("transactionmaster", "TRANNO", regsid.ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString(), Convert.ToInt32(transactionmaster.TRANREFID)).ToString());
                            //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.gstautonum("transactionmaster", "TRANNO", regsid.ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString()).ToString());
                            else
                                transactionmaster.TRANNO = Convert.ToInt32(auto_numbr_invoice.bsgstautonum("transactionmaster", "TRANNO", regsid.ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString(), Convert.ToInt32(transactionmaster.TRANREFID)).ToString());
                            //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.bsgstautonum("transactionmaster", "TRANNO", regsid.ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString()).ToString());

                            int ano = transactionmaster.TRANNO;
                            //string format = "SUD/IMP/";
                            string format = "";
                            if (regsid == 15)
                            {
                                if (bill == "MNR")
                                    format = "MNR/" ;
                                else if (bill == "O&M")
                                    format = "OM/" ;
                                else if (bill == "ACT")
                                    format = "ACT/";
                                else if (bill == "NTL")
                                    format = "ACT/";
                                else
                                    format = "IMP/" ;
                            }

                            else
                            {
                                if (bill == "MNR")
                                    format = "BSMNR/" ;
                                else if (bill == "O&M")
                                    format = "BSOM/" ;
                                else if (bill == "ACT")
                                    format = "BSACT/";
                                else
                                    format = "BSIMP/" ;
                            }

                            string btyp = auto_numbr_invoice.GetCateBillType(Convert.ToInt32(transactionmaster.TRANREFID)).ToString();
                            if (btyp == "")
                            {
                                format = format + Session["GPrxDesc"] + "/";
                            }
                            else
                                format = format.Replace("/", "") + Session["GPrxDesc"] + btyp;

                            string billformat = "";
                            //switch (regsid)
                            //{
                            //    case 41: billformat = "STL/MNUL/IMP/"; break; //"EXP/LI/"; break;
                            //    case 42: billformat = "STL/MNUL/IMP/"; break; // "EXP/" + bill + "/"; break;
                            //    case 43: billformat = "STL/MNUL/IMP/"; break;

                            //}

                            if (bill == "MNR")
                                billformat = "STL/MNR/";
                            else if (bill == "O&M")
                                billformat = "STL/OM/";
                            else if (bill == "ACT")
                                billformat = "STL/ACT/";
                            else if (bill == "NTL")
                                billformat = "STL/MNCH/";
                            else
                                billformat = "STL/MNUL/";


                            

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

                        string[] BILLEDID = F_Form.GetValues("detaildata[0].BILLEDID");
                        string[] TRANVHLFROM = F_Form.GetValues("TRANVHLFROM");
                       
                        string[] TRANVHLTO = F_Form.GetValues("TRANVHLTO");
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
                        if (F_Form.GetValues("TRANDREFNAME") == null)
                        {
                            TRANDREFNAME = F_Form.GetValues("TRANDREFNAME");
                        }
                        else
                        {
                            TRANDREFNAME = F_Form.GetValues("detaildata[0].TRANDREFNAME");
                        }
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
                            transactiondetail.TRANIDATE = DateTime.Now;  //Convert.ToDateTime(TRANIDATE[count]);
                            transactiondetail.TRANSDATE = DateTime.Now;
                            transactiondetail.TRANEDATE = DateTime.Now;
                            transactiondetail.TRANVDATE = DateTime.Now;
                            //transactiondetail.TRANVHLFROM = (TRANVHLFROM[count]).ToString();
                            //transactiondetail.TRANVHLTO = (TRANVHLTO[count]).ToString();
                            //transactiondetail.TRANDGAMT = Convert.ToDecimal(TRANDGAMT[count]);
                            //transactiondetail.TRANDNAMT = Convert.ToDecimal(TRANDNAMT[count]);
                            //transactiondetail.BILLEDID = Convert.ToInt32(BILLEDID[count]);
                            transactiondetail.TRANVHLFROM = ""; //(TRANVHLFROM[count]).ToString();
                            transactiondetail.TRANVHLTO = "";// (TRANVHLTO[count]).ToString();
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
                        trans.Commit(); Response.Redirect("Index");
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        Response.Write("Sorry!!An Error Ocurred...");
                    }
                }
            }

        }
        #endregion

        #region MGST FORM Insert/Update Values Into DataBase
        public void msavedata(FormCollection F_Form)
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
                        transactionmaster.SDPTID = 1;
                        transactionmaster.TRANTID = Convert.ToInt16(F_Form["STAX"]);
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

                        if (transactionmaster.TRANDATE > Convert.ToDateTime(todayd))
                        {
                            transactionmaster.TRANDATE = Convert.ToDateTime(todayd);
                        }

                        transactionmaster.TRANTIME = Convert.ToDateTime(F_Form["masterdata[0].TRANTIME"]);

                        if (transactionmaster.TRANTIME > Convert.ToDateTime(todaydt))
                        {
                            transactionmaster.TRANTIME = Convert.ToDateTime(todaydt);
                        }

                        transactionmaster.TRANREFID = Convert.ToInt32(F_Form["masterdata[0].TRANREFID"]);
                        transactionmaster.TRANREFNAME = F_Form["masterdata[0].TRANREFNAME"].ToString();
                        transactionmaster.LCATEID = 0;
                        
                        if (TRANMID == 0)
                        {
                            transactionmaster.TRANBTYPE = Convert.ToInt16(F_Form["TRANBTYPE"]);
                        }
                        else
                        {
                            transactionmaster.TRANBTYPE = Convert.ToInt16(F_Form["txtTRANBTYPE"]); 
                        }
                        

                        transactionmaster.REGSTRID = Convert.ToInt16(F_Form["REGSTRID"]);
                        transactionmaster.TRANMODE = Convert.ToInt16(F_Form["TRANMODE"]);
                        transactionmaster.TRANMODEDETL = (F_Form["masterdata[0].TRANMODEDETL"]);
                        transactionmaster.TRANGAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANGAMT"]);
                        transactionmaster.TRANNAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANNAMT"]);
                        transactionmaster.TRANROAMT = 0; //Convert.ToDecimal(F_Form["masterdata[0].TRANROAMT"]);
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

                        transactionmaster.TRANTALLYCHAID = Convert.ToInt32(F_Form["masterdata[0].TRANTALLYCHAID"]);
                        transactionmaster.TRANTALLYCHANAME = F_Form["masterdata[0].TRANTALLYCHANAME"].ToString();
                        transactionmaster.TCATEAGSTNO = Convert.ToString(F_Form["masterdata[0].TCATEAGSTNO"]);

                        string TCATEAID = Convert.ToString(F_Form["TCATEAID"]);
                        if (TCATEAID == "" || TCATEAID == null || TCATEAID == "0")
                        { transactionmaster.TCATEAID = 0; }
                        else { transactionmaster.TCATEAID = Convert.ToInt32(TCATEAID); }

                        string TSTATEID = Convert.ToString(F_Form["masterdata[0].TSTATEID"]);
                        if (TSTATEID == "" || TSTATEID == null || TSTATEID == "0")
                        { transactionmaster.TSTATEID = 0; }
                        else { transactionmaster.TSTATEID = Convert.ToInt32(TSTATEID); }

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
                        var bill = "";
                        //if (Convert.ToInt32(F_Form["TRANBTYPE"]) == 1) bill = "LI";
                        //else bill = "DS";
                        if (Convert.ToInt32(F_Form["TRANBTYPE"]) == 0)
                            bill = "GEN";
                        else if (Convert.ToInt32(F_Form["TRANBTYPE"]) == 5)
                            bill = "MNR";
                        else if (Convert.ToInt32(F_Form["TRANBTYPE"]) == 7)
                            bill = "ACT";
                        else if (Convert.ToInt32(F_Form["TRANBTYPE"]) == 8)
                            bill = "NTL";
                        else bill = "O&M";
                        if (TRANMID == 0)
                        {
                            //transactionmaster.TRANNO = Convert.ToInt16(Autonumber.autonum("transactionmaster", "TRANNO", "TRANNO<>0 and TRANBTYPE=" + Convert.ToInt16(F_Form["TRANBTYPE"]) + " and REGSTRID=" + Convert.ToInt16(F_Form["REGSTRID"]) + " and SDPTID=2 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                            if (regsid == 15)
                                transactionmaster.TRANNO = Convert.ToInt32(auto_numbr_invoice.gstautonum("transactionmaster", "TRANNO", regsid.ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString(), Convert.ToInt32(transactionmaster.TRANREFID)).ToString());
                            //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.gstautonum("transactionmaster", "TRANNO", regsid.ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString()).ToString());
                            else
                                transactionmaster.TRANNO = Convert.ToInt32(auto_numbr_invoice.bsgstautonum("transactionmaster", "TRANNO", regsid.ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString(), Convert.ToInt32(transactionmaster.TRANREFID)).ToString());
                            //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.bsgstautonum("transactionmaster", "TRANNO", regsid.ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString()).ToString());

                            int ano = transactionmaster.TRANNO;
                            //string format = "SUD/IMP/";
                            string format = "";
                            string btyp = auto_numbr_invoice.GetCateBillType(Convert.ToInt32(transactionmaster.TRANREFID)).ToString();
                            if (regsid == 15)
                            {
                                if (bill == "MNR")
                                {
                                    format = "MNR/" + Session["GPrxDesc"] + "/";
                                }
                                else if (bill == "O&M")
                                {
                                    format = "OM/" + Session["GPrxDesc"] + "/";
                                }
                                else if (bill == "ACT")
                                {
                                    format = "ACT/" + Session["GPrxDesc"] + "/";
                                }
                                else
                                {
                                    if (btyp == "")
                                    {
                                        format = "IMP/" + Session["GPrxDesc"] + "/";
                                    }
                                    else
                                    {
                                        format = "IMP" + Session["GPrxDesc"] + btyp;
                                    }
                                    
                                }
                                    
                            }

                            else
                            {
                                if (bill == "MNR")
                                    format = "BSMNR/" + Session["GPrxDesc"] + "/";
                                else if (bill == "O&M")
                                    format = "BSOM/" + Session["GPrxDesc"] + "/";
                                else if (bill == "ACT")
                                    format = "BSACT/" + Session["GPrxDesc"] + "/";
                                else
                                    format = "BSIMP/" + Session["GPrxDesc"] + "/";
                            }

                            string billformat = "";
                            //switch (regsid)
                            //{
                            //    case 41: billformat = "STL/MNUL/IMP/"; break; //"EXP/LI/"; break;
                            //    case 42: billformat = "STL/MNUL/IMP/"; break; // "EXP/" + bill + "/"; break;
                            //    case 43: billformat = "STL/MNUL/IMP/"; break;

                            //}

                            if (bill == "MNR")
                                billformat = "STL/MNR/";
                            else if (bill == "O&M")
                                billformat = "STL/OM/";
                            else if (bill == "ACT")
                                billformat = "STL/ACT/";
                            else if (bill == "NTL")
                                billformat = "STL/MNCH/";
                            else
                                billformat = "STL/MNUL/";



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

                        string[] BILLEDID = F_Form.GetValues("detaildata[0].BILLEDID");
                        string[] TRANVHLFROM = F_Form.GetValues("TRANVHLFROM");
                        string[] TRANVHLTO = F_Form.GetValues("TRANVHLTO");
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
                        //context.Database.ExecuteSqlCommand("EXEC PR_IMPORT_MANUAL_BILL_TRANMASTER_UPD @PTRANMID =" + TRANMID);
                        trans.Commit();
                        TRANMID = transactionmaster.TRANMID;

                        context.Database.ExecuteSqlCommand("EXEC PR_IMPORT_MANUAL_BILL_TRANMASTER_UPD @PTRANMID =" + TRANMID);
                        Response.Redirect("Index");
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        Response.Write("Sorry!!An Error Ocurred...");
                    }
                }
            }

        }
        #endregion

        #region GetStateType
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
        #endregion

        #region datediff
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
        #endregion

        #region PrintView
        [Authorize(Roles = "ImportManualBillPrint")]
        public void PrintView(string id)
        {
            var param = id.Split('~');

            var ids = Convert.ToInt32(param[0]);
            var gsttype = Convert.ToInt32(param[1]);

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "IMPORT_OSBILL", Convert.ToInt32(ids), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                //ReportDocument cryRpt = new ReportDocument();
                using (var cryRpt = new ReportDocument())
                {
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

                    //if (gsttype == 0)
                    //{ cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "IMPORT_OS_Detail.RPT"); }
                    //else
                    { cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "GST_IMPORT_OS_Detail.RPT"); }

                    cryRpt.RecordSelectionFormula = "{VW_IMPORT_OS_DETAIL_RPT_GST.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_IMPORT_OS_DETAIL_RPT_GST.TRANMID} = " + ids;


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

        }
        [Authorize(Roles = "ImportManualBillPrint")]
        public void APrintView(int id)
        {
            // Response.Write(@"10.10.5.5"); Response.End();
            //  ........delete TMPRPT...//
            //var param = id.Split(';');

            var ids = id;


            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "IMPORTINVOICE", Convert.ToInt32(ids), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                //ReportDocument cryRpt = new ReportDocument();
                using (var cryRpt = new ReportDocument())
                {
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

                    //if (rpttype == 0)
                    cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "GST_Import_OS_Detail_E01.RPT");
                    //else cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_group_rpt_E01.RPT");

                    cryRpt.RecordSelectionFormula = "{VW_IMPORT_OS_DETAIL_RPT_GST.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_IMPORT_OS_DETAIL_RPT_GST.TRANMID} = " + ids;



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
                    //string path = "E:\\CFS\\" + Session["CUSRID"] + "\\ImportInv";
                    //if (!(Directory.Exists(path)))
                    //{
                    //    Directory.CreateDirectory(path);
                    //}
                    //cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, path + "\\" + Query[0].TRANNO + ".pdf");
                    //  cryRpt.SaveAs(path+ "\\"+Query[0].TRANNO+".pdf");
                    cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                    cryRpt.Close(); 
                    cryRpt.Dispose();

                    GC.Collect();
                    stringbuilder.Clear();

                }
               
            }

        }
        #endregion

        #region OTHERPrintView
        public void OTHERPrintView(int? id = 0)
        {

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM NEW_TMPRPT_IDS WHERE KUSRID ='" + Session["CUSRID"] + "'");
            //context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS");
            var TMPRPT_IDS = TMP_InsertPrint.NewInsertToTMP("NEW_TMPRPT_IDS", "OPTNSTR", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                //ReportDocument cryRpt = new ReportDocument();
                using (var cryRpt = new ReportDocument())
                {
                    TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                    TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                    ConnectionInfo crConnectionInfo = new ConnectionInfo();
                    Tables CrTables;

                    // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");

                    ////........Get TRANPCOUNT...//
                    //var Query = context.Database.SqlQuery<int>("select QMPCOUNT from QUOTATIONMASTER where QMID=" + id).ToList();
                    //var PCNT = 0;

                    //if (Query.Count() != 0) { PCNT = Query[0]; }
                    //var TRANPCOUNT = ++PCNT;
                    //// Response.Write(++PCNT);
                    //// Response.End();

                    //context.Database.ExecuteSqlCommand("UPDATE QUOTATIONMASTER SET QMPCOUNT=" + TRANPCOUNT + " WHERE QMID=" + id);

                    string rptname = "";
                    string QMDNO = id.ToString();
                    //var qmbtype = 0;
                    //var statetype = 0;
                    string hstr = "";
                    //var result = context.Database.SqlQuery<TransactionMaster>("Select * From TransactionMaster where TRANMID=" + id).ToList();
                    //if (result.Count() != 0) { QMDNO = result[0].TRANNO.ToString(); }

                    var strPath = ConfigurationManager.AppSettings["Reporturl"];

                    var pdfPath = ConfigurationManager.AppSettings["pdfurl"];

                    //cryRpt.Load("d:\\Reports\\VBJReports\\PurchaseOrder.Rpt");

                    rptname = "E_1003.rpt";
                    hstr = "{VW_IMPORT_EINVOICE_MANUAL_BILL_DETAIL_RPT.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_IMPORT_EINVOICE_MANUAL_BILL_DETAIL_RPT.TRANMID} = " + id;

                    cryRpt.Load(strPath + "\\" + rptname);

                    cryRpt.RecordSelectionFormula = hstr;

                    String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
                    SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
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

                    //PictureObject picture = new PictureObject();
                    //string path = "D:\\KGK\\" + Session["CUSRID"] + "\\Quotation";
                    string path = pdfPath + "\\" + Session["CUSRID"] + "\\import";
                    if (!(Directory.Exists(path)))
                    {
                        Directory.CreateDirectory(path);
                    }
                    cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, path + "\\" + QMDNO + ".pdf");

                    cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                    //cryRpt.PrintToPrinter(1,false,0,0);
                    cryRpt.Close();
                    cryRpt.Dispose();

                    GC.Collect();
                    stringbuilder.Clear();

                }
                   
            }

        }
        #endregion

        #region Del
        [Authorize(Roles = "ImportManualBillDelete")]
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

        }
        #endregion

        #region GetItemList
        private List<ItemList> GetItemList(int id)
        {
            SqlDataReader reader = null;
            string _connStr = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(_connStr);

            SqlCommand sqlCmd = new SqlCommand("pr_EInvoice_Import_Manual_Transaction_Detail_Assgn", myConnection);
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
        #endregion

        #region AutoCha
        public JsonResult AutoCha(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct().OrderBy(X => X.CATENAME);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region AutoTallyCha
        public JsonResult AutoTallyCha(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 1 || m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct().OrderBy(X => X.CATENAME);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region DefaultCF
        public JsonResult DefaultCF()
        {
            //   DbSqlQuery<CostFactorMaster> data = context.costfactormasters.SqlQuery("select * from costfactormaster  where CFID  in(2) order by CFID DESC");
            //DbSqlQuery<CostFactorMaster> data2 = context.costfactormasters.SqlQuery("select * from costfactormaster  where DISPSTATUS=0 and CFID  in(96,97) order by CFID");
            //data.Concat(data2);
            var data = context.Database.SqlQuery<CostFactorMaster>("select * from costfactormaster  where ((DISPSTATUS=0 and CFID  in(96,97)) or  CFID  in(2)) order by CFID").ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region GetGSTRATE
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
                var query = context.Database.SqlQuery<VW_IMPORT_SLABTYPE_HSN_DETAIL_ASSGN>("select HSNCODE,CGSTEXPRN,SGSTEXPRN,IGSTEXPRN from VW_IMPORT_SLABTYPE_HSN_DETAIL_ASSGN where  TARIFFMID = 2 AND SLABTID=" + SlabTId + " order by HSNCODE").ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var query = context.Database.SqlQuery<VW_IMPORT_SLABTYPE_HSN_DETAIL_ASSGN>("select HSNCODE,ACGSTEXPRN as CGSTEXPRN,ASGSTEXPRN as SGSTEXPRN,AIGSTEXPRN as IGSTEXPRN from VW_IMPORT_SLABTYPE_HSN_DETAIL_ASSGN where TARIFFMID = 2 AND SLABTID=" + SlabTId + " order by HSNCODE").ToList();

                return Json(query, JsonRequestBehavior.AllowGet);


            }

        }
        #endregion

        //#region CInvoice
        //// [Authorize(Roles = "ImportEInvoice")]
        //public ActionResult CInvoice(int id = 0)/*10rs.reminder*/
        //{

        //    SqlDataReader reader = null;

        //    string _connStr = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        //    SqlConnection myConnection = new SqlConnection(_connStr);

        //    string _SconnStr = ConfigurationManager.ConnectionStrings["SCFSERPContext"].ConnectionString;
        //    SqlConnection SmyConnection = new SqlConnection(_SconnStr);

        //    var tranmid = id;// Convert.ToInt32(Request.Form.Get("id"));// Convert.ToInt32(ids);

        //    SqlCommand sqlCmd = new SqlCommand();
        //    sqlCmd.CommandType = CommandType.Text;
        //    sqlCmd.CommandText = "Select * from Z_IMPORT_MANUAL_EINVOICE_DETAILS Where TRANMID = " + tranmid;
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
        //#endregion

        //#region UInvoice
        //public void UInvoice(int id = 0)/*10rs.reminder*/
        //{

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

        //#endregion

        #region AForm
        public ActionResult AForm(string id)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            var param = id.Split('~');

            var ids = Convert.ToInt32(param[0]);
            //var gstamt = Convert.ToInt32(param[1]);

            ViewBag.id = ids;
            //ViewBag.FGSTAMT = gstamt;

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

        #endregion

        #region  UpdateEMailMobile
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

        #endregion

        #region GetHsncodedetail
        public JsonResult GetHsncodedetail(int id)
        {
            string squery = "Select *From VW_ACCOUNTHEADER_HSNCODE_DETAIL WHERE ACHEADID = " + id;

            var sql = context.Database.SqlQuery<VW_ACCOUNTHEADER_HSNCODE_DETAIL>(squery).ToList();                       

            return Json(sql, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}