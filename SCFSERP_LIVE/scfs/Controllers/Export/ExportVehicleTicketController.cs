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
using scfs_erp;
using System.IO;
using QRCoder;
using System.Drawing;

namespace scfs_erp.Controllers.Export
{
    [SessionExpire]
    public class ExportVehicleTicketController : Controller
    {
        // GET: ExportVehicleTicket
        #region Context Declaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region Index Page
        [Authorize(Roles = "ExportVehicleTicketIndex")]
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
            if (Request.Form.Get("VTCTYPE") != null)
            {
                Session["VTCTYPE"] = Request.Form.Get("VTCTYPE");
            }
            else
            {
                Session["VTCTYPE"] = "99";
            }
            //...........VT type......//
            List<SelectListItem> selectedVTTYP = new List<SelectListItem>();
            if (Convert.ToInt32(Session["VTCTYPE"]) == 1)
            {
                SelectListItem selectedItemVTTY = new SelectListItem { Text = "DIRECT", Value = "1", Selected = true };
                selectedVTTYP.Add(selectedItemVTTY);
                selectedItemVTTY = new SelectListItem { Text = "STUFF", Value = "0", Selected = false };
                selectedVTTYP.Add(selectedItemVTTY);
                selectedItemVTTY = new SelectListItem { Text = "BOTH", Value = "99", Selected = false };
                selectedVTTYP.Add(selectedItemVTTY);
            }
            else if (Convert.ToInt32(Session["VTCTYPE"]) == 0)
            {
                SelectListItem selectedItemVTTY = new SelectListItem { Text = "DIRECT", Value = "1", Selected = false };
                selectedVTTYP.Add(selectedItemVTTY);
                selectedItemVTTY = new SelectListItem { Text = "STUFF", Value = "0", Selected = true };
                selectedVTTYP.Add(selectedItemVTTY);
                selectedItemVTTY = new SelectListItem { Text = "BOTH", Value = "99", Selected = false };
                selectedVTTYP.Add(selectedItemVTTY);
            }
            else
            {
                SelectListItem selectedItemVTTY = new SelectListItem { Text = "DIRECT", Value = "1", Selected = false };
                selectedVTTYP.Add(selectedItemVTTY);
                selectedItemVTTY = new SelectListItem { Text = "STUFF", Value = "0", Selected = false };
                selectedVTTYP.Add(selectedItemVTTY);
                selectedItemVTTY = new SelectListItem { Text = "BOTH", Value = "99", Selected = true };
                selectedVTTYP.Add(selectedItemVTTY);

            }
            ViewBag.VTCTYPE = selectedVTTYP;
            //....end    
            DateTime fromdate = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;
            DateTime todate = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;

            TotalContainerDetails(fromdate, todate);

            return View();
        }
        #endregion

        #region Get Table Data
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Export_Vehicle_Ticket_Master(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), 0);//Convert.ToInt32(Session["VTCTYPE"])
                var aaData = data.Select(d => new string[] { d.VTDATE.Value.ToString("dd/MM/yyyy"), d.VTDNO.ToString(), d.CONTNRNO, d.CONTNRSDESC, d.STFMDNO, d.VTDESC, d.VHLNO, d.VTQTY.ToString(), d.VTDID.ToString(), d.GIDId.ToString(), d.VTSSEALNO, d.GODID.ToString(), d.VTCTYPE.ToString() }).ToArray();
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

        #region TotalContainerDetails
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

            //if (Session["SDATE"].ToString() != "")
            //{
            //    string infdate = Convert.ToString(Session["SDATE"].ToString());
            //    var in_date = infdate.Split(' ');
            //    var in_date1 = in_date[0].Split('-');
            //    fdate = Convert.ToString(in_date1[2] + "-" + in_date1[1] + "-" + in_date1[0]);
            //}
            //else
            //{
            //    fromdate = DateTime.Now.Date;
            //    fdate = Convert.ToString(fromdate);
            //}

            //if (Session["EDATE"].ToString() != "")
            //{
            //    string intdate = Convert.ToString(Session["EDATE"].ToString());

            //    var in_date1 = intdate.Split(' ');
            //    var in_date2 = in_date1[0].Split('-');
            //    tdate = Convert.ToString(in_date2[2] + "-" + in_date2[1] + "-" + in_date2[0]);
            //}
            //else
            //{
            //    todate = DateTime.Now.Date;
            //    tdate = Convert.ToString(todate);
            //}

            var result = context.Database.SqlQuery<PR_EXPORT_LOADVEHICLEGATEOUT_DETAILS_Result>("EXEC PR_EXPORT_LOADVEHICLEGATEOUT_DETAILS @PFDT='" + fdate + "',@PTDT='" + tdate + "',@PSDPTID=2").ToList();

            foreach (var rslt in result)
            {
                if ((rslt.Sno == 2) && (rslt.Descriptn == "EXPORT - VEHICLETICKE"))
                {

                    @ViewBag.Total20 = rslt.c_20;
                    @ViewBag.Total40 = rslt.c_40;
                    @ViewBag.Total45 = rslt.c_45;
                    @ViewBag.TotalTues = rslt.c_tues;

                    Session["EVT20"] = rslt.c_20;
                    Session["EVT40"] = rslt.c_40;
                    Session["EVT45"] = rslt.c_45;
                    Session["EVTTU"] = rslt.c_tues;

                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Redirect to Form
        [Authorize(Roles = "ExportVehicleTicketEdit")]
        public void Edit(string id)
        {
            Response.Redirect("/ExportVehicleTicket/Form/" + id);
        }
        #endregion

        #region Redirect Direct Form
        [Authorize(Roles = "ExportVehicleTicketEdit")]
        public void DirectEdit(string id)
        {
            Response.Redirect("/ExportVehicleTicket/DForm/" + id);
        }
        #endregion

        #region VT Stuffing form
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            var VTDID = 0; var GODID = 0;
            if (id.Contains(';'))
            {
                var param = id.Split(';');
                VTDID = Convert.ToInt32(param[0]);
                GODID = Convert.ToInt32(param[1]);
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

            ViewBag.GIDID = new SelectList(context.Database.SqlQuery<VW_VEHICLETICKET_EXPORT_CONTAINER_CBX_ASSGN>("select * from VW_VEHICLETICKET_EXPORT_CONTAINER_CBX_ASSGN order by contnrno").ToList(), "GIDID", "CONTNRNO");

            //ViewBag.GOOTYPE = new SelectList(context.export_Gateout_Gootypes, "GOOTYPE", "GOOTYPEDESC");
            ViewBag.GOOTYPE = new SelectList(context.Export_OperationTypeMaster.OrderBy(m => m.EOPTDESC), "EOPTID", "EOPTDESC");

            //-----------------------------Transport type-----------
            List<SelectListItem> selectedType_ = new List<SelectListItem>();
            SelectListItem selectedItemtt = new SelectListItem { Text = "STL", Value = "1", Selected = false };
            selectedType_.Add(selectedItemtt);
            selectedItemtt = new SelectListItem { Text = "SANCO", Value = "2", Selected = true };
            selectedType_.Add(selectedItemtt);
            selectedItemtt = new SelectListItem { Text = "OTHERS", Value = "3", Selected = false };
            selectedType_.Add(selectedItemtt);
            ViewBag.GOTTYPE = selectedType_;
            //end


            //-----------------------------CHA type-----------
            List<SelectListItem> selectedCHA = new List<SelectListItem>();
            SelectListItem selectedItemCHA = new SelectListItem { Text = "SANCO", Value = "1", Selected = true };
            selectedCHA.Add(selectedItemCHA);

            selectedItemCHA = new SelectListItem { Text = "OTHERS", Value = "2", Selected = false };
            selectedCHA.Add(selectedItemCHA);
            ViewBag.GOCTYPE = selectedCHA;
            //end
            if (VTDID != 0)//Edit Mode
            {
                tab = context.vehicleticketdetail.Find(VTDID); tab1 = context.gateoutdetail.Find(GODID);
                //ViewBag.GOOTYPE = new SelectList(context.export_Gateout_Gootypes, "GOOTYPE", "GOOTYPEDESC", tab1.GOOTYPE);
                //ViewBag.GOOTYPE = new SelectList(context.Export_OperationTypeMaster.OrderBy(m => m.EOPTDESC), "EOPTID", "EOPTDESC", tab1.GOOTYPE);
                  //ViewBag.ASLDID = new SelectList(context.VW_VEHICLETICKET_EXPORT_CONTAINER_CBX_ASSGN, "ASLDID", "CONTNRNO",tab.ASLDID);
                //-----------Getting Gate_In Details-----------------//

                var query = context.Database.SqlQuery<string>("select CONTNRNO from VW_EXPORT_VEHICLE_TICKET_MOD_ASSGN where VTDID=" + VTDID).ToList();


                var query1 = (from r in context.gateindetails.Where(x => x.SDPTID == 2 && x.DISPSTATUS == 0 && x.CONTNRID == 1)
                              join s in context.containersizemasters on r.CONTNRSID equals s.CONTNRSID
                              join d in context.stuffingdetails on r.GIDID equals d.GIDID
                              join m in context.stuffingmasters on d.STFMID equals m.STFMID
                              where (r.GIDID == tab.GIDID)

                              select new { r.GIDID, r.GIDATE, r.EXPRTRNAME, r.CHANAME, r.CHAID, r.CONTNRNO, r.STMRNAME, r.IGMNO, r.GPLNO, s.CONTNRSDESC, r.GPWGHT, m.STFMNO, r.CSEALNO, r.LPSEALNO }
                            ).ToList();
                if (query1.Count > 0)
                {
                    ViewBag.CONTNRNO = query1[0].CONTNRNO.ToString();
                    ViewBag.CONTNRSDESC = query1[0].CONTNRSDESC.ToString();
                    ViewBag.GIDATE = query1[0].GIDATE.ToString("dd/MM/yyyy");
                    if (query1[0].EXPRTRNAME is null)
                        ViewBag.EXPRTRNAME = "";
                    else
                        ViewBag.EXPRTRNAME = query1[0].EXPRTRNAME.ToString();
                    ViewBag.CHANAME = query1[0].CHANAME.ToString();
                    ViewBag.STFMNO = query1[0].STFMNO.ToString();
                   // ViewBag.CSEALNO = query1[0].CSEALNO.ToString();
                    //ViewBag.LPSEALNO = query1[0].LPSEALNO.ToString();
                }
                //ViewBag.CONTNRNO = query[0].ToString();
                //List<SelectListItem> selectedTypeedit = new List<SelectListItem>();
                //if (Convert.ToInt32(tab1.GOTTYPE) == 1)
                //{

                //    SelectListItem selectedItemtYP = new SelectListItem { Text = "STL", Value = "1", Selected = true };
                //    selectedTypeedit.Add(selectedItemtYP);
                //    selectedItemtYP = new SelectListItem { Text = "SANCO", Value = "2", Selected = false };
                //    selectedTypeedit.Add(selectedItemtYP);
                //    selectedItemtYP = new SelectListItem { Text = "OTHERS", Value = "3", Selected = false };
                //    selectedTypeedit.Add(selectedItemtYP);
                //    ViewBag.GOTTYPE = selectedTypeedit;
                //}
                //else if (Convert.ToInt32(tab1.GOTTYPE) == 3)
                //{

                //    SelectListItem selectedItemtYP = new SelectListItem { Text = "STL", Value = "1", Selected = false };
                //    selectedTypeedit.Add(selectedItemtYP);
                //    selectedItemtYP = new SelectListItem { Text = "SANCO", Value = "2", Selected = false };
                //    selectedTypeedit.Add(selectedItemtYP);
                //    selectedItemtYP = new SelectListItem { Text = "OTHERS", Value = "3", Selected = true };
                //    selectedTypeedit.Add(selectedItemtYP);
                //    ViewBag.GOTTYPE = selectedTypeedit;
                //}



                ////----------------------cha type----------------
                //if (Convert.ToInt32(tab1.GOCTYPE) == 2)
                //{
                //    List<SelectListItem> selectedCHAedit = new List<SelectListItem>();
                //    SelectListItem selectedItemCHAtyp = new SelectListItem { Text = "SANCO", Value = "1", Selected = false };
                //    selectedCHAedit.Add(selectedItemCHAtyp);

                //    selectedItemCHAtyp = new SelectListItem { Text = "OTHERS", Value = "2", Selected = true };
                //    selectedCHAedit.Add(selectedItemCHAtyp);
                //    ViewBag.GOCTYPE = selectedCHAedit;
                //}
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
                vg.godetail = context.gateoutdetail.Find(GODID);
                vg.vtdetail = context.vehicleticketdetail.Find(VTDID);
            }
            return View(vg);
        }
        #endregion

        #region Direct form
        public ActionResult DForm(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            var VTDID = 0; var GODID = 0;
            if (id.Contains(';'))
            {
                var param = id.Split(';');
                VTDID = Convert.ToInt32(param[0]);
                GODID = Convert.ToInt32(param[1]);
            }

            VehicleTicketDetail tab = new VehicleTicketDetail(); 

            //GateOutDetail tab1 = new GateOutDetail();
            //VehicleTicketGateOut vg = new VehicleTicketGateOut();

            tab.EVSDATE = DateTime.Now.Date;
            tab.EVLDATE = DateTime.Now.Date;
            tab.ELRDATE = DateTime.Now.Date;
            tab.VTDATE = DateTime.Now.Date;
            tab.VTTIME = DateTime.Now;

            tab.VTDID = 0;

            ViewBag.GIDID = new SelectList(context.Database.SqlQuery<VW_VEHICLETICKET_EXPORT_CONTAINER_CBX_ASSGN_DIRECT>("select * from VW_VEHICLETICKET_EXPORT_CONTAINER_CBX_ASSGN_DIRECT order by contnrno").ToList(), "GIDID", "CONTNRNO");

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

            //ViewBag.GOOTYPE = new SelectList(context.export_Gateout_Gootypes, "GOOTYPE", "GOOTYPEDESC");
            ViewBag.GOOTYPE = new SelectList(context.Export_OperationTypeMaster.OrderBy(m => m.EOPTDESC), "EOPTID", "EOPTDESC");


            //-----------------------------Transport type-----------
            List<SelectListItem> selectedType_ = new List<SelectListItem>();
            SelectListItem selectedItemtt = new SelectListItem { Text = "STL", Value = "1", Selected = false };
            selectedType_.Add(selectedItemtt);
            selectedItemtt = new SelectListItem { Text = "SANCO", Value = "2", Selected = true };
            selectedType_.Add(selectedItemtt);
            selectedItemtt = new SelectListItem { Text = "OTHERS", Value = "3", Selected = false };
            selectedType_.Add(selectedItemtt);
            ViewBag.GOTTYPE = selectedType_;
            //end

            //-----------------------------CHA type-----------
            List<SelectListItem> selectedCHA = new List<SelectListItem>();
            SelectListItem selectedItemCHA = new SelectListItem { Text = "SANCO", Value = "1", Selected = true };
            selectedCHA.Add(selectedItemCHA);

            selectedItemCHA = new SelectListItem { Text = "OTHERS", Value = "2", Selected = false };
            selectedCHA.Add(selectedItemCHA);
            ViewBag.GOCTYPE = selectedCHA;
            //end
            if (VTDID != 0)//Edit Mode
            {
                tab = context.vehicleticketdetail.Find(VTDID); //tab1 = context.gateoutdetail.Find(GODID);
                //ViewBag.GOOTYPE = new SelectList(context.export_Gateout_Gootypes, "GOOTYPE", "GOOTYPEDESC", tab1.GOOTYPE);
                //ViewBag.GOOTYPE = new SelectList(context.Export_OperationTypeMaster.OrderBy(m => m.EOPTDESC), "EOPTID", "EOPTDESC", tab1.GOOTYPE);
                  //ViewBag.ASLDID = new SelectList(context.VW_VEHICLETICKET_EXPORT_CONTAINER_CBX_ASSGN, "ASLDID", "CONTNRNO",tab.ASLDID);
                //-----------Getting Gate_In Details-----------------//

                var query = context.Database.SqlQuery<string>("select CONTNRNO from VW_EXPORT_VEHICLE_TICKET_MOD_ASSGN where VTDID=" + VTDID).ToList();
                ViewBag.CONTNRNO = query[0].ToString();
                //List<SelectListItem> selectedTypeedit = new List<SelectListItem>();
                //if (Convert.ToInt32(tab1.GOTTYPE) == 1)
                //{

                //    SelectListItem selectedItemtYP = new SelectListItem { Text = "STL", Value = "1", Selected = true };
                //    selectedTypeedit.Add(selectedItemtYP);
                //    selectedItemtYP = new SelectListItem { Text = "SANCO", Value = "2", Selected = false };
                //    selectedTypeedit.Add(selectedItemtYP);
                //    selectedItemtYP = new SelectListItem { Text = "OTHERS", Value = "3", Selected = false };
                //    selectedTypeedit.Add(selectedItemtYP);
                //    ViewBag.GOTTYPE = selectedTypeedit;
                //}
                //else if (Convert.ToInt32(tab1.GOTTYPE) == 3)
                //{

                //    SelectListItem selectedItemtYP = new SelectListItem { Text = "STL", Value = "1", Selected = false };
                //    selectedTypeedit.Add(selectedItemtYP);
                //    selectedItemtYP = new SelectListItem { Text = "SANCO", Value = "2", Selected = false };
                //    selectedTypeedit.Add(selectedItemtYP);
                //    selectedItemtYP = new SelectListItem { Text = "OTHERS", Value = "3", Selected = true };
                //    selectedTypeedit.Add(selectedItemtYP);
                //    ViewBag.GOTTYPE = selectedTypeedit;
                //}



                ////----------------------cha type----------------
                //if (Convert.ToInt32(tab1.GOCTYPE) == 2)
                //{
                //    List<SelectListItem> selectedCHAedit = new List<SelectListItem>();
                //    SelectListItem selectedItemCHAtyp = new SelectListItem { Text = "SANCO", Value = "1", Selected = false };
                //    selectedCHAedit.Add(selectedItemCHAtyp);

                //    selectedItemCHAtyp = new SelectListItem { Text = "OTHERS", Value = "2", Selected = true };
                //    selectedCHAedit.Add(selectedItemCHAtyp);
                //    ViewBag.GOCTYPE = selectedCHAedit;
                //}
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
                
                //vg.vtdetail = context.vehicleticketdetail.Find(VTDID);
            }
            return View(tab);
        }
        #endregion

        #region direct save
        public void dsavedata(VehicleTicketDetail tab)
        {
            using (context = new SCFSERPContext())
            {

                try
                {
                    using (var trans = context.Database.BeginTransaction())
                    {
                        
                        tab.LMUSRID = Session["CUSRID"].ToString();
                        //tab.vtdetail.LMUSRID = 1;
                        tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                        tab.SDPTID = 2;
                        tab.DISPSTATUS = 0;
                        tab.PRCSDATE = DateTime.Now;
                        //tab.vtdetail.VTQTY = 0;

                        string nop = Convert.ToString(tab.VTQTY);

                        if (nop == "" || nop == null)
                        { tab.VTQTY = 0; }
                        else { tab.VTQTY = Convert.ToDecimal(nop); }

                        string ASLDID = Convert.ToString(tab.ASLDID);

                        if (ASLDID == "" || ASLDID == null)
                        { tab.ASLDID = 0; }
                        else { tab.ASLDID = Convert.ToInt32(ASLDID); }


                        string VTSSEALNO = Convert.ToString(tab.VTSSEALNO);

                        if (VTSSEALNO == "" || VTSSEALNO == null)
                        { tab.VTSSEALNO = "-"; }
                        else { tab.VTSSEALNO = Convert.ToString(VTSSEALNO); }

                        //tab.vtdetail.VTSSEALNO = "-";
                        tab.VTSTYPE = 0;
                        tab.VTCTYPE = 1;

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

                        tab.GIDID = Convert.ToInt32(Request.Form.Get("GIDID"));
                        if ((tab.VTDID).ToString() != "0")
                        {
                            context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                        else
                        {
                            
                            tab.VTNO = Convert.ToInt32(Autonumber.autonum("vehicleticketDetail", "VTNO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=2 and VTSTYPE=0 and VTCTYPE= 0").ToString());
                            tab.CUSRID = Session["CUSRID"].ToString(); 
                            int ano = tab.VTNO;
                            string prfx = string.Format("{0:D5}", ano);
                            tab.VTDNO = prfx.ToString();
                            context.vehicleticketdetail.Add(tab);
                            context.SaveChanges();
                        }
                        //..........................gateout detail...................//

                        //tab.godetail.GODATE = tab.vtdetail.VTDATE; ;
                        //tab.godetail.GOTIME = tab.vtdetail.VTTIME; ;
                        //tab.godetail.COMPYID = Convert.ToInt32(Session["compyid"]);
                        //tab.godetail.VHLNO = tab.vtdetail.VHLNO;
                        //tab.godetail.GIDID = Convert.ToInt32(Request.Form.Get("GIDID"));
                        //tab.godetail.SDPTID = 2;
                        //tab.godetail.REGSTRID = 1;
                        //tab.godetail.TRANDID = 0;
                        //tab.godetail.GOBTYPE = 1;

                        //tab.godetail.CUSRID = Session["CUSRID"].ToString();
                        //tab.godetail.LMUSRID = Session["CUSRID"].ToString();
                        //tab.godetail.PRCSDATE = DateTime.Now;
                        //tab.godetail.EHIDATE = DateTime.Now;
                        //tab.godetail.EHITIME = DateTime.Now;
                        //tab.godetail.GDRVNAME = tab.vtdetail.DRVNAME;
                        //if ((tab.godetail.GODID).ToString() != "0")
                        //{
                        //    context.Entry(tab.godetail).State = System.Data.Entity.EntityState.Modified;
                        //    context.SaveChanges();
                        //}
                        //else
                        //{
                        //    tab.godetail.GONO = Convert.ToInt16(Autonumber.autonum("GateOutDetail", "GONO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=2").ToString());
                        //    int ano = tab.godetail.GONO;
                        //    // string prfx = string.Format("{0:D5}", ano);
                        //    tab.godetail.GODNO = ano.ToString();
                        //    context.gateoutdetail.Add(tab.godetail);
                        //    context.SaveChanges();
                        //}

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

        #region savedata
        public void savedata(VehicleTicketGateOut tab)
        {
            using (context = new SCFSERPContext())
            {

                try
                {
                    using (var trans = context.Database.BeginTransaction())
                    {
                        
                        tab.vtdetail.LMUSRID = Session["CUSRID"].ToString();
                        //tab.vtdetail.LMUSRID = 1;
                        tab.vtdetail.COMPYID = Convert.ToInt32(Session["compyid"]);
                        tab.vtdetail.SDPTID = 2;
                        tab.vtdetail.DISPSTATUS = 0;
                        tab.vtdetail.PRCSDATE = DateTime.Now;
                        
                        string nop = Convert.ToString(tab.vtdetail.VTQTY);

                        if (nop == "" || nop == null)
                        { tab.vtdetail.VTQTY = 0; }
                        else { tab.vtdetail.VTQTY = Convert.ToDecimal(nop); }

                        string ASLDID = Convert.ToString(tab.vtdetail.ASLDID);

                        if (ASLDID == "" || ASLDID == null)
                        { tab.vtdetail.ASLDID = 0; }
                        else { tab.vtdetail.ASLDID = Convert.ToInt32(ASLDID); }

                        string VTSSEALNO = Convert.ToString(tab.vtdetail.VTSSEALNO);

                        if (VTSSEALNO == "" || VTSSEALNO == null)
                        { tab.vtdetail.VTSSEALNO = "-"; }
                        else { tab.vtdetail.VTSSEALNO = Convert.ToString(VTSSEALNO); }
                        tab.vtdetail.VTSTYPE = 0;
                        tab.vtdetail.VTCTYPE = 0;

                        string indate = Convert.ToString(tab.vtdetail.VTDATE);
                        if (indate != null || indate != "")
                        {
                            tab.vtdetail.VTDATE = Convert.ToDateTime(indate).Date;
                        }
                        else { tab.vtdetail.VTDATE = DateTime.Now.Date; }

                        string intime = Convert.ToString(tab.vtdetail.VTTIME);
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

                                    tab.vtdetail.VTTIME = Convert.ToDateTime(in_datetime);
                                }
                                else { tab.vtdetail.VTTIME = DateTime.Now; }
                            }
                            else { tab.vtdetail.VTTIME = DateTime.Now; }
                        }
                        else { tab.vtdetail.VTTIME = DateTime.Now; }

                        tab.vtdetail.GIDID = Convert.ToInt32(Request.Form.Get("GIDID"));
                        if ((tab.vtdetail.VTDID).ToString() != "0")
                        {
                            context.Entry(tab.vtdetail).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                        else
                        {
                            tab.vtdetail.VTNO = Convert.ToInt32(Autonumber.autonum("vehicleticketDetail", "VTNO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=2 and VTSTYPE=0  and VTCTYPE=0").ToString());
                            tab.vtdetail.CUSRID = Session["CUSRID"].ToString(); 
                            int ano = tab.vtdetail.VTNO;
                            string prfx = string.Format("{0:D5}", ano);
                            tab.vtdetail.VTDNO = prfx.ToString();
                            context.vehicleticketdetail.Add(tab.vtdetail);
                            context.SaveChanges();
                        }
                        //..........................gateout detail...................//

                        //tab.godetail.GODATE = tab.vtdetail.VTDATE;
                        //tab.godetail.GOTIME = tab.vtdetail.VTTIME;
                        //tab.godetail.COMPYID = Convert.ToInt32(Session["compyid"]);
                        //tab.godetail.VHLNO = tab.vtdetail.VHLNO;
                        //tab.godetail.GIDID = Convert.ToInt32(Request.Form.Get("GIDID"));
                        //tab.godetail.SDPTID = 2;
                        //tab.godetail.REGSTRID = 1;
                        //tab.godetail.TRANDID = 0;
                        //tab.godetail.GOBTYPE = 1;

                        //tab.godetail.CUSRID = Session["CUSRID"].ToString();
                        //tab.godetail.LMUSRID = Session["CUSRID"].ToString();
                        //tab.godetail.PRCSDATE = DateTime.Now;
                        //tab.godetail.EHIDATE = DateTime.Now;
                        //tab.godetail.EHITIME = DateTime.Now;
                        //tab.godetail.GDRVNAME = tab.vtdetail.DRVNAME;
                        //if ((tab.godetail.GODID).ToString() != "0")
                        //{
                        //    context.Entry(tab.godetail).State = System.Data.Entity.EntityState.Modified;
                        //    context.SaveChanges();
                        //}
                        //else
                        //{
                        //    tab.godetail.GONO = Convert.ToInt16(Autonumber.autonum("GateOutDetail", "GONO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=2").ToString());
                        //    int ano = tab.godetail.GONO;
                        //    // string prfx = string.Format("{0:D5}", ano);
                        //    tab.godetail.GODNO = ano.ToString();
                        //    context.gateoutdetail.Add(tab.godetail);
                        //    context.SaveChanges();
                        //}

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

        #region GetDriverName
        public JsonResult GetDriver(string id)
        {
            int gid = 0;
            if (id == "" || id == null)
            { gid = 0; }
            else { gid = Convert.ToInt32(id); }

            var result = (from r in context.vehicleticketdetail.Where(x => x.GIDID == gid)
                          select new { r.DRVNAME }
                            ).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Details
        public JsonResult Detail(int id)
        {
            var query = context.Database.SqlQuery<VW_VEHICLETICKET_EXPORT_CONTAINER_CTRL_ASSGN>("select * from VW_VEHICLETICKET_EXPORT_CONTAINER_CTRL_ASSGN where GIDID=" + id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public JsonResult directDetail(int id)
        {
            var query = context.Database.SqlQuery<VW_VEHICLETICKET_EXPORT_CONTAINER_CTRL_ASSGN_DIRECT>("select * from VW_VEHICLETICKET_EXPORT_CONTAINER_CTRL_ASSGN_DIRECT where GIDID=" + id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SlipMaxDate
        public JsonResult SlipMaxDate(string id)
        {

            int ids = 0;
            if (id != "" || id != null) { ids = Convert.ToInt32(id); }

            var data = (from q in context.authorizatioslipmaster
                        join b in context.authorizationslipdetail on q.ASLMID equals b.ASLMID
                        where b.ASLDID == ids && q.SDPTID == 2
                        group q by q.ASLMDATE into g
                        select new { ASLMDATE = g.Max(t => t.ASLMDATE) }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Containerno auto complete
        public JsonResult AutoContainer(string term)
        {

            string sqry1 = "select distinct CONTNRNO,GIDID  from VW_VEHICLETICKET_EXPORT_CONTAINER_CBX_ASSGN WHERE CONTNRNO LIKE '%" + term + "%'  ";

            var result = context.Database.SqlQuery<VW_VEHICLETICKET_EXPORT_CONTAINER_CBX_ASSGN>(sqry1).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Autocomplete Transporter Name        
        public JsonResult AutoTransporter(string term)
        {
            var result = (from r in context.categorymasters.Where(x => x.CATETID == 5 && x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).OrderBy(x => x.CATENAME).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Vehicle PNR On  Id  
        public JsonResult AutoVehicleNo(string term)
        {
            var result = (from vehicle in context.vehiclemasters.Where(x => x.DISPSTATUS == 0)
                          where vehicle.VHLMDESC.ToLower().Contains(term.ToLower())
                          select new { vehicle.VHLMDESC, vehicle.VHLMID }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Autocomplete Vehicle based On  Id  
        public JsonResult GetVehicle(string id)//vehicl
        {
            var param = id.Split('-');
            var tid = 0;
            var vid = 0;

            if (param[0] != "" || param[0] != "0" || param[0] != null)
            { tid = Convert.ToInt32(param[0]); }
            else { tid = 0; }

            if (param[1] != "" || param[1] != "0" || param[1] != null)
            { vid = Convert.ToInt32(param[1]); }
            else { vid = 0; }

            var query = context.Database.SqlQuery<VehicleMaster>("select * from VehicleMaster WHERE TRNSPRTID = " + tid + " and VHLMID = " + vid + "").ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region print popup
        public ActionResult PrintDet(string id)
        {
            var param = id.Split('-');

            ViewBag.VTSSEALNO = param[1];
            ViewBag.VTQTY = param[0];
            ViewBag.VTDID = param[2];
            return View();
        }
        #endregion

        #region Printview
        [Authorize(Roles = "ExportVehicleTicketPrint")]
        public void PrintView(int id)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "EXPORTVEHICLETICKET", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Export_VT.RPT");
                cryRpt.RecordSelectionFormula = "{VW_EXPORT_VT_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_VT_PRINT_ASSGN.VTDID} = " + id;

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

        #region Qr Code Printview
        [Authorize(Roles = "ExportVehicleTicketPrint")]
        public void QRCPrintView(int id)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

            GenerateQRCodeFile(id);

            string QRpath = Server.MapPath("~/EXPVTQRCode/");
            string VTQRpath = QRpath + id.ToString() + ".png";

            if (System.IO.File.Exists(VTQRpath))
            {
                context.Database.ExecuteSqlCommand("Update VEHICLETICKETDETAIL Set QRCDIMGPATH = '" + VTQRpath + "' WHERE VTDID =" + id);

                context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
                var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "EXPORTVEHICLETICKET", Convert.ToInt32(id), Session["CUSRID"].ToString());
                if (TMPRPT_IDS == "Successfully Added")
                {
                    ReportDocument cryRpt = new ReportDocument();
                    TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                    TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                    ConnectionInfo crConnectionInfo = new ConnectionInfo();
                    Tables CrTables;

                    cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Export_VT_QrCode.RPT");
                    cryRpt.RecordSelectionFormula = "{VW_EXPORT_VT_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_VT_PRINT_ASSGN.VTDID} = " + id;

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

        }
        #endregion

        #region QrCode Gneration
        public void GenerateQRCodeFile(int? id = 0)
        {

            string barcodePath = Server.MapPath("~/EXPVTQRCode/" + id.ToString() + ".png");
            var result = context.Database.SqlQuery<pr_Get_Export_VT_Details_For_QRCode_Result>("exec pr_Get_Export_VT_Details_For_QRCode @PVTDID=" + id).ToList();//........procedure  for edit mode details data
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
                //string QRContent = TmpContnrNo + "|" + TmpSize + "|" + TmpInDate + "|" + TmpImprtName + "|" + TmpStmrName + "|" + TmpVhlNo + "|" + TmpVslName + "|" + TmpVslName + "|" + TmpVoyNo + "|" + TmpPrdtDesc + "|" + TmpWght + "|" + TmpIGMNo + "|" + TmpLNo + "|" + TmpLPsealNo + "|" + TmpBLNo + "|";
                string QRContent = TmpContnrNo + "|" + TmpSize + "|" + TmpInDate + "|" + TmpStmrName + "|" + TmpVhlNo + "|" + TmpVslName + "|"  ;
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(QRContent, QRCodeGenerator.ECCLevel.Q);


                        using (Bitmap bitMap = qrCode.GetGraphic(20))
                        {
                            bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] byteImage = ms.ToArray();
                            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                            img.Save(barcodePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                    }
                }
                catch (Exception e)
                {
                    Response.Write(e.Message);
                }

            }

        }
        #endregion


        #region Delete Record
        [Authorize(Roles = "ExportVehicleTicketDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            var param = id.Split('-');
            var vtid = Convert.ToInt32(param[0]);
            String temp = Delete_fun.delete_check1(fld, param[1]);
            if (temp.Equals("PROCEED"))
            {
                VehicleTicketDetail vehicleticketdetail = context.vehicleticketdetail.Find(vtid);
                context.vehicleticketdetail.Remove(vehicleticketdetail);
               // GateOutDetail gateoutdetail = context.gateoutdetail.Find(Convert.ToInt32(param[2]));
             //   context.gateoutdetail.Remove(gateoutdetail);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion

    }
}