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

    [Table("VW_EXPORT_GATEOUT_TRUCKNO_CBX_ASSGN")]
    public partial class VW_EXPORT_GATEOUT_TRUCKNO_CBX_ASSGN
    {
        [Key]
        public int GIDID { get; set; }
        public string AVHLNO { get; set; }
        public string DRVNAME { get; set; }
        public Nullable<int> CHAID { get; set; }
        public string CHANAME { get; set; }
        public System.DateTime GIDATE { get; set; }
    }
}
