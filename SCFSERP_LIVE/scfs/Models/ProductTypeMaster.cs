using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("PRODUCTTYPEMASTER")]
    public class ProductTypeMaster
    {
        [Key]
        public int PRDTTID { get; set; }
        public string PRDTTDESC { get; set; }
    }
}