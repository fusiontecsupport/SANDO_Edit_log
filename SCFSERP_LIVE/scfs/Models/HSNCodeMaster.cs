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
    [Table("HSNCODEMASTER")]
    public class HSNCodeMaster
    {
        [Key]
        public int HSNID { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Please Enter numeric or Alphanumeric string")]
        [Remote("ValidateHSNCODE", "Common", AdditionalFields = "i_HSNCODE", ErrorMessage = "This is already used.")]
        public string HSNCODE { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Please Enter numeric or Alphanumeric string")]
        [Remote("ValidateHSNDESC", "Common", AdditionalFields = "i_HSNDESC", ErrorMessage = "This is already used.")]
        public string HSNDESC { get; set; }

        [DisplayName("GST %")]
        public decimal TAXEXPRN { get; set; }

        [DisplayName("CGST %")]
        public decimal CGSTEXPRN { get; set; }

        [DisplayName("SGST %")]
        public decimal SGSTEXPRN { get; set; }

        [DisplayName("IGST %")]
        public decimal IGSTEXPRN { get; set; }

        [DisplayName("CGST %")]
        public decimal ACGSTEXPRN { get; set; }

        [DisplayName("SGST %")]
        public decimal ASGSTEXPRN { get; set; }

        [DisplayName("IGST %")]
        public decimal AIGSTEXPRN { get; set; }

        public int LMUSRID { get; set; }
        public string CUSRID { get; set; }
        [DisplayName("Status")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public Nullable<DateTime> PRCSDATE { get; set; }

    }
}