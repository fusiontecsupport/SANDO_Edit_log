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
    [Table("UNITMASTER")]
    public class UnitMaster
    {
        [Key]
        public int UNITID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Fiels is required")]
        [Remote("ValidateUNITDESC", "Common", AdditionalFields = "i_UNITDESC", ErrorMessage = "This is already used.")]
        public string UNITDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Fiels is required")]
        [Remote("ValidateUNITCODE", "Common", AdditionalFields = "i_UNITCODE", ErrorMessage = "This is already used.")]
        public string UNITCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Fiels is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}