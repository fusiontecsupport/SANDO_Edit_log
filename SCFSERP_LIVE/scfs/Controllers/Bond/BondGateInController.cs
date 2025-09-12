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
using DocumentFormat.OpenXml.Drawing;

namespace scfs_erp.Controllers.Bond
{
    [SessionExpire]
    public class BondGateInController : Controller
    {
        // GET: BondGateIn

        #region contextdeclaration
        BondContext context = new BondContext();

        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        #endregion

        #region IndexForm
        //[Authorize(Roles = "BondGateInIndex")]
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

            return View(context.bondgateindtls.Where(x => x.GIDATE >= sd).Where(x => x.GIDATE <= ed).Where(x => x.SDPTID == 10).ToList());

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

                var data = e.pr_Search_Bond_GateIn(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]));

                var aaData = data.Select(d => new string[] { d.GIDATE.Value.ToString("dd/MM/yyyy"), d.GINO.ToString(), d.BONDNO, d.CONTNRNO.ToString(), d.CONTNRSIZE.ToString(), d.STMRNAME, d.IMPRTRNAME, d.PRDTDESC,d.IGMNO, d.DISPSTATUS, d.GIDID.ToString() }).ToArray();

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
        //[Authorize(Roles = "BondGateInEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            //string url = "" + strPath + "/BondGateIn/Form/" + id;

            Response.Redirect("" + strPath + "/BondGateIn/Form/" + id);

            //Response.Redirect("/BondGateIn/Form/" + id);
        }
        #endregion

        #region Form
        ////[Authorize(Roles = "BondGateInCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            RemoteGateIn remotegatein = new RemoteGateIn();
            BondGateInDetail tab = new BondGateInDetail();

            tab.GIDATE = Convert.ToDateTime(DateTime.Now).Date;
            


            //-------------------Dropdown List--------------------------------------------------//

            var mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_BondMaster_Status ").ToList();
            ViewBag.DISPSTATUS = new SelectList(mtqry, "dval", "dtxt").ToList();

            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Types ").ToList();
            ViewBag.BNDTYPE = new SelectList(mtqry, "dval", "dtxt").ToList();


            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Operation_Types ").ToList();
            ViewBag.TYPEID = new SelectList(mtqry, "dval", "dtxt").ToList();

            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Godown_Types ").ToList();
            ViewBag.GDWNTYPE = new SelectList(mtqry, "dval", "dtxt",2).ToList();

            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_Bond_Godowns 2").ToList();
            //ViewBag.GWNID = new SelectList(mtqry, "dval", "dtxt").ToList();
            ViewBag.GWNID = new SelectList("");
            ViewBag.VSLNAME = new SelectList(context.vesselmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.VSLDESC), "VSLID", "VSLDESC",1);

            mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_Bond_Contnr_Recev_Frm ").ToList();            
            ViewBag.CONTNRRCVFRM = new SelectList(mtqry, "dval", "dtxt").ToList();

            ViewBag.PRDTGID = new SelectList(context.bondproductgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");
            ViewBag.PRDTTID = new SelectList(context.producttypemasters, "PRDTTID", "PRDTTDESC");

            ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CONTNRTDESC), "CONTNRTID", "CONTNRTDESC");
            ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID > 1).Where(x => x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");

            if (id != 0)//--Edit Mode
            {
                tab = context.bondgateindtls.Find(id);
                BondMaster bnd = new BondMaster();
                if(tab.BNDID>0)
                {
                    bnd = context.bondinfodtls.Find(tab.BNDID);
                    ViewBag.BNDDNO = bnd.BNDDNO;
                }
                CategoryMaster CHA = new CategoryMaster();
                CHA = context.categorymasters.Find(tab.CHAID);
                ViewBag.CHANAME = CHA.CATENAME.ToString();
                CHA = context.categorymasters.Find(tab.STMRID);
                ViewBag.STMRNAME = CHA.CATENAME.ToString();
                CHA = context.categorymasters.Find(tab.IMPRTID);
                ViewBag.IMPRTRNAME = CHA.CATENAME.ToString();

                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_get_BondMaster_Status ").ToList();
                ViewBag.DISPSTATUS = new SelectList(mtqry, "dval", "dtxt", tab.DISPSTATUS).ToList();
                mtqry = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_Bond_Contnr_Recev_Frm ").ToList();
                ViewBag.CONTNRRCVFRM = new SelectList(mtqry, "dval", "dtxt",tab.CONTNRRCVFRM).ToList();
                ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CONTNRTDESC), "CONTNRTID", "CONTNRTDESC", tab.CONTNRTID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID > 1).Where(x => x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.VSLNAME = new SelectList(context.vesselmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.VSLDESC), "VSLID", "VSLDESC",tab.VSLID);

                ViewBag.PRDTGID = new SelectList(context.bondproductgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", tab.PRDTGID);
                


            }


            return View(tab);
        }
        #endregion

        #region Savedata
        public void SaveData(BondGateInDetail tab)
        {

            using (BondContext context = new BondContext())
            {
                //using (var trans1 = context.Database.BeginTransaction())
                //{

                //using (var trans = context.Database.BeginTransaction())
                //{
                try

                {
                    bool exists = false;
                    //var sql = context.Database.SqlQuery<int>("select count('*') from BondGateInDetail(nolock) Where COMPYID = " + Convert.ToInt32(Session["compyid"]) + " and BNDID =" + tab.BNDID+ " and CONTNRNO ='" + tab.CONTNRNO + "' and SDPTID=10").ToList();
                    string ids = tab.CONTNRNO + "~" + tab.BNDID.ToString();
                    var sqla = context.Database.SqlQuery<int>("select ISNULL(max( bondgateindetail.GIDID),0) from bondgateindetail  where bondgateindetail.SDPTID=10 and bondgateindetail.CONTNRNO='" + tab.CONTNRNO + "' and bondgateindetail.BNDID = " + tab.BNDID).ToList();

                    if (sqla[0] > 0)
                    {
                        //for (int i = 0; i < sqla.Count; i++)
                        //{

                        var sql = context.Database.SqlQuery<int>("select bondgateindetail.GIDID from bondgateindetail inner join bondgateoutdetail on bondgateindetail.GIDID=bondgateoutdetail.GIDID where bondgateindetail.GIDID=" + Convert.ToInt32(sqla[0]) + " and bondgateindetail.SDPTID=10 and bondgateindetail.CONTNRNO='" + tab.CONTNRNO + "' and bondgateindetail.BNDID = " + tab.BNDID).ToList();

                        if (sql.Count > 0)
                        {
                            exists = false;
                        }
                        else
                        {
                            exists = true;
                        }
                        // }
                    }
                    else
                    {
                        exists = false;
                    }
                    //if (sql[0] > 0 && tab.GIDID == 0)
                    if (exists && tab.GIDID == 0)
                    {
                        Response.Write("Exists");
                    }
                    else
                    {
                        string todaydt = Convert.ToString(DateTime.Now);
                        string todayd = Convert.ToString(DateTime.Now.Date);

                        tab.PRCSDATE = DateTime.Now;
                        tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                        tab.SDPTID = 10;

                        if (tab.GIDID.ToString() != "0")
                        {
                            int ano = tab.GINO;
                            string prfx = string.Format("{0:D5}", ano);
                            tab.GIDNO = prfx.ToString();
                            tab.LMUSRID = Session["CUSRID"].ToString();
                        }
                        else
                            tab.CUSRID = Session["CUSRID"].ToString();


                        if (tab.VSLID > 0)
                        {
                            VesselMaster vsl = new VesselMaster();
                            vsl = context.vesselmasters.Find(tab.VSLID);
                            tab.VSLNAME = vsl.VSLDESC.Trim();
                        }
                        else
                        {
                            tab.VSLID = 1;
                            tab.VOYNO = "-";
                            //    var sql1 = context.Database.SqlQuery<int>("select VSLID from VesselMaster (nolock) where VSLDESC = '" + Convert.ToString(tab.VSLNAME) + "'").ToList();
                            //if(sql1.Count>0)
                            //{
                            //    tab.VSLID = sql1[0];
                            //}                            

                        }


                        if (tab.GIDID.ToString() != "0")
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
                                tab.GINO = Convert.ToInt32(Autonumber.autonum("BondGateInDetail", "GINO", "GINO <> 0 AND SDPTID = 10 and compyid = " + Convert.ToInt32(Session["compyid"]) + "").ToString());
                                int ano = tab.GINO;
                                string prfx = string.Format("{0:D5}", ano);
                                tab.GIDNO = prfx.ToString();


                                context.bondgateindtls.Add(tab);
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

        #region Check Bond Container Duplicate
        public void CheckBondContainerDuplicate(string id)
        {
            
            var param = id.Split('~');
            var cntnrno = "";
            var bndid = 0;

            if (param[0] != "" || param[0] != "0" || param[0] != null)
            { cntnrno = Convert.ToString(param[0]); }
            

            if (param[1] != "" || param[1] != "0" || param[1] != null)
            { bndid = Convert.ToInt32(param[1]); }
            
            var sqla = context.Database.SqlQuery<int>("select ISNULL(max( bondgateindetail.GIDID),0) from bondgateindetail  where bondgateindetail.SDPTID=10 and bondgateindetail.CONTNRNO='" + cntnrno + "' and bondgateindetail.BNDID = "+bndid).ToList();

            if (sqla[0] > 0)
            {
                //for (int i = 0; i < sqla.Count; i++)
                //{

                var sql = context.Database.SqlQuery<int>("select bondgateindetail.GIDID from bondgateindetail inner join bondgateoutdetail on bondgateindetail.GIDID=bondgateoutdetail.GIDID where bondgateindetail.GIDID=" + Convert.ToInt32(sqla[0]) + " and bondgateindetail.SDPTID=10 and bondgateindetail.CONTNRNO='" + cntnrno + "' and bondgateindetail.BNDID = " + bndid).ToList();

                if (sql.Count > 0)
                {
                    Response.Write("PROCEED");
                }
                else
                {
                    Response.Write("Container No. already Exists");
                }
                // }
            }
            else
            {
                Response.Write("PROCEED");
            }

        }
        #endregion
        #region Bond Number Duplicate Check
        public void BondNo_Duplicate_Check(string GIDNO)
        {
            GIDNO = Request.Form.Get("GIDNO");

            string temp = BondNo_Check.recordCount(GIDNO);
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

        #region Autocomplete Bond No
        public JsonResult AutoBondNo(string term)
        {
            var compyid = Convert.ToInt32(Session["compyid"]);
           // var result = (from r in context.bondinfodtls.Where(x => x.COMPYID == compyid )
           //               where r.BNDDNO.ToLower().Contains(term.ToLower())
           //               select new { r.BNDDNO, r.BNDID }).OrderBy(x => x.BNDDNO).Distinct();
            var result = (from r in context.bondinfodtls.Where(x => x.COMPYID > 0)
                          where r.BNDDNO.ToLower().Contains(term.ToLower())
                          select new { r.BNDDNO, r.BNDID }).OrderBy(x => x.BNDDNO).Distinct();
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
           
            if (id != "" && id != "0" && id != null && id != "undefined")
            { gwtid = Convert.ToInt32(id); }
            


            var query = context.Database.SqlQuery<BondGodownMaster>("select * from BondGodownMaster (nolock) WHERE GWNTID = " + gwtid + "").ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Bond Details for Selected Bond ID
        public JsonResult GetBondDetail(string id)//vehicl
        {

            var bndid = 0;

            if (id != "" && id != "0" && id != null && id != "undefined")
            { bndid = Convert.ToInt32(id); }
            var compyid = 0;
            compyid = Convert.ToInt32(Session["compyid"]);



            var query = context.Database.SqlQuery<pr_Get_Bond_Info_Result>("exec pr_Get_Bond_Info @compyid  = " + compyid + " ,  @bondid  = " + bndid).ToList();

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
        //[Authorize(Roles = "BondGateInPrint")]
        public void PrintView(int? id = 0)
        {
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "BondGateIn", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "BondInfo.rpt");
                cryRpt.RecordSelectionFormula = "{VW_BOND_PRINT_ASSGN.KUSRID} ='" + Session["CUSRID"].ToString() + "' and {VW_BOND_PRINT_ASSGN.GIDID} =" + id;

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


        //[Authorize(Roles = "BondGateInPrint")]
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
                cryRpt.RecordSelectionFormula = "{VW_BOND_PRINT_ASSGN.KUSRID} ='" + Session["CUSRID"].ToString() + "' and {VW_BOND_PRINT_ASSGN.GIDID} =" + id;

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
        //[Authorize(Roles = "BondGateInDelete")]
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
                BondContainerOut containerOut = context.bondcontnroutdtls.Find(Convert.ToInt32(id));
                context.bondcontnroutdtls.Remove(containerOut);
                context.SaveChanges();

                Response.Write("Deleted Successfully ...");
            }
            else
            {
                Response.Write("Record already exists, deletion is not possible!");
            }

        }
        #endregion


    }
}