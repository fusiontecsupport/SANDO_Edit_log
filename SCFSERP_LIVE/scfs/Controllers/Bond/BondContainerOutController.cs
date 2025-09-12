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

namespace scfs_erp.Controllers.Bond
{
    public class BondContainerOutController : Controller
    {
        // GET: BondContainerOut
        #region Context Declaration
        BondContext context = new BondContext();
        #endregion

        #region Index Page
        //[Authorize(Roles = "BondGOIndex")]
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

            //....end    
            DateTime fromdate = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;
            DateTime todate = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;

            return View();
        }
        #endregion

        #region Get Table Data
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new BondEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Bond_GateOut(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                var aaData = data.Select(d => new string[] { d.GODATE.Value.ToString("dd/MM/yyyy"), d.GODNO.ToString(), d.CONTNRNO.ToString(), d.CONTNRSDESC,   d.PRDTDESC, d.VHLNO, d.GIDID.ToString(), d.GODID.ToString() }).ToArray();
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
        //[Authorize(Roles = "BondContainerOutEdit")]
        public void Edit(string id)
        {
            Response.Redirect("/BondContainerOut/Form/" + id);
        }
        #endregion

        #region Bond Container GO form
        //[Authorize(Roles = "BondContainerOutCreate")]
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            BondContainerOut tab = new BondContainerOut();

            var GODID = 0;
            if (id != "0")
            {
                GODID = Convert.ToInt32(id);
            }
            else
            {
                tab.GODATE = DateTime.Now.Date;
                tab.GOTIME = DateTime.Now;
            }

            tab.GODID = 0;

            var mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_GI_Nos_GO 0 ").ToList();
            ViewBag.GIDID = new SelectList(mtqry, "dval", "dtxt").ToList();

            if (GODID != 0)//Edit Mode
            {
                tab = context.bondcontnroutdtls.Find(GODID);

                //-----------Getting Gate_In Details-----------------//

                

                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_GI_Nos_GO " + GODID).ToList();
                ViewBag.GIDID = new SelectList(mtqry, "dval", "dtxt", tab.GIDID).ToList();

                


            }
            return View(tab);
        }
        #endregion


        #region Get Container Details
        public JsonResult GetContainerDtl(string id)
        {
            int GId = 0;

            if (id != "" && id != "0")
            {
                GId = Convert.ToInt32(id);
                var query1 = (from m in context.bondgateindtls
                              join b in context.bondinfodtls on m.BNDID equals b.BNDID
                              join c in context.categorymasters on m.CHAID equals c.CATEID
                              join i in context.categorymasters on m.IMPRTID equals i.CATEID
                              where (m.GIDID == GId && c.CATETID == 4 && i.CATETID == 1)

                              select new { m.IGMNO,m.GPLNO, CHANAME = c.CATENAME, m.CHAID, m.GIDNO, b.BNDDNO, m.GIDID, m.PRDTDESC, m.IMPRTID, IMPRTRNAME = i.CATENAME, m.STMRNAME }
                                ).ToList();
                return Json(query1, JsonRequestBehavior.AllowGet);
            }
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region savedata
        public void savedata(BondContainerOut tab)
        {
            using (context = new BondContext())
            {

                try
                {
                    using (var trans = context.Database.BeginTransaction())
                    {
                        tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                        tab.SDPTID = 10;
                        tab.DISPSTATUS = 0;
                        tab.PRCSDATE = DateTime.Now;
                        
                        string GIDID = Convert.ToString(tab.GIDID);

                        if (GIDID == "" || GIDID == null)
                        { tab.GIDID = 0; }
                        else { tab.GIDID = Convert.ToInt32(GIDID); }


                        string indate = Convert.ToString(tab.GODATE);
                        if (indate != null || indate != "")
                        {
                            tab.GODATE = Convert.ToDateTime(indate).Date;
                        }
                        else { tab.GODATE = DateTime.Now.Date; }

                        string intime = Convert.ToString(tab.GOTIME);
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

                                    tab.GOTIME = Convert.ToDateTime(in_datetime);
                                }
                                else { tab.GOTIME = DateTime.Now; }
                            }
                            else { tab.GOTIME = DateTime.Now; }
                        }
                        else { tab.GOTIME = DateTime.Now; }


                        if ((tab.GODID).ToString() != "0")
                        {
                            tab.LMUSRID = Session["CUSRID"].ToString();
                            context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                        else
                        {
                            tab.GONO = Convert.ToInt32(Autonumber.autonum("BONDGATEOUTDETAIL", "GONO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=10").ToString());
                            tab.CUSRID = Session["CUSRID"].ToString();
                            int ano = tab.GONO;
                            string prfx = string.Format("{0:D5}", ano);
                            tab.GODNO = prfx.ToString();
                            context.bondcontnroutdtls.Add(tab);
                            context.SaveChanges();
                        }

                        trans.Commit(); Response.Redirect("Index");
                    }
                }
                catch(Exception ex)
                {
                    //trans.Rollback();
                    var msg = ex.Message;
                    Response.Redirect("/Error/AccessDenied");
                }
            }
        }
        #endregion

        #region Delete Bond Container Out
        //[Authorize(Roles = "BondContainerOutDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            // Code Modified for validating the by Rajesh / Yamuna on 16-Jul-2021 <Start>
            String temp = Delete_fun.delete_check1(fld, id);
            //var d = context.Database.SqlQuery<int>("Select count(GIDID) as 'Cnt' from AUTHORIZATIONSLIPDETAIL (nolock) where GIDID=" + Convert.ToInt32(id)).ToList();


            //if (d[0] == 0 || d[0] == null )
            if (temp.Equals("PROCEED"))
            // Code Modified for validating the by Rajesh / Yamuna on 16-Jul-2021 <End>
            {
                BondContainerOut bondgo = context.bondcontnroutdtls.Find(Convert.ToInt32(id));
                context.bondcontnroutdtls.Remove(bondgo);
                context.SaveChanges();

                Response.Write("Deleted Successfully ...");
            }
            else
            {
                Response.Write("Record already exists, deletion is not possible!");
            }

        }
        #endregion

        #region Print Page

        //[Authorize(Roles = "BondGOPrint")]
        public void PrintView(int? id = 0)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "BONDGATEOUTDETAIL", Convert.ToInt32(id), Session["CUSRID"].ToString());

            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Bond_GateOut.rpt");
                cryRpt.RecordSelectionFormula = "{VW_BOND_GATE_OUT_PRINT_ASSGN.KUSRID} ='" + Session["CUSRID"].ToString() + "' and {VW_BOND_GATE_OUT_PRINT_ASSGN.GODID} =" + id;

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


    }
}