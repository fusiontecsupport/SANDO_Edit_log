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
using scfs.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Tables = CrystalDecisions.CrystalReports.Engine.Tables;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Data;

namespace scfs_erp.Controllers.Bond
{
    [SessionExpire]
    public class BondInformationController : Controller
    {
        // GET: BondInformation

        #region contextdeclaration
        BondContext context = new BondContext();

        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        #endregion

        #region IndexForm
        //[Authorize(Roles = "BondInformationIndex")]
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

            return View(context.bondinfodtls.Where(x => x.BNDDATE >= sd).Where(x => x.BNDDATE <= ed).Where(x => x.SDPTID == 10).ToList());

            //return View();
        }
        #endregion

        #region GetAjaxData
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {

            using (var e = new BondEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Bond_Information(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]));

                var aaData = data.Select(d => new string[] { d.BNDDATE, d.BNDNO.ToString(), d.BNDDNO, d.CHANAME, d.IMPRTRNAME, d.BNDCIFAMT.ToString(),    d.DISPSTATUS, d.BNDID.ToString() }).ToArray();

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

        #region FormModify
        //[Authorize(Roles = "BondInformationEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            //string url = "" + strPath + "/BondInformation/Form/" + id;

            Response.Redirect("" + strPath + "/BondInformation/Form/" + id);

            //Response.Redirect("/BondInformation/Form/" + id);
        }
        #endregion

        #region Form
        ////[Authorize(Roles = "BondInformationCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            
            BondMaster tab = new BondMaster();
            
            tab.BNDDATE = Convert.ToDateTime(DateTime.Now).Date;
            tab.INSRSDATE = DateTime.Now.Date;


            //-------------------Dropdown List--------------------------------------------------//

            var mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_BondMaster_Status ").ToList();
            ViewBag.DISPSTATUS = new SelectList(mtqry, "dval", "dtxt").ToList();

            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Types ").ToList();
            ViewBag.BNDTYPE = new SelectList(mtqry, "dval", "dtxt").ToList();

            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Entry_Types ").ToList();
            ViewBag.BNDETYPE = new SelectList(mtqry, "dval", "dtxt").ToList();

            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Operation_Types ").ToList();
            ViewBag.TYPEID = new SelectList(mtqry, "dval", "dtxt").ToList();

            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Godown_Types ").ToList();
            ViewBag.GDWNTYPE = new SelectList(mtqry, "dval", "dtxt", 2).ToList();

            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Godowns 2 ").ToList();
            ViewBag.GWNID = new SelectList(mtqry, "dval", "dtxt").ToList();
            //ViewBag.GWNID = new SelectList("");

            ViewBag.PRDTGID = new SelectList(context.bondproductgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");
            ViewBag.PRDTTID = new SelectList(context.producttypemasters, "PRDTTID", "PRDTTDESC");



            if (id != 0)//--Edit Mode
            {
                tab = context.bondinfodtls.Find(id);
                
                CategoryMaster CHA = new CategoryMaster();
                CHA = context.categorymasters.Find(tab.CHAID);
                ViewBag.CHANAME = CHA.CATENAME.ToString();
                CHA = context.categorymasters.Find(tab.IMPRTID);
                ViewBag.IMPRTRNAME = CHA.CATENAME.ToString();

                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_BondMaster_Status ").ToList();
                ViewBag.DISPSTATUS = new SelectList(mtqry, "dval", "dtxt",tab.DISPSTATUS).ToList();

                
                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Types ").ToList();
                ViewBag.BNDTYPE = new SelectList(mtqry, "dval", "dtxt",tab.BNDTYPE).ToList();

                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Entry_Types ").ToList();
                ViewBag.BNDETYPE = new SelectList(mtqry, "dval", "dtxt", tab.BNDETYPE).ToList();

                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Operation_Types ").ToList();
                ViewBag.TYPEID = new SelectList(mtqry, "dval", "dtxt",tab.TYPEID).ToList();

                BondGodownMaster bgwn = new BondGodownMaster();

                bgwn = context.bondgodownmasters.Find(tab.GWNID);

                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Godown_Types ").ToList();
                ViewBag.GDWNTYPE = new SelectList(mtqry, "dval", "dtxt", bgwn.GWNTID).ToList();

                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Godowns " + bgwn.GWNTID).ToList();
                ViewBag.GWNID = new SelectList(mtqry, "dval", "dtxt", tab.GWNID).ToList();

                ViewBag.PRDTGID = new SelectList(context.bondproductgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", tab.PRDTGID);
                ViewBag.PRDTTID = new SelectList(context.producttypemasters, "PRDTTID", "PRDTTDESC",tab.PRDTTID);


            }
            

            return View(tab);
        }
        #endregion

        #region Savedata
        

        public void SaveData(BondMaster tab)
        {
            
            using (BondContext context = new BondContext())
            {
                //using (var trans1 = context.Database.BeginTransaction())
                //{

                //using (var trans = context.Database.BeginTransaction())
                //{
                try

                {

                    var sql = context.Database.SqlQuery<int>("select count('*') from bondmaster(nolock) Where  BNDDNO ='" + tab.BNDDNO + "' AND BNDID <> "+ tab.BNDID+" and SDPTID=10").ToList(); //COMPYID = " + Convert.ToInt32(Session["compyid"]) + " and

                    if (sql[0] > 0)
                    {
                        Response.Write("Exists");
                    }
                    else
                    {
                        string todaydt = Convert.ToString(DateTime.Now);
                        string todayd = Convert.ToString(DateTime.Now.Date);

                        
                        tab.YRDID = 0;
                        tab.PRCSDATE = DateTime.Now;
                        tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                        tab.SDPTID = 10;

                        if (tab.BNDID.ToString() != "0")
                            tab.LMUSRID = Session["CUSRID"].ToString();
                        else
                            tab.CUSRID = Session["CUSRID"].ToString();



                        if (tab.BNDID.ToString() != "0")
                        {
                            using (var trans = context.Database.BeginTransaction())
                            {

                                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                trans.Commit();
                            }
                        }

                        else
                        {

                            using (var trans = context.Database.BeginTransaction())
                            {
                                tab.BNDNO = Convert.ToInt32(Autonumber.autonum("bondmaster", "BNDNO", "BNDNO <> 0 AND SDPTID = 10 and compyid = " + Convert.ToInt32(Session["compyid"]) + "").ToString());
                                //int ano = tab.BNDNO;
                                //string prfx = string.Format("{0:D5}", ano);
                                //tab.BNDDNO = prfx.ToString();

                                
                                context.bondinfodtls.Add(tab);
                                //context.Entry(tab).State = System.Data.Entity.EntityState.Added;
                                context.SaveChanges();
                                trans.Commit();
                            }

                        }
                        Response.Write("Success");


                    }
                }
                
                catch (Exception E)
                {
                    Response.Write(E);
                    //trans.Rollback();
                      Response.Write("Sorry!! An Error Occurred.... ");
                    //Response.Redirect("/Error/AccessDenied");
                }
                //}
            }


        }
        #endregion

        #region Bond Number Duplicate Check
        public void BondNo_Duplicate_Check(string BNDDNO)
        {
            BNDDNO = Request.Form.Get("BNDDNO");
            
            string temp = BondNo_Check.recordCount(BNDDNO);
            if (temp != "PROCEED")
            {
                Response.Write("Bond Number already exists");

            }
            else
            {
                Response.Write("PROCEED");
            }

        }

        #endregion

        #region Autocomplete Vessel Name        
        public JsonResult AutoVessel(string term)
        {
            var result = (from vessel in context.vesselmasters
                          where vessel.VSLDESC.ToLower().Contains(term.ToLower())
                          select new { vessel.VSLDESC, vessel.VSLID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Transporter Name        
        public JsonResult AutoTransporter(string term)
        {
            var result = (from r in context.categorymasters.Where(x => x.CATETID == 5 && x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).OrderBy(x => x.CATENAME).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Steamer Name    
        public JsonResult AutoSteamer(string term)
        {
            var result = (from category in context.categorymasters.Where(m => m.CATETID == 3).Where(x => x.DISPSTATUS == 0)
                          where category.CATENAME.ToLower().Contains(term.ToLower())
                          select new { category.CATENAME, category.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Importer Name          
        public JsonResult AutoImporter(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 1).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete CHA Name  
        public JsonResult AutoChaname(string term)
        {
            var result = (from r in context.categorymasters.Where(x => x.CATETID == 4 && x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).OrderBy(x => x.CATENAME).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Billing CHA Name  
        public JsonResult AutoBChaname(string term)
        {
            //var result = (from r in context.categorymasters.Where(x => (x.CATETID == 4) && x.DISPSTATUS == 0)
            //              where r.CATENAME.ToLower().Contains(term.ToLower())
            //              select new { r.CATENAME, r.CATEID }).OrderBy(x => x.CATENAME).Distinct();
            var e = new SCFSERPEntities();
            var result = e.pr_Fetch_CHAIMP_Dtl(4, term.ToString());

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Slot Name  
        public JsonResult AutoSlot(string term)
        {
            var result = (from slot in context.slotmasters.Where(x => x.DISPSTATUS == 0)
                          where slot.SLOTDESC.ToLower().Contains(term.ToLower())
                          select new { slot.SLOTID, slot.SLOTDESC }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete cascadingS dropdown          
        public JsonResult GetSlot(int id)
        {
            var slot = (from a in context.slotmasters.Where(x => x.DISPSTATUS == 0) where a.SLOTID == id select a).ToList();
            return new JsonResult() { Data = slot, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region Autocomplete Vehicle based On  Id  
        public JsonResult GetVehicle(string id)//vehicl
        {
            var param = id.Split('-');
            var tid = 0;
            var vid = 0;

            if (param[0] != "" || param[0] != "0" || param[0] != null)
            { tid = Convert.ToInt32(param[0]); }
            else { tid = 0; }

            if (param[1] != "" || param[1] != "0" || param[1] != null)
            { vid = Convert.ToInt32(param[1]); }
            else { vid = 0; }

            var query = context.Database.SqlQuery<VehicleMaster>("select * from VehicleMaster WHERE TRNSPRTID = " + tid + " and VHLMID = " + vid + "").ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Godown Master for Selected Godown Type  
        public JsonResult GetGodownDtl(string id)//vehicl
        {
            
            var gwtid = 0;
            
            if (id != "" && id != "0" && id != null)
            { gwtid = Convert.ToInt32(id); }
            


            var query = context.Database.SqlQuery<BondGodownMaster>("select * from BondGodownMaster (nolock) WHERE GWNTID = " + gwtid + "").ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Vehicle PNR On  Id  
        public JsonResult AutoVehicleNo(string term)
        {
            string vno = ""; int Tid = 0;
            var Param = term.Split(';');

            if (Param[0] != "" || Param[0] != null) { vno = Convert.ToString(Param[0]); } else { vno = ""; }
            if (Param[1] != "" || Param[1] != null) { Tid = Convert.ToInt32(Param[1]); } else { Tid = 0; }


            var result = (from vehicle in context.vehiclemasters.Where(m => m.DISPSTATUS == 0 && m.TRNSPRTID == Tid)
                          where vehicle.VHLMDESC.ToLower().Contains(vno.ToLower())
                          select new { vehicle.VHLMDESC, vehicle.VHLMID }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

       

        //--------Autocomplete CHA Name
        public JsonResult NewAutoCha(string term)
        {

            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID, r.CATEBGSTNO }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //cha and importer

        public JsonResult NewAutoImporter(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 1).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID, r.CATEBGSTNO }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        

        #region PrintView
        //[Authorize(Roles = "BondInformationPrint")]
        public void PrintView(int? id = 0)
        {
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "BondInformation", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "BondInfo.rpt");
                cryRpt.RecordSelectionFormula = "{VW_BOND_PRINT_ASSGN.KUSRID} ='" + Session["CUSRID"].ToString() + "' and {VW_BOND_PRINT_ASSGN.BNDID} =" + id;

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


        //[Authorize(Roles = "BondInformationPrint")]
        public void TPrintView(int? id = 0)/*truck*/
        {
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "NONPNRTRUCKIN", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                //cryRpt.Load("D:\\scfsreports\\NonPnr_TruckIn.rpt");
                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "BondInfo.rpt");
                cryRpt.RecordSelectionFormula = "{VW_BOND_PRINT_ASSGN.KUSRID} ='" + Session["CUSRID"].ToString() + "' and {VW_BOND_PRINT_ASSGN.BNDID} =" + id;

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
        #endregion

        #region DeleteBondInfo        
        //[Authorize(Roles = "BondInformationDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            // Code Modified for validating the by Rajesh / Yamuna on 16-Jul-2021 <Start>
            String temp = Delete_fun.delete_check1(fld, id);
            //var d = context.Database.SqlQuery<int>("Select count(BNDID) as 'Cnt' from AUTHORIZATIONSLIPDETAIL (nolock) where BNDID=" + Convert.ToInt32(id)).ToList();


            //if (d[0] == 0 || d[0] == null )
            if (temp.Equals("PROCEED"))
            // Code Modified for validating the by Rajesh / Yamuna on 16-Jul-2021 <End>
            {
                var sql = context.Database.SqlQuery<int>("SELECT BNDID from BondMaster where BNDID=" + Convert.ToInt32(id)).ToList();
                var bndid = (sql[0]).ToString();
                BondMaster bondinfodtls = context.bondinfodtls.Find(Convert.ToInt32(bndid));
                context.bondinfodtls.Remove(bondinfodtls);
                context.SaveChanges();

                Response.Write("Deleted Successfully ...");
            }
            else
            {
                // Code Modified for validating the by Rajesh / Yamuna on 16-Jul-2021 <Start>
                Response.Write("Record already exists, deletion is not possible!");
                // Code Modified for validating the by Rajesh / Yamuna on 16-Jul-2021 <End>
            }

        }
        #endregion
    }
}