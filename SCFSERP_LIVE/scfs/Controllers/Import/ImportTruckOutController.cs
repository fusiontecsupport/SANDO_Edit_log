using scfs.Data;
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

namespace scfs_erp.Controllers.Import
{
    [SessionExpire]
    public class ImportTruckOutController : Controller
    {
        // GET: ImportTruckOut

        #region Context declaration
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        #endregion

        #region Index Screen
        [Authorize(Roles = "ImportTruckOutTOIndex")]
        public ActionResult TOIndex()
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
                    

        #region GetAjaxData
        public JsonResult TOGetAjaxData(JQueryDataTableParamModel param)/*model 22.edmx*/
        {
            using (var e = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Import_TruckOut(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(Session["compyid"]));
                var aaData = data.Select(d => new string[] { d.GODATE.Value.ToString("dd/MM/yyyy"), d.GOTIME.Value.ToString("hh:mm tt"), d.GODNO.ToString(), d.VHLNO, d.GODID.ToString() }).ToArray();
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

        #region Edit Form
        [Authorize(Roles = "ImportTruckOutEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ImportTruckOut/Form/" + id);
        }
        #endregion

        #region View Form
        [Authorize(Roles = "ImportTruckOutEdit")]
        public void TIEdit(int id)
        {
            Response.Redirect("/ImportTruckOut/TIForm/" + id);
        }
        #endregion

        #region Truck Out Form

        [Authorize(Roles = "ImportTruckOutForm")]
        public ActionResult TIForm(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GateInDetail tab = new GateInDetail();
            
            if (id != 0)//Edit Mode
            {

                //  var query = context.Database.SqlQuery<VW_EXPORT_GATEOUT_MOD_ASSGN>("select * from VW_EXPORT_GATEOUT_MOD_ASSGN where GODID=" + id).ToList();
                tab = context.gateindetails.Find(id);

                var query = context.Database.SqlQuery<GateInDetail>("select * from GATEINDETAIL where GIDID=" + tab.GIDID).ToList();
                if (query.Count > 0)
                {
                    ViewBag.GIDATE = query[0].GIDATE.ToString("dd/MM/yyyy");
                    ViewBag.STMRNAME = query[0].STMRNAME;
                    ViewBag.IMPRTNAME = query[0].IMPRTNAME;
                }
              




            }
            return View(tab);
        }

        #endregion

        #region Truck Out Form
        [Authorize(Roles = "ImportTruckOutForm")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GateOutDetail tab = new GateOutDetail();
           
            tab.GODID = 0;
            tab.GODATE = DateTime.Now;
            tab.GOTIME = DateTime.Now;

            if (id != 0)//Edit Mode
            {

                //  var query = context.Database.SqlQuery<VW_EXPORT_GATEOUT_MOD_ASSGN>("select * from VW_EXPORT_GATEOUT_MOD_ASSGN where GODID=" + id).ToList();
                tab = context.gateoutdetail.Find(id);
                
                var query = context.Database.SqlQuery<GateInDetail>("select * from GATEINDETAIL where GIDID=" + tab.GIDID).ToList();
                if (query.Count > 0)
                {
                    ViewBag.GIDATE = query[0].GIDATE.ToString("dd/MM/yyyy");
                    ViewBag.STMRNAME = query[0].STMRNAME;
                    ViewBag.IMPRTNAME = query[0].IMPRTNAME;
                }
               

            }
            return View(tab);
        }

        #endregion

        public JsonResult AutoVehicle(string term)/*model2.edmx*/
        {

            var result = (from r in context.VW_IMPORT_GATEOUT_TRUCKNO_CBX_ASSGN
                          where r.AVHLNO.ToLower().Contains(term.ToLower())
                          select new { r.AVHLNO }).Distinct();                          
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult Detail(string id)
        {

            var query = context.Database.SqlQuery<VW_IMPORT_EMPTYGATEIN_CONTAINER_DEATILS_ASSGN>("select TOP 1 * from VW_IMPORT_EMPTYGATEIN_CONTAINER_DEATILS_ASSGN where VHLNO = '" + Convert.ToString(id) + "' ").ToList();
            return Json(query, JsonRequestBehavior.AllowGet);

            //var query = context.Database.SqlQuery<VW_IMPORT_GATEOUT_CONTAINER_CBX_CHNG_ASSGN>("select * from VW_IMPORT_GATEOUT_CONTAINER_CBX_CHNG_ASSGN where GIDID=" + id).ToList();
            //return Json(query, JsonRequestBehavior.AllowGet);
        }

        #region Insert/Modify 
        public void savedata(GateOutDetail tab)
        {
            string todaydt = Convert.ToString(DateTime.Now);
            string todayd = Convert.ToString(DateTime.Now.Date);

            tab.COMPYID = Convert.ToInt32(Session["compyid"]);
            tab.SDPTID = 1;
            tab.REGSTRID = 3;
            tab.TRANDID = 0;
            tab.GOBTYPE = 1;
            tab.LSEALNO = null;
            tab.SSEALNO = null;
            if (tab.CUSRID == null || (tab.GODID).ToString() == "0")
                tab.CUSRID = Session["CUSRID"].ToString();
            tab.LMUSRID = Session["CUSRID"].ToString();
            tab.PRCSDATE = DateTime.Now;
            tab.EHIDATE = DateTime.Now;
            tab.EHITIME = DateTime.Now;

            string indate = Convert.ToString(tab.GODATE);
            if (indate != null || indate != "")
            {
                tab.GODATE = Convert.ToDateTime(indate).Date;
            }
            else { tab.GODATE = DateTime.Now.Date; }

            if (tab.GODATE > Convert.ToDateTime(todayd))
            {
                tab.GODATE = Convert.ToDateTime(todayd);
            }

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

            if (tab.GOTIME > Convert.ToDateTime(todaydt))
            {
                tab.GOTIME = Convert.ToDateTime(todaydt);
            }

            //var ASLDID = Request.Form.Get("ASLDID");
            //var CSEALNO = Request.Form.Get("CSEAL");
            //var ASEALNO = Request.Form.Get("ASEAL");

            if ((tab.GODID).ToString() != "0")
            {
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                tab.GONO = Convert.ToInt32(Autonumber.autonum("GateOutDetail", "GONO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=1").ToString());
                int ano = tab.GONO;
                string prfx = string.Format("{0:D5}", ano);
                tab.GODNO = ano.ToString();
                context.gateoutdetail.Add(tab);
                context.SaveChanges();

                //AuthorizationSlipDetail ad = context.authorizationslipdetail.Find(Convert.ToInt32(ASLDID));
                //context.Entry(ad).Entity.CSEALNO = CSEALNO;
                //context.Entry(ad).Entity.ASEALNO = ASEALNO;
                //context.SaveChanges();


            }
            Response.Redirect("TOIndex");
            //Response.Redirect("Save");
        }
        #endregion

        //..........................Printview...
        [Authorize(Roles = "ImportTruckOutPrint")]
        public void PrintView(int? id = 0)
        {

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "IMPORTTRUCKOUT", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;



                // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");


                //........Get TRANPCOUNT...//
                //var Query = context.Database.SqlQuery<int>("select TRANPCOUNT from transactionmaster where TRANMID=" + id).ToList();
                //var PCNT = 0;

                //if (Query.Count() != 0) { PCNT = Query[0]; }
                //var TRANPCOUNT = ++PCNT;
                //// Response.Write(++PCNT);
                //// Response.End();

                //context.Database.ExecuteSqlCommand("UPDATE transactionmaster SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + id);


                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "IMPORT_TruckOut.RPT");

                cryRpt.RecordSelectionFormula = "{VW_IMPORT_TRUCK_Out_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_IMPORT_TRUCK_Out_PRINT_ASSGN.GODID} = " + id;



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

        #region delete 
        //[Authorize(Roles = "ImportTruckOutDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {

                GateOutDetail god = new GateOutDetail();
                god = context.gateoutdetail.Find(Convert.ToInt32(id));
                context.gateoutdetail.Remove(god);
                context.SaveChanges();

                Response.Write("Deleted Successfully ...");               

                
            }
            else
                Response.Write(temp);
        }
        #endregion
        //end
    }
}