using scfs_erp.Context;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers.Search
{
    [SessionExpire]
    public class ExportContainerDetailsController : Controller
    {
        // GET: ExportContainerDetails
        SCFSERPContext context = new SCFSERPContext();
        //Authorize(Roles = "ContainerDetailsView")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ViewBag.GIDID = new SelectList("");
            return View();
        }

        public ActionResult SBSearch()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ViewBag.GIDID = new SelectList("");
            return View();
        }

        //public JsonResult GetContnrno(string term)
        //{
        //    var data = (from r in context.gateindetails.Where(x => x.SDPTID == 2)
        //                where r.CONTNRNO.Contains(term.ToLower())
        //                select new { r.CONTNRNO, r.GIDID }).ToList();
        //    return Json(data, JsonRequestBehavior.AllowGet);

        //}
        public JsonResult GetContnrno(string id)
        {
            var data = (from r in context.gateindetails.Where(x => x.CONTNRNO == id).Where(x => x.SDPTID == 2).Where(x => x.CONTNRID == 1)
                            //    where r.CONTNRNO.Contains(term.ToLower())
                        select new { r.CONTNRNO, r.GIDID, r.IGMNO }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

        }
       


        public string GetShippingDet(string id)//...STUFFING/SHIPPING DETAIL
        {

            var param = id.Split(';');
            var gidid = Convert.ToInt32(param[0]);
            var chaid = Convert.ToInt32(param[1]);
            //var data = context.Database.SqlQuery<VW_EXPORT_CONT_STUFFING_DETAIL_ASSGN>("select * from VW_EXPORT_CONT_STUFFING_DETAIL_ASSGN  where GIDID=" + gidid + " AND CHAID=" + chaid + "");
            var data = context.Database.SqlQuery<VW_EXPORT_CONT_STUFFING_DETAIL_ASSGN>("select * from VW_EXPORT_CONT_STUFFING_DETAIL_ASSGN  where GIDID=" + gidid + "");


            string html = "";



            int i = 1;

            foreach (var rst in data)
            {

                html = html + "<tr><td>" + i + "</td><td>" + rst.STFMDNO + "<input style='display:none' type=text name=STFMDNO id='STFMDNO' class='STFMDNO'  onchange='total()' value='" + rst.STFMDNO + "'>";
                html = html + "</td><td>" + rst.STFDSBNO + "<input style='display:none' type=text name=STFDSBNO id='STFDSBNO' class='STFDSBNO'  onchange='total()' value='" + rst.STFDSBNO + "'>";
                html = html + "</td><td>" + rst.STFDSBDATE.ToString("dd/MM/yyyy") + "<input style='display:none' type=text name=STFDSBDATE id='STFDSBDATE' class='STFDSBDATE'  onchange='total()' value='" + rst.STFDSBDATE + "'>";
                html = html + "</td><td>" + rst.STFDSBDNO + "<input style='display:none' type=text name=STFDSBDNO id='STFDSBDNO' class='STFDSBDNO'  onchange='total()' value='" + rst.STFDSBDNO + "'>";
                //html = html + "</td><td>" + rst.STFDSBDDATE.ToString("dd/MM/yyyy") + "<input style='display:none' type=text name=STFDSBDDATE id='STFDSBDDATE' class='STFDSBDDATE'  onchange='total()' value='" + rst.STFDSBDDATE + "'>";
                html = html + "</td><td>" + rst.PRDTDESC + "<input style='display:none' type=text name=PRDTDESC id='PRDTDESC' class='PRDTDESC'  onchange='total()' value='" + rst.PRDTDESC + "'>";
                html = html + "</td><td>" + rst.STFDNOP + "<input style='display:none' type=text name=STFDNOP id='STFDNOP' class='STFDNOP'  onchange='total()' value='" + rst.STFDNOP + "'>";
                html = html + "</td><td>" + rst.STFDQTY + "<input style='display:none' type=text name=STFDQTY id='STFDQTY' class='STFDQTY'  onchange='total()' value='" + rst.STFDQTY + "'></td>";
                html = html + "</td><td>" + rst.ESBD_SBILLNO + "<input style='display:none' type=text name=ESBD_SBILLNO id='ESBD_SBILLNO' class='ESBD_SBILLNO'  onchange='total()' value='" + rst.ESBD_SBILLNO + "'>";
                html = html + "</td><td>" + rst.ESBD_SBILLDATE + "<input style='display:none' type=text name=ESBD_SBILLDATE id='ESBD_SBILLDATE' class='ESBD_SBILLDATE'  onchange='total()' value='" + rst.ESBD_SBILLDATE + "'></td>";
                html = html + "</td><td>" + rst.ESBMNOP + "<input style='display:none' type=text name=ESBMNOP id='ESBM_NOP' class='ESBM_NOP'  onchange='total()' value='" + rst.ESBMNOP + "'></td>";
                html = html + "</td><td>" + rst.TRUCKNO + "<input style='display:none' type=text name=TRUCKNO id='TRUCKNO' class='TRUCKNO'  onchange='total()' value='" + rst.TRUCKNO + "'></td>";
                html = html + "</td><td>" + rst.ASLMDNO + "<input style='display:none' type=text name=ASLMDNO id='ASLMDNO' class='ASLMDNO'  onchange='total()' value='" + rst.ASLMDNO + "'></td>";
                html = html + "</td><td>" + rst.ASLMDATE + "<input style='display:none' type=text name=ASLMDATE id='ASLMDATE' class='ASLMDATE'  onchange='total()' value='" + rst.ASLMDATE + "'></td>";
                html = html + "</td><td>" + rst.VTDNO + "<input style='display:none' type=text name=VTDNO id='VTDNO' class='VTDNO'  onchange='total()' value='" + rst.VTDNO + "'></td>";
                html = html + "</td><td>" + rst.VTDATE + "<input style='display:none' type=text name=VTDATE id='VTDATE' class='VTDATE'  onchange='total()' value='" + rst.VTDATE + "'></td></tr>";
                i++;
            }


            return html;


        }


        public string GetBillDet(string id)//...bill DETAIL
        {

            var param = id.Split(';');
            var gidid = Convert.ToInt32(param[0]);
            var chaid = Convert.ToInt32(param[1]);
            string qry = "select * from TransactionMaster inner join TransactionDetail on TransactionMaster.TRANMID = TransactionDetail.TRANMID  where TransactionDetail.TRANDREFID = " + gidid;// + " AND TransactionMaster.TRANREFID = " + chaid ;
            var data = context.Database.SqlQuery<TransactionMaster>(qry).ToList();


            string html = "";



            int i = 1;

            foreach (var rst in data)
            {


                string alink = "StuffingBill/APrintView/" + rst.TRANMID;
                string title = "target='_blank'";

                html = html + "<tr><td>" + i + "</td><td>" + rst.TRANDATE.ToString("dd/MM/yyyy") + "<input style='display:none' type=text name=STFDSBNO id='STFDSBNO' class='STFDSBNO'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANTIME.ToString("hh:mm t") + "<input style='display:none' type=text name=STFDSBDATE id='STFDSBDATE' class='STFDSBDATE'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANDNO + "<input style='display:none' type=text name=STFDSBDNO id='STFDSBDNO' class='STFDSBDNO'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANREFNAME + "<input style='display:none' type=text name=STFDSBDDATE id='STFDSBDDATE' class='STFDSBDDATE'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANREFNO + "<input style='display:none' type=text name=PRDTDESC id='PRDTDESC' class='PRDTDESC'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANREFDATE + "<input style='display:none' type=text name=STFDNOP id='STFDNOP' class='STFDNOP'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANNAMT + "<input style='display:none' type=text name=STFDQTY id='STFDQTY' class='STFDQTY'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td><a class='btn btn-primary goto' href='/" + alink + "' " + title + ">view</a></td></tr>";
                i++;
            }


            return html;


        }


        ///.................................SHIPPING BILL BASED SEARCH.............
        ///
        public JsonResult GetShippingMaster(string id)
        {
            id.Replace("~", "/");
            var param = id.Split(';'); var condtnstr = "";
            if (param[1] == "0") condtnstr = "ESBMDNO='" + param[0] + "'";
            else if (param[1] == "1")
            {
                var data0 = context.Database.SqlQuery<ExportShippingBillDetail>("SELECT * FROM EXPORTSHIPPINGBILLDETAIL (NOLOCK) WHERE ESBOINVNO ='" + param[0] + "'").ToList();
                if (data0.Count > 0)
                {
                    condtnstr = " ESBMID =" + data0[0].ESBMID + "";
                }
                else
                    condtnstr = " ESBMID = 0";

            }
            else if (param[1] == "2")
            {
                var data0 = context.Database.SqlQuery<ExportShippingBillDetail>("SELECT * FROM EXPORTSHIPPINGBILLDETAIL (NOLOCK) WHERE ESBD_SBILLNO ='" + param[0] + "'").ToList();
                if (data0.Count > 0)
                {

                    condtnstr = " ESBMID in (";
                    foreach(var dat in data0)
                    {
                        condtnstr += dat.ESBMID + ",";
                    }
                    condtnstr = condtnstr.Substring(0, condtnstr.Length - 1)+")";
                }
                else
                    condtnstr = " ESBMID = 0";

            }//condtnstr = "ESBMREFNO='" + param[0] + "'";
            var data = context.Database.SqlQuery<ExportShippingBillMaster>("SELECT * FROM ExportShippingBillMaster  (NOLOCK) WHERE  " + condtnstr + "").ToList();

            //string html = "";

            //int i = 1;

            //foreach (var rst in data)
            //{

            //    html = html + "<tr><td>" + i + "</td><td>" + rst.ESBMDATE.ToString("dd/MM/yyyy");
            //    html = html + "</td><td>" + rst.CHANAME + "<input style='display:none' type=text name=CHANAME id='CHANAME' class='CHANAME'  onchange='total()' value='" + rst.CHANAME + "'>";
            //    html = html + "</td><td>" + rst.EXPRTNAME + "<input style='display:none' type=text name=EXPRTNAME id='EXPRTNAME' class='EXPRTNAME'  onchange='total()' value='" + rst.EXPRTNAME + "'>";
            //    html = html + "</td><td>" + rst.PRDTDESC + "<input style='display:none' type=text name=PRDTDESC id='PRDTDESC' class='PRDTDESC'  onchange='total()' value='" + rst.PRDTDESC + "'>";
            //    html = html + "</td><td>" + rst.ESBMDPNAME + "<input style='display:none' type=text name=ESBMDPNAME id='ESBMDPNAME' class='ESBMDPNAME'  onchange='total()' value='" + rst.ESBMDPNAME + "'>";
            //    html = html + "</td><td>" + rst.ESBMNOP + "<input style='display:none' type=text name=ESBMNOP id='ESBMNOP' class='ESBMNOP'  onchange='total()' value='" + rst.ESBMNOP + "'>";
            //    html = html + "</td><td>" + rst.ESBMQTY + "<input style='display:none' type=text name=ESBMQTY id='ESBMQTY' class='ESBMQTY'  onchange='total()' value='" + rst.ESBMQTY + "'></td>";                
            //    i++;
            //}


            //ViewBag.SBDDIV = html;
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public string GetCODet(int id)//...CARTING ORDER DETAIL
        {
            var data = context.Database.SqlQuery<VW_EXPORT_SHIPPINGBILL_WISE_CARTINGORDER_DETAIL_ASSGN>("select * from VW_EXPORT_SHIPPINGBILL_WISE_CARTINGORDER_DETAIL_ASSGN  where ESBMID=" + id + "").ToList();


            string html = "";



            int i = 1;

            foreach (var rst in data)
            {

                html = html + "<tr><td>" + i + "</td><td>" + rst.SBMDNO + "<input style='display:none' type=text name=SBMDNO id='SBMDNO' class='SBMDNO'  onchange='total()' value='" + rst.SBMDNO + "'>";


                //html = html + "</td><td>" + rst.SBMDATE.ToString("dd/MM/yyyy") + "<input style='display:none' type=text name=STFDSBDATE id='STFDSBDATE' class='STFDSBDATE'  onchange='total()' value='" + rst.SBMDATE + "'>";
                //html = html + "</td><td>" + rst.ESBD_SBILLNO + "<input style='display:none' type=text name=STFDSBDNO id='STFDSBDNO' class='STFDSBDNO'  onchange='total()' value='" + rst.SBDDNO + "'>";
                //html = html + "</td><td>" + rst.PRDTDESC + "<input style='display:none' type=text name=PRDTDESC id='PRDTDESC' class='PRDTDESC'  onchange='total()' value='" + rst.PRDTDESC + "'>";
                //html = html + "</td><td>" + rst.TRUCKNO + "<input style='display:none' type=text name=TRUCKNO id='TRUCKNO' class='TRUCKNO'  onchange='total()' value='" + rst.TRUCKNO + "'>";
                //html = html + "</td><td>" + rst.SBDDATE.ToString("dd/MM/yyyy") + "<input style='display:none' type=text name=STFDSBDDATE id='STFDSBDDATE' class='STFDSBDDATE'  onchange='total()' value='" + rst.SBDDATE + "'>";
                //html = html + "</td><td>" + rst.SBDNOP + "<input style='display:none' type=text name=STFDQTY id='STFDQTY' class='STFDQTY'  onchange='total()' value='" + rst.SBDNOP + "'></td>";
                //html = html + "</td><td>" + rst.SBDQTY + "<input style='display:none' type=text name=STFDQTY id='STFDQTY' class='STFDQTY'  onchange='total()' value='" + rst.SBDQTY + "'></td></tr>";
                html = html + "</td><td>" + rst.SBMDATE.ToString("dd/MM/yyyy") + "<input style='display:none' type=text name=STFDSBDATE id='STFDSBDATE' class='STFDSBDATE'  onchange='total()' value='" + rst.SBMDATE + "'>";
                html = html + "</td><td>" + rst.ESBD_SBILLNO + "<input style='display:none' type=text name=STFDSBDNO id='STFDSBDNO' class='STFDSBDNO'  onchange='total()' value='" + rst.SBDDNO + "'>";
                html = html + "</td><td>" + rst.PRDTDESC + "<input style='display:none' type=text name=PRDTDESC id='PRDTDESC' class='PRDTDESC'  onchange='total()' value='" + rst.PRDTDESC + "'>";
                html = html + "</td><td>" + rst.SBDNOP + "<input style='display:none' type=text name=SBDNOP id='SBDNOP' class='SBDNOP'  onchange='total()' value='" + rst.SBDNOP + "'>";
                html = html + "</td><td>" + rst.SBDQTY + "<input style='display:none' type=text name=STFDQTY id='STFDQTY' class='STFDQTY'  onchange='total()' value='" + rst.SBDQTY + "'>";
                html = html + "</td><td>" + rst.TRUCKNO + "<input style='display:none' type=text name=TRUCKNO id='TRUCKNO' class='TRUCKNO'  onchange='total()' value='" + rst.TRUCKNO + "'>";
                html = html + "</td><td>" + rst.SBDDATE.ToString("dd/MM/yyyy") + "<input style='display:none' type=text name=STFDSBDDATE id='STFDSBDDATE' class='STFDSBDDATE'  onchange='total()' value='" + rst.SBDDATE + "'>";
                html = html + "</td><td>" + rst.ESBMNOP + "<input style='display:none' type=text name=ESBMNOP id='ESBMNOP' class='ESBMNOP'  onchange='total()' value='" + rst.ESBMNOP + "'>";
                html = html + "</td><td>" + rst.ESBMQTY + "<input style='display:none' type=text name=ESBMQTY id='ESBMQTY' class='ESBMQTY'  onchange='total()' value='" + rst.ESBMQTY + "'></td></tr>";

                i++;
            }


            return html;


        }

        public string GetCODetOSBLN(string id)//...CARTING ORDER DETAIL
        {
            var data = context.Database.SqlQuery<VW_EXPORT_SHIPPINGBILL_WISE_CARTINGORDER_DETAIL_ASSGN>("select * from VW_EXPORT_SHIPPINGBILL_WISE_CARTINGORDER_DETAIL_ASSGN  where ESBD_SBILLNO='" + id + "'").ToList();


            string html = "";



            int i = 1;

            foreach (var rst in data)
            {

                html = html + "<tr><td>" + i + "</td><td>" + rst.SBMDNO + "<input style='display:none' type=text name=SBMDNO id='SBMDNO' class='SBMDNO'  onchange='total()' value='" + rst.SBMDNO + "'>";

                html = html + "</td><td>" + rst.SBMDATE.ToString("dd/MM/yyyy") + "<input style='display:none' type=text name=STFDSBDATE id='STFDSBDATE' class='STFDSBDATE'  onchange='total()' value='" + rst.SBMDATE + "'>";
                html = html + "</td><td>" + rst.ESBD_SBILLNO + "<input style='display:none' type=text name=STFDSBDNO id='STFDSBDNO' class='STFDSBDNO'  onchange='total()' value='" + rst.SBDDNO + "'>";
                html = html + "</td><td>" + rst.PRDTDESC + "<input style='display:none' type=text name=PRDTDESC id='PRDTDESC' class='PRDTDESC'  onchange='total()' value='" + rst.PRDTDESC + "'>";
                html = html + "</td><td>" + rst.SBDNOP + "<input style='display:none' type=text name=SBDNOP id='SBDNOP' class='SBDNOP'  onchange='total()' value='" + rst.SBDNOP + "'>";
                html = html + "</td><td>" + rst.SBDQTY + "<input style='display:none' type=text name=STFDQTY id='STFDQTY' class='STFDQTY'  onchange='total()' value='" + rst.SBDQTY + "'>";
                html = html + "</td><td>" + rst.TRUCKNO + "<input style='display:none' type=text name=TRUCKNO id='TRUCKNO' class='TRUCKNO'  onchange='total()' value='" + rst.TRUCKNO + "'>";
                html = html + "</td><td>" + rst.SBDDATE.ToString("dd/MM/yyyy") + "<input style='display:none' type=text name=STFDSBDDATE id='STFDSBDDATE' class='STFDSBDDATE'  onchange='total()' value='" + rst.SBDDATE + "'>";
                html = html + "</td><td>" + rst.ESBMNOP + "<input style='display:none' type=text name=ESBMNOP id='ESBMNOP' class='ESBMNOP'  onchange='total()' value='" + rst.ESBMNOP + "'>";
                html = html + "</td><td>" + rst.ESBMQTY + "<input style='display:none' type=text name=ESBMQTY id='ESBMQTY' class='ESBMQTY'  onchange='total()' value='" + rst.ESBMQTY + "'></td></tr>";
                i++;
            }


            return html;


        }

        public string GetCODetOINV(string id)//...CARTING ORDER DETAIL
        {
            var data = context.Database.SqlQuery<VW_EXPORT_SHIPPINGBILL_WISE_CARTINGORDER_DETAIL_ASSGN>("select * from VW_EXPORT_SHIPPINGBILL_WISE_CARTINGORDER_DETAIL_ASSGN  where ESBOINVNO='" + id + "'").ToList();


            string html = "";



            int i = 1;

            foreach (var rst in data)
            {

                html = html + "<tr><td>" + i + "</td><td>" + rst.SBMDNO + "<input style='display:none' type=text name=SBMDNO id='SBMDNO' class='SBMDNO'  onchange='total()' value='" + rst.SBMDNO + "'>";


                html = html + "</td><td>" + rst.SBMDATE.ToString("dd/MM/yyyy") + "<input style='display:none' type=text name=STFDSBDATE id='STFDSBDATE' class='STFDSBDATE'  onchange='total()' value='" + rst.SBMDATE + "'>";
                html = html + "</td><td>" + rst.ESBD_SBILLNO + "<input style='display:none' type=text name=STFDSBDNO id='STFDSBDNO' class='STFDSBDNO'  onchange='total()' value='" + rst.SBDDNO + "'>";
                html = html + "</td><td>" + rst.PRDTDESC + "<input style='display:none' type=text name=PRDTDESC id='PRDTDESC' class='PRDTDESC'  onchange='total()' value='" + rst.PRDTDESC + "'>";
                html = html + "</td><td>" + rst.SBDNOP + "<input style='display:none' type=text name=SBDNOP id='SBDNOP' class='SBDNOP'  onchange='total()' value='" + rst.SBDNOP + "'>";
                html = html + "</td><td>" + rst.SBDQTY + "<input style='display:none' type=text name=STFDQTY id='STFDQTY' class='STFDQTY'  onchange='total()' value='" + rst.SBDQTY + "'>";
                html = html + "</td><td>" + rst.TRUCKNO + "<input style='display:none' type=text name=TRUCKNO id='TRUCKNO' class='TRUCKNO'  onchange='total()' value='" + rst.TRUCKNO + "'>";
                html = html + "</td><td>" + rst.SBDDATE.ToString("dd/MM/yyyy") + "<input style='display:none' type=text name=STFDSBDDATE id='STFDSBDDATE' class='STFDSBDDATE'  onchange='total()' value='" + rst.SBDDATE + "'>";
                html = html + "</td><td>" + rst.ESBMNOP + "<input style='display:none' type=text name=ESBMNOP id='ESBMNOP' class='ESBMNOP'  onchange='total()' value='" + rst.ESBMNOP + "'>";
                html = html + "</td><td>" + rst.ESBMQTY + "<input style='display:none' type=text name=ESBMQTY id='ESBMQTY' class='ESBMQTY'  onchange='total()' value='" + rst.ESBMQTY + "'></td></tr>";
                i++;
            }


            return html;


        }
        public JsonResult GetDetail(int id)
        {
            var data = context.Database.SqlQuery<VW_EXPORT_CONTAINER_DETAIL_QUERY_ASSGN>("SELECT * FROM VW_EXPORT_CONTAINER_DETAIL_QUERY_ASSGN WHERE  GIDID=" + id + "").ToList();
            //var data = (from r in context.gateindetails.Where(x => x.CONTNRNO == id)
            //            join c in context.containersizemasters on r.CONTNRSID equals c.CONTNRSID
            //            select new { r.GIDID, r.GIDATE, r.VOYNO, r.VSLID, r.VSLNAME, r.IMPRTID, r.IMPRTNAME, r.STMRID, r.STMRNAME, r.CONTNRNO, r.GPLNO, r.IGMNO,c.CONTNRSID, c.CONTNRSCODE, r.GPETYPE, r.GPEAMT, r.GPWTYPE, r.GPAAMT,r.GPSTYPE,r.GPSCNTYPE }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public string GetStuffingDet(int id)//...STUFFING/SHIPPING DETAIL
        {
            var data = context.Database.SqlQuery<VW_EXPORT_SHIPPING_BILL_WISE_STUFFNG_DETAIL_ASSGN>("select * from VW_EXPORT_SHIPPING_BILL_WISE_STUFFNG_DETAIL_ASSGN  where ESBMID=" + id + "").ToList();


            string html = "";



            int i = 1;

            foreach (var rst in data)
            {

                html = html + "<tr><td>" + i + "</td><td>" + rst.STFMDNO + "<input style='display:none' type=text name=STFMDNO id='STFMDNO' class='STFMDNO'  onchange='total()' value='" + rst.STFMDNO + "'>"; html = html + "<input style='display:none' type=text name=STFMID id='STFMID' class='STFMID'  onchange='total()' value='" + data[0].STFMID + "'>";
                html = html + "</td><td>" + rst.STFMDATE.ToString("dd/MM/yyyy") + "<input style='display:none' type=text name=STFDSBNO id='STFDSBNO' class='STFDSBNO'  onchange='total()' value='" + rst.STFDSBNO + "'>";
                //    html = html + "</td><td>" + rst.STFDSBDATE.ToString("dd/MM/yyyy") + "<input style='display:none' type=text name=STFDSBDATE id='STFDSBDATE' class='STFDSBDATE'  onchange='total()' value='" + rst.STFDSBDATE + "'>";
                html = html + "</td><td>" + rst.STFDSBDNO + "<input style='display:none' type=text name=STFDSBDNO id='STFDSBDNO' class='STFDSBDNO'  onchange='total()' value='" + rst.STFDSBDNO + "'>";
                html = html + "</td><td>" + rst.STFDSBDDATE.ToString("dd/MM/yyyy") + "<input style='display:none' type=text name=STFDSBDDATE id='STFDSBDDATE' class='STFDSBDDATE'  onchange='total()' value='" + rst.STFDSBDDATE + "'>";
                html = html + "</td><td>" + rst.PRDTDESC + "<input style='display:none' type=text name=PRDTDESC id='PRDTDESC' class='PRDTDESC'  onchange='total()' value='" + rst.PRDTDESC + "'>";
                html = html + "</td><td>" + rst.STFDNOP + "<input style='display:none' type=text name=STFDNOP id='STFDNOP' class='STFDNOP'  onchange='total()' value='" + rst.STFDNOP + "'>";
                html = html + "</td><td>" + rst.STFDQTY + "<input style='display:none' type=text name=STFDQTY id='STFDQTY' class='STFDQTY'  onchange='total()' value='" + rst.STFDQTY + "'></td></tr>";
                i++;
            }


            return html;


        }
        public string GetSBBillDet(string id)//...bill DETAIL
        {

            var param = id.Split(';');
            var STFMID = Convert.ToInt32(param[1]);
            var chaid = Convert.ToInt32(param[0]);
            var data = context.Database.SqlQuery<VW_EXPORT_SHIPPING_BILL_INVOICE_DETAIL>("Select * From VW_EXPORT_SHIPPING_BILL_INVOICE_DETAIL where ESBMID = " + STFMID ); //+ " AND   TRANREFID=" + chaid + ""


            string html = "";



            int i = 1;

            foreach (var rst in data)
            {


                html = html + "<tr><td>" + i + "</td><td>" + rst.TRANDATE.ToString("dd/MM/yyyy") + "<input style='display:none' type=text name=STFDSBNO id='STFDSBNO' class='STFDSBNO'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANTIME.ToString("hh:mm t") + "<input style='display:none' type=text name=STFDSBDATE id='STFDSBDATE' class='STFDSBDATE'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANDNO + "<input style='display:none' type=text name=STFDSBDNO id='STFDSBDNO' class='STFDSBDNO'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANREFNAME + "<input style='display:none' type=text name=STFDSBDDATE id='STFDSBDDATE' class='STFDSBDDATE'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANREFNO + "<input style='display:none' type=text name=PRDTDESC id='PRDTDESC' class='PRDTDESC'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANREFDATE + "<input style='display:none' type=text name=STFDNOP id='STFDNOP' class='STFDNOP'  onchange='total()' value='" + rst.TRANDATE + "'>";
                html = html + "</td><td>" + rst.TRANNAMT + "<input style='display:none' type=text name=STFDQTY id='STFDQTY' class='STFDQTY'  onchange='total()' value='" + rst.TRANDATE + "'></td></tr>";
                i++;
            }


            return html;


        }


    }
}