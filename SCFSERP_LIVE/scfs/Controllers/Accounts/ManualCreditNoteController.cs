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
using System.Web;
using System.Web.Mvc;
using scfs.Data;


namespace scfs_erp.Controllers.Accounts
{
    [SessionExpire]
    public class ManualCreditNoteController : Controller
    {
        // GET: ManualCreditNote

        #region Context declaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region Credit Note Index
        [Authorize(Roles = "ManualCreditNoteIndex")]
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
                ViewBag.SDPTID = new SelectList(context.softdepartmentmasters.Where(x => x.SDPTID == 1 || x.SDPTID == 2 || x.SDPTID == 9 || x.SDPTID == 11 || x.SDPTID == 12 || x.SDPTID == 13).OrderBy(x => x.SDPTID), "SDPTID", "SDPTNAME", Convert.ToInt32(Session["SDPTID"]));
            }
            else
            {
                Session["SDPTID"] = "1";
                ViewBag.SDPTID = new SelectList(context.softdepartmentmasters.Where(x => x.SDPTID == 1 || x.SDPTID == 2 || x.SDPTID == 9 || x.SDPTID == 11 || x.SDPTID == 12 || x.SDPTID == 13).OrderBy(x => x.SDPTID), "SDPTID", "SDPTNAME");
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

                var data = e.pr_Search_Finance_CreditNote(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                   totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToInt32(Session["SDPTID"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]),1);
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
        [Authorize(Roles = "ManualCreditNoteEdit")]
        public void Edit(int id)
        {
            Response.Redirect("~/ManualCreditNote/CNForm/" + id);

        }
        #endregion

        #region Details Form
        [Authorize(Roles = "ManualCreditNoteCreate")]
        public ActionResult CNForm(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            CreditNote_TransactionMaster tab = new CreditNote_TransactionMaster();
            CreditNoteMD vm = new CreditNoteMD();


            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItemDSP);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            ViewBag.CATEAID = new SelectList("");

            ViewBag.TALLY_CATEAID = new SelectList("");

            //List<SelectListItem> invoiceType = new List<SelectListItem>();
            //SelectListItem InTItemDSP = new SelectListItem { Text = "IMPORT", Value = "1", Selected = true };
            //invoiceType.Add(InTItemDSP);
            //InTItemDSP = new SelectListItem { Text = "NON-PNR", Value = "9", Selected = false };
            //invoiceType.Add(InTItemDSP);
            //InTItemDSP = new SelectListItem { Text = "EXPORT", Value = "2", Selected = false };
            //invoiceType.Add(InTItemDSP);
            //InTItemDSP = new SelectListItem { Text = "E-SEAL", Value = "11", Selected = false };
            //invoiceType.Add(InTItemDSP);
            //ViewBag.SDPTID = invoiceType;

            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE == 6), "TRANMODE", "TRANMODEDETL", 6);
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.TRANDREFID = new SelectList(context.CreditNoteTypeMaster.OrderBy(x => x.CNTID), "CNTID", "CNTDESC");
            ViewBag.SDPTID = new SelectList(context.softdepartmentmasters.Where(x => x.SDPTID == 1 || x.SDPTID == 2 || x.SDPTID == 9 || x.SDPTID == 11 || x.SDPTID == 12 || x.SDPTID == 13).OrderBy(x => x.SDPTID), "SDPTID", "SDPTNAME");

            if (id != 0)//....Edit Mode
            {
                tab = context.creditnotetransactionmaster.Find(id);

                ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE == 6), "TRANMODE", "TRANMODEDETL", tab.TRANMODE);
                ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC", tab.BANKMID);
                var sqy = context.Database.SqlQuery<Category_Address_Details>("Select *from CATEGORY_ADDRESS_DETAIL WHERE CATEAID=" + tab.CATEAID).ToList();
                if (sqy.Count > 0)
                {
                    ViewBag.CATEAID = new SelectList(context.categoryaddressdetails.Where(x => x.CATEAID > 0), "CATEAID", "CATEATYPEDESC", tab.CATEAID);
                }
                else { ViewBag.CATEAID = new SelectList(""); }

                if (tab.TALLY_CATEAID > 0)
                {
                    ViewBag.TALLY_TRANREFID = tab.TALLY_TRANREFID;
                    ViewBag.TALLY_TRANREFNAME = tab.TALLY_TRANREFNAME;
                    ViewBag.TALLY_CATEAID = new SelectList(context.categoryaddressdetails.Where(x => x.CATEAID > 0), "CATEAID", "CATEATYPEDESC", tab.TALLY_CATEAID);
                }
                else
                {
                    ViewBag.TALLY_TRANREFID = tab.TRANREFID;
                    ViewBag.TALLY_CATEAID = new SelectList(context.categoryaddressdetails.Where(x => x.CATEAID > 0), "CATEAID", "CATEATYPEDESC", tab.CATEAID);
                }

                ViewBag.SDPTID = new SelectList(context.softdepartmentmasters.Where(x => x.SDPTID == 1 || x.SDPTID == 2 || x.SDPTID == 9 || x.SDPTID == 11 || x.SDPTID == 12 || x.SDPTID == 13).OrderBy(x => x.SDPTID), "SDPTID", "SDPTNAME", tab.SDPTID);

                vm.masterdata = context.creditnotetransactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.detaildata = context.creditnotetransactiondetail.Where(det => det.TRANMID == id).ToList();
                if (vm.detaildata.Count > 0)
                {
                    ViewBag.TRANDREFID = new SelectList(context.CreditNoteTypeMaster.OrderBy(x => x.CNTID), "CNTID", "CNTDESC", Convert.ToInt32(vm.detaildata[0].TRANDREFID));
                }
                else
                {
                    ViewBag.TRANDREFID = new SelectList(context.CreditNoteTypeMaster.OrderBy(x => x.CNTID), "CNTID", "CNTDESC");
                }

            }
            return View(vm);
        }
        #endregion

        #region Auto Complete for Bill Refno
        public JsonResult AutoBillNo(string term)
        {
            string billno = ""; int SDPTID = 0; int CHAID = 0;
            if (term.Contains('-'))
            {
                var Param = term.Split('-');

                if (Param[0] != "" || Param[0] != null)
                { billno = Convert.ToString(Param[0]); }
                else { billno = ""; }

                if (Param[1] != "" || Param[1] != null)
                { SDPTID = Convert.ToInt32(Param[1]); }
                else { SDPTID = 0; }

                if (Param[2] != "" || Param[2] != null)
                { CHAID = Convert.ToInt32(Param[2]); }
                else { CHAID = 0; }
            }
            else { billno = term.ToString(); }

            //var result = (from r in context.transactionmaster.Where(m => m.DISPSTATUS == 0 && m.SDPTID == SDPTID && m.TRANREFID == CHAID)
            //              where r.TRANDNO.ToLower().Contains(billno.ToLower())
            //              select new { r.TRANDNO, r.TRANMID }).Distinct();
            //return Json(result, JsonRequestBehavior.AllowGet);

            var result = (from r in context.transactionmaster.Where(m => m.DISPSTATUS == 0 && m.REGSTRID < 60)
                          where r.TRANDNO.ToLower().Contains(term.ToLower())
                          select new { r.TRANDNO, r.TRANMID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetAutoBillNo(string term, int Sdptid, int Tranrffid)
        public JsonResult GetAutoBillNo(string term)
        {
            string billno = ""; int SDPTID = 0; int CHAID = 0;
            var Param = term.Split(';');

            if (Param[0] != "" || Param[0] != null) { billno = Convert.ToString(Param[0]); } else { billno = ""; }
            if (Param[1] != "" || Param[1] != null) { SDPTID = Convert.ToInt32(Param[1]); } else { SDPTID = 0; }
            if (Param[2] != "" || Param[2] != null) { CHAID = Convert.ToInt32(Param[2]); } else { CHAID = 0; }

            var result = context.Database.SqlQuery<pr_Finance_TranSactionBillNo_Result>("EXEC pr_Finance_TranSactionBillNo @FilterTerm='%" + billno + "%',@Sdptid = " + Convert.ToInt32(SDPTID) + ",@Tranrffid = " + Convert.ToInt32(CHAID) + "").ToList();

            return Json(result, JsonRequestBehavior.AllowGet);

            //var result = (from r in context.transactionmaster.Where(m => m.DISPSTATUS == 0 && m.REGSTRID < 60)
            //              where r.TRANDNO.ToLower().Contains(term.ToLower())
            //              select new { r.TRANDNO, r.TRANMID }).Distinct();
            //return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStateType(int id)
        {
            var result = context.Database.SqlQuery<StateMaster>("Select * from StateMaster(nolock) Where StateId = "+ Convert.ToInt32(id) + "").ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Get Transaction Detail
        public JsonResult Getcreditnotedetail(string id)
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
                        CreditNote_TransactionMaster creditnotemaster = new CreditNote_TransactionMaster();
                        CreditNote_TransactionDetail creditnotedetail = new CreditNote_TransactionDetail();
                        //-------Getting Primarykey field--------
                        Int32 TRANMID = Convert.ToInt32(F_Form["masterdata[0].TRANMID"]);
                        Int32 TRANDID = 0;
                        string DELIDS = "";
                        //-----End


                        if (TRANMID != 0)
                        {
                            creditnotemaster = context.creditnotetransactionmaster.Find(TRANMID);
                        }

                        //...........transaction master.............//
                        creditnotemaster.TRANMID = TRANMID;
                        creditnotemaster.COMPYID = Convert.ToInt32(Session["compyid"]);
                        string SDPTID = Convert.ToString(F_Form["SDPTID"]);
                        if (SDPTID == "" || SDPTID == null || SDPTID == "0")
                        { creditnotemaster.SDPTID = 0; }
                        else { creditnotemaster.SDPTID = Convert.ToInt32(SDPTID); }

                        creditnotemaster.TRANTID = 2;
                        creditnotemaster.TRANLMID = 0;
                        creditnotemaster.TRANLSID = 0;
                        creditnotemaster.TRANLSNO = null;
                        creditnotemaster.TRANLMNO = "";
                        creditnotemaster.TRANLMDATE = DateTime.Now;
                        creditnotemaster.TRANLSDATE = DateTime.Now;
                        creditnotemaster.TRANNARTN = null;

                        var cusr = creditnotemaster.CUSRID;
                        if (TRANMID.ToString() == "0" || cusr == null || cusr == "")
                            creditnotemaster.CUSRID = Session["CUSRID"].ToString();
                        creditnotemaster.LMUSRID = Session["CUSRID"].ToString();
                        creditnotemaster.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
                        creditnotemaster.PRCSDATE = DateTime.Now;
                        creditnotemaster.TRANDATE = Convert.ToDateTime(F_Form["masterdata[0].TRANDATE"]).Date;
                        creditnotemaster.TRANTIME = DateTime.Now;
                        creditnotemaster.TRANREFID = Convert.ToInt32(F_Form["masterdata[0].TRANREFID"]);//bill id
                        creditnotemaster.TRANREFNAME = F_Form["masterdata[0].TRANREFNAME"].ToString();//bill no

                        creditnotemaster.TALLY_TRANREFID = Convert.ToInt32(F_Form["TALLY_TRANREFID"]);//bill id
                        creditnotemaster.TALLY_TRANREFNAME = F_Form["TALLY_TRANREFNAME"].ToString();//bill no

                        string TALLY_CATEAID = Convert.ToString(F_Form["TALLY_CATEAID"]);
                        if (TALLY_CATEAID == "" || TALLY_CATEAID == null || TALLY_CATEAID == "0")
                        { creditnotemaster.TALLY_CATEAID = 0; }
                        else { creditnotemaster.TALLY_CATEAID = Convert.ToInt32(TALLY_CATEAID); }

                        creditnotemaster.LCATEID = 0;
                        creditnotemaster.TRANBTYPE = 1;// Convert.ToInt16(F_Form["TRANBTYPE"]);
                        creditnotemaster.REGSTRID = 61;
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
                        { creditnotemaster.TRANMODE = 0; }
                        else { creditnotemaster.TRANMODE = Convert.ToInt16(TRANMODE); }
                        //creditnotemaster.TRANMODE = Convert.ToInt16(F_Form["TRANMODE"]);

                        creditnotemaster.TRANMODEDETL = F_Form["masterdata[0].TRANMODEDETL"].ToString();
                        creditnotemaster.TRANGAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANGAMT"]);
                        creditnotemaster.TRANNAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANNAMT"]);
                        creditnotemaster.TRANROAMT = 0;// Convert.ToDecimal(F_Form["masterdata[0].TRANROAMT"]);
                        string TRANREFAMT = Convert.ToString(F_Form["masterdata[0].TRANREFAMT"]);
                        if (TRANREFAMT == "" || TRANREFAMT == null || TRANREFAMT == "0")
                        { creditnotemaster.TRANREFAMT = 0; }
                        else { creditnotemaster.TRANREFAMT = Convert.ToDecimal(TRANREFAMT); }

                        //creditnotemaster.TRANREFAMT =  Convert.ToDecimal(F_Form["masterdata[0].TRANREFAMT"]);

                        creditnotemaster.TRANRMKS = F_Form["masterdata[0].TRANRMKS"].ToString();
                        string amountinwords = AmtInWrd.ConvertNumbertoWords(F_Form["masterdata[0].TRANNAMT"]).ToString();
                        if (amountinwords != "")
                        { creditnotemaster.TRANAMTWRDS = amountinwords.ToString(); }
                        else { creditnotemaster.TRANAMTWRDS = "-"; }

                        //creditnotemaster.TRANAMTWRDS = Convert.ToString(AmtInWrd.ConvertNumbertoWords(F_Form["masterdata[0].TRANNAMT"]));
                        creditnotemaster.TRANINOC1 = 0; // Convert.ToDecimal(F_Form["masterdata[0].TRANINOC1"]);
                        creditnotemaster.TRANINOC2 = 0;// Convert.ToDecimal(F_Form["masterdata[0].TRANINOC2"]);
                        creditnotemaster.TRANSAMT = 0;// Convert.ToDecimal(F_Form["TRANSAMT"]);
                        creditnotemaster.TRANAAMT = 0;// Convert.ToDecimal(F_Form["TRANAAMT"]);
                        creditnotemaster.TRANHAMT = 0;// Convert.ToDecimal(F_Form["TRANHAMT"]);
                        creditnotemaster.TRANEAMT = 0;// Convert.ToDecimal(F_Form["TRANEAMT"]);
                        creditnotemaster.TRANFAMT = 0;// Convert.ToDecimal(F_Form["TRANFAMT"]);
                        creditnotemaster.TRANTCAMT = 0;// Convert.ToDecimal(F_Form["TRANTCAMT"]);

                        creditnotemaster.STRG_HSNCODE = "-";// F_Form["STRG_HSN_CODE"].ToString();
                        creditnotemaster.HANDL_HSNCODE = "-";// F_Form["HANDL_HSN_CODE"].ToString();

                        creditnotemaster.STRG_TAXABLE_AMT = 0;// Convert.ToDecimal(F_Form["STRG_TAXABLE_AMT"]);
                        creditnotemaster.HANDL_TAXABLE_AMT = 0;// Convert.ToDecimal(F_Form["HANDL_TAXABLE_AMT"]);
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
                        creditnotemaster.STRG_CGST_EXPRN = Convert.ToDecimal(TRAND_STRG_CGST_EXPRN[0]);
                        creditnotemaster.STRG_IGST_EXPRN = Convert.ToDecimal(TRAND_STRG_IGST_EXPRN[0]);
                        creditnotemaster.STRG_SGST_EXPRN = Convert.ToDecimal(TRAND_STRG_SGST_EXPRN[0]);
                        //--------------------- Stroage Amount --------------------------------------------//
                        creditnotemaster.STRG_CGST_AMT = Convert.ToDecimal(TRAND_STRG_CGST_AMT[0]);
                        creditnotemaster.STRG_IGST_AMT = Convert.ToDecimal(TRAND_STRG_IGST_AMT[0]);
                        creditnotemaster.STRG_SGST_AMT = Convert.ToDecimal(TRAND_STRG_SGST_AMT[0]);

                        creditnotemaster.HANDL_CGST_EXPRN = 0;
                        creditnotemaster.HANDL_SGST_EXPRN = 0;
                        creditnotemaster.HANDL_IGST_EXPRN = 0;
                        creditnotemaster.HANDL_CGST_AMT = 0;
                        creditnotemaster.HANDL_SGST_AMT = 0;
                        creditnotemaster.HANDL_IGST_AMT = 0;

                        creditnotemaster.TRANCGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANCGSTAMT"]);
                        creditnotemaster.TRANSGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANSGSTAMT"]);
                        creditnotemaster.TRANIGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANIGSTAMT"]);
                        creditnotemaster.TRAN_COVID_DISC_AMT = 0; // Convert.ToDecimal(F_Form["masterdata[0].TRAN_COVID_DISC_AMT"]);

                        creditnotemaster.TRANNARTN = "-";//Convert.ToString(F_Form["masterdata[0].TRANNARTN"]);

                        string CATEAID = Convert.ToString(F_Form["CATEAID"]);
                        if (CATEAID == "" || CATEAID == null || CATEAID == "0")
                        { creditnotemaster.CATEAID = 0; }
                        else { creditnotemaster.CATEAID = Convert.ToInt32(CATEAID); }

                        string STATEID = Convert.ToString(F_Form["masterdata[0].STATEID"]);
                        if (STATEID == "" || STATEID == null || STATEID == "0")
                        { creditnotemaster.STATEID = 0; }
                        else { creditnotemaster.STATEID = Convert.ToInt32(STATEID); }

                        creditnotemaster.CATEAGSTNO = Convert.ToString(F_Form["masterdata[0].CATEAGSTNO"]);
                        creditnotemaster.TRANIMPADDR1 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR1"]);
                        creditnotemaster.TRANIMPADDR2 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR2"]);
                        creditnotemaster.TRANIMPADDR3 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR3"]);
                        creditnotemaster.TRANIMPADDR4 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR4"]);

                        if (TRANMODE != "2" && TRANMODE != "3")
                        {
                            creditnotemaster.TRANREFNO = "";
                            creditnotemaster.TRANREFBNAME = "";
                            creditnotemaster.BANKMID = 0;
                            creditnotemaster.TRANREFDATE = DateTime.Now;
                        }
                        else
                        {
                            creditnotemaster.TRANREFNO = (F_Form["masterdata[0].TRANREFNO"]).ToString();
                            creditnotemaster.TRANREFBNAME = (F_Form["masterdata[0].TRANREFBNAME"]).ToString();
                            creditnotemaster.BANKMID = Convert.ToInt32(F_Form["BANKMID"]);
                            creditnotemaster.TRANREFDATE = Convert.ToDateTime(F_Form["masterdata[0].TRANREFDATE"]).Date;
                        }


                        //.................Autonumber............//
                        var sdptid = Convert.ToInt32(F_Form["SDPTID"]);

                        string billformat = "";

                        if (TRANMID == 0)
                        {
                           
                            creditnotemaster.TRANNO = Convert.ToInt32(Autonumber.manualcreditnote("CreditNote_TransactionMaster", "TRANNO", sdptid.ToString(), Session["compyid"].ToString(), creditnotemaster.TRANBTYPE.ToString()).ToString());

                            //creditnotemaster.TRANNO = Convert.ToInt32(Autonumber.autonum("CreditNote_TransactionMaster", "TRANNO", "TRANNO <> 0 and compyid = " + Convert.ToInt32(Session["compyid"]) + "").ToString());

                            int ano = creditnotemaster.TRANNO;
                            if (sdptid == 1)
                            {
                                billformat = "CR/IMP/";
                            }
                            else if (sdptid == 2)
                            {
                                billformat = "CR/EXP/";
                            }

                            else if (sdptid == 9)
                            {
                                billformat = "CR/NPN/";
                            }

                            else if (sdptid == 11)
                            {
                                billformat = "CR/ESL/";
                            }

                            else if (sdptid == 12)
                            {
                                billformat = "CR/BND/";
                            }

                            else if (sdptid == 13)
                            {
                                billformat = "CR/MNR/";
                            }


                            string prfx = string.Format("{0:D4}", ano);
                            creditnotemaster.TRANDNO = prfx.ToString();

                            string billprfx = string.Format(billformat + Session["GPrxDesc"] + "/" + "{0:D4}", ano);
                            creditnotemaster.TRANBILLREFNO = billprfx.ToString();

                            context.creditnotetransactionmaster.Add(creditnotemaster);
                            context.SaveChanges();
                            TRANMID = creditnotemaster.TRANMID;
                        }
                        else
                        {
                            int ano = creditnotemaster.TRANNO;
                            if (sdptid == 1)
                            {
                                billformat = "CR/IMP/";
                            }
                            else if (sdptid == 2)
                            {
                                billformat = "CR/EXP/";
                            }

                            else if (sdptid == 9)
                            {
                                billformat = "CR/NPN/";
                            }

                            else if (sdptid == 11)
                            {
                                billformat = "CR/ESL/";
                            }

                            else if (sdptid == 12)
                            {
                                billformat = "CR/BND/";
                            }

                            else if (sdptid == 13)
                            {
                                billformat = "CR/MNR/";
                            }


                            string prfx = string.Format("{0:D4}", ano);
                            creditnotemaster.TRANDNO = prfx.ToString();

                            string billprfx = string.Format(billformat + Session["GPrxDesc"] + "/"+ "{0:D4}", ano);
                            creditnotemaster.TRANBILLREFNO = billprfx.ToString();
                            context.Entry(creditnotemaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }


                        //-------------transaction Details
                        string[] F_TRANDID = F_Form.GetValues("TRANDID");
                        string[] TRANDREFNO = F_Form.GetValues("TRANDREFNO");
                        string[] TRANDAID = F_Form.GetValues("TRANDAID");
                        string[] TRANDREFID = F_Form.GetValues("TRANDREFID");
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
                                    creditnotedetail = context.creditnotetransactiondetail.Find(TRANDID);
                                }
                                creditnotedetail.TRANMID = creditnotemaster.TRANMID;
                                creditnotedetail.TRANDREFNO = (TRANDREFNO[count]).ToString();
                                creditnotedetail.TRANDREFNAME = "";//(TRANDREFNAME[count]).ToString();
                                creditnotedetail.TRANDREFID = Convert.ToInt32(TRANDREFID[count]);//GIDID
                                creditnotedetail.TRANIDATE = Convert.ToDateTime(TRANIDATE[count]);
                                creditnotedetail.TRANSDATE = DateTime.Now.Date; // Convert.ToDateTime(TRANSDATE[count]);
                                creditnotedetail.TRANEDATE = DateTime.Now.Date; // Convert.ToDateTime(TRANEDATE[count]);
                                creditnotedetail.TRANVDATE = DateTime.Now.Date; // Convert.ToDateTime(F_Form["detaildata[0].TRANVDATE"]);
                                creditnotedetail.TRANDSAMT = 0;// Convert.ToDecimal(TRANDSAMT[count]);
                                creditnotedetail.TRANDHAMT = 0;// Convert.ToDecimal(TRANDHAMT[count]);
                                creditnotedetail.TRANDEAMT = 0;// Convert.ToDecimal(TRANDEAMT[count]);
                                creditnotedetail.TRANDFAMT = 0;// Convert.ToDecimal(TRANDFAMT[count]);
                                creditnotedetail.TRANDAAMT = 0;// Convert.ToDecimal(TRANDAAMT[count]);
                                creditnotedetail.TRANDNAMT = 0;// Convert.ToDecimal(TRANDNAMT[count]);
                                creditnotedetail.TRANDNOP = 0;// Convert.ToDecimal(TRANDNOP[count]);
                                creditnotedetail.TRANDQTY = Convert.ToDecimal(NOD[count]);//NO.OF DAYS
                                creditnotedetail.TARIFFMID = 0;
                                creditnotedetail.TRANDDESC = TRANDDESC[count].ToString();
                                creditnotedetail.TRANDRATE = Convert.ToDecimal(TRANDRATE[count]);//TRANSPORT CHARGE
                                creditnotedetail.TRANOTYPE = 0;
                                creditnotedetail.TRANDGAMT = Convert.ToDecimal(TRANDGAMT[count]);
                                creditnotedetail.TRANDNAMT = Convert.ToDecimal(TRANDNAMT[count]);
                                creditnotedetail.BILLEDID = 0;// Convert.ToInt32(BILLEDID[count]);
                                creditnotedetail.RCOL1 = 0;// Convert.ToDecimal(RCOL1[count]);
                                creditnotedetail.RCOL2 = 0;// Convert.ToDecimal(RCOL2[count]);
                                creditnotedetail.RCOL3 = 0;// Convert.ToDecimal(RCOL3[count]);
                                creditnotedetail.RCOL4 = 0;// Convert.ToDecimal(RCOL4[count]);
                                creditnotedetail.RCOL5 = 0;// Convert.ToDecimal(RCOL5[count]);
                                creditnotedetail.RCOL6 = 0;// Convert.ToDecimal(RCOL6[count]);
                                creditnotedetail.RCOL7 = 0;
                                creditnotedetail.RAMT1 = 0;// Convert.ToDecimal(RAMT1[count]);
                                creditnotedetail.RAMT2 = 0;// Convert.ToDecimal(RAMT2[count]);
                                creditnotedetail.RAMT3 = 0;// Convert.ToDecimal(RAMT3[count]);
                                creditnotedetail.RAMT4 = 0;// Convert.ToDecimal(RAMT4[count]);
                                creditnotedetail.RAMT5 = 0;// Convert.ToDecimal(RAMT5[count]);
                                creditnotedetail.RAMT6 = 0;// Convert.ToDecimal(RAMT6[count]);
                                creditnotedetail.RCAMT1 = 0;// Convert.ToDecimal(RCAMT1[count]);
                                creditnotedetail.RCAMT2 = 0;// Convert.ToDecimal(RCAMT2[count]);
                                creditnotedetail.RCAMT3 = 0;// Convert.ToDecimal(RCAMT3[count]);
                                creditnotedetail.RCAMT4 = 0;//Convert.ToDecimal(RCAMT4[count]);
                                creditnotedetail.RCAMT5 = 0;// Convert.ToDecimal(RCAMT5[count]);
                                creditnotedetail.RCAMT6 = 0;// Convert.ToDecimal(RCAMT6[count]);
                                creditnotedetail.SLABTID = 0;
                                creditnotedetail.TRANYTYPE = 0;
                                creditnotedetail.TRANDWGHT = 0;
                                creditnotedetail.TRANDAID = Convert.ToInt32(TRANDAID[count]);
                                creditnotedetail.SBDID = 0;
                                creditnotedetail.TRAND_COVID_DISC_AMT = 0;
                                //--------------------- HSN Code --------------------------------------------//
                                creditnotedetail.TRANDHSNCODE = Convert.ToString(TRANDHSNCODE[count]);               
                                
                                //--------------------- Stroage Expression --------------------------------------------//
                                creditnotedetail.TRAND_STRG_CGST_EXPRN = Convert.ToDecimal(TRAND_STRG_CGST_EXPRN[count]);
                                creditnotedetail.TRAND_STRG_IGST_EXPRN = Convert.ToDecimal(TRAND_STRG_IGST_EXPRN[count]);
                                creditnotedetail.TRAND_STRG_SGST_EXPRN = Convert.ToDecimal(TRAND_STRG_SGST_EXPRN[count]);
                                //--------------------- Stroage Amount --------------------------------------------//
                                creditnotedetail.TRAND_STRG_CGST_AMT = Convert.ToDecimal(TRAND_STRG_CGST_AMT[count]);
                                creditnotedetail.TRAND_STRG_IGST_AMT = Convert.ToDecimal(TRAND_STRG_IGST_AMT[count]);
                                creditnotedetail.TRAND_STRG_SGST_AMT = Convert.ToDecimal(TRAND_STRG_SGST_AMT[count]);
                                //--------------------- Handling Expression --------------------------------------------//
                                creditnotedetail.TRAND_HANDL_CGST_EXPRN = 0; // Convert.ToDecimal(TRAND_HANDL_CGST_EXPRN[count]);
                                creditnotedetail.TRAND_HANDL_IGST_EXPRN = 0; // Convert.ToDecimal(TRAND_HANDL_IGST_EXPRN[count]);
                                creditnotedetail.TRAND_HANDL_SGST_EXPRN = 0; // Convert.ToDecimal(TRAND_HANDL_SGST_EXPRN[count]);
                                //--------------------- Handiling Amount --------------------------------------------//
                                creditnotedetail.TRAND_HANDL_CGST_AMT = 0; // Convert.ToDecimal(TRAND_HANDL_CGST_AMT[count]);
                                creditnotedetail.TRAND_HANDL_IGST_AMT = 0; // Convert.ToDecimal(TRAND_HANDL_IGST_AMT[count]);
                                creditnotedetail.TRAND_HANDL_SGST_AMT = 0; // Convert.ToDecimal(TRAND_HANDL_SGST_AMT[count]);

                                if (Convert.ToInt32(TRANDID) == 0)
                                {
                                    context.creditnotetransactiondetail.Add(creditnotedetail);
                                    context.SaveChanges();
                                    TRANDID = creditnotedetail.TRANDID;
                                }
                                else
                                {
                                    creditnotedetail.TRANDID = TRANDID;
                                    context.Entry(creditnotedetail).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                }//..............end
                                DELIDS = DELIDS + "," + TRANDID.ToString();
                            }
                        }



                        context.Database.ExecuteSqlCommand("DELETE FROM Creditnote_TransactionDetail  WHERE TRANMID=" + TRANMID + " and  TRANDID NOT IN(" + DELIDS.Substring(1) + ")");
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

        #region Print View
        [Authorize(Roles = "ManualCreditNotePrint")]
        public void PrintView(int? id = 0)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "MENUALCREDITNOTE", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                var Query = context.Database.SqlQuery<int>("select TRANPCOUNT from CREDITNOTE_TRANSACTIONMASTER where TRANMID=" + id).ToList();
                var PCNT = 0;

                if (Query.Count() != 0) { PCNT = Query[0]; }
                var TRANPCOUNT = ++PCNT;

                context.Database.ExecuteSqlCommand("UPDATE CREDITNOTE_TRANSACTIONMASTER SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + id);

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Finance_CreditNote.RPT");
                cryRpt.RecordSelectionFormula = "{VW_FINANCE_CREDITNOTE_PRINT.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_FINANCE_CREDITNOTE_PRINT.TRANMID} = " + id;

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
        #endregion

        #region Delete 
        [Authorize(Roles = "ManualCreditNoteDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);

            if (temp.Equals("PROCEED"))
            {
                CreditNote_TransactionMaster creditnotemaster = context.creditnotetransactionmaster.Find(Convert.ToInt32(id));
                context.creditnotetransactionmaster.Remove(creditnotemaster);
                context.SaveChanges();
                Response.Write("Deleted successfully...");
            }
            else
                Response.Write(temp);
        }
        #endregion
    }
}