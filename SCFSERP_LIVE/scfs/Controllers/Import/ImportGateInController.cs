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
using System.Data.Entity;
using System.Reflection;

namespace scfs_erp.Controllers.Import
{
    public class ImportGateInController : Controller
    {
        // GET: ImportGateIn
        #region Context declaration
        SCFSERPContext context = new SCFSERPContext();

        #endregion

        #region Index Form
        [Authorize(Roles = "ImportGateInIndex")]
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
            DateTime sd = Convert.ToDateTime(Session["SDATE"]).Date;
            DateTime ed = Convert.ToDateTime(Session["EDATE"]).Date;

            DateTime fromdate = Convert.ToDateTime(Session["SDATE"]).Date;
            DateTime todate = Convert.ToDateTime(Session["EDATE"]).Date;


            TotalContainerDetails(fromdate, todate);


            return View(context.gateindetails.Where(x => x.GIDATE >= sd).Where(x => x.GIDATE <= ed).Where(x => x.SDPTID == 1).Where(x => x.CONTNRID >= 1).ToList());
        }
        #endregion

        #region Get data from database

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {

            using (var e = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Import_GateInGridAssgn(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToDateTime(Session["SDATE"]),
                                                Convert.ToDateTime(Session["EDATE"]),
                                                Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]));
                var aaData = data.Select(d => new string[] { d.GIDATE.Value.ToString("dd/MM/yyyy"), d.GIDNO,
                    d.CONTNRNO, d.CONTNRSID, d.IGMNO, d.GPLNO, d.IMPRTNAME, d.STMRNAME,  d.VSLNAME, d.BLNO,
                    d.PRDTDESC, d.DISPSTATUS, d.GIDID.ToString() }).ToList();

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

            context.Database.CommandTimeout = 0;
            var result = context.Database.SqlQuery<PR_IMPORT_DASHBOARD_DETAILS_Result>("EXEC PR_IMPORT_DASHBOARD_DETAILS @PFDT='" + fdate + "',@PTDT='" + tdate + "',@PSDPTID=" + 1).ToList();

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

        #region Redirect to form from index
        [Authorize(Roles = "ImportGateInEdit")]
        public void Edit(string id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/ImportGateIn/Form/" + id);

            //Response.Redirect("/ImportGateIn/Form/" + id);
        }
        #endregion

        #region Creating or Modify Form
        [Authorize(Roles = "ImportGateInCreate")]
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            RemoteGateIn remotegatein = new RemoteGateIn();
            GateInDetail tab = new GateInDetail();

            string ginty = "";

            var GID = 0; var RGId = 0;
            if (id.Contains(';'))
            {
                var param = id.Split(';');
                RGId = Convert.ToInt32(param[0]);
                GID = Convert.ToInt32(param[1]);
                ginty = "RGateIn";
                Session["GTY"] = ginty;
            }
            else { GID = Convert.ToInt32(id); RGId = 0; ginty = "GateIn"; Session["GTY"] = ginty; }

            //tab.GITIME = DateTime.Now;
            //tab.GICCTLTIME = DateTime.Now;
            //tab.IGMDATE = DateTime.Now.Date;

            tab.GIDATE = DateTime.Now.Date;
            tab.GITIME = DateTime.Now;
            tab.GITIME = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);

            if (RGId > 0)
            {
                remotegatein = context.remotegateindetails.Find(RGId);
                tab.RGIDID = RGId;
                //tab.RGIDID = remotegatein.GIDID;
                tab.VSLNAME = remotegatein.VSLNAME;
                tab.VSLID = remotegatein.VSLID;
                tab.GINO = remotegatein.GINO;
                tab.GIDNO = remotegatein.GIDNO;
                tab.GICCTLDATE = Convert.ToDateTime(remotegatein.GICCTLDATE).Date;
                tab.GICCTLTIME = Convert.ToDateTime(remotegatein.GICCTLTIME);
                //tab.GITIME = remotegatein.GITIME;
                tab.VOYNO = remotegatein.VOYNO;
                tab.GPLNO = remotegatein.GPLNO;
                tab.IGMNO = remotegatein.IGMNO;
                tab.IMPRTNAME = remotegatein.IMPRTNAME;
                tab.IMPRTID = remotegatein.IMPRTID;
                tab.STMRNAME = remotegatein.STMRNAME;
                tab.STMRID = remotegatein.STMRID;
                tab.CONTNRNO = remotegatein.CONTNRNO;
                tab.CONTNRSID = remotegatein.CONTNRSID;
                tab.CONTNRTID = remotegatein.CONTNRTID;
                tab.PRDTDESC = remotegatein.PRDTDESC;
                tab.PRDTGID = remotegatein.PRDTGID;
                tab.GPWGHT = remotegatein.GPWGHT;//-----------
                tab.GPEAMT = remotegatein.GPEAMT;
                tab.GPAAMT = remotegatein.GPAAMT;
                tab.IGMDATE = remotegatein.IGMDATE;
                tab.BLNO = remotegatein.BLNO;
                tab.GIISOCODE = remotegatein.GIISOCODE;

                //ViewBag.ROWID = new SelectList(context.rowmasters.Where(x => x.DISPSTATUS == 0), "ROWID", "ROWDESC");
                ViewBag.ROWID = new SelectList(context.rowmasters.Where(x => x.DISPSTATUS == 0), "ROWID", "ROWDESC", 6);
                //ViewBag.SLOTID = new SelectList(context.slotmasters.Where(x => x.DISPSTATUS == 0), "SLOTID", "SLOTDESC");
                ViewBag.SLOTID = new SelectList(context.slotmasters.Where(x => x.DISPSTATUS == 0), "SLOTID", "SLOTDESC", 6);
                ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", remotegatein.PRDTGID);
                ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CONTNRTDESC), "CONTNRTID", "CONTNRTDESC", remotegatein.CONTNRTID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID > 1).Where(x => x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC", remotegatein.CONTNRSID);
                ViewBag.GPPTYPE = new SelectList(context.porttypemaster, "GPPTYPE", "GPPTYPEDESC", remotegatein.GPPTYPE);
                ViewBag.GPMODEID = new SelectList(context.gpmodemasters.Where(x => x.DISPSTATUS == 0 && x.GPMODEID != 5 && x.GPMODEID != 4).OrderBy(x => x.GPMODEDESC), "GPMODEID", "GPMODEDESC", remotegatein.GPMODEID);
            }
            else
            {
                tab.GIDATE = DateTime.Now.Date;
                tab.GITIME = DateTime.Now;
                tab.GITIME = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);
                tab.GICCTLDATE = DateTime.Now.Date;
                tab.GICCTLTIME = DateTime.Now;

                //-------------------Dropdown List--------------------------------------------------//
                //ViewBag.ROWID = new SelectList(context.rowmasters.Where(x => x.DISPSTATUS == 0), "ROWID", "ROWDESC");
                ViewBag.ROWID = new SelectList(context.rowmasters.Where(x => x.DISPSTATUS == 0), "ROWID", "ROWDESC", 6);
                //ViewBag.SLOTID = new SelectList(context.slotmasters.Where(x => x.DISPSTATUS == 0), "SLOTID", "SLOTDESC");
                ViewBag.SLOTID = new SelectList(context.slotmasters.Where(x => x.DISPSTATUS == 0), "SLOTID", "SLOTDESC", 6);
                ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");
                ViewBag.GPPTYPE = new SelectList(context.porttypemaster, "GPPTYPE", "GPPTYPEDESC", tab.GPPTYPE);
                ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CONTNRTDESC), "CONTNRTID", "CONTNRTDESC");
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.DISPSTATUS == 0 && x.CONTNRSID > 1), "CONTNRSID", "CONTNRSDESC");
                ViewBag.GPMODEID = new SelectList(context.gpmodemasters.Where(x => x.DISPSTATUS == 0 && x.GPMODEID != 5 && x.GPMODEID != 4).OrderBy(x => x.GPMODEDESC), "GPMODEID", "GPMODEDESC", 2);
            }

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

            List<SelectListItem> selectedGPSCNMTYPE = new List<SelectListItem>();
            SelectListItem selectedItemMtype1 = new SelectListItem { Text = "MISMATCH", Value = "1", Selected = false };
            selectedGPSCNMTYPE.Add(selectedItemMtype1);
            selectedItemMtype1 = new SelectListItem { Text = "CLEAN", Value = "2", Selected = false };
            selectedGPSCNMTYPE.Add(selectedItemMtype1);
            selectedItemMtype1 = new SelectListItem { Text = "NOT SCANNED", Value = "3", Selected = false };
            selectedGPSCNMTYPE.Add(selectedItemMtype1);
            ViewBag.GPSCNMTYPE = selectedGPSCNMTYPE;

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

            if (GID != 0)//--Edit Mode
            {
                tab = context.gateindetails.Find(GID);

                ginty = "GateIn"; Session["GTY"] = ginty;

                //----------------------selected values in dropdown list------------------------------------//
                ViewBag.SLOTID = new SelectList(context.slotmasters.Where(x => x.DISPSTATUS == 0), "SLOTID", "SLOTDESC", tab.SLOTID);
                ViewBag.ROWID = new SelectList(context.rowmasters.Where(x => x.DISPSTATUS == 0), "ROWID", "ROWDESC", tab.ROWID);
                ViewBag.GPPTYPE = new SelectList(context.porttypemaster, "GPPTYPE", "GPPTYPEDESC", tab.GPPTYPE);
                ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", tab.PRDTGID);
                ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CONTNRTDESC), "CONTNRTID", "CONTNRTDESC", tab.CONTNRTID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.DISPSTATUS == 0 && x.CONTNRSID > 1), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.GPMODEID = new SelectList(context.gpmodemasters.Where(x => x.DISPSTATUS == 0 && x.GPMODEID != 5 && x.GPMODEID != 4).OrderBy(x => x.GPMODEDESC), "GPMODEID", "GPMODEDESC", tab.GPMODEID);

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


                List<SelectListItem> selectedGPSCNMTYPE2 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GPSCNMTYPE) == 1)
                {
                    SelectListItem selectedItemsts14 = new SelectListItem { Text = "MISMATCH", Value = "1", Selected = true };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                    selectedItemsts14 = new SelectListItem { Text = "CLEAN", Value = "2", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                    selectedItemsts14 = new SelectListItem { Text = "NOT SCANNED", Value = "3", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);

                }
                else if (Convert.ToInt32(tab.GPSCNMTYPE) == 2)
                {
                    SelectListItem selectedItemsts14 = new SelectListItem { Text = "MISMATCH", Value = "1", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                    selectedItemsts14 = new SelectListItem { Text = "CLEAN", Value = "2", Selected = true };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                    selectedItemsts14 = new SelectListItem { Text = "NOT SCANNED", Value = "3", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                }
                else if (Convert.ToInt32(tab.GPSCNMTYPE) == 3)
                {
                    SelectListItem selectedItemsts14 = new SelectListItem { Text = "MISMATCH", Value = "1", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                    selectedItemsts14 = new SelectListItem { Text = "CLEAN", Value = "2", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                    selectedItemsts14 = new SelectListItem { Text = "NOT SCANNED", Value = "3", Selected = true };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                }
                else 
                {
                    SelectListItem selectedItemsts14 = new SelectListItem { Text = "MISMATCH", Value = "1", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                    selectedItemsts14 = new SelectListItem { Text = "CLEAR", Value = "2", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                    selectedItemsts14 = new SelectListItem { Text = "NOT SCANNED", Value = "3", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                }
                ViewBag.GPSCNMTYPE = selectedGPSCNMTYPE2;


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

        #region Save data
        [HttpPost]
        public void saveidata(GateInDetail tab)
        {
            var R_GIDID = Request.Form.Get("R_GIDID");

            string todaydt = Convert.ToString(DateTime.Now);
            string todayd = Convert.ToString(DateTime.Now.Date);

            tab.PRCSDATE = DateTime.Now;
            //tab.GIDATE = Convert.ToDateTime(tab.GIDATE).Date;// DateTime.Now.Date;
            //tab.GITIME = Convert.ToDateTime(tab.GITIME);
            //tab.GITIME = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);
            //tab.GICCTLTIME = Convert.ToDateTime(tab.GICCTLTIME);
            //tab.GICCTLDATE = Convert.ToDateTime(tab.GICCTLDATE);
            tab.CONTNRID = 1;
            tab.YRDID = 1;
            tab.COMPYID = Convert.ToInt32(Session["compyid"]);
            tab.SDPTID = 1;
            tab.AVHLNO = tab.VHLNO;
            tab.ESBDATE = DateTime.Now;
            tab.DISPSTATUS = tab.DISPSTATUS;
            tab.EXPRTRID = 0;
            tab.EXPRTRNAME = "-";
            tab.BCHAID = 0;
            tab.UNITID = 0;

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
                else
                {
                    var in_time = intime;
                    var in_date = indate;

                    if ((in_time.Contains(':')) && (in_date.Contains('/')))
                    {
                        var in_time1 = in_time.Split(':');
                        var in_date1 = in_date.Split('/');

                        string in_datetime = in_date1[2] + "-" + in_date1[1] + "-" + in_date1[0] + "  " + in_time1[0] + ":" + in_time1[1] + ":" + in_time1[2];

                        tab.GITIME = Convert.ToDateTime(in_datetime);
                    }
                    else { tab.GITIME = DateTime.Now; }
                }
            }
            else { tab.GITIME = DateTime.Now; }

            if (tab.GITIME > Convert.ToDateTime(todaydt))
            {
                tab.GITIME = Convert.ToDateTime(todaydt);
            }

            // GATE IN CCT DATE AND TIME
            string cctindate = Convert.ToString(tab.GICCTLDATE);
            if (cctindate != null || cctindate != "")
            {
                tab.GICCTLDATE = Convert.ToDateTime(cctindate).Date;
            }
            else { tab.GICCTLDATE = DateTime.Now.Date; }

            if (tab.GICCTLDATE > Convert.ToDateTime(todayd))
            {
                tab.GICCTLDATE = Convert.ToDateTime(todayd);
            }

            string cctintime = Convert.ToString(tab.GICCTLTIME);
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

                        tab.GICCTLTIME = Convert.ToDateTime(CCTin_datetime);
                    }
                    else { tab.GICCTLTIME = DateTime.Now; }
                }
                else { tab.GICCTLTIME = DateTime.Now; }
            }
            else { tab.GICCTLTIME = DateTime.Now; }

            if (tab.GICCTLTIME > Convert.ToDateTime(todaydt))
            {
                tab.GICCTLTIME = Convert.ToDateTime(todaydt);
            }

            if (tab.CUSRID == "" || tab.CUSRID == null)
            {
                if (Session["CUSRID"] != null)
                {
                    tab.CUSRID = Session["CUSRID"].ToString();
                }
                else { tab.CUSRID = ""; }
            }

            if (tab.GPSCNTYPE == 0)
            {
                tab.GPSCNMTYPE = 0;
            }

            tab.LMUSRID = Session["CUSRID"].ToString();
            if (tab.GIDID.ToString() != "0")
            {
                // Load original row for logging (no tracking to avoid state conflicts)
                var original = context.gateindetails.AsNoTracking().FirstOrDefault(x => x.GIDID == tab.GIDID);

                context.Entry(tab).Entity.NGIDID = tab.GIDID + 1;
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                // Best-effort logging to SCFS_LOG
                try
                {
                    LogGateInEdits(original, tab, Session["CUSRID"] != null ? Session["CUSRID"].ToString() : "");
                }
                catch { }

                Response.Write("saved");
            }

            else
            {

                string sqry = "SELECT *FROM GATEINDETAIL  WHERE VOYNO='" + tab.VOYNO + "' And IGMNO='" + tab.IGMNO + "' ";
                sqry += " And GPLNO='" + tab.GPLNO + "' And CONTNRNO ='" + tab.CONTNRNO + "' And COMPYID=" + tab.COMPYID + " And SDPTID=1";
                var sl = context.Database.SqlQuery<GateInDetail>(sqry).ToList();

                if (sl.Count > 0)
                {
                    Response.Write("exists");
                }
                else
                {
                    string sqry1 = "SELECT *FROM GATEINDETAIL  WHERE VOYNO='" + tab.VOYNO + "' And IGMNO='" + tab.IGMNO + "' ";
                    sqry1 += " And GPLNO='" + tab.GPLNO + "' And CONTNRNO ='" + tab.CONTNRNO + "' And COMPYID=" + tab.COMPYID + " And SDPTID=1";
                    var sl1 = context.Database.SqlQuery<GateInDetail>(sqry1).ToList();

                    if (sl1.Count > 0)

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
                        remotegatein.AGIDID = tab.GIDID;

                        //context.remotegateindetails.Remove(remotegatein);
                        context.SaveChanges();

                    }/*end*/

                    //-----second record-----------------------------// 
                    tab.GITIME = tab.GITIME;
                    tab.TRNSPRTNAME = tab.TRNSPRTNAME;
                    tab.AVHLNO = tab.VHLNO;
                    tab.DRVNAME = tab.DRVNAME;
                    tab.GPREFNO = tab.GPREFNO;
                    tab.IMPRTID = tab.IMPRTID;// 0;
                    tab.IMPRTNAME = tab.IMPRTNAME;//  "-";
                    tab.STMRID = tab.STMRID;// 0;
                    tab.STMRNAME = tab.STMRNAME;// "-";
                    tab.CONDTNID = 0;
                    tab.CONTNRNO = "-";
                    tab.CONTNRTID = 0;
                    tab.CONTNRID = 0;
                    tab.CONTNRSID = 0;
                    tab.LPSEALNO = "-";
                    tab.CSEALNO = "-";
                    tab.YRDID = 0;
                    tab.VSLID = 0;
                    tab.VSLNAME = "-";
                    tab.VOYNO = "-";
                    tab.PRDTGID = 0;
                    tab.PRDTDESC = "-";
                    tab.UNITID = 0;
                    tab.GPLNO = "-";
                    tab.GPWGHT = 0;
                    tab.GPEAMT = 0;
                    tab.GPAAMT = 0;
                    tab.IGMNO = tab.IGMNO;
                    tab.GIISOCODE = tab.GIISOCODE;
                    tab.GIDMGDESC = tab.GIDMGDESC;
                    tab.GPWTYPE = 0;
                    tab.GPSTYPE = 0;
                    tab.RGIDID = 0;
                    tab.GPETYPE = 0;
                    tab.ROWID = 0;
                    tab.SLOTID = 0;
                    tab.DISPSTATUS = 0;
                    tab.GIVHLTYPE = 0;
                    tab.TRNSPRTID = 0;
                    tab.NGIDID = 0;
                    tab.BOEDID = tab.GIDID;
                    tab.BLNO = tab.BLNO;
                    tab.BOENO = tab.BOENO;
                    tab.CFSNAME = tab.CFSNAME;


                    var GINO = Request.Form.Get("GINO");

                    //if (context.gateindetails.Where(u => u.GINO == Convert.ToInt32(GINO)).Count()!=0) 
                    tab.GINO = Convert.ToInt32(Autonumber.autonum("gateindetail", "GINO", "GINO <> 0 AND SDPTID = 1 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                    //  else tab.GINO = 1;
                    int anoo = tab.GINO;
                    string prfxx = string.Format("{0:D5}", anoo);
                    tab.GIDNO = prfxx.ToString();
                    context.gateindetails.Add(tab);
                    context.SaveChanges();

                }
                Response.Write("saved");
                //Response.Redirect("Index");
            }
        }

        public void savedata(GateInDetail tab)
        {
            var R_GIDID = Request.Form.Get("R_GIDID");

            string todaydt = Convert.ToString(DateTime.Now);
            string todayd = Convert.ToString(DateTime.Now.Date);

            tab.PRCSDATE = DateTime.Now;
            tab.GIDATE = Convert.ToDateTime(tab.GIDATE).Date;
            tab.GITIME = Convert.ToDateTime(tab.GITIME);
            tab.GITIME = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);
            tab.GICCTLDATE = Convert.ToDateTime(tab.GICCTLTIME).Date;
            tab.CONTNRID = 1;
            tab.YRDID = 1;
            tab.COMPYID = Convert.ToInt32(Session["compyid"]);
            tab.SDPTID = 1;
            tab.AVHLNO = tab.VHLNO;
            tab.ESBDATE = DateTime.Now;
            tab.DISPSTATUS = tab.DISPSTATUS;
            tab.LMUSRID = Session["CUSRID"].ToString();

            if (tab.GIDATE > Convert.ToDateTime(todayd))
            {
                tab.GIDATE = Convert.ToDateTime(todayd);
            }

            if (tab.GITIME > Convert.ToDateTime(todaydt))
            {
                tab.GITIME = Convert.ToDateTime(todaydt);
            }

            if (tab.GICCTLDATE > Convert.ToDateTime(todayd))
            {
                tab.GICCTLDATE = Convert.ToDateTime(todayd);
            }

            if (tab.GICCTLTIME > Convert.ToDateTime(todaydt))
            {
                tab.GICCTLTIME = Convert.ToDateTime(todaydt);
            }

            if (tab.CUSRID == "" || tab.CUSRID == null)
            {
                if (Session["CUSRID"] != null)
                {
                    tab.CUSRID = Session["CUSRID"].ToString();
                }
                else { tab.CUSRID = "0"; }
            }
            tab.BOEDATE = Convert.ToDateTime(tab.BOEDATE).Date;

            if (tab.GIDID.ToString() != "0")
            {
                context.Entry(tab).Entity.NGIDID = tab.GIDID + 1;
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            else
            {

                //----------------first record------------------//              

                tab.GINO = Convert.ToInt32(Autonumber.autonum("gateindetail", "GINO", "GINO <> 0 AND SDPTID = 1 and compyid = " + Convert.ToInt32(Session["compyid"]) + "").ToString());
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
                    context.SaveChanges();

                }/*end*/

                //-----second record-----------------------------// 
                tab.GIDATE = Convert.ToDateTime(tab.GIDATE).Date;
                tab.GITIME = Convert.ToDateTime(tab.GITIME);
                tab.GITIME = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);
                tab.TRNSPRTNAME = tab.TRNSPRTNAME;
                tab.AVHLNO = tab.VHLNO;
                tab.DRVNAME = tab.DRVNAME;
                tab.GPREFNO = tab.GPREFNO;
                tab.IMPRTID = 0;
                tab.IMPRTNAME = "-";
                tab.STMRID = 0;
                tab.STMRNAME = "-";
                tab.CONDTNID = 0;
                tab.CONTNRNO = "-";
                tab.CONTNRTID = 0;
                tab.CONTNRID = 0;
                tab.CONTNRSID = 0;
                tab.LPSEALNO = "-";
                tab.CSEALNO = "-";
                tab.YRDID = 0;
                tab.VSLID = 0;
                tab.VSLNAME = "-";
                tab.VOYNO = "-";
                tab.PRDTGID = 0;
                tab.PRDTDESC = "-";
                tab.UNITID = 0;
                tab.GPLNO = "-";
                tab.GPWGHT = 0;
                tab.GPEAMT = 0;
                tab.GPAAMT = 0;
                tab.IGMNO = tab.IGMNO;
                tab.GIISOCODE = tab.GIISOCODE;
                tab.GIDMGDESC = tab.GIDMGDESC;
                tab.GPWTYPE = 0;
                tab.GPSTYPE = 0;
                tab.GPETYPE = 0;
                //tab.ROWID = 0;
                tab.SLOTID = 0;
                tab.DISPSTATUS = 0;
                tab.GIVHLTYPE = 0;
                tab.TRNSPRTID = 0;
                tab.NGIDID = 0;
                tab.BOEDID = tab.GIDID;
                tab.BLNO = tab.BLNO;
                tab.BOENO = tab.BOENO;
                tab.CFSNAME = tab.CFSNAME;


                var GINO = Request.Form.Get("GINO");

                //if (context.gateindetails.Where(u => u.GINO == Convert.ToInt32(GINO)).Count()!=0) 
                tab.GINO = Convert.ToInt32(Autonumber.autonum("gateindetail", "GINO", "GINO <> 0 AND SDPTID = 1 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                //  else tab.GINO = 1;
                int anoo = tab.GINO;
                string prfxx = string.Format("{0:D5}", anoo);
                tab.GIDNO = prfxx.ToString();
                context.gateindetails.Add(tab);
                context.SaveChanges();

            }

            Response.Redirect("Index");

        }
        #endregion

        public void CONT_Duplicate_Check(string VOYNO, string GPLNO, string IGMNO, string CONTNRNO, string date)
        {
            VOYNO = Request.Form.Get("VOYNO");
            GPLNO = Request.Form.Get("GPLNO");
            IGMNO = Request.Form.Get("IGMNO");
            CONTNRNO = Request.Form.Get("CONTNRNO");
            date = Request.Form.Get("GIDATE");

            string temp = ContainerNo_Check.recordCount(VOYNO, IGMNO, GPLNO, CONTNRNO);
            if (temp != "PROCEED")
            {
                Response.Write("Container number already exists");
            }
            else
            {
                var query = context.Database.SqlQuery<PR_IMPORT_GATEIN_CONTAINER_CHK_ASSGN_Result>("Exec PR_IMPORT_GATEIN_CONTAINER_CHK_ASSGN @PCONTNRNO='" + CONTNRNO + "'").ToList();
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

        #region Auto Vessel
        public JsonResult AutoVessel(string term)
        {
            var result = (from vessel in context.vesselmasters
                          where vessel.VSLDESC.ToLower().Contains(term.ToLower())
                          select new { vessel.VSLDESC, vessel.VSLID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete transporter Name
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

        #region Autocomplete Cha Name
        public JsonResult AutoChaName(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete slot Name
        public JsonResult AutoSlot(string term)
        {
            var result = (from slot in context.slotmasters.Where(x => x.DISPSTATUS == 0)
                          where slot.SLOTDESC.ToLower().Contains(term.ToLower())
                          select new { slot.SLOTID, slot.SLOTDESC }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSlot(int id)
        {
            
            var slot = (from a in context.slotmasters.Where(x => x.DISPSTATUS == 0 && x.ROWID == id) select a).ToList();
            return Json(slot, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult GetVehicle(int id)//vehicl
        {
            var query = (from a in context.vehiclemasters.Where(x => x.DISPSTATUS == 0) where a.TRNSPRTID == id select a).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }


        //-------------Autocomplete Vehicle No
        public JsonResult VehicleNo(string term)
        {
            var result = (from r in context.vehiclemasters.Where(x => x.DISPSTATUS == 0)
                          join t in context.categorymasters.Where(x => x.DISPSTATUS == 0)
                          on r.TRNSPRTID equals t.CATEID into y
                          from k in y.DefaultIfEmpty()
                          where r.VHLMDESC.ToLower().Contains(term.ToLower())
                          select new { r.VHLMDESC, r.TRNSPRTID, k.CATENAME }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }//-----End of vehicle

        public ActionResult checkpostdata(int? id = 0)
        {
            String idx = Request.Form.Get("id");

            RemoteGateIn remotegateindetails = context.remotegateindetails.Find(idx);
            if (remotegateindetails == null)
                return HttpNotFound();
            else
                return View(remotegateindetails);
        }

        public ActionResult test1()
        {
            String idx = Request.Form.Get("id");
            GateInDetail tab = new GateInDetail();
            RemoteGateIn remote = new RemoteGateIn();
            remote = context.remotegateindetails.Find(idx);
            return View();
        }

        [Authorize(Roles = "ImportGateInPrint")]
        public void PrintView(int? id = 0)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "IMPORTGATEIN", Convert.ToInt32(id), Session["CUSRID"].ToString());

            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Import_GateIn.rpt");
                cryRpt.RecordSelectionFormula = "{VW_IMPORT_GATE_IN_PRINT_ASSGN.KUSRID} ='" + Session["CUSRID"].ToString() + "' and {VW_IMPORT_GATE_IN_PRINT_ASSGN.GIDID} =" + id;

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

        [Authorize(Roles = "ImportGateInPrint")]
        public void TPrintView(int? id = 0)/*truck*/
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "IMPORTTRUCKIN", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Import_TruckIn.rpt");
                cryRpt.RecordSelectionFormula = "{VW_IMPORT_GATE_IN_PRINT_ASSGN.KUSRID} ='" + Session["CUSRID"].ToString() + "' and {VW_IMPORT_GATE_IN_PRINT_ASSGN.GIDID} =" + id;

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


        //--------Delete Row----------
        [Authorize(Roles = "ImportGateInDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                GateInDetail gateindetails = new GateInDetail();
                //var sql = context.Database.SqlQuery<int>("SELECT GIDID from GATEINDETAIL where BOEDID=" + Convert.ToInt32(id)).ToList();
                //var gidid = (sql[0]).ToString();

                //gateindetails = context.gateindetails.Find(Convert.ToInt32(gidid));
                gateindetails = context.gateindetails.Find(Convert.ToInt32(id));
                context.gateindetails.Remove(gateindetails);
                gateindetails = context.gateindetails.Find(Convert.ToInt32(id));
                context.gateindetails.Remove(gateindetails);
                context.SaveChanges();

                Response.Write("Deleted Successfully ...");
            }
            else

                Response.Write(temp);

        }//-----End of Delete Row

        // ========================= Edit Logging (SCFS_LOG) =========================
        private void LogGateInEdits(GateInDetail before, GateInDetail after, string userId)
        {
            if (before == null || after == null) return;
            var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
            if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString)) return;

            // Exclude system or noisy fields and those you don't want to log
            var exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                // system/housekeeping fields
                "NGIDID", "PRCSDATE", "ESBDATE", "LMUSRID", "CUSRID",
                // the unwanted gate pass dimension/weight fields
                "GPTWGHT", "GPHEIGHT", "GPWIDTH", "GPLENGTH", "GPCBM", "GPGWGHT", "GPNWGHT", "GPNOP"
            };

            var props = typeof(GateInDetail).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in props)
            {
                if (!p.CanRead) continue;
                // Skip complex navigation properties
                if (p.PropertyType.IsClass && p.PropertyType != typeof(string) && !p.PropertyType.IsValueType)
                    continue;
                if (exclude.Contains(p.Name)) continue;

                var ov = p.GetValue(before, null);
                var nv = p.GetValue(after, null);

                if (BothNull(ov, nv)) continue;

                // Compare by underlying type to avoid logging formatting-only differences
                var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                bool changed;

                if (type == typeof(decimal))
                {
                    var d1 = ToNullableDecimal(ov) ?? 0m;
                    var d2 = ToNullableDecimal(nv) ?? 0m;
                    // skip if both are zero-equivalent
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
                else if (type == typeof(int) || type == typeof(long) || type == typeof(short))
                {
                    var i1 = Convert.ToInt64(ov ?? 0);
                    var i2 = Convert.ToInt64(nv ?? 0);
                    if (i1 == 0 && i2 == 0) continue;
                    changed = i1 != i2;
                }
                else if (type == typeof(DateTime))
                {
                    var t1 = (ov as DateTime?) ?? default(DateTime);
                    var t2 = (nv as DateTime?) ?? default(DateTime);
                    // ignore millisecond differences
                    t1 = new DateTime(t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second);
                    t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
                    changed = t1 != t2;
                }
                else if (type == typeof(string))
                {
                    var s1 = (Convert.ToString(ov) ?? string.Empty).Trim();
                    var s2 = (Convert.ToString(nv) ?? string.Empty).Trim();
                    // treat "-" and empty as same default; skip if both default/empty
                    bool def1 = string.IsNullOrEmpty(s1) || s1 == "-" || s1 == "0" || s1 == "0.0" || s1 == "0.00" || s1 == "0.000" || s1 == "0.0000";
                    bool def2 = string.IsNullOrEmpty(s2) || s2 == "-" || s2 == "0" || s2 == "0.0" || s2 == "0.00" || s2 == "0.000" || s2 == "0.0000";
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

                var os = FormatVal(ov);
                var ns = FormatVal(nv);

                InsertEditLogRow(cs.ConnectionString, after.GIDID, p.Name, os, ns, userId);
            }
        }

        private static string FormatVal(object value)
        {
            if (value == null) return null;
            if (value is DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm:ss");
            if (value is DateTime?)
            {
                var ndt = (DateTime?)value;
                return ndt.HasValue ? ndt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null;
            }
            if (value is decimal dec) return dec.ToString("0.####");
            var ndecs = value as decimal?;
            if (ndecs.HasValue) return ndecs.Value.ToString("0.####");
            return Convert.ToString(value);
        }

        private static bool BothNull(object a, object b) => a == null && b == null;

        private static decimal? ToNullableDecimal(object v)
        {
            if (v == null) return null;
            if (v is decimal d) return d;
            var nd = v as decimal?;
            if (nd.HasValue) return nd.Value;
            decimal parsed;
            return decimal.TryParse(Convert.ToString(v), out parsed) ? parsed : (decimal?)null;
        }

        private static void InsertEditLogRow(string connectionString, int gidid, string fieldName, string oldValue, string newValue, string changedBy)
        {
            using (var sql = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"INSERT INTO GateInDetailEditLog
                (GIDID, FieldName, OldValue, NewValue, ChangedBy, ChangedOn)
                VALUES (@GIDID, @FieldName, @OldValue, @NewValue, @ChangedBy, GETDATE())", sql))
            {
                cmd.Parameters.AddWithValue("@GIDID", gidid);
                cmd.Parameters.AddWithValue("@FieldName", fieldName);
                cmd.Parameters.AddWithValue("@OldValue", (object)oldValue ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NewValue", (object)newValue ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ChangedBy", changedBy ?? "");
                sql.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}