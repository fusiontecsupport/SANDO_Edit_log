using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace scfs_erp.Models
{
    [Table("BOOKEDMASTER")]
    public class BookedMaster
    {
        [Key]
        public int BOOKID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateBOOKDESC", "Common", AdditionalFields = "i_BOOKDESC", ErrorMessage = "This is already used.")]
        public string BOOKDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateBOOKCODE", "Common", AdditionalFields = "i_BOOKCODE", ErrorMessage = "This is already used.")]
        public string BOOKCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }

    }
}