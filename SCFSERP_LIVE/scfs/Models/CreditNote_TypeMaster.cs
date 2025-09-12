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
    [Table("CREDITNOTE_TYPEMASTER")]
    public class CreditNote_TypeMaster
    {

        [Key]
        public int CNTID { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateCNTDESC", "Common", AdditionalFields = "i_CNTDESC", ErrorMessage = "This is already used.")]
        public string CNTDESC { get; set; }
        public string CNT_LDGR_DESC { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }

    }
}