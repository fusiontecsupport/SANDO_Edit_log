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
    public class ExBondMaster
    {
        [Key]
        public int EBNDID { get; set; }
        public int BNDID { get; set; }
        public DateTime EBNDEDATE { get; set; }
        public decimal EBNDNOP { get; set; }
    }
}