
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
    public class CategoryBasicDetailsController : Controller
    {
        // GET: CategoryBasicDetails

        private readonly SCFSERPContext context = new SCFSERPContext();
        public ActionResult Index()
        {
            return View();
        }

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
            ViewBag.CUSTGID = new SelectList(context.customergroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CUSTGID), "CUSTGID", "CUSTGDESC");

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
            SelectListItem selectedItem1 = new SelectListItem { Text = "Customer", Value = "0", Selected = true };
            selectedCATEBTYPE.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "Notional", Value = "1", Selected = false };
            selectedCATEBTYPE.Add(selectedItem1);
            ViewBag.CATEBTYPE = selectedCATEBTYPE;
            //-----------------End

            //--------------------------------CATEBTYPE------------
            List<SelectListItem> selectedCATEBillTYPE = new List<SelectListItem>();
            SelectListItem sItem1 = new SelectListItem { Text = "General", Value = "1", Selected = true };
            selectedCATEBillTYPE.Add(sItem1);
            sItem1 = new SelectListItem { Text = "CHA Wise", Value = "2", Selected = false };
            selectedCATEBillTYPE.Add(sItem1);
            ViewBag.CATEBILLTYPE = selectedCATEBillTYPE;
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
                ViewBag.HSNID = new SelectList(context.HSNCodeMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.HSNCODE), "HSNID", "HSNCODE", tab.HSNID);
                ViewBag.CATETID = new SelectList(context.categorytypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATETDESC), "CATETID", "CATETDESC", tab.CATETID);
                ViewBag.CUSTGID = new SelectList(context.customergroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CUSTGID), "CUSTGID", "CUSTGDESC", tab.CUSTGID);

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

                List<SelectListItem> selectedSCBillType = new List<SelectListItem>();
                SelectListItem sbItem2 = new SelectListItem();
                if (Convert.ToInt32(tab.CATEBILLTYPE) == 1)
                {
                    sbItem2 = new SelectListItem { Text = "General", Value = "1", Selected = true };
                    selectedSCBillType.Add(sbItem2);
                    sbItem2 = new SelectListItem { Text = "CHA Wise", Value = "2", Selected = false };
                    selectedSCBillType.Add(sbItem2);
                }
                else
                {
                    sbItem2 = new SelectListItem { Text = "General", Value = "1", Selected = false };
                    selectedSCBillType.Add(sbItem2);
                    sbItem2 = new SelectListItem { Text = "CHA Wise", Value = "2", Selected = true };
                    selectedSCBillType.Add(sbItem2);                   
                }
                ViewBag.CATEBILLTYPE = selectedSCBillType;
            }
            return View(tab);
        }//End of Form


        //-----------------Insert or Modify data------------------//
        //public void savedata(CategoryMaster tab)
        //{
        //    if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
        //    tab.LMUSRID = 1;
        //    tab.PRCSDATE = DateTime.Now;
        //    if ((tab.CATEID).ToString() != "0")
        //    {
        //        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
        //        context.SaveChanges();
        //    }
        //    else
        //    {
        //        context.categorymasters.Add(tab);
        //        context.SaveChanges();
        //    }
        //    Response.Redirect("Index");
        //}

        //End of savedata

        //-------------------------Delete Record-------//

        [HttpPost]
        public ActionResult savedata(CategoryMaster tab)
        {
            string status = "";
            using (SCFSERPContext dataContext = new SCFSERPContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
                {

                    try
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

                        if ((tab.CATEID).ToString() != "0" || tab.CATEID != 0)
                        {

                            context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            status = "Update";

                        }
                        else
                        {
                            var query = context.categorymasters.SqlQuery("SELECT *FROM CATEGORYMASTER  WHERE CATENAME='" + tab.CATENAME + "' AND CATECODE='" + tab.CATECODE + "' AND CATETID = " + tab.CATETID).ToList<CategoryMaster>();

                            if (query.Count != 0)
                            {
                                status = "Existing";
                                //return Json(status, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                context.categorymasters.Add(tab);
                                context.SaveChanges();

                                status = "Success";
                                //return Json(status, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        status = "Error";
                        string Message = ex.Message.ToString();
                        //return Json(status, Message, JsonRequestBehavior.AllowGet);
                        //Response.Write("Sorry!! An Error Occurred.... ");
                    }
                }
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult savedata(CategoryMaster tab)
        //{
        //    string status = "";
        //    using (SCFSERPContext dataContext = new SCFSERPContext())
        //    {
        //        using (var trans = dataContext.Database.BeginTransaction())
        //        {

        //            try
        //            {

        //                if (tab.CUSRID == "" || tab.CUSRID == null)
        //                {
        //                    if (Session["CUSRID"] != null)
        //                    {
        //                        tab.CUSRID = Session["CUSRID"].ToString();
        //                    }
        //                    else { tab.CUSRID = "0"; }
        //                }
        //                tab.LMUSRID = 1;
        //                tab.PRCSDATE = DateTime.Now;

        //                if ((tab.CATEID).ToString() != "0" || tab.CATEID != 0)
        //                {

        //                    context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
        //                    context.SaveChanges();
        //                    status = "Update";                          

        //                }
        //                else
        //                {
        //                    var query = context.categorymasters.SqlQuery("SELECT *FROM CATEGORYMASTER  WHERE CATENAME='" + tab.CATENAME + "' AND CATECODE='" + tab.CATECODE + "' AND CATETID = " + tab.CATETID).ToList<CategoryMaster>();

        //                    if (query.Count != 0)
        //                    {
        //                        status = "Existing";
        //                        //return Json(status, JsonRequestBehavior.AllowGet);
        //                    }
        //                    else
        //                    {
        //                        context.categorymasters.Add(tab);
        //                        context.SaveChanges();

        //                        status = "Success";
        //                        //return Json(status, JsonRequestBehavior.AllowGet);
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                trans.Rollback();

        //                status = "Error";
        //                string Message = ex.Message.ToString();
        //                //return Json(status, Message, JsonRequestBehavior.AllowGet);
        //                //Response.Write("Sorry!! An Error Occurred.... ");
        //            }
        //        }
        //    }
        //    return Json(status, JsonRequestBehavior.AllowGet);
        //}
    }
}