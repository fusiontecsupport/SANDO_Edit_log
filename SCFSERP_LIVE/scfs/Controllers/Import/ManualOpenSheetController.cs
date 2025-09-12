using scfs.Data;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Configuration;

namespace scfs_erp.Controllers
{
    [SessionExpire]
    public class ManualOpenSheetController : Controller
    {
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        [Authorize(Roles = "ManualOpenSheetIndex")]
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
            return View(context.manualopensheetmasters.Where(x => x.OSMDATE >= sd).Where(x => x.OSMDATE <= ed).ToList());
        }//........end of index grid
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {

            using (var e = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_SearchManualOpenSheetGridAssgn(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]), 0);

                var aaData = data.Select(d => new string[] { d.OSMDATE.Value.ToString("dd/MM/yyyy"), d.OSMDNO, d.OSMNAME, d.OSMVSLNAME, d.OSMIGMNO, d.OSMLNO, d.BOENO, d.BOEDATE.Value.ToString("dd/MM/yyyy"), d.DISPSTATUS, d.OSMID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "ManualOpenSheetEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ManualOpenSheet/Form/" + id);
        }
        //..............form data..............//
        [Authorize(Roles = "ManualOpenSheetCreate")]
        public ActionResult Form(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ManualOpenSheetMD vm = new ManualOpenSheetMD();
            ManualOpenSheetMaster master = new ManualOpenSheetMaster();

            ViewBag.OSMLCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME");
            ViewBag.OSMUNITID = new SelectList(context.unitmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.UNITCODE), "UNITID", "UNITCODE", 2);
            //  ViewBag.OSMLNO = new SelectList(context.gateindetails, "GPLNO", "GPLNO");
            ViewBag.OSMTYPE = new SelectList(context.opensheetviadetails, "OSMTYPE", "OSMTYPEDESC");
            ViewBag.T_PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");
            ViewBag.OSMLNO = new SelectList("");

            ViewBag.OSBCHACATEAID = new SelectList("");

            ViewBag.OSBBCHACATEAID = new SelectList("");
            //------------------------------DOTYPE-------------------------
            List<SelectListItem> selectedDOTYPE = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "No", Value = "0", Selected = false };
            selectedDOTYPE.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "Yes", Value = "1", Selected = true };
            selectedDOTYPE.Add(selectedItem1);
            ViewBag.DOTYPE = selectedDOTYPE;

            //------------------------------SealCut-------------------------
            List<SelectListItem> selectedScut = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "No", Value = "0", Selected = true };
            selectedScut.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Yes", Value = "1", Selected = false };
            selectedScut.Add(selectedItem);
            ViewBag.SCTYPE = selectedScut;

            //------------------------------INSPTYPE-------------------------
            List<SelectListItem> selectedINSPTYPE = new List<SelectListItem>();
            SelectListItem selectedItemINS = new SelectListItem { Text = "No", Value = "0", Selected = false };
            selectedINSPTYPE.Add(selectedItemINS);
            selectedItemINS = new SelectListItem { Text = "Yes", Value = "1", Selected = true };
            selectedINSPTYPE.Add(selectedItemINS);
            ViewBag.INSP_TYPE = selectedINSPTYPE;


            //-------------------------------DISPSTATUS----

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = false };
            selectedDISPSTATUS.Add(selectedItemDSP);
            selectedItemDSP = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = true };
            selectedDISPSTATUS.Add(selectedItemDSP);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;


            //-------------------------------container type----

            List<SelectListItem> selectedcontainertype = new List<SelectListItem>();
            SelectListItem selectedcontainer = new SelectListItem { Text = "FCL", Value = "0", Selected = true };
            selectedcontainertype.Add(selectedcontainer);
            // selectedcontainer = new SelectListItem { Text = "LCL", Value = "1", Selected = false };
            //  selectedcontainertype.Add(selectedcontainer);
            ViewBag.OSMLDTYPE = selectedcontainertype;



            //BILLED TO
            //.........s.Tax...//
            List<SelectListItem> selectedtaxlst1 = new List<SelectListItem>();
            SelectListItem selectedItemtax1 = new SelectListItem { Text = "IMPORTER", Value = "1", Selected = false };
            selectedtaxlst1.Add(selectedItemtax1);
            selectedItemtax1 = new SelectListItem { Text = "CHA", Value = "0", Selected = true };
            selectedtaxlst1.Add(selectedItemtax1);
            ViewBag.BILLEDTO = selectedtaxlst1;



            //-------------opensheetdetails---------------

            if (id != 0)
            {
                master = context.manualopensheetmasters.Find(id);//find selected record

                vm.manualmasterdata = context.manualopensheetmasters.Where(det => det.OSMID == id).ToList();
                vm.manualdetaildata = context.manualopensheetdetails.Where(det => det.OSMID == id).ToList();



                //---------Dropdown lists-------------------
                ViewBag.OSMLCATEID = new SelectList(context.categorymasters.Where(m => m.CATETID == 6).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CATENAME), "CATEID", "CATENAME", master.OSMLCATEID);
                ViewBag.OSMUNITID = new SelectList(context.unitmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.UNITDESC), "UNITID", "UNITCODE", master.OSMUNITID);
                ViewBag.OSMLNO = new SelectList(context.gateindetails, "GPLNO", "GPLNO", master.OSMLNO);
                ViewBag.OSMTYPE = new SelectList(context.opensheetviadetails, "OSMTYPE", "OSMTYPEDESC", master.OSMTYPE);
                int chaid = Convert.ToInt32(vm.manualmasterdata[0].CHAID);
                int chaaid = Convert.ToInt32(vm.manualmasterdata[0].OSBCHACATEAID);
                ViewBag.OSBCHACATEAID = new SelectList(context.categoryaddressdetails.Where(m => m.CATEID == chaid).OrderBy(x => x.CATEATYPEDESC), "CATEAID", "CATEATYPEDESC", chaaid).ToList();
                int bchaid = Convert.ToInt32(vm.manualmasterdata[0].OSBILLREFID);
                int bchaaid = Convert.ToInt32(vm.manualmasterdata[0].OSBBCHACATEAID);
                ViewBag.OSBBCHACATEAID = new SelectList(context.categoryaddressdetails.Where(m => m.CATEID == bchaid).OrderBy(x => x.CATEATYPEDESC), "CATEAID", "CATEATYPEDESC", bchaaid).ToList();


                var sql5 = context.Database.SqlQuery<CategoryMaster>("select * from CategoryMaster where CATEID=" + master.OSBILLREFID).ToList();
                ViewBag.OSBILLREFNAME = sql5[0].CATENAME;
                ViewBag.OSBILLGSTNO = sql5[0].CATEBGSTNO;

                ViewBag.T_PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");
                //--------End of dropdown


                //------------------------------DOTYPE-------------------------
                List<SelectListItem> selectedDOTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt16(master.DOTYPE) == 0)
                {
                    SelectListItem selectedItemDO = new SelectListItem { Text = "No", Value = "0", Selected = true };
                    selectedDOTYPE1.Add(selectedItemDO);
                    selectedItemDO = new SelectListItem { Text = "Yes", Value = "1", Selected = false };
                    selectedDOTYPE1.Add(selectedItemDO);
                    ViewBag.DOTYPE = selectedDOTYPE1;
                }

                //------------------------------SealCut-------------------------
                List<SelectListItem> selectedScut1 = new List<SelectListItem>();
                if (Convert.ToInt16(master.SCTYPE) == 0)
                {
                    SelectListItem selectedItemSC = new SelectListItem { Text = "No", Value = "0", Selected = true };
                    selectedScut1.Add(selectedItemSC);
                    selectedItemSC = new SelectListItem { Text = "Yes", Value = "1", Selected = false };
                    selectedScut1.Add(selectedItemSC);
                    ViewBag.SCTYPE = selectedScut1;
                }


                //------------------------------INSPTYPE-------------------------
                List<SelectListItem> selectedINSPTYPE1 = new List<SelectListItem>();
                if (vm.manualdetaildata != null)
                {
                    if (Convert.ToInt16(vm.manualdetaildata[0].INSPTYPE) == 0)
                    {
                        SelectListItem selectedItemINS1 = new SelectListItem { Text = "No", Value = "0", Selected = true };
                        selectedINSPTYPE1.Add(selectedItemINS1);
                        selectedItemINS1 = new SelectListItem { Text = "Yes", Value = "1", Selected = false };
                        selectedINSPTYPE1.Add(selectedItemINS1);
                        ViewBag.INSP_TYPE = selectedINSPTYPE1;
                    }
                    else
                    {

                        SelectListItem selectedItemINS1 = new SelectListItem { Text = "No", Value = "0", Selected = false };
                        selectedINSPTYPE1.Add(selectedItemINS1);
                        selectedItemINS1 = new SelectListItem { Text = "Yes", Value = "1", Selected = true };
                        selectedINSPTYPE1.Add(selectedItemINS1);
                        ViewBag.INSP_TYPE = selectedINSPTYPE1;

                    }
                }

                //-------------------------------container type----

                List<SelectListItem> selectedcontainertype_ = new List<SelectListItem>();
                if (master.OSMLDTYPE == 1)
                {
                    SelectListItem selectedcontainer_ = new SelectListItem { Text = "FCL", Value = "0", Selected = false };
                    selectedcontainertype_.Add(selectedcontainer_);
                    selectedcontainer_ = new SelectListItem { Text = "LCL", Value = "1", Selected = true };
                    selectedcontainertype_.Add(selectedcontainer_);
                    ViewBag.OSMLDTYPE = selectedcontainertype_;
                }


                //BILLED TO 

                List<SelectListItem> selectedcontainertype01 = new List<SelectListItem>();
                if (master.OSBILLEDTO == 1)
                {
                    SelectListItem selectedcontainer01 = new SelectListItem { Text = "IMPORTER", Value = "1", Selected = true };
                    selectedcontainertype01.Add(selectedcontainer01);
                    selectedcontainer01 = new SelectListItem { Text = "CHA", Value = "0", Selected = false };
                    selectedcontainertype01.Add(selectedcontainer01);
                    ViewBag.BILLEDTO = selectedcontainertype01;
                }


                //----------------To Display GateInDetails-----------------------

                //GateInDetail gdet = context.gateindetails.Find(Convert.ToInt32(vm.detaildata[0].GIDID));
                //if (vm.detaildata[0].GIDID == gdet.GIDID)
                //{
                //    ViewBag.VOYNO = gdet.VOYNO;
                //    ViewBag.IMPRTNAME = gdet.IMPRTNAME;
                //    ViewBag.STMRNAME = gdet.STMRNAME;

                //}//------End


            }//---End Of IF


            return View(vm);
        }//----End of Form




        //------------------------Insert and update data---------------------------
        public void savedataOld(FormCollection F_Form)
        {
            ManualOpenSheetMaster manualopensheetmaster = new ManualOpenSheetMaster();
            ManualOpenSheetDetail manualopensheetdetail = new ManualOpenSheetDetail();

            //-------Getting Primarykey field--------
            Int32 OSMID = Convert.ToInt32(F_Form["manualmasterdata[0].OSMID"]);
            //Int32 BILLEMID = 0;// Convert.ToInt32(F_Form["bmasterdata[0].BILLEMID"]);
            Int32 TMPBOEID = Convert.ToInt32(F_Form["TMPBOEID"]);
            //-----End

            if (OSMID != 0)//Getting Primary id in Edit mode
            {

                manualopensheetmaster = context.manualopensheetmasters.Find(OSMID);
            }

            //--------------------------OpenSheet Master---------//
            manualopensheetmaster.SDPTID = 1;
            manualopensheetmaster.COMPYID = Convert.ToInt32(Session["compyid"]);
            manualopensheetmaster.OSMDATE = Convert.ToDateTime(F_Form["manualmasterdata[0].OSMDATE"]).Date;
            manualopensheetmaster.OSMTIME = Convert.ToDateTime(F_Form["manualmasterdata[0].OSMDATE"]);

            manualopensheetmaster.CHAID = Convert.ToInt32(F_Form["manualmasterdata[0].CHAID"]);
            manualopensheetmaster.OSMNAME = F_Form["manualmasterdata[0].OSMNAME"].ToString();
            manualopensheetmaster.OSMLNAME = F_Form["manualmasterdata[0].OSMLNAME"].ToString();
            manualopensheetmaster.OSMCNAME = F_Form["manualmasterdata[0].OSMCNAME"].ToString();
            manualopensheetmaster.BOENO = Convert.ToString(F_Form["manualmasterdata[0].BOENO"]);
            manualopensheetmaster.BOEDATE = Convert.ToDateTime(F_Form["manualmasterdata[0].BOEDATE"]).Date;
            manualopensheetmaster.DOTYPE = Convert.ToInt16(F_Form["DOTYPE"]);
            manualopensheetmaster.DODATE = Convert.ToDateTime(F_Form["manualmasterdata[0].DODATE"]).Date;
            manualopensheetmaster.DOIDATE = Convert.ToDateTime(F_Form["manualmasterdata[0].DOIDATE"]).Date;
            manualopensheetmaster.DONO = Convert.ToString(F_Form["manualmasterdata[0].DONO"]);
            manualopensheetmaster.SCTYPE = Convert.ToInt16(F_Form["SCTYPE"]);
            manualopensheetmaster.OSMLDTYPE = Convert.ToInt16(F_Form["OSMLDTYPE"]);
            if (manualopensheetmaster.SCTYPE == 1)
            {
                manualopensheetmaster.SCDATE = Convert.ToDateTime(F_Form["manualmasterdata[0].SCDATE"]).Date;
                manualopensheetmaster.SCTIME = Convert.ToDateTime(F_Form["manualmasterdata[0].SCDATE"]);
            }



            manualopensheetmaster.SCDESC = Convert.ToString(F_Form["manualmasterdata[0].SCDESC"]);
            manualopensheetmaster.SCREMRKS = Convert.ToString(F_Form["manualmasterdata[0].SCREMRKS"]);
            manualopensheetmaster.OSMIGMNO = Convert.ToString(F_Form["manualmasterdata[0].OSMIGMNO"]);
            manualopensheetmaster.OSMLNO = Convert.ToString(F_Form["OSMLNO"]);
            manualopensheetmaster.OSMVSLNAME = Convert.ToString(F_Form["manualmasterdata[0].OSMVSLNAME"]);
            manualopensheetmaster.OSMAAMT = Convert.ToDecimal(F_Form["manualmasterdata[0].OSMAAMT"]);
            manualopensheetmaster.OSMBLNO = Convert.ToString(F_Form["manualmasterdata[0].OSMBLNO"]);
            manualopensheetmaster.OSMNOP = Convert.ToDecimal(F_Form["manualmasterdata[0].OSMNOP"]);
            manualopensheetmaster.OSMWGHT = Convert.ToDecimal(F_Form["manualmasterdata[0].OSMWGHT"]);
            manualopensheetmaster.OSMUNITID = Convert.ToInt32(F_Form["OSMUNITID"]);
            manualopensheetmaster.OSMTNOC = 0; // Convert.ToInt32(F_Form["manualmasterdata[0].OSMTNOC"]);
            manualopensheetmaster.OSMFNOC = 0; // Convert.ToInt32(F_Form["manualmasterdata[0].OSMFNOC"]);           
            manualopensheetmaster.OSMLCATEID = Convert.ToInt32(F_Form["OSMLCATEID"]);
            manualopensheetmaster.OSMLCATENAME = Convert.ToString(F_Form["manualmasterdata[0].OSMLCATENAME"]);
            manualopensheetmaster.OSMTYPE = Convert.ToInt16(F_Form["OSMTYPE"]);

            manualopensheetmaster.OSBILLEDTO = Convert.ToInt16(F_Form["BILLEDTO"]);
            manualopensheetmaster.OSBILLREFID = Convert.ToInt32(F_Form["manualmasterdata[0].OSBILLREFID"]);
            manualopensheetmaster.OSBILLREFNAME = Convert.ToString(F_Form["manualmasterdata[0].OSBILLREFNAME"]);

            manualopensheetmaster.OSMDUTYAMT = Convert.ToDecimal(F_Form["manualmasterdata[0].OSMDUTYAMT"]);
            manualopensheetmaster.OSMBLDATE = Convert.ToDateTime(F_Form["manualmasterdata[0].OSMBLDATE"]).Date;
            manualopensheetmaster.OSMIGMDATE = Convert.ToDateTime(F_Form["manualmasterdata[0].OSMIGMDATE"]).Date;

            string OSCATEAID = Convert.ToString(F_Form["OSBCHACATEAID"]);
            if (OSCATEAID != "" || OSCATEAID != null)
            {
                manualopensheetmaster.OSBCHACATEAID = Convert.ToInt32(OSCATEAID);
            }
            else
            {
                manualopensheetmaster.OSBCHACATEAID = 0;
            }

            string OSBBCHACATEAID = Convert.ToString(F_Form["OSBBCHACATEAID"]);
            if (OSBBCHACATEAID != "" || OSBBCHACATEAID != null)
            {
                manualopensheetmaster.OSBBCHACATEAID = Convert.ToInt32(OSBBCHACATEAID);
            }
            else
            {
                manualopensheetmaster.OSBBCHACATEAID = 0;
            }

            string OSBCHASTATEID = Convert.ToString(F_Form["manualmasterdata[0].OSBCHASTATEID"]);
            if (OSBCHASTATEID != "" || OSBCHASTATEID != null)
            {
                manualopensheetmaster.OSBCHASTATEID = Convert.ToInt32(OSBCHASTATEID);
            }
            else
            {
                manualopensheetmaster.OSBCHASTATEID = 0;
            }
            string OSBBCHASTATEID = Convert.ToString(F_Form["manualmasterdata[0].OSBBCHASTATEID"]);
            if (OSBBCHASTATEID != "" || OSBBCHASTATEID != null)
            {
                manualopensheetmaster.OSBBCHASTATEID = Convert.ToInt32(OSBBCHASTATEID);
            }
            else
            {
                manualopensheetmaster.OSBBCHASTATEID = 0;
            }

            manualopensheetmaster.OSBCHACATEAGSTNO = Convert.ToString(F_Form["manualmasterdata[0].OSBCHACATEAGSTNO"]);
            manualopensheetmaster.OSBBCHACATEAGSTNO = Convert.ToString(F_Form["manualmasterdata[0].OSBBCHACATEAGSTNO"]);
            manualopensheetmaster.OSBCHAADDR1 = Convert.ToString(F_Form["manualmasterdata[0].OSBCHAADDR1"]);
            manualopensheetmaster.OSBBCHAADDR1 = Convert.ToString(F_Form["manualmasterdata[0].OSBBCHAADDR1"]);
            manualopensheetmaster.OSBCHAADDR2 = Convert.ToString(F_Form["manualmasterdata[0].OSBCHAADDR2"]);
            manualopensheetmaster.OSBBCHAADDR2 = Convert.ToString(F_Form["manualmasterdata[0].OSBBCHAADDR1"]);
            manualopensheetmaster.OSBCHAADDR3 = Convert.ToString(F_Form["manualmasterdata[0].OSBCHAADDR3"]);
            manualopensheetmaster.OSBBCHAADDR3 = Convert.ToString(F_Form["manualmasterdata[0].OSBBCHAADDR3"]);
            manualopensheetmaster.OSBCHAADDR4 = Convert.ToString(F_Form["manualmasterdata[0].OSBCHAADDR4"]);
            manualopensheetmaster.OSBBCHAADDR4 = Convert.ToString(F_Form["manualmasterdata[0].OSBBCHAADDR4"]);

            if (OSMID == 0)
                manualopensheetmaster.CUSRID = Session["CUSRID"].ToString();

            manualopensheetmaster.LMUSRID = Session["CUSRID"].ToString();
            manualopensheetmaster.DISPSTATUS = 0;
            manualopensheetmaster.PRCSDATE = DateTime.Now;
            //--------End of OpenSheet Master 



            if (OSMID == 0)
            {
                manualopensheetmaster.OSMNO = Convert.ToInt32(Autonumber.autonum("manualopensheetmaster", "OSMNO", "OSMNO<>0 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                int ano = manualopensheetmaster.OSMNO;
                string prfx = string.Format("{0:D5}", ano);
                manualopensheetmaster.OSMDNO = prfx.ToString();
                context.manualopensheetmasters.Add(manualopensheetmaster);
                context.SaveChanges();
            }
            else
            {
                context.Entry(manualopensheetmaster).State = System.Data.Entity.EntityState.Modified;

                context.SaveChanges();
            }


            //--------------------Bill Entry Detail and Open Sheet Detail---------------------------
            string[] OSDID = F_Form.GetValues("OSDID");
            string[] FBILLEDID = F_Form.GetValues("BILLEDID");
            string[] F_GIDID = F_Form.GetValues("F_GIDID");
            string[] LSEALNO = F_Form.GetValues("LSEALNO");
            string[] SSEALNO = F_Form.GetValues("SSEALNO");
            string[] GIDATE = F_Form.GetValues("GIDATE");


            for (int count = 0; count < OSDID.Count(); count++)
            {
                var OSDID_ = Convert.ToInt32(OSDID[count]); var BILLEDID = Convert.ToInt32(FBILLEDID[count]);
                if (OSDID_ != 0) { manualopensheetdetail = context.manualopensheetdetails.Find(OSDID_); }

                //----------OpenSheet Detail -----------------//
                manualopensheetdetail.OSMID = manualopensheetmaster.OSMID;
                manualopensheetdetail.GIDID = Convert.ToInt32(F_GIDID[count]);
                manualopensheetdetail.LSEALNO = LSEALNO[count].ToString();
                manualopensheetdetail.SSEALNO = SSEALNO[count].ToString();
                manualopensheetdetail.GIDATE = Convert.ToDateTime(GIDATE[count]).Date;
                manualopensheetdetail.BILLEDID = 0;
                manualopensheetdetail.INSPTYPE = Convert.ToInt16(F_Form["INSP_TYPE"]);
                //manualopensheetdetail.INSPTYPE = Convert.ToInt16(F_Form["INSPTYPE"]);
                if (BILLEDID == 0)
                    manualopensheetdetail.CUSRID = Session["CUSRID"].ToString();

                manualopensheetdetail.LMUSRID = Session["CUSRID"].ToString(); ;
                manualopensheetdetail.DISPSTATUS = 0;
                manualopensheetdetail.PRCSDATE = DateTime.Now;
                //----------End of OpenSheet Detail
                if (OSDID_ == 0)
                {
                    context.manualopensheetdetails.Add(manualopensheetdetail);
                    context.SaveChanges();
                }
                else
                {
                    context.Entry(manualopensheetdetail).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                //-------------bill entry detail-----------//


            }

            string[] PRDTGID1 = F_Form.GetValues("T_PRDTGID");

            string PRDTGID = Convert.ToString(F_Form["T_PRDTGID"]);

            if (PRDTGID == "" || PRDTGID == null || PRDTGID == "0")
            {

            }
            else
            {
                string upquery = "UPDATE  GATEINDETAIL  SET PRDTGID = " + Convert.ToInt32(PRDTGID) + " WHERE SDPTID = 1 AND IGMNO = '" + Convert.ToString(manualopensheetmaster.OSMIGMNO) + "' AND GPLNO = '" + Convert.ToString(manualopensheetmaster.OSMLNO) + "'";
                context.Database.ExecuteSqlCommand(upquery);
            }

            Response.Redirect("Index");

        }//---End of Savedata

        //------------------------Insert and update data---------------------------
        public void savedata(FormCollection F_Form)
        {
            ManualOpenSheetMaster manualopensheetmaster = new ManualOpenSheetMaster();
            ManualOpenSheetDetail manualopensheetdetail = new ManualOpenSheetDetail();

            string todaydt = Convert.ToString(DateTime.Now);
            string todayd = Convert.ToString(DateTime.Now.Date);

            //-------Getting Primarykey field--------
            Int32 OSMID = Convert.ToInt32(F_Form["manualmasterdata[0].OSMID"]);
            //Int32 BILLEMID = 0;// Convert.ToInt32(F_Form["bmasterdata[0].BILLEMID"]);
           
            //-----End

            if (OSMID != 0)//Getting Primary id in Edit mode
            {

                manualopensheetmaster = context.manualopensheetmasters.Find(OSMID);
            }

            //--------------------------OpenSheet Master---------//
            manualopensheetmaster.SDPTID = 1;
            manualopensheetmaster.COMPYID = Convert.ToInt32(Session["compyid"]);

            string indate = Convert.ToString(F_Form["manualmasterdata[0].OSMDATE"]);
            if (indate != null || indate != "")
            {
                manualopensheetmaster.OSMDATE = Convert.ToDateTime(indate).Date;
            }
            else { manualopensheetmaster.OSMDATE = DateTime.Now.Date; }

            if (manualopensheetmaster.OSMDATE > Convert.ToDateTime(todayd))
            {
                manualopensheetmaster.OSMDATE = Convert.ToDateTime(todayd);
            }

            string intime = Convert.ToString(F_Form["manualmasterdata[0].OSMTIME"]);
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

                        manualopensheetmaster.OSMTIME = Convert.ToDateTime(in_datetime);
                    }
                    else { manualopensheetmaster.OSMTIME = DateTime.Now; }
                }
                else { manualopensheetmaster.OSMTIME = DateTime.Now; }
            }
            else { manualopensheetmaster.OSMTIME = DateTime.Now; }

            if (manualopensheetmaster.OSMTIME > Convert.ToDateTime(todaydt))
            {
                manualopensheetmaster.OSMTIME = Convert.ToDateTime(todaydt);
            }

            //manualopensheetmaster.OSMDATE = Convert.ToDateTime(F_Form["manualmasterdata[0].OSMDATE"]).Date;
            //manualopensheetmaster.OSMTIME = Convert.ToDateTime(F_Form["manualmasterdata[0].OSMTIME"]);

            manualopensheetmaster.CHAID = Convert.ToInt32(F_Form["manualmasterdata[0].CHAID"]);
            manualopensheetmaster.OSMNAME = F_Form["manualmasterdata[0].OSMNAME"].ToString();
            manualopensheetmaster.OSMLNAME = F_Form["manualmasterdata[0].OSMLNAME"].ToString();
            manualopensheetmaster.OSMCNAME = F_Form["manualmasterdata[0].OSMCNAME"].ToString();
            manualopensheetmaster.BOENO = ""; //Convert.ToString(F_Form["manualmasterdata[0].BOENO"]);
            manualopensheetmaster.BOEDATE = DateTime.Now.Date; //Convert.ToDateTime(F_Form["manualmasterdata[0].BOEDATE"]).Date;
            manualopensheetmaster.DOTYPE = 0;// Convert.ToInt16(F_Form["DOTYPE"]);
            manualopensheetmaster.DODATE = DateTime.Now.Date; //Convert.ToDateTime(F_Form["manualmasterdata[0].DODATE"]).Date;
            manualopensheetmaster.DOIDATE = DateTime.Now.Date; // Convert.ToDateTime(F_Form["manualmasterdata[0].DOIDATE"]).Date;
            manualopensheetmaster.DONO = ""; // Convert.ToString(F_Form["manualmasterdata[0].DONO"]);
            manualopensheetmaster.SCTYPE = 0; // Convert.ToInt16(F_Form["SCTYPE"]);
            manualopensheetmaster.OSMLDTYPE = 0; Convert.ToInt16(F_Form["OSMLDTYPE"]);
            if (manualopensheetmaster.SCTYPE == 1)
            {
                manualopensheetmaster.SCDATE = Convert.ToDateTime(F_Form["manualmasterdata[0].SCDATE"]).Date;
                manualopensheetmaster.SCTIME = Convert.ToDateTime(F_Form["manualmasterdata[0].SCDATE"]);
            }
            else
            {
                manualopensheetmaster.SCDATE = DateTime.Now.Date;// Convert.ToDateTime(F_Form["manualmasterdata[0].SCDATE"]).Date;
                manualopensheetmaster.SCTIME = DateTime.Now; // Convert.ToDateTime(F_Form["manualmasterdata[0].SCDATE"]);
            }

            manualopensheetmaster.SCDESC = ""; // Convert.ToString(F_Form["manualmasterdata[0].SCDESC"]);
            manualopensheetmaster.SCREMRKS = Convert.ToString(F_Form["manualmasterdata[0].SCREMRKS"]);
            manualopensheetmaster.OSMIGMNO = Convert.ToString(F_Form["manualmasterdata[0].OSMIGMNO"]);
            manualopensheetmaster.OSMLNO = Convert.ToString(F_Form["OSMLNO"]);
            manualopensheetmaster.OSMVSLNAME = Convert.ToString(F_Form["manualmasterdata[0].OSMVSLNAME"]);
            manualopensheetmaster.OSMAAMT = 0; // Convert.ToDecimal(F_Form["manualmasterdata[0].OSMAAMT"]);
            manualopensheetmaster.OSMBLNO = Convert.ToString(F_Form["manualmasterdata[0].OSMBLNO"]);
            manualopensheetmaster.OSMNOP = 0;  // Convert.ToDecimal(F_Form["manualmasterdata[0].OSMNOP"]);
            manualopensheetmaster.OSMWGHT = 0; // Convert.ToDecimal(F_Form["manualmasterdata[0].OSMWGHT"]);
            manualopensheetmaster.OSMUNITID = 0; // Convert.ToInt32(F_Form["OSMUNITID"]);
            manualopensheetmaster.OSMTNOC = 0;// Convert.ToInt32(F_Form["manualmasterdata[0].OSMTNOC"]);
            manualopensheetmaster.OSMFNOC = 0;// Convert.ToInt32(F_Form["manualmasterdata[0].OSMFNOC"]);           
            manualopensheetmaster.OSMLCATEID = 0; // Convert.ToInt32(F_Form["OSMLCATEID"]);
            manualopensheetmaster.OSMLCATENAME = ""; // Convert.ToString(F_Form["manualmasterdata[0].OSMLCATENAME"]);
            manualopensheetmaster.OSMTYPE = 0; // Convert.ToInt16(F_Form["OSMTYPE"]);

            manualopensheetmaster.OSBILLEDTO = 0;// Convert.ToInt16(F_Form["BILLEDTO"]);
            manualopensheetmaster.OSBILLREFID = Convert.ToInt32(F_Form["manualmasterdata[0].CHAID"]);
            manualopensheetmaster.OSBILLREFNAME = Convert.ToString(F_Form["manualmasterdata[0].OSMNAME"]);

            manualopensheetmaster.OSMDUTYAMT = 0; //Convert.ToDecimal(F_Form["manualmasterdata[0].OSMDUTYAMT"]);
            manualopensheetmaster.OSMBLDATE = Convert.ToDateTime(F_Form["manualmasterdata[0].OSMBLDATE"]).Date;
            manualopensheetmaster.OSMIGMDATE = Convert.ToDateTime(F_Form["manualmasterdata[0].OSMIGMDATE"]).Date;

            string OSCATEAID = Convert.ToString(F_Form["OSBCHACATEAID"]);
            if (OSCATEAID != "" || OSCATEAID != null)
            {
                manualopensheetmaster.OSBCHACATEAID = Convert.ToInt32(OSCATEAID); manualopensheetmaster.OSBBCHACATEAID = Convert.ToInt32(OSCATEAID);
            }
            else
            {
                manualopensheetmaster.OSBCHACATEAID = 0; manualopensheetmaster.OSBBCHACATEAID = 0;
            }
           

            string OSBCHASTATEID = Convert.ToString(F_Form["manualmasterdata[0].OSBCHASTATEID"]);
            if (OSBCHASTATEID != "" || OSBCHASTATEID != null)
            {
                manualopensheetmaster.OSBCHASTATEID = Convert.ToInt32(OSBCHASTATEID);
                manualopensheetmaster.OSBBCHASTATEID = Convert.ToInt32(OSBCHASTATEID);
            }
            else
            {
                manualopensheetmaster.OSBCHASTATEID = 0; manualopensheetmaster.OSBBCHASTATEID = 0;
            }
         

            manualopensheetmaster.OSBCHACATEAGSTNO = Convert.ToString(F_Form["manualmasterdata[0].OSBCHACATEAGSTNO"]);
            manualopensheetmaster.OSBBCHACATEAGSTNO = Convert.ToString(F_Form["manualmasterdata[0].OSBCHACATEAGSTNO"]);
            manualopensheetmaster.OSBCHAADDR1 = Convert.ToString(F_Form["manualmasterdata[0].OSBCHAADDR1"]);
            manualopensheetmaster.OSBBCHAADDR1 = Convert.ToString(F_Form["manualmasterdata[0].OSBCHAADDR1"]);
            manualopensheetmaster.OSBCHAADDR2 = Convert.ToString(F_Form["manualmasterdata[0].OSBCHAADDR2"]);
            manualopensheetmaster.OSBBCHAADDR2 = Convert.ToString(F_Form["manualmasterdata[0].OSBCHAADDR2"]);
            manualopensheetmaster.OSBCHAADDR3 = Convert.ToString(F_Form["manualmasterdata[0].OSBCHAADDR3"]);
            manualopensheetmaster.OSBBCHAADDR3 = Convert.ToString(F_Form["manualmasterdata[0].OSBCHAADDR3"]);
            manualopensheetmaster.OSBCHAADDR4 = Convert.ToString(F_Form["manualmasterdata[0].OSBCHAADDR4"]);
            manualopensheetmaster.OSBBCHAADDR4 = Convert.ToString(F_Form["manualmasterdata[0].OSBCHAADDR4"]);

            if (OSMID == 0)
                manualopensheetmaster.CUSRID = Session["CUSRID"].ToString();

            manualopensheetmaster.LMUSRID = Session["CUSRID"].ToString();
            manualopensheetmaster.DISPSTATUS = 0;
            manualopensheetmaster.PRCSDATE = DateTime.Now;
            //--------End of OpenSheet Master 



            if (OSMID == 0)
            {
                manualopensheetmaster.OSMNO = Convert.ToInt32(Autonumber.autonum("manualopensheetmaster", "OSMNO", "OSMNO<>0 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                int ano = manualopensheetmaster.OSMNO;
                string prfx = string.Format("{0:D5}", ano);
                manualopensheetmaster.OSMDNO = prfx.ToString();
                context.manualopensheetmasters.Add(manualopensheetmaster);
                context.SaveChanges();
            }
            else
            {
                context.Entry(manualopensheetmaster).State = System.Data.Entity.EntityState.Modified;

                context.SaveChanges();
            }


            //--------------------Bill Entry Detail and Open Sheet Detail---------------------------
            string[] OSDID = F_Form.GetValues("OSDID");
            string[] FBILLEDID = F_Form.GetValues("BILLEDID");
            string[] F_GIDID = F_Form.GetValues("F_GIDID");
            string[] LSEALNO = F_Form.GetValues("LSEALNO");
            string[] SSEALNO = F_Form.GetValues("SSEALNO");
            string[] GIDATE = F_Form.GetValues("GIDATE");


            for (int count = 0; count < OSDID.Count(); count++)
            {
                var OSDID_ = Convert.ToInt32(OSDID[count]); var BILLEDID = Convert.ToInt32(FBILLEDID[count]);
                if (OSDID_ != 0) { manualopensheetdetail = context.manualopensheetdetails.Find(OSDID_); }

                //----------OpenSheet Detail -----------------//
                manualopensheetdetail.OSMID = manualopensheetmaster.OSMID;
                manualopensheetdetail.GIDID = Convert.ToInt32(F_GIDID[count]);
                manualopensheetdetail.LSEALNO = LSEALNO[count].ToString();
                manualopensheetdetail.SSEALNO = SSEALNO[count].ToString();
                manualopensheetdetail.GIDATE = Convert.ToDateTime(GIDATE[count]).Date;
                manualopensheetdetail.BILLEDID = 0;
                manualopensheetdetail.INSPTYPE = Convert.ToInt16(F_Form["INSP_TYPE"]);
                //manualopensheetdetail.INSPTYPE = Convert.ToInt16(F_Form["INSPTYPE"]);
                if (BILLEDID == 0)
                    manualopensheetdetail.CUSRID = Session["CUSRID"].ToString();

                manualopensheetdetail.LMUSRID = Session["CUSRID"].ToString(); ;
                manualopensheetdetail.DISPSTATUS = 0;
                manualopensheetdetail.PRCSDATE = DateTime.Now;
                //----------End of OpenSheet Detail
                if (OSDID_ == 0)
                {
                    context.manualopensheetdetails.Add(manualopensheetdetail);
                    context.SaveChanges();
                }
                else
                {
                    context.Entry(manualopensheetdetail).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                //-------------bill entry detail-----------//


            }

            string[] PRDTGID1 = F_Form.GetValues("T_PRDTGID");

            string PRDTGID = Convert.ToString(F_Form["T_PRDTGID"]);

            if (PRDTGID == "" || PRDTGID == null || PRDTGID == "0")
            {

            }
            else
            {
                string upquery = "UPDATE  GATEINDETAIL  SET PRDTGID = " + Convert.ToInt32(PRDTGID) + " WHERE SDPTID = 1 AND IGMNO = '" + Convert.ToString(manualopensheetmaster.OSMIGMNO) + "' AND GPLNO = '" + Convert.ToString(manualopensheetmaster.OSMLNO) + "'";
                context.Database.ExecuteSqlCommand(upquery);
            }

            Response.Redirect("Index");

        }//---End of Savedata




        //------------To Get Respective Line No to IGM No-----
        public JsonResult GetLineNo(string id)
        {
            var group = (from vw in context.view_manualopensheet_cbx_assign_01 where vw.IGMNO == id select new { vw.GPLNO }).Distinct().ToList();
            return new JsonResult() { Data = group, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        //------------------Autocomplete CHA-------------
        //public JsonResult AutoCha(string term)
        //{
        //    var result = (from catem in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
        //                  where catem.CATENAME.ToLower().Contains(term.ToLower())
        //                  select new { catem.CATENAME, catem.CATEID, catem.CATEBGSTNO }).Distinct();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult AutoCha(string term)
        {
            var result = (from catem in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where catem.CATENAME.ToLower().Contains(term.ToLower())
                          select new { catem.CATENAME, catem.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //--------Autocomplete CHA Name
        //public JsonResult NewAutoCha(string term)
        //{

        //    var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
        //                  where r.CATENAME.ToLower().Contains(term.ToLower())
        //                  select new { r.CATENAME, r.CATEID, r.CATEBGSTNO }).Distinct();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult NewAutoCha(string term)
        {

            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //cha and importer

        //public JsonResult NewAutoImporter(string term)
        //{
        //    var result = (from r in context.categorymasters.Where(m => m.CATETID == 1).Where(x => x.DISPSTATUS == 0)
        //                  where r.CATENAME.ToLower().Contains(term.ToLower())
        //                  select new { r.CATENAME, r.CATEID, r.CATEBGSTNO }).Distinct(); //CATEBGSTNO - CATEGSTNO RAJESH TO CHECK
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult NewAutoImporter(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 1).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct(); //CATEBGSTNO - CATEGSTNO RAJESH TO CHECK
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //------------Autocomplete IGM----------
        public JsonResult AutoIGM(string term)
        {
            var result = (from view in context.view_manualopensheet_cbx_assign_01
                          where view.IGMNO.ToLower().Contains(term.ToLower())
                          select new { view.IGMNO }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }




        //----------Open Sheet Details Table Display--------
        public void Detail(string PIGMNO, string PGPLNO)
        {
            using (SCFSERPContext contxt = new SCFSERPContext())
            {

                var query = context.Database.SqlQuery<SP_MANUALOPENSHEET_CONTAINER_FLX_ASSGN_Result>("EXEC SP_MANUALOPENSHEET_CONTAINER_FLX_ASSGN @PIGMNO='" + PIGMNO + "',@PGPLNO='" + PGPLNO + "'").ToList();


                var tabl = " <div class='panel-heading navbar-inverse'  style=color:white>Open Sheet Details</div>" +
                    "<Table id=mytabl class='table table-striped table-bordered bootstrap-datatable'> <thead><tr>" +
                    "<th>S.No</th>" +
                    "<th>Container No</th>" +
                    "<th>Size</th>" +
                    "<th>InDate</th>" +
                    "<th>IGM No</th>" +
                    "<th>Line No </th>" +
                    "<th>Liner Seal No</th>" +
                    "<th>SANCO Seal No</th>" +
                    "<th> Inspection </th> <th> Blocked </th></tr> </thead>";
                var type = ""; var i = 1;var indesc = "";
                foreach (var rslt in query)
                {
                    if (rslt.CSEALNO == null) rslt.CSEALNO = "-";
                    if (rslt.LPSEALNO == null) rslt.LPSEALNO = "-";
                    if (rslt.GBDID == 0) type = "YES"; else type = "NO";
                    if (rslt.INS_TYPE == 0) indesc = "No"; else indesc = "Yes";
                    

                    tabl = tabl + "<tbody><tr><td class=hide><input type=text id=OSDID class=OSDID name=OSDID value='0'>";
                    tabl = tabl + "<input type=text id=BILLEDID value=0 class=BILLEDID name=BILLEDID></td><td class=hide>";
                    tabl = tabl + "<input type=text id=F_GIDID value=" + rslt.GIDID + "  class=F_GIDID name=F_GIDID hidden></td>";
                    tabl = tabl + "<td class=hide><input type=text id=STMRNAME value='" + rslt.STMRNAME + "' class=STMRNAME name=STMRNAME></td>";
                    tabl = tabl + "<td class=hide><input type=text id=IMPRTNAME value='" + rslt.IMPRTNAME + "' class=IMPRTNAME name=IMPRTNAME hidden>";
                    tabl = tabl + "<input type=text id=PRDTGID value='" + rslt.PRDTGID + "' class=PRDTGID name=PRDTGID hidden>";
                    tabl = tabl + "<input type=text id=GIPRDTGCODE value='" + rslt.PRDTGCODE + "' class=GIPRDTGCODE name=GIPRDTGCODE hidden>";
                    tabl = tabl + "<input type=text id=GIPRDTGDESC value='" + rslt.PRDTGDESC + "' class=GIPRDTGDESC name=GIPRDTGDESC hidden></td>";
                    tabl = tabl + "<td class=hide><input type=text id=GIBLNO value='" + rslt.BLNO + "' class=GIBLNO name=GIBLNO hidden>";
                    tabl = tabl + "<input type=text id=GIBOEDATE value='" + rslt.BOEDATE + "' class=GIBOEDATE name=GIBOEDATE hidden>";
                    tabl = tabl + "<input type=text id=GIIGMDATE value='" + rslt.IGMDATE + "' class=GIIGMDATE name=GIIGMDATE hidden></td>";
                    tabl = tabl + "<td class=hide><input type=text id=VOYNO value='" + rslt.VOYNO + "' class=VOYNO name=VOYNO hidden></td>";
                    tabl = tabl + "<td class=hide><input type=text id=VSLNAME value='" + rslt.VSLNAME + "' class=VSLNAME name=VSLNAME hidden></td>";              
                    tabl = tabl + "<td>" + i + "</td><td><input type=text id=CONTNRNO value=" + rslt.CONTNRNO + " class=CONTNRNO name=CONTNRNO readonly></td>";
                    tabl = tabl + "<td><input type=text value=" + rslt.CONTNRSDESC + " id=CONTNRSDESC class=CONTNRSDESC name=CONTNRSDESC style=width:45px readonly></td>";
                    tabl = tabl + "<td><input type=text id=GIDATE value=" + rslt.GIDATE + " class=GIDATE name=GIDATE readonly></td>";
                    tabl = tabl + "<td><input type=text value=" + rslt.IGMNO + " id=IGMNO class=IGMNO name=IGMNO style=width:100px readonly></td>";
                    tabl = tabl + "<td><input type=text id=LineNo value=" + rslt.GPLNO + " class=LineNo name=LineNo style=width:100px readonly></td>";
                    tabl = tabl + "<td><input type=text value='" + rslt.LPSEALNO + "' id=LSEALNO class=LSEALNO name=LSEALNO style=width:100px ></td>";
                    tabl = tabl + "<td><input type=text value='" + rslt.CSEALNO + "' id=SSEALNO  class=SSEALNO name=SSEALNO style=width:100px ></td>";
                    tabl = tabl + "<td><input type=text id=INSPTYPE  class='INSPTYPE' name=INSPTYPE value=" + indesc + " style=width:100px readonly='readonly'></td>";                    
                    tabl = tabl + "<td><input type=text id=BLOCK  class=BLOCK name=BLOCK style=width:100px readonly value='" + type + "'></td></tr></tbody>";
                    i++;
                }
                tabl = tabl + "</Table>";
                Response.Write(tabl);
            }


        }


        ////--------------------Duplicate Check for BOENO----------
        //public void BOEDetail()
        //{
        //    string BoeNo = Request.Form.Get("BoeNo");
        //    using (var contxt = new SCFSERPContext())
        //    {
        //        var query = contxt.Database.SqlQuery<string>("select BILLEMDNO from BILLENTRYMASTER where  BILLEMDNO='" + BoeNo + "' ").ToList();

        //        var BillNo = query.Count();
        //        if (BillNo != 0)
        //        {
        //            Response.Write("Bill of Entry No. already exists");
        //        }

        //    }

        //}//end
        public void GetBOEID(string id)
        {
            var query = context.Database.SqlQuery<int>("select BILLEMID from BILLENTRYMASTER where  BILLEMDNO='" + id + "' ").ToList();

            var BillNo = query.Count();
            if (BillNo != 0)
            {
                Response.Write(query[0]);
            }
            else
            {
                Response.Write("0");
            }
        }//end
        public void BOECheck(string id)
        {
            var param = id.Split('~');
            var boedate = param[3];
            var edate = boedate.Split('-');
            var adate = edate[2] + "-" + edate[1] + "-" + edate[0];
            //var query = context.Database.SqlQuery<ManualOpenSheetMaster>("select * from ManualOpenSheetMaster where  BOENO='" + param[0] + "' ").ToList();
            var query = context.Database.SqlQuery<ManualOpenSheetMaster>("select * from ManualOpenSheetMaster where  BOENO='" + param[0] + "' And BOEDATE = '" + adate + "'").ToList();
            if (query.Count > 0)
            {
                var qry = context.Database.SqlQuery<ManualOpenSheetMaster>("select * from ManualOpenSheetMaster where  OSMIGMNO='" + param[1] + "' and OSMLNO='" + param[2] + "' ").ToList();
                if (qry.Count > 0) { Response.Write("PROCEED"); }
                else if (qry.Count == 0) { Response.Write("IGMNO and LineNo Does Not Match With Existing BOENO...!"); }
            }
            else
            {
                Response.Write("PROCEED");
            }

        }


        //----------Detail Table in Edit Mode----------------
        public void DetailEdit(int id)
        {
            using (SCFSERPContext contxt = new SCFSERPContext())
            {
                //var detail = (from open in contxt.manualopensheetdetails
                //              join boe in contxt.billentrydetails on open.BILLEDID equals boe.BILLEDID
                //              join gate in contxt.gateindetails on open.GIDID equals gate.GIDID
                //              //join  blck in contxt.gateinblockdetails on gate.GIDID equals blck.GIDID
                //              where gate.IGMNO == Igm && gate.GPLNO == Line
                //              select new { boe.BILLEDID,boe.BILLEMID,open.OSDID, open.OSMID, open.GIDID, open.GIDATE, open.LSEALNO, open.SSEALNO, gate.IMPRTNAME, gate.STMRNAME, gate.VSLNAME, gate.CONTNRNO, gate.CONTNRSID, gate.IGMNO, gate.GPLNO,gate.VOYNO }).Distinct();
                var detail = context.Database.SqlQuery<VW_MANUALOPENSHEET_DETAIL_CTRL_ASSGN>("select * from VW_MANUALOPENSHEET_DETAIL_CTRL_ASSGN where  OSMID=" + id).ToList();
                var tabl = " <div class='panel-heading navbar-inverse'  style=color:white>Open Sheet Details</div>" +
                    "<Table id=mytabl class='table table-striped table-bordered bootstrap-datatable'> <thead><tr><th>S.No</th>" +
                    "<th>Container No</th>" +
                    "<th>Size</th>" +
                    "<th>InDate</th>" +
                    "<th>IGM No</th>" +
                    "<th>Line No</th>" +
                    "<th>Liner Seal No</th>" +
                    "<th>SANCO Seal No</th>" +
                    "<th>Inspection</th> <th> Blocked </th></tr> </thead>";
                var sseal = "-"; var lseal = "-"; var i = 1; 
                foreach (var result in detail)
                {
                    if (result.SSEALNO != null) sseal = result.SSEALNO;
                    else sseal = "-";
                    if (result.LSEALNO != null) lseal = result.LSEALNO;
                    else lseal = "-";

                    
                    tabl = tabl + "<tbody><tr>";
                    tabl = tabl + "<td class=hide><input type=text id=OSDID value=" + result.OSDID + " class=OSDID name=OSDID>";
                    tabl = tabl + "<input type=text id=BILLEDID value=" + result.BILLEDID + " class=BILLEDID name=BILLEDID>";
                    tabl = tabl + "<input type=text id=FBILLEMID value=" + result.BILLEMID + " class=FBILLEMID name=FBILLEMID></td>";
                    tabl = tabl + "<td class=hide><input type=text id=F_GIDID value=" + result.GIDID + "  class=F_GIDID name=F_GIDID hidden></td>";
                    tabl = tabl + "<td class=hide><input type=text id=STMRNAME value='" + result.STMRNAME + "' class=STMRNAME name=STMRNAME></td>";
                    tabl = tabl + "<td class=hide><input type=text id=IMPRTNAME value='" + result.IMPRTNAME + "' class=IMPRTNAME name=IMPRTNAME hidden>";
                    tabl = tabl + "<input type=text id=PRDTGDESC value='" + result.PRDTGDESC + "' class=PRDTGDESC name=PRDTGDESC hidden>";
                    tabl = tabl + "<input type=text id=PRDTGID value='" + result.PRDTGID + "' class=PRDTGID name=PRDTGID hidden>";
                    tabl = tabl + "<input type=text id=PRDTGCODE value='" + result.PRDTGCODE + "' class=PRDTGCODE name=PRDTGCODE hidden></td>";
                    tabl = tabl + "<td class=hide><input type=text id=VOYNO value='" + result.VOYNO + "' class=VOYNO name=VOYNO hidden></td>";
                    tabl = tabl + "<td class=hide><input type=text id=VSLNAME value='" + result.VSLNAME + "' class=VSLNAME name=VSLNAME hidden></td>";
                    tabl = tabl + "<td>" + i + "</td><td><input type=text id=CONTNRNO value=" + result.CONTNRNO + " class=CONTNRNO name=CONTNRNO readonly></td>";
                    tabl = tabl + "<td><input type=text value=" + result.CONTNRSID + " id=CONTNRSDESC class=CONTNRSDESC name=CONTNRSDESC style=width:45px readonly></td>";
                    tabl = tabl + "<td><input type=text id=GIDATE value=" + result.GIDATE + " class=GIDATE name=GIDATE readonly></td>";
                    tabl = tabl + "<td><input type=text value=" + result.OSMIGMNO + " id=IGMNO class=IGMNO name=IGMNO style=width:100px readonly></td>";
                    tabl = tabl + "<td><input type=text id=LineNo value=" + result.OSMLNO + " class=LineNo name=LineNo style=width:100px readonly></td>";
                    tabl = tabl + "<td><input type=text value='" + lseal + "' id=LSEALNO class=LSEALNO name=LSEALNO style=width:100px></td>";
                    tabl = tabl + "<td><input type=text id=SSEALNO value='" + sseal + "' class=SSEALNO name=SSEALNO style=width:100px></td>";
                    tabl = tabl + "<td><input type=text id=INSPTYPE  class='INSPTYPE' name=INSPTYPE value=" + result.INSP_DES + " style=width:100px readonly='readonly'></td>";
                    tabl = tabl + "<td><input type=text id=BLOCK  class=BLOCK name=BLOCK style=width:100px readonly value='" + result.Block + "'></td></tr></tbody>";
                    i++;
                }
                tabl = tabl + "</Table>";
                Response.Write(tabl);

            }


        }


        [Authorize(Roles = "ManualOpenSheetPrint")]
        public void PrintView(int? id = 0)
        {
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");

            var sealno = "";

            var query = context.Database.SqlQuery<OpenSheetSealDetails>("select * from OpenSheet_Seal_Detail where OSMID=" + id).ToList();
            if (query.Count > 0)
            {

                foreach (var val in query)
                {
                    sealno = sealno + "," + "'" + val.OSSDESC + "'";
                }
            }
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "OPENSHT_GEN", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;


                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Import_Manual_Opensheet_General.rpt");
                cryRpt.RecordSelectionFormula = "{VW_MANUAL_OSHEET_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' and {VW_MANUAL_OSHEET_CRY_PRINT_ASSGN.OSMID} =" + id;

                string paramName = "@FSEALNO";

                for (int i = 0; i < cryRpt.DataDefinition.FormulaFields.Count; i++)
                    if (cryRpt.DataDefinition.FormulaFields[i].FormulaName == "{" + paramName + "}")
                        cryRpt.DataDefinition.FormulaFields[i].Text = "'" + sealno + "'";
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
        [Authorize(Roles = "ManualOpenSheetPrint")]
        public void EPrintView(int? id = 0)/*EIR*/
        {
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "OPENSHTEIR", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;


                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Import_Manual_Opensheet_EIR.rpt");
                cryRpt.RecordSelectionFormula = "{VW_MANUAL_OSHEET_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' and {VW_MANUAL_OSHEET_CRY_PRINT_ASSGN.OSMID} =" + id;

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
        //-----------------Delete Row-----------------
        [Authorize(Roles = "ManualOpenSheetDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);


            if (temp.Equals("PROCEED"))
            {
                ManualOpenSheetMaster manualopensheetmasters = context.manualopensheetmasters.Find(Convert.ToInt32(id));
                context.manualopensheetmasters.Remove(manualopensheetmasters);

                context.SaveChanges();
                Response.Write("Deleted successfully...");
            }
            else
                Response.Write(temp);

        }//---End of Delete
    }//---End of Class
}//--End of namespace