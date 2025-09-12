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
    [Table("CONTAINERTYPEMASTER")]
    public class ContainerTypeMaster
    {
        [Key]
        public int CONTNRTID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateCONTNRTDESC", "Common", AdditionalFields = "i_CONTNRTDESC", ErrorMessage = "This is already used.")]
        public string CONTNRTDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateCONTNRTCODE", "Common", AdditionalFields = "i_CONTNRTCODE", ErrorMessage = "This is already used.")]
        public string CONTNRTCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}