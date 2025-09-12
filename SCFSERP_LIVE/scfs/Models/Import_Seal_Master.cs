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
    [Table("IMPORT_SEAL_MASTER")]
    public class Import_Seal_Master
    {
        [Key]
        public int SEALMID { get; set; }

        public DateTime SEALMDATE { get; set; }

        public decimal SEALMNO { get; set; }

        public string SEALMDESC { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public short DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }

        public int OSSDID { get; set; }
    }
}