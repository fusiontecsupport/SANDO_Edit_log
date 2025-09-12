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
    [Table("PORTTYPEMASTER")]
    public class PortTypeMaster
    {
        [Key]
        public int GPPTYPE { get; set; }
        public string GPPTYPEDESC { get; set; }
    }
}