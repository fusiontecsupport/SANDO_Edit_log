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
    [Table("PRODUCTGROUPMASTER")]
    public class ProductGroupMaster
    {
        [Key]
        public int PRDTGID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidatePRDTGDESC", "Common", AdditionalFields = "i_PRDTGDESC", ErrorMessage = "This is already used.")]
        public string PRDTGDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidatePRDTGCODE", "Common", AdditionalFields = "i_PRDTGCODE", ErrorMessage = "This is already used.")]
        public string PRDTGCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}

