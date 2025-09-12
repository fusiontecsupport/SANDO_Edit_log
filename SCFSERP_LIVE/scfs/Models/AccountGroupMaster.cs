using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//namespace scfs_erp.Models
//{
namespace scfs_erp.Models
{
    [Table("ACCOUNTGROUPMASTER")]
    public class AccountGroupMaster
    {
        [Key]
        public int ACHEADGID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateACHEADGDESC", "Common", AdditionalFields ="i_ACHEADGDESC", ErrorMessage = "This is already used.")]
        
        public string ACHEADGDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateACHEADGCODE", "Common", AdditionalFields = "i_ACHEADGCODE", ErrorMessage = "This is already used.")]
        public string ACHEADGCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get;set;}
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}