using ClosedXML.Excel;
using scfs.Data;
using scfs_erp;
using scfs_erp.Context;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs.Controllers.Search
{
    [SessionExpire]
    public class HandlingContainerCountController : Controller
    {
        // GET: HandlingContainerCount
        SCFSERPContext context = new SCFSERPContext();
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
            return View();
        }

        public void showrpt()
        {

            int etype = Convert.ToInt32(Request.Form.Get("etype"));

            var afromDate = Request.Form.Get("from").Split('-');
            var fromDate = afromDate[2] + "-" + afromDate[1] + "-" + afromDate[0];

            var atoDate = Request.Form.Get("to").Split('-');
            var toDate = atoDate[2] + "-" + atoDate[1] + "-" + atoDate[0];

            var SDATE = afromDate[0] + "-" + afromDate[1] + "-" + afromDate[2];
            var EDATE = atoDate[0] + "-" + atoDate[1] + "-" + atoDate[2];

            var date1 = Convert.ToDateTime(SDATE).ToString("dd-MMM-yyyy");
            var date2 = Convert.ToDateTime(EDATE).ToString("dd-MMM-yyyy");


            string filename = "";
            string query = "";
            string spname = "";
            string tblname = "";
            string strhead = "";
            switch (etype)
            {
                case 0:
                    tblname = "Consolidated";
                    spname = "pr_Import_Export_Handling_Containers_Count_Assgn";
                    strhead = "Handling Container Count Details From " + date1 + " Till " + date2;
                    query = "exec [pr_Import_Export_Handling_Containers_Count_Assgn] @PSDate = '" + date1 + "' , @PEDate = '" + date2 + "'";
                    filename = "Handling_Containers_Count_Details_" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    break;
                case 1:
                    tblname = "Detailed";
                    spname = "pr_Import_Export_Handling_Containers_Count_Assgn_All";
                    strhead = "Details From " + date1 + " Till " + date2;
                    query = "exec [pr_Import_Export_Handling_Containers_Count_Assgn_All] @PSDate = '" + date1 + "' , @PEDate = '" + date2 + "'";
                    filename = "Import_Export_Handling_Containers_Count_Details_" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    break;
            }

            filename = filename + ".xlsx";

            DataTable dt0 = new DataTable();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            DataTable dt4 = new DataTable();
            DataTable dt5 = new DataTable();
            DataTable dt6 = new DataTable();

            DataTable dt7 = new DataTable();
            DataTable dt8 = new DataTable();

            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnection con = new SqlConnection(constring);

            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds, spname);
            //da.Fill(dt);
            con.Close();

            int crows = 0;
            int lrow = 0;

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(tblname);

                string compname = "";
                string range = "";
                switch (etype)
                {
                    case 0:

                        compname = Session["COMPNAME"].ToString();

                        ws.Cell(1, 1).Value = "" + compname.ToString();
                        ws.Cell(2, 1).Value = "" + strhead.ToString();

                        dt0 = ds.Tables[0];
                        dt0.TableName = tblname;
                        ws.Name = tblname;
                        ws.Cell(4, 1).InsertTable(dt0);
                        ws.Tables.Table(0).ShowAutoFilter = false;
                        ws.Tables.Table(0).ShowRowStripes = false;
                        //wb.Worksheets.Add(dt0);

                        wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        wb.Style.Font.Bold = true;
                        wb.Style.Border = null;
                        break;
                    case 1:
                        compname = "IMPORT " + Session["GYrDesc"].ToString();

                        // Merge and Add a Heading
                        ws.Range("A1:L1").Merge().Value = compname.ToString();
                        ws.Range("A1:L1").Style.Font.Bold = true;
                        ws.Range("A1:L1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // Merge and Add a Heading
                        ws.Range("A2:D2").Merge().Value = strhead.ToString();
                        ws.Range("A2:D2").Style.Font.Bold = true;
                        ws.Range("A2:D2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        //Assging Import Total Count

                        dt0 = ds.Tables[0];
                        dt0.TableName = tblname;
                        ws.Name = tblname;

                        crows = 4;
                        lrow = ds.Tables[0].Rows.Count;

                        ws.Cell(crows, 1).InsertTable(dt0);
                        ws.Tables.Table(0).ShowAutoFilter = false;
                        ws.Tables.Table(0).ShowRowStripes = false;

                        ws.Row(crows + lrow).Style.Font.Bold = true;

                        //Assging Import bmw 

                        ws.Range("F3:K3").Merge().Value = "Teus Handled";
                        ws.Range("F3:K3").Style.Font.Bold = true;
                        ws.Range("F3:K3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        dt1 = ds.Tables[1];
                        crows = 4;
                        lrow = ds.Tables[1].Rows.Count;

                        ws.Cell(crows, 6).InsertTable(dt1);
                        ws.Tables.Table(1).ShowAutoFilter = false;
                        ws.Tables.Table(1).ShowRowStripes = false;

                        ws.Row(crows + lrow).Style.Font.Bold = true;

                        //ASING EXPORT
                        compname = "EXPORT " + Session["GYrDesc"].ToString();
                        crows = crows + lrow + 2;

                        // Merge and Add a Heading
                        range = "A" + crows + ":L" + crows;
                        ws.Range(range).Merge().Value = compname.ToString();
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        dt2 = ds.Tables[2];
                        crows = crows + 2;
                        lrow = ds.Tables[2].Rows.Count;

                        ws.Cell(crows, 1).InsertTable(dt2);
                        ws.Tables.Table(2).ShowAutoFilter = false;
                        ws.Tables.Table(2).ShowRowStripes = false;
                        crows = crows + lrow;
                        ws.Row(crows).Style.Font.Bold = true;

                        //ASING import scanning container
                        compname = "Scanning Container " + Session["GYrDesc"].ToString();
                        crows = crows + 2;

                        // Merge and Add a Heading
                        range = "A" + crows + ":L" + crows;
                        ws.Range(range).Merge().Value = compname.ToString();
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        dt3 = ds.Tables[3];
                        crows = crows + 2;
                        lrow = ds.Tables[3].Rows.Count;

                        ws.Cell(crows, 1).InsertTable(dt3);
                        ws.Tables.Table(3).ShowAutoFilter = false;
                        ws.Tables.Table(3).ShowRowStripes = false;
                        crows = crows + lrow;
                        ws.Row(crows).Style.Font.Bold = true;

                        //ASING non pnr container
                        compname = "NON PNR Details " + Session["GYrDesc"].ToString();
                        crows = crows + 2;

                        // Merge and Add a Heading
                        range = "A" + crows + ":L" + crows;
                        ws.Range(range).Merge().Value = compname.ToString();
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        crows = crows + 2;
                        range = "B" + crows + ":C" + crows;
                        ws.Range(range).Merge().Value = "BMW";
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Fill.BackgroundColor =  XLColor.DeepSkyBlue;
                        ws.Range(range).Style.Font.FontColor = XLColor.White;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        range = "D" + crows + ":E" + crows;
                        ws.Range(range).Merge().Value = "ZF";
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Fill.BackgroundColor = XLColor.DeepSkyBlue;
                        ws.Range(range).Style.Font.FontColor = XLColor.White;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        range = "F" + crows + ":G" + crows;
                        ws.Range(range).Merge().Value = "JSW/NTC";
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Fill.BackgroundColor = XLColor.DeepSkyBlue;
                        ws.Range(range).Style.Font.FontColor = XLColor.White;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        range = "H" + crows + ":I" + crows;
                        ws.Range(range).Merge().Value = "Others";
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Fill.BackgroundColor = XLColor.DeepSkyBlue;
                        ws.Range(range).Style.Font.FontColor = XLColor.White;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        range = "J" + crows + ":K" + crows;
                        ws.Range(range).Merge().Value = "Total";
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Fill.BackgroundColor = XLColor.DeepSkyBlue;
                        ws.Range(range).Style.Font.FontColor = XLColor.White;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        range = "L" + crows + ":L" + crows;
                        ws.Range(range).Merge().Value = "TUES";
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Fill.BackgroundColor = XLColor.DeepSkyBlue;
                        ws.Range(range).Style.Font.FontColor = XLColor.White;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        dt4 = ds.Tables[4];
                        crows = crows + 1;
                        lrow = ds.Tables[4].Rows.Count;

                        string[] headers = { "Month", "20'", "40'", "20'", "40'", "20'", "40'", "20'", "40'", "20'", "40'", "" };
                        // Assign headers dynamically
                        for (int col = 0; col < headers.Length; col++)
                        {
                            ws.Cell(crows, col + 1).Value = headers[col];
                            ws.Cell(crows, col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }

                        int startRow = crows + 1; // Start from row 2 to keep space for the title
                        foreach (DataRow row in dt4.Rows)
                        {
                            for (int col = 0; col < dt4.Columns.Count; col++)
                            {
                                if (col == 0)
                                {
                                    if (DateTime.TryParse(row[col].ToString(), out DateTime dateValue))
                                    {
                                        ws.Cell(startRow, col + 1).Value = dateValue;
                                        ws.Cell(startRow, col + 1).Style.DateFormat.Format = "MMM-yyyy"; // Format as 'Jun-2024'
                                    }
                                    //ws.Cell(startRow, col + 1).Value = row[col].ToString();
                                }
                                else
                                {
                                    ws.Cell(startRow, col + 1).Value = row[col];
                                }
                                
                                ws.Cell(startRow, col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            }
                            startRow++;
                        }

                        ws.Row(crows).Style.Font.Bold = true;
                        crows = crows + lrow;
                        ws.Row(crows).Style.Font.Bold = true;

                        //START
                        //ASING DESTUFF IMPORT MANUNAL / MECH
                        compname = "Import Details " + Session["GYrDesc"].ToString();
                        crows = crows + 2;

                        // Merge and Add a Heading
                        range = "A" + crows + ":F" + crows;
                        ws.Range(range).Merge().Value = compname.ToString();
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        crows = crows + 2;
                        range = "B" + crows + ":C" + crows;
                        ws.Range(range).Merge().Value = "Manual";
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Fill.BackgroundColor = XLColor.DeepSkyBlue;
                        ws.Range(range).Style.Font.FontColor = XLColor.White;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        range = "D" + crows + ":E" + crows;
                        ws.Range(range).Merge().Value = "Mechanical";
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Fill.BackgroundColor = XLColor.DeepSkyBlue;
                        ws.Range(range).Style.Font.FontColor = XLColor.White;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        range = "F" + crows + ":F" + crows;
                        ws.Range(range).Merge().Value = "TUES";
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Fill.BackgroundColor = XLColor.DeepSkyBlue;
                        ws.Range(range).Style.Font.FontColor = XLColor.White;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        dt7 = ds.Tables[5];
                        crows = crows + 1;
                        lrow = ds.Tables[5].Rows.Count;

                        string[] headers7 = { "Month", "20'", "40'", "20'", "40'", "" };
                        // Assign headers dynamically
                        for (int col = 0; col < headers7.Length; col++)
                        {
                            ws.Cell(crows, col + 1).Value = headers7[col];
                            ws.Cell(crows, col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }

                        int startRow7 = crows + 1; // Start from row 2 to keep space for the title
                        foreach (DataRow row in dt7.Rows)
                        {
                            for (int col = 0; col < dt7.Columns.Count; col++)
                            {
                                if (col == 0)
                                {
                                    if (DateTime.TryParse(row[col].ToString(), out DateTime dateValue))
                                    {
                                        ws.Cell(startRow7, col + 1).Value = dateValue;
                                        ws.Cell(startRow7, col + 1).Style.DateFormat.Format = "MMM-yyyy"; // Format as 'Jun-2024'
                                    }
                                    //ws.Cell(startRow, col + 1).Value = row[col].ToString();
                                }
                                else
                                {
                                    ws.Cell(startRow7, col + 1).Value = row[col];
                                }
                                
                                ws.Cell(startRow7, col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            }
                            startRow7++;
                        }

                        ws.Row(crows).Style.Font.Bold = true;
                        //crows = crows + lrow;
                        ws.Row(crows + lrow).Style.Font.Bold = true;

                        //END

                        //start
                        //ASING stuffing EXPORT MANUNAL / MECH
                        compname = "Export Details " + Session["GYrDesc"].ToString();
                        crows = crows - 3;

                        // Merge and Add a Heading
                        range = "H" + crows + ":M" + crows;
                        ws.Range(range).Merge().Value = compname.ToString();
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        crows = crows + 2;
                        range = "I" + crows + ":J" + crows;
                        ws.Range(range).Merge().Value = "Manual";
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Fill.BackgroundColor = XLColor.DeepSkyBlue;
                        ws.Range(range).Style.Font.FontColor = XLColor.White;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        range = "K" + crows + ":L" + crows;
                        ws.Range(range).Merge().Value = "Mechanical";
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Fill.BackgroundColor = XLColor.DeepSkyBlue;
                        ws.Range(range).Style.Font.FontColor = XLColor.White;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        range = "M" + crows + ":M" + crows;
                        ws.Range(range).Merge().Value = "TUES";
                        ws.Range(range).Style.Font.Bold = true;
                        ws.Range(range).Style.Fill.BackgroundColor = XLColor.DeepSkyBlue;
                        ws.Range(range).Style.Font.FontColor = XLColor.White;
                        ws.Range(range).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        dt8 = ds.Tables[6];
                        crows = crows + 1;
                        lrow = ds.Tables[6].Rows.Count;

                        string[] headers8 = { "Month", "20'", "40'", "20'", "40'", "" };
                        // Assign headers dynamically
                        for (int col = 0; col < headers8.Length; col++)
                        {
                            ws.Cell(crows, col + 8).Value = headers8[col];
                            ws.Cell(crows, col + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }

                        int startRow8 = crows + 1; // Start from row 2 to keep space for the title
                        foreach (DataRow row in dt8.Rows)
                        {
                            for (int col = 0; col < dt8.Columns.Count; col++)
                            {
                                if (col == 0)
                                {
                                    if (DateTime.TryParse(row[col].ToString(), out DateTime dateValue))
                                    {
                                        ws.Cell(startRow8, col + 8).Value = dateValue;
                                        ws.Cell(startRow8, col + 8).Style.DateFormat.Format = "MMM-yyyy"; // Format as 'Jun-2024'
                                    }
                                    //ws.Cell(startRow, col + 1).Value = row[col].ToString();
                                }
                                else
                                {
                                    ws.Cell(startRow8, col + 8).Value = row[col];
                                }
                                
                                ws.Cell(startRow8, col + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            }
                            startRow8++;
                        }

                        ws.Row(crows).Style.Font.Bold = true;
                        crows = crows + lrow;
                        ws.Row(crows).Style.Font.Bold = true;

                        //end

                        // Auto-adjust column widths
                        ws.Columns().AdjustToContents();
                        

                        wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        wb.Style.Font.Bold = true;
                        wb.Style.Border = null;


                        break;
                }


                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= " + filename);// Collection_Details.xlsx");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    //Response.End();
                }
            }
            //return RedirectToAction("Index", "SalesInvoiceRpt");

        }

    }
}