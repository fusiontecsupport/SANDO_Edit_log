using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("EXPORTSHIPPINGBILLDETAIL")]
    public class ExportShippingBillDetail
    {
        [Key]
        public int ESBDID { get; set; }

        public int ESBMID { get; set; }

        public string ESBD_SBILLNO { get; set; }

        public Nullable<DateTime> ESBD_SBILLDATE { get; set; }

        public string ESBOINVNO { get; set; }

        public Nullable<DateTime> ESBOINVDATE { get; set; }

        public Nullable<decimal> ESBOINVAMT { get; set; }

        public Nullable<decimal> ESBOINVFOBAMT { get; set; }

        public string ESBDDPNAME { get; set; }

        public string PRDTDESC { get; set; }

        public Nullable<decimal> ESBMNOP { get; set; }

        public Nullable<decimal> ESBMQTY { get; set; }

        public Nullable<decimal> ESBMWGHT { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Nullable<DateTime> PRCSDATE { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public Nullable<int> GIDID { get; set; }

    }
}