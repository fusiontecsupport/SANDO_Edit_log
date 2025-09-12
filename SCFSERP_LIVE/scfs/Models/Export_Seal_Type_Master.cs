using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("EXPORT_SEAL_TYPE_MASTER")]
    public class Export_Seal_Type_Master
    {
        [Key]
        public int ESLID { get; set; }

        public int ESLTID { get; set; }

        public string ESLTDESC { get; set; }

    }
}