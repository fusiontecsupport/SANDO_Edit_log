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
    [Table("VW_ACCOUNTING_YEAR_DETAIL_ASSGN")]
    public class VW_ACCOUNTING_YEAR_DETAIL_ASSGN
    {
        [Key]
        public int COMPYID { get; set; }
        public DateTime FDATE { get; set; }
        public DateTime TDATE { get; set; }
        public string YRDESC { get; set; }

    }
}