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
    [Table("TARIFFMASTER")]
    public class TariffMaster
    {
        [Key]
        public int TARIFFMID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateTARIFFMDESC", "Common", AdditionalFields = "i_TARIFFMDESC", ErrorMessage = "This is already used.")]
        public string TARIFFMDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateTARIFFMCODE", "Common", AdditionalFields = "i_TARIFFMCODE", ErrorMessage = "This is already used.")]
        public string TARIFFMCODE { get; set; }
        public int SDPTID { get; set; }

        public Nullable<int> TGID { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }

    }
}