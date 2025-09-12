using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("STUFFINGPRODUCTDETAIL")]
    public class StuffingProductDetail
    {
        [Key]
        public int STFPID { get; set; }
        public int STFDID { get; set; }
        public int SBDID { get; set; }
        public string STFDSBNO { get; set; }
        public string STFDSBDNO { get; set; }
        public DateTime STFDSBDATE { get; set; }
        public DateTime STFDSBDDATE { get; set; }
        public Nullable<decimal> STFDNOP { get; set; }
        public Nullable<decimal> STFDQTY { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }

        public Nullable<Int16> DISPSTATUS { get; set; }
        public Nullable<DateTime> PRCSDATE { get; set; }
        public string STFCORDNO { get; set; }


        public int ESBDID { get; set; }

        public string ESBD_SBILLNO { get; set; }

        public Nullable<DateTime> ESBD_SBILLDATE { get; set; }
    }
}