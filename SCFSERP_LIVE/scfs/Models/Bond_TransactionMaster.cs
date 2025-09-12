using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

//namespace scfs_erp.Models
//{
namespace scfs_erp.Models
{
    [Table("TRANSACTIONMASTER")]
    public class Bond_TransactionMaster
    {
        [Key]
        public int TRANMID { get; set; }
        public DateTime TRANDATE { get; set; }
        public Int16 TRANBTYPE { get; set; }
    }
}