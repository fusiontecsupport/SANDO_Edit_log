
using scfs.Data;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using scfs_erp;

namespace scfs_erp.Controllers.Export
{
    [SessionExpire]
    public class ExportSlabMasterController : Controller
    {
        // GET: ExportSlabMaster

        SCFSERPContext context = new SCFSERPContext();

        ExportSlabMaster tab = new ExportSlabMaster();
        //[Authorize(Roles = "ExportSlabMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            if (Request.Form.Get("TARIFFMID") != null)
            {
                Session["TARIFFMID"] = Request.Form.Get("TARIFFMID");
                Session["SLABTID"] = Request.Form.Get("SLABTID");
                Session["CHRGETYPE"] = Request.Form.Get("CHRGETYPE");
            }
            else
            {
                Session["TARIFFMID"] = 1;
                Session["SLABTID"] = 2;
                Session["CHRGETYPE"] = 1;
            }

            ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", Session["TARIFFMID"]);
            ViewBag.SLABTID = new SelectList(context.exportslabtypemaster.Where(x => x.SLABTID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", Session["SLABTID"]);
            ViewBag.CHRGETYPE = new SelectList(context.exportslabchargetype.OrderBy(x => x.CHRGETYPEDESC), "CHRGETYPE", "CHRGETYPEDESC", Session["CHRGETYPE"] );
            // Response.Write(Session["FCHAID"]);
            return View(context.exportslabmasters.ToList());//Loading Grid
        }
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                if (Convert.ToInt32(Session["TARIFFMID"])  == 1)
                {
                    var data = e.pr_Search_Export_SlabMaster(param.sSearch,
                                                   Convert.ToInt32(Request["iSortCol_0"]),
                                                   Request["sSortDir_0"],
                                                   param.iDisplayStart,
                                                   param.iDisplayStart + param.iDisplayLength,
                                                   totalRowsCount,
                                                   filteredRowsCount, Convert.ToInt32(Session["TARIFFMID"]), Convert.ToInt32(Session["CHRGETYPE"]), Convert.ToInt32(Session["SLABTID"])).ToList();

                    var aaData = data.Select(d => new string[] { d.SLABMDATE.ToString("dd/MM/yyyy"), d.CONTNRSID, d.YRDTYPE, d.SDTYPE, d.EOPTDESC, d.WTYPE, d.SLABMIN.ToString(), d.SLABMAX.ToString(), d.SLABAMT.ToString(), d.SLABMID.ToString() }).ToArray();

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
                    var data = e.pr_Search_Export_SlabMaster_CHAWise(param.sSearch,
                                                  Convert.ToInt32(Request["iSortCol_0"]),
                                                  Request["sSortDir_0"],
                                                  param.iDisplayStart,
                                                  param.iDisplayStart + param.iDisplayLength,
                                                  totalRowsCount,
                                                  filteredRowsCount, Convert.ToInt32(Session["TARIFFMID"]), Convert.ToInt32(Session["CHRGETYPE"]), Convert.ToInt32(Session["SLABTID"]), Convert.ToInt32(Session["FCHAID"])).ToList();
                                        
                    var aaData = data.Select(d => new string[] { d.SLABMDATE.Value.ToString("dd/MM/yyyy"), d.CONTNRSID, d.YRDTYPE, d.SDTYPE,  d.EOPTDESC, d.WTYPE, d.SLABMIN.ToString(), d.SLABMAX.ToString(), d.SLABAMT.ToString(), d.SLABMID.ToString() }).ToArray();

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

        //[Authorize(Roles = "ExportSlabMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/ExportSlabMaster/Copy/" + id);


            //Response.Redirect("/ExportSlabMaster/Copy/" + id);
        }

        //----------------------Initializing Form--------------------------//
        //[Authorize(Roles = "ExportSlabMasterCreate")]
        public ActionResult Form(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ExportSlabMaster tab = new ExportSlabMaster();
            tab.SLABMID = 0;
            tab.SLABMDATE = DateTime.Now;


            //-------------------------------------Dropdown List---------------------------------------//
            //-------------------------------------Dropdown List---------------------------------------//
            ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.YRDTYPE = new SelectList(context.exportslabyardtype, "YRDTYPE", "YRDDESC");
            ViewBag.WTYPE = new SelectList(context.exportslabwagestype, "WTYPE", "WTYPEDESC");
            ViewBag.EOPTID = new SelectList(context.Export_OperationTypeMaster, "EOPTID", "EOPTDESC");
            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");
            ViewBag.SLABTID = new SelectList(context.exportslabtypemaster.Where(x => x.SLABTID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC");
            ViewBag.CHRGETYPE = new SelectList(context.exportslabchargetype, "CHRGETYPE", "CHRGETYPEDESC");
            //   ViewBag.CHAID = new SelectList(context.categorymasters.Where(u => u.CATETID == 4), "CATEID", "CATENAME");

            //--------------------------SDTYPE--------

            List<SelectListItem> selectedSDTYPE = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "NotRequired", Value = "0", Selected = false };
            selectedSDTYPE.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "Required", Value = "1", Selected = true };
            selectedSDTYPE.Add(selectedItem1);
            ViewBag.SDTYPE = selectedSDTYPE;


            //-----------------DISPSTATUS----

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem5 = new SelectListItem { Text = "Disabled", Value = "0", Selected = false };
            selectedDISPSTATUS.Add(selectedItem5);
            selectedItem5 = new SelectListItem { Text = "Enabled", Value = "1", Selected = true };
            selectedDISPSTATUS.Add(selectedItem5);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            if (id < 0)
            {
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";
                string cid = id.ToString();
                cid = cid.Remove(0, 1);
                tab = context.exportslabmasters.Find(Convert.ToInt32(cid));
                //--------------------------------Selected values in Dropdown List-----------------------------//
                ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.YRDTYPE = new SelectList(context.exportslabyardtype, "YRDTYPE", "YRDDESC", tab.YRDTYPE);
                ViewBag.WTYPE = new SelectList(context.exportslabwagestype, "WTYPE", "WTYPEDESC", tab.WTYPE);
                
                ViewBag.EOPTID = new SelectList(context.Export_OperationTypeMaster, "EOPTID", "EOPTDESC", tab.EOPTID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.exportslabtypemaster.Where(x => x.SLABTID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
                ViewBag.CHRGETYPE = new SelectList(context.exportslabchargetype, "CHRGETYPE", "CHRGETYPEDESC", tab.CHRGETYPE);
                if (tab.CHAID != 0)
                {
                    CategoryMaster category = context.categorymasters.Find(tab.CHAID);
                    ViewBag.CHADESC = category.CATENAME;
                }
            }
            if (id != 0 && id > 0)
            {
                // if (id != 0)//Edit Mode
                //{
                tab = context.exportslabmasters.Find(id);
                //--------------------------------Selected values in Dropdown List-----------------------------//
                ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.YRDTYPE = new SelectList(context.exportslabyardtype, "YRDTYPE", "YRDDESC", tab.YRDTYPE);
                ViewBag.WTYPE = new SelectList(context.exportslabwagestype, "WTYPE", "WTYPEDESC", tab.WTYPE);
                ViewBag.EOPTID = new SelectList(context.Export_OperationTypeMaster, "EOPTID", "EOPTDESC", tab.EOPTID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.exportslabtypemaster.Where(x => x.SLABTID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
                ViewBag.CHRGETYPE = new SelectList(context.exportslabchargetype, "CHRGETYPE", "CHRGETYPEDESC", tab.CHRGETYPE);
                if (tab.CHAID != 0)
                {
                    CategoryMaster category = context.categorymasters.Find(tab.CHAID);
                    ViewBag.CHADESC = category.CATENAME;
                }

                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 0)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "0", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "1", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
                //---------------------SDTYPE-------------------------------------//
                List<SelectListItem> selectedSDTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.SDTYPE) == 0)
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "NotRequired", Value = "0", Selected = true };
                    selectedSDTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Required", Value = "1", Selected = false };
                    selectedSDTYPE1.Add(selectedItem11);
                    ViewBag.SDTYPE = selectedSDTYPE1;
                }
            }
            return View(tab);
        }//End of Form

        public ActionResult NForm(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ExportSlabMaster tab = new ExportSlabMaster();
            tab.SLABMID = 0;
            tab.SLABMDATE = DateTime.Now;
            ViewBag.ATARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.ASLABTID = new SelectList(context.exportslabtypemaster.Where(x => x.SLABTID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC");
            //-------------------------------------Dropdown List---------------------------------------//
            ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.YRDTYPE = new SelectList(context.exportslabyardtype, "YRDTYPE", "YRDDESC");
            ViewBag.WTYPE = new SelectList(context.exportslabwagestype, "WTYPE", "WTYPEDESC");
            ViewBag.EOPTID = new SelectList(context.Export_OperationTypeMaster, "EOPTID", "EOPTDESC");

            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");
            ViewBag.SLABTID = new SelectList(context.exportslabtypemaster.Where(x => x.SLABTID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC");
            ViewBag.CHRGETYPE = new SelectList(context.exportslabchargetype, "CHRGETYPE", "CHRGETYPEDESC");
            //   ViewBag.CHAID = new SelectList(context.categorymasters.Where(u => u.CATETID == 4), "CATEID", "CATENAME");

            //--------------------------SDTYPE--------

            List<SelectListItem> selectedSDTYPE = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "NotRequired", Value = "0", Selected = false };
            selectedSDTYPE.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "Required", Value = "1", Selected = true };
            selectedSDTYPE.Add(selectedItem1);
            ViewBag.SDTYPE = selectedSDTYPE;


            //-----------------DISPSTATUS----

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem5 = new SelectListItem { Text = "Disabled", Value = "0", Selected = false };
            selectedDISPSTATUS.Add(selectedItem5);
            selectedItem5 = new SelectListItem { Text = "Enabled", Value = "1", Selected = true };
            selectedDISPSTATUS.Add(selectedItem5);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            if (id < 0)
            {
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";
                string cid = id.ToString();
                cid = cid.Remove(0, 1);
                tab = context.exportslabmasters.Find(Convert.ToInt32(cid));
                //--------------------------------Selected values in Dropdown List-----------------------------//
                ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.YRDTYPE = new SelectList(context.exportslabyardtype, "YRDTYPE", "YRDDESC", tab.YRDTYPE);
                ViewBag.WTYPE = new SelectList(context.exportslabwagestype, "WTYPE", "WTYPEDESC", tab.WTYPE);
                
                ViewBag.EOPTID = new SelectList(context.Export_OperationTypeMaster, "EOPTID", "EOPTDESC", tab.EOPTID);

                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.exportslabtypemaster.Where(x => x.SLABTID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
                ViewBag.CHRGETYPE = new SelectList(context.exportslabchargetype, "CHRGETYPE", "CHRGETYPEDESC", tab.CHRGETYPE);
                if (tab.CHAID != 0)
                {
                    CategoryMaster category = context.categorymasters.Find(tab.CHAID);
                    ViewBag.CHADESC = category.CATENAME;
                }
            }
            if (id != 0 && id > 0)
            {
                // if (id != 0)//Edit Mode
                //{
                tab = context.exportslabmasters.Find(id);
                //--------------------------------Selected values in Dropdown List-----------------------------//
                ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.YRDTYPE = new SelectList(context.exportslabyardtype, "YRDTYPE", "YRDDESC", tab.YRDTYPE);
                ViewBag.WTYPE = new SelectList(context.exportslabwagestype, "WTYPE", "WTYPEDESC", tab.WTYPE);
                ViewBag.EOPTID = new SelectList(context.Export_OperationTypeMaster, "EOPTID", "EOPTDESC", tab.EOPTID);

                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.exportslabtypemaster.Where(x => x.SLABTID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
                ViewBag.CHRGETYPE = new SelectList(context.exportslabchargetype, "CHRGETYPE", "CHRGETYPEDESC", tab.CHRGETYPE);
                if (tab.CHAID != 0)
                {
                    CategoryMaster category = context.categorymasters.Find(tab.CHAID);
                    ViewBag.CHADESC = category.CATENAME;
                }

                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 0)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "0", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "1", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
                //---------------------SDTYPE-------------------------------------//
                List<SelectListItem> selectedSDTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.SDTYPE) == 0)
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "NotRequired", Value = "0", Selected = true };
                    selectedSDTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Required", Value = "1", Selected = false };
                    selectedSDTYPE1.Add(selectedItem11);
                    ViewBag.SDTYPE = selectedSDTYPE1;
                }
            }
            return View(tab);
        }//End of Form


        public string GetDetail(string id)
        {

            var param = id.Split('~');
            var TARIFFMID = Convert.ToInt32(param[0]);
            var CHRGETYPE = Convert.ToInt32(param[1]);
            var SLABTID = Convert.ToInt32(param[2]);
            //if (Session["FSTMRID"].ToString() == "0")
            //{
            var data = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select * from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and CHRGETYPE=" + CHRGETYPE + " and SLABTID=" + SLABTID + "").ToList();
            var html = "";
            var contnrsdesc = ""; var yrddesc = ""; var slabdesc = ""; var handlng = ""; var wagedesc = ""; var eopdesc = "";
            foreach (var dt in data)
            {
                if (dt.CONTNRSID == 3) { contnrsdesc = "20"; } else if (dt.CONTNRSID == 4) { contnrsdesc = "40"; } else if (dt.CONTNRSID == 5) { contnrsdesc = "45"; } else { contnrsdesc = "NR"; }
                if (dt.YRDTYPE == 1) { yrddesc = "Open"; } else if (dt.YRDTYPE == 2) { yrddesc = "Closed"; } else { yrddesc = "NR"; }
                if (dt.SDTYPE == 1) { slabdesc = "R"; } else { slabdesc = "NR"; }
                if (dt.HTYPE == 1) { handlng = "FLT"; } else if (dt.HTYPE == 2) { handlng = "TLT"; } else if (dt.HTYPE == 3) { handlng = "CRANE"; } else if (dt.HTYPE == 4) { handlng = "MANUAL"; } else if (dt.HTYPE == 5) { handlng = "OWN"; } else { handlng = "NR"; }
                if (dt.WTYPE == 1) { wagedesc = "PACKAGE"; } else if (dt.WTYPE == 2) { wagedesc = "WEIGHT"; } else if (dt.WTYPE == 3) { wagedesc = "SCRAP"; } else if (dt.WTYPE == 4) { wagedesc = "L.CARGO"; } else { wagedesc = "NR"; }
                if (dt.EOPTID == 0) { eopdesc = "NR"; } else if (dt.EOPTID == 1) { eopdesc = "All"; } else if (dt.EOPTID == 2) { eopdesc = "OWM"; } else if (dt.EOPTID == 3) { eopdesc = "OWMC"; } else if (dt.EOPTID == 4) { eopdesc = "GSM"; } else if (dt.EOPTID == 5) { eopdesc = "GSMC"; } else if (dt.EOPTID == 6) { eopdesc = "SSOWINSP"; } else if (dt.EOPTID == 7) { eopdesc = "SSGI"; } else if (dt.EOPTID == 8) { eopdesc = "CWOWI"; } else if (dt.EOPTID == 9) { eopdesc = "CEGI"; } else { eopdesc = "OTHR"; }
                html = html + "<tr><td><input type='text' id='SLABMID' name='SLABMID' value=0 class='hidden'><input type='text' id='ASLABMDATE' class='hidden' name='ASLABMDATE' value='" + dt.SLABMDATE.ToString("dd/MM/yyyy") + "'>" + dt.SLABMDATE.ToString("dd/MM/yyyy") + "</td>";
                html = html + "<td><input type='text' id='CONTNRSID' name='CONTNRSID' value=" + dt.CONTNRSID + " class='hidden'>" + contnrsdesc + "</td>";
                html = html + "<td><input type='text' id='YRDTYPE' name='YRDTYPE' value=" + dt.YRDTYPE + " class='hidden'>" + yrddesc + "</td>";
                html = html + "<td><input type='text' id='SDTYPE' name='SDTYPE' value=" + dt.SDTYPE + " class='hidden'>" + slabdesc + "</td>";
                html = html + "<td><input type='text' id='HTYPE' name='HTYPE' value=" + dt.HTYPE + " class='hidden'>" + handlng + "</td>";
                html = html + "<td><input type='text' id='EOPTID' name='EOPTID' value=" + dt.EOPTID + " class='hidden'>" + eopdesc + "</td>";
                html = html + "<td><input type='text' id='WTYPE' name='WTYPE' value=" + dt.WTYPE + " class='hidden'>" + wagedesc + "</td>";
                html = html + "<td><input type='text' id='SLABMIN' name='SLABMIN' value='" + dt.SLABMIN + "' class='hidden1'></td>";
                html = html + "<td><input type='text' id='SLABMAX' name='SLABMAX' value='" + dt.SLABMAX + "' class='hidden1'></td>";
                html = html + "<td><input type='text' id='SLABAMT' name='SLABAMT' value='" + dt.SLABAMT + "' class='hidden1'></td>";
                html = html + "<td><input type='text' id='SLABUAMT' name='SLABUAMT' value='" + dt.SLABUAMT + "' class='hidden1'></td></tr>";

            }

            return html;
            //  return Json(data, JsonRequestBehavior.AllowGet);
            //   }

        }

        //public string GetDetail(string id)
        //{

        //    var param = id.Split('~');
        //    var TARIFFMID = Convert.ToInt32(param[0]);
        //    var CHRGETYPE = Convert.ToInt32(param[1]);
        //    var SLABTID = Convert.ToInt32(param[2]);
        //    //if (Session["FSTMRID"].ToString() == "0")
        //    //{
        //    var data = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select * from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and CHRGETYPE=" + CHRGETYPE + " and SLABTID=" + SLABTID + "").ToList();
        //    var html = "";
        //    var contnrsdesc = ""; var yrddesc = ""; var slabdesc = ""; var handlng = ""; var wagedesc = "";
        //    foreach (var dt in data)
        //    {
        //        if (dt.CONTNRSID == 3) { contnrsdesc = "20"; } else if (dt.CONTNRSID == 4) { contnrsdesc = "40"; } else if (dt.CONTNRSID == 5) { contnrsdesc = "45"; } else { contnrsdesc = "NR"; }
        //        if (dt.YRDTYPE == 1) { yrddesc = "Open"; } else if (dt.YRDTYPE == 2) { yrddesc = "Closed"; } else { yrddesc = "NR"; }
        //        if (dt.SDTYPE == 1) { slabdesc = "R"; } else { slabdesc = "NR"; }
        //        //if (dt.HTYPE == 1) { handlng = "FLT"; } else if (dt.HTYPE == 2) { handlng = "TLT"; } else if (dt.HTYPE == 3) { handlng = "CRANE"; } else if (dt.HTYPE == 4) { handlng = "MANUAL"; } else if (dt.HTYPE == 5) { handlng = "OWN"; } else { handlng = "NR"; }
        //        //if (dt.WTYPE == 1) { wagedesc = "PACKAGE"; } else if (dt.WTYPE == 2) { wagedesc = "WEIGHT"; } else if (dt.WTYPE == 3) { wagedesc = "SCRAP"; } else if (dt.WTYPE == 4) { wagedesc = "L.CARGO"; } else { wagedesc = "NR"; }
        //        html = html + "<tr><td><input type='text' id='SLABMID' name='SLABMID' value=0 class='hidden'><input type='text' id='ASLABMDATE' class='hidden' name='ASLABMDATE' value='" + dt.SLABMDATE.ToString("dd/MM/yyyy") + "'>" + dt.SLABMDATE.ToString("dd/MM/yyyy") + "</td>";
        //        html = html + "<td><input type='text' id='CONTNRSID' name='CONTNRSID' value=" + dt.CONTNRSID + " class='hidden'>" + contnrsdesc + "</td>";
        //        html = html + "<td><input type='text' id='YRDTYPE' name='YRDTYPE' value=" + dt.YRDTYPE + " class='hidden'>" + yrddesc + "</td>";
        //        html = html + "<td><input type='text' id='SDTYPE' name='SDTYPE' value=" + dt.SDTYPE + " class='hidden'>" + slabdesc + "</td>";
        //        //html = html + "<td><input type='text' id='HTYPE' name='HTYPE' value=" + dt.HTYPE + " class='hidden'>" + handlng + "</td>";
        //        //html = html + "<td><input type='text' id='WTYPE' name='WTYPE' value=" + dt.WTYPE + " class='hidden'>" + wagedesc + "</td>";
        //        html = html + "<td><input type='text' id='SLABMIN' name='SLABMIN' value='" + dt.SLABMIN + "' class='hidden1'></td>";
        //        html = html + "<td><input type='text' id='SLABMAX' name='SLABMAX' value='" + dt.SLABMAX + "' class='hidden1'></td>";
        //        html = html + "<td><input type='text' id='SLABAMT' name='SLABAMT' value='" + dt.SLABAMT + "' class='hidden1'></td>";
        //        html = html + "<td><input type='text' id='SLABUAMT' name='SLABUAMT' value='" + dt.SLABUAMT + "' class='hidden1'></td></tr>";

        //    }

        //    return html;
        //    //  return Json(data, JsonRequestBehavior.AllowGet);
        //    //   }

        //}

        public void nsavedata(FormCollection myfrm)
        {
            ExportSlabMaster ExportSlabMaster = new ExportSlabMaster();
            //  Int32 SLABMID = Convert.ToInt32(myfrm["SLABMID"]);
            string[] SLABMID = myfrm.GetValues("SLABMID");
            string[] CONTNRSID = myfrm.GetValues("CONTNRSID");
            string[] YRDTYPE = myfrm.GetValues("YRDTYPE");
            string[] SDTYPE = myfrm.GetValues("SDTYPE");

            string[] HTYPE = myfrm.GetValues("HTYPE");
            string[] WTYPE = myfrm.GetValues("WTYPE");

            string[] EOPTID = myfrm.GetValues("EOPTID");

            string[] SLABMIN = myfrm.GetValues("SLABMIN");
            string[] SLABMAX = myfrm.GetValues("SLABMAX");
            string[] SLABAMT = myfrm.GetValues("SLABAMT");
            string[] SLABUAMT = myfrm.GetValues("SLABUAMT");

            for (int i = 0; i < SLABMID.Count(); i++)
            {

                ExportSlabMaster.CHAID = Convert.ToInt32(myfrm["CHAID"]);
                ExportSlabMaster.CHRGETYPE = Convert.ToInt16(myfrm["CHRGETYPE"]);

                ExportSlabMaster.SLABMDATE = Convert.ToDateTime(myfrm["SLABMDATE"]);
                ExportSlabMaster.COMPYID = Convert.ToInt32(Session["compyid"]);
                ExportSlabMaster.CUSRID = Convert.ToString(Session["CUSRID"]);
                ExportSlabMaster.DISPSTATUS = 0;

                //ExportSlabMaster.LMUSRID = 1;
                ExportSlabMaster.LMUSRID = Convert.ToString(Session["CUSRID"]);
                ExportSlabMaster.PRCSDATE = DateTime.Now;
                ExportSlabMaster.SLABTID = Convert.ToInt32(myfrm["SLABTID"]);
                ExportSlabMaster.TARIFFMID = Convert.ToInt32(myfrm["TARIFFMID"]);
                ExportSlabMaster.HTYPE = Convert.ToInt16(HTYPE[i]);
                ExportSlabMaster.YRDTYPE = Convert.ToInt16(YRDTYPE[i]);
                ExportSlabMaster.WTYPE = Convert.ToInt16(WTYPE[i]);
                ExportSlabMaster.EOPTID = Convert.ToInt32(EOPTID[i]);
                ExportSlabMaster.CONTNRSID = Convert.ToInt32(CONTNRSID[i]);
                ExportSlabMaster.SDTYPE = Convert.ToInt16(SDTYPE[i]);
                ExportSlabMaster.SLABMIN = Convert.ToDecimal(SLABMIN[i]);
                ExportSlabMaster.SLABMAX = Convert.ToDecimal(SLABMAX[i]);
                ExportSlabMaster.SLABAMT = Convert.ToDecimal(SLABAMT[i]);
                ExportSlabMaster.SLABUAMT = Convert.ToDecimal(SLABUAMT[i]);
                context.exportslabmasters.Add(ExportSlabMaster);
                context.SaveChanges();
            }

            Response.Redirect("Index");
        }
        public ActionResult Copy(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ExportSlabMaster tab = new ExportSlabMaster();
            tab.SLABMID = 0;
            tab.SLABMDATE = DateTime.Now;


            //-------------------------------------Dropdown List---------------------------------------//
            ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.YRDTYPE = new SelectList(context.exportslabyardtype, "YRDTYPE", "YRDDESC");
            ViewBag.WTYPE = new SelectList(context.exportslabwagestype, "WTYPE", "WTYPEDESC");
            ViewBag.EOPTID = new SelectList(context.Export_OperationTypeMaster, "EOPTID", "EOPTDESC");
            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");
            ViewBag.SLABTID = new SelectList(context.exportslabtypemaster.Where(x => x.SLABTID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC");
            ViewBag.CHRGETYPE = new SelectList(context.exportslabchargetype, "CHRGETYPE", "CHRGETYPEDESC");
            //   ViewBag.CHAID = new SelectList(context.categorymasters.Where(u => u.CATETID == 4), "CATEID", "CATENAME");

            //--------------------------SDTYPE--------

            List<SelectListItem> selectedSDTYPE = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "NotRequired", Value = "0", Selected = false };
            selectedSDTYPE.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "Required", Value = "1", Selected = true };
            selectedSDTYPE.Add(selectedItem1);
            ViewBag.SDTYPE = selectedSDTYPE;


            //-----------------DISPSTATUS----

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem5 = new SelectListItem { Text = "Disabled", Value = "0", Selected = false };
            selectedDISPSTATUS.Add(selectedItem5);
            selectedItem5 = new SelectListItem { Text = "Enabled", Value = "1", Selected = true };
            selectedDISPSTATUS.Add(selectedItem5);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            if (id < 0)
            {
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";
                string cid = id.ToString();
                cid = cid.Remove(0, 1);
                tab = context.exportslabmasters.Find(Convert.ToInt32(cid));
                //--------------------------------Selected values in Dropdown List-----------------------------//
                ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.YRDTYPE = new SelectList(context.exportslabyardtype, "YRDTYPE", "YRDDESC", tab.YRDTYPE);
                ViewBag.WTYPE = new SelectList(context.exportslabwagestype, "WTYPE", "WTYPEDESC", tab.WTYPE);
                ViewBag.EOPTID = new SelectList(context.Export_OperationTypeMaster, "EOPTID", "EOPTDESC",tab.EOPTID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.exportslabtypemaster.Where(x => x.SLABTID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
                ViewBag.CHRGETYPE = new SelectList(context.exportslabchargetype, "CHRGETYPE", "CHRGETYPEDESC", tab.CHRGETYPE);
                if (tab.CHAID != 0)
                {
                    CategoryMaster category = context.categorymasters.Find(tab.CHAID);
                    ViewBag.CHADESC = category.CATENAME;
                }
            }
            if (id != 0 && id > 0)
            {
                // if (id != 0)//Edit Mode
                //{
                tab = context.exportslabmasters.Find(id);


                var sql = context.Database.SqlQuery<ExportSlabMaster>("SELECT * FROM EXPORTSLABMASTER WHERE (SLABMDATE='" + tab.SLABMDATE.ToString("dd/MM/yyyy") + "' ) AND (TARIFFMID=" + tab.TARIFFMID + ") AND (CHRGETYPE=" + tab.CHRGETYPE + ") AND (CONTNRSID=" + tab.CONTNRSID + ") AND (SDTYPE=" + tab.SDTYPE + ") AND (YRDTYPE=" + tab.YRDTYPE + ") AND (WTYPE=" + tab.WTYPE + ") AND (HTYPE=" + tab.HTYPE + ") AND (CHAID=" + tab.CHAID + ") AND (SLABTID=" + tab.SLABTID + ")").ToList();
                var html = "";

                foreach (var dt in sql)
                {
                    html = html + "<tr class='item-row'><td><input type='text' class='form-control SLABMIN' id='SLABMIN' name='SLABMIN' value=" + dt.SLABMIN + " /></td><td><input type='text' class='form-control SLABMAX' id='SLABMAX' name='SLABMAX' value=" + dt.SLABMAX + " /></td><td><input type='text' class='form-control SLABAMT' id='SLABAMT' name='SLABAMT' value=" + dt.SLABAMT + " /></td><td><input type='text' class='form-control SLABUAMT' id='SLABUAMT' name='SLABUAMT' value=" + dt.SLABUAMT + " /></td> <td><a class='' id='del_detail'> <i class='glyphicon glyphicon-trash' style='color:#ff0000;font-size:large'></i></a></td></tr>";
                }
                ViewBag.html = html;
                //--------------------------------Selected values in Dropdown List-----------------------------//
                ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.YRDTYPE = new SelectList(context.exportslabyardtype, "YRDTYPE", "YRDDESC", tab.YRDTYPE);
                ViewBag.WTYPE = new SelectList(context.exportslabwagestype, "WTYPE", "WTYPEDESC", tab.WTYPE);
                ViewBag.EOPTID = new SelectList(context.Export_OperationTypeMaster, "EOPTID", "EOPTDESC",tab.EOPTID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.SLABTID = new SelectList(context.exportslabtypemaster.Where(x => x.SLABTID > 1 && x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
                ViewBag.CHRGETYPE = new SelectList(context.exportslabchargetype, "CHRGETYPE", "CHRGETYPEDESC", tab.CHRGETYPE);
                if (tab.CHAID != 0)
                {
                    CategoryMaster category = context.categorymasters.Find(tab.CHAID);
                    ViewBag.CHADESC = category.CATENAME;
                }

                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 0)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "0", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "1", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
                //---------------------SDTYPE-------------------------------------//
                List<SelectListItem> selectedSDTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.SDTYPE) == 0)
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "NotRequired", Value = "0", Selected = true };
                    selectedSDTYPE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "Required", Value = "1", Selected = false };
                    selectedSDTYPE1.Add(selectedItem11);
                    ViewBag.SDTYPE = selectedSDTYPE1;
                }
            }
            return View(tab);
        }//End of Form


        //---------------------Insert or Modify data------------------//
        public void savedata(FormCollection myfrm)
        {

            using (SCFSERPContext dataContext = new SCFSERPContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        ExportSlabMaster ExportSlabMaster = new ExportSlabMaster();
                        Int32 SLABMID = Convert.ToInt32(myfrm["SLABMID"]);

                        ExportSlabMaster.CHAID = Convert.ToInt32(myfrm["CHAID"]);
                        ExportSlabMaster.CHRGETYPE = Convert.ToInt16(myfrm["CHRGETYPE"]);
                        ExportSlabMaster.CONTNRSID = Convert.ToInt32(myfrm["CONTNRSID"]);
                        ExportSlabMaster.SDTYPE = Convert.ToInt16(myfrm["SDTYPE"]);
                        ExportSlabMaster.SLABMDATE = Convert.ToDateTime(myfrm["SLABMDATE"]);
                        ExportSlabMaster.COMPYID = Convert.ToInt32(Session["compyid"]);
                        ExportSlabMaster.CUSRID = Convert.ToString(Session["CUSRID"]);                        
                        ExportSlabMaster.DISPSTATUS = 0;
                        ExportSlabMaster.HTYPE = 0;
                        ExportSlabMaster.LMUSRID = Convert.ToString(Session["CUSRID"]);
                        //ExportSlabMaster.LMUSRID = 1;
                        ExportSlabMaster.PRCSDATE = DateTime.Now;
                        ExportSlabMaster.SLABTID = Convert.ToInt32(myfrm["SLABTID"]);
                        ExportSlabMaster.TARIFFMID = Convert.ToInt32(myfrm["TARIFFMID"]);
                        ExportSlabMaster.YRDTYPE = Convert.ToInt16(myfrm["YRDTYPE"]);
                        ExportSlabMaster.WTYPE = Convert.ToInt16(myfrm["WTYPE"]);
                        ExportSlabMaster.EOPTID = Convert.ToInt32(myfrm["EOPTID"]);

                        string[] SLABMIN = myfrm.GetValues("SLABMIN");
                        string[] SLABMAX = myfrm.GetValues("SLABMAX");
                        string[] SLABAMT = myfrm.GetValues("SLABAMT");
                        string[] SLABUAMT = myfrm.GetValues("SLABUAMT");
                        int row = 0;
                        foreach (var data in SLABAMT)
                        {
                            ExportSlabMaster.SLABMIN = Convert.ToDecimal(SLABMIN[row]);
                            ExportSlabMaster.SLABMAX = Convert.ToDecimal(SLABMAX[row]);
                            ExportSlabMaster.SLABAMT = Convert.ToDecimal(SLABAMT[row]);
                            ExportSlabMaster.SLABUAMT = Convert.ToDecimal(SLABUAMT[row]);

                            context.exportslabmasters.Add(ExportSlabMaster);
                            context.SaveChanges();
                            row++;
                        }
                        //if (tab.SLABMID.ToString() != "0")
                        //{
                        //    context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                        //    context.SaveChanges();
                        //}
                        //else
                        //{
                        //  //  var duplcate = context.exportslabmasters.Where(u => u.SLABTID == Convert.ToInt32(SLABTID)) ;
                        //   // Response.Write(duplcate.Count());


                        //    context.exportslabmasters.Add(tab);
                        //    context.SaveChanges();
                        //}
                        trans.Commit();

                        if (Request.Form.Get("continue") == null)
                        {
                            Response.Write("Saved");
                        }
                        else
                        {
                            //Response.Redirect("Form/-1");
                            Response.Redirect("Form/-" + ExportSlabMaster.SLABMID);
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
        public void checkdata(ExportSlabMaster ex)
        {
            // Response.Write(ex.CHRGETYPE + "//" + ex.SLABMDATE);
            DateTime SLABDATE = Convert.ToDateTime(ex.SLABMDATE);
            var sql = context.Database.SqlQuery<int>("EXEC PR_ExportSlabDuplicateCheck @PSLABMDATE='" + SLABDATE.ToString("MM/dd/yyyy") + "',@PTARIFFMID=" + ex.TARIFFMID + ",@PCHRGETYPE=" + ex.CHRGETYPE + ",@PCONTNRSID=" + ex.CONTNRSID + ",@PSDTYPE=" + ex.SDTYPE + ",@PYRDTYPE=" + ex.YRDTYPE + ",@PWTYPE=" + ex.WTYPE + ",@PHTYPE=" + ex.HTYPE + ",@PCHAID=" + ex.CHAID + ",@PSLABTID=" + ex.SLABTID + ",@PEOPTID=" + ex.EOPTID + ", @CNT=0").ToList();
            Response.Write(sql[0]);
        }


        //----------Autocomplete CHA Name
        public JsonResult AutoCha(string term)
        {
            var result = (from cha in context.categorymasters.Where(u => u.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where cha.CATENAME.ToLower().Contains(term.ToLower())
                          select new { cha.CATENAME, cha.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //---End of vessel


        //------------------------Delete Record----------//
        //[Authorize(Roles = "ExportSlabMasterDelete")]
        public void Del()
        {

            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                ExportSlabMaster exportslabmasters = context.exportslabmasters.Find(Convert.ToInt32(id));
                DateTime slabmdate = Convert.ToDateTime(exportslabmasters.SLABMDATE);
                context.Database.ExecuteSqlCommand("delete from ExportSlabMaster where SLABTID=" + exportslabmasters.SLABTID + " AND TARIFFMID=" + exportslabmasters.TARIFFMID + " AND CHRGETYPE=" + exportslabmasters.CHRGETYPE + " AND CONTNRSID=" + exportslabmasters.CONTNRSID + " AND SDTYPE=" + exportslabmasters.SDTYPE + " AND YRDTYPE=" + exportslabmasters.YRDTYPE + " AND WTYPE=" + exportslabmasters.WTYPE + " AND HTYPE=" + exportslabmasters.HTYPE + " AND CHAID=" + exportslabmasters.CHAID + " AND EOPTID=" + exportslabmasters.EOPTID + " AND SLABMDATE='" + slabmdate.ToString("MM/dd/yyyy") + "'");
                //context.exportslabmasters.Remove(exportslabmasters);
                // context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//End of Delete
    }
}