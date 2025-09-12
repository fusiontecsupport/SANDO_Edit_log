
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
    [Table("BONDGODOWNMASTER")]
    public class BondGodownMaster
    {
        [Key]
        public int GWNID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateGWNDESC", "Common", AdditionalFields = "i_GWNDESC", ErrorMessage = "This is already used.")]
        public string GWNDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateGWNCODE", "Common", AdditionalFields = "i_GWNCODE", ErrorMessage = "This is already used.")]
        public string GWNCODE { get; set; }

        [DisplayName("Godown Type")]
        [Required(ErrorMessage = "Field is required")]
        public int GWNTID { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}