using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("STUFFINGMASTER")]
    public class StuffingMaster
    {
        [Key]
        public int STFMID { get; set; }
        public int COMPYID { get; set; }
        public DateTime STFMDATE { get; set; }
        public DateTime STFMTIME { get; set; }
        public int STFMNO { get; set; }
        public string STFMDNO { get; set; }
        public int CHAID { get; set; }
        public string STFMNAME { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public Nullable<Int16> DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
        public Nullable<int> STFTID { get; set; }
        public Nullable<int> LCATEID { get; set; }
        public string LCATENAME { get; set; }
        public int TGID { get; set; }
        public int EOPTID { get; set; }
        public int SLABTID { get; set; }
        public Int16 STFBILLEDTO { get; set; }
        public int STFBILLREFID { get; set; }
        public string STFBILLREFNAME { get; set; }
        public int STFCATEAID { get; set; }

        //public Nullable<int> SLABTID { get; set; }
        public string STF_SBILL_RNO { get; set; }
        public string STF_FORM13_RNO { get; set; }

        public string TRANIMPADDR1 { get; set; }
        public string TRANIMPADDR2 { get; set; }
        public string TRANIMPADDR3 { get; set; }
        public string TRANIMPADDR4 { get; set; }
        public int STATEID { get; set; }
        public string CATEAGSTNO { get; set; }
        public int STFBCATEAID { get; set; }
        public string STFBCHAGSTNO { get; set; }
        public string STFBCHAADDR1 { get; set; }
        public string STFBCHAADDR2 { get; set; }
        public string STFBCHAADDR3 { get; set; }
        public string STFBCHAADDR4 { get; set; }
        public int STFBCHASTATEID { get; set; }

    }
}