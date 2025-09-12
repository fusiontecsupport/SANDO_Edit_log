using scfs.Data;
using scfs_erp.Context;
using scfs_erp.Models;
using scfs_erp.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Configuration;
using System.Data.SqlClient;
using scfs_erp;

namespace scfs_erp.Controllers.Export
{
    [SessionExpire]
    public class ExportCartingOrderController : Controller
    {
        // GET: ExportCartingOrder
        #region contextdeclaration
        SCFSERPContext context = new SCFSERPContext();
        CFSExportEntities db = new CFSExportEntities();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        #endregion

        #region Indexpage
        [Authorize(Roles = "ExportCartingOrderIndex")]
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
            return View(context.shippingbillmasters.Where(x => x.SBMDATE >= sd).Where(x => x.SBMDATE <= ed).ToList());
        }

        [Authorize(Roles = "ExportShippingAdmissionIndex")]
        public ActionResult SAIndex()
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

            Session["ESBDIdx"] = "SA";
            return View();
        }
        #endregion

        #region GetAjaxData
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_SearchExportCartingOrder(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(Session["compyid"]));
                var aaData = data.Select(d => new string[] { d.SBMDATE.Value.ToString("dd/MM/yyyy"), d.SBMTIME.Value.ToString("hh:mm tt"), d.SBMDNO.ToString(), d.EXPRTNAME, d.CHANAME, d.VHLNO, d.DISPSTATUS, d.SBMID.ToString() }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSAAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Export_Shipping_Admission(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount);
                var aaData = data.Select(d => new string[] { d.SBMDATE.ToString(), d.SBMDNO.ToString(), d.ESBMIDATE.ToString(), d.ESBMDNO.ToString(), d.PRDTDESC.ToString(), d.VHLNO.ToString(), d.SBDQTY.ToString(), d.SBMID.ToString(), d.ESBMID.ToString(), d.GIDID.ToString() }).ToArray();
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

        #region EditCartingOrder
        [Authorize(Roles = "ExportCartingOrderEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ExportCartingOrder/Form/" + id);
        }
        #endregion

        #region CreatingCartingOrder
        [Authorize(Roles = "ExportCartingOrderCreate")]
        public ActionResult Form(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ShippingBillMaster tab = new ShippingBillMaster();
            ShippingBillMD vm = new ShippingBillMD();
            ViewBag.PRDTGID = new SelectList("");
            ViewBag.PRDTTID = new SelectList("");
            ViewBag.UsrGrp = Session["Group"].ToString();
            ViewBag.UNLOADEDNOP = "";
            ViewBag.GDWNID = new SelectList(context.godownmasters.Where(x => x.DISPSTATUS == 0 && x.GDWNID > 1), "GDWNID", "GDWNDESC");
            ViewBag.STAGID = new SelectList(context.stagmasters.Where(x => x.DISPSTATUS == 0 && x.STAGID > 1), "STAGID", "STAGDESC");
            //  
            ViewBag.ESBMID = new SelectList("");// db.VW_EXPORT_SHIPPINGBILL_NO_CTRL_ASSGN, "ESBMID", "ESBMDNO");
            ViewBag.GIDID = new SelectList("");
            ViewBag.SBMDATE = DateTime.Now;
            if (id != 0)
            {
                tab = context.shippingbillmasters.Find(id);//find selected record
                vm.masterdata = context.shippingbillmasters.Where(det => det.SBMID == id).ToList();
                vm.detaildata = context.shippingbilldetails.Where(det => det.SBMID == id).ToList();
                var qry = context.Database.SqlQuery<ExportShippingBillMaster>("select *from EXPORTSHIPPINGBILLMASTER where ESBMID=" + vm.detaildata[0].ESBMID).ToList();
                ViewBag.ESBMID = qry[0].ESBMID;
                ViewBag.ESBMDNO = qry[0].ESBMDNO;
                //ViewBag.ESBMID = new SelectList(context.exportshippingbillmasters, "ESBMID", "ESBMDNO");
            }              
            return View(vm);
        }
        #endregion

        #region GetESBMID
        public void GetESBMID(int id)
        {
            var group = context.Database.SqlQuery<Nullable<int>>("select ESBMID from [VW_SHIPPINGBILL_GATEINT_TRUCKNO_CBX_ASSGN] where GIDID=" + id).ToList();
            if (group[0] != null)
                Response.Write(group[0]);
            else
                Response.Write("0");
        }
        #endregion

        #region GetShippingBillNo
        public JsonResult GetShippingBillNo(int id)
        {
            //List<VW_EXPORT_SHIPPINGBILL_NO_CTRL_ASSGN> group = new List<VW_EXPORT_SHIPPINGBILL_NO_CTRL_ASSGN>();

            //if (id > 0)
            //{
            //    group = context.Database.SqlQuery<VW_EXPORT_SHIPPINGBILL_NO_CTRL_ASSGN>("select * from VW_EXPORT_SHIPPINGBILL_NO_CTRL_ASSGN where ESBMID=" + id).ToList();
            //    var list2 = context.Database.SqlQuery<VW_EXPORT_SHIPPINGBILL_NO_CTRL_ASSGN>("select * from VW_EXPORT_SHIPPINGBILL_NO_CTRL_ASSGN where ESBMID <>" + id).ToList();
            //    return new JsonResult() { Data = group.Concat(list2), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //}
            //else
            //{
            //    group = context.Database.SqlQuery<VW_EXPORT_SHIPPINGBILL_NO_CTRL_ASSGN>("select * from VW_EXPORT_SHIPPINGBILL_NO_CTRL_ASSGN").ToList();
            //    return new JsonResult() { Data = group, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //}
            List<VW_SHIPPINGBILL_GATEINT_TRUCKNO_CBX_ASSGN> group = new List<VW_SHIPPINGBILL_GATEINT_TRUCKNO_CBX_ASSGN>();

            group = context.Database.SqlQuery<VW_SHIPPINGBILL_GATEINT_TRUCKNO_CBX_ASSGN>("select * from VW_SHIPPINGBILL_GATEINT_TRUCKNO_CBX_ASSGN where GIDID=" + id).ToList();
            return new JsonResult() { Data = group, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }
        #endregion

        #region savedata
        public void savedata(FormCollection F_Form)
        {
            using (SCFSERPContext context = new SCFSERPContext())
            {
                //using (var trans = context.Database.BeginTransaction())
                //{
                try
                {
                    ShippingBillMaster shippingbillmaster = new ShippingBillMaster();
                    ShippingBillDetail shippingbilldetail = new ShippingBillDetail();
                    //-------Getting Primarykey field--------
                    Int32 SBMID = Convert.ToInt32(F_Form["masterdata[0].SBMID"]);
                    Int32 SBDID = 0;
                    string DELIDS = "";
                    //-----End
                    if (SBMID != 0)
                    {
                        shippingbillmaster = context.shippingbillmasters.Find(SBMID);
                    }

                    string todaydt = Convert.ToString(DateTime.Now);
                    string todayd = Convert.ToString(DateTime.Now.Date);

                    string indate = Convert.ToString(F_Form["masterdata[0].SBMDATE"]);
                    string intime = Convert.ToString(F_Form["masterdata[0].SBMTIME"]);

                    shippingbillmaster.SBMID = SBMID;
                    shippingbillmaster.COMPYID = Convert.ToInt32(Session["compyid"]);
                    shippingbillmaster.SBMRMKS = "";
                    shippingbillmaster.LMUSRID = Session["CUSRID"].ToString();
                    if (SBMID == 0 || shippingbillmaster.CUSRID == null || shippingbillmaster.CUSRID == "" || shippingbillmaster.CUSRID == "1" || shippingbillmaster.CUSRID == "0")
                    {
                        shippingbillmaster.CUSRID = Session["CUSRID"].ToString();
                    }
                    shippingbillmaster.DISPSTATUS = 0;
                    shippingbillmaster.PRCSDATE = DateTime.Now;

                    if (indate != null || indate != "")
                    {
                        shippingbillmaster.SBMDATE = Convert.ToDateTime(indate).Date;
                    }
                    else { shippingbillmaster.SBMDATE = DateTime.Now.Date; }

                    if (shippingbillmaster.SBMDATE > Convert.ToDateTime(todayd))
                    {
                        shippingbillmaster.SBMDATE = Convert.ToDateTime(todayd);
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

                                shippingbillmaster.SBMTIME = Convert.ToDateTime(in_datetime);
                            }
                            else { shippingbillmaster.SBMTIME = DateTime.Now; }
                        }
                        else { shippingbillmaster.SBMTIME = DateTime.Now; }
                    }
                    else { shippingbillmaster.SBMTIME = DateTime.Now; }

                    if (shippingbillmaster.SBMTIME > Convert.ToDateTime(todaydt))
                    {
                        shippingbillmaster.SBMTIME = Convert.ToDateTime(todaydt);
                    }

                    //shippingbillmaster.SBMDATE = Convert.ToDateTime(F_Form["masterdata[0].SBMTIME"]).Date;
                    //shippingbillmaster.SBMTIME = Convert.ToDateTime(F_Form["masterdata[0].SBMTIME"]);

                    shippingbillmaster.CHAID = Convert.ToInt32(F_Form["masterdata[0].CHAID"]);
                    shippingbillmaster.CHANAME = F_Form["masterdata[0].CHANAME"].ToString();
                    shippingbillmaster.EXPRTID = Convert.ToInt32(F_Form["masterdata[0].EXPRTID"]);
                    shippingbillmaster.EXPRTNAME = F_Form["masterdata[0].EXPRTNAME"].ToString();
                    if (SBMID == 0)
                    {
                        using (var trans = context.Database.BeginTransaction())
                        {
                            shippingbillmaster.SBMID = 0;
                            shippingbillmaster.SBMNO = Convert.ToInt32(Autonumber.autonum("shippingbillmaster", "SBMNO", "SBMNO<>0 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());

                            int ano = shippingbillmaster.SBMNO;
                            string prfx = string.Format("{0:D5}", ano);
                            shippingbillmaster.SBMDNO = prfx.ToString();
                            context.shippingbillmasters.Add(shippingbillmaster);
                            context.SaveChanges();
                            SBMID = shippingbilldetail.SBMID; 

                            trans.Commit();
                        }
                    }
                    else
                    {
                        using (var trans = context.Database.BeginTransaction())
                        {
                            context.Entry(shippingbillmaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges(); trans.Commit();
                        }
                    }


                    //-------------Shipping Bill Details
                    string[] F_SBDID = F_Form.GetValues("SBDID");
                    string[] GIDID = F_Form.GetValues("GIDID");
                    string[] SBDDATE = F_Form.GetValues("SBDDATE");
                    string[] ESBMID = F_Form.GetValues("detaildata[0].ESBMID");
                    string[] SBTYPE = F_Form.GetValues("SBTYPE");

                    string[] GDWNID = F_Form.GetValues("detaildata[0].GDWNID");
                    string[] STAGID = F_Form.GetValues("detaildata[0].STAGID");
                    string[] PRDTDESC = F_Form.GetValues("PRDTDESC");
                    string[] TRUCKNO = F_Form.GetValues("TRUCKNO");
                    string[] SBDNOP = F_Form.GetValues("SBDNOP");
                    string[] SBDQTY = F_Form.GetValues("SBDQTY");
                    string[] PRDTGID = F_Form.GetValues("PRDTGID");
                    string[] PRDTTID = F_Form.GetValues("PRDTTID");
                    string[] UNLOADEDNOP = F_Form.GetValues("UNLOADEDNOP");

                    for (int count = 0; count < F_SBDID.Count(); count++)
                    {
                        SBDID = Convert.ToInt32(F_SBDID[count]);
                        if (SBDID != 0)
                        {
                            shippingbilldetail = context.shippingbilldetails.Find(SBDID);
                        }
                        shippingbilldetail.SBMID = shippingbillmaster.SBMID;
                        shippingbilldetail.GIDID = Convert.ToInt32(GIDID[count]);
                        shippingbilldetail.SBDDATE = Convert.ToDateTime(SBDDATE[count]);
                        shippingbilldetail.ESBMID = Convert.ToInt32(ESBMID[count]);
                        shippingbilldetail.SBTYPE = Convert.ToInt16(SBTYPE[count]);
                        shippingbilldetail.GDWNID = Convert.ToInt32(GDWNID[count]);
                        shippingbilldetail.PRDTDESC = PRDTDESC[count].ToString();
                        shippingbilldetail.TRUCKNO = TRUCKNO[count].ToString();
                        shippingbilldetail.SBDNOP = Convert.ToDecimal(SBDNOP[count]);
                        shippingbilldetail.SBDQTY = Convert.ToDecimal(SBDQTY[count]);
                        shippingbilldetail.PRDTGID = Convert.ToInt32(PRDTGID[count]);
                        shippingbilldetail.PRDTTID = Convert.ToInt32(PRDTTID[count]);
                        shippingbilldetail.UNLOADEDNOP = Convert.ToDecimal(UNLOADEDNOP[count]);
                        if (STAGID[count] == "") shippingbilldetail.STAGID = 0;
                        else
                            shippingbilldetail.STAGID = Convert.ToInt32(STAGID[count]);

                        shippingbilldetail.LMUSRID = Session["CUSRID"].ToString();
                        if (SBMID == 0 || shippingbilldetail.CUSRID == null || shippingbilldetail.CUSRID == "" || shippingbilldetail.CUSRID == "1" || shippingbilldetail.CUSRID == "0")
                        {
                            shippingbilldetail.CUSRID = Session["CUSRID"].ToString();
                        }

                        shippingbilldetail.DISPSTATUS = 0;
                        shippingbilldetail.PRCSDATE = DateTime.Now;
                        if (Convert.ToInt32(SBDID) == 0)
                        {
                            using (var trans = context.Database.BeginTransaction())
                            {
                                shippingbilldetail.SBDNO = Convert.ToInt16(autono.autonum("shippingbilldetail", "SBDNO", Session["compyid"].ToString()));
                                int ano = shippingbilldetail.SBDNO;
                                string prfx = string.Format("{0:D5}", ano);
                                shippingbilldetail.SBDDNO = prfx.ToString();
                                context.shippingbilldetails.Add(shippingbilldetail);
                                context.SaveChanges();
                                SBDID = shippingbilldetail.SBDID;


                                //Automatic Truck OUT for Unloading
                                GateOutDetail GOtab = new GateOutDetail();
                                GOtab.GODID = 0;
                                GOtab.GODATE = DateTime.Now.Date;
                                GOtab.GOTIME = DateTime.Now;

                                GOtab.COMPYID = Convert.ToInt32(Session["compyid"]);
                                GOtab.SDPTID = 2;
                                GOtab.REGSTRID = 1;
                                GOtab.TRANDID = 0;
                                GOtab.GOBTYPE = 1;
                                GOtab.LSEALNO = null;
                                GOtab.SSEALNO = null;
                                GOtab.CUSRID = "admin";
                                GOtab.LMUSRID = Session["CUSRID"].ToString();
                                GOtab.PRCSDATE = DateTime.Now;
                                GOtab.EHIDATE = DateTime.Now;
                                GOtab.EHITIME = DateTime.Now;
                                GOtab.CHAID = Convert.ToInt32(F_Form["masterdata[0].CHAID"]);
                                GOtab.CHASNAME = F_Form["masterdata[0].CHANAME"].ToString();
                                GOtab.VHLNO = TRUCKNO[count].ToString();
                                GOtab.GONO = Convert.ToInt32(Autonumber.autonum("GateOutDetail", "GONO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=2").ToString());
                                int agno = GOtab.GONO;
                                string gprfx = string.Format("{0:D5}", agno);
                                GOtab.GODNO = agno.ToString();
                                context.gateoutdetail.Add(GOtab);
                                context.SaveChanges();

                                //EXPORT SHIPPING BILL MASTER ADDITION from Carting Order &  Gate In (Update)
                                GateInDetail egi = new GateInDetail();
                                egi = context.gateindetails.Find(shippingbilldetail.GIDID);
                                context.Entry(egi).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges(); 
                                
                                if (egi.EXPRTRID == 0)
                                {
                                    ExportShippingBillMaster EXPORTSHIPPINGBILLMSTDTL = new ExportShippingBillMaster();
                                    //.......................................export shipping bill insert / modify.........................//
                                    if (Convert.ToInt32(egi.GPSTYPE) == 5 || Convert.ToInt32(egi.GPSTYPE) == 1)
                                    {
                                        if (shippingbilldetail.ESBMID == 0)
                                        {


                                            EXPORTSHIPPINGBILLMSTDTL.CUSRID = Session["CUSRID"].ToString();
                                            EXPORTSHIPPINGBILLMSTDTL.LMUSRID = Session["CUSRID"].ToString();
                                            EXPORTSHIPPINGBILLMSTDTL.PRCSDATE = DateTime.Now;
                                            EXPORTSHIPPINGBILLMSTDTL.COMPYID = Convert.ToInt32(Session["compyid"]);


                                            EXPORTSHIPPINGBILLMSTDTL.ESBMITYPE = 0;
                                            EXPORTSHIPPINGBILLMSTDTL.ESBMIDATE = DateTime.Now;
                                            EXPORTSHIPPINGBILLMSTDTL.CHAID = Convert.ToInt32(shippingbillmaster.CHAID);
                                            EXPORTSHIPPINGBILLMSTDTL.CHANAME = Convert.ToString(shippingbillmaster.CHANAME);
                                            EXPORTSHIPPINGBILLMSTDTL.EXPRTID = Convert.ToInt32(shippingbillmaster.EXPRTID);
                                            EXPORTSHIPPINGBILLMSTDTL.EXPRTNAME = Convert.ToString(shippingbillmaster.EXPRTNAME);
                                            EXPORTSHIPPINGBILLMSTDTL.ESBMREFAMT = 0;
                                            EXPORTSHIPPINGBILLMSTDTL.ESBMFOBAMT = 0;
                                            EXPORTSHIPPINGBILLMSTDTL.ESBMDPNAME = "-";
                                            EXPORTSHIPPINGBILLMSTDTL.PRDTGID = Convert.ToInt32(egi.PRDTGID);
                                            EXPORTSHIPPINGBILLMSTDTL.PRDTDESC = egi.PRDTDESC.ToString();
                                            EXPORTSHIPPINGBILLMSTDTL.ESBMQTY = Convert.ToDecimal(shippingbilldetail.SBDQTY);
                                            EXPORTSHIPPINGBILLMSTDTL.ESBMNOP = Convert.ToDecimal(shippingbilldetail.SBDNOP);

                                            EXPORTSHIPPINGBILLMSTDTL.ESBMDATE = Convert.ToDateTime(shippingbillmaster.SBMDATE);
                                            EXPORTSHIPPINGBILLMSTDTL.ESBMREFNO = Convert.ToString("-");
                                            EXPORTSHIPPINGBILLMSTDTL.ESBMREFDATE = Convert.ToDateTime(shippingbillmaster.SBMDATE);

                                            EXPORTSHIPPINGBILLMSTDTL.DISPSTATUS = 0;
                                            EXPORTSHIPPINGBILLMSTDTL.ESBMNO = Convert.ToInt32(Autonumber.autonum("EXPORTSHIPPINGBILLMASTER", "ESBMNO", "ESBMNO <> 0 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                                            int ano2 = EXPORTSHIPPINGBILLMSTDTL.ESBMNO;
                                            string prfx2 = string.Format("{0:D5}", ano2);
                                            EXPORTSHIPPINGBILLMSTDTL.ESBMDNO = prfx2.ToString();

                                            context.exportshippingbillmasters.Add(EXPORTSHIPPINGBILLMSTDTL);
                                            context.SaveChanges();

                                            egi.ESBNO = EXPORTSHIPPINGBILLMSTDTL.ESBMDNO;
                                            egi.ESBDATE = EXPORTSHIPPINGBILLMSTDTL.ESBMDATE;
                                            egi.ESBMID = EXPORTSHIPPINGBILLMSTDTL.ESBMID;
                                            egi.EXPRTRID = EXPORTSHIPPINGBILLMSTDTL.EXPRTID;
                                            egi.EXPRTRNAME = EXPORTSHIPPINGBILLMSTDTL.EXPRTNAME;

                                            context.Entry(egi).State = System.Data.Entity.EntityState.Modified;
                                            context.SaveChanges();

                                            shippingbilldetail.ESBMID = EXPORTSHIPPINGBILLMSTDTL.ESBMID;

                                            context.Entry(shippingbilldetail).State = System.Data.Entity.EntityState.Modified;
                                            context.SaveChanges();


                                        }
                                    }
                                }
                                trans.Commit();
                            }


                        }
                        else
                        {
                            using (var trans = context.Database.BeginTransaction())
                            {
                                shippingbilldetail.SBDID = SBDID;
                                context.Entry(shippingbilldetail).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();

                                trans.Commit();
                            }
                        }//..............end
                        DELIDS = DELIDS + "," + SBDID.ToString();


                    }
                    context.Database.ExecuteSqlCommand("DELETE FROM shippingbilldetail  WHERE SBMID=" + SBMID + " and  SBDID NOT IN(" + DELIDS.Substring(1) + ")");
                    //trans.Commit(); 
                    Response.Redirect("Index");
                }
                catch (Exception ex)
                {
                    // trans.Rollback();
                    var em = ex.Message;
                    Response.Redirect("/Error/AccessDenied");
                }
            }
            //}
        }
        #endregion

        #region AutocompleteExporterName
        public JsonResult AutoExpoter(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 2).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region AutocompleteCHAName
        public JsonResult AutoCha(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region cascadingslot
        public JsonResult GetSlotNo(int id)
        {
            var group = (from vw in context.stagmasters.Where(x => x.DISPSTATUS == 0) where vw.GDWNID == id select new { vw.STAGID, vw.STAGDESC }).ToList();
            return new JsonResult() { Data = group, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region Execution of stored procedure to get Truck No
        public JsonResult sp_Truck(string id)
        {
            var param = id.Split('~');
            var CHAID = Convert.ToInt32(0);
            var EXPRTID = Convert.ToInt32(0);
            if (param[0] != "")
                CHAID = Convert.ToInt32(param[0]);
            if (param[1] != "")
            EXPRTID = Convert.ToInt32(param[1]);
            var qry = "EXEC  SP_SHIPPINGBILL_GATEINT_TRUCKNO_CBX_ASSGN @PCHAID=" + Convert.ToInt32(CHAID) + ", @PEXPRTID=" + Convert.ToInt32(EXPRTID) + ", @PCUSRID='" + Session["CUSRID"].ToString() + "'";
            var result = context.Database.SqlQuery<SP_SHIPPINGBILL_GATEINT_TRUCKNO_CBX_ASSGN_Result>(qry).Distinct().ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult sp_Truck_TMPADDMOD(int id)
        {
            
           
            context.Database.ExecuteSqlCommand("EXEC TMP_CARTING_ORDER_TRUCK_ADDMOD @PCUSRID='" + Session["CUSRID"] + "',@GIDID=" + id);
            var result = context.Database.SqlQuery<TMP_CARTING_ORDER_TRUCK_DUPCHK>("select * from TMP_CARTING_ORDER_TRUCK_DUPCHK  WHERE  CUSRID='" + Session["CUSRID"] + "'").Distinct().ToList();
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult sp_Truck_TMPDEL()
        {
            context.Database.ExecuteSqlCommand("DELETE TMP_CARTING_ORDER_TRUCK_DUPCHK WHERE  CUSRID='" + Session["CUSRID"] + "'");
            var result = context.Database.SqlQuery<TMP_CARTING_ORDER_TRUCK_DUPCHK>("select * from TMP_CARTING_ORDER_TRUCK_DUPCHK  WHERE  CUSRID='" + Session["CUSRID"] + "'").Distinct().ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAutosp_Truck(string term)
        {
            var param = term.Split('~');
            var avhlno = Convert.ToString(param[0]);
            var CHAID = Convert.ToInt32(0);
            var EXPRTID = Convert.ToInt32(0);
            if (param[1] != "")
                CHAID = Convert.ToInt32(param[1]);
            if (param[2] != "")
                EXPRTID = Convert.ToInt32(param[2]);
            var result = context.Database.SqlQuery<pr_Export_AUTO_TRUCKNO_CBX_ASSGN_Result>("EXEC  pr_Export_AUTO_TRUCKNO_CBX_ASSGN   @FilterTerm='%" + avhlno + "%', @PCHAID = " + Convert.ToInt32(CHAID) + ", @PEXPRTID = " + Convert.ToInt32(EXPRTID)).Distinct().ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Export_details
        public JsonResult Export_details(int id)
        {
            var GIDID = Convert.ToInt32(id);
            var query = context.Database.SqlQuery<Vw_ShippingBill_Export_Gate_In_Ctrl_Assgn>("select * from VW_SHIPPINGBILL_EXPORT_GATE_IN_CTRL_ASSGN where GIDID=" + GIDID).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        #endregion
        
        #region CartingOrderPrintView
        [Authorize(Roles = "ExportCartingOrderPrint")]
        public void CartingOrderPrintView(int? id = 0)
        {

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");

            var sql = "INSERT INTO TMPRPT_IDS(KUSRID, OPTNSTR, RPTID)";
            sql = sql + "SELECT '" + Session["CUSRID"].ToString() + "', 'EXPORT_CARTNG_ORDR' ,SBDID FROM SHIPPINGBILLDETAIL WHERE SBMID=" + id + "";
            context.Database.ExecuteSqlCommand(sql);

            ReportDocument cryRpt = new ReportDocument();
            TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
            TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
            ConnectionInfo crConnectionInfo = new ConnectionInfo();
            Tables CrTables;
            //cryRpt.Load( ConfigurationManager.AppSettings["Reporturl"]+  "Export_CartingOrder.RPT");
            cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Export_CartingOrder.RPT");

            cryRpt.RecordSelectionFormula = "{VW_EXPORT_CARTING_ORDER_DETAIL_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_CARTING_ORDER_DETAIL_PRINT_ASSGN.SBMID} = " + id;
            crConnectionInfo.ServerName = stringbuilder.DataSource;
            crConnectionInfo.DatabaseName = stringbuilder.InitialCatalog;
            //crConnectionInfo.UserID = "ftec";
            //crConnectionInfo.Password = "ftec";
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
            //}

        }
        #endregion
        
        #region UnloadingPrintView
        [Authorize(Roles = "ExportCartingOrderPrint")]
        public void UnloadingPrintView(int? id = 0)
        {

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");

            var sql = "INSERT INTO TMPRPT_IDS(KUSRID, OPTNSTR, RPTID)";
            sql = sql + "SELECT '" + Session["CUSRID"].ToString() + "', 'EXPORT_UNLOADING' ,SBDID FROM SHIPPINGBILLDETAIL WHERE SBMID=" + id + "";
            context.Database.ExecuteSqlCommand(sql);

            ReportDocument cryRpt = new ReportDocument();
            TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
            TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
            ConnectionInfo crConnectionInfo = new ConnectionInfo();
            Tables CrTables;
            //cryRpt.Load( ConfigurationManager.AppSettings["Reporturl"]+  "Export_CartingOrder.RPT");
            cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Export_CartingOrder_Unloading.RPT");

            cryRpt.RecordSelectionFormula = "{VW_EXPORT_TRUCK_UNLOAD_DETAIL_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_TRUCK_UNLOAD_DETAIL_PRINT_ASSGN.SBMID} = " + id;
            crConnectionInfo.ServerName = stringbuilder.DataSource;
            crConnectionInfo.DatabaseName = stringbuilder.InitialCatalog;
            //crConnectionInfo.UserID = "ftec";
            //crConnectionInfo.Password = "ftec";
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
            //}

        }
        #endregion
        
        #region Del        
        [Authorize(Roles = "ExportCartingOrderDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            //String temp = CartngOrdr_Del.delete_check1(fld, id);
            String temp = Delete_fun.delete_check1(fld, id);

            if (temp.Equals("PROCEED"))
            {
                ShippingBillMaster shippingbillmaster = context.shippingbillmasters.Find(Convert.ToInt32(id));
                context.shippingbillmasters.Remove(shippingbillmaster);
                context.SaveChanges();
                Response.Write("Deleted successfully...");
            }
            else
                Response.Write(temp);

        }
        #endregion

        #region TForm
        public ActionResult TForm(string id)
        {
            string[] param = id.Split('~');
            int esbmid = 0; int gidid = 0;
            if (param.Length>0)
            {
                esbmid = Convert.ToInt32(param[0]);
                gidid = Convert.ToInt32(param[1]);
            }
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ExportShippingAdmissionMD vm = new ExportShippingAdmissionMD();
            string tqry = "EXEC pr_Search_Export_Corderdetails @ESBMID =" + esbmid + ", @GIDID = " + gidid;
            vm.details = context.Database.SqlQuery<pr_Search_Export_Corderdetails_Result>(tqry).ToList();
            if (vm.details.Count > 0)
            {
                ViewBag.SBMID = vm.details[0].SBMID;
                ViewBag.GIDID = vm.details[0].GIDID;
                ViewBag.SBMDATE = vm.details[0].SBMDATE;
                ViewBag.SBMDNO = vm.details[0].SBMDNO;
                ViewBag.ESBMID = vm.details[0].ESBMID;
                ViewBag.ESBMIDATE = vm.details[0].ESBMIDATE;
                ViewBag.ESBMDNO = vm.details[0].ESBMDNO;
                ViewBag.VHLNO = vm.details[0].VHLNO;
                ViewBag.SBDQTY = vm.details[0].SBDQTY;
                ViewBag.EXPRTNAME = vm.details[0].EXPRTNAME;
                ViewBag.EXPRTID = vm.details[0].EXPRTID;
                ViewBag.PRDTDESC = vm.details[0].PRDTDESC;
                ViewBag.DESTINATION = vm.details[0].DESTINATION;
                var PRDTGID = context.Database.SqlQuery<Nullable<int>>("select PRDTGID from GATEINDETAIL (nolock) where GIDID=" + vm.details[0].GIDID).ToList();
                if (PRDTGID[0] != null)                    
                    ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", PRDTGID[0]);
                else
                    ViewBag.PRDTGID = new SelectList("");
                
            }

            vm.sbillDetails = context.billdetail.Where(dt => dt.ESBMID == esbmid).ToList();
            if (vm.sbillDetails.Count == 0)
                vm.sbillDetails = null;
            
            return View(vm);
        }
        #endregion

        #region OSForm
        public ActionResult OSForm(int? id = 0)
        {

            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ViewBag.fromDt = DateTime.Now.Date;
            ViewBag.toDt = DateTime.Now.Date;
            ExportShippingAdmission vm = new ExportShippingAdmission();
            vm.details = context.Database.SqlQuery<pr_Search_Export_Corderdetails_Result>("EXEC pr_Search_Export_Corderdetails @ESBMID =" + id).ToList();
            vm.sbillDetails = context.billdetail.Where(dt => dt.ESBMID == id).ToList();
            if (vm.sbillDetails.Count == 0)
                vm.sbillDetails = null;

            //vm.sbillDetailsMultiple = context.Database.SqlQuery<pr_Search_ExportShippingAdmission_Multiple_Result>("EXEC pr_Search_ExportShippingAdmission_Multiple ").ToList();

            ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");
            return View(vm);
        }
        #endregion

        #region sbdMultipleDetail
        public string sbdMultipleDetail(string ids)
        {
            var param = ids.Split('~');
            
            var SDATE = param[0].Split('-');
            var EDATE = param[1].Split('-');
            if (SDATE.Length < 2)
                SDATE = param[0].Split('/');
            if (EDATE.Length < 2)
                EDATE = param[1].Split('/');
            if (SDATE[1].Length == 1)
                SDATE[1] = "0" + SDATE[1];
            if (EDATE[1].Length == 1)
                EDATE[1] = "0" + EDATE[1];
            var sdt = SDATE[2] + '-' + SDATE[1] + "-" + SDATE[0];
            var edt = EDATE[2] + '-' + EDATE[1] + "-" + EDATE[0];
            var CHAID = Convert.ToInt32(param[2]);
            var EXPRTID = Convert.ToInt32(param[3]);
            var qry = "EXEC pr_Search_ExportShippingAdmission_Multiple @PCHAID=" + CHAID + ",@PSDATE='" + sdt + "',@PEDATE='" + edt + "',@PEXPRTID=" + EXPRTID + ", @PCOMPYID = " + Convert.ToInt32(Session["compyid"]);
            var query = context.Database.SqlQuery<pr_Search_ExportShippingAdmission_Multiple_Result>(qry).ToList();

            //<input type='checkbox' id='CHKALL' name='CHKALL' class='CHKALL' onchange='checkall()' style='width:30px'/>
            var tabl = " <div class='panel-heading navbar-inverse' style=color:white>Carting Order Details</div>";
            tabl = tabl + "<Table id=TDETAIL class='table table-striped table-bordered bootstrap-datatable'>";
            tabl = tabl + "<thead><tr><th></th><th class=hide></th>";
            tabl = tabl + "<th>C.Order Date</th>";
            tabl = tabl + "<th>C.Order No</th>";
            tabl = tabl + "<th>S.Bill Date</th>";
            tabl = tabl + "<th>S.Bill No</th>";
            tabl = tabl + "<th>Product</th>";
            tabl = tabl + "<th>Vehicle No</th>";
            tabl = tabl + "<th>Unloaded Nop</th>";
            tabl = tabl + "<th>Weight</th>";
            tabl = tabl + "</tr> </thead>";
            var count = 0;

            foreach (var rslt in query)
            {
                decimal ESBMWGHT = 0;

                if (rslt.ESBMWGHT == null)
                {
                    ESBMWGHT = 0;
                }
                else
                {
                    ESBMWGHT = Convert.ToDecimal(rslt.ESBMWGHT);
                }

                tabl = tabl + "<tbody><tr>";
                tabl = tabl + "<td><input type=checkbox id=CHKBX class=CHKBX name=CHKBX value='' " + count + "  onchange=SelectedCont(this) style='width:30px'><input type=text id=booltype class='hide booltype' name=booltype value=''></td>";
                tabl = tabl + "<td class=hide><input type=text id=GIDATE value=" + rslt.GIDATE + "  class=GIDATE name=GIDATE>";
                tabl = tabl + "<input type=text id=GIDNO value=" + rslt.GIDNO + "  class=GIDNO  name=GIDNO>";
                tabl = tabl + "<input type=text id=SBDQTY value=" + rslt.SBDQTY + "  class=SBDQTY  name=SBDQTY>";
                tabl = tabl + "<input type=text id=SBMID value=" + rslt.SBMID + "  class=SBMID  name=SBMID>";
                tabl = tabl + "<input type=text id=ESBMID value=" + rslt.ESBMID + "  class=ESBMID  name=ESBMID>";
                tabl = tabl + "<input type=text id=ESBMQTY value='0'  class=ESBMQTY  name=ESBMQTY>";
                tabl = tabl + "<input type=text id=ESBDID value='0'  class=ESBDID  name=ESBDID>";
                tabl = tabl + "<input type=text id=ESBDDPNAME value='-'  class=ESBDDPNAME  name=ESBDDPNAME></td>";
                tabl = tabl + "<td><input type=text id=SBMDATE style='width:100px' value='" + rslt.SBMDATE + "' class=SBMDATE name=SBMDATE style='width:40px' readonly='readonly'></td>";
                tabl = tabl + "<td><input type=text id=SBMDNO  style='width:100px' value='" + rslt.SBMDNO + "' class=SBMDNO name=SBMDNO readonly='readonly'></td>";
                tabl = tabl + "<td><input type=text id=ESBMIDATE  style='width:100px' value='" + rslt.ESBMIDATE + "' class=ESBMIDATE name=ESBMIDATE readonly='readonly'></td>";
                tabl = tabl + "<td><input type=text id='ESBMDNO'  style='width:100px' value='" + rslt.ESBMDNO + "' class='ESBMDNO' name='ESBMDNO' style='width:100px'  readonly='readonly'></td>";
                tabl = tabl + "<td><input type=text id=PRDTDESC  style='width:150px' value='" + rslt.PRDTDESC + "' class=PRDTDESC name=PRDTDESC readonly='readonly'></td>";
                tabl = tabl + "<td><input type=text id=VHLNO value='" + rslt.VHLNO + "'  style='width:100px' class=VHLNO name=VHLNO  readonly='readonly'></td>";
                tabl = tabl + "<td><input type=text id=SBDQTY value='" + rslt.SBDQTY + "' class=SBDQTY name=SBDQTY readonly=readonly style='width:75px'></td>";
                tabl = tabl + "<td><input type=text id=ESBMWGHT value='" + ESBMWGHT + "' class=ESBMWGHT name=ESBMWGHT style='width:75px'></td>";
                tabl = tabl + "</tr></tbody>";
                count++;
            }
            tabl = tabl + "</Table>";

            return tabl;

        }
        #endregion

        #region SaveESBdetail
        // [HttpPost]
        public void SaveESBdetail(FormCollection F_Form)
        {
            string sts = "";

            using (context = new SCFSERPContext())
            {
                using (var tran = context.Database.BeginTransaction())
                {

                    try
                    {

                        ExportShippingBillDetail billdetail = new ExportShippingBillDetail();

                        string[] SBMID = F_Form.GetValues("SBMID");
                        string[] GIDID = F_Form.GetValues("GIDID");
                        string[] ESBMID = F_Form.GetValues("ESBMID");
                        string[] EXPRTNAME = F_Form.GetValues("EXPRTNAME");
                        string[] DESTINATION = F_Form.GetValues("DESTINATION");
                        string[] EXPRTID = F_Form.GetValues("EXPRTID");
                        string[] F_ESBDID = F_Form.GetValues("ESBDID");
                        string[] ESBDSBILLNO = F_Form.GetValues("ESBD_SBILLNO");
                        string[] ESBDSBILLDATE = F_Form.GetValues("ESBD_SBILLDATE");
                        string[] ESBOINVNO = F_Form.GetValues("ESBOINVNO");
                        string[] ESBOINVDATE = F_Form.GetValues("ESBOINVDATE");
                        string[] ESBOINVAMT = F_Form.GetValues("ESBOINVAMT");
                        string[] ESBOINVFOBAMT = F_Form.GetValues("ESBOINVFOBAMT");
                        string[] ESBMNOP = F_Form.GetValues("ESBMNOP");
                        string[] ESBMWGHT = F_Form.GetValues("ESBMWGHT");
                        string[] PRDTDESC = F_Form.GetValues("PRDTDESC");
                        string[] PRDTGID = F_Form.GetValues("PRDTGID");

                        string mid = Convert.ToString(ESBMID[0]);

                        string gid = Convert.ToString(GIDID[0]);

                        string sbmId = Convert.ToString(SBMID[0]);

                        string exptr = Convert.ToString(EXPRTNAME[0]);

                        string exptrId = Convert.ToString(EXPRTID[0]);

                        string dest = Convert.ToString(DESTINATION[0]);

                        string Prdtsc = Convert.ToString(PRDTDESC[0]);
                        string PrdTGId = Convert.ToString(PRDTGID[0]);

                        if (Prdtsc == "" || Prdtsc == null)
                        {
                            Prdtsc = "-";
                        }

                        if ((mid != "" || mid != null || mid != "0") && exptr != "")
                        {
                            string osuquery = " UPDATE EXPORTSHIPPINGBILLMASTER SET EXPRTNAME = '" + Convert.ToString(exptr) + "',";
                            osuquery += " EXPRTID = " + Convert.ToInt32(exptrId) + ", ";
                            osuquery += " DESTINATION = '" + Convert.ToString(dest) + "', ";
                            osuquery += " PRDTGID = " + Convert.ToInt32(PrdTGId) + ", ";
                            osuquery += " PRDTDESC = '" + Convert.ToString(Prdtsc) + "' ";
                            osuquery += "  WHERE ESBMID =" + Convert.ToInt32(mid) + " ";
                            context.Database.ExecuteSqlCommand(osuquery);
                        }

                        if ((gid != "" || gid != null || gid != "0")  && exptr !="")
                        {
                            string uquery = " UPDATE GATEINDETAIL SET EXPRTRNAME = '" + Convert.ToString(exptr) + "', ";
                            uquery += " EXPRTRID = " + Convert.ToInt32(exptrId) + " , ";
                            uquery += " PRDTGID = " + Convert.ToInt32(PrdTGId) + ", ";
                            uquery += " PRDTDESC = '" + Convert.ToString(Prdtsc) + "' ";
                            uquery += "  WHERE GIDID =" + Convert.ToInt32(gid) + " AND SDPTID = 2 ";
                            context.Database.ExecuteSqlCommand(uquery);

                        }

                        if ((sbmId != "" || sbmId != null || sbmId != "0") && exptr != "")
                        {
                            string uquery = " UPDATE SHIPPINGBILLMASTER SET EXPRTNAME = '" + Convert.ToString(exptr) + "', ";
                            uquery += " EXPRTID = " + Convert.ToInt32(exptrId) + " ";
                            uquery += "  WHERE SBMID =" + Convert.ToInt32(sbmId) + "  ";
                            context.Database.ExecuteSqlCommand(uquery);
                        }

                        int ESBDID = 0;

                        if (ESBDSBILLNO != null)
                        {
                            for (int count = 0; count < ESBDSBILLNO.Count(); count++)
                            {

                                ESBDID = Convert.ToInt32(F_ESBDID[count]);

                                if (ESBDID != 0)
                                {
                                    billdetail = context.billdetail.Find(ESBDID);
                                }

                                //mid = Convert.ToString(ESBMID[count]);

                                if (mid != null || mid != "0" || mid != "")
                                {
                                    billdetail.ESBMID = Convert.ToInt32(mid);
                                }
                                else { billdetail.ESBMID = 0; }

                                billdetail.GIDID = Convert.ToInt32(gid);
                                billdetail.ESBD_SBILLNO = Convert.ToString(ESBDSBILLNO[count]);
                                billdetail.ESBD_SBILLDATE = Convert.ToDateTime(ESBDSBILLDATE[count]);
                                billdetail.ESBOINVNO = Convert.ToString(ESBOINVNO[count]);
                                billdetail.ESBOINVDATE = Convert.ToDateTime(ESBOINVDATE[count]);
                                billdetail.ESBOINVAMT = Convert.ToDecimal(ESBOINVAMT[count]);
                                billdetail.ESBOINVFOBAMT = Convert.ToDecimal(ESBOINVFOBAMT[count]);
                                billdetail.ESBDDPNAME = "-";
                                billdetail.PRDTDESC = Convert.ToString(Prdtsc);
                                billdetail.ESBMNOP = Convert.ToDecimal(ESBMNOP[count]);
                                billdetail.ESBMWGHT = Convert.ToDecimal(ESBMWGHT[count]);
                                billdetail.ESBMQTY = 0;
                                billdetail.LMUSRID = Session["CUSRID"].ToString();

                                if (ESBDID == 0 || billdetail.CUSRID == null)
                                {
                                    billdetail.CUSRID = Session["CUSRID"].ToString();
                                }
                                billdetail.DISPSTATUS = 0;
                                billdetail.PRCSDATE = DateTime.Now;


                                if (ESBDID == 0)
                                {
                                    context.billdetail.Add(billdetail);
                                    context.SaveChanges();
                                    sts = "Saved";

                                }
                                else
                                {
                                    context.Entry(billdetail).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                    sts = "Saved";

                                }
                            }
                        }

                        tran.Commit(); Response.Redirect("SAIndex");
                        //return Json(sts, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        sts = e.Message.ToString();
                        Response.Redirect("SAIndex");
                        // return Json(sts, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            //Response.Redirect("index");
            //return Json(sts, JsonRequestBehavior.AllowGet); //Response.Write(sts);

        }
        #endregion

        #region SaveESBMultipleData
        public void SaveESBMultipleData(FormCollection F_Form)
        {
            using (context = new SCFSERPContext())
            {
                ExportShippingBillDetail billdetail = new ExportShippingBillDetail();
                string sts = ""; string booltype = "";
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        
                        string[] F_ESBDID = F_Form.GetValues("ESBDID");
                        string[] ESBMID = F_Form.GetValues("ESBMID");
                        string[] ESBMEXPRTNAME = F_Form.GetValues("ESBMEXPRTNAME");
                        string[] DESTINATION = F_Form.GetValues("DESTINATION");
                        string[] ESBMEXPRTID = F_Form.GetValues("ESBMEXPRTID");
                        string[] ESBD_SBILLNO = F_Form.GetValues("ESBD_SBILLNO");
                        string[] ESBD_SBILLDATE = F_Form.GetValues("ESBD_SBILLDATE");
                        string[] ESBOINVNO = F_Form.GetValues("ESBOINVNO");
                        string[] ESBOINVDATE = F_Form.GetValues("ESBOINVDATE");
                        string[] ESBOINVAMT = F_Form.GetValues("ESBOINVAMT");
                        string[] ESBOINVFOBAMT = F_Form.GetValues("ESBOINVFOBAMT");
                        string[] ESBDDPNAME = F_Form.GetValues("ESBDDPNAME");
                        string[] PRDTDESC = F_Form.GetValues("PRDTDESC");
                        string[] ESBMNOP = F_Form.GetValues("ESBMNOP");
                        string[] ESBMQTY = F_Form.GetValues("ESBMQTY");
                        string[] ESBMWGHT = F_Form.GetValues("ESBMWGHT");
                        string[] F_booltype = F_Form.GetValues("booltype");
                        Int32 ESBDID = 0;
                        string[] GIDID = F_Form.GetValues("GIDID");
                        string gid = Convert.ToString(GIDID[0]);

                        string mid = Convert.ToString(ESBMID[0]);

                        string exptr = Convert.ToString(ESBMEXPRTNAME[0]);

                        string exptrId = Convert.ToString(ESBMEXPRTID[0]);

                        string dest = Convert.ToString(DESTINATION[0]);


                        if ((mid != "" || mid != null || mid != "0") && exptr != "")
                        {
                            string osuquery = " UPDATE EXPORTSHIPPINGBILLMASTER SET EXPRTNAME = '" + Convert.ToString(exptr) + "',";
                            osuquery += " EXPRTID = " + Convert.ToInt32(exptrId) + " ,";
                            osuquery += " DESTINATION = '" + Convert.ToString(dest) + "' ";
                            osuquery += "  WHERE ESBMID =" + Convert.ToInt32(mid) + " ";
                            context.Database.ExecuteSqlCommand(osuquery);
                        }

                        for (int count = 0; count < F_ESBDID.Count(); count++)
                        {
                            ESBDID = Convert.ToInt32(F_ESBDID[count]);
                            booltype = F_booltype[count].ToString();
                            if (booltype == "true")
                            {
                                if (ESBDID != 0)
                                {
                                    billdetail = context.billdetail.Find(ESBDID);
                                }
                                if (ESBDID == 0 || billdetail.CUSRID == null || billdetail.CUSRID == "" || billdetail.CUSRID == "1" || billdetail.CUSRID == "0")
                                {
                                    billdetail.CUSRID = Session["CUSRID"].ToString();
                                }
                                billdetail.GIDID = Convert.ToInt32(gid);
                                billdetail.DISPSTATUS = 0;
                                billdetail.PRCSDATE = DateTime.Now;
                                billdetail.ESBMID = Convert.ToInt32(ESBMID[count]);
                                //billdetail.ESBDID = ESBDID;
                                billdetail.ESBD_SBILLNO = ESBD_SBILLNO[0].ToString();
                                billdetail.ESBD_SBILLDATE = Convert.ToDateTime(ESBD_SBILLDATE[0]);
                                billdetail.ESBMID = Convert.ToInt32(ESBMID[count]);
                                billdetail.ESBOINVNO = ESBOINVNO[0].ToString();
                                billdetail.ESBOINVDATE = Convert.ToDateTime(ESBOINVDATE[0]);
                                billdetail.ESBOINVAMT = Convert.ToDecimal(ESBOINVAMT[0]);
                                billdetail.ESBOINVFOBAMT = Convert.ToDecimal(ESBOINVFOBAMT[0]);
                                billdetail.ESBDDPNAME = ESBDDPNAME[count].ToString();
                                billdetail.PRDTDESC = PRDTDESC[count].ToString();
                                billdetail.ESBMNOP = Convert.ToDecimal(ESBMNOP[0]);
                                billdetail.ESBMQTY = Convert.ToDecimal(ESBMQTY[0]);
                                billdetail.ESBMWGHT = Convert.ToDecimal(ESBMWGHT[count]);
                                billdetail.DISPSTATUS = 0;
                                billdetail.PRCSDATE = DateTime.Now;
                                billdetail.LMUSRID = Session["CUSRID"].ToString();
                                if (ESBDID == 0)
                                {
                                    context.billdetail.Add(billdetail);
                                    context.SaveChanges();
                                    sts = "Saved";

                                }
                                else
                                {
                                    context.Entry(billdetail).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                    sts = "Update";

                                }

                            }

                        }
                        trans.Commit(); Response.Redirect("SAIndex");
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        sts = ex.Message;
                    }

                }
            }
            Response.Redirect("SAIndex");
        }
        #endregion

        #region Del_det
        public void Del_det()
        {
            using (SCFSERPContext context = new SCFSERPContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        String id = Request.Form.Get("id");

                        String fld = "ExportShippingBillDetail";

                        string temp = "";

                        if ((id != "0") && (fld == "ExportShippingBillDetail"))
                        {
                            temp = "PROCEED";
                        }

                        if (temp.Equals("PROCEED"))
                        {

                            if ((id != null) || (id != "0") || (id != ""))
                            {
                                ExportShippingBillDetail exportshippingbilldetails = context.billdetail.Find(Convert.ToInt32(id));
                                context.billdetail.Remove(exportshippingbilldetails);
                                context.SaveChanges();
                                Response.Write("Deleted Successfully ...");

                            }
                        }
                        else
                            Response.Write(temp);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback(); //Response.Redirect("/Error/SavepointErr");
                        Response.Write("Sorry !!!An Error Occurred");
                    }
                }
            }
        }
        #endregion
    }

}