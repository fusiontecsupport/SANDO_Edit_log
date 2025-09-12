using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("BILLENTRYMASTER")]
    public class BillEntryMaster
    {
        [Key]
        public int BILLEMID { get; set; }
        public int SDPTID { get; set; }
        public int COMPYID { get; set; }
        public DateTime BILLEMDATE { get; set; }
        public DateTime BILLEMTIME { get; set; }
        public int BILLEMNO { get; set; }
        public string BILLEMDNO { get; set; }
        public int CHAID { get; set; }
        public string BILLEMNAME { get; set; }
        public decimal BILLEMAAMT { get; set; }
        public string BLNO { get; set; }
        public decimal NOP { get; set; }
        public decimal WGHT { get; set; }
        public int UNITID { get; set; }
        public short BILLEDMTYPE { get; set; }
        public string DPAIDNO { get; set; }
        public decimal DPAIDAMT { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}