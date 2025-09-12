
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
    public class TariffGroupMasterController : Controller
    {
        // GET: TariffGroupMaster

        SCFSERPContext context = new SCFSERPContext();

        [Authorize(Roles = "TariffGroupMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(context.tariffgroupmasters.ToList());//---Loading Grid
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_TariffGroupMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.TGCODE, d.TGDESC, d.DISPSTATUS.ToString(), d.TGID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "TariffGroupMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/TariffGroupMaster/Form/" + id);

            //Response.Redirect("/TariffGroupMaster/Form/" + id);
        }

        //----------------Initializing Form-----------------------//
        [Authorize(Roles = "TariffGroupMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            TariffGroupMaster tab = new TariffGroupMaster();

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            tab.TGID = 0;
            ViewBag.SDPTID = new SelectList(context.softdepartmentmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SDPTNAME), "SDPTID", "SDPTNAME");
            if (id != 0)
            {
                tab = context.tariffgroupmasters.Find(id);

                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem3 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem3);
                    selectedItem3 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem3);

                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
                ViewBag.SDPTID = new SelectList(context.softdepartmentmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SDPTNAME), "SDPTID", "SDPTNAME", tab.SDPTID);
            }
            return View(tab);
        }//End of Form

        //-------------------Insert or Modify data-------------//
        //public void savedata(TariffGroupMaster tab)
        //{
        //    if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
        //    tab.LMUSRID = 1;
        //    tab.PRCSDATE = DateTime.Now;


        //    if ((tab.TGID).ToString() != "0")
        //    {
        //        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
        //        context.SaveChanges();
        //    }
        //    else
        //    {
        //        context.tariffgroupmasters.Add(tab);
        //        context.SaveChanges();

        //    }
        //    Response.Redirect("Index");
        //}

        [HttpPost]
        public JsonResult savedata(TariffGroupMaster tab)
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

            if ((tab.TGID).ToString() != "0")
            {

                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                status = "Update";
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = context.tariffgroupmasters.SqlQuery("SELECT *FROM TARIFFGROUPMASTER WHERE TGDESC='" + tab.TGDESC + "' AND TGCODE='" + tab.TGCODE + "'").ToList<TariffGroupMaster>();

                if (query.Count != 0)
                {
                    status = "Existing";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    context.tariffgroupmasters.Add(tab);
                    context.SaveChanges();

                    status = "Success";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
            }
            //Response.Redirect("Index");
        }

        //--End of Savedata

        //---------------Delete Record-------------//
        [Authorize(Roles = "TariffGroupMasterDelete")]
        public void Del()
        {
            using (SCFSERPContext dataContext = new SCFSERPContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        String id = Request.Form.Get("id");
                        String fld = Request.Form.Get("fld");
                        String temp = Delete_fun.delete_check1(fld, id);
                        if (temp.Equals("PROCEED"))
                        {
                            TariffGroupMaster TariffGroupMasters = context.tariffgroupmasters.Find(Convert.ToInt32(id));
                            context.tariffgroupmasters.Remove(TariffGroupMasters);
                            context.SaveChanges();
                            Response.Write("Deleted Successfully ...");

                        }
                        else
                            Response.Write(temp); trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                        trans.Rollback();
                        Response.Write("Sorry!! An Error Occurred.... ");
                    }
                }
            }
        }//--End of Delete
    }
}