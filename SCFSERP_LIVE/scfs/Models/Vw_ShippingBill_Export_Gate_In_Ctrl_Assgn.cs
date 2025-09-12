using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("[VW_SHIPPINGBILL_EXPORT_GATE_IN_CTRL_ASSGN]")]
    public class Vw_ShippingBill_Export_Gate_In_Ctrl_Assgn
    {
        [Key]
        public int GIDID { get; set; }
        public DateTime GIDATE { get; set; }
        public int PRDTGID { get; set; }
        public int PRDTTID { get; set; }
        public decimal GPNOP { get; set; }
        public decimal GPWGHT { get; set; }
        public string PRDTDESC { get; set; }
        public string PRDTGDESC { get; set; }
        public decimal SBDNOP { get; set; }
    }
}