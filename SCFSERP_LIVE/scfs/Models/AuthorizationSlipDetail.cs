using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("AUTHORIZATIONSLIPDETAIL")]
    public class AuthorizationSlipDetail
    {
        [Key]
        public int ASLDID { get; set; }
        public int ASLMID { get; set; }
        public int OSDID { get; set; }

        public int GIDID { get; set; }
        public Int32? LCATEID { get; set; }
        public short ASLDTYPE { get; set; }
        public short ASLLTYPE { get; set; }
        public short ASLOTYPE { get; set; }
        public string VHLNO { get; set; }
        public string DRVNAME { get; set; }
        public string ASLFDESC { get; set; }
        public string ASLTDESC { get; set; }

        public Nullable<DateTime> ASLDODATE { get; set; }
        public int STFDID { get; set; }

        public string CSEALNO { get; set; }
        public string ASEALNO { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }

        public int SLTID { get; set; }
    }

    [Table("IMPORT_DESTUFF_SLIP_OPERATION_TYPE")]
    public class ImportDestuffSlipOperation
    {
        [Key]
        public int OPRTYPE { get; set; }
        public string OPRTYPEDESC { get; set; }
    }
}