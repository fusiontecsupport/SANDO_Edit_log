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
using System.Web;
using System.Web.Mvc;
using static scfs_erp.Models.EInvoice;

namespace scfs.Controllers.EInvoice
{
    [SessionExpire]
    public class ExportManualEInvoiceController : Controller
    {
        // GET: ExportManualEInvoice
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        
        [Authorize(Roles = "ExportManualEInvoiceIndex")]
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
                Session["REGSTRID"] = "49";
            }

            if (Session["Group"].ToString() == "Exports")
            {
                ViewBag.aaa = "hide";
            }
            else
            {
                ViewBag.aaa = "hide1";
            }

            //...........Bill type......//
            //List<SelectListItem> selectedBILLYPE = new List<SelectListItem>();
            //if (Convert.ToInt32(Session["TRANBTYPE"]) == 1)
            //{
            //    SelectListItem selectedItemGPTY = new SelectListItem { Text = "STUFF", Value = "1", Selected = true };
            //    selectedBILLYPE.Add(selectedItemGPTY);
            //    selectedItemGPTY = new SelectListItem { Text = "GRT", Value = "2", Selected = false };
            //    selectedBILLYPE.Add(selectedItemGPTY);

            //}
            //else
            //{
            //    SelectListItem selectedItemGPTY = new SelectListItem { Text = "STUFF", Value = "1", Selected = false };
            //    selectedBILLYPE.Add(selectedItemGPTY);
            //    selectedItemGPTY = new SelectListItem { Text = "GRT", Value = "2", Selected = true };
            //    selectedBILLYPE.Add(selectedItemGPTY);

            //}
            //ViewBag.TRANBTYPE = selectedBILLYPE;
            //....end

            //............Billed to....//
            ViewBag.REGSTRID = new SelectList(context.Export_Invoice_Register.Where(x => x.REGSTRID == 49), "REGSTRID", "REGSTRDESC", Convert.ToInt32(Session["REGSTRID"]));
            //.....end


            DateTime sd = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;

            DateTime ed = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;
            return View();
            // return View(context.transactionmaster.Where(x => x.TRANDATE >= sd).Where(x => x.TRANDATE <= ed).ToList());
        }//...End of index grid


        //public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        //{
        //    using (var e = new CFSExportEntities())
        //    {
        //        var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
        //        var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

        //        var data = e.pr_Search_Export_Stuff_Billing(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
        //            totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), 0, Convert.ToInt32(Session["REGSTRID"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
        //        var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANTIME.Value.ToString("hh:mm tt"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANNAMT.ToString(), d.ACKNO, d.DISPSTATUS, d.GSTAMT.ToString(), d.TRANMID.ToString() }).ToArray();
        //        return Json(new
        //        {
        //            sEcho = param.sEcho,
        //            aaData = aaData,
        //            iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
        //            iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_ExportManualBill(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToInt32(Session["REGSTRID"]), 0, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANTIME.Value.ToString("hh:mm tt"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANNAMT.ToString(), d.GSTAMT.ToString(), d.ACKNO, d.DISPSTATUS, d.TRANMID.ToString() }).ToArray();
                //var aaData = data.Select(d => new string[] { d.TRANDATE.Value.ToString("dd/MM/yyyy"), d.TRANTIME.Value.ToString("hh:mm tt"), d.TRANDNO.ToString(), d.TRANREFNAME, d.TRANNAMT.ToString(), d.DISPSTATUS, d.TRANMID.ToString(), d.GSTAMT.ToString(), d.ACKNO }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "ExportManualEInvoicePrint")]
        public void UPrintView(int? id = 0)
        {

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM NEW_TMPRPT_IDS WHERE KUSRID ='" + Session["CUSRID"] + "'");
            //context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS");
            var TMPRPT_IDS = TMP_InsertPrint.NewInsertToTMP("NEW_TMPRPT_IDS", "OPTNSTR", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                string rptname = "";
                string QMDNO = id.ToString();
                var qmbtype = 0;
                var statetype = 0;
                string hstr = "";
                //var result = context.Database.SqlQuery<TransactionMaster>("Select * From TransactionMaster where TRANMID=" + id).ToList();
                //if (result.Count() != 0) { QMDNO = result[0].TRANNO.ToString(); }

                var strPath = ConfigurationManager.AppSettings["Reporturl"];

                var pdfPath = ConfigurationManager.AppSettings["pdfurl"];

                rptname = "E_2003.rpt";
                hstr = "{VW_IMPORT_EINVOICE_MANUAL_BILL_DETAIL_RPT.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_IMPORT_EINVOICE_MANUAL_BILL_DETAIL_RPT.TRANMID} = " + id;// + " AND {?Pm-VW_EXPORT_INVOICE_DOSPRINT_03.TRANMID} = " + id;


                cryRpt.Load(strPath + "\\" + rptname);

                cryRpt.RecordSelectionFormula = hstr;

                String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
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

                string path = pdfPath + "\\" + Session["CUSRID"] + "\\export";
                if (!(Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);
                }
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, path + "\\" + QMDNO + ".pdf");

                cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                //cryRpt.PrintToPrinter(1,false,0,0);
                cryRpt.Dispose();
                cryRpt.Close();
                GC.Collect();
                stringbuilder.Clear();
            }

        }


        private List<ItemList> GetItemList(int id)
        {
            SqlDataReader reader = null;
            string _connStr = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(_connStr);

            SqlCommand sqlCmd = new SqlCommand("pr_EInvoice_Export_Manual_Transaction_Detail_Assgn", myConnection);
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

        [Authorize(Roles = "ExportManualEInvoiceUpload")]
        public ActionResult CInvoice(int id = 0)/*10rs.reminder*/
        {

            SqlDataReader reader = null;
            SqlDataReader Sreader = null;
            string _connStr = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(_connStr);

            //string _SconnStr = ConfigurationManager.ConnectionStrings["ServerContext"].ConnectionString;
            //SqlConnection SmyConnection = new SqlConnection(_SconnStr);

            var tranmid = id;// Convert.ToInt32(Request.Form.Get("id"));// Convert.ToInt32(ids);

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "Select * from Z_EXPORT_MANUAL_EINVOICE_DETAILS Where TRANMID = " + tranmid;
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
                //SqlDataAdapter Sqladapter = new SqlDataAdapter(SsqlCmd);
                //Sqladapter.Fill(dt);


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
                //}


                //update

            }


            //SmyConnection.Close();
            myConnection.Close();

            return Content(stringjson);



        }

        [Authorize(Roles = "ExportManualEInvoiceUpload")]
        public void UInvoice(int id = 0)/*10rs.reminder*/
        {
            SqlDataReader reader = null;
            SqlDataReader Sreader = null;
            string _connStr = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
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


    }
}