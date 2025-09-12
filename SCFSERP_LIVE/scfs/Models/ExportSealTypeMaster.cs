using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("EXPORT_SEALTYPE_MASTER")]
    public class ExportSealTypeMaster
    {
        [Key]
        public int GPETYPE { get; set; }
        public string GPETYPEDESC { get; set; }
    }
}