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
    [Table("EXPORTSLABMASTER")]
    public class ExportSlabMaster
    {
        [Key]
        public int SLABMID { get; set; }

        [DisplayName("Tariff")]
        public int COMPYID { get; set; }

        [DisplayName("W.E From")]

        public DateTime SLABMDATE { get; set; }
        [DisplayName("Slab Type")]

        public int SLABTID { get; set; }

        public int TARIFFMID { get; set; }


        [DisplayName("CHA")]
        public int CHAID { get; set; }


        [DisplayName("Charge Type")]

        public Int16 CHRGETYPE { get; set; }

        [DisplayName("Size")]

        public int CONTNRSID { get; set; }
        [DisplayName("Slab Days")]

        public Int16 SDTYPE { get; set; }

        [DisplayName("Yard")]
        public Int16 YRDTYPE { get; set; }

        [DisplayName("Handling Type")]
        public Int16 HTYPE { get; set; }

        [DisplayName("Wages")]
        public Int16 WTYPE { get; set; }

        [DisplayName("MIN")]

        public decimal SLABMIN { get; set; }
        [DisplayName("MAX")]

        public decimal SLABMAX { get; set; }
        [DisplayName("Value")]

        public decimal SLABAMT { get; set; }

        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        [DisplayName("Status")]

        public Int16 DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
        public decimal SLABUAMT { get; set; }

        public Int32 EOPTID { get; set; }
    }
}