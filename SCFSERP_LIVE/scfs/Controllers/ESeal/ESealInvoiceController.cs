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
using scfs.Data;
using scfs_erp;

namespace scfs_erp.Controllers.ESeal
{
    [SessionExpire]
    public class ESealInvoiceController : Controller
    {
        // GET: ESealInvoice

        #region contextdeclaration
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        #endregion

        #region Index form
        [Authorize(Roles = "ESeal_Invoice_Index")]
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
            if (Request.Form.Get("REGSTRID") != null)
            {
                Session["REGSTRID"] = Request.Form.Get("REGSTRID");
            }
            else
            {
                Session["REGSTRID"] = "54";
            }


            List<SelectListItem> selectedregid_ = new List<SelectListItem>();
            if (Convert.ToInt32(Session["REGSTRID"]) == 54)
            {
                SelectListItem selectedItemreg_ = new SelectListItem { Text = "TAX INVOICE", Value = "54", Selected = true };
                selectedregid_.Add(selectedItemreg_);
                selectedItemreg_ = new SelectListItem { Text = "BILL OF SUPPLY", Value = "55", Selected = false };
                selectedregid_.Add(selectedItemreg_);
                ViewBag.REGSTRID = selectedregid_;
            }
            else if (Convert.ToInt32(Session["REGSTRID"]) == 55)
            {
                SelectListItem selectedItemreg_ = new SelectListItem { Text = "TAX INVOICE", Value = "54", Selected = false };
                selectedregid_.Add(selectedItemreg_);
                selectedItemreg_ = new SelectListItem { Text = "BILL OF SUPPLY", Value = "55", Selected = true };
                selectedregid_.Add(selectedItemreg_);
                ViewBag.REGSTRID = selectedregid_;
            }


            return View();
        }
        #endregion

        #region get data for table
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_ESeal_Invoice(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToInt32(Session["REGSTRID"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                var aaData = data.Select(d => new string[] { d.TRANDATE.ToString(), d.TRANTIME.ToString(), d.TRANDNO.ToString(), d.TRANREFNAME, d.CONTNRNO, d.CONTNRSDESC, d.CONTNRTDESC, d.EXPRTRNAME, d.TRANNAMT.ToString(), d.GSTAMT.ToString(), d.DISPSTATUS, d.GIDID.ToString(), d.TRANDID.ToString(), d.TRANMID.ToString() }).ToArray();
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

        [Authorize(Roles = "ESeal_Invoice_Edit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/ESealInvoice/ESIForm/" + id);            
        }

        #region Invoice Form
        public ActionResult ESIForm(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            TransactionMaster tab = new TransactionMaster();
            TransactionMD vm = new TransactionMD();

            ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.BILLEDID = new SelectList(context.containersizemasters.Where(x => x.DISPSTATUS == 0 && x.CONTNRSID > 1), "CONTNRSID", "CONTNRSDESC");
            //ViewBag.TRANDREFID = new SelectList(context.chargemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CHRGDESC), "CHRGID", "CHRGDESC");
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.TARIFFGID = new SelectList(context.ExportTariffGroupMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TGDESC), "TGID", "TGDESC");
            ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", "TARIFFTMID");

            List<SelectListItem> selectedregid = new List<SelectListItem>();
            SelectListItem selectedItemreg = new SelectListItem { Text = "TAX INVOICE", Value = "54", Selected = true };
            selectedregid.Add(selectedItemreg);
            selectedItemreg = new SelectListItem { Text = "BILL OF SUPPLY", Value = "55", Selected = false };
            selectedregid.Add(selectedItemreg);
            ViewBag.REGSTRID = selectedregid;

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemDISP = new SelectListItem { Text = "In Books", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItemDISP);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            ViewBag.CATEAID = new SelectList("");

            if (id != 0)//....Edit Mode
            {
                tab = context.transactionmaster.Find(id);//find selected record

                //...................................Selected dropdown value..................................//

                ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC", tab.BANKMID);
                ViewBag.TRANMODE = new SelectList(context.transactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL", tab.TRANMODE);


                var sqy = context.Database.SqlQuery<Category_Address_Details>("Select *from CATEGORY_ADDRESS_DETAIL WHERE CATEAID=" + tab.CATEAID).ToList();
                if (sqy.Count > 0)
                {
                    ViewBag.CATEAID = new SelectList(context.categoryaddressdetails.Where(x => x.CATEAID > 0), "CATEAID", "CATEATYPEDESC", tab.CATEAID);

                }
                else { ViewBag.CATEAID = new SelectList(""); }

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

                List<SelectListItem> selectedregid_ = new List<SelectListItem>();
                
                if (Convert.ToInt32(tab.REGSTRID) == 54)
                {
                    SelectListItem selectedItemreg_ = new SelectListItem { Text = "TAX INVOICE", Value = "54", Selected = true };
                    selectedregid_.Add(selectedItemreg_);
                   // selectedItemreg_ = new SelectListItem { Text = "BILL OF SUPPLY", Value = "55", Selected = false };
                   // selectedregid_.Add(selectedItemreg_);
                    ViewBag.REGSTRID = selectedregid_;
                }
                else if (Convert.ToInt32(tab.REGSTRID) == 55)
                {
                    SelectListItem selectedItemreg_ = new SelectListItem { Text = "BILL OF SUPPLY", Value = "55", Selected = true };
                    selectedregid_.Add(selectedItemreg_); 
                    //selectedItemreg_ = new SelectListItem { Text = "TAX INVOICE", Value = "54", Selected = false };
                    //selectedregid_.Add(selectedItemreg_);
                    ViewBag.REGSTRID = selectedregid_;
                }

                //.....end

                vm.masterdata = context.transactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.detaildata = context.transactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.costfactor = context.transactionmasterfactor.Where(det => det.TRANMID == id).ToList();
                var tariffmid = Convert.ToInt32(vm.detaildata[0].TARIFFMID);
                ViewBag.TARIFFMID = new SelectList(context.exporttariffmaster.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC", vm.detaildata[0].TARIFFMID);
                
                var qry = context.Database.SqlQuery<int>("select TGID frOm EXPORTTARIFFMASTER WHERE TARIFFMID=" + tariffmid).ToList();

                ViewBag.TARIFFGID = new SelectList(context.ExportTariffGroupMasters.Where(x => x.DISPSTATUS == 0), "TGID", "TGDESC", qry[0]);
                var chaid = tab.TRANREFID;
                vm.eslinvcedata = context.Database.SqlQuery<pr_ESeal_Invoice_Grid_Assgn_Result>("EXEC pr_ESeal_Invoice_Grid_Assgn @CHAID=" + chaid + ", @fromdate = '', @todate='', @PTRANMID=" + id + ", @CATEAID=0").ToList();//........procedure  for edit mode details data
                
            }

            return View(vm);
        }
        #endregion

        #region Autocomplete CHA Name
        public JsonResult AutoCha(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct().OrderBy(X => X.CATENAME);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Details for Grid
        public string Detail(string id)
        {
            int tariffmid = 0;
            
            var param = id.Split('~');
            var CHAID = (param[0]); var fromdate = "";
            var todate = ""; var TRANMID = Convert.ToInt32(param[3]); var CATEAID = Convert.ToInt32(param[4]);

            if (param[1] != "" || param[1] != null)
            {
                var fd = param[1].Split('-');
                fromdate = fd[2] + "-" + fd[1] + "-" + fd[0];
            }
            else { fromdate = ""; }
            if (param[2] != "" || param[2] != null)
            {
                var ed = param[2].Split('-');
                todate = ed[2] + "-" + ed[1] + "-" + ed[0];
            }
            else { todate = ""; }


            var qry = "EXEC pr_ESeal_Invoice_Grid_Assgn @CHAID=" + CHAID + ",@fromdate='" + fromdate + "',@todate='" + todate + "',  @PTRANMID=" + TRANMID + ",@CATEAID=" + CATEAID;
            var query = context.Database.SqlQuery<pr_ESeal_Invoice_Grid_Assgn_Result>(qry).ToList();

            var tabl = "";
            var count = 0;

            foreach (var rslt in query)
            {

                if (rslt.GODATE == null) rslt.GODATE = DateTime.Now.Date;
                if (rslt.GOTIME == null) rslt.GOTIME = DateTime.Now;
                if (tariffmid > 6)
                {
                    tabl = tabl + "<tr><td class='hidden1'><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS value='' " + count + "  onchange=total() style='width:30px'><input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value=''></td><td class='col-lg-0'><input type='text' id='TRANDREFNAME' value='" + rslt.CONTNRNO + "'  class='TRANDREFNAME' readonly='readonly' name='TRANDREFNAME' ></td><td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class=SIZE name=SIZE style='width:40px' readonly='readonly'></td><td ><input type=text id=TRANIDATE value='" + rslt.GIDATE.Value.ToString("dd/MM/yyyy") + "' class=TRANIDATE name=TRANIDATE style='width:70px' readonly='readonly'></td><td ><input type=text id=TRANSDATE value='" + rslt.GIDATE.Value.ToString("dd/MM/yyyy") + "' class=TRANSDATE name=TRANSDATE readonly='readonly' style='width:70px'></td><td><input type=text id='TRANEDATE' value='" + rslt.GODATE.Value.ToString("dd/MM/yyyy") + "' class='TRANEDATE datepicker' name='TRANEDATE' style='width:70px' onchange='calculation()'></td><td class='hide'><input type=text id=TRANDWGHT value=" + rslt.WGHT + " class=TRANDWGHT name=TRANDWGHT readonly=readonly style='width:70px'></td><td><input type=text id=TRANDSAMT value='0' class=TRANDSAMT name=TRANDSAMT style='width:70px' readonly='readonly'><input type=text id=TRAND_COVID_DISC_AMT value='0' class='TRAND_COVID_DISC_AMT hide' name=TRAND_COVID_DISC_AMT style='width:70px' readonly='readonly'></td><td><input type=text id=TRANDHAMT value='0' class=TRANDHAMT name=TRANDHAMT readonly=readonly style='width:70px'></td><td class='hide'><input type=text value='0' id=TRANDEAMT class='TRANDEAMT' name=TRANDEAMT  readonly=readonly style='width:70px'></td><td class='hide'><input type=text value='0' id=TRANDFAMT class='TRANDFAMT' name=TRANDFAMT  readonly=readonly style='width:70px'></td><td class='hide'><input type=text value='0' id=TRANDPAMT class='TRANDPAMT' name=TRANDPAMT  readonly=readonly style='width:70px'></td><td class='hide'><input type=text id=TRANDAAMT value='0' class='TRANDAAMT' name=TRANDAAMT   readonly=readonly style='width:70px'></td><td><input type=text value='0' id=TRANDNAMT class=TRANDNAMT name=TRANDNAMT readonly='readonly' style='width:70px'> <td class=hide><input type=text id=TRANDVDATE class=TRANDVDATE name=TRANDVDATE value='" + rslt.DODATE.Value.ToString("dd/MM/yyyy") + "'><input type=text id=F_DPAIDNO value='" + rslt.DPAIDNO + "'  class='F_DPAIDNO' name=F_DPAIDNO ><input type=text id=F_DPAIDAMT value=" + rslt.DPAIDAMT + "  class=F_DPAIDAMT name=F_DPAIDAMT ></td>  <td class=hide><input type=text id=CONTNRTID value=0  class=CONTNRTID name=CONTNRTID ><input type=text id=CONTNRSID value=" + rslt.CONTNRSID + "  class=CONTNRSID name=CONTNRSID ><input type=text id=TRANDREFID value=" + rslt.GIDID + "  class=TRANDREFID name=TRANDREFID ><input type=text id=BILLEDID value=" + rslt.BILLEDID + "  class=BILLEDID name=BILLEDID ><input type=text id=BILLEMID value=" + rslt.BILLEMID + "  class=BILLEMID name=BILLEMID ><input type=text id=F_TARIFFMID value=" + rslt.TARIFFMID + "  class=F_TARIFFMID name=F_TARIFFMID ><input type=text id=F_TRANDOTYPE value=" + rslt.TRANDOTYPE + "  class=F_TRANDOTYPE name=F_TRANDOTYPE ><input type=text id=F_CHANAME value='" + rslt.TRANREFNAME + "'  class=F_CHANAME name=F_CHANAME ><input type=text id=F_STMRNAME value='" + rslt.STMRNAME + "'  class=F_STMRNAME name=F_STMRNAME ><input type=text id=F_CHAID value=" + rslt.TRANREFID + "  class=F_CHAID name=F_CHAID ><input type=text id=F_STMRID value=" + rslt.STMRID + "  class=F_STMRID name=F_STMRID ></td><td class=hide><input type=text id=TRANDID value=0  class=TRANDID name=TRANDID ></td><td class='hide'><input type=text id=days value=0  class=days name=days ><input type=text id=NOD value='" + rslt.NOD + "'  class=NOD name=NOD ><input type=text id=TRANDRATE value='" + rslt.TRANDRATE + "'  class=TRANDRATE name=TRANDRATE ></td><td class=hide><input type=text id=RAMT7 value=" + Convert.ToDecimal(rslt.RAMT7) + "  class=RAMT7 name=RAMT7 style='display:none1' ><input type=text id=RAMT1 value=" + Convert.ToDecimal(rslt.RAMT1) + "  class=RAMT1 name=RAMT1 style='display:none1' ><input type=text id=RAMT2 value=" + Convert.ToDecimal(rslt.RAMT2) + "  class=RAMT2 name=RAMT2 style='display:none1' ><input type=text id=RAMT3 value=" + Convert.ToDecimal(rslt.RAMT3) + "  class=RAMT3 name=RAMT3 style='display:none1' ><input type=text id=RAMT4 value=" + Convert.ToDecimal(rslt.RAMT4) + "  class=RAMT4 name=RAMT4 style='display:none1' ><input type=text id=RAMT5 value=" + Convert.ToDecimal(rslt.RAMT5) + "  class=RAMT5 name=RAMT5 style='display:none1' ><input type=text id=RAMT6 value=" + Convert.ToDecimal(rslt.RAMT6) + " class=RAMT6 name=RAMT6 style='display:none1' ></td><td class=hide><input type=text id=SLABMIN6 value='0'  class=SLABMIN6 name=SLABMIN6 style='display:none1' ><input type=text id=SLABMAX6 value='0'  class=SLABMAX6 name=SLABMAX6 style='display:none1' ><input type=text id=SLABMIN value='0'  class=SLABMIN name=SLABMIN style='display:none1' ><input type=text id=SLABMAX value='0'  class=SLABMAX name=SLABMAX style='display:none1' ><input type=text id=SLABMIN1 value='0'  class=SLABMIN1 name=SLABMIN1 style='display:none1' ><input type=text id=SLABMAX1 value='0'  class=SLABMAX1 name=SLABMAX1 style='display:none1' ><input type=text id=SLABMIN2 value='0'  class=SLABMIN2 name=SLABMIN2 style='display:none1' ><input type=text id=SLABMAX2 value='0'  class=SLABMAX2 name=SLABMAX2 style='display:none1' > <input type=text id=SLABMIN3 value='0'  class=SLABMIN3 name=SLABMIN3 style='display:none1' ><input type=text id=SLABMAX3 value='0'  class=SLABMAX3 name=SLABMAX3 style='display:none1' >  <input type=text id=SLABMIN4 value='0'  class=SLABMIN4 name=SLABMIN4 style='display:none1' ><input type=text id=SLABMAX4 value='0'  class=SLABMAX4 name=SLABMAX4 style='display:none1' > <input type=text id=SLABMIN5 value='0'  class=SLABMIN5 name=SLABMIN5 style='display:none1' ><input type=text id=SLABMAX5 value='0'  class=SLABMAX5 name=SLABMAX5 style='display:none1' > </td><td class=hide> <input type=text id=RCAMT7 value=value=" + Convert.ToDecimal(rslt.RCAMT7) + "  class=RCAMT7 name=RCAMT7 style='display:none1' ><input type=text id=RCAMT1 value=value=" + Convert.ToDecimal(rslt.RCAMT1) + "  class=RCAMT1 name=RCAMT1 style='display:none1' ><input type=text id=RCAMT2 value=" + Convert.ToDecimal(rslt.RCAMT2) + "  class=RCAMT2 name=RCAMT2 style='display:none1' ><input type=text id=RCAMT3 value=" + Convert.ToDecimal(rslt.RCAMT3) + "  class=RCAMT3 name=RCAMT3 style='display:none1'><input type=text id=RCAMT4 value=" + Convert.ToDecimal(rslt.RCAMT4) + "  class=RCAMT4 name=RCAMT4 style='display:none1' ><input type=text id=RCAMT5 value=" + Convert.ToDecimal(rslt.RCAMT5) + "  class=RCAMT5 name=RCAMT5 style='display:none1' ><input type=text id=RCAMT6 value=" + Convert.ToDecimal(rslt.RCAMT6) + "  class=RCAMT6 name=RCAMT6 style='display:none1' ></td><td class=hide><input type=text id=RCOL7 value=" + Convert.ToDecimal(rslt.RCOL7) + "  class=RCOL7 name=RCOL7 style='display:none1' ><input type=text id=RCOL1 value=" + Convert.ToDecimal(rslt.RCOL1) + "  class=RCOL1 name=RCOL1 style='display:none1' ><input type=text id=RCOL2 value=" + Convert.ToDecimal(rslt.RCOL2) + " class=RCOL2 name=RCOL2 style='display:none1' ><input type=text id=RCOL3 value=" + Convert.ToDecimal(rslt.RCOL3) + "  class=RCOL3 name=RCOL3 style='display:none1' ><input type=text id=RCOL4 value=" + Convert.ToDecimal(rslt.RCOL4) + "  class=RCOL4 name=RCOL4 style='display:none1' ><input type=text id=RCOL5 value=" + Convert.ToDecimal(rslt.RCOL5) + " class=RCOL5 name=RCOL5 style='display:none1' ><input type=text id=RCOL6 value=" + Convert.ToDecimal(rslt.RCOL6) + "  class=RCOL6 name=RCOL6 style='display:none1' ></td><td class='hide'><input type='text' value=" + rslt.GPAAMT + " id='GPAAMT' class='GPAAMT' name='GPAAMT'><input type='text' value=" + rslt.GPEAMT + " id='GPEAMT' class='GPEAMT' name='GPEAMT'><input type='text' value=" + rslt.GPETYPE + " id='GPETYPE' class='GPETYPE' name='GPETYPE'><input type='text' value=" + rslt.GPCSTYPE + " id='GPCSTYPE' class='GPCSTYPE' name='GPCSTYPE'><input type='text' value=" + rslt.GPCSAMT + " id='GPCSAMT' class='GPCSAMT' name='GPCSAMT'><input type='text' value=" + rslt.GPLBTYPE + " id='GPLBTYPE' class='GPLBTYPE' name='GPLBTYPE'><input type='text' value=" + rslt.GPLBAMT + " id='GPLBAMT' class='GPLBAMT' name='GPLBAMT'><input type='text' value=" + rslt.GPSTYPE + " id='GPSTYPE' class='GPSTYPE' name='GPSTYPE'><input type='text' value=" + rslt.GPWTYPE + " id='GPWTYPE' class='GPWTYPE' name='GPWTYPE'><input type='text' value=" + rslt.GPSCNTYPE + " id='GPSCNTYPE' class='GPSCNTYPE' name='GPSCNTYPE'><input type='text' value=" + rslt.PRDTGID + " id='F_PRDTGID' class='F_PRDTGID' name='F_PRDTGID'><input type='text' value=" + rslt.STATETYPE + " id='F_STATETYPE' class='F_STATETYPE' name='F_STATETYPE'></td></tr>";
                }
                else
                {
                    tabl = tabl + "<tr><td class='hidden1'><input type=checkbox id=STFDIDS class=STFDIDS name=STFDIDS value='' " + count + "  onchange=total() style='width:30px'><input type=text id=boolSTFDIDS class='hide boolSTFDIDS' name=boolSTFDIDS value=''></td><td class='col-lg-0'><input type='text' id='TRANDREFNAME' value='" + rslt.CONTNRNO + "'  class='TRANDREFNAME' readonly='readonly' name='TRANDREFNAME' ></td><td><input type=text id=SIZE value='" + rslt.CONTNRSDESC + "' class=SIZE name=SIZE style='width:40px' readonly='readonly'></td><td ><input type=text id=TRANIDATE value='" + rslt.GIDATE.Value.ToString("dd/MM/yyyy") + "' class=TRANIDATE name=TRANIDATE style='width:70px' readonly='readonly'></td><td ><input type=text id=TRANSDATE value='" + rslt.STRGDATE.Value.ToString("dd/MM/yyyy") + "' class=TRANSDATE name=TRANSDATE readonly='readonly' style='width:70px'></td><td><input type=text id='TRANEDATE' value='" + rslt.GODATE.Value.ToString("dd/MM/yyyy") + "' class='TRANEDATE datepicker' name='TRANEDATE' style='width:70px' onchange='calculation()'></td><td class='hide'><input type=text id=TRANDWGHT value=" + rslt.WGHT + " class=TRANDWGHT name=TRANDWGHT readonly=readonly style='width:70px'></td><td><input type=text id=TRANDSAMT value='0' class=TRANDSAMT name=TRANDSAMT style='width:70px' readonly='readonly'><input type=text id=TRAND_COVID_DISC_AMT value='0' class='TRAND_COVID_DISC_AMT hide' name=TRAND_COVID_DISC_AMT style='width:70px' readonly='readonly'></td><td><input type=text id=TRANDHAMT value='0' class=TRANDHAMT name=TRANDHAMT readonly=readonly style='width:70px'></td><td class='hide'><input type=text value='0' id=TRANDEAMT class='TRANDEAMT' name=TRANDEAMT  readonly=readonly style='width:70px'></td><td class='hide'><input type=text value='0' id=TRANDFAMT class='TRANDFAMT' name=TRANDFAMT  readonly=readonly style='width:70px'></td><td class='hide'><input type=text value='0' id=TRANDPAMT class='TRANDPAMT' name=TRANDPAMT  readonly=readonly style='width:70px'></td><td class='hide'><input type=text id=TRANDAAMT value='0' class='TRANDAAMT' name=TRANDAAMT   readonly=readonly style='width:70px'></td><td><input type=text value='0' id=TRANDNAMT class=TRANDNAMT name=TRANDNAMT readonly='readonly' style='width:70px'> <td class=hide><input type=text id=TRANDVDATE class=TRANDVDATE name=TRANDVDATE value='" + rslt.DODATE.Value.ToString("dd/MM/yyyy") + "'><input type=text id=F_DPAIDNO value='" + rslt.DPAIDNO + "'  class='F_DPAIDNO' name=F_DPAIDNO ><input type=text id=F_DPAIDAMT value=" + rslt.DPAIDAMT + "  class=F_DPAIDAMT name=F_DPAIDAMT ></td>  <td class=hide><input type=text id=CONTNRTID value=0  class=CONTNRTID name=CONTNRTID ><input type=text id=CONTNRSID value=" + rslt.CONTNRSID + "  class=CONTNRSID name=CONTNRSID ><input type=text id=TRANDREFID value=" + rslt.GIDID + "  class=TRANDREFID name=TRANDREFID ><input type=text id=BILLEDID value=" + rslt.BILLEDID + "  class=BILLEDID name=BILLEDID ><input type=text id=BILLEMID value=" + rslt.BILLEMID + "  class=BILLEMID name=BILLEMID ><input type=text id=F_TARIFFMID value=" + rslt.TARIFFMID + "  class=F_TARIFFMID name=F_TARIFFMID ><input type=text id=F_TRANDOTYPE value=" + rslt.TRANDOTYPE + "  class=F_TRANDOTYPE name=F_TRANDOTYPE ><input type=text id=F_CHANAME value='" + rslt.TRANREFNAME + "'  class=F_CHANAME name=F_CHANAME ><input type=text id=F_STMRNAME value='" + rslt.STMRNAME + "'  class=F_STMRNAME name=F_STMRNAME ><input type=text id=F_CHAID value=" + rslt.TRANREFID + "  class=F_CHAID name=F_CHAID ><input type=text id=F_STMRID value=" + rslt.STMRID + "  class=F_STMRID name=F_STMRID ></td><td class=hide><input type=text id=TRANDID value=0  class=TRANDID name=TRANDID ></td><td class='hide'><input type=text id=days value=0  class=days name=days ><input type=text id=NOD value='" + rslt.NOD + "'  class=NOD name=NOD ><input type=text id=TRANDRATE value='" + rslt.TRANDRATE + "'  class=TRANDRATE name=TRANDRATE ></td><td class=hide><input type=text id=RAMT7 value=" + Convert.ToDecimal(rslt.RAMT7) + "  class=RAMT7 name=RAMT7 style='display:none1' ><input type=text id=RAMT1 value=" + Convert.ToDecimal(rslt.RAMT1) + "  class=RAMT1 name=RAMT1 style='display:none1' ><input type=text id=RAMT2 value=" + Convert.ToDecimal(rslt.RAMT2) + "  class=RAMT2 name=RAMT2 style='display:none1' ><input type=text id=RAMT3 value=" + Convert.ToDecimal(rslt.RAMT3) + "  class=RAMT3 name=RAMT3 style='display:none1' ><input type=text id=RAMT4 value=" + Convert.ToDecimal(rslt.RAMT4) + "  class=RAMT4 name=RAMT4 style='display:none1' ><input type=text id=RAMT5 value=" + Convert.ToDecimal(rslt.RAMT5) + "  class=RAMT5 name=RAMT5 style='display:none1' ><input type=text id=RAMT6 value=" + Convert.ToDecimal(rslt.RAMT6) + " class=RAMT6 name=RAMT6 style='display:none1' ></td><td class=hide><input type=text id=SLABMIN6 value='0'  class=SLABMIN6 name=SLABMIN6 style='display:none1' ><input type=text id=SLABMAX6 value='0'  class=SLABMAX6 name=SLABMAX6 style='display:none1' ><input type=text id=SLABMIN value='0'  class=SLABMIN name=SLABMIN style='display:none1' ><input type=text id=SLABMAX value='0'  class=SLABMAX name=SLABMAX style='display:none1' ><input type=text id=SLABMIN1 value='0'  class=SLABMIN1 name=SLABMIN1 style='display:none1' ><input type=text id=SLABMAX1 value='0'  class=SLABMAX1 name=SLABMAX1 style='display:none1' ><input type=text id=SLABMIN2 value='0'  class=SLABMIN2 name=SLABMIN2 style='display:none1' ><input type=text id=SLABMAX2 value='0'  class=SLABMAX2 name=SLABMAX2 style='display:none1' > <input type=text id=SLABMIN3 value='0'  class=SLABMIN3 name=SLABMIN3 style='display:none1' ><input type=text id=SLABMAX3 value='0'  class=SLABMAX3 name=SLABMAX3 style='display:none1' >  <input type=text id=SLABMIN4 value='0'  class=SLABMIN4 name=SLABMIN4 style='display:none1' ><input type=text id=SLABMAX4 value='0'  class=SLABMAX4 name=SLABMAX4 style='display:none1' > <input type=text id=SLABMIN5 value='0'  class=SLABMIN5 name=SLABMIN5 style='display:none1' ><input type=text id=SLABMAX5 value='0'  class=SLABMAX5 name=SLABMAX5 style='display:none1' > </td><td class=hide> <input type=text id=RCAMT7 value=value=" + Convert.ToDecimal(rslt.RCAMT7) + "  class=RCAMT7 name=RCAMT7 style='display:none1' ><input type=text id=RCAMT1 value=value=" + Convert.ToDecimal(rslt.RCAMT1) + "  class=RCAMT1 name=RCAMT1 style='display:none1' ><input type=text id=RCAMT2 value=" + Convert.ToDecimal(rslt.RCAMT2) + "  class=RCAMT2 name=RCAMT2 style='display:none1' ><input type=text id=RCAMT3 value=" + Convert.ToDecimal(rslt.RCAMT3) + "  class=RCAMT3 name=RCAMT3 style='display:none1'><input type=text id=RCAMT4 value=" + Convert.ToDecimal(rslt.RCAMT4) + "  class=RCAMT4 name=RCAMT4 style='display:none1' ><input type=text id=RCAMT5 value=" + Convert.ToDecimal(rslt.RCAMT5) + "  class=RCAMT5 name=RCAMT5 style='display:none1' ><input type=text id=RCAMT6 value=" + Convert.ToDecimal(rslt.RCAMT6) + "  class=RCAMT6 name=RCAMT6 style='display:none1' ></td><td class=hide><input type=text id=RCOL7 value=" + Convert.ToDecimal(rslt.RCOL7) + "  class=RCOL7 name=RCOL7 style='display:none1' ><input type=text id=RCOL1 value=" + Convert.ToDecimal(rslt.RCOL1) + "  class=RCOL1 name=RCOL1 style='display:none1' ><input type=text id=RCOL2 value=" + Convert.ToDecimal(rslt.RCOL2) + " class=RCOL2 name=RCOL2 style='display:none1' ><input type=text id=RCOL3 value=" + Convert.ToDecimal(rslt.RCOL3) + "  class=RCOL3 name=RCOL3 style='display:none1' ><input type=text id=RCOL4 value=" + Convert.ToDecimal(rslt.RCOL4) + "  class=RCOL4 name=RCOL4 style='display:none1' ><input type=text id=RCOL5 value=" + Convert.ToDecimal(rslt.RCOL5) + " class=RCOL5 name=RCOL5 style='display:none1' ><input type=text id=RCOL6 value=" + Convert.ToDecimal(rslt.RCOL6) + "  class=RCOL6 name=RCOL6 style='display:none1' ></td><td class='hide'><input type='text' value=" + rslt.GPAAMT + " id='GPAAMT' class='GPAAMT' name='GPAAMT'><input type='text' value=" + rslt.GPEAMT + " id='GPEAMT' class='GPEAMT' name='GPEAMT'><input type='text' value=" + rslt.GPETYPE + " id='GPETYPE' class='GPETYPE' name='GPETYPE'><input type='text' value=" + rslt.GPCSTYPE + " id='GPCSTYPE' class='GPCSTYPE' name='GPCSTYPE'><input type='text' value=" + rslt.GPCSAMT + " id='GPCSAMT' class='GPCSAMT' name='GPCSAMT'><input type='text' value=" + rslt.GPLBTYPE + " id='GPLBTYPE' class='GPLBTYPE' name='GPLBTYPE'><input type='text' value=" + rslt.GPLBAMT + " id='GPLBAMT' class='GPLBAMT' name='GPLBAMT'><input type='text' value=" + rslt.GPSTYPE + " id='GPSTYPE' class='GPSTYPE' name='GPSTYPE'><input type='text' value=" + rslt.GPWTYPE + " id='GPWTYPE' class='GPWTYPE' name='GPWTYPE'><input type='text' value=" + rslt.GPSCNTYPE + " id='GPSCNTYPE' class='GPSCNTYPE' name='GPSCNTYPE'><input type='text' value=" + rslt.PRDTGID + " id='F_PRDTGID' class='F_PRDTGID' name='F_PRDTGID'><input type='text' value=" + rslt.STATETYPE + " id='F_STATETYPE' class='F_STATETYPE' name='F_STATETYPE'></td></tr>";
                }

                count++;
            }
            tabl = tabl + "</Table>";

            return tabl;

        }

        #endregion


        public JsonResult GetExportGSTRATE(string id)
        {
            var param = id.Split('~');
            var StateType = 0;
            if (param[0] != "")
            { StateType = Convert.ToInt32(param[0]); }
            else
            { StateType = 0; }

            var SlabTId = Convert.ToInt32(param[1]);
            if (StateType == 0)
            {
                var query = context.Database.SqlQuery<VW_EXPORT_SLABTYPE_HSN_DETAIL_ASSGN>("select HSNCODE,CGSTEXPRN,SGSTEXPRN,IGSTEXPRN from VW_EXPORT_SLABTYPE_HSN_DETAIL_ASSGN where SLABTID=" + SlabTId + " order by HSNCODE").ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var query = context.Database.SqlQuery<VW_EXPORT_SLABTYPE_HSN_DETAIL_ASSGN>("select HSNCODE,ACGSTEXPRN as CGSTEXPRN,ASGSTEXPRN as SGSTEXPRN,AIGSTEXPRN as IGSTEXPRN from VW_EXPORT_SLABTYPE_HSN_DETAIL_ASSGN where SLABTID=" + SlabTId + " order by HSNCODE").ToList();

                return Json(query, JsonRequestBehavior.AllowGet);


            }

        } //...end
        public JsonResult Bill_Detail(string id)
        {
            var param = id.Split('-');

            var TARIFFMID = Convert.ToInt32(param[0]);

            var CHRGETYPE = Convert.ToInt32(param[1]);
            var CONTNRSID = Convert.ToInt32(param[2]);
            var CHAID = Convert.ToInt32(param[3]);
            var EOPTID = Convert.ToInt32(param[4]);
            /* INSTEAD OF SLABTID=5 ,,PARAM[5]*/
            if (TARIFFMID == 4)
            {
                //var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (" + Convert.ToInt32(param[4]) + ",14,15,16) and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + "and CHAID=" + CHAID).ToList();
                var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (20) and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and CHAID=" + CHAID ).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABTID,SLABAMT from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + " and SLABTID in (20) and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }


        }//.....end

        [Authorize(Roles = "ESeal_Invoice_Create")]
        public void savedata(FormCollection F_Form)
        {
            using (SCFSERPContext context = new SCFSERPContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        string todaydt = Convert.ToString(DateTime.Now);
                        string todayd = Convert.ToString(DateTime.Now.Date);

                        TransactionMaster transactionmaster = new TransactionMaster();
                        TransactionDetail transactiondetail = new TransactionDetail();
                        //-------Getting Primarykey field--------
                        Int32 TRANMID = Convert.ToInt32(F_Form["masterdata[0].TRANMID"]);
                        Int32 TRANDID = 0;
                        string DELIDS = "";
                        //-----End

                        if (TRANMID != 0)
                        {
                            transactionmaster = context.transactionmaster.Find(TRANMID);
                        }

                        //...........transaction master.............//
                        transactionmaster.TRANMID = TRANMID;
                        transactionmaster.COMPYID = Convert.ToInt32(Session["compyid"]);
                        transactionmaster.SDPTID = 11;
                        transactionmaster.TRANTID = 2;
                        transactionmaster.TRANLMID = Convert.ToInt32(F_Form["TRANLMID"]);
                        transactionmaster.TRANLSID = 0;
                        transactionmaster.TRANLSNO = null;
                        transactionmaster.TRANLMNO = "";//F_Form["masterdata[0].TRANLMNO"].ToString();
                        transactionmaster.TRANLMDATE = DateTime.Now;
                        transactionmaster.TRANLSDATE = DateTime.Now;
                        transactionmaster.TRANNARTN = null;
                        transactionmaster.LMUSRID = Session["CUSRID"].ToString();
                        if (TRANMID == 0)
                        {
                            transactionmaster.CUSRID = Session["CUSRID"].ToString();
                        }
                        transactionmaster.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
                        transactionmaster.PRCSDATE = DateTime.Now;

                        string indate = Convert.ToString(F_Form["masterdata[0].TRANDATE"]);
                        if (indate != null || indate != "")
                        {
                            transactionmaster.TRANDATE = Convert.ToDateTime(indate).Date;
                        }
                        else { transactionmaster.TRANDATE = DateTime.Now.Date; }

                        if (transactionmaster.TRANDATE > Convert.ToDateTime(todayd))
                        {
                            transactionmaster.TRANDATE = Convert.ToDateTime(todayd);
                        }

                        string intime = Convert.ToString(F_Form["masterdata[0].TRANTIME"]);
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

                                    transactionmaster.TRANTIME = Convert.ToDateTime(in_datetime);
                                }
                                else { transactionmaster.TRANTIME = DateTime.Now; }
                            }
                            else
                            {
                                var in_time = intime;
                                var in_date = indate;

                                if ((in_time.Contains(':')) && (in_date.Contains('/')))
                                {
                                    var in_time1 = in_time.Split(':');
                                    var in_date1 = in_date.Split('/');

                                    string in_datetime = in_date1[2] + "-" + in_date1[1] + "-" + in_date1[0] + "  " + in_time1[0] + ":" + in_time1[1] + ":" + in_time1[2];

                                    transactionmaster.TRANTIME = Convert.ToDateTime(in_datetime);
                                }
                                else { transactionmaster.TRANTIME = DateTime.Now; }
                            }
                        }
                        else { transactionmaster.TRANTIME = DateTime.Now; }

                        if (transactionmaster.TRANTIME > Convert.ToDateTime(todaydt))
                        {
                            transactionmaster.TRANTIME = Convert.ToDateTime(todaydt);
                        }

                        //transactionmaster.TRANDATE = Convert.ToDateTime(F_Form["masterdata[0].TRANTIME"]).Date;
                        //transactionmaster.TRANTIME = Convert.ToDateTime(F_Form["masterdata[0].TRANTIME"]);

                        transactionmaster.TRANREFID = Convert.ToInt32(F_Form["masterdata[0].TRANREFID"]);
                        transactionmaster.TRANREFNAME = F_Form["masterdata[0].TRANREFNAME"].ToString();
                        transactionmaster.LCATEID = Convert.ToInt32(F_Form["LCATEID"]);
                        transactionmaster.CATEAID = Convert.ToInt32(F_Form["CATEAID"]);
                        transactionmaster.TRANBTYPE = Convert.ToInt16(F_Form["TRANBTYPE"]);
                        transactionmaster.REGSTRID = Convert.ToInt16(F_Form["REGSTRID"]);
                        transactionmaster.TRANMODE = Convert.ToInt16(F_Form["TRANMODE"]);
                        transactionmaster.TRANMODEDETL = (F_Form["masterdata[0].TRANMODEDETL"]);
                        transactionmaster.TRANGAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANGAMT"]);
                        transactionmaster.TRANNAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANNAMT"]);
                        transactionmaster.TRANROAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANROAMT"]);
                        transactionmaster.TRANREFAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANREFAMT"]);
                        transactionmaster.TRANRMKS = (F_Form["masterdata[0].TRANRMKS"]).ToString();
                        transactionmaster.TRANAMTWRDS = AmtInWrd.ConvertNumbertoWords(F_Form["masterdata[0].TRANNAMT"]);

                        transactionmaster.TRANSAMT = Convert.ToDecimal(F_Form["TRANSAMT"]);
                        transactionmaster.TRANAAMT =  Convert.ToDecimal(F_Form["TRANAAMT"]);
                        transactionmaster.TRANADONAMT = Convert.ToDecimal(F_Form["TRANADONAMT"]);
                        transactionmaster.TRANHAMT = Convert.ToDecimal(F_Form["TRANHAMT"]);
                        transactionmaster.TRANEAMT = Convert.ToDecimal(F_Form["TRANEAMT"]);
                        transactionmaster.TRANFAMT =Convert.ToDecimal(F_Form["TRANFAMT"]);
                        
                        if (F_Form["TRANTCAMT"].Length != 0)
                        { transactionmaster.TRANTCAMT = Convert.ToDecimal(F_Form["TRANTCAMT"]); }
                        else { transactionmaster.TRANTCAMT = 0; }

                        transactionmaster.STRG_HSNCODE = F_Form["STRG_HSN_CODE"].ToString();
                        transactionmaster.HANDL_HSNCODE = F_Form["HANDL_HSN_CODE"].ToString();

                        transactionmaster.STRG_TAXABLE_AMT = Convert.ToDecimal(F_Form["STRG_TAXABLE_AMT"]);
                        transactionmaster.HANDL_TAXABLE_AMT = Convert.ToDecimal(F_Form["HANDL_TAXABLE_AMT"]);

                        transactionmaster.STRG_CGST_EXPRN = Convert.ToDecimal(F_Form["STRG_CGST_EXPRN"]);
                        transactionmaster.STRG_SGST_EXPRN = Convert.ToDecimal(F_Form["STRG_SGST_EXPRN"]);
                        transactionmaster.STRG_IGST_EXPRN = Convert.ToDecimal(F_Form["STRG_IGST_EXPRN"]);
                        transactionmaster.STRG_CGST_AMT = Convert.ToDecimal(F_Form["STRG_CGST_AMT"]);
                        transactionmaster.STRG_SGST_AMT = Convert.ToDecimal(F_Form["STRG_SGST_AMT"]);
                        transactionmaster.STRG_IGST_AMT = Convert.ToDecimal(F_Form["STRG_IGST_AMT"]);

                        transactionmaster.HANDL_CGST_EXPRN = Convert.ToDecimal(F_Form["HANDL_CGST_EXPRN"]);
                        transactionmaster.HANDL_SGST_EXPRN = Convert.ToDecimal(F_Form["HANDL_SGST_EXPRN"]);
                        transactionmaster.HANDL_IGST_EXPRN = Convert.ToDecimal(F_Form["HANDL_IGST_EXPRN"]);
                        transactionmaster.HANDL_CGST_AMT = Convert.ToDecimal(F_Form["HANDL_CGST_AMT"]);
                        transactionmaster.HANDL_SGST_AMT = Convert.ToDecimal(F_Form["HANDL_SGST_AMT"]);
                        transactionmaster.HANDL_IGST_AMT = Convert.ToDecimal(F_Form["HANDL_IGST_AMT"]);

                        transactionmaster.TRANCGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANCGSTAMT"]);
                        transactionmaster.TRANSGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANSGSTAMT"]);
                        transactionmaster.TRANIGSTAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANIGSTAMT"]);

                        transactionmaster.TRAN_COVID_DISC_AMT = Convert.ToDecimal(F_Form["masterdata[0].TRAN_COVID_DISC_AMT"]);

                        transactionmaster.TRANNARTN = Convert.ToString(F_Form["masterdata[0].TRANNARTN"]);
                        transactionmaster.TRANCSNAME = null;
                        string CATEAID = Convert.ToString(F_Form["CATEAID"]);
                        if (CATEAID != "" || CATEAID != null || CATEAID != "0")
                        {
                            transactionmaster.CATEAID = Convert.ToInt32(CATEAID);
                        }
                        else { transactionmaster.CATEAID = 0; }

                        string STATEID = Convert.ToString(F_Form["masterdata[0].STATEID"]);
                        if (STATEID != "" || STATEID != null || STATEID != "0")
                        {
                            transactionmaster.STATEID = Convert.ToInt32(STATEID);
                        }
                        else { transactionmaster.STATEID = 0; }

                        transactionmaster.CATEAGSTNO = Convert.ToString(F_Form["masterdata[0].CATEAGSTNO"]);
                        transactionmaster.TRANIMPADDR1 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR1"]);
                        transactionmaster.TRANIMPADDR2 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR2"]);
                        transactionmaster.TRANIMPADDR3 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR3"]);
                        transactionmaster.TRANIMPADDR4 = Convert.ToString(F_Form["masterdata[0].TRANIMPADDR4"]);

                        transactionmaster.TALLYSTAT = 0;
                        //transactionmaster.IRNNO = "";
                        //transactionmaster.ACKNO = "";
                        //transactionmaster.ACKDT = DateTime.Now;
                        transactionmaster.QRCODEPATH = "";
                        var tranmode = Convert.ToInt16(F_Form["TRANMODE"]);
                        if (tranmode != 2 && tranmode != 3)
                        {
                            transactionmaster.TRANREFNO = "";
                            transactionmaster.TRANREFBNAME = "";
                            transactionmaster.BANKMID = 0;
                            transactionmaster.TRANREFDATE = Convert.ToDateTime(DateTime.Now).Date;
                        }
                        else
                        {
                            transactionmaster.TRANREFNO = (F_Form["masterdata[0].TRANREFNO"]).ToString();
                            transactionmaster.TRANREFBNAME = (F_Form["masterdata[0].TRANREFBNAME"]).ToString();
                            transactionmaster.BANKMID = Convert.ToInt32(F_Form["BANKMID"]);
                            transactionmaster.TRANREFDATE = Convert.ToDateTime(F_Form["masterdata[0].TRANREFDATE"]).Date;

                        }


                        //.................Autonumber............//
                        var regsid = Convert.ToInt32(F_Form["REGSTRID"]);
                        var btype = Convert.ToInt32(F_Form["TRANBTYPE"]);
                        //string format = "SUD/EXP/";
                        string format = "ESL/" ;
                        string btyp = auto_numbr_invoice.GetCateBillType(Convert.ToInt32(transactionmaster.TRANREFID)).ToString();
                        if (btyp == "")
                        {
                            format = format + Session["GPrxDesc"] + "/";
                        }
                        else
                            format = format.Replace("/", "") + Session["GPrxDesc"] + btyp;
                        string billformat = "";
                        string prfx = "";
                        string billprfx = "";
                        int ano = 0;



                        if (TRANMID == 0)
                        {
                            //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.gstexportautonum("transactionmaster", "TRANNO", "16", Session["compyid"].ToString(), "1").ToString());

                            if (regsid == 54 )
                            {

                                //ano = transactionmaster.TRANNO;
                                billformat = "EXP/";
                                //prfx = string.Format(format + "{0:D5}", ano);
                                // transactionmaster.TRANDNO = prfx.ToString();
                            }
                            else if (regsid == 55 )
                            {
                                //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.gstexportautonum("transactionmaster", "TRANNO", "16", Session["compyid"].ToString(), "2").ToString());

                                //ano = transactionmaster.TRANNO;
                                billformat = "BSESL/EXP/";
                                //prfx = string.Format(format + "{0:D5}", ano);
                                // transactionmaster.TRANDNO = prfx.ToString();
                            }


                            //........end of autonumber
                            if (regsid == 54)
                                transactionmaster.TRANNO = Convert.ToInt32(auto_numbr_invoice.gstexportautonum("transactionmaster", "TRANNO", regsid.ToString(), Session["compyid"].ToString(), btype.ToString(), Convert.ToInt32(transactionmaster.TRANREFID)).ToString());
                            //transactionmaster.TRANNO = Convert.ToInt16(auto_numbr_invoice.gstexportautonum("transactionmaster", "TRANNO", "0", Session["compyid"].ToString(), btype.ToString()).ToString());
                            else
                                transactionmaster.TRANNO = Convert.ToInt32(auto_numbr_invoice.gstexportautonum_BS("transactionmaster", "TRANNO", "0", Session["compyid"].ToString(), btype.ToString(), Convert.ToInt32(transactionmaster.TRANREFID)).ToString());
                            ano = transactionmaster.TRANNO;
                            prfx = string.Format(format + "{0:D5}", ano);
                            billprfx = string.Format(billformat + "{0:D5}", ano);
                            transactionmaster.TRANDNO = prfx.ToString();
                            transactionmaster.TRANBILLREFNO = billprfx.ToString();
                            context.transactionmaster.Add(transactionmaster);
                            context.SaveChanges();
                            TRANMID = transactiondetail.TRANMID;
                        }
                        else
                        {
                            //transactionmaster.REGSTRID = Convert.ToInt16(F_Form["masterdata[0].REGSTRID"]);
                            //transactionmaster.TRANMODE = Convert.ToInt16(F_Form["TRANMODE"]);
                            context.Entry(transactionmaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            // TRANMID = transactiondetail.TRANMID;
                        }


                        //-------------transaction Details
                        string[] F_TRANDID = F_Form.GetValues("TRANDID");
                        //string[] TRANDREFNO = F_Form.GetValues("TRANDREFNO");
                        string[] TRANDREFNAME = F_Form.GetValues("TRANDREFNAME");
                        string[] TRANIDATE = F_Form.GetValues("TRANIDATE");
                        string[] TRANSDATE = F_Form.GetValues("TRANSDATE");
                        string[] TRANEDATE = F_Form.GetValues("TRANEDATE");
                        string[] STFDIDS = F_Form.GetValues("STFDIDS");
                        string[] STFDID = F_Form.GetValues("STFDID");
                        string[] boolSTFDIDS = F_Form.GetValues("boolSTFDIDS");
                        string[] TRANDHAMT = F_Form.GetValues("TRANDHAMT");
                        string[] TRANDEAMT = F_Form.GetValues("TRANDEAMT");
                        string[] TRANDFAMT = F_Form.GetValues("TRANDFAMT");
                        string[] TRANDPAMT = F_Form.GetValues("TRANDPAMT");
                        string[] TRANDNAMT = F_Form.GetValues("TRANDNAMT");
                        string[] TRANDSAMT = F_Form.GetValues("TRANDSAMT");
                        string[] TRANDNOP = F_Form.GetValues("TRANDNOP");
                        string[] TRANDQTY = F_Form.GetValues("TRANDQTY");
                        string[] TRANDREFID = F_Form.GetValues("TRANDREFID");
                        string[] RAMT1 = F_Form.GetValues("RAMT1");
                        string[] RAMT2 = F_Form.GetValues("RAMT2");
                        string[] RAMT3 = F_Form.GetValues("RAMT3");
                        string[] RAMT4 = F_Form.GetValues("RAMT4");
                        string[] RAMT5 = F_Form.GetValues("RAMT5");
                        string[] RAMT6 = F_Form.GetValues("RAMT6");
                        string[] RCAMT1 = F_Form.GetValues("RCAMT1");
                        string[] RCAMT2 = F_Form.GetValues("RCAMT2");
                        string[] RCAMT3 = F_Form.GetValues("RCAMT3");
                        string[] RCAMT4 = F_Form.GetValues("RCAMT4");
                        string[] RCAMT5 = F_Form.GetValues("RCAMT5");
                        string[] RCAMT6 = F_Form.GetValues("RCAMT6");
                        string[] RCOL1 = F_Form.GetValues("RCOL1");
                        string[] RCOL2 = F_Form.GetValues("RCOL2");
                        string[] RCOL3 = F_Form.GetValues("RCOL3");
                        string[] RCOL4 = F_Form.GetValues("RCOL4");
                        string[] RCOL5 = F_Form.GetValues("RCOL5");
                        string[] RCOL6 = F_Form.GetValues("RCOL6");
                        string[] days = F_Form.GetValues("days");
                        string[] TRAND_COVID_DISC_AMT = F_Form.GetValues("TRAND_COVID_DISC_AMT");

                        // var transamt = 0.00; var traneamt = 0; var tranfamt = 0; var tranpamt = 0; var tranhamt = 0; var tranaamt = 0; var trantcamt = 0;
                        for (int count = 0; count < boolSTFDIDS.Count(); count++)
                        {
                            if (boolSTFDIDS[count] == "true")
                            {
                                TRANDID = Convert.ToInt32(F_TRANDID[count]);
                                var boolSTFDID = Convert.ToString(boolSTFDIDS[count]);
                                if (TRANDID != 0 && boolSTFDID == "true")
                                {
                                    transactiondetail = context.transactiondetail.Find(TRANDID);
                                }
                                transactiondetail.TRANMID = transactionmaster.TRANMID;
                                transactiondetail.TRANDREFNO = "";// (TRANDREFNO[count]).ToString();
                                transactiondetail.TRANDREFNAME = (TRANDREFNAME[count]).ToString();
                                transactiondetail.TRANDREFID =  Convert.ToInt32(TRANDREFID[count]);
                                transactiondetail.TRANIDATE = Convert.ToDateTime(TRANIDATE[count]);
                                transactiondetail.TRANSDATE = Convert.ToDateTime(TRANSDATE[count]);
                                transactiondetail.TRANEDATE = Convert.ToDateTime(TRANEDATE[count]);
                                transactiondetail.TRANVDATE = DateTime.Now;
                                transactiondetail.TRANDSAMT = Convert.ToDecimal(TRANDSAMT[count]);
                                transactiondetail.TRANDHAMT = Convert.ToDecimal(TRANDHAMT[count]);
                                transactiondetail.TRANDEAMT = Convert.ToDecimal(TRANDEAMT[count]);
                                transactiondetail.TRANDFAMT = Convert.ToDecimal(TRANDFAMT[count]);
                                transactiondetail.TRANDPAMT = 0;// Convert.ToDecimal(TRANDPAMT[count]);
                                transactiondetail.TRANDNAMT = Convert.ToDecimal(TRANDNAMT[count]);
                                transactiondetail.TRANDNOP = 0;// Convert.ToDecimal(TRANDNOP[count]);
                                transactiondetail.TRANDQTY = 0;// Convert.ToDecimal(TRANDQTY[count]);
                                transactiondetail.TARIFFMID = Convert.ToInt32(F_Form["TARIFFMID"]);
                                transactiondetail.TRANDRATE = 0;
                               // transactiondetail.STFDID = Convert.ToInt32(STFDID[count]);
                                transactiondetail.TRANDGAMT = Convert.ToDecimal(TRANDNAMT[count]);
                                transactiondetail.BILLEDID = 0;
                                transactiondetail.RCOL1 = Convert.ToDecimal(RCOL1[count]);
                                transactiondetail.RCOL2 = Convert.ToDecimal(RCOL2[count]);
                                transactiondetail.RCOL3 = Convert.ToDecimal(RCOL3[count]);
                                transactiondetail.RCOL4 = Convert.ToDecimal(RCOL4[count]);
                                transactiondetail.RCOL5 = Convert.ToDecimal(RCOL5[count]);
                                transactiondetail.RCOL6 =  Convert.ToDecimal(RCOL6[count]);
                                transactiondetail.RCOL7 = 0;// Convert.ToDecimal(RCOL7[count]);
                                transactiondetail.RAMT1 = Convert.ToDecimal(RAMT1[count]);
                                transactiondetail.RAMT2 = Convert.ToDecimal(RAMT2[count]);
                                transactiondetail.RAMT3 = Convert.ToDecimal(RAMT3[count]);
                                transactiondetail.RAMT4 = Convert.ToDecimal(RAMT4[count]);
                                transactiondetail.RAMT5 = Convert.ToDecimal(RAMT5[count]);
                                transactiondetail.RAMT6 = Convert.ToDecimal(RAMT6[count]);
                                transactiondetail.RCAMT1 = Convert.ToDecimal(RCAMT1[count]);
                                transactiondetail.RCAMT2 = Convert.ToDecimal(RCAMT2[count]);
                                transactiondetail.RCAMT3 = Convert.ToDecimal(RCAMT3[count]);
                                transactiondetail.RCAMT4 = Convert.ToDecimal(RCAMT4[count]);
                                transactiondetail.RCAMT5 =  Convert.ToDecimal(RCAMT5[count]);
                                transactiondetail.RCAMT6 =  Convert.ToDecimal(RCAMT6[count]);
                                transactiondetail.SLABTID = 0;
                                transactiondetail.TRANYTYPE = 0;
                                transactiondetail.TRANDWGHT = 0;
                                transactiondetail.TRANDAID = 0;
                                transactiondetail.SBDID = 0;
                                transactiondetail.TRAND_COVID_DISC_AMT = Convert.ToDecimal(TRAND_COVID_DISC_AMT[count]);
                                //transamt = transamt + Convert.ToDecimal(TRANDSAMT[count]);//
                                //traneamt = traneamt + Convert.ToInt32(TRANDEAMT[count]);
                                //tranfamt = tranfamt + Convert.ToInt32(TRANDFAMT[count]);
                                //tranhamt = tranhamt + Convert.ToInt32(TRANDHAMT[count]);
                                //tranpamt = tranpamt + Convert.ToInt32(TRANDPAMT[count]);
                                //trantcamt = trantcamt + Convert.ToInt32(F_Form["TRANTCAMT"]);
                                if (Convert.ToInt32(TRANDID) == 0)
                                {
                                    context.transactiondetail.Add(transactiondetail);
                                    context.SaveChanges();
                                    TRANDID = transactiondetail.TRANDID;
                                }
                                else
                                {
                                    transactiondetail.TRANDID = TRANDID;
                                    context.Entry(transactiondetail).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                }//..............end


                                //,..............update master..........//
                                //  context.Database.ExecuteSqlCommand("UPDATE TRANSACTIONMASTER SET TRANSAMT=" + Convert.ToDecimal(transamt) + ",TRANHAMT=" + Convert.ToDecimal(tranhamt) + ",TRANEAMT=" + Convert.ToDecimal(traneamt) + ",TRANFAMT=" + Convert.ToDecimal(tranfamt) + ",TRANPAMT=" + Convert.ToDecimal(tranpamt) + ",TRANTCAMT=" + Convert.ToDecimal(trantcamt) + " WHERE TRANMID="+TRANMID+"");

                                DELIDS = DELIDS + "," + TRANDID.ToString();
                                // traneamt = traneamt + Convert.ToInt32(TRANDSAMT[count]);
                            }
                        }

                        //context.Entry(transactionmaster).State = System.Data.Entity.EntityState.Modified;
                        //context.Entry(transactionmaster).State = System.Data.Entity.EntityState.Modified;
                        //context.Entry(transactionmaster).State = System.Data.Entity.EntityState.Modified;
                        //context.Entry(transactionmaster).State = System.Data.Entity.EntityState.Modified;




                        //-------delete transaction master factor-------//
                        context.Database.ExecuteSqlCommand("DELETE FROM transactionmasterfactor WHERE tranmid=" + TRANMID);

                        //Transaction Type Master-------//

                        TransactionMasterFactor transactionmasterfactors = new TransactionMasterFactor();
                        string[] DEDEXPRN = F_Form.GetValues("CFEXPR");
                        string[] TAX1 = F_Form.GetValues("TAX");
                        string[] DEDMODE = F_Form.GetValues("CFMODE");
                        string[] DEDTYPE = F_Form.GetValues("CFTYPE");
                        string[] DORDRID = F_Form.GetValues("DORDRID");
                        string[] DEDNOS = F_Form.GetValues("DEDNOS");
                        string[] DEDVALUE = F_Form.GetValues("CFAMOUNT");
                        string[] CFAMOUNT = F_Form.GetValues("CFAMOUNT");
                        string[] CFDESC = F_Form.GetValues("CFDESC");

                        if (CFDESC != null)//if (DORDRID != null)
                        {
                            for (int count2 = 0; count2 < CFDESC.Count(); count2++)
                            {

                                transactionmasterfactors.TRANMID = transactionmaster.TRANMID;
                                transactionmasterfactors.DORDRID = Convert.ToInt16(DORDRID[count2]);
                                transactionmasterfactors.DEDMODE = DEDMODE[count2].ToString();
                                transactionmasterfactors.DEDVALUE = Convert.ToDecimal(DEDVALUE[count2]);
                                transactionmasterfactors.DEDTYPE = Convert.ToInt16(DEDTYPE[count2]);
                                transactionmasterfactors.DEDEXPRN = Convert.ToDecimal(DEDEXPRN[count2]);
                                transactionmasterfactors.CFID = Convert.ToInt32(TAX1[count2]);
                                transactionmasterfactors.DEDCFDESC = CFDESC[count2];
                                transactionmasterfactors.DEDNOS = Convert.ToDecimal(DEDNOS[count2]);
                                transactionmasterfactors.CFOPTN = 0;
                                transactionmasterfactors.DEDORDR = 0;
                                context.transactionmasterfactor.Add(transactionmasterfactors);
                                context.SaveChanges();
                            }
                        }
                        context.Database.ExecuteSqlCommand("DELETE FROM transactiondetail  WHERE TRANMID=" + TRANMID + " and  TRANDID NOT IN(" + DELIDS.Substring(1) + ")");
                        trans.Commit(); Response.Redirect("Index");
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        trans.Rollback();
                        //Response.Write("Sorry!!An Error Ocurred...");
                        Response.Redirect("/Error/AccessDenied");
                    }
                }
            }

        }
        public JsonResult RATECARD(string id)
        {
            var param = id.Split('-');
            var TARIFFMID = Convert.ToInt32(param[0]);
            var CHRGETYPE = Convert.ToInt32(param[1]);
            var CONTNRSID = Convert.ToInt32(param[2]);
            var CHAID = Convert.ToInt32(param[3]);
            var SLABMIN = Convert.ToInt32(param[4]);
            if (TARIFFMID == 4)
            {
                var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID in (20) and HTYPE=0 and SDTYPE=1 and SLABMIN <= " + SLABMIN + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and CHAID=" + CHAID + " order by SLABMIN").ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID  in (20) and HTYPE=0 and SDTYPE=1 and SLABMIN <= " + SLABMIN + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " order by SLABMIN").ToList();

                return Json(query, JsonRequestBehavior.AllowGet);


            }

        } //...end

        public JsonResult GetHandlingAmt(string id)
        {
            var param = id.Split('-');
            var TARIFFMID = Convert.ToInt32(param[0]);
            var CHRGETYPE = Convert.ToInt32(param[1]);
            var CONTNRSID = Convert.ToInt32(param[2]);
            var CHAID = Convert.ToInt32(param[3]);
            var SLABMIN = Convert.ToInt32(param[4]);
            if (TARIFFMID == 4)
            {
                var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID in (21) and HTYPE=0 and SDTYPE=0 and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and CHAID=" + CHAID + " order by SLABMIN").ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var query = context.Database.SqlQuery<VW_EXPORT_RATECARDMASTER_FLX_ASSGN>("select SLABAMT,SLABMIN,SLABMAX from VW_EXPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID  in (21) and HTYPE=0 and SDTYPE=0 and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " order by SLABMIN").ToList();

                return Json(query, JsonRequestBehavior.AllowGet);


            }

        } //...end
        public JsonResult COVIDRATECARD(string id)
        {
            var param = id.Split('~');
            //
            var zsdate = param[0];
            var zedate = param[1];
            var ztariffmid = 0;

            if ((param[2]) != "") { ztariffmid = Convert.ToInt32(param[2]); }

            var zstmrmid = Convert.ToInt32(param[3]);
            var zchrgtype = Convert.ToInt32(param[4]);
            var zcontnrsid = Convert.ToInt32(param[5]);
            var zotype = Convert.ToInt32(param[7]);
            var zchrgdate = param[6];
            var xcovidsdate = Convert.ToDateTime(Session["COVIDSDATE"]).ToString("dd/MM/yyyy").Split('-');
            var zcovidsdate = xcovidsdate[1] + '-' + xcovidsdate[0] + '-' + xcovidsdate[2];

            var xcovidedate = Convert.ToDateTime(Session["COVIDEDATE"]).ToString("dd/MM/yyyy").Split('-');
            var zcovidedate = xcovidedate[1] + '-' + xcovidedate[0] + '-' + xcovidedate[2];

            using (var e = new CFSExportEntities())
            {
                //var query = context.Database.SqlQuery<z_pr_New_Import_Covid_Slab_Assgn_Result>("z_pr_New_Import_Covid_Slab_Assgn @PKUSRID = '" + Session["CUSRID"] + "', @PSDATE = '" + zsdate + "', @PEDATE = '" + zedate + "', @PTARIFFMID = " + ztariffmid + ", @PSTMRID = " + zstmrmid + ", @PCHRGETYPE = " + zchrgtype + ", @PSLABTID = 2, @PSLABMIN = 0, @PCONTNRSID = " + zcontnrsid + ", @PSLABHTYPE = 0, @PCHRGDATE = '" + zchrgdate + "' @PCDate1 = '" + zcovidsdate + "', @PCDate2 = '" + zcovidedate + "'").ToList();
                var query = context.Database.SqlQuery<z_pr_New_Export_Covid_Slab_Assgn_Result>("z_pr_New_Export_Covid_Slab_Assgn @PKUSRID = '" + Session["CUSRID"].ToString() + "', @PSDATE = '" + zsdate + "', @PEDATE = '" + zedate + "', @PTARIFFMID = " + ztariffmid + ", @PSTMRID = " + zstmrmid + ", @PCHRGETYPE = " + zchrgtype + ", @PSLABTID = 2, @PSLABMIN = 0, @PCONTNRSID = " + zcontnrsid + ", @PSLABHTYPE = " + zotype + ", @PCHRGDATE = '" + zchrgdate + "', @PCDate1 = '" + zcovidsdate + "', @PCDate2 = '" + zcovidedate + "'").ToList();
                //var a = 1;
                return Json(query, JsonRequestBehavior.AllowGet);
            }




        } //...end


        //...............Delete Row.............
        [Authorize(Roles = "ESeal_Invoice_Delete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);

            if (temp.Equals("PROCEED"))
            {
                TransactionMaster transactionmaster = context.transactionmaster.Find(Convert.ToInt32(id));
                context.transactionmaster.Remove(transactionmaster);
                context.SaveChanges();
                Response.Write("Deleted successfully...");
            }
            else
                Response.Write(temp);

        }//-----End of Delete Row
         //..........................Printview...
        [Authorize(Roles = "ESeal_Invoice_Print")]
        public void PrintView(string id)
        {

            var param = id.Split(';');
            // Response.Write(@"10.10.5.5"); Response.End();
            //  ........delete TMPRPT...//

            var ids = Convert.ToInt32(param[0]);
            //var rpttype = Convert.ToInt32(param[1]);
            //var gsttype = Convert.ToInt32(param[2]);
            //var billedto = Convert.ToInt32(param[3]);
            var strHead = "E-Seal Charges";  //param[4].ToString();           
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "ESEALINVOICE", Convert.ToInt32(ids), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;



                // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");


                //........Get TRANPCOUNT...//
                var Query = context.Database.SqlQuery<int>("select TRANPCOUNT from transactionmaster where TRANMID=" + ids).ToList();
                var PCNT = 0;

                if (Query.Count() != 0) { PCNT = Query[0]; }
                var TRANPCOUNT = ++PCNT;
                // Response.Write(++PCNT);
                // Response.End();

                context.Database.ExecuteSqlCommand("UPDATE transactionmaster SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + ids);

                string RptNamePath = "";
                //gsttype = 1;
                
                RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_ESeal_Invoice_Rpt.RPT";
                // cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] +  "NonPnr_Invoice_Group_rpt.RPT");

                cryRpt.Load(RptNamePath);
                cryRpt.RecordSelectionFormula = "{VW_TRANSACTION_ESEAL_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_TRANSACTION_ESEAL_CRY_PRINT_ASSGN.TRANMID} = " + ids;



                string paramName = "@FTHANDLING";

                for (int i = 0; i < cryRpt.DataDefinition.FormulaFields.Count; i++)
                    if (cryRpt.DataDefinition.FormulaFields[i].FormulaName == "{" + paramName + "}")
                        cryRpt.DataDefinition.FormulaFields[i].Text = "'" + strHead + "'";


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
    }

    

}