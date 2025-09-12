using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
  [Table("TRANSACTION_MODE_MASTER")]
    public class TransactionModeMaster
    {
      [Key]
      public int TRANMODE { get; set; }
      public string TRANMODEDETL { get; set; }

    }
}