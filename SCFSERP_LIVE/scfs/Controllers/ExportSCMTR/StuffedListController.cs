
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using scfs_erp;
using scfs_erp.Context;
using scfs.Data;
using scfs_erp.Helper;
using scfs_erp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static scfs_erp.Models.Export_SCMTR_Stuffing;


namespace scfs_erp.Controllers.ExportSCMTR
{
    [SessionExpire]
    public class StuffedListController : Controller
    {
        // GET: StuffedList

        #region Context Declaration
        SCFSERPContext context = new SCFSERPContext();

        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

        #endregion

        #region Index
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

        #region GetAjaxData
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_SCMTR_Stuffed_List(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                var aaData = data.Select(d => new string[] { d.STFDSBNO.ToString(), d.STFDSBDATE.Value.ToString("dd/MM/yyyy"), d.STFMDNO.ToString(), d.STFMDATE.Value.ToString("dd/MM/yyyy"), d.STFBILLREFNAME.ToString(), d.CONTNRNO.ToString(), d.CONTNRSDESC.ToString(), d.STFDNOP.ToString(), d.DISPSTATUS.ToString(), d.STFMID.ToString(), d.STFDID.ToString(), d.STFXID.ToString() }).ToArray();
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

        #region SForm
        //[Authorize(Roles = "SRFDetailEdit")]
        public ActionResult SForm(string id)/*BATCH*/
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }



            //StuffingDetail dtab = new StuffingDetail();
            Stuffing_Json_Detail tab = new Stuffing_Json_Detail();
            StuffingList vm = new StuffingList();

            int stfmid = 0; int stfdid = 0; int stfxid = 0;
            if (id.Contains("-"))
            {
                var param = id.Split('-');
                stfdid = Convert.ToInt32(param[0]);
                stfxid = Convert.ToInt32(param[1]);
            }

            ViewBag.id = stfxid;
            ViewBag.STFDID = stfdid;

            ViewBag.STF_MSG_TID = new SelectList(context.stuffing_message_types.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.STF_MSG_TID), "STF_MSG_TID", "STF_MSG_TDESC", 1);
            ViewBag.PRID = new SelectList(context.portofreportings.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRID), "PRID", "PRDESC", 1);
            ViewBag.REID = new SelectList(context.reportingevents.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.REID), "REID", "REDESC", 1);
            ViewBag.RPTYPEID = new SelectList(context.reportingpartytypes.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.RPTYPEID), "RPTYPEID", "RPTYPEDESC", 1);
            ViewBag.EQTID = new SelectList(context.equipment_type_masters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.EQTID), "EQTID", "EQTDESC", 4);
            ViewBag.ELSID = new SelectList(context.equipment_load_statuss.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ELSID), "ELSID", "ELSDESC", 1);
            ViewBag.ESTYPEID = new SelectList(context.equipment_seal_types.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ESTYPEID), "ESTYPEID", "ESTYPEDESC", 2);
            ViewBag.ESID = new SelectList(context.equipment_status.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ESID), "ESID", "ESDESC", 15);
            ViewBag.EUQCID = new SelectList(context.equipment_UQCs.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.EUQCID), "EUQCID", "EUQCDESC", 1);
            ViewBag.FDID = new SelectList(context.final_destination_codes.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.FDID), "FDID", "FDDESC", 1);

            var query = context.Database.SqlQuery<CompanyMaster>("Select * From CompanyMaster Where COMPID = 1").ToList();
            if (query.Count > 0)
            {
                ViewBag.STF_RPTYPE_CODE = query[0].COMP_CUSTODIAN_CODE;
                ViewBag.STF_RLOCT_NAME = query[0].COMP_RPT_LOCT_NAME;
                ViewBag.STF_RLOCT_CODE = query[0].COMP_RPT_LOCT_CODE;
                ViewBag.STF_AP_PANNO = query[0].COMP_AUTH_PAN_NO;
            }

            vm.stuffeddetails = context.Database.SqlQuery<z_pr_XML_Export_Stuffed_Detail_Assgn_Result>("z_pr_XML_Export_Stuffed_Detail_Assgn @PSTFDID=" + stfdid).ToList();
            vm.sbilldetails = context.Database.SqlQuery<z_pr_XML_Stuffed_SBill_No_Detail_Assgn_Result>("z_pr_XML_Stuffed_SBill_No_Detail_Assgn @PSTFDID=" + stfdid).ToList();

            ViewBag.STF_EQPKG_DESC = "P";

            if (stfxid != 0)
            {
                tab = context.stuffing_json_details.Find(stfxid);//-----------find id

                //MESSAGE TYPE
                ViewBag.STF_MSG_TID = new SelectList(context.stuffing_message_types.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.STF_MSG_TID), "STF_MSG_TID", "STF_MSG_TDESC", tab.STF_MSG_TID);
                ViewBag.PRID = new SelectList(context.portofreportings.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRID), "PRID", "PRDESC", tab.PRID);
                ViewBag.REID = new SelectList(context.reportingevents.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.REID), "REID", "REDESC", tab.REID);
                ViewBag.RPTYPEID = new SelectList(context.reportingpartytypes.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.RPTYPEID), "RPTYPEID", "RPTYPEDESC", tab.RPTYPEID);
                ViewBag.EQTID = new SelectList(context.equipment_type_masters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.EQTID), "EQTID", "EQTDESC", tab.EQTID);
                ViewBag.ELSID = new SelectList(context.equipment_load_statuss.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ELSID), "ELSID", "ELSDESC", tab.ELSID);
                ViewBag.ESTYPEID = new SelectList(context.equipment_seal_types.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ESTYPEID), "ESTYPEID", "ESTYPEDESC", tab.ESTYPEID);
                ViewBag.ESID = new SelectList(context.equipment_status.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ESID), "ESID", "ESDESC", tab.ESID);
                ViewBag.EUQCID = new SelectList(context.equipment_UQCs.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.EUQCID), "EUQCID", "EUQCDESC", tab.EUQCID);
                ViewBag.FDID = new SelectList(context.final_destination_codes.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.FDID), "FDID", "FDDESC", tab.FDID);

                ViewBag.STFXNO = tab.STFXNO;
                ViewBag.STFXDNO = tab.STFXDNO;

                ViewBag.STF_RPTYPE_CODE = tab.STF_RPTYPE_CODE;
                ViewBag.STF_RLOCT_NAME = tab.STF_RLOCT_NAME;
                ViewBag.STF_RLOCT_CODE = tab.STF_RLOCT_CODE;
                ViewBag.STF_AP_PANNO = tab.STF_AP_PANNO;
                //ViewBag.STF_FDSTNCODE = tab.STF_FDSTNCODE;
                ViewBag.STF_EQSEALNO = tab.STF_EQSEALNO;
                ViewBag.STF_OTHR_EQID = tab.STF_OTHR_EQID;
                ViewBag.STF_EQPKG_DESC = tab.STF_EQPKG_DESC;
                ViewBag.STF_EQPKG_QTY = tab.STF_EQPKG_QTY;

                //vm.jsondetaildata = context.stuffing_json_details.Where(det => det.STFXID == stfxid).ToList();
            }

            return View(vm);
        }
        #endregion

        #region S_Savedata
        [Authorize]
        public ActionResult S_Savedata(FormCollection sfrm)//......batch save
        {
            int STFDID = 0;
            int STFXID = 0;
            int STFPID = 0;

            using (SCFSERPContext context = new SCFSERPContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        Stuffing_Json_Detail jsondetail = new Stuffing_Json_Detail();

                        STFPID = 0;
                        STFDID = Convert.ToInt32(sfrm["STFDID"]);
                        STFXID = Convert.ToInt32(sfrm["STFXID"]);
                        string[] STFXNO = sfrm.GetValues("STFXNO");
                        string[] STFXDNO = sfrm.GetValues("STFXDNO");

                        string[] STF_MSG_TID = sfrm.GetValues("STF_MSG_TID");
                        string[] PRID = sfrm.GetValues("PRID");
                        string[] REID = sfrm.GetValues("REID");
                        string[] RPTYPEID = sfrm.GetValues("RPTYPEID");
                        string[] STF_RPTYPE_CODE = sfrm.GetValues("STF_RPTYPE_CODE");
                        string[] STF_RLOCT_NAME = sfrm.GetValues("STF_RLOCT_NAME");
                        string[] STF_RLOCT_CODE = sfrm.GetValues("STF_RLOCT_CODE");
                        string[] STF_AP_PANNO = sfrm.GetValues("STF_AP_PANNO");

                        string[] STF_CONTNRNO = sfrm.GetValues("STF_CONTNRNO");
                        string[] EQTID = sfrm.GetValues("EQTID");
                        string[] STF_CONTNRSDESC = sfrm.GetValues("STF_CONTNRSDESC");
                        string[] ELSID = sfrm.GetValues("ELSID");
                        string[] STF_AEQUIPDESC = sfrm.GetValues("STF_AEQUIPDESC");
                        string[] FDID = sfrm.GetValues("FDID");
                        string[] EVENTDATE = sfrm.GetValues("EVENTDATE");
                        string[] ESTYPEID = sfrm.GetValues("ESTYPEID");

                        string[] STF_EQSEALNO = sfrm.GetValues("STF_EQSEALNO");
                        string[] STF_OTHR_EQID = sfrm.GetValues("STF_OTHR_EQID");//sfrm.GetValues("statusdetaildata[0].SRFD_TAPE_TEST");
                        string[] ESID = sfrm.GetValues("ESID");//sfrm.GetValues("statusdetaildata[0].SRFD_RUB_TEST");
                        string[] STF_EQPKG_DESC = sfrm.GetValues("STF_EQPKG_DESC");
                        string[] STF_EQPKG_QTY = sfrm.GetValues("STF_EQPKG_QTY");
                        string[] EUQCID = sfrm.GetValues("EUQCID");//sfrm.GetValues("statusdetaildata[0].SRFDTYPE");

                        for (int row = 0; row < STF_CONTNRNO.Count(); row++)
                        {
                            var bools = "true";// booltype[row];
                            if (bools == "true")
                            {

                                if (STFXID != 0)//Getting Primary id in Edit mode
                                {
                                    jsondetail = context.stuffing_json_details.Find(STFXID);
                                }
                                jsondetail.STFDID = Convert.ToInt32(STFDID);
                                jsondetail.STF_MSG_TID = Convert.ToInt32(STF_MSG_TID[row]);
                                jsondetail.PRID = Convert.ToInt32(PRID[row]);
                                jsondetail.REID = Convert.ToInt32(REID[row]);
                                jsondetail.RPTYPEID = Convert.ToInt32(RPTYPEID[row]);

                                jsondetail.STF_RPTYPE_CODE = STF_RPTYPE_CODE[row].ToString();
                                jsondetail.STF_RLOCT_NAME = STF_RLOCT_NAME[row].ToString();
                                jsondetail.STF_RLOCT_CODE = STF_RLOCT_CODE[row].ToString();
                                jsondetail.STF_AP_PANNO = STF_AP_PANNO[row].ToString();
                                jsondetail.STF_CONTNRNO = STF_CONTNRNO[row].ToString();

                                jsondetail.EQTID = Convert.ToInt32(EQTID[row]);
                                jsondetail.STF_CONTNRSDESC = STF_CONTNRSDESC[row].ToString();
                                jsondetail.ELSID = Convert.ToInt32(ELSID[row]);

                                jsondetail.STF_AEQUIPDESC = STF_AEQUIPDESC[row].ToString();
                                //jsondetail.STF_FDSTNCODE = "Nil";// STF_FDSTNCODE[row].ToString();
                                jsondetail.FDID = Convert.ToInt32(FDID[row]);
                                jsondetail.STF_EVENTDATE = Convert.ToDateTime(EVENTDATE[row]);

                                jsondetail.ESTYPEID = Convert.ToInt32(ESTYPEID[row]);
                                jsondetail.STF_EQSEALNO = STF_EQSEALNO[row].ToString();
                                jsondetail.STF_OTHR_EQID = STF_OTHR_EQID[row].ToString();
                                jsondetail.ESID = Convert.ToInt32(ESID[row]);

                                jsondetail.STF_EQPKG_DESC = STF_EQPKG_DESC[row].ToString();
                                jsondetail.STF_EQPKG_QTY = Convert.ToDecimal(STF_EQPKG_QTY[row]);
                                jsondetail.EUQCID = Convert.ToInt32(EUQCID[row]);
                                jsondetail.CREATEDDATETIME = DateTime.Now;


                                if (Convert.ToInt32(STFXID) == 0)
                                {
                                    jsondetail.STFXNO = Convert.ToInt32(Autonumber.autonum("STUFFING_JSON_DETAIL", "STFXNO", "STFDID > 0").ToString());
                                    int ano = jsondetail.STFXNO;
                                    string prfx = string.Format("{0:D5}", ano);
                                    jsondetail.STFXDNO = prfx.ToString();

                                    context.stuffing_json_details.Add(jsondetail);
                                    context.SaveChanges();
                                    STFXID = jsondetail.STFXID;
                                }
                                else
                                {
                                    context.Entry(jsondetail).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                }

                            }
                            trans.Commit();
                            if (STFXID > 0)
                            {
                                string[] A_STFPID = sfrm.GetValues("STFPID");
                                string[] STF_PACKETS_FROM = sfrm.GetValues("STF_PACKETS_FROM");
                                string[] STF_PACKETS_TO = sfrm.GetValues("STF_PACKETS_TO");
                                for (int count = 0; count < A_STFPID.Count(); count++)
                                {
                                    STFPID = Convert.ToInt32(A_STFPID[count]);
                                    int PKTFROM = Convert.ToInt32(STF_PACKETS_FROM[count]);
                                    int PKTTO = Convert.ToInt32(STF_PACKETS_TO[count]);
                                    if (STFPID > 0)
                                    {
                                        context.Database.ExecuteSqlCommand("Exec z_XML_JSON_Packets_From_To_Details_Update @PSTFPID = " + STFPID + ", @PSTFPFrom = " + PKTFROM + ", @PSTFPTo = " + PKTTO);
                                    }
                                }

                                Export_Json_Creation(STFXID);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        trans.Rollback(); Response.Redirect("/Error/SavepointErr");
                    }
                }
            }

            return Json("saved");
        }

        #endregion

        #region Export_Json_Creation
        public ActionResult Export_Json_Creation(int id = 0)/*10rs.reminder*/
        {

            SqlDataReader reader = null;
            SqlDataReader Sreader = null;
            string _connStr = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(_connStr);

            int stfxid = id;// Convert.ToInt32(Request.Form.Get("id"));// Convert.ToInt32(ids);
            int stfdid = 0;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "Select * from z_EXport_SCMTR_Json_File_Detail_Assgn Where STFXID = " + stfxid;
            sqlCmd.Connection = myConnection;
            myConnection.Open();
            reader = sqlCmd.ExecuteReader();

            string stringjson = "";
            string jsonfilename = "";
            string eventDate = "";

            string monthdesc = DateTime.Now.ToString("MMM");

            while (reader.Read())
            {
                stfdid = Convert.ToInt32(reader["STFDID"]);
                //string cdate = DateTime.Now.ToString("yyyyMMdd");
                //string ctime = "T" + DateTime.Now.ToString("hh:mm");

                string cdate = Convert.ToDateTime(reader["CREATEDDATETIME"]).ToString("yyyyMMdd");
                string ctime = "T" + Convert.ToDateTime(reader["CREATEDDATETIME"]).ToString("hh:mm");

                eventDate = Convert.ToDateTime(reader["STF_EVENTDATE"]).ToString("yyyyMMddThh:mm");

                jsonfilename = reader["STF_MSG_TCODE"].ToString() + "_" + reader["messageID"].ToString()
                                    + "_" + reader["RECODE"].ToString() + "_" + reader["senderID"].ToString()
                                    + "_" + reader["sequenceOrControlNumber"].ToString() + "_" + cdate + "_" + monthdesc + ".json";

                var response = new Response()
                {
                    //Version = "1.1",

                    headerField = new SCMTR_Header()
                    {
                        indicator = reader["indicator"].ToString(),
                        messageID = reader["messageID"].ToString(),
                        sequenceOrControlNumber = Convert.ToInt32(reader["sequenceOrControlNumber"]),
                        date = cdate,//"20210709",
                        time = ctime,//"T11:21",
                        reportingEvent = reader["RECODE"].ToString(),
                        senderID = reader["senderID"].ToString(),
                        receiverID = reader["receiverID"].ToString(),
                        versionNo = reader["versionNo"].ToString(),
                    },

                    master = new SCMTR_MASTERS()
                    {

                        //cargoDetails = GetCargoDetailList(stfxid, stfdid, reader["STF_MSG_TCODE"].ToString(), reader["PRCODE"].ToString(),
                        //                   reader["ELSCODE"].ToString(), reader["EUQCCODE"].ToString()),

                        //cargoContainer = GetCargoContainerList(stfxid),

                        declaration = new SCMTR_Declaration()
                        {
                            messageType = reader["STF_MSG_TCODE"].ToString(),
                            portOfReporting = reader["PRCODE"].ToString(),
                            jobNo = Convert.ToInt32(reader["sequenceOrControlNumber"]),
                            jobDate = "20210709",
                            reportingEvent = reader["RECODE"].ToString(),
                        },

                        location = new SCMTR_Location()
                        {
                            reportingPartyType = reader["RPTYPECODE"].ToString(),
                            reportingPartyCode = reader["STF_RPTYPE_CODE"].ToString(),
                            reportingLocationCode = reader["STF_RLOCT_CODE"].ToString(),
                            reportingLocationName = reader["STF_RLOCT_NAME"].ToString(),
                            authorisedPersonPAN = reader["STF_AP_PANNO"].ToString(),
                        },

                        cargoContainer = GetCargoContainerList(stfxid, stfdid),

                    },


                };

                stringjson = JsonConvert.SerializeObject(response);

                //string jsondata = new JavaScriptSerializer().Serialize(stringjson);
                string path = Server.MapPath("~/App_Data/");
                //string path = ConfigurationManager.AppSettings["JsonFileFilePath"];
                string pathLocalFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), jsonfilename);

                System.IO.File.WriteAllText(path + jsonfilename, stringjson.ToString());
                //try
                //{
                //    System.IO.File.WriteAllText(path + jsonfilename, stringjson.ToString());
                //}
                //catch(Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //}


                // write JSON directly to a file  
                //using (StreamWriter file = System.IO.File.CreateText(path + jsonfilename))
                //using (JsonTextWriter writer = new JsonTextWriter(file))
                //{
                //    stringjson.ToString().WriteTo(writer);
                //}

                // Write that JSON to txt file,  
                //System.IO.File.WriteAllText(path + jsonfilename + ".json", stringjson);
                //System.IO.File.WriteAllText(pathLocalFile + ".json", stringjson);

                //
                //if (System.IO.File.Exists(pathLocalFile))
                //{
                //    System.IO.File.Delete(pathLocalFile);
                //}


                // Write file contents on console.     
                //using (StreamReader sr = System.IO.File.OpenText(jsonfilename))
                //{
                //    string s = "";
                //    while ((s = sr.ReadLine()) != null)
                //    {
                //        Console.WriteLine(s);
                //    }
                //}
                //

                //update

            }
            myConnection.Close();

            return Content(stringjson);


        }
        #endregion

        #region GetCargoContainerList
        private List<SCMTR_Cargo_Container> GetCargoContainerList(int stfxid, int stfdid)
        {
            SqlDataReader reader = null;
            string _connStr = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(_connStr);

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "Select * from z_EXport_SCMTR_Json_File_Detail_Assgn Where STFXID = " + stfxid;
            sqlCmd.Connection = myConnection;
            myConnection.Open();
            reader = sqlCmd.ExecuteReader();

            int ccount = 1;
            List<SCMTR_Cargo_Container> cargoContainer = new List<SCMTR_Cargo_Container>();

            while (reader.Read())
            {
                string eventDate = Convert.ToDateTime(reader["STF_EVENTDATE"]).ToString("yyyyMMddThh:mm");

                cargoContainer.Add(new SCMTR_Cargo_Container
                {
                    messageType = reader["STF_MSG_TCODE"].ToString(),
                    equipmentSequenceNo = 1,
                    equipmentID = reader["STF_CONTNRNO"].ToString(),
                    equipmentType = reader["EQTCODE"].ToString(),
                    equipmentSize = reader["STF_CONTNRSDESC"].ToString(),
                    equipmentLoadStatus = reader["ELSCODE"].ToString(),
                    finalDestinationLocation = reader["FDCODE"].ToString(),
                    eventDate = eventDate,//"20210707T14:45",
                    equipmentSealType = reader["ESTYPEDESC"].ToString(),
                    equipmentSealNumber = reader["STF_EQSEALNO"].ToString(),
                    equipmentStatus = reader["ESCODE"].ToString(),
                    equipmentPkg = reader["STF_EQPKG_DESC"].ToString(),
                    equipmentQuantity = Convert.ToInt32(reader["STF_EQPKG_QTY"]),
                    equipmentQUC = reader["EUQCCODE"].ToString(),
                    cargoDetails = GetCargoDetailList(stfxid, stfdid, reader["STF_MSG_TCODE"].ToString(), reader["PRCODE"].ToString(),
                                       reader["ELSCODE"].ToString(), reader["EUQCCODE"].ToString(), reader["STF_MSG_TCODE"].ToString()),
                });

                ccount++;
            }


            return cargoContainer;
        }

        #endregion

        #region GetCargoDetailList
        private List<SCMTR_Cargo_Details> GetCargoDetailList(int stfxid, int id, string msgtype, string docsite, string loadstatus, string pkgcode, string shipmentLoadStatus)
        {
            SqlDataReader reader = null;
            string _connStr = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(_connStr);

            SqlCommand sqlCmd = new SqlCommand("z_pr_XML_Stuffed_SBill_No_Detail_With_CIN_No", myConnection);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@PSTFDID", id);
            sqlCmd.Connection = myConnection;
            myConnection.Open();
            reader = sqlCmd.ExecuteReader();

            int ccount = 1;
            List<SCMTR_Cargo_Details> cargoDetails = new List<SCMTR_Cargo_Details>();

            while (reader.Read())
            {

                cargoDetails.Add(new SCMTR_Cargo_Details
                {
                    messageType = msgtype,
                    cargoSequenceNo = ccount,
                    documentType = "ESB",
                    documentSite = docsite,
                    documentNumber = Convert.ToInt32(reader["STFDSBNO"]),
                    documentDate = Convert.ToDateTime(reader["STFDSBDATE"]).ToString("yyyyMMdd"),
                    shipmentLoadStatus = shipmentLoadStatus,
                    packageType = "P",
                    quantity = Convert.ToInt32(reader["STFDNOP"]),
                    packetsFrom = Convert.ToInt32(reader["STF_PACKETS_FROM"]),
                    packetsTo = Convert.ToInt32(reader["STF_PACKETS_TO"]),
                    packUQC = pkgcode,
                    mcinPcin = reader["CinNo"].ToString(),
                });
                //GetCargoContainerList(stfxid);
                ccount++;
            }


            return cargoDetails;
        }

        #endregion
    }
}