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
    [Table("EMPTY_DAMAGECODEMASTER")]
    public class EmptyDamageCodeMaster
    {
        [Key]
        public int DMGID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateDMGDESC", "Common", AdditionalFields = "i_DMGDESC", ErrorMessage = "This is already used.")]
        public string DMGDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateDMGCODE", "Common", AdditionalFields = "i_DMGCODE", ErrorMessage = "This is already used.")]
        public string DMGCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}