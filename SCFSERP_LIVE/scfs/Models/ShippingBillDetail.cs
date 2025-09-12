using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("SHIPPINGBILLDETAIL")]
    public class ShippingBillDetail
    {
        [Key]

        public int SBDID { get; set; }
        public int SBMID { get; set; }
        public int GIDID { get; set; }
        public int SBDNO { get; set; }
        public string SBDDNO { get; set; }
        public string TRUCKNO { get; set; }
        public DateTime SBDDATE { get; set; }
        public int PRDTTID { get; set; }
        public int PRDTGID { get; set; }
        public string PRDTDESC { get; set; }
        public int GDWNID { get; set; }
        public int STAGID { get; set; }
        public short SBTYPE { get; set; }
        public decimal SBDNOP { get; set; }
        public decimal SBDQTY { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public DateTime PRCSDATE { get; set; }
        public short DISPSTATUS { get; set; }
        public int ESBMID { get; set; }

        public Nullable<decimal> UNLOADEDNOP { get; set; }
        public Nullable<decimal> UNLOADEDQTY { get; set; }
    }
}