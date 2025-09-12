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
    public class NonPnrGateInController : Controller
    {
        // GET: NonPnrGateIn

        #region contextdeclaration
        SCFSERPContext context = new SCFSERPContext();

        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        #endregion

        #region IndexForm
        [Authorize(Roles = "NonPnrGateInIndex")]
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

            return View(context.gateindetails.Where(x => x.GIDATE >= sd).Where(x => x.GIDATE <= ed).Where(x => x.SDPTID == 9).Where(x => x.CONTNRID >= 1).ToList());

            //return View();
        }
        #endregion

        #region GetAjaxData
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {

            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_NonPnr_GateInGridAssgn(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]));

                var aaData = data.Select(d => new string[] { d.GIDATE.Value.ToString("dd/MM/yyyy"), d.GIDNO, d.CONTNRNO, d.CONTNRSID, d.IGMNO, d.GPLNO, d.IMPRTNAME, d.STMRNAME, d.PRDTDESC, d.DISPSTATUS, d.GIDID.ToString() }).ToArray();

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

        #region FormModify
        [Authorize(Roles = "NonPnrGateInEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            //string url = "" + strPath + "/NonPnrGateIn/Form/" + id;

            Response.Redirect("" + strPath + "/NonPnrGateIn/Form/" + id);

            //Response.Redirect("/NonPnrGateIn/Form/" + id);
        }
        #endregion

        #region ViewForm
        //[Authorize(Roles = "NonPnrGateInCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            RemoteGateIn remotegatein = new RemoteGateIn();
            GateInDetail tab = new GateInDetail();
            tab.GITIME = DateTime.Now;
            tab.GIDATE = Convert.ToDateTime(DateTime.Now).Date;
            tab.GITIME = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);

            tab.GICCTLTIME = DateTime.Now;
            tab.IGMDATE = DateTime.Now.Date;

            //-------------------Dropdown List--------------------------------------------------//
            //ViewBag.ROWID = new SelectList(context.rowmasters.Where(x => x.DISPSTATUS == 0), "ROWID", "ROWDESC");
            ViewBag.SLOTID = new SelectList(context.slotmasters.Where(x => x.DISPSTATUS == 0), "SLOTID", "SLOTDESC");
            ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");
            ViewBag.GPPTYPE = new SelectList(context.porttypemaster, "GPPTYPE", "GPPTYPEDESC", tab.GPPTYPE);
            ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CONTNRTDESC), "CONTNRTID", "CONTNRTDESC");
            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.DISPSTATUS == 0 && x.CONTNRSID > 1), "CONTNRSID", "CONTNRSDESC");
            ViewBag.TRNSPRTNAME = new SelectList(context.categorymasters.Where(m => m.CATETID == 5 && m.DISPSTATUS == 0).OrderBy(m => m.CATENAME), "CATEID", "CATENAME");
            //Code modified to filter only DPDDPD option [from NPNR] - By Rajesh on 21-07-21
            ViewBag.GPMODEID = new SelectList(context.gpmodemasters.Where(x => x.DISPSTATUS == 0 && x.GPMODECODE == "DPDDPD").OrderBy(x => x.GPMODEDESC), "GPMODEID", "GPMODEDESC");

            //---------------------port--------------------------------------------------//   
            //------------------Escord---------------//

            List<SelectListItem> selectedGPETYPE = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "NO", Value = "0", Selected = false };
            selectedGPETYPE.Add(selectedItem);
            selectedItem = new SelectListItem
            {
                Text = "YES",
                Value = "1",
                Selected = false
            };
            selectedGPETYPE.Add(selectedItem);
            ViewBag.GPETYPE = selectedGPETYPE;
            // ------------------S.Amend-----------//

            List<SelectListItem> selectedGPSTYPE = new List<SelectListItem>();
            SelectListItem selectedItemst = new SelectListItem { Text = "NO", Value = "0", Selected = false };
            selectedGPSTYPE.Add(selectedItemst);
            selectedItemst = new SelectListItem { Text = "YES", Value = "1", Selected = false };
            selectedGPSTYPE.Add(selectedItemst);
            ViewBag.GPSTYPE = selectedGPSTYPE;

            //  ------------------Weightment--------------//
            List<SelectListItem> selectedGPWTYPE = new List<SelectListItem>();
            SelectListItem selectedItemsts = new SelectListItem { Text = "NO", Value = "0", Selected = false };
            selectedGPWTYPE.Add(selectedItemsts);
            selectedItemsts = new SelectListItem { Text = "YES", Value = "1", Selected = true };
            selectedGPWTYPE.Add(selectedItemsts);
            ViewBag.GPWTYPE = selectedGPWTYPE;
            //---------------scanned----------------------------
            List<SelectListItem> selectedGPSCNTYPE = new List<SelectListItem>();
            SelectListItem selectedItemsts1 = new SelectListItem { Text = "NO", Value = "0", Selected = true };
            selectedGPSCNTYPE.Add(selectedItemsts1);
            selectedItemsts1 = new SelectListItem { Text = "YES", Value = "1", Selected = false };
            selectedGPSCNTYPE.Add(selectedItemsts1);
            ViewBag.GPSCNTYPE = selectedGPSCNTYPE;
            //---------------
            // -----------------------FCL------------------//

            List<SelectListItem> selectedGFCLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemstf = new SelectListItem { Text = "LCL", Value = "0", Selected = false };
            selectedGFCLTYPE.Add(selectedItemstf);
            selectedItemstf = new SelectListItem { Text = "FCL", Value = "1", Selected = true };
            selectedGFCLTYPE.Add(selectedItemstf);
            ViewBag.GFCLTYPE = selectedGFCLTYPE;


            // ------------------Reefer Container Plug in-----------//

            List<SelectListItem> selectedGPRefer_List = new List<SelectListItem>();
            SelectListItem selectedGPRefer = new SelectListItem { Text = "NO", Value = "1", Selected = false };
            selectedGPRefer_List.Add(selectedGPRefer);
            selectedGPRefer = new SelectListItem { Text = "YES", Value = "2", Selected = false };
            selectedGPRefer_List.Add(selectedGPRefer);
            ViewBag.GRADEID = selectedGPRefer_List;

            //----------------------------------End----------------------------------------

            if (id != 0)//--Edit Mode
            {
                tab = context.gateindetails.Find(id);
                

                //----------------------selected values in dropdown list------------------------------------//
                ViewBag.SLOTID = new SelectList(context.slotmasters.Where(x => x.DISPSTATUS == 0), "SLOTID", "SLOTDESC", tab.SLOTID);
                //ViewBag.ROWID = new SelectList(context.rowmasters.Where(x => x.DISPSTATUS == 0), "ROWID", "ROWDESC", tab.ROWID);
                ViewBag.GPPTYPE = new SelectList(context.porttypemaster, "GPPTYPE", "GPPTYPEDESC", tab.GPPTYPE);               
                ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", tab.PRDTGID);
                ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CONTNRTDESC), "CONTNRTID", "CONTNRTDESC", tab.CONTNRTID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.DISPSTATUS == 0 && x.CONTNRSID > 1), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.TRNSPRTNAME = new SelectList(context.categorymasters.Where(m => m.CATETID == 5 && m.DISPSTATUS == 0).OrderBy(m => m.CATENAME), "CATEID", "CATENAME", tab.TRNSPRTNAME);
                //Code Modified to filter only NPNR option - By Rajesh on 16-Jul-2021
                //Code modified to filter only DPDDPD option - By Rajesh on 21-07-21
                ViewBag.GPMODEID = new SelectList(context.gpmodemasters.Where(x => x.DISPSTATUS == 0 && x.GPMODECODE == "DPDDPD").OrderBy(x => x.GPMODEDESC), "GPMODEID", "GPMODEDESC", tab.GPMODEID);


                //-------------------------------escord
                List<SelectListItem> selectedGPETYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GPETYPE) == 0)
                {
                    SelectListItem selectedItem1 = new SelectListItem { Text = "NO", Value = "0", Selected = true };
                    selectedGPETYPE1.Add(selectedItem1);
                    selectedItem1 = new SelectListItem { Text = "YES", Value = "1", Selected = false };
                    selectedGPETYPE1.Add(selectedItem1);
                }
                else
                {
                    SelectListItem selectedItem1 = new SelectListItem { Text = "NO", Value = "0", Selected = false };
                    selectedGPETYPE1.Add(selectedItem1);
                    selectedItem1 = new SelectListItem { Text = "YES", Value = "1", Selected = true };
                    selectedGPETYPE1.Add(selectedItem1);
                }
                ViewBag.GPETYPE = selectedGPETYPE1;

                //------End---

                //---------------scanned----------------------------
                List<SelectListItem> selectedGPSCNTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GPSCNTYPE) == 0)
                {
                    SelectListItem selectedItemsts11 = new SelectListItem { Text = "NO", Value = "0", Selected = true };
                    selectedGPSCNTYPE1.Add(selectedItemsts11);
                    selectedItemsts11 = new SelectListItem { Text = "YES", Value = "1", Selected = false };
                    selectedGPSCNTYPE1.Add(selectedItemsts11);

                }
                else
                {
                    SelectListItem selectedItemsts11 = new SelectListItem { Text = "NO", Value = "0", Selected = false };
                    selectedGPSCNTYPE1.Add(selectedItemsts11);
                    selectedItemsts11 = new SelectListItem { Text = "YES", Value = "1", Selected = true };
                    selectedGPSCNTYPE1.Add(selectedItemsts11);
                }
                ViewBag.GPSCNTYPE = selectedGPSCNTYPE1;
                //-----------End----------



                // ------------------S.Amend-----------//
                List<SelectListItem> selectedGPSTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GPSTYPE) == 0)
                {
                    SelectListItem selectedItemst1 = new SelectListItem { Text = "NO", Value = "0", Selected = true };
                    selectedGPSTYPE1.Add(selectedItemst1);
                    selectedItemst1 = new SelectListItem { Text = "YES", Value = "1", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemst1);
                }
                else
                {
                    SelectListItem selectedItemst1 = new SelectListItem { Text = "NO", Value = "0", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemst1);
                    selectedItemst1 = new SelectListItem { Text = "YES", Value = "1", Selected = true };
                    selectedGPSTYPE1.Add(selectedItemst1);
                }
                ViewBag.GPSTYPE = selectedGPSTYPE1;


                //  ------------------Weightment--------------//
                List<SelectListItem> selectedGPWTYPE2 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GPWTYPE) == 0)
                {
                    SelectListItem selectedItemsts2 = new SelectListItem { Text = "NO", Value = "0", Selected = true };
                    selectedGPWTYPE2.Add(selectedItemsts2);
                    selectedItemsts2 = new SelectListItem { Text = "YES", Value = "1", Selected = false };
                    selectedGPWTYPE2.Add(selectedItemsts2);
                    ViewBag.GPWTYPE = selectedGPWTYPE2;
                }
                else
                {
                    SelectListItem selectedItemsts2 = new SelectListItem { Text = "NO", Value = "0", Selected = false };
                    selectedGPWTYPE2.Add(selectedItemsts2);
                    selectedItemsts2 = new SelectListItem { Text = "YES", Value = "1", Selected = true };
                    selectedGPWTYPE2.Add(selectedItemsts2);
                    ViewBag.GPWTYPE = selectedGPWTYPE2;
                }
                ViewBag.GPWTYPE = selectedGPWTYPE2;
                // -----------------------FCL------------------//
                List<SelectListItem> selectedGFCLTYPE3 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GFCLTYPE) == 0)
                {
                    SelectListItem selectedItemstf3 = new SelectListItem { Text = "LCL", Value = "0", Selected = true };
                    selectedGFCLTYPE3.Add(selectedItemstf3);
                    selectedItemstf3 = new SelectListItem { Text = "FCL", Value = "1", Selected = false };
                    selectedGFCLTYPE3.Add(selectedItemstf3);

                }
                else
                {
                    SelectListItem selectedItemstf3 = new SelectListItem { Text = "LCL", Value = "0", Selected = false };
                    selectedGFCLTYPE3.Add(selectedItemstf3);
                    selectedItemstf3 = new SelectListItem { Text = "FCL", Value = "1", Selected = true };
                    selectedGFCLTYPE3.Add(selectedItemstf3);
                }
                ViewBag.GFCLTYPE = selectedGFCLTYPE3;


                List<SelectListItem> selectedGPRefer_List1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GRADEID) == 1)
                {
                    SelectListItem selectedGPRefer1 = new SelectListItem { Text = "NO", Value = "1", Selected = true };
                    selectedGPRefer_List1.Add(selectedGPRefer1);
                    selectedGPRefer1 = new SelectListItem { Text = "YES", Value = "2", Selected = false };
                    selectedGPRefer_List1.Add(selectedGPRefer1);
                }
                else if (Convert.ToInt32(tab.GRADEID) == 2)
                {
                    SelectListItem selectedGPRefer1 = new SelectListItem { Text = "NO", Value = "1", Selected = false };
                    selectedGPRefer_List1.Add(selectedGPRefer1);
                    selectedGPRefer1 = new SelectListItem { Text = "YES", Value = "2", Selected = true };
                    selectedGPRefer_List1.Add(selectedGPRefer1);
                }
                else
                {
                    SelectListItem selectedGPRefer1 = new SelectListItem { Text = "NO", Value = "1", Selected = false };
                    selectedGPRefer_List1.Add(selectedGPRefer1);
                    selectedGPRefer1 = new SelectListItem { Text = "YES", Value = "2", Selected = false };
                    selectedGPRefer_List1.Add(selectedGPRefer1);
                }
                ViewBag.GRADEID = selectedGPRefer_List1;

            }


            return View(tab);
        }
        #endregion

        #region Savedata
        [HttpPost]
        public void savendata(GateInDetail tab)
        {
            var R_GIDID = Request.Form.Get("R_GIDID");

            string todaydt = Convert.ToString(DateTime.Now);
            string todayd = Convert.ToString(DateTime.Now.Date);

            tab.PRCSDATE = DateTime.Now;
            //tab.GIDATE = Convert.ToDateTime(tab.GIDATE).Date;
            //tab.GITIME = Convert.ToDateTime(tab.GITIME);
            //tab.GITIME = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);

            //string[] gidt = tab.GIDATE.ToString("dd-MM-yyyy").Split('-');
            //string[] gitm = tab.GITIME.ToString("HH:mm:ss").Split(':');
            //string gidttm = gidt[2] + "-" + gidt[1] + "-" + gidt[0] + " " + gitm[0] + ":" + gitm[1] + ":" + gitm[2]+"}";
            //tab.GITIME = Convert.ToDateTime(gidttm).Date;
            tab.GICCTLDATE = Convert.ToDateTime(tab.GICCTLTIME).Date;
            tab.CONTNRID = 1;
            tab.YRDID = 1;
            tab.COMPYID = Convert.ToInt32(Session["compyid"]);
            tab.SDPTID = 9;
            tab.AVHLNO = tab.VHLNO;
            tab.ESBDATE = DateTime.Now;
            tab.DISPSTATUS = tab.DISPSTATUS;
            tab.LMUSRID = Session["CUSRID"].ToString();

            string indate = Convert.ToString(tab.GIDATE);
            if (indate != null || indate != "")
            {
                tab.GIDATE = Convert.ToDateTime(indate).Date;
            }
            else { tab.GIDATE = DateTime.Now.Date; }

            if (tab.GIDATE > Convert.ToDateTime(todayd))
            {
                tab.GIDATE = Convert.ToDateTime(todayd);
            }

            string intime = Convert.ToString(tab.GITIME);
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

                        tab.GITIME = Convert.ToDateTime(in_datetime);
                    }
                    else { tab.GITIME = DateTime.Now; }
                }
                else { tab.GITIME = DateTime.Now; }
            }
            else { tab.GITIME = DateTime.Now; }

            if (tab.GITIME > Convert.ToDateTime(todaydt))
            {
                tab.GITIME = Convert.ToDateTime(todaydt);
            }
           
            tab.BOEDATE = Convert.ToDateTime(tab.BOEDATE).Date;

            if (tab.GIDID.ToString() != "0")
            {
                tab.CUSRID = Session["CUSRID"].ToString();
                context.Entry(tab).Entity.NGIDID = tab.GIDID + 1;
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            else
            {

                //----------------first record------------------//              

                tab.GINO = Convert.ToInt32(Autonumber.autonum("gateindetail", "GINO", "GINO <> 0 AND SDPTID = 9 and compyid = " + Convert.ToInt32(Session["compyid"]) + "").ToString());
                int ano = tab.GINO;
                string prfx = string.Format("{0:D5}", ano);
                tab.GIDNO = prfx.ToString();


                if (R_GIDID != null)
                    tab.RGIDID = Convert.ToInt32(R_GIDID);
               
                context.gateindetails.Add(tab);
                context.SaveChanges();
                context.Entry(tab).Entity.NGIDID = tab.GIDID + 1;
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                ///*/.....delete remote gate in*/
                if (R_GIDID != "0")
                {
                    RemoteGateIn remotegatein = context.remotegateindetails.Find(Convert.ToInt32(R_GIDID));
                    remotegatein.GIDID = tab.GIDID;

                    //context.remotegateindetails.Remove(remotegatein);
                    //context.SaveChanges();

                }/*end*/

                ////-----second record-----------------------------// 
                //tab.GITIME = tab.GITIME;
                //tab.TRNSPRTNAME = tab.TRNSPRTNAME;
                //tab.AVHLNO = tab.VHLNO;
                //tab.DRVNAME = tab.DRVNAME;
                //tab.GPREFNO = tab.GPREFNO;
                //tab.IMPRTID = 0;
                //tab.IMPRTNAME = "-";
                //tab.STMRID = 0;
                //tab.STMRNAME = "-";
                //tab.CONDTNID = 0;
                //tab.CONTNRNO = "-";
                //tab.CONTNRTID = 0;
                //tab.CONTNRID = 0;
                //tab.CONTNRSID = 0;
                //tab.LPSEALNO = "-";
                //tab.CSEALNO = "-";
                //tab.YRDID = 0;
                //tab.VSLID = 0;
                //tab.VSLNAME = "-";
                //tab.VOYNO = "-";
                //tab.PRDTGID = 0;
                //tab.PRDTDESC = "-";
                //tab.UNITID = 0;
                //tab.GPLNO = "-";
                //tab.GPWGHT = 0;
                //tab.GPEAMT = 0;
                //tab.GPAAMT = 0;
                //tab.IGMNO = tab.IGMNO;
                //tab.GIISOCODE = tab.GIISOCODE;
                //tab.GIDMGDESC = tab.GIDMGDESC;
                //tab.GPWTYPE = 0;
                //tab.GPSTYPE = 0;
                //tab.GPETYPE = 0;
                ////tab.ROWID = 0;
                //tab.SLOTID = 0;
                //tab.GIVHLTYPE = 0;
                //tab.TRNSPRTID = 0;
                //tab.NGIDID = 0;
                //tab.BOEDID = tab.GIDID;
                //tab.BLNO = tab.BLNO;
                //tab.BOENO = tab.BOENO;
                //tab.CFSNAME = tab.CFSNAME;

                //var GINO = Request.Form.Get("GINO");

                ////if (context.gateindetails.Where(u => u.GINO == Convert.ToInt32(GINO)).Count()!=0) 
                //tab.GINO = Convert.ToInt32(Autonumber.autonum("gateindetail", "GINO", "GINO <> 0 AND SDPTID = 9 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                ////  else tab.GINO = 1;
                //int anoo = tab.GINO;
                //string prfxx = string.Format("{0:D5}", anoo);
                //tab.GIDNO = prfxx.ToString();
                //context.gateindetails.Add(tab);
                //context.SaveChanges();

            }

            Response.Write("saved");
            //Response.Redirect("Index");
        }

        public void savedata(GateInDetail tab)
        {
            var R_GIDID = Request.Form.Get("R_GIDID");

            string todaydt = Convert.ToString(DateTime.Now);
            string todayd = Convert.ToString(DateTime.Now.Date);

            tab.PRCSDATE = DateTime.Now;
            //tab.GIDATE = Convert.ToDateTime(tab.GIDATE).Date;
            //string[] gidt = tab.GIDATE.ToString("dd-MM-yyyy").Split('-');
            //string[] gitm = tab.GITIME.ToString("HH:mm:ss").Split(':');
            //string gidttm = "{" + gidt[2] + "-" + gidt[1] + "-" + gidt[0] + " " + gitm[0] + ":" + gitm[1] + ":" + gitm[2] + "}";
            //tab.GITIME = Convert.ToDateTime(gidttm).Date;
            //tab.GIDATE = Convert.ToDateTime(tab.GIDATE).Date;
            //tab.GITIME = Convert.ToDateTime(tab.GITIME);
            //tab.GITIME = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);

            tab.GICCTLDATE = Convert.ToDateTime(tab.GICCTLTIME).Date;
            tab.CONTNRID = 1;
            tab.YRDID = 1;
            tab.COMPYID = Convert.ToInt32(Session["compyid"]);
            tab.SDPTID = 9;
            tab.AVHLNO = tab.VHLNO;
            tab.ESBDATE = DateTime.Now;
            tab.DISPSTATUS = tab.DISPSTATUS;
            tab.LMUSRID = Session["CUSRID"].ToString();

            string indate = Convert.ToString(tab.GIDATE);
            if (indate != null || indate != "")
            {
                tab.GIDATE = Convert.ToDateTime(indate).Date;
            }
            else { tab.GIDATE = DateTime.Now.Date; }

            if (tab.GIDATE > Convert.ToDateTime(todayd))
            {
                tab.GIDATE = Convert.ToDateTime(todayd);
            }

            string intime = Convert.ToString(tab.GITIME);
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

                        tab.GITIME = Convert.ToDateTime(in_datetime);
                    }
                    else { tab.GITIME = DateTime.Now; }
                }
                else { tab.GITIME = DateTime.Now; }
            }
            else { tab.GITIME = DateTime.Now; }

            if (tab.GITIME > Convert.ToDateTime(todaydt))
            {
                tab.GITIME = Convert.ToDateTime(todaydt);
            }

            tab.BOEDATE = Convert.ToDateTime(tab.BOEDATE).Date;

            if (tab.GIDID.ToString() != "0")
            {
                tab.CUSRID = Session["CUSRID"].ToString();

                context.Entry(tab).Entity.NGIDID = tab.GIDID + 1;
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            else
            {

                //----------------first record------------------//              

                tab.GINO = Convert.ToInt32(Autonumber.autonum("gateindetail", "GINO", "GINO <> 0 AND SDPTID = 9 and compyid = " + Convert.ToInt32(Session["compyid"]) + "").ToString());
                int ano = tab.GINO;
                string prfx = string.Format("{0:D5}", ano);
                tab.GIDNO = prfx.ToString();


                if (R_GIDID != null)
                    tab.RGIDID = Convert.ToInt32(R_GIDID);
                //    tab.CUSRID = Session["CUSRID"].ToString();
                //else tab.CUSRID = "0";
                context.gateindetails.Add(tab);
                context.SaveChanges();
                context.Entry(tab).Entity.NGIDID = tab.GIDID + 1;
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                ///*/.....delete remote gate in*/
                if (R_GIDID != "0")
                {
                    RemoteGateIn remotegatein = context.remotegateindetails.Find(Convert.ToInt32(R_GIDID));
                    remotegatein.GIDID = tab.GIDID;

                    //context.remotegateindetails.Remove(remotegatein);
                    //context.SaveChanges();

                }/*end*/

                ////-----second record-----------------------------// 
                //tab.GITIME = tab.GITIME;
                //tab.TRNSPRTNAME = tab.TRNSPRTNAME;
                //tab.AVHLNO = tab.VHLNO;
                //tab.DRVNAME = tab.DRVNAME;
                //tab.GPREFNO = tab.GPREFNO;
                //tab.IMPRTID = 0;
                //tab.IMPRTNAME = "-";
                //tab.STMRID = 0;
                //tab.STMRNAME = "-";
                //tab.CONDTNID = 0;
                //tab.CONTNRNO = "-";
                //tab.CONTNRTID = 0;
                //tab.CONTNRID = 0;
                //tab.CONTNRSID = 0;
                //tab.LPSEALNO = "-";
                //tab.CSEALNO = "-";
                //tab.YRDID = 0;
                //tab.VSLID = 0;
                //tab.VSLNAME = "-";
                //tab.VOYNO = "-";
                //tab.PRDTGID = 0;
                //tab.PRDTDESC = "-";
                //tab.UNITID = 0;
                //tab.GPLNO = "-";
                //tab.GPWGHT = 0;
                //tab.GPEAMT = 0;
                //tab.GPAAMT = 0;
                //tab.IGMNO = tab.IGMNO;
                //tab.GIISOCODE = tab.GIISOCODE;
                //tab.GIDMGDESC = tab.GIDMGDESC;
                //tab.GPWTYPE = 0;
                //tab.GPSTYPE = 0;
                //tab.GPETYPE = 0;
                ////tab.ROWID = 0;
                //tab.SLOTID = 0;                
                //tab.GIVHLTYPE = 0;
                //tab.TRNSPRTID = 0;               
                //tab.NGIDID = 0;
                //tab.BOEDID = tab.GIDID;
                //tab.BLNO = tab.BLNO;
                //tab.BOENO = tab.BOENO;
                //tab.CFSNAME = tab.CFSNAME;

                //var GINO = Request.Form.Get("GINO");

                ////if (context.gateindetails.Where(u => u.GINO == Convert.ToInt32(GINO)).Count()!=0) 
                //tab.GINO = Convert.ToInt32(Autonumber.autonum("gateindetail", "GINO", "GINO <> 0 AND SDPTID = 9 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                ////  else tab.GINO = 1;
                //int anoo = tab.GINO;
                //string prfxx = string.Format("{0:D5}", anoo);
                //tab.GIDNO = prfxx.ToString();
                //context.gateindetails.Add(tab);
                //context.SaveChanges();

            }

            Response.Redirect("Index");

        }
        #endregion

        #region ContainerDuplicate Check
        public void CONT_Duplicate_Check(string VOYNO, string GPLNO, string IGMNO, string CONTNRNO, string date)
        {
            VOYNO = Request.Form.Get("VOYNO");
            GPLNO = Request.Form.Get("GPLNO");
            IGMNO = Request.Form.Get("IGMNO");
            CONTNRNO = Request.Form.Get("CONTNRNO");
            date = Request.Form.Get("date");

            
            string temp = ContainerNo_Check.nonpnrrecordCount(VOYNO, IGMNO, GPLNO, CONTNRNO);
            if (temp != "PROCEED")
            {
                Response.Write("Container number already exists");

            }
            else
            {
                var query = context.Database.SqlQuery<PR_GATEIN_CONTAINER_CHK_ASSGN_Result>("Exec PR_GATEIN_CONTAINER_CHK_ASSGN @PCONTNRNO='" + CONTNRNO + "'").ToList();
                if (query.Count > 0)
                {
                    DateTime gedate = Convert.ToDateTime(query[0].GEDATE);
                    DateTime gidate = Convert.ToDateTime(date);
                    var s = (gedate - gidate).Days;
                    //   Response.Write(s);
                    //if (s != 0)
                    //{
                    //    Response.Write("DATE INCORRECT");
                    //}
                    if (gidate >= gedate)
                    {
                        Response.Write("PROCEED");
                    }
                    else
                    {
                        Response.Write("DATE INCORRECT");
                    }
                    //if (s < 10)
                    //{
                    //    Response.Write("DATE INCORRECT");
                    //}
                }
                else
                {
                    Response.Write("PROCEED");
                }
            }

        }

        #endregion

        

        #region  CheckAmt
        public void CheckAmt()
        {
            string vno = Request.Form.Get("vno");

            string amt = Request.Form.Get("amt");

            if (Convert.ToInt16(vno) == 1 && Convert.ToDecimal(amt) == 0)
            {
                Response.Write("Escord Amount is Required");

            }
            else
            {

            }

        }
        #endregion

        #region Autocomplete Vessel Name        
        public JsonResult AutoVessel(string term)
        {
            var result = (from vessel in context.vesselmasters
                          where vessel.VSLDESC.ToLower().Contains(term.ToLower())
                          select new { vessel.VSLDESC, vessel.VSLID }).Distinct();
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

        #region Autocomplete Steamer Name    
        public JsonResult AutoSteamer(string term)
        {
            var result = (from category in context.categorymasters.Where(m => m.CATETID == 3).Where(x => x.DISPSTATUS == 0)
                          where category.CATENAME.ToLower().Contains(term.ToLower())
                          select new { category.CATENAME, category.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Importer Name          
        public JsonResult AutoImpoter(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 1).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete CHA Name  
        public JsonResult AutoChaname(string term)
        {
            var result = (from r in context.categorymasters.Where(x => x.CATETID == 4 && x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).OrderBy(x => x.CATENAME).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Billing CHA Name  
        public JsonResult AutoBChaname(string term)
        {
            //var result = (from r in context.categorymasters.Where(x => (x.CATETID == 4) && x.DISPSTATUS == 0)
            //              where r.CATENAME.ToLower().Contains(term.ToLower())
            //              select new { r.CATENAME, r.CATEID }).OrderBy(x => x.CATENAME).Distinct();
            var e = new SCFSERPEntities();
            var result = e.pr_Fetch_CHAIMP_Dtl(4,term.ToString());

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Slot Name  
        public JsonResult AutoSlot(string term)
        {
            var result = (from slot in context.slotmasters.Where(x => x.DISPSTATUS == 0)
                          where slot.SLOTDESC.ToLower().Contains(term.ToLower())
                          select new { slot.SLOTID, slot.SLOTDESC }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete cascadingS dropdown          
        public JsonResult GetSlot(int id)
        {
            var slot = (from a in context.slotmasters.Where(x => x.DISPSTATUS == 0) where a.SLOTID == id select a).ToList();
            return new JsonResult() { Data = slot, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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

        #region Autocomplete Vehicle PNR On  Id  
        public JsonResult AutoVehicleNo(string term)
        {
            string vno = ""; int Tid = 0;
            var Param = term.Split(';');

            if (Param[0] != "" || Param[0] != null) { vno = Convert.ToString(Param[0]); } else { vno = ""; }
            if (Param[1] != "" || Param[1] != null) { Tid = Convert.ToInt32(Param[1]); } else { Tid = 0; }


            var result = (from vehicle in context.vehiclemasters.Where(m => m.DISPSTATUS == 0 && m.TRNSPRTID == Tid)
                          where vehicle.VHLMDESC.ToLower().Contains(vno.ToLower())
                          select new { vehicle.VHLMDESC, vehicle.VHLMID }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion
        
        #region Autocomplete checkpostdata Name  
        public ActionResult checkpostdata(int? id = 0)
        {
            String idx = Request.Form.Get("id");

            RemoteGateIn remotegateindetails = context.remotegateindetails.Find(idx);
            if (remotegateindetails == null)
                return HttpNotFound();
            else
                return View(remotegateindetails);
        }
        #endregion

        //--------Autocomplete CHA Name
        public JsonResult NewAutoCha(string term)
        {

            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID, r.CATEBGSTNO }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //cha and importer

        public JsonResult NewAutoImporter(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 1).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID, r.CATEBGSTNO }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region test1 
        public ActionResult test1()
        {
            String idx = Request.Form.Get("id");
            GateInDetail tab = new GateInDetail();
            RemoteGateIn remote = new RemoteGateIn();
            remote = context.remotegateindetails.Find(idx);
            return View();
        }
        #endregion

        #region PrintView
        [Authorize(Roles = "NonPnrGateInPrint")]
        public void PrintView(int? id = 0)
        {
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "NONPNRGATEIN", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "NonPnr_GateIn.rpt");
                cryRpt.RecordSelectionFormula = "{VW_NONPNR_GATE_IN_PRINT_ASSGN.KUSRID} ='" + Session["CUSRID"].ToString() + "' and {VW_NONPNR_GATE_IN_PRINT_ASSGN.GIDID} =" + id;

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


        [Authorize(Roles = "NonPnrGateInPrint")]
        public void TPrintView(int? id = 0)/*truck*/
        {
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "NONPNRTRUCKIN", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                //cryRpt.Load("D:\\scfsreports\\NonPnr_TruckIn.rpt");
                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "NonPnr_GateIn.rpt");
                cryRpt.RecordSelectionFormula = "{VW_NONPNR_GATE_IN_PRINT_ASSGN.KUSRID} ='" + Session["CUSRID"].ToString() + "' and {VW_NONPNR_GATE_IN_PRINT_ASSGN.GIDID} =" + id;

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
        #endregion

        #region DeleteGateIn        
        [Authorize(Roles = "NonPnrGateInDelete")]
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
                var sql = context.Database.SqlQuery<int>("SELECT GIDID from GATEINDETAIL where GIDID=" + Convert.ToInt32(id)).ToList();
                var gidid = (sql[0]).ToString();
                GateInDetail gateindetails = context.gateindetails.Find(Convert.ToInt32(gidid));
                context.gateindetails.Remove(gateindetails);
                context.SaveChanges();

                Response.Write("Deleted Successfully ...");
            }
            else
            {
                // Code Modified for validating the by Rajesh / Yamuna on 16-Jul-2021 <Start>
                Response.Write("Record already exists, deletion is not possible!");
                // Code Modified for validating the by Rajesh / Yamuna on 16-Jul-2021 <End>
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

            context.Database.CommandTimeout = 0;
            var result = context.Database.SqlQuery<PR_IMPORT_DASHBOARD_DETAILS_Result>("EXEC PR_NONPNR_DASHBOARD_DETAILS @PFDT='" + fdate + "',@PTDT='" + tdate + "',@PSDPTID=" +9).ToList();

            foreach (var rslt in result)
            {
                if ((rslt.Sno == 1) && (rslt.Descriptn == "IMPORT - GATEIN"))
                {
                    @ViewBag.Total20 = rslt.c_20;
                    @ViewBag.Total40 = rslt.c_40;
                    @ViewBag.Total45 = rslt.c_45;
                    @ViewBag.TotalTues = rslt.c_tues;

                    Session["GI20"] = rslt.c_20;
                    Session["GI40"] = rslt.c_40;
                    Session["GI45"] = rslt.c_45;
                    Session["GITU"] = rslt.c_tues;
                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

    }
}