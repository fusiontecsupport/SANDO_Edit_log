using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("TAXTYPEMASTER")]
    public class TaxTypeMaster
    {
        [Key]
        public int CFOPTN { get; set; }
        public string CFOPTNDESC { get; set; }
    }
}