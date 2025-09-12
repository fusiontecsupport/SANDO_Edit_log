
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
    public class HSNCodeMasterController : Controller
    {
        // GET: HSNCodeMaster
        SCFSERPContext context = new SCFSERPContext();
        [Authorize(Roles = "HSNCodeMasterIndex")]
        public ActionResult Index()
        {
            return View(context.HSNCodeMasters.ToList());//Loading Grid
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                var data = e.pr_SearchHSNCodeMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                //var aaData = data.Select(d => new string[] { d.HSNCODE, d.HSNDESC, d.TAXEXPRN.ToString(), d.CGSTEXPRN.ToString(), d.SGSTEXPRN.ToString(), d.IGSTEXPRN.ToString(), d.DISPSTATUS, d.HSNID.ToString() }).ToArray();
                var aaData = data.Select(d => new string[] { d.HSNCODE, d.HSNDESC, d.DISPSTATUS, d.HSNID.ToString() }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/HSNCodeMaster/Form/" + id);

            //Response.Redirect("/HSNCodeMaster/Form/" + id);
        }


        //-------------Initializing Form-------------//
        [Authorize(Roles = "HSNCodeMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            HSNCodeMaster tab = new HSNCodeMaster();
            tab.HSNID = 0;
            tab.PRCSDATE = DateTime.Now;
            //-----------------------------DISPSTATUS -----------------------------//
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            // IMP
            if (id == -1)
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";


            if (id != 0 && id != -1)  // IMP
            {
                tab = context.HSNCodeMasters.Find(id);
                //------------------------------Selected values in Dropdown List--------------------------------//
                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
            }
            return View(tab);
        }//End of Form

        //---------------------Insert or Modify data------------------//

        [HttpPost]
        public JsonResult savedata(HSNCodeMaster tab)
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
            if ((tab.HSNID).ToString() != "0")
            {

                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                status = "Update";
                return Json(status, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var query = context.HSNCodeMasters.SqlQuery("SELECT *FROM HSNCODEMASTER WHERE HSNDESC='" + tab.HSNDESC + "' AND HSNCODE='" + tab.HSNCODE + "'").ToList<HSNCodeMaster>();


                var exitingdata = context.HSNCodeMasters.SqlQuery("SELECT *FROM HSNCODEMASTER WHERE HSNDESC=' " + tab.HSNDESC + "' AND HSNCODE='" + tab.HSNCODE + "'");

                if (query.Count != 0)
                {
                    status = "Existing";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    context.HSNCodeMasters.Add(tab);
                    context.SaveChanges();

                    status = "Success";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
            }
        }


        //public void savedata(HSNCodeMaster tab)
        //{
        //    tab.CUSRID = Session["CUSRID"].ToString();
        //    tab.LMUSRID = 1;
        //    tab.PRCSDATE = DateTime.Now;

        //    var s = tab.HSNDESC;//...ProperCase
        //    //s = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());//end
        //    //tab.CFDESC = s;
        //    if ((tab.HSNID).ToString() != "0")
        //    {
        //        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
        //        context.SaveChanges();
        //    }
        //    else
        //    {
        //        context.HSNCodeMasters.Add(tab);
        //        context.SaveChanges();
        //    }

        //    // IMP
        //    if (Request.Form.Get("continue") == null)
        //    {
        //        Response.Redirect("index");
        //    }
        //    else
        //    {
        //        Response.Redirect("Form/-1");
        //    }

        //}
        //End of savedata

        //------------------------Delete Record----------//

        [Authorize(Roles = "HSNCodeMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                HSNCodeMaster HSNCodeMaster = context.HSNCodeMasters.Find(Convert.ToInt32(id));
                context.HSNCodeMasters.Remove(HSNCodeMaster);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//End of Delete
    }
}