using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("PORT_OF_REPORTING")]
    public class PortofReporting
    {
        [Key]
        public int PRID { get; set; }
        public string PRDESC { get; set; }
        public int CUSRID { get; set; }
        public int LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}