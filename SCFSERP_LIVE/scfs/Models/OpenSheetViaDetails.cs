using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("OPENSHEET_VIA_DETAILS")]
    public class OpenSheetViaDetails
    {
        [Key]
        public int OSMTYPE { get; set; }
        public string OSMTYPEDESC { get; set; }
    }
}