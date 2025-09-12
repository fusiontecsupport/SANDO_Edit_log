using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using scfs.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using scfs_erp;

namespace scfs.Controllers.Import
{
    [SessionExpire]
    public class ImportGateInBlockController : Controller
    {
        // GET: ImportGateInBlock

        #region Context declaration
        SCFSERPContext context = new SCFSERPContext();
        CFSImportEntities db = new CFSImportEntities();
        #endregion

        #region Index Page
        [Authorize(Roles = "GateInBlockIndex")]
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

            return View(context.gateinblockdetails.Where(x => x.GBDATE >= sd).Where(x => x.GBDATE <= ed).ToList());

            //return View();
        }
        #endregion

        #region GetAjaxData
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {

            using (db = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = db.pr_Search_Import_GateInBlock(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]));

                var aaData = data.Select(d => new string[] { d.GBDATE.Value.ToString("dd/MM/yyyy"), d.GBNO.ToString(), d.CONTNRNO, d.CONTNRSID, d.IGMNO, d.GBTYPE, d.VSLNAME, d.STMRNAME, d.PRDTDESC, d.GPSTYPE, d.GPSCNTYPE, d.DISPSTATUS, d.GBDID.ToString() }).ToArray();

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

        #region Gate in Block Edit
        [Authorize(Roles = "GateInBlockEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ImportGateInBlock/Form/" + id);
        }
        #endregion

        #region Form
        [Authorize(Roles = "GateInBlockCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GateInBlockDetails tab = new GateInBlockDetails();
            tab.GBDATE = DateTime.Now;
            tab.GBUDATE = DateTime.Now;
            tab.GBTIME = DateTime.Now;
            tab.GBDID = 0;

            //-----------------------------blocktype--------
            List<SelectListItem> selectedBLCK = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Customs Block", Value = "0", Selected = false };
            selectedBLCK.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "SCFS Block", Value = "1", Selected = true };
            selectedBLCK.Add(selectedItem);
            ViewBag.GBTYPE = selectedBLCK;

            //-----------------------------type-----------
            List<SelectListItem> selectedType = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "Block", Value = "0", Selected = false };
            //selectedType.Add(selectedItem1);
            //selectedItem1 = new SelectListItem { Text = "UnBlock", Value = "1", Selected = true };
            selectedType.Add(selectedItem1);
            ViewBag.GBSTYPE = selectedType;


            if (id != 0)//Edit Mode
            {
                tab = context.gateinblockdetails.Find(id);

                //-----------Getting Gate_In Details-----------------//
                GateInDetail gdet = context.gateindetails.Find(tab.GIDID);
                if (tab.GIDID == gdet.GIDID)
                {

                    ViewBag.CONTNRNO = gdet.CONTNRNO;
                    ViewBag.GPETYPE = gdet.CONTNRSID;
                    ViewBag.GPLNO = gdet.GPLNO;
                    ViewBag.GIDATE = gdet.GIDATE;
                    ViewBag.VOYNO = gdet.VOYNO;
                    ViewBag.IGMNO = gdet.IGMNO;
                    ViewBag.VSLNAME = gdet.VSLNAME;
                    ViewBag.IMPRTNAME = gdet.IMPRTNAME;
                    ViewBag.STMRNAME = gdet.STMRNAME;
                }//End

                //-----------------------------blocktype--------
                var aa = Session["Group"].ToString();
                if (Session["Group"].ToString() == "Admin" || Session["Group"].ToString() == "GroupAdmin" || Session["Group"].ToString() == "SuperAdmin")
                {
                    List<SelectListItem> selectedBLCK1 = new List<SelectListItem>();
                    if (Convert.ToInt16(tab.GBTYPE) == 0)
                    {
                        SelectListItem selectedItemBLCK = new SelectListItem { Text = "Customs Block", Value = "0", Selected = true };
                        selectedBLCK1.Add(selectedItemBLCK);
                        selectedItemBLCK = new SelectListItem { Text = "SCFS Block", Value = "1", Selected = false };
                        selectedBLCK1.Add(selectedItemBLCK);
                        ViewBag.GBTYPE = selectedBLCK1;
                    }
                    else
                    {
                        SelectListItem selectedItemBLCK = new SelectListItem { Text = "Customs Block", Value = "0", Selected = false };
                        selectedBLCK1.Add(selectedItemBLCK);
                        selectedItemBLCK = new SelectListItem { Text = "SCFS Block", Value = "1", Selected = true };
                        selectedBLCK1.Add(selectedItemBLCK);
                        ViewBag.GBTYPE = selectedBLCK1;
                    }
                }
                else
                {
                    List<SelectListItem> selectedBLCK1 = new List<SelectListItem>();
                    if (Convert.ToInt16(tab.GBTYPE) == 0)
                    {
                        SelectListItem selectedItemBLCK = new SelectListItem { Text = "Customs Block", Value = "0", Selected = true };
                        selectedBLCK1.Add(selectedItemBLCK);
                        //selectedItemBLCK = new SelectListItem { Text = "SCFS Block", Value = "1", Selected = false };
                        //selectedBLCK1.Add(selectedItemBLCK);
                        ViewBag.GBTYPE = selectedBLCK1;
                    }
                    else
                    {
                        SelectListItem selectedItemBLCK = new SelectListItem { Text = "SCFS Block", Value = "1", Selected = true };
                        selectedBLCK1.Add(selectedItemBLCK);
                        //selectedItemBLCK = new SelectListItem { Text = "SCFS Block", Value = "1", Selected = true };
                        //selectedBLCK1.Add(selectedItemBLCK);
                        ViewBag.GBTYPE = selectedBLCK1;
                    }
                }

                //-----------------------------type-----------
                if (Session["Group"].ToString() == "Admin" || Session["Group"].ToString() == "GroupAdmin" || Session["Group"].ToString() == "SuperAdmin")
                {
                    List<SelectListItem> selectedType1 = new List<SelectListItem>();
                    if (Convert.ToInt16(tab.GBSTYPE) == 0)
                    {
                        SelectListItem selectedItemSTYPE = new SelectListItem { Text = "Block", Value = "0", Selected = true };
                        selectedType1.Add(selectedItemSTYPE);
                        selectedItemSTYPE = new SelectListItem { Text = "UnBlock", Value = "1", Selected = false };
                        selectedType1.Add(selectedItemSTYPE);
                        ViewBag.GBSTYPE = selectedType1;
                    }
                    else
                    {
                        SelectListItem selectedItemSTYPE = new SelectListItem { Text = "Block", Value = "0", Selected = false };
                        selectedType1.Add(selectedItemSTYPE);
                        selectedItemSTYPE = new SelectListItem { Text = "UnBlock", Value = "1", Selected = true };
                        selectedType1.Add(selectedItemSTYPE);
                        ViewBag.GBSTYPE = selectedType1;
                    }
                }
                else
                {
                    if (Convert.ToInt16(tab.GBTYPE) == 0) //If Customs Block They can't change
                    {
                        List<SelectListItem> selectedType1 = new List<SelectListItem>();
                        if (Convert.ToInt16(tab.GBSTYPE) == 0)
                        {
                            SelectListItem selectedItemSTYPE = new SelectListItem { Text = "Block", Value = "0", Selected = true };
                            selectedType1.Add(selectedItemSTYPE);
                            //selectedItemSTYPE = new SelectListItem { Text = "UnBlock", Value = "1", Selected = false };
                            //selectedType1.Add(selectedItemSTYPE);
                            ViewBag.GBSTYPE = selectedType1;
                        }
                        else
                        {
                            SelectListItem selectedItemSTYPE = new SelectListItem { Text = "UnBlock", Value = "1", Selected = true };
                            selectedType1.Add(selectedItemSTYPE);
                            //selectedItemSTYPE = new SelectListItem { Text = "UnBlock", Value = "1", Selected = true };
                            //selectedType1.Add(selectedItemSTYPE);
                            ViewBag.GBSTYPE = selectedType1;
                        }
                    }
                    else
                    {
                        List<SelectListItem> selectedType1 = new List<SelectListItem>(); //If SCFS Block They can change
                        if (Convert.ToInt16(tab.GBSTYPE) == 0)
                        {
                            SelectListItem selectedItemSTYPE = new SelectListItem { Text = "Block", Value = "0", Selected = true };
                            selectedType1.Add(selectedItemSTYPE);
                            selectedItemSTYPE = new SelectListItem { Text = "UnBlock", Value = "1", Selected = false };
                            selectedType1.Add(selectedItemSTYPE);
                            ViewBag.GBSTYPE = selectedType1;
                        }
                        else
                        {
                            SelectListItem selectedItemSTYPE = new SelectListItem { Text = "Block", Value = "0", Selected = false };
                            selectedType1.Add(selectedItemSTYPE);
                            selectedItemSTYPE = new SelectListItem { Text = "UnBlock", Value = "1", Selected = true };
                            selectedType1.Add(selectedItemSTYPE);
                            ViewBag.GBSTYPE = selectedType1;
                        }
                    }

                }
            }
            return View(tab);
        }
        #endregion

        #region CONTNRNO Autocomplete
        public JsonResult AutoContainer(string term)
        {
            var result = (from view in context.VW_Gatein_Block_Contnrno_Assgn
                          where view.OSGID == 0 && view.CONTNRNO.ToLower().Contains(term.ToLower())
                          select new { view.CONTNRNO, view.GIDID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult AutoContainer(string term)
        //{
        //    //var result = (from r in dbo.VW_GATEIN_BLOCK_CONTNRNO_ASSGN  
        //    //              where r.CONTNRNO.StartsWith(term)
        //    //              select new { r.CONTNRNO, r.GIDID, r.CONTNRSID, r.GPLNO, r.VOYNO, r.IGMNO, r.VSLNAME, r.IMPRTNAME, r.STMRNAME, r.GIDATE }).Distinct();
        //    //return Json(result, JsonRequestBehavior.AllowGet);
                    

        //    var result = (from r in dbo.VW_GATEIN_BLOCK_CONTNRNO_ASSGN
        //                  where r.CONTNRNO.ToLower().Contains(term.ToLower())
        //                  select new { r.CONTNRNO, r.GIDID }).Distinct();
        //    return Json(result, JsonRequestBehavior.AllowGet);          
           
        //}
        #endregion

        #region Getdata
        public JsonResult GetDetails(string id)
        {
            
            if (id != "" || id != null || id != "0")
            {
                var sqr = context.Database.SqlQuery<VW_Gatein_Block_Contnrno_Assgn>("select *from VW_GATEIN_BLOCK_CONTNRNO_ASSGN where GIDID=" + Convert.ToInt32(id)).ToList();
                if (sqr.Count > 0)
                {
                    var result = sqr.ToList();
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = "";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }                
            }
            else
            {
                var result = "";
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //var result = (from r in dbo.VW_GATEIN_BLOCK_CONTNRNO_ASSGN
            //              where r.CONTNRNO.StartsWith(term)
            //              select new { r.CONTNRNO, r.GIDID, r.CONTNRSID, r.GPLNO, r.VOYNO, r.IGMNO, r.VSLNAME, r.IMPRTNAME, r.STMRNAME, r.GIDATE }).Distinct();
            //return Json(result, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Savedata
        public void savedata(GateInBlockDetails tab)
        {
            
            if (Session["CUSRID"] != null) tab.LMUSRID = Session["CUSRID"].ToString(); else tab.LMUSRID = "";
            //tab.LMUSRID = Convert.ToString(Session["CUSRID"]);
            tab.COMPYID = Convert.ToInt32(Session["compyid"]);
            tab.SDPTID = 1;
            tab.DISPSTATUS = 0;
            //tab.GBDATE = Convert.ToDateTime(tab.GBDATE).Date;

            string todaydt = Convert.ToString(DateTime.Now);
            string todayd = Convert.ToString(DateTime.Now.Date);

            string indate = Convert.ToString(tab.GBDATE);
            if (indate != null || indate != "")
            {
                tab.GBDATE = Convert.ToDateTime(indate).Date;
            }
            else { tab.GBDATE = DateTime.Now.Date; }

            if (tab.GBDATE > Convert.ToDateTime(todayd))
            {
                tab.GBDATE = Convert.ToDateTime(todayd);
            }

            string intime = Convert.ToString(tab.GBTIME);
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

                        tab.GBTIME = Convert.ToDateTime(in_datetime);
                    }
                    else { tab.GBTIME = DateTime.Now; }
                }
                else { tab.GBTIME = DateTime.Now; }
            }
            else { tab.GBTIME = DateTime.Now; }

            if (tab.GBTIME > Convert.ToDateTime(todaydt))
            {
                tab.GBTIME = Convert.ToDateTime(todaydt);
            }

            string Type = Convert.ToString(tab.GBSTYPE);

            if (Type == "1")
            {
                string ubdate = Convert.ToString(tab.GBUDATE);
                if (ubdate != null || ubdate != "")
                {
                    tab.GBUDATE = Convert.ToDateTime(ubdate).Date;
                }
                else { tab.GBUDATE = null; }

                string ubtime = Convert.ToString(tab.GBUTIME);
                if ((ubtime != null || ubtime != "") && ((ubdate != null || ubdate != "")))
                {
                    if ((ubtime.Contains(' ')) && (ubdate.Contains(' ')))
                    {
                        var ub_time = ubtime.Split(' ');
                        var ub_date = ubdate.Split(' ');

                        if ((ub_time[1].Contains(':')) && (ub_date[0].Contains('/')))
                        {

                            var ub_time1 = ub_time[1].Split(':');
                            var ub_date1 = ub_date[0].Split('/');

                            string ub_datetime = ub_date1[2] + "-" + ub_date1[1] + "-" + ub_date1[0] + "  " + ub_time1[0] + ":" + ub_time1[1] + ":" + ub_time1[2];

                            tab.GBUTIME = Convert.ToDateTime(ub_datetime);
                        }
                        else { tab.GBUTIME = null; }
                    }
                    else { tab.GBUTIME = null; }
                }
                else { tab.GBUTIME = null; }
            }
            else
            {
                tab.GBUDATE = null; tab.GBUTIME = null;
            }
            //  ViewBag.DATE = Request.Form.Get("GBDATE");
            //  tab.GBTIME = Convert.ToDateTime();
            tab.PRCSDATE = DateTime.Now;
            if ((tab.GBDID).ToString() != "0")
            {
               
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {              
                if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
                tab.GBNO = Convert.ToInt32(Autonumber.autonum("gatein_block_details", "GBNO", "GBNO<>0").ToString());
                int ano = tab.GBNO;
                string prfx = string.Format("{0:D5}", ano);
                tab.GBDNO = prfx.ToString();
                context.gateinblockdetails.Add(tab);
                context.SaveChanges();
            }
            Response.Redirect("Index");
        }
        #endregion

        #region Delete
        [Authorize(Roles = "GateInBlockDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                GateInBlockDetails gateinblockdetails = context.gateinblockdetails.Find(Convert.ToInt32(id));
                context.gateinblockdetails.Remove(gateinblockdetails);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion
    }
}