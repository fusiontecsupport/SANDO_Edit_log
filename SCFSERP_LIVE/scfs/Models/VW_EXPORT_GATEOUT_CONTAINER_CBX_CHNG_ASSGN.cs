
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

    [Table("VW_EXPORT_GATEOUT_CONTAINER_CBX_CHNG_ASSGN")]
    public partial class VW_EXPORT_GATEOUT_CONTAINER_CBX_CHNG_ASSGN
    {
        [Key]
        public int GIDID { get; set; }
        public string CONTNRNO { get; set; }
        public string CONTNRSDESC { get; set; }
        public string CONTNRTCODE { get; set; }
        public int VTNO { get; set; }
        public string VTDNO { get; set; }
        public int STFMNO { get; set; }
        public string CHANAME { get; set; }
        public int CHAID { get; set; }
        public string DRVNAME { get; set; }
        public int ASLDID { get; set; }
        public string CSEALNO { get; set; }
        public string ASEALNO { get; set; }
        public string GIDATE { get; set; }
        public short ASLMTYPE { get; set; }
        public string GOTRNSPRTNAME { get; set; }
        public string VHLNO { get; set; }
    }
}

