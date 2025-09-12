using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("EXPORT_INVOICE_REGISTER")]
    public class Export_Invoice_Register
    {
        [Key]
        public int REGSTRID { get; set; }
        public string REGSTRDESC { get; set; }
    }
}