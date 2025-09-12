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

    [Table("EXPORTTARIFFMASTER")]
    public class ExportTariffMaster
    {
        [Key]
        public Int32 TARIFFMID { get; set; }
        public Int32 TARIFFTMID { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateTARIFFMDESC1", "Common", AdditionalFields = "i_TARIFFMDESC", ErrorMessage = "This is already used.")]
        public string TARIFFMDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateTARIFFMCODE1", "Common", AdditionalFields = "i_TARIFFMCODE", ErrorMessage = "This is already used.")]
        public string TARIFFMCODE { get; set; }
        public Int32 TGID { get; set; }
        public Int32 SDPTID { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public Int16 DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
        
    }
}