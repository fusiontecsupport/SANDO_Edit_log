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
    [Table("EMPTY_LOCATIONCODEMASTER")]
    public class EmptyLocationCodeMaster
    {
        [Key]
        public int LOCTID { get; set; }
               
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateLOCTCODE1", "Common", AdditionalFields = "i_LOCTCODE", ErrorMessage = "This is already used.")]
        public string LOCTCODE { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateLOCTDESC1", "Common", AdditionalFields = "i_LOCTDESC", ErrorMessage = "This is already used.")]
        public string LOCTDESC { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}