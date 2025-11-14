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
using QRCoder;
using System.Drawing;
using System.Reflection;

namespace scfs_erp.Controllers.Import
{
    [SessionExpire]
    public class ImportVehicleTicketController : Controller
    {
        // GET: ImportVehicleTicket

        #region Context declaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region Index Page
        [Authorize(Roles = "ImportVehicleTicketIndex")]
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
            if (Request.Form.Get("TYPEID") != null)
            {
                Session["TYPEID"] = Request.Form.Get("TYPEID");
            }
            else if (Request.Form.Get("TYPEID") == "2")
            {
                Session["TYPEID"] = "2";
            }
            else
            {
                Session["TYPEID"] = "1";
            }
            List<SelectListItem> selectedType = new List<SelectListItem>();
            if (Convert.ToInt32(Session["TYPEID"]) == 1)
            {
                SelectListItem selectedItem1 = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
                selectedType.Add(selectedItem1);
                selectedItem1 = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
                selectedType.Add(selectedItem1);
                ViewBag.TYPEID = selectedType;
            }
            else
            {
                SelectListItem selectedItem1 = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                selectedType.Add(selectedItem1);
                selectedItem1 = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = true };
                selectedType.Add(selectedItem1);
                ViewBag.TYPEID = selectedType;
            }

            DateTime fromdate = Convert.ToDateTime(Session["SDATE"]).Date;
            DateTime todate = Convert.ToDateTime(Session["EDATE"]).Date;


            TotalContainerDetails(fromdate, todate);

            return View();
        }
        #endregion

        #region  TotalContainerDetails
        public JsonResult TotalContainerDetails(DateTime fromdate, DateTime todate)
        {
            string fdate = ""; string tdate = ""; int sdptid = 1;
            if (fromdate == null)
            {
                fromdate = DateTime.Now.Date;
                fdate = Convert.ToString(fromdate);
            }
            else
            {
                string infdate = Convert.ToString(fromdate);
                var in_date = infdate.Split(' ');
                var in_date1 = in_date[0].Split('/');
                fdate = Convert.ToString(in_date1[2] + "-" + in_date1[1] + "-" + in_date1[0]);
            }
            if (todate == null)
            {
                todate = DateTime.Now.Date;
                tdate = Convert.ToString(todate);
            }
            else
            {
                string intdate = Convert.ToString(todate);

                var in_date1 = intdate.Split(' ');
                var in_date2 = in_date1[0].Split('/');
                tdate = Convert.ToString(in_date2[2] + "-" + in_date2[1] + "-" + in_date2[0]);

            }


            var result = context.Database.SqlQuery<PR_IMPORT_LOADESTUFFTOTCONTAINER_DETAILS_Result>("EXEC PR_IMPORT_LOADESTUFFTOTCONTAINER_DETAILS @PFDT='" + fdate + "',@PTDT='" + tdate + "',@PSDPTID=" + 1).ToList();

            foreach (var rslt in result)
            {
                if ((rslt.Sno == 3) && (rslt.Descriptn == "IMPORT - VEHICLETICKET"))
                {
                    @ViewBag.Total20 = rslt.c_20;
                    @ViewBag.Total40 = rslt.c_40;
                    @ViewBag.Total45 = rslt.c_45;
                    @ViewBag.TotalTues = rslt.c_tues;

                    Session["IVT20"] = rslt.c_20;
                    Session["IVT40"] = rslt.c_40;
                    Session["IVT45"] = rslt.c_45;
                    Session["IVTTU"] = rslt.c_tues;
                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region GetAjaxData
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Import_VehicleTicket(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(Session["TYPEID"]));

                var aaData = data.Select(d => new string[] { Convert.ToDateTime(d.VTDATE).ToString("dd/MM/yyyy"), d.VTDNO.ToString(), d.CONTNRNO, d.IGMNO, d.GPLNO, d.CONTNRSDESC, d.ASLMDNO, d.VTTYPE.ToString(), d.CHANAME.ToString(), d.BOENO.ToString(), d.VTDID.ToString(), d.GIDId.ToString(), d.GODID.ToString(),  d.EGIDID.ToString(), d.GOSTS, d.VTSTYPE.ToString() }).ToArray();
                //var aaData = data.Select(d => new string[] { d.VTDATE.Value.ToString("dd/MM/yyyy"), d.VTDNO.ToString(), d.CONTNRNO, d.CONTNRSDESC, d.ASLMDNO, d.VTDESC, d.VHLNO, d.VTQTY.ToString(), d.VTDID.ToString(), d.GIDId.ToString(), d.VTSSEALNO, d.GODID.ToString(), d.EGIDID.ToString() }).ToArray();
                //var aaData = data.Select(d => new string[] { d.VTDATE.Value.ToString("dd/MM/yyyy"), d.VTDNO.ToString(), d.CONTNRNO, d.CONTNRSDESC, d.ASLMDNO, d.VTDESC, d.VHLNO, d.VTQTY.ToString(), d.VTDID.ToString(), d.GIDId.ToString(), d.VTSSEALNO, d.GODID.ToString(), d.EGIDID.ToString() }).ToArray();
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

        #region Edit Page
        [Authorize(Roles = "ImportVehicleTicketEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ImportVehicleTicket/NForm/" + id);
        }
        #endregion

        #region NFORM 
        public ActionResult NForm(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            VehicleTicketDetail tab = new VehicleTicketDetail();
            tab.EVSDATE = DateTime.Now.Date;
            tab.EVLDATE = DateTime.Now.Date;
            tab.ELRDATE = DateTime.Now.Date;
            tab.VTDATE = DateTime.Now.Date;
            tab.VTTIME = DateTime.Now;
            tab.VTDID = 0;

            //vg.vtdetail.VTDID = 0; vg.godetail.GODID = 0;
            //ViewBag.ASLDID = new SelectList(context.Database.SqlQuery<VW_VEHICLETICKET_CONTAINER_CBX_ASSGN>("select * from VW_VEHICLETICKET_CONTAINER_CBX_ASSGN").ToList(), "ASLDID", "CONTNRNO");

            ViewBag.ASLDID = new SelectList("");

            //-----------------------------type-----------
            List<SelectListItem> selectedType = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
            selectedType.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
            selectedType.Add(selectedItem1);
            ViewBag.VTTYPE = selectedType;

            ViewBag.GOOTYPE = new SelectList(context.export_Gateout_Gootypes, "GOOTYPE", "GOOTYPEDESC");

            //-----------------------------by type-----------
            List<SelectListItem> selectedType_ = new List<SelectListItem>();
            SelectListItem selectedItemtt = new SelectListItem { Text = "PART", Value = "0", Selected = false };
            selectedType_.Add(selectedItemtt);
            selectedItemtt = new SelectListItem { Text = "FULL", Value = "1", Selected = true };
            selectedType_.Add(selectedItemtt);
            ViewBag.VTSTYPE = selectedType_;
            //end


            //-----------------------------CHA type-----------
            List<SelectListItem> selectedCHA = new List<SelectListItem>();
            SelectListItem selectedItemCHA = new SelectListItem { Text = "CARGO OUT", Value = "0", Selected = true };
            selectedCHA.Add(selectedItemCHA);
            selectedItemCHA = new SelectListItem { Text = "CARGO IN", Value = "1", Selected = false };
            selectedCHA.Add(selectedItemCHA);
            ViewBag.VTCTYPE = selectedCHA;
            //end

            if (id != 0)//Edit Mode
            {
                tab = context.vehicleticketdetail.Find(id);

                //-----------------------------CHA type-----------
                List<SelectListItem> selectedCHA_MOD = new List<SelectListItem>();
                if (Convert.ToInt32(tab.VTCTYPE) == 1)
                {
                    SelectListItem selectedItemCHA_MOD = new SelectListItem { Text = "CARGO OUT", Value = "0", Selected = false };
                    selectedCHA_MOD.Add(selectedItemCHA_MOD);
                    selectedItemCHA_MOD = new SelectListItem { Text = "CARGO IN", Value = "1", Selected = true };
                    selectedCHA_MOD.Add(selectedItemCHA_MOD);
                    ViewBag.VTCTYPE = selectedCHA_MOD;
                }
                else if (Convert.ToInt32(tab.VTCTYPE) == 0)
                {
                    SelectListItem selectedItemCHA_MOD = new SelectListItem { Text = "CARGO OUT", Value = "0", Selected = true };
                    selectedCHA_MOD.Add(selectedItemCHA_MOD);
                    //selectedItemCHA_MOD = new SelectListItem { Text = "CARGO IN", Value = "1", Selected = true };
                    //selectedCHA_MOD.Add(selectedItemCHA_MOD);
                    ViewBag.VTCTYPE = selectedCHA_MOD;
                }
                //-----------------------------by type-----------
                List<SelectListItem> selectedType_MOD = new List<SelectListItem>();
                if (Convert.ToInt32(tab.VTSTYPE) == 0)
                {
                    SelectListItem selectedItemtt_MOD = new SelectListItem { Text = "PART", Value = "0", Selected = true };
                    selectedType_MOD.Add(selectedItemtt_MOD);
                    selectedItemtt_MOD = new SelectListItem { Text = "FULL", Value = "1", Selected = false };
                    selectedType_MOD.Add(selectedItemtt_MOD);
                    ViewBag.VTSTYPE = selectedType_MOD;
                }
                else if (Convert.ToInt32(tab.VTSTYPE) == 1)
                {
                    SelectListItem selectedItemtt_MOD = new SelectListItem { Text = "FULL", Value = "1", Selected = true };
                    selectedType_MOD.Add(selectedItemtt_MOD);
                    //selectedItemtt_MOD = new SelectListItem { Text = "FULL", Value = "1", Selected = false };
                    //selectedType_MOD.Add(selectedItemtt_MOD);
                    ViewBag.VTSTYPE = selectedType_MOD;
                }
                //-----------Getting Gate_In Details-----------------//

                var query = context.Database.SqlQuery<string>("select CONTNRNO from GATEINDETAIL where GIDID=" + tab.GIDID).ToList();
                ViewBag.CONTNRNO = query[0].ToString();

            }
            return View(tab);

        }
        #endregion

        #region Savedata
        public void savedata(VehicleTicketDetail tab)
        {
            tab.CUSRID = Session["CUSRID"].ToString();
            tab.LMUSRID = Session["CUSRID"].ToString(); ;
            tab.COMPYID = Convert.ToInt32(Session["compyid"]);
            tab.SDPTID = 1;
            tab.DISPSTATUS = 0;
            tab.PRCSDATE = DateTime.Now;
            //tab.VTQTY = 0;
            //tab.VTSSEALNO = "-";
            //tab.VTSTYPE = 0;
            //tab.VTDATE = tab.VTTIME.Date;
            tab.EVLDATE = null; tab.ELRDATE = null; tab.EVSDATE = null;
            tab.STFDID = 0;

            string todaydt = Convert.ToString(DateTime.Now);
            string todayd = Convert.ToString(DateTime.Now.Date);

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
                else { tab.VTTIME = DateTime.Now; }
            }
            else { tab.VTTIME = DateTime.Now; }

            if (tab.VTTIME > Convert.ToDateTime(todaydt))
            {
                tab.VTTIME = Convert.ToDateTime(todaydt);
            }

            if ((tab.VTDID).ToString() != "0")
            {
                // Load original record for logging (no tracking to avoid state conflicts)
                var original = context.vehicleticketdetail.AsNoTracking().FirstOrDefault(x => x.VTDID == tab.VTDID);

                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                // Best-effort logging to SCFS_LOG
                try
                {
                    System.Diagnostics.Debug.WriteLine($"IMPORT VEHICLE TICKET SAVE METHOD CALLED: VTDID={tab.VTDID}, VTDNO={tab.VTDNO}");
                    if (original != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"ORIGINAL RECORD FOUND: VTDID={original.VTDID}, calling LogVehicleTicketEdits");
                        // Ensure baseline snapshot (Version = "0") exists for this record before logging diffs
                        EnsureBaselineVersionZero(original, Session["CUSRID"] != null ? Session["CUSRID"].ToString() : "");
                        LogVehicleTicketEdits(original, tab, Session["CUSRID"] != null ? Session["CUSRID"].ToString() : "");
                        System.Diagnostics.Debug.WriteLine($"LogVehicleTicketEdits completed successfully");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"ORIGINAL RECORD NOT FOUND for VTDID={tab.VTDID}");
                    }
                }
                catch (Exception ex)
                {
                    // Log the error for debugging
                    System.Diagnostics.Debug.WriteLine($"Import Vehicle Ticket edit logging failed: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                }

                if (tab.VTCTYPE == 1 && tab.VTSTYPE == 1)
                {
                    if (tab.CGIDID > 0 || tab.CGIDID != null)
                    {
                        var CGIDID = tab.CGIDID;

                        string uqry = "Update GATEINDETAIL SET GPWGHT = " + Convert.ToDecimal(tab.VTQTY) + " Where  GIDID = " + Convert.ToInt32(CGIDID) + " ";

                        context.Database.ExecuteSqlCommand(uqry);
                    }
                }

            }
            else
            {
                tab.VTNO = Convert.ToInt32(Autonumber.autonum("vehicleticketDetail", "VTNO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=1").ToString());
                int ano = tab.VTNO;
                string prfx = string.Format("{0:D5}", ano);
                tab.VTDNO = prfx.ToString();
                context.vehicleticketdetail.Add(tab);
                context.SaveChanges();

                var VTDID = tab.VTDID;



                /*......GATE IN INSERT....*/
                if (tab.VTTYPE == 2 && tab.VTSTYPE == 1)
                {


                    GateInDetail gatein = new GateInDetail();
                    gatein.COMPYID = Convert.ToInt32(Session["compyid"]);
                    gatein.SDPTID = 3;
                    gatein.GITIME = tab.VTTIME;
                    gatein.GIDATE = tab.VTTIME.Date;
                    gatein.GICCTLTIME = Convert.ToDateTime(tab.VTTIME);
                    gatein.GICCTLDATE = Convert.ToDateTime(tab.VTTIME).Date;
                    gatein.GINO = Convert.ToInt32(Autonumber.cargoautonum("gateindetail", "GINO", "3", Convert.ToString(gatein.COMPYID)).ToString());
                    //gatein.GINO = Convert.ToInt32(Autonumber.autonum("gateindetail", "GINO", "GINO<>0 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                    int anoo = gatein.GINO;
                    string prfxx = string.Format("{0:D5}", anoo);
                    gatein.GIDNO = prfxx.ToString();
                    gatein.GIVHLTYPE = 0; //.actual gatein givhltype
                    gatein.TRNSPRTID = 0;
                    gatein.TRNSPRTNAME = "-";
                    gatein.AVHLNO = tab.VHLNO; // "-"; 
                    gatein.VHLNO = tab.VHLNO; // "-";
                    gatein.DRVNAME = tab.DRVNAME; // "-";
                    gatein.GPREFNO = "-";
                    gatein.IMPRTID = 0;
                    gatein.IMPRTNAME = "-";
                    gatein.STMRID = Convert.ToInt32(Request.Form.Get("TMPSTMRID"));//stmrid from gatein
                    gatein.STMRNAME = Convert.ToString(Request.Form.Get("STMRNAME"));//stmrname from gatein
                    gatein.CHAID = Convert.ToInt32(Request.Form.Get("TMPCHAID"));//chaid from gatein
                    gatein.CHANAME = Convert.ToString(Request.Form.Get("CHANAME"));//chaname from gatein
                    gatein.CONDTNID = 0;
                    gatein.CONTNRNO = Convert.ToString(Request.Form.Get("TMPCONTNRNO"));
                    gatein.CONTNRTID = Convert.ToInt32(Request.Form.Get("TMPCONTNRTID"));
                    gatein.CONTNRID = 0;
                    gatein.CONTNRSID = Convert.ToInt32(Request.Form.Get("TMPCONTNRSID"));
                    gatein.LPSEALNO = "-";
                    gatein.CSEALNO = "-";
                    gatein.YRDID = 0;
                    gatein.VSLID = 0;
                    gatein.VSLNAME = "-";
                    gatein.VOYNO = "-";
                    gatein.PRDTGID = 0;
                    gatein.PRDTDESC = "-";
                    gatein.UNITID = 0;
                    gatein.GPLNO = "-";
                    gatein.GPWGHT = 0;
                    gatein.GPEAMT = 0;
                    gatein.GPAAMT = 0;
                    gatein.IGMNO = "-";
                    gatein.GIISOCODE = "-";
                    gatein.GIDMGDESC = "-";
                    gatein.GPWTYPE = 0;
                    gatein.GPSTYPE = 0;
                    gatein.GPETYPE = 0;
                    gatein.SLOTID = 0;
                    tab.CUSRID = tab.CUSRID;
                    tab.LMUSRID = tab.LMUSRID;
                    tab.DISPSTATUS = 0;
                    tab.PRCSDATE = tab.PRCSDATE;
                    gatein.CUSRID = tab.CUSRID;
                    gatein.LMUSRID = tab.LMUSRID;
                    gatein.DISPSTATUS = 0;
                    gatein.PRCSDATE = DateTime.Now;
                    context.gateindetails.Add(gatein);
                    context.SaveChanges();

                    var EGIDID = gatein.GIDID;

                    string uqry = "Update VEHICLETICKETDETAIL SET EGIDID = " + Convert.ToInt32(EGIDID) + " Where  VTDID = " + Convert.ToInt32(VTDID) + " ";

                    context.Database.ExecuteSqlCommand(uqry);

                    //tab = context.vehicleticketdetail.Find(VTDID);
                    //context.Entry(tab).Entity.EGIDID = gatein.GIDID;
                    //context.SaveChanges();
                }

                /*......CARGO IN INSERT....*/
                if (tab.VTCTYPE == 1 && tab.VTSTYPE == 1)
                {
                    GateInDetail gatein = new GateInDetail();
                    GateInDetail tgatein = new GateInDetail();
                    tab = context.vehicleticketdetail.Find(VTDID);
                    tgatein = context.gateindetails.Find(tab.GIDID);
                    gatein.COMPYID = Convert.ToInt32(Session["compyid"]);
                    gatein.SDPTID = 4;
                    gatein.GITIME = tab.VTTIME;
                    gatein.GIDATE = tab.VTTIME.Date;
                    gatein.GICCTLTIME = Convert.ToDateTime(tab.VTTIME);
                    gatein.GICCTLDATE = Convert.ToDateTime(tab.VTTIME).Date;


                    gatein.GINO = Convert.ToInt32(Autonumber.cargoautonum("gateindetail", "GINO", "4", Convert.ToString(gatein.COMPYID)).ToString());

                    //gatein.GINO = Convert.ToInt32(Autonumber.autonum("gateindetail", "GINO", "GINO<>0 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                    int anoo = gatein.GINO;
                    string prfxx = string.Format("{0:D5}", anoo);
                    gatein.GIDNO = prfxx.ToString();
                    gatein.GIVHLTYPE = tgatein.GIVHLTYPE;//.actual gatein givhltype
                    gatein.TRNSPRTID = tgatein.TRNSPRTID;
                    gatein.TRNSPRTNAME = tgatein.TRNSPRTNAME;
                    gatein.AVHLNO = "-";
                    gatein.VHLNO = "-";
                    gatein.DRVNAME = "-";
                    gatein.GPREFNO = Convert.ToString(tgatein.GPREFNO);
                    gatein.IMPRTID = Convert.ToInt32(tgatein.IMPRTID);
                    gatein.IMPRTNAME = Convert.ToString(tgatein.IMPRTNAME); ;
                    gatein.STMRID = Convert.ToInt32(tgatein.STMRID);//stmrid from gatein
                    gatein.STMRNAME = Convert.ToString(tgatein.STMRNAME);//stmrname from gatein
                    gatein.CHAID = Convert.ToInt32(Request.Form.Get("TMPCHAID"));//stmrid from gatein
                    gatein.CHANAME = Convert.ToString(Request.Form.Get("CHANAME"));//stmrname from gatein
                    gatein.CONDTNID = 0;
                    gatein.CONTNRNO = Convert.ToString(tgatein.CONTNRNO);
                    gatein.CONTNRTID = Convert.ToInt32(tgatein.CONTNRTID);
                    gatein.CONTNRID = 0;
                    gatein.CONTNRSID = Convert.ToInt32(tgatein.CONTNRSID);
                    gatein.LPSEALNO = "-";
                    gatein.CSEALNO = "-";
                    gatein.YRDID = 0;
                    gatein.VSLID = 0;
                    gatein.VSLNAME = "-";
                    gatein.VOYNO = "-";
                    gatein.PRDTGID = Convert.ToInt32(tgatein.PRDTGID);
                    gatein.PRDTDESC = Convert.ToString(tgatein.PRDTDESC);
                    gatein.UNITID = Convert.ToInt32(tgatein.UNITID);
                    gatein.GPLNO = Convert.ToString(tgatein.GPLNO);
                    gatein.GPWGHT = Convert.ToDecimal(tab.VTQTY);
                    gatein.GPEAMT = 0;
                    gatein.GPAAMT = 0;
                    gatein.IGMNO = Convert.ToString(tgatein.IGMNO);
                    gatein.GIISOCODE = "-";
                    gatein.GIDMGDESC = "-";
                    gatein.GPWTYPE = 0;
                    gatein.GPSTYPE = 0;
                    gatein.GPETYPE = 0;
                    gatein.SLOTID = 0;
                    tab.CUSRID = tab.CUSRID;
                    tab.LMUSRID = tab.LMUSRID;
                    tab.DISPSTATUS = 0;
                    tab.PRCSDATE = tab.PRCSDATE;
                    gatein.CUSRID = tab.CUSRID;
                    gatein.LMUSRID = tab.LMUSRID;
                    gatein.DISPSTATUS = 0;
                    gatein.PRCSDATE = DateTime.Now;

                    context.gateindetails.Add(gatein);
                    context.SaveChanges();

                    var CGIDID = gatein.GIDID;

                    string uqry = "Update VEHICLETICKETDETAIL SET CGIDID = " + Convert.ToInt32(CGIDID) + " Where  VTDID = " + Convert.ToInt32(VTDID) + " ";

                    context.Database.ExecuteSqlCommand(uqry);


                    //context.Entry(tab).Entity.CGIDID = Convert.ToInt32(GIDID);
                    //context.SaveChanges();
                }
            }

            Response.Redirect("Index");
        }
        #endregion

        #region GetContDetails
        public JsonResult GetContDetails(int id)
        {
            var data = context.Database.SqlQuery<VW_VEHICLETICKET_IMPORT_CONTAINER_CTRL_ASSGN>("SELECT * FROM VW_VEHICLETICKET_IMPORT_CONTAINER_CTRL_ASSGN (nolock) WHERE ASLDID=" + id + "").ToList();
            if (data[0].OOCDATE != null && data[0].OOCDATE != "")
                ViewBag.OOCDATE = Convert.ToDateTime(data[0].OOCDATE).ToString("dd/MM/yyyy");
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SlipMaxDate
        public ActionResult SlipMaxDate(int id)
        {

            var data = (from q in context.authorizatioslipmaster
                        join b in context.authorizationslipdetail on q.ASLMID equals b.ASLMID
                        where b.ASLDID == id && q.SDPTID == 1
                        group q by q.ASLMDATE into g
                        select new { ASLMDATE = g.Max(t => t.ASLMDATE) }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Getcont
        public JsonResult Getcont(string id)
        {
            var param = id.Split(';');
            var qry = "SELECT *FROM VW_VEHICLETICKET_IMPORT_CONTAINER_CBX_ASSGN WHERE IGMNO='" + param[0] + "' and GPLNO='" + param[1] + "'";
            var data = context.Database.SqlQuery<VW_VEHICLETICKET_IMPORT_CONTAINER_CBX_ASSGN>(qry).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetCha
        public JsonResult GetCha(int id)
        {
            var sql = (from r in context.gateindetails.Where(x => x.GIDID == id)
                       join s in context.opensheetdetails on r.GIDID equals s.GIDID
                       join om in context.opensheetmasters on s.OSMID equals om.OSMID
                       join bd in context.billentrydetails on s.BILLEDID equals bd.BILLEDID
                       join m in context.billentrymasters on bd.BILLEMID equals m.BILLEMID
                       select new { m.CHAID, m.BILLEMNAME }
                              ).ToList();

            return Json(sql, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region delete 
        [Authorize(Roles = "ImportVehicleTicketDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            var param = id.Split('-'); var vtid = 0; var gidid = 0;
            if (param[0] != "0" || param[0] != "" || param[0] != null)
                vtid = Convert.ToInt32(param[0]);
            if (param[3] != "0" || param[3] != "" || param[3] != null)
                gidid = Convert.ToInt32(param[3]);
            //if (param[3] != "0")
            //    gidid = Convert.ToInt32(param[3]);//empty gidid
            String temp = Delete_fun.delete_check1(fld, param[2]);
            if (temp.Equals("PROCEED"))
            {
                VehicleTicketDetail vehicleticketdetail = context.vehicleticketdetail.Find(vtid);
                context.vehicleticketdetail.Remove(vehicleticketdetail);
                if (gidid != 0)
                {
                    GateInDetail gateindetails = context.gateindetails.Find(gidid);
                    if (gateindetails != null)
                    context.gateindetails.Remove(gateindetails);
                }
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion

        #region PrintView
        [Authorize(Roles = "ImportVehicleTicketPrint")]
        public void PrintView(int? id = 0)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "Imp_vehicle_ticket", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Import_VT.RPT");
                cryRpt.RecordSelectionFormula = "{VW_VT_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_VT_CRY_PRINT_ASSGN.VTDID} = " + id;

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

        //[Authorize(Roles = "ImportVehicleTicketPrint")]
        public void QRCPrintView(int? id = 0)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
            //GenerateQRCodeTxtFile(Convert.ToInt32(id));

            GenerateQRCodeFile(id);

            string QRpath = Server.MapPath("~/VTQRCode/");
            //string QRfilename = ConfigurationManager.AppSettings["QRCodeFileName"];
            string VTQRpath = QRpath + id.ToString() + ".png";


            if (System.IO.File.Exists(VTQRpath))
            {
                //  ........delete TMPRPT...//
                context.Database.ExecuteSqlCommand("Update VEHICLETICKETDETAIL Set QRCDIMGPATH = '" + VTQRpath + "' WHERE VTDID =" + id);

                context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
                var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "QRCODEVTPRINT", Convert.ToInt32(id), Session["CUSRID"].ToString());
                if (TMPRPT_IDS == "Successfully Added")
                {
                    ReportDocument cryRpt = new ReportDocument();
                    TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                    TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                    ConnectionInfo crConnectionInfo = new ConnectionInfo();
                    Tables CrTables;

                    cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Import_VT_QRCode.RPT");
                    cryRpt.RecordSelectionFormula = "{VW_VT_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_VT_CRY_PRINT_ASSGN.VTDID} = " + id;

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
                Response.Write("success");
            }
            else
            {
                Response.Write("Error in QR Code Generation...");
            }


        }

        public void GenerateQRCodeTxtFile(int? id = 0)
        {
            string QRpath = Server.MapPath("~/VTQRCode/");
            string QRfilename = QRpath  + id.ToString() + ".txt";
            // FileInfo info = new FileInfo(QRfilename);
            VehicleTicketDetail vtobj = new VehicleTicketDetail();
            vtobj = context.vehicleticketdetail.Find(id);
            vtobj.QRCDIMGPATH = QRfilename.Replace(".txt",".jpg");
            context.Entry(vtobj).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            if (System.IO.File.Exists(QRfilename))
                System.IO.File.Delete(QRfilename);

            //fs = System.IO.File.Open(QRfilename, System.IO.FileMode.CreateNew, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);

            var result = context.Database.SqlQuery<pr_Get_VT_Details_For_QRCode_Result>("exec pr_Get_VT_Details_For_QRCode @PVTDID=" + id).ToList();//........procedure  for edit mode details data
            foreach (var rslt in result)
            {
                var TmpContnrNo = rslt.CONTNRNO;
                var TmpSize = rslt.CONTNRSDESC;
                var TmpInDate = rslt.VTDATE;
                var TmpImprtName = rslt.IMPRTNAME;
                var TmpStmrName = rslt.STMRNAME;
                var TmpVhlNo = rslt.VHLNO;
                var TmpVslName = rslt.VSLNAME;
                var TmpVoyNo = rslt.VOYNO;
                var TmpPrdtDesc = rslt.PRDTDESC;
                var TmpWght = rslt.GPWGHT;
                var TmpIGMNo = rslt.IGMNO;
                var TmpLNo = rslt.GPLNO;
                var TmpLPsealNo = rslt.LPSEALNO;
                var TmpBLNo = rslt.OSMBLNO;
                string QRContent = "|" + TmpContnrNo + "|" + TmpSize + "|" + TmpInDate + "|" + TmpImprtName + "|" + TmpStmrName + "|" + TmpVhlNo + "|" + TmpVslName + "|" + TmpVslName + "|" + TmpVoyNo + "|" + TmpPrdtDesc + "|" + TmpWght + "|" + TmpIGMNo + "|" + TmpLNo + "|" + TmpLPsealNo + "|" + TmpBLNo + "|";
                try
                {
                    System.IO.File.WriteAllText(QRfilename, QRContent);
                    //System.IO.File.Copy(QRfilename, "z:\\www\\scfs\\barcode" + QRfilename.Replace(id.ToString() + ".txt", "QrCode.txt"));
                    
                    string qrc_url = ConfigurationManager.AppSettings["QRCodeURL"] + "?id=" + id.ToString();
                    //var getRequest = (HttpWebRequest)WebRequest.Create(qrc_url);
                    //var getResponse = (HttpWebResponse)getRequest.GetResponse();
                    //Response.Redirect(qrc_url);

                    //Response.Write("<script>");
                    //Response.Write("window.open('" + qrc_url + "','_blank')");
                    //Response.Write("</script>");
                    Response.Write(qrc_url);
                }
                catch (Exception e)
                {
                    Response.Write(e.Message);
                }
                
            }

        }
        #endregion

        #region DCheck
        public void DOCheck()//....delv,order check
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");

            String temp = Delete_fun.delete_check1(fld, id);
            //if (temp.Equals("PROCEED"))
            //{


            //    Response.Write("Deleted Successfully ...");
            //}
            //else
            Response.Write(temp);
        }//End of Delete
        #endregion

        #region QrCode Gneration
        public void GenerateQRCodeFile(int? id = 0)
        {
            //string QRpath = Server.MapPath("~/VTQRCode/");
            //string QRfilename = QRpath + id.ToString() + ".txt";
            //// FileInfo info = new FileInfo(QRfilename);
            //VehicleTicketDetail vtobj = new VehicleTicketDetail();
            //vtobj = context.vehicleticketdetail.Find(id);
            //vtobj.QRCDIMGPATH = QRfilename.Replace(".txt", ".jpg");
            //context.Entry(vtobj).State = System.Data.Entity.EntityState.Modified;
            //context.SaveChanges();

            //if (System.IO.File.Exists(QRfilename))
            //    System.IO.File.Delete(QRfilename);

            string barcodePath = Server.MapPath("~/VTQRCode/" + id.ToString() + ".png");
            var result = context.Database.SqlQuery<pr_Get_VT_Details_For_QRCode_Result>("exec pr_Get_VT_Details_For_QRCode @PVTDID=" + id).ToList();//........procedure  for edit mode details data
            foreach (var rslt in result)
            {
                var TmpContnrNo = rslt.CONTNRNO;
                var TmpSize = rslt.CONTNRSDESC;
                var TmpInDate = rslt.VTDATE;
                var TmpImprtName = rslt.IMPRTNAME;
                var TmpStmrName = rslt.STMRNAME;
                var TmpVhlNo = rslt.VHLNO;
                var TmpVslName = rslt.VSLNAME;
                var TmpVoyNo = rslt.VOYNO;
                var TmpPrdtDesc = rslt.PRDTDESC;
                var TmpWght = rslt.GPWGHT;
                var TmpIGMNo = rslt.IGMNO;
                var TmpLNo = rslt.GPLNO;
                var TmpLPsealNo = rslt.LPSEALNO;
                var TmpBLNo = rslt.OSMBLNO;
                string QRContent = TmpContnrNo + "|" + TmpSize + "|" + TmpInDate + "|" + TmpImprtName + "|" + TmpStmrName + "|" + TmpVhlNo + "|" + TmpVslName + "|" + TmpVslName + "|" + TmpVoyNo + "|" + TmpPrdtDesc + "|" + TmpWght + "|" + TmpIGMNo + "|" + TmpLNo + "|" + TmpLPsealNo + "|" + TmpBLNo + "|";
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(QRContent, QRCodeGenerator.ECCLevel.Q);


                        using (Bitmap bitMap = qrCode.GetGraphic(20))
                        {
                            //using (MemoryStream ms = new MemoryStream())
                            //{
                            bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] byteImage = ms.ToArray();
                            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                            img.Save(barcodePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                            //imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                            //}

                            //bitMap.Save(ms, ImageFormat.Png);
                            //ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    //Response.Write(qrc_url);
                }
                catch (Exception e)
                {
                    Response.Write(e.Message);
                }

            }

        }
        #endregion

        #region Edit Log Functionality

        // Display edit log for Vehicle Ticket
        public ActionResult EditLogVehicleTicket(int? vtdid)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
            var list = new List<scfs_erp.Models.GateInDetailEditLogRow>();

            if (cs != null && !string.IsNullOrWhiteSpace(cs.ConnectionString))
            {
                using (var sql = new SqlConnection(cs.ConnectionString))
                {
                    sql.Open();
                    string query = @"SELECT [GIDNO],[FieldName],[OldValue],[NewValue],[ChangedBy],[ChangedOn],[Version],[Modules]
                                    FROM [dbo].[GateInDetailEditLog]
                                    WHERE [Modules] = 'ImportVehicleTicket'";
                    
                    if (vtdid.HasValue)
                    {
                        // Find VTDNO from VTDID
                        var vehicleTicket = context.vehicleticketdetail.AsNoTracking().FirstOrDefault(x => x.VTDID == vtdid.Value);
                        if (vehicleTicket != null && !string.IsNullOrEmpty(vehicleTicket.VTDNO))
                        {
                            query += " AND [GIDNO] = @VTDNO";
                        }
                        else
                        {
                            query += " AND CAST([GIDNO] AS INT) = @VTDID";
                        }
                    }

                    query += " ORDER BY [ChangedOn] DESC, [Version] DESC, [FieldName]";

                    using (var cmd = new SqlCommand(query, sql))
                    {
                        if (vtdid.HasValue)
                        {
                            var vehicleTicket = context.vehicleticketdetail.AsNoTracking().FirstOrDefault(x => x.VTDID == vtdid.Value);
                            if (vehicleTicket != null && !string.IsNullOrEmpty(vehicleTicket.VTDNO))
                            {
                                cmd.Parameters.AddWithValue("@VTDNO", vehicleTicket.VTDNO);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@VTDID", vtdid.Value);
                            }
                        }

                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                list.Add(new scfs_erp.Models.GateInDetailEditLogRow
                                {
                                    GIDNO = Convert.ToString(r["GIDNO"]),
                                    FieldName = Convert.ToString(r["FieldName"]),
                                    OldValue = r["OldValue"] == DBNull.Value ? null : Convert.ToString(r["OldValue"]),
                                    NewValue = r["NewValue"] == DBNull.Value ? null : Convert.ToString(r["NewValue"]),
                                    ChangedBy = Convert.ToString(r["ChangedBy"]),
                                    ChangedOn = r["ChangedOn"] != DBNull.Value ? Convert.ToDateTime(r["ChangedOn"]) : DateTime.MinValue,
                                    Version = Convert.ToString(r["Version"]),
                                    Modules = r["Modules"] == DBNull.Value ? null : Convert.ToString(r["Modules"])
                                });
                            }
                        }
                    }
                }
            }

            // Map technical field names to friendly form labels and raw codes to display values
            try
            {
                // Build lookup dictionaries - handle duplicates by taking first
                var dictTransporter = context.categorymasters.Where(c => c.CATETID == 5 && c.DISPSTATUS == 0)
                    .GroupBy(x => x.CATEID)
                    .ToDictionary(g => g.Key, g => g.First().CATENAME);
                var dictVehicle = context.vehiclemasters.Where(v => v.DISPSTATUS == 0)
                    .GroupBy(x => x.VHLMID)
                    .ToDictionary(g => g.Key, g => g.First().VHLMDESC);

                string Map(string field, string raw)
                {
                    if (string.IsNullOrWhiteSpace(raw)) return raw;
                    int ival;
                    switch (field?.ToUpperInvariant())
                    {
                        case "TRNSPRTID":
                            return int.TryParse(raw, out ival) && dictTransporter.ContainsKey(ival) ? dictTransporter[ival] : raw;
                        case "VHLMID":
                            return int.TryParse(raw, out ival) && dictVehicle.ContainsKey(ival) ? dictVehicle[ival] : raw;
                        case "VTTYPE":
                            if (raw == "1") return "Empty";
                            if (raw == "2") return "Loaded";
                            return raw;
                        case "VTSTYPE":
                            if (raw == "1") return "In";
                            if (raw == "2") return "Out";
                            return raw;
                        case "VTCTYPE":
                            if (raw == "1") return "Cargo";
                            if (raw == "2") return "Empty";
                            return raw;
                        case "DISPSTATUS":
                            return raw == "1" ? "Disabled" : raw == "0" ? "Enabled" : raw;
                        default:
                            return raw;
                    }
                }

                string Friendly(string field)
                {
                    if (string.IsNullOrWhiteSpace(field)) return field;
                    var f = field.Trim();
                    switch (f.ToUpperInvariant())
                    {
                        case "VTDATE": return "Date";
                        case "VTTIME": return "Time";
                        case "VTNO": return "Ticket No";
                        case "VTDNO": return "Ticket Detail No";
                        case "ASLDID": return "ASL ID";
                        case "VHLNO": return "Vehicle No";
                        case "DRVNAME": return "Driver Name";
                        case "VTDESC": return "Description";
                        case "VTQTY": return "Quantity";
                        case "VTTYPE": return "Type";
                        case "VTSTYPE": return "Status Type";
                        case "VTREMRKS": return "Remarks";
                        case "GIDID": return "Gate In ID";
                        case "EGIDID": return "Export Gate In ID";
                        case "VTSSEALNO": return "Seal No";
                        case "VTCTYPE": return "Container Type";
                        case "CGIDID": return "Cargo Gate In ID";
                        case "STFDID": return "Stuffing ID";
                        case "EVLDATE": return "Empty Vehicle Load Date";
                        case "EVSDATE": return "Empty Vehicle Stuff Date";
                        case "ELRDATE": return "Empty Load Return Date";
                        case "TRNSPRTID": return "Transporter ID";
                        case "TRNSPRTNAME": return "Transporter Name";
                        case "GTRNSPRTNAME": return "Other Transporter Name";
                        case "VHLMID": return "Vehicle Master ID";
                        case "QRCDIMGPATH": return "QR Code Image Path";
                        case "DISPSTATUS": return "Status";
                        default: return field;
                    }
                }

                foreach (var row in list)
                {
                    row.OldValue = Map(row.FieldName, row.OldValue);
                    row.NewValue = Map(row.FieldName, row.NewValue);
                    row.FieldName = Friendly(row.FieldName);
                }
            }
            catch { /* Best-effort mapping; do not fail page if lookups have issues */ }

            ViewBag.Module = "ImportVehicleTicket";
            return View("~/Views/ImportGateIn/EditLogGateIn.cshtml", list);
        }

        // Compare two versions for a given VTDNO
        public ActionResult EditLogVehicleTicketCompare(int? vtdid, string versionA, string versionB)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            if (vtdid == null || string.IsNullOrWhiteSpace(versionA) || string.IsNullOrWhiteSpace(versionB))
            {
                TempData["Err"] = "Please provide VTDID, Version A and Version B to compare.";
                return RedirectToAction("EditLogVehicleTicket", new { vtdid = vtdid });
            }

            // Get VTDNO from VTDID
            string vtdnoString = vtdid.Value.ToString();
            var vehicleTicket = context.vehicleticketdetail.AsNoTracking().FirstOrDefault(x => x.VTDID == vtdid.Value);
            if (vehicleTicket != null && !string.IsNullOrEmpty(vehicleTicket.VTDNO))
            {
                vtdnoString = vehicleTicket.VTDNO;
            }

            // Normalize version strings
            versionA = (versionA ?? string.Empty).Trim();
            versionB = (versionB ?? string.Empty).Trim();
            
            // Map '0' or 'v0'/'V0' to 'v0-<VTDNO>' for baseline comparisons
            if (vtdid.HasValue)
            {
                var baseLabel = "v0-" + vtdnoString;
                if (string.Equals(versionA, "0", StringComparison.OrdinalIgnoreCase) || 
                    string.Equals(versionA, "V0", StringComparison.OrdinalIgnoreCase) || 
                    string.Equals(versionA, "v0", StringComparison.OrdinalIgnoreCase))
                    versionA = baseLabel;
                if (string.Equals(versionB, "0", StringComparison.OrdinalIgnoreCase) || 
                    string.Equals(versionB, "V0", StringComparison.OrdinalIgnoreCase) || 
                    string.Equals(versionB, "v0", StringComparison.OrdinalIgnoreCase))
                    versionB = baseLabel;
            }

            var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
            var rowsA = new List<scfs_erp.Models.GateInDetailEditLogRow>();
            var rowsB = new List<scfs_erp.Models.GateInDetailEditLogRow>();
            
            if (cs != null && !string.IsNullOrWhiteSpace(cs.ConnectionString))
            {
                using (var sql = new SqlConnection(cs.ConnectionString))
                using (var cmd = new SqlCommand(@"SELECT [GIDNO],[FieldName],[OldValue],[NewValue],[ChangedBy],[ChangedOn],[Version],[Modules]
                                                FROM [dbo].[GateInDetailEditLog]
                                                WHERE [Modules] = 'ImportVehicleTicket'
                                                  AND [GIDNO] = @VTDNO
                                                  AND RTRIM(LTRIM([Version])) = @V", sql))
                {
                    cmd.Parameters.Add("@VTDNO", System.Data.SqlDbType.NVarChar, 50);
                    cmd.Parameters.Add("@V", System.Data.SqlDbType.NVarChar, 100);

                    sql.Open();
                    cmd.Parameters["@VTDNO"].Value = vtdnoString;
                    cmd.Parameters["@V"].Value = versionA;
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            rowsA.Add(new scfs_erp.Models.GateInDetailEditLogRow
                            {
                                GIDNO = Convert.ToString(r["GIDNO"]),
                                FieldName = Convert.ToString(r["FieldName"]),
                                OldValue = r["OldValue"] == DBNull.Value ? null : Convert.ToString(r["OldValue"]),
                                NewValue = r["NewValue"] == DBNull.Value ? null : Convert.ToString(r["NewValue"]),
                                ChangedBy = Convert.ToString(r["ChangedBy"]),
                                ChangedOn = r["ChangedOn"] != DBNull.Value ? Convert.ToDateTime(r["ChangedOn"]) : DateTime.MinValue,
                                Version = versionA,
                                Modules = r["Modules"] == DBNull.Value ? null : Convert.ToString(r["Modules"])
                            });
                        }
                    }

                    cmd.Parameters["@V"].Value = versionB;
                    using (var r2 = cmd.ExecuteReader())
                    {
                        while (r2.Read())
                        {
                            rowsB.Add(new scfs_erp.Models.GateInDetailEditLogRow
                            {
                                GIDNO = Convert.ToString(r2["GIDNO"]),
                                FieldName = Convert.ToString(r2["FieldName"]),
                                OldValue = r2["OldValue"] == DBNull.Value ? null : Convert.ToString(r2["OldValue"]),
                                NewValue = r2["NewValue"] == DBNull.Value ? null : Convert.ToString(r2["NewValue"]),
                                ChangedBy = Convert.ToString(r2["ChangedBy"]),
                                ChangedOn = r2["ChangedOn"] != DBNull.Value ? Convert.ToDateTime(r2["ChangedOn"]) : DateTime.MinValue,
                                Version = versionB,
                                Modules = r2["Modules"] == DBNull.Value ? null : Convert.ToString(r2["Modules"])
                            });
                        }
                    }
                }
            }

            // Map technical field names to friendly form labels
            try
            {
                // Build lookup dictionaries - handle duplicates by taking first
                var dictTransporter = context.categorymasters.Where(c => c.CATETID == 5 && c.DISPSTATUS == 0)
                    .GroupBy(x => x.CATEID)
                    .ToDictionary(g => g.Key, g => g.First().CATENAME);
                var dictVehicle = context.vehiclemasters.Where(v => v.DISPSTATUS == 0)
                    .GroupBy(x => x.VHLMID)
                    .ToDictionary(g => g.Key, g => g.First().VHLMDESC);

                string Map(string field, string raw)
                {
                    if (string.IsNullOrWhiteSpace(raw)) return raw;
                    int ival;
                    switch (field?.ToUpperInvariant())
                    {
                        case "TRNSPRTID":
                            return int.TryParse(raw, out ival) && dictTransporter.ContainsKey(ival) ? dictTransporter[ival] : raw;
                        case "VHLMID":
                            return int.TryParse(raw, out ival) && dictVehicle.ContainsKey(ival) ? dictVehicle[ival] : raw;
                        case "VTTYPE":
                            if (raw == "1") return "Empty";
                            if (raw == "2") return "Loaded";
                            return raw;
                        case "VTSTYPE":
                            if (raw == "1") return "In";
                            if (raw == "2") return "Out";
                            return raw;
                        case "VTCTYPE":
                            if (raw == "1") return "Cargo";
                            if (raw == "2") return "Empty";
                            return raw;
                        case "DISPSTATUS":
                            return raw == "1" ? "Disabled" : raw == "0" ? "Enabled" : raw;
                        default:
                            return raw;
                    }
                }

                string Friendly(string field)
                {
                    if (string.IsNullOrWhiteSpace(field)) return field;
                    var f = field.Trim();
                    switch (f.ToUpperInvariant())
                    {
                        case "VTDATE": return "Date";
                        case "VTTIME": return "Time";
                        case "VTNO": return "Ticket No";
                        case "VTDNO": return "Ticket Detail No";
                        case "ASLDID": return "ASL ID";
                        case "VHLNO": return "Vehicle No";
                        case "DRVNAME": return "Driver Name";
                        case "VTDESC": return "Description";
                        case "VTQTY": return "Quantity";
                        case "VTTYPE": return "Type";
                        case "VTSTYPE": return "Status Type";
                        case "VTREMRKS": return "Remarks";
                        case "GIDID": return "Gate In ID";
                        case "EGIDID": return "Export Gate In ID";
                        case "VTSSEALNO": return "Seal No";
                        case "VTCTYPE": return "Container Type";
                        case "CGIDID": return "Cargo Gate In ID";
                        case "STFDID": return "Stuffing ID";
                        case "EVLDATE": return "Empty Vehicle Load Date";
                        case "EVSDATE": return "Empty Vehicle Stuff Date";
                        case "ELRDATE": return "Empty Load Return Date";
                        case "TRNSPRTID": return "Transporter ID";
                        case "TRNSPRTNAME": return "Transporter Name";
                        case "GTRNSPRTNAME": return "Other Transporter Name";
                        case "VHLMID": return "Vehicle Master ID";
                        case "QRCDIMGPATH": return "QR Code Image Path";
                        case "DISPSTATUS": return "Status";
                        default: return field;
                    }
                }

                foreach (var row in rowsA)
                {
                    row.OldValue = Map(row.FieldName, row.OldValue);
                    row.NewValue = Map(row.FieldName, row.NewValue);
                    row.FieldName = Friendly(row.FieldName);
                }
                foreach (var row in rowsB)
                {
                    row.OldValue = Map(row.FieldName, row.OldValue);
                    row.NewValue = Map(row.FieldName, row.NewValue);
                    row.FieldName = Friendly(row.FieldName);
                }
            }
            catch { /* best-effort mapping for compare page */ }

            ViewBag.GIDNO = vtdid.Value;
            ViewBag.VersionA = versionA;
            ViewBag.VersionB = versionB;
            ViewBag.RowsA = rowsA;
            ViewBag.RowsB = rowsB;
            ViewBag.Module = "ImportVehicleTicket";

            return View("~/Views/ImportGateIn/EditLogGateInCompare.cshtml");
        }

        private void LogVehicleTicketEdits(VehicleTicketDetail before, VehicleTicketDetail after, string userId)
        {
            if (before == null || after == null) return;
            var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
            if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString)) return;

            // Exclude system or noisy fields
            var exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "VTDID", "PRCSDATE", "LMUSRID", "CUSRID",
                "COMPYID", "SDPTID",
                "TRNSPRTID", "VHLMID"
            };

            // Compute the next version
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
                    WHERE [GIDNO] = @VTDNO AND [Modules] = 'ImportVehicleTicket'", sql))
                {
                    cmd.Parameters.AddWithValue("@VTDNO", after.VTDNO);
                    sql.Open();
                    var obj = cmd.ExecuteScalar();
                    if (obj != null && obj != DBNull.Value)
                        nextVersion = Convert.ToInt32(obj);
                }
            }
            catch { }

            var props = typeof(VehicleTicketDetail).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in props)
            {
                if (!p.CanRead) continue;
                if (p.PropertyType.IsClass && p.PropertyType != typeof(string) && !p.PropertyType.IsValueType)
                    continue;
                if (exclude.Contains(p.Name)) continue;

                var ov = p.GetValue(before, null);
                var nv = p.GetValue(after, null);

                if (BothNull(ov, nv)) continue;

                var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                bool changed;

                if (type == typeof(decimal) || type == typeof(decimal?))
                {
                    var d1 = ToNullableDecimal(ov) ?? 0m;
                    var d2 = ToNullableDecimal(nv) ?? 0m;
                    if (d1 == 0m && d2 == 0m) continue;
                    changed = d1 != d2;
                }
                else if (type == typeof(double) || type == typeof(float))
                {
                    var d1 = Convert.ToDouble(ov ?? 0.0);
                    var d2 = Convert.ToDouble(nv ?? 0.0);
                    if (Math.Abs(d1) < 1e-9 && Math.Abs(d2) < 1e-9) continue;
                    changed = Math.Abs(d1 - d2) > 1e-9;
                }
                else if (type == typeof(int) || type == typeof(int?) || type == typeof(long) || type == typeof(long?) || type == typeof(short) || type == typeof(short?))
                {
                    var i1 = ov == null ? (long?)null : Convert.ToInt64(ov);
                    var i2 = nv == null ? (long?)null : Convert.ToInt64(nv);
                    if (!i1.HasValue && !i2.HasValue) continue;
                    var val1 = i1 ?? 0;
                    var val2 = i2 ?? 0;
                    changed = val1 != val2;
                    if (val1 == 0 && val2 == 0) continue;
                }
                else if (type == typeof(DateTime))
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
                        t1 = new DateTime(t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second);
                        t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
                        changed = t1 != t2;
                    }
                }
                else if (type == typeof(string))
                {
                    var s1 = (Convert.ToString(ov) ?? string.Empty).Trim();
                    var s2 = (Convert.ToString(nv) ?? string.Empty).Trim();
                    bool def1 = string.IsNullOrEmpty(s1) || s1 == "-" || s1 == "0";
                    bool def2 = string.IsNullOrEmpty(s2) || s2 == "-" || s2 == "0";
                    if (def1 && def2) continue;
                    changed = !string.Equals(s1, s2, StringComparison.Ordinal);
                }
                else
                {
                    var s1 = FormatVal(ov);
                    var s2 = FormatVal(nv);
                    changed = !string.Equals(s1, s2, StringComparison.Ordinal);
                }

                if (!changed) continue;

                var os = FormatValForLogging(p.Name, ov);
                var ns = FormatValForLogging(p.Name, nv);

                var versionLabel = $"V{nextVersion}-{after.VTDNO}";
                InsertEditLogRow(cs.ConnectionString, after.VTDNO, p.Name, os, ns, userId, versionLabel, "ImportVehicleTicket");
            }
        }

        private void EnsureBaselineVersionZero(VehicleTicketDetail snapshot, string userId)
        {
            try
            {
                var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
                if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString)) return;
                if (string.IsNullOrWhiteSpace(snapshot.VTDNO)) return;

                using (var sql = new SqlConnection(cs.ConnectionString))
                using (var cmd = new SqlCommand("SELECT COUNT(1) FROM [dbo].[GateInDetailEditLog] WHERE [GIDNO]=@VTDNO AND [Modules]='ImportVehicleTicket' AND (RTRIM(LTRIM([Version]))=@VLower OR RTRIM(LTRIM([Version]))=@VUpper OR RTRIM(LTRIM([Version]))='0' OR RTRIM(LTRIM([Version]))='V0')", sql))
                {
                    cmd.Parameters.AddWithValue("@VTDNO", snapshot.VTDNO);
                    var baselineVerLower = "v0-" + snapshot.VTDNO;
                    var baselineVerUpper = "V0-" + snapshot.VTDNO;
                    cmd.Parameters.AddWithValue("@VLower", baselineVerLower);
                    cmd.Parameters.AddWithValue("@VUpper", baselineVerUpper);
                    sql.Open();
                    var exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    if (exists) return;
                }

                InsertBaselineSnapshot(snapshot, userId);
            }
            catch { }
        }

        private void InsertBaselineSnapshot(VehicleTicketDetail snapshot, string userId)
        {
            var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
            if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString)) return;
            if (string.IsNullOrWhiteSpace(snapshot.VTDNO)) return;
            var baselineVer = "v0-" + snapshot.VTDNO;

            var exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "VTDID", "PRCSDATE", "LMUSRID", "CUSRID", "COMPYID", "SDPTID", "TRNSPRTID", "VHLMID"
            };

            var props = typeof(VehicleTicketDetail).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in props)
            {
                if (!p.CanRead) continue;
                if (p.PropertyType.IsClass && p.PropertyType != typeof(string) && !p.PropertyType.IsValueType) continue;
                if (exclude.Contains(p.Name)) continue;

                if (p.Name.EndsWith("ID", StringComparison.OrdinalIgnoreCase))
                {
                    var baseName = p.Name.Substring(0, p.Name.Length - 2);
                    var nameProp = props.FirstOrDefault(q => q.PropertyType == typeof(string) && (
                        q.Name.Equals(baseName, StringComparison.OrdinalIgnoreCase) ||
                        q.Name.Equals(baseName + "NAME", StringComparison.OrdinalIgnoreCase)
                    ));
                    if (nameProp != null) continue;
                }

                var valObj = p.GetValue(snapshot, null);
                var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

                if (type == typeof(string))
                {
                    var s = (Convert.ToString(valObj) ?? string.Empty).Trim();
                    bool isDefault = string.IsNullOrEmpty(s) || s == "-" || s == "0";
                    if (isDefault) continue;
                }
                else if (type == typeof(int) || type == typeof(long) || type == typeof(short))
                {
                    var i = Convert.ToInt64(valObj ?? 0);
                    if (i == 0) continue;
                }
                else if (type == typeof(decimal))
                {
                    var d = ToNullableDecimal(valObj) ?? 0m;
                    if (d == 0m) continue;
                }
                else if (type == typeof(double) || type == typeof(float))
                {
                    var d = Convert.ToDouble(valObj ?? 0.0);
                    if (Math.Abs(d) < 1e-9) continue;
                }

                var newVal = FormatValForLogging(p.Name, valObj);
                InsertEditLogRow(cs.ConnectionString, snapshot.VTDNO, p.Name, null, newVal, userId, baselineVer, "ImportVehicleTicket");
            }
        }

        private string FormatValForLogging(string fieldName, object value)
        {
            var formattedValue = FormatVal(value);
            if (string.IsNullOrEmpty(formattedValue)) return formattedValue;

            try
            {
                int lookupId;
                if (fieldName.Equals("TRNSPRTID", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(formattedValue, out lookupId) && lookupId > 0)
                    {
                        var transporter = context.categorymasters.FirstOrDefault(c => c.CATETID == 5 && c.CATEID == lookupId && c.DISPSTATUS == 0);
                        if (transporter != null && !string.IsNullOrEmpty(transporter.CATENAME))
                            return transporter.CATENAME;
                    }
                }
                else if (fieldName.Equals("VHLMID", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(formattedValue, out lookupId) && lookupId > 0)
                    {
                        var vehicle = context.vehiclemasters.FirstOrDefault(v => v.VHLMID == lookupId && v.DISPSTATUS == 0);
                        if (vehicle != null && !string.IsNullOrEmpty(vehicle.VHLMDESC))
                            return vehicle.VHLMDESC;
                    }
                }
                else if (fieldName.Equals("VTTYPE", StringComparison.OrdinalIgnoreCase))
                {
                    if (formattedValue == "1") return "Empty";
                    if (formattedValue == "2") return "Loaded";
                }
                else if (fieldName.Equals("VTSTYPE", StringComparison.OrdinalIgnoreCase))
                {
                    if (formattedValue == "1") return "In";
                    if (formattedValue == "2") return "Out";
                }
                else if (fieldName.Equals("VTCTYPE", StringComparison.OrdinalIgnoreCase))
                {
                    if (formattedValue == "1") return "Cargo";
                    if (formattedValue == "2") return "Empty";
                }
                else if (fieldName.Equals("DISPSTATUS", StringComparison.OrdinalIgnoreCase))
                {
                    if (formattedValue == "1") return "Disabled";
                    if (formattedValue == "0") return "Enabled";
                }
            }
            catch { }

            return formattedValue;
        }

        private static string FormatVal(object v)
        {
            if (v == null || v == DBNull.Value) return string.Empty;
            if (v is DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm:ss");
            return Convert.ToString(v);
        }

        private static bool BothNull(object a, object b)
        {
            return (a == null || a == DBNull.Value) && (b == null || b == DBNull.Value);
        }

        private static decimal? ToNullableDecimal(object v)
        {
            if (v == null || v == DBNull.Value) return null;
            if (decimal.TryParse(Convert.ToString(v), out decimal d)) return d;
            return null;
        }

        private static void InsertEditLogRow(string connectionString, string vtdno, string fieldName, string oldValue, string newValue, string changedBy, string versionLabel, string modules)
        {
            try
            {
                using (var sql = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"
                    INSERT INTO [dbo].[GateInDetailEditLog] ([GIDNO], [FieldName], [OldValue], [NewValue], [ChangedBy], [ChangedOn], [Version], [Modules])
                    VALUES (@GIDNO, @FieldName, @OldValue, @NewValue, @ChangedBy, @ChangedOn, @Version, @Modules)", sql))
                {
                    cmd.Parameters.AddWithValue("@GIDNO", vtdno);
                    cmd.Parameters.AddWithValue("@FieldName", fieldName);
                    cmd.Parameters.AddWithValue("@OldValue", (object)oldValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NewValue", (object)newValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ChangedBy", changedBy ?? string.Empty);
                    cmd.Parameters.AddWithValue("@ChangedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Version", versionLabel);
                    cmd.Parameters.AddWithValue("@Modules", modules);
                    sql.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        #endregion

    }
}