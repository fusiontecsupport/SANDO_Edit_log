using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("OPENSHEET_SEAL_DETAIL")]
    public class OpenSheetSealDetails
    {
        [Key]
        public int OSSID { get; set; }

        public DateTime OSSDATE { get; set; }

        public int OSMID { get; set; }
        
        public int SEALMID { get; set; }

        public string OSSDESC { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public Int16 DISPSTATUS { get; set; }

        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }

        public int COMPYID { get; set; }

        public Nullable<int> OSDID { get; set; }
    }
}