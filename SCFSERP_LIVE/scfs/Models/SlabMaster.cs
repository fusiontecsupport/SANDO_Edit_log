using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{

    [Table("SLABMASTER")]
    public class SlabMaster
    {
        [Key]
        public int SLABMID { get; set; }

        public int COMPYID { get; set; }       

        public DateTime SLABMDATE { get; set; }

        public int SLABTID { get; set; }

        public int TARIFFMID { get; set; }

        public int STMRID { get; set; }

        public Int16 CHRGETYPE { get; set; }


        //public string SLABTDESC { get; set; }

        public int CONTNRSID { get; set; }

        public Int16 SDTYPE { get; set; }

        public Int16 YRDTYPE { get; set; }

        public Int16 HTYPE { get; set; }

        public Int16 WTYPE { get; set; }

        public decimal SLABMIN { get; set; }

        public decimal SLABMAX { get; set; }

        public decimal SLABAMT { get; set; }
        //public string SLABTCODE { get; set; }       
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]

        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
        
        // CODE ADDED BY RAJEHS ON 28-07-2021 <S> FOR SDPT TYPE BASED SLAB 
        public int SDPTTYPEID { get; set; }
        // CODE ADDED BY RAJEHS ON 28-07-2021 <E> FOR SDPT TYPE BASED SLAB         
    }
}