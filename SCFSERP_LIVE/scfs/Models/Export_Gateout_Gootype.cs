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
    [Table("EXPORT_GATEOUT_GOOTYPE")]
    public class Export_Gateout_Gootype
    {
        [Key]
        public int GOOTYPE { get; set; }
        public string GOOTYPEDESC { get; set; }
    }
}