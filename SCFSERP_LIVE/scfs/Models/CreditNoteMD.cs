using scfs_erp;
using scfs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace scfs_erp.Models
{
    public class CreditNoteMD
    {
        public List<CreditNote_TransactionMaster> masterdata { get; set; }
        public List<CreditNote_TransactionDetail> detaildata { get; set; }
    }
}