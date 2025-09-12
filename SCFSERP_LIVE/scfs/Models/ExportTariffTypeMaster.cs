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

    [Table("EXPORTTARIFFTYPEMASTER")]
    public class ExportTariffTypeMaster
    {
        [Key]
        public int TARIFFTMID { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateTARIFFTMDESC", "Common", AdditionalFields = "i_TARIFFTMDESC", ErrorMessage = "This is already used.")]
        public string TARIFFTMDESC { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateTARIFFTMCODE", "Common", AdditionalFields = "i_TARIFFTMCODE", ErrorMessage = "This is already used.")]
        public string TARIFFTMCODE { get; set; }

        public int SDPTID { get; set; }
        
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }

        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }

        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}