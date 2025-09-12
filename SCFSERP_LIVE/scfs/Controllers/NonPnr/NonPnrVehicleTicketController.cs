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
using System.IO;
using QRCoder;
using System.Drawing;

namespace scfs_erp.Controllers.NonPnr
{
    [SessionExpire]
    public class NonPnrVehicleTicketController : Controller
    {
        // GET: NonPnrVehicleTicket

        SCFSERPContext context = new SCFSERPContext();

        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
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
            if (Request.Form.Get("TRANBTYPE") != null)
            {
                Session["TRANBTYPE"] = Request.Form.Get("TRANBTYPE");
            }
            else
            {
                Session["TRANBTYPE"] = "1";
            }
            //...........Bill type......//
            List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
            if (Convert.ToInt32(Session["TRANBTYPE"]) == 1)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);

            }
            else
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = true };
                selectedBILLYPE.Add(selectedItemGPTY);

            }
            ViewBag.TRANBTYPE = selectedBILLYPE;
            //....end           

            DateTime sd = Convert.ToDateTime(Session["SDATE"]).Date;
            DateTime ed = Convert.ToDateTime(Session["EDATE"]).Date;
            return View();
        }

        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/NonPnrVehicleTicket/NForm/" + id);

            //Response.Redirect("/NonPnrVehicleTicket/NForm/" + id);
        }


        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {

            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                var aslmType = new System.Data.Entity.Core.Objects.ObjectParameter("ASLMTYPE", typeof(int));


                var data = e.pr_Search_NonPnr_VehicleTicket(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]),
                                                Convert.ToInt32(Session["TRANBTYPE"])).ToList();

                //var aaData = data.Select(d => new string[] { Convert.ToDateTime(d.VTDATE).ToString("dd/MM/yyyy"), d.VTDNO.ToString(), d.CONTNRNO, d.CONTNRSDESC, d.ASLMDNO, d.VTTYPE.ToString(), d.CHANAME.ToString(), d.BOENO.ToString(), d.VTDID.ToString() }).ToArray();
                var aaData = data.Select(d => new string[] { Convert.ToDateTime(d.VTDATE).ToString("dd/MM/yyyy"), d.VTDNO.ToString(), d.CONTNRNO, d.IGMNO, d.GPLNO, d.CONTNRSDESC, d.ASLMDNO, d.VTTYPE.ToString(), d.CHANAME.ToString(), d.BOENO.ToString(), d.VTDID.ToString(), d.GIDId.ToString(), d.GODID.ToString(), d.EGIDID.ToString(), d.GOSTS }).ToArray();




                //var aaData = data.Select(d => new string[] { d.VTDATE.Value.ToString("dd/MM/yyyy"), d.VTDNO, d.CONTNRNO, d.CONTNRSDESC, d.VTSSEALNO, d.VTDESC, d.VHLNO, d.VTQTY.ToString(), d.VTDID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize(Roles = "NonPnrVehicleTicketCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }


            VehicleTicketDetail tab = new VehicleTicketDetail();
            tab.EVSDATE = DateTime.Now.Date;
            tab.EVLDATE = DateTime.Now.Date;
            tab.ELRDATE = DateTime.Now.Date;
            tab.VTDATE = DateTime.Now.Date;
            tab.VTTIME = DateTime.Now;

            tab.VTDID = 0;


            //ViewBag.ASLDID = new SelectList(context.Database.SqlQuery<VW_VEHICLETICKET_NONPNR_CONTAINER_CBX_ASSGN>("select * from VW_VEHICLETICKET_NONPNR_CONTAINER_CBX_ASSGN").ToList(), "ASLDID", "CONTNRNO"); 
            ViewBag.ASLDID = new SelectList(""); 
            



            //-----------------------------type-----------
            List<SelectListItem> selectedType = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
            selectedType.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
            selectedType.Add(selectedItem1);
            ViewBag.VTTYPE = selectedType;


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
                //-----------Getting Gate_In Details-----------------//

                var query = context.Database.SqlQuery<string>("select CONTNRNO from GATEINDETAIL where GIDID=" + tab.GIDID).ToList();
                ViewBag.CONTNRNO = query[0].ToString();

            }
            return View(tab);
        }

        public ActionResult NForm(int? id = 0)//new with igm,line no based
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }


            VehicleTicketDetail tab = new VehicleTicketDetail();
            tab.EVSDATE = DateTime.Now.Date;
            tab.EVLDATE = DateTime.Now.Date;
            tab.ELRDATE = DateTime.Now.Date;
            tab.VTDATE = DateTime.Now.Date;
            tab.VTTIME = DateTime.Now;

            tab.VTDID = 0;

            //ViewBag.ASLDID = new SelectList(context.Database.SqlQuery<VW_VEHICLETICKET_NONPNR_CONTAINER_CBX_ASSGN>("select * from VW_VEHICLETICKET_NONPNR_CONTAINER_CBX_ASSGN").ToList(), "ASLDID", "CONTNRNO");
            ViewBag.ASLDID = new SelectList("");


            //-----------------------------type-----------
            List <SelectListItem> selectedType = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
            selectedType.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
            selectedType.Add(selectedItem1);
            ViewBag.VTTYPE = selectedType;            

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
                //-----------Getting Gate_In Details-----------------//

                var query = context.Database.SqlQuery<string>("select CONTNRNO from GATEINDETAIL where GIDID=" + tab.GIDID).ToList();
                ViewBag.CONTNRNO = query[0].ToString();

            }
            return View(tab);
        }

        public void savedata(VehicleTicketDetail tab)
         {
            if ((tab.VTDID).ToString() == "0")
            {
                tab.CUSRID = Session["CUSRID"].ToString(); 
            }


            tab.LMUSRID = Session["CUSRID"].ToString();
            tab.COMPYID = Convert.ToInt32(Session["compyid"]);
            tab.SDPTID = 9;
            tab.DISPSTATUS = 0;
            tab.PRCSDATE = DateTime.Now;
            //tab.VTQTY = 0;
            //tab.VTSSEALNO = "-";
            //tab.VTSTYPE = 0;
            //tab.VTDATE = tab.VTTIME.Date;

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

            tab.EVLDATE = null; tab.ELRDATE = null; tab.EVSDATE = null;
            tab.STFDID = 0;

           
            if ((tab.VTDID).ToString() != "0")
            {
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                tab.VTNO = Convert.ToInt32(Autonumber.autonum("vehicleticketDetail", "VTNO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=9").ToString());
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
                    gatein.GIVHLTYPE = 0;//.actual gatein givhltype
                    gatein.TRNSPRTID = 0;
                    gatein.TRNSPRTNAME = "-";
                    gatein.AVHLNO = "-"; gatein.VHLNO = "-";
                    gatein.DRVNAME = "-";
                    gatein.GPREFNO = "-";
                    gatein.IMPRTID = 0;
                    gatein.IMPRTNAME = "-";
                    gatein.STMRID = Convert.ToInt32(Request.Form.Get("TMPSTMRID"));//stmrid from gatein
                    gatein.STMRNAME = Convert.ToString(Request.Form.Get("STMRNAME"));//stmrname from gatein
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
                    gatein.CHAID = 0;
                    gatein.CHANAME = "-";
                    gatein.BOENO = "-";
                    gatein.BOEDATE = DateTime.Now;
                    gatein.BLNO = "-";
                    gatein.BOEDID = 0;
                    gatein.CFSNAME = "-";
                    gatein.BCHAID = 0;
                    gatein.BCHANAME = "-";

                    
                    tab.CUSRID = tab.CUSRID;
                    tab.LMUSRID = tab.LMUSRID;
                    tab.DISPSTATUS = 0;
                    tab.PRCSDATE = tab.PRCSDATE;
                    gatein.PRCSDATE = DateTime.Now;

                    context.gateindetails.Add(gatein);
                    context.SaveChanges();

                    tab = context.vehicleticketdetail.Find(VTDID);
                    context.Entry(tab).Entity.EGIDID = gatein.GIDID;
                    context.SaveChanges();
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
            //code commented by rajesh on 21/jul/21 as requested by SANCO Team <s>
            ////code added to Autopopulate the Vehicle Ticket Information to Gate Out Details - By Rajesh S on 17-Jul-2021 <Start>
            ///* ----- GATE OUT INSERT -----*/
            //var et = new SCFSERPEntities();
            //var data = et.pr_AutoPopulate_VT_to_GO(tab.VTDID, Convert.ToInt32(Session["compyid"]));
            ////code added to Autopopulate the Vehicle Ticket Information to Gate Out Details - By Rajesh S on 17-Jul-2021 <End>
            //code commented by rajesh on 21/jul/21 as requested by SANCO Team <e>
            Response.Redirect("Index");
        }//End of savedata

        
        public JsonResult GetContDetails(int id)
        {
            var data = context.Database.SqlQuery<VW_NONPNR_VEHICLETICKET_CONTAINER_CTRL_ASSGN>("SELECT * FROM VW_NONPNR_VEHICLETICKET_CONTAINER_CTRL_ASSGN WHERE ASLDID=" + id + "").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #region SlipMaxDate
        public JsonResult SlipMaxDate(string id)
        {
            int ASLDID = 0;

            if(id!=""||id!=null)
            {
                ASLDID = Convert.ToInt32(id);
            }
            else { ASLDID = 0; }

            var data = (from q in context.authorizatioslipmaster
                        join b in context.authorizationslipdetail on q.ASLMID equals b.ASLMID
                        where b.ASLDID == ASLDID && q.SDPTID == 9
                        group q by q.ASLMDATE into g
                        select new { ASLMDATE = g.Max(t => t.ASLMDATE) }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public JsonResult Getcont(string id)
        {
            var param = id.Split(';');
            var data = context.Database.SqlQuery<VW_VEHICLETICKET_NONPNR_CONTAINER_CBX_ASSGN>("SELECT * FROM VW_VEHICLETICKET_NONPNR_CONTAINER_CBX_ASSGN WHERE IGMNO='" + param[0] + "' and GPLNO='" + param[1] + "'").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "NonPnrVehicleTicketPrint")]
        public void PrintView(int? id = 0)
        {

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "NonPnr_vehicle_ticket", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;


                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "NonPnr_VT.RPT");

                cryRpt.RecordSelectionFormula = "{VW_NONPNR_VT_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_NONPNR_VT_CRY_PRINT_ASSGN.VTDID} = " + id;



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

        [Authorize(Roles = "NonPnrVehicleTicketPrint")]
        public void QRCPrintView(int? id = 0)
        {

            GenerateQRCodeFile(id);

            string QRpath = Server.MapPath("~/NVTQRCode/");
            string VTQRpath = QRpath + id.ToString() + ".png";

            //  ........delete TMPRPT...//
            if (System.IO.File.Exists(VTQRpath))
            {
                context.Database.ExecuteSqlCommand("Update VEHICLETICKETDETAIL Set QRCDIMGPATH = '" + VTQRpath + "' WHERE VTDID =" + id);

                context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
                var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "NonPnr_vehicle_ticket", Convert.ToInt32(id), Session["CUSRID"].ToString());
                if (TMPRPT_IDS == "Successfully Added")
                {
                    ReportDocument cryRpt = new ReportDocument();
                    TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                    TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                    ConnectionInfo crConnectionInfo = new ConnectionInfo();
                    Tables CrTables;


                    cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "NonPnr_VT_QrCode.RPT");

                    cryRpt.RecordSelectionFormula = "{VW_NONPNR_VT_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_NONPNR_VT_CRY_PRINT_ASSGN.VTDID} = " + id;



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
                else
                {
                    Response.Write("Error in QR Code Generation...");
                }
            }


        }

        [Authorize(Roles = "NonPnrVehicleTicketDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            var param = id.Split('-'); var vtid = 0; var gidid = 0;
            if (param[0] != "0" && param[0] != "" && param[0] != null)
                vtid = Convert.ToInt32(param[0]);
            if (param[3] != "0" && param[3] != "" && param[3] != null)
                gidid = Convert.ToInt32(param[3]); 
            
            String temp = Delete_fun.delete_check1(fld, param[2]);
            if (temp.Equals("PROCEED"))
            {
                VehicleTicketDetail vehicleticketdetail = context.vehicleticketdetail.Find(vtid);
                context.vehicleticketdetail.Remove(vehicleticketdetail);
                if (gidid != 0)
                {
                    GateInDetail gateindetails = context.gateindetails.Find(gidid);
                    context.gateindetails.Remove(gateindetails);
                }
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        public void DOCheck()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");

            String temp = Delete_fun.delete_check1(fld, id);
            
            Response.Write(temp);
        }

        #region QrCode Gneration
        public void GenerateQRCodeFile(int? id = 0)
        {

            string barcodePath = Server.MapPath("~/NVTQRCode/" + id.ToString() + ".png");
            var result = context.Database.SqlQuery<pr_Get_Non_PNR_VT_Details_For_QRCode_Result>("exec pr_Get_Non_PNR_VT_Details_For_QRCode @PVTDID=" + id).ToList();//........procedure  for edit mode details data
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

    }
}