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
    [Table("BONDGATEINDETAIL")]
    public class BondGateInDetail
    {
        [Key]
        public int GIDID { get; set; }
        public int COMPYID { get; set; }
        public int SDPTID { get; set; }
        public System.DateTime GIDATE { get; set; }
        public System.DateTime GITIME { get; set; }
        public int GINO { get; set; }
        public string GIDNO { get; set; }
        public int TRNSPRTID { get; set; }
        public string TRNSPRTNAME { get; set; }
        public string VHLNO { get; set; }
        public string DRVNAME { get; set; }
        public int IMPRTID { get; set; }
        public string IMPRTNAME { get; set; }
        public int STMRID { get; set; }
        public string STMRNAME { get; set; }
        public int CHAID { get; set; }
        public string CHANAME { get; set; }
        public int CONTNRTID { get; set; }
        public int CONTNRID { get; set; }
        public int CONTNRSID { get; set; }
        public string CONTNRNO { get; set; }
        public int VSLID { get; set; }
        public string VSLNAME { get; set; }
        public string VOYNO { get; set; }
        public string IGMNO { get; set; }
        public string GPLNO { get; set; }
        public int PRDTGID { get; set; }
        public string PRDTDESC { get; set; }
        public int UNITID { get; set; }
        public Nullable<decimal> GPWGHT { get; set; }
        public Nullable<decimal> GPNOP { get; set; }
        public Nullable<decimal> GPCBM { get; set; }
        public int BNDID { get; set; }
        public int CONTNRRCVFRM { get; set; }
        public string CONTNRRCVFRMOTH { get; set; }
        public string GIREMKRS { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public Nullable<short> DISPSTATUS { get; set; }
        public Nullable<System.DateTime> PRCSDATE { get; set; }
    }
}