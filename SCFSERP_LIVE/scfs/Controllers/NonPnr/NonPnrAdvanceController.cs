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


namespace scfs_erp.Controllers.NonPnr
{
    [SessionExpire]
    public class NonPnrAdvanceController : Controller
    {
        // GET: NonPnrAdvance
        #region Context declaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region NonPnr Advance Index
        [Authorize(Roles = "NonPnrAdvanceIndex")]
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

                var data = e.pr_Search_Advance(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), 9, 59, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANTIME.Value.ToString("hh:mm tt"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANREFAMT.ToString(), d.DISPSTATUS, d.TRANMID.ToString() }).ToArray();
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

        #region Redirect to Form
        [Authorize(Roles = "NonPnrAdvanceEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/NonPnrAdvance/NPAForm/" + id);
        }
        #endregion

        #region Advance Form
        [Authorize(Roles = "NonPnrAdvanceCreate")]
        public ActionResult NPAForm(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            TransactionMaster tab = new TransactionMaster();
            tab.TRANMID = 0;
            tab.TRANTIME = DateTime.Now;

            //..........................................Dropdown data.........................//
            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE != 0).Where(x => x.TRANMODE != 4), "TRANMODE", "TRANMODEDETL");
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.SDPTID = new SelectList(context.softdepartmentmasters.Where(x => x.SDPTID == 2 || x.SDPTID == 1 || x.SDPTID == 5), "SDPTID", "SDPTNAME");
            //.....end

            //........display status.........//
            List<SelectListItem> sds1 = new List<SelectListItem>();
            SelectListItem sid = new SelectListItem { Text = "In Books", Value = "0", Selected = true };
            sds1.Add(sid);
            ViewBag.DISPSTATUS = sds1;
            //.............end......//
            if (id != 0)
            {
                tab = context.transactionmaster.Find(id);
                ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE != 0).Where(x => x.TRANMODE != 4), "TRANMODE", "TRANMODEDETL", tab.TRANMODE);
                ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC", tab.BANKMID);

                List<SelectListItem> selectedDISP = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItemDIS = new SelectListItem { Text = "In Books", Value = "0", Selected = false };
                    selectedDISP.Add(selectedItemDIS);
                    selectedItemDIS = new SelectListItem { Text = "Cancelled", Value = "1", Selected = true };
                    selectedDISP.Add(selectedItemDIS);
                }
                else
                {
                    SelectListItem selectedItemDIS = new SelectListItem { Text = "In Books", Value = "0", Selected = true };
                    selectedDISP.Add(selectedItemDIS);
                    selectedItemDIS = new SelectListItem { Text = "Cancelled", Value = "1", Selected = false };
                    selectedDISP.Add(selectedItemDIS);
                }
                ViewBag.DISPSTATUS = selectedDISP;

            }
            return View(tab);
        }
        #endregion

        #region Insert or Modify data        
        public void Savedata(FormCollection F_Form)
        {
            using (context = new SCFSERPContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        TransactionMaster transactionmaster = new TransactionMaster();
                        TransactionDetail transactiondetail = new TransactionDetail();
                        //-------Getting Primarykey field--------
                        Int32 TRANMID = Convert.ToInt32(F_Form["TRANMID"]);

                        //-----End


                        if (TRANMID != 0)
                        {
                            transactionmaster = context.transactionmaster.Find(TRANMID);
                            transactionmaster.TRANRMKS = (F_Form["TRANRMKS"]).ToString();
                        }

                        //...........transaction master.............//

                        transactionmaster.COMPYID = Convert.ToInt32(Session["compyid"]);
                        transactionmaster.SDPTID = 9;
                        transactionmaster.TRANTID = 2;
                        transactionmaster.TRANLMID = 0;
                        transactionmaster.TRANLSID = 0;
                        transactionmaster.TRANLSNO = null;
                        transactionmaster.TRANLMNO = null;
                        transactionmaster.TRANLMDATE = DateTime.Now;
                        transactionmaster.TRANLSDATE = DateTime.Now;
                        transactionmaster.TRANNARTN = null;
                        if (TRANMID == 0)
                            transactionmaster.CUSRID = Session["CUSRID"].ToString();
                        transactionmaster.LMUSRID = Session["CUSRID"].ToString();
                        //transactionmaster.LMUSRID = 1;
                        transactionmaster.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
                        transactionmaster.PRCSDATE = DateTime.Now;
                        transactionmaster.TRANDATE = Convert.ToDateTime(F_Form["TRANTIME"]).Date;
                        transactionmaster.TRANTIME = Convert.ToDateTime(F_Form["TRANTIME"]);
                        transactionmaster.TRANREFID = Convert.ToInt32(F_Form["TRANREFID"]);
                        transactionmaster.TRANREFNAME = F_Form["TRANREFNAME"].ToString();
                        transactionmaster.LCATEID = 0;
                        transactionmaster.TRANBTYPE = 0;
                        transactionmaster.REGSTRID = 59;
                        transactionmaster.TRANMODE = Convert.ToInt16(F_Form["TRANMODE"]);
                        transactionmaster.TRANMODEDETL = (F_Form["TRANMODEDETL"]);

                        string amt = Convert.ToString(F_Form["TRANREFAMT"]);

                        if (amt == "" || amt == null)
                        {
                            transactionmaster.TRANGAMT = 0;
                            transactionmaster.TRANNAMT = 0;
                        }
                        else
                        {
                            transactionmaster.TRANGAMT = Convert.ToDecimal(amt);
                            transactionmaster.TRANNAMT = Convert.ToDecimal(amt);
                        }
                        //transactionmaster.TRANGAMT = Convert.ToDecimal(F_Form["TRANREFAMT"]);
                        //transactionmaster.TRANNAMT = Convert.ToDecimal(F_Form["TRANREFAMT"]);
                        transactionmaster.TRANROAMT = 0;
                        transactionmaster.TRANREFAMT = Convert.ToDecimal(F_Form["TRANREFAMT"]);
                        if (!F_Form["TRANREFAMT"].Contains('.'))
                        {
                            F_Form["TRANREFAMT"] = F_Form["TRANREFAMT"] + ".00";
                        }
                        transactionmaster.TRANAMTWRDS = AmtInWrd.ConvertNumbertoWords(F_Form["TRANREFAMT"]);

                        var tranmode = Convert.ToInt16(F_Form["TRANMODE"]);
                        if (tranmode == 1)
                        {
                            transactionmaster.TRANREFNO = "";
                            transactionmaster.TRANREFBNAME = "";
                            transactionmaster.BANKMID = 0;
                            transactionmaster.TRANREFDATE = DateTime.Now;
                        }
                        else
                        {
                            transactionmaster.TRANREFNO = (F_Form["TRANREFNO"]).ToString();
                            transactionmaster.TRANREFBNAME = (F_Form["TRANREFBNAME"]).ToString();
                            transactionmaster.BANKMID = Convert.ToInt32(F_Form["BANKMID"]);
                            transactionmaster.TRANREFDATE = Convert.ToDateTime(F_Form["TRANREFDATE"]).Date;

                        }


                        if (TRANMID == 0)
                        {
                            int SDPID = 9;

                            transactionmaster.TRANNO = Convert.ToInt32(Autonumber.autonum("transactionmaster", "TRANNO", "REGSTRID=59 and COMPYID=" + Convert.ToInt32(Session["compyid"]) + "and SDPTID=" + Convert.ToInt32(SDPID) + "and TRANBTYPE=0").ToString());

                            int ano = transactionmaster.TRANNO;
                            string prfx = string.Format("{0:D5}", ano);
                            transactionmaster.TRANDNO = prfx.ToString();
                            context.transactionmaster.Add(transactionmaster);
                            context.SaveChanges();
                            TRANMID = transactiondetail.TRANMID;
                        }
                        else
                        {
                            context.Entry(transactionmaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }



                        trans.Commit(); Response.Redirect("Index");
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        //Response.Write("Sorry!!An Error Ocurred...");
                        Response.Redirect("/Error/AccessDenied");
                    }

                }
            }
            //return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete CHA Name        
        public JsonResult AutoCha(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Print View
        [Authorize(Roles = "NonPnrAdvancePrint")]
        public void PrintView(int? id = 0)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "NonPnrADVANCE", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                var Query = context.Database.SqlQuery<int>("select TRANPCOUNT from transactionmaster where TRANMID=" + id).ToList();
                var PCNT = 0;

                if (Query.Count() != 0) { PCNT = Query[0]; }
                var TRANPCOUNT = ++PCNT;

                context.Database.ExecuteSqlCommand("UPDATE transactionmaster SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + id);

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "NonPnr_Advance.RPT");
                cryRpt.RecordSelectionFormula = "{VW_NONPNR_ADVANCE_PRINT.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_NONPNR_ADVANCE_PRINT.TRANMID} = " + id;

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

        #region Delete Advance
        [Authorize(Roles = "NonPnrAdvanceDelete")]
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
    }
}