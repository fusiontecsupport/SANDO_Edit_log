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
    [Table("CONTAINERTHRUMASTER")]
    public class ContainerThruMaster
    {
        [Key]
        public int CONTNRFID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateCONTNRFDESC", "Common", AdditionalFields = "i_CONTNRFDESC", ErrorMessage = "This is already used.")]
        public string CONTNRFDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateCONTNRFCODE", "Common", AdditionalFields = "i_CONTNRFCODE", ErrorMessage = "This is already used.")]
        public string CONTNRFCODE { get; set; }
        [DisplayName("Type")]
        [Required(ErrorMessage = "Field is required")]
        public short CONTNRFTYPE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}