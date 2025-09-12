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
    [Table("GATEOUTDETAIL")]
    public class GateOutDetail
    {
        [Key]
        public int GODID { get; set; }
        public int COMPYID { get; set; }
        public int SDPTID { get; set; }
        public int REGSTRID { get; set; }
        public System.DateTime GODATE { get; set; }
        public System.DateTime GOTIME { get; set; }
        public int GONO { get; set; }
        public string GODNO { get; set; }
        public int GIDID { get; set; }
        public int TRANDID { get; set; }
        public string VHLNO { get; set; }
        public string GDRVNAME { get; set; }
        public string CHASNAME { get; set; }
        public short GOBTYPE { get; set; }
        public int CHAID { get; set; }
        public string LSEALNO { get; set; }
        public string SSEALNO { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public System.DateTime PRCSDATE { get; set; }
        public decimal GOQTY { get; set; }
        public int OTDID { get; set; }
        public string OTDNO { get; set; }
        public int CONTNRFID { get; set; }
        public int BOOKID { get; set; }
        public string GOPLCNAME { get; set; }
        public string GOBKGNO { get; set; }
        public string GOEGMNO { get; set; }
        public int OVSLID { get; set; }
        public string OVSLNAME { get; set; }
        public string OVOYNO { get; set; }
        public string GOREMRKS { get; set; }
        public System.DateTime EHIDATE { get; set; }
        public System.DateTime EHITIME { get; set; }
        public string GOTRNSPRTNAME { get; set; }
        public short GOTTYPE { get; set; }
        public short GOOTYPE { get; set; }
        public short GOCTYPE { get; set; }
        public string EBLNO { get; set; }

        public string ESSBLNO { get; set; }

        public Nullable<DateTime> ESSBLDT { get; set; }

        public Nullable<int> ESPORT { get; set; }

        public string ESTOPLACE { get; set; }

        public string ESINVNO { get; set; }

        public Nullable<DateTime> ESINVDT { get; set; }
    }
}