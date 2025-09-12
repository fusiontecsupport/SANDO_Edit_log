
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
    [Table("VW_OPENSHEET_BILL_ENTRY_MID_ASSGN")]
    public class VW_OPENSHEET_BILL_ENTRY_MID_ASSGN
    {
        [Key]
        public int OSMID { get; set; }
        public int BILLEMID { get; set; }       
    }
}