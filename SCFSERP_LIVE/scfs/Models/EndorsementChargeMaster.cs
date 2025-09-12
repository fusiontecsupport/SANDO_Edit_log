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

    [Table("ENDORSEMENT_CHARGE_MASTER")]
    public class EndorsementChargeMaster
    {
        [Key]
        public int ECMID { get; set; }
        public int ECTID { get; set; }
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateECMDESC", "Common", AdditionalFields = "i_ECMDESC", ErrorMessage = "This is already used.")]
        public string ECMDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateECMCODE", "Common", AdditionalFields = "i_ECMCODE", ErrorMessage = "This is already used.")]
        public string ECMCODE { get; set; }
        public short CHRGDON { get; set; }
        public int CFID { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}