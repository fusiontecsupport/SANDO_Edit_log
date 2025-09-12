using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs.Models
{
    [Table("DEBITNOTE_TRANSACTIONDETAIL")]
    public class DebitNote_TransactionDetail
    {
        [Key]
        public int TRANDID { get; set; }
        public int TRANMID { get; set; }
        public int SLABTID { get; set; }
        public int BILLEDID { get; set; }
        public int TRANDREFID { get; set; }
        public string TRANDREFNAME { get; set; }
        public string TRANDREFNO { get; set; }

        public DateTime TRANIDATE { get; set; }
        public DateTime TRANSDATE { get; set; }
        public DateTime TRANEDATE { get; set; }
        public DateTime TRANVDATE { get; set; }

        public int TARIFFMID { get; set; }
        public short TRANOTYPE { get; set; }
        public short TRANYTYPE { get; set; }

        public decimal RCOL1 { get; set; }
        public decimal RCOL2 { get; set; }
        public decimal RCOL3 { get; set; }
        public decimal RCOL4 { get; set; }
        public decimal RAMT1 { get; set; }
        public decimal RAMT2 { get; set; }
        public decimal RAMT3 { get; set; }
        public decimal RAMT4 { get; set; }
        public decimal RCAMT1 { get; set; }
        public decimal RCAMT2 { get; set; }
        public decimal RCAMT3 { get; set; }
        public decimal RCAMT4 { get; set; }
        public decimal TRANDQTY { get; set; }
        public decimal TRANDRATE { get; set; }
        public decimal TRANDWGHT { get; set; }
        public decimal TRANDGAMT { get; set; }
        public decimal TRANDSAMT { get; set; }
        public decimal TRANDHAMT { get; set; }
        public decimal TRANDNAMT { get; set; }

        public decimal TRANDAHAMT { get; set; }

        public int TRANDAID { get; set; }
        public string TRANVHLFROM { get; set; }
        public string TRANVHLTO { get; set; }
        public short DISPSTATUS { get; set; }

        public int STFDID { get; set; }
        public int SBDID { get; set; }
        public decimal TRANDNOP { get; set; }
        public decimal TRANDEAMT { get; set; }
        public decimal TRANDFAMT { get; set; }
        public decimal TRANDPAMT { get; set; }
        public decimal TRANDAAMT { get; set; }

        public decimal RCOL5 { get; set; }
        public decimal RCOL6 { get; set; }
        public decimal RCOL7 { get; set; }

        public decimal RAMT5 { get; set; }
        public decimal RAMT6 { get; set; }
        public decimal RAMT7 { get; set; }

        public decimal RCAMT5 { get; set; }
        public decimal RCAMT6 { get; set; }
        public decimal RCAMT7 { get; set; }

        public int ACHEADID { get; set; }
        public string TRANDDESC { get; set; }
        public decimal TRAND_COVID_DISC_AMT { get; set; }

        public decimal TRAND_STRG_CGST_EXPRN { get; set; }

        public decimal TRAND_STRG_IGST_EXPRN { get; set; }

        public decimal TRAND_STRG_SGST_EXPRN { get; set; }

        public decimal TRAND_HANDL_CGST_EXPRN { get; set; }

        public decimal TRAND_HANDL_IGST_EXPRN { get; set; }

        public decimal TRAND_HANDL_SGST_EXPRN { get; set; }

        public decimal TRAND_STRG_CGST_AMT { get; set; }

        public decimal TRAND_STRG_IGST_AMT { get; set; }

        public decimal TRAND_STRG_SGST_AMT { get; set; }

        public decimal TRAND_HANDL_CGST_AMT { get; set; }

        public decimal TRAND_HANDL_IGST_AMT { get; set; }

        public decimal TRAND_HANDL_SGST_AMT { get; set; }

        public string TRANDHSNCODE { get; set; }

        //public decimal TRANDIAMT { get; set; }
        //public decimal TRANDHALTAMT { get; set; }
        //public decimal TRANDTRNSPRTAMT { get; set; }

    }
}