using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("CREDITNOTE_TRANSACTIONMASTER")]
    
    public class CreditNote_TransactionMaster
    {
        [Key]
        public int TRANMID { get; set; }
        public int COMPYID { get; set; }
        public int SDPTID { get; set; }

        public int REGSTRID { get; set; }
        public short TRANBTYPE { get; set; }
        public DateTime TRANDATE { get; set; }
        public DateTime TRANTIME { get; set; }
        public int TRANNO { get; set; }
        public string TRANDNO { get; set; }


        public int TRANREFID { get; set; }
        public string TRANREFNAME { get; set; }
        public int LCATEID { get; set; }
        public Decimal TRANTID { get; set; }
        public int BANKMID { get; set; }
        public short TRANMODE { get; set; }
        public string TRANMODEDETL { get; set; }
        public string TRANREFNO { get; set; }
        public DateTime TRANREFDATE { get; set; }
        public string TRANREFBNAME { get; set; }
        public decimal TRANREFAMT { get; set; }
        public decimal TRANROAMT { get; set; }
        public decimal TRANGAMT { get; set; }
        public decimal TRANSAMT { get; set; }
        public decimal TRANHAMT { get; set; }
        public decimal TRANNAMT { get; set; }
        public string TRANAMTWRDS { get; set; }
        public int TRANLMID { get; set; }
        public string TRANLMNO { get; set; }
        public DateTime TRANLMDATE { get; set; }
        public int TRANLSID { get; set; }
        public string TRANLSNO { get; set; }
        public DateTime TRANLSDATE { get; set; }

        public string TRANNARTN { get; set; }
        public string TRANRMKS { get; set; }


        public string TRANCSNAME { get; set; }
        public int TRANPCOUNT { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }


        public decimal TRANEAMT { get; set; }
        public decimal TRANFAMT { get; set; }
        public decimal TRANPAMT { get; set; }
        public decimal TRANTCAMT { get; set; }
        public decimal TRANAAMT { get; set; }
        public decimal TRANINOC1 { get; set; }
        public decimal TRANINOC2 { get; set; }


        public string TRANHBLNO { get; set; }
        public string TRANPONO { get; set; }
        public string TRANIMPADDR1 { get; set; }
        public string TRANIMPADDR2 { get; set; }
        public string TRANIMPADDR3 { get; set; }
        public string TRANIMPADDR4 { get; set; }

        public int LEMID { get; set; }
        public decimal TRANAHAMT { get; set; }

        public string STRG_HSNCODE { get; set; }
        public string HANDL_HSNCODE { get; set; }
        public decimal STRG_TAXABLE_AMT { get; set; }
        public decimal HANDL_TAXABLE_AMT { get; set; }
        public decimal STRG_CGST_EXPRN { get; set; }
        public decimal STRG_SGST_EXPRN { get; set; }
        public decimal STRG_IGST_EXPRN { get; set; }
        public decimal STRG_CGST_AMT { get; set; }
        public decimal STRG_SGST_AMT { get; set; }
        public decimal STRG_IGST_AMT { get; set; }
        public decimal HANDL_CGST_EXPRN { get; set; }
        public decimal HANDL_SGST_EXPRN { get; set; }
        public decimal HANDL_IGST_EXPRN { get; set; }
        public decimal HANDL_CGST_AMT { get; set; }
        public decimal HANDL_SGST_AMT { get; set; }
        public decimal HANDL_IGST_AMT { get; set; }
        public string TRANGSTNO { get; set; }
        public int CATEAID { get; set; }

        public Nullable<int> STATEID { get; set; }

        public string CATEAGSTNO { get; set; }

        public int TALLYSTAT { get; set; }
        public decimal TRAN_COVID_DISC_AMT { get; set; }

        public string IRNNO { get; set; }
        public string ACKNO { get; set; }
        public Nullable<DateTime> ACKDT { get; set; }
        public string QRCODEPATH { get; set; }

        public decimal TRANCGSTAMT { get; set; }
        public decimal TRANSGSTAMT { get; set; }
        public decimal TRANIGSTAMT { get; set; }
        public string TRANBILLREFNO { get; set; }
        public int TALLY_TRANREFID { get; set; }
        public string TALLY_TRANREFNAME { get; set; }
        public int TALLY_CATEAID { get; set; }

    }
}