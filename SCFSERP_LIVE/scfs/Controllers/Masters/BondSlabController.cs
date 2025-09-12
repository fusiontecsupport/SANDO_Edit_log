
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

namespace scfs_erp.Controllers.Masters
{
    public class BondSlabMasterController : Controller
    {
        // GET: BondSlabMaster

        BondContext context = new BondContext();

        BondSlabMaster tab = new BondSlabMaster();
        //
        // GET: /RemoteGateIn/
        //[Authorize(Roles = "BondSlabMasterIndex")]
        public ActionResult Index()
        {

            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            if (Request.Form.Get("TARIFFMID") != null)
            {
                Session["TARIFFMID"] = Request.Form.Get("TARIFFMID");
                Session["SLABTID"] = Request.Form.Get("SLABTID");
                Session["CHRGETYPE"] = Request.Form.Get("CHRGETYPE");
                Session["FCHAID"] = Request.Form.Get("CHAID");
                Session["FCHANAME"] = Request.Form.Get("STMRDESC");                
            }
            else
            {
                
                Session["TARIFFMID"] = 1;
                Session["SLABTID"] = 2;
                Session["CHRGETYPE"] = 1; Session["FCHAID"] = 0; Session["FCHANAME"] = "";
            }


            ViewBag.TARIFFMID = new SelectList(context.bondtariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", Convert.ToInt32(Session["TARIFFMID"]));
            ViewBag.SLABTID = new SelectList(context.bondslabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", Convert.ToInt32(Session["SLABTID"]));


            List<SelectListItem> selectedCHRGETYPE = new List<SelectListItem>();
            if (Session["CHRGETYPE"].ToString() == "1")
            {
                SelectListItem selectedItem0 = new SelectListItem { Text = "FCL", Value = "1", Selected = true };
                selectedCHRGETYPE.Add(selectedItem0);
                selectedItem0 = new SelectListItem { Text = "LCL", Value = "2", Selected = false };
                selectedCHRGETYPE.Add(selectedItem0);
                ViewBag.CHRGETYPE = selectedCHRGETYPE;
            }
            else if (Session["CHRGETYPE"].ToString() == "2")
            {
                SelectListItem selectedItem0 = new SelectListItem { Text = "FCL", Value = "1", Selected = false };
                selectedCHRGETYPE.Add(selectedItem0);
                selectedItem0 = new SelectListItem { Text = "LCL", Value = "2", Selected = true };
                selectedCHRGETYPE.Add(selectedItem0);
                ViewBag.CHRGETYPE = selectedCHRGETYPE;
            }

            
            return View();//Loading Grid

        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new BondEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                //if (Session["FCHAID"].ToString() == "0")
                //{
                var data = e.pr_Search_BondSlabMaster(param.sSearch,
                                              Convert.ToInt32(Request["iSortCol_0"]),
                                              Request["sSortDir_0"],
                                              param.iDisplayStart,
                                              param.iDisplayStart + param.iDisplayLength,
                                              totalRowsCount,
                                              filteredRowsCount, Convert.ToInt32(Session["TARIFFMID"]), Convert.ToInt16(Session["CHRGETYPE"]), Convert.ToInt32(Session["SLABTID"]));

                var aaData = data.Select(d => new string[] { d.SLABMDATE.Value.ToString("dd/MM/yyyy"), d.CONTNRSID, d.YRDTYPE, d.SDTYPE, d.HTYPE, d.WTYPE, d.SLABMIN.ToString(), d.SLABMAX.ToString(), d.SLABAMT.ToString(), d.SLABMID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    var data = e.pr_Search_BondSlabMaster_Steamer_Wise(param.sSearch,
                //                                  Convert.ToInt32(Request["iSortCol_0"]),
                //                                  Request["sSortDir_0"],
                //                                  param.iDisplayStart,
                //                                  param.iDisplayStart + param.iDisplayLength,
                //                                  totalRowsCount,
                //                                  filteredRowsCount, Convert.ToInt32(Session["TARIFFMID"]), Convert.ToInt32(Session["CHRGETYPE"]), Convert.ToInt32(Session["SLABTID"]), Convert.ToInt32(Session["FCHAID"]));

                //    var aaData = data.Select(d => new string[] { d.SLABMDATE.Value.ToString("dd/MM/yyyy"), d.CONTNRSID, d.YRDTYPE, d.SDTYPE, d.HTYPE, d.WTYPE, d.SLABMIN.ToString(), d.SLABMAX.ToString(), d.SLABAMT.ToString(), d.SLABMID.ToString() }).ToArray();

                //    return Json(new
                //    {
                //        sEcho = param.sEcho,
                //        aaData = aaData,
                //        iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                //        iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                //    }, JsonRequestBehavior.AllowGet);
                //}
            }
        }

        //[Authorize(Roles = "BondSlabMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/BondSlabMaster/Copy/" + id);


            //Response.Redirect("/ExportBondSlabMaster/Copy/" + id);
        }


        //----------------------Initializing Form--------------------------//
        //[Authorize(Roles = "BondSlabMasterCreate")]
        public ActionResult Form(int? id = 0)
        {

            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            BondSlabMaster tab = new BondSlabMaster();
            tab.SLABMDATE = DateTime.Now.Date;
            tab.SLABMID = 0;
            //-------------------------Dropdown List------//
            //  ViewBag.CHAID = new SelectList(context.bondtariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x=>x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.TARIFFMID = new SelectList(context.bondtariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.CHRGETYPE = new SelectList(context.chargemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CHRGDESC), "CHRGID", "CHRGDESC");
            ViewBag.YRDTYPE = new SelectList(context.bondyardmasters, "YRDID", "YRDDESC",1);
            //ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC");
            var mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Operation_Types ").ToList();
            ViewBag.HTYPE = new SelectList(mtqry, "dval", "dtxt",1).ToList();            
            ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC",1);
            
            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_ExBond_VT_Handling_Types").ToList();
            ViewBag.HANDTYPE = new SelectList(mtqry, "dval", "dtxt",0).ToList();

            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");
            ViewBag.SLABTID = new SelectList(context.bondslabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC");
            ViewBag.PRDTGID = new SelectList(context.bondproductgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC",1);
            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_Bond_Tariff_Period_Types ").ToList();
            ViewBag.PERIODTID = new SelectList(mtqry, "dval", "dtxt", 1).ToList();


            //---------CHRGETYPE--------

            List<SelectListItem> selectedCHRGETYPE = new List<SelectListItem>();
            SelectListItem selectedItem0 = new SelectListItem { Text = "FCL", Value = "1", Selected = false };
            selectedCHRGETYPE.Add(selectedItem0);
            selectedItem0 = new SelectListItem { Text = "LCL", Value = "2", Selected = false };
            selectedCHRGETYPE.Add(selectedItem0);
            ViewBag.CHRGETYPE = selectedCHRGETYPE;



            //---------SDTYPE--------

            List<SelectListItem> selectedSDTYPE = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "NotRequired", Value = "0", Selected = true };
            selectedSDTYPE.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "Required", Value = "1", Selected = false };
            selectedSDTYPE.Add(selectedItem1);
            ViewBag.SDTYPE = selectedSDTYPE;



            //-----------------DISPSTATUS----

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem5 = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem5);
            selectedItem5 = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem5);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            //---End
            if (id < 0)
            {
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";
                string cid = id.ToString();
                cid = cid.Remove(0, 1);
                tab = context.bondslabmasters.Find(Convert.ToInt32(cid));
                //--------------------------------Selected values in Dropdown List-----------------------------//
                ViewBag.YRDTYPE = new SelectList(context.bondyardmasters, "YRDID", "YRDDESC", tab.YRDTYPE);
                //ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC", tab.HTYPE);
                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Operation_Types ").ToList();
                ViewBag.HTYPE = new SelectList(mtqry, "dval", "dtxt",tab.HTYPE).ToList();
                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_Bond_Tariff_Period_Types ").ToList();
                ViewBag.PERIODTID = new SelectList(mtqry, "dval", "dtxt",tab.PERIODTID).ToList();
                ViewBag.PRDTGID = new SelectList(context.bondproductgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", tab.PRDTGID);
                ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC", tab.WTYPE);

                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_ExBond_VT_Handling_Types").ToList();
                ViewBag.HANDTYPE = new SelectList(mtqry, "dval", "dtxt", tab.HANDTYPE).ToList();

                ViewBag.CHRGETYPE = new SelectList(context.chargemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CHRGDESC), "CHRGID", "CHRGDESC",tab.CHRGETYPE);
                ViewBag.CHAID = new SelectList(context.bondtariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.CHAID);
                ViewBag.TARIFFMID = new SelectList(context.bondtariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.CONTNRSDESC), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.bondslabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
                ////---------CHRGETYPE------------//
                List<SelectListItem> selectedCHRGETYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.CHRGETYPE) == 1)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "FCL", Value = "1", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "LCL", Value = "2", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                else if (Convert.ToInt32(tab.CHRGETYPE) == 2)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "FCL", Value = "1", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "LCL", Value = "2", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                ViewBag.CHRGETYPE = selectedCHRGETYPE1;
                ////End

                //---------SDTYPE-----------------//
                List<SelectListItem> selectedSDTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.SDTYPE) == 0)
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "NotRequired", Value = "0", Selected = true };
                    selectedSDTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Required", Value = "1", Selected = false };
                    selectedSDTYPE1.Add(selectedItem11);
                }
                else
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "NotRequired", Value = "0", Selected = false };
                    selectedSDTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Required", Value = "1", Selected = true };
                    selectedSDTYPE1.Add(selectedItem11);
                }
                ViewBag.SDTYPE = selectedSDTYPE1;

                
            }
            if (id != 0 && id > 0)
            {
                // if (id != 0)//Edit Mode
                // {
                tab = context.bondslabmasters.Find(id);
                //--------------------Selected values in Dropdown List------------------------//
                ViewBag.YRDTYPE = new SelectList(context.bondyardmasters, "YRDID", "YRDDESC", tab.YRDTYPE);
                //ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC", tab.HTYPE);
                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Operation_Types ").ToList();
                ViewBag.HTYPE = new SelectList(mtqry, "dval", "dtxt",tab.HTYPE).ToList();
                ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC", tab.WTYPE);

                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_ExBond_VT_Handling_Types").ToList();
                ViewBag.HANDTYPE = new SelectList(mtqry, "dval", "dtxt", tab.HANDTYPE).ToList();

                ViewBag.CHRGETYPE = new SelectList(context.chargemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CHRGDESC), "CHRGID", "CHRGDESC", tab.CHRGETYPE);
                ViewBag.CHAID = new SelectList(context.bondtariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.CHAID);
                ViewBag.TARIFFMID = new SelectList(context.bondtariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.CONTNRSDESC), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.bondslabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
                ViewBag.PRDTGID = new SelectList(context.bondproductgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", tab.PRDTGID);
                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_Bond_Tariff_Period_Types ").ToList();
                ViewBag.PERIODTID = new SelectList(mtqry, "dval", "dtxt", tab.PERIODTID).ToList();                
                //--------------DISPSTATUS-------------------------------------//
                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }


                //---------CHRGETYPE------------//
                List<SelectListItem> selectedCHRGETYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.CHRGETYPE) == 1)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "FCL", Value = "1", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "LCL", Value = "2", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                else if (Convert.ToInt32(tab.CHRGETYPE) == 2)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "FCL", Value = "1", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "LCL", Value = "2", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                ViewBag.CHRGETYPE = selectedCHRGETYPE1;
                //End

                //---------SDTYPE-----------------//
                List<SelectListItem> selectedSDTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.SDTYPE) == 0)
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "NotRequired", Value = "1", Selected = true };
                    selectedSDTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Required", Value = "2", Selected = false };
                    selectedSDTYPE1.Add(selectedItem11);
                }
                else
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "NotRequired", Value = "1", Selected = false };
                    selectedSDTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Required", Value = "2", Selected = true };
                    selectedSDTYPE1.Add(selectedItem11);
                }
                ViewBag.SDTYPE = selectedSDTYPE1;

                
                //End
            }
            return View(tab);
        }



        //End of Form

        public ActionResult NForm(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            BondSlabMaster tab = new BondSlabMaster();



            ViewBag.ATARIFFMID = new SelectList(context.bondtariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
                        

            List<SelectListItem> selectedCHRGETYPE_n = new List<SelectListItem>();
            SelectListItem selectedItem_ = new SelectListItem { Text = "FCL", Value = "1", Selected = true };
            selectedCHRGETYPE_n.Add(selectedItem_);
            selectedItem_ = new SelectListItem { Text = "LCL", Value = "2", Selected = false };
            selectedCHRGETYPE_n.Add(selectedItem_);
            ViewBag.ACHRGETYPE = selectedCHRGETYPE_n;

            ViewBag.ASLABTID = new SelectList(context.bondslabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC");
           // ViewBag.APRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");

            tab.SLABMDATE = DateTime.Now.Date;
            tab.SLABMID = 0;
            //-------------------------Dropdown List------//
            //  ViewBag.CHAID = new SelectList(context.bondtariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x=>x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.TARIFFMID = new SelectList(context.bondtariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.YRDTYPE = new SelectList(context.bondyardmasters, "YRDID", "YRDDESC");
            //ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC");
            var mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Operation_Types ").ToList();
            ViewBag.HTYPE = new SelectList(mtqry, "dval", "dtxt").ToList();
            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_Bond_Tariff_Period_Types ").ToList();
            ViewBag.PERIODTID = new SelectList(mtqry, "dval", "dtxt").ToList();

            ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC");

            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_ExBond_VT_Handling_Types").ToList();
            ViewBag.HANDTYPE = new SelectList(mtqry, "dval", "dtxt", 0).ToList();

            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");
            ViewBag.SLABTID = new SelectList(context.bondslabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC");

            //---------CHRGETYPE--------

            List<SelectListItem> selectedCHRGETYPE = new List<SelectListItem>();
            SelectListItem selectedItem0 = new SelectListItem { Text = "FCL", Value = "1", Selected = false };
            selectedCHRGETYPE.Add(selectedItem0);
            selectedItem0 = new SelectListItem { Text = "LCL", Value = "2", Selected = false };
            selectedCHRGETYPE.Add(selectedItem0);
            ViewBag.CHRGETYPE = selectedCHRGETYPE;


            //---------SDTYPE--------

            List<SelectListItem> selectedSDTYPE = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "NotRequired", Value = "1", Selected = false };
            selectedSDTYPE.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "Required", Value = "2", Selected = true };
            selectedSDTYPE.Add(selectedItem1);
            ViewBag.SDTYPE = selectedSDTYPE;



            //-----------------DISPSTATUS----

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem5 = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem5);
            selectedItem5 = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem5);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            //---End
            if (id < 0)
            {
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";
                string cid = id.ToString();
                cid = cid.Remove(0, 1);
                tab = context.bondslabmasters.Find(Convert.ToInt32(cid));
                //--------------------------------Selected values in Dropdown List-----------------------------//

                ViewBag.YRDTYPE = new SelectList(context.bondyardmasters, "YRDID", "YRDDESC", tab.YRDTYPE);
                //ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC", tab.HTYPE);
                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Operation_Types ").ToList();
                ViewBag.HTYPE = new SelectList(mtqry, "dval", "dtxt").ToList();
                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_Bond_Tariff_Period_Types ").ToList();
                ViewBag.PERIODTID = new SelectList(mtqry, "dval", "dtxt").ToList();

                ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC", tab.WTYPE);

                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_ExBond_VT_Handling_Types").ToList();
                ViewBag.HANDTYPE = new SelectList(mtqry, "dval", "dtxt", tab.HANDTYPE).ToList();

                ViewBag.CHAID = new SelectList(context.bondtariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.CHAID);
                ViewBag.TARIFFMID = new SelectList(context.bondtariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.CONTNRSDESC), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.bondslabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
                ViewBag.PRDTGID = new SelectList(context.bondproductgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC",tab.PRDTGID);
                //---------CHRGETYPE------------//
                List<SelectListItem> selectedCHRGETYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.CHRGETYPE) == 1)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "FCL", Value = "1", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "LCL", Value = "2", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                else if (Convert.ToInt32(tab.CHRGETYPE) == 2)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "FCL", Value = "1", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "LCL", Value = "2", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                ViewBag.CHRGETYPE = selectedCHRGETYPE1;
                //End

                //---------SDTYPE-----------------//
                List<SelectListItem> selectedSDTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.SDTYPE) == 0)
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "NotRequired", Value = "1", Selected = true };
                    selectedSDTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Required", Value = "2", Selected = false };
                    selectedSDTYPE1.Add(selectedItem11);
                }
                else
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "NotRequired", Value = "1", Selected = false };
                    selectedSDTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Required", Value = "2", Selected = true };
                    selectedSDTYPE1.Add(selectedItem11);
                }
                ViewBag.SDTYPE = selectedSDTYPE1;
            }
            if (id != 0 && id > 0)
            {
                // if (id != 0)//Edit Mode
                // {
                tab = context.bondslabmasters.Find(id);
                //--------------------Selected values in Dropdown List------------------------//
                ViewBag.YRDTYPE = new SelectList(context.bondyardmasters, "YRDID", "YRDDESC", tab.YRDTYPE);
                //ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC", tab.HTYPE);
                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Operation_Types ").ToList();
                ViewBag.HTYPE = new SelectList(mtqry, "dval", "dtxt", tab.HTYPE).ToList();
                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_Bond_Tariff_Period_Types ").ToList();
                ViewBag.PERIODTID = new SelectList(mtqry, "dval", "dtxt", tab.PERIODTID).ToList();
                ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC", tab.WTYPE);

                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_ExBond_VT_Handling_Types").ToList();
                ViewBag.HANDTYPE = new SelectList(mtqry, "dval", "dtxt", tab.HANDTYPE).ToList();

                ViewBag.CHAID = new SelectList(context.bondtariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.CHAID);
                ViewBag.TARIFFMID = new SelectList(context.bondtariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.CONTNRSDESC), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.bondslabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
                ViewBag.PRDTGID = new SelectList(context.bondproductgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC",tab.PRDTGID);
                //--------------DISPSTATUS-------------------------------------//
                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }


                //---------CHRGETYPE------------//
                List<SelectListItem> selectedCHRGETYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.CHRGETYPE) == 1)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "FCL", Value = "1", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "LCL", Value = "2", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                else if (Convert.ToInt32(tab.CHRGETYPE) == 2)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "FCL", Value = "1", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "LCL", Value = "2", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                ViewBag.CHRGETYPE = selectedCHRGETYPE1;
                //End

                //---------SDTYPE-----------------//
                List<SelectListItem> selectedSDTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.SDTYPE) == 0)
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "NotRequired", Value = "1", Selected = true };
                    selectedSDTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Required", Value = "2", Selected = false };
                    selectedSDTYPE1.Add(selectedItem11);
                }
                else
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "NotRequired", Value = "1", Selected = false };
                    selectedSDTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Required", Value = "2", Selected = true };
                    selectedSDTYPE1.Add(selectedItem11);
                }
                ViewBag.SDTYPE = selectedSDTYPE1;
                //End
            }
            return View(tab);
        }//End of transfer

        public string GetDetail(string id)
        {

            var param = id.Split('~');
            var TARIFFMID = Convert.ToInt32(param[0]);
            var CHRGETYPE = Convert.ToInt32(param[1]);
            var SLABTID = Convert.ToInt32(param[2]);


            //if (Session["FCHAID"].ToString() == "0")
            //{
            string slqry = "select * from [VW_BOND_RATECARDMASTER_FLX_ASSGN] (nolock) where TARIFFMID=" + TARIFFMID + " and CHRGETYPE=" + CHRGETYPE + " and SLABTID=" + SLABTID + "";
            var data = context.Database.SqlQuery<VW_BOND_RATECARDMASTER_FLX_ASSGN>(slqry).ToList();
            var html = ""; var prdtyp = "";
            var contnrsdesc = ""; var yrddesc = ""; var slabdesc = ""; var handlng = ""; var vthandlng = ""; var wagedesc = "";
            var prdgdesc = "";
            foreach (var dt in data)
            {
                if (dt.CONTNRSID == 3) { contnrsdesc = "20"; } else if (dt.CONTNRSID == 4) { contnrsdesc = "40"; } else if (dt.CONTNRSID == 5) { contnrsdesc = "45"; } else { contnrsdesc = "NR"; }

                if (dt.YRDTYPE == 2) { yrddesc = "Open"; }
                else if (dt.YRDTYPE == 3) { yrddesc = "Closed"; }
                else { yrddesc = "NR"; }

                if (dt.SDTYPE == 1) { slabdesc = "NR"; } else { slabdesc = "R"; }
                //if (dt.HTYPE == 1) { handlng = "FLT"; } else if (dt.HTYPE == 2) { handlng = "TLT"; } else if (dt.HTYPE == 3) { handlng = "CRANE"; } else if (dt.HTYPE == 4) { handlng = "MANUAL"; } else if (dt.HTYPE == 5) { handlng = "OWN"; } else { handlng = "NR"; }
                if (dt.HTYPE == 1) { handlng = "Manual"; }
                else if (dt.HTYPE == 2) { handlng = "Mechanical"; }
                else if (dt.HTYPE == 3) { handlng = "Both"; }

                if (dt.HANDTYPE == 1) { vthandlng = "Loading"; }
                else if (dt.HANDTYPE == 2) { vthandlng = "Unloading"; }
                else { vthandlng = "NR"; }

                if (dt.PERIODTID == 1) { prdtyp = "Not Required"; }
                else if (dt.PERIODTID == 2) { prdtyp = "Weekly"; }
                else if (dt.PERIODTID == 3) { prdtyp = "Daily"; }
                BondProductGroupMaster pgrp = new BondProductGroupMaster();
                prdgdesc = "";
                if (dt.PRDTGID != 0 )
                {
                    pgrp = context.bondproductgroupmasters.Find(dt.PRDTGID);
                    prdgdesc = pgrp.PRDTGDESC;
                }

                if (dt.WTYPE == 1) { wagedesc = "NR"; }
                else if (dt.WTYPE == 2) { wagedesc = "PACKAGE"; }
                else if (dt.WTYPE == 3) { wagedesc = "WEIGHT"; }
                else if (dt.WTYPE == 4) { wagedesc = "SCRAP"; }
                else { wagedesc = "L.CARGO"; }
                html = html + "<tr><td><input type='text' id='SLABMID' name='SLABMID' value=0 class='hidden'><input type='text' id='ASLABMDATE' class='hidden' name='ASLABMDATE' value='" + dt.SLABMDATE.ToString("dd/MM/yyyy") + "'>" + dt.SLABMDATE.ToString("dd/MM/yyyy") + "</td>";
                html = html + "<td><input type='text' id='CONTNRSID' name='CONTNRSID' value=" + dt.CONTNRSID + " class='hidden'>" + contnrsdesc + "</td>";
                html = html + "<td><input type='text' id='YRDTYPE' name='YRDTYPE' value=" + dt.YRDTYPE + " class='hidden'>" + yrddesc + "</td>";
                html = html + "<td><input type='text' id='SDTYPE' name='SDTYPE' value=" + dt.SDTYPE + " class='hidden'>" + slabdesc + "</td>";
                html = html + "<td><input type='text' id='HTYPE' name='HTYPE' value=" + dt.HTYPE + " class='hidden'>" + handlng + "</td>";
                html = html + "<td><input type='text' id='WTYPE' name='WTYPE' value=" + dt.WTYPE + " class='hidden'>" + wagedesc + "</td>";
                html = html + "<td><input type='text' id='HANDTYPE' name='HANDTYPE' value=" + dt.HANDTYPE + " class='hidden'>" + vthandlng + "</td>";
                html = html + "<td><input type='text' id='PERIODTID' name='PERIODTID' value=" + dt.PERIODTID + " class='hidden'>" + prdtyp + "</td>";
                html = html + "<td><input type='text' id='PRDTGID' name='PRDTGID' value=" + dt.PRDTGID + " class='hidden'>" + prdgdesc + "</td>";
                html = html + "<td><input type='text' id='SLABMIN' name='SLABMIN' value='" + dt.SLABMIN + "' class='hidden1'></td>";
                html = html + "<td><input type='text' id='SLABMAX' name='SLABMAX' value='" + dt.SLABMAX + "' class='hidden1'></td>";
                html = html + "<td><input type='text' id='SLABAMT' name='SLABAMT' value='" + dt.SLABAMT + "' class='hidden1'></td></tr>";

            }

            return html;
            //  return Json(data, JsonRequestBehavior.AllowGet);
            //   }

        }

        public void nsavedata(FormCollection myfrm)
        {
            BondSlabMaster bondslabmasters = new BondSlabMaster();
            //  Int32 SLABMID = Convert.ToInt32(myfrm["SLABMID"]);
            
            string[] SLABMID = myfrm.GetValues("SLABMID");
            string[] CONTNRSID = myfrm.GetValues("CONTNRSID");
            string[] YRDTYPE = myfrm.GetValues("YRDTYPE");
            string[] SDTYPE = myfrm.GetValues("SDTYPE");

            string[] HTYPE = myfrm.GetValues("HTYPE");
            string[] WTYPE = myfrm.GetValues("WTYPE");
            string[] HANDTYPE = myfrm.GetValues("HANDTYPE");
            
            string[] PRDTGID = myfrm.GetValues("PRDTGID");
            string[] PERIODTID = myfrm.GetValues("PERIODTID");

            string[] SLABMIN = myfrm.GetValues("SLABMIN");
            string[] SLABMAX = myfrm.GetValues("SLABMAX");
            string[] SLABAMT = myfrm.GetValues("SLABAMT");


            for (int i = 0; i < SLABMIN.Count(); i++)
            {

                //if (myfrm["SLABMID"] != null)
                 //   bondslabmasters.SLABMID = Convert.ToInt32(myfrm["SLABMID"]);

                bondslabmasters.CHAID = Convert.ToInt32(myfrm["CHAID"]);
                bondslabmasters.CHRGETYPE = Convert.ToInt16(myfrm["CHRGETYPE"]);
                

                bondslabmasters.SLABMDATE = Convert.ToDateTime(myfrm["SLABMDATE"]);
                bondslabmasters.COMPYID = Convert.ToInt32(Session["compyid"]);
                if (bondslabmasters.SLABMID == 0)
                {
                    bondslabmasters.CUSRID = Convert.ToString(Session["CUSRID"]);
                    bondslabmasters.LMUSRID = "";
                }
                else
                    bondslabmasters.LMUSRID = Convert.ToString(Session["CUSRID"]);
                bondslabmasters.DISPSTATUS = 0;

                
                bondslabmasters.PRCSDATE = DateTime.Now;
                bondslabmasters.SLABTID = Convert.ToInt32(myfrm["SLABTID"]);
                bondslabmasters.TARIFFMID = Convert.ToInt32(myfrm["TARIFFMID"]);
                bondslabmasters.HTYPE = Convert.ToInt16(HTYPE[i]);
                bondslabmasters.YRDTYPE = Convert.ToInt16(YRDTYPE[i]);
                bondslabmasters.WTYPE = Convert.ToInt16(WTYPE[i]);
                bondslabmasters.HANDTYPE = Convert.ToInt16(HANDTYPE[i]);
                
                bondslabmasters.PERIODTID = Convert.ToInt32(PERIODTID[i]);
                bondslabmasters.PRDTGID = Convert.ToInt32(PRDTGID[i]);
                bondslabmasters.CONTNRSID = Convert.ToInt32(CONTNRSID[i]);
                bondslabmasters.SDTYPE = Convert.ToInt16(SDTYPE[i]);
                bondslabmasters.SLABMIN = Convert.ToDecimal(SLABMIN[i]);
                bondslabmasters.SLABMAX = Convert.ToDecimal(SLABMAX[i]);
                bondslabmasters.SLABAMT = Convert.ToDecimal(SLABAMT[i]);
                context.bondslabmasters.Add(bondslabmasters);
                context.SaveChanges();
            }

            Response.Redirect("Index");
        }


        public ActionResult Copy(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            BondSlabMaster tab = new BondSlabMaster();
            tab.SLABMDATE = DateTime.Now.Date;
            tab.SLABMID = 0;
            //-------------------------Dropdown List------//
            //  ViewBag.CHAID = new SelectList(context.bondtariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x=>x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.TARIFFMID = new SelectList(context.bondtariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");

            ViewBag.YRDTYPE = new SelectList(context.bondyardmasters, "YRDID", "YRDDESC");
            ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC");
            ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC");
            var mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_ExBond_VT_Handling_Types").ToList();
            ViewBag.HANDTYPE = new SelectList(mtqry, "dval", "dtxt",0).ToList();


            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");
            ViewBag.SLABTID = new SelectList(context.bondslabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC");
            ViewBag.PRDTGID = new SelectList(context.bondproductgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");

            //---------CHRGETYPE--------

            List<SelectListItem> selectedCHRGETYPE = new List<SelectListItem>();
            SelectListItem selectedItem0 = new SelectListItem { Text = "FCL", Value = "1", Selected = false };
            selectedCHRGETYPE.Add(selectedItem0);
            selectedItem0 = new SelectListItem { Text = "LCL", Value = "2", Selected = false };
            selectedCHRGETYPE.Add(selectedItem0);
            ViewBag.CHRGETYPE = selectedCHRGETYPE;


            //---------SDTYPE--------

            List<SelectListItem> selectedSDTYPE = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "NotRequired", Value = "0", Selected = false };
            selectedSDTYPE.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "Required", Value = "1", Selected = true };
            selectedSDTYPE.Add(selectedItem1);
            ViewBag.SDTYPE = selectedSDTYPE;


            
            //-----------------DISPSTATUS----

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem5 = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem5);
            selectedItem5 = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem5);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            //---End
            if (id < 0)
            {
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";
                string cid = id.ToString();
                cid = cid.Remove(0, 1);
                tab = context.bondslabmasters.Find(Convert.ToInt32(cid));
                //--------------------------------Selected values in Dropdown List-----------------------------//

                ViewBag.YRDTYPE = new SelectList(context.bondyardmasters, "YRDID", "YRDDESC", tab.YRDTYPE);
                ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC", tab.HTYPE);
                ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC", tab.WTYPE);
                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_ExBond_VT_Handling_Types").ToList();
                ViewBag.HANDTYPE = new SelectList(mtqry, "dval", "dtxt", tab.HANDTYPE).ToList();

                ViewBag.CHAID = new SelectList(context.bondtariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.CHAID);
                ViewBag.TARIFFMID = new SelectList(context.bondtariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.CONTNRSDESC), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.bondslabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
                ViewBag.PRDTGID = new SelectList(context.bondproductgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", tab.PRDTGID);
                //---------CHRGETYPE------------//
                List<SelectListItem> selectedCHRGETYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.CHRGETYPE) == 1)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "FCL", Value = "1", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "LCL", Value = "2", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                else if (Convert.ToInt32(tab.CHRGETYPE) == 2)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "FCL", Value = "1", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "LCL", Value = "2", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                ViewBag.CHRGETYPE = selectedCHRGETYPE1;
                //End

                //---------SDTYPE-----------------//
                List<SelectListItem> selectedSDTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.SDTYPE) == 0)
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "NotRequired", Value = "0", Selected = true };
                    selectedSDTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Required", Value = "1", Selected = false };
                    selectedSDTYPE1.Add(selectedItem11);
                }
                else
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "NotRequired", Value = "0", Selected = false };
                    selectedSDTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Required", Value = "1", Selected = true };
                    selectedSDTYPE1.Add(selectedItem11);
                }
                ViewBag.SDTYPE = selectedSDTYPE1;

                
            }
            if (id != 0 && id > 0)
            {
                // if (id != 0)//Edit Mode
                // {
                tab = context.bondslabmasters.Find(id);


                var sql = context.Database.SqlQuery<BondSlabMaster>("SELECT * FROM BondSlabMaster WHERE (SLABMDATE='" + tab.SLABMDATE.ToString("dd/MM/yyyy") + "' ) AND (TARIFFMID=" + tab.TARIFFMID + ") AND (CHRGETYPE=" + tab.CHRGETYPE + ") AND (CONTNRSID=" + tab.CONTNRSID + ") AND (SDTYPE=" + tab.SDTYPE + ") AND (YRDTYPE=" + tab.YRDTYPE + ") AND (WTYPE=" + tab.WTYPE + ") AND (HTYPE=" + tab.HTYPE + ") AND (CHAID=" + tab.CHAID + ") AND (SLABTID=" + tab.SLABTID + ")").ToList();
                var html = "";

                foreach (var dt in sql)
                {
                    html = html + "<tr class='item-row'><td><input type='text' class='form-control SLABMIN' id='SLABMIN' name='SLABMIN' value=" + dt.SLABMIN + " /></td><td><input type='text' class='form-control SLABMAX' id='SLABMAX' name='SLABMAX' value=" + dt.SLABMAX + " /></td><td><input type='text' class='form-control SLABAMT' id='SLABAMT' name='SLABAMT' value=" + dt.SLABAMT + " /></td> <td><a class='' id='del_detail'> <i class='glyphicon glyphicon-trash' style='color:#ff0000;font-size:large'></i></a></td></tr>";
                }
                ViewBag.html = html;

                //--------------------Selected values in Dropdown List------------------------//

                ViewBag.YRDTYPE = new SelectList(context.bondyardmasters, "YRDID", "YRDDESC", tab.YRDTYPE);
                ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC", tab.HTYPE);
                ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC", tab.WTYPE);
                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_ExBond_VT_Handling_Types").ToList();
                ViewBag.HANDTYPE = new SelectList(mtqry, "dval", "dtxt", tab.HANDTYPE).ToList();

                ViewBag.CHAID = new SelectList(context.bondtariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.CHAID);
                ViewBag.TARIFFMID = new SelectList(context.bondtariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.CONTNRSDESC), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.bondslabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
                //--------------DISPSTATUS-------------------------------------//
                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }


                //---------CHRGETYPE------------//
                List<SelectListItem> selectedCHRGETYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.CHRGETYPE) == 1)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "FCL", Value = "1", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "LCL", Value = "2", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                else if (Convert.ToInt32(tab.CHRGETYPE) == 2)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "FCL", Value = "1", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "LCL", Value = "2", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                ViewBag.CHRGETYPE = selectedCHRGETYPE1;
                //End

                //---------SDTYPE-----------------//
                List<SelectListItem> selectedSDTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.SDTYPE) == 0)
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "NotRequired", Value = "0", Selected = true };
                    selectedSDTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Required", Value = "1", Selected = false };
                    selectedSDTYPE1.Add(selectedItem11);
                }
                else
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "NotRequired", Value = "0", Selected = false };
                    selectedSDTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Required", Value = "1", Selected = true };
                    selectedSDTYPE1.Add(selectedItem11);
                }
                ViewBag.SDTYPE = selectedSDTYPE1;

                
                //End
            }
            return View(tab);
        }//End of Form

        public void savedata(FormCollection myfrm)
        {

            using (SCFSERPContext dataContext = new SCFSERPContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        BondSlabMaster bndslbmsts = new BondSlabMaster();
                        Int32 SLABMID = Convert.ToInt32(myfrm["SLABMID"]);                        
                        bndslbmsts.CHAID = Convert.ToInt32(myfrm["CHAID"]);
                        bndslbmsts.CHRGETYPE = Convert.ToInt16(myfrm["CHRGETYPE"]);
                        bndslbmsts.CONTNRSID = Convert.ToInt32(myfrm["CONTNRSID"]);
                        bndslbmsts.SDTYPE = Convert.ToInt16(myfrm["SDTYPE"]);
                        bndslbmsts.SLABMDATE = Convert.ToDateTime(myfrm["SLABMDATE"]);
                        bndslbmsts.COMPYID = Convert.ToInt32(Session["compyid"]);
                        if(SLABMID==0)
                        {
                            bndslbmsts.LMUSRID = "";
                            bndslbmsts.CUSRID = Convert.ToString(Session["CUSRID"]);
                        }
                        else
                            bndslbmsts.LMUSRID = Convert.ToString(Session["CUSRID"]);
                        bndslbmsts.DISPSTATUS = 0;
                        bndslbmsts.HTYPE = Convert.ToInt16(myfrm["HTYPE"]);
                        
                        bndslbmsts.PRCSDATE = DateTime.Now;
                        bndslbmsts.SLABTID = Convert.ToInt32(myfrm["SLABTID"]);
                        bndslbmsts.TARIFFMID = Convert.ToInt32(myfrm["TARIFFMID"]);
                        bndslbmsts.PERIODTID = Convert.ToInt32(myfrm["PERIODTID"]);
                        bndslbmsts.YRDTYPE = Convert.ToInt16(myfrm["YRDTYPE"]);
                        bndslbmsts.WTYPE = Convert.ToInt16(myfrm["WTYPE"]);
                        bndslbmsts.HANDTYPE = Convert.ToInt16(myfrm["HANDTYPE"]);
                       
                        bndslbmsts.PRDTGID = Convert.ToInt16(myfrm["PRDTGID"]);
                        
                        string[] SLABMIN = myfrm.GetValues("SLABMIN");
                        string[] SLABMAX = myfrm.GetValues("SLABMAX");
                        string[] SLABAMT = myfrm.GetValues("SLABAMT");

                        //string[] SLABUAMT = myfrm.GetValues("SLABUAMT");

                        int row = 0;

                        foreach (var data in SLABAMT)
                        {
                            bndslbmsts.SLABMIN = Convert.ToDecimal(SLABMIN[row]);
                            bndslbmsts.SLABMAX = Convert.ToDecimal(SLABMAX[row]);
                            bndslbmsts.SLABAMT = Convert.ToDecimal(SLABAMT[row]);

                            //bndslbmsts.SLABUAMT = Convert.ToDecimal(SLABUAMT[row]);

                            context.bondslabmasters.Add(bndslbmsts);
                            context.SaveChanges();
                            row++;
                        }

                        trans.Commit();

                        if (myfrm["continue"] == null)
                        {
                            Response.Write("Saved");
                        }
                        else
                        {
                            //Response.Redirect("Form/-1");
                            Response.Redirect("Form/");
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                        trans.Rollback();
                        Response.Write("Sorry!! An Error Occurred.... ");
                    }
                }
            }
            //  Response.Redirect("Index");

        }//End of savedata

        //.....duplicate check
        public void checkdata(BondSlabMaster ex)
        {
            // Response.Write(ex.CHRGETYPE + "//" + ex.SLABMDATE);
            DateTime SLABDATE = Convert.ToDateTime(ex.SLABMDATE);
            var sql = context.Database.SqlQuery<int>("EXEC PR_BondSlabDuplicateCheck @PSLABMDATE='" + SLABDATE.ToString("MM/dd/yyyy") + "',@PTARIFFMID=" + ex.TARIFFMID + ",@PCHRGETYPE=" + ex.CHRGETYPE + ",@PCONTNRSID=" + ex.CONTNRSID + ",@PSDTYPE=" + ex.SDTYPE + ",@PYRDTYPE=" + ex.YRDTYPE + ",@PWTYPE=" + ex.WTYPE + ",@PHTYPE=" + ex.HTYPE + ",@PHANDTYPE=" + ex.HANDTYPE + ",@PCHAID=" + ex.CHAID + ",@PSLABTID=" + ex.SLABTID + ",@CNT=0").ToList();
            Response.Write(sql[0]);
        }



        //--------------------------Insert or Modify data------------------------//
        //public void savedata(BondSlabMaster tab)
        //{
        //    var SLABMID = Request.Form.Get("id");
        //    var SLABTID = Request.Form.Get("T_Id");
        //    tab.COMPYID = Convert.ToInt32(Session["compyid"]);
        //    tab.PRCSDATE = DateTime.Now;
        //    if(SLABMID!="0")
        //    {
        //        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
        //        context.SaveChanges();
        //        Response.Redirect("Index");
        //    }
        //    else
        //    {
        //        BondSlabMaster user = context.bondslabmasters.FirstOrDefault(u => u.SLABTID == tab.SLABTID || u.CHRGETYPE == tab.CHRGETYPE || u.TARIFFMID == tab.TARIFFMID || u.CONTNRSID == tab.CONTNRSID || u.SDTYPE == tab.SDTYPE || u.YRDTYPE == tab.YRDTYPE || u.WTYPE == tab.WTYPE || u.HTYPE == tab.HTYPE || u.CHAID == tab.CHAID || u.SLABMIN == tab.SLABMIN || u.SLABMAX == tab.SLABMAX || u.SLABMDATE == tab.SLABMDATE);

        //        if (user == null)
        //        {
        //            context.bondslabmasters.Add(tab);
        //            context.SaveChanges();
        //            Response.Redirect("Index");
        //        }
        //        else
        //        {
        //            Response.Write("Entered data already exists in database");
        //        }
        //    }

        //}//End of savedata
        //------------------------Delete Record----------//
        //[Authorize(Roles = "BondSlabMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                BondSlabMaster bndslbmsts = context.bondslabmasters.Find(Convert.ToInt32(id));
                //  context.bondslabmasters.Remove(bondslabmasters);

                DateTime slabmdate = Convert.ToDateTime(bndslbmsts.SLABMDATE);
                context.Database.ExecuteSqlCommand("delete from BondSlabMaster where SLABTID=" + bndslbmsts.SLABTID + " AND TARIFFMID=" + bndslbmsts.TARIFFMID + " AND CHRGETYPE=" + bndslbmsts.CHRGETYPE + " AND CONTNRSID=" + bndslbmsts.CONTNRSID + " AND SDTYPE=" + bndslbmsts.SDTYPE + " AND YRDTYPE=" + bndslbmsts.YRDTYPE + " AND WTYPE=" + bndslbmsts.WTYPE +" AND HANDTYPE=" + bndslbmsts.HANDTYPE + " AND HTYPE=" + bndslbmsts.HTYPE + " AND CHAID=" + bndslbmsts.CHAID + " AND SLABMDATE='" + slabmdate.ToString("MM/dd/yyyy") + "'");
                //context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//End of Delete
    }
}