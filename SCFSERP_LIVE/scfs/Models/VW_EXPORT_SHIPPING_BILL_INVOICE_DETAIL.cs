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
    [Table("VW_EXPORT_SHIPPING_BILL_INVOICE_DETAIL")]
    public class VW_EXPORT_SHIPPING_BILL_INVOICE_DETAIL
    {
        [Key]
        public int TRANMID { get; set; }
        public System.DateTime TRANDATE { get; set; }
        public System.DateTime TRANTIME { get; set; }
        public string TRANDNO { get; set; }
        public string TRANREFNO { get; set; }
        public string TRANREFNAME { get; set; }
        public System.DateTime TRANREFDATE { get; set; }
        public decimal TRANNAMT { get; set; }
        public int TRANREFID { get; set; }

    }
}