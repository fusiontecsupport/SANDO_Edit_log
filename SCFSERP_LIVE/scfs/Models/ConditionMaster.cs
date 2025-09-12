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

    [Table("CONDITIONMASTER")]
    public class ConditionMaster
    {
        [Key]
        public int CONDTNID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Please Enter Description !!")]
        [Remote("ValidateCONDTNDESC", "Common", AdditionalFields = "i_CONDTNDESC", ErrorMessage = "This is already used.")]
        public string CONDTNDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Please Enter Code !!")]
        [Remote("ValidateCONDTNCODE", "Common", AdditionalFields = "i_CONDTNCODE", ErrorMessage = "This is already used.")]
        public string CONDTNCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}