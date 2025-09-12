using scfs.Data;
using scfs_erp.Context;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers.Search
{
    [SessionExpire]
    public class BLSearchDetailsController : Controller
    {
        // GET: BLSearchDetails
        SCFSERPContext context = new SCFSERPContext();
        //[Authorize(Roles = "BLSearchDetailsView")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ViewBag.GIDID = new SelectList("");
            return View();
        }

        public JsonResult GetContainerNo(string id)
        {
            var query = context.Database.SqlQuery<pr_BL_Search_Container_No_Assgn_Result>("EXEC pr_BL_Search_Container_No_Assgn @PBLNo = '" + id + "'").ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetail(int id)
        {
            var data = context.Database.SqlQuery<VW_IMPORT_CONTAINER_DETAIL_QUERY_ASSGN>("SELECT * FROM VW_IMPORT_CONTAINER_DETAIL_QUERY_ASSGN WHERE  GIDID=" + id + "").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOpensheetDet(int id)
        {
            //  var data = context.Database.SqlQuery<OpenSheetMaster>("SELECT * FROM OpenSheetMaster inner join OpenSheetDetail on OpenSheetMaster.OSMID=OpenSheetDetail.OSMID WHERE  GIDID=" + id + "").ToList();
            var data = (from r in context.opensheetmasters
                        join c in context.opensheetdetails on r.OSMID equals c.OSMID
                        where c.GIDID == id
                        select new { r.OSMID, c.OSDID, r.OSMDNO, r.OSMDATE, r.DODATE, r.OSMTIME, r.OSMNAME, r.OSMCNAME, r.CHAID }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDestuffDet(int id)
        {

            var data = (from r in context.authorizatioslipmaster.Where(x => x.SDPTID == 1 && x.ASLMTYPE == 2)
                        join c in context.authorizationslipdetail.Where(X => X.OSDID == id) on r.ASLMID equals c.ASLMID
                        join ct in context.categorymasters.Where(x => x.CATETID == 6) on c.LCATEID equals ct.CATEID
                        join ot in context.ImportDestuffSlipOperation on c.ASLOTYPE equals ot.OPRTYPE
                        select new { r.ASLMID, c.ASLDID, r.ASLMDNO, r.ASLMDATE, r.ASLMTIME, ct.CATENAME, ot.OPRTYPEDESC, c.ASLLTYPE, c.ASLDTYPE }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLoadSlipDet(int id)
        {

            var data = (from r in context.authorizatioslipmaster.Where(x => x.SDPTID == 1 && x.ASLMTYPE == 1)
                        join c in context.authorizationslipdetail.Where(X => X.OSDID == id) on r.ASLMID equals c.ASLMID
                        select new { r.ASLMID, c.ASLDID, r.ASLMDNO, r.ASLMDATE, r.ASLMTIME, c.VHLNO, c.DRVNAME }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVehicleDet(int id)
        {
            var data = context.Database.SqlQuery<VehicleTicketDetail>("SELECT * FROM VehicleTicketDetail WHERE  GIDID=" + id + "").ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDODet(int id)
        {
            var data = (from r in context.DeliveryOrderMasters.Where(x => x.SDPTID == 1)
                        join c in context.DeliveryOrderDetails.Where(X => X.BILLEDID == id) on r.DOMID equals c.DOMID
                        select new { r.DODNO, r.DOREFNAME, r.DODATE, r.DOTIME }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public string GetBillDet(string id)//...bill DETAIL
        {

            var param = id.Split(';');
            var gidid = Convert.ToInt32(param[0]);
            var chaid = Convert.ToInt32(param[1]);
            //var data = context.Database.SqlQuery<TransactionMaster>("select * from TransactionMaster inner join TransactionDetail on TransactionMaster.TRANMID=TransactionDetail.TRANMID  where TransactionDetail.TRANDREFID=" + gidid + " AND TransactionMaster.TRANREFID=" + chaid + "").ToList();
            var data = context.Database.SqlQuery<TransactionMaster>("select * from TransactionMaster inner join TransactionDetail on TransactionMaster.TRANMID=TransactionDetail.TRANMID  where TransactionDetail.TRANDREFID=" + gidid + "").ToList();


            string html = "";



            int i = 1;

            foreach (var rst in data)
            {


                html = html + "<tr><td>" + i + "</td><td>" + rst.TRANDATE.ToString("dd/MM/yyyy") + "<input style='display:none' type=text name=STFDSBNO id='STFDSBNO' class='STFDSBNO'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANTIME.ToString("hh:mm tt") + "<input style='display:none' type=text name=STFDSBDATE id='STFDSBDATE' class='STFDSBDATE'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANDNO + "<input style='display:none' type=text name=STFDSBDNO id='STFDSBDNO' class='STFDSBDNO'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANREFNAME + "<input style='display:none' type=text name=STFDSBDDATE id='STFDSBDDATE' class='STFDSBDDATE'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANREFNO + "<input style='display:none' type=text name=PRDTDESC id='PRDTDESC' class='PRDTDESC'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANREFDATE + "<input style='display:none' type=text name=STFDNOP id='STFDNOP' class='STFDNOP'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANNAMT + "<input style='display:none' type=text name=STFDQTY id='STFDQTY' class='STFDQTY'  onchange='total()' value='" + rst.TRANDATE + "'></td></tr>";
                i++;
            }
            if (data.Count == 0)
                html = html + "<tr><td colspan=8>No Records Found</td></tr>";
            return html;


        }


    }
}