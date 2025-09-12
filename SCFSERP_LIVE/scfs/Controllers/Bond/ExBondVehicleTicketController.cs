using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using scfs.Data;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace scfs.Controllers.Bond
{
    public class ExBondVehicleTicketController : Controller
    {
        // GET: ExBondVehicleTicket
        #region Context Declaration
        BondContext context = new BondContext();

        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

        #endregion

        #region Index Page
        //[Authorize(Roles = "ExBondVTIndex")]
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

                var data = e.pr_Search_Bond_Vehicle_Ticket(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                var aaData = data.Select(d => new string[] { d.VTDATE.Value.ToString("dd/MM/yyyy"), d.VTDNO.ToString(),  d.EBNDDNO, d.BNDDNO, d.PRDTGDESC.ToString(), d.VHLNO, d.DRIVERNAME, d.VTQTY.ToString(), d.VTDID.ToString() }).ToArray();
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
        //[Authorize(Roles = "ExBondVehicleTicketEdit")]
        public void Edit(string id)
        {
            Response.Redirect("/ExBondVehicleTicket/Form/" + id);
        }
        #endregion

        #region VT Cargo form
        //[Authorize(Roles = "ExBondVehicleTicketCreate")]
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ExBondVehicleTicket tab = new ExBondVehicleTicket();

            var VTDID = 0;
            if (id != "0")
            {
                VTDID = Convert.ToInt32(id);
            }
            else
            {
                tab.VTDATE = DateTime.Now.Date;
                tab.VTTIME = DateTime.Now;
            }

            tab.VTDID = 0;

            var mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_ExBond_Nos_VT 0 ").ToList();
            ViewBag.EBNDID = new SelectList(mtqry.OrderBy(x => x.dtxt), "dval", "dtxt").ToList();
            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID > 1).Where(x => x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");
            if (VTDID != 0)//Edit Mode
            {
                tab = context.exbondvtdtls.Find(VTDID); 
                
                //-----------Getting Gate_In Details-----------------//

                var query = context.Database.SqlQuery<string>("select CONTNRNO from VW_EXPORT_VEHICLE_TICKET_MOD_ASSGN where VTDID=" + VTDID).ToList();

                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_ExBond_Nos_VT " + VTDID).ToList();
                ViewBag.EBNDID = new SelectList(mtqry.OrderBy(x=>x.dtxt), "dval", "dtxt",tab.EBNDID).ToList();
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID > 1).Where(x => x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC",tab.CONTNRSID);
                var query1 = (from m in context.bondinfodtls
                              join e in context.exbondinfodtls on m.BNDID equals e.BNDID
                              join c in context.categorymasters on m.CHAID equals c.CATEID
                              join i in context.categorymasters on m.IMPRTID equals i.CATEID
                              where (e.EBNDID == tab.EBNDID && c.CATETID == 4 && i.CATETID == 1)
                              select new {  CHANAME=c.CATENAME, m.CHAID, m.BNDIGMNO, m.BNDNO, m.BNDID, e.EBNDID,e.EBNDDNO,m.PRDTDESC, m.IMPRTID, IMPRTRNAME = i.CATENAME  }
                            ).ToList();

                if (query1.Count > 0)
                {
                    ViewBag.PRDTDESC = query1[0].PRDTDESC.ToString();
                    ViewBag.BNDIGMNO = query1[0].BNDIGMNO.ToString();
                    ViewBag.BNDNO = query1[0].BNDNO.ToString();
                    ViewBag.CHANAME = query1[0].CHANAME.ToString();
                    ViewBag.IMPRTRNAME = query1[0].IMPRTRNAME.ToString();
                    ViewBag.EBNDDNO = query1[0].EBNDDNO.ToString();
                    
                }
                
                
            }
            return View(tab);
        }
        #endregion

        #region Autocomplete ExBond Details
        public JsonResult AutoExBondNo(string term)
        {


            var result = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_ExBond_Nos_VT @vtid=0, @term='" + term +"'").ToList();
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region savedata
        public void savedata(ExBondVehicleTicket tab)
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

                        string nop = Convert.ToString(tab.VTQTY);

                        if (nop == "" || nop == null)
                        { tab.VTQTY = 0; }
                        else { tab.VTQTY = Convert.ToDecimal(nop); }

                        string EBNDID = Convert.ToString(tab.EBNDID);

                        if (EBNDID == "" || EBNDID == null)
                        { tab.EBNDID = 0; }
                        else { tab.EBNDID = Convert.ToInt32(EBNDID); }

                        string CONTNRSID = Convert.ToString(tab.CONTNRSID);

                        if (CONTNRSID == "" || CONTNRSID == null)
                        { tab.CONTNRSID = 0; }
                        else { tab.CONTNRSID = Convert.ToInt32(CONTNRSID); }

                        string EBVTNOC = Convert.ToString(tab.EBVTNOC);

                        if (EBVTNOC == "" || EBVTNOC == null)
                        { tab.EBVTNOC = 0; }
                        else { tab.EBVTNOC = Convert.ToDecimal(EBVTNOC); }

                        tab.VTSTYPE = 0;
                        tab.VTTYPE = 0;

                        string indate = Convert.ToString(tab.VTDATE);
                        if (indate != null || indate != "")
                        {
                            tab.VTDATE = Convert.ToDateTime(indate).Date;
                        }
                        else { tab.VTDATE = DateTime.Now.Date; }

                        string intime = Convert.ToString(tab.VTTIME);
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

                                    tab.VTTIME = Convert.ToDateTime(in_datetime);
                                }
                                else { tab.VTTIME = DateTime.Now; }
                            }
                            else { tab.VTTIME = DateTime.Now; }
                        }
                        else { tab.VTTIME = DateTime.Now; }

                        
                        if ((tab.VTDID).ToString() != "0")
                        {
                            tab.LMUSRID = Session["CUSRID"].ToString();
                            context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                        else
                        {
                            tab.VTNO = Convert.ToInt32(Autonumber.autonum("BONDVEHICLETICKETDETAIL", "VTNO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=10 and VTSTYPE=0").ToString());
                            tab.CUSRID = Session["CUSRID"].ToString();
                            int ano = tab.VTNO;
                            string prfx = string.Format("{0:D5}", ano);
                            tab.VTDNO = prfx.ToString();
                            context.exbondvtdtls.Add(tab);
                            context.SaveChanges();
                        }
                        
                        trans.Commit(); Response.Redirect("Index");
                    }
                }
                catch
                {
                    //trans.Rollback();
                    Response.Redirect("/Error/AccessDenied");
                }
            }
        }
        #endregion


        #region DeleteVTInfo        
        //[Authorize(Roles = "ExBondVTDelete")]
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
                ExBondVehicleTicket exbondvt = context.exbondvtdtls.Find(Convert.ToInt32(id));
                context.exbondvtdtls.Remove(exbondvt);
                context.SaveChanges();

                Response.Write("Deleted Successfully ...");
            }
            else
            {                
                Response.Write("Record already exists, deletion is not possible!");                
            }

        }
        #endregion

        public void PrintView(int? id = 0)
        {
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "BondGateIn", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                CrystalDecisions.CrystalReports.Engine.Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "ExBond_VT.rpt");
                cryRpt.RecordSelectionFormula = "{VW_EXBOND_VT_PRINT_RPT.KUSRID} ='" + Session["CUSRID"].ToString() + "' and {VW_EXBOND_VT_PRINT_RPT.VTDID} =" + id;

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

    }
}