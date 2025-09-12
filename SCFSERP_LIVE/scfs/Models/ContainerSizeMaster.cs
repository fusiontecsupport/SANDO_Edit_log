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
    [Table("CONTAINERSIZEMASTER")]
    public class ContainerSizeMaster
    {
        [Key]
        public int CONTNRSID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateCONTNRSDESC", "Common", AdditionalFields = "i_CONTNRSDESC", ErrorMessage = "This is already used.")]
        public string CONTNRSDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateCONTNRSCODE", "Common", AdditionalFields = "i_CONTNRSCODE", ErrorMessage = "This is already used.")]
        public string CONTNRSCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}