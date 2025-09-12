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
using scfs_erp;

namespace scfs.Controllers.Import
{
    [SessionExpire]
    public class PreCHAUpdateController : Controller
    {
        // GET: PreCHAUpdate
        #region Context declaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region Index 
        //[Authorize(Roles = "ImporterUpdateIndex")]

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
            //   ViewBag.STMRID = new SelectList(context.categorymasters.Where(x => x.CATETID == 3 && x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME");
            return View();
        }
        #endregion

        #region Form
        public ActionResult Form()
        {
            return View();
        }
        #endregion

        #region Autocha
        public JsonResult AutoCHA(string term)/*model2.edmx*/
        {

            var result = (from r in context.categorymasters.Where(x => x.CATETID == 4 && x.DISPSTATUS == 0)
                          where r.CATENAME.StartsWith(term)
                          select new { r.CATENAME, r.CATEID }).Distinct().OrderBy(x => x.CATENAME);
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Detail
        public string Detail(string id)
        {
            var param = id.Split('~');
            var igmno = (param[0]); var gplno = param[1];

            var query = (from r in context.gateindetails.Where(x => x.SDPTID == 1 && x.DISPSTATUS == 0 && x.CONTNRID == 1)
                         join s in context.containersizemasters on r.CONTNRSID equals s.CONTNRSID
                         where (r.IGMNO == igmno)
                         where (r.GPLNO == gplno)
                         select new { r.GIDID, r.GIDATE, r.CONTNRNO, r.STMRNAME, r.IGMNO, r.GPLNO, s.CONTNRSDESC, r.PRE_CHANAME, r.IMPRTNAME }
                             ).ToList();

            //  context.Database.SqlQuery<GateInDetail>("select * GateInDetail where SDPTID=1 and DISPSTATUS=0 and CONTNRID=1 where IGMNO='" + igmno + "' and GPLNO='" + gplno + "'").ToList();


            var tabl = "";
            var count = 1;

            foreach (var rslt in query)
            {

                tabl = tabl + "<tr><td>" + count + "</td><td>" + rslt.GIDATE.ToString("dd/MM/yyyy") + "<input type=text id=sno value=0  class='sno hide' readonly='readonly' name=sno style='width:56px'><input type=text id=GIDID value=" + rslt.GIDID + "  class='GIDID hide' readonly='readonly' name=GIDID style='width:56px'></td><td class='col-lg-0'><input type='text' id='CONTNRNO' value='" + rslt.CONTNRNO + "'  class='CONTNRNO hide' readonly='readonly' name='CONTNRNO' >" + rslt.CONTNRNO + "</td><td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class='hide SIZE' name=SIZE style='width:40px' readonly='readonly'>" + rslt.CONTNRSDESC + "</td><td >" + rslt.PRE_CHANAME + "<input type=text id=TRANIDATE value='' class='hide TRANIDATE' name=TRANIDATE style='width:70px' readonly='readonly'></td><td class='hide'>" + rslt.GPLNO + "<input type=text id=TRANSDATE value='' class='hide TRANSDATE' name=TRANSDATE readonly='readonly' style='width:70px'></td><td>" + rslt.IMPRTNAME + "<input type=text id='TRANEDATE' value='' class='TRANEDATE hide' name='TRANEDATE' style='width:70px' onchange='calculation()'></td><td class='hidden1'><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS value='' " + count + "  onchange=total() style='width:30px'><input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value=''></td></tr>";
                count++;
            }
            tabl = tabl + "</Table>";

            return tabl;

        }
        #endregion

        #region Save
        public void savedata(FormCollection F_Form)
        {
            using (SCFSERPContext context = new SCFSERPContext())
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

                                context.Database.ExecuteSqlCommand("UPDATE GATEINDETAIL SET PRE_CHAID = " + F_Form.Get("TRANREFID") + ",PRE_CHANAME = '" + F_Form.Get("TRANREFNAME") + "' WHERE GIDID=" + GIDID);
                            }

                        }

                        trans.Commit(); Response.Redirect("/PreCHAUpdate/Form");
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Response.Write("Sorry!!An Error Ocurred..." + ex.Message);
                        // Response.Redirect("/Error/AccessDenied");
                    }
                }
            }

        }
        #endregion

        #region GetCHA
        public JsonResult GetCHA(string term)/*model2.edmx*/
        {

            var result = (from r in context.categorymasters.Where(x => x.CATETID == 4 && x.DISPSTATUS == 0)

                          select new { r.CATEID, r.CATENAME }).Distinct().OrderBy(x => x.CATENAME);
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion
    }
}