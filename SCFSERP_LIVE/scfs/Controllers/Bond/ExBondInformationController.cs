using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using scfs.Data;
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

namespace scfs.Controllers.Bond
{
    public class ExBondInformationController : Controller
    {
        // GET: ExBondInformation

        #region contextdeclaration
        BondContext context = new BondContext();

        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        #endregion

        #region IndexForm
        //[Authorize(Roles = "Ex_BondInformationIndex")]
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

            return View(context.exbondinfodtls.Where(x => x.EBNDDATE >= sd).Where(x => x.EBNDDATE <= ed).Where(x => x.SDPTID == 10).ToList());

        }
        #endregion

        #region GetAjaxData
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {

            using (var e = new BondEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_ExBond_Information(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]));

                var aaData = data.Select(d => new string[] { d.EBNDDATE, d.EBNDNO.ToString(), d.EBNDDNO, d.CHANAME, d.IMPRTRNAME, d.EBNDASSAMT.ToString(), d.DISPSTATUS, d.EBNDID.ToString() }).ToArray();

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

        #region FormModify
        //[Authorize(Roles = "Ex_BondInformationEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            //string url = "" + strPath + "/ExBondInformation/Form/" + id;

            Response.Redirect("" + strPath + "/ExBondInformation/Form/" + id);

            //Response.Redirect("/ExBondInformation/Form/" + id);
        }
        #endregion

        #region Form
        ////[Authorize(Roles = "Ex_BondInformationCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            Ex_BondMaster tab = new Ex_BondMaster();

            tab.EBNDDATE = Convert.ToDateTime(DateTime.Now).Date;
            tab.INSRSDATE = DateTime.Now.Date;


            //-------------------Dropdown List--------------------------------------------------//

            var mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Ex_BondMaster_Status ").ToList();
            ViewBag.DISPSTATUS = new SelectList(mtqry, "dval", "dtxt").ToList();

            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Types ").ToList();
            ViewBag.EBNDCTYPE = new SelectList(mtqry, "dval", "dtxt").ToList();

            ViewBag.BNDID = new SelectList("");
            ViewBag.PRDTGID = new SelectList(context.bondproductgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");
            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID > 1).Where(x => x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");

            if (id != 0)//--Edit Mode
            {
                tab = context.exbondinfodtls.Find(id);

                if (tab != null)
                {
                    CategoryMaster CHA = new CategoryMaster();
                    if (tab.CHAID != null && tab.CHAID != 0)
                    {
                        CHA = context.categorymasters.Find(tab.CHAID);
                        if (CHA != null)
                        {
                            ViewBag.CHANAME = CHA.CATENAME.ToString();
                        }
                    }
                    if (tab.IMPRTID != null && tab.IMPRTID != 0)
                    {
                        CHA = context.categorymasters.Find(tab.IMPRTID);
                        if (CHA != null)
                        {
                            ViewBag.IMPRTRNAME = CHA.CATENAME.ToString();
                        }
                    }

                    mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Ex_BondMaster_Status ").ToList();
                    ViewBag.DISPSTATUS = new SelectList(mtqry, "dval", "dtxt", tab.DISPSTATUS).ToList();


                    mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Types ").ToList();
                    ViewBag.EBNDCTYPE = new SelectList(mtqry, "dval", "dtxt", tab.EBNDCTYPE).ToList();
                    
                    if (tab.BNDID != null && tab.BNDID != 0)
                    {
                        BondMaster bm = context.bondinfodtls.Find(tab.BNDID);
                        if (bm != null)
                        {
                            List<SelectListItem> selectedBond = new List<SelectListItem>();
                            SelectListItem selectedItemBond = new SelectListItem { Text = bm.BNDDNO, Value = bm.BNDID.ToString(), Selected = true };
                            selectedBond.Add(selectedItemBond);
                            ViewBag.BNDID = selectedBond;
                        }
                        else
                        {
                            ViewBag.BNDID = new SelectList("");
                        }
                    }
                    else
                    {
                        ViewBag.BNDID = new SelectList("");
                    }

                    ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID > 1).Where(x => x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);

                    ViewBag.PRDTGID = new SelectList(context.bondproductgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", tab.PRDTGID);
                }
            }


            return View(tab);
        }
        #endregion

        #region Ex Bond Details for Selected Ex Bond ID
        public JsonResult GetExBondDetail(string id)//vehicl
        {

            var bndid = 0;

            if (id != "" && id != "0" && id != null && id != "undefined")
            { bndid = Convert.ToInt32(id); }
            var compyid = 0;
            compyid = Convert.ToInt32(Session["compyid"]);

            var query = context.Database.SqlQuery<pr_Get_ExBond_Info_Result>("exec pr_Get_ExBond_Info @compyid  = " + compyid + " ,  @exbondid  = " + bndid).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        #endregion
              
        #region Bond Details for Selected CHA & Importer
        public JsonResult GetBondNos(string id)
        {
            var param = id.Split('~');
            var chaid = 0;
            var imprtrid = 0;

            if (param[0] != "" || param[0] != "0" || param[0] != null)
            { chaid = Convert.ToInt32(param[0]); }
            else { chaid = 0; }

            if (param[1] != "" || param[1] != "0" || param[1] != null)
            { imprtrid = Convert.ToInt32(param[1]); }
            else { imprtrid = 0; }
            
            var compyid = 0;
            compyid = Convert.ToInt32(Session["compyid"]);


            string bqry = "exec pr_Get_Bond_List @compyid  = " + compyid + " ,  @chaid  = " + chaid + " ,  @imprtrid  = " + imprtrid + ", @opt =0";
            var query = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>(bqry).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Savedata
        public void SaveData(Ex_BondMaster tab)
        {
            
            using (BondContext context = new BondContext())
            {
                try
                {

                    var sql = context.Database.SqlQuery<int>("select count('*') from exbondmaster(nolock) Where EBNDDNO ='" + tab.EBNDDNO + "' and EBNDID <>"+tab.EBNDID +" and SDPTID=10").ToList(); //COMPYID = " + Convert.ToInt32(Session["compyid"]) + " and 

                    if (sql[0] > 0)
                    {
                        Response.Write("Exists");
                    }
                    else
                    {
                        string todaydt = Convert.ToString(DateTime.Now);
                        string todayd = Convert.ToString(DateTime.Now.Date);


                        tab.PRCSDATE = DateTime.Now;
                        tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                        tab.SDPTID = 10;

                        if (tab.EBNDID.ToString() != "0")
                            tab.LMUSRID = Session["CUSRID"].ToString();
                        else
                            tab.CUSRID = Session["CUSRID"].ToString();



                        if (tab.EBNDID.ToString() != "0")
                        {
                            // Load original record for logging (no tracking to avoid state conflicts)
                            var original = context.exbondinfodtls.AsNoTracking().FirstOrDefault(x => x.EBNDID == tab.EBNDID);

                            using (var trans = context.Database.BeginTransaction())
                            {

                                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                trans.Commit();
                            }

                            // Best-effort logging to SCFSERP_EditLog
                            try
                            {
                                System.Diagnostics.Debug.WriteLine($"EXBOND INFORMATION SAVE METHOD CALLED: EBNDID={tab.EBNDID}, EBNDDNO={tab.EBNDDNO}");
                                if (original != null)
                                {
                                    System.Diagnostics.Debug.WriteLine($"ORIGINAL RECORD FOUND: EBNDID={original.EBNDID}, calling LogExBondEdits");
                                    // Ensure baseline snapshot (Version = "0") exists for this record before logging diffs
                                    EnsureBaselineVersionZero(original, Session["CUSRID"] != null ? Session["CUSRID"].ToString() : "");
                                    LogExBondEdits(original, tab, Session["CUSRID"] != null ? Session["CUSRID"].ToString() : "");
                                    System.Diagnostics.Debug.WriteLine($"LogExBondEdits completed successfully");
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"ORIGINAL RECORD NOT FOUND for EBNDID={tab.EBNDID}");
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log the error for debugging but don't fail the save
                                System.Diagnostics.Debug.WriteLine($"ExBond Information edit logging failed: {ex.Message}");
                                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                            }
                        }

                        else
                        {

                            using (var trans = context.Database.BeginTransaction())
                            {
                                tab.EBNDNO = Convert.ToInt32(Autonumber.autonum("exbondmaster", "EBNDNO", "EBNDNO <> 0 AND SDPTID = 10 and compyid = " + Convert.ToInt32(Session["compyid"]) + "").ToString());
                                //int ano = tab.EBNDNO;
                                //string prfx = string.Format("{0:D5}", ano);
                                //tab.EBNDDNO = prfx.ToString();


                                context.exbondinfodtls.Add(tab);
                                //context.Entry(tab).State = System.Data.Entity.EntityState.Added;
                                context.SaveChanges();
                                trans.Commit();
                            }

                        }
                        Response.Write("Success");


                    }
                }

                catch (Exception E)
                {
                    Response.Write(E);
                    //trans.Rollback();
                    Response.Write("Sorry!! An Error Occurred.... ");
                    //Response.Redirect("/Error/AccessDenied");
                }
                //}
            }


        }
        #endregion
        #region ExBond Number Duplicate Check
        public void ExBondNo_Duplicate_Check(string EBNDDNO)
        {
            EBNDDNO = Request.Form.Get("EBNDDNO");

            string temp = ExBondNo_Check.recordCount(EBNDDNO);
            if (temp != "PROCEED")
            {
                Response.Write("Ex Bond Number already exists");

            }
            else
            {
                Response.Write("PROCEED");
            }

        }

        #endregion

        #region Autocomplete Steamer Name    
        public JsonResult AutoSteamer(string term)
        {
            var result = (from category in context.categorymasters.Where(m => m.CATETID == 3).Where(x => x.DISPSTATUS == 0)
                          where category.CATENAME.ToLower().Contains(term.ToLower())
                          select new { category.CATENAME, category.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Importer Name          
        public JsonResult AutoImporter(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 1).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete CHA Name  
        public JsonResult AutoChaname(string term)
        {
            var result = (from r in context.categorymasters.Where(x => x.CATETID == 4 && x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).OrderBy(x => x.CATENAME).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Billing CHA Name  
        public JsonResult AutoBChaname(string term)
        {
            //var result = (from r in context.categorymasters.Where(x => (x.CATETID == 4) && x.DISPSTATUS == 0)
            //              where r.CATENAME.ToLower().Contains(term.ToLower())
            //              select new { r.CATENAME, r.CATEID }).OrderBy(x => x.CATENAME).Distinct();
            var e = new SCFSERPEntities();
            var result = e.pr_Fetch_CHAIMP_Dtl(4, term.ToString());

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region PrintView
        //[Authorize(Roles = "Ex_BondInformationPrint")]
        public void PrintView(int? id = 0)
        {
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "Ex_BondInformation", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "ExBondInfo.rpt");
                cryRpt.RecordSelectionFormula = "{VW_EX_BOND_PRINT_ASSGN.KUSRID} ='" + Session["CUSRID"].ToString() + "' and {VW_EX_BOND_PRINT_ASSGN.EBNDID} =" + id;

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


        //[Authorize(Roles = "Ex_BondInformationPrint")]
        public void TPrintView(int? id = 0)/*truck*/
        {
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "NONPNRTRUCKIN", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                //cryRpt.Load("D:\\scfsreports\\NonPnr_TruckIn.rpt");
                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "BondInfo.rpt");
                cryRpt.RecordSelectionFormula = "{VW_EX_BOND_PRINT_ASSGN.KUSRID} ='" + Session["CUSRID"].ToString() + "' and {VW_EX_BOND_PRINT_ASSGN.EBNDID} =" + id;

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
        #endregion

        #region DeleteBondInfo        
        //[Authorize(Roles = "Ex_BondInformationDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            // Code Modified for validating the by Rajesh / Yamuna on 16-Jul-2021 <Start>
            String temp = Delete_fun.delete_check1(fld, id);
            //var d = context.Database.SqlQuery<int>("Select count(EBNDID) as 'Cnt' from AUTHORIZATIONSLIPDETAIL (nolock) where EBNDID=" + Convert.ToInt32(id)).ToList();


            //if (d[0] == 0 || d[0] == null )
            if (temp.Equals("PROCEED"))
            // Code Modified for validating the by Rajesh / Yamuna on 16-Jul-2021 <End>
            {
                var sql = context.Database.SqlQuery<int>("SELECT EBNDID from ExBondMaster where EBNDID=" + Convert.ToInt32(id)).ToList();
                var bndid = (sql[0]).ToString();
                Ex_BondMaster exbondinfodtls = context.exbondinfodtls.Find(Convert.ToInt32(bndid));
                context.exbondinfodtls.Remove(exbondinfodtls);
                context.SaveChanges();

                Response.Write("Deleted Successfully ...");
            }
            else
            {
                // Code Modified for validating the by Rajesh / Yamuna on 16-Jul-2021 <Start>
                Response.Write("Deletion is not possible!");
                // Code Modified for validating the by Rajesh / Yamuna on 16-Jul-2021 <End>
            }

        }
        #endregion

        #region Edit Logging Methods
        private void LogExBondEdits(Ex_BondMaster before, Ex_BondMaster after, string userId)
        {
            if (before == null || after == null)
            {
                System.Diagnostics.Debug.WriteLine($"LogExBondEdits: before={before != null}, after={after != null}");
                return;
            }
            var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
            if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
            {
                System.Diagnostics.Debug.WriteLine("LogExBondEdits: No SCFSERP_EditLog connection string found");
                return;
            }

            // Exclude system or noisy fields
            var exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "EBNDID", "PRCSDATE", "LMUSRID", "CUSRID", "COMPYID", "SDPTID",
                "IMPRTID", "CHAID", "BNDID", "CONTNRSID", "PRDTGID", "EBNDCTYPE"
            };

            // Compute the next version ONCE per save
            int nextVersion = 1;
            try
            {
                using (var sql = new SqlConnection(cs.ConnectionString))
                using (var cmd = new SqlCommand(@"
                    SELECT ISNULL(
                        MAX(TRY_CAST(
                            SUBSTRING([Version], 2, 
                                CASE WHEN CHARINDEX('-', [Version]) > 0 
                                     THEN CHARINDEX('-', [Version]) - 2 
                                     ELSE LEN([Version]) - 1
                                END
                            ) AS INT)
                        ), 0) + 1
                    FROM [dbo].[GateInDetailEditLog]
                    WHERE [GIDNO] = @GIDNO AND [Modules] = 'ExBondGateIn'", sql))
                {
                    cmd.Parameters.AddWithValue("@GIDNO", after.EBNDDNO ?? after.EBNDID.ToString());
                    sql.Open();
                    var obj = cmd.ExecuteScalar();
                    if (obj != null && obj != DBNull.Value)
                        nextVersion = Convert.ToInt32(obj);
                }
            }
            catch { /* ignore logging version errors */ }

            var props = typeof(Ex_BondMaster).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            int fieldsChecked = 0;
            int fieldsChanged = 0;

            foreach (var p in props)
            {
                if (!p.CanRead) continue;
                if (p.PropertyType.IsClass && p.PropertyType != typeof(string) && !p.PropertyType.IsValueType)
                    continue;
                if (exclude.Contains(p.Name)) continue;

                fieldsChecked++;
                var ov = p.GetValue(before, null);
                var nv = p.GetValue(after, null);

                if (BothNull(ov, nv)) continue;

                var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                bool changed = false;
                bool shouldLog = true;

                if (type == typeof(decimal) || type == typeof(decimal?))
                {
                    var d1 = ToNullableDecimal(ov) ?? 0m;
                    var d2 = ToNullableDecimal(nv) ?? 0m;
                    if (d1 == 0m && d2 == 0m) { shouldLog = false; continue; }
                    changed = d1 != d2;
                }
                else if (type == typeof(int) || type == typeof(int?) || type == typeof(long) || type == typeof(long?) || type == typeof(short) || type == typeof(short?))
                {
                    var i1 = ov == null ? (long?)null : Convert.ToInt64(ov);
                    var i2 = nv == null ? (long?)null : Convert.ToInt64(nv);
                    if (!i1.HasValue && !i2.HasValue) continue;
                    var val1 = i1 ?? 0;
                    var val2 = i2 ?? 0;
                    changed = val1 != val2;
                    if (val1 == 0 && val2 == 0) { shouldLog = false; continue; }
                }
                else if (type == typeof(DateTime) || type == typeof(DateTime?))
                {
                    var t1 = (ov as DateTime?) ?? default(DateTime);
                    var t2 = (nv as DateTime?) ?? default(DateTime);
                    if (t1 == default(DateTime) && t2 == default(DateTime)) continue;
                    if (p.Name.Contains("DATE") && !p.Name.Contains("TIME"))
                    {
                        changed = t1.Date != t2.Date;
                    }
                    else
                    {
                        changed = t1 != t2;
                    }
                }
                else if (type == typeof(string))
                {
                    var s1 = (Convert.ToString(ov) ?? string.Empty).Trim();
                    var s2 = (Convert.ToString(nv) ?? string.Empty).Trim();
                    bool def1 = string.IsNullOrEmpty(s1) || s1 == "-" || s1 == "0";
                    bool def2 = string.IsNullOrEmpty(s2) || s2 == "-" || s2 == "0";
                    if (def1 && def2) { shouldLog = false; continue; }
                    changed = !string.Equals(s1, s2, StringComparison.Ordinal);
                }
                else
                {
                    var s1 = FormatVal(ov);
                    var s2 = FormatVal(nv);
                    if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2)) { shouldLog = false; continue; }
                    changed = !string.Equals(s1 ?? "", s2 ?? "", StringComparison.Ordinal);
                }

                if (!shouldLog || !changed) continue;

                fieldsChanged++;
                var os = FormatValForLogging(p.Name, ov) ?? "";
                var ns = FormatValForLogging(p.Name, nv) ?? "";

                var gidno = after.EBNDDNO ?? after.EBNDID.ToString();
                var versionLabel = $"V{nextVersion}-{gidno}";
                InsertEditLogRow(cs.ConnectionString, gidno, p.Name, os, ns, userId, versionLabel, "ExBondGateIn");
            }

            System.Diagnostics.Debug.WriteLine($"LogExBondEdits: Checked {fieldsChecked} fields, {fieldsChanged} fields changed");
        }

        private static string FormatVal(object value)
        {
            if (value == null) return null;
            if (value is DateTime dt) return dt.ToString("dd/MM/yyyy");
            if (value is DateTime?)
            {
                var ndt = (DateTime?)value;
                return ndt.HasValue ? ndt.Value.ToString("dd/MM/yyyy") : null;
            }
            if (value is decimal dec) return dec.ToString("0.####");
            var ndecs = value as decimal?;
            if (ndecs.HasValue) return ndecs.Value.ToString("0.####");
            return Convert.ToString(value);
        }

        private string FormatValForLogging(string fieldName, object value)
        {
            if (value == null) return null;

            if (value is DateTime dt)
            {
                return dt.ToString("dd/MM/yyyy");
            }
            if (value is DateTime?)
            {
                var ndt = (DateTime?)value;
                if (!ndt.HasValue) return null;
                return ndt.Value.ToString("dd/MM/yyyy");
            }

            var formattedValue = FormatVal(value);
            if (string.IsNullOrEmpty(formattedValue)) return formattedValue;

            try
            {
                if (fieldName.Equals("PRDTGID", StringComparison.OrdinalIgnoreCase))
                {
                    int productGroupId;
                    if (int.TryParse(formattedValue, out productGroupId) && productGroupId > 0)
                    {
                        var productGroup = context.bondproductgroupmasters.FirstOrDefault(p => p.PRDTGID == productGroupId);
                        if (productGroup != null && !string.IsNullOrEmpty(productGroup.PRDTGDESC))
                            return productGroup.PRDTGDESC;
                    }
                }
                else if (fieldName.Equals("CONTNRSID", StringComparison.OrdinalIgnoreCase))
                {
                    int containerSizeId;
                    if (int.TryParse(formattedValue, out containerSizeId) && containerSizeId > 0)
                    {
                        var containerSize = context.containersizemasters.FirstOrDefault(c => c.CONTNRSID == containerSizeId);
                        if (containerSize != null && !string.IsNullOrEmpty(containerSize.CONTNRSDESC))
                            return containerSize.CONTNRSDESC;
                    }
                }
                else if (fieldName.Equals("EBNDCTYPE", StringComparison.OrdinalIgnoreCase))
                {
                    int bondTypeId;
                    if (int.TryParse(formattedValue, out bondTypeId) && bondTypeId > 0)
                    {
                        try
                        {
                            var bondTypes = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Types").ToList();
                            var bondType = bondTypes.FirstOrDefault(b => b.dval == bondTypeId);
                            if (bondType != null && !string.IsNullOrEmpty(bondType.dtxt))
                                return bondType.dtxt;
                        }
                        catch { }
                    }
                }
                else if (fieldName.Equals("DISPSTATUS", StringComparison.OrdinalIgnoreCase))
                {
                    if (formattedValue == "1") return "Disabled";
                    if (formattedValue == "0") return "Enabled";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FormatValForLogging lookup failed for {fieldName}: {ex.Message}");
            }

            return formattedValue;
        }

        private static bool BothNull(object a, object b) => a == null && b == null;

        private static decimal? ToNullableDecimal(object v)
        {
            if (v == null) return null;
            if (v is decimal d) return d;
            var nd = v as decimal?;
            if (nd.HasValue) return nd.Value;
            decimal parsed;
            return decimal.TryParse(Convert.ToString(v), out parsed) ? parsed : (decimal?)null;
        }

        private static void InsertEditLogRow(string connectionString, string gidno, string fieldName, string oldValue, string newValue, string changedBy, string versionLabel, string modules)
        {
            try
            {
                using (var sql = new SqlConnection(connectionString))
                {
                    sql.Open();
                    using (var cmd = new SqlCommand(@"INSERT INTO [dbo].[GateInDetailEditLog]
                        ([GIDNO], [FieldName], [OldValue], [NewValue], [ChangedBy], [ChangedOn], [Version], [Modules])
                        VALUES (@GIDNO, @FieldName, @OldValue, @NewValue, @ChangedBy, GETDATE(), @Version, @Modules)", sql))
                    {
                        cmd.Parameters.AddWithValue("@GIDNO", gidno);
                        cmd.Parameters.AddWithValue("@FieldName", fieldName);
                        cmd.Parameters.AddWithValue("@OldValue", (object)oldValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NewValue", (object)newValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ChangedBy", changedBy ?? "");
                        cmd.Parameters.AddWithValue("@Version", (object)versionLabel ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Modules", modules ?? string.Empty);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to insert edit log: {ex.Message}");
                throw;
            }
        }

        private void EnsureBaselineVersionZero(Ex_BondMaster snapshot, string userId)
        {
            try
            {
                var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
                if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString)) return;

                var gidno = snapshot.EBNDDNO ?? snapshot.EBNDID.ToString();
                if (string.IsNullOrWhiteSpace(gidno)) return;

                using (var sql = new SqlConnection(cs.ConnectionString))
                using (var cmd = new SqlCommand("SELECT COUNT(1) FROM [dbo].[GateInDetailEditLog] WHERE [GIDNO]=@GIDNO AND [Modules]='ExBondGateIn' AND (RTRIM(LTRIM([Version]))=@VLower OR RTRIM(LTRIM([Version]))=@VUpper OR RTRIM(LTRIM([Version]))='0' OR RTRIM(LTRIM([Version]))='V0')", sql))
                {
                    cmd.Parameters.AddWithValue("@GIDNO", gidno);
                    var baselineVerLower = "v0-" + gidno;
                    var baselineVerUpper = "V0-" + gidno;
                    cmd.Parameters.AddWithValue("@VLower", baselineVerLower);
                    cmd.Parameters.AddWithValue("@VUpper", baselineVerUpper);
                    sql.Open();
                    var exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    if (exists) return;
                }

                InsertBaselineSnapshot(snapshot, userId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EnsureBaselineVersionZero failed: {ex.Message}");
            }
        }

        private void InsertBaselineSnapshot(Ex_BondMaster snapshot, string userId)
        {
            var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
            if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString)) return;

            var gidno = snapshot.EBNDDNO ?? snapshot.EBNDID.ToString();
            if (string.IsNullOrWhiteSpace(gidno)) return;
            var baselineVer = "v0-" + gidno;

            var exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "EBNDID", "PRCSDATE", "LMUSRID", "CUSRID", "COMPYID", "SDPTID",
                "IMPRTID", "CHAID", "BNDID", "CONTNRSID", "PRDTGID", "EBNDCTYPE"
            };

            var props = typeof(Ex_BondMaster).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var p in props)
            {
                if (!p.CanRead) continue;
                if (p.PropertyType.IsClass && p.PropertyType != typeof(string) && !p.PropertyType.IsValueType)
                    continue;
                if (exclude.Contains(p.Name)) continue;

                var val = p.GetValue(snapshot, null);
                if (val == null) continue;

                var formatted = FormatValForLogging(p.Name, val);
                if (string.IsNullOrEmpty(formatted)) continue;

                InsertEditLogRow(cs.ConnectionString, gidno, p.Name, null, formatted, userId, baselineVer, "ExBondGateIn");
            }
        }
        #endregion
    }
}