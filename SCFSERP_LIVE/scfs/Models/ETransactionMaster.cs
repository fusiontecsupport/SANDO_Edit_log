using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("ETRANSACTIONMASTER")]
    public class ETransactionMaster
    {
        [Key]
        public int ETRANMID { get; set; }
        public int TRANMID { get; set; }
        public string EINVDESC { get; set; }
        public string ECINVDESC { get; set; }
    }
}