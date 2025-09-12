using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using scfs.Data;
using scfs_erp;
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

namespace scfs.Controllers.ShutOut
{
    [SessionExpire]
    [Authorize]
    public class ShutoutCargoController : Controller
    {
        // GET: ShutoutCargo
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

        [Authorize(Roles = "ShutoutCargoIndex")]
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
            return View(context.stuffingmasters.Where(x => x.STFMDATE >= sd).Where(x => x.STFMDATE <= ed).Where(x => x.STFTID == 2).ToList());
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                int trc = Convert.ToInt32(totalRowsCount.Value);
                int frc = Convert.ToInt32(filteredRowsCount.Value);
                int srn = Convert.ToInt32(param.iDisplayStart);
                int ern = Convert.ToInt32(param.iDisplayStart + param.iDisplayLength);
                int CompyId = Convert.ToInt32(Session["compyid"]);

                DateTime sdate = Convert.ToDateTime(Session["SDATE"]).Date;
                DateTime edate = Convert.ToDateTime(Session["EDATE"]).Date;

                string squery = "exec [pr_Search_Export_ShutOut_Cargo_Master] @FilterTerm='" + param.sSearch + "',";
                squery += "@SortIndex=" + Convert.ToInt32(Request["iSortCol_0"]) + ",@SortDirection='" + Request["sSortDir_0"] + "',";
                squery += "@StartRowNum=" + srn + ",@EndRowNum=" + ern + ",";
                squery += "@TotalRowsCount=" + trc + ",@FilteredRowsCount=" + frc + ",@PCompyId=" + CompyId + ",@PSTFTId=2,";
                squery += "@PSDate='" + sdate.ToString("yyyy-MM-dd") + "',@PEDate='" + edate.ToString("yyyy-MM-dd") + "'";

                var data = context.Database.SqlQuery<pr_Search_Export_ShutOut_Cargo_Master_Result>(squery).ToList();

                //var data = e.pr_Search_Export_Stuffing_Master(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                //    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), 0, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));

                if (data.Count > 0)
                {
                    var aaData = data.Select(d => new string[] { d.STFMDATE.Value.ToString("dd/MM/yyyy"), d.STFMDNO.ToString(), d.STFMNAME, d.STFDSBDNO, d.STFCORDNO, d.CONTNRNO, d.STFDNOP.ToString(), d.DISPSTATUS.ToString(), d.STFMID.ToString(), d.STFDID.ToString() }).ToArray();

                    return Json(new
                    {
                        sEcho = param.sEcho,
                        aaData = aaData,
                        iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                        iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var aaData = "";

                    return Json(new
                    {
                        sEcho = param.sEcho,
                        aaData = aaData,
                        iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                        iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                    }, JsonRequestBehavior.AllowGet);
                }

            }
        }

        public JsonResult Delete_GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                var data = e.pr_Search_Export_ShutOut_Cargo_Master(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), 2, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                var aaData = data.Select(d => new string[] { d.STFMDATE.Value.ToString("dd/MM/yyyy"), d.STFMDNO.ToString(), d.STFMNAME, d.STFDSBDNO, d.STFCORDNO, d.STFDNOP.ToString(), d.STFMID.ToString(), d.STFDID.ToString() }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "ShutoutCargoEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ShutoutCargo/NForm/" + id);
        }

        [Authorize(Roles = "ShutoutCargoCreate")]
        public ActionResult NForm(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            context.Database.ExecuteSqlCommand("DELETE FROM TMP_STUFFING_SBDID WHERE KUSRID='" + Session["CUSRID"] + "'");
            StuffingMaster tab = new StuffingMaster();
            StuffingMD vm = new StuffingMD();
            ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6 && m.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME");
            //ViewBag.EOPTID = new SelectList(context.Export_OperationTypeMaster.OrderBy(m => m.EOPTDESC), "EOPTID", "EOPTDESC");
            ViewBag.EOPTID = new SelectList(context.Export_OperationTypeMaster.Where(m => (m.EOPTID == 2 || m.EOPTID == 3 || m.EOPTID == 4 || m.EOPTID == 5) && m.DISPSTATUS == 0).OrderBy(m => m.EOPTDESC), "EOPTID", "EOPTDESC");
            //ViewBag.SLABTID = new SelectList(context.exportslabtypemaster.Where(X => X.DISPSTATUS == 0 && X.SLABSTYPE == 2).OrderBy(m => m.SLABTDESC), "SLABTID", "SLABTDESC");
            ViewBag.SLABTID = new SelectList(context.exportslabtypemaster.Where(X => X.DISPSTATUS == 0).OrderBy(m => m.SLABTDESC), "SLABTID", "SLABTDESC");
            ViewBag.STFCORDID = new SelectList(""); ViewBag.SBDID = new SelectList(""); ViewBag.GIDID = new SelectList("");
            ViewBag.STFTYPE = new SelectList(context.Database.SqlQuery<Export_StuffTypeMaster>("SELECT * FROM Export_StuffTypeMaster").ToList(), "ESTID", "ESTDESC");

            //BILLED TO
            List<SelectListItem> selectedtaxlst1 = new List<SelectListItem>();
            SelectListItem selectedItemtax1 = new SelectListItem { Text = "EXPORTER", Value = "1", Selected = false };
            selectedtaxlst1.Add(selectedItemtax1);
            selectedItemtax1 = new SelectListItem { Text = "CHA", Value = "0", Selected = true };
            selectedtaxlst1.Add(selectedItemtax1);
            ViewBag.BILLEDTO = selectedtaxlst1;

            //STFCATEAID
            ViewBag.STFCATEAID = new SelectList("");
            ViewBag.STFBCATEAID = new SelectList("");
            if (id != 0)
            {
                tab = context.stuffingmasters.Find(id);//find selected record
                var Cont_result = context.Database.SqlQuery<SP_STUFFING_CONTAINER_NO_CBX_ASSGN_Result>("SP_STUFFING_CONTAINER_NO_CBX_ASSGN @PCHAID=" + tab.CHAID + ",@PSTFMID=" + id + "").ToList();
                //ViewBag.EOPTID = new SelectList(context.Export_OperationTypeMaster.OrderBy(m => m.EOPTDESC), "EOPTID", "EOPTDESC", tab.EOPTID);
                ViewBag.EOPTID = new SelectList(context.Export_OperationTypeMaster.Where(m => (m.EOPTID == 2 || m.EOPTID == 3 || m.EOPTID == 4 || m.EOPTID == 5) && m.DISPSTATUS == 0).OrderBy(m => m.EOPTDESC), "EOPTID", "EOPTDESC", tab.EOPTID);
                ViewBag.SLABTID = new SelectList(context.exportslabtypemaster.Where(X => X.DISPSTATUS == 0).OrderBy(m => m.SLABTDESC), "SLABTID", "SLABTDESC", tab.EOPTID);
                //.......................................Dropdown data........................................//
                ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6 && m.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME", tab.LCATEID);
                ViewBag.GIDID = new SelectList(Cont_result, "GIDID", "CONTNRNO");
                ViewBag.STFCATEAID = new SelectList(context.categoryaddressdetails.Where(m => m.CATEID == tab.CHAID).OrderBy(x => x.CATEATYPEDESC), "CATEAID", "CATEATYPEDESC", tab.STFCATEAID);
                ViewBag.STFBCATEAID = new SelectList(context.categoryaddressdetails.Where(m => m.CATEID == tab.STFBILLREFID).OrderBy(x => x.CATEATYPEDESC), "CATEAID", "CATEATYPEDESC", tab.STFBCATEAID);

                var sql5 = context.Database.SqlQuery<CategoryMaster>("select * from CategoryMaster where CATEID=" + tab.STFBILLREFID).ToList();
                ViewBag.STFBILLREFNAME = sql5[0].CATENAME;
                //ViewBag.CATEAGSTNO = tab.CATEAGSTNO;

                List<SelectListItem> selectedcontainertype01 = new List<SelectListItem>();
                if (tab.STFBILLEDTO == 1)
                {
                    SelectListItem selectedcontainer01 = new SelectListItem { Text = "EXPPORTER", Value = "1", Selected = true };
                    selectedcontainertype01.Add(selectedcontainer01);
                    selectedcontainer01 = new SelectListItem { Text = "CHA", Value = "0", Selected = false };
                    selectedcontainertype01.Add(selectedcontainer01);
                    ViewBag.BILLEDTO = selectedcontainertype01;
                }

                //.........End
                vm.masterdata = context.stuffingmasters.Where(det => det.STFMID == id).ToList();
                vm.detaildata = context.stuffingdetails.Where(det => det.STFMID == id).ToList();
                vm.shutoutdetail = context.Database.SqlQuery<PR_SHUTOUT_CARGO_FLX_ASSGN_Result>("PR_SHUTOUT_CARGO_FLX_ASSGN @PSTFMID=" + id + "").ToList();//........procedure  for edit mode details data

            }

            return View(vm);

        }//...........End of Form

        //.........insert and update...................//
        public void savedata(FormCollection F_Form, string term)
        {
            using (SCFSERPContext context = new SCFSERPContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        StuffingMaster stuffingmaster = new StuffingMaster();
                        StuffingDetail stuffingdetail = new StuffingDetail();
                        StuffingProductDetail stuffingproductdetail = new StuffingProductDetail();

                        //.................Getting Primarykey field..................//

                        //Int32 STFMID = Convert.ToInt32(F_Form["masterdata[0].STFMID"]);
                        Int32 STFMID = 0;
                        string fid = Convert.ToString(F_Form["masterdata[0].STFMID"]);
                        if (fid == "" || fid == null)
                        {
                            STFMID = 0;
                        }
                        else { STFMID = Convert.ToInt32(fid); }
                        Int32 STFDID = 0;
                        Int32 STFPID = 0;
                        string DELIDS = "";
                        string P_DELIDS = "";//.....End

                        if (STFMID != 0)//Getting Primary id in Edit mode
                        {
                            stuffingmaster = context.stuffingmasters.Find(STFMID);
                        }
                        //........................Stuffing Master..............//

                        stuffingmaster.COMPYID = Convert.ToInt32(Session["compyid"]);
                        stuffingmaster.STFTID = 2;

                        string todaydt = Convert.ToString(DateTime.Now);
                        string todayd = Convert.ToString(DateTime.Now.Date);

                        string indate = Convert.ToString(F_Form["masterdata[0].STFMDATE"]);
                        string intime = Convert.ToString(F_Form["masterdata[0].STFMTIME"]);

                        if (indate != null || indate != "")
                        {
                            stuffingmaster.STFMDATE = Convert.ToDateTime(indate).Date;
                        }
                        else { stuffingmaster.STFMDATE = DateTime.Now.Date; }

                        if (stuffingmaster.STFMDATE > Convert.ToDateTime(todayd))
                        {
                            stuffingmaster.STFMDATE = Convert.ToDateTime(todayd);
                        }

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

                                    stuffingmaster.STFMTIME = Convert.ToDateTime(in_datetime);
                                }
                                else { stuffingmaster.STFMTIME = DateTime.Now; }
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

                                    stuffingmaster.STFMTIME = Convert.ToDateTime(in_datetime);
                                }
                                else { stuffingmaster.STFMTIME = DateTime.Now; }

                            }
                        }
                        else { stuffingmaster.STFMTIME = DateTime.Now; }

                        if (stuffingmaster.STFMTIME > Convert.ToDateTime(todaydt))
                        {
                            stuffingmaster.STFMTIME = Convert.ToDateTime(todaydt);
                        }

                        //stuffingmaster.STFMDATE = Convert.ToDateTime(F_Form["masterdata[0].STFMTIME"]).Date;
                        //stuffingmaster.STFMTIME = Convert.ToDateTime(F_Form["masterdata[0].STFMTIME"]);

                        stuffingmaster.CHAID = Convert.ToInt32(F_Form["masterdata[0].CHAID"]);
                        stuffingmaster.STFMNAME = F_Form["masterdata[0].STFMNAME"].ToString();
                        stuffingmaster.LCATEID = Convert.ToInt32(F_Form["masterdata[0].CHAID"]);
                        stuffingmaster.LCATENAME = F_Form["masterdata[0].STFMNAME"].ToString();
                        stuffingmaster.EOPTID = Convert.ToInt32(F_Form["EOPTID"]);
                        stuffingmaster.TGID = 0;
                        stuffingmaster.SLABTID = 0;

                        if (STFMID == 0)
                        {
                            stuffingmaster.CUSRID = Session["CUSRID"].ToString();
                        }
                        stuffingmaster.LMUSRID = Session["CUSRID"].ToString();

                        //stuffingmaster.LMUSRID = 1;
                        stuffingmaster.DISPSTATUS = 0;
                        stuffingmaster.PRCSDATE = DateTime.Now;

                        stuffingmaster.STFBILLEDTO = 0;// Convert.ToInt16(F_Form["BILLEDTO"]);
                        //if (Convert.ToString(F_Form["masterdata[0].STFBILLREFID"]) != "" || Convert.ToString(F_Form["masterdata[0].STFBILLREFID"]) != "0")
                        //{
                            stuffingmaster.STFBILLREFID = Convert.ToInt32(F_Form["masterdata[0].CHAID"]);
                        //}
                        //else { stuffingmaster.STFBILLREFID = 0; }
                        stuffingmaster.STFBILLREFNAME = Convert.ToString(F_Form["masterdata[0].STFMNAME"]);
                        stuffingmaster.STFCATEAID = 0;
                        string STFCATEAID = Convert.ToString(F_Form["STFCATEAID"]);
                        if (STFCATEAID != "" || STFCATEAID != null || STFCATEAID != "0")
                        {
                            stuffingmaster.STFCATEAID = Convert.ToInt32(STFCATEAID);
                        }
                        else { stuffingmaster.STFCATEAID = 0; }

                        string STATEID = Convert.ToString(F_Form["masterdata[0].STATEID"]);
                        if (STATEID != "" || STATEID != null)
                        {
                            stuffingmaster.STATEID = Convert.ToInt32(STATEID);
                        }
                        else { stuffingmaster.STATEID = 0; }

                        stuffingmaster.CATEAGSTNO = Convert.ToString(F_Form["masterdata[0].CATEAGSTNO"]);
                        stuffingmaster.TRANIMPADDR1 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR1"]);
                        stuffingmaster.TRANIMPADDR2 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR2"]);
                        stuffingmaster.TRANIMPADDR3 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR3"]);
                        stuffingmaster.TRANIMPADDR4 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR4"]);

                        string STFBCATEAID = Convert.ToString(F_Form["STFBCATEAID"]);
                        if (STFCATEAID != "" || STFCATEAID != null)
                        {
                            stuffingmaster.STFBCATEAID = Convert.ToInt32(STFCATEAID);
                        }
                        else { stuffingmaster.STFBCATEAID = 0; }

                        //string STFBCHASTATEID = Convert.ToString(F_Form["masterdata[0].STFBCHASTATEID"]);
                        //if (STFBCHASTATEID != "" || STFBCHASTATEID != null)
                        //{
                            stuffingmaster.STFBCHASTATEID = Convert.ToInt32(STATEID);
                        //}
                        //else { stuffingmaster.STFBCHASTATEID = 0; }

                        stuffingmaster.STFBCHAGSTNO = Convert.ToString(F_Form["masterdata[0].CATEAGSTNO"]);
                        stuffingmaster.STFBCHAADDR1 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR1"]);
                        stuffingmaster.STFBCHAADDR2 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR2"]);
                        stuffingmaster.STFBCHAADDR3 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR3"]);
                        stuffingmaster.STFBCHAADDR4 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR4"]);

                        stuffingmaster.STF_SBILL_RNO = "";// Convert.ToString(F_Form["masterdata[0].STF_SBILL_RNO"]);
                        stuffingmaster.STF_FORM13_RNO = "";//Convert.ToString(F_Form["masterdata[0].STF_FORM13_RNO"]);

                        if (STFMID == 0)
                        {
                            stuffingmaster.STFMNO = Convert.ToInt32(Autonumber.autonum("stuffingmaster", "STFMNO", "STFTID = 2 AND COMPYID = " + Convert.ToInt32(Session["compyid"]) + "").ToString());
                            int ano = stuffingmaster.STFMNO;
                            string prfx = string.Format("{0:D5}", ano);
                            stuffingmaster.STFMDNO = prfx.ToString();
                            context.stuffingmasters.Add(stuffingmaster);
                            context.SaveChanges();
                        }
                        else
                        {
                            context.Entry(stuffingmaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        } //.........End of stuffing Master 


                        //....................stuffing Detail---------------------------
                        string[] A_STFDID = F_Form.GetValues("STFDID");
                        string[] A_STFPID = F_Form.GetValues("STFPID");
                        string[] GIDID = F_Form.GetValues("GIDID");
                        string[] STFTYPE = F_Form.GetValues("detaildata[0].STFTYPE");
                        string[] STFREFNO = F_Form.GetValues("STFREFNO");
                        string[] booltype = F_Form.GetValues("booltype");
                        string[] STFCORDNO = F_Form.GetValues("STFCORDNO");
                        string[] STFDSBDATE = F_Form.GetValues("STFDSBDATE");
                        string[] STFDNOP = F_Form.GetValues("STFDNOP");
                        string[] STFDQTY = F_Form.GetValues("STFDQTY");
                        string[] STFDSBDDATE = F_Form.GetValues("STFDSBDDATE");
                        string[] STFDSBNO = F_Form.GetValues("STFDSBNO");
                        string[] STFDSBDNO = F_Form.GetValues("STFDSBDNO");
                        string[] SBDID = F_Form.GetValues("SBDID");
                        string[] desc = F_Form.GetValues("STFCORDDESC");

                        string[] ESBDID = F_Form.GetValues("ESBDID");
                        string[] ESBD_SBILLDATE = F_Form.GetValues("ESBD_SBILLDATE");
                        string[] ESBD_SBILLNO = F_Form.GetValues("ESBD_SBILLNO");

                        HashSet<int> hash = new HashSet<int>(context.stuffingdetails.Select(m => m.GIDID));

                        for (int count = 0; count < A_STFDID.Count(); count++)
                        {
                            //if (booltype[count] == "true")
                            //{
                            STFDID = Convert.ToInt32(A_STFDID[count]);
                            STFPID = Convert.ToInt32(A_STFPID[count]);
                            // var bools = Convert.ToString(booltype[count]);
                            if (STFDID != 0 /*&& bools == "true"*/)
                            {
                                stuffingdetail = context.stuffingdetails.Find(STFDID);
                                stuffingproductdetail = context.stuffingproductdetails.Find(STFPID);
                            }

                            var f_gidid = 0;// Convert.ToInt32(GIDID[count]);
                            if (Convert.ToInt32(STFDID) == 0)
                            {

                                //......... insert condition for Container No.....................
                                //if (!hash.Contains(f_gidid))
                                //{
                                    stuffingdetail.STFMID = stuffingmaster.STFMID;
                                    stuffingdetail.GIDID = 0;// Convert.ToInt32(GIDID[count]);
                                    stuffingdetail.STFTYPE = Convert.ToInt16(STFTYPE[count]);
                                    stuffingdetail.CUSRID = Session["CUSRID"].ToString();
                                    stuffingdetail.LMUSRID = Session["CUSRID"].ToString();
                                    //stuffingdetail.LMUSRID = 1;
                                    stuffingdetail.DISPSTATUS = 0;
                                    stuffingdetail.PRCSDATE = DateTime.Now;
                                    context.stuffingdetails.Add(stuffingdetail);
                                    context.SaveChanges();
                                    STFDID = stuffingdetail.STFDID;
                                    hash.Add(f_gidid);
                                //}
                                //else
                                //{
                                //    hash.Add(f_gidid);
                                //    var sql = context.Database.SqlQuery<int>("SELECT STFDID from stuffingdetail where GIDID=" + Convert.ToInt32(f_gidid)).ToList();
                                //    var ids = (sql[0]).ToString();
                                //    stuffingdetail = context.stuffingdetails.Find(Convert.ToInt32(ids));
                                //    context.Entry(stuffingdetail).Entity.STFTYPE = Convert.ToInt16(STFTYPE[count]);

                                //}
                                STFDID = stuffingdetail.STFDID;

                            }
                            else
                            {
                                hash.Add(f_gidid);
                                stuffingdetail.STFMID = stuffingmaster.STFMID;
                                stuffingdetail.GIDID = Convert.ToInt32(GIDID[count]);
                                stuffingdetail.STFTYPE = Convert.ToInt16(STFTYPE[count]);
                                //stuffingdetail.CUSRID = Session["CUSRID"].ToString();
                                stuffingdetail.LMUSRID = Session["CUSRID"].ToString();
                                //stuffingdetail.LMUSRID = 1;
                                stuffingdetail.DISPSTATUS = 0;
                                stuffingdetail.PRCSDATE = DateTime.Now;
                                // stuffingdetail.STFDID = STFDID;
                                context.Entry(stuffingdetail).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                            }
                            //.................Stuffing Product detail..............//
                            stuffingproductdetail.STFDID = stuffingdetail.STFDID;
                            stuffingproductdetail.STFCORDNO = STFCORDNO[count].ToString();
                            stuffingproductdetail.STFDSBNO = STFDSBNO[count];
                            stuffingproductdetail.SBDID = Convert.ToInt32(SBDID[count]);

                            stuffingproductdetail.ESBDID = Convert.ToInt32(ESBDID[count]);
                            stuffingproductdetail.ESBD_SBILLDATE = Convert.ToDateTime(ESBD_SBILLDATE[count]).Date;
                            stuffingproductdetail.ESBD_SBILLNO = ESBD_SBILLNO[count].ToString();


                            stuffingproductdetail.STFDSBDNO = STFDSBDNO[count];
                            stuffingproductdetail.STFDSBDATE = Convert.ToDateTime(STFDSBDATE[count]).Date;
                            stuffingproductdetail.STFDSBDDATE = Convert.ToDateTime(STFDSBDDATE[count]).Date;
                            stuffingproductdetail.STFDNOP = Convert.ToDecimal(STFDNOP[count]);
                            stuffingproductdetail.STFDQTY = Convert.ToDecimal(STFDQTY[count]);
                            if (Session["CUSRID"] != null)
                            {
                                stuffingproductdetail.CUSRID = Session["CUSRID"].ToString();

                            }
                            stuffingproductdetail.LMUSRID = Session["CUSRID"].ToString();
                            //   stuffingproductdetail.CUSRID = "admin";
                            //stuffingproductdetail.LMUSRID = 1;
                            stuffingproductdetail.DISPSTATUS = 0;
                            stuffingproductdetail.PRCSDATE = DateTime.Now;
                            if (Convert.ToInt32(STFPID) == 0)
                            {
                                context.stuffingproductdetails.Add(stuffingproductdetail);
                                context.SaveChanges();
                                STFPID = stuffingproductdetail.STFPID;
                            }
                            else
                            {
                                stuffingproductdetail.STFPID = STFPID;
                                context.Entry(stuffingproductdetail).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                            }
                            DELIDS = DELIDS + "," + STFDID.ToString();
                            P_DELIDS = P_DELIDS + "," + STFPID.ToString();
                            //count++;
                        }

                        //   }

                        context.Database.ExecuteSqlCommand("DELETE FROM stuffingproductdetail  WHERE STFDID IN(" + DELIDS.Substring(1) + ") and  STFPID NOT IN(" + P_DELIDS.Substring(1) + ")");
                        context.Database.ExecuteSqlCommand("DELETE FROM stuffingdetail  WHERE STFMID=" + STFMID + " and  STFDID NOT IN(" + DELIDS.Substring(1) + ")");
                        context.Database.ExecuteSqlCommand("DELETE FROM TMP_STUFFING_SBDID WHERE KUSRID='" + Session["CUSRID"] + "'");
                        trans.Commit(); Response.Redirect("Index");
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message.ToString();
                        trans.Rollback();
                        Response.Redirect("/Error/AccessDenied");
                    }
                }
            }
        }//---End of Savedata


        //..........Autocomplete CHA Name
        public JsonResult AutoCha(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //---End 

        //..........................Printview...
        //[Authorize(Roles = "ShutoutCargoPrint")]
        public void PrintView(int? id = 0)
        {

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "SHUTOUTDETAIL", Convert.ToInt32(id), Session["CUSRID"].ToString());
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


                //cryRpt.Load("D:\\CFSReports\\Export_Stuffing.RPT");
                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "ShutoutCargo.RPT");
                cryRpt.RecordSelectionFormula = "{VW_SHUTOUT_DETAIL_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_SHUTOUT_DETAIL_PRINT_ASSGN.STFMID} = " + id;



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
        //end
        //...............Delete Row.............
        [Authorize(Roles = "ShutoutCargoDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");

            var param = id.Split('-');

            String temp = Delete_fun.delete_check1(fld, param[1]);

            if (temp.Equals("PROCEED"))
            {
                StuffingMaster stuffingmaster = context.stuffingmasters.Find(Convert.ToInt32(param[0]));
                context.stuffingmasters.Remove(stuffingmaster);
                context.SaveChanges();
                Response.Write("Deleted successfully...");
            }
            else
                Response.Write(temp);
        }//...end

        public JsonResult GetCartingNo(int id)
        {
            var data = context.Database.SqlQuery<pr_Export_ShutOut_COrder_No_Assgn_Result>("pr_Export_ShutOut_COrder_No_Assgn @PCHAID=" + id + ",@PKUSRID='" + Session["CUSRID"] + "'").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}