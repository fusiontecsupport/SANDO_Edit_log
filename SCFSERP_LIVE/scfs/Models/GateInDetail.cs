using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("GATEINDETAIL")]
    public class GateInDetail
    {
        [Key]
        public int GIDID { get; set; }

        public int COMPYID { get; set; }

        public int SDPTID { get; set; }

        public DateTime GIDATE { get; set; }

        public DateTime GITIME { get; set; }

        public Nullable<DateTime> GICCTLDATE { get; set; }

        public Nullable<DateTime> GICCTLTIME { get; set; }

        public int GINO { get; set; }

        public string GIDNO { get; set; }

        public int GIVHLTYPE { get; set; }

        public int TRNSPRTID { get; set; }

        public string TRNSPRTNAME { get; set; }

        public string VHLNO { get; set; }

        public string DRVNAME { get; set; }

        public string GPREFNO { get; set; }

        public int IMPRTID { get; set; }

        public string IMPRTNAME { get; set; }

        public int STMRID { get; set; }

        public string STMRNAME { get; set; }

        public int CONTNRTID { get; set; }

        public int CONTNRID { get; set; }

        public int CONTNRSID { get; set; }

        public string CONTNRNO { get; set; }

        public string LPSEALNO { get; set; }

        public string CSEALNO { get; set; }

        public int YRDID { get; set; }

        public int VSLID { get; set; }

        public string VSLNAME { get; set; }

        public string VOYNO { get; set; }

        public int PRDTGID { get; set; }

        public string PRDTDESC { get; set; }

        public int UNITID { get; set; }

		public decimal GPWGHT { get; set; }

		public string IGMNO { get; set; }

		public string GPLNO { get; set; }

		public int GPWTYPE { get; set; }

		public int GPSTYPE { get; set; }

		public int GPETYPE { get; set; }

		public int BOEDID { get; set; }

		public int INVDID { get; set; }

		public int RGIDID { get; set; }

		public string GIREMKRS { get; set; }

		public int ROWID { get; set; }

		public int SLOTID { get; set; }

		public string CUSRID { get; set; }

		public string LMUSRID { get; set; }

		public int DISPSTATUS { get; set; }

		public Nullable<DateTime> PRCSDATE { get; set; }

		public decimal GPEAMT { get; set; }

		public int CHAID { get; set; }

		public string CHANAME { get; set; }

		public string AVHLNO { get; set; }

		public decimal GPNOP { get; set; }

		public int PRDTTID { get; set; }

		public string GIISOCODE { get; set; }

		public string GIDMGDESC { get; set; }

		public int NGIDID { get; set; }

		public decimal GPNWGHT { get; set; }

		public decimal GPGWGHT { get; set; }

		public decimal GPCBM { get; set; }

		public decimal GPLENGTH { get; set; }

		public decimal GPWIDTH { get; set; }

		public decimal GPHEIGHT { get; set; }

		public string GPBLNO { get; set; }

		public decimal GPAAMT { get; set; }

		public int CLNTID { get; set; }

		public string CLNTNAME { get; set; }

		public string SHPRNAME { get; set; }

		public string GPPLCNAME { get; set; }

		public decimal GPTWGHT { get; set; }

		public int CONTNRFID { get; set; }

		public int CONDTNID { get; set; }

		public int GRADEID { get; set; }

		public int CNTNRSID { get; set; }

		public int GPPTYPE { get; set; }

		public int GFCLTYPE { get; set; }

		public int GSEALTYPE { get; set; }

		public int GSECTYPE { get; set; }

		public string GSEALREMKRS { get; set; }

		public string GSECREMKRS { get; set; }

		public string GPNRNO { get; set; }

		public string ESBNO { get; set; }

		public string EVSLRNO { get; set; }

		public string EMONO { get; set; }

		public string EEGMNO { get; set; }

		public Nullable<DateTime> ESBDATE { get; set; }		

		public int GPSCNTYPE { get; set; }

		

		public Nullable<DateTime> IGMDATE { get; set; }

		public string BLNO { get; set; }

		public int GPMODEID { get; set; }

		public string BOENO { get; set; }

		public Nullable<DateTime> BOEDATE { get; set; }

		public string CFSNAME { get; set; }

		public int BCHAID { get; set; }

		public string BCHANAME { get; set; }
		
		//columns added by Rajesh on 23-07-2021 for CR <S>
		public Nullable<int> VHLMID { get; set; }
		public string GTRNSPRTNAME { get; set; }
		//columns added by Rajesh on 23-07-2021 for CR <S>

		//columns added by Yamuna on 27-07-2021 for CR <S>
		public Nullable<int> STAGID { get; set; }

		public Nullable<int> GDWNID { get; set; }

		public Nullable<int> ESBMID { get; set; }

		public Nullable<int> EXPRTRID { get; set; }

		public string EXPRTRNAME { get; set; }

		public string DRVMBLNO { get; set; }

		public string DRVLCNO { get; set; }
		public int PRE_CHAID { get; set; }
		public string PRE_CHANAME { get; set; }

		public int GPSCNMTYPE { get; set; }

		//columns added by Yamuna on 27-07-2021 for CR <S>
	}
}