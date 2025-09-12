using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    public class VehicleTicketGateOut
    {
        public VehicleTicketDetail vtdetail { get; set; }
        public GateOutDetail godetail { get; set; }

        public VW_ESEAL_GATEINDETAIL_CONTAINERNO containerno { get; set; }
    }
}