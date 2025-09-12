using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace scfs_erp.Models
{
    [Table("STUFFINGDETAIL")]
    public class StuffingDetail
    {
        [Key]
        public int STFDID { get; set; }
        public int STFMID { get; set; }
        public int GIDID { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public Nullable<Int16> DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
        public Int16 STFTYPE { get; set; }
    }
}