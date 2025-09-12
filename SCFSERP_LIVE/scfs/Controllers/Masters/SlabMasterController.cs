
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
    public class SlabMasterController : Controller
    {
        // GET: SlabMaster

        SCFSERPContext context = new SCFSERPContext();
        
        SlabMaster tab = new SlabMaster();
        //
        // GET: /RemoteGateIn/
        //[Authorize(Roles = "ImportSlabMasterIndex")]
        public ActionResult Index()
        {

            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            if (Request.Form.Get("TARIFFMID") != null)
            {
                Session["TARIFFMID"] = Request.Form.Get("TARIFFMID");                
                Session["SLABTID"] = Request.Form.Get("SLABTID");
                Session["CHRGETYPE"] = Request.Form.Get("CHRGETYPE");
                Session["FSTMRID"] = Request.Form.Get("STMRID");
                Session["FSTMRNAME"] = Request.Form.Get("STMRDESC");
                Session["AASDPTTYPEID"] = Request.Form.Get("AASDPTTYPEID");
            }
            else
            {
                Session["AASDPTTYPEID"] = 1;
                Session["TARIFFMID"] = 1;
                Session["SLABTID"] = 2;
                Session["CHRGETYPE"] = 1; Session["FSTMRID"] = 0; Session["FSTMRNAME"] = "";
            }


            ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", Convert.ToInt32(Session["TARIFFMID"]));
            ViewBag.SLABTID = new SelectList(context.slabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", Convert.ToInt32(Session["SLABTID"]));


            List<SelectListItem> selectedCHRGETYPE = new List<SelectListItem>();
            if (Session["CHRGETYPE"].ToString() == "1")
            {
                SelectListItem selectedItem0 = new SelectListItem { Text = "LD", Value = "1", Selected = true };
                selectedCHRGETYPE.Add(selectedItem0);
                selectedItem0 = new SelectListItem { Text = "DS", Value = "2", Selected = false };
                selectedCHRGETYPE.Add(selectedItem0);
                ViewBag.CHRGETYPE = selectedCHRGETYPE;
            }
            else if (Session["CHRGETYPE"].ToString() == "2")
            {
                SelectListItem selectedItem0 = new SelectListItem { Text = "LD", Value = "1", Selected = false };
                selectedCHRGETYPE.Add(selectedItem0);
                selectedItem0 = new SelectListItem { Text = "DS", Value = "2", Selected = true };
                selectedCHRGETYPE.Add(selectedItem0);
                ViewBag.CHRGETYPE = selectedCHRGETYPE;
            }

            List<SelectListItem> selectedASDPTTYPE = new List<SelectListItem>();
            if (Session["AASDPTTYPEID"].ToString() == "1")
            {
                SelectListItem selectedItem0 = new SelectListItem { Text = "Import", Value = "1", Selected = true };
                selectedASDPTTYPE.Add(selectedItem0);
                selectedItem0 = new SelectListItem { Text = "Non PNR", Value = "9", Selected = false };
                selectedASDPTTYPE.Add(selectedItem0);
                ViewBag.AASDPTTYPEID = selectedASDPTTYPE;
            }
            else if (Session["AASDPTTYPEID"].ToString() == "9")
            {
                SelectListItem selectedItem0 = new SelectListItem { Text = "Import", Value = "1", Selected = false };
                selectedASDPTTYPE.Add(selectedItem0);
                selectedItem0 = new SelectListItem { Text = "Non PNR", Value = "9", Selected = true };
                selectedASDPTTYPE.Add(selectedItem0);
                ViewBag.AASDPTTYPEID = selectedASDPTTYPE;
            }
            return View();//Loading Grid

        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                //if (Session["FSTMRID"].ToString() == "0")
                //{
                var data = e.pr_Search_SlabMaster(param.sSearch,
                                              Convert.ToInt32(Request["iSortCol_0"]),
                                              Request["sSortDir_0"],
                                              param.iDisplayStart,
                                              param.iDisplayStart + param.iDisplayLength,
                                              totalRowsCount,
                                              filteredRowsCount, Convert.ToInt32(Session["TARIFFMID"]), Convert.ToInt16(Session["CHRGETYPE"]), Convert.ToInt32(Session["SLABTID"]), Convert.ToInt32(Session["AASDPTTYPEID"]));

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
                //    var data = e.pr_Search_SlabMaster_Steamer_Wise(param.sSearch,
                //                                  Convert.ToInt32(Request["iSortCol_0"]),
                //                                  Request["sSortDir_0"],
                //                                  param.iDisplayStart,
                //                                  param.iDisplayStart + param.iDisplayLength,
                //                                  totalRowsCount,
                //                                  filteredRowsCount, Convert.ToInt32(Session["TARIFFMID"]), Convert.ToInt32(Session["CHRGETYPE"]), Convert.ToInt32(Session["SLABTID"]), Convert.ToInt32(Session["FSTMRID"]));

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

        //[Authorize(Roles = "ImportSlabMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/SlabMaster/Copy/" + id);


            //Response.Redirect("/ExportSlabMaster/Copy/" + id);
        }


        //----------------------Initializing Form--------------------------//
        [Authorize(Roles = "ImportSlabMasterCreate")]
        public ActionResult Form(int? id = 0)
        {

            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            SlabMaster tab = new SlabMaster();
            tab.SLABMDATE = DateTime.Now.Date;
            tab.SLABMID = 0;
            //-------------------------Dropdown List------//
            //  ViewBag.STMRID = new SelectList(context.tariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x=>x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.CHRGETYPE = new SelectList(context.chargemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CHRGDESC), "CHRGID", "CHRGDESC");
            ViewBag.YRDTYPE = new SelectList(context.import_slab_yard_type_masters, "YRDTYPE", "YRDDESC");
            ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC");
            ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC");
            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");
            ViewBag.SLABTID = new SelectList(context.slabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC");

            //---------SDPTTYPE--------

            List<SelectListItem> selectedSDPTTYPE = new List<SelectListItem>();
            SelectListItem selectedItemSDPT0 = new SelectListItem { Text = "Import", Value = "1", Selected = false };
            selectedSDPTTYPE.Add(selectedItemSDPT0);
            selectedItemSDPT0 = new SelectListItem { Text = "Non PNR", Value = "9", Selected = false };
            selectedSDPTTYPE.Add(selectedItemSDPT0);
            ViewBag.SDPTTYPEID = selectedSDPTTYPE;

            //---------CHRGETYPE--------

            List<SelectListItem> selectedCHRGETYPE = new List<SelectListItem>();
            SelectListItem selectedItem0 = new SelectListItem { Text = "LD", Value = "1", Selected = false };
            selectedCHRGETYPE.Add(selectedItem0);
            selectedItem0 = new SelectListItem { Text = "DS", Value = "2", Selected = false };
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
                tab = context.slabmasters.Find(Convert.ToInt32(cid));
                //--------------------------------Selected values in Dropdown List-----------------------------//
                ViewBag.YRDTYPE = new SelectList(context.import_slab_yard_type_masters, "YRDTYPE", "YRDDESC", tab.YRDTYPE);
                ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC", tab.HTYPE);
                ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC", tab.WTYPE);
                ViewBag.CHRGETYPE = new SelectList(context.chargemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CHRGDESC), "CHRGID", "CHRGDESC");
                ViewBag.STMRID = new SelectList(context.tariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.STMRID);
                ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.CONTNRSDESC), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.slabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
                ////---------CHRGETYPE------------//
                List<SelectListItem> selectedCHRGETYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.CHRGETYPE) == 1)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "LD", Value = "1", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "DS", Value = "2", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                else if (Convert.ToInt32(tab.CHRGETYPE) == 2)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "LD", Value = "1", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "DS", Value = "2", Selected = true };
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

                //---------SDPTTYPE-----------------//
                List<SelectListItem> selectedSDPTTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.SDPTTYPEID) == 1)
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "Import", Value = "1", Selected = true };
                    selectedSDPTTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Non PNR", Value = "9", Selected = false };
                    selectedSDPTTYPE1.Add(selectedItem11);
                }
                else
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "Import", Value = "1", Selected = false };
                    selectedSDPTTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Non PNR", Value = "9", Selected = true };
                    selectedSDPTTYPE1.Add(selectedItem11);
                }
                ViewBag.SDPTTYPEID = selectedSDPTTYPE1;
            }
            if (id != 0 && id > 0)
            {
                // if (id != 0)//Edit Mode
                // {
                tab = context.slabmasters.Find(id);
                //--------------------Selected values in Dropdown List------------------------//
                ViewBag.YRDTYPE = new SelectList(context.import_slab_yard_type_masters, "YRDTYPE", "YRDDESC", tab.YRDTYPE);
                ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC", tab.HTYPE);
                ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC", tab.WTYPE);
                ViewBag.CHRGETYPE = new SelectList(context.chargemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CHRGDESC), "CHRGID", "CHRGDESC", tab.CHRGETYPE);
                ViewBag.STMRID = new SelectList(context.tariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.STMRID);
                ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.CONTNRSDESC), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.slabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
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
                    SelectListItem selectedItem01 = new SelectListItem { Text = "LD", Value = "1", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "DS", Value = "2", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                else if (Convert.ToInt32(tab.CHRGETYPE) == 2)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "LD", Value = "1", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "DS", Value = "2", Selected = true };
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

                //---------SDPTTYPE-----------------//
                List<SelectListItem> selectedSDPTTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.SDPTTYPEID) == 1)
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "Import", Value = "1", Selected = true };
                    selectedSDPTTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Non PNR", Value = "9", Selected = false };
                    selectedSDPTTYPE1.Add(selectedItem11);
                }
                else
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "Import", Value = "1", Selected = false };
                    selectedSDPTTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Non PNR", Value = "9", Selected = true };
                    selectedSDPTTYPE1.Add(selectedItem11);
                }
                ViewBag.SDPTTYPEID = selectedSDPTTYPE1;
                //End
            }
            return View(tab);
        }



        //End of Form

        public ActionResult NForm(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            SlabMaster tab = new SlabMaster();



            ViewBag.ATARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");


            List<SelectListItem> selectedSDPTTYPE_n = new List<SelectListItem>();
            SelectListItem selectedItem_n = new SelectListItem { Text = "Import", Value = "1", Selected = true };
            selectedSDPTTYPE_n.Add(selectedItem_n);
            selectedItem_n = new SelectListItem { Text = "Non PNR", Value = "9", Selected = false };
            selectedSDPTTYPE_n.Add(selectedItem_n);
            ViewBag.ASDPTTYPEID = selectedSDPTTYPE_n;
            ViewBag.SDPTTYPEID = selectedSDPTTYPE_n;

            List<SelectListItem> selectedCHRGETYPE_n = new List<SelectListItem>();
            SelectListItem selectedItem_ = new SelectListItem { Text = "LD", Value = "1", Selected = true };
            selectedCHRGETYPE_n.Add(selectedItem_);
            selectedItem_ = new SelectListItem { Text = "DS", Value = "2", Selected = false };
            selectedCHRGETYPE_n.Add(selectedItem_);
            ViewBag.ACHRGETYPE = selectedCHRGETYPE_n;

            ViewBag.ASLABTID = new SelectList(context.slabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC");


            tab.SLABMDATE = DateTime.Now.Date;
            tab.SLABMID = 0;
            //-------------------------Dropdown List------//
            //  ViewBag.STMRID = new SelectList(context.tariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x=>x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.YRDTYPE = new SelectList(context.import_slab_yard_type_masters, "YRDTYPE", "YRDDESC");
            ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC");
            ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC");
            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");
            ViewBag.SLABTID = new SelectList(context.slabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC");

            //---------CHRGETYPE--------

            List<SelectListItem> selectedCHRGETYPE = new List<SelectListItem>();
            SelectListItem selectedItem0 = new SelectListItem { Text = "LD", Value = "1", Selected = false };
            selectedCHRGETYPE.Add(selectedItem0);
            selectedItem0 = new SelectListItem { Text = "DS", Value = "2", Selected = false };
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
                tab = context.slabmasters.Find(Convert.ToInt32(cid));
                //--------------------------------Selected values in Dropdown List-----------------------------//
               
                ViewBag.YRDTYPE = new SelectList(context.import_slab_yard_type_masters, "YRDTYPE", "YRDDESC", tab.YRDTYPE);
                ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC", tab.HTYPE);
                ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC", tab.WTYPE);
                ViewBag.STMRID = new SelectList(context.tariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.STMRID);
                ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.CONTNRSDESC), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.slabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
                //---------CHRGETYPE------------//
                List<SelectListItem> selectedCHRGETYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.CHRGETYPE) == 1)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "LD", Value = "1", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "DS", Value = "2", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                else if (Convert.ToInt32(tab.CHRGETYPE) == 2)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "LD", Value = "1", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "DS", Value = "2", Selected = true };
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
                tab = context.slabmasters.Find(id);
                //--------------------Selected values in Dropdown List------------------------//
                ViewBag.YRDTYPE = new SelectList(context.import_slab_yard_type_masters, "YRDTYPE", "YRDDESC", tab.YRDTYPE);
                ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC", tab.HTYPE);
                ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC", tab.WTYPE);
                ViewBag.STMRID = new SelectList(context.tariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.STMRID);
                ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.CONTNRSDESC), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.slabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
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
                    SelectListItem selectedItem01 = new SelectListItem { Text = "LD", Value = "1", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "DS", Value = "2", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                else if (Convert.ToInt32(tab.CHRGETYPE) == 2)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "LD", Value = "1", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "DS", Value = "2", Selected = true };
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
        }//End of transfer

        public string GetDetail(string id)
        {

            var param = id.Split('~');
            var TARIFFMID = Convert.ToInt32(param[0]);
            var CHRGETYPE = Convert.ToInt32(param[1]);
            var SLABTID = Convert.ToInt32(param[2]);
            var SDPTTYPEID = Convert.ToInt32(param[3]);
            //if (Session["FSTMRID"].ToString() == "0")
            //{
            var data = context.Database.SqlQuery<VW_IMPORT_RATECARDMASTER_FLX_ASSGN>("select * from VW_IMPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and CHRGETYPE=" + CHRGETYPE + " and SLABTID=" + SLABTID + " and SDPTTYPEID = " + SDPTTYPEID + "" ).ToList();
            var html = "";
            var contnrsdesc = ""; var yrddesc = ""; var slabdesc = ""; var handlng = ""; var wagedesc = "";
            foreach (var dt in data)
            {
                if (dt.CONTNRSID == 3) { contnrsdesc = "20"; } else if (dt.CONTNRSID == 4) { contnrsdesc = "40"; } else if (dt.CONTNRSID == 5) { contnrsdesc = "45"; } else { contnrsdesc = "NR"; }

                if (dt.YRDTYPE == 2) { yrddesc = "Open"; } 
                else if (dt.YRDTYPE == 3) { yrddesc = "Closed"; } 
                else { yrddesc = "NR"; }

                if (dt.SDTYPE == 1) { slabdesc = "R"; } else { slabdesc = "NR"; }
                //if (dt.HTYPE == 1) { handlng = "FLT"; } else if (dt.HTYPE == 2) { handlng = "TLT"; } else if (dt.HTYPE == 3) { handlng = "CRANE"; } else if (dt.HTYPE == 4) { handlng = "MANUAL"; } else if (dt.HTYPE == 5) { handlng = "OWN"; } else { handlng = "NR"; }
                if (dt.HTYPE == 1) { handlng = "Nil"; }
                else if (dt.HTYPE == 2) { handlng = "FLT"; } 
                else if (dt.HTYPE == 3) { handlng = "TLT"; }
                else if (dt.HTYPE == 4) { handlng = "CRANE"; } 
                else if (dt.HTYPE == 5) { handlng = "MANUAL"; } 
                else if (dt.HTYPE == 6) { handlng = "OWN"; } 
                else { handlng = "NR"; }
                if (dt.WTYPE == 1) { wagedesc = "PACKAGE"; } 
                else if (dt.WTYPE == 2) { wagedesc = "WEIGHT"; } 
                else if (dt.WTYPE == 3) { wagedesc = "SCRAP"; } 
                else if (dt.WTYPE == 4) { wagedesc = "L.CARGO"; } 
                else { wagedesc = "NR"; }
                html = html + "<tr><td><input type='text' id='SLABMID' name='SLABMID' value=0 class='hidden'><input type='text' id='ASLABMDATE' class='hidden' name='ASLABMDATE' value='" + dt.SLABMDATE.ToString("dd/MM/yyyy") + "'>" + dt.SLABMDATE.ToString("dd/MM/yyyy") + "</td>";
                html = html + "<td><input type='text' id='CONTNRSID' name='CONTNRSID' value=" + dt.CONTNRSID + " class='hidden'>" + contnrsdesc + "</td>";
                html = html + "<td><input type='text' id='YRDTYPE' name='YRDTYPE' value=" + dt.YRDTYPE + " class='hidden'>" + yrddesc + "</td>";
                html = html + "<td><input type='text' id='SDTYPE' name='SDTYPE' value=" + dt.SDTYPE + " class='hidden'>" + slabdesc + "</td>";
                html = html + "<td><input type='text' id='HTYPE' name='HTYPE' value=" + dt.HTYPE + " class='hidden'>" + handlng + "</td>";
                html = html + "<td><input type='text' id='WTYPE' name='WTYPE' value=" + dt.WTYPE + " class='hidden'>" + wagedesc + "</td>";
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
            SlabMaster SlabMaster = new SlabMaster();
            //  Int32 SLABMID = Convert.ToInt32(myfrm["SLABMID"]);
            string[] SDPTTYPEID = myfrm.GetValues("ASDPTTYPEID");
            string[] SLABMID = myfrm.GetValues("SLABMID");
            string[] CONTNRSID = myfrm.GetValues("CONTNRSID");
            string[] YRDTYPE = myfrm.GetValues("YRDTYPE");
            string[] SDTYPE = myfrm.GetValues("SDTYPE");

            string[] HTYPE = myfrm.GetValues("HTYPE");
            string[] WTYPE = myfrm.GetValues("WTYPE");

            string[] SLABMIN = myfrm.GetValues("SLABMIN");
            string[] SLABMAX = myfrm.GetValues("SLABMAX");
            string[] SLABAMT = myfrm.GetValues("SLABAMT");


            for (int i = 0; i < SLABMID.Count(); i++)
            {

                SlabMaster.STMRID = Convert.ToInt32(myfrm["STMRID"]);
                SlabMaster.CHRGETYPE = Convert.ToInt16(myfrm["CHRGETYPE"]);
                SlabMaster.SDPTTYPEID = Convert.ToInt16(myfrm["ASDPTTYPEID"]);

                SlabMaster.SLABMDATE = Convert.ToDateTime(myfrm["SLABMDATE"]);
                SlabMaster.COMPYID = Convert.ToInt32(Session["compyid"]);
                SlabMaster.CUSRID = Convert.ToString(Session["CUSRID"]);
                SlabMaster.DISPSTATUS = 0;

                SlabMaster.LMUSRID = 1;
                SlabMaster.PRCSDATE = DateTime.Now;
                SlabMaster.SLABTID = Convert.ToInt32(myfrm["SLABTID"]);
                SlabMaster.TARIFFMID = Convert.ToInt32(myfrm["TARIFFMID"]);
                SlabMaster.HTYPE = Convert.ToInt16(HTYPE[i]);
                SlabMaster.YRDTYPE = Convert.ToInt16(YRDTYPE[i]);
                SlabMaster.WTYPE = Convert.ToInt16(WTYPE[i]);
                SlabMaster.CONTNRSID = Convert.ToInt32(CONTNRSID[i]);
                SlabMaster.SDTYPE = Convert.ToInt16(SDTYPE[i]);
                SlabMaster.SLABMIN = Convert.ToDecimal(SLABMIN[i]);
                SlabMaster.SLABMAX = Convert.ToDecimal(SLABMAX[i]);
                SlabMaster.SLABAMT = Convert.ToDecimal(SLABAMT[i]);
                context.slabmasters.Add(SlabMaster);
                context.SaveChanges();
            }

            Response.Redirect("Index");
        }


        public ActionResult Copy(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            SlabMaster tab = new SlabMaster();
            tab.SLABMDATE = DateTime.Now.Date;
            tab.SLABMID = 0;
            //-------------------------Dropdown List------//
            //  ViewBag.STMRID = new SelectList(context.tariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x=>x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            
            ViewBag.YRDTYPE = new SelectList(context.import_slab_yard_type_masters, "YRDTYPE", "YRDDESC");
            ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC");
            ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC");

            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");
            ViewBag.SLABTID = new SelectList(context.slabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC");

            //---------CHRGETYPE--------

            List<SelectListItem> selectedCHRGETYPE = new List<SelectListItem>();
            SelectListItem selectedItem0 = new SelectListItem { Text = "LD", Value = "1", Selected = false };
            selectedCHRGETYPE.Add(selectedItem0);
            selectedItem0 = new SelectListItem { Text = "DS", Value = "2", Selected = false };
            selectedCHRGETYPE.Add(selectedItem0);
            ViewBag.CHRGETYPE = selectedCHRGETYPE;


            //---------SDTYPE--------

            List<SelectListItem> selectedSDTYPE = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "NotRequired", Value = "0", Selected = false };
            selectedSDTYPE.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "Required", Value = "1", Selected = true };
            selectedSDTYPE.Add(selectedItem1);
            ViewBag.SDTYPE = selectedSDTYPE;


            //---------SDPTTYPE--------
            List<SelectListItem> selectedSDPTTYPE = new List<SelectListItem>();
            SelectListItem selectedSDPItem1 = new SelectListItem { Text = "Import", Value = "1", Selected = false };
            selectedSDPTTYPE.Add(selectedSDPItem1);
            selectedItem1 = new SelectListItem { Text = "Non PNR", Value = "9", Selected = true };
            selectedSDPTTYPE.Add(selectedItem1);
            ViewBag.SDPTTYPEID = selectedSDPTTYPE;



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
                tab = context.slabmasters.Find(Convert.ToInt32(cid));
                //--------------------------------Selected values in Dropdown List-----------------------------//
                
                ViewBag.YRDTYPE = new SelectList(context.import_slab_yard_type_masters, "YRDTYPE", "YRDDESC", tab.YRDTYPE);
                ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC", tab.HTYPE);
                ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC", tab.WTYPE);

                ViewBag.STMRID = new SelectList(context.tariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.STMRID);
                ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.CONTNRSDESC), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.slabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
                //---------CHRGETYPE------------//
                List<SelectListItem> selectedCHRGETYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.CHRGETYPE) == 1)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "LD", Value = "1", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "DS", Value = "2", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                else if (Convert.ToInt32(tab.CHRGETYPE) == 2)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "LD", Value = "1", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "DS", Value = "2", Selected = true };
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

                //---------SDPTTYPE-----------------//
                List<SelectListItem> selectedSDPTTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.SDPTTYPEID) == 1)
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "Import", Value = "1", Selected = true };
                    selectedSDPTTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Non PNR", Value = "9", Selected = false };
                    selectedSDPTTYPE1.Add(selectedItem11);
                }
                else
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "Import", Value = "1", Selected = false };
                    selectedSDPTTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Non PNR", Value = "9", Selected = true };
                    selectedSDPTTYPE1.Add(selectedItem11);
                }
                ViewBag.SDPTTYPEID = selectedSDPTTYPE1;
            }
            if (id != 0 && id > 0)
            {
                // if (id != 0)//Edit Mode
                // {
                tab = context.slabmasters.Find(id);


                var sql = context.Database.SqlQuery<SlabMaster>("SELECT * FROM SLABMASTER WHERE (SLABMDATE='" + tab.SLABMDATE.ToString("dd/MM/yyyy") + "' ) AND (TARIFFMID=" + tab.TARIFFMID + ") AND (CHRGETYPE=" + tab.CHRGETYPE + ") AND (CONTNRSID=" + tab.CONTNRSID + ") AND (SDTYPE=" + tab.SDTYPE + ") AND (YRDTYPE=" + tab.YRDTYPE + ") AND (WTYPE=" + tab.WTYPE + ") AND (HTYPE=" + tab.HTYPE + ") AND (STMRID=" + tab.STMRID + ") AND (SLABTID=" + tab.SLABTID + ")").ToList();
                var html = "";

                foreach (var dt in sql)
                {
                    html = html + "<tr class='item-row'><td><input type='text' class='form-control SLABMIN' id='SLABMIN' name='SLABMIN' value=" + dt.SLABMIN + " /></td><td><input type='text' class='form-control SLABMAX' id='SLABMAX' name='SLABMAX' value=" + dt.SLABMAX + " /></td><td><input type='text' class='form-control SLABAMT' id='SLABAMT' name='SLABAMT' value=" + dt.SLABAMT + " /></td> <td><a class='' id='del_detail'> <i class='glyphicon glyphicon-trash' style='color:#ff0000;font-size:large'></i></a></td></tr>";
                }
                ViewBag.html = html;

                //--------------------Selected values in Dropdown List------------------------//
                
                ViewBag.YRDTYPE = new SelectList(context.import_slab_yard_type_masters, "YRDTYPE", "YRDDESC", tab.YRDTYPE);
                ViewBag.HTYPE = new SelectList(context.import_slab_handling_type_masters, "HTYPE", "HTYPEDESC", tab.HTYPE);
                ViewBag.WTYPE = new SelectList(context.import_slab_wages_type_masters, "WTYPE", "WTYPEDESC", tab.WTYPE);

                ViewBag.STMRID = new SelectList(context.tariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.STMRID);
                ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.CONTNRSDESC), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.slabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
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
                    SelectListItem selectedItem01 = new SelectListItem { Text = "LD", Value = "1", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "DS", Value = "2", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                else if (Convert.ToInt32(tab.CHRGETYPE) == 2)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "LD", Value = "1", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "DS", Value = "2", Selected = true };
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

                //---------SDPTTYPE-----------------//
                List<SelectListItem> selectedSDPTTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.SDPTTYPEID) == 1)
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "Import", Value = "1", Selected = true };
                    selectedSDPTTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Non PNR", Value = "9", Selected = false };
                    selectedSDPTTYPE1.Add(selectedItem11);
                }
                else
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "Import", Value = "1", Selected = false };
                    selectedSDPTTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Non PNR", Value = "9", Selected = true };
                    selectedSDPTTYPE1.Add(selectedItem11);
                }
                ViewBag.SDPTTYPEID = selectedSDPTTYPE1;
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
                        SlabMaster SlabMaster = new SlabMaster();
                        Int32 SLABMID = Convert.ToInt32(myfrm["SLABMID"]);
                        SlabMaster.SDPTTYPEID = Convert.ToInt32(myfrm["SDPTTYPEID"]);
                        SlabMaster.STMRID = Convert.ToInt32(myfrm["STMRID"]);
                        SlabMaster.CHRGETYPE = Convert.ToInt16(myfrm["CHRGETYPE"]);
                        SlabMaster.CONTNRSID = Convert.ToInt32(myfrm["CONTNRSID"]);
                        SlabMaster.SDTYPE = Convert.ToInt16(myfrm["SDTYPE"]);
                        SlabMaster.SLABMDATE = Convert.ToDateTime(myfrm["SLABMDATE"]);
                        SlabMaster.COMPYID = Convert.ToInt32(Session["compyid"]);
                        SlabMaster.CUSRID = Convert.ToString(Session["CUSRID"]);
                        SlabMaster.DISPSTATUS = 0;
                        SlabMaster.HTYPE = Convert.ToInt16(myfrm["HTYPE"]);
                        SlabMaster.LMUSRID = 1;
                        SlabMaster.PRCSDATE = DateTime.Now;
                        SlabMaster.SLABTID = Convert.ToInt32(myfrm["SLABTID"]);
                        SlabMaster.TARIFFMID = Convert.ToInt32(myfrm["TARIFFMID"]);
                        SlabMaster.YRDTYPE = Convert.ToInt16(myfrm["YRDTYPE"]);
                        SlabMaster.WTYPE = Convert.ToInt16(myfrm["WTYPE"]);

                        string[] SLABMIN = myfrm.GetValues("SLABMIN");
                        string[] SLABMAX = myfrm.GetValues("SLABMAX");
                        string[] SLABAMT = myfrm.GetValues("SLABAMT");

                        //string[] SLABUAMT = myfrm.GetValues("SLABUAMT");

                        int row = 0;
                        
                        foreach (var data in SLABAMT)
                        {
                            SlabMaster.SLABMIN = Convert.ToDecimal(SLABMIN[row]);
                            SlabMaster.SLABMAX = Convert.ToDecimal(SLABMAX[row]);
                            SlabMaster.SLABAMT = Convert.ToDecimal(SLABAMT[row]);

                            //SlabMaster.SLABUAMT = Convert.ToDecimal(SLABUAMT[row]);

                            context.slabmasters.Add(SlabMaster);
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
                            Response.Redirect("Form/-" + SlabMaster.SLABMID);
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
        public void checkdata(SlabMaster ex)
        {
            // Response.Write(ex.CHRGETYPE + "//" + ex.SLABMDATE);
            DateTime SLABDATE = Convert.ToDateTime(ex.SLABMDATE);
            var sql = context.Database.SqlQuery<int>("EXEC PR_ImportSlabDuplicateCheck @PSLABMDATE='" + SLABDATE.ToString("MM/dd/yyyy") + "',@PTARIFFMID=" + ex.TARIFFMID + ",@PCHRGETYPE=" + ex.CHRGETYPE + ",@PCONTNRSID=" + ex.CONTNRSID + ",@PSDTYPE=" + ex.SDTYPE + ",@PYRDTYPE=" + ex.YRDTYPE + ",@PWTYPE=" + ex.WTYPE + ",@PHTYPE=" + ex.HTYPE + ",@PSTMRID=" + ex.STMRID + ",@PSLABTID=" + ex.SLABTID + ",@CNT=0").ToList();
            Response.Write(sql[0]);
        }



        //--------------------------Insert or Modify data------------------------//
        //public void savedata(SlabMaster tab)
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
        //        SlabMaster user = context.slabmasters.FirstOrDefault(u => u.SLABTID == tab.SLABTID || u.CHRGETYPE == tab.CHRGETYPE || u.TARIFFMID == tab.TARIFFMID || u.CONTNRSID == tab.CONTNRSID || u.SDTYPE == tab.SDTYPE || u.YRDTYPE == tab.YRDTYPE || u.WTYPE == tab.WTYPE || u.HTYPE == tab.HTYPE || u.STMRID == tab.STMRID || u.SLABMIN == tab.SLABMIN || u.SLABMAX == tab.SLABMAX || u.SLABMDATE == tab.SLABMDATE);

        //        if (user == null)
        //        {
        //            context.slabmasters.Add(tab);
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
        [Authorize(Roles = "ImportSlabMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                SlabMaster slabmasters = context.slabmasters.Find(Convert.ToInt32(id));
                //  context.slabmasters.Remove(slabmasters);

                DateTime slabmdate = Convert.ToDateTime(slabmasters.SLABMDATE);
                context.Database.ExecuteSqlCommand("delete from SlabMaster where SLABTID=" + slabmasters.SLABTID + " AND TARIFFMID=" + slabmasters.TARIFFMID + " AND CHRGETYPE=" + slabmasters.CHRGETYPE + " AND CONTNRSID=" + slabmasters.CONTNRSID + " AND SDTYPE=" + slabmasters.SDTYPE + " AND YRDTYPE=" + slabmasters.YRDTYPE + " AND WTYPE=" + slabmasters.WTYPE + " AND HTYPE=" + slabmasters.HTYPE + " AND STMRID=" + slabmasters.STMRID + " AND SLABMDATE='" + slabmdate.ToString("MM/dd/yyyy") + "'");
                //context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//End of Delete
    }
}