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
using System.Reflection;

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
                            // Load original row for logging (no tracking to avoid state conflicts)
                            // IMPORTANT: Load BEFORE any modifications to ensure we capture the true original state
                            var original = context.bondgateindtls.AsNoTracking().FirstOrDefault(x => x.GIDID == tab.GIDID);

                            if (original == null)
                            {
                                System.Diagnostics.Debug.WriteLine($"WARNING: Could not find original record for GIDID={tab.GIDID}");
                            }

                            using (var trans = context.Database.BeginTransaction())
                            {
                                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                trans.Commit();
                            }

                            // Best-effort logging to SCFS_LOG
                            try
                            {
                                System.Diagnostics.Debug.WriteLine($"BOND GATEIN SAVE METHOD CALLED: GIDID={tab.GIDID}, GIDNO={tab.GIDNO}");
                                if (original != null)
                                {
                                    System.Diagnostics.Debug.WriteLine($"ORIGINAL RECORD FOUND: GIDID={original.GIDID}, GIDNO={original.GIDNO}");
                                    // Ensure baseline snapshot (Version = "0") exists for this record before logging diffs
                                    EnsureBaselineVersionZero(original, Session["CUSRID"] != null ? Session["CUSRID"].ToString() : "");
                                    LogGateInEdits(original, tab, Session["CUSRID"] != null ? Session["CUSRID"].ToString() : "");
                                    System.Diagnostics.Debug.WriteLine($"LogGateInEdits completed successfully");
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"ORIGINAL RECORD NOT FOUND for GIDID={tab.GIDID}");
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log the error for debugging but don't fail the save
                                System.Diagnostics.Debug.WriteLine($"Edit logging failed: {ex.Message}");
                                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
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

        // ========================= Edit Log Pages =========================
        public ActionResult EditLog()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View();
        }

        public ActionResult EditLogGateIn(int? gidid, DateTime? from = null, DateTime? to = null, string user = null, string fieldName = null, string version = null)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            var list = new List<scfs_erp.Models.GateInDetailEditLogRow>();
            var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
            if (cs != null && !string.IsNullOrWhiteSpace(cs.ConnectionString))
            {
                using (var sql = new SqlConnection(cs.ConnectionString))
                using (var cmd = new SqlCommand(@"SELECT TOP 2000 [GIDNO],[FieldName],[OldValue],[NewValue],[ChangedBy],[ChangedOn],[Version],[Modules]
                                                FROM [dbo].[GateInDetailEditLog]
                                                WHERE [Modules] = 'BondGateIn'
                                                  AND (@GIDNO IS NULL OR [GIDNO] = @GIDNO)
                                                  AND (@FROM IS NULL OR [ChangedOn] >= @FROM)
                                                  AND (@TO   IS NULL OR [ChangedOn] <  DATEADD(day, 1, @TO))
                                                  AND (@USER IS NULL OR [ChangedBy] LIKE @USERPAT)
                                                  AND (@FIELD IS NULL OR [FieldName] LIKE @FIELDPAT)
                                                  AND (@VERSION IS NULL OR [Version] LIKE @VERPAT)
                                                  AND NOT (RTRIM(LTRIM([Version])) IN ('0','V0') OR LEFT(RTRIM(LTRIM([Version])),3) IN ('v0-','V0-'))
                                                ORDER BY [ChangedOn] DESC, [GIDNO] DESC", sql))
                {
                    cmd.Parameters.AddWithValue("@GIDNO", (object)gidid ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FROM", (object)from ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TO", (object)to ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@USER", string.IsNullOrWhiteSpace(user) ? (object)DBNull.Value : user);
                    cmd.Parameters.AddWithValue("@USERPAT", string.IsNullOrWhiteSpace(user) ? (object)DBNull.Value : (object)("%" + user + "%"));
                    cmd.Parameters.AddWithValue("@FIELD", string.IsNullOrWhiteSpace(fieldName) ? (object)DBNull.Value : fieldName);
                    cmd.Parameters.AddWithValue("@FIELDPAT", string.IsNullOrWhiteSpace(fieldName) ? (object)DBNull.Value : (object)("%" + fieldName + "%"));
                    cmd.Parameters.AddWithValue("@VERSION", string.IsNullOrWhiteSpace(version) ? (object)DBNull.Value : version);
                    cmd.Parameters.AddWithValue("@VERPAT", string.IsNullOrWhiteSpace(version) ? (object)DBNull.Value : (object)("%" + version + "%"));
                    sql.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new scfs_erp.Models.GateInDetailEditLogRow
                            {
                                GIDNO = Convert.ToString(r["GIDNO"]),
                                FieldName = Convert.ToString(r["FieldName"]),
                                OldValue = r["OldValue"] == DBNull.Value ? null : Convert.ToString(r["OldValue"]),
                                NewValue = r["NewValue"] == DBNull.Value ? null : Convert.ToString(r["NewValue"]),
                                ChangedBy = Convert.ToString(r["ChangedBy"]),
                                ChangedOn = r["ChangedOn"] != DBNull.Value ? Convert.ToDateTime(r["ChangedOn"]) : DateTime.MinValue,
                                Version = r["Version"] == DBNull.Value ? null : Convert.ToString(r["Version"]),
                                Modules = r["Modules"] == DBNull.Value ? null : Convert.ToString(r["Modules"])
                            });
                        }
                    }
                }
            }

            // Map raw DB codes to form-friendly display values for known fields
            try
            {
                // Build lookup dictionaries once
                var dictPrdtGrp = context.bondproductgroupmasters.ToDictionary(x => x.PRDTGID, x => x.PRDTGDESC);
                var dictPrdtType = context.producttypemasters.ToDictionary(x => x.PRDTTID, x => x.PRDTTDESC);
                var dictContType = context.containertypemasters.ToDictionary(x => x.CONTNRTID, x => x.CONTNRTDESC);
                var dictContSize = context.containersizemasters.ToDictionary(x => x.CONTNRSID, x => x.CONTNRSDESC);
                
                // Get Cargo Type lookup from stored procedure
                var cargoTypes = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_Bond_Contnr_Recev_Frm").ToList();
                var dictCargoType = cargoTypes.ToDictionary(x => x.dval ?? 0, x => x.dtxt ?? "");

                string Map(string field, string raw)
                {
                    if (raw == null) return raw;
                    var f = (field ?? string.Empty).Trim();
                    var val = raw.Trim();
                    if (string.IsNullOrEmpty(val)) return raw;
                    int ival;
                    switch (f.ToUpperInvariant())
                    {
                        case "PRDTGID":
                            return int.TryParse(val, out ival) && dictPrdtGrp.ContainsKey(ival) ? dictPrdtGrp[ival] : raw;
                        case "PRDTTID":
                            return int.TryParse(val, out ival) && dictPrdtType.ContainsKey(ival) ? dictPrdtType[ival] : raw;
                        case "CONTNRTID":
                            return int.TryParse(val, out ival) && dictContType.ContainsKey(ival) ? dictContType[ival] : raw;
                        case "CONTNRSID":
                            if (int.TryParse(val, out ival))
                            {
                                if (ival == 0) return "Not Required";
                                if (dictContSize.ContainsKey(ival)) return dictContSize[ival];
                            }
                            return raw;
                        case "DISPSTATUS":
                            return val == "1" ? "Disabled" : val == "0" ? "Enabled" : raw;
                        case "CONTNRRCVFRM":
                            if (int.TryParse(val, out ival) && dictCargoType.ContainsKey(ival))
                            {
                                return dictCargoType[ival];
                            }
                            return raw;
                        default:
                            return raw;
                    }
                }

                string Friendly(string field)
                {
                    if (string.IsNullOrWhiteSpace(field)) return field;
                    var f = field.Trim();
                    switch (f.ToUpperInvariant())
                    {
                        case "GIDATE": return "Gate In Date";
                        case "GITIME": return "Gate In Time";
                        case "GINO": return "Gate In No";
                        case "GIDNO": return "Gate In Detail No";
                        case "DRVNAME": return "Driver Name";
                        case "TRNSPRTNAME": return "Transporter Name";
                        case "VHLNO": return "Vehicle No";
                        case "VOYNO": return "Voyage No";
                        case "IGMNO": return "IGM No.";
                        case "GPLNO": return "Line No";
                        case "IMPRTNAME":
                        case "IMPRTID": return "Importer Name";
                        case "STMRNAME":
                        case "STMRID": return "Steamer Name";
                        case "CHANAME": return "CHA Name";
                        case "CONTNRNO": return "Container No";
                        case "CONTNRSID": return "Container Size";
                        case "CONTNRTID": return "Container Type";
                        case "PRDTGID": return "Product Category";
                        case "PRDTDESC": return "Product Description";
                        case "PRDTTID": return "Product Type";
                        case "GPWGHT": return "Weight (MT)";
                        case "GPNOP": return "NOP";
                        case "GPCBM": return "CBM";
                        case "BNDID": return "Bond ID";
                        case "CONTNRRCVFRM": return "Cargo Type";
                        case "CONTNRRCVFRMOTH": return "Received From (Others)";
                        case "GIREMKRS": return "Remarks";
                        case "DISPSTATUS": return "Status";
                        default: return field; // fallback to technical name
                    }
                }

                foreach (var row in list)
                {
                    row.OldValue = Map(row.FieldName, row.OldValue);
                    row.NewValue = Map(row.FieldName, row.NewValue);
                    row.FieldName = Friendly(row.FieldName);
                }
            }
            catch { /* Best-effort mapping; do not fail page if lookups have issues */ }

            return View(list);
        }

        // Compare two versions for a given GIDNO
        public ActionResult EditLogGateInCompare(int? gidid, string versionA, string versionB)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            // Fallbacks: try alternate parameter names that routing might provide
            if (gidid == null)
            {
                int tmp;
                var qsGid = Request["gidid"] ?? Request["id"];
                if (!string.IsNullOrWhiteSpace(qsGid) && int.TryParse(qsGid, out tmp))
                {
                    gidid = tmp;
                }
            }

            if (gidid == null || string.IsNullOrWhiteSpace(versionA) || string.IsNullOrWhiteSpace(versionB))
            {
                TempData["Err"] = "Please provide GIDNO, Version A and Version B to compare.";
                return RedirectToAction("EditLogGateIn", new { gidid = gidid });
            }

            // Normalize version strings (trim all whitespace including tabs) and support baseline shortcuts
            versionA = (versionA ?? string.Empty).Trim().Replace("\t", "").Replace("\r", "").Replace("\n", "");
            versionB = (versionB ?? string.Empty).Trim().Replace("\t", "").Replace("\r", "").Replace("\n", "");
            
            // Map '0' or 'v0'/'V0' to 'v0-<GIDNO>' for baseline comparisons
            // First, get the actual GIDNO string from the BondGateInDetail table
            string gidnoString = gidid.Value.ToString();
            try
            {
                var bondRecord = context.bondgateindtls.AsNoTracking().FirstOrDefault(x => x.GIDID == gidid.Value);
                if (bondRecord != null && !string.IsNullOrEmpty(bondRecord.GIDNO))
                {
                    gidnoString = bondRecord.GIDNO;
                }
            }
            catch { /* fallback to gidid.Value.ToString() */ }
            
            // Also try to get from log table as fallback
            var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
            if (cs != null && !string.IsNullOrWhiteSpace(cs.ConnectionString))
            {
                try
                {
                    using (var sql = new SqlConnection(cs.ConnectionString))
                    {
                        sql.Open();
                        // Get the actual GIDNO string from the first record
                        using (var cmdGetGidno = new SqlCommand(@"SELECT TOP 1 [GIDNO] FROM [dbo].[GateInDetailEditLog] 
                                                                  WHERE [Modules] = 'BondGateIn' AND CAST([GIDNO] AS INT) = @GIDID", sql))
                        {
                            cmdGetGidno.Parameters.AddWithValue("@GIDID", gidid.Value);
                            var obj = cmdGetGidno.ExecuteScalar();
                            if (obj != null && obj != DBNull.Value)
                            {
                                var logGidno = Convert.ToString(obj);
                                if (!string.IsNullOrEmpty(logGidno))
                                {
                                    gidnoString = logGidno;
                                }
                            }
                        }
                    }
                }
                catch { /* use gidid.Value.ToString() as final fallback */ }
            }
            
            if (gidid.HasValue)
            {
                var baseLabel = "v0-" + gidnoString;
                if (string.Equals(versionA, "0", StringComparison.OrdinalIgnoreCase) || 
                    string.Equals(versionA, "V0", StringComparison.OrdinalIgnoreCase) || 
                    string.Equals(versionA, "v0", StringComparison.OrdinalIgnoreCase))
                    versionA = baseLabel;
                if (string.Equals(versionB, "0", StringComparison.OrdinalIgnoreCase) || 
                    string.Equals(versionB, "V0", StringComparison.OrdinalIgnoreCase) || 
                    string.Equals(versionB, "v0", StringComparison.OrdinalIgnoreCase))
                    versionB = baseLabel;
            }

            var rowsA = new List<scfs_erp.Models.GateInDetailEditLogRow>();
            var rowsB = new List<scfs_erp.Models.GateInDetailEditLogRow>();
            if (cs != null && !string.IsNullOrWhiteSpace(cs.ConnectionString))
            {
                using (var sql = new SqlConnection(cs.ConnectionString))
                using (var cmd = new SqlCommand(@"SELECT [GIDNO],[FieldName],[OldValue],[NewValue],[ChangedBy],[ChangedOn],[Version],[Modules]
                                                FROM [dbo].[GateInDetailEditLog]
                                                WHERE [Modules] = 'BondGateIn'
                                                  AND [GIDNO] = @GIDNO
                                                  AND RTRIM(LTRIM([Version])) = @V", sql))
                {
                    cmd.Parameters.Add("@GIDNO", System.Data.SqlDbType.NVarChar, 50);
                    cmd.Parameters.Add("@V", System.Data.SqlDbType.NVarChar, 100);

                    sql.Open();
                    cmd.Parameters["@GIDNO"].Value = gidnoString;
                    cmd.Parameters["@V"].Value = versionA;
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            rowsA.Add(new scfs_erp.Models.GateInDetailEditLogRow
                            {
                                GIDNO = Convert.ToString(r["GIDNO"]),
                                FieldName = Convert.ToString(r["FieldName"]),
                                OldValue = r["OldValue"] == DBNull.Value ? null : Convert.ToString(r["OldValue"]),
                                NewValue = r["NewValue"] == DBNull.Value ? null : Convert.ToString(r["NewValue"]),
                                ChangedBy = Convert.ToString(r["ChangedBy"]),
                                ChangedOn = r["ChangedOn"] != DBNull.Value ? Convert.ToDateTime(r["ChangedOn"]) : DateTime.MinValue,
                                Version = versionA,
                                Modules = r["Modules"] == DBNull.Value ? null : Convert.ToString(r["Modules"])
                            });
                        }
                    }

                    cmd.Parameters["@V"].Value = versionB;
                    using (var r2 = cmd.ExecuteReader())
                    {
                        while (r2.Read())
                        {
                            rowsB.Add(new scfs_erp.Models.GateInDetailEditLogRow
                            {
                                GIDNO = Convert.ToString(r2["GIDNO"]),
                                FieldName = Convert.ToString(r2["FieldName"]),
                                OldValue = r2["OldValue"] == DBNull.Value ? null : Convert.ToString(r2["OldValue"]),
                                NewValue = r2["NewValue"] == DBNull.Value ? null : Convert.ToString(r2["NewValue"]),
                                ChangedBy = Convert.ToString(r2["ChangedBy"]),
                                ChangedOn = r2["ChangedOn"] != DBNull.Value ? Convert.ToDateTime(r2["ChangedOn"]) : DateTime.MinValue,
                                Version = versionB,
                                Modules = r2["Modules"] == DBNull.Value ? null : Convert.ToString(r2["Modules"])
                            });
                        }
                    }
                }
            }

            // Map technical field names to friendly form labels and raw codes to display values
            try
            {
                // Build lookup dictionaries once
                var dictPrdtGrp = context.bondproductgroupmasters.ToDictionary(x => x.PRDTGID, x => x.PRDTGDESC);
                var dictPrdtType = context.producttypemasters.ToDictionary(x => x.PRDTTID, x => x.PRDTTDESC);
                var dictContType = context.containertypemasters.ToDictionary(x => x.CONTNRTID, x => x.CONTNRTDESC);
                var dictContSize = context.containersizemasters.ToDictionary(x => x.CONTNRSID, x => x.CONTNRSDESC);
                
                // Get Cargo Type lookup from stored procedure
                var cargoTypes = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_Bond_Contnr_Recev_Frm").ToList();
                var dictCargoType = cargoTypes.ToDictionary(x => x.dval ?? 0, x => x.dtxt ?? "");

                string Map(string field, string raw)
                {
                    if (string.IsNullOrWhiteSpace(raw)) return raw;
                    int ival;
                    switch (field?.ToUpperInvariant())
                    {
                        case "PRDTGID":
                            return int.TryParse(raw, out ival) && dictPrdtGrp.ContainsKey(ival) ? dictPrdtGrp[ival] : raw;
                        case "PRDTTID":
                            return int.TryParse(raw, out ival) && dictPrdtType.ContainsKey(ival) ? dictPrdtType[ival] : raw;
                        case "CONTNRTID":
                            return int.TryParse(raw, out ival) && dictContType.ContainsKey(ival) ? dictContType[ival] : raw;
                        case "CONTNRSID":
                            if (int.TryParse(raw, out ival))
                            {
                                if (ival == 0) return "Not Required";
                                if (dictContSize.ContainsKey(ival)) return dictContSize[ival];
                            }
                            return raw;
                        case "DISPSTATUS":
                            return raw == "1" ? "Disabled" : raw == "0" ? "Enabled" : raw;
                        case "CONTNRRCVFRM":
                            if (int.TryParse(raw, out ival) && dictCargoType.ContainsKey(ival))
                            {
                                return dictCargoType[ival];
                            }
                            return raw;
                        default:
                            return raw;
                    }
                }

                string Friendly(string field)
                {
                    if (string.IsNullOrWhiteSpace(field)) return field;
                    var f = field.Trim();
                    switch (f.ToUpperInvariant())
                    {
                        case "GIDATE": return "Gate In Date";
                        case "GITIME": return "Gate In Time";
                        case "GINO": return "Gate In No";
                        case "GIDNO": return "Gate In Detail No";
                        case "DRVNAME": return "Driver Name";
                        case "TRNSPRTNAME": return "Transporter Name";
                        case "VHLNO": return "Vehicle No";
                        case "VOYNO": return "Voyage No";
                        case "IGMNO": return "IGM No.";
                        case "GPLNO": return "Line No";
                        case "IMPRTNAME":
                        case "IMPRTID": return "Importer Name";
                        case "STMRNAME":
                        case "STMRID": return "Steamer Name";
                        case "CHANAME": return "CHA Name";
                        case "CONTNRNO": return "Container No";
                        case "CONTNRSID": return "Container Size";
                        case "CONTNRTID": return "Container Type";
                        case "PRDTGID": return "Product Category";
                        case "PRDTDESC": return "Product Description";
                        case "PRDTTID": return "Product Type";
                        case "GPWGHT": return "Weight (MT)";
                        case "GPNOP": return "NOP";
                        case "GPCBM": return "CBM";
                        case "BNDID": return "Bond ID";
                        case "CONTNRRCVFRM": return "Cargo Type";
                        case "CONTNRRCVFRMOTH": return "Received From (Others)";
                        case "GIREMKRS": return "Remarks";
                        case "DISPSTATUS": return "Status";
                        default: return field; // fallback to technical name
                    }
                }

                foreach (var row in rowsA)
                {
                    row.OldValue = Map(row.FieldName, row.OldValue);
                    row.NewValue = Map(row.FieldName, row.NewValue);
                    row.FieldName = Friendly(row.FieldName);
                }
                foreach (var row in rowsB)
                {
                    row.OldValue = Map(row.FieldName, row.OldValue);
                    row.NewValue = Map(row.FieldName, row.NewValue);
                    row.FieldName = Friendly(row.FieldName);
                }
            }
            catch { /* best-effort mapping for compare page */ }

            ViewBag.GIDNO = gidid.Value;
            ViewBag.VersionA = versionA;
            ViewBag.VersionB = versionB;
            ViewBag.RowsA = rowsA;
            ViewBag.RowsB = rowsB;

            return View();
        }

        // ========================= Edit Logging Helper Methods =========================
        private void LogGateInEdits(BondGateInDetail before, BondGateInDetail after, string userId)
        {
            if (before == null || after == null)
            {
                System.Diagnostics.Debug.WriteLine($"LogGateInEdits: before={before != null}, after={after != null}");
                return;
            }
            var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
            if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
            {
                System.Diagnostics.Debug.WriteLine("LogGateInEdits: No SCFSERP_EditLog connection string found");
                return;
            }

            // Exclude system or noisy fields and those you don't want to log
            var exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                // system/housekeeping fields that auto-update
                "GIDID", "PRCSDATE", "LMUSRID", "CUSRID",
                // system-mirrored fields
                "CONTNRID", "COMPYID", "SDPTID", "UNITID",
                // IDs that have corresponding NAME fields
                "TRNSPRTID", "IMPRTID", "STMRID", "CHAID",
                "VSLID", "VSLNAME", "PRDTTID", "BNDID"
                // Note: PRDTGID, CONTNRTID, CONTNRSID are now logged (they don't have corresponding NAME fields that show descriptions)
            };

            // Compute the next version ONCE per save so all rows for this edit share the same Version
            int nextVersion = 1;
            try
            {
                using (var sql = new SqlConnection(cs.ConnectionString))
                using (var cmd = new SqlCommand(@"
                    SELECT ISNULL(
                        MAX(TRY_CAST(
                            SUBSTRING([Version], 2, 
                                CASE WHEN CHARINDEX('-', [Version]) > 0 
                                     THEN CHARINDEX('-', [Version]) - 2 
                                     ELSE LEN([Version]) - 1
                                END
                            ) AS INT)
                        ), 0) + 1
                    FROM [dbo].[GateInDetailEditLog]
                    WHERE [GIDNO] = @GIDNO AND [Modules] = 'BondGateIn'", sql))
                {
                    cmd.Parameters.AddWithValue("@GIDNO", after.GIDNO);
                    sql.Open();
                    var obj = cmd.ExecuteScalar();
                    if (obj != null && obj != DBNull.Value)
                        nextVersion = Convert.ToInt32(obj);
                }
            }
            catch { /* ignore logging version errors */ }

            var props = typeof(BondGateInDetail).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            int fieldsChecked = 0;
            int fieldsChanged = 0;
            foreach (var p in props)
            {
                if (!p.CanRead) continue;
                // Skip complex navigation properties
                if (p.PropertyType.IsClass && p.PropertyType != typeof(string) && !p.PropertyType.IsValueType)
                    continue;
                if (exclude.Contains(p.Name)) continue;
                
                fieldsChecked++;

                var ov = p.GetValue(before, null);
                var nv = p.GetValue(after, null);

                // Compare by underlying type to avoid logging formatting-only differences
                var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                bool changed = false;
                bool shouldLog = true;

                if (type == typeof(decimal) || type == typeof(decimal?))
                {
                    var d1 = ToNullableDecimal(ov);
                    var d2 = ToNullableDecimal(nv);
                    // Only skip if both are null or both are zero
                    if (!d1.HasValue && !d2.HasValue) { shouldLog = false; }
                    else if (d1.HasValue && d2.HasValue && d1.Value == 0m && d2.Value == 0m) { shouldLog = false; }
                    else
                    {
                        var val1 = d1 ?? 0m;
                        var val2 = d2 ?? 0m;
                        changed = val1 != val2;
                    }
                }
                else if (type == typeof(double) || type == typeof(double?) || type == typeof(float) || type == typeof(float?))
                {
                    var d1 = ov == null ? (double?)null : Convert.ToDouble(ov);
                    var d2 = nv == null ? (double?)null : Convert.ToDouble(nv);
                    if (!d1.HasValue && !d2.HasValue) { shouldLog = false; }
                    else if (d1.HasValue && d2.HasValue && Math.Abs(d1.Value) < 1e-9 && Math.Abs(d2.Value) < 1e-9) { shouldLog = false; }
                    else
                    {
                        var val1 = d1 ?? 0.0;
                        var val2 = d2 ?? 0.0;
                        changed = Math.Abs(val1 - val2) > 1e-9;
                    }
                }
                else if (type == typeof(int) || type == typeof(int?) || type == typeof(long) || type == typeof(long?) || type == typeof(short) || type == typeof(short?))
                {
                    var i1 = ov == null ? (long?)null : Convert.ToInt64(ov);
                    var i2 = nv == null ? (long?)null : Convert.ToInt64(nv);
                    // Only skip if both are null
                    if (!i1.HasValue && !i2.HasValue) { shouldLog = false; }
                    else
                    {
                        // Always check for changes, including 0 values (for fields like CONTNRSID that can be "not required")
                        var val1 = i1 ?? 0;
                        var val2 = i2 ?? 0;
                        changed = val1 != val2;
                        // Only skip if both are 0 AND the field is not one that should log 0 changes
                        // For Container Size (CONTNRSID), we want to log even when changing to/from 0
                        if (val1 == 0 && val2 == 0)
                        {
                            // Skip only if both are 0 and unchanged
                            shouldLog = false;
                        }
                    }
                }
                else if (type == typeof(DateTime) || type == typeof(DateTime?))
                {
                    var t1 = (ov as DateTime?) ?? (ov != null ? (DateTime)ov : default(DateTime?));
                    var t2 = (nv as DateTime?) ?? (nv != null ? (DateTime)nv : default(DateTime?));

                    if (!t1.HasValue && !t2.HasValue) { shouldLog = false; }
                    else
                    {
                        var dt1 = t1 ?? default(DateTime);
                        var dt2 = t2 ?? default(DateTime);

                        // For date-only fields, compare only date part
                        if (p.Name.Contains("DATE") && !p.Name.Contains("TIME"))
                        {
                            changed = dt1.Date != dt2.Date;
                        }
                        else
                        {
                            // For datetime fields, ignore millisecond differences
                            var t1Normalized = new DateTime(dt1.Year, dt1.Month, dt1.Day, dt1.Hour, dt1.Minute, dt1.Second);
                            var t2Normalized = new DateTime(dt2.Year, dt2.Month, dt2.Day, dt2.Hour, dt2.Minute, dt2.Second);
                            changed = t1Normalized != t2Normalized;
                        }
                    }
                }
                else if (type == typeof(string))
                {
                    var s1 = (Convert.ToString(ov) ?? string.Empty).Trim();
                    var s2 = (Convert.ToString(nv) ?? string.Empty).Trim();
                    // Only skip if both are truly empty/null (treat empty and null as same)
                    if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2)) { shouldLog = false; }
                    else
                    {
                        changed = !string.Equals(s1, s2, StringComparison.Ordinal);
                    }
                }
                else
                {
                    var s1 = FormatVal(ov);
                    var s2 = FormatVal(nv);
                    if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2)) { shouldLog = false; }
                    else
                    {
                        changed = !string.Equals(s1 ?? "", s2 ?? "", StringComparison.Ordinal);
                    }
                }

                // Skip if both values are null/empty/default AND they're the same
                if (!shouldLog || !changed) continue;

                fieldsChanged++;
                System.Diagnostics.Debug.WriteLine($"Field changed: {p.Name} - Old: {ov ?? (object)"(null)"}, New: {nv ?? (object)"(null)"}");

                // Convert lookup IDs to display values before logging
                // Always format values, even if null, to show the change clearly
                var os = FormatValForLogging(p.Name, ov) ?? "";
                var ns = FormatValForLogging(p.Name, nv) ?? "";

                // Save the ORIGINAL property name (database column name) to maintain consistency
                var versionLabel = $"V{nextVersion}-{after.GIDNO}"; // Version label e.g., V1-04097
                InsertEditLogRow(cs.ConnectionString, after.GIDNO, p.Name, os, ns, userId, versionLabel, "BondGateIn");
            }
            
            System.Diagnostics.Debug.WriteLine($"LogGateInEdits: Checked {fieldsChecked} fields, {fieldsChanged} fields changed");
        }

        private static string FormatVal(object value)
        {
            if (value == null) return null;
            if (value is DateTime dt) return dt.ToString("dd/MM/yyyy HH:mm:ss");
            if (value is DateTime?)
            {
                var ndt = (DateTime?)value;
                return ndt.HasValue ? ndt.Value.ToString("dd/MM/yyyy HH:mm:ss") : null;
            }
            if (value is decimal dec) return dec.ToString("0.####");
            var ndecs = value as decimal?;
            if (ndecs.HasValue) return ndecs.Value.ToString("0.####");
            return Convert.ToString(value);
        }

        // Format value for logging - converts lookup IDs to display values
        private string FormatValForLogging(string fieldName, object value)
        {
            // Handle null values
            if (value == null) return null;
            
            // Format dates with user-friendly format
            if (value is DateTime dt)
            {
                if (fieldName.Contains("DATE") && !fieldName.Contains("TIME"))
                {
                    return dt.ToString("dd/MM/yyyy");
                }
                else if (fieldName.Contains("TIME"))
                {
                    return dt.ToString("HH:mm:ss");
                }
                return dt.ToString("dd/MM/yyyy HH:mm:ss");
            }
            if (value is DateTime?)
            {
                var ndt = (DateTime?)value;
                if (!ndt.HasValue) return null;
                if (fieldName.Contains("DATE") && !fieldName.Contains("TIME"))
                {
                    return ndt.Value.ToString("dd/MM/yyyy");
                }
                else if (fieldName.Contains("TIME"))
                {
                    return ndt.Value.ToString("HH:mm:ss");
                }
                return ndt.Value.ToString("dd/MM/yyyy HH:mm:ss");
            }
            
            // First format the value normally
            var formattedValue = FormatVal(value);
            if (string.IsNullOrEmpty(formattedValue)) return formattedValue;

            // Convert lookup field IDs to their display values
            try
            {
                // Product Group lookup
                if (fieldName.Equals("PRDTGID", StringComparison.OrdinalIgnoreCase))
                {
                    int productGroupId;
                    if (int.TryParse(formattedValue, out productGroupId) && productGroupId > 0)
                    {
                        var productGroup = context.bondproductgroupmasters.FirstOrDefault(p => p.PRDTGID == productGroupId);
                        if (productGroup != null && !string.IsNullOrEmpty(productGroup.PRDTGDESC))
                        {
                            return productGroup.PRDTGDESC;
                        }
                    }
                }
                // Product Type lookup
                else if (fieldName.Equals("PRDTTID", StringComparison.OrdinalIgnoreCase))
                {
                    int productTypeId;
                    if (int.TryParse(formattedValue, out productTypeId) && productTypeId > 0)
                    {
                        var productType = context.producttypemasters.FirstOrDefault(p => p.PRDTTID == productTypeId);
                        if (productType != null && !string.IsNullOrEmpty(productType.PRDTTDESC))
                        {
                            return productType.PRDTTDESC;
                        }
                    }
                }
                // Container Type lookup
                else if (fieldName.Equals("CONTNRTID", StringComparison.OrdinalIgnoreCase))
                {
                    int containerTypeId;
                    if (int.TryParse(formattedValue, out containerTypeId) && containerTypeId > 0)
                    {
                        var containerType = context.containertypemasters.FirstOrDefault(c => c.CONTNRTID == containerTypeId);
                        if (containerType != null && !string.IsNullOrEmpty(containerType.CONTNRTDESC))
                        {
                            return containerType.CONTNRTDESC;
                        }
                    }
                }
                // Container Size lookup
                else if (fieldName.Equals("CONTNRSID", StringComparison.OrdinalIgnoreCase))
                {
                    int containerSizeId;
                    if (int.TryParse(formattedValue, out containerSizeId))
                    {
                        if (containerSizeId == 0)
                        {
                            return "Not Required";
                        }
                        else if (containerSizeId > 0)
                        {
                            var containerSize = context.containersizemasters.FirstOrDefault(c => c.CONTNRSID == containerSizeId);
                            if (containerSize != null && !string.IsNullOrEmpty(containerSize.CONTNRSDESC))
                            {
                                return containerSize.CONTNRSDESC;
                            }
                        }
                    }
                }
                // Cargo Received From lookup - get from stored procedure
                else if (fieldName.Equals("CONTNRRCVFRM", StringComparison.OrdinalIgnoreCase))
                {
                    int cargoTypeId;
                    if (int.TryParse(formattedValue, out cargoTypeId) && cargoTypeId > 0)
                    {
                        try
                        {
                            var cargoTypes = context.Database.SqlQuery<pr_get_BondMaster_Status_Result>("exec pr_Get_Bond_Contnr_Recev_Frm").ToList();
                            var cargoType = cargoTypes.FirstOrDefault(c => c.dval == cargoTypeId);
                            if (cargoType != null && !string.IsNullOrEmpty(cargoType.dtxt))
                            {
                                return cargoType.dtxt;
                            }
                        }
                        catch { /* fallback to raw value */ }
                    }
                }
                // Status lookup
                else if (fieldName.Equals("DISPSTATUS", StringComparison.OrdinalIgnoreCase))
                {
                    if (formattedValue == "1") return "Disabled";
                    if (formattedValue == "0") return "Enabled";
                }
            }
            catch (Exception ex)
            {
                // If lookup fails, return the original formatted value
                System.Diagnostics.Debug.WriteLine($"FormatValForLogging lookup failed for {fieldName}: {ex.Message}");
            }

            return formattedValue;
        }

        private static bool BothNull(object a, object b) => a == null && b == null;

        private static decimal? ToNullableDecimal(object v)
        {
            if (v == null) return null;
            if (v is decimal d) return d;
            var nd = v as decimal?;
            if (nd.HasValue) return nd.Value;
            decimal parsed;
            return decimal.TryParse(Convert.ToString(v), out parsed) ? parsed : (decimal?)null;
        }

        private static void InsertEditLogRow(string connectionString, string gidno, string fieldName, string oldValue, string newValue, string changedBy, string versionLabel, string modules)
        {
            try
            {
                using (var sql = new SqlConnection(connectionString))
                {
                    sql.Open();
                    using (var cmd = new SqlCommand(@"INSERT INTO [dbo].[GateInDetailEditLog]
                        ([GIDNO], [FieldName], [OldValue], [NewValue], [ChangedBy], [ChangedOn], [Version], [Modules])
                        VALUES (@GIDNO, @FieldName, @OldValue, @NewValue, @ChangedBy, GETDATE(), @Version, @Modules)", sql))
                    {
                        cmd.Parameters.AddWithValue("@GIDNO", gidno);
                        cmd.Parameters.AddWithValue("@FieldName", fieldName);
                        cmd.Parameters.AddWithValue("@OldValue", (object)oldValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NewValue", (object)newValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ChangedBy", changedBy ?? "");
                        // Store version label as string (e.g., V1-518084)
                        cmd.Parameters.AddWithValue("@Version", (object)versionLabel ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Modules", modules ?? string.Empty);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to insert edit log: {ex.Message}");
                throw; // Re-throw to be caught by the calling method
            }
        }

        // Ensure a baseline snapshot (Version = "0") exists for the given record.
        // It captures the pre-edit values as NewValue with OldValue set to NULL, one row per field.
        private void EnsureBaselineVersionZero(BondGateInDetail snapshot, string userId)
        {
            try
            {
                var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
                if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString)) return;

                // GIDNO is stored as string in entity; preserve it as-is with leading zeros
                if (string.IsNullOrWhiteSpace(snapshot.GIDNO)) return;

                using (var sql = new SqlConnection(cs.ConnectionString))
                using (var cmd = new SqlCommand("SELECT COUNT(1) FROM [dbo].[GateInDetailEditLog] WHERE [GIDNO]=@GIDNO AND [Modules]='BondGateIn' AND (RTRIM(LTRIM([Version]))=@VLower OR RTRIM(LTRIM([Version]))=@VUpper OR RTRIM(LTRIM([Version]))='0' OR RTRIM(LTRIM([Version]))='V0')", sql))
                {
                    cmd.Parameters.AddWithValue("@GIDNO", snapshot.GIDNO);
                    var baselineVerLower = "v0-" + snapshot.GIDNO;
                    var baselineVerUpper = "V0-" + snapshot.GIDNO;
                    cmd.Parameters.AddWithValue("@VLower", baselineVerLower);
                    cmd.Parameters.AddWithValue("@VUpper", baselineVerUpper);
                    sql.Open();
                    var exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    if (exists) return; // baseline already present
                }

                InsertBaselineSnapshot(snapshot, userId);
            }
            catch (Exception ex)
            {
                // Do not block the main flow if baseline creation fails
                System.Diagnostics.Debug.WriteLine($"EnsureBaselineVersionZero failed: {ex.Message}");
            }
        }

        // Insert one row per relevant field for Version = "0" using the provided snapshot values
        private void InsertBaselineSnapshot(BondGateInDetail snapshot, string userId)
        {
            var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
            if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString)) return;

            if (string.IsNullOrWhiteSpace(snapshot.GIDNO)) return;
            var baselineVer = "v0-" + snapshot.GIDNO;

            // Use the same exclusion rules as differential logging
            var exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "PRCSDATE", "LMUSRID", "CUSRID",
                "CONTNRID", "COMPYID", "SDPTID", "UNITID",
                "TRNSPRTID", "IMPRTID", "STMRID", "CHAID", "VSLID", "CONTNRTID", "CONTNRSID", "PRDTGID", "PRDTTID", "BNDID"
            };

            var props = typeof(BondGateInDetail).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var p in props)
            {
                if (!p.CanRead) continue;
                if (p.PropertyType.IsClass && p.PropertyType != typeof(string) && !p.PropertyType.IsValueType) continue;
                if (exclude.Contains(p.Name)) continue;

                // Skip numeric code fields when a companion NAME string exists
                if (p.Name.EndsWith("ID", StringComparison.OrdinalIgnoreCase))
                {
                    var baseName = p.Name.Substring(0, p.Name.Length - 2);
                    var nameProp = props.FirstOrDefault(q => q.PropertyType == typeof(string) && (
                        q.Name.Equals(baseName, StringComparison.OrdinalIgnoreCase) ||
                        q.Name.Equals(baseName + "NAME", StringComparison.OrdinalIgnoreCase) ||
                        (q.Name.EndsWith("NAME", StringComparison.OrdinalIgnoreCase) && q.Name.StartsWith(baseName, StringComparison.OrdinalIgnoreCase))
                    ));
                    if (nameProp != null) continue;
                }

                var valObj = p.GetValue(snapshot, null);
                var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

                // Skip empty/defaults for strings and zero-equivalents for numbers to reduce noise
                if (type == typeof(string))
                {
                    var s = (Convert.ToString(valObj) ?? string.Empty).Trim();
                    bool isDefault = string.IsNullOrEmpty(s) || s == "-" || s == "0" || s == "0.0" || s == "0.00" || s == "0.000" || s == "0.0000";
                    if (isDefault) continue;
                }
                else if (type == typeof(int) || type == typeof(long) || type == typeof(short))
                {
                    var i = Convert.ToInt64(valObj ?? 0);
                    if (i == 0) continue;
                }
                else if (type == typeof(decimal))
                {
                    var d = ToNullableDecimal(valObj) ?? 0m;
                    if (d == 0m) continue;
                }
                else if (type == typeof(DateTime))
                {
                    var dt = (valObj as DateTime?) ?? default(DateTime);
                    if (dt == default(DateTime)) continue;
                }

                var formattedVal = FormatValForLogging(p.Name, valObj);
                if (string.IsNullOrEmpty(formattedVal)) continue;

                InsertEditLogRow(cs.ConnectionString, snapshot.GIDNO, p.Name, null, formattedVal, userId, baselineVer, "BondGateIn");
            }
        }

    }
}