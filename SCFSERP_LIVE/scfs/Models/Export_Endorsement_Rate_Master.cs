using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("EXPORT_ENDORSEMENT_RATE_MASTER")]
    public class Export_Endorsement_Rate_Master
    {
        [Key]
        public int ECRID { get; set; }

        [DisplayName("Tariff")]
        public int COMPYID { get; set; }

        [DisplayName("W.E From")]

        public DateTime ECRDATE { get; set; }
        [DisplayName("Slab Type")]

        public int ECMID { get; set; }

        public short CHRGETYPE { get; set; }


        [DisplayName("Size")]

        public int CONTNRSID { get; set; }


        public short CHRGDON { get; set; }

        [DisplayName("MIN")]

        public decimal ECRMIN { get; set; }
        [DisplayName("MAX")]

        public decimal ECRMAX { get; set; }
        [DisplayName("Value")]

        public decimal ECRAMT { get; set; }

        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]

        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}