using scfs.Data;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System;
using System.Configuration;


namespace scfs_erp.Controllers.Import
{
    [SessionExpire]
    public class ImportRemoteGateInController : Controller
    {
        // GET: ImportRemoteGateIn
        #region Context declaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region RemoteGateIn Index
        [Authorize(Roles = "RemoteGateInIndex")]
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
                       

            DateTime fromdate = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;
            DateTime todate = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;


            TotalContainerDetails(fromdate, todate);


            return View();
        }

        [Authorize(Roles = "ImportGateInTransferIndex")]
        public ActionResult GIndex()//..gate in transfer
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            if (Request.Form.Get("from") != null)
            {
                Session["SDATE"] = Request.Form.Get("from");
                Session["EDATE"] = Request.Form.Get("to");
            }
            else
            {
                Session["SDATE"] = DateTime.Now.Date.ToString("dd-MM-yyyy");
                Session["EDATE"] = DateTime.Now.Date.ToString("dd-MM-yyyy");
            }

            DateTime fromdate = Convert.ToDateTime(Session["SDATE"]).Date;
            DateTime todate = Convert.ToDateTime(Session["EDATE"]).Date;


            TotalContainerDetails(fromdate, todate);

           
            return View();
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
                      

            var result = context.Database.SqlQuery<PR_IMPORT_REMOTEGATEINCONTAINER_DETAILS_Result>("EXEC PR_IMPORT_REMOTEGATEINCONTAINER_DETAILS @PFDT='" + fdate + "',@PTDT='" + tdate + "',@PSDPTID=" + 1).ToList();

            foreach (var rslt in result)
            {
                if ((rslt.Sno == 1) && (rslt.Descriptn == "IMPORT - REMOTEGATEIN"))
                {
                    @ViewBag.Total20 = rslt.c_20;
                    @ViewBag.Total40 = rslt.c_40;
                    @ViewBag.Total45 = rslt.c_45;
                    @ViewBag.TotalTues = rslt.c_tues;

                    Session["RGI20"] = rslt.c_20;
                    Session["RGI40"] = rslt.c_40;
                    Session["RGI45"] = rslt.c_45;
                    Session["RGITU"] = rslt.c_tues;
                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Get data from database
        public JsonResult RGateinGetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Import_RemoteGateIn(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));

                var aaData = data.Select(d => new string[] { d.GIDATE.ToString(), d.GIDNO, d.CONTNRNO, d.CONTNRSID, d.IGMNO, d.GPLNO, d.VSLNAME, d.STMRNAME, d.PRDTDESC, d.DISPSTATUS, d.GIDID.ToString(), d.AGIDNO ?? "" }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RTGateinGetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Import_RemoteGateInTransfer(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.GIDATE.ToString(), d.GIDNO, d.CONTNRNO, d.CONTNRSID, d.IGMNO, d.GPLNO, d.VSLNAME, d.STMRNAME, d.PRDTDESC, d.DISPSTATUS, d.GIDID.ToString(), d.AGIDNO ?? "" }).ToArray();

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

        #region Creating Modify and create Form 
        [Authorize(Roles = "RemoteGateInEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ImportRemoteGateIn/Form/" + id);
        }

        [Authorize(Roles = "RemoteGateInCreate")]
        //---------------Remote GateIn Form---------------------------------
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            RemoteGateIn remotegatein = new RemoteGateIn();
            remotegatein.GITIME = DateTime.Now;
            remotegatein.GICCTLTIME = DateTime.Now;
            remotegatein.GIDID = 0;
            remotegatein.IGMDATE = DateTime.Now.Date;
            ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");
            ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CONTNRTDESC), "CONTNRTID", "CONTNRTDESC");
            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID > 1).Where(x => x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");
            ViewBag.GPPTYPE = new SelectList(context.porttypemaster, "GPPTYPE", "GPPTYPEDESC");
            ViewBag.GPMODEID = new SelectList(context.gpmodemasters.Where(x => x.DISPSTATUS == 0 && x.GPMODEID != 5 && x.GPMODEID != 4).OrderBy(x => x.GPMODEDESC), "GPMODEID", "GPMODEDESC", 2);

            if (id != 0)   //------Edit mode----
            {
                remotegatein = context.remotegateindetails.Find(id);

                ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", remotegatein.PRDTGID);
                ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CONTNRTDESC), "CONTNRTID", "CONTNRTDESC", remotegatein.CONTNRTID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID > 1).Where(x => x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC", remotegatein.CONTNRSID);
                ViewBag.GPPTYPE = new SelectList(context.porttypemaster, "GPPTYPE", "GPPTYPEDESC", remotegatein.GPPTYPE);
                ViewBag.GPMODEID = new SelectList(context.gpmodemasters.Where(x => x.DISPSTATUS == 0 && x.GPMODEID != 5 && x.GPMODEID != 4).OrderBy(x => x.GPMODEDESC), "GPMODEID", "GPMODEDESC", remotegatein.GPMODEID);
            }//----End of If---

            return View(remotegatein);
        }
        #endregion

        #region save data in database
        [HttpPost]
        public JsonResult SaveData(RemoteGateIn remotegatein)
        {
            using (SCFSERPContext dataContext = new SCFSERPContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
                {
                    string status = "";
                    try
                    {
                        string todaydt = Convert.ToString(DateTime.Now);
                        string todayd = Convert.ToString(DateTime.Now.Date);

                        remotegatein.COMPYID = Convert.ToInt32(Session["compyid"]);
                        remotegatein.SDPTID = 1;
                        remotegatein.AGIDNO = "0";

                        //remotegatein.GIDATE = Convert.ToDateTime(remotegatein.GITIME).Date;
                        //remotegatein.GICCTLDATE = Convert.ToDateTime(remotegatein.GICCTLTIME).Date;
                        //remotegatein.GIDATE = DateTime.Now.Date;
                        //remotegatein.GITIME = DateTime.Now;
                        //remotegatein.GICCTLDATE = DateTime.Now.Date; 
                        //remotegatein.GICCTLTIME = DateTime.Now;

                        string indate = Convert.ToString(remotegatein.GIDATE);
                        if (indate != null || indate != "")
                        {
                            remotegatein.GIDATE = Convert.ToDateTime(indate).Date;
                        }
                        else { remotegatein.GIDATE = DateTime.Now.Date; }

                        if (remotegatein.GIDATE > Convert.ToDateTime(todayd))
                        {
                            remotegatein.GIDATE = Convert.ToDateTime(todayd);
                        }

                        string intime = Convert.ToString(remotegatein.GITIME);
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

                                    remotegatein.GITIME = Convert.ToDateTime(in_datetime);
                                }
                                else { remotegatein.GITIME = DateTime.Now; }
                            }
                            else { remotegatein.GITIME = DateTime.Now; }
                        }
                        else { remotegatein.GITIME = DateTime.Now; }

                        if (remotegatein.GITIME > Convert.ToDateTime(todaydt))
                        {
                            remotegatein.GITIME = Convert.ToDateTime(todaydt);
                        }

                        // GATE IN CCT DATE AND TIME
                        string cctindate = Convert.ToString(remotegatein.GICCTLDATE);
                        if (cctindate != null || cctindate != "")
                        {
                            remotegatein.GICCTLDATE = Convert.ToDateTime(indate).Date;
                        }
                        else { remotegatein.GICCTLDATE = DateTime.Now.Date; }

                        if (remotegatein.GICCTLDATE > Convert.ToDateTime(todayd))
                        {
                            remotegatein.GICCTLDATE = Convert.ToDateTime(todayd);
                        }

                        string cctintime = Convert.ToString(remotegatein.GITIME);
                        if ((cctintime != null || cctintime != "") && ((cctindate != null || cctindate != "")))
                        {
                            if ((cctintime.Contains(' ')) && (cctindate.Contains(' ')))
                            {
                                var CCin_time = cctintime.Split(' ');
                                var CCTin_date = cctindate.Split(' ');

                                if ((CCin_time[1].Contains(':')) && (CCTin_date[0].Contains('/')))
                                {
                                    var CCTin_time1 = CCin_time[1].Split(':');
                                    var CCTin_date1 = CCTin_date[0].Split('/');

                                    string CCTin_datetime = CCTin_date1[2] + "-" + CCTin_date1[1] + "-" + CCTin_date1[0] + "  " + CCTin_time1[0] + ":" + CCTin_time1[1] + ":" + CCTin_time1[2];

                                    remotegatein.GICCTLTIME = Convert.ToDateTime(CCTin_datetime);
                                }
                                else { remotegatein.GICCTLTIME = DateTime.Now; }
                            }
                            else { remotegatein.GICCTLTIME = DateTime.Now; }
                        }
                        else { remotegatein.GICCTLTIME = DateTime.Now; }

                        if (remotegatein.GICCTLTIME > Convert.ToDateTime(todaydt))
                        {
                            remotegatein.GICCTLTIME = Convert.ToDateTime(todaydt);
                        }

                        //var newGIDateTime = new DateTime(remotegatein.GIDATE.Year, remotegatein.GIDATE.Month, remotegatein.GIDATE.Day, remotegatein.GITIME.Hour, remotegatein.GITIME.Minute, remotegatein.GITIME.Second);

                        //remotegatein.GITIME = Convert.ToDateTime(newGIDateTime);

                        // var newGICCTDateTime = new DateTime(remotegatein.GICCTLDATE.Year, remotegatein.GICCTLDATE.Month, remotegatein.GICCTLDATE.Day, remotegatein.GICCTLTIME.Hour, remotegatein.GICCTLTIME.Minute, remotegatein.GICCTLTIME.Second);

                        // remotegatein.GICCTLTIME = Convert.ToDateTime(newGICCTDateTime);

                        if (remotegatein.CUSRID == "" || remotegatein.CUSRID == null)
                        {
                            if (Session["CUSRID"] != null)
                            {
                                remotegatein.CUSRID = Session["CUSRID"].ToString();
                            }
                            else { remotegatein.CUSRID = ""; }
                        }
                        remotegatein.LMUSRID = Session["CUSRID"].ToString();
                        remotegatein.PRCSDATE = DateTime.Now;

                        if ((remotegatein.GIDID).ToString() != "0")
                        {
                            context.Entry(remotegatein).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            status = "Update";
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            string sqr = "";
                            sqr = "Select *from REMOTEGATEINDETAIL WHERE VOYNO = '" + remotegatein.VOYNO.ToString() + "' and IGMNO='" + remotegatein.IGMNO.ToString() + "' and GPLNO='" + remotegatein.GPLNO.ToString() + "'";
                            sqr += " and CONTNRNO = '" + remotegatein.CONTNRNO.ToString() + "'";                           

                            var sq = context.Database.SqlQuery<RemoteGateIn>(sqr).ToList();

                            if (sq.Count > 0)
                            {
                                status = "Exists";
                                return Json(status, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                remotegatein.CUSRID = Session["CUSRID"].ToString();
                                remotegatein.GINO = Convert.ToInt32(Autonumber.autonum("remotegateindetail", "GINO", "GINO <> 0 AND SDPTID = 1 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                                int temp = remotegatein.GINO;
                                string GIDNO = string.Format("{0:D5}", temp);
                                remotegatein.GIDNO = GIDNO.ToString();

                                context.remotegateindetails.Add(remotegatein);
                                context.SaveChanges();

                                status = "Success";
                                return Json(status, JsonRequestBehavior.AllowGet);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        status = "Error";
                        string Message = ex.Message.ToString();
                        return Json(status, Message, JsonRequestBehavior.AllowGet);
                        //Response.Write("Sorry!! An Error Occurred.... ");
                    }
                }
            }
        }
        #endregion

        #region AutoComplete Vessel Name        
        public JsonResult AutoVessel(string term)
        {
            var result = (from vessel in context.vesselmasters.Where(x => x.DISPSTATUS == 0)
                          where vessel.VSLDESC.ToLower().Contains(term.ToLower())
                          select new { vessel.VSLDESC, vessel.VSLID }).Distinct().OrderBy(x => x.VSLDESC);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region AutoComplete Steamer Name  
        public JsonResult AutoSteamer(string term)
        {
            var result = (from category in context.categorymasters.Where(m => m.CATETID == 3).Where(x => x.DISPSTATUS == 0)
                          where category.CATENAME.ToLower().Contains(term.ToLower())
                          select new { category.CATENAME, category.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region AutoComplete Importer Name
        public JsonResult AutoImpoter(string term)
        {
            var result = (from category in context.categorymasters.Where(m => m.CATETID == 1).Where(x => x.DISPSTATUS == 0)
                          where category.CATENAME.ToLower().Contains(term.ToLower())
                          select new { category.CATENAME, category.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete R Gatein
        [Authorize(Roles = "RemoteGateInDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                RemoteGateIn remotegateindetails = context.remotegateindetails.Find(Convert.ToInt32(id));
                context.remotegateindetails.Remove(remotegateindetails);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write("Gate In exists, deletion is not possible!");
            //Response.Write(temp);

        }
        #endregion
    }
}