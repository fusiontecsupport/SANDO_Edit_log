using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("VW_ESEAL_GATEINDETAIL_CONTAINERNO")]
    public class VW_ESEAL_GATEINDETAIL_CONTAINERNO
    {
        [Key]
        public int GIDID { get; set; }
        public string CONTNRNO { get; set; }
        
    }
}