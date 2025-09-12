using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs.Models
{
    [Table("VW_TRANSACTION_SLAB_RE_CALC_ASSGN")]
    public class VW_TRANSACTION_SLAB_RE_CALC_ASSGN
    {
        [Key]

        public int TRANDID { get; set; }
        public int TRANMID { get; set; }
        public int TARIFFMID { get; set; }
        public Int16 TRANOTYPE { get; set; }
        public decimal TRANDQTY { get; set; }
        public Int16 TRANBTYPE { get; set; }
        public int CONTNRSID { get; set; }
        public int STMRID { get; set; }

    }
}