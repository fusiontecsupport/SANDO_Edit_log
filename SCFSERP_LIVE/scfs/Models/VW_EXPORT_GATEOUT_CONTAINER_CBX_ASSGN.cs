using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("VW_EXPORT_GATEOUT_CONTAINER_CBX_ASSGN")]
    public class VW_EXPORT_GATEOUT_CONTAINER_CBX_ASSGN
    {
        [Key]
        public int VTDID { get; set; }
        public string VTDNO { get; set; }
        public int GIDID { get; set; }
        public string CONTNRNO { get; set; }
    }
}