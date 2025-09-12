using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("EXPORT_SEALTYPEMASTER")]
    public class Export_SealTypeMaster
    {
        [Key]
        public int SLTID { get; set; }
        public String SLTDESC { get; set; }
    }
}