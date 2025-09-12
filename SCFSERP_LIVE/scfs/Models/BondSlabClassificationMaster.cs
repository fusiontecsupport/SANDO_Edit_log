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
    [Table("BONDSLABCLASSIFICATIONMASTER")]
    public class BondSlabClassificationMaster
    {
        [Key]
        public int SLABCID { get; set; }
        public string SLABCDESC { get; set; }
        public string SLABCCODE { get; set; }
        public Nullable<short> DISPORDR { get; set; }
        public Nullable<short> DISPSTATUS { get; set; }
        public Nullable<System.DateTime> PRCSDATE { get; set; }
    }
}