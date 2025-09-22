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
using System.Data.Entity;
using System.Reflection;

namespace scfs_erp.Controllers.Import
{
    public class ImportGateInController : Controller
    {
        // GET: ImportGateIn
        #region Context declaration
        SCFSERPContext context = new SCFSERPContext();

        #endregion

        #region Index Form
        //[Authorize(Roles = "ImportGateInIndex")] // Role-based restriction removed to allow broader access
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

            DateTime fromdate = Convert.ToDateTime(Session["SDATE"]).Date;
            DateTime todate = Convert.ToDateTime(Session["EDATE"]).Date;


            TotalContainerDetails(fromdate, todate);


            return View(context.gateindetails.Where(x => x.GIDATE >= sd).Where(x => x.GIDATE <= ed).Where(x => x.SDPTID == 1).Where(x => x.CONTNRID >= 1).ToList());
        }
        #endregion

        #region Get data from database

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            try
            {
                using (var e = new CFSImportEntities())
                {
                    var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                    var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                    // Safely get session values with defaults
                    var startDate = Session["SDATE"] != null ? Convert.ToDateTime(Session["SDATE"]) : DateTime.Now.Date;
                    var endDate = Session["EDATE"] != null ? Convert.ToDateTime(Session["EDATE"]) : DateTime.Now.Date;
                    var companyId = Session["compyid"] != null ? Convert.ToInt32(Session["compyid"]) : 0;

                    if (companyId == 0)
                    {
                        return Json(new { error = "Invalid company ID" }, JsonRequestBehavior.AllowGet);
                    }

                    var data = e.pr_Search_Import_GateInGridAssgn(param.sSearch ?? "",
                                                    Convert.ToInt32(Request["iSortCol_0"] ?? "0"),
                                                    Request["sSortDir_0"] ?? "asc",
                                                    param.iDisplayStart,
                                                    param.iDisplayStart + param.iDisplayLength,
                                                    totalRowsCount,
                                                    filteredRowsCount, 
                                                    startDate,
                                                    endDate,
                                                    companyId);

                    var aaData = data.Select(d => new string[] { 
                        d.GIDATE?.ToString("dd/MM/yyyy") ?? "",
                        d.GIDNO ?? "",
                        d.CONTNRNO ?? "", 
                        d.CONTNRSID ?? "", 
                        d.IGMNO ?? "", 
                        d.GPLNO ?? "", 
                        d.IMPRTNAME ?? "", 
                        d.STMRNAME ?? "",  
                        d.VSLNAME ?? "", 
                        d.BLNO ?? "",
                        d.PRDTDESC ?? "", 
                        d.DISPSTATUS ?? "", 
                        d.GIDID?.ToString() ?? "" 
                    }).ToList();

                    return Json(new
                    {
                        sEcho = param.sEcho,
                        aaData = aaData,
                        iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                        iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Log the error details for debugging
                return Json(new { 
                    error = "Database error occurred", 
                    details = ex.Message,
                    innerException = ex.InnerException?.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }

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
                                                WHERE (@GIDNO IS NULL OR [GIDNO] = @GIDNO)
                                                  AND (@FROM IS NULL OR [ChangedOn] >= @FROM)
                                                  AND (@TO   IS NULL OR [ChangedOn] <  DATEADD(day, 1, @TO))
                                                  AND (@USER IS NULL OR [ChangedBy] LIKE @USERPAT)
                                                  AND (@FIELD IS NULL OR [FieldName] LIKE @FIELDPAT)
                                                  AND (@VERSION IS NULL OR [Version] LIKE @VERPAT)
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
                                GIDNO = r["GIDNO"] != DBNull.Value ? Convert.ToInt32(r["GIDNO"]) : 0,
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

                // Removed invalid fallback block that referenced undefined variables (a, b, versionA, versionB)
            }
            // Map raw DB codes to form-friendly display values for known fields
            try
            {
                // Build lookup dictionaries once
                var dictSlot = context.slotmasters.ToDictionary(x => x.SLOTID, x => x.SLOTDESC);
                var dictRow = context.rowmasters.ToDictionary(x => x.ROWID, x => x.ROWDESC);
                var dictPrdtGrp = context.productgroupmasters.ToDictionary(x => x.PRDTGID, x => x.PRDTGDESC);
                var dictContType = context.containertypemasters.ToDictionary(x => x.CONTNRTID, x => x.CONTNRTDESC);
                var dictContSize = context.containersizemasters.ToDictionary(x => x.CONTNRSID, x => x.CONTNRSDESC);
                var dictGpMode = context.gpmodemasters.ToDictionary(x => x.GPMODEID, x => x.GPMODEDESC);
                var dictPortType = context.porttypemaster.ToDictionary(x => x.GPPTYPE, x => x.GPPTYPEDESC);

                string Map(string field, string raw)
                {
                    if (raw == null) return raw;
                    var f = (field ?? string.Empty).Trim();
                    var val = raw.Trim();
                    if (string.IsNullOrEmpty(val)) return raw;
                    int ival;
                    switch (f.ToUpperInvariant())
                    {
                        case "SLOTID":
                            return int.TryParse(val, out ival) && dictSlot.ContainsKey(ival) ? dictSlot[ival] : raw;
                        case "ROWID":
                            return int.TryParse(val, out ival) && dictRow.ContainsKey(ival) ? dictRow[ival] : raw;
                        case "PRDTGID":
                            return int.TryParse(val, out ival) && dictPrdtGrp.ContainsKey(ival) ? dictPrdtGrp[ival] : raw;
                        case "CONTNRTID":
                            return int.TryParse(val, out ival) && dictContType.ContainsKey(ival) ? dictContType[ival] : raw;
                        case "CONTNRSID":
                            return int.TryParse(val, out ival) && dictContSize.ContainsKey(ival) ? dictContSize[ival] : raw;
                        case "GPMODEID":
                            return int.TryParse(val, out ival) && dictGpMode.ContainsKey(ival) ? dictGpMode[ival] : raw;
                        case "GPPTYPE":
                            return int.TryParse(val, out ival) && dictPortType.ContainsKey(ival) ? dictPortType[ival] : raw;
                        case "GPETYPE":
                        case "GPSTYPE":
                        case "GPWTYPE":
                        case "GPSCNTYPE":
                            return val == "1" ? "YES" : val == "0" ? "NO" : raw;
                        case "GPSCNMTYPE":
                            if (val == "1") return "MISMATCH";
                            if (val == "2") return "CLEAN";
                            if (val == "3") return "NOT SCANNED";
                            return raw;
                        case "GFCLTYPE":
                            return val == "1" ? "FCL" : val == "0" ? "LCL" : raw;
                        case "GRADEID":
                            return val == "2" ? "YES" : val == "1" ? "NO" : raw;
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
                        case "GIDATE": return "In Date";
                        case "GITIME": return "In Time";
                        case "GICCTLDATE": return "Port Out Date";
                        case "GICCTLTIME": return "Port Out Time";
                        case "GINO": return "Gate In No";
                        case "GIDNO": return "No";
                        case "GPREFNO": return "Ref No";
                        case "DRVNAME": return "Driver Name";
                        case "TRNSPRTNAME": return "Transpoter Name";
                        case "GTRNSPRTNAME": return "Other Transpoter Name";
                        case "VHLNO": return "Vehicle No";
                        case "GPNRNO": return "PNR No";
                        case "VSLNAME":
                        case "VSLID": return "Vessel Name";
                        case "VOYNO": return "Voyage No";
                        case "IGMNO": return "IGM No.";
                        case "GPLNO": return "Line No";
                        case "IMPRTNAME":
                        case "IMPRTID": return "Importer Name";
                        case "STMRNAME":
                        case "STMRID": return "Steamer Name";
                        case "CHANAME": return "CHA Name";
                        case "BOENO": return "Bill of Entry No";
                        case "BOEDATE": return "Bill of Entry Date";
                        case "CONTNRNO": return "Container No";
                        case "CONTNRSID": return "Size";
                        case "CONTNRTID": return "Type";
                        case "GIISOCODE": return "ISO Code";
                        case "LPSEALNO": return "L.seal no";
                        case "CSEALNO": return "C.seal no";
                        case "ROWID": return "Row";
                        case "SLOTID": return "Slot";
                        case "PRDTGID": return "Product Category";
                        case "PRDTDESC": return "Product Description";
                        case "GPWTYPE": return "Weightment";
                        case "GPWGHT": return "Weight";
                        case "GPPTYPE": return "Port";
                        case "IGMDATE": return "IGM Date";
                        case "BLNO": return "BL No.";
                        case "GFCLTYPE": return "FCL";
                        case "GIDMGDESC": return "Damage";
                        case "GPMODEID": return "GP Mode";
                        case "GPETYPE": return "SSR/Escort";
                        case "GPSTYPE": return "S.Amend / Mismatch";
                        case "GPEAMT": return "SSR/Escort Amount";
                        case "GPAAMT": return "Addtnl. Amount";
                        case "GPSCNTYPE": return "Scanned";
                        case "GPSCNMTYPE": return "Scan Type";
                        case "GRADEID": return "Refer(Plug)";
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

        // Export current filtered GateIn log as CSV
        public ActionResult EditLogGateInExport(int? gidid, DateTime? from = null, DateTime? to = null, string user = null, string fieldName = null, string version = null)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            var rows = new List<scfs_erp.Models.GateInDetailEditLogRow>();
            var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
            if (cs != null && !string.IsNullOrWhiteSpace(cs.ConnectionString))
            {
                using (var sql = new SqlConnection(cs.ConnectionString))
                using (var cmd = new SqlCommand(@"SELECT [GIDNO],[FieldName],[OldValue],[NewValue],[ChangedBy],[ChangedOn],[Version],[Modules]
                                                FROM [dbo].[GateInDetailEditLog]
                                                WHERE (@GIDNO IS NULL OR [GIDNO] = @GIDNO)
                                                  AND (@FROM IS NULL OR [ChangedOn] >= @FROM)
                                                  AND (@TO   IS NULL OR [ChangedOn] <  DATEADD(day, 1, @TO))
                                                  AND (@USER IS NULL OR [ChangedBy] LIKE @USERPAT)
                                                  AND (@FIELD IS NULL OR [FieldName] LIKE @FIELDPAT)
                                                  AND (@VERSION IS NULL OR [Version] LIKE @VERPAT)
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
                            rows.Add(new scfs_erp.Models.GateInDetailEditLogRow
                            {
                                GIDNO = r["GIDNO"] != DBNull.Value ? Convert.ToInt32(r["GIDNO"]) : 0,
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

            // Map friendly labels and display values for CSV
            try
            {
                var dictSlot = context.slotmasters.ToDictionary(x => x.SLOTID, x => x.SLOTDESC);
                var dictRow = context.rowmasters.ToDictionary(x => x.ROWID, x => x.ROWDESC);
                var dictPrdtGrp = context.productgroupmasters.ToDictionary(x => x.PRDTGID, x => x.PRDTGDESC);
                var dictContType = context.containertypemasters.ToDictionary(x => x.CONTNRTID, x => x.CONTNRTDESC);
                var dictContSize = context.containersizemasters.ToDictionary(x => x.CONTNRSID, x => x.CONTNRSDESC);
                var dictGpMode = context.gpmodemasters.ToDictionary(x => x.GPMODEID, x => x.GPMODEDESC);
                var dictPortType = context.porttypemaster.ToDictionary(x => x.GPPTYPE, x => x.GPPTYPEDESC);

                string Map(string field, string raw)
                {
                    if (string.IsNullOrWhiteSpace(raw)) return raw;
                    int ival;
                    switch (field?.ToUpperInvariant())
                    {
                        case "SLOTID":
                            return int.TryParse(raw, out ival) && dictSlot.ContainsKey(ival) ? dictSlot[ival] : raw;
                        case "ROWID":
                            return int.TryParse(raw, out ival) && dictRow.ContainsKey(ival) ? dictRow[ival] : raw;
                        case "PRDTGID":
                            return int.TryParse(raw, out ival) && dictPrdtGrp.ContainsKey(ival) ? dictPrdtGrp[ival] : raw;
                        case "CONTNRTID":
                            return int.TryParse(raw, out ival) && dictContType.ContainsKey(ival) ? dictContType[ival] : raw;
                        case "CONTNRSID":
                            return int.TryParse(raw, out ival) && dictContSize.ContainsKey(ival) ? dictContSize[ival] : raw;
                        case "GPMODEID":
                            return int.TryParse(raw, out ival) && dictGpMode.ContainsKey(ival) ? dictGpMode[ival] : raw;
                        case "GPPTYPE":
                            return int.TryParse(raw, out ival) && dictPortType.ContainsKey(ival) ? dictPortType[ival] : raw;
                        case "GPETYPE":
                        case "GPSTYPE":
                        case "GPWTYPE":
                        case "GPSCNTYPE":
                            return raw == "1" ? "YES" : raw == "0" ? "NO" : raw;
                        case "GPSCNMTYPE":
                            if (raw == "1") return "MISMATCH";
                            if (raw == "2") return "CLEAN";
                            if (raw == "3") return "NOT SCANNED";
                            return raw;
                        case "GFCLTYPE":
                            return raw == "1" ? "FCL" : raw == "0" ? "LCL" : raw;
                        case "GRADEID":
                            return raw == "2" ? "YES" : raw == "1" ? "NO" : raw;
                        default:
                            return raw;
                    }
                }

                string Friendly(string field)
                {
                    if (string.IsNullOrWhiteSpace(field)) return field;
                    switch (field.ToUpperInvariant())
                    {
                        case "GIDATE": return "In Date";
                        case "GITIME": return "In Time";
                        case "GICCTLDATE": return "Port Out Date";
                        case "GICCTLTIME": return "Port Out Time";
                        case "GINO": return "Gate In No";
                        case "GIDNO": return "No";
                        case "GPREFNO": return "Ref No";
                        case "DRVNAME": return "Driver Name";
                        case "TRNSPRTNAME": return "Transpoter Name";
                        case "GTRNSPRTNAME": return "Other Transpoter Name";
                        case "VHLNO": return "Vehicle No";
                        case "GPNRNO": return "PNR No";
                        case "VSLNAME":
                        case "VSLID": return "Vessel Name";
                        case "VOYNO": return "Voyage No";
                        case "IGMNO": return "IGM No.";
                        case "GPLNO": return "Line No";
                        case "IMPRTNAME":
                        case "IMPRTID": return "Importer Name";
                        case "STMRNAME":
                        case "STMRID": return "Steamer Name";
                        case "CHANAME": return "CHA Name";
                        case "BOENO": return "Bill of Entry No";
                        case "BOEDATE": return "Bill of Entry Date";
                        case "CONTNRNO": return "Container No";
                        case "CONTNRSID": return "Size";
                        case "CONTNRTID": return "Type";
                        case "GIISOCODE": return "ISO Code";
                        case "LPSEALNO": return "L.seal no";
                        case "CSEALNO": return "C.seal no";
                        case "ROWID": return "Row";
                        case "SLOTID": return "Slot";
                        case "PRDTGID": return "Product Category";
                        case "PRDTDESC": return "Product Description";
                        case "GPWTYPE": return "Weightment";
                        case "GPWGHT": return "Weight";
                        case "GPPTYPE": return "Port";
                        case "IGMDATE": return "IGM Date";
                        case "BLNO": return "BL No.";
                        case "GFCLTYPE": return "FCL";
                        case "GIDMGDESC": return "Damage";
                        case "GPMODEID": return "GP Mode";
                        case "GPETYPE": return "SSR/Escort";
                        case "GPSTYPE": return "S.Amend / Mismatch";
                        case "GPEAMT": return "SSR/Escort Amount";
                        case "GPAAMT": return "Addtnl. Amount";
                        case "GPSCNTYPE": return "Scanned";
                        case "GPSCNMTYPE": return "Scan Type";
                        case "GRADEID": return "Refer(Plug)";
                        default: return field;
                    }
                }

                foreach (var row in rows)
                {
                    row.OldValue = Map(row.FieldName, row.OldValue);
                    row.NewValue = Map(row.FieldName, row.NewValue);
                    row.FieldName = Friendly(row.FieldName);
                }
            }
            catch { /* best-effort mapping for CSV export */ }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ChangedOn,GIDNO,Version,Field,OldValue,NewValue,ChangedBy,Module");
            foreach (var x in rows)
            {
                string esc(string s) => string.IsNullOrEmpty(s) ? "" : ("\"" + s.Replace("\"", "\"\"") + "\"");
                sb.AppendLine(string.Join(",",
                    esc(x.ChangedOn.ToString("yyyy-MM-dd HH:mm:ss")),
                    x.GIDNO,
                    esc(x.Version),
                    esc(x.FieldName),
                    esc(x.OldValue),
                    esc(x.NewValue),
                    esc(x.ChangedBy),
                    esc(x.Modules)));
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "GateInEditLog.csv");
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

            // Normalize version strings (trim whitespace)
            versionA = (versionA ?? string.Empty).Trim();
            versionB = (versionB ?? string.Empty).Trim();

            var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
            var a = new List<scfs_erp.Models.GateInDetailEditLogRow>();
            var b = new List<scfs_erp.Models.GateInDetailEditLogRow>();
            if (cs != null && !string.IsNullOrWhiteSpace(cs.ConnectionString))
            {
                using (var sql = new SqlConnection(cs.ConnectionString))
                using (var cmd = new SqlCommand(@"SELECT [GIDNO],[FieldName],[OldValue],[NewValue],[ChangedBy],[ChangedOn],[Version],[Modules]
                                                FROM [dbo].[GateInDetailEditLog]
                                                WHERE [GIDNO]=@GIDNO AND RTRIM(LTRIM([Version]))=@V", sql))
                {
                    cmd.Parameters.Add("@GIDNO", System.Data.SqlDbType.Int);
                    cmd.Parameters.Add("@V", System.Data.SqlDbType.NVarChar, 100);

                    sql.Open();
                    cmd.Parameters["@GIDNO"].Value = gidid.Value;
                    cmd.Parameters["@V"].Value = versionA;
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            a.Add(new scfs_erp.Models.GateInDetailEditLogRow
                            {
                                GIDNO = gidid.Value,
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
                            b.Add(new scfs_erp.Models.GateInDetailEditLogRow
                            {
                                GIDNO = gidid.Value,
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
                var dictSlot = context.slotmasters.ToDictionary(x => x.SLOTID, x => x.SLOTDESC);
                var dictRow = context.rowmasters.ToDictionary(x => x.ROWID, x => x.ROWDESC);
                var dictPrdtGrp = context.productgroupmasters.ToDictionary(x => x.PRDTGID, x => x.PRDTGDESC);
                var dictContType = context.containertypemasters.ToDictionary(x => x.CONTNRTID, x => x.CONTNRTDESC);
                var dictContSize = context.containersizemasters.ToDictionary(x => x.CONTNRSID, x => x.CONTNRSDESC);
                var dictGpMode = context.gpmodemasters.ToDictionary(x => x.GPMODEID, x => x.GPMODEDESC);
                var dictPortType = context.porttypemaster.ToDictionary(x => x.GPPTYPE, x => x.GPPTYPEDESC);

                string Map(string field, string raw)
                {
                    if (string.IsNullOrWhiteSpace(raw)) return raw;
                    int ival;
                    switch (field?.ToUpperInvariant())
                    {
                        case "SLOTID":
                            return int.TryParse(raw, out ival) && dictSlot.ContainsKey(ival) ? dictSlot[ival] : raw;
                        case "ROWID":
                            return int.TryParse(raw, out ival) && dictRow.ContainsKey(ival) ? dictRow[ival] : raw;
                        case "PRDTGID":
                            return int.TryParse(raw, out ival) && dictPrdtGrp.ContainsKey(ival) ? dictPrdtGrp[ival] : raw;
                        case "CONTNRTID":
                            return int.TryParse(raw, out ival) && dictContType.ContainsKey(ival) ? dictContType[ival] : raw;
                        case "CONTNRSID":
                            return int.TryParse(raw, out ival) && dictContSize.ContainsKey(ival) ? dictContSize[ival] : raw;
                        case "GPMODEID":
                            return int.TryParse(raw, out ival) && dictGpMode.ContainsKey(ival) ? dictGpMode[ival] : raw;
                        case "GPPTYPE":
                            return int.TryParse(raw, out ival) && dictPortType.ContainsKey(ival) ? dictPortType[ival] : raw;
                        case "GPETYPE":
                        case "GPSTYPE":
                        case "GPWTYPE":
                        case "GPSCNTYPE":
                            return raw == "1" ? "YES" : raw == "0" ? "NO" : raw;
                        case "GPSCNMTYPE":
                            if (raw == "1") return "MISMATCH";
                            if (raw == "2") return "CLEAN";
                            if (raw == "3") return "NOT SCANNED";
                            return raw;
                        case "GFCLTYPE":
                            return raw == "1" ? "FCL" : raw == "0" ? "LCL" : raw;
                        case "GRADEID":
                            return raw == "2" ? "YES" : raw == "1" ? "NO" : raw;
                        default:
                            return raw;
                    }
                }

                string Friendly(string field)
                {
                    if (string.IsNullOrWhiteSpace(field)) return field;
                    switch (field.ToUpperInvariant())
                    {
                        case "GIDATE": return "In Date";
                        case "GITIME": return "In Time";
                        case "GICCTLDATE": return "Port Out Date";
                        case "GICCTLTIME": return "Port Out Time";
                        case "GINO": return "Gate In No";
                        case "GIDNO": return "No";
                        case "GPREFNO": return "Ref No";
                        case "DRVNAME": return "Driver Name";
                        case "TRNSPRTNAME": return "Transpoter Name";
                        case "GTRNSPRTNAME": return "Other Transpoter Name";
                        case "VHLNO": return "Vehicle No";
                        case "GPNRNO": return "PNR No";
                        case "VSLNAME":
                        case "VSLID": return "Vessel Name";
                        case "VOYNO": return "Voyage No";
                        case "IGMNO": return "IGM No.";
                        case "GPLNO": return "Line No";
                        case "IMPRTNAME":
                        case "IMPRTID": return "Importer Name";
                        case "STMRNAME":
                        case "STMRID": return "Steamer Name";
                        case "CHANAME": return "CHA Name";
                        case "BOENO": return "Bill of Entry No";
                        case "BOEDATE": return "Bill of Entry Date";
                        case "CONTNRNO": return "Container No";
                        case "CONTNRSID": return "Size";
                        case "CONTNRTID": return "Type";
                        case "GIISOCODE": return "ISO Code";
                        case "LPSEALNO": return "L.seal no";
                        case "CSEALNO": return "C.seal no";
                        case "ROWID": return "Row";
                        case "SLOTID": return "Slot";
                        case "PRDTGID": return "Product Category";
                        case "PRDTDESC": return "Product Description";
                        case "GPWTYPE": return "Weightment";
                        case "GPWGHT": return "Weight";
                        case "GPPTYPE": return "Port";
                        case "IGMDATE": return "IGM Date";
                        case "BLNO": return "BL No.";
                        case "GFCLTYPE": return "FCL";
                        case "GIDMGDESC": return "Damage";
                        case "GPMODEID": return "GP Mode";
                        case "GPETYPE": return "SSR/Escort";
                        case "GPSTYPE": return "S.Amend / Mismatch";
                        case "GPEAMT": return "SSR/Escort Amount";
                        case "GPAAMT": return "Addtnl. Amount";
                        case "GPSCNTYPE": return "Scanned";
                        case "GPSCNMTYPE": return "Scan Type";
                        case "GRADEID": return "Refer(Plug)";
                        default: return field; // fallback to technical name
                    }
                }

                foreach (var row in a)
                {
                    row.OldValue = Map(row.FieldName, row.OldValue);
                    row.NewValue = Map(row.FieldName, row.NewValue);
                    row.FieldName = Friendly(row.FieldName);
                }
                foreach (var row in b)
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
            ViewBag.RowsA = a;
            ViewBag.RowsB = b;
            return View();
        }
        #endregion

        #region TotalContainerDetails
        public JsonResult TotalContainerDetails(DateTime fromdate, DateTime todate)
        {
            string fdate = ""; string tdate = ""; int sdptid = 1;
            if (fromdate == null)
            {
                fromdate = DateTime.Now.Date;
                fdate = Convert.ToString(fromdate);
            }
            else
            {
                string infdate = Convert.ToString(fromdate);
                var in_date = infdate.Split(' ');
                var in_date1 = in_date[0].Split('/');
                fdate = Convert.ToString(in_date1[2] + "-" + in_date1[1] + "-" + in_date1[0]);
            }
            if (todate == null)
            {
                todate = DateTime.Now.Date;
                tdate = Convert.ToString(todate);
            }
            else
            {
                string intdate = Convert.ToString(todate);

                var in_date1 = intdate.Split(' ');
                var in_date2 = in_date1[0].Split('/');
                tdate = Convert.ToString(in_date2[2] + "-" + in_date2[1] + "-" + in_date2[0]);

            }

            context.Database.CommandTimeout = 0;
            var result = context.Database.SqlQuery<PR_IMPORT_DASHBOARD_DETAILS_Result>("EXEC PR_IMPORT_DASHBOARD_DETAILS @PFDT='" + fdate + "',@PTDT='" + tdate + "',@PSDPTID=" + 1).ToList();

            foreach (var rslt in result)
            {
                if ((rslt.Sno == 1) && (rslt.Descriptn == "IMPORT - GATEIN"))
                {
                    @ViewBag.Total20 = rslt.c_20;
                    @ViewBag.Total40 = rslt.c_40;
                    @ViewBag.Total45 = rslt.c_45;
                    @ViewBag.TotalTues = rslt.c_tues;

                    Session["GI20"] = rslt.c_20;
                    Session["GI40"] = rslt.c_40;
                    Session["GI45"] = rslt.c_45;
                    Session["GITU"] = rslt.c_tues;
                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Redirect to form from index
        //[Authorize(Roles = "ImportGateInEdit")] // Role-based restriction removed to allow broader access
        public void Edit(string id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/ImportGateIn/Form/" + id);

            //Response.Redirect("/ImportGateIn/Form/" + id);
        }
        #endregion

        #region Creating or Modify Form
        //[Authorize(Roles = "ImportGateInCreate")] // Role-based restriction removed to allow broader access
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            RemoteGateIn remotegatein = new RemoteGateIn();
            GateInDetail tab = new GateInDetail();

            string ginty = "";

            var GID = 0; var RGId = 0;
            if (id.Contains(';'))
            {
                var param = id.Split(';');
                RGId = Convert.ToInt32(param[0]);
                GID = Convert.ToInt32(param[1]);
                ginty = "RGateIn";
                Session["GTY"] = ginty;
            }
            else { GID = Convert.ToInt32(id); RGId = 0; ginty = "GateIn"; Session["GTY"] = ginty; }

            //tab.GITIME = DateTime.Now;
            //tab.GICCTLTIME = DateTime.Now;
            //tab.IGMDATE = DateTime.Now.Date;

            tab.GIDATE = DateTime.Now.Date;
            tab.GITIME = DateTime.Now;
            tab.GITIME = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);

            if (RGId > 0)
            {
                remotegatein = context.remotegateindetails.Find(RGId);
                tab.RGIDID = RGId;
                //tab.RGIDID = remotegatein.GIDNO;
                tab.VSLNAME = remotegatein.VSLNAME;
                tab.VSLID = remotegatein.VSLID;
                tab.GINO = remotegatein.GINO;
                tab.GIDNO = remotegatein.GIDNO;
                tab.GICCTLDATE = Convert.ToDateTime(remotegatein.GICCTLDATE).Date;
                tab.GICCTLTIME = Convert.ToDateTime(remotegatein.GICCTLTIME);
                //tab.GITIME = remotegatein.GITIME;
                tab.VOYNO = remotegatein.VOYNO;
                tab.GPLNO = remotegatein.GPLNO;
                tab.IGMNO = remotegatein.IGMNO;
                tab.IMPRTNAME = remotegatein.IMPRTNAME;
                tab.IMPRTID = remotegatein.IMPRTID;
                tab.STMRNAME = remotegatein.STMRNAME;
                tab.STMRID = remotegatein.STMRID;
                tab.CONTNRNO = remotegatein.CONTNRNO;
                tab.CONTNRSID = remotegatein.CONTNRSID;
                tab.CONTNRTID = remotegatein.CONTNRTID;
                tab.PRDTDESC = remotegatein.PRDTDESC;
                tab.PRDTGID = remotegatein.PRDTGID;
                tab.GPWGHT = remotegatein.GPWGHT;//-----------
                tab.GPEAMT = remotegatein.GPEAMT;
                tab.GPAAMT = remotegatein.GPAAMT;
                tab.IGMDATE = remotegatein.IGMDATE;
                tab.BLNO = remotegatein.BLNO;
                tab.GIISOCODE = remotegatein.GIISOCODE;

                //ViewBag.ROWID = new SelectList(context.rowmasters.Where(x => x.DISPSTATUS == 0), "ROWID", "ROWDESC");
                ViewBag.ROWID = new SelectList(context.rowmasters.Where(x => x.DISPSTATUS == 0), "ROWID", "ROWDESC", 6);
                //ViewBag.SLOTID = new SelectList(context.slotmasters.Where(x => x.DISPSTATUS == 0), "SLOTID", "SLOTDESC");
                ViewBag.SLOTID = new SelectList(context.slotmasters.Where(x => x.DISPSTATUS == 0), "SLOTID", "SLOTDESC", 6);
                ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", remotegatein.PRDTGID);
                ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CONTNRTDESC), "CONTNRTID", "CONTNRTDESC", remotegatein.CONTNRTID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID > 1).Where(x => x.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC", remotegatein.CONTNRSID);
                ViewBag.GPPTYPE = new SelectList(context.porttypemaster, "GPPTYPE", "GPPTYPEDESC", remotegatein.GPPTYPE);
                ViewBag.GPMODEID = new SelectList(context.gpmodemasters.Where(x => x.DISPSTATUS == 0 && x.GPMODEID != 5 && x.GPMODEID != 4).OrderBy(x => x.GPMODEDESC), "GPMODEID", "GPMODEDESC", remotegatein.GPMODEID);
            }
            else
            {
                tab.GIDATE = DateTime.Now.Date;
                tab.GITIME = DateTime.Now;
                tab.GITIME = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);
                tab.GICCTLDATE = DateTime.Now.Date;
                tab.GICCTLTIME = DateTime.Now;

                //-------------------Dropdown List--------------------------------------------------//
                //ViewBag.ROWID = new SelectList(context.rowmasters.Where(x => x.DISPSTATUS == 0), "ROWID", "ROWDESC");
                ViewBag.ROWID = new SelectList(context.rowmasters.Where(x => x.DISPSTATUS == 0), "ROWID", "ROWDESC", 6);
                //ViewBag.SLOTID = new SelectList(context.slotmasters.Where(x => x.DISPSTATUS == 0), "SLOTID", "SLOTDESC");
                ViewBag.SLOTID = new SelectList(context.slotmasters.Where(x => x.DISPSTATUS == 0), "SLOTID", "SLOTDESC", 6);
                ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC");
                ViewBag.GPPTYPE = new SelectList(context.porttypemaster, "GPPTYPE", "GPPTYPEDESC", tab.GPPTYPE);
                ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CONTNRTDESC), "CONTNRTID", "CONTNRTDESC");
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.DISPSTATUS == 0 && x.CONTNRSID > 1), "CONTNRSID", "CONTNRSDESC");
                ViewBag.GPMODEID = new SelectList(context.gpmodemasters.Where(x => x.DISPSTATUS == 0 && x.GPMODEID != 5 && x.GPMODEID != 4).OrderBy(x => x.GPMODEDESC), "GPMODEID", "GPMODEDESC", 2);
            }

            //---------------------port--------------------------------------------------//   
            //------------------Escord---------------//

            List<SelectListItem> selectedGPETYPE = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "NO", Value = "0", Selected = false };
            selectedGPETYPE.Add(selectedItem);
            selectedItem = new SelectListItem
            {
                Text = "YES",
                Value = "1",
                Selected = false
            };
            selectedGPETYPE.Add(selectedItem);
            ViewBag.GPETYPE = selectedGPETYPE;
            // ------------------S.Amend-----------//

            List<SelectListItem> selectedGPSTYPE = new List<SelectListItem>();
            SelectListItem selectedItemst = new SelectListItem { Text = "NO", Value = "0", Selected = false };
            selectedGPSTYPE.Add(selectedItemst);
            selectedItemst = new SelectListItem { Text = "YES", Value = "1", Selected = false };
            selectedGPSTYPE.Add(selectedItemst);
            ViewBag.GPSTYPE = selectedGPSTYPE;

            //  ------------------Weightment--------------//
            List<SelectListItem> selectedGPWTYPE = new List<SelectListItem>();
            SelectListItem selectedItemsts = new SelectListItem { Text = "NO", Value = "0", Selected = false };
            selectedGPWTYPE.Add(selectedItemsts);
            selectedItemsts = new SelectListItem { Text = "YES", Value = "1", Selected = true };
            selectedGPWTYPE.Add(selectedItemsts);
            ViewBag.GPWTYPE = selectedGPWTYPE;
            //---------------scanned----------------------------
            List<SelectListItem> selectedGPSCNTYPE = new List<SelectListItem>();
            SelectListItem selectedItemsts1 = new SelectListItem { Text = "NO", Value = "0", Selected = true };
            selectedGPSCNTYPE.Add(selectedItemsts1);
            selectedItemsts1 = new SelectListItem { Text = "YES", Value = "1", Selected = false };
            selectedGPSCNTYPE.Add(selectedItemsts1);
            ViewBag.GPSCNTYPE = selectedGPSCNTYPE;

            List<SelectListItem> selectedGPSCNMTYPE = new List<SelectListItem>();
            SelectListItem selectedItemMtype1 = new SelectListItem { Text = "MISMATCH", Value = "1", Selected = false };
            selectedGPSCNMTYPE.Add(selectedItemMtype1);
            selectedItemMtype1 = new SelectListItem { Text = "CLEAN", Value = "2", Selected = false };
            selectedGPSCNMTYPE.Add(selectedItemMtype1);
            selectedItemMtype1 = new SelectListItem { Text = "NOT SCANNED", Value = "3", Selected = false };
            selectedGPSCNMTYPE.Add(selectedItemMtype1);
            ViewBag.GPSCNMTYPE = selectedGPSCNMTYPE;

            //---------------
            // -----------------------FCL------------------//

            List<SelectListItem> selectedGFCLTYPE = new List<SelectListItem>();
            SelectListItem selectedItemstf = new SelectListItem { Text = "LCL", Value = "0", Selected = false };
            selectedGFCLTYPE.Add(selectedItemstf);
            selectedItemstf = new SelectListItem { Text = "FCL", Value = "1", Selected = true };
            selectedGFCLTYPE.Add(selectedItemstf);
            ViewBag.GFCLTYPE = selectedGFCLTYPE;

            // ------------------Reefer Container Plug in-----------//
            List<SelectListItem> selectedGPRefer_List = new List<SelectListItem>();
            SelectListItem selectedGPRefer = new SelectListItem { Text = "NO", Value = "1", Selected = false };
            selectedGPRefer_List.Add(selectedGPRefer);
            selectedGPRefer = new SelectListItem { Text = "YES", Value = "2", Selected = false };
            selectedGPRefer_List.Add(selectedGPRefer);
            ViewBag.GRADEID = selectedGPRefer_List;

            //----------------------------------End----------------------------------------

            if (GID != 0)//--Edit Mode
            {
                tab = context.gateindetails.Find(GID);

                ginty = "GateIn"; Session["GTY"] = ginty;

                //----------------------selected values in dropdown list------------------------------------//
                ViewBag.SLOTID = new SelectList(context.slotmasters.Where(x => x.DISPSTATUS == 0), "SLOTID", "SLOTDESC", tab.SLOTID);
                ViewBag.ROWID = new SelectList(context.rowmasters.Where(x => x.DISPSTATUS == 0), "ROWID", "ROWDESC", tab.ROWID);
                ViewBag.GPPTYPE = new SelectList(context.porttypemaster, "GPPTYPE", "GPPTYPEDESC", tab.GPPTYPE);
                ViewBag.PRDTGID = new SelectList(context.productgroupmasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.PRDTGDESC), "PRDTGID", "PRDTGDESC", tab.PRDTGID);
                ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CONTNRTDESC), "CONTNRTID", "CONTNRTDESC", tab.CONTNRTID);
                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(x => x.DISPSTATUS == 0 && x.CONTNRSID > 1), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.GPMODEID = new SelectList(context.gpmodemasters.Where(x => x.DISPSTATUS == 0 && x.GPMODEID != 5 && x.GPMODEID != 4).OrderBy(x => x.GPMODEDESC), "GPMODEID", "GPMODEDESC", tab.GPMODEID);

                //-------------------------------escord
                List<SelectListItem> selectedGPETYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GPETYPE) == 0)
                {
                    SelectListItem selectedItem1 = new SelectListItem { Text = "NO", Value = "0", Selected = true };
                    selectedGPETYPE1.Add(selectedItem1);
                    selectedItem1 = new SelectListItem { Text = "YES", Value = "1", Selected = false };
                    selectedGPETYPE1.Add(selectedItem1);
                }
                else
                {
                    SelectListItem selectedItem1 = new SelectListItem { Text = "NO", Value = "0", Selected = false };
                    selectedGPETYPE1.Add(selectedItem1);
                    selectedItem1 = new SelectListItem { Text = "YES", Value = "1", Selected = true };
                    selectedGPETYPE1.Add(selectedItem1);
                }
                ViewBag.GPETYPE = selectedGPETYPE1;

                //------End---

                //---------------scanned----------------------------
                List<SelectListItem> selectedGPSCNTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GPSCNTYPE) == 0)
                {
                    SelectListItem selectedItemsts11 = new SelectListItem { Text = "NO", Value = "0", Selected = true };
                    selectedGPSCNTYPE1.Add(selectedItemsts11);
                    selectedItemsts11 = new SelectListItem { Text = "YES", Value = "1", Selected = false };
                    selectedGPSCNTYPE1.Add(selectedItemsts11);

                }
                else
                {
                    SelectListItem selectedItemsts11 = new SelectListItem { Text = "NO", Value = "0", Selected = false };
                    selectedGPSCNTYPE1.Add(selectedItemsts11);
                    selectedItemsts11 = new SelectListItem { Text = "YES", Value = "1", Selected = true };
                    selectedGPSCNTYPE1.Add(selectedItemsts11);
                }
                ViewBag.GPSCNTYPE = selectedGPSCNTYPE1;
                //-----------End----------


                List<SelectListItem> selectedGPSCNMTYPE2 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GPSCNMTYPE) == 1)
                {
                    SelectListItem selectedItemsts14 = new SelectListItem { Text = "MISMATCH", Value = "1", Selected = true };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                    selectedItemsts14 = new SelectListItem { Text = "CLEAN", Value = "2", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                    selectedItemsts14 = new SelectListItem { Text = "NOT SCANNED", Value = "3", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);

                }
                else if (Convert.ToInt32(tab.GPSCNMTYPE) == 2)
                {
                    SelectListItem selectedItemsts14 = new SelectListItem { Text = "MISMATCH", Value = "1", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                    selectedItemsts14 = new SelectListItem { Text = "CLEAN", Value = "2", Selected = true };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                    selectedItemsts14 = new SelectListItem { Text = "NOT SCANNED", Value = "3", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                }
                else if (Convert.ToInt32(tab.GPSCNMTYPE) == 3)
                {
                    SelectListItem selectedItemsts14 = new SelectListItem { Text = "MISMATCH", Value = "1", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                    selectedItemsts14 = new SelectListItem { Text = "CLEAN", Value = "2", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                    selectedItemsts14 = new SelectListItem { Text = "NOT SCANNED", Value = "3", Selected = true };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                }
                else 
                {
                    SelectListItem selectedItemsts14 = new SelectListItem { Text = "MISMATCH", Value = "1", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                    selectedItemsts14 = new SelectListItem { Text = "CLEAR", Value = "2", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                    selectedItemsts14 = new SelectListItem { Text = "NOT SCANNED", Value = "3", Selected = false };
                    selectedGPSCNMTYPE2.Add(selectedItemsts14);
                }
                ViewBag.GPSCNMTYPE = selectedGPSCNMTYPE2;


                // ------------------S.Amend-----------//
                List<SelectListItem> selectedGPSTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GPSTYPE) == 0)
                {
                    SelectListItem selectedItemst1 = new SelectListItem { Text = "NO", Value = "0", Selected = true };
                    selectedGPSTYPE1.Add(selectedItemst1);
                    selectedItemst1 = new SelectListItem { Text = "YES", Value = "1", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemst1);
                }
                else
                {
                    SelectListItem selectedItemst1 = new SelectListItem { Text = "NO", Value = "0", Selected = false };
                    selectedGPSTYPE1.Add(selectedItemst1);
                    selectedItemst1 = new SelectListItem { Text = "YES", Value = "1", Selected = true };
                    selectedGPSTYPE1.Add(selectedItemst1);
                }
                ViewBag.GPSTYPE = selectedGPSTYPE1;


                //  ------------------Weightment--------------//
                List<SelectListItem> selectedGPWTYPE2 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GPWTYPE) == 0)
                {
                    SelectListItem selectedItemsts2 = new SelectListItem { Text = "NO", Value = "0", Selected = true };
                    selectedGPWTYPE2.Add(selectedItemsts2);
                    selectedItemsts2 = new SelectListItem { Text = "YES", Value = "1", Selected = false };
                    selectedGPWTYPE2.Add(selectedItemsts2);
                    ViewBag.GPWTYPE = selectedGPWTYPE2;
                }
                else
                {
                    SelectListItem selectedItemsts2 = new SelectListItem { Text = "NO", Value = "0", Selected = false };
                    selectedGPWTYPE2.Add(selectedItemsts2);
                    selectedItemsts2 = new SelectListItem { Text = "YES", Value = "1", Selected = true };
                    selectedGPWTYPE2.Add(selectedItemsts2);
                    ViewBag.GPWTYPE = selectedGPWTYPE2;
                }
                ViewBag.GPWTYPE = selectedGPWTYPE2;
                // -----------------------FCL------------------//
                List<SelectListItem> selectedGFCLTYPE3 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GFCLTYPE) == 0)
                {
                    SelectListItem selectedItemstf3 = new SelectListItem { Text = "LCL", Value = "0", Selected = true };
                    selectedGFCLTYPE3.Add(selectedItemstf3);
                    selectedItemstf3 = new SelectListItem { Text = "FCL", Value = "1", Selected = false };
                    selectedGFCLTYPE3.Add(selectedItemstf3);

                }
                else
                {
                    SelectListItem selectedItemstf3 = new SelectListItem { Text = "LCL", Value = "0", Selected = false };
                    selectedGFCLTYPE3.Add(selectedItemstf3);
                    selectedItemstf3 = new SelectListItem { Text = "FCL", Value = "1", Selected = true };
                    selectedGFCLTYPE3.Add(selectedItemstf3);
                }
                ViewBag.GFCLTYPE = selectedGFCLTYPE3;


                List<SelectListItem> selectedGPRefer_List1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.GRADEID) == 1)
                {
                    SelectListItem selectedGPRefer1 = new SelectListItem { Text = "NO", Value = "1", Selected = true };
                    selectedGPRefer_List1.Add(selectedGPRefer1);
                    selectedGPRefer1 = new SelectListItem { Text = "YES", Value = "2", Selected = false };
                    selectedGPRefer_List1.Add(selectedGPRefer1);
                }
                else if (Convert.ToInt32(tab.GRADEID) == 2)
                {
                    SelectListItem selectedGPRefer1 = new SelectListItem { Text = "NO", Value = "1", Selected = false };
                    selectedGPRefer_List1.Add(selectedGPRefer1);
                    selectedGPRefer1 = new SelectListItem { Text = "YES", Value = "2", Selected = true };
                    selectedGPRefer_List1.Add(selectedGPRefer1);
                }
                else
                {
                    SelectListItem selectedGPRefer1 = new SelectListItem { Text = "NO", Value = "1", Selected = false };
                    selectedGPRefer_List1.Add(selectedGPRefer1);
                    selectedGPRefer1 = new SelectListItem { Text = "YES", Value = "2", Selected = false };
                    selectedGPRefer_List1.Add(selectedGPRefer1);
                }
                ViewBag.GRADEID = selectedGPRefer_List1;

            }


            return View(tab);
        }
        #endregion

        #region Save data
        [HttpPost]
        public void saveidata(GateInDetail tab)
        {
            var R_GIDNO = Request.Form.Get("R_GIDNO");

            string todaydt = Convert.ToString(DateTime.Now);
            string todayd = Convert.ToString(DateTime.Now.Date);

            tab.PRCSDATE = DateTime.Now;
            //tab.GIDATE = Convert.ToDateTime(tab.GIDATE).Date;// DateTime.Now.Date;
            //tab.GITIME = Convert.ToDateTime(tab.GITIME);
            //tab.GITIME = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);
            //tab.GICCTLTIME = Convert.ToDateTime(tab.GICCTLTIME);
            //tab.GICCTLDATE = Convert.ToDateTime(tab.GICCTLDATE);
            tab.CONTNRID = 1;
            tab.YRDID = 1;
            tab.COMPYID = Convert.ToInt32(Session["compyid"]);
            tab.SDPTID = 1;
            tab.AVHLNO = tab.VHLNO;
            tab.ESBDATE = DateTime.Now;
            tab.DISPSTATUS = tab.DISPSTATUS;
            tab.EXPRTRID = 0;
            tab.EXPRTRNAME = "-";
            tab.BCHAID = 0;
            tab.UNITID = 0;

            string indate = Convert.ToString(tab.GIDATE);
            if (indate != null || indate != "")
            {
                tab.GIDATE = Convert.ToDateTime(indate).Date;
            }            
            else { tab.GIDATE = DateTime.Now.Date; }

            if (tab.GIDATE > Convert.ToDateTime(todayd))
            {
                tab.GIDATE = Convert.ToDateTime(todayd);
            }

            string intime = Convert.ToString(tab.GITIME);
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

                        tab.GITIME = Convert.ToDateTime(in_datetime);
                    }                    
                    else { tab.GITIME = DateTime.Now; }
                }
                else
                {
                    var in_time = intime;
                    var in_date = indate;

                    if ((in_time.Contains(':')) && (in_date.Contains('/')))
                    {
                        var in_time1 = in_time.Split(':');
                        var in_date1 = in_date.Split('/');

                        string in_datetime = in_date1[2] + "-" + in_date1[1] + "-" + in_date1[0] + "  " + in_time1[0] + ":" + in_time1[1] + ":" + in_time1[2];

                        tab.GITIME = Convert.ToDateTime(in_datetime);
                    }
                    else { tab.GITIME = DateTime.Now; }
                }
            }
            else { tab.GITIME = DateTime.Now; }

            if (tab.GITIME > Convert.ToDateTime(todaydt))
            {
                tab.GITIME = Convert.ToDateTime(todaydt);
            }

            // GATE IN CCT DATE AND TIME
            string cctindate = Convert.ToString(tab.GICCTLDATE);
            if (cctindate != null || cctindate != "")
            {
                tab.GICCTLDATE = Convert.ToDateTime(cctindate).Date;
            }
            else { tab.GICCTLDATE = DateTime.Now.Date; }

            if (tab.GICCTLDATE > Convert.ToDateTime(todayd))
            {
                tab.GICCTLDATE = Convert.ToDateTime(todayd);
            }

            string cctintime = Convert.ToString(tab.GICCTLTIME);
            if ((cctintime != null || cctintime != "") && ((cctindate != null || cctindate != "")))
            {
                if ((cctintime.Contains(' ')) && (cctindate.Contains(' ')))
                {
                    var CCin_time = cctintime.Split(' ');
                    var CCTin_date = cctindate.Split(' ');

                    if ((CCin_time[1].Contains(':')) && (CCTin_date[0].Contains('/')))
                    {
                        var CCTin_time1 = CCin_time[1].Split(':');
                        var CCTin_date1 = CCTin_date[0].Split('/');

                        string CCTin_datetime = CCTin_date1[2] + "-" + CCTin_date1[1] + "-" + CCTin_date1[0] + "  " + CCTin_time1[0] + ":" + CCTin_time1[1] + ":" + CCTin_time1[2];

                        tab.GICCTLTIME = Convert.ToDateTime(CCTin_datetime);
                    }
                    else { tab.GICCTLTIME = DateTime.Now; }
                }
                else { tab.GICCTLTIME = DateTime.Now; }
            }
            else { tab.GICCTLTIME = DateTime.Now; }

            if (tab.GICCTLTIME > Convert.ToDateTime(todaydt))
            {
                tab.GICCTLTIME = Convert.ToDateTime(todaydt);
            }

            if (tab.CUSRID == "" || tab.CUSRID == null)
            {
                if (Session["CUSRID"] != null)
                {
                    tab.CUSRID = Session["CUSRID"].ToString();
                }
                else { tab.CUSRID = ""; }
            }

            if (tab.GPSCNTYPE == 0)
            {
                tab.GPSCNMTYPE = 0;
            }

            // Ensure last modified user ID is set safely even if session CUSRID is not present
            tab.LMUSRID = Session["CUSRID"] != null ? Session["CUSRID"].ToString() : "";
            if (tab.GIDNO != "0" && !string.IsNullOrEmpty(tab.GIDNO))
            {
                // Load original row for logging (no tracking to avoid state conflicts)
                var original = context.gateindetails.AsNoTracking().FirstOrDefault(x => x.GIDID == tab.GIDID);

                context.Entry(tab).Entity.NGIDID = tab.GIDID + 1;
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                // Best-effort logging to SCFS_LOG
                try
                {
                    System.Diagnostics.Debug.WriteLine($"SAVE METHOD CALLED: GIDID={tab.GIDID}, GIDNO={tab.GIDNO}");
                    if (original != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"ORIGINAL RECORD FOUND: GIDID={original.GIDID}, calling LogGateInEdits");
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
                    // Log the error for debugging
                    System.Diagnostics.Debug.WriteLine($"Edit logging failed: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                }

                Response.Write("saved");
            }

            else
            {

                string sqry = "SELECT *FROM GATEINDETAIL  WHERE VOYNO='" + tab.VOYNO + "' And IGMNO='" + tab.IGMNO + "' ";
                sqry += " And GPLNO='" + tab.GPLNO + "' And CONTNRNO ='" + tab.CONTNRNO + "' And COMPYID=" + tab.COMPYID + " And SDPTID=1";
                var sl = context.Database.SqlQuery<GateInDetail>(sqry).ToList();

                if (sl.Count > 0)
                {
                    Response.Write("exists");
                }
                else
                {
                    string sqry1 = "SELECT *FROM GATEINDETAIL  WHERE VOYNO='" + tab.VOYNO + "' And IGMNO='" + tab.IGMNO + "' ";
                    sqry1 += " And GPLNO='" + tab.GPLNO + "' And CONTNRNO ='" + tab.CONTNRNO + "' And COMPYID=" + tab.COMPYID + " And SDPTID=1";
                    var sl1 = context.Database.SqlQuery<GateInDetail>(sqry1).ToList();

                    if (sl1.Count > 0)

                    if (R_GIDNO != null)
                        tab.RGIDID = Convert.ToInt32(R_GIDNO);

                    context.gateindetails.Add(tab);
                    context.SaveChanges();
                    context.Entry(tab).Entity.NGIDID = tab.GIDID + 1;
                    context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();

                    ///*/.....delete remote gate in*/
                    if (R_GIDNO != "0")
                    {
                        RemoteGateIn remotegatein = context.remotegateindetails.Find(Convert.ToInt32(R_GIDNO));
                        remotegatein.AGIDNO = tab.GIDNO;

                        //context.remotegateindetails.Remove(remotegatein);
                        context.SaveChanges();

                    }/*end*/

                    //-----second record-----------------------------// 
                    tab.GITIME = tab.GITIME;
                    tab.TRNSPRTNAME = tab.TRNSPRTNAME;
                    tab.AVHLNO = tab.VHLNO;
                    tab.DRVNAME = tab.DRVNAME;
                    tab.GPREFNO = tab.GPREFNO;
                    tab.IMPRTID = tab.IMPRTID;// 0;
                    tab.IMPRTNAME = tab.IMPRTNAME;//  "-";
                    tab.STMRID = tab.STMRID;// 0;
                    tab.STMRNAME = tab.STMRNAME;// "-";
                    tab.CONDTNID = 0;
                    tab.CONTNRNO = "-";
                    tab.CONTNRTID = 0;
                    tab.CONTNRID = 0;
                    tab.CONTNRSID = 0;
                    tab.LPSEALNO = "-";
                    tab.CSEALNO = "-";
                    tab.YRDID = 0;
                    tab.VSLID = 0;
                    tab.VSLNAME = "-";
                    tab.VOYNO = "-";
                    tab.PRDTGID = 0;
                    tab.PRDTDESC = "-";
                    tab.UNITID = 0;
                    tab.GPLNO = "-";
                    tab.GPWGHT = 0;
                    tab.GPEAMT = 0;
                    tab.GPAAMT = 0;
                    tab.IGMNO = tab.IGMNO;
                    tab.GIISOCODE = tab.GIISOCODE;
                    tab.GIDMGDESC = tab.GIDMGDESC;
                    tab.GPWTYPE = 0;
                    tab.GPSTYPE = 0;
                    tab.RGIDID = 0;
                    tab.GPETYPE = 0;
                    tab.ROWID = 0;
                    tab.SLOTID = 0;
                    tab.DISPSTATUS = 0;
                    tab.GIVHLTYPE = 0;
                    tab.TRNSPRTID = 0;
                    tab.NGIDID = 0;
                    tab.BOEDID = tab.GIDID;
                    tab.BLNO = tab.BLNO;
                    tab.BOENO = tab.BOENO;
                    tab.CFSNAME = tab.CFSNAME;


                    var GINO = Request.Form.Get("GINO");

                    //if (context.gateindetails.Where(u => u.GINO == Convert.ToInt32(GINO)).Count()!=0) 
                    tab.GINO = Convert.ToInt32(Autonumber.autonum("gateindetail", "GINO", "GINO <> 0 AND SDPTID = 1 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                    //  else tab.GINO = 1;
                    int anoo = tab.GINO;
                    string prfxx = string.Format("{0:D5}", anoo);
                    tab.GIDNO = prfxx.ToString();
                    context.gateindetails.Add(tab);
                    context.SaveChanges();

                }
                Response.Write("saved");
                //Response.Redirect("Index");
            }
        }

        public void savedata(GateInDetail tab)
        {
            var R_GIDNO = Request.Form.Get("R_GIDNO");

            string todaydt = Convert.ToString(DateTime.Now);
            string todayd = Convert.ToString(DateTime.Now.Date);

            tab.PRCSDATE = DateTime.Now;
            tab.GIDATE = Convert.ToDateTime(tab.GIDATE).Date;
            tab.GITIME = Convert.ToDateTime(tab.GITIME);
            tab.GITIME = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);
            tab.GICCTLDATE = Convert.ToDateTime(tab.GICCTLTIME).Date;
            tab.CONTNRID = 1;
            tab.YRDID = 1;
            tab.COMPYID = Convert.ToInt32(Session["compyid"]);
            tab.SDPTID = 1;
            tab.AVHLNO = tab.VHLNO;
            tab.ESBDATE = DateTime.Now;
            tab.DISPSTATUS = tab.DISPSTATUS;
            tab.LMUSRID = Session["CUSRID"].ToString();

            if (tab.GIDATE > Convert.ToDateTime(todayd))
            {
                tab.GIDATE = Convert.ToDateTime(todayd);
            }

            if (tab.GITIME > Convert.ToDateTime(todaydt))
            {
                tab.GITIME = Convert.ToDateTime(todaydt);
            }

            if (tab.GICCTLDATE > Convert.ToDateTime(todayd))
            {
                tab.GICCTLDATE = Convert.ToDateTime(todayd);
            }

            if (tab.GICCTLTIME > Convert.ToDateTime(todaydt))
            {
                tab.GICCTLTIME = Convert.ToDateTime(todaydt);
            }

            if (tab.CUSRID == "" || tab.CUSRID == null)
            {
                if (Session["CUSRID"] != null)
                {
                    tab.CUSRID = Session["CUSRID"].ToString();
                }
                else { tab.CUSRID = "0"; }
            }
            tab.BOEDATE = Convert.ToDateTime(tab.BOEDATE).Date;

            if (tab.GIDNO != "0" && !string.IsNullOrEmpty(tab.GIDNO))
            {
                // Load original row for logging (no tracking to avoid state conflicts)
                var original = context.gateindetails.AsNoTracking().FirstOrDefault(x => x.GIDID == tab.GIDID);

                context.Entry(tab).Entity.NGIDID = tab.GIDID + 1;
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                // Best-effort logging to SCFS_LOG
                try
                {
                    if (original != null)
                    {
                        LogGateInEdits(original, tab, Session["CUSRID"] != null ? Session["CUSRID"].ToString() : "");
                    }
                }
                catch (Exception ex)
                {
                    // Log the error for debugging
                    System.Diagnostics.Debug.WriteLine($"Edit logging failed: {ex.Message}");
                }
            }

            else
            {

                //----------------first record------------------//              

                tab.GINO = Convert.ToInt32(Autonumber.autonum("gateindetail", "GINO", "GINO <> 0 AND SDPTID = 1 and compyid = " + Convert.ToInt32(Session["compyid"]) + "").ToString());
                int ano = tab.GINO;
                string prfx = string.Format("{0:D5}", ano);
                tab.GIDNO = prfx.ToString();


                if (R_GIDNO != null)
                    tab.RGIDID = Convert.ToInt32(R_GIDNO);

                context.gateindetails.Add(tab);
                context.SaveChanges();
                context.Entry(tab).Entity.NGIDID = tab.GIDID + 1;
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                ///*/.....delete remote gate in*/
                if (R_GIDNO != "0")
                {
                    RemoteGateIn remotegatein = context.remotegateindetails.Find(Convert.ToInt32(R_GIDNO));
                    remotegatein.GIDNO = tab.GIDNO;

                    //context.remotegateindetails.Remove(remotegatein);
                    context.SaveChanges();

                }/*end*/

                //-----second record-----------------------------// 
                tab.GIDATE = Convert.ToDateTime(tab.GIDATE).Date;
                tab.GITIME = Convert.ToDateTime(tab.GITIME);
                tab.GITIME = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);
                tab.TRNSPRTNAME = tab.TRNSPRTNAME;
                tab.AVHLNO = tab.VHLNO;
                tab.DRVNAME = tab.DRVNAME;
                tab.GPREFNO = tab.GPREFNO;
                tab.IMPRTID = 0;
                tab.IMPRTNAME = "-";
                tab.STMRID = 0;
                tab.STMRNAME = "-";
                tab.CONDTNID = 0;
                tab.CONTNRNO = "-";
                tab.CONTNRTID = 0;
                tab.CONTNRID = 0;
                tab.CONTNRSID = 0;
                tab.LPSEALNO = "-";
                tab.CSEALNO = "-";
                tab.YRDID = 0;
                tab.VSLID = 0;
                tab.VSLNAME = "-";
                tab.VOYNO = "-";
                tab.PRDTGID = 0;
                tab.PRDTDESC = "-";
                tab.UNITID = 0;
                tab.GPLNO = "-";
                tab.GPWGHT = 0;
                tab.GPEAMT = 0;
                tab.GPAAMT = 0;
                tab.IGMNO = tab.IGMNO;
                tab.GIISOCODE = tab.GIISOCODE;
                tab.GIDMGDESC = tab.GIDMGDESC;
                tab.GPWTYPE = 0;
                tab.GPSTYPE = 0;
                tab.GPETYPE = 0;
                //tab.ROWID = 0;
                tab.SLOTID = 0;
                tab.DISPSTATUS = 0;
                tab.GIVHLTYPE = 0;
                tab.TRNSPRTID = 0;
                tab.NGIDID = 0;
                tab.BOEDID = tab.GIDID;
                tab.BLNO = tab.BLNO;
                tab.BOENO = tab.BOENO;
                tab.CFSNAME = tab.CFSNAME;


                var GINO = Request.Form.Get("GINO");

                //if (context.gateindetails.Where(u => u.GINO == Convert.ToInt32(GINO)).Count()!=0) 
                tab.GINO = Convert.ToInt32(Autonumber.autonum("gateindetail", "GINO", "GINO <> 0 AND SDPTID = 1 and compyid=" + Convert.ToInt32(Session["compyid"]) + "").ToString());
                //  else tab.GINO = 1;
                int anoo = tab.GINO;
                string prfxx = string.Format("{0:D5}", anoo);
                tab.GIDNO = prfxx.ToString();
                context.gateindetails.Add(tab);
                context.SaveChanges();

            }

            Response.Redirect("Index");

        }
        #endregion

        public void CONT_Duplicate_Check(string VOYNO, string GPLNO, string IGMNO, string CONTNRNO, string date)
        {
            VOYNO = Request.Form.Get("VOYNO");
            GPLNO = Request.Form.Get("GPLNO");
            IGMNO = Request.Form.Get("IGMNO");
            CONTNRNO = Request.Form.Get("CONTNRNO");
            date = Request.Form.Get("GIDATE");

            string temp = ContainerNo_Check.recordCount(VOYNO, IGMNO, GPLNO, CONTNRNO);
            if (temp != "PROCEED")
            {
                Response.Write("Container number already exists");
            }
            else
            {
                var query = context.Database.SqlQuery<PR_IMPORT_GATEIN_CONTAINER_CHK_ASSGN_Result>("Exec PR_IMPORT_GATEIN_CONTAINER_CHK_ASSGN @PCONTNRNO='" + CONTNRNO + "'").ToList();
                if (query.Count > 0)
                {
                    DateTime gedate = Convert.ToDateTime(query[0].GEDATE);
                    DateTime gidate = Convert.ToDateTime(date);
                    var s = (gedate - gidate).Days;
                    //   Response.Write(s);
                    //if (s != 0)
                    //{
                    //    Response.Write("DATE INCORRECT");
                    //}
                    if (gidate >= gedate)
                    {
                        Response.Write("PROCEED");
                    }
                    else
                    {
                        Response.Write("DATE INCORRECT");
                    }
                    //if (s < 10)
                    //{
                    //    Response.Write("DATE INCORRECT");
                    //}
                }
                else
                {
                    Response.Write("PROCEED");
                }
            }

        }

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

        public void CheckAmt()
        {
            string vno = Request.Form.Get("vno");

            string amt = Request.Form.Get("amt");

            if (Convert.ToInt16(vno) == 1 && Convert.ToDecimal(amt) == 0)
            {
                Response.Write("Escord Amount is Required");

            }
            else
            {

            }

        }

        #region Auto Vessel
        public JsonResult AutoVessel(string term)
        {
            var result = (from vessel in context.vesselmasters
                          where vessel.VSLDESC.ToLower().Contains(term.ToLower())
                          select new { vessel.VSLDESC, vessel.VSLID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete transporter Name
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
        public JsonResult AutoImpoter(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 1).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Cha Name
        public JsonResult AutoChaName(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete slot Name
        public JsonResult AutoSlot(string term)
        {
            var result = (from slot in context.slotmasters.Where(x => x.DISPSTATUS == 0)
                          where slot.SLOTDESC.ToLower().Contains(term.ToLower())
                          select new { slot.SLOTID, slot.SLOTDESC }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSlot(int id)
        {
            
            var slot = (from a in context.slotmasters.Where(x => x.DISPSTATUS == 0 && x.ROWID == id) select a).ToList();
            return Json(slot, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult GetVehicle(int id)//vehicl
        {
            var query = (from a in context.vehiclemasters.Where(x => x.DISPSTATUS == 0) where a.TRNSPRTID == id select a).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }


        //-------------Autocomplete Vehicle No
        public JsonResult VehicleNo(string term)
        {
            var result = (from r in context.vehiclemasters.Where(x => x.DISPSTATUS == 0)
                          join t in context.categorymasters.Where(x => x.DISPSTATUS == 0)
                          on r.TRNSPRTID equals t.CATEID into y
                          from k in y.DefaultIfEmpty()
                          where r.VHLMDESC.ToLower().Contains(term.ToLower())
                          select new { r.VHLMDESC, r.TRNSPRTID, k.CATENAME }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }//-----End of vehicle

        public ActionResult checkpostdata(int? id = 0)
        {
            String idx = Request.Form.Get("id");

            RemoteGateIn remotegateindetails = context.remotegateindetails.Find(idx);
            if (remotegateindetails == null)
                return HttpNotFound();
            else
                return View(remotegateindetails);
        }

        public ActionResult test1()
        {
            String idx = Request.Form.Get("id");
            GateInDetail tab = new GateInDetail();
            RemoteGateIn remote = new RemoteGateIn();
            remote = context.remotegateindetails.Find(idx);
            return View();
        }

        [Authorize(Roles = "ImportGateInPrint")]
        public void PrintView(int? id = 0)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "IMPORTGATEIN", Convert.ToInt32(id), Session["CUSRID"].ToString());

            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Import_GateIn.rpt");
                cryRpt.RecordSelectionFormula = "{VW_IMPORT_GATE_IN_PRINT_ASSGN.KUSRID} ='" + Session["CUSRID"].ToString() + "' and {VW_IMPORT_GATE_IN_PRINT_ASSGN.GIDNO} =" + id;

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

        [Authorize(Roles = "ImportGateInPrint")]
        public void TPrintView(int? id = 0)/*truck*/
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "IMPORTTRUCKIN", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Import_TruckIn.rpt");
                cryRpt.RecordSelectionFormula = "{VW_IMPORT_GATE_IN_PRINT_ASSGN.KUSRID} ='" + Session["CUSRID"].ToString() + "' and {VW_IMPORT_GATE_IN_PRINT_ASSGN.GIDNO} =" + id;

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


        //--------Delete Row----------
        [Authorize(Roles = "ImportGateInDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                GateInDetail gateindetails = new GateInDetail();
                //var sql = context.Database.SqlQuery<int>("SELECT GIDNO from GATEINDETAIL where BOEDID=" + Convert.ToInt32(id)).ToList();
                //var gidid = (sql[0]).ToString();

                //gateindetails = context.gateindetails.Find(Convert.ToInt32(gidid));
                gateindetails = context.gateindetails.Find(Convert.ToInt32(id));
                context.gateindetails.Remove(gateindetails);
                gateindetails = context.gateindetails.Find(Convert.ToInt32(id));
                context.gateindetails.Remove(gateindetails);
                context.SaveChanges();

                Response.Write("Deleted Successfully ...");
            }
            else

                Response.Write(temp);

        }//-----End of Delete Row

        // ========================= Edit Logging (SCFS_LOG) =========================
        private void LogGateInEdits(GateInDetail before, GateInDetail after, string userId)
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
                // system/housekeeping fields
                "NGIDID", "PRCSDATE", "ESBDATE", "LMUSRID", "CUSRID",
                // the unwanted gate pass dimension/weight fields
                "GPTWGHT", "GPHEIGHT", "GPWIDTH", "GPLENGTH", "GPCBM", "GPGWGHT", "GPNWGHT", "GPNOP"
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
                    WHERE [GIDNO] = @GIDNO", sql))
                {
                    cmd.Parameters.AddWithValue("@GIDNO", Convert.ToInt32(after.GIDNO));
                    sql.Open();
                    var obj = cmd.ExecuteScalar();
                    if (obj != null && obj != DBNull.Value)
                        nextVersion = Convert.ToInt32(obj);
                }
            }
            catch { /* ignore logging version errors */ }

            var props = typeof(GateInDetail).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in props)
            {
                if (!p.CanRead) continue;
                // Skip complex navigation properties
                if (p.PropertyType.IsClass && p.PropertyType != typeof(string) && !p.PropertyType.IsValueType)
                    continue;
                if (exclude.Contains(p.Name)) continue;

                // If this is an '*ID' field and a corresponding string name property exists,
                // skip logging the ID to avoid noisy numeric codes (e.g., STMRID vs STMRNAME)
                if (p.Name.EndsWith("ID", StringComparison.OrdinalIgnoreCase))
                {
                    var baseName = p.Name.Substring(0, p.Name.Length - 2); // remove 'ID'
                    var nameProp = props.FirstOrDefault(q =>
                        q.PropertyType == typeof(string) &&
                        (
                            q.Name.Equals(baseName, StringComparison.OrdinalIgnoreCase) ||
                            q.Name.Equals(baseName + "NAME", StringComparison.OrdinalIgnoreCase) ||
                            (q.Name.EndsWith("NAME", StringComparison.OrdinalIgnoreCase) && q.Name.StartsWith(baseName, StringComparison.OrdinalIgnoreCase))
                        ));
                    if (nameProp != null) continue;
                }

                var ov = p.GetValue(before, null);
                var nv = p.GetValue(after, null);

                if (BothNull(ov, nv)) continue;

                // Compare by underlying type to avoid logging formatting-only differences
                var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                bool changed;

                if (type == typeof(decimal))
                {
                    var d1 = ToNullableDecimal(ov) ?? 0m;
                    var d2 = ToNullableDecimal(nv) ?? 0m;
                    // skip if both are zero-equivalent
                    if (d1 == 0m && d2 == 0m) continue;
                    changed = d1 != d2;
                }
                else if (type == typeof(double) || type == typeof(float))
                {
                    var d1 = Convert.ToDouble(ov ?? 0.0);
                    var d2 = Convert.ToDouble(nv ?? 0.0);
                    if (Math.Abs(d1) < 1e-9 && Math.Abs(d2) < 1e-9) continue;
                    changed = Math.Abs(d1 - d2) > 1e-9;
                }
                else if (type == typeof(int) || type == typeof(long) || type == typeof(short))
                {
                    var i1 = Convert.ToInt64(ov ?? 0);
                    var i2 = Convert.ToInt64(nv ?? 0);
                    if (i1 == 0 && i2 == 0) continue;
                    changed = i1 != i2;
                }
                else if (type == typeof(DateTime))
                {
                    var t1 = (ov as DateTime?) ?? default(DateTime);
                    var t2 = (nv as DateTime?) ?? default(DateTime);
                    // ignore millisecond differences
                    t1 = new DateTime(t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second);
                    t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
                    changed = t1 != t2;
                }
                else if (type == typeof(string))
                {
                    var s1 = (Convert.ToString(ov) ?? string.Empty).Trim();
                    var s2 = (Convert.ToString(nv) ?? string.Empty).Trim();
                    // treat "-" and empty as same default; skip if both default/empty
                    bool def1 = string.IsNullOrEmpty(s1) || s1 == "-" || s1 == "0" || s1 == "0.0" || s1 == "0.00" || s1 == "0.000" || s1 == "0.0000";
                    bool def2 = string.IsNullOrEmpty(s2) || s2 == "-" || s2 == "0" || s2 == "0.0" || s2 == "0.00" || s2 == "0.000" || s2 == "0.0000";
                    if (def1 && def2) continue;
                    changed = !string.Equals(s1, s2, StringComparison.Ordinal);
                }
                else
                {
                    var s1 = FormatVal(ov);
                    var s2 = FormatVal(nv);
                    changed = !string.Equals(s1, s2, StringComparison.Ordinal);
                }

                if (!changed) continue;

                var os = FormatVal(ov);
                var ns = FormatVal(nv);

                var versionLabel = $"V{nextVersion}-{after.GIDNO}"; // Version label e.g., V1-518084
                InsertEditLogRow(cs.ConnectionString, Convert.ToInt32(after.GIDNO), p.Name, os, ns, userId, versionLabel, "ImportGateIn");
            }
        }

        private static string FormatVal(object value)
        {
            if (value == null) return null;
            if (value is DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm:ss");
            if (value is DateTime?)
            {
                var ndt = (DateTime?)value;
                return ndt.HasValue ? ndt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null;
            }
            if (value is decimal dec) return dec.ToString("0.####");
            var ndecs = value as decimal?;
            if (ndecs.HasValue) return ndecs.Value.ToString("0.####");
            return Convert.ToString(value);
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

        private static void InsertEditLogRow(string connectionString, int gidid, string fieldName, string oldValue, string newValue, string changedBy, string versionLabel, string modules)
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
                        cmd.Parameters.AddWithValue("@GIDNO", gidid);
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

        // Test method to verify edit logging is working
        public JsonResult TestEditLogging(int gidid)
        {
            try
            {
                var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
                if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
                {
                    return Json(new { success = false, message = "SCFSERP_EditLog connection string not found" }, JsonRequestBehavior.AllowGet);
                }

                using (var testContext = new SCFSERPContext())
                {
                    var record = testContext.gateindetails.FirstOrDefault(x => x.GIDID == gidid);
                    if (record == null)
                    {
                        return Json(new { success = false, message = $"No record found with GIDID {gidid}" }, JsonRequestBehavior.AllowGet);
                    }

                    // Test inserting a log entry
                    InsertEditLogRow(cs.ConnectionString, gidid, "TEST_FIELD", "OLD_VALUE", "NEW_VALUE", 
                        Session["CUSRID"]?.ToString() ?? "TEST_USER", "1", "ImportGateIn");

                    // Test reading back the log entry
                    using (var sql = new SqlConnection(cs.ConnectionString))
                    {
                        sql.Open();
                        using (var cmd = new SqlCommand("SELECT COUNT(*) FROM [dbo].[GateInDetailEditLog] WHERE [GIDNO] = @GIDNO AND [FieldName] = 'TEST_FIELD'", sql))
                        {
                            cmd.Parameters.AddWithValue("@GIDNO", gidid);
                            var count = (int)cmd.ExecuteScalar();
                            
                            return Json(new { 
                                success = true, 
                                message = $"Edit logging test successful. Found {count} test records for GIDID {gidid}",
                                connectionString = cs.ConnectionString,
                                gidid = gidid,
                                gidno = record.GIDNO
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = $"Edit logging test failed: {ex.Message}",
                    innerException = ex.InnerException?.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // Debug method to check if logging is being called during save
        public JsonResult DebugEditLogging()
        {
            try
            {
                var cs = ConfigurationManager.ConnectionStrings["SCFSERP_EditLog"];
                var results = new List<object>();
                
                results.Add(new { 
                    check = "Connection String", 
                    status = cs != null ? "Found" : "Missing",
                    value = cs?.ConnectionString ?? "NULL"
                });

                if (cs != null)
                {
                    try
                    {
                        using (var sql = new SqlConnection(cs.ConnectionString))
                        {
                            sql.Open();
                            results.Add(new { check = "Database Connection", status = "Success", value = "Connected" });
                            
                            // Check if table exists
                            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'GateInDetailEditLog'", sql))
                            {
                                var tableExists = (int)cmd.ExecuteScalar() > 0;
                                results.Add(new { check = "Table Exists", status = tableExists ? "Yes" : "No", value = tableExists.ToString() });
                            }
                            
                            // Check total records in table
                            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM [dbo].[GateInDetailEditLog]", sql))
                            {
                                var totalRecords = (int)cmd.ExecuteScalar();
                                results.Add(new { check = "Total Records", status = "Info", value = totalRecords.ToString() });
                            }
                            
                            // Check recent records
                            using (var cmd = new SqlCommand("SELECT TOP 5 GIDNO, FieldName, ChangedBy, ChangedOn FROM [dbo].[GateInDetailEditLog] ORDER BY ChangedOn DESC", sql))
                            using (var reader = cmd.ExecuteReader())
                            {
                                var recentRecords = new List<object>();
                                while (reader.Read())
                                {
                                    recentRecords.Add(new {
                                        GIDNO = reader["GIDNO"],
                                        FieldName = reader["FieldName"],
                                        ChangedBy = reader["ChangedBy"],
                                        ChangedOn = reader["ChangedOn"]
                                    });
                                }
                                results.Add(new { check = "Recent Records", status = "Info", value = recentRecords });
                            }
                        }
                    }
                    catch (Exception dbEx)
                    {
                        results.Add(new { check = "Database Connection", status = "Failed", value = dbEx.Message });
                    }
                }

                return Json(new { success = true, results = results }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}