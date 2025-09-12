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
    [Table("EXPORT_SLAB_WAGES_TYPE_MASTER")]
    public class Export_Slab_wages_type
    {
        [Key]
        public int WTID { get; set; }
        public int WTYPE { get; set; }
        public string WTYPEDESC { get; set; }
    }
}