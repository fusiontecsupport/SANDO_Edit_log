using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("emailinvoicetemplate")]
    public class emailinvoicetemplate
    {
        [Key]
        public int billid { get; set; }
        public int sdptid { get; set; }

        public string mailsubject { get; set; }
        public string mailbody { get; set; }
        public DateTime maildttm { get; set; }
        public string remarks { get; set; }
    }
}