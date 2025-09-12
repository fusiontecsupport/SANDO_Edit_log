using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;

namespace scfs_erp.Models
{

    [Table("LOCATIONMASTER")]
    public class LocationMaster
    {
        [Key]
        public int LOCTID { get; set; }
        [DisplayName("Descrition")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateLOCTDESC", "Common", AdditionalFields = "i_LOCTDESC", ErrorMessage = "This is already used.")]
        public string LOCTDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateLOCTCODE", "Common", AdditionalFields = "i_LOCTCODE", ErrorMessage = "This is already used.")]
        public string LOCTCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}