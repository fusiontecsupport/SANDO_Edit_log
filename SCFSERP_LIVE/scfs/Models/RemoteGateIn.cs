using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("REMOTEGATEINDETAIL")]
    public class RemoteGateIn
    {
        [Key]
        public int GIDID { get; set; }
        public string AGIDNO { get; set; }

        public int COMPYID { get; set; }

        public int SDPTID { get; set; }

        public DateTime GIDATE { get; set; }

        public DateTime GITIME { get; set; }

        public DateTime GICCTLDATE { get; set; }

        public DateTime GICCTLTIME { get; set; }

        public int GINO { get; set; }

        public string GIDNO { get; set; }

        public string GPREFNO { get; set; }

        public int IMPRTID { get; set; }

        public string IMPRTNAME { get; set; }

        public int STMRID { get; set; }

        public string STMRNAME { get; set; }

        public int CONTNRTID { get; set; }

        public int CONTNRID { get; set; }

        public int CONTNRSID { get; set; }

        public string CONTNRNO { get; set; }

        public int VSLID { get; set; }

        public string VSLNAME { get; set; }

        public string VOYNO { get; set; }

        public int PRDTGID { get; set; }

        public string PRDTDESC { get; set; }

        public int UNITID { get; set; }

        public decimal GPWGHT { get; set; }

        public string IGMNO { get; set; }

        public string GPLNO { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public short DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }

        public string GIISOCODE { get; set; }

        public decimal GPNWGHT { get; set; }

        public decimal GPGWGHT { get; set; }

        public decimal GPCBM { get; set; }

        public decimal GPLENGTH { get; set; }

        public decimal GPWIDTH { get; set; }

        public decimal GPHEIGHT { get; set; }

        public string GPBLNO { get; set; }

        public decimal GPEAMT { get; set; }

        public decimal GPAAMT { get; set; }

        public int GPPTYPE { get; set; }

        public int WMDID { get; set; }

        public DateTime IGMDATE { get; set; }

        public string BLNO { get; set; }

        public int GPMODEID { get; set; }

    }
}