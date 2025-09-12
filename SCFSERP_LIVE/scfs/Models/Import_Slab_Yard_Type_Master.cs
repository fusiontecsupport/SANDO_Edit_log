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
    [Table("IMPORT_SLAB_YARD_TYPE_MASTER")]
    public class Import_Slab_Yard_Type_Master
    {
        [Key]
        public int YRDTYPE { get; set; }

        public string YRDDESC { get; set; }
    }
}