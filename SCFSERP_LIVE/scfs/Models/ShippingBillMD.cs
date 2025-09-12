using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    public class ShippingBillMD
    {
        public List<ShippingBillMaster> masterdata { get; set; }

        public List<ShippingBillDetail> detaildata { get; set; }
    }
}