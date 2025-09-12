using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using scfs_erp.Context;

using scfs.Data;
using scfs_erp.Helper;
using scfs_erp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;


using static scfs_erp.Models.EInvoice;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;

namespace scfs_erp.Controllers.Bond
{
    [SessionExpire]
    public class ExBondVTInvoiceController : Controller
    {
        // GET: ExBondVTInvoice
        BondContext context = new BondContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

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
            //...........Tax type......//
            List<SelectListItem> selectedTAXTYPE = new List<SelectListItem>();
            if (Request.Form.Get("F_TAXTYPE") != null)
            {
                Session["F_TAXTYPE"] = Request.Form.Get("F_TAXTYPE");
                if (Convert.ToInt32(Session["F_TAXTYPE"]) == 1)
                {
                    SelectListItem selectedItemTAX = new SelectListItem { Text = "TAXABLE", Value = "1", Selected = true };
                    selectedTAXTYPE.Add(selectedItemTAX);
                    selectedItemTAX = new SelectListItem { Text = "BILL OF SUPPLY", Value = "2", Selected = false };
                    selectedTAXTYPE.Add(selectedItemTAX);

                }
                else
                {

                    SelectListItem selectedItemTAX = new SelectListItem { Text = "TAXABLE", Value = "1", Selected = false };
                    selectedTAXTYPE.Add(selectedItemTAX);
                    selectedItemTAX = new SelectListItem { Text = "BILL OF SUPPLY", Value = "2", Selected = true };
                    selectedTAXTYPE.Add(selectedItemTAX);

                }
            }
            else
            {
                Session["F_TAXTYPE"] = 1;
                SelectListItem selectedItemTAX = new SelectListItem { Text = "TAXABLE", Value = "1", Selected = true };
                selectedTAXTYPE.Add(selectedItemTAX);
                selectedItemTAX = new SelectListItem { Text = "BILL OF SUPPLY", Value = "2", Selected = false };
                selectedTAXTYPE.Add(selectedItemTAX);

            }
            
            ViewBag.F_TAXTYPE = selectedTAXTYPE;

            DateTime sd = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;

            DateTime ed = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;
            
            return View();

        }//...End of index grid

        //[Authorize(Roles = "ExBondVTInvoiceEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ExBondVTInvoice/GSTForm/" + id);
        }


        //[Authorize(Roles = "ExBondVTInvoiceCreate")]
        public ActionResult GSTForm(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            BondTransactionMaster tab = new BondTransactionMaster();
            BondTransactionMD vm = new BondTransactionMD();

            context.Database.ExecuteSqlCommand("DELETE FROM tmp_usr_exbond_vtinv_add_dtl WHERE userid ='" + Session["CUSRID"] + "'");
            //..........................................Dropdown data.........................//
            ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6 && m.DISPSTATUS == 0).OrderBy(m => m.CATENAME), "CATEID", "CATENAME");
            ViewBag.F_TARIFFMID = new SelectList(context.bondtariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(m => m.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.TARIFFMID = new SelectList(context.bondtariffmasters.Where(x => x.DISPSTATUS == 0).OrderBy(m => m.TARIFFMDESC), "TARIFFMID", "TARIFFMDESC");
            ViewBag.TRANMODE = new SelectList(context.bondtransactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL");
            ViewBag.SBMDATE = DateTime.Now;
            ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC");
            ViewBag.TRANLMID = new SelectList("");
            ViewBag.F_YRDID = new SelectList(context.bondyardmasters, "YRDID", "YRDDESC");
            var mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Operation_Types ").ToList();
            ViewBag.F_OPRN = new SelectList(mtqry, "dval", "dtxt").ToList();
            ViewBag.F_CNTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID > 1).Where(x => x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");
            ViewBag.SLABTID = new SelectList("");// SelectList(context.bondslabtypemasters.Where(x => x.DISPSTATUS == 0), "SLABTID", "SLABTDESC");
            ViewBag.F_SLABTID = new SelectList("");// SelectList(context.bondslabtypemasters.Where(x => x.DISPSTATUS == 0), "SLABTID", "SLABTDESC");
            ViewBag.F_PERIODTID = new SelectList("");
            ViewBag.F_HNDLG = new SelectList("");
            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_ExBond_VT_Handling_Types").ToList();
            ViewBag.F_HNDLG = new SelectList(mtqry, "dval", "dtxt").ToList();
            ViewBag.PERIODTID = new SelectList("");
            ViewBag.BCATEAID = new SelectList("");
            ViewBag.PRDTGID = new SelectList(context.bondproductgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");

            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_ExBond_Nos_VTInvoice 0 ").ToList();
            ViewBag.F_EBNDID = new SelectList(mtqry, "dval", "dtxt").ToList();

            
            ViewBag.F_VTDID = new SelectList("").ToList();

            //...........Bill type......//
            List<SelectListItem> selectedBILLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "FCL", Value = "1", Selected = true };
            selectedBILLTYPE.Add(selectedItemDSP);
            //selectedItemDSP = new SelectListItem { Text = "LCL", Value = "0", Selected = false };
            //selectedBILLTYPE.Add(selectedItemDSP);
            ViewBag.TRANBTYPE = selectedBILLTYPE;
            ViewBag.F_TRANBTYPE = selectedBILLTYPE;
            //....end

            //...........Tax type......//
            List<SelectListItem> selectedTAXTYPE = new List<SelectListItem>();
            SelectListItem selectedItemTAX = new SelectListItem { Text = "TAXABLE", Value = "1", Selected = true };
            selectedTAXTYPE.Add(selectedItemTAX);
            selectedItemTAX = new SelectListItem { Text = "BILL OF SUPPLY", Value = "2", Selected = false };
            selectedTAXTYPE.Add(selectedItemTAX);
            ViewBag.F_TAXTYPE = selectedTAXTYPE;

            //............Billed to....//
            //ViewBag.REGSTRID = new SelectList(context.Bond_Invoice_Register.Where(x => x.REGSTRID != 1).Where(x => x.REGSTRID != 2).Where(x => x.REGSTRID != 6).Where(x => x.REGSTRID != 46).Where(x => x.REGSTRID != 47).Where(x => x.REGSTRID != 48).Where(x => x.REGSTRID != 51).Where(x => x.REGSTRID != 52).Where(x => x.REGSTRID != 53), "REGSTRID", "REGSTRDESC");
            ViewBag.REGSTRID = new SelectList(context.Bond_Invoice_Register.Where(x => x.REGSTRID == 16 || x.REGSTRID == 17 || x.REGSTRID == 18), "REGSTRID", "REGSTRDESC", Convert.ToInt32(Session["REGSTRID"]));
            //.....end

            //........display status.........//
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemDISP = new SelectListItem { Text = "In Books", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItemDISP);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            //....end
            if (id != 0)//....Edit Mode
            {
                tab = context.bondtransactionmaster.Find(id);//find selected record
                ViewBag.TRANLMNO = tab.TRANLMNO;
                //...................................Selected dropdown value..................................//
                ViewBag.LCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME", tab.CHAID);
                ViewBag.BANKMID = new SelectList(context.bankmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.BANKMDESC), "BANKMID", "BANKMDESC", tab.BANKMID);
                ViewBag.TRANMODE = new SelectList(context.bondtransactionmodemaster.Where(x => x.TRANMODE > 0), "TRANMODE", "TRANMODEDETL", tab.TRANMODE);
                ViewBag.REGSTRID = new SelectList(context.Bond_Invoice_Register.Where(x => x.REGSTRID == tab.REGSTRID), "REGSTRID", "REGSTRDESC", tab.REGSTRID);
                
                string vqry = "exec pr_Get_ExBond_VT_Nos_Invoice @vtid=0,@ebndid= " + tab.TRANLMID + ",@tranmid= " + tab.TRANMID;
                 mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>(vqry).ToList();
                int defvtid = 0;
                int defebndid = 0;
                if (mtqry.Count > 0)
                {
                    defvtid= Convert.ToInt32(mtqry[0].dval);
                    ViewBag.TRANLMID = new SelectList(mtqry, "dval", "dtxt", defvtid).ToList();
                    ExBondVehicleTicket ebvt = new ExBondVehicleTicket();
                    ebvt = context.exbondvtdtls.Find(defvtid);  
                    mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_ExBond_Nos_VTInvoice " + tab.TRANMID).ToList();
                    if (mtqry.Count > 0)
                    {
                        defebndid = Convert.ToInt32(mtqry[0].dval);
                        ViewBag.F_EBNDID = new SelectList(mtqry, "dval", "dtxt", defebndid).ToList();
                    } 
                    
                }
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


                //...........Tax type......//
                List<SelectListItem> selectedTAXTYPE0 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.TAXTYPE) == 1)
                {
                    
                    SelectListItem selectedItemTAX0 = new SelectListItem { Text = "TAXABLE", Value = "1", Selected = true };
                    selectedTAXTYPE0.Add(selectedItemTAX0);
                    //selectedItemTAX0 = new SelectListItem { Text = "BILL OF SUPPLY", Value = "2", Selected = false };
                    //selectedTAXTYPE0.Add(selectedItemTAX0);                    
                }
                else
                {
                    SelectListItem selectedItemTAX0 = new SelectListItem { Text = "BILL OF SUPPLY", Value = "2", Selected = true };
                    selectedTAXTYPE0.Add(selectedItemTAX0);
                    //selectedItemTAX0 = new SelectListItem { Text = "TAXABLE", Value = "0", Selected = false };
                    //selectedTAXTYPE0.Add(selectedItemTAX0);
                }
                ViewBag.F_TAXTYPE = selectedTAXTYPE0;
                ////....................Bill type.................//
                //List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
                //if (Convert.ToInt32(tab.TRANBTYPE) == 1)
                //{
                //    SelectListItem selectedItemGPTY = new SelectListItem { Text = "STUFF", Value = "1", Selected = true };
                //    selectedBILLYPE.Add(selectedItemGPTY);
                //    selectedItemGPTY = new SelectListItem { Text = "LCL", Value = "", Selected = false };
                //    selectedBILLYPE.Add(selectedItemGPTY);

                //}
                //else
                //{
                //    SelectListItem selectedItemGPTY = new SelectListItem { Text = "STUFF", Value = "1", Selected = false };
                //    selectedBILLYPE.Add(selectedItemGPTY);
                //    selectedItemGPTY = new SelectListItem { Text = "LCL", Value = "", Selected = true };
                //    selectedBILLYPE.Add(selectedItemGPTY);

                //}
                //ViewBag.TRANBTYPE = selectedBILLYPE;
                ////..........end

                vm.bndmasterdata = context.bondtransactionmaster.Where(det => det.TRANMID == id).ToList();
                vm.bnddetaildata = context.bondtransactiondetail.Where(det => det.TRANMID == id).ToList();
                vm.bndcostfactor = context.bondtransactionmasterfactor.Where(det => det.TRANMID == id).ToList();
                string cusrid = Session["CUSRID"].ToString();                
                vm.bondvtinvcdata = context.Database.SqlQuery<pr_Ex_Bond_VT_Invoice_BondNO_Grid_Assgn_Result>("exec pr_Ex_Bond_VT_Invoice_BondNO_Grid_Assgn @userid = '" + cusrid + "', @PVTDID=" + tab.TRANLMID + ", @PTRANMID=" + id + ", @PEBNDID=0, @PBNDID=0").ToList();//........procedure  for edit mode details data

            }
            return View(vm);
        }

        public JsonResult GetVTList(string ids)
        {
            var param = ids.Split('~');
            int tranmid = 0;
            int vtdid = 0;
            int ebndid = 0;
            if (param[0] != "" && param[0] != "undefined")
                tranmid = Convert.ToInt32(param[0]);
            if (param[1] != "" && param[1] != "undefined")
                ebndid = Convert.ToInt32(param[1]);
            if (param[2] != "" && param[2] != "undefined")
                vtdid = Convert.ToInt32(param[2]);

            if (ebndid > 0)
            {
                string vqry = "exec pr_Get_ExBond_VT_Nos_Invoice @vtid=" + vtdid + ",@ebndid= " + ebndid+ ",@tranmid= " + tranmid;
                var mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>(vqry).ToList();

                return Json(mtqry, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }


        }
        
        public JsonResult GetCatLocationDetail(int CATEID)
        {
            if (CATEID > 0)
            {

                var result = (from r in context.categoryaddressdetails
                              where r.CATEID == CATEID
                              select new { r.CATEAID, r.CATEATYPEDESC }).Distinct();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }


        }
        public JsonResult GetCatAddressDetail(int CATEAID)
        {
            if (CATEAID > 0)
            {
                var result = (from r in context.categoryaddressdetails
                              where r.CATEAID == CATEAID
                              select new { r.STATEID, r.CATEAGSTNO, r.CATEAADDR1, r.CATEAADDR2, r.CATEAADDR3, r.CATEAADDR4 }).Distinct();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }


        }
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new BondEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_ExBondVTInvoice(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), 0, 1, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(Session["F_TAXTYPE"]));
                var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANNAMT.ToString(), d.ACKNO, d.DISPSTATUS, d.GSTAMT.ToString(), d.TRANMID.ToString() }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //Get Slab based Tariff
        public JsonResult GetSlab(string id)
        {

            var tid = Convert.ToInt32(id);
            var result = context.Database.SqlQuery<BondSlabTypeMaster>("select distinct a.* from BondSlabTypeMaster a(nolock) join BondSlabMaster b (nolock) on a.SLABTID = b.SLABTID where a.dispstatus=0 and b.TARIFFMID =" + tid + "").ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //---End 
        //Get Slab based Tariff
        public JsonResult GetPeriodTypes(string id)
        {

            var tid = Convert.ToInt32(id);
            var mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_Bond_Tariff_Period_Types @tariffmid=" + id).ToList();
            var result = new SelectList(mtqry, "dval", "dtxt").ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //---End 

        public string Detail(string ids)
        {
            var tabl = "";
            if (ids == "" || ids == null)
            {
                tabl = "";
            }
            else
            {
                var param = ids.Split('~');
                var BNDID = 0;
                if (Convert.ToString(param[0]) != "" && Convert.ToString(param[0]) != "undefined")
                { BNDID = Convert.ToInt32(param[0]); }

                var TRANMID = 0;
                if (Convert.ToString(param[1]) != "")
                { TRANMID = Convert.ToInt32(param[1]); }



                var BCHAID = 0;
                if (Convert.ToString(param[4]) != "")
                { BCHAID = Convert.ToInt32(param[4]); }

                var BAID = 0;
                if (Convert.ToString(param[5]) != "")
                { BAID = Convert.ToInt32(param[5]); }

                var TARIFFMID = 0;
                if (Convert.ToString(param[6]) != "")
                { TARIFFMID = Convert.ToInt32(param[6]); }

                var SLABTID = 0;
                if (Convert.ToString(param[7]) != "" && Convert.ToString(param[7]) != "undefined")
                { SLABTID = Convert.ToInt32(param[7]); }

                var CHRGETYPE = 0;
                if (Convert.ToString(param[8]) != "" && param[8] != "null")
                { CHRGETYPE = Convert.ToInt32(param[8]); }

                var PRDTID = 0;
                if (Convert.ToString(param[9]) != "" && param[9] != "null")
                { PRDTID = Convert.ToInt32(param[9]); }

                var YRDID = 0;
                if (Convert.ToString(param[10]) != "")
                { YRDID = Convert.ToInt32(param[10]); }

                var OTYPID = 0;
                if (Convert.ToString(param[11]) != "")
                { OTYPID = Convert.ToInt32(param[11]); }

                var HTYPE = 0;
                if (Convert.ToString(param[12]) != "")
                { HTYPE = Convert.ToInt32(param[12]); }

                var CONTNRSID = 0;
                if (Convert.ToString(param[13]) != "")
                { CONTNRSID = Convert.ToInt32(param[13]); }


                var qty = 0.0;
                if (Convert.ToString(param[14]) != "")
                { qty = Convert.ToDouble(param[14]); }

                var nop = 0.0;
                if (Convert.ToString(param[15]) != "")
                { nop = Convert.ToDouble(param[15]); }

                var rate = 0.0;
                if (Convert.ToString(param[16]) != "")
                { rate = Convert.ToDouble(param[16]); }

                var amt = 0.0;
                if (Convert.ToString(param[17]) != "")
                { amt = Convert.ToDouble(param[17]); }

                var vtdid = 0.0;
                if (Convert.ToString(param[18]) != "" && Convert.ToString(param[18]) != "" && Convert.ToString(param[18]) != "null")
                { vtdid = Convert.ToInt32(param[18]); }
                var ebndid = 0.0;
                if (Convert.ToString(param[19]) != "" && Convert.ToString(param[19]) != "" && Convert.ToString(param[19]) != "null")
                { ebndid = Convert.ToInt32(param[19]); }

                var taxtype = 0;
                if (Convert.ToString(param[20]) != "")
                { taxtype = Convert.ToInt16(param[20]); }

                var vtspc = 0.0;
                if (Convert.ToString(param[21]) != "")
                { vtspc = Convert.ToDouble(param[21]); }

                var SDATE = param[2].Split('-');
                var EDATE = param[3].Split('-');
                var sdt = ""; var edt = "";
                if (EDATE.Length > 1)
                    edt = EDATE[2] + '-' + EDATE[1] + "-" + EDATE[0];
                if (SDATE.Length > 1)
                    sdt = SDATE[2] + '-' + SDATE[1] + "-" + SDATE[0];
                string cusrid = Session["CUSRID"].ToString();
                string gqry = "EXEC [pr_Ex_Bond_VT_Invoice_BondNO_Grid_Assgn] @userid = '" + cusrid + "',";
                gqry = gqry + " @PVTDID =" + vtdid + ",";
                gqry = gqry + " @PEBNDID =" + ebndid + ",";
                gqry = gqry + " @PBNDID =" + BNDID + ",";
                gqry = gqry + " @PTRANMID = " + TRANMID + ", @PSDATE = '" + sdt + "',";
                gqry = gqry + " @PEDATE = '" + edt + "', @PBCHAID = " + BCHAID + ",";
                gqry = gqry + " @PBCATEAID = " + BAID + ", @PTARIFFMID = " + TARIFFMID + ",";
                gqry = gqry + " @PSLABTID = " + SLABTID + ", @PCHRGTYPE = " + CHRGETYPE + ",";
                gqry = gqry + " @PRDTID = " + PRDTID + ", @PYRDID = " + YRDID + ",";
                gqry = gqry + " @POPRID = " + OTYPID + ", @PHNDTID  = " + HTYPE + ",";
                gqry = gqry + " @PCNTSID = " + CONTNRSID + ", @PQTY  = " + qty + ",";
                gqry = gqry + " @PNOP = " + nop + ", @PRATE  = " + rate + ",";
                gqry = gqry + " @PAMT = " + amt + ",";
                gqry = gqry + " @PHTYPE = " + HTYPE + ",";
                gqry = gqry + " @PTAXTYPE = " + taxtype + ",";
                gqry = gqry + " @PVTSPC = " + vtspc + "";
                

                var query = context.Database.SqlQuery<pr_Ex_Bond_VT_Invoice_BondNO_Grid_Assgn_Result>(gqry).ToList();

                tabl = " <div class='panel-heading navbar-inverse' style=color:white>Ex-Bond VT Bill Details</div>";
                tabl = tabl + "<Table id=TDETAIL class='table table-striped table-bordered bootstrap-datatable'>";
                tabl = tabl + "<thead><tr><th><input type='checkbox' id='CHCK_ALL' name='CHCK_ALL' class='CHCK_ALL' onchange='checkall()' style='width:30px'/></th>";
                tabl = tabl + "<th>Ex Bond No</th><th>Bond No</th><th>Tariff</th><th>Slab Type</th> <th class='hide'>Period</th><th>BType</th><th>CSize</th><th>Wks./Cntnrs./Ins.</th><th>Oprn</th><th>NOP</th><th>Charge Date</th><th>Rate</th><th>Amount</th>";
                tabl = tabl + "<th class='hide'>Total</th><th></th></tr> </thead>";
                var count = 0;

                foreach (var rslt in query)
                {
                    var st = ""; var bt = "";

                    if (rslt.TRANDID != 0) { st = "checked"; bt = "true"; }
                    else { bt = "false"; st = ""; }


                    tabl = tabl + "<tbody><tr><td><input type=checkbox id=TRANDIDS class=TRANDIDS name=TRANDIDS checked='" + bt + "'   onchange=total() style='width:30px'>";
                    tabl = tabl + "<input type=text id=boolTRANDIDS class='hide boolTRANDIDS' name=boolTRANDIDS value='" + bt + "'></td>";
                    tabl = tabl + "<td class=hide><input type=text id=TRANDID value=" + rslt.TRANDID + "  class=TRANDID name=TRANDID></td>";
                    tabl = tabl + "<td class='hide'><input type=text id=VTDNO value=" + rslt.VTDNO + "  class=VTDNO readonly name=VTDNO style='width:110px'><input type=text id=VTDID value='" + rslt.VTDID + "'  class='hide VTDID' name=VTDID></td>";
                    tabl = tabl + "<td><input type=text id=TRANDEBNDNO value=" + rslt.TRANDEBNDNO + "  class=TRANDEBNDNO readonly name=TRANDEBNDNO style='width:110px'><input type=text id=EBNDID value='" + rslt.EBNDID + "'  class='hide EBNDID' name=EBNDID></td>";                    
                    tabl = tabl + "<td><input type=text id=TRANDBNDNO value=" + rslt.TRANDBNDNO + "  class=TRANDBNDNO readonly name=TRANDBNDNO style='width:110px'><input type=text id=BNDID value='" + rslt.BNDID + "'  class='hide BNDID' name=BNDID></td>";                    
                    tabl = tabl + "<td><input type=text id=TARIFFMID value='" + rslt.TARIFFMID + "' class='hide TARIFFMID' readonly name=TARIFFMID style='width:40px'><input type=text id=TARIFFDESC  readonly value='" + rslt.TARIFFDESC + "' class=TARIFFDESC name=TARIFFDESC style='width:120px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANHTYPE value='" + rslt.TRANHTYPE + "' class='hide TRANHTYPE'  readonly name=TRANHTYPE style='width:40px'><input type=text id=SLABTID value='" + rslt.SLABTID + "' class='hide SLABTID'  readonly name=SLABTID style='width:40px'><input type=text id=VTDID value='" + rslt.VTDID + "' class='hide VTDID'  readonly name=VTDID style='width:40px'><input type=text id=SLABTDESC value='" + rslt.SLABTDESC + "' class=SLABTDESC  readonly name=SLABTDESC style='width:120px'></td>";
                    tabl = tabl + "<td class='hide'><input type=text id=PERIODTID value='" + rslt.PERIODTID + "' class='hide PERIODTID' name=PERIODTID   readonly style='width:40px'><input type=text id=PERIODTDESC value='" + rslt.PERIODTDESC + "' class=PERIODTDESC  readonly name=PERIODTDESC style='width:70px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANCTYPE value='" + rslt.TRANCTYPE + "' class='hide TRANCTYPE' readonly name=TRANCTYPE style='width:40px'><input type=text id=TRANCTYPEDESC value='" + rslt.TRANCTYPEDESC + "' class='TRANCTYPEDESC' readonly name=TRANCTYPEDESC style='width:40px'></td>";
                    tabl = tabl + "<td><input type=text id=CONTNRSID value='" + rslt.CONTNRSID + "' class='hide CONTNRSID' readonly name=CONTNRSID style='width:40px'><input type=text id=CONTNRSDESC value='" + rslt.CONTNRSDESC + "' class='CONTNRSDESC' readonly name=CONTNRSDESC style='width:40px'></td>";
                    tabl = tabl + "<td class='hide'><input type=text id=YRDID value='" + rslt.YRDID + "' class='hide YRDID' readonly name=YRDID style='width:40px'><input type=text id=YRDDESC value='" + rslt.YRDDESC + "' class='YRDDESC hide' readonly name=YRDDESC style='width:120px'></td>";
                    if(rslt.SLABTDESC == "INSURANCE" )
                    {
                        tabl = tabl + "<td><input type=text id=TRANDQTY value='" + rslt.TRANDQTY + "'  class='TRANDQTY hide' readonly name=TRANDQTY><input type='text' value='" + rslt.TRANDINSAMT + "' id='TRANDINSAMT' class='TRANDINSAMT' readonly name='TRANDINSAMT'></td>";
                    }
                    else
                    {
                        tabl = tabl + "<td><input type=text id=TRANDQTY value='" + rslt.TRANDQTY + "'  class='TRANDQTY ' readonly name=TRANDQTY><input type='text' value='" + rslt.TRANDINSAMT + "' id='TRANDINSAMT' class='hide TRANDINSAMT' readonly name='TRANDINSAMT'></td>";
                    }


                    tabl = tabl + "<td><input type=text id=TRANOTYPE value='" + rslt.TRANOTYPE + "' class='hide TRANOTYPE' readonly name=TRANOTYPE style='width:40px'><input type=text id=TRANOTYPEDESC value='" + rslt.TRANOTYPEDESC + "' class='TRANOTYPEDESC' readonly name=TRANOTYPEDESC style='width:75px'></td>";
                    tabl = tabl + "<td><input type=text id=TRANDNOP class='TRANDNOP' readonly name=TRANDNOP style='width:50px' value=" + rslt.TRANDNOP + "></td>";
                    tabl = tabl + "<td><input type=text id=TRANCDATE value='" + rslt.TRANCDATE + "' style='width:80px' class='TRANCDATE' readonly name=TRANCDATE>";

                    tabl = tabl + "<input type=text id=TRANIFDATE value='" + rslt.TRANIFDATE + "' class='TRANIFDATE hide'  readonly name=TRANIFDATE>";
                    tabl = tabl + "<input type=text id=TRANIEDATE value='" + rslt.TRANIEDATE + "' class='hide TRANIEDATE'  readonly name=TRANIEDATE>";
                    tabl = tabl + "<input type=text id=TRANSFDATE value='" + rslt.TRANSFDATE + "' class='TRANSFDATE hide'  readonly name=TRANSFDATE>";
                    tabl = tabl + "<input type=text id=TRANSEDATE value='" + rslt.TRANSEDATE + "' class='hide TRANSEDATE'  readonly name=TRANSEDATE></td>";

                    tabl = tabl + "<td><input type=text id=TRANDRATE class='TRANDRATE' readonly style='width:75px' name=TRANDRATE value='" + rslt.TRANDRATE + "'></td>";

                    tabl = tabl + "<td><input type=text id=TRANDGAMT value=" + rslt.TRANDGAMT + " style='width:75px' class='TRANDGAMT' readonly name=TRANDGAMT>";
                    tabl = tabl + "<input type=text id=TRANDSAMT value='0' class='TRANDSAMT hide' readonly name=TRANDSAMT style='width:75px'>";
                    tabl = tabl + "<input type=text id=TRANDIAMT value='0' class='TRANDIAMT hide' readonly name=TRANDIAMT style='width:75px'></td>";


                    tabl = tabl + "<td class='hide'><input type=text value='" + rslt.TRANDNAMT + "' id=TRANDNAMT class='TRANDNAMT' style='width:75px' readonly name=TRANDNAMT  style=width:100px>";
                    tabl = tabl + "<input type=text id=STATETYPE value='" + rslt.STATETYPE + "' class='STATETYPE hide' readonly name=STATETYPE style='width:75px'></td>";

                    tabl = tabl + "<td class=hide><input type=text id=SLABMIN value='0'  class=SLABMIN name=SLABMIN style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMAX value='0'  class=SLABMAX name=SLABMAX style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMIN1 value='0'  class=SLABMIN1 name=SLABMIN1 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMAX1 value='0'  class=SLABMAX1 name=SLABMAX1 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMIN2 value='0'  class=SLABMIN2 name=SLABMIN2 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMAX2 value='0'  class=SLABMAX2 name=SLABMAX2 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMIN3 value='0'  class=SLABMIN3 name=SLABMIN3 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMAX3 value='0'  class=SLABMAX3 name=SLABMAX3 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMIN4 value='0'  class=SLABMIN4 name=SLABMIN4 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMAX4 value='0'  class=SLABMAX4 name=SLABMAX4 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMIN5 value='0'  class=SLABMIN5 name=SLABMIN5 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMAX5 value='0'  class=SLABMAX5 name=SLABMAX5 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMIN6 value='0'  class=SLABMIN6 name=SLABMIN6 style='display:none' >";
                    tabl = tabl + "<input type=text id=SLABMAX6 value='0'  class=SLABMAX6 name=SLABMAX6 style='display:none' > </td>";
                    tabl = tabl + "<td class=hide> <input type=text id=RCAMT1 value=0  class=RCAMT1 name=RCAMT1 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT2 value=0  class=RCAMT2 name=RCAMT2 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT3 value=0  class=RCAMT3 name=RCAMT3 style='display:none'>";
                    tabl = tabl + "<input type=text id=RCAMT4 value='0'  class=RCAMT4 name=RCAMT4 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT5 value='0'  class=RCAMT5 name=RCAMT5 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT6 value='0'  class=RCAMT6 name=RCAMT6 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCAMT7 value='0'  class=RCAMT7 name=RCAMT7 style='display:none' ></td>";
                    tabl = tabl + "<td class=hide><input type=text id=RCOL1 value='0'  class=RCOL1 name=RCOL1 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL2 value='0'  class=RCOL2 name=RCOL2 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL3 value='0'  class=RCOL3 name=RCOL3 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL4 value='0'  class=RCOL4 name=RCOL4 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL5 value='0'  class=RCOL5 name=RCOL5 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL6 value='0'  class=RCOL6 name=RCOL6 style='display:none' >";
                    tabl = tabl + "<input type=text id=RCOL7 value='0'  class=RCOL7 name=RCOL7 style='display:none' >";
                    tabl = tabl + "<input type='text' value='" + rslt.STATETYPE + "' id='STATETYPE' class='STATETYPE' readonly name='STATETYPE'>";
                    tabl = tabl + "<input type='text' value='" + rslt.TRAND_HSN_CODE + "' id='TRAND_HSN_CODE' class='TRAND_HSN_CODE' readonly name='TRAND_HSN_CODE'>";
                    tabl = tabl + "<input type='text' value='" + rslt.INS_TRAND_HSN_CODE + "' id='INS_TRAND_HSN_CODE' class='INS_TRAND_HSN_CODE' readonly name='INS_TRAND_HSN_CODE'>";
                    tabl = tabl + "<input type='text' value='" + rslt.TRAND_TAXABLE_AMT + "' id='TRAND_TAXABLE_AMT' class='TRAND_TAXABLE_AMT' readonly name='TRAND_TAXABLE_AMT'>";
                    tabl = tabl + "<input type='text' value='" + rslt.INS_TRAND_TAXABLE_AMT + "' id='INS_TRAND_TAXABLE_AMT' class='INS_TRAND_TAXABLE_AMT' readonly name='INS_TRAND_TAXABLE_AMT'>";
                    tabl = tabl + "<input type='text' value='" + rslt.TRAND_CGST_EXPRN + "' id='TRAND_CGST_EXPRN' class='TRAND_CGST_EXPRN' readonly name='TRAND_CGST_EXPRN'>";
                    tabl = tabl + "<input type='text' value='" + rslt.TRAND_SGST_EXPRN + "' id='TRAND_SGST_EXPRN' class='TRAND_SGST_EXPRN' readonly name='TRAND_SGST_EXPRN'>";
                    tabl = tabl + "<input type='text' value='" + rslt.TRAND_IGST_EXPRN + "' id='TRAND_IGST_EXPRN' class='TRAND_IGST_EXPRN' readonly name='TRAND_IGST_EXPRN'>";
                    tabl = tabl + "<input type='text' value='" + rslt.TRAND_CGST_AMT + "' id='TRAND_CGST_AMT' class='TRAND_CGST_AMT' readonly name='TRAND_CGST_AMT'>";
                    tabl = tabl + "<input type='text' value='" + rslt.TRAND_SGST_AMT + "' id='TRAND_SGST_AMT' class='TRAND_SGST_AMT' readonly name='TRAND_SGST_AMT'>";
                    tabl = tabl + "<input type='text' value='" + rslt.TRAND_IGST_AMT + "' id='TRAND_IGST_AMT' class='TRAND_IGST_AMT' readonly name='TRAND_IGST_AMT'>";
                    tabl = tabl + "<input type='text' value='" + rslt.INS_TRAND_CGST_EXPRN + "' id='INS_TRAND_CGST_EXPRN' class='INS_TRAND_CGST_EXPRN' readonly name='INS_TRAND_CGST_EXPRN'>";
                    tabl = tabl + "<input type='text' value='" + rslt.INS_TRAND_SGST_EXPRN + "' id='INS_TRAND_SGST_EXPRN' class='INS_TRAND_SGST_EXPRN' readonly name='INS_TRAND_SGST_EXPRN'>";
                    tabl = tabl + "<input type='text' value='" + rslt.INS_TRAND_IGST_EXPRN + "' id='INS_TRAND_IGST_EXPRN' class='INS_TRAND_IGST_EXPRN' readonly name='INS_TRAND_IGST_EXPRN'>";
                    tabl = tabl + "<input type='text' value='" + rslt.INS_TRAND_CGST_AMT + "' id='INS_TRAND_CGST_AMT' class='INS_TRAND_CGST_AMT' readonly name='INS_TRAND_CGST_AMT'>";
                    tabl = tabl + "<input type='text' value='" + rslt.INS_TRAND_SGST_AMT + "' id='INS_TRAND_SGST_AMT' class='INS_TRAND_SGST_AMT' readonly name='INS_TRAND_SGST_AMT'>";
                    tabl = tabl + "<input type='text' value='" + rslt.INS_TRAND_IGST_AMT + "' id='INS_TRAND_IGST_AMT' class='INS_TRAND_IGST_AMT' readonly name='INS_TRAND_IGST_AMT'>";
                    tabl = tabl + "<input type='text' value='" + rslt.TRANDCIFAMT + "' id='TRANDCIFAMT' class='TRANDCIFAMT' readonly name='TRANDCIFAMT'>";
                    tabl = tabl + "<input type='text' value='" + rslt.TRANDDTYAMT + "' id='TRANDDTYAMT' class='TRANDDTYAMT' readonly name='TRANDDTYAMT'>";
                    tabl = tabl + "<input type='text' value='" + rslt.EXBNDVTSPC + "' id='EXBNDVTSPC' class='EXBNDVTSPC' readonly name='EXBNDVTSPC'>";
                    //tabl = tabl + "<input type='text' value='" + rslt.TRANDINSAMT + "' id='TRANDINSAMT' class='TRANDINSAMT' readonly name='TRANDINSAMT'>";

                    tabl = tabl + "</td></tr></tbody>";
                    count++;
                }
                tabl = tabl + "</Table>";
            }



            return tabl;

        }

        //......end


        public JsonResult GetBondGSTRATE(string id)
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
                var query = context.Database.SqlQuery<VW_BOND_SLABTYPE_HSN_DETAIL_ASSGN>("select HSNCODE,CGSTEXPRN,SGSTEXPRN,IGSTEXPRN from VW_BOND_SLABTYPE_HSN_DETAIL_ASSGN where SLABTID=" + SlabTId + " order by HSNCODE").ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var query = context.Database.SqlQuery<VW_BOND_SLABTYPE_HSN_DETAIL_ASSGN>("select HSNCODE,ACGSTEXPRN as CGSTEXPRN,ASGSTEXPRN as SGSTEXPRN,AIGSTEXPRN as IGSTEXPRN from VW_BOND_SLABTYPE_HSN_DETAIL_ASSGN where SLABTID=" + SlabTId + " order by HSNCODE").ToList();

                return Json(query, JsonRequestBehavior.AllowGet);


            }

        } //...end

        //............ratecardmaster.....................
        public JsonResult RATECARD(string id)
        {
            if (id != null)
            {
                var param = id.Split('-');
                var TARIFFMID = 0;
                if (Convert.ToString(param[0]) != "")
                { TARIFFMID = Convert.ToInt32(param[0]); }
                var CHRGETYPE = 0;
                if (Convert.ToString(param[1]) != "")
                { CHRGETYPE = Convert.ToInt32(param[1]); }
                var CONTNRSID = 0;
                if (Convert.ToString(param[2]) != "")
                { CONTNRSID = Convert.ToInt32(param[2]); }
                var CHAID = 0;
                if (Convert.ToString(param[3]) != "")
                { CHAID = Convert.ToInt32(param[3]); }
                var SLABTID = 0;
                if (Convert.ToString(param[4]) != "" && Convert.ToString(param[4]) != "undefined")
                { SLABTID = Convert.ToInt32(param[4]); }
                var PRDTID = 0;
                if (Convert.ToString(param[5]) != "" && param[5] != null)
                { PRDTID = Convert.ToInt32(param[5]); }

                var YRDID = 0;
                if (Convert.ToString(param[6]) != "")
                { YRDID = Convert.ToInt32(param[6]); }
                var HTYPE = 0;
                if (Convert.ToString(param[7]) != "")
                { HTYPE = Convert.ToInt32(param[7]); }
                var PRDTGID = 0;
                if (Convert.ToString(param[8]) != "")
                { PRDTGID = Convert.ToInt32(param[8]); }
                var HANDTYPE = 0;
                if (Convert.ToString(param[9]) != "")
                { HANDTYPE = Convert.ToInt32(param[9]); }
                var SLABMIN = 0;// Convert.ToInt32(param[8]);
                if (TARIFFMID == 4)
                {
                    string tqry = "select SLABAMT,SLABMIN,SLABMAX from VW_BOND_RATECARDMASTER_FLX_ASSGN (NOLOCK) where TARIFFMID=" + TARIFFMID + " and SLABTID=" + SLABTID + " and HTYPE=" + HTYPE + " AND YRDTYPE = " + YRDID + " AND PERIODTID = " + PRDTID + " and CHRGETYPE=" + CHRGETYPE + " and HANDTYPE= " + HANDTYPE + " and CONTNRSID= " + CONTNRSID + " and (PRDTGID = " + PRDTGID + " or PRDTGID =1) and CHAID =" + CHAID + " order by SLABMIN";
                    var query = context.Database.SqlQuery<VW_BOND_RATECARDMASTER_FLX_ASSGN>(tqry).ToList();
                    return Json(query, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string tqry = "select SLABAMT,SLABMIN,SLABMAX from VW_BOND_RATECARDMASTER_FLX_ASSGN (NOLOCK) where TARIFFMID=" + TARIFFMID + " and SLABTID=" + SLABTID + " and HTYPE=" + HTYPE + " AND YRDTYPE = " + YRDID + " AND PERIODTID = " + PRDTID + " and CHRGETYPE=" + CHRGETYPE +  " and HANDTYPE= " + HANDTYPE + " and CONTNRSID= " + CONTNRSID + " and (PRDTGID = " + PRDTGID + " or PRDTGID =1) order by SLABMIN";
                    var query = context.Database.SqlQuery<VW_BOND_RATECARDMASTER_FLX_ASSGN>(tqry).ToList();

                    return Json(query, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }


        } //...end

        //.................Insert/update values into database.............//
        public void savedata(FormCollection F_Form)
        {
            using (BondContext dataContext = new BondContext())
            {
                using (var trans = dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        string todaydt = Convert.ToString(DateTime.Now);
                        string todayd = Convert.ToString(DateTime.Now.Date);

                        BondTransactionMaster bondtransactionmaster = new BondTransactionMaster();
                        BondTransactionDetail bondtransactiondetail = new BondTransactionDetail();
                        //-------Getting Primarykey field--------
                        Int32 TRANMID = Convert.ToInt32(F_Form["bndmasterdata[0].TRANMID"]);
                        Int32 TRANDID = 0;
                        string DELIDS = "";
                        //-----End


                        if (TRANMID != 0)
                        {
                            bondtransactionmaster = context.bondtransactionmaster.Find(TRANMID);
                        }

                        //...........transaction master.............//
                        bondtransactionmaster.TRANMID = TRANMID;
                        bondtransactionmaster.COMPYID = Convert.ToInt32(Session["compyid"]);
                        bondtransactionmaster.SDPTID = 10;
                        bondtransactionmaster.TRANTID = 3;
                        bondtransactionmaster.BCATEAID = Convert.ToInt32(F_Form["BCATEAID"].ToString());
                        bondtransactionmaster.CHAID = Convert.ToInt32(F_Form["bndmasterdata[0].CHAID"].ToString());
                        bondtransactionmaster.TRANGSTNO = (F_Form["bndmasterdata[0].TRANGSTNO"].ToString());
                        bondtransactionmaster.TRANLMID = Convert.ToInt32(F_Form["bndmasterdata[0].TRANLMID"]);
                        bondtransactionmaster.TRANLMNO = F_Form["TRANLMNO"].ToString();
                        bondtransactionmaster.TRANLMDATE = Convert.ToDateTime(F_Form["bndmasterdata[0].TRANLMDATE"]);

                        bondtransactionmaster.TRANNARTN = Convert.ToString(F_Form["bndmasterdata[0].TRANNARTN"]);
                        bondtransactionmaster.CUSRID = Session["CUSRID"].ToString();
                        if (TRANMID == 0)
                        {
                            bondtransactionmaster.CUSRID = Session["CUSRID"].ToString();

                        }
                        bondtransactionmaster.LMUSRID = Session["CUSRID"].ToString();
                        bondtransactionmaster.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
                        bondtransactionmaster.PRCSDATE = DateTime.Now;


                        string indate = Convert.ToString(F_Form["bndmasterdata[0].TRANDATE"]);
                        string intime = Convert.ToString(F_Form["bndmasterdata[0].TRANTIME"]);

                        if (indate != null || indate != "")
                        {
                            bondtransactionmaster.TRANDATE = Convert.ToDateTime(indate).Date;
                        }
                        else { bondtransactionmaster.TRANDATE = DateTime.Now.Date; }

                        if (bondtransactionmaster.TRANDATE > Convert.ToDateTime(todayd))
                        {
                            bondtransactionmaster.TRANDATE = Convert.ToDateTime(todayd);
                        }

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

                                    bondtransactionmaster.TRANTIME = Convert.ToDateTime(in_datetime);
                                }
                                else { bondtransactionmaster.TRANTIME = DateTime.Now; }
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

                                    bondtransactionmaster.TRANTIME = Convert.ToDateTime(in_datetime);
                                }
                                else { bondtransactionmaster.TRANTIME = DateTime.Now; }

                            }
                        }
                        else { bondtransactionmaster.TRANTIME = DateTime.Now; }

                        if (bondtransactionmaster.TRANTIME > Convert.ToDateTime(todaydt))
                        {
                            bondtransactionmaster.TRANTIME = Convert.ToDateTime(todaydt);
                        }

                        //bondtransactionmaster.TRANDATE = Convert.ToDateTime(F_Form["bndmasterdata[0].TRANTIME"]).Date;
                        //bondtransactionmaster.TRANTIME = Convert.ToDateTime(F_Form["bndmasterdata[0].TRANTIME"]);

                        bondtransactionmaster.BILLREFID = Convert.ToInt32(F_Form["bndmasterdata[0].BILLREFID"]);
                        bondtransactionmaster.TRANREFNAME = F_Form["bndmasterdata[0].TRANREFNAME"].ToString();
                        bondtransactionmaster.TRANIMPRTNAME = F_Form["bndmasterdata[0].TRANIMPRTNAME"].ToString();
                        bondtransactionmaster.IMPRTID = Convert.ToInt32(F_Form["bndmasterdata[0].IMPRTID"]);
                        bondtransactionmaster.TRANBTYPE = Convert.ToInt16(F_Form["F_TRANBTYPE"]);
                        bondtransactionmaster.TAXTYPE = Convert.ToInt32(F_Form["F_TAXTYPE"]);
                        bondtransactionmaster.REGSTRID = Convert.ToInt16(F_Form["REGSTRID"]);
                        bondtransactionmaster.TRANMODE = Convert.ToInt16(F_Form["TRANMODE"]);
                        bondtransactionmaster.TRANMODEDETL = (F_Form["bndmasterdata[0].TRANMODEDETL"]);
                        bondtransactionmaster.TRANGAMT = Convert.ToDecimal(F_Form["bndmasterdata[0].TRANGAMT"]);
                        bondtransactionmaster.TRANNAMT = Convert.ToDecimal(F_Form["bndmasterdata[0].TRANNAMT"]);
                        bondtransactionmaster.TRANROAMT = Convert.ToDecimal(F_Form["bndmasterdata[0].TRANROAMT"]);
                        bondtransactionmaster.TRANREFAMT = Convert.ToDecimal(F_Form["bndmasterdata[0].TRANREFAMT"]);
                        bondtransactionmaster.TRANRMKS = (F_Form["bndmasterdata[0].TRANRMKS"]).ToString();
                        bondtransactionmaster.TRANCHANAME = (F_Form["bndmasterdata[0].TRANCHANAME"]).ToString();

                        bondtransactionmaster.TRANAMTWRDS = AmtInWrd.ConvertNumbertoWords(F_Form["bndmasterdata[0].TRANNAMT"]);

                        if (F_Form["TRANSAMT"].Length != 0)
                        { bondtransactionmaster.TRANSAMT = Convert.ToDecimal(F_Form["TRANSAMT"]); }
                        else
                        { bondtransactionmaster.TRANSAMT = 0; }



                        bondtransactionmaster.TRANPTAMT = 0;
                        bondtransactionmaster.TRANPLAMT = 0;



                        //if (F_Form["TRANFAMT"].Length != 0)
                        //{ bondtransactionmaster.TRANFAMT = Convert.ToDecimal(F_Form["TRANFAMT"]); }
                        //else
                        //{ bondtransactionmaster.TRANFAMT = 0; }

                        ////bondtransactionmaster.TRANSAMT = Convert.ToDecimal(F_Form["TRANSAMT"]);
                        ////bondtransactionmaster.TRANPAMT = Convert.ToDecimal(F_Form["TRANPAMT"]);
                        ////bondtransactionmaster.TRANHAMT = Convert.ToDecimal(F_Form["TRANHAMT"]);
                        ////bondtransactionmaster.TRANEAMT = Convert.ToDecimal(F_Form["TRANEAMT"]);
                        ////bondtransactionmaster.TRANFAMT = Convert.ToDecimal(F_Form["TRANFAMT"]);
                        //if (F_Form["TRANTCAMT"].Length != 0)
                        //{ bondtransactionmaster.TRANTCAMT = Convert.ToDecimal(F_Form["TRANTCAMT"]); }
                        //else
                        //{ bondtransactionmaster.TRANTCAMT = 0; }


                        ////bondtransactionmaster.HANDL_HSNCODE = F_Form["HANDL_HSN_CODE"].ToString();

                        bondtransactionmaster.TRAN_TAXABLE_AMT = Convert.ToDecimal(F_Form["TRAN_TAXABLE_AMT"]);
                        //bondtransactionmaster.HANDL_TAXABLE_AMT = Convert.ToDecimal(F_Form["HANDL_TAXABLE_AMT"]);

                        bondtransactionmaster.TRAN_CGST_AMT = Math.Round(Convert.ToDecimal(F_Form["TRAN_CGST_AMT"]),0);
                        bondtransactionmaster.TRAN_SGST_AMT = Math.Round(Convert.ToDecimal(F_Form["TRAN_SGST_AMT"]),0);
                        bondtransactionmaster.TRAN_IGST_AMT = Math.Round(Convert.ToDecimal(F_Form["TRAN_IGST_AMT"]),0);

                        //bondtransactionmaster.HANDL_CGST_EXPRN = Convert.ToDecimal(F_Form["HANDL_CGST_EXPRN"]);
                        //bondtransactionmaster.HANDL_SGST_EXPRN = Convert.ToDecimal(F_Form["HANDL_SGST_EXPRN"]);
                        //bondtransactionmaster.HANDL_IGST_EXPRN = Convert.ToDecimal(F_Form["HANDL_IGST_EXPRN"]);
                        //bondtransactionmaster.HANDL_CGST_AMT = Convert.ToDecimal(F_Form["HANDL_CGST_AMT"]);
                        //bondtransactionmaster.HANDL_SGST_AMT = Convert.ToDecimal(F_Form["HANDL_SGST_AMT"]);
                        //bondtransactionmaster.HANDL_IGST_AMT = Convert.ToDecimal(F_Form["HANDL_IGST_AMT"]);

                        var BAid = Convert.ToInt32(F_Form["BCATEAID"]);
                        bondtransactionmaster.BCATEAID = BAid;
                        var tranmode = Convert.ToInt16(F_Form["TRANMODE"]);
                        if (tranmode != 2 && tranmode != 3)
                        {
                            bondtransactionmaster.TRANREFNO = "";
                            bondtransactionmaster.TRANREFBNAME = "";
                            bondtransactionmaster.BANKMID = 0;
                            bondtransactionmaster.TRANREFDATE = DateTime.Now;
                        }
                        else
                        {
                            bondtransactionmaster.TRANREFNO = (F_Form["bndmasterdata[0].TRANREFNO"]).ToString();
                            bondtransactionmaster.TRANREFBNAME = (F_Form["bndmasterdata[0].TRANREFBNAME"]).ToString();
                            bondtransactionmaster.BANKMID = Convert.ToInt32(F_Form["BANKMID"]);
                            bondtransactionmaster.TRANREFDATE = Convert.ToDateTime(F_Form["bndmasterdata[0].TRANREFDATE"]).Date;

                        }


                        //.................Autonumber............//
                        var regsid = Convert.ToInt32(F_Form["REGSTRID"]);
                        var btype = Convert.ToInt32(F_Form["TRANBTYPE"]);
                        var TAXType = Convert.ToInt32(F_Form["TAXTYPE"]);

                        if (TRANMID == 0)
                        {
                            bondtransactionmaster.TRANNO = Convert.ToInt32(auto_numbr_invoice.gstBondAutonum("bondtransactionmaster", "TRANNO", F_Form["REGSTRID"].ToString(), Session["compyid"].ToString(), F_Form["F_TRANBTYPE"].ToString(), Convert.ToInt32(bondtransactionmaster.BILLREFID), Convert.ToInt32(bondtransactionmaster.TRANTID), TAXType).ToString());

                            int ano = bondtransactionmaster.TRANNO;
                            //string format = "SUD/EXP/";
                            string taxtype = "";
                            if (bondtransactionmaster.TAXTYPE == 1)
                            {
                                taxtype = "BWH/";
                            }
                            else
                            {
                                taxtype = "BSBWH/";
                            }
                            string format = "";
                            string btyp = auto_numbr_invoice.GetCateBillType(Convert.ToInt32(bondtransactionmaster.BILLREFID)).ToString();
                            if (btyp == "")
                            {
                                format = taxtype + Session["GPrxDesc"] + "/";
                            }
                            else
                                format = taxtype + Session["GPrxDesc"] + btyp;
                            string billformat = "";
                            switch (regsid)
                            {
                                case 16: billformat = taxtype + "/EXB/CU/"; break;
                                case 17: billformat = taxtype + "/EXB/CH/"; break;
                                case 18: billformat = taxtype + "ZB/BOND/"; break;

                            }
                            string prfx = string.Format(format + "{0:D5}", ano);
                            string billprfx = string.Format(billformat + "{0:D5}", ano);
                            bondtransactionmaster.TRANDNO = prfx.ToString();
                            bondtransactionmaster.TRANBILLREFNO = billprfx.ToString();

                            //........end of autonumber
                            context.bondtransactionmaster.Add(bondtransactionmaster);
                            context.SaveChanges();
                            TRANMID = bondtransactionmaster.TRANMID;
                        }
                        else
                        {
                            //bondtransactionmaster.REGSTRID = Convert.ToInt16(F_Form["bndmasterdata[0].REGSTRID"]);
                            // bondtransactionmaster.TRANMODE = Convert.ToInt16(F_Form["TRANMODE"]);
                            context.Entry(bondtransactionmaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }


                        //-------------transaction Details
                        string[] F_TRANDID = F_Form.GetValues("TRANDID");
                        string[] VTDID = F_Form.GetValues("VTDID");
                        string[] TRANDEBNDNO = F_Form.GetValues("TRANDEBNDNO");
                        string[] EBNDID = F_Form.GetValues("EBNDID");
                        string[] TRANDBNDNO = F_Form.GetValues("TRANDBNDNO");
                        string[] TRANCDATE = F_Form.GetValues("TRANCDATE");
                        string[] TRANIFDATE = F_Form.GetValues("TRANIFDATE");
                        string[] TRANIEDATE = F_Form.GetValues("TRANIEDATE");
                        string[] TRANSFDATE = F_Form.GetValues("TRANSFDATE");
                        string[] TRANSEDATE = F_Form.GetValues("TRANSEDATE");
                        string[] TRANDIDS = F_Form.GetValues("TRANDIDS");
                        string[] boolTRANDIDS = F_Form.GetValues("boolTRANDIDS");
                        string[] TRANDHAMT = F_Form.GetValues("TRANDHAMT");
                        string[] TRANDIAMT = F_Form.GetValues("TRANDIAMT");
                        string[] TRANDFAMT = F_Form.GetValues("TRANDFAMT");
                        string[] TRANDPAMT = F_Form.GetValues("TRANDPAMT");
                        string[] TRANDNAMT = F_Form.GetValues("TRANDNAMT");
                        string[] TRANDGAMT = F_Form.GetValues("TRANDGAMT");
                        string[] TRANDSAMT = F_Form.GetValues("TRANDSAMT");
                        string[] TRANDNOP = F_Form.GetValues("TRANDNOP");
                        string[] TRANDQTY = F_Form.GetValues("TRANDQTY");
                        string[] TRANDRATE = F_Form.GetValues("TRANDRATE");
                        string[] TRANHTYPE = F_Form.GetValues("TRANHTYPE");
                        string[] TARIFFMID = F_Form.GetValues("TARIFFMID");
                        string[] SLABTID = F_Form.GetValues("SLABTID");
                        string[] PERIODTID = F_Form.GetValues("PERIODTID");
                        string[] TRANCTYPE = F_Form.GetValues("TRANCTYPE");
                        string[] TRANOTYPE = F_Form.GetValues("TRANOTYPE");
                        string[] CONTNRSID = F_Form.GetValues("CONTNRSID");
                        string[] YRDID = F_Form.GetValues("YRDID");
                        string[] BNDID = F_Form.GetValues("BNDID");
                        string[] TRANDCIFAMT = F_Form.GetValues("TRANDCIFAMT");
                        string[] TRANDDTYAMT = F_Form.GetValues("TRANDDTYAMT");
                        string[] TRANDINSAMT = F_Form.GetValues("TRANDINSAMT");

                        string[] STATETYPE = F_Form.GetValues("STATETYPE");
                        string[] TRAND_HSN_CODE = F_Form.GetValues("TRAND_HSN_CODE");                        
                        string[] INS_TRAND_HSN_CODE = F_Form.GetValues("INS_TRAND_HSN_CODE");
                        string[] TRAND_TAXABLE_AMT = F_Form.GetValues("TRAND_TAXABLE_AMT");
                        string[] INS_TRAND_TAXABLE_AMT = F_Form.GetValues("INS_TRAND_TAXABLE_AMT");
                        string[] TRAND_CGST_EXPRN = F_Form.GetValues("TRAND_CGST_EXPRN");
                        string[] TRAND_SGST_EXPRN = F_Form.GetValues("TRAND_SGST_EXPRN");
                        string[] TRAND_IGST_EXPRN = F_Form.GetValues("TRAND_IGST_EXPRN");
                        string[] TRAND_CGST_AMT = F_Form.GetValues("TRAND_CGST_AMT");
                        string[] TRAND_SGST_AMT = F_Form.GetValues("TRAND_SGST_AMT");
                        string[] TRAND_IGST_AMT = F_Form.GetValues("TRAND_IGST_AMT");
                        string[] INS_TRAND_CGST_EXPRN = F_Form.GetValues("INS_TRAND_CGST_EXPRN");
                        string[] INS_TRAND_SGST_EXPRN = F_Form.GetValues("INS_TRAND_SGST_EXPRN");
                        string[] INS_TRAND_IGST_EXPRN = F_Form.GetValues("INS_TRAND_IGST_EXPRN");
                        string[] INS_TRAND_CGST_AMT = F_Form.GetValues("INS_TRAND_CGST_AMT");
                        string[] INS_TRAND_SGST_AMT = F_Form.GetValues("INS_TRAND_SGST_AMT");
                        string[] INS_TRAND_IGST_AMT = F_Form.GetValues("INS_TRAND_IGST_AMT");
                        string[] EXBNDVTSPC = F_Form.GetValues("EXBNDVTSPC");

                        decimal trancgstamt = 0;
                        decimal transgstamt = 0;
                        decimal tranigstamt = 0;
                        decimal trantaxableamt = 0;
                        for (int count = 0; count < boolTRANDIDS.Count(); count++)
                        {
                            if (boolTRANDIDS[count] == "true")
                            {
                                TRANDID = Convert.ToInt32(F_TRANDID[count]);
                                var boolTRANDID = Convert.ToString(boolTRANDIDS[count]);
                                if (TRANDID != 0 && boolTRANDID == "true")
                                {
                                    bondtransactiondetail = context.bondtransactiondetail.Find(TRANDID);
                                }
                                bondtransactiondetail.TRANMID = bondtransactionmaster.TRANMID;
                                bondtransactiondetail.TRANDBNDNO = (TRANDBNDNO[count]).ToString();
                                bondtransactiondetail.TRANDEBNDNO = (TRANDEBNDNO[count]).ToString();
                                bondtransactiondetail.EBNDID = Convert.ToInt32(EBNDID[count]);
                                bondtransactiondetail.VTDID = Convert.ToInt32(VTDID[count]); 
                                bondtransactiondetail.TRANHTYPE = Convert.ToInt16(TRANHTYPE[count]); ;
                                bondtransactiondetail.TRANDWGHT = 0;

                                bondtransactiondetail.BNDID = Convert.ToInt32(BNDID[count]);
                                bondtransactiondetail.TRANCDATE = Convert.ToDateTime(TRANCDATE[count]);
                                if (TRANIFDATE[count] != null && TRANIFDATE[count] != "")
                                    bondtransactiondetail.TRANIFDATE = Convert.ToDateTime(TRANIFDATE[count]);
                                if (TRANIEDATE[count] != null && TRANIEDATE[count] != "")
                                    bondtransactiondetail.TRANIEDATE = Convert.ToDateTime(TRANIEDATE[count]);
                                if (TRANSFDATE[count] != null && TRANSFDATE[count] != "")
                                    bondtransactiondetail.TRANSFDATE = Convert.ToDateTime(TRANSFDATE[count]);
                                if (TRANSEDATE[count] != null && TRANSEDATE[count] != "")
                                    bondtransactiondetail.TRANSEDATE = Convert.ToDateTime(TRANSEDATE[count]);

                                if (TRANDSAMT[count] != null)
                                    bondtransactiondetail.TRANDSAMT = Convert.ToDecimal(TRANDSAMT[count]);
                                else
                                    bondtransactiondetail.TRANDSAMT = 0;
                                //if (TRANDHAMT[count] != null)
                                //  bondtransactiondetail.TRANDHAMT = Convert.ToDecimal(TRANDHAMT[count]);
                                //else
                                bondtransactiondetail.TRANDHAMT = 0;
                                if (TRANDIAMT[count] != null)
                                    bondtransactiondetail.TRANDIAMT = Convert.ToDecimal(TRANDIAMT[count]);
                                else
                                    bondtransactiondetail.TRANDIAMT = 0;
                                if (TRANDCIFAMT[count] != null)
                                    bondtransactiondetail.TRANDCIFAMT = Convert.ToDecimal(TRANDCIFAMT[count]);
                                else
                                    bondtransactiondetail.TRANDCIFAMT = 0;
                                if (TRANDDTYAMT[count] != null)
                                    bondtransactiondetail.TRANDDTYAMT = Convert.ToDecimal(TRANDDTYAMT[count]);
                                else
                                    bondtransactiondetail.TRANDDTYAMT = 0;
                                if (TRANDINSAMT[count] != null)
                                    bondtransactiondetail.TRANDINSAMT = Convert.ToDecimal(TRANDINSAMT[count]);
                                else
                                    bondtransactiondetail.TRANDINSAMT = 0;

                                if (EXBNDVTSPC[count] != null)
                                    bondtransactiondetail.EXBNDVTSPC = Convert.ToDecimal(EXBNDVTSPC[count]);
                                else
                                    bondtransactiondetail.EXBNDVTSPC = 0;

                                bondtransactiondetail.TRANDNAMT = Convert.ToDecimal(TRANDNAMT[count]);
                                
                                bondtransactiondetail.TRANDNOP = Convert.ToDecimal(TRANDNOP[count]);
                                bondtransactiondetail.TRANDQTY = Convert.ToDecimal(TRANDQTY[count]);
                                bondtransactiondetail.TRANDRATE = Convert.ToDecimal(TRANDRATE[count]);
                                bondtransactiondetail.TARIFFMID = Convert.ToInt32(TARIFFMID[count]);
                                bondtransactiondetail.SLABTID = Convert.ToInt32(SLABTID[count]);
                                bondtransactiondetail.PERIODTID = Convert.ToInt32(PERIODTID[count]);
                                bondtransactiondetail.TRANCTYPE = Convert.ToInt16(TRANCTYPE[count]);
                                bondtransactiondetail.CONTNRSID = Convert.ToInt32(CONTNRSID[count]);
                                bondtransactiondetail.YRDID = Convert.ToInt32(YRDID[count]);
                                bondtransactiondetail.TRANOTYPE = Convert.ToInt16(TRANOTYPE[count]);

                                bondtransactiondetail.STATETYPE = Convert.ToInt32(STATETYPE[count]);
                                bondtransactiondetail.TRAND_HSN_CODE = TRAND_HSN_CODE[count].ToString();
                                bondtransactiondetail.INS_TRAND_HSN_CODE = INS_TRAND_HSN_CODE[count].ToString();

                                bondtransactiondetail.TRAND_TAXABLE_AMT = Convert.ToDecimal(TRAND_TAXABLE_AMT[count]);
                                bondtransactiondetail.INS_TRAND_TAXABLE_AMT = Convert.ToDecimal(INS_TRAND_TAXABLE_AMT[count]);

                                bondtransactiondetail.TRAND_CGST_EXPRN = Convert.ToDecimal(TRAND_CGST_EXPRN[count]);
                                bondtransactiondetail.TRAND_SGST_EXPRN = Convert.ToDecimal(TRAND_SGST_EXPRN[count]);
                                bondtransactiondetail.TRAND_IGST_EXPRN = Convert.ToDecimal(TRAND_IGST_EXPRN[count]);

                                bondtransactiondetail.TRAND_CGST_AMT = Convert.ToDecimal(TRAND_CGST_AMT[count]);
                                bondtransactiondetail.TRAND_SGST_AMT = Convert.ToDecimal(TRAND_SGST_AMT[count]);
                                bondtransactiondetail.TRAND_IGST_AMT = Convert.ToDecimal(TRAND_IGST_AMT[count]);
                                if (INS_TRAND_CGST_AMT[count] != null)
                                    bondtransactiondetail.INS_TRAND_CGST_AMT = Convert.ToDecimal(INS_TRAND_CGST_AMT[count]);
                                else
                                    bondtransactiondetail.INS_TRAND_CGST_AMT = 0;
                                if (INS_TRAND_SGST_AMT[count] != null)
                                    bondtransactiondetail.INS_TRAND_SGST_AMT = Convert.ToDecimal(INS_TRAND_SGST_AMT[count]);
                                else
                                    bondtransactiondetail.INS_TRAND_SGST_AMT = 0;
                                if (INS_TRAND_IGST_AMT[count] != null)
                                    bondtransactiondetail.INS_TRAND_IGST_AMT = Convert.ToDecimal(INS_TRAND_IGST_AMT[count]);
                                else
                                    bondtransactiondetail.INS_TRAND_IGST_AMT = 0;

                                if (TRANDCIFAMT[count] != null)
                                    bondtransactiondetail.TRANDCIFAMT = Convert.ToDecimal(TRANDCIFAMT[count]);
                                else
                                    bondtransactiondetail.TRANDCIFAMT = 0;
                                if (TRANDDTYAMT[count] != null)
                                    bondtransactiondetail.TRANDDTYAMT = Convert.ToDecimal(TRANDDTYAMT[count]);
                                else
                                    bondtransactiondetail.TRANDDTYAMT = 0;
                                if (TRANDINSAMT[count] != null)
                                    bondtransactiondetail.TRANDINSAMT = Convert.ToDecimal(TRANDINSAMT[count]);
                                else
                                    bondtransactiondetail.TRANDINSAMT = 0;

                                bondtransactiondetail.TRANDEIAMT = 0;



                                bondtransactiondetail.TRANDGAMT = Convert.ToDecimal(TRANDGAMT[count]);
                                //bondtransactiondetail.BILLEDID = 0;
                                //bondtransactiondetail.RCOL1 = Convert.ToDecimal(RCOL1[count]);
                                //bondtransactiondetail.RCOL2 = Convert.ToDecimal(RCOL2[count]);
                                //bondtransactiondetail.RCOL3 = Convert.ToDecimal(RCOL3[count]);
                                //bondtransactiondetail.RCOL4 = 0;
                                //bondtransactiondetail.RCOL5 = 0;
                                //bondtransactiondetail.RCOL6 = 0;
                                //bondtransactiondetail.RCOL7 = 0;
                                //bondtransactiondetail.RAMT1 = Convert.ToDecimal(RAMT1[count]);
                                //bondtransactiondetail.RAMT2 = Convert.ToDecimal(RAMT2[count]);
                                //bondtransactiondetail.RAMT3 = Convert.ToDecimal(RAMT3[count]);
                                //bondtransactiondetail.RAMT4 = 0;// Convert.ToDecimal(RAMT4[count]);
                                //bondtransactiondetail.RAMT5 = 0;// Convert.ToDecimal(RAMT5[count]);
                                //bondtransactiondetail.RAMT6 = 0;// Convert.ToDecimal(RAMT6[count]);
                                //bondtransactiondetail.RCAMT1 = Convert.ToDecimal(RCAMT1[count]);
                                //bondtransactiondetail.RCAMT2 = Convert.ToDecimal(RCAMT2[count]);
                                //bondtransactiondetail.RCAMT3 = Convert.ToDecimal(RCAMT3[count]);
                                //bondtransactiondetail.RCAMT4 = 0;
                                //bondtransactiondetail.RCAMT5 = 0;
                                //bondtransactiondetail.RCAMT6 = 0;
                                ////bondtransactiondetail.RCAMT4 = Convert.ToDecimal(RCAMT4[count]);
                                ////bondtransactiondetail.RCAMT5 = Convert.ToDecimal(RCAMT5[count]);
                                ////bondtransactiondetail.RCAMT6 = Convert.ToDecimal(RCAMT6[count]);


                                bondtransactiondetail.TRANDWGHT = 0;
                                bondtransactiondetail.TRANDAID = 0;
                                bondtransactiondetail.TRANHTYPE = Convert.ToInt16(TRANHTYPE[count]);


                                trancgstamt = trancgstamt + Convert.ToDecimal(bondtransactiondetail.TRAND_CGST_AMT);
                                transgstamt = transgstamt + Convert.ToDecimal(bondtransactiondetail.TRAND_SGST_AMT);
                                tranigstamt = tranigstamt + Convert.ToDecimal(bondtransactiondetail.TRAND_IGST_AMT);
                                trantaxableamt = trantaxableamt + Convert.ToDecimal(bondtransactiondetail.TRAND_TAXABLE_AMT);

                                if (Convert.ToInt32(TRANDID) == 0)
                                {
                                    context.bondtransactiondetail.Add(bondtransactiondetail);
                                    context.SaveChanges();
                                    TRANDID = bondtransactiondetail.TRANDID;
                                }
                                else
                                {
                                    bondtransactiondetail.TRANDID = TRANDID;
                                    context.Entry(bondtransactiondetail).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                }//..............end
                                DELIDS = DELIDS + "," + TRANDID.ToString();
                            }


                            bondtransactionmaster.TRAN_CGST_AMT = Math.Round(trancgstamt,0);
                            bondtransactionmaster.TRAN_SGST_AMT = Math.Round(transgstamt,0);
                            bondtransactionmaster.TRAN_IGST_AMT = Math.Round(tranigstamt,0);
                            bondtransactionmaster.TRAN_TAXABLE_AMT = trantaxableamt;
                            context.Entry(bondtransactionmaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }

                        //-------delete transaction master factor-------//
                        context.Database.ExecuteSqlCommand("DELETE FROM bondtransactionmasterfactor WHERE tranmid=" + TRANMID);

                        //Transaction Type Master-------//

                        BondTransactionMasterFactor bondtransactionmasterfactors = new BondTransactionMasterFactor();
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

                                bondtransactionmasterfactors.TRANMID = bondtransactionmaster.TRANMID;
                                bondtransactionmasterfactors.DORDRID = Convert.ToInt16(DORDRID[count2]);
                                bondtransactionmasterfactors.DEDMODE = DEDMODE[count2].ToString();
                                bondtransactionmasterfactors.DEDVALUE = Convert.ToDecimal(DEDVALUE[count2]);
                                bondtransactionmasterfactors.DEDTYPE = Convert.ToInt16(DEDTYPE[count2]);
                                bondtransactionmasterfactors.DEDEXPRN = Convert.ToDecimal(DEDEXPRN[count2]);
                                bondtransactionmasterfactors.CFID = Convert.ToInt32(TAX1[count2]);
                                //bondtransactionmasterfactors.DEDCFDESC = CFDESC[count2];
                                bondtransactionmasterfactors.DEDNOS = Convert.ToDecimal(DEDNOS[count2]);
                                bondtransactionmasterfactors.CFOPTN = 0;
                                bondtransactionmasterfactors.DEDORDR = 0;
                                context.bondtransactionmasterfactor.Add(bondtransactionmasterfactors);
                                context.SaveChanges();
                            }
                        }
                        context.Database.ExecuteSqlCommand("DELETE FROM bondtransactiondetail  WHERE TRANMID=" + TRANMID + " and  TRANDID NOT IN(" + DELIDS.Substring(1) + ")");
                        trans.Commit();
                    }
                    catch (SqlException ex)
                    {
                        trans.Rollback();
                        throw ex;
                        // Response.Write("Sorry!!An Error Ocurred...");
                    }
                }
            }
            Response.Redirect("Index");
        }

        #region Select Ex Bond VT Details for Selected Bond ID
        public JsonResult GetVTExBondDetail(string id)//vehicl
        {

            var vtdid = 0;

            if (id != "" && id != "0" && id != "null" && id != "undefined")
            { vtdid = Convert.ToInt32(id); }
            var compyid = 0;
            compyid = Convert.ToInt32(Session["compyid"]);
            string vtbqry = "exec pr_Get_ExBond_VT_Info @compyid  = " + compyid + " ,  @VTDID  = " + vtdid;
            var query = context.Database.SqlQuery<pr_Get_ExBond_VT_Info_Result>(vtbqry).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete ExBond VT Invoice
        //[Authorize(Roles = "ExBondVTInvoiceDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            // Code Modified for validating the by Rajesh / Yamuna on 16-Jul-2021 <Start>
            String temp = Delete_fun.delete_check1(fld, id);
            //var d = context.Database.SqlQuery<int>("Select count(GIDID) as 'Cnt' from AUTHORIZATIONSLIPDETAIL (nolock) where GIDID=" + Convert.ToInt32(id)).ToList();


            //if (d[0] == 0 || d[0] == null )
            if (temp.Equals("PROCEED"))
            // Code Modified for validating the by Rajesh / Yamuna on 16-Jul-2021 <End>
            {
                BondTransactionMaster bondinv = context.bondtransactionmaster.Find(Convert.ToInt32(id));
                context.bondtransactionmaster.Remove(bondinv);
                context.SaveChanges();

                Response.Write("Deleted Successfully ...");
            }
            else
            {
                Response.Write("Deletion is not possible!");
            }

        }
        #endregion
        public void PrintView(int? id = 0)
        {

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM NEW_TMPRPT_IDS WHERE  OPTNSTR='BONDVTINVOICE' and (KUSRID ='" + Session["CUSRID"] + "' or RPTID = " + Convert.ToInt32(id)+")");
            //context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS");
            var TMPRPT_IDS = TMP_InsertPrint.In_VT_Bond_NewInsertToTMP("NEW_TMPRPT_IDS", "BONDVTINVOICE", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                CrystalDecisions.CrystalReports.Engine.Tables CrTables;

                // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");

                //........gET bnd id and ex bond id
                int bndid = 0; int ebndid = 0;
                var BQuery = context.Database.SqlQuery<BondTransactionDetail>("Select * From bondtransactiondetail where TRANMID=" + id).ToList();
                if (BQuery.Count() != 0)
                {
                    bndid = Convert.ToInt32(BQuery[0].BNDID);
                    ebndid = Convert.ToInt32(BQuery[0].EBNDID);
                }

                int tranbtype = 0;
                var eQuery = context.Database.SqlQuery<BondTransactionMaster>("Select * From bondtransactionmaster where TRANMID=" + id).ToList();
                if (eQuery.Count() != 0)
                {

                    tranbtype = Convert.ToInt32(eQuery[0].TRANTID);
                }

                //....... Get EX Bond DATE........//
                //string trandate = "";
                //var AQuery = context.Database.SqlQuery<BondTransactionMaster>("Select * From bondtransactionmaster where TRANMID=" + id).ToList();
                //if (AQuery.Count() != 0) { trandate = Convert.ToDateTime(AQuery[0].TRANDATE).Date.ToString("dd-MMM-yyyy"); }

                string trandate = "";
                var AQuery = context.Database.SqlQuery<BondMaster>("Select * From BondMaster where BNDID=" + bndid).ToList();
                if (AQuery.Count() != 0) { trandate = Convert.ToDateTime(AQuery[0].BNDTDATE).Date.ToString("dd-MMM-yyyy"); }

                //........Get previous packages...//
                decimal tmpPNOP = 0;
                //var CQuery = context.Database.SqlQuery<decimal>("select SUM(EBNDNOP) from EXBONDMASTER where BNDID=" + bndid + " AND EBNDEDATE < '" + trandate + "'").ToList();
                //var CQuery = context.Database.SqlQuery<Z_pr_ExBond_NOP_Assgn_Result>("select SUM(EBNDNOP) from EXBONDMASTER where BNDID=" + bndid + " AND EBNDEDATE < '" + trandate + "'").ToList();
                //var CQuery = context.Database.SqlQuery<Z_pr_ExBond_NOP_Assgn_Result>("EXEC Z_pr_ExBond_NOP_Assgn @PBNDID = " + bndid + ", @PEBNDEDATE = '" + trandate + "'").ToList();
                //if (CQuery.Count() != 0) { tmpPNOP = Convert.ToDecimal(CQuery[0].EBNDNOP); }

                //........Get CURRENT packages...//
                decimal tmpBNOP = 0;
                //var DQuery = context.Database.SqlQuery<decimal>("Select BBNDNOP From Z_TRANSACTION_BONDPRINT_ASSGN Where TRANMID=" + id).ToList();
                //if (DQuery.Count() != 0) 
                //{
                //    tmpBNOP = Convert.ToDecimal(DQuery[0]) - tmpPNOP;
                //}

                //if (tmpBNOP < 0) { tmpBNOP = 0; }


                string rptname = "";
                string QMDNO = id.ToString();
                var qmbtype = 0;
                var statetype = 0;
                string hstr = "";
                //var result = context.Database.SqlQuery<bondtransactionmaster>("Select * From bondtransactionmaster where TRANMID=" + id).ToList();
                //if (result.Count() != 0) { QMDNO = result[0].TRANNO.ToString(); }

                var strPath = ConfigurationManager.AppSettings["Reporturl"];

                var pdfPath = ConfigurationManager.AppSettings["pdfurl"];

                //cryRpt.Load("d:\\Reports\\VBJReports\\PurchaseOrder.Rpt");

                //if (tranbtype == 4)
                //{
                //    rptname = "B_1007.rpt";
                //    hstr = "{VW_BOND_EINVOICE_TRANSACTION_DIRECT_GST_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_BOND_EINVOICE_TRANSACTION_DIRECT_GST_PRINT_ASSGN.TRANMID} = " + id;
                //}
                //else
                //{
                //    rptname = "B_1001.rpt";
                //    hstr = "{VW_BOND_EINVOICE_TRANSACTION_GST_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_BOND_EINVOICE_TRANSACTION_GST_PRINT_ASSGN.TRANMID} = " + id;
                //}
                rptname = "EXBONDINVOICE.rpt";
                hstr = "{VW_EXBOND_VT_INVOICE_TRANSACTION_GST_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXBOND_VT_INVOICE_TRANSACTION_GST_PRINT_ASSGN.TRANMID} = " + id;
                hstr = hstr + " and {VW_EXBOND_VT_INVOICE_TRANSACTION_GST_PRINT_ASSGN.OPTNSTR} = 'BONDVTINVOICE'";


                cryRpt.Load(strPath + "\\" + rptname);

                cryRpt.RecordSelectionFormula = hstr;

                string paramName = "@FPNOP";

                for (int i = 0; i < cryRpt.DataDefinition.FormulaFields.Count; i++)
                    if (cryRpt.DataDefinition.FormulaFields[i].FormulaName == "{" + paramName + "}")
                        cryRpt.DataDefinition.FormulaFields[i].Text = "'" + tmpPNOP.ToString() + "'";

                string paramName2 = "@BNOP";

                for (int i = 0; i < cryRpt.DataDefinition.FormulaFields.Count; i++)
                    if (cryRpt.DataDefinition.FormulaFields[i].FormulaName == "{" + paramName2 + "}")
                        cryRpt.DataDefinition.FormulaFields[i].Text = "'" + tmpBNOP.ToString() + "'";


                String constring = ConfigurationManager.ConnectionStrings["BondContext"].ConnectionString;
                SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
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

                //PictureObject picture = new PictureObject();
                //string path = "D:\\KGK\\" + Session["CUSRID"] + "\\Quotation";
                string path = pdfPath + "\\" + Session["CUSRID"] + "\\EXBondInvoice";
                if (!(Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);
                }
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, path + "\\" + QMDNO + ".pdf");

                cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                //cryRpt.PrintToPrinter(1,false,0,0);
                cryRpt.Close();
                cryRpt.Dispose();
                GC.Collect();
                stringbuilder.Clear();
            }

        }
        #region Autocomplete CHA Name  
        public JsonResult AutoChaname(string term)
        {
            var result = (from r in context.categorymasters.Where(x => x.CATETID == 4 && x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).OrderBy(x => x.CATENAME).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete CHA / Importer Name  
        public JsonResult AutoChanImprtrame(string term)
        {
            var result = (from r in context.categorymasters.Where(x => (x.CATETID == 4 || x.CATETID == 1) && x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).OrderBy(x => x.CATENAME).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Bond No
        public JsonResult AutoBondNo(string term)
        {
            var compyid = Convert.ToInt32(Session["compyid"]);
            var result = (from r in context.bondinfodtls.Where(x => x.COMPYID == compyid)
                          where r.BNDDNO.ToLower().Contains(term.ToLower())
                          select new { r.BNDDNO, r.BNDID }).OrderBy(x => x.BNDDNO).Distinct();

            //string bndqry = "exec pr_get_Bond_Details_for_Invoice @term= '" + term + "'";            
            //var bndqryres = context.Database.SqlQuery<pr_get_Bond_Details_for_Invoice_Result>(bndqry).ToList();
            //var result = new SelectList(bndqryres, "BNDID", "BNDDNO").ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        private List<ItemList> GetItemList(int id)
        {
            SqlDataReader reader = null;
            //string _connStr = ConfigurationManager.ConnectionStrings["BondContext"].ConnectionString; 
            string _connStr = ConfigurationManager.ConnectionStrings["SCFS_BondContext"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(_connStr);

            SqlCommand sqlCmd = new SqlCommand("pr_EInvoice_Bond_Transaction_Detail_Assgn", myConnection);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@PTranMID", id);
            sqlCmd.Connection = myConnection;
            myConnection.Open();
            reader = sqlCmd.ExecuteReader();

            List<ItemList> ItemList = new List<ItemList>();

            while (reader.Read())
            {

                ItemList.Add(new ItemList
                {
                    SlNo = 1,
                    PrdDesc = reader["PrdDesc"].ToString(),
                    IsServc = "Y",
                    HsnCd = reader["HsnCd"].ToString(),
                    Barcde = "123456",
                    Qty = 1,
                    FreeQty = 0,
                    Unit = reader["UnitCode"].ToString(),
                    UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                    TotAmt = Convert.ToDecimal(reader["TotAmt"]),
                    Discount = 0,
                    PreTaxVal = 1,
                    AssAmt = Convert.ToDecimal(reader["AssAmt"]),
                    GstRt = Convert.ToDecimal(reader["GstRt"]),
                    IgstAmt = Convert.ToDecimal(reader["IgstAmt"]),
                    CgstAmt = Convert.ToDecimal(reader["CgstAmt"]),
                    SgstAmt = Convert.ToDecimal(reader["SgstAmt"]),
                    CesRt = 0,
                    CesAmt = 0,
                    CesNonAdvlAmt = 0,
                    StateCesRt = 0,
                    StateCesAmt = 0,
                    StateCesNonAdvlAmt = 0,
                    OthChrg = 0,
                    TotItemVal = Convert.ToDecimal(reader["TotItemVal"])
                    //OrdLineRef = "",
                    //OrgCntry = "",
                    //PrdSlNo = ""
                });
            }


            return ItemList;
        }
        public ActionResult CInvoice(int id = 0)/*10rs.reminder*/
        {

            SqlDataReader reader = null;
            SqlDataReader Sreader = null;
            //string _connStr = ConfigurationManager.ConnectionStrings["BondContext"].ConnectionString;
            string _connStr = ConfigurationManager.ConnectionStrings["SCFS_BondContext"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(_connStr);

            string _SconnStr = ConfigurationManager.ConnectionStrings["SCFSERPContext"].ConnectionString;
            SqlConnection SmyConnection = new SqlConnection(_SconnStr);

            var tranmid = id;// Convert.ToInt32(Request.Form.Get("id"));// Convert.ToInt32(ids);

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "Select * from Z_BOND_EINVOICE_DETAILS Where TRANMID = " + tranmid;
            sqlCmd.Connection = myConnection;
            myConnection.Open();
            reader = sqlCmd.ExecuteReader();

            string stringjson = "";

            decimal strgamt = 0;
            decimal strg_cgst_amt = 0;
            decimal strg_sgst_amt = 0;
            decimal strg_igst_amt = 0;

            decimal handlamt = 0;
            decimal handl_cgst_amt = 0;
            decimal handl_sgst_amt = 0;
            decimal handl_igst_amt = 0;

            decimal cgst_amt = 0;
            decimal sgst_amt = 0;
            decimal igst_amt = 0;


            while (reader.Read())
            {
                //strgamt = Convert.ToDecimal(reader["STRG_TAXABLE_AMT"]);
                //strg_cgst_amt = Convert.ToDecimal(reader["STRG_CGST_AMT"]);
                //strg_sgst_amt = Convert.ToDecimal(reader["STRG_SGST_AMT"]);
                //strg_igst_amt = Convert.ToDecimal(reader["STRG_IGST_AMT"]);

                //handlamt = Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]);
                //handl_cgst_amt = Convert.ToDecimal(reader["HANDL_CGST_AMT"]);
                //handl_sgst_amt = Convert.ToDecimal(reader["HANDL_SGST_AMT"]);
                //handl_igst_amt = Convert.ToDecimal(reader["HANDL_IGST_AMT"]);

                cgst_amt = Convert.ToDecimal(reader["CGSTAMT"]);
                sgst_amt = Convert.ToDecimal(reader["SGSTAMT"]);
                igst_amt = Convert.ToDecimal(reader["IGSTAMT"]);

                var response = new Response()
                {
                    Version = "1.1",

                    TranDtls = new TranDtls()
                    {
                        TaxSch = "GST",
                        SupTyp = "B2B",
                        RegRev = "N",
                        EcmGstin = null,
                        IgstOnIntra = "N"
                    },

                    DocDtls = new DocDtls()
                    {
                        Typ = "INV",
                        No = reader["TRANDNO"].ToString(),
                        Dt = Convert.ToDateTime(reader["TRANDATE"]).Date.ToString("dd/MM/yyyy")
                    },

                    SellerDtls = new SellerDtls()
                    {
                        Gstin = reader["COMPGSTNO"].ToString(),
                        LglNm = reader["COMPNAME"].ToString(),
                        Addr1 = reader["COMPADDR1"].ToString(),
                        Addr2 = reader["COMPADDR2"].ToString(),
                        Loc = reader["COMPLOCTDESC"].ToString(),
                        Pin = Convert.ToInt32(reader["COMPPINCODE"]),
                        Stcd = reader["COMPSTATECODE"].ToString(),
                        Ph = reader["COMPPHN1"].ToString(),
                        Em = reader["COMPMAIL"].ToString()
                    },

                    BuyerDtls = new BuyerDtls()
                    {
                        Gstin = reader["CATEBGSTNO"].ToString(),
                        LglNm = reader["TRANCHANAME"].ToString(),
                        Pos = reader["STATECODE"].ToString(),
                        Addr1 = reader["TRANIMPADDR1"].ToString(),
                        Addr2 = reader["TRANIMPADDR2"].ToString(),
                        Loc = reader["TRANIMPADDR3"].ToString(),
                        Pin = Convert.ToInt32(reader["TRANIMPADDR4"]),
                        Stcd = reader["STATECODE"].ToString(),
                        Ph = reader["CATEPHN1"].ToString(),
                        Em = null// reader["CATEMAIL"].ToString()
                    },

                    ValDtls = new ValDtls()
                    {
                        AssVal = Convert.ToDecimal(reader["TRANGAMT"]),
                        CesVal = 0,
                        CgstVal = cgst_amt,// Convert.ToDecimal(reader["HANDL_CGST_AMT"]),
                        IgstVal = igst_amt,// Convert.ToDecimal(reader["HANDL_IGST_AMT"]),
                        OthChrg = 0,
                        SgstVal = sgst_amt,// Convert.ToDecimal(reader["HANDL_sGST_AMT"]),
                        Discount = 0,
                        StCesVal = 0,
                        RndOffAmt = 0,
                        TotInvVal = Convert.ToDecimal(reader["TRANNAMT"]),
                        TotItemValSum = Convert.ToDecimal(reader["TRANGAMT"]),
                    },

                    ItemList = GetItemList(tranmid),
                    //ItemList = new List<ItemList>()
                    //{
                    //    new ItemList()
                    //    {
                    //        SlNo = 1,
                    //        PrdDesc = "Handling",
                    //        IsServc = "Y",
                    //        HsnCd = reader["HANDL_HSNCODE"].ToString(),
                    //        Barcde = "123456",
                    //        Qty = 1,
                    //        FreeQty = 0,
                    //        Unit = reader["UNITCODE"].ToString(),
                    //        UnitPrice = Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]),
                    //        TotAmt = Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]),
                    //        Discount = 0,
                    //        PreTaxVal = 1,
                    //        AssAmt = Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]),
                    //        GstRt = 18,
                    //        IgstAmt =Convert.ToDecimal(reader["HANDL_IGST_AMT"]),
                    //        CgstAmt = Convert.ToDecimal(reader["HANDL_CGST_AMT"]),
                    //        SgstAmt = Convert.ToDecimal(reader["HANDL_SGST_AMT"]),
                    //        CesRt = 0,
                    //        CesAmt = 0,
                    //        CesNonAdvlAmt = 0,
                    //        StateCesRt = 0,
                    //        StateCesAmt = 0,
                    //        StateCesNonAdvlAmt = 0,
                    //        OthChrg = 0,
                    //        TotItemVal = Convert.ToDecimal(reader["TOTALITEMVAL"])
                    //        //OrdLineRef = "",
                    //        //OrgCntry = "",
                    //        //PrdSlNo = ""
                    //    },

                    //    new ItemList()
                    //    {
                    //        SlNo = 2,
                    //        PrdDesc = "Handling",
                    //        IsServc = "Y    ",
                    //        HsnCd = reader["HANDL_HSNCODE"].ToString(),
                    //        Barcde = "123456",
                    //        Qty = 1,
                    //        FreeQty = 0,
                    //        Unit = reader["UNITCODE"].ToString(),
                    //        UnitPrice = Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]),
                    //        TotAmt = Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]),
                    //        Discount = 0,
                    //        PreTaxVal = 1,
                    //        AssAmt = Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]),
                    //        GstRt = 18,
                    //        IgstAmt =Convert.ToDecimal(reader["HANDL_IGST_AMT"]),
                    //        CgstAmt = Convert.ToDecimal(reader["HANDL_CGST_AMT"]),
                    //        SgstAmt = Convert.ToDecimal(reader["HANDL_SGST_AMT"]),
                    //        CesRt = 0,
                    //        CesAmt = 0,
                    //        CesNonAdvlAmt = 0,
                    //        StateCesRt = 0,
                    //        StateCesAmt = 0,
                    //        StateCesNonAdvlAmt = 0,
                    //        OthChrg = 0,
                    //        TotItemVal = Convert.ToDecimal(reader["TOTALITEMVAL"])
                    //        //OrdLineRef = "",
                    //        //OrgCntry = "",
                    //        //PrdSlNo = ""
                    //    },


                    //}

                };

                stringjson = JsonConvert.SerializeObject(response);
                //update
                string result = "";
                DataTable dt = new DataTable();
                SqlCommand SsqlCmd = new SqlCommand();
                SsqlCmd.CommandType = CommandType.Text;
                SsqlCmd.CommandText = "Select * from Ebondtransactionmaster Where TRANMID = " + tranmid;
                SsqlCmd.Connection = SmyConnection;
                SmyConnection.Open();
                // Sreader = SsqlCmd.ExecuteReader();
                SqlDataAdapter Sqladapter = new SqlDataAdapter(SsqlCmd);
                Sqladapter.Fill(dt);
                //dt.Load(Sreader);
                // int numRows = dt.Rows.Count;



                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow row in dt.Rows)
                    {
                        SqlConnection ZmyConnection = new SqlConnection(_SconnStr);
                        SqlCommand cmd = new SqlCommand("ETransaction_Update_Assgn", ZmyConnection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@CustomerID", 0);    
                        cmd.Parameters.AddWithValue("@PTranMID", tranmid);
                        cmd.Parameters.AddWithValue("@PEINVDESC", stringjson);
                        cmd.Parameters.AddWithValue("@PECINVDESC", row["ECINVDESC"].ToString());
                        ZmyConnection.Open();
                        //result = cmd.ExecuteScalar().ToString();
                        ZmyConnection.Close();
                    }

                    //while (Sreader.Read()) 
                    //{
                    //    SqlCommand cmd = new SqlCommand("ETransaction_Update_Assgn", SmyConnection);
                    //    cmd.CommandType = CommandType.StoredProcedure;
                    //    //cmd.Parameters.AddWithValue("@CustomerID", 0);    
                    //    cmd.Parameters.AddWithValue("@PTranMID", tranmid);
                    //    cmd.Parameters.AddWithValue("@PEINVDESC", stringjson);
                    //    cmd.Parameters.AddWithValue("@PECINVDESC", Sreader["ECINVDESC"].ToString());
                    //    SmyConnection.Open();
                    //    result = cmd.ExecuteScalar().ToString();
                    //    SmyConnection.Close();
                    //}

                }
                else
                {
                    SqlConnection ZmyConnection = new SqlConnection(_SconnStr);
                    SqlCommand cmd = new SqlCommand("ETransaction_Insert_Assgn", ZmyConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@CustomerID", 0);    
                    cmd.Parameters.AddWithValue("@PTranMID", tranmid);
                    cmd.Parameters.AddWithValue("@PEINVDESC", stringjson);
                    cmd.Parameters.AddWithValue("@PECINVDESC", "");
                    ZmyConnection.Open();
                    cmd.ExecuteNonQuery();
                    ZmyConnection.Close();
                    //result = cmd.ExecuteNonQuery().ToString();
                }


                //update

            }

            //  var strPostData = "https://www.fusiontec.com/ebill2/check.php?ids=" + stringjson;





            // string name = stuff.Irn;// responseString.Substring(2);// stuff.Name;
            // string address = stuff.Address.City;


            //context.Database.ExecuteSqlCommand("insert into SMS_STATUS_INFO(kusrid,optnstr,rptid,LastModified,MobileNumber)select '" + Session["CUSRID"] + "','" + responseString + "'," + sql[i].STUDENT_ID + ",'" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "','" + sql[i].STUDENT_PHNNO + "'");



            SmyConnection.Close();
            myConnection.Close();

            return Content(stringjson);

            //Response.Write(msg);

            //var sterm = term.TrimEnd(',');

            ////  var textsms = "Dear Student, we are pleased to confirm that your scholarship amount has been transferred by NEFT directly to your bank account. Please verify receipt from your bank. Thereafter you must check your email, login using the link provided and acknowledge receipt of the payment, positively within 15 days from today's date. If you fail to acknowledge online your scholarship will be cancelled immediately and you will receive no further payments.";
            //var textsms = GetSMStext(4);
            ////var sql = context.Database.SqlQuery<Student_Detail>("select * from Student_Detail where STUDENT_ID in (" + sterm + ")").ToList();Session["FSTAGEID"].ToString()
            //var sql = context.Database.SqlQuery<Student_Detail>("select * from Student_Detail where STUDENT_ID not in (" + sterm + ") and STAGEID = " + Convert.ToInt32(Session["FSTAGEID"]) + " and CATEID = " + Convert.ToInt32(Session["FCATEID"]) + " and DISPSTATUS=0 and CYRID not in(4,7)").ToList();
            //for (int i = 0; i < sql.Count; i++)
            //{
            //    try
            //    {
            //        var sentack = CheckAndUpdate.CheckCondition("STUDENT_PAYMENT_DETAIL", "SENT_ACK", "STUDENT_ID=" + sql[i].STUDENT_ID + "");
            //        if (sentack != "1.00")
            //            context.Database.ExecuteSqlCommand("UPDATE STUDENT_PAYMENT_DETAIL SET RS_10_ACK='No',SENT_ACK='0',SMS_SENT=1,LINKSENT_DATE='" + DateTime.Now.Date.ToString("MM/dd/yyyy") + "' WHERE STUDENT_ID =" + sql[i].STUDENT_ID + "");
            //        else
            //            context.Database.ExecuteSqlCommand("UPDATE STUDENT_PAYMENT_DETAIL SET SMS_SENT=1,LINKSENT_DATE='" + DateTime.Now.Date.ToString("MM/dd/yyyy") + "' WHERE STUDENT_ID =" + sql[i].STUDENT_ID + "");
            //        //var strPostData = "http://api.msg91.com/api/sendhttp.php?authkey=71405A7Yy0Qqi53ff0539&mobiles=" + sql[i].STUDENT_PHNNO + "&message=" + textsms + "&sender=MVDSAF&route=4&response=json";
            //        //HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(strPostData);
            //        //HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
            //        //System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
            //        //// string responseString = respStreamReader.ReadToEnd();
            //        //string responseString = respStreamReader.ReadLine().Substring(46).Substring(0, 7);
            //        //context.Database.ExecuteSqlCommand("insert into SMS_STATUS_INFO(kusrid,optnstr,rptid,LastModified,MobileNumber)select '" + Session["CUSRID"] + "','" + responseString + "'," + sql[i].STUDENT_ID + ",'" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "','" + sql[i].STUDENT_PHNNO + "'");
            //        //Response.Write("updated Succesfully");
            //    }
            //    catch (Exception e)
            //    {
            //        context.Database.ExecuteSqlCommand("insert into SMS_STATUS_INFO(kusrid,optnstr,rptid,LastModified,MobileNumber)select '" + Session["CUSRID"] + "','" + e.Message + "'," + sql[i].STUDENT_ID + ",'" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "','" + sql[i].STUDENT_PHNNO + "'");
            //        Response.Write("Sorry Error Occurred While Processing.Contact Admin.");
            //    }
            //}
            //Response.Write("updated Succesfully");
        }
        public void UInvoice(int id = 0)/*10rs.reminder*/
        {
            SqlDataReader reader = null;
            SqlDataReader Sreader = null;
            string _connStr = ConfigurationManager.ConnectionStrings["BondContext"].ConnectionString;
            //string _connStr = ConfigurationManager.ConnectionStrings["SCFS_BondContext"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(_connStr);

            var tranmid = id;// Convert.ToInt32(Request.Form.Get("id"));// Convert.ToInt32(ids);

            var strPostData = "https://www.fusiontec.com/ebill2/einvoice.php?ids=" + id;
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(strPostData);
            HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
            System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());

            // string responseString = respStreamReader.ReadToEnd();
            string responseString = respStreamReader.ReadLine();//.Substring(46).Substring(0, 7);

            responseString = responseString.Replace("<br>", "~");
            responseString = responseString.Replace("=>", "!");
            var param = responseString.Split('~');

            var status = 0;
            string zirnno = "";// param[2].ToString();
            string zackdt = "";//param[3].ToString();
            string zackno = "";//param[4].ToString();
            string imgUrl = "";

            string msg = "";


            if (param[0] != "") { status = (Convert.ToInt32(param[0].Substring(9))); } else { status = 0; }
            if (param[1] != "") { msg = param[1].Substring(10); } else { msg = ""; }

            if (status == 1)
            {
                if (param[2] != "") { zirnno = param[2].Substring(6); } else { zirnno = ""; }
                if (param[3] != "") { zackdt = param[3].Substring(8); } else { zackdt = ""; }
                if (param[4] != "") { zackno = param[4].Substring(8); } else { zackno = ""; }
                if (param[14] != "") { imgUrl = param[14].ToString(); } else { imgUrl = ""; }

                SqlConnection GmyConnection = new SqlConnection(_connStr);
                SqlCommand cmd = new SqlCommand("pr_IRN_Bond_Transaction_Update_Assgn", GmyConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@CustomerID", 0);    
                cmd.Parameters.AddWithValue("@PTranMID", tranmid);
                cmd.Parameters.AddWithValue("@PIRNNO", zirnno);
                cmd.Parameters.AddWithValue("@PACKNO", zackno);
                cmd.Parameters.AddWithValue("@PACKDT", Convert.ToDateTime(zackdt));
                GmyConnection.Open();
                cmd.ExecuteNonQuery();
                GmyConnection.Close();

                //string remoteFileUrl = "https://my.gstzen.in/" + imgUrl;
                string remoteFileUrl = "https://fusiontec.com//ebill2//images//qrcode.png";
                string localFileName = tranmid.ToString() + ".png";

                string path = Server.MapPath("~/BQrCode");


                WebClient webClient = new WebClient();
                webClient.DownloadFile(remoteFileUrl, path + "\\" + localFileName);

                SqlConnection XmyConnection = new SqlConnection(_connStr);
                SqlCommand Xcmd = new SqlCommand("pr_Bond_Transaction_QrCode_Path_Update_Assgn", XmyConnection);
                Xcmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@CustomerID", 0);    
                Xcmd.Parameters.AddWithValue("@PTranMID", tranmid);
                Xcmd.Parameters.AddWithValue("@PPath", path + "\\" + localFileName);
                XmyConnection.Open();
                Xcmd.ExecuteNonQuery();
                //result = cmd.ExecuteScalar().ToString();
                XmyConnection.Close();

                msg = "Uploaded Succesfully";

            }
            else
            {
                //msg = "";

            }
            Response.Write(msg);
            
        }

    }
}