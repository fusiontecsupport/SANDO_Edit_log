using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using scfs.Data;
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
    public class DeliveryOrderController : Controller
    {
        //
        // GET: /DeliveryOrder/
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        [Authorize(Roles = "ImportDeliveryOrderIndex")]
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
        }//........end of index grid
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {

            using (var e = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_DeliveryOrder(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));

                var aaData = data.Select(d => new string[] { d.DODATE.Value.ToString("dd/MM/yyyy"), d.DONO, d.DODREFNO, d.DODREFNAME, d.CONTNRSDESC, d.DOREFNAME, d.IGMNO, d.GPLNO, d.DISPSTATUS.ToString(), d.DOMID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //[Authorize(Roles = "ImportDeliveryOrderEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/DeliveryOrder/Form/" + id);
        }
        [Authorize(Roles = "ImportDeliveryOrderCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            DeliveryOrderMaster tab = new DeliveryOrderMaster();
            DeliveryOrderMD vm = new DeliveryOrderMD();
            ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");
            ViewBag.DOMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.TARIFFMID = new SelectList("");
            ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");

            if (id != 0)
            {
                vm.masterdata = context.DeliveryOrderMasters.Find(id);
                vm.detaildata = context.DeliveryOrderDetails.Where(x => x.DOMID == id).ToList();
                ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC", vm.masterdata.BANKMID);
                ViewBag.DOMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL", vm.masterdata.DOMODE);
                ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", vm.masterdata.TARIFFMID);
                var tariffmid = Convert.ToInt32(vm.masterdata.TARIFFMID);
                var sql = context.Database.SqlQuery<int>("select TGID from TariffMaster where TARIFFMID=" + tariffmid).ToList();
                ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC", Convert.ToInt32(sql[0]));
                vm.detailedit = context.Database.SqlQuery<VW_DELIVERY_ORDER_DETAIL_GRID_ASSGN>("select * from VW_DELIVERY_ORDER_DETAIL_GRID_ASSGN where DOMID=" + Convert.ToInt32(id) + "").ToList();

                ViewBag.IGMNO = vm.detailedit[0].IGMNO;
                ViewBag.GPLNO = vm.detailedit[0].GPLNO;
                ViewBag.BILLEMID = vm.detailedit[0].BILLEMID;
                ViewBag.DPAIDAMT = vm.detailedit[0].DPAIDAMT;
                ViewBag.DPAIDNO = vm.detailedit[0].DPAIDNO;
            }

            return View(vm);
        }


        public JsonResult GetTariff(int id)
        {
            var data = context.Database.SqlQuery<TariffMaster>("Select * from TariffMaster where DISPSTATUS=0 and TGID=" + id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCHADetail(string id)
        {
            var param = id.Split(';');

            var qry = "PR_DORDER_IGMNO_BOE_NO_CTRL_ASSGN_E001 @PIGMNO='" + param[0] + "',@PGPLNO='" + param[1] + "'";
            var data = context.Database.SqlQuery<PR_DORDER_IGMNO_BOE_NO_CTRL_ASSGN_E001_Result>(qry);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public void Detail(string id)
        {
            var param = id.Split(';');
            using (SCFSERPContext contxt = new SCFSERPContext())
            {

                var query = context.Database.SqlQuery<PR_DORDER_IGMNO_BILLENTRY_WISE_CONTAINER_FLX_ASSGN_Result>("PR_DORDER_IGMNO_BILLENTRY_WISE_CONTAINER_FLX_ASSGN @PIGMNO='" + param[0] + "',@PGPLNO='" + param[1] + "'").ToList();


                var tabl = " <div class='panel-heading navbar-inverse'  style=color:white>DO Details</div><Table id=mytabl class='table table-striped table-bordered bootstrap-datatable'> <thead><tr> <th>BOE NO</th><th>Container No</th><th> Size</th><th>InDate</th><th>Vessel</th> <th> Voy No </th><th>Product</th><th>Weight</th></tr> </thead>";

                foreach (var rslt in query)
                {

                    tabl = tabl + "<tbody><tr><td class=hide><input type=text id='DODID' class='DODID' name='DODID' value=0></td><td class=hide><input type=text id=F_GIDID value=" + rslt.GIDID + "  class=F_GIDID name=F_GIDID hidden></td><td class=hide><input type=text id=STMRNAME value='" + rslt.STMRNAME + "' class=STMRNAME name=STMRNAME></td><td class=hide><input type=text id=IMPRTNAME value='" + rslt.IMPRTNAME + "' class=IMPRTNAME name=IMPRTNAME hidden></td><td><input type=text value='" + rslt.BILLEMDNO + "' id='BILLEMDNO' class='form-control BILLEMDNO' name='BILLEMDNO'  readonly><input type=text value='" + rslt.BILLEDID + "' id='BILLEDID' class='hide form-control BILLEDID' name='BILLEDID'  readonly></td><td><input type=text id=CONTNRNO value=" + rslt.CONTNRNO + " class=CONTNRNO name=CONTNRNO readonly></td><td><input type=text value=" + rslt.CONTNRSDESC + " id=CONTNRSDESC class=CONTNRSDESC name=CONTNRSDESC style=width:45px readonly></td><td><input type=text id=GIDATE value=" + rslt.GIDATE + " class=GIDATE name=GIDATE readonly></td><td><input type=text value='" + rslt.VSLNAME + "' id='VSLNAME' class='VSLNAME form-control' name='VSLNAME' readonly></td><td><input type=text id='VOYNO' value='" + rslt.VOYNO + "' class='VOYNO form-control' name='VOYNO' readonly></td><td><input type=text id='PRDTDESC' value='" + rslt.PRDTDESC + "'  class='PRDTDESC form-control' name='PRDTDESC' readonly></td><td><input type=text id='GPWGHT' value=" + rslt.GPWGHT + "  class='GPWGHT form-control' name='GPWGHT' readonly></td></tr></tbody>";

                }
                tabl = tabl + "</Table>";
                Response.Write(tabl);
            }
        }
        public void DetailEdit(string id)
        {
            var param = id.Split(';');
            using (SCFSERPContext contxt = new SCFSERPContext())
            {

                var query = context.Database.SqlQuery<VW_DELIVERY_ORDER_DETAIL_GRID_ASSGN>("select * from VW_DELIVERY_ORDER_DETAIL_GRID_ASSGN where DOMID=" + Convert.ToInt32(param[0]) + "").ToList();


                var tabl = " <div class='panel-heading navbar-inverse'  style=color:white>DO Details</div><Table id=mytabl class='table table-striped table-bordered bootstrap-datatable'> <thead><tr> <th>BOE NO</th><th>Container No</th><th> Size</th><th>InDate</th><th>Vessel</th> <th> Voy No </th><th>Product</th><th>Weight</th></tr> </thead>";

                foreach (var rslt in query)
                {

                    tabl = tabl + "<tbody><tr><td class=hide><input type=text id='DODID' class='DODID' name='DODID' value=" + rslt.DODID + "></td><td class=hide><input type=text id=F_GIDID value=" + rslt.GIDID + "  class=F_GIDID name=F_GIDID hidden></td><td class=hide><input type=text id=STMRNAME value='" + rslt.STMRNAME + "' class=STMRNAME name=STMRNAME></td><td class=hide><input type=text id=IMPRTNAME value='" + rslt.IMPRTNAME + "' class=IMPRTNAME name=IMPRTNAME hidden></td><td><input type=text value='" + rslt.DODREFNO + "' id='BILLEMDNO' class='form-control BILLEMDNO' name='BILLEMDNO'  readonly><input type=text value='" + rslt.BILLEDID + "' id='BILLEDID' class='hide form-control BILLEDID' name='BILLEDID'  readonly></td><td><input type=text id=CONTNRNO value=" + rslt.CONTNRNO + " class=CONTNRNO name=CONTNRNO readonly></td><td><input type=text value=" + rslt.CONTNRSDESC + " id=CONTNRSDESC class=CONTNRSDESC name=CONTNRSDESC style=width:45px readonly></td><td><input type=text id=GIDATE value=" + rslt.GIDATE + " class=GIDATE name=GIDATE readonly></td><td><input type=text value='" + rslt.VSLNAME + "' id='VSLNAME' class='VSLNAME form-control' name='VSLNAME' readonly></td><td><input type=text id='VOYNO' value='" + rslt.VOYNO + "' class='VOYNO form-control' name='VOYNO' readonly></td><td><input type=text id='PRDTDESC' value='" + rslt.PRDTDESC + "'  class='PRDTDESC form-control' name='PRDTDESC' readonly></td><td><input type=text id='GPWGHT' value=" + rslt.GPWGHT + "  class='GPWGHT form-control' name='GPWGHT' readonly></td><td class=hide><input type=text id='BILLEMID_MOD' value=" + rslt.BILLEMID + "  class='BILLEMID_MOD form-control' name='BILLEMID_MOD'><input type=text id='IGMNO_MOD' value=" + rslt.IGMNO + "  class='IGMNO_MOD form-control' name='IGMNO_MOD'><input type=text id='GPLNO_MOD' value=" + rslt.GPLNO + "  class='GPLNO_MOD form-control' name='GPLNO_MOD'><input type=text id='DPAIDNO_MOD' value=" + rslt.DPAIDNO + "  class='DPAIDNO_MOD form-control' name='DPAIDNO_MOD'><input type=text id='DPAIDAMT_MOD' value=" + rslt.DPAIDAMT + "  class='DPAIDAMT_MOD form-control' name='DPAIDAMT_MOD'></td></tr></tbody>";

                }
                tabl = tabl + "</Table>";
                Response.Write(tabl);
            }
        }
        //.................Insert/update values into database.............//
        public void savedata(FormCollection F_Form)
        {
            using (SCFSERPContext dataContext = new SCFSERPContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        string todaydt = Convert.ToString(DateTime.Now);
                        string todayd = Convert.ToString(DateTime.Now.Date);

                        DeliveryOrderMaster DeliveryOrderMaster = new DeliveryOrderMaster();
                        DeliveryOrderDetail DeliveryOrderDetail = new DeliveryOrderDetail();
                        //-------Getting Primarykey field--------
                        Int32 DOMID = Convert.ToInt32(F_Form["masterdata.DOMID"]);
                        Int32 DODID = 0;
                        string DELIDS = "";
                        //-----End


                        if (DOMID != 0)
                        {
                            DeliveryOrderMaster = context.DeliveryOrderMasters.Find(DOMID);
                        }

                        DeliveryOrderMaster.COMPYID = Convert.ToInt32(Session["compyid"]);
                        DeliveryOrderMaster.SDPTID = 1;

                        string indate = Convert.ToString(F_Form["masterdata.DODATE"]);                        
                        if (indate != null || indate != "")
                        {
                            DeliveryOrderMaster.DODATE = Convert.ToDateTime(indate).Date;
                        }
                        else { DeliveryOrderMaster.DODATE = DateTime.Now.Date; }

                        if (DeliveryOrderMaster.DODATE > Convert.ToDateTime(todayd))
                        { DeliveryOrderMaster.DODATE = Convert.ToDateTime(todayd); }

                        string intime = Convert.ToString(F_Form["masterdata.DOTIME"]);
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

                                    DeliveryOrderMaster.DOTIME = Convert.ToDateTime(in_datetime);
                                }
                                else { DeliveryOrderMaster.DOTIME = DateTime.Now; }
                            }
                            else { DeliveryOrderMaster.DOTIME = DateTime.Now; }
                        }
                        else { DeliveryOrderMaster.DOTIME = DateTime.Now; }

                        if (DeliveryOrderMaster.DOTIME > Convert.ToDateTime(todaydt))
                        { DeliveryOrderMaster.DOTIME = Convert.ToDateTime(todaydt); }

                        //DeliveryOrderMaster.DODATE = Convert.ToDateTime(F_Form["masterdata.DOTIME"]).Date;
                        //DeliveryOrderMaster.DOTIME = Convert.ToDateTime(F_Form["masterdata.DOTIME"]);

                        DeliveryOrderMaster.DOREFID = Convert.ToInt32(F_Form["masterdata.DOREFID"]);

                        DeliveryOrderMaster.DOREFNAME = F_Form["masterdata.DOREFNAME"].ToString();
                        DeliveryOrderMaster.DONARTN = null; DeliveryOrderMaster.DORMKS = null;
                        if (DOMID == 0 )
                            DeliveryOrderMaster.CUSRID = Session["CUSRID"].ToString();
                        DeliveryOrderMaster.LMUSRID = Session["CUSRID"].ToString();
                        DeliveryOrderMaster.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
                        DeliveryOrderMaster.PRCSDATE = DateTime.Now;
                        DeliveryOrderMaster.DOPCOUNT = 0;
                        DeliveryOrderMaster.TARIFFMID = Convert.ToInt32(F_Form["TARIFFMID"]);
                        DeliveryOrderMaster.DOMODE = Convert.ToInt16(F_Form["DOMODE"]);
                        DeliveryOrderMaster.DOMODEDETL = (F_Form["masterdata.DOMODEDETL"]);
                        DeliveryOrderMaster.DOREFAMT = Convert.ToDecimal(F_Form["masterdata.DOREFAMT"]);

                        //string OOCdate = Convert.ToString(F_Form["masterdata.OOCDATE"]);
                        //if (OOCdate != null || OOCdate != "")
                        //{
                        //    DeliveryOrderMaster.OOCDATE = Convert.ToDateTime(OOCdate).Date;
                        //}
                        //else { DeliveryOrderMaster.OOCDATE = DateTime.Now.Date; }

                        //DeliveryOrderMaster.OOCNO = (F_Form["masterdata.OOCNO"]);

                        var DOMODE = Convert.ToInt16(F_Form["DOMODE"]);
                        if (DOMODE != 2 && DOMODE != 3)
                        {
                            DeliveryOrderMaster.DOREFNO = "";
                            DeliveryOrderMaster.DOREFBNAME = "";
                            DeliveryOrderMaster.BANKMID = 0;
                            DeliveryOrderMaster.DOREFDATE = DateTime.Now;
                        }
                        else
                        {
                            DeliveryOrderMaster.DOREFNO = (F_Form["masterdata.DOREFNO"]).ToString();
                            DeliveryOrderMaster.DOREFBNAME = (F_Form["masterdata.DOREFBNAME"]).ToString();
                            DeliveryOrderMaster.BANKMID = Convert.ToInt32(F_Form["BANKMID"]);
                            DeliveryOrderMaster.DOREFDATE = Convert.ToDateTime(F_Form["masterdata.DOREFDATE"]).Date;

                        }

                        if (DOMID == 0)
                        {
                            DeliveryOrderMaster.DONO = Convert.ToInt32(Autonumber.autonum("DeliveryOrderMaster", "DONO", "DONO<>0 and SDPTID=1 and COMPYID=" + Convert.ToInt32(Session["compyid"]) + "").ToString());

                            int ano = DeliveryOrderMaster.DONO;
                            string prfx = string.Format( "{0:D5}", ano);
                            DeliveryOrderMaster.DODNO = prfx.ToString();

                            //........end of autonumber
                            context.DeliveryOrderMasters.Add(DeliveryOrderMaster);
                            context.SaveChanges();
                            DOMID = DeliveryOrderDetail.DOMID;
                        }
                        else
                        {
                            context.Entry(DeliveryOrderMaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }


                        //-------------transaction Details
                        string[] F_DODID = F_Form.GetValues("DODID");
                        // string[] F_GIDID = F_Form.GetValues("F_GIDID");
                        string[] BILLEDID = F_Form.GetValues("BILLEDID");
                        string[] DOIDATE = F_Form.GetValues("GIDATE");
                        string[] DODREFNO = F_Form.GetValues("BILLEMDNO");
                        string[] DODREFNAME = F_Form.GetValues("CONTNRNO");
                        string[] DODREFID = F_Form.GetValues("F_GIDID");
                        for (int count = 0; count < F_DODID.Count(); count++)
                        {

                            DODID = Convert.ToInt32(F_DODID[count]);

                            if (DODID != 0)
                            {
                                DeliveryOrderDetail = context.DeliveryOrderDetails.Find(DODID);
                            }
                            DeliveryOrderDetail.DOMID = DeliveryOrderMaster.DOMID;
                            DeliveryOrderDetail.DODREFNO = (DODREFNO[count]).ToString();
                            DeliveryOrderDetail.DODREFNAME = (DODREFNAME[count]).ToString();
                            DeliveryOrderDetail.DODREFID = Convert.ToInt32(DODREFID[count]);
                            DeliveryOrderDetail.DOVDATE = Convert.ToDateTime(F_Form["detaildata[0].DOVDATE"]);
                            DeliveryOrderDetail.DOIDATE = Convert.ToDateTime(DOIDATE[count]);
                            DeliveryOrderDetail.BILLEDID = Convert.ToInt32(BILLEDID[count]);
                            DeliveryOrderDetail.DISPSTATUS = 0;
                            //  DeliveryOrderDetail.DOVDATE = DateTime.Now;
                            //  DeliveryOrderDetail.DOIDATE = DateTime.Now;

                            if (Convert.ToInt32(DODID) == 0)
                            {
                                context.DeliveryOrderDetails.Add(DeliveryOrderDetail);
                                context.SaveChanges();
                                DODID = DeliveryOrderDetail.DODID;
                            }
                            else
                            {

                                context.Entry(DeliveryOrderDetail).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                            }//..............end
                            DELIDS = DELIDS + "," + DODID.ToString();

                            context.Database.ExecuteSqlCommand("UPDATE BILLENTRYMASTER SET DPAIDNO='" + F_Form["F_DPAIDNO"].ToString() + "',DPAIDAMT=" + Convert.ToDecimal(F_Form["F_DPAIDAMT"]) + " WHERE BILLEMID=" + Convert.ToInt32(F_Form["BILLEMID"]));

                        }


                        context.Database.ExecuteSqlCommand("DELETE FROM DeliveryOrderDetail  WHERE DOMID=" + DOMID + " and  DODID NOT IN(" + DELIDS.Substring(1) + ")");
                        trans.Commit(); Response.Redirect("Index");
                    }
                    catch (Exception ex)
                    {
                        var msg = ex.Message;
                        trans.Rollback();
                        //Response.Write("Sorry!!An Error Ocurred...");
                        Response.Redirect("/Error/AccessDenied");
                    }
                }
            }

        }
        [Authorize(Roles = "ImportDeliveryOrderPrint")]
        public void PrintView(int id)
        {
            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "DO", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;


                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] +  "DELIVERYORDER.RPT");

                cryRpt.RecordSelectionFormula = "{VW_DORDER_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_DORDER_CRY_PRINT_ASSGN.DOMID} = " + id;



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
                GC.Collect();
                stringbuilder.Clear();
            }

        }
        //end
        [Authorize(Roles = "ImportDeliveryOrderDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");

            //   var param = id.Split('-');

            String temp = Delete_fun.delete_check1(fld, id);

            if (temp.Equals("PROCEED"))
            {
                DeliveryOrderMaster DeliveryOrderMaster = context.DeliveryOrderMasters.Find(Convert.ToInt32(id));
                context.DeliveryOrderMasters.Remove(DeliveryOrderMaster);
                context.Database.ExecuteSqlCommand("delete from deliveryorderdetail where domid=" + Convert.ToInt32(id));
                context.SaveChanges();
                Response.Write("Deleted successfully...");
            }
            else
                Response.Write(temp);
        }//..End of delete
    }
}