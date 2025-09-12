
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


namespace scfs_erp.Controllers.Masters
{
    public class ImportSlabNarrationController : Controller
    {
        // GET: ImportSlabNarration

        #region context declaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region Index
        [Authorize(Roles = "ImportSlabNarrationIndex")]
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
                Session["AASDPTTYPEID"] = 1;
                Session["TARIFFMID"] = 1;
                Session["SLABTID"] = 16;
                Session["CHRGETYPE"] = 1;
            }


            ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", Convert.ToInt32(Session["TARIFFMID"]));
            ViewBag.SLABTID = new SelectList(context.slabtypemasters.Where(x => x.DISPSTATUS == 0).Where(x => x.SLABTID == 16 || x.SLABTID == 4).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", Convert.ToInt32(Session["SLABTID"]));


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
                ViewBag.AASDPTTYPEID = selectedASDPTTYPE;
            }
            
            return View();//Loading Grid

        }
        #endregion

        #region GetAjaxData
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                //if (Session["FSTMRID"].ToString() == "0")
                //{
                var data = e.pr_Search_SlabNarrationMaster(param.sSearch,
                                              Convert.ToInt32(Request["iSortCol_0"]),
                                              Request["sSortDir_0"],
                                              param.iDisplayStart,
                                              param.iDisplayStart + param.iDisplayLength,
                                              totalRowsCount,
                                              filteredRowsCount, Convert.ToInt32(Session["TARIFFMID"]), Convert.ToInt16(Session["CHRGETYPE"]), Convert.ToInt32(Session["SLABTID"]));

                var aaData = data.Select(d => new string[] { d.PRCSDATE, d.TARIFF, d.SLABTYPE, d.CHRGETYPE, d.SLABNARTN, d.SLABNID.ToString() }).ToArray();

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

        #region Form 
        //[Authorize(Roles = "ImportSlabNarrationCreate")]
        public ActionResult Form(int? id = 0)
        {

            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            SlabNarrationMaster tab = new SlabNarrationMaster();
            tab.PRCSDATE = DateTime.Now.Date;
            tab.SLABNID = 0;
            //-------------------------Dropdown List------//
            
            ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");            
            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.CONTNRSID > 1 && x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");
            ViewBag.SLABTID = new SelectList(context.slabtypemasters.Where(x => x.DISPSTATUS == 0).Where(x => x.SLABTID == 16 || x.SLABTID == 4).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC");

          
            List<SelectListItem> selectedSDPTTYPE = new List<SelectListItem>();
            SelectListItem selectedItemSDPT0 = new SelectListItem { Text = "Import", Value = "1", Selected = true };
            selectedSDPTTYPE.Add(selectedItemSDPT0);
            ViewBag.SDPTTYPEID = selectedSDPTTYPE;


            List<SelectListItem> selectedCHRGETYPE = new List<SelectListItem>();
            SelectListItem selectedItem0 = new SelectListItem { Text = "LD", Value = "1", Selected = true };
            selectedCHRGETYPE.Add(selectedItem0);
            selectedItem0 = new SelectListItem { Text = "DS", Value = "2", Selected = false };
            selectedCHRGETYPE.Add(selectedItem0);
            ViewBag.CHRGETYPE = selectedCHRGETYPE;

            if (id != 0 && id > 0)
            {
               
                tab = context.slabNarrations.Find(id);

                ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.STMRID = new SelectList(context.tariffmasters.Where(p => p.TARIFFMID == 4 || p.TARIFFMID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID); ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);

                ViewBag.SLABTID = new SelectList(context.slabtypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLABTDESC), "SLABTID", "SLABTDESC", tab.SLABTID);
              
                List<SelectListItem> selectedCHRGETYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.BILLTID) == 1)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "LD", Value = "1", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "DS", Value = "2", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                else if (Convert.ToInt32(tab.BILLTID) == 2)
                {
                    SelectListItem selectedItem01 = new SelectListItem { Text = "LD", Value = "1", Selected = false };
                    selectedCHRGETYPE1.Add(selectedItem01);
                    selectedItem01 = new SelectListItem { Text = "DS", Value = "2", Selected = true };
                    selectedCHRGETYPE1.Add(selectedItem01);
                }
                ViewBag.CHRGETYPE = selectedCHRGETYPE1;
               
                //---------SDPTTYPE-----------------//
                List<SelectListItem> selectedSDPTTYPE1 = new List<SelectListItem>();
                SelectListItem selectedItem11 = new SelectListItem { Text = "Import", Value = "1", Selected = true };
                selectedSDPTTYPE1.Add(selectedItem11);


                ViewBag.SDPTTYPEID = selectedSDPTTYPE1;
                //End
            }
            return View(tab);
        }
        #endregion

        #region savedata
        public void savedata(FormCollection myfrm)
        {

            using (SCFSERPContext dataContext = new SCFSERPContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
                {
                    try
                    {
                     
                        SlabNarrationMaster SlabNarrationMaster = new SlabNarrationMaster();
                        Int32 SLABNID = Convert.ToInt32(myfrm["SLABNID"]);
                       
                        SlabNarrationMaster.BILLTID = Convert.ToInt16(myfrm["CHRGETYPE"]);                 
                        SlabNarrationMaster.PRCSDATE = DateTime.Now;
                        SlabNarrationMaster.SLABTID = Convert.ToInt32(myfrm["SLABTID"]);
                        SlabNarrationMaster.TARIFFMID = Convert.ToInt32(myfrm["TARIFFMID"]);
                        SlabNarrationMaster.SLABNARTN= Convert.ToString(myfrm["SLABNARTN"]);


                        if (SLABNID != 0)
                        {
                            context.Entry(SlabNarrationMaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                         
                        }
                        else
                        {
                            context.slabNarrations.Add(SlabNarrationMaster);
                            context.SaveChanges();
                        }
                                                

                        trans.Commit();

                        Response.Redirect("Index");
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

        #endregion

        #region delete
        //[Authorize(Roles = "ImportSlabNarrationDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = "";
            if (Convert.ToInt32(id) > 0)
            {
                temp = "PROCEED";
            }
            //String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                SlabNarrationMaster slabNarrationMasters = context.slabNarrations.Find(Convert.ToInt32(id));

                context.Database.ExecuteSqlCommand("delete from SLABNARRATIONMASTER where SLABNID=" + Convert.ToInt32(id) + "");
                //context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//End of Delete

        #endregion

    }
}