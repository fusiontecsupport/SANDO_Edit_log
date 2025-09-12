using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("VW_GATEIN_BLOCK_CONTNRNO_ASSGN")]
    public class VW_Gatein_Block_Contnrno_Assgn
    {
        [Key]
        public int GIDID { get; set; }
        public string CONTNRNO { get; set; }
       
        public int CONTNRSID { get; set; }
        public string GPLNO { get; set; }
        public string VOYNO { get; set; }
        public string IGMNO { get; set; }
        public string VSLNAME { get; set; }
        public string IMPRTNAME { get; set; }
        public string STMRNAME { get; set; }
        //public System.DateTime GIDATE { get; set; }

        public string GIDATE { get; set; }
        public Nullable<int> GBDID { get; set; }
        public Nullable<int> OSGID { get; set; }
        
    }
}