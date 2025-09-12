using scfs_erp.Context;
using scfs_erp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using static scfs_erp.Models.EInvoice;
using scfs.Data;
using System.Net;
using System.IO;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using scfs_erp.Helper;
using scfs.Models;

namespace scfs_erp.Controllers.EInvoice
{
    public class ImportEInvoiceController : Controller
    {
        // GET: ImportEInvoice

        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        [Authorize(Roles = "ImportEInvoiceIndex")]
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
            if (Request.Form.Get("TRANBTYPE") != null)
            {
                Session["TRANBTYPE"] = Request.Form.Get("TRANBTYPE");
                Session["REGSTRID"] = Request.Form.Get("REGSTRID");
            }
            else
            {
                Session["TRANBTYPE"] = "1";
                Session["REGSTRID"] = "1";
            }


            //...........Bill type......//
            List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
            if (Convert.ToInt32(Session["TRANBTYPE"]) == 1)
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);

            }
            else
            {
                SelectListItem selectedItemGPTY = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                selectedBILLYPE.Add(selectedItemGPTY);
                selectedItemGPTY = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = true };
                selectedBILLYPE.Add(selectedItemGPTY);

            }
            ViewBag.TRANBTYPE = selectedBILLYPE;
            //....end

            //............Billed to....//
            ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == 1 || x.REGSTRID == 2 || x.REGSTRID == 6 || x.REGSTRID == 70), "REGSTRID", "REGSTRDESC", Convert.ToInt32(Session["REGSTRID"]));
            //.....end


            DateTime sd = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;

            DateTime ed = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;
            return View();
            // return View(context.transactionmaster.Where(x => x.TRANDATE >= sd).Where(x => x.TRANDATE <= ed).ToList());
        }//...End of index grid

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Import_Invoice(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToInt32(Session["TRANBTYPE"]), Convert.ToInt32(Session["REGSTRID"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANTIME.Value.ToString("hh:mm tt"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANNAMT.ToString(), d.GSTAMT.ToString(), d.DISPSTATUS, d.ACKNO,d.dono.ToString(), d.TRANMID.ToString() }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "ImportEInvoiceUpload")]
        public ActionResult CInvoice(int id = 0)/*10rs.reminder*/
        {

            SqlDataReader reader = null;
            //SqlDataReader Sreader = null;
            string _connStr = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(_connStr);

            string _SconnStr = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnection SmyConnection = new SqlConnection(_SconnStr);

            var tranmid = id;// Convert.ToInt32(Request.Form.Get("id"));// Convert.ToInt32(ids);

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "Select * from Z_IMPORT_EINVOICE_DETAILS Where TRANMID = " + tranmid;
            sqlCmd.Connection = myConnection;
            myConnection.Open();
            reader = sqlCmd.ExecuteReader();

            int custgid = 0;
            string suptyp = "";
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
                strgamt = Convert.ToDecimal(reader["STRG_TAXABLE_AMT"]);
                strg_cgst_amt = Convert.ToDecimal(reader["STRG_CGST_AMT"]);
                strg_sgst_amt = Convert.ToDecimal(reader["STRG_SGST_AMT"]);
                strg_igst_amt = Convert.ToDecimal(reader["STRG_IGST_AMT"]);

                handlamt = Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]);
                handl_cgst_amt = Convert.ToDecimal(reader["HANDL_CGST_AMT"]);
                handl_sgst_amt = Convert.ToDecimal(reader["HANDL_SGST_AMT"]);
                handl_igst_amt = Convert.ToDecimal(reader["HANDL_IGST_AMT"]);

                cgst_amt = Convert.ToDecimal(reader["CGST_AMT"]);
                sgst_amt = Convert.ToDecimal(reader["SGST_AMT"]);
                igst_amt = Convert.ToDecimal(reader["IGST_AMT"]);

                custgid = Convert.ToInt32(reader["CUSTGID"]);

                switch (custgid)
                {
                    case 6:
                        suptyp = "SEZWOP";
                        break;
                    default:
                        suptyp = "B2B";
                        break;
                }

                var response = new Response()
                {
                    Version = "1.1",

                    TranDtls = new TranDtls()
                    {
                        TaxSch = "GST",
                        SupTyp = suptyp,//"B2B",
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
                        LglNm = reader["TRANREFNAME"].ToString(),
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
                        AssVal = strgamt + handlamt,// Convert.ToDecimal(reader["HANDL_TAXABLE_AMT"]),
                        CesVal = 0,
                        CgstVal = cgst_amt,// Convert.ToDecimal(reader["HANDL_CGST_AMT"]),
                        IgstVal = igst_amt,// Convert.ToDecimal(reader["HANDL_IGST_AMT"]),
                        OthChrg = 0,
                        SgstVal = sgst_amt,// Convert.ToDecimal(reader["HANDL_sGST_AMT"]),
                        Discount = 0,
                        StCesVal = 0,
                        RndOffAmt = 0,
                        TotInvVal = Convert.ToDecimal(reader["TRANNAMT"]),
                        TotItemValSum = strgamt + handlamt,//Convert.ToDecimal(reader["TOTALITEMVAL"])
                    },

                    ItemList = GetItemList(tranmid),
                 

                };

                stringjson = JsonConvert.SerializeObject(response);
                //update
                //string result = "";
                //DataTable dt = new DataTable();
                //SqlCommand SsqlCmd = new SqlCommand();
                //SsqlCmd.CommandType = CommandType.Text;
                //SsqlCmd.CommandText = "Select * from ETRANSACTIONMASTER Where TRANMID = " + tranmid;
                //SsqlCmd.Connection = SmyConnection;
                //SmyConnection.Open();
                //// Sreader = SsqlCmd.ExecuteReader();
                //SqlDataAdapter Sqladapter = new SqlDataAdapter(SsqlCmd);
                //Sqladapter.Fill(dt);
                ////dt.Load(Sreader);
                //// int numRows = dt.Rows.Count;



                //if (dt.Rows.Count > 0)
                //{

                //    foreach (DataRow row in dt.Rows)
                //    {
                //        SqlConnection ZmyConnection = new SqlConnection(_SconnStr);
                //        SqlCommand cmd = new SqlCommand("ETransaction_Update_Assgn", ZmyConnection);
                //        cmd.CommandType = CommandType.StoredProcedure;
                //        //cmd.Parameters.AddWithValue("@CustomerID", 0);    
                //        cmd.Parameters.AddWithValue("@PTranMID", tranmid);
                //        cmd.Parameters.AddWithValue("@PEINVDESC", stringjson);
                //        cmd.Parameters.AddWithValue("@PECINVDESC", row["ECINVDESC"].ToString());
                //        ZmyConnection.Open();
                //        //result = cmd.ExecuteScalar().ToString();
                //        ZmyConnection.Close();
                //    }

                //    //while (Sreader.Read()) 
                //    //{
                //    //    SqlCommand cmd = new SqlCommand("ETransaction_Update_Assgn", SmyConnection);
                //    //    cmd.CommandType = CommandType.StoredProcedure;
                //    //    //cmd.Parameters.AddWithValue("@CustomerID", 0);    
                //    //    cmd.Parameters.AddWithValue("@PTranMID", tranmid);
                //    //    cmd.Parameters.AddWithValue("@PEINVDESC", stringjson);
                //    //    cmd.Parameters.AddWithValue("@PECINVDESC", Sreader["ECINVDESC"].ToString());
                //    //    SmyConnection.Open();
                //    //    result = cmd.ExecuteScalar().ToString();
                //    //    SmyConnection.Close();
                //    //}

                //}
                //else
                //{
                //    SqlConnection ZmyConnection = new SqlConnection(_SconnStr);
                //    SqlCommand cmd = new SqlCommand("ETransaction_Insert_Assgn", ZmyConnection);
                //    cmd.CommandType = CommandType.StoredProcedure;
                //    //cmd.Parameters.AddWithValue("@CustomerID", 0);    
                //    cmd.Parameters.AddWithValue("@PTranMID", tranmid);
                //    cmd.Parameters.AddWithValue("@PEINVDESC", stringjson);
                //    cmd.Parameters.AddWithValue("@PECINVDESC", "");
                //    ZmyConnection.Open();
                //    cmd.ExecuteNonQuery();
                //    ZmyConnection.Close();
                //    //result = cmd.ExecuteNonQuery().ToString();
                //}


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

        [Authorize(Roles = "ImportEInvoicePrint")]
        public void UPrintView(int? id = 0)
        {

            // Response.Write(@"10.10.5.5"); Response.End();
            //  ........delete TMPRPT...//
            //var param = id.Split(';');

            var ids = id;

            calc(Convert.ToInt32(id));

            context.Database.ExecuteSqlCommand("DELETE FROM NEW_TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.NewInsertToTMP("NEW_TMPRPT_IDS", "IMPORTEINVOICE", Convert.ToInt32(ids), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;



                // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");


                //........Get TRANPCOUNT...//
                var Query = context.Database.SqlQuery<TransactionMaster>("select * from transactionmaster where TRANMID=" + ids).ToList();
                var PCNT = 0;
                var slbnrnsts = 0;
                if (Query.Count() != 0) { 
                    PCNT = Convert.ToInt32(Query[0].TRANPCOUNT);
                    slbnrnsts = Convert.ToInt32(Query[0].SLABNARN_STS);
                }
                var TRANPCOUNT = ++PCNT;
                // Response.Write(++PCNT);
                // Response.End();
                context.Database.ExecuteSqlCommand("UPDATE transactionmaster SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + ids);

                //var tariffmid = 0;
                //var Query1 = context.Database.SqlQuery<TransactionDetail>("select * From TransactionDetail where TranMId=" + id).ToList();
                //if (Query1.Count() != 0)
                //{
                //    tariffmid = Convert.ToInt32(Query1[0].TARIFFMID);
                //}

                //var chkid = 0;
                //var AQuery = context.Database.SqlQuery<int>("select Count(*) From SLABNARRATIONMASTER where TARIFFMID=" + tariffmid).ToList();
                //if (AQuery.Count() != 0)
                //{
                //    chkid = Convert.ToInt32(AQuery[0]);
                //}


                string rptname = "";
                //if (rpttype == 0)

                //if (chkid == 0)
                if (slbnrnsts == 0)
                {
                    rptname = "E_1001.rpt";
                }
                else
                {
                    rptname = "BMW_1001.rpt";
                }
                //else cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_group_rpt_E01.RPT");
                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + rptname);
                cryRpt.RecordSelectionFormula = "{VW_IMPORT_EINVOICE_TRANSACTION_GST_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_IMPORT_EINVOICE_TRANSACTION_GST_PRINT_ASSGN.TRANMID} = " + ids;



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
                //string path = "E:\\CFS\\" + Session["CUSRID"] + "\\ImportInv";
                //if (!(Directory.Exists(path)))
                //{
                //    Directory.CreateDirectory(path);
                //}
                //cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, path + "\\" + Query[0].TRANNO + ".pdf");
                //  cryRpt.SaveAs(path+ "\\"+Query[0].TRANNO+".pdf");
                cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                cryRpt.Dispose();
                cryRpt.Close();
                GC.Collect();
                stringbuilder.Clear();
            }
        
        }

        [Authorize(Roles = "ImportEInvoiceUpload")]
        public void UInvoice(int id = 0)/*10rs.reminder*/
        {
            SqlDataReader reader = null;
            SqlDataReader Sreader = null;
            string _connStr = ConfigurationManager.ConnectionStrings["scfserp"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(_connStr);

            var tranmid = id;// Convert.ToInt32(Request.Form.Get("id"));// Convert.ToInt32(ids);

            // using System.Net;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            // Use SecurityProtocolType.Ssl3 if needed for compatibility reasons

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;

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
                //if (param[2] != "") { zirnno = param[2].Substring(6); } else { zirnno = ""; }
                //if (param[3] != "") { zackdt = param[3].Substring(8); } else { zackdt = ""; }
                //if (param[4] != "") { zackno = param[4].Substring(8); } else { zackno = ""; }
                //if (param[14] != "") { imgUrl = param[14].ToString(); } else { imgUrl = ""; }

                if (param[3] != "") { zirnno = param[3].Substring(6); } else { zirnno = ""; }
                if (param[4] != "") { zackdt = param[4].Substring(8); } else { zackdt = ""; }
                if (param[5] != "") { zackno = param[5].Substring(8); } else { zackno = ""; }
                if (param[17] != "") { imgUrl = param[17].ToString(); } else { imgUrl = ""; }

                SqlConnection GmyConnection = new SqlConnection(_connStr);
                SqlCommand cmd = new SqlCommand("pr_IRN_Transaction_Update_Assgn", GmyConnection);
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

                string path = Server.MapPath("~/QrCode");


                WebClient webClient = new WebClient();
                webClient.DownloadFile(remoteFileUrl, path + "\\" + localFileName);

                SqlConnection XmyConnection = new SqlConnection(_connStr);
                SqlCommand Xcmd = new SqlCommand("pr_Transaction_QrCode_Path_Update_Assgn", XmyConnection);
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


        private List<ItemList> GetItemList(int id)
        {
            SqlDataReader reader = null;
            string _connStr = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(_connStr);

            SqlCommand sqlCmd = new SqlCommand("pr_EInvoice_Import_Transaction_Detail_Assgn", myConnection);
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

        public void calc(int id)
        {
            using (SCFSERPContext contxt = new SCFSERPContext())
            {
                int trandid = 0;
                int TARIFFMID = 0;
                int TRANOTYPE = 0;
                decimal TRANDQTY = 0;
                int CHRGETYPE = 0;
                int CONTNRSID = 0;
                int STMRID = 0;

                int OTYPE = 0;

                var squery = context.Database.SqlQuery<VW_TRANSACTION_SLAB_RE_CALC_ASSGN>("select * from VW_TRANSACTION_SLAB_RE_CALC_ASSGN where TRANMID = " + id).ToList();
                foreach (var zrslt in squery)
                {
                    TARIFFMID = zrslt.TARIFFMID;
                    TRANOTYPE = zrslt.TRANOTYPE;
                    TRANDQTY = zrslt.TRANDQTY;
                    CHRGETYPE = zrslt.TRANBTYPE;
                    CONTNRSID = zrslt.CONTNRSID;
                    STMRID = zrslt.STMRID;
                    trandid = zrslt.TRANDID;

                    if (CHRGETYPE == 2)
                    {
                        if (TRANOTYPE == 1 || TRANOTYPE == 3 || TRANOTYPE == 4 || TRANOTYPE == 5)
                        {
                            OTYPE = 2;
                        }
                    }
                    else
                    {
                        if (TRANOTYPE == 0 || TRANOTYPE == 1 || TRANOTYPE == 2 || TRANOTYPE == 3 || TRANOTYPE == 4 || TRANOTYPE == 5)
                        {
                            OTYPE = 1;
                        }
                    }

                    var zquery = "";
                    int ccount = 1;

                    if (TARIFFMID == 4)
                    {
                        zquery = "select SLABAMT,SLABMIN,SLABMAX from VW_IMPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID = 2 and HTYPE = " + OTYPE + " and SDTYPE = 1 and SLABMIN <= " + TRANDQTY + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " and STMRID=" + STMRID + " order by SLABMIN";
                    }
                    else
                    {
                        zquery = "select SLABAMT,SLABMIN,SLABMAX from VW_IMPORT_RATECARDMASTER_FLX_ASSGN where TARIFFMID=" + TARIFFMID + "and SLABTID=2 and HTYPE=" + OTYPE + " and SDTYPE=1 and SLABMIN <= " + TRANDQTY + " and CHRGETYPE=" + CHRGETYPE + " and CONTNRSID= " + CONTNRSID + " order by SLABMIN";

                    }
                    var xquery = context.Database.SqlQuery<VW_IMPORT_RATECARDMASTER_FLX_ASSGN>(zquery).ToList();

                    decimal ramt1 = 0;
                    decimal ramt2 = 0;
                    decimal ramt3 = 0;
                    decimal ramt4 = 0;
                    decimal ramt5 = 0;
                    decimal ramt6 = 0;

                    decimal rcamt1 = 0;
                    decimal rcamt2 = 0;
                    decimal rcamt3 = 0;
                    decimal rcamt4 = 0;
                    decimal rcamt5 = 0;
                    decimal rcamt6 = 0;

                    decimal rcol1 = 0;
                    decimal rcol2 = 0;
                    decimal rcol3 = 0;
                    decimal rcol4 = 0;
                    decimal rcol5 = 0;
                    decimal rcol6 = 0;

                    decimal slabmin = 0;
                    decimal slabmax = 0;

                    decimal slabmin1 = 0;
                    decimal slabmax1 = 0;

                    decimal slabmin2 = 0;
                    decimal slabmax2 = 0;

                    var mmdiff = 0;

                    string kgstrsql = "";

                    int bdays = Convert.ToInt32(TRANDQTY);

                    foreach (var rslt in xquery)
                    {
                        switch (ccount)
                        {
                            case 1:
                                ramt1 = rslt.SLABAMT;
                                ramt2 = 0;
                                ramt3 = 0;
                                ramt4 = 0;
                                ramt5 = 0;
                                ramt6 = 0;

                                slabmin = rslt.SLABMIN;
                                slabmax = rslt.SLABMAX;

                                slabmin1 = 0;
                                slabmax1 = 0;

                                slabmin2 = 0;
                                slabmax2 = 0;

                                //var slabmin = $(".SLABMIN")[i].value;
                                //var slabmax = $(".SLABMAX")[i].value;
                                mmdiff = Convert.ToInt32(slabmin) - Convert.ToInt32(slabmax);// ($(".SLABMIN")[i].value) -($(".SLABMAX")[i].value);

                                //var slabmin1 = $(".SLABMIN1")[i].value;
                                //var slabmax1 = $(".SLABMAX1")[i].value;

                                rcol1 = Math.Abs(mmdiff);
                                rcol2 = 0;

                                rcamt1 = ramt1 * rcol1;
                                rcamt2 = 0;
                                bdays = Convert.ToInt32(bdays) - Convert.ToInt32(rcol1);

                                kgstrsql = "Update TransactionDetail Set RCOL1 = " + rcol1 + ", RAMT1 = " + ramt1 + ", RCAMT1 = " + rcamt1 + " Where TRANDID = " + trandid;
                                context.Database.ExecuteSqlCommand(kgstrsql);
                                //    $(".RCOL1")[i].value = ($(".days")[i].value);//Math.abs(mmdiff);
                                //    $(".RCOL2")[i].value = 0;//(Math.abs((mmdiff - (($(".days")[i].value)))) + 1);

                                //var ramt1 = $(".RAMT1")[i].value;
                                //var ramt2 = $(".RAMT2")[i].value;

                                //var rcamt1 = ramt1 * $(".RCOL1")[i].value;// Math.abs(mmdiff);
                                //var rcamt2 = 0;//ramt2 * $(".RCOL2")[i].value;

                                //    $(".RCAMT1")[i].value = rcamt1;
                                //    $(".RCAMT2")[i].value = rcamt2;
                                //    $(".RCAMT3")[i].value = 0;

                                //    $(".RCAMT4")[i].value = 0;
                                //    $(".RCAMT5")[i].value = 0;
                                //    $(".RCAMT6")[i].value = 0;
                                break;

                            case 2:
                                ramt1 = 0;// rslt.SLABAMT;
                                ramt2 = rslt.SLABAMT;
                                ramt3 = 0;
                                ramt4 = 0;
                                ramt5 = 0;
                                ramt6 = 0;

                                slabmin = rslt.SLABMIN;
                                slabmax = rslt.SLABMAX;

                                slabmin1 = rslt.SLABMIN;
                                slabmax1 = rslt.SLABMAX;

                                slabmin2 = 0;
                                slabmax2 = 0;

                                //var slabmin = $(".SLABMIN")[i].value;
                                //var slabmax = $(".SLABMAX")[i].value;
                                mmdiff = Convert.ToInt32(slabmin) - Convert.ToInt32(slabmax);

                                //var slabmin1 = $(".SLABMIN1")[i].value;
                                //var slabmax1 = $(".SLABMAX1")[i].value;

                                //rcol1 = Math.Abs(mmdiff);
                                rcol2 = Math.Abs(mmdiff) + 1 ;// (Math.Abs((slabmin1 - TRANDQTY)) +1);

                                if (bdays > rcol2)
                                {
                                    rcamt2 = ramt2 * rcol2;
                                    kgstrsql = "Update TransactionDetail Set RCOL2 = " + rcol2 + ", RAMT2 = " + ramt2 + ", RCAMT2 = " + rcamt2 + " Where TRANDID = " + trandid;
                                }
                                else
                                {
                                    rcamt2 = ramt2 * bdays;
                                    kgstrsql = "Update TransactionDetail Set RCOL2 = " + bdays + ", RAMT2 = " + ramt2 + ", RCAMT2 = " + rcamt2 + " Where TRANDID = " + trandid;
                                }
                                bdays = Convert.ToInt32(bdays) - Convert.ToInt32(rcol2);

                                
                                context.Database.ExecuteSqlCommand(kgstrsql);

                                //var ramt1 = $(".RAMT1")[i].value;
                                //var ramt2 = $(".RAMT2")[i].value;

                                //rcamt1 = 0;// ramt1 * Math.Abs(mmdiff);
                                //rcamt2 = ramt2 * rcol2;

                                //$(".RCAMT1")[i].value = rcamt1;
                                //$(".RCAMT2")[i].value = rcamt2;
                                //$(".RCAMT3")[i].value = 0;

                                //$(".RCAMT4")[i].value = 0;
                                //$(".RCAMT5")[i].value = 0;
                                //$(".RCAMT6")[i].value = 0;

                                break;
                            case 3:
                                ramt1 = 0;// rslt.SLABAMT;
                                ramt2 = 0;// rslt.SLABAMT;
                                ramt3 = rslt.SLABAMT;
                                ramt4 = 0;
                                ramt5 = 0;
                                ramt6 = 0;

                                slabmin = rslt.SLABMIN;
                                slabmax = rslt.SLABMAX;

                                slabmin1 = rslt.SLABMIN;
                                slabmax1 = rslt.SLABMAX;

                                slabmin2 = rslt.SLABMIN;
                                slabmax2 = rslt.SLABMAX;

                                //var slabmin = $(".SLABMIN")[i].value;
                                //var slabmax = $(".SLABMAX")[i].value;
                                mmdiff = Convert.ToInt32(slabmin1) - Convert.ToInt32(slabmax1) ;// ($(".SLABMIN")[i].value) -($(".SLABMAX")[i].value);

                                //var slabmin1 = $(".SLABMIN1")[i].value;
                                //var slabmax1 = $(".SLABMAX1")[i].value;
                                //var mmdiff1 = (($(".SLABMAX1")[i].value) -($(".SLABMIN1")[i].value)) +parseInt(1);

                                //var slabmin2 = $(".SLABMIN2")[i].value;
                                //var slabmax2 = $(".SLABMAX2")[i].value;


                                //var ramt1 = $(".RAMT1")[i].value;
                                //var ramt2 = $(".RAMT2")[i].value;
                                //var ramt3 = $(".RAMT3")[i].value;

                                //rcol1 = Math.Abs(mmdiff);
                                //rcol2 = Math.Abs(mmdiff);
                                rcol3 = Math.Abs(mmdiff) + 1; //(Math.Abs((slabmin2 - TRANDQTY)) +1);

                                if (bdays > rcol3)
                                {
                                    rcamt3 = ramt3 * rcol3;
                                    kgstrsql = "Update TransactionDetail Set RCOL3 = " + rcol3 + ", RAMT3 = " + ramt3 + ", RCAMT3 = " + rcamt3 + " Where TRANDID = " + trandid;
                                }
                                else
                                {
                                    rcamt3 = ramt3 * bdays;
                                    kgstrsql = "Update TransactionDetail Set RCOL3 = " + bdays + ", RAMT3 = " + ramt3 + ", RCAMT3 = " + rcamt3 + " Where TRANDID = " + trandid;

                                }
                                bdays = Convert.ToInt32(bdays) - Convert.ToInt32(rcol3);

                                context.Database.ExecuteSqlCommand(kgstrsql);

                                //rcamt1 = ramt1 * Math.Abs(mmdiff);
                                //rcamt2 = ramt2 * Math.Abs(mmdiff);
                                //rcamt3 = ramt3 * rcol3;//$(".RCOL3")[i].value;

                                //$(".RCAMT1")[i].value = rcamt1;
                                //$(".RCAMT2")[i].value = rcamt2;
                                //$(".RCAMT3")[i].value = rcamt3;

                                //$(".RCAMT4")[i].value = 0;
                                //$(".RCAMT5")[i].value = 0;
                                //$(".RCAMT6")[i].value = 0;

                                break;
                            case 4:
                                ramt1 = 0;// rslt.SLABAMT;
                                ramt2 = 0;// rslt.SLABAMT;
                                ramt3 = 0;
                                ramt4 = rslt.SLABAMT;
                                ramt5 = 0;
                                ramt6 = 0;

                                slabmin = rslt.SLABMIN;
                                slabmax = rslt.SLABMAX;

                                slabmin1 = rslt.SLABMIN;
                                slabmax1 = rslt.SLABMAX;

                                slabmin2 = rslt.SLABMIN;
                                slabmax2 = rslt.SLABMAX;

                                //var slabmin = $(".SLABMIN")[i].value;
                                //var slabmax = $(".SLABMAX")[i].value;
                                mmdiff = Convert.ToInt32(slabmin1) - Convert.ToInt32(slabmax1);// ($(".SLABMIN")[i].value) -($(".SLABMAX")[i].value);

                                //var slabmin1 = $(".SLABMIN1")[i].value;
                                //var slabmax1 = $(".SLABMAX1")[i].value;
                                //var mmdiff1 = (($(".SLABMAX1")[i].value) -($(".SLABMIN1")[i].value)) +parseInt(1);

                                //var slabmin2 = $(".SLABMIN2")[i].value;
                                //var slabmax2 = $(".SLABMAX2")[i].value;


                                //var ramt1 = $(".RAMT1")[i].value;
                                //var ramt2 = $(".RAMT2")[i].value;
                                //var ramt3 = $(".RAMT3")[i].value;

                                //rcol1 = Math.Abs(mmdiff);
                                //rcol2 = Math.Abs(mmdiff);
                                //rcol3 = Math.Abs(mmdiff) + 1; //(Math.Abs((slabmin2 - TRANDQTY)) +1);
                                rcol4 = Math.Abs(mmdiff) + 1; //(Math.Abs((slabmin2 - TRANDQTY)) +1);

                                if (bdays > rcol4)
                                {
                                    rcamt4 = ramt4 * rcol4;
                                    kgstrsql = "Update TransactionDetail Set RCOL4 = " + rcol4 + ", RAMT4 = " + ramt4 + ", RCAMT4 = " + rcamt4 + " Where TRANDID = " + trandid;
                                }
                                else
                                {
                                    rcamt4 = ramt4 * bdays;
                                    kgstrsql = "Update TransactionDetail Set RCOL4 = " + bdays + ", RAMT4 = " + ramt4 + ", RCAMT4 = " + rcamt4 + " Where TRANDID = " + trandid;
                                }
                                bdays = Convert.ToInt32(bdays) - Convert.ToInt32(rcol4);

                                context.Database.ExecuteSqlCommand(kgstrsql);

                                //rcamt1 = ramt1 * Math.Abs(mmdiff);
                                //rcamt2 = ramt2 * Math.Abs(mmdiff);
                                //rcamt3 = ramt3 * rcol3;//$(".RCOL3")[i].value;

                                //$(".RCAMT1")[i].value = rcamt1;
                                //$(".RCAMT2")[i].value = rcamt2;
                                //$(".RCAMT3")[i].value = rcamt3;

                                //$(".RCAMT4")[i].value = 0;
                                //$(".RCAMT5")[i].value = 0;
                                //$(".RCAMT6")[i].value = 0;

                                break;
                            case 5:
                                ramt1 = 0;// rslt.SLABAMT;
                                ramt2 = 0;// rslt.SLABAMT;
                                ramt3 = 0;
                                ramt4 = 0;
                                ramt5 = rslt.SLABAMT;
                                ramt6 = 0;

                                slabmin = rslt.SLABMIN;
                                slabmax = rslt.SLABMAX;

                                slabmin1 = rslt.SLABMIN;
                                slabmax1 = rslt.SLABMAX;

                                slabmin2 = rslt.SLABMIN;
                                slabmax2 = rslt.SLABMAX;

                                //var slabmin = $(".SLABMIN")[i].value;
                                //var slabmax = $(".SLABMAX")[i].value;
                                mmdiff = Convert.ToInt32(slabmin1) - Convert.ToInt32(slabmax1);// ($(".SLABMIN")[i].value) -($(".SLABMAX")[i].value);

                                //var slabmin1 = $(".SLABMIN1")[i].value;
                                //var slabmax1 = $(".SLABMAX1")[i].value;
                                //var mmdiff1 = (($(".SLABMAX1")[i].value) -($(".SLABMIN1")[i].value)) +parseInt(1);

                                //var slabmin2 = $(".SLABMIN2")[i].value;
                                //var slabmax2 = $(".SLABMAX2")[i].value;


                                //var ramt1 = $(".RAMT1")[i].value;
                                //var ramt2 = $(".RAMT2")[i].value;
                                //var ramt3 = $(".RAMT3")[i].value;

                                //rcol1 = Math.Abs(mmdiff);
                                //rcol2 = Math.Abs(mmdiff);
                                //rcol3 = Math.Abs(mmdiff) + 1; //(Math.Abs((slabmin2 - TRANDQTY)) +1);
                                //rcol4 = Math.Abs(mmdiff) + 1; //(Math.Abs((slabmin2 - TRANDQTY)) +1);
                                rcol5 = Math.Abs(mmdiff) + 1; //(Math.Abs((slabmin2 - TRANDQTY)) +1);

                                if (bdays > rcol5)
                                {
                                    rcamt5 = ramt5 * rcol5;
                                    kgstrsql = "Update TransactionDetail Set RCOL5 = " + rcol5 + ", RAMT5 = " + ramt5 + ", RCAMT5 = " + rcamt5 + " Where TRANDID = " + trandid;
                                }
                                else
                                {
                                    rcamt5 = ramt5 * bdays;
                                    kgstrsql = "Update TransactionDetail Set RCOL5 = " + bdays + ", RAMT5 = " + ramt5 + ", RCAMT5 = " + rcamt5 + " Where TRANDID = " + trandid;
                                }
                                bdays = Convert.ToInt32(bdays) - Convert.ToInt32(rcol5);

                                context.Database.ExecuteSqlCommand(kgstrsql);

                                //rcamt1 = ramt1 * Math.Abs(mmdiff);
                                //rcamt2 = ramt2 * Math.Abs(mmdiff);
                                //rcamt3 = ramt3 * rcol3;//$(".RCOL3")[i].value;

                                //$(".RCAMT1")[i].value = rcamt1;
                                //$(".RCAMT2")[i].value = rcamt2;
                                //$(".RCAMT3")[i].value = rcamt3;

                                //$(".RCAMT4")[i].value = 0;
                                //$(".RCAMT5")[i].value = 0;
                                //$(".RCAMT6")[i].value = 0;

                                break;
                        }

                        ccount++;
                    }

                }






            //    Response.Write(tabl);
            }
        }
    }
}