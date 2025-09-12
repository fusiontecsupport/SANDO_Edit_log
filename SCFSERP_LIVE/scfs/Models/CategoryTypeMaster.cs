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
    [Table("CATEGORYTYPEMASTER")]
    public class CategoryTypeMaster
    {
        [Key]
        public int CATETID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage="Field is required")]
        [Remote("ValidateCATETDESC", "Common", AdditionalFields = "i_CATETDESC", ErrorMessage = "This is already used.")]
        public string CATETDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateCATETCODE", "Common", AdditionalFields = "i_CATETCODE", ErrorMessage = "This is already used.")]
        public string CATETCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
       
        public DateTime PRCSDATE { get; set; }

    }
}