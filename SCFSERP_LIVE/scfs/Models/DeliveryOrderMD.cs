using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using scfs.Data;

namespace scfs_erp.Models
{
    public class DeliveryOrderMD
    {
        public DeliveryOrderMaster masterdata { get; set; }
        public List<DeliveryOrderDetail> detaildata { get; set; }
        public List<VW_DELIVERY_ORDER_DETAIL_GRID_ASSGN> detailedit { get; set; }
    }
}