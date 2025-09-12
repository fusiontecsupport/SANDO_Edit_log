using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("SHIPPINGBILLMASTER")]
    public class ShippingBillMaster
    {
        [Key]
        public int SBMID { get; set; }
        public int COMPYID { get; set; }
        public int SBMNO { get; set; }
        public string SBMDNO { get; set; }
        public DateTime SBMDATE { get; set; }
        public DateTime SBMTIME { get; set; }
        public int EXPRTID { get; set; }
        public string EXPRTNAME { get; set; }
        public int CHAID { get; set; }
        public string CHANAME { get; set; }
        public string SBMRMKS { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public DateTime PRCSDATE { get; set; }
        public short DISPSTATUS { get; set; }
    }
}