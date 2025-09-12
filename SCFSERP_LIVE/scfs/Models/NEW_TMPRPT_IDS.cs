using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("NEW_TMPRPT_IDS")]
    public class NEW_TMPRPT_IDS
    {
        [Key]
        public string KUSRID { get; set; }
        public string OPTNSTR { get; set; }
        public int RPTID { get; set; }
    }
}