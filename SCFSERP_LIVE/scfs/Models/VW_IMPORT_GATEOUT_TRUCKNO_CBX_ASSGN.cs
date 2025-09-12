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
    [Table("VW_IMPORT_GATEOUT_TRUCKNO_CBX_ASSGN")]
    
    public class VW_IMPORT_GATEOUT_TRUCKNO_CBX_ASSGN
    {
        [Key]
        public int GIDID { get; set; }

        public string AVHLNO { get; set; }
        

    }
}