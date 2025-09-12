
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{

    [Table("DELIVERYORDERDETAIL")]
    public class DeliveryOrderDetail
    {
        [Key]
        public int DODID { get; set; }
        public int DOMID { get; set; }
        public int BILLEDID { get; set; }
        public int DODREFID { get; set; }
        public string DODREFNAME { get; set; }
        public string DODREFNO { get; set; }
        public DateTime DOIDATE { get; set; }
        public DateTime DOVDATE { get; set; }
        public short DISPSTATUS { get; set; }
    }
}

