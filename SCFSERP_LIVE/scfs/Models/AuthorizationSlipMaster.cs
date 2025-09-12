using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("AUTHORIZATIONSLIPMASTER")]
    public class AuthorizationSlipMaster
    {
        [Key]
        public int ASLMID { get; set; }
        public int SDPTID { get; set; }
        public int COMPYID { get; set; }
        public DateTime ASLMDATE { get; set; }
        public DateTime ASLMTIME { get; set; }
        public int ASLMNO { get; set; }
        public string ASLMDNO { get; set; }
        public Nullable<short> ASLMTYPE { get; set; }
        public int NGINO { get; set; }
        public int NGIDID { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }

        //public Nullable<short> ASLMLTYPE { get; set; }
    }
}