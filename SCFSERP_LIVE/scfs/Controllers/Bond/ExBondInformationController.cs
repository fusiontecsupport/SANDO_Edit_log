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

                CategoryMaster CHA = new CategoryMaster();
                CHA = context.categorymasters.Find(tab.CHAID);
                ViewBag.CHANAME = CHA.CATENAME.ToString();
                CHA = context.categorymasters.Find(tab.IMPRTID);
                ViewBag.IMPRTRNAME = CHA.CATENAME.ToString();

                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Ex_BondMaster_Status ").ToList();
                ViewBag.DISPSTATUS = new SelectList(mtqry, "dval", "dtxt", tab.DISPSTATUS).ToList();


                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Types ").ToList();
                ViewBag.EBNDCTYPE = new SelectList(mtqry, "dval", "dtxt", tab.EBNDCTYPE).ToList();
                BondMaster bm = new BondMaster();
                bm = context.bondinfodtls.Find(tab.BNDID);
                List<SelectListItem> selectedBond = new List<SelectListItem>();
                SelectListItem selectedItemBond = new SelectListItem { Text = bm.BNDDNO, Value = bm.BNDID.ToString(), Selected = true };
                selectedBond.Add(selectedItemBond);
                ViewBag.BNDID = selectedBond;

                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID > 1).Where(x => x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);

                ViewBag.PRDTGID = new SelectList(context.bondproductgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", tab.PRDTGID);
                


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
                            using (var trans = context.Database.BeginTransaction())
                            {

                                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                trans.Commit();
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
    }
}