using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("EXBONDMASTER")]
    public class Ex_BondMaster
    {
        [Key] 
        public int EBNDID { get; set; }
        public Nullable<int> COMPYID { get; set; }
        public Nullable<int> SDPTID { get; set; }
        public Nullable<int> EBNDNO { get; set; }
        public string EBNDDNO { get; set; }
        public Nullable<System.DateTime> EBNDDATE { get; set; }
        public Nullable<int> IMPRTID { get; set; }
        public Nullable<int> CHAID { get; set; }
        public string EBNDBENO { get; set; }
        public Nullable<System.DateTime> EBNDBEDATE { get; set; }
        public Nullable<int> BNDID { get; set; }
        public Nullable<short> TYPEID { get; set; }
        public Nullable<int> CONTNRSID { get; set; }
        public Nullable<int> PRDTGID { get; set; }
        public Nullable<short> EBNDCTYPE { get; set; }
        public Nullable<decimal> EBNDNOC { get; set; }
        public Nullable<decimal> EBNDNOP { get; set; }
        public Nullable<decimal> EBNDSPC { get; set; }
        public Nullable<decimal> EBNDASSAMT { get; set; }
        public Nullable<decimal> EBNDDTYAMT { get; set; }
        public Nullable<decimal> EBNDINSAMT { get; set; }
        public Nullable<decimal> EBNDBSPC { get; set; }
        public Nullable<decimal> EBNDBCFAMT { get; set; }
        public Nullable<decimal> EBNDBDTYAMT { get; set; }
        public Nullable<decimal> EBNDDBINSAMT { get; set; }
        public Nullable<System.DateTime> STRGSDATE { get; set; }
        public Nullable<System.DateTime> STRGEDATE { get; set; }
        public Nullable<System.DateTime> INSRSDATE { get; set; }
        public Nullable<System.DateTime> INSREDATE { get; set; }
        public Nullable<int> INVDID { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public Nullable<short> DISPSTATUS { get; set; }
        public Nullable<System.DateTime> PRCSDATE { get; set; }
        public Nullable<System.DateTime> EBNDEDATE { get; set; }
        public string EBNDREMKRS { get; set; }
    }
}