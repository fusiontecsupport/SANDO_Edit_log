
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

namespace scfs_erp.Controllers
{
    public class CategoryMasterController : Controller
    {
        //
        // GET: /CategoryMaster/
        SCFSERPContext context = new SCFSERPContext();

        [Authorize(Roles = "CategoryMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            var catetid = Request.Form.Get("CATETID");

            if (Request.Form.Get("CATETID") != null)
            {

                Session["CATETID"] = Convert.ToInt32(catetid);
            }
            else
            {
                Session["CATETID"] = 4;
            }
            ViewBag.CATETID = new SelectList(context.categorytypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATETDESC), "CATETID", "CATETDESC", Session["CATETID"]);

            return View();//Loading Grid
        }

        [Authorize(Roles = "CategoryMasterCreate")]
        public void Create()
        {
            Response.Redirect("Form/0");
        }
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_CategoryMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToInt32(Session["CATETID"]));

                var aaData = data.Select(d => new string[] { d.CATECODE, d.CATENAME, d.STATEDESC, d.HSNDESC, d.CATEGSTNO, d.CATEPANNO, d.CUSTGDESC, d.DISPSTATUS, d.CATEID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        

        [Authorize(Roles = "CategoryMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            //Response.Redirect("" + strPath + "/CategoryBasicDetails/Form/" + id);
            Response.Redirect("" + strPath + "/CategoryMaster/Form/" + id);

            //Response.Redirect("/CategoryBasicDetails/Form/" + id);
        }




        //------------------Initializing Form---------------------//
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            CategoryMaster tab = new CategoryMaster();
            tab.CATEID = 0; //var typeid = 0;

            //------------------------------Dropdown List -------------------------------------------//
            ViewBag.ACHEADID = new SelectList(context.accountheadmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ACHEADDESC), "ACHEADID", "ACHEADDESC");
            ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.STATEID = new SelectList(context.statemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.STATEDESC), "STATEID", "STATEDESC");
            ViewBag.HSNID = new SelectList(context.HSNCodeMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.HSNCODE), "HSNID", "HSNCODE");
            ViewBag.CATETID = new SelectList(context.categorytypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATETDESC), "CATETID", "CATETDESC");

            //--------------------------------DISPSTATUS---------
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            //--------------------------------CATESTRCALCTYPE---------
            List<SelectListItem> selectedCATESTRCALCTYPE = new List<SelectListItem>();
            SelectListItem selectedzItem = new SelectListItem { Text = "Max", Value = "1", Selected = false };
            selectedCATESTRCALCTYPE.Add(selectedzItem);
            selectedzItem = new SelectListItem { Text = "Min", Value = "0", Selected = true };
            selectedCATESTRCALCTYPE.Add(selectedzItem);
            ViewBag.CATESTRCALCTYPE = selectedCATESTRCALCTYPE;


            //--------------------------------CATEBTYPE------------
            List<SelectListItem> selectedCATEBTYPE = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "Customer", Value = "0", Selected = false };
            selectedCATEBTYPE.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "Notional", Value = "1", Selected = true };
            selectedCATEBTYPE.Add(selectedItem1);
            ViewBag.CATEBTYPE = selectedCATEBTYPE;
            //-----------------End

            ViewBag.primary = "";
            if (id != 0)
            {
                tab = context.categorymasters.Find(id);
                ViewBag.primary = id;
                //--------------------------Selected Values in Dropdown List-------------------------------//
                ViewBag.ACHEADID = new SelectList(context.accountheadmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ACHEADDESC), "ACHEADID", "ACHEADDESC", tab.ACHEADID);
                ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", tab.TARIFFMID);
                ViewBag.STATEID = new SelectList(context.statemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.STATEDESC), "STATEID", "STATEDESC", tab.STATEID);
                ViewBag.HSNID = new SelectList(context.HSNCodeMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.HSNCODE), "HSNID", "HSNCODE", tab.CATEPHNID);
                ViewBag.CATETID = new SelectList(context.categorytypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATETDESC), "CATETID", "CATETDESC", tab.CATETID);

                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem3 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem3);
                    selectedItem3 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem3);

                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
                else
                {
                    SelectListItem selectedItem3 = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem3);
                    selectedItem3 = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem3);

                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }

                List<SelectListItem> selectedCATEBTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.CATEBTYPE) == 0)
                {
                    SelectListItem selectedItem2 = new SelectListItem { Text = "Customer", Value = "0", Selected = true };
                    selectedCATEBTYPE1.Add(selectedItem2);
                    selectedItem2 = new SelectListItem { Text = "Notional", Value = "1", Selected = false };
                    selectedCATEBTYPE1.Add(selectedItem2);

                    ViewBag.CATEBTYPE = selectedCATEBTYPE1;
                }
                else
                {
                    SelectListItem selectedItem2 = new SelectListItem { Text = "Customer", Value = "0", Selected = false };
                    selectedCATEBTYPE1.Add(selectedItem2);
                    selectedItem2 = new SelectListItem { Text = "Notional", Value = "1", Selected = true };
                    selectedCATEBTYPE1.Add(selectedItem2);

                    ViewBag.CATEBTYPE = selectedCATEBTYPE1;
                }
                List<SelectListItem> selectedSTRCALCTYP1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.CATESTRCALCTYPE) == 0)
                {
                    SelectListItem selectedItem2 = new SelectListItem { Text = "Min", Value = "0", Selected = true };
                    selectedSTRCALCTYP1.Add(selectedItem2);
                    selectedItem2 = new SelectListItem { Text = "Max", Value = "1", Selected = false };
                    selectedSTRCALCTYP1.Add(selectedItem2);

                    ViewBag.CATESTRCALCTYPE = selectedSTRCALCTYP1;
                }
                else
                {
                    SelectListItem selectedItem2 = new SelectListItem { Text = "Min", Value = "0", Selected = false };
                    selectedSTRCALCTYP1.Add(selectedItem2);
                    selectedItem2 = new SelectListItem { Text = "Max", Value = "1", Selected = true };
                    selectedSTRCALCTYP1.Add(selectedItem2);

                    ViewBag.CATESTRCALCTYPE = selectedSTRCALCTYP1;
                }
            }
            return View(tab);
        }//End of Form

        //-----------------Insert or Modify data------------------//
        public void savedata(CategoryMaster tab)
        {
            if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
            tab.LMUSRID = 1;
            tab.PRCSDATE = DateTime.Now;
            if ((tab.CATEID).ToString() != "0")
            {
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                context.categorymasters.Add(tab);
                context.SaveChanges();
            }
            Response.Redirect("Index");
        }//End of savedata
         //-------------------------Delete Record-------//
        [Authorize(Roles = "CategoryMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                CategoryMaster categorymasters = context.categorymasters.Find(Convert.ToInt32(id));
                context.categorymasters.Remove(categorymasters);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//End of Delete

    }//End of class
}//End of namespace