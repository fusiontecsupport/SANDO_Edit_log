using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("EXPORT_VEHICLE_TYPE_MASTER")]
    public class ExportVehicleTypeMaster
    {
        [Key]
        public int GPWTYPE { get; set; }
        public string GPWTYPEDESC { get; set; }
    }
}