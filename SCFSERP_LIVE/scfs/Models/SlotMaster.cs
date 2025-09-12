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
    [Table("SLOTMASTER")]
    public class SlotMaster
    {
        [Key]
        public int SLOTID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateSLOTDESC", "Common", AdditionalFields = "i_SLOTDESC", ErrorMessage = "This is already used.")]
        public string SLOTDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateSLOTCODE", "Common", AdditionalFields = "i_SLOTCODE", ErrorMessage = "This is already used.")]
        public string SLOTCODE { get; set; }
        [DisplayName("Row")]
        [Required(ErrorMessage = "Field is required")]
        public int ROWID { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}