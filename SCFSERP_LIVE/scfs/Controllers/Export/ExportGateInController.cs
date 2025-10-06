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
using System.Data.Entity;
using System.Reflection;
using System.Collections;
using System.Text;

namespace scfs_erp.Controllers.Export
{
    [SessionExpire]
    public class ExportGateInController : Controller
    {
        // GET: ExportGateIn

        #region contextdeclaration
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        #endregion

        #region Indexpage
        [Authorize(Roles = "ExportGateInIndex")]
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

            if (Request.Form.Get("GPWTYPE") != null)
            { Session["GPWTYPE"] = Request.Form.Get("GPWTYPE"); }
            else
            { Session["GPWTYPE"] = "0"; }

            if (Request.Form.Get("GPSTYPE") != null)
            { Session["GPSTYPE"] = Request.Form.Get("GPSTYPE"); }
            else
            { Session["GPSTYPE"] = "1"; }

            List<SelectListItem> selectedGPWTYPE = new List<SelectListItem>();
            if (Convert.ToInt32(Session["GPWTYPE"]) == 1)
            {
                SelectListItem selectedItem1 = new SelectListItem { Text = "LORRY", Value = "0", Selected = false };
                selectedGPWTYPE.Add(selectedItem1);
                selectedItem1 = new SelectListItem { Text = "TRAILOR", Value = "1", Selected = true };
                selectedGPWTYPE.Add(selectedItem1);
                selectedItem1 = new SelectListItem { Text = "ALL", Value = "2", Selected = false };
                selectedGPWTYPE.Add(selectedItem1);
            }
            else if (Convert.ToInt32(Session["GPWTYPE"]) == 2)
            {
                SelectListItem selectedItem1 = new SelectListItem { Text = "LORRY", Value = "0", Selected = false };
                selectedGPWTYPE.Add(selectedItem1);
                selectedItem1 = new SelectListItem { Text = "TRAILOR", Value = "1", Selected = false };
                selectedGPWTYPE.Add(selectedItem1);
                selectedItem1 = new SelectListItem { Text = "ALL", Value = "2", Selected = true };
                selectedGPWTYPE.Add(selectedItem1);
            }
            else
            {
                SelectListItem selectedItem1 = new SelectListItem { Text = "LORRY", Value = "0", Selected = true };
                selectedGPWTYPE.Add(selectedItem1);
                selectedItem1 = new SelectListItem { Text = "TRAILOR", Value = "1", Selected = false };
                selectedGPWTYPE.Add(selectedItem1);
                selectedItem1 = new SelectListItem { Text = "ALL", Value = "2", Selected = false };
                selectedGPWTYPE.Add(selectedItem1);
            }
            ViewBag.GPWTYPE = selectedGPWTYPE;

            int GPSTYPE = Convert.ToInt32(Session["GPSTYPE"]);

            ViewBag.GPSTYPE = new SelectList(context.exportvehiclegroupmasters, "GPSTYPE", "GPSTYPEDESC", GPSTYPE);            

            DateTime sd = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;
            DateTime ed = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;
            return View(context.gateindetails.Where(x => x.GIDATE >= sd).Where(x => x.CONTNRID >= 1).Where(x => x.SDPTID == 2).Where(x => x.GIDATE <= ed).ToList());
        }
        #endregion

        #region GetAjaxData
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)/*model 22.edmx*/
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Export_GateIn(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(Session["compyid"]), Convert.ToInt32(Session["GPWTYPE"]), Convert.ToInt32(Session["GPSTYPE"]));
                var aaData = data.Select(d => new string[] { d.GIDATE.Value.ToString("dd/MM/yyyy"), d.GITIME.Value.ToString("hh:mm tt"), d.GIDNO, d.CHANAME, d.PRDTDESC, d.CONTNRNO, d.CONTNRSID, d.VHLNO, d.DISPSTATUS, d.GIDID.ToString() }).ToArray();
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

        #region Editpage
        [Authorize(Roles = "ExportGateInEdit")]
        public void Edit(int id)
        {
            Response.Redirect("~/ExportGateIn/NForm/" + id);
            // Response.Redirect("/ExportGateIn/Form/" + id);
        }
        #endregion

        #region EditandInserpage Form
        [Authorize(Roles = "ExportGateInCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GateInDetail tab = new GateInDetail();
            tab.GIDID = 0;




            tab.GIDATE = DateTime.Now.Date; 
            tab.GITIME = DateTime.Now;
            tab.GITIME = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);
            tab.ESBDATE = DateTime.Now;
            tab.GICCTLTIME = DateTime.Now;

            //--------------------Dropdown list------------------------------------//
            ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");
            ViewBag.VSLNAME = new SelectList(context.vesselmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.VSLDESC), "VSLDESC", "VSLDESC");
            ViewBag.GDWNID = new SelectList(context.godownmasters.Where(x => x.DISPSTATUS == 0 && x.GDWNID > 1), "GDWNID", "GDWNDESC");
            ViewBag.STAGID = new SelectList(context.stagmasters.Where(x => x.DISPSTATUS == 0 && x.STAGID > 1), "STAGID", "STAGDESC");
            ViewBag.GPETYPE = new SelectList(context.exportsealtypemasters, "GPETYPE", "GPETYPEDESC");
            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID != 1 && m.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");
            ViewBag.PRDTTID = new SelectList(context.producttypemasters, "PRDTTID", "PRDTTDESC");
            ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0), "CONTNRTID", "CONTNRTDESC");
            ViewBag.GPWTYPE = new SelectList(context.exportvehicletypemasters, "GPWTYPE", "GPWTYPEDESC");


            //-----------------------------Container (OR) Trailor Type-----
            List<SelectListItem> selectedGPSTYPE = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "EMPTY", Value = "0", Selected = false };
            selectedGPSTYPE.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
            selectedGPSTYPE.Add(selectedItem1);
            ViewBag.GPSTYPE = selectedGPSTYPE;


            //-------------------------------DISPSTATUS----

            if (Convert.ToString(Session["Group"]) == "Admin" || Convert.ToString(Session["Group"]) == "SuperAdmin" || Convert.ToString(Session["Group"]).Contains("GroupAdmin"))
            {
                List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
                SelectListItem selectedItem31 = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = true };
                selectedDISPSTATUS.Add(selectedItem31);
                selectedItem31 = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = false };
                selectedDISPSTATUS.Add(selectedItem31);
                ViewBag.DISPSTATUS = selectedDISPSTATUS;
            }
            else
            {
                List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
                SelectListItem selectedItemDSP = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = true };
                selectedDISPSTATUS.Add(selectedItemDSP);
                ViewBag.DISPSTATUS = selectedDISPSTATUS;
            }

            List<SelectListItem> selectewhpoints = new List<SelectListItem>();
            SelectListItem selitem = new SelectListItem { Text = "Ground WH", Value = "0", Selected = true };
            selectewhpoints.Add(selitem);
            selitem = new SelectListItem { Text = "Elevated WH", Value = "1", Selected = false };
            selectewhpoints.Add(selitem);
            selitem = new SelectListItem { Text = "ITC WH", Value = "2", Selected = false };
            selectewhpoints.Add(selitem);
            selitem = new SelectListItem { Text = "Open Yard", Value = "3", Selected = false };
            selectewhpoints.Add(selitem);
            ViewBag.WHPOINT = selectewhpoints;

            if (id != 0)
            {
                tab = context.gateindetails.Find(id);

                ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", tab.PRDTGID);
                ViewBag.VSLNAME = new SelectList(context.vesselmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.VSLDESC), "VSLDESC", "VSLDESC", tab.VSLNAME);
                //ViewBag.GDWNID = new SelectList(context.godownmasters.Where(x => x.DISPSTATUS == 0), "GDWNID", "GDWNDESC", tab.GDWNID);
                ViewBag.GPETYPE = new SelectList(context.exportsealtypemasters, "GPETYPE", "GPETYPEDESC", tab.GPETYPE);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID != 1 && m.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.PRDTTID = new SelectList(context.producttypemasters, "PRDTTID", "PRDTTDESC", tab.PRDTTID);
                ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0), "CONTNRTID", "CONTNRTDESC", tab.CONTNRTID);
                //ViewBag.STAGID = new SelectList(context.stagmasters.Where(x => x.DISPSTATUS == 0), "STAGID", "STAGDESC", tab.STAGID);
                ViewBag.GPWTYPE = new SelectList(context.exportvehicletypemasters, "GPWTYPE", "GPWTYPEDESC", tab.GPWTYPE);
                ViewBag.GPSTYPE = new SelectList(context.exportvehiclegroupmasters, "GPSTYPE", "GPSTYPEDESC", tab.GPSTYPE);

                //-------------------------DISPSTATUS----------------------------
                if (Convert.ToString(Session["Group"]) == "Admin" || Convert.ToString(Session["Group"]) == "SuperAdmin" || Convert.ToString(Session["Group"]).Contains("GroupAdmin"))
                {
                    List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                    if (Convert.ToInt32(tab.DISPSTATUS) == 0)
                    {
                        SelectListItem selectedItem31 = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = true };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        selectedItem31 = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = false };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                    }
                    else
                    {
                        SelectListItem selectedItem31 = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = false };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        selectedItem31 = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = true };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                    }
                }
                else
                {
                    List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
                    SelectListItem selectedItemDSP = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = true };
                    selectedDISPSTATUS.Add(selectedItemDSP);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS;
                }

                //-------------------------GPWTYPE---------------------
                List<SelectListItem> selectedGPWTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GPWTYPE) == 0)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LORRY", Value = "0", Selected = true };
                    selectedGPWTYPE1.Add(selectedItemGPTY);
                    selectedItemGPTY = new SelectListItem { Text = "TRAILOR", Value = "1", Selected = false };
                    selectedGPWTYPE1.Add(selectedItemGPTY);

                }
                else
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LORRY", Value = "0", Selected = false };
                    selectedGPWTYPE1.Add(selectedItemGPTY);
                    selectedItemGPTY = new SelectListItem { Text = "TRAILOR", Value = "1", Selected = true };
                    selectedGPWTYPE1.Add(selectedItemGPTY);

                }
                ViewBag.GPWTYPE = selectedGPWTYPE1;


                //---------------------------------GPSTYPE-------------------------------------


                List<SelectListItem> selectedGPSTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GPSTYPE) == 0)
                {
                    SelectListItem selectedItemGPS = new SelectListItem { Text = "EMPTY", Value = "0", Selected = true };
                    selectedGPSTYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemGPS);

                }
                else if (Convert.ToInt32(tab.GPSTYPE) == 1)
                {
                    SelectListItem selectedItemGPS = new SelectListItem { Text = "EMPTY", Value = "0", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
                    selectedGPSTYPE1.Add(selectedItemGPS);

                }
                else if (Convert.ToInt32(tab.GPSTYPE) == 3)
                {
                    SelectListItem selectedItemGPS = new SelectListItem { Text = "EMPTY", Value = "3", Selected = true };
                    selectedGPSTYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "EMPTY CONT", Value = "4", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "LOAD CONT", Value = "5", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemGPS);

                }
                else if (Convert.ToInt32(tab.GPSTYPE) == 4)
                {
                    SelectListItem selectedItemGPS = new SelectListItem { Text = "EMPTY", Value = "3", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "EMPTY CONT", Value = "4", Selected = true };
                    selectedGPSTYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "LOAD CONT", Value = "5", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemGPS);

                }
                else if (Convert.ToInt32(tab.GPSTYPE) == 5)
                {
                    SelectListItem selectedItemGPS = new SelectListItem { Text = "EMPTY", Value = "3", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "EMPTY CONT", Value = "4", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "LOAD CONT", Value = "5", Selected = true };
                    selectedGPSTYPE1.Add(selectedItemGPS);

                }
                ViewBag.GPSTYPE = selectedGPSTYPE1;
            }
            return View(tab);//---Loading form
        }

        #endregion

        #region ExportGateInform NForm
        [Authorize(Roles = "ExportGateInCreate")]
        public ActionResult NForm(int? id = 0)/*new format*/
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GateInDetail tab = new GateInDetail();
            GateInGrp vm = new GateInGrp();
            //vm.gateindata.GIDID = 0;
            //vm.gateindata.GITIME = DateTime.Now;
            //vm.gateindata.ESBDATE = DateTime.Now;
            //vm.gateindata.GICCTLTIME = DateTime.Now;
            //--------------------Dropdown list------------------------------------//
            ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");
            ViewBag.VSLNAME = new SelectList(context.vesselmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.VSLDESC), "VSLDESC", "VSLDESC");
            ViewBag.GDWNID = new SelectList(context.godownmasters.Where(x => x.DISPSTATUS == 0 && x.GDWNID > 1), "GDWNID", "GDWNDESC");
            ViewBag.STAGID = new SelectList(context.stagmasters.Where(x => x.DISPSTATUS == 0 && x.STAGID > 1), "STAGID", "STAGDESC");
            ViewBag.GPETYPE = new SelectList(context.exportsealtypemasters, "GPETYPE", "GPETYPEDESC");
            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID != 1 && m.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");
            ViewBag.PRDTTID = new SelectList(context.producttypemasters, "PRDTTID", "PRDTTDESC");
            ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0), "CONTNRTID", "CONTNRTDESC");
            ViewBag.GPWTYPE = new SelectList(context.exportvehicletypemasters, "GPWTYPE", "GPWTYPEDESC");
            ViewBag.UsrGrp = Session["Group"].ToString();

            //-----------------------------Container (OR) Trailor Type-----
            List<SelectListItem> selectedGPSTYPE = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "EMPTY", Value = "0", Selected = false };
            selectedGPSTYPE.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
            selectedGPSTYPE.Add(selectedItem1);
            ViewBag.GPSTYPE = selectedGPSTYPE;


            //-------------------------------DISPSTATUS----

            if (Convert.ToString(Session["Group"]) == "Admin" || Convert.ToString(Session["Group"]) == "SuperAdmin" || Convert.ToString(Session["Group"]).Contains("GroupAdmin"))
            {
                List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
                SelectListItem selectedItem31 = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = true };
                selectedDISPSTATUS.Add(selectedItem31);
                selectedItem31 = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = false };
                selectedDISPSTATUS.Add(selectedItem31);
                ViewBag.DISPSTATUS = selectedDISPSTATUS;
            }
            else
            {
                List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
                SelectListItem selectedItemDSP = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = true };
                selectedDISPSTATUS.Add(selectedItemDSP);
                ViewBag.DISPSTATUS = selectedDISPSTATUS;
            }

            if (id != 0)
            {
                tab = context.gateindetails.Find(id);

                vm.gateindata = context.gateindetails.Find(id);
                var ESBMID = tab.ESBMID;
                if (Convert.ToInt32(ESBMID) > 0)
                {
                    vm.shippingbilldata = context.exportshippingbillmasters.Find(Convert.ToInt32(ESBMID));
                }


                ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", tab.PRDTGID);
                ViewBag.VSLNAME = new SelectList(context.vesselmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.VSLDESC), "VSLDESC", "VSLDESC", tab.VSLNAME);
                ViewBag.GDWNID = new SelectList(context.godownmasters.Where(x => x.DISPSTATUS == 0), "GDWNID", "GDWNDESC", tab.GDWNID);
                ViewBag.GPETYPE = new SelectList(context.exportsealtypemasters, "GPETYPE", "GPETYPEDESC", tab.GPETYPE);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID != 1 && m.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.PRDTTID = new SelectList(context.producttypemasters, "PRDTTID", "PRDTTDESC", tab.PRDTTID);
                ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0), "CONTNRTID", "CONTNRTDESC", tab.CONTNRTID);
                ViewBag.STAGID = new SelectList(context.stagmasters.Where(x => x.DISPSTATUS == 0), "STAGID", "STAGDESC", tab.STAGID);
                ViewBag.GPWTYPE = new SelectList(context.exportvehicletypemasters, "GPWTYPE", "GPWTYPEDESC", tab.GPWTYPE);
                ViewBag.GPSTYPE = new SelectList(context.exportvehiclegroupmasters, "GPSTYPE", "GPSTYPEDESC", tab.GPSTYPE);

                //-------------------------DISPSTATUS----------------------------
                
                if (Convert.ToString(Session["Group"]) == "Admin" || Convert.ToString(Session["Group"]) == "SuperAdmin" || Convert.ToString(Session["Group"]).Contains("GroupAdmin"))
                {
                    List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                    if (Convert.ToInt32(tab.DISPSTATUS) == 0)
                    {
                        SelectListItem selectedItem31 = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = true };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        selectedItem31 = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = false };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                    }
                    else
                    {
                        SelectListItem selectedItem31 = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = false };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        selectedItem31 = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = true };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                    }
                }
                else
                {
                    List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                    SelectListItem selectedItemDSP = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItemDSP);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }

                //-------------------------GPWTYPE---------------------
                List<SelectListItem> selectedGPWTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GPWTYPE) == 0)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LORRY", Value = "0", Selected = true };
                    selectedGPWTYPE1.Add(selectedItemGPTY);
                    selectedItemGPTY = new SelectListItem { Text = "TRAILOR", Value = "1", Selected = false };
                    selectedGPWTYPE1.Add(selectedItemGPTY);

                }
                else
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LORRY", Value = "0", Selected = false };
                    selectedGPWTYPE1.Add(selectedItemGPTY);
                    selectedItemGPTY = new SelectListItem { Text = "TRAILOR", Value = "1", Selected = true };
                    selectedGPWTYPE1.Add(selectedItemGPTY);

                }
                ViewBag.GPWTYPE = selectedGPWTYPE1;


                //---------------------------------GPSTYPE-------------------------------------


                List<SelectListItem> selectedGPSTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GPSTYPE) == 0)
                {
                    SelectListItem selectedItemGPS = new SelectListItem { Text = "EMPTY", Value = "0", Selected = true };
                    selectedGPSTYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemGPS);

                }
                else if (Convert.ToInt32(tab.GPSTYPE) == 1)
                {
                    SelectListItem selectedItemGPS = new SelectListItem { Text = "EMPTY", Value = "0", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
                    selectedGPSTYPE1.Add(selectedItemGPS);

                }
                else if (Convert.ToInt32(tab.GPSTYPE) == 3)
                {
                    SelectListItem selectedItemGPS = new SelectListItem { Text = "EMPTY", Value = "3", Selected = true };
                    selectedGPSTYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "EMPTY CONT", Value = "4", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "LOAD CONT", Value = "5", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemGPS);

                }
                else if (Convert.ToInt32(tab.GPSTYPE) == 4)
                {
                    SelectListItem selectedItemGPS = new SelectListItem { Text = "EMPTY", Value = "3", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "EMPTY CONT", Value = "4", Selected = true };
                    selectedGPSTYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "LOAD CONT", Value = "5", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemGPS);

                }
                else if (Convert.ToInt32(tab.GPSTYPE) == 5)
                {
                    SelectListItem selectedItemGPS = new SelectListItem { Text = "EMPTY", Value = "3", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "EMPTY CONT", Value = "4", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemGPS);
                    selectedItemGPS = new SelectListItem { Text = "LOAD CONT", Value = "5", Selected = true };
                    selectedGPSTYPE1.Add(selectedItemGPS);

                }
                ViewBag.GPSTYPE = selectedGPSTYPE1;
            }
            return View(vm);//---Loading form
        }

        #endregion

        #region CheckContainerDuplicate
        public void CheckContainerDuplicate(string id)
        {
            var sqla = context.Database.SqlQuery<int>("select ISNULL(max( gateindetail.GIDID),0) from gateindetail  where gateindetail.SDPTID=2 and gateindetail.CONTNRNO='" + id + "'").ToList();

            if (sqla[0] > 0)
            {
                //for (int i = 0; i < sqla.Count; i++)
                //{

                var sql = context.Database.SqlQuery<int>("select gateindetail.GIDID from gateindetail inner join gateoutdetail on gateindetail.GIDID=gateoutdetail.GIDID where gateindetail.GIDID=" + Convert.ToInt32(sqla[0]) + " and gateindetail.SDPTID=2 and gateindetail.CONTNRNO='" + id + "'").ToList();

                if (sql.Count > 0)
                {
                    Response.Write("PROCEED");
                }
                else
                {
                    Response.Write("Container No. already Exists");
                }
                // }
            }
            else
            {
                Response.Write("PROCEED");
            }

        }
        #endregion

        #region Duplicate Check for export shipping billno
        public void NoCheck()
        {
            string ESBMDNO = Request.Form.Get("No");
            using (var contxt = new SCFSERPContext())
            {
                var query = contxt.Database.SqlQuery<int>("select ESBMID from ExportShippingbillmaster where  ESBMDNO='" + ESBMDNO + "' ").ToList();

                var No = query.Count();
                if (No != 0)
                {
                    Response.Write(query[0]);
                    //  Response.Write("Entered shipping bill No. already exists");
                }

            }

        }
        #endregion

        #region Insert or modify data
        public void savedata(GateInDetail tab)
        {
            //if (Session["CUSRID"] != null)
            //    tab.CUSRID = Session["CUSRID"].ToString();
            using (SCFSERPContext context = new SCFSERPContext())
            {
                //using (var trans1 = context.Database.BeginTransaction())
                //{


                try
                {
                    string todaydt = Convert.ToString(DateTime.Now);
                    string todayd = Convert.ToString(DateTime.Now.Date);

                    tab.LMUSRID = Session["CUSRID"].ToString();
                    tab.CUSRID = Session["CUSRID"].ToString();

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



                    //tab.GIDATE = Convert.ToDateTime(tab.GIDATE).Date;                    
                    //tab.GITIME = Convert.ToDateTime(tab.GITIME);

                    //var newGIDateTime = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);

                    //tab.GITIME = Convert.ToDateTime(newGIDateTime).Date;
                    //tab.GITIME = Convert.ToDateTime(newGIDateTime);
                    tab.ESBDATE = DateTime.Now;
                    tab.PRCSDATE = DateTime.Now;
                    tab.GICCTLDATE = DateTime.Now;
                    tab.GICCTLTIME = DateTime.Now;

                    //   int ano = tab.GINO;
                    //   string prfx = string.Format("{0:D5}", ano);

                    // tab.GIDNO = prfx.ToString();

                    tab.SDPTID = 2;
                    tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                    tab.IMPRTID = 0;
                    tab.IMPRTNAME = "_";
                    tab.STMRID = 0;
                    tab.STMRNAME = "-";
                    tab.YRDID = 0;
                    tab.VSLID = 0;
                    tab.VOYNO = "-";


                    var vsl = Request.Form.Get("VSLNAME");
                    if (vsl == "")
                    {
                        tab.VSLNAME = "";
                    }

                    if ((tab.GPWTYPE == 0 || tab.GPWTYPE == 1) && (tab.GPSTYPE == 0 || tab.GPSTYPE == 3))
                    {
                        tab.CONTNRNO = "-";
                        tab.VSLNAME = "";
                        tab.PRDTDESC = "-";
                        tab.CHAID = 0;
                        tab.CHANAME = "";
                        tab.GPPLCNAME = "";
                        //tab.STAGID = 0;
                        //tab.GDWNID = 0;
                        tab.ESBDATE = DateTime.Now;
                    }

                    if (tab.GPWTYPE == 0 && (tab.GPSTYPE == 1 || tab.GPSTYPE == 3))
                    {
                        tab.CONTNRNO = "-";
                        tab.VSLNAME = "-";
                        tab.ESBDATE = DateTime.Now;
                    }
                    if (tab.GPWTYPE == 1 && (tab.GPSTYPE == 1 || tab.GPSTYPE == 4))
                    {
                        tab.VSLNAME = "-";
                        tab.PRDTDESC = "-";
                        tab.ESBDATE = DateTime.Now;
                    }

                    if ((tab.GIDID).ToString() != "0")//-----------Insert (OR) Update Mode CheckPoint
                    {
                        using (var trans = context.Database.BeginTransaction())
                        {
                            tab.CONTNRID = 1;
                            tab.AVHLNO = tab.VHLNO;
                            context.Entry(tab).Entity.NGIDID = tab.GIDID + 1;
                            context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            trans.Commit();
                        }
                    }
                    else
                    {
                        using (var trans = context.Database.BeginTransaction())
                        {
                            tab.GINO = Convert.ToInt32(Autonumber.autonum("gateindetail", "GINO", "GINO <> 0 and SDPTID = 2 AND compyid = " + Convert.ToInt32(Session["compyid"]) + "").ToString());
                            int ano = tab.GINO;
                            string prfx = string.Format("{0:D5}", ano);

                            tab.GIDNO = prfx.ToString();
                            tab.CONTNRID = 1;
                            tab.AVHLNO = tab.VHLNO;
                            context.gateindetails.Add(tab);

                            context.SaveChanges(); //trans.Commit();
                                                   //   }
                                                   //  using (var trans = context.Database.BeginTransaction())
                                                   //  {
                            context.Entry(tab).Entity.NGIDID = tab.GIDID + 1;
                            context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();// trans.Commit();
                                                  //   }
                                                  //-----------------Second record-----------------------------------
                                                  // using (var trans = context.Database.BeginTransaction())
                                                  // {
                            if (tab.GPSTYPE == 4 || tab.GPSTYPE == 5)
                            {
                                tab.CONTNRID = 0;
                                tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                                tab.SDPTID = 2;
                                tab.GICCTLDATE = Convert.ToDateTime(tab.GICCTLTIME).Date;
                                tab.GIDATE = Convert.ToDateTime(tab.GIDATE).Date;
                                tab.GITIME = Convert.ToDateTime(tab.GITIME);
                                
                                //var newGIDateTime = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);

                                //tab.GITIME = Convert.ToDateTime(newGIDateTime).Date;
                                //tab.GITIME = Convert.ToDateTime(newGIDateTime);
                                tab.NGIDID = 0;
                                tab.GIVHLTYPE = 0;
                                tab.TRNSPRTID = tab.TRNSPRTID;
                                tab.TRNSPRTNAME = tab.TRNSPRTNAME;
                                tab.VHLNO = tab.VHLNO;
                                //  tab.VHLNO = "1234567891234567896";
                                tab.AVHLNO = tab.VHLNO;
                                tab.DRVNAME = tab.DRVNAME;
                                tab.GPREFNO = tab.GPREFNO;
                                tab.PRDTGID = 0;
                                tab.GPWGHT = tab.GPWGHT;
                                tab.IMPRTID = 0;
                                tab.IMPRTNAME = "-";
                                tab.STMRID = 0;
                                tab.STMRNAME = "-";
                                tab.CONTNRTID = 0;
                                tab.CONTNRSID = 2;
                                tab.CONTNRNO = "-";
                                tab.YRDID = 0;
                                tab.VSLID = 0;
                                tab.VSLNAME = "-";
                                tab.VOYNO = "-";
                                tab.PRDTTID = 0;
                                tab.PRDTDESC = "-";
                                tab.GPWGHT = 0;
                                tab.GPNOP = 0;
                                //  tab.GINO = Convert.ToInt16(Autonumber.autonum("gateindetail", "GINO", "GINO<>0").ToString());
                                tab.GINO = tab.GINO + 1;
                                int anoo = tab.GINO;
                                string prfxx = string.Format("{0:D5}", anoo);
                                tab.GIDNO = prfxx.ToString();
                                tab.BOEDID = tab.GIDID;
                                tab.GPSTYPE = 3; tab.GPWTYPE = 1;
                                //if ((tab.GPWTYPE == 0 || tab.GPWTYPE == 1) && (tab.GPSTYPE == 0 || tab.GPSTYPE == 3))
                                //{
                                tab.PRDTDESC = "-";
                                //tab.CHAID = 0;
                                //tab.CHANAME = "";
                                tab.GPPLCNAME = "";
                                //tab.STAGID = 0;
                                //tab.GDWNID = 0;
                                tab.CONTNRTID = 0;
                                tab.CONTNRSID = 2;
                                tab.CONTNRNO = "-";
                                tab.ESBDATE = DateTime.Now;
                                //}

                                //if (tab.GPWTYPE == 0 && tab.GPSTYPE == 1)
                                //{
                                //    tab.CONTNRNO = "-";
                                //    tab.VSLNAME = "-";
                                //    tab.CONTNRTID = 0;
                                //    tab.CONTNRSID = 2;
                                //    tab.CONTNRNO = "-";
                                //    tab.ESBDATE = DateTime.Now;
                                //}
                                //if (tab.GPWTYPE == 1 && (tab.GPSTYPE == 1 || tab.GPSTYPE == 4))
                                //{
                                //    tab.CONTNRTID = 0;
                                //    tab.CONTNRSID = 2;
                                //    tab.CONTNRNO = "-";
                                //    tab.VSLNAME = "";
                                //    tab.PRDTDESC = "-";
                                //    tab.ESBDATE = DateTime.Now;
                                //}
                                context.gateindetails.Add(tab);
                                context.SaveChanges();

                            }
                            trans.Commit();
                        }
                    }
                    //-----------------------UPDATE
                    using (var trans = context.Database.BeginTransaction())
                    {
                        if (tab.GPSTYPE == 1 || tab.GPSTYPE == 5)
                        { context.Database.ExecuteSqlCommand("UPDATE SHIPPINGBILLDETAIL SET PRDTDESC = '" + tab.PRDTDESC + "' WHERE GIDID = " + tab.GIDID + ""); }

                        trans.Commit();
                    }
                    //trans1.Commit();
                    Response.Redirect("Index");
                }
                catch (Exception E)
                {
                    Response.Write(E);
                    //trans1.Rollback();
                    //  Response.Write("Sorry!! An Error Occurred.... ");
                    Response.Redirect("/Error/AccessDenied");
                }
            }

            // }
            //Response.Redirect("Index");
        }

        #endregion

        #region Inserting Shipping Bill 
        public void nsavedata(FormCollection myfrm)
        {
            GateInDetail tab = new GateInDetail();
            Int32 gidid = Convert.ToInt32(myfrm.Get("gateindata.GIDID"));
            Int32 esbmid = Convert.ToInt32(myfrm.Get("shippingbilldata.ESBMID"));
            using (var trans = context.Database.BeginTransaction())
            {
                try
                {
                    string todaydt = Convert.ToString(DateTime.Now);
                    string todayd = Convert.ToString(DateTime.Now.Date);

                    ExportShippingBillMaster EXPORTSHIPPINGBILLMSTDTL = new ExportShippingBillMaster();
                    //.......................................export shipping bill insert / modify.........................//
                    string indate = Convert.ToString(myfrm.Get("gateindata.GIDATE"));
                    string intime = Convert.ToString(myfrm.Get("gateindata.GITIME"));

                    //if (Convert.ToInt32(myfrm.Get("GPSTYPE")) == 1 || Convert.ToInt32(myfrm.Get("GPSTYPE")) == 5)
                    if (Convert.ToInt32(myfrm.Get("GPSTYPE")) == 5 || Convert.ToInt32(myfrm.Get("GPSTYPE")) == 1)
                    {
                        if (Convert.ToInt32(myfrm.Get("AESBMID")) > 0 && esbmid == 0)
                        {
                            EXPORTSHIPPINGBILLMSTDTL = context.exportshippingbillmasters.Find(Convert.ToInt32(myfrm.Get("AESBMID")));
                            var nop = Convert.ToInt32(EXPORTSHIPPINGBILLMSTDTL.ESBMNOP);
                            nop = nop + Convert.ToInt32(myfrm.Get("gateindata.GPNOP"));
                            context.Entry(EXPORTSHIPPINGBILLMSTDTL).Entity.ESBMNOP = Convert.ToDecimal(nop);
                            context.SaveChanges();
                        }
                        else
                        {


                            if (esbmid != 0) EXPORTSHIPPINGBILLMSTDTL = context.exportshippingbillmasters.Find(esbmid);
                            if (esbmid == 0)
                                EXPORTSHIPPINGBILLMSTDTL.CUSRID = Session["CUSRID"].ToString();
                            EXPORTSHIPPINGBILLMSTDTL.LMUSRID = Session["CUSRID"].ToString();
                            EXPORTSHIPPINGBILLMSTDTL.PRCSDATE = DateTime.Now;
                            EXPORTSHIPPINGBILLMSTDTL.COMPYID = Convert.ToInt32(Session["compyid"]);
                            //EXPORTSHIPPINGBILLMSTDTL.ESBMQTY = Convert.ToDecimal(Request.Form.Get("ESBMQTY"));

                            EXPORTSHIPPINGBILLMSTDTL.ESBMITYPE = 0;
                            EXPORTSHIPPINGBILLMSTDTL.ESBMIDATE = DateTime.Now;
                            EXPORTSHIPPINGBILLMSTDTL.CHAID = Convert.ToInt32(myfrm.Get("gateindata.CHAID"));//tab.CHAID;
                            EXPORTSHIPPINGBILLMSTDTL.CHANAME = myfrm.Get("gateindata.CHANAME").ToString(); //tab.CHANAME;

                            EXPORTSHIPPINGBILLMSTDTL.EXPRTID = Convert.ToInt32(myfrm.Get("shippingbilldata.EXPRTID"));
                            EXPORTSHIPPINGBILLMSTDTL.EXPRTNAME = Convert.ToString(myfrm.Get("shippingbilldata.EXPRTNAME"));
                            EXPORTSHIPPINGBILLMSTDTL.ESBMREFAMT = 0;
                            EXPORTSHIPPINGBILLMSTDTL.ESBMFOBAMT = 0;
                            EXPORTSHIPPINGBILLMSTDTL.ESBMDPNAME = "-";
                            EXPORTSHIPPINGBILLMSTDTL.PRDTGID = Convert.ToInt32(myfrm.Get("PRDTGID")); //tab.PRDTGID;
                            EXPORTSHIPPINGBILLMSTDTL.PRDTDESC = myfrm.Get("gateindata.PRDTDESC").ToString(); //tab.PRDTDESC;
                            EXPORTSHIPPINGBILLMSTDTL.ESBMNOP = Convert.ToDecimal(myfrm.Get("gateindata.GPNOP")); //tab.GPNOP;
                            EXPORTSHIPPINGBILLMSTDTL.ESBMQTY = Convert.ToDecimal(myfrm.Get("gateindata.GPWGHT")); //tab.GPWGHT;
                            EXPORTSHIPPINGBILLMSTDTL.ESBMDNO = Convert.ToString(myfrm.Get("shippingbilldata.ESBMDNO"));
                            EXPORTSHIPPINGBILLMSTDTL.ESBMDATE = Convert.ToDateTime(myfrm.Get("gateindata.gidate"));
                            EXPORTSHIPPINGBILLMSTDTL.ESBMREFNO = Convert.ToString(myfrm.Get("shippingbilldata.ESBMREFNO"));

                            if (indate != null || indate != "")
                            {
                                EXPORTSHIPPINGBILLMSTDTL.ESBMREFDATE = Convert.ToDateTime(indate).Date;
                            }
                            else { EXPORTSHIPPINGBILLMSTDTL.ESBMREFDATE = DateTime.Now.Date; }

                            if (EXPORTSHIPPINGBILLMSTDTL.ESBMREFDATE > Convert.ToDateTime(todayd))
                            {
                                EXPORTSHIPPINGBILLMSTDTL.ESBMREFDATE = Convert.ToDateTime(todayd);
                            }

                            //EXPORTSHIPPINGBILLMSTDTL.ESBMREFDATE = Convert.ToDateTime(myfrm.Get("gateindata.gidate"));

                            EXPORTSHIPPINGBILLMSTDTL.DISPSTATUS = 0;
                            if ((esbmid).ToString() != "0")
                            {
                                context.Entry(EXPORTSHIPPINGBILLMSTDTL).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                            }
                            else
                            {
                                EXPORTSHIPPINGBILLMSTDTL.ESBMNO = Convert.ToInt32(Autonumber.autonum("EXPORTSHIPPINGBILLMASTER", "ESBMNO", "ESBMNO <> 0 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                                int ano = EXPORTSHIPPINGBILLMSTDTL.ESBMNO;
                                string prfx = string.Format("{0:D5}", ano);
                                EXPORTSHIPPINGBILLMSTDTL.ESBMDNO = prfx.ToString();

                                context.exportshippingbillmasters.Add(EXPORTSHIPPINGBILLMSTDTL);
                                context.SaveChanges();
                            }
                        }
                        //trans.Commit();
                        //}
                    }//...end

                    if (gidid != 0) tab = context.gateindetails.Find(gidid);
                    tab.LMUSRID = Session["CUSRID"].ToString();
                    tab.CUSRID = Session["CUSRID"].ToString();

                    
                    if (indate != null || indate != "")
                    {
                        tab.GIDATE = Convert.ToDateTime(indate).Date;
                    }
                    else { tab.GIDATE = DateTime.Now.Date; }

                    if (tab.GIDATE > Convert.ToDateTime(todayd))
                    {
                        tab.GIDATE = Convert.ToDateTime(todayd);
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

                    tab.ESBDATE = DateTime.Now;
                    tab.PRCSDATE = DateTime.Now;
                    tab.GICCTLDATE = DateTime.Now;
                    tab.GICCTLTIME = DateTime.Now;
                    tab.SDPTID = 2;
                    tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                    tab.GIVHLTYPE = 0;
                    if (myfrm.Get("gateindata.TRNSPRTID") == "") tab.TRNSPRTID = 0; else tab.TRNSPRTID = Convert.ToInt32(myfrm.Get("gateindata.TRNSPRTID"));
                    //tab.TRNSPRTID = 0;
                    tab.TRNSPRTNAME = myfrm.Get("gateindata.TRNSPRTNAME").ToString();
                    tab.UNITID = 0;
                    tab.IMPRTID = 0;
                    tab.IMPRTNAME = "_";
                    //tab.STMRID = 0;
                    //tab.STMRNAME = "-";
                    if (myfrm.Get("gateindata.VHLMID") == "") tab.VHLMID = 0; else tab.VHLMID = Convert.ToInt32(myfrm.Get("gateindata.VHLMID"));
                    //tab.VHLMID = 0;
                    //tab.GPNRNO = "-";
                    if (myfrm.Get("gateindata.GPNRNO") == "") tab.GPNRNO = "-"; else tab.GPNRNO = myfrm.Get("gateindata.GPNRNO").ToString();
                                       
                    if (myfrm.Get("gateindata.EVSLRNO") == "") tab.EVSLRNO = "-"; else tab.EVSLRNO = myfrm.Get("gateindata.EVSLRNO").ToString();
                    if (myfrm.Get("gateindata.EMONO") == "") tab.EMONO = "-"; else tab.EMONO = myfrm.Get("gateindata.EMONO").ToString();
                    if (myfrm.Get("gateindata.EEGMNO") == "") tab.EEGMNO = "-"; else tab.EEGMNO = myfrm.Get("gateindata.EEGMNO").ToString();

                    tab.YRDID = 0;
                    tab.VSLID = 0;
                    tab.VOYNO = "-";
                    tab.CONTNRID = 1;
                    tab.VHLNO = myfrm.Get("gateindata.VHLNO");
                    tab.AVHLNO = myfrm.Get("gateindata.VHLNO");
                    tab.DRVNAME = myfrm.Get("gateindata.DRVNAME");
                    tab.GPREFNO = myfrm.Get("gateindata.GPREFNO");
                    var ESBMID = EXPORTSHIPPINGBILLMSTDTL.ESBMID;
                    if (Convert.ToInt32(EXPORTSHIPPINGBILLMSTDTL.ESBMID) > 0)
                    {
                        tab.ESBMID = EXPORTSHIPPINGBILLMSTDTL.ESBMID;
                    }
                    else
                    {
                        tab.ESBMID = 0;
                    }
                    if (EXPORTSHIPPINGBILLMSTDTL.ESBMREFNO != "")
                    {
                        tab.ESBNO = EXPORTSHIPPINGBILLMSTDTL.ESBMREFNO;
                    }
                    else
                    {
                        tab.ESBNO = "0";
                    }


                    if (myfrm.Get("gateindata.GIREMKRS") == "") tab.GIREMKRS = null; else tab.GIREMKRS = myfrm.Get("gateindata.GIREMKRS").ToString();

                    if (myfrm.Get("gateindata.GPNOP") == "") tab.GPNOP = 0; else tab.GPNOP = Convert.ToDecimal(myfrm.Get("gateindata.GPNOP"));
                    if (myfrm.Get("gateindata.GPWGHT") == "") tab.GPWGHT = 0; else tab.GPWGHT = Convert.ToDecimal(myfrm.Get("gateindata.GPWGHT"));
                    if (myfrm.Get("GPWTYPE") == "") tab.GPWTYPE = 0; else tab.GPWTYPE = Convert.ToInt16(myfrm.Get("GPWTYPE"));
                    if (myfrm.Get("GPSTYPE") == "") tab.GPSTYPE = 0; else tab.GPSTYPE = Convert.ToInt16(myfrm.Get("GPSTYPE"));
                    if (myfrm.Get("GPETYPE") == "") tab.GPETYPE = 0; else tab.GPETYPE = Convert.ToInt16(myfrm.Get("GPETYPE"));

                    if (myfrm.Get("PRDTTID") == "") tab.PRDTTID = 0; else tab.PRDTTID = Convert.ToInt32(myfrm.Get("PRDTTID"));
                    if (myfrm.Get("CONTNRTID") == "") tab.CONTNRTID = 0; else tab.CONTNRTID = Convert.ToInt32(myfrm.Get("CONTNRTID"));
                    if (myfrm.Get("CONTNRSID") == "") tab.CONTNRSID = 2; else tab.CONTNRSID = Convert.ToInt32(myfrm.Get("CONTNRSID"));

                    if (myfrm.Get("VSLNAME") == "") tab.VSLNAME = "-"; else tab.VSLNAME = myfrm.Get("VSLNAME").ToString();
                    if (myfrm.Get("gateindata.CONTNRNO") == "") tab.CONTNRNO = "-"; else tab.CONTNRNO = myfrm.Get("gateindata.CONTNRNO").ToString();

                    if (myfrm.Get("PRDTGID") == "") tab.PRDTGID = 0; else tab.PRDTGID = Convert.ToInt32(myfrm.Get("PRDTGID"));

                    if (myfrm.Get("gateindata.PRDTDESC") == "") tab.PRDTDESC = "-"; else tab.PRDTDESC = myfrm.Get("gateindata.PRDTDESC").ToString();
                    if (myfrm.Get("gateindata.CHAID") == "") tab.CHAID = 0; else tab.CHAID = Convert.ToInt32(myfrm.Get("gateindata.CHAID"));
                    if (myfrm.Get("gateindata.CHANAME") == "") tab.CHANAME = "-"; else tab.CHANAME = myfrm.Get("gateindata.CHANAME").ToString();
                    if (myfrm.Get("gateindata.BCHAID") == "") tab.BCHAID = 0; else tab.BCHAID = Convert.ToInt32(myfrm.Get("gateindata.BCHAID"));
                    if (myfrm.Get("gateindata.BCHANAME") == "") tab.BCHANAME = "-"; else tab.BCHANAME = myfrm.Get("gateindata.BCHANAME").ToString();
                    if (myfrm.Get("gateindata.GPPLCNAME") == "") tab.GPPLCNAME = "-"; else tab.GPPLCNAME = myfrm.Get("gateindata.GPPLCNAME").ToString();
                    if (myfrm.Get("gateindata.STMRID") == "") tab.STMRID = 0; else tab.STMRID = Convert.ToInt32(myfrm.Get("gateindata.STMRID"));
                    if (myfrm.Get("gateindata.STMRNAME") == "") tab.STMRNAME = "-"; else tab.STMRNAME = myfrm.Get("gateindata.STMRNAME").ToString();
                    if (myfrm.Get("STAGID") == "") tab.STAGID = 0; else tab.STAGID = Convert.ToInt32(myfrm.Get("STAGID"));
                    if (myfrm.Get("GDWNID") == "") tab.GDWNID = 0; else tab.GDWNID = Convert.ToInt32(myfrm.Get("GDWNID"));
                    if (myfrm.Get("gateindata.ESBDATE") == "") tab.ESBDATE = null; else tab.ESBDATE = Convert.ToDateTime(myfrm.Get("gateindata.ESBDATE"));

                    if (myfrm.Get("gateindata.LPSEALNO") == "") tab.LPSEALNO = "-"; else tab.LPSEALNO = myfrm.Get("gateindata.LPSEALNO").ToString();
                    if (myfrm.Get("gateindata.CSEALNO") == "") tab.CSEALNO = "-"; else tab.CSEALNO = myfrm.Get("gateindata.CSEALNO").ToString();
                    if (myfrm.Get("DISPSTATUS") == "") tab.DISPSTATUS = 0; else tab.DISPSTATUS = Convert.ToInt16(myfrm.Get("DISPSTATUS"));

                    if (gidid != 0)//-----------Insert (OR) Update Mode CheckPoint
                    {
                        // Load original record for logging (no tracking to avoid state conflicts)
                        var original = context.gateindetails.AsNoTracking().FirstOrDefault(x => x.GIDID == gidid);

                        context.Entry(tab).Entity.NGIDID = tab.GIDID + 1;
                        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();

                        // Best-effort logging to SCFS_LOG
                        try
                        {
                            System.Diagnostics.Debug.WriteLine($"EXPORT GATEIN SAVE METHOD CALLED: GIDID={tab.GIDID}, GIDNO={tab.GIDNO}");
                            if (original != null)
                            {
                                System.Diagnostics.Debug.WriteLine($"ORIGINAL RECORD FOUND: GIDID={original.GIDID}, calling LogGateInEdits");
                                // Ensure baseline snapshot (Version = "0") exists for this record before logging diffs
                                EnsureBaselineVersionZero(original, Session["CUSRID"] != null ? Session["CUSRID"].ToString() : "");
                                LogGateInEdits(original, tab, Session["CUSRID"] != null ? Session["CUSRID"].ToString() : "");
                                System.Diagnostics.Debug.WriteLine($"LogGateInEdits completed successfully");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"ORIGINAL RECORD NOT FOUND for GIDID={tab.GIDID}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log the error for debugging
                            System.Diagnostics.Debug.WriteLine($"Export GateIn edit logging failed: {ex.Message}");
                            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                        }
                    }
                    else
                    {
                        //using (var trans = context.Database.BeginTransaction())
                        //{
                        tab.GINO = Convert.ToInt32(Autonumber.autonum("gateindetail", "GINO", "GINO <> 0 and SDPTID = 2 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                        int ano = tab.GINO;
                        string prfx = string.Format("{0:D5}", ano);
                        tab.GIDNO = prfx.ToString();
                        context.gateindetails.Add(tab);
                        context.SaveChanges();

                        context.Entry(tab).Entity.NGIDID = tab.GIDID + 1;
                        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        //-----------------Second record-----------------------------------

                        if (Convert.ToInt32(myfrm.Get("GPSTYPE")) == 4 || Convert.ToInt32(myfrm.Get("GPSTYPE")) == 5)
                        {
                            tab.CONTNRID = 0;
                            tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                            tab.SDPTID = 2;
                            tab.GICCTLDATE = Convert.ToDateTime(tab.GICCTLTIME).Date;

                            if (indate != null || indate != "")
                            {
                                tab.GIDATE = Convert.ToDateTime(indate).Date;
                            }
                            else { tab.GIDATE = DateTime.Now.Date; }

                            if (tab.GIDATE > Convert.ToDateTime(todayd))
                            {
                                tab.GIDATE = Convert.ToDateTime(todayd);
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

                            tab.NGIDID = 0;
                            tab.GIVHLTYPE = 0;
                            tab.TRNSPRTID = tab.TRNSPRTID;
                            tab.TRNSPRTNAME = tab.TRNSPRTNAME;
                            tab.VHLNO = tab.VHLNO;
                            tab.AVHLNO = tab.VHLNO;
                            tab.DRVNAME = tab.DRVNAME;
                            tab.GPREFNO = tab.GPREFNO;
                            tab.PRDTGID = 0;
                            tab.GPWGHT = tab.GPWGHT;
                            tab.IMPRTID = 0;
                            tab.IMPRTNAME = "-";
                            tab.STMRID = 0;
                            tab.STMRNAME = "-";
                            tab.CONTNRTID = 0;
                            tab.CONTNRSID = 2;
                            tab.CONTNRNO = "-";
                            tab.YRDID = 0;
                            tab.VSLID = 0;
                            tab.VSLNAME = "-";
                            tab.VOYNO = "-";
                            tab.PRDTTID = 0;
                            tab.PRDTDESC = "-";
                            tab.GPWGHT = 0;
                            tab.GPNOP = 0;
                            tab.GINO = tab.GINO + 1;
                            int anoo = tab.GINO;
                            string prfxx = string.Format("{0:D5}", anoo);
                            tab.GIDNO = prfxx.ToString();
                            tab.BOEDID = tab.GIDID;
                            tab.GPSTYPE = 3; tab.GPWTYPE = 1;
                            tab.PRDTDESC = "-";
                            tab.GPPLCNAME = "";
                            tab.STAGID = 0;
                            tab.GDWNID = 0;
                            tab.CONTNRTID = 0;
                            tab.CONTNRSID = 2;
                            tab.CONTNRNO = "-";
                            tab.ESBDATE = DateTime.Now;

                            context.gateindetails.Add(tab);
                            context.SaveChanges();

                        }

                    }
                    trans.Commit(); Response.Redirect("Index");
                }//..end of try

                catch (SqlException e)
                {
                    Response.Write(e);
                    trans.Rollback();
                    Response.Redirect("/Error/AccessDenied");
                }
            }

            //-----------------------UPDATE carting order
            if (gidid != 0)
            {
                if (tab.GPSTYPE == 1 || tab.GPSTYPE == 5)
                { context.Database.ExecuteSqlCommand("UPDATE SHIPPINGBILLDETAIL SET PRDTDESC = '" + tab.PRDTDESC + "' WHERE GIDID = " + gidid + ""); }
            } //...................................end


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

        #region Autocomplete CHA from Category Master
        public JsonResult AutoCha(string term)
        {
            var result = (from category in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where category.CATENAME.ToLower().Contains(term.ToLower())
                          select new { category.CATENAME, category.CATEID }).Distinct();
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

        #region To Get Row to the Respective Godown
        public JsonResult GetStag(int id)
        {
            var stag = (from stg in context.stagmasters.Where(x => x.DISPSTATUS == 0) where stg.GDWNID == id select stg).ToList();
            return new JsonResult() { Data = stag, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region To Get Vehicle Type to the Respective Vehicle Group
        public JsonResult GetVehicleGroup(int id)
        {
            var group = (from vehicle in context.exportvehiclegroupmasters where vehicle.GPWTYPE == id select vehicle).ToList();
            return new JsonResult() { Data = group, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region Autocomplete Vehicle based On Transporter Id  And Vehicle details
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

        #region Autocomplete Steamer from Category Master
        public JsonResult AutoSteamer(string term)
        {
            var result = (from category in context.categorymasters.Where(m => m.CATETID == 3).Where(x => x.DISPSTATUS == 0)
                          where category.CATENAME.ToLower().Contains(term.ToLower())
                          select new { category.CATENAME, category.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Printview...
        [Authorize(Roles = "ExportGateInPrint")]
        public void PrintView(int? id = 0)
        {

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "EXPORTGATEIN", Convert.ToInt32(id), Session["CUSRID"].ToString());
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

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Export_GateIn.rpt");
                cryRpt.RecordSelectionFormula = "{VW_EXPORT_GATE_IN_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_GATE_IN_PRINT_ASSGN.GIDID} = " + id;

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

        #region Delete Row
        [Authorize(Roles = "ExportGateInDelete")]
        public void Del()
        {
            using (SCFSERPContext dataContext = new SCFSERPContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
                {
                    try
                    {

                        //String id = Request.Form.Get("id");
                        //String fld = Request.Form.Get("fld");

                        ////   var param = id.Split('-');

                        //String temp = Delete_fun.delete_check1(fld, id);

                        //if (temp.Equals("PROCEED"))
                        //{
                        //    GateInDetail gateindetails = context.gateindetails.Find(Convert.ToInt32(id));
                        //    context.gateindetails.Remove(gateindetails);
                        //    context.SaveChanges();
                        //    trans.Commit();
                        //    Response.Write("Deleted successfully...");
                        //}
                        //else
                        //    Response.Write(temp);



                        String id = Request.Form.Get("id");
                        String fld = Request.Form.Get("fld");
                        //String temp = Delete_fun.delete_check1(fld, id);

                        int esbmid = 0;

                        string Squery = "SELECT *FROM  SHIPPINGBILLMASTER (NOLOCK) INNER JOIN ";
                        Squery += "SHIPPINGBILLDETAIL(NOLOCK)   ON SHIPPINGBILLMASTER.SBMID = SHIPPINGBILLDETAIL.SBMID  INNER JOIN ";
                        Squery += "GATEINDETAIL(NOLOCK) ON GATEINDETAIL.GIDID = SHIPPINGBILLDETAIL.GIDID WHERE  SHIPPINGBILLDETAIL.GIDID =" + id;

                        var esbmidchk = context.Database.SqlQuery<ShippingBillDetail>(Squery).ToList();
                        if (esbmidchk.Count > 0)
                        {

                            Response.Write("Selected Record Referred Carting Order Detail.........!");
                        }
                        else
                        {
                            GateInDetail gateindetails = context.gateindetails.Find(Convert.ToInt32(id));
                            context.gateindetails.Remove(gateindetails);
                            context.SaveChanges(); trans.Commit();

                            Response.Write("Deleted successfully...");
                        }

                    }
                    catch
                    {
                        trans.Rollback();
                        Response.Write("Sorry!! An Error Occurred.... ");
                    }
                }
            }

        }
        #endregion

        // ========================= Edit Log Pages =========================
        public ActionResult EditLog()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View();
        }

        public ActionResult EditLogGateIn(int? gidid, DateTime? from = null, DateTime? to = null, string user = null, string fieldName = null, string version = null)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            var list = new List<scfs_erp.Models.GateInDetailEditLogRow>();
            var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
            if (cs != null && !string.IsNullOrWhiteSpace(cs.ConnectionString))
            {
                using (var sql = new SqlConnection(cs.ConnectionString))
                using (var cmd = new SqlCommand(@"SELECT TOP 2000 [GIDNO],[FieldName],[OldValue],[NewValue],[ChangedBy],[ChangedOn],[Version],[Modules]
                                                FROM [dbo].[GateInDetailEditLog]
                                                WHERE (@GIDNO IS NULL OR [GIDNO] = @GIDNO)
                                                  AND (@FROM IS NULL OR [ChangedOn] >= @FROM)
                                                  AND (@TO   IS NULL OR [ChangedOn] <  DATEADD(day, 1, @TO))
                                                  AND (@USER IS NULL OR [ChangedBy] LIKE '%' + @USER + '%')
                                                  AND (@FIELD IS NULL OR [FieldName] LIKE '%' + @FIELD + '%')
                                                  AND (@VERSION IS NULL OR [Version] LIKE '%' + @VERSION + '%')
                                                  AND [Modules] = 'ExportGateIn'
                                                ORDER BY [GIDNO] DESC, [Version] DESC, [ChangedOn] DESC", sql))
                {
                    cmd.Parameters.Add("@GIDNO", System.Data.SqlDbType.Int);
                    cmd.Parameters.Add("@FROM", System.Data.SqlDbType.DateTime);
                    cmd.Parameters.Add("@TO", System.Data.SqlDbType.DateTime);
                    cmd.Parameters.Add("@USER", System.Data.SqlDbType.VarChar, 100);
                    cmd.Parameters.Add("@FIELD", System.Data.SqlDbType.VarChar, 100);
                    cmd.Parameters.Add("@VERSION", System.Data.SqlDbType.VarChar, 50);

                    cmd.Parameters["@GIDNO"].Value = gidid.HasValue ? (object)gidid.Value : DBNull.Value;
                    cmd.Parameters["@FROM"].Value = from.HasValue ? (object)from.Value : DBNull.Value;
                    cmd.Parameters["@TO"].Value = to.HasValue ? (object)to.Value : DBNull.Value;
                    cmd.Parameters["@USER"].Value = !string.IsNullOrWhiteSpace(user) ? (object)user : DBNull.Value;
                    cmd.Parameters["@FIELD"].Value = !string.IsNullOrWhiteSpace(fieldName) ? (object)fieldName : DBNull.Value;
                    cmd.Parameters["@VERSION"].Value = !string.IsNullOrWhiteSpace(version) ? (object)version : DBNull.Value;

                    sql.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new scfs_erp.Models.GateInDetailEditLogRow
                            {
                                GIDNO = Convert.ToString(r["GIDNO"]),
                                FieldName = Convert.ToString(r["FieldName"]),
                                OldValue = Convert.ToString(r["OldValue"]),
                                NewValue = Convert.ToString(r["NewValue"]),
                                ChangedBy = Convert.ToString(r["ChangedBy"]),
                                ChangedOn = r["ChangedOn"] != DBNull.Value ? Convert.ToDateTime(r["ChangedOn"]) : DateTime.MinValue,
                                Version = Convert.ToString(r["Version"]),
                                Modules = Convert.ToString(r["Modules"])
                            });
                        }
                    }
                }
            }
            return View("~/Views/ImportGateIn/EditLogGateIn.cshtml", list);
        }

        // ========================= Edit Logging (SCFS_LOG) =========================
        private void LogGateInEdits(GateInDetail before, GateInDetail after, string userId)
        {
            if (before == null || after == null) 
            {
                System.Diagnostics.Debug.WriteLine($"LogGateInEdits: before={before != null}, after={after != null}");
                return;
            }
            var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
            if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString)) 
            {
                System.Diagnostics.Debug.WriteLine("LogGateInEdits: No SCFSERP_EditLog connection string found");
                return;
            }

            // Exclude system or noisy fields and those you don't want to log
            var exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                // system/housekeeping fields
                "NGIDID", "PRCSDATE", "ESBDATE", "LMUSRID", "CUSRID",
                // the unwanted gate pass dimension/weight fields
                "GPTWGHT", "GPHEIGHT", "GPWIDTH", "GPLENGTH", "GPCBM", "GPGWGHT", "GPNWGHT", "GPNOP",
                // system-mirrored fields
                "AVHLNO"
            };

            // Compute the next version ONCE per save so all rows for this edit share the same Version
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
                    WHERE [GIDNO] = @GIDNO AND [Modules] = 'ExportGateIn'", sql))
                {
                    cmd.Parameters.AddWithValue("@GIDNO", after.GIDNO);
                    sql.Open();
                    var obj = cmd.ExecuteScalar();
                    if (obj != null && obj != DBNull.Value)
                        nextVersion = Convert.ToInt32(obj);
                }
            }
            catch { /* ignore logging version errors */ }

            var props = typeof(GateInDetail).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in props)
            {
                if (!p.CanRead) continue;
                // Skip complex navigation properties
                if (p.PropertyType.IsClass && p.PropertyType != typeof(string) && !p.PropertyType.IsValueType)
                    continue;
                if (exclude.Contains(p.Name)) continue;

                // If this is an '*ID' field and a corresponding string name property exists,
                // skip logging the ID to avoid noisy numeric codes (e.g., STMRID vs STMRNAME)
                if (p.Name.EndsWith("ID", StringComparison.OrdinalIgnoreCase))
                {
                    var baseName = p.Name.Substring(0, p.Name.Length - 2); // remove 'ID'
                    var nameProp = props.FirstOrDefault(q =>
                        q.PropertyType == typeof(string) &&
                        (
                            q.Name.Equals(baseName, StringComparison.OrdinalIgnoreCase) ||
                            q.Name.Equals(baseName + "NAME", StringComparison.OrdinalIgnoreCase) ||
                            (q.Name.EndsWith("NAME", StringComparison.OrdinalIgnoreCase) && q.Name.StartsWith(baseName, StringComparison.OrdinalIgnoreCase))
                        ));
                    if (nameProp != null) continue;
                }

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

                var versionLabel = $"V{nextVersion}-{after.GIDNO}"; // Version label e.g., V1-04097
                InsertEditLogRow(cs.ConnectionString, after.GIDNO, p.Name, os, ns, userId, versionLabel, "ExportGateIn");
            }
        }

        private static bool BothNull(object a, object b)
        {
            return (a == null || a == DBNull.Value) && (b == null || b == DBNull.Value);
        }

        private static decimal? ToNullableDecimal(object v)
        {
            if (v == null || v == DBNull.Value) return null;
            decimal parsed;
            return decimal.TryParse(Convert.ToString(v), out parsed) ? parsed : (decimal?)null;
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
            return Convert.ToString(value);
        }

        // Ensure a baseline snapshot (Version = "0") exists for the given record.
        // It captures the pre-edit values as NewValue with OldValue set to NULL, one row per field.
        private void EnsureBaselineVersionZero(GateInDetail snapshot, string userId)
        {
            try
            {
                var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
                if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString)) return;

                // GIDNO is stored as string in entity; preserve it as-is with leading zeros
                if (string.IsNullOrWhiteSpace(snapshot.GIDNO)) return;

                using (var sql = new SqlConnection(cs.ConnectionString))
                using (var cmd = new SqlCommand("SELECT COUNT(1) FROM [dbo].[GateInDetailEditLog] WHERE [GIDNO]=@GIDNO AND [Modules]='ExportGateIn' AND (RTRIM(LTRIM([Version]))=@VLower OR RTRIM(LTRIM([Version]))=@VUpper OR RTRIM(LTRIM([Version]))='0' OR RTRIM(LTRIM([Version]))='V0')", sql))
                {
                    cmd.Parameters.AddWithValue("@GIDNO", snapshot.GIDNO);
                    var baselineVerLower = "v0-" + snapshot.GIDNO;
                    var baselineVerUpper = "V0-" + snapshot.GIDNO;
                    cmd.Parameters.AddWithValue("@VLower", baselineVerLower);
                    cmd.Parameters.AddWithValue("@VUpper", baselineVerUpper);
                    sql.Open();
                    var exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    if (exists) return; // baseline already present
                }

                InsertBaselineSnapshot(snapshot, userId);
            }
            catch (Exception ex)
            {
                // Do not block the main flow if baseline creation fails
                System.Diagnostics.Debug.WriteLine($"EnsureBaselineVersionZero failed: {ex.Message}");
            }
        }

        // Insert one row per relevant field for Version = "0" using the provided snapshot values
        private void InsertBaselineSnapshot(GateInDetail snapshot, string userId)
        {
            var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
            if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString)) return;

            if (string.IsNullOrWhiteSpace(snapshot.GIDNO)) return;
            var baselineVer = "v0-" + snapshot.GIDNO;

            // Use the same exclusion rules as differential logging
            var exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "NGIDID", "PRCSDATE", "ESBDATE", "LMUSRID", "CUSRID",
                "GPTWGHT", "GPHEIGHT", "GPWIDTH", "GPLENGTH", "GPCBM", "GPGWGHT", "GPNWGHT", "GPNOP",
                "AVHLNO"
            };

            var props = typeof(GateInDetail).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in props)
            {
                if (!p.CanRead) continue;
                if (p.PropertyType.IsClass && p.PropertyType != typeof(string) && !p.PropertyType.IsValueType) continue;
                if (exclude.Contains(p.Name)) continue;

                // Skip numeric code fields when a companion NAME string exists
                if (p.Name.EndsWith("ID", StringComparison.OrdinalIgnoreCase))
                {
                    var baseName = p.Name.Substring(0, p.Name.Length - 2);
                    var nameProp = props.FirstOrDefault(q => q.PropertyType == typeof(string) && (
                        q.Name.Equals(baseName, StringComparison.OrdinalIgnoreCase) ||
                        q.Name.Equals(baseName + "NAME", StringComparison.OrdinalIgnoreCase) ||
                        (q.Name.EndsWith("NAME", StringComparison.OrdinalIgnoreCase) && q.Name.StartsWith(baseName, StringComparison.OrdinalIgnoreCase))
                    ));
                    if (nameProp != null) continue;
                }

                var valObj = p.GetValue(snapshot, null);
                var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

                // Skip empty/defaults for strings and zero-equivalents for numbers to reduce noise
                if (type == typeof(string))
                {
                    var s = (Convert.ToString(valObj) ?? string.Empty).Trim();
                    bool isDefault = string.IsNullOrEmpty(s) || s == "-" || s == "0" || s == "0.0" || s == "0.00" || s == "0.000" || s == "0.0000";
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

                // Format value consistently and insert with Version = "0"
                var newVal = FormatVal(valObj);
                InsertEditLogRow(cs.ConnectionString, snapshot.GIDNO, p.Name, null, newVal, userId, baselineVer, "ExportGateIn");
            }
        }

        private static void InsertEditLogRow(string connectionString, string gidno, string fieldName, string oldValue, string newValue, string changedBy, string versionLabel, string modules)
        {
            try
            {
                using (var sql = new SqlConnection(connectionString))
                {
                    sql.Open();
                    using (var cmd = new SqlCommand(@"INSERT INTO [dbo].[GateInDetailEditLog]
                        ([GIDNO], [FieldName], [OldValue], [NewValue], [ChangedBy], [ChangedOn], [Version], [Modules])
                        VALUES (@GIDNO, @FieldName, @OldValue, @NewValue, @ChangedBy, @ChangedOn, @Version, @Modules)", sql))
                    {
                        cmd.Parameters.AddWithValue("@GIDNO", gidno);
                        cmd.Parameters.AddWithValue("@FieldName", fieldName ?? "");
                        cmd.Parameters.AddWithValue("@OldValue", oldValue ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NewValue", newValue ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ChangedBy", changedBy ?? "");
                        cmd.Parameters.AddWithValue("@ChangedOn", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Version", versionLabel ?? "");
                        cmd.Parameters.AddWithValue("@Modules", modules ?? "");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"InsertEditLogRow failed: {ex.Message}");
            }
        }

    }
}
