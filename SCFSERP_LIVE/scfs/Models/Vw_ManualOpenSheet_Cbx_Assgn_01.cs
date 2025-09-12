using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("VW_MANUALOPENSHEET_CONTAINER_CBX_ASSGN_01")]
    public class Vw_ManualOpensheet_Cbx_Assgn_01
    {
        [Key]
        public int GBDID { get; set; }
        public int GIDID { get; set; }

        public DateTime GIDATE { get; set; }

        public string IMPRTNAME { get; set; }
        public string VOYNO { get; set; }

        public string STMRNAME { get; set; }

        public string CONTNRSDESC { get; set; }

        public string CONTNRNO { get; set; }

        public string LPSEALNO { get; set; }

        public string CSEALNO { get; set; }

        public string VSLNAME { get; set; }


        // public string VOYNO { get; set; }

        public string IGMNO { get; set; }

        public string GPLNO { get; set; }


    }
}