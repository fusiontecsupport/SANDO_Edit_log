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
    [Table("BONDSLABTYPEMASTER")]
    public class BondSlabTypeMaster
    {
        [Key]
        public int SLABTID { get; set; }
        
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateBNDSLABTDESC", "Common", AdditionalFields = "i_SLABTDESC", ErrorMessage = "This is already used.")]
        public string SLABTDESC { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateBNDSLABTCODE", "Common", AdditionalFields = "i_SLABTCODE", ErrorMessage = "This is already used.")] 
        public string SLABTCODE { get; set; }
        public Nullable<short> SLABPTYPE { get; set; }
        public Nullable<short> SLABCTYPE { get; set; }

        [DisplayName("Slab Classification")]
        public int SLABCID { get; set; }

        [DisplayName("HSNCode")]
        [Required(ErrorMessage = "Field is required")]
        public int HSNID { get; set; }
        public int DISPORDER { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public Nullable<short> DISPSTATUS { get; set; }
        public Nullable<System.DateTime> PRCSDATE { get; set; }

    }
}