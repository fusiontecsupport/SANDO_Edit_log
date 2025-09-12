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
    [Table("ROWMASTER")]
    public class RowMaster
    {
        [Key]
        public int ROWID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateROWDESC", "Common", AdditionalFields = "i_ROWDESC", ErrorMessage = "This is already used.")]
        public string ROWDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateROWCODE", "Common", AdditionalFields = "i_ROWCODE", ErrorMessage = "This is already used.")]
        public string ROWCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime PRCSDATE { get; set; }
    }
}