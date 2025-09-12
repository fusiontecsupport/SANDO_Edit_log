using scfs.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers.Import
{
    [SessionExpire]
    public class ImportUpdateDetailsController : Controller
    {
        // GET: ImportUpdateDetails

        #region Context declaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region LINE NO UPDATE Index
        //[Authorize(Roles = "ImportUpdateDetailsIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View();
        }
        #endregion

        #region PRODUCT DETAILS UPDATE PIndex
        //[Authorize(Roles = "ImportUpdateDetailsPIndex")]
        public ActionResult PIndex()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");
            return View();
        }
        #endregion

        #region WEIGHT UPDATE WIndex
        //[Authorize(Roles = "ImportUpdateDetailsWIndex")]
        public ActionResult WIndex()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View();
        }
        #endregion

        #region Detail
        public string Detail(string id)/*line no update details*/
        {
            var param = id.Split('~');
            var igmno = (param[0]); var gplno = param[1];

            //var query = (from r in context.gateindetails.Where(x => x.SDPTID == 1 && x.DISPSTATUS == 0 && x.CONTNRID == 1)
            //             join s in context.containersizemasters on r.CONTNRSID equals s.CONTNRSID
            //             where (r.IGMNO == igmno)
            //             where (r.GPLNO == gplno)
            //             select new { r.GIDID, r.GIDATE, r.CONTNRNO, r.STMRNAME, r.IGMNO, r.GPLNO, s.CONTNRSDESC }
            //                 ).ToList();

            var query = context.Database.SqlQuery<pr_IGMNO_Line_Search_ContainerNo_GridAssgn_N01_Result>("EXEC pr_IGMNO_Line_Search_ContainerNo_GridAssgn_N01  @PIGNNo = '" + igmno.ToString() + "', @PGPLNo = '" + gplno.ToString() + "'").ToList();

            var tabl = "";
            var count = 1;
            var chkdesc = "";

            foreach (var rslt in query)
            {

                if (rslt.OSDID > 0)
                {
                    chkdesc = "disabled readonly";
                }
                else
                {
                    chkdesc = "";
                }
                //tabl = tabl + "<tr><td>" + count + "</td><td>" + rslt.GIDATE.ToString("dd/MM/yyyy") + "<input type=text id=sno value=0  class='sno hide' readonly='readonly' name=sno style='width:56px'><input type=text id=GIDID value=" + rslt.GIDID + "  class='GIDID hide' readonly='readonly' name=GIDID style='width:56px'></td><td class='col-lg-0'><input type='text' id='CONTNRNO' value='" + rslt.CONTNRNO + "'  class='CONTNRNO hide' readonly='readonly' name='CONTNRNO' >" + rslt.CONTNRNO + "</td><td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class='hide SIZE' name=SIZE style='width:40px' readonly='readonly'>" + rslt.CONTNRSDESC + "</td><td >" + rslt.IGMNO + "<input type=text id=TRANIDATE value='' class='hide TRANIDATE' name=TRANIDATE style='width:70px' readonly='readonly'></td><td >" + rslt.GPLNO + "<input type=text id=TRANSDATE value='' class='hide TRANSDATE' name=TRANSDATE readonly='readonly' style='width:70px'></td><td>" + rslt.STMRNAME + "<input type=text id='TRANEDATE' value='' class='TRANEDATE hide' name='TRANEDATE' style='width:70px' onchange='calculation()'></td><td class='hidden1'><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS value='' " + count + "  onchange=total() style='width:30px'><input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value=''></td></tr>";
                tabl = tabl + "<tr><td>" + count + "</td><td>" + rslt.GIDATE.ToString() + "<input type=text id=sno value=0  class='sno hide' readonly='readonly' name=sno style='width:56px'><input type=text id=GIDID value=" + rslt.GIDID + "  class='GIDID hide' readonly='readonly' name=GIDID style='width:56px'></td><td class='col-lg-0'><input type='text' id='CONTNRNO' value='" + rslt.CONTNRNO + "'  class='CONTNRNO hide' readonly='readonly' name='CONTNRNO' >" + rslt.CONTNRNO + "</td><td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class='hide SIZE' name=SIZE style='width:40px' readonly='readonly'>" + rslt.CONTNRSDESC + "</td><td >" + rslt.IGMNO + "<input type=text id=TRANIDATE value='' class='hide TRANIDATE' name=TRANIDATE style='width:70px' readonly='readonly'></td><td >" + rslt.GPLNO + "<input type=text id=TRANSDATE value='' class='hide TRANSDATE' name=TRANSDATE readonly='readonly' style='width:70px'></td><td>" + rslt.STMRNAME + "<input type=text id='TRANEDATE' value='' class='TRANEDATE hide' name='TRANEDATE' style='width:70px' onchange='calculation()'></td><td class='hidden1'><input type=checkbox id=STFDIDS class=STFDIDS " + chkdesc + " name =STFDIDS value='' " + count + "  onchange=total() style='width:30px'><input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value=''></td></tr>";
                count++;
            }
            tabl = tabl + "</Table>";

            return tabl;

        }
        #endregion

        #region PDetail
        public string PDetail(string id)/*product update details*/
        {
            var param = id.Split('~');
            var igmno = (param[0]); var gplno = param[1];

            var query = (from r in context.gateindetails.Where(x => x.SDPTID == 1 && x.DISPSTATUS == 0 && x.CONTNRID == 1)
                         join s in context.containersizemasters on r.CONTNRSID equals s.CONTNRSID
                         join p in context.productgroupmasters on r.PRDTGID equals p.PRDTGID
                         where (r.IGMNO == igmno)
                         where (r.GPLNO == gplno)
                         select new { r.GIDID, r.GIDATE, r.CONTNRNO, r.STMRNAME, r.IGMNO, r.GPLNO, s.CONTNRSDESC, p.PRDTGDESC, r.PRDTDESC }
                             ).ToList();

            var tabl = "";
            var count = 1;

            foreach (var rslt in query)
            {

                tabl = tabl + "<tr><td>" + count + "</td><td>" + rslt.GIDATE.ToString("dd/MM/yyyy") + "<input type=text id=sno value=0  class='sno hide' readonly='readonly' name=sno style='width:56px'>";
                tabl = tabl + "<input type=text id=GIDID value=" + rslt.GIDID + "  class='GIDID hide' readonly='readonly' name=GIDID style='width:56px'></td>";
                tabl = tabl + "<td class='col-lg-0'><input type='text' id='CONTNRNO' value='" + rslt.CONTNRNO + "'  class='CONTNRNO hide' readonly='readonly' name='CONTNRNO' >" + rslt.CONTNRNO + "</td>";
                tabl = tabl + "<td>" + rslt.CONTNRSDESC + "</td>";
                tabl = tabl + "<td >" + rslt.IGMNO + "</td>";
                tabl = tabl + "<td >" + rslt.GPLNO + "</td>";
                tabl = tabl + "<td>" + rslt.STMRNAME + "</td>";
                tabl = tabl + "<td>" + rslt.PRDTGDESC + "</td>";
                tabl = tabl + "<td>" + rslt.PRDTDESC + "";
                tabl = tabl + "<input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value=''></td></tr>";
                count++;
            }
            tabl = tabl + "</Table>";

            return tabl;

        }
        #endregion

        #region Auto complete for Container No
        //public JsonResult AutoContainer(string term)
        //{
        //    var result = (from r in context.gateindetails.Where(x => x.SDPTID == 1 && x.CNTNRSID > 0)
        //                  where r.CONTNRNO.Contains(term.ToLower().ToString())
        //                  select new { r.CONTNRNO, r.GIDID }).Distinct();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult AutoContainer(string term)
        {
            var result = context.Database.SqlQuery<pr_UPDATAE_CONTAINERNo_Result>("EXEC pr_UPDATAE_CONTAINERNo @FilterTerm='%" + term + "%'").ToList();

           
            return Json(result, JsonRequestBehavior.AllowGet);

            //var result = (from r in context.transactionmaster.Where(m => m.DISPSTATUS == 0 && m.REGSTRID < 60)
            //              where r.TRANDNO.ToLower().Contains(term.ToLower())
            //              select new { r.TRANDNO, r.TRANMID }).Distinct();
            //return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SSRChargesAutoContainer(string term)
        {
            var result = context.Database.SqlQuery<pr_UPDATAE_SSRCHARGES_CONTAINERNo_Result>("EXEC pr_UPDATAE_SSRCHARGES_CONTAINERNo @FilterTerm='%" + term + "%'").ToList();


            return Json(result, JsonRequestBehavior.AllowGet);

            //var result = (from r in context.transactionmaster.Where(m => m.DISPSTATUS == 0 && m.REGSTRID < 60)
            //              where r.TRANDNO.ToLower().Contains(term.ToLower())
            //              select new { r.TRANDNO, r.TRANMID }).Distinct();
            //return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region RSDetail
        public string RSDetail(string id)/*product update details*/
        {

            var containerno = id;

            var query = (from r in context.gateindetails.Where(x => x.SDPTID == 1 && x.DISPSTATUS == 0 && x.CONTNRSID > 0)
                         join s in context.containersizemasters on r.CONTNRSID equals s.CONTNRSID 
                         join r1 in context.rowmasters on r.ROWID equals r1.ROWID  
                         join s1 in context.slotmasters on r.SLOTID equals s1.SLOTID
                         where (r.CONTNRNO == containerno)
                         select new { r.GIDID, r.GIDATE, s.CONTNRSDESC, r.VOYNO, r.IMPRTNAME, r.VSLNAME, r.STMRNAME, r.IGMNO, r.GPLNO, r1.ROWDESC, r.ROWID, s1.SLOTDESC, r.SLOTID, r.CONTNRNO }
                             ).ToList();

            var tabl = "";
            var count = 1;

            foreach (var rslt in query)
            {
                tabl = tabl + "<Table>";
                tabl = tabl + "<tr><td>" + count + "</td>";
                tabl = tabl + "<td>" + rslt.GIDATE.ToString("dd/MM/yyyy") + "<input type=text id=GIDID value=" + rslt.GIDID + "  class='GIDID hide' readonly='readonly' name=GIDID style='width:56px'></td>";
                tabl = tabl + "<td>" + rslt.CONTNRSDESC + "</td>";
                tabl = tabl + "<td>" + rslt.IGMNO + "</td>";
                tabl = tabl + "<td >" + rslt.GPLNO + "</td>";
                tabl = tabl + "<td >" + rslt.CONTNRNO + "</td>";
                tabl = tabl + "<td >" + rslt.VOYNO + "</td>";
                tabl = tabl + "<td >" + rslt.VSLNAME + "</td>";
                tabl = tabl + "<td >" + rslt.IMPRTNAME + "</td>";                                
                tabl = tabl + "<td>" + rslt.STMRNAME + "</td>";
                tabl = tabl + "<td>" + rslt.ROWDESC + "<input type=text id=ROWID value='" + rslt.ROWID.ToString() + "'  class='ROWID hide' readonly='readonly' name=ROWID></td>";
                tabl = tabl + "<td>" + rslt.SLOTDESC + "<input type=text id=GIDATE value='" + rslt.SLOTID.ToString() + "'  class='SLOTID hide' readonly='readonly' name=SLOTID></td>";
                tabl = tabl + "</tr>";
                count++;
            }
            tabl = tabl + "</Table>";

            return tabl;

        }
        #endregion

        #region WDetail
        public string WDetail(string id)/*weight update details*/
        {
            var param = id.Split('~');
            var igmno = (param[0]); var gplno = param[1];

            var query = (from r in context.gateindetails.Where(x => x.SDPTID == 1 && x.DISPSTATUS == 0 && x.CONTNRID == 1)
                         join s in context.containersizemasters on r.CONTNRSID equals s.CONTNRSID
                         where (r.IGMNO == igmno)
                         where (r.GPLNO == gplno)
                         select new { r.GIDID, r.GIDATE, r.CONTNRNO, r.STMRNAME, r.IGMNO, r.GPLNO, s.CONTNRSDESC, r.GPWGHT }
                             ).ToList();

            var tabl = "";
            var count = 1;

            foreach (var rslt in query)
            {

                tabl = tabl + "<tr><td>" + count + "</td><td>" + rslt.GIDATE.ToString("dd/MM/yyyy") + "<input type=text id=sno value=0  class='sno hide' readonly='readonly' name=sno style='width:56px'><input type=text id=GIDID value=" + rslt.GIDID + "  class='GIDID hide' readonly='readonly' name=GIDID style='width:56px'></td><td class='col-lg-0'><input type='text' id='CONTNRNO' value='" + rslt.CONTNRNO + "'  class='CONTNRNO hide' readonly='readonly' name='CONTNRNO' >" + rslt.CONTNRNO + "</td><td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class='hide SIZE' name=SIZE style='width:40px' readonly='readonly'>" + rslt.CONTNRSDESC + "</td><td >" + rslt.IGMNO + "<input type=text id=TRANIDATE value='' class='hide TRANIDATE' name=TRANIDATE style='width:70px' readonly='readonly'></td><td >" + rslt.GPLNO + "<input type=text id=TRANSDATE value='' class='hide TRANSDATE' name=TRANSDATE readonly='readonly' style='width:70px'></td><td>" + rslt.STMRNAME + "<input type=text id='TRANEDATE' value='' class='TRANEDATE hide' name='TRANEDATE' style='width:70px' onchange='calculation()'></td><td >" + rslt.GPWGHT + "</td><td><input type=text id='N_GPWGHT' class='form-control N_GPWGHT' name='N_GPWGHT' value=''></td><td class='hidden1'><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS value='' " + count + "  onchange=total() style='width:30px'><input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value=''></td></tr>";
                count++;
            }
            tabl = tabl + "</Table>";

            return tabl;

        }
        #endregion

        #region LINE NO UPDATESAVE 
        public void update(FormCollection F_Form)/*LINE NO UPDATE*/
        {

            using (var trans = context.Database.BeginTransaction())
            {
                try
                {
                    Int32 GIDID = 0;
                    //   TransactionMasterFactor transactionmasterfactors = new TransactionMasterFactor();
                    string[] f_GIDID = F_Form.GetValues("GIDID");
                    string[] boolSTFDIDS = F_Form.GetValues("boolSTFDIDS");



                    for (int count2 = 0; count2 < f_GIDID.Count(); count2++)
                    {

                        GIDID = Convert.ToInt32(f_GIDID[count2]);
                        if (boolSTFDIDS[count2] == "true")
                        {
                            context.Database.ExecuteSqlCommand("UPDATE GATEINDETAIL SET GPLNO = '" + Convert.ToString(F_Form.Get("N_LINENO")) + "' WHERE GIDID=" + GIDID);
                        }


                    }

                    trans.Commit(); Response.Redirect("Index");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Response.Write("Sorry!!An Error Ocurred..." + ex.Message);
                    // Response.Redirect("/Error/AccessDenied");
                }
            }


        }
        #endregion

        #region PRODUCT DETAILS UPDATESAVE
        public void productupdate(FormCollection F_Form)/*PRODUCT DETAILS UPDATE*/
        {

            using (var trans = context.Database.BeginTransaction())
            {
                try
                {
                    Int32 GIDID = 0;
                    //   TransactionMasterFactor transactionmasterfactors = new TransactionMasterFactor();
                    string[] f_GIDID = F_Form.GetValues("GIDID");
                    string[] boolSTFDIDS = F_Form.GetValues("boolSTFDIDS");



                    for (int count2 = 0; count2 < f_GIDID.Count(); count2++)
                    {

                        GIDID = Convert.ToInt32(f_GIDID[count2]);
                        if (boolSTFDIDS[count2] == "true")
                        {
                            context.Database.ExecuteSqlCommand("UPDATE GATEINDETAIL SET PRDTGID=" + Convert.ToInt32(F_Form.Get("PRDTGID")) + ",PRDTDESC='" + Convert.ToString(F_Form.Get("N_PRDTDESC")) + "' WHERE GIDID=" + GIDID);
                        }


                    }

                    trans.Commit(); Response.Redirect("PIndex");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Response.Write("Sorry!!An Error Ocurred..." + ex.Message);
                    // Response.Redirect("/Error/AccessDenied");
                }
            }


        }
        #endregion

        #region WEIGHT UPDATESAVE
        public void weightupdate(FormCollection F_Form)/*WEIGHT UPDATE*/
        {

            using (var trans = context.Database.BeginTransaction())
            {
                try
                {
                    Int32 GIDID = 0;
                    //   TransactionMasterFactor transactionmasterfactors = new TransactionMasterFactor();
                    string[] f_GIDID = F_Form.GetValues("GIDID");
                    string[] boolSTFDIDS = F_Form.GetValues("boolSTFDIDS");
                    string[] N_GPWGHT = F_Form.GetValues("N_GPWGHT");


                    for (int count2 = 0; count2 < f_GIDID.Count(); count2++)
                    {

                        GIDID = Convert.ToInt32(f_GIDID[count2]);
                        if (boolSTFDIDS[count2] == "true")
                        {
                            context.Database.ExecuteSqlCommand("UPDATE GATEINDETAIL SET GPWGHT='" + Convert.ToDecimal(N_GPWGHT[count2]) + "' WHERE GIDID=" + GIDID);
                        }


                    }

                    trans.Commit(); Response.Redirect("WIndex");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Response.Write("Sorry!!An Error Ocurred..." + ex.Message);
                    // Response.Redirect("/Error/AccessDenied");
                }
            }


        }
        #endregion

        #region ROW SOLT DETAILS UPDATE RSIndex
        //[Authorize(Roles = "ImportUpdateDetailsRSIndex")]
        public ActionResult RSIndex()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ViewBag.ROWID = new SelectList(context.rowmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ROWDESC), "ROWID", "ROWDESC", 6);
            ViewBag.SLOTID = new SelectList(context.slotmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.SLOTDESC), "SLOTID", "SLOTDESC", 6);
            return View();
        }
        #endregion

        #region ROW AND SLOT UPDATESAVE
        public void RSUpdate(FormCollection F_Form)/*WEIGHT UPDATE*/
        {

            using (var trans = context.Database.BeginTransaction())
            {
                try
                {
                    Int32 GIDID = 0;
                    //   TransactionMasterFactor transactionmasterfactors = new TransactionMasterFactor();
                    string[] f_GIDID = F_Form.GetValues("GIDID");
                  
                    string[] N_ROWID = F_Form.GetValues("ROWID");
                    string[] N_SLOTID = F_Form.GetValues("SLOTID");
                    if (N_ROWID != null && N_SLOTID != null)
                    {
                        for (int count2 = 0; count2 < N_ROWID.Count(); count2++)
                        {

                            GIDID = Convert.ToInt32(f_GIDID[count2]);

                            context.Database.ExecuteSqlCommand("UPDATE GATEINDETAIL SET ROWID='" + Convert.ToInt32(N_ROWID[count2]) + "' , SLOTID = '" + Convert.ToInt32(N_SLOTID[count2]) + "'  WHERE GIDID=" + GIDID);

                        }
                    }
                    

                    trans.Commit(); Response.Redirect("RSIndex");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Response.Write("Sorry!!An Error Ocurred..." + ex.Message);
                    // Response.Redirect("/Error/AccessDenied");
                }
            }


        }
        #endregion

        #region Trasporter TRANSIndex
        //[Authorize(Roles = "ImportUpdateDetailsTRANSIndex")]
        public ActionResult TRANSIndex()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GateInDetail tab = new GateInDetail();
            
            
            return View(tab);
        }
        #endregion

        #region TRANSDetail
        public string TRANSDetail(string id)/*product update details*/
        {

            var containerno = id;

            //var query = (from r in context.gateindetails.Where(x => x.SDPTID == 1 && x.DISPSTATUS == 0 && x.CONTNRSID > 0)
            //             join s in context.containersizemasters on r.CONTNRSID equals s.CONTNRSID
            //             where (r.CONTNRNO == containerno)
            //             select new { r.GIDID, r.GIDATE, s.CONTNRSDESC, r.VOYNO, r.IMPRTNAME, r.VSLNAME, r.STMRNAME, r.TRNSPRTNAME, r.TRNSPRTID, r.VHLNO, r.VHLMID, r.GPNRNO, r.IGMNO, r.GPLNO, r.CONTNRNO }
            //                 ).ToList();

            //var tabl = "";
            //var count = 1;

            int compyid = Convert.ToInt32(Session["compyid"]);

            var query = context.Database.SqlQuery<pr_SSRCharge_Container_Detail_Assgn_Result>("EXEC pr_SSRCharge_Container_Detail_Assgn @PContnrNo ='" + containerno + "'").ToList();
            var tabl = "";
            var count = 1;

            foreach (var rslt in query)
            {
                tabl = tabl + "<Table>";
                tabl = tabl + "<tr><td>" + count + "</td>";
                tabl = tabl + "<td>" + rslt.GIDATE.ToString("dd/MM/yyyy") + "<input type=text id=GIDID value=" + rslt.GIDID + "  class='GIDID hide' readonly='readonly' name=GIDID style='width:56px'></td>";
                tabl = tabl + "<td>" + rslt.CONTNRSDESC + "</td>";
                tabl = tabl + "<td>" + rslt.IGMNO + "</td>";
                tabl = tabl + "<td >" + rslt.GPLNO + "</td>";
                tabl = tabl + "<td >" + rslt.CONTNRNO + "</td>";
                tabl = tabl + "<td >" + rslt.VOYNO + "</td>";
                tabl = tabl + "<td >" + rslt.VSLNAME + "</td>";
                tabl = tabl + "<td >" + rslt.IMPRTNAME + "</td>";
                tabl = tabl + "<td>" + rslt.STMRNAME + "</td>";
                tabl = tabl + "<td>" + rslt.TRNSPRTNAME + "<input type=text id=TRNSPRTID value='" + rslt.TRNSPRTID.ToString() + "'  class='TRNSPRTID hide' readonly='readonly' name=TRNSPRTID></td>";
                tabl = tabl + "<td>" + rslt.VHLNO + "</td>";
                tabl = tabl + "<td>" + rslt.GPNRNO + "</td>";
                tabl = tabl + "</tr>";
                count++;
            }
            tabl = tabl + "</Table>";

            return tabl;

        }
        #endregion

        #region TransSave
        public ActionResult TransUpdate(GateInDetail tab)/*WEIGHT UPDATE*/
        {
            string status = "";
            using (var trans = context.Database.BeginTransaction())
            {
                try
                {
                    Int32 GIDID = 0;
                    //   TransactionMasterFactor transactionmasterfactors = new TransactionMasterFactor();
                    string f_GIDID = Convert.ToString(tab.GIDID);
                    string TRNSPRTNAME = Convert.ToString(tab.TRNSPRTNAME);
                    string TRNSPRTID = Convert.ToString(tab.TRNSPRTID);
                    string GTRNSPRTNAME = Convert.ToString(tab.GTRNSPRTNAME);
                    string VHLNO = Convert.ToString(tab.VHLNO);
                    string GPNRNO = Convert.ToString(tab.GPNRNO);
                    string VHLMID = Convert.ToString(tab.VHLMID);

                    if (f_GIDID != null)
                    {
                        GIDID = Convert.ToInt32(f_GIDID);

                        if (VHLMID == null || VHLMID == "0")
                        {
                            VHLMID = "0";
                        }

                        if (TRNSPRTID == null || TRNSPRTID == "0")
                        {
                            TRNSPRTID = "0";
                        }
                        string uquery = "UPDATE GATEINDETAIL SET TRNSPRTNAME='" + Convert.ToString(TRNSPRTNAME) + "',";
                        uquery += "TRNSPRTID=" + Convert.ToInt32(TRNSPRTID) + ",GTRNSPRTNAME='" + Convert.ToString(GTRNSPRTNAME) + "',";
                        uquery += "VHLNO='" + Convert.ToString(VHLNO) + "',GPNRNO='" + Convert.ToString(GPNRNO) + "',";
                        uquery += "VHLMID=" + Convert.ToInt32(VHLMID) + " WHERE GIDID=" + GIDID + " ";
                        context.Database.ExecuteSqlCommand(uquery);


                    }

                    status = "saved";
                    trans.Commit(); //Response.Redirect("TRANSIndex");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Response.Write("Sorry!!An Error Ocurred..." + ex.Message);
                    // Response.Redirect("/Error/AccessDenied");
                }
            }
            return Json(status, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region SSR CHARGES Index
        //[Authorize(Roles = "ImportUpdateDetailsRSIndex")]
        public ActionResult SSCIndex()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            List<SelectListItem> selectedGPETYPE = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "NO", Value = "0", Selected = false };
            selectedGPETYPE.Add(selectedItem);
            selectedItem = new SelectListItem
            {
                Text = "YES",
                Value = "1",
                Selected = false
            };
            selectedGPETYPE.Add(selectedItem);
            ViewBag.GPETYPE = selectedGPETYPE;
            return View();
        }
        #endregion

        #region SSRCHARGES Details
        public string SSRChargeDetail(string id)/*product update details*/
        {

            var containerno = id;
            int compyid = Convert.ToInt32(Session["compyid"]);
            //var query = (from r in context.gateindetails.Where(x => x.SDPTID == 1 && x.DISPSTATUS == 0 && x.CONTNRSID > 0 && x.COMPYID == compyid)
            //             join s in context.containersizemasters on r.CONTNRSID equals s.CONTNRSID
                         
            //             where (r.CONTNRNO == containerno)
            //             select new { r.GIDID, r.GIDATE, s.CONTNRSDESC, r.VOYNO, r.IMPRTNAME, r.VSLNAME, r.STMRNAME, r.IGMNO, r.GPLNO,r.GPETYPE, r.GPEAMT, r.GPAAMT, r.CONTNRNO }
            //                 ).ToList();

            var query = context.Database.SqlQuery<pr_SSRCharge_Container_Detail_Assgn_Result>("EXEC pr_SSRCharge_Container_Detail_Assgn @PContnrNo ='" + containerno + "'").ToList();
            var tabl = "";
            var count = 1;

            foreach (var rslt in query)
            {

                string gpetype = "";
                if(rslt.GPETYPE==0)
                {
                    gpetype = "No";
                }
                else
                {
                    gpetype = "Yes";
                }

                tabl = tabl + "<Table>";
                tabl = tabl + "<tr><td>" + count + "</td>";
                tabl = tabl + "<td>" + rslt.GIDATE.ToString("dd/MM/yyyy") + "<input type=text id='SGIDID' value=" + rslt.GIDID + "  class='SGIDID hide' readonly='readonly' name='SGIDID' style='width:56px'></td>";
                tabl = tabl + "<td>" + rslt.CONTNRSDESC + "</td>";
                tabl = tabl + "<td>" + rslt.IGMNO + "</td>";
                tabl = tabl + "<td >" + rslt.GPLNO + "</td>";
                tabl = tabl + "<td >" + rslt.CONTNRNO + "</td>";
                tabl = tabl + "<td >" + rslt.VOYNO + "</td>";
                tabl = tabl + "<td >" + rslt.VSLNAME + "</td>";
                tabl = tabl + "<td >" + rslt.IMPRTNAME + "</td>";
                tabl = tabl + "<td>" + rslt.STMRNAME + "</td>";
                tabl = tabl + "<td>" + gpetype + "</td>";
                tabl = tabl + "<td>" + rslt.GPEAMT + "</td>";
                tabl = tabl + "<td>" + rslt.GPAAMT + "</td>";
                tabl = tabl + "</tr>";
                count++;
            }
            tabl = tabl + "</Table>";

            return tabl;

        }
        #endregion

        #region SSRCHargesUpdate
        public ActionResult SSRCHargesUpdate(GateInDetail tab)/*WEIGHT UPDATE*/
        {
            string status = "";
            using (var trans = context.Database.BeginTransaction())
            {
                try
                {
                    Int32 GIDID = 0;
                    //   TransactionMasterFactor transactionmasterfactors = new TransactionMasterFactor();
                    string f_GIDID = Convert.ToString(tab.GIDID);
                    string GPETYPE = Convert.ToString(tab.GPETYPE);
                    string GPEAMT = Convert.ToString(tab.GPEAMT);
                    string GPAAMT = Convert.ToString(tab.GPAAMT);


                    if (f_GIDID != null)
                    {
                        GIDID = Convert.ToInt32(f_GIDID);

                        if (GPEAMT == null || GPEAMT == "")
                        {
                            GPEAMT = "0";
                        }

                        if (GPAAMT == null || GPAAMT == "")
                        {
                            GPAAMT = "0";
                        }
                        string uquery = "UPDATE GATEINDETAIL SET GPETYPE='" + Convert.ToString(GPETYPE) + "',";
                        uquery += "GPEAMT=" + Convert.ToDecimal(GPEAMT) + ",GPAAMT='" + Convert.ToDecimal(GPAAMT) + "' ";
                        uquery += " WHERE GIDID=" + GIDID + " ";
                        context.Database.ExecuteSqlCommand(uquery);


                    }

                    status = "saved";
                    trans.Commit(); //Response.Redirect("TRANSIndex");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Response.Write("Sorry!!An Error Ocurred..." + ex.Message);
                    // Response.Redirect("/Error/AccessDenied");
                }
            }
            return Json(status, JsonRequestBehavior.AllowGet);

        }
        #endregion
    }
}