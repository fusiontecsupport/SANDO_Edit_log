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

namespace scfs_erp.Controllers.Export
{
    [SessionExpire]
    public class ExportShippingBillMasterController : Controller
    {
        // GET: ExportShippingBillMaster

        #region contextdeclaration
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        #endregion

        #region Indexpage
        [Authorize(Roles = "ExportShippingBillIndex")]
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
            Session["ESBDIdx"] = "SB";
            DateTime sd = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;
            DateTime ed = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;
            return View();
        }
        #endregion

        #region GetAjaxData
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)/*model 22.edmx*/
       {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Export_ShippingBillMasterDetails(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(Session["compyid"]));
                var aaData = data.Select(d => new string[] { d.ESBMDATE.Value.ToString("dd/MM/yyyy"), d.ESBMDNO.ToString(), d.EXPRTNAME.ToString(), d.CHANAME, d.VHLNO, d.ESBMNOP.ToString(), d.ESBMQTY.ToString(), d.ESBMID.ToString(), d.SAFlg.ToString(),d.GIDID.ToString() }).ToArray();
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
        // GET: ExportShippingBillMaster/Details/5
        [Authorize(Roles = "ExportShippingBillEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ExportShippingBillMaster/Form/" + id);
        }
        #endregion

        #region Cretae Or Modify Page View
        [Authorize(Roles = "ExportShippingBillCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ExportShippingBillMaster tab = new ExportShippingBillMaster();
            tab.ESBMID = 0;
            tab.ESBMDATE = DateTime.Now;
            tab.ESBMREFDATE = DateTime.Now;

            ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");
            List<SelectListItem> selectedESBMITYPE = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "NO", Value = "0", Selected = false };
            selectedESBMITYPE.Add(selectedItem);
            ViewBag.ESBMITYPE = selectedESBMITYPE;

            if (id != 0)//Edit Mode
            {
                tab = context.exportshippingbillmasters.Find(id);
                if (Convert.ToInt32(tab.ESBMITYPE) == 1)
                {
                    List<SelectListItem> selectedESBMITYPE1 = new List<SelectListItem>();
                    SelectListItem selectedItemGPS = new SelectListItem { Text = "No", Value = "0", Selected = false };
                    selectedESBMITYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "Yes", Value = "1", Selected = true };
                    selectedESBMITYPE1.Add(selectedItemGPS);
                    ViewBag.ESBMITYPE = selectedESBMITYPE1;
                }

                ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", tab.PRDTGID);

            }
            return View(tab);
        }
        #endregion

        #region Insert or Modify data
        public void savedata(ExportShippingBillMaster tab)
        {
            using (SCFSERPContext context = new SCFSERPContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
                        tab.LMUSRID = Session["CUSRID"].ToString();
                        tab.PRCSDATE = DateTime.Now;
                        tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                        tab.ESBMQTY = Convert.ToDecimal(Request.Form.Get("ESBMQTY"));
                        tab.DISPSTATUS = 0;
                        if (Convert.ToString(tab.ESBMREFAMT) == "" || Convert.ToString(tab.ESBMREFAMT) == null)
                        {
                            tab.ESBMREFAMT = 0;
                        }

                        if (Convert.ToString(tab.ESBMFOBAMT) == "" || Convert.ToString(tab.ESBMFOBAMT) == null)
                        {
                            tab.ESBMFOBAMT = 0;
                        }
                        if (tab.ESBMITYPE == 0) tab.ESBMIDATE = DateTime.Now;
                        if ((tab.ESBMID).ToString() != "0")
                        {
                            context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                        else
                        {
                            context.exportshippingbillmasters.Add(tab);
                            context.SaveChanges();
                        }
                        trans.Commit(); Response.Redirect("Index");
                    }
                    catch
                    {
                        trans.Rollback();
                        Response.Redirect("/Error/AccessDenied");
                    }
                }
            }
        }
        #endregion

        #region CHA Autocomplete
        public JsonResult AutoCha(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Expoter Autocomplete
        public JsonResult AutoExporter(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 2).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Duplicate Check for no
        public void NoCheck()
        {
            string ESBMDNO = Request.Form.Get("No");
            using (var contxt = new SCFSERPContext())
            {
                var query = contxt.Database.SqlQuery<string>("select ESBMDNO from ExportShippingbillmaster where  ESBMDNO='" + ESBMDNO + "' ").ToList();

                var No = query.Count();
                if (No != 0)
                {
                    Response.Write("Entered shipping bill No. already exists");
                }

            }

        }
        #endregion

        #region cancel Record
        [Authorize(Roles = "ExportShippingBillDelete")]
        public void CancelBill()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                ExportShippingBillMaster exportshippingbillmasters = context.exportshippingbillmasters.Find(Convert.ToInt32(id));
                context.Entry(exportshippingbillmasters).Entity.DISPSTATUS = 1;
                context.SaveChanges();
                Response.Write("Cancelled Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion

        #region Delete Record
        [Authorize(Roles = "ExportShippingBillDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                ExportShippingBillMaster exportshippingbillmasters = context.exportshippingbillmasters.Find(Convert.ToInt32(id));
                context.exportshippingbillmasters.Remove(exportshippingbillmasters);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion

        #region BForm
        public ActionResult BForm(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            string[] ids = id.Split('~');
            int esbmid = 0;
            int gidid = 0;
            if (ids[0] != "")
                esbmid = Convert.ToInt32(ids[0]);
            if (ids[1] != "")
                gidid = Convert.ToInt32(ids[1]);
            ExportShippingAdmissionMD vm = new ExportShippingAdmissionMD();
            vm.details = context.Database.SqlQuery<pr_Search_Export_Corderdetails_Result>("EXEC pr_Search_Export_Corderdetails @ESBMID =" + esbmid+ ",@GIDID = " + gidid).ToList();
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
            }


            vm.sbillDetails = context.billdetail.Where(dt => dt.ESBMID == esbmid).ToList();
            if (vm.sbillDetails.Count == 0)
                vm.sbillDetails = null;
            ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");
            return View(vm);

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

                        string mid = Convert.ToString(ESBMID[0]);

                        string gid = Convert.ToString(GIDID[0]);

                        string sbmId = Convert.ToString(SBMID[0]);

                        string exptr = Convert.ToString(EXPRTNAME[0]);

                        string exptrId = Convert.ToString(EXPRTID[0]);

                        string dest = Convert.ToString(DESTINATION[0]);

                        string Prdtsc = Convert.ToString(PRDTDESC[0]);

                        if (Prdtsc == "" || Prdtsc == null)
                        {
                            Prdtsc = "-";
                        }

                        if ((mid != "" || mid != null || mid != "0") && exptr !="")
                        {
                            string osuquery = " UPDATE EXPORTSHIPPINGBILLMASTER SET EXPRTNAME = '" + Convert.ToString(exptr) + "',";
                            osuquery += " EXPRTID = " + Convert.ToInt32(exptrId) + ", ";
                            osuquery += " DESTINATION = '" + Convert.ToString(dest) + "' ";
                            osuquery += "  WHERE ESBMID =" + Convert.ToInt32(mid) + " ";
                            context.Database.ExecuteSqlCommand(osuquery);
                        }

                        if ((gid != "" || gid != null || gid != "0") && exptr !="")
                        {
                            string uquery = " UPDATE GATEINDETAIL SET EXPRTRNAME = '" + Convert.ToString(exptr) + "', ";
                            uquery += " EXPRTRID = " + Convert.ToInt32(exptrId) + " ";
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

                        tran.Commit(); Response.Redirect("index");
                        //return Json(sts, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        sts = e.Message.ToString();
                        Response.Redirect("index");
                        // return Json(sts, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            //Response.Redirect("index");
            //return Json(sts, JsonRequestBehavior.AllowGet); //Response.Write(sts);

        }
        #endregion

        #region Stuffing Check
        public JsonResult StuffingCheck(string id)
        {

            int ESBMID = 0;
            if (id != "" || id != null)
            {
                ESBMID = Convert.ToInt32(id);
            }

            string sqry = "select *from VW_STUFFING_CHECKEXISTING_ORGSHIPPINGBILL where ESBMID= " + ESBMID + " ";
            var data = context.Database.SqlQuery<VW_STUFFING_CHECKEXISTING_ORGSHIPPINGBILL>(sqry).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
