
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
    public class CategoryAddressDetailsController : Controller
    {
        // GET: CategoryAddressDetails

        private readonly SCFSERPContext context = new SCFSERPContext();
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Form(int? id = 0)
        {
            Category_Address_Details tab = new Category_Address_Details();
            CategoryList vm = new CategoryList();

            ViewBag.CATEID = id;

            ViewBag.STATEID = new SelectList(context.statemasters.Where(x => x.DISPSTATUS == 0), "STATEID", "STATEDESC");

            tab.CATEAID = 0;
            if (id != 0)
            {
               
                if (ViewBag.CATEID != 0)
                {
                    tab = context.categoryaddressdetails.FirstOrDefault(c => c.CATEID == id);
                    //tab = context.categoryaddressdetails.Find(id);                    
                }
                if (tab != null)
                {
                    ViewBag.STATEID = new SelectList(context.statemasters.Where(x => x.DISPSTATUS == 0), "STATEID", "STATEDESC", tab.STATEID);
                    
                    vm.CategoryAddressDetails = context.categoryaddressdetails.Where(det => det.CATEID == id).OrderBy(det => det.CATEAID).ToList();
                    vm.CategoryMaster = context.categorymasters.Find(id);
                }
            }
            return View(vm);
        }

        [HttpPost]
        public void savedata(FormCollection F_Form)
        {
            Category_Address_Details tab = new Category_Address_Details();
            Int32 CATEID = Convert.ToInt32(F_Form["CATEID"]);
            Int32 CATEAID = 0;
            string DELIDS = "";
            CategoryMaster tab1 = new CategoryMaster();
            tab1 = context.categorymasters.Find(CATEID);

            string[] A_PID = F_Form.GetValues("CATEAID");
            tab.CATEID = Convert.ToInt32(F_Form["CATEID"]);
            string[] STATEID = F_Form.GetValues("CategoryAddressDetails[0].STATEID");
            //string[] STATEID = F_Form.GetValues("STATEID");
            string[] CATEAGSTNO = F_Form.GetValues("CATEAGSTNO");
            string[] CATEAADDR1 = F_Form.GetValues("CATEAADDR1");
            string[] CATEAADDR2 = F_Form.GetValues("CATEAADDR2");
            string[] CATEAADDR3 = F_Form.GetValues("CATEAADDR3");
            string[] CATEAADDR4 = F_Form.GetValues("CATEAADDR4");
            string[] CATEATYPEDESC = F_Form.GetValues("CATEATYPEDESC");
            string[] CATEAPANNO = F_Form.GetValues("CATEAPANNO");

            for (int i = 0; i < A_PID.Count(); i++)
            {
                CATEAID = Convert.ToInt32(A_PID[i]);
                if (CATEAID != 0)
                {
                    tab = context.categoryaddressdetails.Find(CATEAID);
                }

                tab.CATEID = Convert.ToInt32(F_Form["CATEID"]);


                tab.STATEID = Convert.ToInt32(STATEID[i]);
                tab.CATEAGSTNO = CATEAGSTNO[i].ToString();
                tab.CATEAADDR1 = CATEAADDR1[i].ToString();
                tab.CATEAADDR2 = CATEAADDR2[i].ToString();
                tab.CATEAADDR3 = CATEAADDR3[i].ToString();
                tab.CATEAADDR4 = CATEAADDR4[i].ToString();
                tab.CATEATYPEDESC = CATEATYPEDESC[i].ToString();
                tab.CATEAPANNO = CATEAPANNO[i].ToString();

                if (CATEAID == 0)
                {
                    context.categoryaddressdetails.Add(tab);
                    context.SaveChanges();
                    CATEAID = tab.CATEAID;
                }
                else
                {
                    context.Entry(tab).State = System.Data.Entity.EntityState.Modified; ;
                    context.SaveChanges();
                }

                DELIDS = DELIDS + "," + CATEAID.ToString();
            }
            context.Database.ExecuteSqlCommand("DELETE FROM CATEGORY_ADDRESS_DETAIL  WHERE CATEID=" + CATEID + " and  CATEAID NOT IN(" + DELIDS.Substring(1) + ")");
            context.Database.ExecuteSqlCommand("exec pr_category_gst_state_upd  @cateid=" + CATEID);
            
            //RedirectToAction("Index", "CategoryMaster");

            Response.Write("saved");
            //string data = "saved";

            //return Json(data, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult savedata(FormCollection F_Form)
        //{
        //    Category_Address_Details tab = new Category_Address_Details();
        //    Int32 CATEID = Convert.ToInt32(F_Form["CATEID"]);
        //    Int32 CATEAID = 0;
        //    string DELIDS = "";
        //    CategoryMaster tab1 = new CategoryMaster();
        //    tab1 = context.categorymasters.Find(CATEID);
        //    string status = "";
        //    string[] A_PID = F_Form.GetValues("CATEAID");
        //    tab.CATEID = Convert.ToInt32(F_Form["CATEID"]);
        //    string[] STATEID = F_Form.GetValues("STATEID");
        //    string[] CATEAGSTNO = F_Form.GetValues("CATEAGSTNO");
        //    string[] CATEAADDR1 = F_Form.GetValues("CATEAADDR1");
        //    string[] CATEAADDR2 = F_Form.GetValues("CATEAADDR2");
        //    string[] CATEAADDR3 = F_Form.GetValues("CATEAADDR3");
        //    string[] CATEAADDR4 = F_Form.GetValues("CATEAADDR4");
        //    string[] CATEATYPEDESC = F_Form.GetValues("CATEATYPEDESC");

        //    for (int i = 0; i < A_PID.Count(); i++)
        //    {
        //        CATEAID = Convert.ToInt32(A_PID[i]);
        //        if (CATEAID != 0)
        //        {
        //            tab = context.categoryaddressdetails.Find(CATEAID);
        //        }

        //        tab.CATEID = Convert.ToInt32(F_Form["CATEID"]);


        //        tab.STATEID = Convert.ToInt32(STATEID[i]);
        //        tab.CATEAGSTNO = CATEAGSTNO[i].ToString();
        //        tab.CATEAADDR1 = CATEAADDR1[i].ToString();
        //        tab.CATEAADDR2 = CATEAADDR2[i].ToString();
        //        tab.CATEAADDR3 = CATEAADDR3[i].ToString();
        //        tab.CATEAADDR4 = CATEAADDR4[i].ToString();
        //        tab.CATEATYPEDESC = CATEATYPEDESC[i].ToString();

        //        if (CATEAID == 0)
        //        {
        //            context.categoryaddressdetails.Add(tab);
        //            context.SaveChanges();
        //            CATEAID = tab.CATEAID;

        //        }
        //        else
        //        {

        //            context.Entry(tab).State = System.Data.Entity.EntityState.Modified; ;
        //            context.SaveChanges();
        //        }



        //        DELIDS = DELIDS + "," + CATEAID.ToString();
        //    }
        //    context.Database.ExecuteSqlCommand("DELETE FROM CATEGORY_ADDRESS_DETAIL  WHERE CATEID=" + CATEID + " and  CATEAID NOT IN(" + DELIDS.Substring(1) + ")");

        //    status = "Success";
        //    return Json(status, JsonRequestBehavior.AllowGet);
        //}

    }
}