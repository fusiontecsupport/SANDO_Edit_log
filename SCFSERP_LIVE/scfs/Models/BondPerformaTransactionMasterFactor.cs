using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("BONDPERFORMATRANSACTIONMASTERFACTOR")]
    public class BondPerformaTransactionMasterFactor
    {
        [Key]
        public int TRANMFID { get; set; }
        public Nullable<int> TRANMID { get; set; }
        public Nullable<int> CFID { get; set; }
        public Nullable<decimal> DEDEXPRN { get; set; }
        public Nullable<decimal> DEDNOS { get; set; }
        public string DEDMODE { get; set; }
        public Nullable<int> DEDTYPE { get; set; }
        public Nullable<int> DEDORDR { get; set; }
        public Nullable<short> CFOPTN { get; set; }
        public Nullable<short> DORDRID { get; set; }
        public Nullable<decimal> DEDVALUE { get; set; }
    }
}