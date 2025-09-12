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
    [Table("GPMODEMASTER")]
    public class GPModeMaster
    {
        [Key]
        public int GPMODEID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateGPMODEDESC", "Common", AdditionalFields = "i_GPMODEDESC", ErrorMessage = "This is already used.")]
        public string GPMODEDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateGPMODECODE", "Common", AdditionalFields = "i_GPMODECODE", ErrorMessage = "This is already used.")]
        public string GPMODECODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }

}