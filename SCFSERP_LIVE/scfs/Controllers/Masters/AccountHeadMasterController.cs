using scfs_erp;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using scfs.Data;
using System.Configuration;

namespace scfs_erp.Controllers
{
    public class AccountHeadMasterController : Controller
    {
        SCFSERPContext context = new SCFSERPContext();
        //
        // GET: /AccountHeadMaster/
        [Authorize(Roles = "AccountHeadMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(context.accountheadmasters.ToList());//Loading Grid
        }
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_AccountHead(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.ACHEADCODE, d.ACHEADDESC, d.DISPSTATUS, d.ACHEADID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "AccountHeadMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/AccountHeadMaster/Form/" + id);

            //Response.Redirect("/AccountHeadMaster/Form/" + id);
        }
        //-------------Initializing Form-------------//
        [Authorize(Roles = "AccountHeadMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            AccountHeadMaster tab = new AccountHeadMaster();
            ViewBag.ACHEADGID = new SelectList(context.accountgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ACHEADGDESC), "ACHEADGID", "ACHEADGDESC");
            ViewBag.HSNID = new SelectList(context.HSNCodeMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.HSNDESC), "HSNID", "HSNDESC", 1);

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            tab.ACHEADID = 0;
            if (id != 0)
            {
                tab = context.accountheadmasters.Find(id);
                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();

                ViewBag.ACHEADGID = new SelectList(context.accountgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ACHEADGDESC), "ACHEADGID", "ACHEADGDESC", tab.ACHEADGID);
                ViewBag.HSNID = new SelectList(context.HSNCodeMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.HSNDESC), "HSNID", "HSNDESC", tab.HSNID);

                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem1 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem1);
                    selectedItem1 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem1);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }

            }
            return View(tab);
        }//--End of Form

        //-----------------Imsert or Modify data------------------//
        public void savedata(AccountHeadMaster tab)
        {
            if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
            tab.LMUSRID = 1;
            tab.PRCSDATE = DateTime.Now;
            if ((tab.ACHEADID).ToString() != "0")
            {
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                context.accountheadmasters.Add(tab);
                context.SaveChanges();
            }
            Response.Redirect("Index");
        }//---------End

        //-----------------------------Delete Record---//
        [Authorize(Roles = "AccountHeadMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                AccountHeadMaster accountheadmasters = context.accountheadmasters.Find(Convert.ToInt32(id));
                context.accountheadmasters.Remove(accountheadmasters);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//--End of Delete
    }//--End of class
}//--End of namespace
