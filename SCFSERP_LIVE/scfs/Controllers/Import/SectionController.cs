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
    public class SectionController : Controller
    {
        // GET: Section

        #region Context declaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region Index Form
        //[Authorize(Roles = "ImportSectionIndex")]
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

            DateTime sd = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;

            DateTime ed = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;
            return View();
        }
        #endregion

        #region GetAjaxdata
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Import_Invoice(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToInt32(Session["TRANBTYPE"]), Convert.ToInt32(Session["REGSTRID"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANTIME.Value.ToString("hh:mm tt"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANNAMT.ToString(), d.DISPSTATUS, d.TRANMID.ToString() }).ToArray();
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

        #region Edit
        //[Authorize(Roles = "ImportSectionEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/Section/Form/" + id);
        }
        #endregion

        #region Form
        //[Authorize(Roles = "ImportSectionForm")]
        public ActionResult Form(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            TransactionMaster tab = new TransactionMaster();
            TransactionMD vm = new TransactionMD();

            //..........................................Dropdown data.........................//
            ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME");
            // ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.SBMDATE = DateTime.Now;
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.TRANLMID = new SelectList("");
            ViewBag.TRANOTYPE = new SelectList(context.ImportDestuffSlipOperation, "OPRTYPE", "OPRTYPEDESC");
            ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");

            ViewBag.TARIFFMID = new SelectList("");
            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
            selectedBILLTYPE.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
            selectedBILLTYPE.Add(selectedItemDSP);
            ViewBag.TRANBTYPE = selectedBILLTYPE;
            //....end

            //.........s.Tax...//
            List<SelectListItem> selectedtaxlst = new List<SelectListItem>();
            SelectListItem selectedItemtax = new SelectListItem { Text = "YES", Value = "1", Selected = true };
            selectedtaxlst.Add(selectedItemtax);
            selectedItemtax = new SelectListItem { Text = "NO", Value = "0", Selected = false };
            selectedtaxlst.Add(selectedItemtax);
            ViewBag.STAX = selectedtaxlst;

            ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == 1 || x.REGSTRID == 2 || x.REGSTRID == 6), "REGSTRID", "REGSTRDESC");
            //.....end

            //........display status.........//
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemDISP = new SelectListItem { Text = "In Books", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItemDISP);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            //....end

            if (id != 0)//....Edit Mode
            {
                tab = context.transactionmaster.Find(id);//find selected record

                //...................................Selected dropdown value..................................//
                ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME", tab.LCATEID);
                ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC", tab.BANKMID);
                ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL", tab.TRANMODE);
                ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == tab.REGSTRID), "REGSTRID", "REGSTRDESC", tab.REGSTRID);


                //.................Display status.................//
                List<SelectListItem> selectedDISP = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItemDIS = new SelectListItem { Text = "In Books", Value = "0", Selected = false };
                    selectedDISP.Add(selectedItemDIS);
                    selectedItemDIS = new SelectListItem { Text = "Cancelled", Value = "1", Selected = true };
                    selectedDISP.Add(selectedItemDIS);
                    ViewBag.DISPSTATUS = selectedDISP;
                }
                else
                {
                    SelectListItem selectedItemDIS = new SelectListItem { Text = "In Books", Value = "0", Selected = true };
                    selectedDISP.Add(selectedItemDIS);
                    selectedItemDIS = new SelectListItem { Text = "Cancelled", Value = "1", Selected = false };
                    selectedDISP.Add(selectedItemDIS);
                    ViewBag.DISPSTATUS = selectedDISP;
                }//....end

                //....................Bill type.................//
                List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
                if (Convert.ToInt32(tab.TRANBTYPE) == 1)
                {
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);
                    // selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
                    // selectedBILLYPE.Add(selectedItemGPTY);

                }
                else
                {

                    //selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                    //  selectedBILLYPE.Add(selectedItemGPTY);
                    SelectListItem selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = true };
                    selectedBILLYPE.Add(selectedItemGPTY);

                }
                ViewBag.TRANBTYPE = selectedBILLYPE;
                //..........end

                vm.masterdata = context.transactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.detaildata = context.transactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.costfactor = context.transactionmasterfactor.Where(det => det.TRANMID == id).ToList();
                vm.impinvcedata = context.Database.SqlQuery<pr_Import_Invoice_IGMNO_Grid_Assgn_Result>("pr_Import_Invoice_IGMNO_Grid_Assgn @PIGMNO='-',@PLNO='-',@PTRANMID=" + id + "").ToList();//........procedure  for edit mode details data
                var billemid = Convert.ToInt32(vm.impinvcedata[0].BILLEMID);
                var sealcnt = context.Database.SqlQuery<int>("select NOC from VW_IMPORT_BILL_OPEN_SHEET_SEAL_COUNT_ASSGN where BILLEMID=" + billemid).ToList();
                if (sealcnt.Count > 0)
                    ViewBag.NOC = sealcnt[0];
                else
                    ViewBag.NOC = 0;

                ViewBag.TARIFFMID = new SelectList(context.tariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
                var tariffmid = Convert.ToInt32(vm.detaildata[0].TARIFFMID);
                var sql = context.Database.SqlQuery<int>("select TGID from TariffMaster where TARIFFMID=" + tariffmid).ToList();
                ViewBag.TARIFFGID = new SelectList(context.tariffgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC", Convert.ToInt32(sql[0]));
            }

            return View(vm);
        }//........End of form
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
                         select new { r.GIDID, r.GIDATE, r.CONTNRNO, r.STMRNAME, r.IGMNO, r.GPLNO, s.CONTNRSDESC,r.GSECREMKRS,r.GIREMKRS }
                             ).ToList();

            //  context.Database.SqlQuery<GateInDetail>("select * GateInDetail where SDPTID=1 and DISPSTATUS=0 and CONTNRID=1 where IGMNO='" + igmno + "' and GPLNO='" + gplno + "'").ToList();


            var tabl = "";
            var count = 1;

            foreach (var rslt in query)
            {

                tabl = tabl + "<tr><td>" + count + "</td>";
                tabl = tabl + "<td>" + rslt.GIDATE.ToString("dd/MM/yyyy") + "<input type=text id=sno value=0  class='sno hide' readonly='readonly' name=sno style='width:56px'>";
                tabl = tabl + "<input type=text id=GIDID value=" + rslt.GIDID + "  class='GIDID hide' readonly='readonly' name=GIDID style='width:56px'></td>";
                tabl = tabl + "<td class='col-lg-0'><input type='text' id='CONTNRNO' value='" + rslt.CONTNRNO + "'  class='CONTNRNO hide' readonly='readonly' name='CONTNRNO' >" + rslt.CONTNRNO + "</td>";
                tabl = tabl + "<td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class='hide SIZE' name=SIZE style='width:40px' readonly='readonly'>" + rslt.CONTNRSDESC + "</td>";
                tabl = tabl + "<td >" + rslt.IGMNO + "</td>";
                tabl = tabl + "<td >" + rslt.GPLNO + "</td>";
                tabl = tabl + "<td>" + rslt.STMRNAME + "</td>";
                tabl = tabl + "<td>" + rslt.GSECREMKRS + "</td>";
                tabl = tabl + "<td>" + rslt.GIREMKRS + "</td>";
                tabl = tabl + "<td class='hidden1'><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS value='' " + count + "  onchange=total() style='width:30px'>";
                tabl = tabl + "<input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value=''></td></tr>";
                count++;
            }
            tabl = tabl + "</Table>";

            return tabl;

        }
        #endregion

        #region Save data
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
                                if (F_Form.Get("GTYPE") == "0")
                                {
                                    if (F_Form.Get("GSTYPE") == "0")
                                    { context.Database.ExecuteSqlCommand("UPDATE GATEINDETAIL SET GSECTYPE=" + Convert.ToInt16(F_Form.Get("GSTYPE")) + ",GSECREMKRS='" + F_Form.Get("GREMRKS") + "' WHERE GIDID=" + GIDID); }
                                    else
                                    { context.Database.ExecuteSqlCommand("UPDATE GATEINDETAIL SET GSECTYPE=" + Convert.ToInt16(F_Form.Get("GSTYPE")) + ",GSECREMKRS='" + F_Form.Get("GREMRKS") + "',GIREMKRS='" + F_Form.Get("GREMRKS") + "' WHERE GIDID=" + GIDID); }
                                }
                                else
                                {
                                    if (F_Form.Get("GSTYPE") == "1")
                                    { context.Database.ExecuteSqlCommand("UPDATE GATEINDETAIL SET GSEALTYPE=0,GSEALREMKRS='" + F_Form.Get("GREMRKS") + "' WHERE GIDID=" + GIDID); }
                                    else
                                    { context.Database.ExecuteSqlCommand("UPDATE GATEINDETAIL SET GSEALTYPE=1,GSEALREMKRS='" + F_Form.Get("GREMRKS") + "',GIREMKRS='" + F_Form.Get("GREMRKS") + "' WHERE GIDID=" + GIDID); }
                                }

                            }
                        }

                        trans.Commit(); Response.Redirect("/Section/Form");
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

    }
}