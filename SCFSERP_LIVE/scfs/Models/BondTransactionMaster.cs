using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("BONDTRANSACTIONMASTER")]
    public class BondTransactionMaster
    {
        [Key]
        public int TRANMID { get; set; }
        public int COMPYID { get; set; }
        public int SDPTID { get; set; }
        public int REGSTRID { get; set; }
        public short TRANBTYPE { get; set; }
        public int TAXTYPE { get; set; }
        public System.DateTime TRANDATE { get; set; }
        public System.DateTime TRANTIME { get; set; }
        public int TRANNO { get; set; }
        public string TRANDNO { get; set; }
        public int CHAID { get; set; }
        public int IMPRTID { get; set; }
        public string TRANCHANAME { get; set; }
        public string TRANIMPRTNAME { get; set; }
        public decimal TRANTID { get; set; }
        public Nullable<int> BANKMID { get; set; }
        public short TRANMODE { get; set; }
        public string TRANMODEDETL { get; set; }
        public string TRANREFNO { get; set; }
        public Nullable<System.DateTime> TRANREFDATE { get; set; }
        public string TRANREFBNAME { get; set; }
        public Nullable<decimal> TRANREFAMT { get; set; }
        public decimal TRANROAMT { get; set; }
        public Nullable<decimal> TRANGAMT { get; set; }
        public Nullable<decimal> TRANSAMT { get; set; }
        public Nullable<decimal> TRANHAMT { get; set; }
        public Nullable<decimal> TRANIAMT { get; set; }
        public decimal TRANNAMT { get; set; }
        public string TRANAMTWRDS { get; set; }
        public int TRANLMID { get; set; }
        public string TRANLMNO { get; set; }
        public Nullable<System.DateTime> TRANLMDATE { get; set; }
        public string TRANNARTN { get; set; }
        public string TRANRMKS { get; set; }
        public Nullable<int> TRANPCOUNT { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public Nullable<short> DISPSTATUS { get; set; }
        public Nullable<System.DateTime> PRCSDATE { get; set; }
        public string TRANREFNAME { get; set; }
        public int BILLREFID { get; set; }
        public string TRANGSTNO { get; set; }
        public decimal TRANPTAMT { get; set; }
        public decimal TRANPLAMT { get; set; }
        public string IRNNO { get; set; }
        public string ACKNO { get; set; }
        public Nullable<System.DateTime> ACKDT { get; set; }
        public string QRCODEPATH { get; set; }
        public decimal TRAN_CGST_AMT { get; set; }
        public decimal TRAN_SGST_AMT { get; set; }
        public decimal TRAN_IGST_AMT { get; set; }

       // public string TRAN_HSNCODE { get; set; }
        public decimal TRAN_TAXABLE_AMT { get; set; }        
        //public decimal TRAN_CGST_EXPRN { get; set; }
        //public decimal TRAN_SGST_EXPRN { get; set; }
        //public decimal TRAN_IGST_EXPRN { get; set; }
        public string TRANBILLREFNO { get; set; }

        public int BCATEAID { get; set; }

    }
}