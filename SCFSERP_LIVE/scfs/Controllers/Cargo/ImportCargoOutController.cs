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
using System.IO;
using System.Text;
using System.Net;

namespace scfs_erp.Controllers.Cargo
{
    public class ImportCargoOutController : Controller
    {
        // GET: ImportCargoOut

        #region Context Declaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region Index Form
        [Authorize(Roles = "ImportCargoOutIndex")]
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

        #region Get Ajaxdata
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)/*model 22.edmx*/
        {
            using (var e = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Import_CargoOut(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(Session["compyid"]));
                var aaData = data.Select(d => new string[] { d.GODATE.Value.ToString("dd/MM/yyyy"), d.GOTIME.Value.ToString("hh:mm tt"), d.GODNO.ToString(),d.CONTNRNO,d.CONTNRSDESC,  d.STMRNAME, d.VHLNO,d.PRDTDESC,d.GOQTY.ToString(), d.GODID.ToString() }).ToArray();
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

        #region CForm
        [Authorize(Roles = "ImportCargoOutCreate")]
        public ActionResult CForm(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GateOutDetail tab = new GateOutDetail();

            ViewBag.GIDID = new SelectList(context.VW_CARGO_OUT_CONTAINER_CBX_ASSGNS.OrderBy(x => x.CONTNRNO), "GIDID", "CONTNRNO");

            if (id != 0)//Edit Mode
            {

                tab = context.gateoutdetail.Find(id);
                ViewBag.GIDID = new SelectList(context.gateindetails.Where(x => x.SDPTID == 4).OrderBy(x => x.CONTNRNO), "GIDID", "CONTNRNO", tab.GIDID);
            }
            return View(tab);

        }
        #endregion

        #region Edit Page
        [Authorize(Roles = "ImportCargoOutEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ImportCargoOut/CForm/" + id);
        }
        #endregion
       
        #region Savedate
        public void SaveData(GateOutDetail tab)
            {
            using (context = new SCFSERPContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {

                        tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                        tab.SDPTID = 4;
                        tab.REGSTRID = 1;
                        tab.TRANDID = 0;
                        tab.GOBTYPE = 3;
                        tab.CHASNAME = "";
                        tab.SSEALNO = "";
                        tab.LSEALNO = "";
                        tab.CHAID = 0;
                        //tab.GOTIME = DateTime.Now;
                        //tab.GODATE = tab.GOTIME.Date;

                        tab.LMUSRID = Session["CUSRID"].ToString();
                        tab.PRCSDATE = DateTime.Now;
                        tab.EHIDATE = DateTime.Now;
                        tab.EHITIME = DateTime.Now;
                        tab.CHAID = 0;
                        tab.CHASNAME = "";

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
                            context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                        else
                        {
                            tab.CUSRID = Session["CUSRID"].ToString();
                            tab.GONO = Convert.ToInt32(Autonumber.autonum("GateOutDetail", "GONO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=4 and GOBTYPE=3").ToString());
                            int ano = tab.GONO;
                            string prfx = string.Format("{0:D5}", ano);
                            tab.GODNO = prfx.ToString();
                            //tab.GODNO = ano.ToString();
                            context.gateoutdetail.Add(tab);
                            context.SaveChanges();
                        }
                        //Response.Redirect("Index");


                        trans.Commit(); Response.Write("Save"); //Response.Redirect("Index");


                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Response.Write("Error");
                        //Response.Write("Sorry!!An Error Ocurred..." + ex.Message);
                    }
                }
            }
        }
        #endregion

        #region Get Cargo gatein Details
        public JsonResult GetCargoGateInDet(int id)
        {
            string sqry = "SELECT *FROM VW_CARGOIN_CONTAINER_GATEINDETAILS_ASSGN  WHERE GIDID =" + id;

            var data = context.Database.SqlQuery<VW_CARGOIN_CONTAINER_GATEINDETAILS_ASSGN>(sqry).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCargoGateOutDet(int id)
        {
            string sqry = "SELECT *FROM VW_CARGOOUT_CONTAINER_GATEOUTDETAILS_ASSGN  WHERE GODID =" + id;

            var data = context.Database.SqlQuery<VW_CARGOOUT_CONTAINER_GATEOUTDETAILS_ASSGN>(sqry).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Printview
        [Authorize(Roles = "ImportCargoOutPrint")]
        public void PrintView(int? id = 0)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "IMPORTCARGOOUT", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Import_CargoOut.RPT");
                //cryRpt.Load("D:\\CFSReports\\Import_GateOut.RPT");

                cryRpt.RecordSelectionFormula = "{VW_CARGO_GATE_OUT_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_CARGO_GATE_OUT_CRY_PRINT_ASSGN.GODID} = " + id;

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
                stringbuilder.Clear();
            }

        }
        #endregion

        #region delete
        [Authorize(Roles = "ImportCargoOutDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            var param = id.Split(';');
            String temp = Delete_fun.delete_check1(fld, param[0]);

            if (temp.Equals("PROCEED"))
            {
                GateOutDetail gateoutdetail = context.gateoutdetail.Find(Convert.ToInt32(id));
                context.gateoutdetail.Remove(gateoutdetail);
                context.SaveChanges();
                Response.Write("Deleted successfully...");
            }
            else
                Response.Write(temp);
        }
        #endregion
    }
}