using scfs_erp.Context;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers
{
    [SessionExpire]
    [Authorize]
    public class OpenSheetSealDetailsController : Controller
    {
        //
        // GET: /OpenSheetSealDetails/
        SCFSERPContext context = new SCFSERPContext();

        //[Authorize(Roles = "OpenSheetSealDetailsIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(context.opensheetsealdetails.ToList());
        }


        // GET: /Form/
        //[Authorize(Roles = "OpenSheetSealDetailsForm")]
        public ActionResult Form(int? id)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            OpenSheetSealDetails tab = new OpenSheetSealDetails();

            tab.OSSDATE = DateTime.Now;
            //-------Getting OpensheetMaster Info----------
            OpenSheetMaster OSM_tab = context.opensheetmasters.Find(id);
            ViewBag.OSMNO = OSM_tab.OSMNO;
            ViewBag.OSMDATE = OSM_tab.OSMDATE;
            ViewBag.OSMTIME = OSM_tab.OSMTIME;
            ViewBag.OSMNAME = OSM_tab.OSMNAME;
            ViewBag.OSMLNAME = OSM_tab.OSMLNAME;
            ViewBag.OSMCNAME = OSM_tab.OSMCNAME;
            ViewBag.BOENO = OSM_tab.BOENO;
            ViewBag.BOEDATE = OSM_tab.BOEDATE;
            ViewBag.OSMLDTYPE = OSM_tab.OSMLDTYPE;
            //-------------------------Dropdown List---------------------------------//
            ViewBag.SEALMID = new SelectList(context.importsealmasters.Where(c => c.OSSDID == 0 && 1 == 0), "SEALMID", "SEALMDESC").Take(10);

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "0", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "1", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            //----------------------End of Dropdown List---------------


            //--------------------------Getting primary key value--------------------------------//
            tab.OSMID = OSM_tab.OSMID;
            var sql = context.Database.SqlQuery<OpenSheetSealDetails>("select * from OPENSHEET_SEAL_DETAIL where  OSMID='" + id + "' ").ToList();
            foreach (var result in sql)
            {
                if (tab.OSMID == result.OSMID)
                {
                    tab.OSSID = result.OSSID;
                }
            }

            //-----------------End-----------------

            // Response.Write(tab.OSSID);


            if (tab.OSSID != 0)//-----------Edit Mode
            {
                tab = context.opensheetsealdetails.Find(Convert.ToInt32(tab.OSSID));

                //ViewBag.SEALMID = new MultiSelectList(context.importsealmasters, "SEALMID", "SEALMDESC", context.opensheetsealdetails.Where(c => c.OSMID == id).Select(c => c.SEALMID).ToArray());
                ViewBag.SEALMID = new MultiSelectList(context.opensheetsealdetails, "SEALMID", "OSSDESC", new[] { context.opensheetsealdetails.Where(c => c.OSMID == id).Select(c => c.SEALMID).ToArray() } );


                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 0)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "0", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "1", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }

            }

            return View(tab);
        }//-----------End of Form

        //GET: NForm
        //[Authorize(Roles = "OpenSheetSealDetailsNForm")]
        public ActionResult NForm(int? id)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            OpenSheetSealDetails tab = new OpenSheetSealDetails();

            tab.OSSDATE = DateTime.Now;
            //-------Getting OpensheetMaster Info----------
            OpenSheetMaster OSM_tab = context.opensheetmasters.Find(id);
            ViewBag.OSMNO = OSM_tab.OSMNO;
            ViewBag.OSMDATE = OSM_tab.OSMDATE;
            ViewBag.OSMTIME = OSM_tab.OSMTIME;
            ViewBag.OSMNAME = OSM_tab.OSMNAME;
            ViewBag.OSMLNAME = OSM_tab.OSMLNAME;
            ViewBag.OSMCNAME = OSM_tab.OSMCNAME;
            ViewBag.BOENO = OSM_tab.BOENO;
            ViewBag.BOEDATE = OSM_tab.BOEDATE;
            ViewBag.OSMLDTYPE = OSM_tab.OSMLDTYPE;
            //-------------------------Dropdown List---------------------------------//
            ViewBag.SEALMID = new SelectList(context.importsealmasters.Where(c => c.OSSDID == 0 && 1 == 0), "SEALMID", "SEALMDESC").Take(10);

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "0", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "1", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            //----------------------End of Dropdown List---------------


            //--------------------------Getting primary key value--------------------------------//
            tab.OSMID = OSM_tab.OSMID;
            var sql = context.Database.SqlQuery<OpenSheetSealDetails>("select * from OPENSHEET_SEAL_DETAIL where  OSMID='" + id + "' ").ToList();
            foreach (var result in sql)
            {
                if (tab.OSMID == result.OSMID)
                {
                    tab.OSSID = result.OSSID;
                }
            }

            //-----------------End-----------------

            // Response.Write(tab.OSSID);


            if (tab.OSSID != 0)//-----------Edit Mode
            {
                tab = context.opensheetsealdetails.Find(Convert.ToInt32(tab.OSSID));

                //ViewBag.SEALMID = new MultiSelectList(context.importsealmasters, "SEALMID", "SEALMDESC", context.opensheetsealdetails.Where(c => c.OSMID == id).Select(c => c.SEALMID).ToArray());
                ViewBag.SEALMID = new MultiSelectList(context.opensheetsealdetails, "SEALMID", "OSSDESC", new[] { context.opensheetsealdetails.Where(c => c.OSMID == id).Select(c => c.SEALMID).ToArray() });


                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 0)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "0", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "1", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }

            }

            return View(tab);
        }//-----------End of Form
         //--------Get Seal NOs
        public JsonResult GetSealNos(string id)
        {
            var param = id.Split('~');
            var fsno = Convert.ToInt32(param[0]);
            var tsno = Convert.ToInt32(param[1]);
            var qry = "select *  from IMPORT_SEAL_MASTER where dispstatus =0 and   ossdid = 0 AND SEALMNO >=" + fsno + " AND SEALMNO <=" + tsno + " order by sealmid";
            var query = context.Database.SqlQuery<Import_Seal_Master>(qry).ToList();
            
            return Json(query, JsonRequestBehavior.AllowGet);

            
            
        }

        //--------------------------Insert or Modify-------------//
        public void savedata(OpenSheetSealDetails tab)
        {
            if (Session["CUSRID"] != null) tab.CUSRID = Session["CUSRID"].ToString(); else tab.CUSRID = "0";
            tab.LMUSRID = Session["CUSRID"].ToString();
            tab.PRCSDATE = DateTime.Now;
            var OSSID = Request.Form.Get("OSSID");
            var SEAL_VAL = Request.Form.Get("SEAL_VAL").Substring(1).Split(',');
            var OSSDESC = Request.Form.Get("OSSDESC").Substring(1).Split(',');
            var OSSDATE = Request.Form.Get("OSSDATE");
            var OSMID = Request.Form.Get("OSMID");

            if (OSSID == "0")//Insert data
            {
                for (int i = 1; i < SEAL_VAL.Count(); i++)
                {
                    tab.OSMID = Convert.ToInt32(OSMID);
                    tab.OSSDATE = Convert.ToDateTime(OSSDATE);
                    tab.SEALMID = Convert.ToInt32(SEAL_VAL[i]);
                    tab.OSSDESC = Convert.ToString(OSSDESC[i]);
                    context.opensheetsealdetails.Add(tab);
                    context.SaveChanges();
                }
            }//--End of insert
            if (OSSID != "0")//--------Modify data
            {
                var query = context.Database.SqlQuery<OpenSheetSealDetails>("select * from OPENSHEET_SEAL_DETAIL where OSMID='" + tab.OSMID + "'").ToList();
                foreach (var result in query)
                {
                    var r_OSSID = result.OSSID;
                    var r_OSMID = result.OSMID;
                    OpenSheetSealDetails opensheetsealdetails = context.opensheetsealdetails.Find(r_OSSID);
                    context.opensheetsealdetails.Remove(opensheetsealdetails);
                    context.SaveChanges();
                }
                for (int i = 1; i < SEAL_VAL.Count(); i++)
                {
                    tab.OSMID = Convert.ToInt32(OSMID);
                    tab.OSSDATE = Convert.ToDateTime(OSSDATE);
                    tab.SEALMID = Convert.ToInt32(SEAL_VAL[i]);
                    tab.OSSDESC = Convert.ToString(OSSDESC[i]);
                    context.opensheetsealdetails.Add(tab);
                    context.SaveChanges();
                }
            }//-----End of modify
            if (Request.Form.Get("OSMLDTYPE") == "0")
                Response.Redirect("/OpenSheet/Index");
            else Response.Redirect("/LCLOpenSheet/Index");
        }//----End of savedata
    }//----End of class
}//---End of namespace