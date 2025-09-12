using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace scfs_erp.Models
{
    [Table("BILLENTRYDETAIL")]
    public class BillEntryDetail
    {
        [Key]
        public int BILLEDID { get; set; }
        public int BILLEMID { get; set; }
        public int GIDID { get; set; }
        public DateTime GIDATE { get; set; }
        public decimal DNOP { get; set; }
        public decimal DWGHT { get; set; }

        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}