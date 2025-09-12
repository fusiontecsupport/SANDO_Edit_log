using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers.Import
{
    public class ImportSealMasterController : Controller
    {
        // GET: ImportSealMaster
        SCFSERPContext context = new SCFSERPContext();

        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(context.importsealmasters.ToList());
        }

        //Get FORM        
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            Import_Seal_Master tab = new Import_Seal_Master();
            tab.SEALMID = 0;
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            if (id != 0)  //Edit Mode
            {
                tab = context.importsealmasters.Find(id);
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

//        [HttpPost]
//        public JsonResult savedata(Import_Seal_Master tab)
//        {
//            if (tab.CUSRID == "" || tab.CUSRID == null)
//            {
//                if (Session["CUSRID"] != null)
//                {
//                    tab.CUSRID = Session["CUSRID"].ToString();
//                }
//                else { tab.CUSRID = "0"; }
//            }
//            tab.LMUSRID = 1;
//            tab.PRCSDATE = DateTime.Now;

//            string status = "";
//            if ((tab.SEALMID).ToString() != "0")
//            {
//                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
//                context.SaveChanges();

//                status = "Update";
//                return Json(status, JsonRequestBehavior.AllowGet);
//            }
//            else
//            {

//                var query = context.importsealmasters.SqlQuery("SELECT *FROM IMPORT_SEAL_MASTER WHERE SEALMDESC='" + tab.SEALMDESC + "' AND SEALMDATE='" + tab.SEALMDATE + "'").ToList<Import_Seal_Master>();

//                if (query.Count != 0)
//                {
//                    status = "Existing";
//                    return Json(status, JsonRequestBehavior.AllowGet);
//                }
//                else
//                {
//                    context.importsealmasters.Add(tab);
//                    context.SaveChanges();

//                    status = "Success";
//                    return Json(status, JsonRequestBehavior.AllowGet);
//                }
//;
//            }

//        }

        ///-----------Insert or Modify----------------------//
        public void savedata(FormCollection myfrm)
        {

            if (myfrm.Get("SEALMID") == "0")
            {
                Import_Seal_Master importsealmasters = new Import_Seal_Master();

                var Sealmno = myfrm.GetValues("SEALMNO");
                var Sealmdate = myfrm.GetValues("SEALMDATE");

                for (int count = 0; count < Sealmno.Count(); count++)
                {
                    importsealmasters.SEALMNO = Convert.ToDecimal(Sealmno[count]);

                    importsealmasters.SEALMDATE = Convert.ToDateTime(Sealmdate[count]);

                    importsealmasters.SEALMDESC = Sealmno[count].ToString();
                    
                    importsealmasters.CUSRID = Session["CUSRID"].ToString();
                    importsealmasters.LMUSRID = Session["CUSRID"].ToString();
                    importsealmasters.DISPSTATUS = 0;
                    importsealmasters.PRCSDATE = DateTime.Now;

                    context.importsealmasters.Add(importsealmasters);
                    context.SaveChanges();
                }

            }

            Response.Redirect("Index");
        }//----End of savedata


        //------------------To Get List Of Seal_No--------------------//
        public void Seal_No(string FMVAL, string TOVAL, string DATE)
        {
            var P_FMVAL = Convert.ToInt32(TOVAL) - Convert.ToInt32(FMVAL);
            var P_DATE = DATE;
            var tabl = "<Table id=mytabl class='table table-striped table-bordered bootstrap-datatable'> <thead><tr> <td>Description</td><td>Date</td></tr> </thead>";

            int i = 0;
            do
            {

                tabl = tabl + "<tbody><tr><td><input type=text id=SEALMNO class=SEALMNO value=" + FMVAL + " name=SEALMNO></td><td><input type=text id=SEALMDATE value='" + P_DATE + "'  class=SEALMDATE name=SEALMDATE></td></tr></tbody>";
                FMVAL = (Convert.ToInt32(FMVAL) + 1).ToString();
                i++;

            } while (i <= P_FMVAL);
            tabl = tabl + "</Table>";
            Response.Write(tabl);
        }//----End of Get Seal


        //---------------Delete Row---------//
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String Date = OpenSheetDetails.Detail(id);
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED") && (Date != "Date Not Found"))
            {
                var sealid = context.Database.SqlQuery<Import_Seal_Master>("SELECT * from IMPORT_SEAL_MASTER where SEALMDATE='" + Date + "'").ToList();
                foreach (var result in sealid)
                {
                    var sealmid = result.SEALMID;
                    Import_Seal_Master importsealmasters = context.importsealmasters.Find(sealmid);
                    context.importsealmasters.Remove(importsealmasters);
                    context.SaveChanges();

                }
                Response.Write("Deleted successfully...");
            }
            else
            {
                Response.Write(temp);
            }

        }//-----End of Delete Row


    }
}