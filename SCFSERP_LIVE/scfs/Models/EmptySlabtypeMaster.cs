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
    [Table("EMPTY_SLABTYPEMASTER")]
    public class EmptySlabtypeMaster
    {

        [Key]
        public int SLABTID { get; set; }
        
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateSLABTCODE1", "Common", AdditionalFields = "i_SLABTCODE", ErrorMessage = "This is already used.")]
        public string SLABTCODE { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateSLABTDESC1", "Common", AdditionalFields = "i_SLABTDESC", ErrorMessage = "This is already used.")]
        public string SLABTDESC { get; set; }

        public int DISPORDER { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}