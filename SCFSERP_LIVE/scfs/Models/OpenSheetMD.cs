using System.Collections.Generic;


namespace scfs_erp.Models
{
    public class OpenSheetMD
    {
        public List<OpenSheetMaster> masterdata { get; set; }

        public List<OpenSheetDetail> detaildata { get; set; }
        public List<BillEntryMaster> bmasterdata { get; set; }
        public List<BillEntryDetail> bdetaildata { get; set; }
        public List<VW_OPENSHEET_BILL_ENTRY_MID_ASSGN> boedetaildata { get; set; }

    }
}