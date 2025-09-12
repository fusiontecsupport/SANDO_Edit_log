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

namespace scfs_erp.Controllers.Export
{
    [SessionExpire]
    public class ExportTruckOutController : Controller
    {
        //
        // GET: /ExportTruckOut/
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        [Authorize(Roles = "ExportTruckOutIndex")]
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
            //return View(context.gateoutdetail.Where(x => x.GODATE >= sd).Where(x => x.GODATE <= ed).ToList());
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_SearchExportTruckOut(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(Session["compyid"]));
                var aaData = data.Select(d => new string[] { d.GODATE.Value.ToString("dd/MM/yyyy"), d.GODNO.ToString(), d.CHASNAME, d.VHLNO, d.GDRVNAME.ToString(), d.GODID.ToString(), d.GIDID.ToString() }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "ExportTruckOutEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ExportTruckOut/Form/" + id);
        }
        //----------------------Initializing Form--------------------------//
        [Authorize(Roles = "ExportTruckOutCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GateOutDetail tab = new GateOutDetail();
            // VW_EXPORT_GATEOUT_MOD_ASSGN tab = new VW_EXPORT_GATEOUT_MOD_ASSGN();
            tab.GODID = 0;
            tab.GODATE = DateTime.Now;
            tab.GOTIME = DateTime.Now;



            if (id != 0)//Edit Mode
            {

                //  var query = context.Database.SqlQuery<VW_EXPORT_GATEOUT_MOD_ASSGN>("select * from VW_EXPORT_GATEOUT_MOD_ASSGN where GODID=" + id).ToList();
                tab = context.gateoutdetail.Find(id);

                var query = context.Database.SqlQuery<DateTime>("select GIDATE from GATEINDETAIL where GIDID=" + tab.GIDID).ToList();
                ViewBag.GIDATE = query[0];




            }
            return View(tab);
        }//End of Form

        //.........................Insert/Modify data.............................//
        public void savedata(GateOutDetail tab)
        {
            tab.COMPYID = Convert.ToInt32(Session["compyid"]);
            tab.SDPTID = 2;
            tab.REGSTRID = 5;
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
                tab.GONO = Convert.ToInt32(Autonumber.autonum("GateOutDetail", "GONO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=2").ToString());
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
            Response.Redirect("Index");
        }

        public JsonResult AutoVehicle(string term)/*model2.edmx*/
        {

            var result = (from r in context.VW_EXPORT_GATEOUT_TRUCKNO_CBX_ASSGN
                          where r.AVHLNO.ToLower().Contains(term.ToLower())
                          select new { r.AVHLNO, r.GIDID, r.CHANAME, r.CHAID, r.GIDATE, r.DRVNAME }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult Detail(int id)
        {

            var query = context.Database.SqlQuery<VW_EXPORT_GATEOUT_CONTAINER_CBX_CHNG_ASSGN>("select * from VW_EXPORT_GATEOUT_CONTAINER_CBX_CHNG_ASSGN where GIDID=" + id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }




        //public JsonResult DetailEdit(int id)
        //{

        //    var query = context.Database.SqlQuery<VW_EXPORT_GATEOUT_MOD_ASSGN>("select * from VW_EXPORT_GATEOUT_MOD_ASSGN where GIDID=" + id).ToList();
        //    return Json(query, JsonRequestBehavior.AllowGet);
        //}

        //..........................Printview...
        [Authorize(Roles = "ExportTruckOutPrint")]
        public void PrintView(int? id = 0)
        {

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "EXPORTTRUCKOUT", Convert.ToInt32(id), Session["CUSRID"].ToString());
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


                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Export_TruckOut.RPT");

                cryRpt.RecordSelectionFormula = "{VW_EXPORT_TRUCK_Out_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_TRUCK_Out_PRINT_ASSGN.GODID} = " + id;



                crConnectionInfo.ServerName = stringbuilder.DataSource;
                crConnectionInfo.DatabaseName = stringbuilder.InitialCatalog;
                crConnectionInfo.UserID = stringbuilder.UserID;
                crConnectionInfo.Password =stringbuilder.Password;

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
        //end
        //...........delete............
        [Authorize(Roles = "ExportTruckOutDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);

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
        //...end
    }
}