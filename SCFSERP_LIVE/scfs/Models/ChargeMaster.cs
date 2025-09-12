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

    [Table("CHARGEMASTER")]
    public class ChargeMaster
    {
        [Key]
        public int CHRGID { get; set; }
        public int SDPTID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateCHRGDESC", "Common", AdditionalFields = "i_CHRGDESC", ErrorMessage = "This is already used.")]
        public string CHRGDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateCHRGCODE", "Common", AdditionalFields = "i_CHRGCODE", ErrorMessage = "This is already used.")]
        public string CHRGCODE { get; set; }
        public int CONTNRSID { get; set; }
        [DisplayName("Amount")]
        [Required(ErrorMessage = "Field is required")]
        public decimal CHRGAMT { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}