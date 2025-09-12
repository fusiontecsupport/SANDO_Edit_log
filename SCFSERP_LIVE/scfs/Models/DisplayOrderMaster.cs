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

    [Table("DISPLAYORDERMASTER")]
    public class DisplayOrderMaster
    {
        [Key]
        public int DORDRID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateDORDRDESC", "Common", AdditionalFields = "i_DORDRDESC", ErrorMessage = "This is already used.")]
        public string DORDRDESC { get; set; }
        [DisplayName("Order")]
        [Required(ErrorMessage = "Field is required")]
        public Int16 DORDORDR { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public Byte DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}