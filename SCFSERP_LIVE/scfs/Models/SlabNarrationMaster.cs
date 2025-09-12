using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("SLABNARRATIONMASTER")]
    public class SlabNarrationMaster
    {
        [Key]
        public int SLABNID { get; set; }

        public int TARIFFMID { get; set; }
        public int SLABTID { get; set; }      

        public string SLABNARTN { get; set; }

        public int BILLTID { get; set; }

        public DateTime PRCSDATE { get; set; }

    }
}