
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
    [Table("BONDGODOWNTYPEMASTER")]
    public class BondGodownTypeMaster
    {
        [Key]
        public int GWNTID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateGWNTDESC", "Common", AdditionalFields = "i_GWNTDESC", ErrorMessage = "This is already used.")]
        public string GWNTDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateGWNTCODE", "Common", AdditionalFields = "i_GWNTCODE", ErrorMessage = "This is already used.")]
        public string GWNTCODE { get; set; }

        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}