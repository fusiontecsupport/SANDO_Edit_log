using scfs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    public class ShutoutVehicleTicketDetail
    {
        public ShutoutVTDetail shutoutvtdetail { get; set; }
        public GateOutDetail shutoutgodetail { get; set; }
        public List<PR_SHUTOUT_CARGO_FLX_ASSGN_Result> shutoutdetail { get; set; }


    }
}