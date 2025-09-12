
using scfs.Data;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers.NonPnr
{
    [SessionExpire]
    public class NonPnrDeStuffAuthorizationSlipController : Controller
    {
        // GET: NonPnrDeStuffAuthorizationSlip

        SCFSERPContext context = new SCFSERPContext();

        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

        [Authorize(Roles = "NonPnrDeStuffSlipIndex")]
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

            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_NonPnr_DeStuffSlip(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));

                var aaData = data.Select(d => new string[] { d.ASLMDATE.Value.ToString("dd/MM/yyyy"), d.ASLMDNO, d.CONTNRNO, d.CONTNRSDESC, d.CHANAME, d.BOENO, d.IGMNO.ToString(), d.GPLNO.ToString() ,  d.DISPSTATUS.ToString(), d.ASLMID.ToString(), d.ASLDID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "NonPnrDeStuffSlipEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/NonPnrDeStuffAuthorizationSlip/Form/" + id);

            //Response.Redirect("/NonPnrDeStuffAuthorizationSlip/Form/" + id);
        }

        //[Authorize(Roles = "NonPnrDeStuffSlipCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            AuthorizationSlipMD vm = new AuthorizationSlipMD();
            AuthorizationSlipMaster tab = new AuthorizationSlipMaster();

            ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME");
            ViewBag.ASLOTYPE = new SelectList(context.ImportDestuffSlipOperation, "OPRTYPE", "OPRTYPEDESC");



            //-----------------------------stuff -TYPE-------------------------
            List<SelectListItem> selectedDOTYPE = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "MANUAL", Value = "0", Selected = true };
            selectedDOTYPE.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "MECHANICAL", Value = "1", Selected = false };
            selectedDOTYPE.Add(selectedItem1);
            ViewBag.ASLDTYPE = selectedDOTYPE;

            //------------------------------Labour Type-------------------------
            List<SelectListItem> selectedScut = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "SANCO", Value = "0", Selected = false };
            selectedScut.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "CHA", Value = "1", Selected = true };
            selectedScut.Add(selectedItem);
            ViewBag.ASLLTYPE = selectedScut;



            //-------------------------------DISPSTATUS----

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItemDSP);
            //  selectedItemDSP = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = false };
            //  selectedDISPSTATUS.Add(selectedItemDSP);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;


            //-------------opensheetdetails---------------

            if (id != 0)
            {
                tab = context.authorizatioslipmaster.Find(id);//find selected record

                vm.masterdata = context.authorizatioslipmaster.Where(det => det.ASLMID == id).ToList();
                vm.detaildata = context.authorizationslipdetail.Where(det => det.ASLMID == id).ToList();
                //ViewBag.ModifyField = DetailEdit(id);
                ViewBag.GPLNO = "";
                ViewBag.IGMNO = "";
                if (vm.detaildata[0].GIDID > 0)
                {
                    var sqry = context.Database.SqlQuery<GateInDetail>("select *from GateInDetail where GIDID =" + vm.detaildata[0].GIDID).ToList();

                    ViewBag.GPLNO = sqry[0].GPLNO;
                    ViewBag.IGMNO = sqry[0].IGMNO;
                }


                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem3 = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem3);
                    selectedItem3 = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem3);

                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
                else
                {
                    SelectListItem selectedItem3 = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem3);
                    selectedItem3 = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem3);

                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
                //---------Dropdown lists-------------------
                ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME");
                //vm.destuffdata = context.Database.SqlQuery<PR_IMPORT_AUTHORIZATION_DETAIL_CTRL_ASSGN_Result>("EXEC PR_IMPORT_AUTHORIZATION_DETAIL_CTRL_ASSGN @PASLMID=" + id).ToList();
                vm.nonpnrdestuffdata= context.Database.SqlQuery<PR_NONPNR_AUTHORIZATION_DETAIL_CTRL_ASSGN_Result>("EXEC PR_NONPNR_AUTHORIZATION_DETAIL_CTRL_ASSGN @PASLMID=" + id).ToList();
                //--------End of dropdown
               

            }//---End Of IF


            return View(vm);
        }


        public void savedata(FormCollection F_Form)
        {

            AuthorizationSlipMaster authorizatioslipmaster = new AuthorizationSlipMaster();
            AuthorizationSlipDetail authorizatioslipdetail = new AuthorizationSlipDetail();
            //-------Getting Primarykey field--------
            Int32 ASLMID = Convert.ToInt32(F_Form["masterdata[0].ASLMID"]);
            Int32 ASLDID = 0;

            string todaydt = Convert.ToString(DateTime.Now);
            string todayd = Convert.ToString(DateTime.Now.Date);

            string DELIDS = "";
            //-----End

            authorizatioslipmaster.COMPYID = Convert.ToInt32(Session["compyid"]);
            authorizatioslipmaster.SDPTID = 9;
            authorizatioslipmaster.ASLMTYPE = 2;

            if (ASLMID == 0)
            {
                authorizatioslipmaster.CUSRID = Session["CUSRID"].ToString();
            }
            authorizatioslipmaster.LMUSRID = Session["CUSRID"].ToString();
            authorizatioslipmaster.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
            authorizatioslipmaster.PRCSDATE = DateTime.Now;

            string indate = Convert.ToString(F_Form["masterdata[0].ASLMDATE"]);
            if (indate != null || indate != "")
            {
                authorizatioslipmaster.ASLMDATE = Convert.ToDateTime(indate).Date;
            }
            else { authorizatioslipmaster.ASLMDATE = DateTime.Now.Date; }

            if (authorizatioslipmaster.ASLMDATE > Convert.ToDateTime(todayd))
            {
                authorizatioslipmaster.ASLMDATE = Convert.ToDateTime(todayd);
            }

            string intime = Convert.ToString(F_Form["masterdata[0].ASLMTIME"]);
            if ((intime != null || intime != "") && ((indate != null || indate != "")))
            {
                if ((intime.Contains(' ')) && (indate.Contains(' ')))
                {
                    var in_time = intime.Split(' ');
                    var in_date = indate.Split(' ');

                    if ((in_time[1].Contains(':')) && (in_date[0].Contains('/')))
                    {

                        var in_time1 = in_time[1].Split(':');
                        var in_date1 = in_date[0].Split('/');

                        string in_datetime = in_date1[2] + "-" + in_date1[1] + "-" + in_date1[0] + "  " + in_time1[0] + ":" + in_time1[1] + ":" + in_time1[2];

                        authorizatioslipmaster.ASLMTIME = Convert.ToDateTime(in_datetime);
                    }
                    else { authorizatioslipmaster.ASLMTIME = DateTime.Now; }
                }
                else { authorizatioslipmaster.ASLMTIME = DateTime.Now; }
            }
            else { authorizatioslipmaster.ASLMTIME = DateTime.Now; }

            if (authorizatioslipmaster.ASLMTIME > Convert.ToDateTime(todaydt))
            {
                authorizatioslipmaster.ASLMTIME = Convert.ToDateTime(todaydt);
            }

            //authorizatioslipmaster.ASLMDATE = Convert.ToDateTime(F_Form["masterdata[0].ASLMTIME"]).Date;
            //authorizatioslipmaster.ASLMTIME = Convert.ToDateTime(F_Form["masterdata[0].ASLMTIME"]);

            if (ASLMID != 0)
            {
                authorizatioslipmaster = context.authorizatioslipmaster.Find(ASLMID);
            }

           
            //authorizatioslipmaster.ASLMLTYPE = 0;
            if (ASLMID == 0)
            {

                authorizatioslipmaster.ASLMNO = Convert.ToInt32(Autonumber.autonum("AUTHORIZATIONSLIPMASTER", "ASLMNO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " AND SDPTID=9 AND ASLMTYPE=2").ToString());

                int ano = authorizatioslipmaster.ASLMNO;
                string prfx = string.Format("{0:D5}", ano);
                authorizatioslipmaster.ASLMDNO = prfx.ToString();
                context.authorizatioslipmaster.Add(authorizatioslipmaster);
                context.SaveChanges();
            }
            else
            {
                context.Entry(authorizatioslipmaster).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }


            //-------------Shipping Bill Details
            string[] F_ASLDID = F_Form.GetValues("ASLDID");
            string[] F_booltype = F_Form.GetValues("booltype");
            string[] OSDID = F_Form.GetValues("OSDID");
            string[] GIDID = F_Form.GetValues("GIDID");
            string LCATEID = F_Form["detaildata[0].LCATEID"];
            string ASLDTYPE = F_Form["detaildata[0].ASLDTYPE"];
            string ASLLTYPE = F_Form["detaildata[0].ASLLTYPE"];
            string ASLOTYPE = F_Form["detaildata[0].ASLOTYPE"];
            //string[] CSEALNO = F_Form.GetValues("CSEALNO");
            //string[] ASEALNO = F_Form.GetValues("ASEALNO");
            string booltype = "";

            for (int count = 0; count < F_ASLDID.Count(); count++)
            {
                ASLDID = Convert.ToInt32(F_ASLDID[count]);
                booltype = F_booltype[count].ToString();
                if (ASLDID != 0)
                {

                    authorizatioslipdetail = context.authorizationslipdetail.Find(ASLDID);


                }
                if (booltype == "true")
                {
                    authorizatioslipdetail.ASLMID = authorizatioslipmaster.ASLMID;
                    authorizatioslipdetail.OSDID = Convert.ToInt32(OSDID[count]);
                    authorizatioslipdetail.GIDID = Convert.ToInt32(GIDID[count]);
                    authorizatioslipdetail.LCATEID = Convert.ToInt32(LCATEID);
                    authorizatioslipdetail.ASLDTYPE = Convert.ToInt16(ASLDTYPE);
                    authorizatioslipdetail.ASLLTYPE = Convert.ToInt16(ASLLTYPE);
                    authorizatioslipdetail.ASLOTYPE = Convert.ToInt16(ASLOTYPE);
                    authorizatioslipdetail.STFDID = 0;
                    authorizatioslipdetail.VHLNO = null;
                    authorizatioslipdetail.DRVNAME = null;                    
                    authorizatioslipdetail.ASLDODATE = null;
                    authorizatioslipdetail.DISPSTATUS = 0;
                    authorizatioslipdetail.PRCSDATE = DateTime.Now;

                    if (ASLDID == 0)
                    {
                        context.authorizationslipdetail.Add(authorizatioslipdetail);
                        context.SaveChanges();
                        ASLDID = authorizatioslipdetail.ASLDID;
                    }
                    else
                    {
                        context.Entry(authorizatioslipdetail).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }

                    DELIDS = DELIDS + "," + ASLDID.ToString();
                }
            }
            // context.Database.ExecuteSqlCommand("DELETE FROM authorizationslipdetail  WHERE ASLMID=" + ASLMID + " and  ASLDID NOT IN(" + DELIDS.Substring(1) + ")");
            Response.Redirect("Index");
        }

        public JsonResult AutoIGMNO(string term)
        {
            var result = (from view in context.gateindetails
                          where view.IGMNO.ToLower().Contains(term.ToLower())
                          select new { view.IGMNO }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AutoGINO(int term)
        {
            var result = (from view in context.gateindetails
                          where view.GINO == Convert.ToInt32(term)
                          select new { view.GINO }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void Detail(string id)
        {
            var param = id.Split(';');
            var PIGMNO = (param[0]);
            var PGPLNO = (param[1]);

            var query = context.Database.SqlQuery<VW_NONPNRASD_GIGMNO_CONTAINER_CBX_ASSGN>("select * from VW_NONPNRASD_GIGMNO_CONTAINER_CBX_ASSGN WHERE IGMNO='" + PIGMNO + "' and GPLNO='" + PGPLNO + "'").ToList();


            var tabl = " <div class='panel-heading navbar-inverse'  style=color:white>Container Details</div><Table id=mytabl class='table table-striped table-bordered bootstrap-datatable'> <thead><tr> <th></th><th>Container No</th><th> Size</th><th>In Date</th><th>Gate In No</th><th>BOE No</th><th>BOE Date</th><th>DODate</th><th>Liner Seal No</th><th>SANCO Seal No</th> </tr> </thead>";

            foreach (var rslt in query)
            {
                if (rslt.LPSEALNO == null) rslt.LPSEALNO = "-";
                if (rslt.CSEALNO == null) rslt.CSEALNO = "-";

                tabl = tabl + "<tbody><tr><td><input type='checkbox' name='CHKBX' class='CHKBX' id='CHKBX' onchange='SelectedCont(this)' />";
                tabl = tabl + "<input type=text name='booltype' class='booltype hidden' /></td>";
                tabl = tabl + "<td class=hide><input type='text' id='OSDID' class='OSDID' name='OSDID' value=0></td>";
                tabl = tabl + "<td class=hide><input type='text' id='GIDID' class='GIDID' name='GIDID' value='" + rslt.GIDID + "'></td>";
                tabl = tabl + "<td class=hide><input type='text' id='ASLDID' value='0'  class='ASLDID' name='ASLDID' hidden></td>";
                tabl = tabl + "<td class=hide><input type='text' id='STMRNAME' value='" + rslt.STMRNAME + "' class='STMRNAME' name='STMRNAME'></td>";
                tabl = tabl + "<td class=hide><input type='text' id='IMPRTNAME' value='" + rslt.IMPRTNAME + "' class='IMPRTNAME' name='IMPRTNAME' hidden></td>";
                tabl = tabl + "<td class=hide><input type='text' id='VOYNO' value='" + rslt.VOYNO + "' class='VOYNO' name='VOYNO' hidden></td>";
                tabl = tabl + "<td class=hide><input type='text' id='VSLNAME' value='" + rslt.VSLNAME + "' class='VSLNAME' name='VSLNAME' hidden></td>";
                tabl = tabl + "<td class='col-lg-3'><input type='text' id='CONTNRNO' value='" + rslt.CONTNRNO + "' class='form-control CONTNRNO' name='CONTNRNO' readonly='readonly'></td>";
                tabl = tabl + "<td class='col-lg-1'><input type='text' value='" + rslt.CONTNRSDESC + "' id='CONTNRSDESC' class='CONTNRSDESC form-control' name='CONTNRSDESC' readonly='readonly'></td>";
                tabl = tabl + "<td class='col-lg-2'><input type='text' id='GIDATE' value='" + rslt.GIDATE.ToString("dd/MM/yyyy") + "' class='GIDATE form-control datepicker' name='GIDATE' readonly='readonly'></td>";
                tabl = tabl + "<td class='col-lg-2'><input type='text' id='GIDNO'  class='form-control GIDNO' name=GIDNO value='" + rslt.GIDNO + "' readonly='readonly'></td>";
                tabl = tabl + "<td class='col-lg-2'><input type='text' id='BOENO'  class='form-control BOENO' name=BOENO value='" + rslt.BOENO + "'  readonly='readonly'></td>";
                tabl = tabl + "<td class='col-lg-2'><input type='text' id='BOEDATE'  class='form-control BOEDATE datepicker' name=BOEDATE value='" + rslt.BOEDATE.Value.ToString("dd/MM/yyyy") + "' readonly='readonly'></td>";
                tabl = tabl + "<td class='col-lg-2'><input type='text' id='DODATE'  class='form-control DODATE datepicker' name=DODATE  readonly style='width:89px' value=''></td>";
                tabl = tabl + "<td class='col-lg-2'><input type='text' value='" + rslt.LPSEALNO + "' id='LSEALNO' class='form-control LSEALNO' name='LSEALNO' readonly='readonly' style='width:65px'></td>";
                tabl = tabl + "<td class='col-lg-2'><input type='text' value='" + rslt.CSEALNO + "' id='SSEALNO'  class='form-control SSEALNO' name='SSEALNO' readonly='readonly' style='width:65px'></td>";
                tabl = tabl + "</tr></tbody>";

            }
            tabl = tabl + "</Table>";
            Response.Write(tabl);
        }


        public JsonResult GetOperation()
        {
            var result = context.Database.SqlQuery<ImportDestuffSlipOperation>("select * from IMPORT_DESTUFF_SLIP_OPERATION_TYPE").ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //..........................Printview...
        [Authorize(Roles = "NonPnrDeStuffSlipPrint")]
        public void PrintView(int? id = 0)
        {

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "Destuff_Slip", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;


                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] +  "NonPnr_Destuff_Slip.RPT");

                cryRpt.RecordSelectionFormula = "{VW_NONPNR_ADSLIP_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_NONPNR_ADSLIP_CRY_PRINT_ASSGN.ASLMID} = " + id;



                crConnectionInfo.ServerName = stringbuilder.DataSource;
                crConnectionInfo.DatabaseName = stringbuilder.InitialCatalog;
                crConnectionInfo.UserID = stringbuilder.UserID;
                crConnectionInfo.Password = stringbuilder.Password;

                CrTables = cryRpt.Database.Tables;
                foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
                {
                    crtableLogoninfo = CrTable.LogOnInfo;
                    crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                    CrTable.ApplyLogOnInfo(crtableLogoninfo);
                }


                cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                cryRpt.Dispose();
                cryRpt.Close();
            }

        }
        //end

        [Authorize(Roles = "NonPnrDeStuffSlipCancel")]
        public void Cancel()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");

            //   var param = id.Split('-');

            String temp = Delete_fun.delete_check1(fld, id);

            if (temp.Equals("PROCEED"))
            {
                AuthorizationSlipMaster authorizatioslipmaster = context.authorizatioslipmaster.Find(Convert.ToInt32(id));
                context.Entry(authorizatioslipmaster).Entity.DISPSTATUS = 1;
                context.SaveChanges();
                Response.Write("Cancelled successfully...");
            }
            else
                Response.Write(temp);
        }//..End of delete

        [Authorize(Roles = "NonPnrDeStuffSlipDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");

            //   var param = id.Split('-');

            String temp = Delete_fun.delete_check1(fld, id);

            if (temp.Equals("PROCEED"))
            {
                AuthorizationSlipMaster authorizatioslipmaster = context.authorizatioslipmaster.Find(Convert.ToInt32(id));
                context.authorizatioslipmaster.Remove(authorizatioslipmaster);
                context.SaveChanges();
                Response.Write("Deleted successfully...");
            }
            else
                Response.Write(temp);
        }//..End of delete

        [Authorize(Roles = "NonPnrDeStuffSlipDelete")]
        public void Del_det()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");

            var param = id.Split('-');

            String temp = Delete_fun.delete_check1(fld, param[1]);

            if (temp.Equals("PROCEED"))
            {
                string temp1 = Delete_fun.delete_check1("IMPORTAUTHSLIP", param[0]);

                if (temp1.Equals("PROCEED"))
                {
                    AuthorizationSlipMaster authorizatioslipmaster = context.authorizatioslipmaster.Find(Convert.ToInt32(param[0]));
                    context.authorizatioslipmaster.Remove(authorizatioslipmaster);
                    context.SaveChanges();
                    Response.Write("Deleted successfully...");
                }
                else
                {
                    AuthorizationSlipDetail authorizationslipdetail = context.authorizationslipdetail.Find(Convert.ToInt32(param[1]));
                    context.authorizationslipdetail.Remove(authorizationslipdetail);
                    context.SaveChanges();
                    Response.Write("Deleted successfully...");
                }
            }
            else
                Response.Write(temp);
        }//..End of delete

    }
}