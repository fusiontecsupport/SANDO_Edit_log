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
    [Table("VW_EXPORT_SHIPPINGBILL_WISE_CARTINGORDER_DETAIL_ASSGN")]
    public class VW_EXPORT_SHIPPINGBILL_WISE_CARTINGORDER_DETAIL_ASSGN
    {
        [Key]
        public int SBDID { get; set; }
        public int ESBMID { get; set; }
        public System.DateTime SBMDATE { get; set; }
        public string SBMDNO { get; set; }
        public Nullable<decimal> SBDNOP { get; set; }
        public Nullable<decimal> SBDQTY { get; set; }
        public string PRDTDESC { get; set; }
        public string TRUCKNO { get; set; }
        public string ESBMDNO { get; set; }
        public System.DateTime SBDDATE { get; set; }
        public int GIDID { get; set; }
        public string SBDDNO { get; set; }

        public string ESBD_SBILLNO { get; set; }
        public Nullable<System.DateTime> ESBOINVDATE { get; set; }
        public Nullable<decimal> ESBMNOP { get; set; }
        public Nullable<decimal> ESBMQTY { get; set; }
    }
}