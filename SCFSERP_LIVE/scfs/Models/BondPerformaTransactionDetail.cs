using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("BONDPERFORMATRANSACTIONDETAIL")]
    public class BondPerformaTransactionDetail
    {
        [Key]
        public int TRANDID { get; set; }
        public int TRANMID { get; set; }
        public int BNDID { get; set; }
        public int EBNDID { get; set; }
        public string TRANDBNDNO { get; set; }
        public string TRANDEBNDNO { get; set; }
        public int TARIFFMID { get; set; }
        public int SLABTID { get; set; }
        public int CONTNRSID { get; set; }
        public int YRDID { get; set; }
        public Nullable<short> TRANCTYPE { get; set; }
        public Nullable<short> TRANOTYPE { get; set; }
        public Nullable<short> TRANHTYPE { get; set; }
        public Nullable<System.DateTime> TRANCDATE { get; set; }
        public Nullable<System.DateTime> TRANSFDATE { get; set; }
        public Nullable<System.DateTime> TRANSEDATE { get; set; }
        public Nullable<System.DateTime> TRANIFDATE { get; set; }
        public Nullable<System.DateTime> TRANIEDATE { get; set; }
        public decimal TRANDQTY { get; set; }
        public Nullable<decimal> TRANDNOP { get; set; }
        public decimal TRANDRATE { get; set; }
        public Nullable<decimal> TRANDWGHT { get; set; }
        public Nullable<decimal> TRANDCIFAMT { get; set; }
        public Nullable<decimal> TRANDDTYAMT { get; set; }
        public Nullable<decimal> TRANDINSAMT { get; set; }
        public Nullable<decimal> TRANDEIAMT { get; set; }
        public decimal TRANDGAMT { get; set; }
        public Nullable<decimal> TRANDSAMT { get; set; }
        public Nullable<decimal> TRANDHAMT { get; set; }
        public Nullable<decimal> TRANDIAMT { get; set; }
        public decimal TRANDNAMT { get; set; }
        public int TRANDAID { get; set; }
        public short DISPSTATUS { get; set; }
        public string RCOLDESC1 { get; set; }
        public string RCOLDESC2 { get; set; }
        public string RCOLDESC3 { get; set; }
        public string RCOLDESC4 { get; set; }
        public Nullable<int> VTDID { get; set; }
        public Nullable<int> STATETYPE { get; set; }
        public string TRAND_HSN_CODE { get; set; }
        public string INS_TRAND_HSN_CODE   {get; set; }
        
        public decimal TRANDPLAMT { get; set; }
        public decimal TRANDPTAMT { get; set; }
        public decimal TRAND_TAXABLE_AMT { get; set; }
        public decimal INS_TRAND_TAXABLE_AMT { get; set; }
        
        public decimal TRAND_CGST_EXPRN { get; set; }
        public decimal TRAND_SGST_EXPRN { get; set; }
        public decimal TRAND_IGST_EXPRN { get; set; }
        public decimal TRAND_CGST_AMT { get; set; }
        public decimal TRAND_SGST_AMT { get; set; }
        public decimal TRAND_IGST_AMT { get; set; }
        public decimal INS_TRAND_CGST_AMT { get; set; }
        public decimal INS_TRAND_SGST_AMT { get; set; }
        public decimal INS_TRAND_IGST_AMT { get; set; }
        public Nullable<int> PERIODTID { get; set; }
        public string BILLDESC { get; set; }

        public Nullable<decimal> EXBNDVTSPC { get; set; }

    }
}