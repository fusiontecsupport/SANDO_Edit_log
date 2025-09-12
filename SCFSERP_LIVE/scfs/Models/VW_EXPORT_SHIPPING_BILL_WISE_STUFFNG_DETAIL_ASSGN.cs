using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Models
{
    [Table("VW_EXPORT_SHIPPING_BILL_WISE_STUFFNG_DETAIL_ASSGN")]
    public class VW_EXPORT_SHIPPING_BILL_WISE_STUFFNG_DETAIL_ASSGN
    {
        [Key]
        public short STFTYPE { get; set; }
        public string PRDTDESC { get; set; }
        public int STFPID { get; set; }
        public int STFDID { get; set; }
        public int SBDID { get; set; }
        public Nullable<decimal> STFDNOP { get; set; }
        public string STFCORDNO { get; set; }
        public Nullable<decimal> STFDQTY { get; set; }
        public System.DateTime STFDSBDATE { get; set; }
        public System.DateTime STFDSBDDATE { get; set; }
        public string STFDSBDNO { get; set; }
        public string STFDSBNO { get; set; }
        public string STFMDNO { get; set; }
        public int GIDID { get; set; }
        public int CHAID { get; set; }
        public int ESBMID { get; set; }
        public int STFMID { get; set; }
        public System.DateTime STFMDATE { get; set; }
    }
}