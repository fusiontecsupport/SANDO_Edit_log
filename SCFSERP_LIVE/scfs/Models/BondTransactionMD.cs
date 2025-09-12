using scfs_erp;
using scfs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    public class BondTransactionMD
    {
        public List<BondTransactionMaster> bndmasterdata { get; set; }
        public List<BondTransactionDetail> bnddetaildata { get; set; }
        public List<BondTransactionMasterFactor> bndcostfactor { get; set; }
        public List<pr_Bond_Invoice_BondNO_Grid_Assgn_Result> bondinvcdata { get; set; }
        public List<pr_Ex_Bond_VT_Invoice_BondNO_Grid_Assgn_Result> bondvtinvcdata { get; set; }
        public List<pr_Bond_Manual_Invoice_BondNO_Grid_Assgn_Result> bondmanualinvcdata { get; set; }
     
    }
}