using scfs_erp;
using scfs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs.Models
{
    public class DebitNoteMD
    {
        public List<DebitNote_TransactionMaster> masterdata { get; set; }
        public List<DebitNote_TransactionDetail> detaildata { get; set; }
    }
}