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
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

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

    }
}