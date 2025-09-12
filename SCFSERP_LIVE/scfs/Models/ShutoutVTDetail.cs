using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("SHUTOUTVTDETAIL")]
    public class ShutoutVTDetail
    {
        [Key]
        public int VTDID { get; set; }
        public int COMPYID { get; set; }
        public int SDPTID { get; set; }
        public DateTime VTDATE { get; set; }
        public DateTime VTTIME { get; set; }
        public int VTNO { get; set; }
        public string VTDNO { get; set; }
        public int ASLDID { get; set; }
        public string VHLNO { get; set; }
        public string DRVNAME { get; set; }
        public string VTDESC { get; set; }
        public Nullable<decimal> VTQTY { get; set; }
        public Nullable<int> VTTYPE { get; set; }
        public Nullable<int> VTSTYPE { get; set; }
        public string VTREMRKS { get; set; }
        public Nullable<int> GIDID { get; set; }
        public Nullable<int> EGIDID { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public Nullable<Int16> DISPSTATUS { get; set; }
        public Nullable<System.DateTime> PRCSDATE { get; set; }
        public string VTSSEALNO { get; set; }
        public Nullable<int> VTCTYPE { get; set; }
        public Nullable<int> CGIDID { get; set; }
        public Nullable<int> STFDID { get; set; }
        public Nullable<DateTime> EVLDATE { get; set; }
        public Nullable<DateTime> EVSDATE { get; set; }
        public Nullable<DateTime> ELRDATE { get; set; }

        public int TRNSPRTID { get; set; }
        public string TRNSPRTNAME { get; set; }
        public string GTRNSPRTNAME { get; set; }
        public int VHLMID { get; set; }
        public string QRCDIMGPATH { get; set; }
        public Nullable<int> STFMID { get; set; }
        //public string VTDLNO { get; set; }
        //public Nullable<DateTime> VTDDATE { get; set; }

    }

}