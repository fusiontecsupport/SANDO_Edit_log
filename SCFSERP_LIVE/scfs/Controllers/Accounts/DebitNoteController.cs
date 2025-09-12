using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
//using FusionApp001.Data;
using Newtonsoft.Json;
using System.Data;
using scfs_erp;
using scfs.Data;
using scfs.Models;
//using static FusionApp001.Models.EInvoice;

namespace scfs.Controllers.Accounts
{
    [SessionExpire]
    public class DebitNoteController : Controller
    {
        // GET: DebitNote
        #region Context declaration
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERPContext"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

        #endregion
        #region Debit Note Index
        //[Authorize(Roles = "ManualDebitNoteEInvoiceIndex")] 
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

            if (Request.Form.Get("SDPTID") != null)
            {
                Session["SDPTID"] = Request.Form.Get("SDPTID");
                ViewBag.SDPTID = new SelectList(context.softdepartmentmasters.Where(x => x.SDPTID == 1 || x.SDPTID == 2 || x.SDPTID == 12).OrderBy(x => x.SDPTID), "SDPTID", "SDPTNAME", Convert.ToInt32(Session["SDPTID"]));
            }
            else
            {
                Session["SDPTID"] = "1";
                ViewBag.SDPTID = new SelectList(context.softdepartmentmasters.Where(x => x.SDPTID == 1 || x.SDPTID == 2 || x.SDPTID == 12).OrderBy(x => x.SDPTID), "SDPTID", "SDPTNAME");
            }
            Session["REGSTRID"] = 61;

            DateTime sd = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;
            DateTime ed = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;
            return View();
        }
        #endregion


        #region Get Ajax data
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Finance_DebitNote(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                   totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToInt32(Session["SDPTID"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), 1);
                var aaData = data.Select(d => new string[] {
                    d.TRANDATE.ToString(),
                    d.TRANDNO.ToString(),
                    d.TRANBILLREFNO,
                    d.TRANREFNAME,
                    d.TRANGSTAMT.ToString(),
                    d.TRANNAMT.ToString(),
                    d.DISPSTATUS,
                    d.ACKNO,
                    d.TRANMID.ToString()  }).ToArray();
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

        #region Editpage
        //[Authorize(Roles = "ManualDebitNoteEInvoiceEdit")]
        public void Edit(int id)
        {
            Response.Redirect("~/DebitNote/CNForm/" + id);

        }
        #endregion

        #region Details Form
        //[Authorize(Roles = "ManualDebitNoteEInvoiceCreate")]
        public ActionResult CNForm(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            DebitNote_TransactionMaster tab = new DebitNote_TransactionMaster();
            DebitNoteMD vm = new DebitNoteMD();


            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItemDSP);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            ViewBag.CATEAID = new SelectList("");

            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE == 6), "TRANMODE", "TRANMODEDETL", 6);
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.TRANDREFID = new SelectList(context.debitnotetypemaster.OrderBy(x => x.DNTID), "DNTID", "DNTDESC");
            ViewBag.SDPTID = new SelectList(context.softdepartmentmasters.Where(x => x.SDPTID == 1 || x.SDPTID == 2 || x.SDPTID == 9 || x.SDPTID == 11 || x.SDPTID == 12 || x.SDPTID == 13 || x.SDPTID == 10).OrderBy(x => x.SDPTID), "SDPTID", "SDPTNAME");

            if (id != 0)//....Edit Mode
            {
                tab = context.debitnotetransactionmaster.Find(id);

                ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE == 6), "TRANMODE", "TRANMODEDETL", tab.TRANMODE);
                ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC", tab.BANKMID);
                //var sqy = context.Database.SqlQuery<Category_Address_Details>("Select *from CATEGORY_ADDRESS_DETAIL WHERE CATEAID=" + tab.CATEAID).ToList();
                //if (sqy.Count > 0)
                //{
                //    ViewBag.CATEAID = new SelectList(context.categoryaddressdetails.Where(x => x.CATEAID > 0), "CATEAID", "CATEATYPEDESC", tab.CATEAID);
                //}
                //else { ViewBag.CATEAID = new SelectList(""); }

                ViewBag.SDPTID = new SelectList(context.softdepartmentmasters.Where(x => x.SDPTID == 1 || x.SDPTID == 2 || x.SDPTID == 9 || x.SDPTID == 11 || x.SDPTID == 12 || x.SDPTID == 13 || x.SDPTID == 10).OrderBy(x => x.SDPTID), "SDPTID", "SDPTNAME", tab.SDPTID);

                vm.masterdata = context.debitnotetransactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.detaildata = context.debitnotetransactiondetail.Where(det => det.TRANMID == id).ToList();
                if (vm.detaildata.Count > 0)
                {
                    ViewBag.TRANDREFID = new SelectList(context.debitnotetypemaster.OrderBy(x => x.DNTID), "DNTID", "DNTDESC", Convert.ToInt32(vm.detaildata[0].TRANDREFID));
                }
                else
                {
                    ViewBag.TRANDREFID = new SelectList(context.debitnotetypemaster.OrderBy(x => x.DNTID), "DNTID", "DNTDESC");
                }

            }
            return View(vm);
        }
        #endregion

        #region Autocomplete CHA Name
        public JsonResult AutoCha(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct().OrderBy(X => X.CATENAME);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Transaction Detail
        public JsonResult GetDebitNotedetail(string id)
        {
            int TRANMID = 0; int SDPTID = 0; int CHAID = 0;


            if (id.Contains('-'))
            {
                var Param = id.Split('-');

                if (Param[0] != "" || Param[0] != null)
                { TRANMID = Convert.ToInt32(Param[0]); }
                else { TRANMID = 0; }

                if (Param[1] != "" || Param[1] != null)
                { SDPTID = Convert.ToInt32(Param[1]); }
                else { SDPTID = 0; }

                if (Param[2] != "" || Param[2] != null)
                { CHAID = Convert.ToInt32(Param[2]); }
                else { CHAID = 0; }
            }
            var result = context.Database.SqlQuery<pr_Get_Finance_Transaction_Details_Result>("EXEC  [dbo].[pr_Get_Finance_Transaction_Details] @PTRANMID = " + TRANMID + "  , @PSDPTID = " + SDPTID + " , @PCHAID = " + CHAID + "").ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SaveData
        public void SaveData(FormCollection F_Form)
        {
            string status = "";
            using (SCFSERPContext context = new SCFSERPContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        DebitNote_TransactionMaster debitnotemaster = new DebitNote_TransactionMaster();
                        DebitNote_TransactionDetail debitnotedetail = new DebitNote_TransactionDetail();
                        //-------Getting Primarykey field--------
                        Int32 TRANMID = Convert.ToInt32(F_Form["masterdata[0].TRANMID"]);
                        Int32 TRANDID = 0;
                        string DELIDS = "";
                        //-----End


                        if (TRANMID != 0)
                        {
                            debitnotemaster = context.debitnotetransactionmaster.Find(TRANMID);
                        }

                        //...........transaction master.............//
                        debitnotemaster.TRANMID = TRANMID;
                        debitnotemaster.COMPYID = Convert.ToInt32(Session["compyid"]);
                        string SDPTID = Convert.ToString(F_Form["SDPTID"]);
                        if (SDPTID == "" || SDPTID == null || SDPTID == "0")
                        { debitnotemaster.SDPTID = 0; }
                        else { debitnotemaster.SDPTID = Convert.ToInt32(SDPTID); }

                        debitnotemaster.TRANTID = 2;
                        debitnotemaster.TRANLMID = 0;
                        debitnotemaster.TRANLSID = 0;
                        debitnotemaster.TRANLSNO = null;
                        debitnotemaster.TRANLMNO = "";
                        debitnotemaster.TRANLMDATE = DateTime.Now;
                        debitnotemaster.TRANLSDATE = DateTime.Now;
                        debitnotemaster.TRANNARTN = null;

                        var cusr = debitnotemaster.CUSRID;
                        if (TRANMID.ToString() == "0" || cusr == null || cusr == "")
                            debitnotemaster.CUSRID = Session["CUSRID"].ToString();
                        debitnotemaster.LMUSRID = Session["CUSRID"].ToString();
                        debitnotemaster.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
                        debitnotemaster.PRCSDATE = DateTime.Now;
                        debitnotemaster.TRANDATE = Convert.ToDateTime(F_Form["masterdata[0].TRANDATE"]).Date;
                        debitnotemaster.TRANTIME = DateTime.Now;
                        debitnotemaster.TRANREFID = Convert.ToInt32(F_Form["masterdata[0].TRANREFID"]);//bill id
                        debitnotemaster.TRANREFNAME = F_Form["masterdata[0].TRANREFNAME"].ToString();//bill no
                        debitnotemaster.LCATEID = 0;
                        debitnotemaster.TRANBTYPE = 1;// Convert.ToInt16(F_Form["TRANBTYPE"]);
                        debitnotemaster.REGSTRID = 62;
                        //if (SDPTID == "" || SDPTID == null || SDPTID == "0")
                        //{ creditnotemaster.REGSTRID = 0; }
                        //else
                        //{

                        //    if (SDPTID == "1")
                        //    {

                        //    }
                        //    else if (SDPTID == "2")
                        //    {
                        //        creditnotemaster.REGSTRID = 2;
                        //    }
                        //    else if (SDPTID == "9")
                        //    {
                        //        creditnotemaster.REGSTRID = 9;
                        //    }
                        //    else if (SDPTID == "11")
                        //    {
                        //        creditnotemaster.REGSTRID = 11;
                        //    }
                        //}

                        string TRANMODE = Convert.ToString(F_Form["TRANMODE"]);
                        if (TRANMODE == "" || TRANMODE == null || TRANMODE == "0")
                        { debitnotemaster.TRANMODE = 0; }
                        else { debitnotemaster.TRANMODE = Convert.ToInt16(TRANMODE); }
                        //creditnotemaster.TRANMODE = Convert.ToInt16(F_Form["TRANMODE"]);

                        debitnotemaster.TRANMODEDETL = F_Form["masterdata[0].TRANMODEDETL"].ToString();
                        debitnotemaster.TRANGAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANGAMT"]);
                        debitnotemaster.TRANNAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANNAMT"]);
                        debitnotemaster.TRANROAMT = 0;// Convert.ToDecimal(F_Form["masterdata[0].TRANROAMT"]);
                        string TRANREFAMT = Convert.ToString(F_Form["masterdata[0].TRANREFAMT"]);
                        if (TRANREFAMT == "" || TRANREFAMT == null || TRANREFAMT == "0")
                        { debitnotemaster.TRANREFAMT = 0; }
                        else { debitnotemaster.TRANREFAMT = Convert.ToDecimal(TRANREFAMT); }

                        //creditnotemaster.TRANREFAMT =  Convert.ToDecimal(F_Form["masterdata[0].TRANREFAMT"]);

                        debitnotemaster.TRANRMKS = F_Form["masterdata[0].TRANRMKS"].ToString();
                        string amountinwords = AmtInWrd.ConvertNumbertoWords(F_Form["masterdata[0].TRANNAMT"]).ToString();
                        if (amountinwords != "")
                        { debitnotemaster.TRANAMTWRDS = amountinwords.ToString(); }
                        else { debitnotemaster.TRANAMTWRDS = "-"; }

                        //creditnotemaster.TRANAMTWRDS = Convert.ToString(AmtInWrd.ConvertNumbertoWords(F_Form["masterdata[0].TRANNAMT"]));
                        debitnotemaster.TRANINOC1 = 0; // Convert.ToDecimal(F_Form["masterdata[0].TRANINOC1"]);
                        debitnotemaster.TRANINOC2 = 0;// Convert.ToDecimal(F_Form["masterdata[0].TRANINOC2"]);
                        debitnotemaster.TRANSAMT = 0;// Convert.ToDecimal(F_Form["TRANSAMT"]);
                        debitnotemaster.TRANAAMT = 0;// Convert.ToDecimal(F_Form["TRANAAMT"]);
                        debitnotemaster.TRANHAMT = 0;// Convert.ToDecimal(F_Form["TRANHAMT"]);
                        debitnotemaster.TRANEAMT = 0;// Convert.ToDecimal(F_Form["TRANEAMT"]);
                        debitnotemaster.TRANFAMT = 0;// Convert.ToDecimal(F_Form["TRANFAMT"]);
                        debitnotemaster.TRANTCAMT = 0;// Convert.ToDecimal(F_Form["TRANTCAMT"]);

                        debitnotemaster.STRG_HSNCODE = "-";// F_Form["STRG_HSN_CODE"].ToString();
                        debitnotemaster.HANDL_HSNCODE = "-";// F_Form["HANDL_HSN_CODE"].ToString();

                        debitnotemaster.STRG_TAXABLE_AMT = 0;// Convert.ToDecimal(F_Form["STRG_TAXABLE_AMT"]);
                        debitnotemaster.HANDL_TAXABLE_AMT = 0;// Convert.ToDecimal(F_Form["HANDL_TAXABLE_AMT"]);
                        string[] TRAND_STRG_CGST_EXPRN = F_Form.GetValues("TRAND_STRG_CGST_EXPRN");
                        string[] TRAND_STRG_IGST_EXPRN = F_Form.GetValues("TRAND_STRG_IGST_EXPRN");
                        string[] TRAND_STRG_SGST_EXPRN = F_Form.GetValues("TRAND_STRG_SGST_EXPRN");
                        string[] TRAND_HANDL_CGST_EXPRN = F_Form.GetValues("TRAND_HANDL_CGST_EXPRN");
                        string[] TRAND_HANDL_IGST_EXPRN = F_Form.GetValues("TRAND_HANDL_IGST_EXPRN");
                        string[] TRAND_HANDL_SGST_EXPRN = F_Form.GetValues("TRAND_HANDL_SGST_EXPRN");
                        string[] TRAND_STRG_CGST_AMT = F_Form.GetValues("TRAND_STRG_CGST_AMT");
                        string[] TRAND_STRG_IGST_AMT = F_Form.GetValues("TRAND_STRG_IGST_AMT");
                        string[] TRAND_STRG_SGST_AMT = F_Form.GetValues("TRAND_STRG_SGST_AMT");
                        string[] TRAND_HANDL_CGST_AMT = F_Form.GetValues("TRAND_HANDL_CGST_AMT");
                        string[] TRAND_HANDL_IGST_AMT = F_Form.GetValues("TRAND_HANDL_IGST_AMT");
                        string[] TRAND_HANDL_SGST_AMT = F_Form.GetValues("TRAND_HANDL_SGST_AMT");
                        //--------------------- Stroage Expression --------------------------------------------//
                        debitnotemaster.STRG_CGST_EXPRN = Convert.ToDecimal(TRAND_STRG_CGST_EXPRN[0]);
                        debitnotemaster.STRG_IGST_EXPRN = Convert.ToDecimal(TRAND_STRG_IGST_EXPRN[0]);
                        debitnotemaster.STRG_SGST_EXPRN = Convert.ToDecimal(TRAND_STRG_SGST_EXPRN[0]);
                        //--------------------- Stroage Amount --------------------------------------------//
                        debitnotemaster.STRG_CGST_AMT = Convert.ToDecimal(TRAND_STRG_CGST_AMT[0]);
                        debitnotemaster.STRG_IGST_AMT = Convert.ToDecimal(TRAND_STRG_IGST_AMT[0]);
                        debitnotemaster.STRG_SGST_AMT = Convert.ToDecimal(TRAND_STRG_SGST_AMT[0]);

                        debitnotemaster.HANDL_CGST_EXPRN = 0;
                        debitnotemaster.HANDL_SGST_EXPRN = 0;
                        debitnotemaster.HANDL_IGST_EXPRN = 0;
                        debitnotemaster.HANDL_CGST_AMT = 0;
                        debitnotemaster.HANDL_SGST_AMT = 0;
                        debitnotemaster.HANDL_IGST_AMT = 0;

                        debitnotemaster.TRANCGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANCGSTAMT"]);
                        debitnotemaster.TRANSGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANSGSTAMT"]);
                        debitnotemaster.TRANIGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANIGSTAMT"]);
                        debitnotemaster.TRAN_COVID_DISC_AMT = 0; // Convert.ToDecimal(F_Form["masterdata[0].TRAN_COVID_DISC_AMT"]);

                        debitnotemaster.TRANNARTN = "-";//Convert.ToString(F_Form["masterdata[0].TRANNARTN"]);

                        string CATEAID = "";// Convert.ToString(F_Form["CATEAID"]);
                        if (CATEAID == "" || CATEAID == null || CATEAID == "0")
                        { debitnotemaster.CATEAID = 0; }
                        else { debitnotemaster.CATEAID = Convert.ToInt32(CATEAID); }

                        string STATEID = Convert.ToString(F_Form["masterdata[0].STATEID"]);
                        if (STATEID == "" || STATEID == null || STATEID == "0")
                        { debitnotemaster.STATEID = 0; }
                        else { debitnotemaster.STATEID = Convert.ToInt32(STATEID); }

                        debitnotemaster.CATEAGSTNO = Convert.ToString(F_Form["masterdata[0].CATEAGSTNO"]);
                        debitnotemaster.TRANIMPADDR1 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR1"]);
                        debitnotemaster.TRANIMPADDR2 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR2"]);
                        debitnotemaster.TRANIMPADDR3 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR3"]);
                        debitnotemaster.TRANIMPADDR4 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR4"]);

                        if (TRANMODE != "2" && TRANMODE != "3")
                        {
                            debitnotemaster.TRANREFNO = "";
                            debitnotemaster.TRANREFBNAME = "";
                            debitnotemaster.BANKMID = 0;
                            debitnotemaster.TRANREFDATE = DateTime.Now;
                        }
                        else
                        {
                            debitnotemaster.TRANREFNO = (F_Form["masterdata[0].TRANREFNO"]).ToString();
                            debitnotemaster.TRANREFBNAME = (F_Form["masterdata[0].TRANREFBNAME"]).ToString();
                            debitnotemaster.BANKMID = Convert.ToInt32(F_Form["BANKMID"]);
                            debitnotemaster.TRANREFDATE = Convert.ToDateTime(F_Form["masterdata[0].TRANREFDATE"]).Date;
                        }


                        //.................Autonumber............//
                        var sdptid = Convert.ToInt32(F_Form["SDPTID"]);

                        string billformat = "";

                        if (TRANMID == 0)
                        {

                            debitnotemaster.TRANNO = Convert.ToInt32(Autonumber.manualcreditnote("DebitNote_TransactionMaster", "TRANNO", sdptid.ToString(), Session["compyid"].ToString(), debitnotemaster.TRANBTYPE.ToString()).ToString());

                            //creditnotemaster.TRANNO = Convert.ToInt32(Autonumber.autonum("CreditNote_TransactionMaster", "TRANNO", "TRANNO <> 0 and compyid = " + Convert.ToInt32(Session["compyid"]) + "").ToString());

                            int ano = debitnotemaster.TRANNO;
                            if (sdptid == 1)
                            {
                                billformat = "DR/IMP/";
                            }
                            else if (sdptid == 2)
                            {
                                billformat = "DR/EXP/";
                            }

                            else if (sdptid == 9)
                            {
                                billformat = "DR/NPN/";
                            }

                            else if (sdptid == 11)
                            {
                                billformat = "DR/ESL/";
                            }

                            else if (sdptid == 12)
                            {
                                billformat = "DR/BWH/";
                            }

                            else if (sdptid == 13)
                            {
                                billformat = "DR/MNR/";
                            }


                            string prfx = string.Format("{0:D4}", ano);
                            debitnotemaster.TRANDNO = prfx.ToString();

                            string billprfx = string.Format(billformat + Session["GPrxDesc"] + "/" + "{0:D4}", ano);
                            debitnotemaster.TRANBILLREFNO = billprfx.ToString();

                            context.debitnotetransactionmaster.Add(debitnotemaster);
                            context.SaveChanges();
                            TRANMID = debitnotemaster.TRANMID;
                        }
                        else
                        {
                            int ano = debitnotemaster.TRANNO;
                            if (sdptid == 1)
                            {
                                billformat = "DR/IMP/";
                            }
                            else if (sdptid == 2)
                            {
                                billformat = "DR/EXP/";
                            }

                            else if (sdptid == 9)
                            {
                                billformat = "DR/NPN/";
                            }

                            else if (sdptid == 11)
                            {
                                billformat = "DR/ESL/";
                            }

                            else if (sdptid == 12)
                            {
                                billformat = "DR/BWH/";
                            }

                            else if (sdptid == 13)
                            {
                                billformat = "DR/MNR/";
                            }


                            string prfx = string.Format("{0:D4}", ano);
                            debitnotemaster.TRANDNO = prfx.ToString();

                            string billprfx = string.Format(billformat + Session["GPrxDesc"] + "/" + "{0:D4}", ano);
                            debitnotemaster.TRANBILLREFNO = billprfx.ToString();
                            context.Entry(debitnotemaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }


                        //-------------transaction Details
                        string[] F_TRANDID = F_Form.GetValues("TRANDID");

                        string[] TRANDREFNO = F_Form.GetValues("TRANDREFNO");
                        string[] TRANDAID = F_Form.GetValues("TRANDAID");
                        //string[] TRANDREFID = F_Form.GetValues("TRANDREFID");

                        string[] TRANDREFID;
                        TRANDREFID = F_Form.GetValues("TRANDREFID");
                        if (TRANDREFID == null)
                            TRANDREFID = F_Form.GetValues("detaildata[0].TRANDREFID");

                        //string[] TRANDREFNAME = F_Form.GetValues("TRANDREFNAME");
                        string[] TRANIDATE = F_Form.GetValues("TRANIDATE");
                        string[] TRANDDESC = F_Form.GetValues("TRANDDESC");
                        string[] TRANDGAMT = F_Form.GetValues("TRANDGAMT");
                        string[] TRANDNAMT = F_Form.GetValues("TRANDNAMT");
                        string[] TRANDHSNCODE = F_Form.GetValues("TRANDHSNCODE");
                        string[] NOD = F_Form.GetValues("TTRANDGST");//days                        
                        string[] TRANDRATE = F_Form.GetValues("TRANDRATE");

                        //string[] TRANSDATE = F_Form.GetValues("TRANSDATE");
                        //string[] TRANEDATE = F_Form.GetValues("TRANEDATE");
                        //string[] STFDIDS = F_Form.GetValues("STFDIDS");
                        //string[] STFDID = F_Form.GetValues("STFDID");
                        //string[] boolSTFDIDS = F_Form.GetValues("boolSTFDIDS");
                        //string[] TRANDHAMT = F_Form.GetValues("TRANDHAMT");
                        //string[] TRANDEAMT = F_Form.GetValues("TRANDEAMT");
                        //string[] TRANDFAMT = F_Form.GetValues("TRANDFAMT");
                        //string[] TRANDAAMT = F_Form.GetValues("TRANDAAMT");


                        //string[] TRANDSAMT = F_Form.GetValues("TRANDSAMT");
                        //string[] TRANDNOP = F_Form.GetValues("TRANDNOP");
                        //string[] TRANDQTY = F_Form.GetValues("TRANDQTY");
                        //string[] TRANDREFID = F_Form.GetValues("TRANDREFID");
                        //string[] RAMT1 = F_Form.GetValues("RAMT1");
                        //string[] RAMT2 = F_Form.GetValues("RAMT2");
                        //string[] RAMT3 = F_Form.GetValues("RAMT3");
                        //string[] RAMT4 = F_Form.GetValues("RAMT4");
                        //string[] RAMT5 = F_Form.GetValues("RAMT5");
                        //string[] RAMT6 = F_Form.GetValues("RAMT6");
                        //string[] RCAMT1 = F_Form.GetValues("RCAMT1");
                        //string[] RCAMT2 = F_Form.GetValues("RCAMT2");
                        //string[] RCAMT3 = F_Form.GetValues("RCAMT3");
                        //string[] RCAMT4 = F_Form.GetValues("RCAMT4");
                        //string[] RCAMT5 = F_Form.GetValues("RCAMT5");
                        //string[] RCAMT6 = F_Form.GetValues("RCAMT6");
                        //string[] RCOL1 = F_Form.GetValues("RCOL1");
                        //string[] RCOL2 = F_Form.GetValues("RCOL2");
                        //string[] RCOL3 = F_Form.GetValues("RCOL3");
                        //string[] RCOL4 = F_Form.GetValues("RCOL4");
                        //string[] RCOL5 = F_Form.GetValues("RCOL5");
                        //string[] RCOL6 = F_Form.GetValues("RCOL6");
                        //string[] days = F_Form.GetValues("days");

                        //string[] BILLEDID = F_Form.GetValues("BILLEDID"); 
                        //string[] F_BILLEMID = F_Form.GetValues("BILLEMID"); 
                        //string[] TRANDWGHT = F_Form.GetValues("TRANDWGHT");
                        //string[] TRANOTYPE = F_Form.GetValues("detaildata[0].TRANOTYPE");
                        //string[] TRAND_COVID_DISC_AMT = F_Form.GetValues("TRAND_COVID_DISC_AMT");

                        for (int count = 0; count < F_TRANDID.Count(); count++)
                        {
                            string refno = Convert.ToString(TRANDREFNO[count]);

                            if (refno != "" || refno != null)
                            {
                                TRANDID = Convert.ToInt32(F_TRANDID[count]);

                                if (TRANDID != 0)
                                {
                                    debitnotedetail = context.debitnotetransactiondetail.Find(TRANDID);
                                }
                                debitnotedetail.TRANMID = debitnotemaster.TRANMID;
                                debitnotedetail.TRANDREFNO = (TRANDREFNO[count]).ToString();
                                debitnotedetail.TRANDREFNAME = "";//(TRANDREFNAME[count]).ToString();
                                debitnotedetail.TRANDREFID = Convert.ToInt32(TRANDREFID[count]);//GIDID
                                debitnotedetail.TRANIDATE = Convert.ToDateTime(TRANIDATE[count]);
                                debitnotedetail.TRANSDATE = DateTime.Now.Date; // Convert.ToDateTime(TRANSDATE[count]);
                                debitnotedetail.TRANEDATE = DateTime.Now.Date; // Convert.ToDateTime(TRANEDATE[count]);
                                debitnotedetail.TRANVDATE = DateTime.Now.Date; // Convert.ToDateTime(F_Form["detaildata[0].TRANVDATE"]);
                                debitnotedetail.TRANDSAMT = 0;// Convert.ToDecimal(TRANDSAMT[count]);
                                debitnotedetail.TRANDHAMT = 0;// Convert.ToDecimal(TRANDHAMT[count]);
                                debitnotedetail.TRANDEAMT = 0;// Convert.ToDecimal(TRANDEAMT[count]);
                                debitnotedetail.TRANDFAMT = 0;// Convert.ToDecimal(TRANDFAMT[count]);
                                debitnotedetail.TRANDAAMT = 0;// Convert.ToDecimal(TRANDAAMT[count]);
                                debitnotedetail.TRANDNAMT = 0;// Convert.ToDecimal(TRANDNAMT[count]);
                                debitnotedetail.TRANDNOP = 0;// Convert.ToDecimal(TRANDNOP[count]);
                                debitnotedetail.TRANDQTY = Convert.ToDecimal(NOD[count]);//NO.OF DAYS
                                debitnotedetail.TARIFFMID = 0;
                                debitnotedetail.TRANDDESC = TRANDDESC[count].ToString();
                                debitnotedetail.TRANDRATE = Convert.ToDecimal(TRANDRATE[count]);//TRANSPORT CHARGE
                                debitnotedetail.TRANOTYPE = 0;
                                debitnotedetail.TRANDGAMT = Convert.ToDecimal(TRANDGAMT[count]);
                                debitnotedetail.TRANDNAMT = Convert.ToDecimal(TRANDNAMT[count]);
                                debitnotedetail.BILLEDID = 0;// Convert.ToInt32(BILLEDID[count]);
                                debitnotedetail.RCOL1 = 0;// Convert.ToDecimal(RCOL1[count]);
                                debitnotedetail.RCOL2 = 0;// Convert.ToDecimal(RCOL2[count]);
                                debitnotedetail.RCOL3 = 0;// Convert.ToDecimal(RCOL3[count]);
                                debitnotedetail.RCOL4 = 0;// Convert.ToDecimal(RCOL4[count]);
                                debitnotedetail.RCOL5 = 0;// Convert.ToDecimal(RCOL5[count]);
                                debitnotedetail.RCOL6 = 0;// Convert.ToDecimal(RCOL6[count]);
                                debitnotedetail.RCOL7 = 0;
                                debitnotedetail.RAMT1 = 0;// Convert.ToDecimal(RAMT1[count]);
                                debitnotedetail.RAMT2 = 0;// Convert.ToDecimal(RAMT2[count]);
                                debitnotedetail.RAMT3 = 0;// Convert.ToDecimal(RAMT3[count]);
                                debitnotedetail.RAMT4 = 0;// Convert.ToDecimal(RAMT4[count]);
                                debitnotedetail.RAMT5 = 0;// Convert.ToDecimal(RAMT5[count]);
                                debitnotedetail.RAMT6 = 0;// Convert.ToDecimal(RAMT6[count]);
                                debitnotedetail.RCAMT1 = 0;// Convert.ToDecimal(RCAMT1[count]);
                                debitnotedetail.RCAMT2 = 0;// Convert.ToDecimal(RCAMT2[count]);
                                debitnotedetail.RCAMT3 = 0;// Convert.ToDecimal(RCAMT3[count]);
                                debitnotedetail.RCAMT4 = 0;//Convert.ToDecimal(RCAMT4[count]);
                                debitnotedetail.RCAMT5 = 0;// Convert.ToDecimal(RCAMT5[count]);
                                debitnotedetail.RCAMT6 = 0;// Convert.ToDecimal(RCAMT6[count]);
                                debitnotedetail.SLABTID = 0;
                                debitnotedetail.TRANYTYPE = 0;
                                debitnotedetail.TRANDWGHT = 0;
                                debitnotedetail.TRANDAID = Convert.ToInt32(TRANDAID[count]);
                                debitnotedetail.SBDID = 0;
                                debitnotedetail.TRAND_COVID_DISC_AMT = 0;
                                //--------------------- HSN Code --------------------------------------------//
                                debitnotedetail.TRANDHSNCODE = Convert.ToString(TRANDHSNCODE[count]);

                                //--------------------- Stroage Expression --------------------------------------------//
                                debitnotedetail.TRAND_STRG_CGST_EXPRN = Convert.ToDecimal(TRAND_STRG_CGST_EXPRN[count]);
                                debitnotedetail.TRAND_STRG_IGST_EXPRN = Convert.ToDecimal(TRAND_STRG_IGST_EXPRN[count]);
                                debitnotedetail.TRAND_STRG_SGST_EXPRN = Convert.ToDecimal(TRAND_STRG_SGST_EXPRN[count]);
                                //--------------------- Stroage Amount --------------------------------------------//
                                debitnotedetail.TRAND_STRG_CGST_AMT = Convert.ToDecimal(TRAND_STRG_CGST_AMT[count]);
                                debitnotedetail.TRAND_STRG_IGST_AMT = Convert.ToDecimal(TRAND_STRG_IGST_AMT[count]);
                                debitnotedetail.TRAND_STRG_SGST_AMT = Convert.ToDecimal(TRAND_STRG_SGST_AMT[count]);
                                //--------------------- Handling Expression --------------------------------------------//
                                debitnotedetail.TRAND_HANDL_CGST_EXPRN = 0; // Convert.ToDecimal(TRAND_HANDL_CGST_EXPRN[count]);
                                debitnotedetail.TRAND_HANDL_IGST_EXPRN = 0; // Convert.ToDecimal(TRAND_HANDL_IGST_EXPRN[count]);
                                debitnotedetail.TRAND_HANDL_SGST_EXPRN = 0; // Convert.ToDecimal(TRAND_HANDL_SGST_EXPRN[count]);
                                //--------------------- Handiling Amount --------------------------------------------//
                                debitnotedetail.TRAND_HANDL_CGST_AMT = 0; // Convert.ToDecimal(TRAND_HANDL_CGST_AMT[count]);
                                debitnotedetail.TRAND_HANDL_IGST_AMT = 0; // Convert.ToDecimal(TRAND_HANDL_IGST_AMT[count]);
                                debitnotedetail.TRAND_HANDL_SGST_AMT = 0; // Convert.ToDecimal(TRAND_HANDL_SGST_AMT[count]);

                                if (Convert.ToInt32(TRANDID) == 0)
                                {
                                    context.debitnotetransactiondetail.Add(debitnotedetail);
                                    context.SaveChanges();
                                    TRANDID = debitnotedetail.TRANDID;
                                }
                                else
                                {
                                    debitnotedetail.TRANDID = TRANDID;
                                    context.Entry(debitnotedetail).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                }//..............end
                                DELIDS = DELIDS + "," + TRANDID.ToString();
                            }
                        }



                        context.Database.ExecuteSqlCommand("DELETE FROM Debitnote_TransactionDetail  WHERE TRANMID=" + TRANMID + " and  TRANDID NOT IN(" + DELIDS.Substring(1) + ")");
                        //  Response.Redirect("Index");
                        trans.Commit();
                        //context.Database.ExecuteSqlCommand("EXEC PR_EXPORT_MANUAL_BILL_TRANMASTER_UPD @PTRANMID =" + TRANMID);
                        status = "Success";

                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        status = "Error";
                        //Response.Write("Sorry!!An Error Ocurred..." + ex.Message);
                        Response.Redirect("/Error/AccessDenied");
                    }
                }
            }
            Response.Redirect("Index");
            //return Json(status, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete 
        //[Authorize(Roles = "ManualDebitNoteEInvoiceDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);

            if (temp.Equals("PROCEED"))
            {
                DebitNote_TransactionMaster debitnotemaster = context.debitnotetransactionmaster.Find(Convert.ToInt32(id));
                context.debitnotetransactionmaster.Remove(debitnotemaster);
                context.SaveChanges();
                Response.Write("Deleted successfully...");
            }
            else
                Response.Write(temp);
        }
        #endregion

        #region Print View
        [Authorize(Roles = "CreditNotePrint")]
        public void PrintView(int? id = 0)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "DEBITNOTE", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                var Query = context.Database.SqlQuery<int>("select TRANPCOUNT from DEBITNOTE_TRANSACTIONMASTER where TRANMID=" + id).ToList();
                var PCNT = 0;

                if (Query.Count() != 0) { PCNT = Query[0]; }
                var TRANPCOUNT = ++PCNT;

                context.Database.ExecuteSqlCommand("UPDATE DEBITNOTE_TRANSACTIONMASTER SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + id);

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Finance_DebitNote.RPT");
                cryRpt.RecordSelectionFormula = "{VW_FINANCE_DEBITNOTE_PRINT.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_FINANCE_CREDITNOTE_PRINT.TRANMID} = " + id;

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
        #endregion


    }
}