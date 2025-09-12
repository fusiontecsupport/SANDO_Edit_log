using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("REPORTING_PARTY_TYPE")]
    public class ReportingPartyType
    {
        [Key]
        public int RPTYPEID { get; set; }
        public string RPTYPEDESC { get; set; }
        public int CUSRID { get; set; }
        public int LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }

    [Table("EQUIPMENT_TYPE_MASTER")]
    public class Equipment_Type_Master
    {
        [Key]
        public int EQTID { get; set; }
        public string EQTDESC { get; set; }
        public int CUSRID { get; set; }
        public int LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }

    [Table("EQUIPMENT_LOAD_STATUS")]
    public class Equipment_Load_Status
    {
        [Key]
        public int ELSID { get; set; }
        public string ELSDESC { get; set; }
        public int CUSRID { get; set; }
        public int LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }

    [Table("EQUIPMENT_SEAL_TYPE")]
    public class Equipment_Seal_Type
    {
        [Key]
        public int ESTYPEID { get; set; }
        public string ESTYPEDESC { get; set; }
        public string ESTYPECODE { get; set; }
        public int CUSRID { get; set; }
        public int LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }

    [Table("EQUIPMENT_STATUS")]
    public class Equipment_Status
    {
        [Key]
        public int ESID { get; set; }
        public string ESDESC { get; set; }
        public string ESCODE { get; set; }
        public int CUSRID { get; set; }
        public int LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }

    [Table("EQUIPMENT_UQC")]
    public class Equipment_UQC
    {
        [Key]
        public int EUQCID { get; set; }
        public string EUQCDESC { get; set; }
        public string EUQCCODE { get; set; }
        public int CUSRID { get; set; }
        public int LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }

    [Table("FINAL_DESTINATION_CODE")]
    public class Final_Destination_Code
    {
        [Key]
        public int FDID { get; set; }
        public string FDDESC { get; set; }
        public string FDCODE { get; set; }
        public int CUSRID { get; set; }
        public int LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}