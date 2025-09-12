using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("EXPORT_CARGO_BLOCK_DETAILS")]
    public class ExportCargoBlockDetails
    {
        [Key]
        public int GBDID { get; set; }
        public int COMPYID { get; set; }
        public int SDPTID { get; set; }
        public DateTime GBDATE { get; set; }
        public DateTime GBTIME { get; set; }
        public int GBNO { get; set; }
        public string GBDNO { get; set; }
        public int SBMID { get; set; }

        public Nullable<Int16> GBTYPE { get; set; }

        public string NARTN { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public Nullable<Int16> DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }

        public Nullable<DateTime> GBUDATE { get; set; }

        public Nullable<DateTime> GBUTIME { get; set; }

        public string UNARTN { get; set; }

        public Nullable<Int16> GBSTYPE { get; set; }
    }
}