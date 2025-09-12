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
    [Table("BONDSLABMASTER")]
    public class BondSlabMaster
    {
        [Key]
        public int SLABMID { get; set; }
        public int COMPYID { get; set; }
        public System.DateTime SLABMDATE { get; set; }
        public int SLABTID { get; set; }
        public int TARIFFMID { get; set; }
        public int CHAID { get; set; }
        public short CHRGETYPE { get; set; }
        public int CONTNRSID { get; set; }
        public int PRDTGID { get; set; }
        public short SDTYPE { get; set; }
        public short YRDTYPE { get; set; }
        public short HTYPE { get; set; }
        public short WTYPE { get; set; }
        public decimal SLABMIN { get; set; }
        public decimal SLABMAX { get; set; }
        public decimal SLABAMT { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public System.DateTime PRCSDATE { get; set; }
        public Nullable<int> PERIODTID { get; set; }
        public short HANDTYPE { get; set; }
    }
}