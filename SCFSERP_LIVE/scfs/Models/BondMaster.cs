using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace scfs_erp.Models
{
    [Table("BONDMASTER")]
    public class BondMaster
    {
        [Key]
        public int BNDID { get; set; }
        public Nullable<int> COMPYID { get; set; }
        public Nullable<int> SDPTID { get; set; }
        public Nullable<int> BNDNO { get; set; }
        public string BNDDNO { get; set; }
        public Nullable<System.DateTime> BNDDATE { get; set; }
        public Nullable<System.DateTime> BNDFDATE { get; set; }
        public Nullable<System.DateTime> BNDTDATE { get; set; }
        public Nullable<System.DateTime> BNDADATE { get; set; }
        public Nullable<int> IMPRTID { get; set; }
        public Nullable<int> CHAID { get; set; }
        public Nullable<int> YRDID { get; set; }
        public Nullable<short> TYPEID { get; set; }
        public string BNDBENO { get; set; }
        public string BNDBLNO { get; set; }
        public string BNDIGMNO { get; set; }
        public string BNDLINENO { get; set; }
        public Nullable<int> BND20 { get; set; }
        public Nullable<int> BND40 { get; set; }
        public Nullable<decimal> BNDGWGHT { get; set; }
        public Nullable<decimal> BNDCIFAMT { get; set; }
        public Nullable<decimal> BNDDTYAMT { get; set; }
        public Nullable<decimal> BNDINSAMT { get; set; }
        public Nullable<decimal> BNDBCIFAMT { get; set; }
        public Nullable<decimal> BNDBDTYAMT { get; set; }
        public Nullable<decimal> BNDBINSAMT { get; set; }
        public Nullable<int> GWNID { get; set; }
        public Nullable<int> PRDTGID { get; set; }
        public string PRDTDESC { get; set; }
        public Nullable<int> PRDTTID { get; set; }
        public Nullable<decimal> BNDNOP { get; set; }
        public Nullable<decimal> BNDSPC { get; set; }
        public Nullable<int> BNDBNOP { get; set; }
        public Nullable<decimal> BNDBSPC { get; set; }
        public Nullable<decimal> BNDHAMT { get; set; }
        public Nullable<System.DateTime> STRGSDATE { get; set; }
        public Nullable<System.DateTime> STRGEDATE { get; set; }
        public Nullable<System.DateTime> INSRSDATE { get; set; }
        public Nullable<System.DateTime> INSREDATE { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public Nullable<short> DISPSTATUS { get; set; }
        public Nullable<System.DateTime> PRCSDATE { get; set; }
        public Nullable<System.DateTime> BNDBEDATE { get; set; }
        public Nullable<System.DateTime> BNDBLDATE { get; set; }
        public short BNDTYPE { get; set; }
        public short BNDETYPE { get; set; }
    }
}