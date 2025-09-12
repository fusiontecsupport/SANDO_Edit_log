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

    [Table("TARIFFGROUPMASTER")]
    public class TariffGroupMaster
    {
        [Key]
        public int TGID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateITGDESC", "Common", AdditionalFields = "i_TGDESC", ErrorMessage = "This is already used.")]
        public string TGDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateITGCODE", "Common", AdditionalFields = "i_TGCODE", ErrorMessage = "This is already used.")]
        public string TGCODE { get; set; }
        public int SDPTID { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}