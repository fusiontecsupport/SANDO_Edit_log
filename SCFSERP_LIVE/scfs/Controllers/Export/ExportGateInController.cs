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
                var aaData = data.Select(d => new string[] { d.GIDATE.Value.ToString("dd/MM/yyyy"), d.GITIME.Value.ToString("hh:mm tt"), d.GIDNO.ToString(), d.CHANAME, d.PRDTDESC, d.CONTNRNO, d.CONTNRSID, d.VHLNO, d.DISPSTATUS, d.GIDID.ToString() }).ToArray();
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
                        //using (var trans = context.Database.BeginTransaction())
                        //{

                        context.Entry(tab).Entity.NGIDID = tab.GIDID + 1;
                        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        //    trans.Commit();
                        //}
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

    }
}
