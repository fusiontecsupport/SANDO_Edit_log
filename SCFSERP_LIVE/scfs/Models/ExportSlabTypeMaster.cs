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
    [Table("EXPORTSLABTYPEMASTER")]
    public class ExportSlabTypeMaster
    {
        [Key]
        public int SLABTID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateSLABTDESC", "common", AdditionalFields = "i_SLABTDESC", ErrorMessage = "This is already used.")]
        public string SLABTDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateSLABTCODE", "Common", AdditionalFields = "i_SLABTCODE", ErrorMessage = "This is already used.")]
        public string SLABTCODE { get; set; }

        public Nullable<Int16> SLABSTYPE { get; set; }
        public int DISPORDER { get; set; }
        public int HSNID { get; set; }
        //public decimal HSNID { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        [DisplayName("Status")]

        public Int16 DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }

    }
}