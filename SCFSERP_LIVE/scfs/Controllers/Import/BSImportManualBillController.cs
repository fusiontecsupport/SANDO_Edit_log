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

namespace scfs_erp.Controllers.Import
{
    [SessionExpire]
    public class BSImportManualBillController : Controller
    {
        // GET: BSImportManualBill
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

        [Authorize(Roles = "BSImportManualBillIndex")]
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
                Session["REGSTRID"] = "33";
            }
            //...........Bill type......//
            List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
            //if (Convert.ToInt32(Session["TRANBTYPE"]) == 3)
            //{
            //    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LI", Value = "3", Selected = true };
            //    selectedBILLYPE.Add(selectedItemGPTY);
            //    selectedItemGPTY = new SelectListItem { Text = "DS", Value = "4", Selected = false };
            //    selectedBILLYPE.Add(selectedItemGPTY);

            //}
            //else
            //{
            //    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LI", Value = "3", Selected = false };
            //    selectedBILLYPE.Add(selectedItemGPTY);
            //    selectedItemGPTY = new SelectListItem { Text = "DS", Value = "4", Selected = true };
            //    selectedBILLYPE.Add(selectedItemGPTY);

            //}
            if (Convert.ToInt32(Session["TRANBTYPE"]) == 0)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = true };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);

            }
            else if (Convert.ToInt32(Session["TRANBTYPE"]) == 5)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = true };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);

            }
            else
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "GEN", Value = "0", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "MNR", Value = "5", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "O&M", Value = "6", Selected = true };
                selectedBILLYPE.Add(selectedItemGPTY);


            }
            ViewBag.TRANBTYPE = selectedBILLYPE;
            ////..........end
            //............Billed to....//
            List<SelectListItem> selectedregid_ = new List<SelectListItem>();
            if (Convert.ToInt32(Session["REGSTRID"]) == 32)
            {
                SelectListItem selectedItemreg_ = new SelectListItem { Text = "EMPTY", Value = "31", Selected = false };
                selectedregid_.Add(selectedItemreg_);
                selectedItemreg_ = new SelectListItem { Text = "LOAD", Value = "32", Selected = true };
                selectedregid_.Add(selectedItemreg_);
                selectedItemreg_ = new SelectListItem { Text = "WGMT", Value = "33", Selected = false };
                selectedregid_.Add(selectedItemreg_);
                ViewBag.REGSTRID = selectedregid_;
            }
            else if (Convert.ToInt32(Session["REGSTRID"]) == 31)
            {
                SelectListItem selectedItemreg_ = new SelectListItem { Text = "EMPTY", Value = "31", Selected = true };
                selectedregid_.Add(selectedItemreg_);
                selectedItemreg_ = new SelectListItem { Text = "LOAD", Value = "32", Selected = false };
                selectedregid_.Add(selectedItemreg_);
                selectedItemreg_ = new SelectListItem { Text = "WGMT", Value = "33", Selected = false };
                selectedregid_.Add(selectedItemreg_);
                ViewBag.REGSTRID = selectedregid_;
            }
            else if (Convert.ToInt32(Session["REGSTRID"]) == 33)
            {
                SelectListItem selectedItemreg_ = new SelectListItem { Text = "EMPTY", Value = "31", Selected = false };
                selectedregid_.Add(selectedItemreg_);
                selectedItemreg_ = new SelectListItem { Text = "LOAD", Value = "32", Selected = false };
                selectedregid_.Add(selectedItemreg_);
                selectedItemreg_ = new SelectListItem { Text = "WGMT", Value = "33", Selected = true };
                selectedregid_.Add(selectedItemreg_);
                ViewBag.REGSTRID = selectedregid_;
            }
            //.....end


            return View();
        }
        //public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        //{
        //    using (var e = new cfsEntities44())
        //    {
        //        var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
        //        var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

        //        var data = e.pr_Search_OutsideDestuff(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
        //            totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToInt32(Session["REGSTRID"]), Convert.ToInt32(Session["TRANBTYPE"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
        //        var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANTIME.Value.ToString("hh:mm tt"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANNAMT.ToString(), d.DISPSTATUS, d.TRANMID.ToString(), d.GSTAMT.ToString() }).ToArray();
        //        return Json(new
        //        {
        //            sEcho = param.sEcho,
        //            aaData = aaData,
        //            iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
        //            iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        [Authorize(Roles = "BSImportManualBillEdit")]
        public void Edit(int id)
        {

            var strPath = ConfigurationManager.AppSettings["BaseURL"];
            
            Response.Redirect("" + strPath + "/BSImportManualBill/Form/" + id);

            //Response.Redirect("/BSImportManualBill/GSTForm/" + id);
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
                        transactionmaster.SDPTID = 1;
                        transactionmaster.TRANTID = 2;
                        transactionmaster.TRANLMID = 0;
                        transactionmaster.TRANLSID = 0;
                        transactionmaster.TRANLSNO = null;
                        transactionmaster.TRANLMNO = "";
                        transactionmaster.TRANLMDATE = DateTime.Now;
                        transactionmaster.TRANLSDATE = DateTime.Now;
                        transactionmaster.TRANNARTN = null;
                        
                        if (TRANMID == 0)
                        {
                            transactionmaster.CUSRID = Session["CUSRID"].ToString();
                        }
                        transactionmaster.LMUSRID = Session["CUSRID"].ToString();
                        transactionmaster.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
                        transactionmaster.PRCSDATE = DateTime.Now;
                        transactionmaster.TRANDATE = Convert.ToDateTime(F_Form["masterdata[0].TRANTIME"]).Date;

                        if (transactionmaster.TRANDATE > Convert.ToDateTime(todayd))
                        { transactionmaster.TRANDATE = Convert.ToDateTime(todayd); }

                        transactionmaster.TRANTIME = Convert.ToDateTime(F_Form["masterdata[0].TRANTIME"]);

                        if (transactionmaster.TRANTIME > Convert.ToDateTime(todaydt))
                        { transactionmaster.TRANTIME = Convert.ToDateTime(todaydt); }

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

                        //transactionmaster.TRANCGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANCGSTAMT"]);
                        //transactionmaster.TRANSGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANSGSTAMT"]);
                        //transactionmaster.TRANIGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANIGSTAMT"]);

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
                        else bill = "O&M";
                        if (TRANMID == 0)
                        {
                            //transactionmaster.TRANNO = Convert.ToInt16(Autonumber.autonum("transactionmaster", "TRANNO", "TRANNO<>0 and TRANBTYPE=" + Convert.ToInt16(F_Form["TRANBTYPE"]) + " and REGSTRID=" + Convert.ToInt16(F_Form["REGSTRID"]) + " and SDPTID=2 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                            transactionmaster.TRANNO = Convert.ToInt32(auto_numbr_invoice.bsgstautonum("transactionmaster", "TRANNO", F_Form["REGSTRID"].ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString(), Convert.ToInt32(transactionmaster.TRANREFID)).ToString());
                            //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.bsgstautonum("transactionmaster", "TRANNO", F_Form["REGSTRID"].ToString(), Session["compyid"].ToString(), F_Form["TRANBTYPE"].ToString()).ToString());
                            int ano = transactionmaster.TRANNO;
                            string format = "";
                            //string format = "SUD/IMP/";
                            if (regsid == 15)
                                format = "IMP/MNUL/" ;
                            else
                                format = "BSIMP/MNUL/";
                            string btyp = auto_numbr_invoice.GetCateBillType(Convert.ToInt32(transactionmaster.TRANREFID)).ToString();                           
                            if (btyp == "")
                            {
                                format = format + Session["GPrxDesc"] + "/";
                            }
                             else
                                format =format.Replace("/","")+ Session["GPrxDesc"] + btyp;

                            string billformat = "";
                            //switch (regsid)
                            //{
                            //    case 41: billformat = "STL/MNUL/IMP/"; break; //"EXP/LI/"; break;
                            //    case 42: billformat = "STL/MNUL/IMP/"; break; // "EXP/" + bill + "/"; break;
                            //    case 43: billformat = "STL/MNUL/IMP/"; break;

                            //}
                            if (regsid == 15)
                                billformat = "IMP/MNUL/" + bill + "/";
                            else
                                billformat = "BSIMP/MNUL/" + bill + "/";
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
                            int ano = transactionmaster.TRANNO;
                            string format = "";
                            //string format = "SUD/IMP/";
                            if (regsid == 15)
                                format = "IMP/MNUL/" ;
                            else
                                format = "BSIMP/MNUL/" ;
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
                            if (regsid == 15)
                                billformat = "IMP/MNUL/" + bill + "/";
                            else
                                billformat = "BSIMP/MNUL/" + bill + "/";
                            string prfx = string.Format(format + "{0:D5}", ano);
                            string billprfx = string.Format(billformat + "{0:D5}", ano);
                            transactionmaster.TRANDNO = prfx.ToString();
                            transactionmaster.TRANBILLREFNO = billprfx.ToString();
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
                        string[] TRANDREFID = F_Form.GetValues("detaildata[0].TRANDREFID");
                        string[] TRANDREFNAME = F_Form.GetValues("TRANDREFNAME");
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
                            transactiondetail.TRANDREFID = Convert.ToInt32(TRANDREFID[count]);
                            transactiondetail.TRANIDATE = Convert.ToDateTime(TRANIDATE[count]);
                            transactiondetail.TRANSDATE = DateTime.Now;
                            transactiondetail.TRANEDATE = DateTime.Now;
                            transactiondetail.TRANVDATE = DateTime.Now;
                            transactiondetail.TRANVHLFROM = (TRANVHLFROM[count]).ToString();
                            transactiondetail.TRANVHLTO = (TRANVHLTO[count]).ToString();
                            transactiondetail.TRANDGAMT = Convert.ToDecimal(TRANDGAMT[count]);
                            transactiondetail.TRANDNAMT = Convert.ToDecimal(TRANDNAMT[count]);
                            transactiondetail.BILLEDID = Convert.ToInt32(BILLEDID[count]);
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


        //--------Autocomplete CHA Name
        //public JsonResult AutoCha(string term)
        //{
        //    var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
        //                  where r.CATENAME.ToLower().Contains(term.ToLower())
        //                  select new { r.CATENAME, r.CATEID }).Distinct().OrderBy(X => X.CATENAME);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}




        //......end




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

        //..........................Printview...
        // [Authorize(Roles = "ImportImportManualBillPrint")]
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

                if (gsttype == 0)
                { cryRpt.Load( ConfigurationManager.AppSettings["Reporturl"]+  "IMPORT_OS_Detail.RPT"); }
                else
                { cryRpt.Load( ConfigurationManager.AppSettings["Reporturl"]+  "GST_IMPORT_BS_Detail.RPT"); }

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
        //end

        //...............Delete Row.............
        [Authorize(Roles = "BSImportManualBillDelete")]
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
}
