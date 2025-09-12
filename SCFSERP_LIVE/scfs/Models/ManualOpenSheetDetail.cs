using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{

    [Table("MANUALOPENSHEETDETAIL")]
    public class ManualOpenSheetDetail
    {
        [Key]
        public int OSDID { get; set; }
        public int OSMID { get; set; }
        public int GIDID { get; set; }

        public DateTime GIDATE { get; set; }

        public string LSEALNO { get; set; }
        public string SSEALNO { get; set; }

        public short INSPTYPE { get; set; }

        public int BILLEDID { get; set; }

        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}