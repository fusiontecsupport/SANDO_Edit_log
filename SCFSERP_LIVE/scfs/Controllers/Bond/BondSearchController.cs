using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using scfs_erp.Context;
using DocumentFormat.OpenXml.Spreadsheet;

namespace scfs.Controllers.Bond
{
    public class BondSearchController : Controller
    {
        // GET: BondSearch

        #region contextdeclaration
        BondContext context = new BondContext();

        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        #endregion
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            if (Request.Form.Get("RPTBNDID") != null)
            {
                Session["RPTBNDID"] = Request.Form.Get("RPTBNDID");
                Session["RPTEBNDID"] = Request.Form.Get("RPTEBNDID");
                Session["BNDDNO"] = Request.Form.Get("BNDDNO");
                Session["EBNDDNO"] = Request.Form.Get("EBNDDNO");
            }
            return View();
        }

        #region Autocomplete Bond No
        public JsonResult AutoBondNo(string term)
        {
            var compyid = Convert.ToInt32(Session["compyid"]);
            var result = (from r in context.bondinfodtls //.Where(x => x.COMPYID == compyid)
                          where r.BNDDNO.ToLower().Contains(term.ToLower())
                          select new { r.BNDDNO, r.BNDID }).OrderBy(x => x.BNDDNO).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Autocomplete ExBond No
        public JsonResult AutoExBondNo(string term)
        {
            var compyid = Convert.ToInt32(Session["compyid"]);
            var result = (from r in context.exbondinfodtls //.Where(x => x.COMPYID == compyid)
                          where r.EBNDDNO.ToLower().Contains(term.ToLower())
                          select new { r.EBNDDNO, r.EBNDID }).OrderBy(x => x.EBNDDNO).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Bond Information  
        public string GetBondDetails()
        {

            string crole = Convert.ToString(Session["Group"]);
            string cusr = Convert.ToString(Session["CUSRID"]);
            int compyid = Convert.ToInt32(Session["compyid"]);
            int bndid = 0;
            int ebndid = 0;
            string BndID = ""; 
            string EBndID = "";
            string BndNO = "";
            string EBndNO = "";

            if (BndID == "" || BndID == null)
                BndID = Convert.ToString(Session["RPTBNDID"]);
            
            if (EBndID == "" || EBndID == null)
                EBndID = Convert.ToString(Session["RPTEBNDID"]);


            if (BndNO == "" || BndNO == null)
                BndNO = Convert.ToString(Session["BNDDNO"]);

            if (EBndNO == "" || EBndNO == null)
                EBndNO = Convert.ToString(Session["EBNDDNO"]);


            if (BndID != "" && BndID != null)
                bndid = Convert.ToInt32(BndID);

            if (EBndID != "" && EBndID != null)
                ebndid = Convert.ToInt32(EBndID);

            //Session["RPTBNDID"] = 0;
            //Session["RPTEBNDID"] = 0;
            //Session["BNDDNO"] = "";
            //Session["EBNDDNO"] = "";


            try
            {

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SCFSERP"].ToString()))
                {

                    conn.Open();


                    SqlCommand cmd = new SqlCommand();
                    SqlDataAdapter da = new SqlDataAdapter("[dbo].[pr_Get_Bond_ExBond_Search_Detail]", conn);

                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.CommandTimeout = 0;
                    da.SelectCommand.Parameters.Add(new SqlParameter("@bndid", SqlDbType.Int));
                    da.SelectCommand.Parameters.Add(new SqlParameter("@ebndid", SqlDbType.Int));
                    da.SelectCommand.Parameters.Add(new SqlParameter("@bndNO", SqlDbType.VarChar));
                    da.SelectCommand.Parameters.Add(new SqlParameter("@ebndNO", SqlDbType.VarChar));


                    da.SelectCommand.Parameters["@bndid"].Value = bndid;
                    da.SelectCommand.Parameters["@ebndid"].Value = ebndid;
                    da.SelectCommand.Parameters["@bndNO"].Value = BndNO;
                    da.SelectCommand.Parameters["@ebndNO"].Value = EBndNO;

                    DataSet ds = new DataSet();
                    StringBuilder html0 = new StringBuilder();
                    StringBuilder html1 = new StringBuilder();
                    StringBuilder html2 = new StringBuilder();
                    StringBuilder html3 = new StringBuilder();
                    StringBuilder sb = new StringBuilder();
                    da.Fill(ds, "DBDtl");

                    ////Building the Header row.
                    int tblcnt = 0;
                    while (tblcnt < ds.Tables.Count)
                    {
                        //if (ds.Tables[tblcnt].Rows.Count > 0)
                        {
                            //Table start.
                            html1.Append("<br/><br/><table class='table table-striped table-bordered datatable'>");

                            //Building the Header row.
                            html1.Append("<tr>");

                            int tblcolcnt = 0;
                            foreach (DataColumn column in ds.Tables[tblcnt].Columns)
                            {
                                if (tblcolcnt > 0)
                                {
                                    html1.Append("<th><h5  style='color:navy !important;'>");
                                    html1.Append(column.ColumnName);
                                    html1.Append("</h5></th>");
                                }
                                else
                                {
                                    html1.Append("<th style='font-weight:bold;color:navy;' colspan='" + (ds.Tables[tblcnt].Columns.Count - 1).ToString() + "' class='bg-info'><h6>");
                                    html1.Append(ds.Tables[tblcnt].Rows[0][column.ColumnName].ToString()); html1.Append("</h6></th></tr><tr>");
                                }
                                tblcolcnt++;
                            }
                            html1.Append("</tr>");

                            if (ds.Tables[tblcnt].Rows.Count > 0)
                            {
                                //Building the Data rows.
                                foreach (DataRow row in ds.Tables[tblcnt].Rows)
                                {
                                    html1.Append("<tr>");
                                    tblcolcnt = 0;
                                    foreach (DataColumn column in ds.Tables[tblcnt].Columns)
                                    {
                                        if (tblcolcnt > 0)
                                        {

                                            //if (column.ColumnName == "Totals")
                                            //{
                                            //    html1.Append(Convert.ToDecimal(row[column.ColumnName]).ToString("#,##,###0.00"));
                                            //}
                                            //else
                                            //{
                                            if (row[1].ToString().ToUpper().Contains("TOTAL"))
                                            {
                                                html1.Append("<td style='font-weight: bold; color:navy;'> <h6>");

                                            }
                                            else
                                            {
                                                html1.Append("<td>");
                                            }

                                            html1.Append(row[column.ColumnName].ToString().Replace("_", ""));
                                            if (row[1].ToString().ToUpper().Contains("TOTAL"))
                                            {
                                                html1.Append("</h6>");
                                            }
                                            //}

                                            html1.Append("</td>");
                                        }
                                        tblcolcnt++;
                                    }
                                    html1.Append("</tr>");
                                }
                            }
                            else
                            {
                                html1.Append("<th style='color:navy;' colspan='" + (ds.Tables[tblcnt].Columns.Count - 1).ToString() + "' class='bg-success'><h6>");
                                html1.Append("No Records Found!!!"); html1.Append("</h6></th></tr><tr>");
                            }
                            //Table end.
                            html1.Append("</table>");
                            tblcnt++;
                            //Append the HTML string to Placeholder.

                        }
                        //else
                        //    tblcnt++;

                    }

                    return html1.ToString();
                }
            }
            catch (Exception ex)
            {
                string status = "error";
                return status;
            }

        }
        #endregion
    }
}