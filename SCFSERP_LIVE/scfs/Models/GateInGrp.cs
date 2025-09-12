using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using scfs.Data;
using scfs_erp.Models;


namespace scfs_erp.Models
{
    public class GateInGrp
    {
        public GateInDetail gateindata { get; set; }
        public ExportShippingBillMaster shippingbilldata { get; set; }
    }
}