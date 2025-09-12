using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{

    [Table("OPENSHEETMASTER")]
    public class OpenSheetMaster
    {
        [Key]
        public int OSMID { get; set; }
        public int COMPYID { get; set; }
        public int SDPTID { get; set; }
        public Nullable<DateTime> OSMDATE { get; set; }
        public Nullable<DateTime> OSMTIME { get; set; }
        public int OSMNO { get; set; }
        public string OSMDNO { get; set; }
        public Nullable<int> CHAID { get; set; }
        
        public string OSMNAME { get; set; }
        public string OSMCNAME { get; set; }
        
        public string OSMIGMNO { get; set; }
        public string OSMVSLNAME { get; set; }
        
        public string BOENO { get; set; }
        public Nullable<DateTime> BOEDATE { get; set; }
        public Nullable<Int16> OSMTYPE { get; set; }
        public Nullable<Int16> DOTYPE { get; set; }
        public Nullable<DateTime> DODATE { get; set; }
        
        public string OSMLNO { get; set; }

        public string DONO { get; set; }
        public Nullable<DateTime> DOIDATE { get; set; }

        public Nullable<Int16> SCTYPE { get; set; }

        public string SCDESC { get; set; }
        public Nullable<DateTime> SCDATE { get; set; }
        public Nullable<DateTime> SCTIME { get; set; }
        public string SCREMRKS { get; set; }
       
        public string OSMBLNO { get; set; }
        public string OSMLNAME { get; set; }
       
        public int OSMUNITID { get; set; }
        public int OSMTNOC { get; set; }
        public int OSMFNOC { get; set; }
        public string OSMLCATENAME { get; set; }
        public int OSMLCATEID { get; set; }
        public Nullable<decimal>  OSMAAMT { get; set; }
       
        public Nullable<decimal> OSMWGHT { get; set; }
      
        public Nullable<decimal> OSMNOP { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public Nullable<Int16>  DISPSTATUS { get; set; }
        public Nullable<DateTime> PRCSDATE { get; set; }
        public Nullable<Int16> OSMLDTYPE { get; set; }
        public Int16 OSBILLEDTO { get; set; }
        public int OSBILLREFID { get; set; }
        public string OSBILLREFNAME { get; set; }
        public Nullable<decimal> OSMDUTYAMT { get; set; }
        public Nullable<DateTime> OSMBLDATE { get; set; }
        public Nullable<DateTime> OSMIGMDATE { get; set; }

        //public int OSCATEAID { get; set; }
        public Nullable<int> OSBCHACATEAID { get; set; }
        public Nullable<int> OSBBCHACATEAID { get; set; }
        public string OSBCHACATEAGSTNO { get; set; }
        public string OSBBCHACATEAGSTNO { get; set; }
        public string OSBCHAADDR1 { get; set; }
        public string OSBBCHAADDR1 { get; set; }
        public string OSBCHAADDR2 { get; set; }
        public string OSBBCHAADDR2 { get; set; }
        public string OSBCHAADDR3 { get; set; }
        public string OSBBCHAADDR3 { get; set; }
        public string OSBCHAADDR4 { get; set; }
        public string OSBBCHAADDR4 { get; set; }        
        public int OSBCHASTATEID { get; set; }
        public int OSBBCHASTATEID { get; set; }
        public string OOCNO { get; set; }
        public Nullable<DateTime> OOCDATE { get; set; }
    }
}