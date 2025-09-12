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
    [Table("BONDYARDMASTER")]
    public class BondYardMaster
    {
        [Key]
        public int YRDID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        
        public string YRDDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        
        public string YRDCODE { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}