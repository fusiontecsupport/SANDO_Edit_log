using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using scfs.Data;
using System.Configuration;

namespace scfs_erp.Controllers
{
    public class AccountGroupMasterController : Controller
    {
        SCFSERPContext context = new SCFSERPContext();
        [Authorize(Roles = "AccountGroupMasterIndex")]
        public ActionResult Index()
        {
            return View(context.accountgroupmasters.ToList());//---Loading Grid
        }
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_AccountGroup(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.ACHEADGCODE, d.ACHEADGDESC, d.DISPSTATUS.ToString(), d.ACHEADGID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "AccountGroupMasterEdit")]
        public void Edit(int id)
        {

            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/AccountGroupMaster/Form/" + id);
                        
            //Response.Redirect("/AccountGroupMaster/Form/" + id);
        }
        //----------------Initializing Form-----------------------//
        [Authorize(Roles = "AccountGroupMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            AccountGroupMaster tab = new AccountGroupMaster();

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            tab.ACHEADGID = 0;
            if (id != 0)
            {
                tab = context.accountgroupmasters.Find(id);

                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem3 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem3);
                    selectedItem3 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem3);

                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }

            }
            return View(tab);
        }//End of Form

        //-------------------Insert or Modify data-------------//
        public void savedata(AccountGroupMaster tab)
        {
            if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
            tab.LMUSRID = 1;
            tab.PRCSDATE = DateTime.Now;
            if ((tab.ACHEADGID).ToString() != "0")
            {
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                context.accountgroupmasters.Add(tab);
                context.SaveChanges();

            }
            Response.Redirect("Index");
        }//--End of Savedata


        //public void savedata(AccountGroupMaster AccountGroupMaster)
        //{
        //    using (SCFSERPContext dataContext = new SCFSERPContext())
        //    {
        //        using (var trans = dataContext.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                AccountGroupMaster.CUSRID = Session["CUSRID"].ToString();
        //                if ((AccountGroupMaster.ACHEADGID).ToString() != "0")
        //                {
        //                    dataContext.Entry(AccountGroupMaster).State = System.Data.Entity.EntityState.Modified;
        //                    dataContext.SaveChanges();
        //                }
        //                else
        //                {
        //                    dataContext.accountgroupmasters.Add(AccountGroupMaster);
        //                    dataContext.SaveChanges();
        //                }
        //                trans.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                trans.Rollback();
        //                Response.Write("Sorry!! An Error Occurred.... ");
        //            }
        //        }
        //    }

        //}


        //---------------Delete Record-------------//
        [Authorize(Roles = "AccountGroupMasterDelete")]
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
                            AccountGroupMaster accountgroupmasters = context.accountgroupmasters.Find(Convert.ToInt32(id));
                            context.accountgroupmasters.Remove(accountgroupmasters);
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
    }//---End of Class
}//---End of namespace

