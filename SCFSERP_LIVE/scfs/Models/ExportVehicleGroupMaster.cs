using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("EXPORT_VEHICLE_GROUP_MASTER")]
    public class ExportVehicleGroupMaster
    {
        [Key]
        public int GPSTYPE { get; set; }
        public string GPSTYPEDESC { get; set; }
        public int GPWTYPE { get; set; }

    }
}