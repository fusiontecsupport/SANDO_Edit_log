
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
    [Table("VW_EXPORT_GATEOUT_MOD_ASSGN")]
    public class VW_EXPORT_GATEOUT_MOD_ASSGN
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
        public Nullable<int> GIDID { get; set; }
        public Nullable<int> TRANDID { get; set; }
        public string VHLNO { get; set; }
        public string DRVNAME { get; set; }
        public string CHASNAME { get; set; }
        public Nullable<short> GOBTYPE { get; set; }
        public int CHAID { get; set; }
        public string LSEALNO { get; set; }
        public string SSEALNO { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public Nullable<short> DISPSTATUS { get; set; }
        public Nullable<System.DateTime> PRCSDATE { get; set; }
        public Nullable<decimal> GOQTY { get; set; }
        public Nullable<int> OTDID { get; set; }
        public string OTDNO { get; set; }
        public Nullable<int> CONTNRFID { get; set; }
        public Nullable<int> BOOKID { get; set; }
        public string GOPLCNAME { get; set; }
        public string GOBKGNO { get; set; }
        public string GOEGMNO { get; set; }
        public Nullable<int> OVSLID { get; set; }
        public string OVSLNAME { get; set; }
        public string OVOYNO { get; set; }
        public string GOREMRKS { get; set; }
        public Nullable<System.DateTime> EHIDATE { get; set; }
        public Nullable<System.DateTime> EHITIME { get; set; }
        public string GOTRNSPRTNAME { get; set; }
        public Nullable<short> GOTTYPE { get; set; }
        public Nullable<short> GOOTYPE { get; set; }
        public Nullable<short> GOCTYPE { get; set; }
        public string CONTNRNO { get; set; }
        public string CONTNRSDESC { get; set; }
        public string CONTNRTCODE { get; set; }
        //public DateTime GIDATE { get; set; }

        public string GIDATE { get; set; }
        public string CHANAME { get; set; }
        public int ASLDID { get; set; }
        public string CSEALNO { get; set; }
        public string ASEALNO { get; set; }
        public Nullable<int> GPSTYPE { get; set; }
        public int VTNO { get; set; }
       
        
        
    }
}