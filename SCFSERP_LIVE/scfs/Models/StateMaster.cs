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
    [Table("STATEMASTER")]
    public class StateMaster
    {
        [Key]
        public int STATEID { get; set; }
        [DisplayName("Descrition")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateSTATEDESC", "Common", AdditionalFields = "i_STATEDESC", ErrorMessage = "This is already used.")]
        public string STATEDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateSTATECODE", "Common", AdditionalFields = "i_STATECODE", ErrorMessage = "This is already used.")]
        public string STATECODE { get; set; }
        public short STATETYPE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}