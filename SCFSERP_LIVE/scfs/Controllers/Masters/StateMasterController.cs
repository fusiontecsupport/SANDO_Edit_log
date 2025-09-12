
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
    public class StateMasterController : Controller
    {
        // GET: StateMaster
        SCFSERPContext context = new SCFSERPContext();
        [Authorize(Roles = "StateMasterIndex")]
        public ActionResult Index()
        {
            return View(context.statemasters.ToList());//Loading Grid
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                var data = e.pr_SearchStateMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.STATECODE, d.STATEDESC, d.STATETYPE, d.DISPSTATUS, d.STATEID.ToString() }).ToArray();
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

            Response.Redirect("" + strPath + "/StateMaster/Form/" + id);

            //Response.Redirect("/StateMaster/Form/" + id);
        }


        //----------------------Initializing Form--------------------------//
        [Authorize(Roles = "StateMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            StateMaster tab = new StateMaster();
            tab.STATEID = 0;

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            List<SelectListItem> selectedSTATETYPE = new List<SelectListItem>();
            SelectListItem selectedSType = new SelectListItem { Text = "InterState", Value = "1", Selected = false };
            selectedSTATETYPE.Add(selectedSType);
            selectedSType = new SelectListItem { Text = "Local", Value = "0", Selected = true };
            selectedSTATETYPE.Add(selectedSType);
            ViewBag.STATETYPE = selectedSTATETYPE;

            // IMP
            if (id == -1)
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";
            if (id != 0 && id != -1)  // IMP
            {
                tab = context.statemasters.Find(id);

                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }

                List<SelectListItem> selectedSTATETYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.STATETYPE) == 1)
                {
                    SelectListItem selectedSType31 = new SelectListItem { Text = "InterState", Value = "1", Selected = true };
                    selectedSTATETYPE1.Add(selectedSType31);
                    selectedSType31 = new SelectListItem { Text = "Local", Value = "0", Selected = false };
                    selectedSTATETYPE1.Add(selectedSType31);
                    ViewBag.STATETYPE = selectedSTATETYPE1;
                }

            }
            return View(tab);
        }
        //End of Form


        //--------------------------Insert or Modify data------------------------//
        [HttpPost]        
        public JsonResult savedata(StateMaster tab)
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
            var s = tab.STATEDESC;//...ProperCase
            s = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());//end
            tab.STATEDESC = s;
            string status = "";
            if ((tab.STATEID).ToString() != "0")
            {

                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                status = "Update";
                return Json(status, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var query = context.statemasters.SqlQuery("SELECT *FROM STATEMASTER WHERE STATEDESC='" + tab.STATEDESC + "' AND STATECODE='" + tab.STATECODE + "'").ToList<StateMaster>();


                var exitingdata = context.statemasters.SqlQuery("SELECT *FROM STATEMASTER WHERE STATEDESC=' " + tab.STATEDESC + "' AND STATECODE='" + tab.STATECODE + "'");

                if (query.Count != 0)
                {
                    status = "Existing";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    context.statemasters.Add(tab);
                    context.SaveChanges();

                    status = "Success";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
            }
        }

        //public void savedata(StateMaster tab)
        //{
        //    tab.CUSRID = Session["CUSRID"].ToString();
        //    tab.LMUSRID = 1;
        //    tab.PRCSDATE = DateTime.Now;
        //    var s = tab.STATEDESC;//...ProperCase
        //    s = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());//end
        //    tab.STATEDESC = s;
        //    if ((tab.STATEID).ToString() != "0")
        //    {
        //        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
        //        context.SaveChanges();
        //    }
        //    else
        //    {
        //        context.statemasters.Add(tab);
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


        [Authorize(Roles = "StateMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            //    String fld = Request.Form.Get("fld");
            //    String temp = Delete_fun.delete_check1(fld, id);
            //    if (temp.Equals("PROCEED"))
            //    {
            StateMaster statemasters = context.statemasters.Find(Convert.ToInt32(id));
            context.statemasters.Remove(statemasters);
            context.SaveChanges();
            Response.Write("Deleted Successfully ...");
        }
         //End of Delete
        
    }
}
