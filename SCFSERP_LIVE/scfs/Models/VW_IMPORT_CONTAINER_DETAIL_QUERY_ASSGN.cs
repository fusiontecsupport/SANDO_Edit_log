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

    [Table("VW_IMPORT_CONTAINER_DETAIL_QUERY_ASSGN")]
    public class VW_IMPORT_CONTAINER_DETAIL_QUERY_ASSGN
    {

        [Key]
        public int GIDID { get; set; }
        public Nullable<System.DateTime> GITIME { get; set; }
        public Nullable<System.DateTime> GIDATE { get; set; }
        public string GIDNO { get; set; }
        public string VHLNO { get; set; }
        public string DRVNAME { get; set; }
        public string IMPRTNAME { get; set; }
        public string STMRNAME { get; set; }
        public string CONTNRNO { get; set; }
        public string CONTNRSDESC { get; set; }
        public string CONTNRTDESC { get; set; }
        public string VSLNAME { get; set; }
        public string VOYNO { get; set; }
        public string PRDTDESC { get; set; }
        public string IGMNO { get; set; }
        public string GPLNO { get; set; }
        public int GPETYPE { get; set; }
        public int GPWTYPE { get; set; }
        public int GPSTYPE { get; set; }
        public string GIREMKRS { get; set; }
        public decimal GPWGHT { get; set; }
        public string GIDMGDESC { get; set; }
        public int GBDID { get; set; }
        public string BILLEMDNO { get; set; }
        public Nullable<System.DateTime> BILLEMDATE { get; set; }
        public string BLNO { get; set; }
        public Nullable<System.DateTime> GODATE { get; set; }
        public Nullable<System.DateTime> GOTIME { get; set; }
        public int BILLEDID { get; set; }
        public string ROWDESC { get; set; }
        public string SLOTDESC { get; set; }

        public string GPMODEDESC { get; set; }

        public string BLOCKEDDATE { get; set; }
        public string UNBLOCKEDDATE { get; set; }
        public Nullable<System.DateTime> IGMDATE { get; set; }

    }
}