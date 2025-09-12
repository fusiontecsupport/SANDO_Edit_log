
using scfs.Data;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers.NonPnr
{
    [SessionExpire]
    public class NonPnrGateOutController : Controller
    {
        // GET: NonPnrGateOut

        SCFSERPContext context = new SCFSERPContext();

        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

        [Authorize(Roles = "NonPnrGateOutIndex")]
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


        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_NonPnr_GateOut(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), 
                    Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(Session["compyid"]));

                //var aaData = data.Select(d => new string[] { d.GODATE.Value.ToString("dd/MM/yyyy"), d.GODNO.ToString(), d.CONTNRNO, d.CONTNRSCODE, d.IGMNO, d.VSLNAME, d.STMRNAME, d.PRDTDESC.ToString(), d.GIDID.ToString(), d.GODID.ToString() }).ToArray();
                var aaData = data.Select(d => new string[] { d.GODATE.Value.ToString("dd/MM/yyyy"), d.GODNO.ToString(), d.CONTNRNO, d.IGMNO, d.GPLNO, d.CONTNRSCODE, d.ASLMDNO, d.VTTYPE.ToString(), d.CHANAME.ToString(), d.BOENO.ToString(), d.GODID.ToString() }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "NonPnrGateOutEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/NonPnrGateOut/Form/" + id);

            //Response.Redirect("/NonPnrGateOut/Form/" + id);
        }


        [Authorize(Roles = "NonPnrGateOutCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GateOutDetail tab = new GateOutDetail();
            tab.GODATE = DateTime.Now.Date;
            tab.GOTIME = DateTime.Now;
            tab.GODID = 0;
            ViewBag.GIDID = new SelectList(context.Database.SqlQuery<VW_NONPNR_GATEOUT_DETAIL_CTRL_ASSGN>("select * from VW_NONPNR_GATEOUT_DETAIL_CTRL_ASSGN").ToList(), "GIDID", "CONTNRNO");

            //-----------------------------type-----------
            List<SelectListItem> selectedType = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
            selectedType.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
            selectedType.Add(selectedItem1);
            ViewBag.GOBTYPE = selectedType;


            if (id != 0)//Edit Mode
            {
                tab = context.gateoutdetail.Find(id);
                List<SelectListItem> selectedType_ = new List<SelectListItem>();
                if (tab.GOBTYPE == 2)
                {

                    SelectListItem selectedItem = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                    selectedType_.Add(selectedItem);
                    selectedItem = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = true };
                    selectedType_.Add(selectedItem);
                    ViewBag.GOBTYPE = selectedType_;
                }

                //-----------Getting Gate_In Details-----------------//
                //code added by yamuna for VT No input based GO <S> 24-07-2021
                //code modified by Rajesh for validating PART / FULL cargo out
                var qry = context.Database.SqlQuery<VW_NONPNR_GATEOUT_DETAIL_CTRL_ASSGN>("select * from VW_NONPNR_GATEOUT_DETAIL_CTRL_ASSGN WHERE GODID=" + tab.GODID).ToList();


                ViewBag.FVTDNO = Convert.ToInt32(qry[0].VTNO);
                //code added by yamuna for VT No input based GO <S> 24-07-2021

                var query = context.Database.SqlQuery<string>("select CONTNRNO from GATEINDETAIL where GIDID=" + qry[0].GIDID).ToList();
                ViewBag.CONTNRNO = query[0].ToString();
                
            }
            return View(tab);
        }

        
        public JsonResult GetVehicleDetails(int id)
        {
            var data = context.Database.SqlQuery<VW_NONPNR_GATEOUT_DETAIL_CTRL_ASSGN>("SELECT * FROM VW_NONPNR_GATEOUT_DETAIL_CTRL_ASSGN WHERE isnull(GODID,0) = 0 and  VTSTYPE = 1 And VTNO = " + id).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetContDetails(int id)
        {
            var data = context.Database.SqlQuery<VW_NONPNR_GATEOUT_DETAIL_CTRL_ASSGN>("SELECT * FROM VW_NONPNR_GATEOUT_DETAIL_CTRL_ASSGN WHERE GIDID=" + id).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetContDetails_Mod(int id)
        {
            var data = context.Database.SqlQuery<VW_NONPNR_GATEOUT_DETAIL_CTRL_ASSGN_001>("SELECT * FROM VW_NONPNR_GATEOUT_DETAIL_CTRL_ASSGN_001 WHERE GIDID=" + id + "").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public void savedata(GateOutDetail tab)
        {
            tab.COMPYID = Convert.ToInt32(Session["compyid"]);
            tab.SDPTID = 9;
            tab.REGSTRID = 1;
            tab.TRANDID = 0;
            //tab.GODATE = tab.GOTIME.Date;
            
            tab.LMUSRID = Session["CUSRID"].ToString();
            tab.PRCSDATE = DateTime.Now;
            tab.EHIDATE = DateTime.Now;
            tab.EHITIME = DateTime.Now;

            string todaydt = Convert.ToString(DateTime.Now);
            string todayd = Convert.ToString(DateTime.Now.Date);

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

            if ((tab.GODID).ToString() != "0")
            {
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                tab.CUSRID = Session["CUSRID"].ToString();
                tab.GONO = Convert.ToInt32(Autonumber.autonum("GateOutDetail", "GONO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=9").ToString());
                int ano = tab.GONO;
                // string prfx = string.Format("{0:D5}", ano);
                tab.GODNO = ano.ToString();

                
                context.gateoutdetail.Add(tab);
                context.SaveChanges();
               
            }
            //code added to Autopopulate the DeStuff Cargo Out Information to Empty Gate IN Details - By Rajesh S on 27-Jul-2021 <Start>
            //* ----- EMPTY GATE IN INSERT -----*/
            var et = new SCFSERPEntities();
            var data = et.pr_AutoPopulate_NonPNR_GO_DESTUFF_EMPTY_INSERT_GI(tab.GODID, Convert.ToInt32(Session["compyid"]));
            //code added to Autopopulate the DeStuff Cargo Out Information to Empty Gate IN Details -By Rajesh S on 27 - Jul - 2021 < End >

            Response.Redirect("Index");
        }

        //[Authorize(Roles = "NonPnrGateOutPrint")]
        public void PrintView(int? id = 0)
        {

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "IMPORTGATEOUT", Convert.ToInt32(id), Session["CUSRID"].ToString());
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


                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "NonPnr_GateOut.RPT");

                cryRpt.RecordSelectionFormula = "{VW_NONPNR_GATEOUT_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_NONPNR_GATEOUT_PRINT_ASSGN.GODID} = " + id;



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

        #region VtMaxDate
        public JsonResult VtMaxDate(string id)
        {
            int GIDID = 0;

            if (id != "" || id != null)
            {
                GIDID = Convert.ToInt32(id);
            }
            else { GIDID = 0; }

            var data = (from q in context.vehicleticketdetail
                        where q.GIDID == GIDID && q.SDPTID == 9
                        group q by q.VTDATE into g
                        select new { VTDATE = g.Max(t => t.VTDATE) }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //[Authorize(Roles = "NonPnrGateOutDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            var param = id.Split(';');
            String temp = Delete_fun.delete_check1(fld, param[0]);

            if (temp.Equals("PROCEED"))
            {
                GateOutDetail gateoutdetail = context.gateoutdetail.Find(Convert.ToInt32(param[1]));
                context.gateoutdetail.Remove(gateoutdetail);
                context.SaveChanges();
                Response.Write("Deleted successfully...");
            }
            else
                Response.Write(temp);
        }
    }
}