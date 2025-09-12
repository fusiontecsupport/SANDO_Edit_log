using scfs.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace scfs.Controllers.General
{
    public class MailFormController : Controller
    {
        // GET: MailForm
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

        [Authorize(Roles = "MailFormIndex")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "MailFormSend")]
        public ActionResult MForm(int id)
        {
            //emailinvoicetemplate vm = new emailinvoicetemplate();


            emailinvoicetemplate objemailinvoicetemplate = new emailinvoicetemplate();
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ViewBag.id = id;

            

            //string custname = "";
            var REGSTRID = 0;
            var SDPTID= 0;
            var tranmid = id;
            var custname = "";
            var custname1 = "";
            var chaid = 0;
            //var qmbtype = 0;
            //string qmdno = "";
            //string qmno = "";
            var query = context.Database.SqlQuery<TransactionMaster>("select * from TransactionMaster where tranmid =" + tranmid).ToList();
            
            if (query.Count() != 0)
            {
               // qmbtype = query[0].QMBTYPE;
                //qmdno = query[0].QMDNO;
                //qmno = query[0].QMNO.ToString();
                REGSTRID = query[0].REGSTRID;
                SDPTID = query[0].SDPTID;
                custname = query[0].TRANREFNAME;
                custname = custname.Replace("&", "");
                custname = custname.Replace("'", "");
                custname = custname.Replace(".", "");
                custname = custname.Replace(",", "");
                 custname1 = query[0].TRANREFNAME;
                 chaid = Convert.ToInt32(query[0].TRANREFID);
            }

            ViewBag.FQMREFNAME = Session["COMPNAME"];
            ViewBag.CATEMAIL = "support@fustiontec.com"; // query[0].QMEMAILID;
            ViewBag.FQMDNO = ""; // qmdno;
            ViewBag.messageid = "";
            ViewBag.REGSTRID = REGSTRID.ToString();
            ViewBag.SDPTID = SDPTID.ToString();
            ViewBag.qmbtype = ""; // qmbtype;
            var sdpttype = "Invoice";
            if (SDPTID == 9)
                sdpttype = "Non PNR";
            else if (SDPTID == 1)
                sdpttype = "Import";
            else if (SDPTID == 2)
                sdpttype = "Export";
            else if (SDPTID == 11)
                sdpttype = "E-Seal";
            
            

            string emailsubject = "";
            string emailbody = "";
            emailsubject = "Invoice: " + query[0].TRANDNO;
            var TmpAStr = "<table width='539' border='0' cellpadding='0' cellspacing='0' style='background:#FFCC66;border:1px solid #663300'><tr><Td colspan='2' style='background:#663300;color:#FFFFFF;font-weight:700;text-align:center;padding:5px'>" + sdpttype + "</Td> </tr> <tr>";
            TmpAStr = TmpAStr + " <th style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left'width='208' >Invoice No. </th> <td style='padding:4px;border-bottom:1px solid #663300' width='331'>" + query[0].TRANDNO + "</td> </tr> <tr> <th style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Date </th>";
            TmpAStr = TmpAStr + " <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANDATE + "</td> </tr> <tr> <th style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Bill Amount </th> <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANNAMT.ToString() + "</td> </tr> <tr> <th style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >CHA Name </th>";
            TmpAStr = TmpAStr + " <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANREFNAME + "</td> </tr> </table> <br> <br> <table width='539' border='0' cellpadding='0' cellspacing='0' style='background:#FFCC66;border:1px solid #663300 '> <tr> <th style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left'width='208' >" + Session["COMPNAME"] + "</th> </tr> <tr> <th style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >" + Session["COMPADDR"] + "</th>";
            TmpAStr = TmpAStr + " </tr> </table>";
            string mlogo = ConfigurationManager.AppSettings["MailLogo"];
            emailbody = "<p><b>"+ custname1+"</b><br></p><p>Please find the attached bill details for your reference.</p>";
            string sign = "<p>Regards</p><p>For Sanco</p><p>billcfs@sancotrans.com</p><p><img src='"+ mlogo+ "' style='width: 900px; ' data-filename=''><br></p><p><b><br></b></p>";
            emailbody = emailbody + TmpAStr + sign;

            
            var sql = context.Database.SqlQuery<CategoryMaster>("select * from CategoryMaster where CATEID=" + chaid).ToList();
            ViewBag.CATEMAIL = sql[0].CATEMAIL; //"technicalteam@fusiontec.com";
            
            switch (SDPTID) 
            {
                case 1:
                    
                    ImportMailPrintView(REGSTRID, tranmid);
                    
                    break;
                case 2:
                    
                    ExportMailPrintView(REGSTRID, tranmid);
                   
                    break;
                case 9:

                    NonPnrMailPrintView(REGSTRID, tranmid);
                    
                    break;
                case 11:

                    ESealMailPrintView(REGSTRID, tranmid); 
                    
                    break;
            }
            
            
            //var tranmid = 0;
            //var scquery = context.Database.SqlQuery<TransactionMaster>("select * from TransactionMaster where TRANMID=" + id).ToList();
            //if (scquery.Count > 0)
            //{
            //    tranmid = scquery[0].TRANMID;
            //}

            ViewBag.emailmsg = "";
            //0 --new mail 1-replay mail
            string strMultipleFileNames = string.Empty;
            ViewBag.REGSTRID = REGSTRID;
//            if (Convert.ToInt32(REGSTRID) == 0)
            {
                //if (qmbtype == 0)
                //{
                //    ViewBag.emailmsg = emailbody + sign;
                //    //Get Default Files in Infologia
                //    ViewBag.SUB = emailsubject;

                //    DirectoryInfo d = new DirectoryInfo(Server.MapPath("~/Invoice/"));//Assuming Test is your Folder
                //    FileInfo[] DefaultFiles = d.GetFiles(tranmid.ToString() + "*"); //Getting All files
                //    foreach (FileInfo filevalue in DefaultFiles)
                //        strMultipleFileNames += filevalue.Name + ",";


                //    if (strMultipleFileNames != string.Empty)
                //    {
                //        ViewBag.InvoiceFiles = id.ToString() + "_" + custname + ".pdf";
                //        ViewBag.LOGFiles = "";// id.ToString() + "_LOG_" + custname + ".pdf";
                //        ViewBag.SCFiles = id.ToString() + "_SC_" + custname + ".pdf";
                //        ViewBag.DefaultFiles = strMultipleFileNames.Substring(0, strMultipleFileNames.Length - 1);
                //    }
                //}
                //else
                {
                    ViewBag.emailmsg = emailbody;
                    //Get Default Files in Infologia
                    ViewBag.SUB = emailsubject;


                    string path1 = Server.MapPath("~/PDF/" + REGSTRID.ToString() + "_" + Session["CUSRID"] + "/Invoice/") + REGSTRID.ToString() + "_" + id.ToString() + ".pdf";
                    string hl = "<a href='" + path1 + ">" + REGSTRID.ToString() + "_" + id.ToString() + ".pdf" + "</a>";
                    ViewBag.InvoiceFiles =  REGSTRID.ToString() + "_" + id.ToString() + ".pdf"; //+ "_" + custname //hl;//

                    ViewBag.InvoiceFilesLink = path1;

                }
            }
            //else
            //{
            //    ViewBag.emailmsg = sign;
            //    ViewBag.InvoiceFiles = id.ToString() + "_" + custname + ".pdf";
            //    var result = context.Database.SqlQuery<USP_GET_EMAILREPLY_Result>("EXEC USP_GET_EMAILREPLY @refcode = '" + qmdno + "'").ToList();
            //    if (result.Count > 0)
            //    {
            //        ViewBag.SUB = result[0].title;
            //        ViewBag.CATEMAIL = result[0].toemailid;
            //        ViewBag.messageid = result[0].newmessageid;
            //    }
            //}
            return View();
        }

        public ActionResult MPForm(int id)
        {
            //emailinvoicetemplate vm = new emailinvoicetemplate();


            emailinvoicetemplate objemailinvoicetemplate = new emailinvoicetemplate();
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ViewBag.id = id;

            //string custname = "";
            var REGSTRID = 0;
            var SDPTID = 0;
            var tranmid = id;
            var custname = "";
            var custname1 = "";
            var chaid = 0;
            //var qmbtype = 0;
            //string qmdno = "";
            //string qmno = "";
            var query = context.Database.SqlQuery<Performa_TransactionMaster>("select * from Performa_TransactionMaster where tranmid =" + tranmid).ToList();
            
            if (query.Count() != 0)
                {
                    // qmbtype = query[0].QMBTYPE;
                    //qmdno = query[0].QMDNO;
                    //qmno = query[0].QMNO.ToString();
                    REGSTRID = query[0].REGSTRID;
                    SDPTID = query[0].SDPTID;
                    custname = query[0].TRANREFNAME;
                    custname = custname.Replace("&", "");
                    custname = custname.Replace("'", "");
                    custname = custname.Replace(".", "");
                    custname = custname.Replace(",", "");
                    custname1 = query[0].TRANREFNAME;
                    chaid = Convert.ToInt32(query[0].TRANREFID);
                }

            ViewBag.FQMREFNAME = Session["COMPNAME"];
            ViewBag.CATEMAIL = "support@fustiontec.com"; // query[0].QMEMAILID;
            ViewBag.FQMDNO = ""; // qmdno;
            ViewBag.messageid = "";
            ViewBag.REGSTRID = REGSTRID.ToString();
            ViewBag.SDPTID = SDPTID.ToString();
            ViewBag.qmbtype = ""; // qmbtype;
            var sdpttype = "Invoice";
            if (SDPTID == 9)
                sdpttype = "Non PNR";
            else if (SDPTID == 1)
                sdpttype = "Import";
            else if (SDPTID == 2)
                sdpttype = "Export";
            else if (SDPTID == 11)
                sdpttype = "E-Seal";



            string emailsubject = "";
            string emailbody = "";
            emailsubject = "Invoice: " + query[0].TRANDNO;
            var TmpAStr = "<table width='539' border='0' cellpadding='0' cellspacing='0' style='background:#FFCC66;border:1px solid #663300'><tr><Td colspan='2' style='background:#663300;color:#FFFFFF;font-weight:700;text-align:center;padding:5px'>" + sdpttype + "</Td> </tr> <tr>";
            TmpAStr = TmpAStr + " <th style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left'width='208' >Invoice No. </th> <td style='padding:4px;border-bottom:1px solid #663300' width='331'>" + query[0].TRANDNO + "</td> </tr> <tr> <th style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Date </th>";
            TmpAStr = TmpAStr + " <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANDATE + "</td> </tr> <tr> <th style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >Bill Amount </th> <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANNAMT.ToString() + "</td> </tr> <tr> <th style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >CHA Name </th>";
            TmpAStr = TmpAStr + " <td style='padding:4px;border-bottom:1px solid #663300' >" + query[0].TRANREFNAME + "</td> </tr> </table> <br> <br> <table width='539' border='0' cellpadding='0' cellspacing='0' style='background:#FFCC66;border:1px solid #663300 '> <tr> <th style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left'width='208' >" + Session["COMPNAME"] + "</th> </tr> <tr> <th style='padding:4px;border-bottom:1px solid #663300;border-right:1px solid #663300' align='left' >" + Session["COMPADDR"] + "</th>";
            TmpAStr = TmpAStr + " </tr> </table>";
            string mlogo = ConfigurationManager.AppSettings["MailLogo"];
            emailbody = "<p><b>" + custname1 + "</b><br></p><p>Please find the attached bill details for your reference.</p>";
            string sign = "<p>Regards</p><p>For Sanco</p><p>billcfs@sancotrans.com</p><p><img src='" + mlogo+ "' style='width: 900px; ' data-filename=''><br></p><p><b><br></b></p>";
            emailbody = emailbody + TmpAStr + sign;


            var sql = context.Database.SqlQuery<CategoryMaster>("select * from CategoryMaster where CATEID=" + chaid).ToList();
            ViewBag.CATEMAIL = sql[0].CATEMAIL; //"technicalteam@fusiontec.com"; //

            ImportPerformaMailPrintView(REGSTRID, tranmid);


            //var tranmid = 0;
            //var scquery = context.Database.SqlQuery<Performa_TransactionMaster>("select * from Performa_TransactionMaster where TRANMID=" + id).ToList();
            //if (scquery.Count > 0)
            //{
            //    tranmid = scquery[0].TRANMID;
            //}

            ViewBag.emailmsg = "";
            //0 --new mail 1-replay mail
            string strMultipleFileNames = string.Empty;
            ViewBag.REGSTRID = REGSTRID;
            //            if (Convert.ToInt32(REGSTRID) == 0)
            {
                //if (qmbtype == 0)
                //{
                //    ViewBag.emailmsg = emailbody + sign;
                //    //Get Default Files in Infologia
                //    ViewBag.SUB = emailsubject;

                //    DirectoryInfo d = new DirectoryInfo(Server.MapPath("~/Invoice/"));//Assuming Test is your Folder
                //    FileInfo[] DefaultFiles = d.GetFiles(tranmid.ToString() + "*"); //Getting All files
                //    foreach (FileInfo filevalue in DefaultFiles)
                //        strMultipleFileNames += filevalue.Name + ",";


                //    if (strMultipleFileNames != string.Empty)
                //    {
                //        ViewBag.InvoiceFiles = id.ToString() + "_" + custname + ".pdf";
                //        ViewBag.LOGFiles = "";// id.ToString() + "_LOG_" + custname + ".pdf";
                //        ViewBag.SCFiles = id.ToString() + "_SC_" + custname + ".pdf";
                //        ViewBag.DefaultFiles = strMultipleFileNames.Substring(0, strMultipleFileNames.Length - 1);
                //    }
                //}
                //else
                {
                    ViewBag.emailmsg = emailbody;
                    //Get Default Files in Infologia
                    ViewBag.SUB = emailsubject;


                    string path1 = Server.MapPath("~/PDF/" + REGSTRID.ToString() + "_" + Session["CUSRID"] + "/Invoice/") + REGSTRID.ToString() + "_" + id.ToString() + ".pdf";
                    string hl = "<a href='" + path1 + ">" + REGSTRID.ToString() + "_" + id.ToString() + ".pdf" + "</a>";
                    ViewBag.InvoiceFiles = REGSTRID.ToString() + "_" + id.ToString() + ".pdf"; //+ "_" + custname //hl;//

                    //ViewBag.InvoiceFiles.Link = hl;

                }
            }
            //else
            //{
            //    ViewBag.emailmsg = sign;
            //    ViewBag.InvoiceFiles = id.ToString() + "_" + custname + ".pdf";
            //    var result = context.Database.SqlQuery<USP_GET_EMAILREPLY_Result>("EXEC USP_GET_EMAILREPLY @refcode = '" + qmdno + "'").ToList();
            //    if (result.Count > 0)
            //    {
            //        ViewBag.SUB = result[0].title;
            //        ViewBag.CATEMAIL = result[0].toemailid;
            //        ViewBag.messageid = result[0].newmessageid;
            //    }
            //}
            return View();
        }

        //..........................Printview...
        //[Authorize(Roles = "InvoiceMail")]
        public void ImportMailPrintView(int? REGSTRID = 0, int? id = 0)
        {

            var scquery = context.Database.SqlQuery<TransactionMaster>("select * from TransactionMaster where TRANMID=" + id + " AND REGSTRID = " + REGSTRID).ToList();
            var custname = "";
            var tranbtype = 0;
           
            if (scquery.Count > 0)
            {
                custname = scquery[0].TRANREFNAME;
                tranbtype = scquery[0].TRANBTYPE;
                REGSTRID = scquery[0].REGSTRID;
            }

            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "IMPORTINVOICE", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;


                var strPath = ConfigurationManager.AppSettings["Reporturl"];

                var pdfPath = ConfigurationManager.AppSettings["pdfurl"];


                // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");


                //........Get TRANPCOUNT...//
                var Query = context.Database.SqlQuery<int>("select TRANPCOUNT from transactionmaster where TRANMID=" + id + " AND REGSTRID = " + REGSTRID).ToList();
                var PCNT = 0;

                if (Query.Count() != 0) { PCNT = Query[0]; }
                var TRANPCOUNT = ++PCNT;
                // Response.Write(++PCNT);
                // Response.End();

                context.Database.ExecuteSqlCommand("UPDATE transactionmaster SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + id);

                string RptNamePath = "";
                var gsttype = 1;
                var rpttype = 0;
                var billedto = 0;
                //switch (billedto)
                //{
                //    case 1:
                //        if (rpttype == 0)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_rpt_IMP.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_import_Invoice_rpt_imp.RPT"; }
                //        else if (rpttype == 1)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_Group_rpt_IMP.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_import_Invoice_Group_rpt_IMP.RPT"; }

                //        break;

                //    default:

                //        if (rpttype == 0)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_rpt.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_import_Invoice_rpt.RPT"; }
                //        else if (rpttype == 1)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_Group_rpt.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_import_Invoice_Group_rpt.RPT"; }

                //        break;
                //}

                if (REGSTRID == 15 || REGSTRID == 34)
                {
                    cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "GST_Import_OS_Detail_E01.RPT");
                    cryRpt.RecordSelectionFormula = "{VW_IMPORT_OS_DETAIL_RPT_GST.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_IMPORT_OS_DETAIL_RPT_GST.TRANMID} = " + id;
                }
                else
                {
                    cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "GST_Import_Invoice_Rpt_E01.RPT");
                    cryRpt.RecordSelectionFormula = "{VW_IMPORT_TRANSACTION_GST_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_IMPORT_TRANSACTION_GST_PRINT_ASSGN.TRANMID} = " + id;
                }
                 

                //cryRpt.Load(RptNamePath);
                



                string paramName = "@FTHANDLING";
                //strHead
                for (int i = 0; i < cryRpt.DataDefinition.FormulaFields.Count; i++)
                    if (cryRpt.DataDefinition.FormulaFields[i].FormulaName == "{" + paramName + "}")
                        cryRpt.DataDefinition.FormulaFields[i].Text = "'" + "Import Charges" + "'";


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

                string zfilename = REGSTRID.ToString() + "_" + id.ToString() + ".pdf";
                string qmfilename = REGSTRID.ToString() + "_" + id.ToString() + ".pdf"; //+ "_" + custname 

                string path1 = Server.MapPath("~/PDF/" + REGSTRID.ToString() + "_" + Session["CUSRID"] + "/Invoice/"); //Server.MapPath(Session["CUSRID"]+ "/Invoice/");
                string path2 = "~/PDF/" +REGSTRID.ToString()+"_"+ Session["CUSRID"] + "/Invoice/";
                string path3 = Server.MapPath("~/Invoice/");
                //string file1 = path1+id.ToString()+".pdf";
                string file1 = path1 + qmfilename;
                string file3 = path3 + zfilename;
                //string path = pdfPath + "\\" + Session["CUSRID"] + "\\Invoice";
                // string file = path1 + "\\" + id.ToString() + ".pdf";

                if (!(Directory.Exists(path3)))
                {
                    Directory.CreateDirectory(path3);
                }
                    //FileStream s = null;
                    if (!(Directory.Exists(path1)))
                {
                    Directory.CreateDirectory(path1);
                    //s = new FileStream(file1, FileMode.OpenOrCreate);
                    //s.Close();
                    //s.Dispose();
                }
                else
                {
                    //string file = path + "\\" + QMDNO + ".pdf";
                    //if (Directory.Exists(path1))
                    //{
                    //    var directory = new DirectoryInfo(path1);
                    //    foreach (FileInfo file in directory.GetFiles())
                    //    {
                    //        if (!IsFileLocked(file)) file.Delete();
                    //    }
                    //}

                    if (Directory.Exists(Path.GetDirectoryName(file1)))
                    {
                        //if (file1 != "")
                        //{
                        //    using (Attachment dataAttach = new Attachment(file1))
                        //    {
                        //        //msg.Attachments.Add(new Attachment(attachmentPath));
                        //        dataAttach.Dispose();
                        //    }
                        //}
                        //System.IO.FileStream fs;
                        //fs = System.IO.File.Open(file1, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read, System.IO.FileShare.None);
                        //fs.Close();

                        //s = new FileStream(file1, FileMode.OpenOrCreate);
                        //s.Close();
                        //s.Dispose();
                        //FileInfo info = new FileInfo(file1);
                        //info.Delete();
                        //System.IO.File.Delete(imgpath);
                        //FileStream s = new FileStream(file1, FileMode.Truncate);
                        //s.Close();
                        //s.Dispose();
                        //System.GC.Collect();
                        //System.GC.WaitForPendingFinalizers();

                        // System.IO.File.Create(file).Close();
                        System.IO.File.Delete(file1);
                    }
                }



                //cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, path + "\\" + QMDNO + ".pdf");
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, file1);
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, file3);
                Stream stream = cryRpt.ExportToStream(ExportFormatType.PortableDocFormat);
                stream.Close();
                stream.Dispose();
                //cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");

                cryRpt.Close();
                cryRpt.Dispose();
                GC.Collect();
                stringbuilder.Clear();
            }

            
        }
        public void NonPnrMailPrintView(int? REGSTRID = 0, int? id = 0)
        {

            var scquery = context.Database.SqlQuery<TransactionMaster>("select * from TransactionMaster where TRANMID=" + id + " AND REGSTRID = " + REGSTRID).ToList();
            var custname = "";
            if (scquery.Count > 0)
            {
                custname = scquery[0].TRANREFNAME;
            }

            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "NONPNRINVOICE", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;


                var strPath = ConfigurationManager.AppSettings["Reporturl"];

                var pdfPath = ConfigurationManager.AppSettings["pdfurl"];


                // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");


                //........Get TRANPCOUNT...//
                var Query = context.Database.SqlQuery<int>("select TRANPCOUNT from transactionmaster where TRANMID=" + id + " AND REGSTRID = " + REGSTRID).ToList();
                var PCNT = 0;

                if (Query.Count() != 0) { PCNT = Query[0]; }
                var TRANPCOUNT = ++PCNT;
                // Response.Write(++PCNT);
                // Response.End();

                context.Database.ExecuteSqlCommand("UPDATE transactionmaster SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + id);
                string RptNamePath = "";
                var rpttype = 0;
                var gsttype = 1;
                var billedto = 0;
                //switch (billedto)
                //{
                //    case 1:
                //        if (rpttype == 0)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "NonPnr_Invoice_rpt_IMP.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_NonPnr_Invoice_Rpt.RPT"; }
                //        else if (rpttype == 1)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "NonPnr_Invoice_Group_rpt_IMP.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_NonPnr_Invoice_Group_rpt_IMP.RPT"; }

                //        break;

                //    default:

                //        if (rpttype == 0)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "NonPnr_Invoice_rpt.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_NonPnr_Invoice_Rpt.RPT"; }
                //        else if (rpttype == 1)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "NonPnr_Invoice_Group_rpt.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_NonPnr_Invoice_Group_rpt.RPT"; }

                //        break;
                //}

                 cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "GST_NonPnr_Invoice_Rpt_E01.RPT");

               
                cryRpt.RecordSelectionFormula = "{VW_TRANSACTION_NONPNR_CRY_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_TRANSACTION_NONPNR_CRY_PRINT_ASSGN.TRANMID} = " + id;



                string paramName = "@FTHANDLING";
                //strHead
                for (int i = 0; i < cryRpt.DataDefinition.FormulaFields.Count; i++)
                    if (cryRpt.DataDefinition.FormulaFields[i].FormulaName == "{" + paramName + "}")
                        cryRpt.DataDefinition.FormulaFields[i].Text = "'" + "Non PNR Charges" + "'";


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

                string zfilename = REGSTRID.ToString() + "_" + id.ToString() + ".pdf";
                string qmfilename = REGSTRID.ToString() + "_" + id.ToString() + ".pdf"; //+ "_" + custname 

                string path1 = Server.MapPath("~/PDF/" + REGSTRID.ToString() + "_" + Session["CUSRID"] + "/Invoice/"); //Server.MapPath(Session["CUSRID"]+ "/Invoice/");
                string path2 = "~/PDF/" + REGSTRID.ToString() + "_" + Session["CUSRID"] + "/Invoice/";
                string path3 = Server.MapPath("~/Invoice/");
                //string file1 = path1+id.ToString()+".pdf";
                string file1 = path1 + qmfilename;
                string file3 = path3 + zfilename;
                //string path = pdfPath + "\\" + Session["CUSRID"] + "\\Invoice";
                // string file = path1 + "\\" + id.ToString() + ".pdf";


                //FileStream s = null;
                if (!(Directory.Exists(path1)))
                {
                    Directory.CreateDirectory(path1);
                    //s = new FileStream(file1, FileMode.OpenOrCreate);
                    //s.Close();
                    //s.Dispose();
                }
                else
                {
                    //string file = path + "\\" + QMDNO + ".pdf";
                    //if (Directory.Exists(path1))
                    //{
                    //    var directory = new DirectoryInfo(path1);
                    //    foreach (FileInfo file in directory.GetFiles())
                    //    {
                    //        if (!IsFileLocked(file)) file.Delete();
                    //    }
                    //}

                    if (Directory.Exists(Path.GetDirectoryName(file1)))
                    {
                        //if (file1 != "")
                        //{
                        //    using (Attachment dataAttach = new Attachment(file1))
                        //    {
                        //        //msg.Attachments.Add(new Attachment(attachmentPath));
                        //        dataAttach.Dispose();
                        //    }
                        //}
                        //System.IO.FileStream fs;
                        //fs = System.IO.File.Open(file1, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read, System.IO.FileShare.None);
                        //fs.Close();

                        //s = new FileStream(file1, FileMode.OpenOrCreate);
                        //s.Close();
                        //s.Dispose();
                        //FileInfo info = new FileInfo(file1);
                        //info.Delete();
                        //System.IO.File.Delete(imgpath);
                        //FileStream s = new FileStream(file1, FileMode.Truncate);
                        //s.Close();
                        //s.Dispose();
                        //System.GC.Collect();
                        //System.GC.WaitForPendingFinalizers();

                        // System.IO.File.Create(file).Close();
                        System.IO.File.Delete(file1);
                    }
                }



                //cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, path + "\\" + QMDNO + ".pdf");
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, file1);
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, file3);
                Stream stream = cryRpt.ExportToStream(ExportFormatType.PortableDocFormat);
                stream.Close();
                stream.Dispose();
                //cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                cryRpt.Close();
                cryRpt.Dispose();
                GC.Collect();
                stringbuilder.Clear();
            }


        }

        public void ESealMailPrintView(int? REGSTRID = 0, int? id = 0)
        {

            var scquery = context.Database.SqlQuery<TransactionMaster>("select * from TransactionMaster where TRANMID=" + id + " AND REGSTRID = " + REGSTRID).ToList();
            var custname = "";
            var tranbtype = 0;
            if (scquery.Count > 0)
            {
                custname = scquery[0].TRANREFNAME;
                tranbtype = scquery[0].TRANBTYPE;
                REGSTRID = scquery[0].REGSTRID;
            }

            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "SEALINVOICE", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;


                var strPath = ConfigurationManager.AppSettings["Reporturl"];

                var pdfPath = ConfigurationManager.AppSettings["pdfurl"];


                // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");


                //........Get TRANPCOUNT...//
                var Query = context.Database.SqlQuery<int>("select TRANPCOUNT from transactionmaster where TRANMID=" + id).ToList();
                var PCNT = 0;

                if (Query.Count() != 0) { PCNT = Query[0]; }
                var TRANPCOUNT = ++PCNT;
                // Response.Write(++PCNT);
                // Response.End();

                context.Database.ExecuteSqlCommand("UPDATE transactionmaster SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + id);

                
                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "GST_ESeal_Invoice_E01.RPT");
                cryRpt.RecordSelectionFormula = "{VW_EXPORT_STUFF_INVOICE_PRINT.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_STUFF_INVOICE_PRINT.TRANMID} = " + id;
                
                string paramName = "@FTHANDLING";
                //strHead
                for (int i = 0; i < cryRpt.DataDefinition.FormulaFields.Count; i++)
                    if (cryRpt.DataDefinition.FormulaFields[i].FormulaName == "{" + paramName + "}")
                        cryRpt.DataDefinition.FormulaFields[i].Text = "'" + "ESeal Bill Charges" + "'";


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

                string zfilename = REGSTRID.ToString() + "_" + id.ToString() + ".pdf";
                string qmfilename = REGSTRID.ToString() + "_" + id.ToString() + ".pdf"; //"_" + custname + 
                //string path = "D:\\KGK\\" + Session["CUSRID"] + "\\Invoice";
                string path1 = Server.MapPath("~/PDF/" + REGSTRID.ToString() + "_" + Session["CUSRID"] + "/Invoice/"); //Server.MapPath(Session["CUSRID"]+ "/Invoice/");
                string path2 = "~/PDF/" + REGSTRID.ToString() + "_" + Session["CUSRID"] + "/Invoice/";
                string path3 = Server.MapPath("~/Invoice/");
                //string file1 = path1+id.ToString()+".pdf";
                string file1 = path1 + qmfilename;
                string file3 = path3 + zfilename;
                //string path = pdfPath + "\\" + Session["CUSRID"] + "\\Invoice";
                // string file = path1 + "\\" + id.ToString() + ".pdf";


                //FileStream s = null;
                if (!(Directory.Exists(path1)))
                {
                    Directory.CreateDirectory(path1);
                    //s = new FileStream(file1, FileMode.OpenOrCreate);
                    //s.Close();
                    //s.Dispose();
                }
                else
                {
                    //string file = path + "\\" + QMDNO + ".pdf";
                    //if (Directory.Exists(path1))
                    //{
                    //    var directory = new DirectoryInfo(path1);
                    //    foreach (FileInfo file in directory.GetFiles())
                    //    {
                    //        if (!IsFileLocked(file)) file.Delete();
                    //    }
                    //}

                    if (Directory.Exists(Path.GetDirectoryName(file1)))
                    {
                        //if (file1 != "")
                        //{
                        //    using (Attachment dataAttach = new Attachment(file1))
                        //    {
                        //        //msg.Attachments.Add(new Attachment(attachmentPath));
                        //        dataAttach.Dispose();
                        //    }
                        //}
                        //System.IO.FileStream fs;
                        //fs = System.IO.File.Open(file1, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read, System.IO.FileShare.None);
                        //fs.Close();

                        //s = new FileStream(file1, FileMode.OpenOrCreate);
                        //s.Close();
                        //s.Dispose();
                        //FileInfo info = new FileInfo(file1);
                        //info.Delete();
                        //System.IO.File.Delete(imgpath);
                        //FileStream s = new FileStream(file1, FileMode.Truncate);
                        //s.Close();
                        //s.Dispose();
                        //System.GC.Collect();
                        //System.GC.WaitForPendingFinalizers();

                        // System.IO.File.Create(file).Close();
                        System.IO.File.Delete(file1);
                    }

                }



                //cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, path + "\\" + QMDNO + ".pdf");
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, file1);
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, file3);
                Stream stream = cryRpt.ExportToStream(ExportFormatType.PortableDocFormat);
                stream.Close();
                stream.Dispose();
                //cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                cryRpt.Close();
                cryRpt.Dispose();
                GC.Collect();
                stringbuilder.Clear();
            }


        }

        public void ExportMailPrintView(int? REGSTRID = 0, int? id = 0)
        {

            var scquery = context.Database.SqlQuery<TransactionMaster>("select * from TransactionMaster where TRANMID=" + id + " AND REGSTRID = " + REGSTRID).ToList();
            var custname = "";
            var tranbtype = 0;
            if (scquery.Count > 0)
            {
                custname = scquery[0].TRANREFNAME;
                tranbtype = scquery[0].TRANBTYPE;
                REGSTRID = scquery[0].REGSTRID;
            }

            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "SEALINVOICE", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;


                var strPath = ConfigurationManager.AppSettings["Reporturl"];

                var pdfPath = ConfigurationManager.AppSettings["pdfurl"];


                // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");


                //........Get TRANPCOUNT...//
                var Query = context.Database.SqlQuery<int>("select TRANPCOUNT from transactionmaster where TRANMID=" + id).ToList();
                var PCNT = 0;

                if (Query.Count() != 0) { PCNT = Query[0]; }
                var TRANPCOUNT = ++PCNT;
                // Response.Write(++PCNT);
                // Response.End();

                context.Database.ExecuteSqlCommand("UPDATE transactionmaster SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + id);

                string RptNamePath = "";
                var gsttype = 1;
                var rpttype = 0;
                var billedto = 0;
                if (REGSTRID == 21 || REGSTRID == 22 || REGSTRID == 23)
                {
                    cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "GST_EXPORT_OS_Detail.RPT");
                    cryRpt.RecordSelectionFormula = "{VW_EXPORT_OS_DETAIL_RPT.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_OS_DETAIL_RPT.TRANMID} = " + id;

                }
                else if(REGSTRID == 46 || REGSTRID == 47 || REGSTRID == 48)
                {
                    cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "GST_BS_Export_Stuff_Invoice_E01.RPT");
                    cryRpt.RecordSelectionFormula = "{VW_EXPORT_STUFF_INVOICE_PRINT.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_STUFF_INVOICE_PRINT.TRANMID} = " + id;
                    
                }                
                else
                {
                    if (tranbtype == 1)
                    {
                        cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Export_Stuff_Invoice_E01.RPT");

                        //cryRpt.Load(RptNamePath);


                    }
                    else
                    {
                        cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Export_Seal_Invoice_E01.RPT");

                        //cryRpt.Load(RptNamePath);


                    }
                    cryRpt.RecordSelectionFormula = "{VW_EXPORT_STUFF_INVOICE_PRINT.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_STUFF_INVOICE_PRINT.TRANMID} = " + id;
                }



                string paramName = "@FTHANDLING";
                //strHead
                for (int i = 0; i < cryRpt.DataDefinition.FormulaFields.Count; i++)
                    if (cryRpt.DataDefinition.FormulaFields[i].FormulaName == "{" + paramName + "}")
                        cryRpt.DataDefinition.FormulaFields[i].Text = "'" + "Bill Charges" + "'";


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

                string zfilename = REGSTRID.ToString() + "_" + id.ToString() + ".pdf";
                string qmfilename = REGSTRID.ToString() + "_" + id.ToString() + ".pdf"; //"_" + custname + 
                //string path = "D:\\KGK\\" + Session["CUSRID"] + "\\Invoice";
                string path1 = Server.MapPath("~/PDF/" + REGSTRID.ToString() + "_" + Session["CUSRID"] + "/Invoice/"); //Server.MapPath(Session["CUSRID"]+ "/Invoice/");
                string path2 = "~/PDF/" + REGSTRID.ToString() + "_" + Session["CUSRID"] + "/Invoice/";
                string path3 = Server.MapPath("~/Invoice/");
                //string file1 = path1+id.ToString()+".pdf";
                string file1 = path1 + qmfilename;
                string file3 = path3 + zfilename;
                //string path = pdfPath + "\\" + Session["CUSRID"] + "\\Invoice";
                // string file = path1 + "\\" + id.ToString() + ".pdf";


                //FileStream s = null;
                if (!(Directory.Exists(path1)))
                {
                    Directory.CreateDirectory(path1);
                    //s = new FileStream(file1, FileMode.OpenOrCreate);
                    //s.Close();
                    //s.Dispose();
                }
                else
                {
                    //string file = path + "\\" + QMDNO + ".pdf";
                    //if (Directory.Exists(path1))
                    //{
                    //    var directory = new DirectoryInfo(path1);
                    //    foreach (FileInfo file in directory.GetFiles())
                    //    {
                    //        if (!IsFileLocked(file)) file.Delete();
                    //    }
                    //}

                    if (Directory.Exists(Path.GetDirectoryName(file1)))
                    {
                        //if (file1 != "")
                        //{
                        //    using (Attachment dataAttach = new Attachment(file1))
                        //    {
                        //        //msg.Attachments.Add(new Attachment(attachmentPath));
                        //        dataAttach.Dispose();
                        //    }
                        //}
                        //System.IO.FileStream fs;
                        //fs = System.IO.File.Open(file1, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read, System.IO.FileShare.None);
                        //fs.Close();

                        //s = new FileStream(file1, FileMode.OpenOrCreate);
                        //s.Close();
                        //s.Dispose();
                        //FileInfo info = new FileInfo(file1);
                        //info.Delete();
                        //System.IO.File.Delete(imgpath);
                        //FileStream s = new FileStream(file1, FileMode.Truncate);
                        //s.Close();
                        //s.Dispose();
                        //System.GC.Collect();
                        //System.GC.WaitForPendingFinalizers();

                        // System.IO.File.Create(file).Close();
                        System.IO.File.Delete(file1);
                    }
                }



                //cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, path + "\\" + QMDNO + ".pdf");
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, file1);
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, file3);
                Stream stream = cryRpt.ExportToStream(ExportFormatType.PortableDocFormat);
                stream.Close();
                stream.Dispose();
                //cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                cryRpt.Close();
                cryRpt.Dispose();
                GC.Collect();
                stringbuilder.Clear();
            }


        }


        public void ImportPerformaMailPrintView(int? REGSTRID = 0, int? id = 0)
        {

            var scquery = context.Database.SqlQuery<Performa_TransactionMaster>("select * from Performa_TransactionMaster where TRANMID=" + id + " AND REGSTRID = " + REGSTRID).ToList();
            var custname = "";
            var tranbtype = 0;

            if (scquery.Count > 0)
            {
                custname = scquery[0].TRANREFNAME;
                tranbtype = scquery[0].TRANBTYPE;
                REGSTRID = scquery[0].REGSTRID;
            }

            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "IMPORTINVOICE", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;


                var strPath = ConfigurationManager.AppSettings["Reporturl"];

                var pdfPath = ConfigurationManager.AppSettings["pdfurl"];


                // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");


                //........Get TRANPCOUNT...//
                var Query = context.Database.SqlQuery<int>("select TRANPCOUNT from Performa_TransactionMaster where TRANMID=" + id + " AND REGSTRID = " + REGSTRID).ToList();
                var PCNT = 0;

                if (Query.Count() != 0) { PCNT = Query[0]; }
                var TRANPCOUNT = ++PCNT;
                // Response.Write(++PCNT);
                // Response.End();

                context.Database.ExecuteSqlCommand("UPDATE Performa_TransactionMaster SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + id);

                string RptNamePath = "";
                var gsttype = 1;
                var rpttype = 0;
                var billedto = 0;
                //switch (billedto)
                //{
                //    case 1:
                //        if (rpttype == 0)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_rpt_IMP.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_import_Invoice_rpt_imp.RPT"; }
                //        else if (rpttype == 1)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_Group_rpt_IMP.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_import_Invoice_Group_rpt_IMP.RPT"; }

                //        break;

                //    default:

                //        if (rpttype == 0)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_rpt.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_import_Invoice_rpt.RPT"; }
                //        else if (rpttype == 1)
                //            if (gsttype == 0) { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "import_Invoice_Group_rpt.RPT"; } else { RptNamePath = ConfigurationManager.AppSettings["Reporturl"] + "GST_import_Invoice_Group_rpt.RPT"; }

                //        break;
                //}

                if (REGSTRID == 15 || REGSTRID == 34)
                {
                    cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "GST_IMPORT_PERFORMA_OS_Detail_E01.RPT");
                    cryRpt.RecordSelectionFormula = "{VW_IMPORT_PERFORMA_OS_DETAIL_RPT_GST.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_IMPORT_OS_DETAIL_RPT_GST.TRANMID} = " + id;
                }
                else
                {
                    cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "GST_import_Performa_Invoice_rpt_E01.RPT");
                    cryRpt.RecordSelectionFormula = "{VW_IMPORT_PERFORMA_TRANSACTION_GST_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_IMPORT_TRANSACTION_GST_PRINT_ASSGN.TRANMID} = " + id;
                }


                cryRpt.Load(RptNamePath);




                string paramName = "@FTHANDLING";
                //strHead
                for (int i = 0; i < cryRpt.DataDefinition.FormulaFields.Count; i++)
                    if (cryRpt.DataDefinition.FormulaFields[i].FormulaName == "{" + paramName + "}")
                        cryRpt.DataDefinition.FormulaFields[i].Text = "'" + "Performa" + "'";


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

                string zfilename = REGSTRID.ToString() + "_" + id.ToString() + ".pdf";
                string qmfilename = REGSTRID.ToString() + "_" + id.ToString() + ".pdf"; //+ "_" + custname 

                string path1 = Server.MapPath("~/PDF/" + REGSTRID.ToString() + "_" + Session["CUSRID"] + "/Invoice/"); //Server.MapPath(Session["CUSRID"]+ "/Invoice/");
                string path2 = "~/PDF/" + REGSTRID.ToString() + "_" + Session["CUSRID"] + "/Invoice/";
                string path3 = Server.MapPath("~/Invoice/");
                //string file1 = path1+id.ToString()+".pdf";
                string file1 = path1 + qmfilename;
                string file3 = path3 + zfilename;
                //string path = pdfPath + "\\" + Session["CUSRID"] + "\\Invoice";
                // string file = path1 + "\\" + id.ToString() + ".pdf";


                //FileStream s = null;
                if (!(Directory.Exists(path1)))
                {
                    Directory.CreateDirectory(path1);
                    //s = new FileStream(file1, FileMode.OpenOrCreate);
                    //s.Close();
                    //s.Dispose();
                }
                else
                {
                    //string file = path + "\\" + QMDNO + ".pdf";
                    //if (Directory.Exists(path1))
                    //{
                    //    var directory = new DirectoryInfo(path1);
                    //    foreach (FileInfo file in directory.GetFiles())
                    //    {
                    //        if (!IsFileLocked(file)) file.Delete();
                    //    }
                    //}

                    if (Directory.Exists(Path.GetDirectoryName(file1)))
                    {
                        //if (file1 != "")
                        //{
                        //    using (Attachment dataAttach = new Attachment(file1))
                        //    {
                        //        //msg.Attachments.Add(new Attachment(attachmentPath));
                        //        dataAttach.Dispose();
                        //    }
                        //}
                        //System.IO.FileStream fs;
                        //fs = System.IO.File.Open(file1, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read, System.IO.FileShare.None);
                        //fs.Close();

                        //s = new FileStream(file1, FileMode.OpenOrCreate);
                        //s.Close();
                        //s.Dispose();
                        //FileInfo info = new FileInfo(file1);
                        //info.Delete();
                        //System.IO.File.Delete(imgpath);
                        //FileStream s = new FileStream(file1, FileMode.Truncate);
                        //s.Close();
                        //s.Dispose();
                        //System.GC.Collect();
                        //System.GC.WaitForPendingFinalizers();

                        // System.IO.File.Create(file).Close();
                        System.IO.File.Delete(file1);
                    }
                }



                //cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, path + "\\" + QMDNO + ".pdf");
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, file1);
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, file3);
                Stream stream = cryRpt.ExportToStream(ExportFormatType.PortableDocFormat);
                stream.Close();
                stream.Dispose();
                //cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                cryRpt.Close();
                cryRpt.Dispose();
                GC.Collect();
                stringbuilder.Clear();
            }


        }
        [ValidateInput(false)]
        public void SendMail(FormCollection mysbfrm, HttpPostedFileBase[] file)
        {
            try
            {
                Session["MailMsg"] = "";
                string custname = mysbfrm["FQMREFNAME"].ToString();
                int REGSTRID = Convert.ToInt32( mysbfrm["REGSTRID"]);
                string strMultipleFileNames = string.Empty;
                //Attachment Files
                if (file != null)
                {
                    foreach (HttpPostedFileBase fileimg in file)
                    {
                        if (fileimg != null)
                        {
                            string path1 = Server.MapPath("~/Attachment/");

                            if (!(Directory.Exists(path1)))
                            {
                                Directory.CreateDirectory(path1);
                            }
                            string strAttachmentFullPath = "~/Attachment/" + Path.GetFileName(fileimg.FileName);
                            string AttachmentPath = Server.MapPath(strAttachmentFullPath);
                            fileimg.SaveAs(AttachmentPath);
                            strMultipleFileNames += strAttachmentFullPath + ",";
                        }
                    }
                    if (strMultipleFileNames != string.Empty)
                        strMultipleFileNames = strMultipleFileNames.Substring(0, strMultipleFileNames.Length - 1);
                }
                //Default Files
                FileInfo[] DefaultFiles = null;
                //if (mysbfrm["REGSTRID"].ToString() == "0")
                //{
                //    if (mysbfrm["DefaultFiles"] != null)
                //    {
                //        DirectoryInfo d = new DirectoryInfo(Server.MapPath("~/Defaultfile/"));//Assuming Test is your Folder
                //        DefaultFiles = d.GetFiles("*");
                //        foreach (FileInfo filevalue in DefaultFiles)
                //        {
                //            if (mysbfrm["DefaultFiles"].ToString().ToLower().Contains(filevalue.Name.ToLower()))
                //            {
                //                string strDefaultSoureFullPath = "~/Defaultfile/" + filevalue.Name;
                //                string DefaultSourePath = Server.MapPath(strDefaultSoureFullPath);
                //                if (System.IO.File.Exists(DefaultSourePath))
                //                    strMultipleFileNames += strDefaultSoureFullPath + ",";
                //            }
                //        }


                //        if (strMultipleFileNames != string.Empty)
                //            strMultipleFileNames = strMultipleFileNames.Substring(0, strMultipleFileNames.Length - 1);
                //    }
                //}

                var qmbtype = 0;// Convert.ToInt32(mysbfrm["qmbtype"]);
                var pdfPath = Server.MapPath("~/PDF/");// ConfigurationManager.AppSettings["pdfurl"];
                string path = pdfPath + "\\" + REGSTRID.ToString()+"_"+ Session["CUSRID"] + "\\Invoice";
                string qmfilename = REGSTRID.ToString() + "_" + mysbfrm["FTRANMID"] + ".pdf"; //+ "_" + custname 
                string logfilename = ""; // REGSTRID.ToString() + "_" + mysbfrm["FTRANMID"] + "_LOG_"+ ".pdf";// + custname 
                string scfilename = "";// REGSTRID.ToString() + "_" + mysbfrm["FTRANMID"] + "_SC_" + ".pdf";//custname + 

                //string strInvoiceFullPath =  "~/PDF/" + Session["CUSRID"] + "/Invoice/" + mysbfrm["FTRANMID"] + ".pdf";
                string strInvoiceFullPath = "~/PDF/" + REGSTRID.ToString() + "_"+ Session["CUSRID"] + "/Invoice/" + qmfilename;
                string strLOGFullPath = "";// "~/PDF/" + Session["CUSRID"] + "/Invoice/" + logfilename;
                string strSCFullPath = "";// "~/PDF/" + Session["CUSRID"] + "/Invoice/" + logfilename;
                //string strInvoiceFullPath = path + "\\" + mysbfrm["FTRANMID"] + ".pdf";
                string InvoicePath = Server.MapPath(strInvoiceFullPath);

                if (System.IO.File.Exists(InvoicePath))
                {
                    if (qmbtype == 0)
                    {
                        strLOGFullPath = "";// "~/PDF/" + Session["CUSRID"] + "/Invoice/" + logfilename;
                        strSCFullPath = "";// "~/PDF/" + Session["CUSRID"] + "/Invoice/" + scfilename;
                        strMultipleFileNames += "," + strInvoiceFullPath + "," + strLOGFullPath + ",";
                    }
                    else
                    {
                        strMultipleFileNames += "," + strInvoiceFullPath + ",";
                    }

                }


                if (strMultipleFileNames != string.Empty)
                    strMultipleFileNames = strMultipleFileNames.Substring(0, strMultipleFileNames.Length - 1);

                // Random generator = new Random();
                //string r = generator.Next(0, 1000000).ToString("D6");

                int qmid = Convert.ToInt32(mysbfrm["FTRANMID"]);
                
                string refcode = mysbfrm["FQMDNO"].ToString();

                string subject = mysbfrm["TMPSUB"].ToString();
                string emailbody = mysbfrm["emailbody"].ToString();
                string toemailid = mysbfrm["CATEMAIL"].ToString();
                string CATEMAILCC = mysbfrm["CATEMAILCC"].ToString();
                string CATEMAILBCC = mysbfrm["CATEMAILBCC"].ToString();

                //string str_SqlQuery = "USP_INSERT_EMAILSENDING @title = '" + subject + "',@description = '" + emailbody + "',@emailid = '" + fromemailid + "',@refcode = '" + refcode + "',@CC = '" + CATEMAILCC + "',@Attachments = '" + strMultipleFileNames + "',@emailstatus = 1,@toemailid = '" + toemailid + "'";

                //context.Database.ExecuteSqlCommand(str_SqlQuery);
                CommonEmailFunction cf = new CommonEmailFunction();
                string email_fun = string.Empty;
                Session["SMTPNAME"]=ConfigurationManager.AppSettings["SMTPHost"];
                Session["SMTPPORT"] = ConfigurationManager.AppSettings["SMTPPort"];
                Session["SMTPEnableSsl"] = ConfigurationManager.AppSettings["SMTPEnableSsl"];
                Session["SMTPCredentialsUser"] = ConfigurationManager.AppSettings["SMTPCredentialsUser"];
                Session["SMTPCredentialsPassword"] = ConfigurationManager.AppSettings["SMTPCredentialsPassword"];
                Session["SMTPCredentialsUser"] = ConfigurationManager.AppSettings["SMTPCredentialsUser"];
                Session["FromMailIDs"] = ConfigurationManager.AppSettings["FromMailIDs"];
                Session["FromMailDisplays"] = ConfigurationManager.AppSettings["FromMailDisplays"];
                Session["BCCMailIDs"] = ConfigurationManager.AppSettings["BCCMailIDs"];
                Session["BCCMailDisplays"] = ConfigurationManager.AppSettings["BCCMailDisplays"];
                string fromemailid = Session["FromMailIDs"].ToString();
                CATEMAILBCC = Session["BCCMailIDs"].ToString();

                if (mysbfrm["SDPTID"].ToString() == "9" || mysbfrm["SDPTID"].ToString() == "1" || mysbfrm["SDPTID"].ToString() == "2" || mysbfrm["SDPTID"].ToString() == "11")
                {
                    email_fun = cf.PasswordRecovery(Convert.ToInt32(qmbtype), toemailid, "registration", subject, emailbody, "User", CATEMAILCC, CATEMAILBCC, file, DefaultFiles, strInvoiceFullPath, strLOGFullPath, strSCFullPath);
                }
                else
                {
                    if (mysbfrm["messageid"].ToString() != "")
                    {
                        email_fun = cf.Replyemail(toemailid, "registration", subject, emailbody, "User", CATEMAILCC, file, mysbfrm["messageid"].ToString(), strInvoiceFullPath);
                    }
                    else
                    {
                        email_fun = cf.PasswordRecovery(Convert.ToInt32(qmbtype), toemailid, "registration", subject, emailbody, "User", CATEMAILCC, CATEMAILBCC,file, DefaultFiles, strInvoiceFullPath, strLOGFullPath, strSCFullPath);
                    }

                }
                if (email_fun == "success")
                {
                    //if (strMultipleFileNames != "")
                    //{
                    //    using (Attachment dataAttach = new Attachment(InvoicePath))
                    //    {
                    //        //msg.Attachments.Add(new Attachment(attachmentPath));
                    //        dataAttach.Dispose();
                    //    }
                    //}

                    //FileStream s = null;
                    //s = new FileStream(strInvoiceFullPath, FileMode.OpenOrCreate);
                    //s.Close();
                    //s.Dispose();

                    //context.Database.ExecuteSqlCommand("UPDATE TransactionMaster SET QM_EMAIL_STATSUS = 1 WHERE QMID=" + qmid);
                    ViewBag.msg = "success";
                    Session["MailMsg"] = "Mail has been Sent Successfully...";
                    //Response.Write("<script>alert('Mail has been Sent Successfully...');</script>");
                    //Response.Write("<script>window.close();</script>");
                    //Response.Redirect("close.html");
                    ModelState.Clear();
                    //Response.Write("<script>alert('Mail has been Sent Successfully...');</script>");
                    //Response.Write("<script>open(history.go(-2), '_self').close();</script>");
                     Response.Write("<script>window.location.replace('/MailForm/Index?msg=suc');</script>"); //works great
                    //Response.Redirect("~/Invoice/Index");
                }


                //Response.Write("<script>alert('Mail has been Sent Successfully...');</script>");
                //Response.Write("<script>window.close();</script>");
                //Response.Redirect("~/Invoice/Index");
                //Server.Transfer("~/Invoice/Index");
            }
            catch (Exception ex)
            {
                //Response.Write("<script>alert('"+ ex.ToString()+"');</script>");
                Response.Write(ex.ToString());
            }
        }

        [ValidateInput(false)]
        public void ReplyAllMail(FormCollection mysbfrm, HttpPostedFileBase[] file)
        {
            try
            {
                string strMultipleFileNames = string.Empty;
                //Attachment Files
                if (file != null)
                {
                    foreach (HttpPostedFileBase fileimg in file)
                    {
                        if (fileimg != null)
                        {
                            string strAttachmentFullPath = "~/Attachment/" + Path.GetFileName(fileimg.FileName);
                            string AttachmentPath = Server.MapPath(strAttachmentFullPath);
                            fileimg.SaveAs(AttachmentPath);
                            strMultipleFileNames += strAttachmentFullPath + ",";
                        }
                    }
                    if (strMultipleFileNames != string.Empty)
                        strMultipleFileNames = strMultipleFileNames.Substring(0, strMultipleFileNames.Length - 1);
                }

                var pdfPath = ConfigurationManager.AppSettings["pdfurl"];
                string path = pdfPath + "\\" + Session["CUSRID"] + "\\Invoice";

                //string strInvoiceFullPath = "~/Invoice/" + mysbfrm["FTRANMID"] + ".pdf";
                string strInvoiceFullPath = "~/PDF/" + Session["CUSRID"] + "/Invoice/" + mysbfrm["FTRANMID"] + ".pdf";
                //string strInvoiceFullPath = path + "\\" + mysbfrm["FTRANMID"] + ".pdf";
                string InvoicePath = Server.MapPath(strInvoiceFullPath);

                if (System.IO.File.Exists(InvoicePath))
                {
                    strMultipleFileNames += "," + strInvoiceFullPath + ",";
                }


                if (strMultipleFileNames != string.Empty)
                    strMultipleFileNames = strMultipleFileNames.Substring(0, strMultipleFileNames.Length - 1);

                // Random generator = new Random();
                //string r = generator.Next(0, 1000000).ToString("D6");

                int qmid = Convert.ToInt32(mysbfrm["FTRANMID"]);
                string fromemailid = Session["USEREMAIL"].ToString();
                string refcode = mysbfrm["FQMDNO"].ToString();

                string subject = mysbfrm["TMPSUB"].ToString();
                string emailbody = mysbfrm["emailbody"].ToString();
                string toemailid = mysbfrm["CATEMAIL"].ToString();
                string CATEMAILCC = mysbfrm["CATEMAILCC"].ToString();
                string str_SqlQuery = "USP_INSERT_EMAILSENDING @title = '" + subject + "',@description = '" + emailbody + "',@emailid = '" + fromemailid + "',@refcode = '" + refcode + "',@CC = '" + CATEMAILCC + "',@Attachments = '" + strMultipleFileNames + "',@emailstatus = 1,@toemailid = '" + toemailid + "'";

                context.Database.ExecuteSqlCommand(str_SqlQuery);
                CommonEmailFunction cf = new CommonEmailFunction();
                string email_fun = string.Empty;
                email_fun = cf.Replyallemail(toemailid, "registration", subject, emailbody, "User", CATEMAILCC, file, mysbfrm["messageid"].ToString(), strInvoiceFullPath);

                if (email_fun == "success")
                {
                    ViewBag.msg = "success";
                    ModelState.Clear();
                }

                context.Database.ExecuteSqlCommand("UPDATE TransactionMaster SET QM_EMAIL_STATSUS = 1 WHERE QMID=" + qmid);

                Response.Write("<script>alert('Mail has been Sent Successfully...');</script>");
                Response.Write("<script>window.close();</script>");
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }
}