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

    [Table("STAGMASTER")]
    public class StagMaster
    {
        [Key]
        public int STAGID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateSTAGDESC", "common", AdditionalFields = "i_STAGDESC", ErrorMessage = "This is already used.")]
        public string STAGDESC { get; set; }
        public int GDWNID { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateSTAGCODE", "common", AdditionalFields = "i_STAGCODE", ErrorMessage = "This is already used.")]
        public string STAGCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}