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
    [Table("IMPORT_SLAB_WAGES_TYPE_MASTER")]
    public class Import_Slab_Wages_Type_Master
    {
        [Key]
        public int WTYPE { get; set; }

        public string WTYPEDESC { get; set; }
    }
}