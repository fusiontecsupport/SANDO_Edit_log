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

    [Table("DESIGNATIONTYPEMASTER")]
    public class DesignationTypeMaster
    {
        [Key]
        public short DSGNTID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Please Enter Description !!")]
        [Remote("ValidateDSGNTDESC", "Common", AdditionalFields = "i_DSGNTDESC", ErrorMessage = "This is already used.")]
        public string DSGNTDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Please Enter Code !!")]
        [Remote("ValidateDSGNTCODE", "Common", AdditionalFields = "i_DSGNTCODE", ErrorMessage = "This is already used.")]
        public string DSGNTCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Please Select Status !!")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
    
}