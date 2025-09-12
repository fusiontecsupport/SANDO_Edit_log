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
    public class Z_TRANSACTION_DOSPRINT_ASSGN
    {
        [Key]

        public int TRANMID { get; set; }
        public int BNDID { get; set; }
        public int BNDNOP { get; set; }
        public int EBNDNOP { get; set; }
        public int EBNDSPC { get; set; }
        public decimal BBNDNOP { get; set; }
    }
}