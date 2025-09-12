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
    [Table("EXPORT_SLAB_CHARGE_TYPE_MASTER")]
    public class Export_Slab_Charge_Type
    {
        [Key]
        public int CTID { get; set; }
        public int CHRGETYPE { get; set; }
        public string CHRGETYPEDESC { get; set; }
    }
}