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
using scfs.Data;
using scfs_erp;

namespace scfs_erp.Controllers.ESeal
{
    [SessionExpire]
    public class ESealVehicleTicketController : Controller
    {
        // GET: ESealVehicleTicket

        #region contextdeclaration
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        #endregion

        #region Indexform
        [Authorize(Roles = "ESealVTIndex")]
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

        #region Getdata for index
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)/*model 22.edmx*/
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_ESeal_VehicleTicket(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                var aaData = data.Select(d => new string[] { d.VTDATE.Value.ToString("dd/MM/yyyy"), d.VTDNO.ToString(), d.CONTNRNO, d.CONTNRSDESC, d.CHANAME, d.EXPRTRNAME, d.TRNSPRTNAME, d.VHLNO, d.DRVNAME, d.DISPSTATUS, d.GIDId.ToString(), d.VTDID.ToString(), d.Edchk }).ToArray();
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

        #region Redirect  Form    
        [Authorize(Roles = "ESealVTEdit")]
        public void Edit(string id)
        {
            Response.Redirect("/ESealVehicleTicket/ESVTDForm/" + id);
        }
        #endregion

        #region E-Seal Vehicle Ticket Details Form

        [Authorize(Roles = "ESealVTCreate")]
        public ActionResult ESVTDForm(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            var VTDID = 0; var GODID = 0;
            if (id.Contains(';'))
            {
                var param = id.Split(';');
                VTDID = Convert.ToInt32(param[0]);
                GODID = Convert.ToInt32(param[1]);
            }
            else
            {
                VTDID = Convert.ToInt32(id);
            }
            VehicleTicketDetail tab = new VehicleTicketDetail();
            GateOutDetail tab1 = new GateOutDetail();
            VehicleTicketGateOut vg = new VehicleTicketGateOut();
            tab.EVSDATE = DateTime.Now.Date;
            tab.EVLDATE = DateTime.Now.Date;
            tab.ELRDATE = DateTime.Now.Date;
            tab.VTDATE = DateTime.Now.Date;
            tab.VTTIME = DateTime.Now;

            tab.VTDID = 0;

            ViewBag.GIDID = new SelectList(context.VW_ESeal_GateInDetail_Containerno.OrderBy(m => m.CONTNRNO), "GIDID", "CONTNRNO");

            //-----------------------------type-----------
            List<SelectListItem> selectedType = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "CCTL", Value = "0", Selected = false };
            selectedType.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "CITPL", Value = "1", Selected = true };
            selectedType.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "KATL", Value = "4", Selected = false };
            selectedType.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "AECTPL", Value = "5", Selected = false };
            selectedType.Add(selectedItem1);
            ViewBag.VTTYPE = selectedType;
                       
            ViewBag.GOOTYPE = new SelectList(context.Export_OperationTypeMaster.OrderBy(m => m.EOPTDESC), "EOPTID", "EOPTDESC");

            tab = context.vehicleticketdetail.Find(VTDID);

            if (VTDID != 0)//Edit Mode
            {
                tab = context.vehicleticketdetail.Find(VTDID);
                
                ViewBag.GIDID = new SelectList(context.VW_ESeal_GateInDetail_Containerno.Where(m => m.GIDID == tab.GIDID).OrderBy(m => m.CONTNRNO), "GIDID", "CONTNRNO", tab.GIDID);

                var query = context.Database.SqlQuery<GateInDetail>("SELECT *FROM GATEINDETAIL WHERE GIDID=" + tab.GIDID).ToList();
                ViewBag.CONTNRNO = query[0].CONTNRNO.ToString();

                //-----------------------------blocktype--------
                List<SelectListItem> selectedBLCK1 = new List<SelectListItem>();
                if (Convert.ToInt16(tab.VTTYPE) == 0)
                {
                    SelectListItem selectedItemBLCK = new SelectListItem { Text = "CCTL", Value = "0", Selected = true };
                    selectedBLCK1.Add(selectedItemBLCK);
                    selectedItemBLCK = new SelectListItem { Text = "CITPL", Value = "1", Selected = false };
                    selectedBLCK1.Add(selectedItemBLCK);
                    selectedItemBLCK = new SelectListItem { Text = "KATL", Value = "4", Selected = false };
                    selectedBLCK1.Add(selectedItemBLCK);
                    selectedItemBLCK = new SelectListItem { Text = "AECTPL", Value = "5", Selected = false };
                    selectedBLCK1.Add(selectedItemBLCK);
                    ViewBag.VTTYPE = selectedBLCK1;
                }
                else if (Convert.ToInt16(tab.VTTYPE) == 1)
                {
                    SelectListItem selectedItemBLCK = new SelectListItem { Text = "CCTL", Value = "0", Selected = false };
                    selectedBLCK1.Add(selectedItemBLCK);
                    selectedItemBLCK = new SelectListItem { Text = "CITPL", Value = "1", Selected = true };
                    selectedBLCK1.Add(selectedItemBLCK);
                    selectedItemBLCK = new SelectListItem { Text = "KATL", Value = "4", Selected = false };
                    selectedBLCK1.Add(selectedItemBLCK);
                    selectedItemBLCK = new SelectListItem { Text = "AECTPL", Value = "5", Selected = false };
                    selectedBLCK1.Add(selectedItemBLCK);
                    ViewBag.VTTYPE = selectedBLCK1;
                }
                else if (Convert.ToInt16(tab.VTTYPE) == 4)
                {
                    SelectListItem selectedItemBLCK = new SelectListItem { Text = "CCTL", Value = "0", Selected = false };
                    selectedBLCK1.Add(selectedItemBLCK);
                    selectedItemBLCK = new SelectListItem { Text = "CITPL", Value = "1", Selected = false };
                    selectedBLCK1.Add(selectedItemBLCK);
                    selectedItemBLCK = new SelectListItem { Text = "KATL", Value = "4", Selected = true };
                    selectedBLCK1.Add(selectedItemBLCK);
                    selectedItemBLCK = new SelectListItem { Text = "AECTPL", Value = "5", Selected = false };
                    selectedBLCK1.Add(selectedItemBLCK);
                    ViewBag.VTTYPE = selectedBLCK1;
                }
                else if (Convert.ToInt16(tab.VTTYPE) == 5)
                {
                    SelectListItem selectedItemBLCK = new SelectListItem { Text = "CCTL", Value = "0", Selected = false };
                    selectedBLCK1.Add(selectedItemBLCK);
                    selectedItemBLCK = new SelectListItem { Text = "CITPL", Value = "1", Selected = false };
                    selectedBLCK1.Add(selectedItemBLCK);
                    selectedItemBLCK = new SelectListItem { Text = "KATL", Value = "4", Selected = false };
                    selectedBLCK1.Add(selectedItemBLCK);
                    selectedItemBLCK = new SelectListItem { Text = "AECTPL", Value = "5", Selected = true };
                    selectedBLCK1.Add(selectedItemBLCK);
                    ViewBag.VTTYPE = selectedBLCK1;
                }

                //vg.godetail = context.gateoutdetail.Find(GODID);
                vg.vtdetail = context.vehicleticketdetail.Find(VTDID);
            }
            return View(tab);


        }
        #endregion

        #region Get Auto Container No
        public JsonResult GetAutoContainerDetails(string term)
        {
            var result = (from r in context.VW_ESeal_GateInDetail_Containerno
                          where r.CONTNRNO.Contains(term)
                          select new { r.CONTNRNO, r.GIDID }).OrderBy(x => x.CONTNRNO).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get ESEAL Gatein details
        public JsonResult GetEsealGateinDetails(int id = 0)
        {
            if (id > 0)
            {
                var result = context.Database.SqlQuery<pr_GET_ESEAL_GATEINDETAIL_FVT_Result>("EXEC pr_GET_ESEAL_GATEINDETAIL_FVT @GIDID=" + Convert.ToInt32(id)).ToList();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = "";

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Eseal Vehicle ticket Savedata 
        [HttpPost]
        public ActionResult SaveVehicleTicket(VehicleTicketDetail tab)
        {
            string status = "";
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    string todaydt = Convert.ToString(DateTime.Now);
                    string todayd = Convert.ToString(DateTime.Now.Date);

                    tab.LMUSRID = Session["CUSRID"].ToString();
                    //tab.vtdetail.LMUSRID = 1;
                    tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                    tab.SDPTID = 11;
                    tab.DISPSTATUS = 0;
                    tab.PRCSDATE = DateTime.Now;
                    tab.VTQTY = 0;
                    //tab.VTSSEALNO = "-";
                    tab.VTSTYPE = 0;
                    tab.VTCTYPE = 1;

                    string indate = Convert.ToString(tab.VTDATE);
                    if (indate != null || indate != "")
                    {
                        tab.VTDATE = Convert.ToDateTime(indate).Date;
                    }
                    else { tab.VTDATE = DateTime.Now.Date; }

                    if (tab.VTDATE > Convert.ToDateTime(todayd))
                    {
                        tab.VTDATE = Convert.ToDateTime(todayd);
                    }

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
                        else
                        {
                            var in_time = intime;
                            var in_date = indate;

                            if ((in_time.Contains(':')) && (in_date.Contains('/')))
                            {
                                var in_time1 = in_time.Split(':');
                                var in_date1 = in_date.Split('/');

                                string in_datetime = in_date1[2] + "-" + in_date1[1] + "-" + in_date1[0] + "  " + in_time1[0] + ":" + in_time1[1] + ":" + in_time1[2];

                                tab.VTTIME = Convert.ToDateTime(in_datetime);
                            }
                            else { tab.VTTIME = DateTime.Now; }
                        }
                    }
                    else { tab.VTTIME = DateTime.Now; }

                    if (tab.VTTIME > Convert.ToDateTime(todaydt))
                    {
                        tab.VTTIME = Convert.ToDateTime(todaydt);
                    }

                    //tab.GIDID = Convert.ToInt32(Request.Form.Get("GIDID"));
                    if ((tab.VTDID).ToString() != "0")
                    {
                        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();

                        status = "Success";
                    }
                    else
                    {

                        tab.VTNO = Convert.ToInt32(Autonumber.autonum("vehicleticketDetail", "VTNO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=11 and VTSTYPE=0").ToString());
                        tab.CUSRID = Session["CUSRID"].ToString();
                        int ano = tab.VTNO;
                        string prfx = string.Format("{0:D5}", ano);
                        tab.VTDNO = prfx.ToString();
                        context.vehicleticketdetail.Add(tab);
                        context.SaveChanges();

                        status = "Success";
                    }

                    trans.Commit();
                    return Json(status, JsonRequestBehavior.AllowGet);

                    //Response.Redirect("Index");
                }
            }
            catch (Exception ex)
            {
                status = ex.Message.ToString();
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Printview     
        [Authorize(Roles = "ESealVTPrint")]
        public void PrintView(int id)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "ESEALVEHICLETICKET", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "ESeal_VT.RPT");
                cryRpt.RecordSelectionFormula = "{VW_ESEAL_VT_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_ESEAL_VT_PRINT_ASSGN.VTDID} = " + id;

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
                GC.Collect();
                stringbuilder.Clear();
            }
        }
        #endregion

        #region Delete Record  
        [Authorize(Roles = "ESealVTDelete")]
        public void Del()
        {

            String id = Request.Form.Get("id");

            String GIDID = "0";
            int VTDID = 0;

            if (id.Contains(";"))
            {
                var ids = id.Split(';');
                GIDID = Convert.ToString(ids[0]);
                VTDID = Convert.ToInt32(ids[1]);
            }

            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, GIDID);
            if (temp.Equals("PROCEED"))
            {
                VehicleTicketDetail vehicleticketdetail = context.vehicleticketdetail.Find(Convert.ToInt32(VTDID));
                context.vehicleticketdetail.Remove(vehicleticketdetail);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else              
                Response.Write(temp);

        }
        #endregion
    }
}