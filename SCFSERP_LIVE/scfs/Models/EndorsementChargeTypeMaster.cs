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
    [Table("ENDORSEMENT_CHARGE_TYPE_MASTER")]
    public class EndorsementChargeTypeMaster
    {
        [Key]
        public int ECTID { get; set; }
        [Remote("ValidateECTDESC", "Common", AdditionalFields = "i_ECTDESC", ErrorMessage = "This is already used.")]
        [Required(ErrorMessage = "Field is required")]
        public string ECTDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateECTCODE", "Common", AdditionalFields = "i_ECTCODE", ErrorMessage = "This is already used.")]
        public string ECTCODE { get; set; }
        public int DISPORDER { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}