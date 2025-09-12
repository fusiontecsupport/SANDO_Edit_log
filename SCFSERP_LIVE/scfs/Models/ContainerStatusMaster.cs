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
    [Table("CONTAINERSTATUSMASTER")]
    public class ContainerStatusMaster
    {
        [Key]
        public int CNTNRSID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateCNTNRSDESC", "Common", AdditionalFields = "i_CNTNRSDESC", ErrorMessage = "This is already used.")]
        public string CNTNRSDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateCNTNRSCODE", "Common", AdditionalFields = "i_CNTNRSCODE", ErrorMessage = "This is already used.")]
        public string CNTNRSCODE { get; set; }

        [DisplayName("Order")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPORDER { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
        
    }
}