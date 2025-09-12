using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using scfs_erp.Models;

namespace scfs_erp.Models
{
    public class BOEDetails
    {
        public List<BillEntryMaster> masterdata { get; set; }
        public List<BillEntryDetail> detaildata { get; set; }
    }
}