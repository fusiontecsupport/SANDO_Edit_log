using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("PERFORMA_TRANSACTIONMASTERFACTOR")]
    public class Performa_TransactionMasterFactor
    {
        [Key]
        public int TRANMFID { get; set; }
        public int TRANMID { get; set; }
        public int CFID { get; set; }
        public decimal DEDEXPRN { get; set; }
        public decimal DEDNOS { get; set; }
        public string DEDMODE { get; set; }
        public int DEDTYPE { get; set; }
        public int DEDORDR { get; set; }
        public short CFOPTN { get; set; }
        public short DORDRID { get; set; }
        public decimal DEDVALUE { get; set; }

        public string DEDCFDESC { get; set; }
    }
}