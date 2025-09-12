using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    
    [Table("BONDGATEOUTDETAIL")]
    public class BondContainerOut
    {
        [Key]
        public int GODID { get; set; }
        public int COMPYID { get; set; }
        public int SDPTID { get; set; }
        public System.DateTime GODATE { get; set; }
        public System.DateTime GOTIME { get; set; }
        public int GONO { get; set; }
        public string GODNO { get; set; }
        public Nullable<int> GIDID { get; set; }
        public string VHLNO { get; set; }
        public string GDRVNAME { get; set; }
        public string CHASNAME { get; set; }
        public Nullable<short> GOBTYPE { get; set; }
        public int CHAID { get; set; }
        public string GOREMRKS { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public Nullable<short> DISPSTATUS { get; set; }
        public Nullable<System.DateTime> PRCSDATE { get; set; }
    }
}