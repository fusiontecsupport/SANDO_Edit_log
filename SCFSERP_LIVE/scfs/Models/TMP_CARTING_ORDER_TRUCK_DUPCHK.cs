using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table ("TMP_CARTING_ORDER_TRUCK_DUPCHK")]
    public class TMP_CARTING_ORDER_TRUCK_DUPCHK
    {
        [Key]
        public string CUSRID { get; set; }
        public string TRUCKNO { get; set; }
        public int GIDID { get; set; }
        

    }
}