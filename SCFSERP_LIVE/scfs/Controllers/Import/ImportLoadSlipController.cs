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
using scfs.Data;

namespace scfs_erp.Controllers.Import
{
    [SessionExpire]
    public class ImportLoadSlipController : Controller
    {
        // GET: ImportLoadSlip
        #region Context declaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region Index Page
        [Authorize(Roles = "ImportLoadSlipIndex")]
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

            
            DateTime fromdate = Convert.ToDateTime(Session["SDATE"]).Date;
            DateTime todate = Convert.ToDateTime(Session["EDATE"]).Date;


            TotalContainerDetails(fromdate, todate);

            return View();
        }
        #endregion

        #region  TotalContainerDetails
        public JsonResult TotalContainerDetails(DateTime fromdate, DateTime todate)
        {
            string fdate = ""; string tdate = ""; int sdptid = 1;
            if (fromdate == null)
            {
                fromdate = DateTime.Now.Date;
                fdate = Convert.ToString(fromdate);
            }
            else
            {
                string infdate = Convert.ToString(fromdate);
                var in_date = infdate.Split(' ');
                var in_date1 = in_date[0].Split('/');
                fdate = Convert.ToString(in_date1[2] + "-" + in_date1[1] + "-" + in_date1[0]);
            }
            if (todate == null)
            {
                todate = DateTime.Now.Date;
                tdate = Convert.ToString(todate);
            }
            else
            {
                string intdate = Convert.ToString(todate);

                var in_date1 = intdate.Split(' ');
                var in_date2 = in_date1[0].Split('/');
                tdate = Convert.ToString(in_date2[2] + "-" + in_date2[1] + "-" + in_date2[0]);

            }


            var result = context.Database.SqlQuery<PR_IMPORT_LOADESTUFFTOTCONTAINER_DETAILS_Result>("EXEC PR_IMPORT_LOADESTUFFTOTCONTAINER_DETAILS @PFDT='" + fdate + "',@PTDT='" + tdate + "',@PSDPTID=" + 1).ToList();

            foreach (var rslt in result)
            {
                if ((rslt.Sno == 1) && (rslt.Descriptn == "IMPORT - LOADSLIP"))
                {
                    @ViewBag.Total20 = rslt.c_20;
                    @ViewBag.Total40 = rslt.c_40;
                    @ViewBag.Total45 = rslt.c_45;
                    @ViewBag.TotalTues = rslt.c_tues;

                    Session["IL20"] = rslt.c_20;
                    Session["IL40"] = rslt.c_40;
                    Session["IL45"] = rslt.c_45;
                    Session["ILTU"] = rslt.c_tues;
                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Get data from database
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {

            using (var e = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Import_LoadSlip(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));

                var aaData = data.Select(d => new string[] { d.ASLMDATE.Value.ToString("dd/MM/yyyy"), d.ASLMDNO, d.CONTNRNO, d.CONTNRSDESC, d.CHANAME, d.BOENO, d.IGMNO, d.GPLNO, d.DISPSTATUS.ToString(), d.ASLMID.ToString(), d.DOSTS }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Form Data
        [Authorize(Roles = "ImportLoadSlipEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ImportLoadSlip/Form/" + id);
        }
        //..............form data..............//
        [Authorize(Roles = "ImportLoadSlipCreate")]
        public ActionResult Form(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            AuthorizationSlipMD vm = new AuthorizationSlipMD();
            AuthorizationSlipMaster tab = new AuthorizationSlipMaster();

            ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME");

            //-------------------------------DISPSTATUS----
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItemDSP);
            //  selectedItemDSP = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = false };
            //  selectedDISPSTATUS.Add(selectedItemDSP);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;



            if (id != 0)
            {
                tab = context.authorizatioslipmaster.Find(id);//find selected record

                vm.masterdata = context.authorizatioslipmaster.Where(det => det.ASLMID == id).ToList();
                vm.detaildata = context.authorizationslipdetail.Where(det => det.ASLMID == id).ToList();

                GateInDetail gitab = new GateInDetail();
                gitab = context.gateindetails.Find(Convert.ToInt32(vm.detaildata[0].GIDID));
                var oocdtry = context.Database.SqlQuery<VW_IMPORT_IGMNO_CONTAINER_CBX_ASSGN>("select * from VW_IMPORT_IGMNO_CONTAINER_CBX_ASSGN_01(nolock) where IGMNO='" + gitab.IGMNO.ToString() + "' and GPLNO='" + gitab.GPLNO.ToString() + "'").ToList();
                if(oocdtry != null)
                {
                    if (oocdtry[0].OOCDATE != null && oocdtry[0].OOCDATE != "")
                        ViewBag.OOCDATE = Convert.ToDateTime(oocdtry[0].OOCDATE).ToString("dd/MM/yyyy");
                }
                

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
                vm.destuffdata = context.Database.SqlQuery<PR_IMPORT_AUTHORIZATION_DETAIL_CTRL_ASSGN_Result>("EXEC PR_IMPORT_AUTHORIZATION_DETAIL_CTRL_ASSGN @PASLMID=" + id).ToList();

            }
            return View(vm);
        }
        #endregion

        #region Detail Data
        public void Detail(string id)
        {
            var param = id.Split(';');
            var PIGMNO = (param[0]);
            var PGPLNO = (param[1]);
            
            var query = context.Database.SqlQuery<VW_IMPORT_IGMNO_CONTAINER_CBX_ASSGN>("select * from VW_IMPORT_IGMNO_CONTAINER_CBX_ASSGN(nolock) where IGMNO='" + PIGMNO + "' and GPLNO='" + PGPLNO + "'").ToList();
            var tabl = " <div class='panel-heading navbar-inverse'  style=color:white>Container Details</div><Table id=mytabl class='table table-striped table-bordered bootstrap-datatable'> <thead><tr> <th></th><th>Container No</th><th> Size</th><th>In Date</th><th>OpenSheet No</th><th>BOE No</th><th>BOE Date</th><th>DODate</th><th>Liner Seal No</th><th>Sanco Seal No</th> </tr> </thead>";
            foreach (var rslt in query) 
            {
                if (rslt.LSEALNO == null) rslt.LSEALNO = "-";
                if (rslt.SSEALNO == null) rslt.SSEALNO = "-";
                if (rslt.BOEDATE == null) rslt.BOEDATE = DateTime.Now.Date;
                if (rslt.GIDATE == null) rslt.GIDATE = DateTime.Now.Date;
                if (rslt.OOCDATE != null && rslt.OOCDATE != "")
                    ViewBag.OOCDATE = Convert.ToDateTime(rslt.OOCDATE).ToString("dd/MM/yyyy");

                tabl = tabl + "<tbody>";
                tabl = tabl + "<tr><td><input type='checkbox' name='CHKBX' class='CHKBX' id='CHKBX' onchange='SelectedCont(this)' /><input type=text name='booltype' class='booltype hidden' /></td><td class=hide>";
                tabl = tabl + "<input type=text id=OSDID class=OSDID name=OSDID value=" + rslt.OSDID + "></td>";
                tabl = tabl + "<td class=hide><input type=text id=ASLDID value=0  class=ASLDID name=ASLDID hidden></td>";
                tabl = tabl + "<td class=hide><input type=text id=STMRNAME  class=STMRNAME name=STMRNAME></td>";
                tabl = tabl + "<td class=hide><input type=text id=IMPRTNAME  class=IMPRTNAME name=IMPRTNAME hidden></td>";
                tabl = tabl + "<td class=hide><input type=text id=VOYNO  class=VOYNO name=VOYNO hidden></td><td class=hide>";
                tabl = tabl + "<input type=text id=VSLNAME value='' class=VSLNAME name=VSLNAME hidden></td>";
                tabl = tabl + "<td class='col-md-3'><input type=text id=CONTNRNO value=" + rslt.CONTNRNO + " class='form-control CONTNRNO' name=CONTNRNO readonly></td>";
                tabl = tabl + "<td class='col-md-1'><input type=text value=" + rslt.CONTNRSDESC + " id=CONTNRSDESC class='CONTNRSDESC form-control' name=CONTNRSDESC readonly>";
                //tabl = tabl + "</td><td class='col-md-2'><input type=text id=GIDATE value='" + rslt.GIDATE.Value.ToString("dd/MM/yyyy") + "' class='GIDATE form-control' name=GIDATE readonly></td>";
                tabl = tabl + "</td><td class='col-md-2'><input type=text id=GIDID value='" + rslt.GIDID + "' class='GIDID form-control hidden' name=GIDID readonly><input type=text id=GIDATE value='" + rslt.GIDATE.ToString("dd/MM/yyyy") + "' class='GIDATE form-control' name=GIDATE readonly></td>";
                tabl = tabl + "<td class='col-lg-2'><input type=text id=OSMNO  class='form-control OSMNO' name=OSMNO value='" + rslt.OSMDNO + "' readonly></td><td class='col-lg-2'>";
                tabl = tabl + "<input type=text id=BOENO  class='form-control BOENO' name=BOENO value='" + rslt.BOENO + "'  readonly></td>";
                tabl = tabl + "<td class='col-md-2'><input type=text id=BOEDATE  class='form-control BOEDATE' name=BOEDATE value='" + rslt.BOEDATE.Value.ToString("dd/MM/yyyy") + "' readonly></td>";
                tabl = tabl + "<td class='col-md-2'><input type=text id=DODATE  class='form-control DODATE' name=DODATE  readonly style=width:89px value='" + rslt.DODATE.Value.ToString("dd/MM/yyyy") + "'></td>";
                tabl = tabl + "<td class='col-md-2 hide'><input type=text id=oocdt  class='form-control oocdt' name=oocdt  readonly style=width:89px value='" + rslt.OOCDATE + "'></td>";
                tabl = tabl + "<td class='col-md-2'><input type=text value='" + rslt.LSEALNO + "' id=LSEALNO class='form-control LSEALNO' name=LSEALNO readonly style=width:65px></td>";
                tabl = tabl + "<td class='col-md-2'><input type=text value='" + rslt.SSEALNO + "' id=SSEALNO  class='form-control SSEALNO' name=SSEALNO readonly style=width:65px></td>";
                tabl = tabl + "</tr></tbody>";

            }
            tabl = tabl + "</Table>";
            Response.Write(tabl);

        }
        #endregion

        #region Save Data
        public void savedata(FormCollection F_Form)
        {

            AuthorizationSlipMaster authorizatioslipmaster = new AuthorizationSlipMaster();
            AuthorizationSlipDetail authorizatioslipdetail = new AuthorizationSlipDetail();
            //-------Getting Primarykey field--------
            Int32 ASLMID = Convert.ToInt32(F_Form["masterdata[0].ASLMID"]);
            Int32 ASLDID = 0;
            string DELIDS = "";
            //-----End
            string todaydt = Convert.ToString(DateTime.Now);
            string todayd = Convert.ToString(DateTime.Now.Date);

            if (ASLMID != 0)
            {
                authorizatioslipmaster = context.authorizatioslipmaster.Find(ASLMID);
            }

            authorizatioslipmaster.COMPYID = Convert.ToInt32(Session["compyid"]);
            authorizatioslipmaster.SDPTID = 1;
            authorizatioslipmaster.ASLMTYPE = 1;
            //authorizatioslipmaster.CUSRID = Session["CUSRID"].ToString();            
            authorizatioslipmaster.LMUSRID = Session["CUSRID"].ToString();
            authorizatioslipmaster.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
            authorizatioslipmaster.PRCSDATE = DateTime.Now;
            //authorizatioslipmaster.ASLMDATE = Convert.ToDateTime(F_Form["masterdata[0].ASLMTIME"]).Date;
            //authorizatioslipmaster.ASLMTIME = Convert.ToDateTime(F_Form["masterdata[0].ASLMTIME"]);

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

            if (ASLMID == 0)
            {

                authorizatioslipmaster.ASLMNO = Convert.ToInt32(Autonumber.autonum("AUTHORIZATIONSLIPMASTER", "ASLMNO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " AND SDPTID=1 AND ASLMTYPE=1").ToString());

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
            string DRVNAME = F_Form["detaildata[0].DRVNAME"];
            string VHLNO = F_Form["detaildata[0].VHLNO"];
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
                    authorizatioslipdetail.LCATEID = 0;
                    authorizatioslipdetail.ASLDTYPE = 3;
                    authorizatioslipdetail.ASLLTYPE = 2;
                    authorizatioslipdetail.ASLOTYPE = 1;
                    authorizatioslipdetail.STFDID = 0;
                    authorizatioslipdetail.VHLNO = VHLNO.ToString();
                    authorizatioslipdetail.DRVNAME = DRVNAME.ToString();
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
        #endregion

        #region Print View
        [Authorize(Roles = "ImportLoadSlipPrint")]
        public void PrintView(int? id = 0)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "Imp_Load_Slip", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Import_Load_Slip.RPT");
                cryRpt.RecordSelectionFormula = "{VW_ADSLIP_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_ADSLIP_CRY_PRINT_ASSGN.ASLMID} = " + id;

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
                cryRpt.Close();
                cryRpt.Dispose();
                GC.Collect();
                stringbuilder.Clear();
            }

        }
        #endregion

        #region Cancel 
        [Authorize(Roles = "ImportLoadSlipCancel")]
        public void UCancel()/*undo Cancel*/
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");

            //   var param = id.Split('-');

            String temp = Delete_fun.delete_check1(fld, id);
            var aslmid = Convert.ToInt32(id);
            if (temp.Equals("PROCEED"))
            {

                AuthorizationSlipMaster authorizatioslipmaster = context.authorizatioslipmaster.Find(Convert.ToInt32(id));
                GateInDetail gateindetails = context.gateindetails.Find(authorizatioslipmaster.NGIDID);
                context.gateindetails.Remove(gateindetails); context.SaveChanges();
                context.Entry(authorizatioslipmaster).Entity.DISPSTATUS = 0;
                context.Entry(authorizatioslipmaster).Entity.NGIDID = 0;
                context.Entry(authorizatioslipmaster).Entity.NGINO = 0;
                context.SaveChanges();


                Response.Write("Undo success...!");
            }
            else
                Response.Write(temp);
        }//..End of delete
        [Authorize(Roles = "ImportLoadSlipCancel")]
        public void Cancel()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");

            //   var param = id.Split('-');
            GateInDetail tab = new Models.GateInDetail();
            String temp = Delete_fun.delete_check1(fld, id);
            var aslmid = Convert.ToInt32(id);
            if (temp.Equals("PROCEED"))
            {
                var sql = (from r in context.authorizatioslipmaster.Where(x => x.ASLMID == aslmid)
                           join x in context.authorizationslipdetail on r.ASLMID equals x.ASLMID
                           join y in context.opensheetdetails on x.OSDID equals y.OSDID
                           join m in context.opensheetmasters on y.OSMID equals m.OSMID
                           select new { r.ASLMID, r.ASLMDNO, r.ASLMDATE, r.ASLMTIME, r.ASLMNO, x.DRVNAME, x.VHLNO, m.OSMIGMNO, m.OSMLNO }).ToList();

                tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                tab.SDPTID = 1;
                tab.GIDATE = Convert.ToDateTime(sql[0].ASLMTIME).Date;
                tab.GICCTLDATE = Convert.ToDateTime(sql[0].ASLMTIME).Date;
                tab.GITIME = Convert.ToDateTime(sql[0].ASLMTIME);
                tab.GICCTLTIME = Convert.ToDateTime(sql[0].ASLMTIME);
                tab.GINO = Convert.ToInt32(Autonumber.autonum("gateindetail", "GINO", "GINO<>0").ToString());
                int anoo = tab.GINO;
                string prfxx = string.Format("{0:D5}", anoo);
                tab.GIDNO = prfxx.ToString();
                tab.GIVHLTYPE = 0;
                tab.TRNSPRTID = 0;
                tab.VHLNO = Convert.ToString(sql[0].VHLNO);
                tab.TRNSPRTNAME = "-";
                tab.AVHLNO = Convert.ToString(sql[0].VHLNO);
                tab.DRVNAME = Convert.ToString(sql[0].DRVNAME);
                tab.GPREFNO = Convert.ToString(sql[0].ASLMNO);
                tab.IMPRTID = 0;
                tab.IMPRTNAME = "-";
                tab.STMRID = 0;
                tab.STMRNAME = "-";
                tab.CONDTNID = 0;
                tab.CONTNRNO = "-";
                tab.CONTNRTID = 0;
                tab.CONTNRID = 0;
                tab.CONTNRSID = 0;
                tab.LPSEALNO = "-";
                tab.CSEALNO = "-";
                tab.YRDID = 0;
                tab.VSLID = 0;
                tab.VSLNAME = "-";
                tab.VOYNO = "-";
                tab.PRDTGID = 0;
                tab.PRDTDESC = "-";
                tab.UNITID = 0;
                tab.GPLNO = Convert.ToString(sql[0].OSMLNO);
                tab.GPWGHT = 0;
                tab.GPEAMT = 0;
                tab.GPAAMT = 0;
                tab.IGMNO = Convert.ToString(sql[0].OSMIGMNO);
                tab.GIISOCODE = "-";
                tab.GIDMGDESC = "-";
                tab.GPWTYPE = 0;
                tab.GPSTYPE = 0;
                tab.GPETYPE = 0;
                tab.SLOTID = 0;
                tab.CUSRID = Session["CUSRID"].ToString();
                tab.LMUSRID = Session["CUSRID"].ToString();
                tab.DISPSTATUS = 0;
                tab.PRCSDATE = DateTime.Now;
                context.gateindetails.Add(tab);
                context.SaveChanges();


                AuthorizationSlipMaster authorizatioslipmaster = context.authorizatioslipmaster.Find(Convert.ToInt32(id));
                context.Entry(authorizatioslipmaster).Entity.DISPSTATUS = 1;
                context.Entry(authorizatioslipmaster).Entity.NGIDID = tab.GIDID;
                context.Entry(authorizatioslipmaster).Entity.NGINO = tab.GINO;
                context.SaveChanges();
                Response.Write("Cancelled successfully...");
            }
            else
                Response.Write(temp);
        }//..End of delete
        #endregion

        #region delete
        [Authorize(Roles = "ImportLoadSlipDelete")]
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
       
        [Authorize(Roles = "ImportLoadSlipDelete")]
        public void Del_det()
        {
            using (SCFSERPContext context = new SCFSERPContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        String id = Request.Form.Get("id");
                        String fld = Request.Form.Get("fld");

                        //   var param = id.Split('-');

                        String temp = Delete_fun.delete_check1(fld, id);

                        if (temp.Equals("PROCEED"))
                        {
                            AuthorizationSlipDetail authorizationSlipDetail = context.authorizationslipdetail.Find(Convert.ToInt32(id));                            
                            context.authorizationslipdetail.Remove(authorizationSlipDetail);
                            context.SaveChanges();
                            Response.Write("Deleted successfully...");
                        }
                        else
                            Response.Write(temp);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback(); //Response.Redirect("/Error/SavepointErr");
                        Response.Write("Sorry !!!An Error Occurred");
                    }
                }
            }
        }
        #endregion

    }
}