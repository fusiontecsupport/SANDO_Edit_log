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
    [Table("EMPTY_COMPONENTCODEMASTER")]
    public class EmptyComponentCodeMaster
    {
        [Key]
        public int COMPTID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateCOMPTDESC", "Common", AdditionalFields = "i_COMPTDESC", ErrorMessage = "This is already used.")]
        public string COMPTDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateCOMPTCODE", "Common", AdditionalFields = "i_COMPTCODE", ErrorMessage = "This is already used.")]
        public string COMPTCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}