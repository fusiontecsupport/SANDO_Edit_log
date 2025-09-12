using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    public class ManualOpenSheetMD
    {
        public List<ManualOpenSheetMaster> manualmasterdata { get; set; }

        public List<ManualOpenSheetDetail> manualdetaildata { get; set; }
    }
}