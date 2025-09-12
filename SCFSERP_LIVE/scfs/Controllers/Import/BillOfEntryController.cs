using scfs_erp.Context;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using scfs_erp;
using scfs.Data;

namespace scfs_erp.Controllers.Import
{
    [SessionExpire]
    public class BillOfEntryController : Controller
    {
        //
        // GET: /BillOfEntry/
        SCFSERPContext context = new SCFSERPContext();
        [Authorize(Roles = "BOEIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            if (string.IsNullOrEmpty(Session["SDATE"] as string))
            {

                Session["SDATE"] = DateTime.Now.ToString("dd-MM-yyyy");
                Session["EDATE"] = DateTime.Now.ToString("dd-MM-yyyy");
            }
            else
            {
                if (Request.Form.Get("from") != null)
                {
                    Session["SDATE"] = Request.Form.Get("from");
                    Session["EDATE"] = Request.Form.Get("to");
                }

            }
            DateTime sd = Convert.ToDateTime(Session["SDATE"]).Date;
            DateTime ed = Convert.ToDateTime(Session["EDATE"]).Date;

            return View();
        }
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {

            using (var e = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_SearchBillOfEntry(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]));

                var aaData = data.Select(d => new string[] { d.BILLEMTIME.Value.ToString("dd/MM/yyyy"), d.BILLEMTIME.Value.ToString("hh:mm tt"), d.BILLEMNO.ToString(), d.BILLEMDNO, d.BILLEMNAME, d.NOC.ToString(), d.DISPSTATUS, d.BILLEMID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "BOECreate")]
        public ActionResult Form(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            BOEDetails vm = new BOEDetails();
            BillEntryMaster master = new BillEntryMaster();


            ViewBag.UNITID = new SelectList(context.unitmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.UNITDESC), "UNITID", "UNITDESC");

            ViewBag.BILLEDMTYPE = new SelectList(context.opensheetviadetails, "OSMTYPE", "OSMTYPEDESC");




            //-------------------------------DISPSTATUS----

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = false };
            selectedDISPSTATUS.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = true };
            selectedDISPSTATUS.Add(selectedItemDSP);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;


            //-------------opensheetdetails---------------

            if (id != 0)
            {
                master = context.billentrymasters.Find(id);//find selected record

                vm.masterdata = context.billentrymasters.Where(det => det.BILLEMID == id).ToList();
                vm.detaildata = context.billentrydetails.Where(det => det.BILLEMID == id).ToList();



                //---------Dropdown lists-------------------

                ViewBag.UNITID = new SelectList(context.unitmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.UNITDESC), "UNITID", "UNITDESC", master.UNITID);

                ViewBag.BILLEDMTYPE = new SelectList(context.opensheetviadetails, "OSMTYPE", "OSMTYPEDESC", master.BILLEDMTYPE);
                //--------End of dropdown




                //----------------To Display GateInDetails-----------------------

                //GateInDetail gdet = context.gateindetails.Find(Convert.ToInt32(vm.detaildata[0].GIDID));
                //if (vm.detaildata[0].GIDID == gdet.GIDID)
                //{
                //    ViewBag.VOYNO = gdet.VOYNO;
                //    ViewBag.IMPRTNAME = gdet.IMPRTNAME;
                //    ViewBag.STMRNAME = gdet.STMRNAME;

                //}//------End


            }//---End Of IF


            return View(vm);
        }//----End of Form


        public void DetailEdit(int id)
        {
            using (SCFSERPContext contxt = new SCFSERPContext())
            {
                var detail = (from open in contxt.billentrydetails.Where(x => x.BILLEMID == id)
                              join gate in contxt.gateindetails on open.GIDID equals gate.GIDID
                              join sdesc in contxt.containersizemasters on gate.CONTNRSID equals sdesc.CONTNRSID
                              // where gate.IGMNO == Igm && gate.GPLNO == Line
                              select new { sdesc.CONTNRSDESC, open.BILLEDID, open.BILLEMID, open.GIDID, open.GIDATE, gate.IMPRTNAME, gate.STMRNAME, gate.VSLNAME, gate.CONTNRNO, gate.CONTNRSID, gate.IGMNO, gate.GPLNO, gate.VOYNO }).Distinct();

                var tabl = " <div class='panel-heading navbar-inverse'  style=color:white>BOE Details</div><Table id=mytabl class='table table-striped table-bordered bootstrap-datatable'> <thead><tr> <th>Container No</th><th> Size</th><th>InDate</th><th>IGM No</th> <th> Line No </th><th>Voyage No</th> <th>Vessel Name </th></tr> </thead>";

                foreach (var result in detail)
                {
                    tabl = tabl + "<tbody><tr><td class=hide><input type=text id=BILLEDID value=" + result.BILLEDID + " class=BILLEDID name=BILLEDID></td><td class=hide><input type=text id=F_GIDID value=" + result.GIDID + "  class=F_GIDID name=F_GIDID hidden></td><td class=hide><input type=text id=STMRNAME value='" + result.STMRNAME + "' class=STMRNAME name=STMRNAME></td><td class=hide><input type=text id=IMPRTNAME value='" + result.IMPRTNAME + "' class=IMPRTNAME name=IMPRTNAME hidden></td><td class='col-lg-2'><input type=text id=CONTNRNO value=" + result.CONTNRNO + " class='form-control CONTNRNO' name=CONTNRNO readonly></td><td class='col-lg-1'><input type=text value=" + result.CONTNRSDESC + " id=CONTNRSDESC class='form-control CONTNRSDESC' name=CONTNRSDESC readonly></td><td class='col-lg-2'><input type=text id=GIDATE value=" + result.GIDATE + " class='form-control GIDATE' name=GIDATE readonly></td><td class='col-lg-2'><input type=text value=" + result.IGMNO + " id=IGMNO class='form-control IGMNO' name=IGMNO readonly></td><td class='col-lg-1'><input type=text id=LineNo value=" + result.GPLNO + " class='form-control LineNo' name=LineNo readonly></td><td><input type=text id=VOYNO value='" + result.VOYNO + "' class='form-control VOYNO' name=VOYNO readonly></td><td><input type=text id=VSLNAME value='" + result.VSLNAME + "' class='form-control VSLNAME' name=VSLNAME readonly></td></tr></tbody>";

                }
                tabl = tabl + "</Table>";
                Response.Write(tabl);

            }


        }
    }
}