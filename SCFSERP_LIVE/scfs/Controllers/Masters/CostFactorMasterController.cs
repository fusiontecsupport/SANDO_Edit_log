
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
    public class CostFactorMasterController : Controller
    {
        // GET: CostFactorMaster

        SCFSERPContext context = new SCFSERPContext();
        [Authorize(Roles = "CostFactorMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(context.costfactormasters.ToList());//Loading Grid
        }
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_SearchCostFactorMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.CFDESC, d.CFEXPR.ToString(), d.DISPSTATUS, d.CFID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "CostFactorMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/CostFactorMaster/Form/" + id);

            //Response.Redirect("/CostFactorMaster/Form/" + id);
        }
        [Authorize(Roles = "CostFactorMasterCreate")]
        //-------------Initializing Form-------------//
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            CostFactorMaster tab = new CostFactorMaster();
            tab.CFID = 0;
            //------------------------Dropdown List------------------------------------------------//
            ViewBag.DORDRID = new SelectList(context.displayordermasters.Where(x => x.DISPSTATUS == 0), "DORDRID", "DORDRDESC");
            ViewBag.ACHEADID = new SelectList(context.accountheadmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ACHEADDESC), "ACHEADID", "ACHEADDESC");
            ViewBag.CFOPTN = new SelectList(context.taxtypemaster, "CFOPTN", "CFOPTNDESC");

            //-----------------------------DISPSTATUS -----------------------------//
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            //-----------------------------CFMODE -----------------------------//
            List<SelectListItem> selectedCFMODE = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "ADD", Value = "0", Selected = false };
            selectedCFMODE.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "DEDUCT", Value = "1", Selected = false };
            selectedCFMODE.Add(selectedItem1);
            ViewBag.CFMODE = selectedCFMODE;

            //-------------/-----------------CFTYPE --------------------------------//
            List<SelectListItem> selectedCFTYPE = new List<SelectListItem>();
            SelectListItem selectedItem2 = new SelectListItem { Text = "VALUE", Value = "0", Selected = false };
            selectedCFTYPE.Add(selectedItem2);
            selectedItem2 = new SelectListItem { Text = "%", Value = "1", Selected = true };
            selectedCFTYPE.Add(selectedItem2);
            ViewBag.CFTYPE = selectedCFTYPE;
            //--------------------------------CFNATR--------------------------------------//

            List<SelectListItem> selectedCFNATR = new List<SelectListItem>();
            SelectListItem selectedItem3 = new SelectListItem { Text = "INCLUSIVE", Value = "0", Selected = false };
            selectedCFNATR.Add(selectedItem3);
            selectedItem3 = new SelectListItem { Text = "EXCLUSIVE", Value = "1", Selected = true };
            selectedCFNATR.Add(selectedItem3);
            ViewBag.CFNATR = selectedCFNATR;
            if (id != 0)//Edit Mode
            {
                tab = context.costfactormasters.Find(id);
                //------------------------------Selected values in Dropdown List--------------------------------//
                ViewBag.DORDRID = new SelectList(context.displayordermasters.Where(x => x.DISPSTATUS == 0), "DORDRID", "DORDRDESC", tab.DORDRID);
                ViewBag.ACHEADID = new SelectList(context.accountheadmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ACHEADDESC), "ACHEADID", "ACHEADDESC", tab.ACHEADID);
                ViewBag.CFOPTN = new SelectList(context.taxtypemaster, "CFOPTN", "CFOPTNDESC", tab.CFOPTN);

                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }

                //---------------------------------------------------CFMODE DROPDOWN
                List<SelectListItem> selectedCFMODE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.CFMODE) == 0)
                {
                    SelectListItem selectedItem11 = new SelectListItem { Text = "ADD", Value = "0", Selected = true };
                    selectedCFMODE1.Add(selectedItem11);
                    selectedItem11 = new SelectListItem { Text = "DEDUCT", Value = "1", Selected = false };
                    selectedCFMODE1.Add(selectedItem11);
                    ViewBag.CFMODE = selectedCFMODE1;
                }

                //----------------------------------------------------CFTYPE DROPDOWN
                List<SelectListItem> selectedCFTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.CFTYPE) == 0)
                {
                    SelectListItem selectedItem22 = new SelectListItem { Text = "VALUE", Value = "0", Selected = true };
                    selectedCFTYPE1.Add(selectedItem22);
                    selectedItem22 = new SelectListItem { Text = "%", Value = "1", Selected = false };
                    selectedCFTYPE1.Add(selectedItem22);
                    ViewBag.CFTYPE = selectedCFTYPE1;
                }
                //------------------------------------------------------CFNATR

                List<SelectListItem> selectedCFNATR1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.CFNATR) == 0)
                {
                    SelectListItem selectedItem32 = new SelectListItem { Text = "INCLUSIVE", Value = "0", Selected = true };
                    selectedCFNATR1.Add(selectedItem32);
                    selectedItem32 = new SelectListItem { Text = "EXCLUSIVE", Value = "1", Selected = false };
                    selectedCFNATR1.Add(selectedItem32);
                    ViewBag.CFNATR = selectedCFNATR1;
                }

            }
            return View(tab);
        }//End of Form
         //---------------------Insert or Modify data------------------//
         //public void savedata(CostFactorMaster tab)
         //{
         //    if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
         //    tab.LMUSRID = 1;
         //    tab.PRCSDATE = DateTime.Now;
         //    if ((tab.CFID).ToString() != "0")
         //    {
         //        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
         //        context.SaveChanges();
         //    }
         //    else
         //    {
         //        context.costfactormasters.Add(tab);
         //        context.SaveChanges();
         //    }
         //    Response.Redirect("Index");
         //}

        [HttpPost]
        public JsonResult savedata(CostFactorMaster tab)
        {
            if (tab.CUSRID == "" || tab.CUSRID == null)
            {
                if (Session["CUSRID"] != null)
                {
                    tab.CUSRID = Session["CUSRID"].ToString();
                }
                else { tab.CUSRID = "0"; }
            }
            tab.LMUSRID = 1;
            tab.PRCSDATE = DateTime.Now;
            string status = "";
            if ((tab.CFID).ToString() != "0")
            {

                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                status = "Update";
                return Json(status, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var query = context.costfactormasters.SqlQuery("SELECT *FROM COSTFACTORMASTER WHERE CFDESC='" + tab.CFDESC + "'").ToList<CostFactorMaster>();

                if (query.Count != 0)
                {
                    status = "Existing";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    context.costfactormasters.Add(tab);
                    context.SaveChanges();

                    status = "Success";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
            }
        }


        //End of savedata
        //------------------------Delete Record----------//
        [Authorize(Roles = "CostFactorMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                CostFactorMaster costfactormasters = context.costfactormasters.Find(Convert.ToInt32(id));
                context.costfactormasters.Remove(costfactormasters);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//End of Delete
    }
}