using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace scfs_erp.Models
{
    [Table("STUFFING_JSON_DETAIL")]
    public class Stuffing_Json_Detail
    {
        [Key]
        public int STFXID { get; set; }
        public int STFDID { get; set; }
        public int STF_MSG_TID { get; set; }
        public int PRID { get; set; }
        public int REID { get; set; }
        public int RPTYPEID { get; set; }
        public string STF_RPTYPE_CODE { get; set; }
        public string STF_RLOCT_NAME { get; set; }
        public string STF_RLOCT_CODE { get; set; }
        public string STF_AP_PANNO { get; set; }
        public string STF_CONTNRNO { get; set; }
        public int EQTID { get; set; }
        public string STF_CONTNRSDESC { get; set; }
        public int ELSID { get; set; }
        public string STF_AEQUIPDESC { get; set; }
        //public string STF_FDSTNCODE { get; set; }
        public int FDID { get; set; }
        public DateTime STF_EVENTDATE { get; set; }
        public int ESTYPEID { get; set; }
        public string STF_EQSEALNO { get; set; }
        public string STF_OTHR_EQID { get; set; }
        public int ESID { get; set; }
        public string STF_EQPKG_DESC { get; set; }
        public decimal STF_EQPKG_QTY { get; set; }
        public int EUQCID { get; set; }
        public DateTime CREATEDDATETIME { get; set; }
        public int STFXNO { get; set; }
        public String STFXDNO { get; set; }
    }
}