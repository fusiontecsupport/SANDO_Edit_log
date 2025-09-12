using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scfs_erp.Models
{
    [Table("GATEINLORRYHIREDETAIL")]
    public class GateInLorryHireDetail
    {
        [Key]
        public int GILDID { get; set; }
        public int COMPYID { get; set; }
        public int SDPTID { get; set; }
        public DateTime GILDATE { get; set; }
        public DateTime GILTIME { get; set; }
        public int GILNO { get; set; }
        public string GILDNO { get; set; }
        public int GIDID { get; set; }
        public string GILBILLNO { get; set; }
        public Nullable<DateTime> GILBILLDATE { get; set; }
        public string GILNARTN { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}