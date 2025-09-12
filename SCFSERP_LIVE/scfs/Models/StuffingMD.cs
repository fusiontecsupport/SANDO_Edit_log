using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using scfs.Data;
using scfs_erp;

namespace scfs_erp.Models
{
    public class StuffingMD
    {
        public List<StuffingMaster> masterdata { get; set; }
        public List<StuffingDetail> detaildata { get; set; }
        public List<StuffingProductDetail> productdata { get; set; }
        public List<SP_STUFFINGDETAIL_FLX_ASSGN_Result> detail { get; set; }
        public List<PR_STUFFINGDETAIL_FLX_ASSGN_Result> stuffsealdetail { get; set; }
        public List<PR_SHUTOUT_CARGO_FLX_ASSGN_Result> shutoutdetail { get; set; }
    }
}