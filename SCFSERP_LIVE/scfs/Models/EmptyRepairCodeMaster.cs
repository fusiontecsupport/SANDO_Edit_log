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
    [Table("EMPTY_REPAIRCODEMASTER")]
    public class EmptyRepairCodeMaster
    {
        [Key]
        public int RPRID { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateRPRCODE", "Common", AdditionalFields = "i_RPRCODE", ErrorMessage = "This is already used.")]
        public string RPRCODE { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateRPRDESC", "Common", AdditionalFields = "i_RPRDESC", ErrorMessage = "This is already used.")]
        public string RPRDESC { get; set; }
       
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}