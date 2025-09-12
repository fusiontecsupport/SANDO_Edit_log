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
    [Table("DESIGNATIONMASTER")]
    public class DesignationMaster
    {
        [Key]
        public int DSGNID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Please Enter Designation !!")]
        [Remote("ValidateDSGNDESC", "Common", AdditionalFields = "i_DSGNDESC", ErrorMessage = "This is already used.")]
        public string DSGNDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Please Enter Code !!")]
        [Remote("ValidateDSGNCODE", "Common", AdditionalFields = "i_DSGNCODE", ErrorMessage = "This is already used.")]
        public string DSGNCODE { get; set; }

        [Required(ErrorMessage = "Please Select Type !!")]
        public short DSGNTID { get; set; }
        [DisplayName("Days")]
        [Required(ErrorMessage = "Field is required")]
        public short DSGNEDAYS { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Please Select Status !!")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}