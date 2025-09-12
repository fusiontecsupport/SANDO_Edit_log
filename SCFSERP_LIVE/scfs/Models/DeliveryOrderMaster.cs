using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("DELIVERYORDERMASTER")]
    public class DeliveryOrderMaster
    {
        [Key]
        public int DOMID { get; set; }
        public int SDPTID { get; set; }
        public int COMPYID { get; set; }
        public DateTime DODATE { get; set; }
        public DateTime DOTIME { get; set; }
        public int DONO { get; set; }
        public string DODNO { get; set; }
        public int DOREFID { get; set; }
        public string DOREFNAME { get; set; }
        public string DOREFNO { get; set; }
        public DateTime DOREFDATE { get; set; }
        public string DONARTN { get; set; }
        public string DORMKS { get; set; }
        public int DOPCOUNT { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
        public short DOMODE { get; set; }
        public string DOMODEDETL { get; set; }
        public string DOREFBNAME { get; set; }
        public decimal DOREFAMT { get; set; }
        public int TARIFFMID { get; set; }
        public int BANKMID { get; set; }
        //public string OOCNO { get; set; }
        //public DateTime OOCDATE { get; set; }

    }
}