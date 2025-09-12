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
    [Table("BONDTARIFFMASTER")]
    public class BondTariffMaster
    {
        [Key]
        public int TARIFFMID { get; set; }
        public string TARIFFMDESC { get; set; }
        public string TARIFFMCODE { get; set; }
        public int SDPTID { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public System.DateTime PRCSDATE { get; set; }
    }
}