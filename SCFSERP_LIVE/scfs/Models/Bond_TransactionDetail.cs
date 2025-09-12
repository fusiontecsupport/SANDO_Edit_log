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
    [Table("TRANSACTIONDETAIL")]
    public class Bond_TransactionDetail
    {
        [Key]
        public int TRANDID { get; set; }
        public int TRANMID { get; set; }
        public int BNDID { get; set; }
        public int EBNDID { get; set; }
    }
}