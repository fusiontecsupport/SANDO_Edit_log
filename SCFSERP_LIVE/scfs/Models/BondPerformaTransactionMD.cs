using scfs_erp;
using scfs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    public class BondPerformaTransactionMD
    {
        public List<BondPerformaTransactionMaster> bndmasterdata { get; set; }
        public List<BondPerformaTransactionDetail> bnddetaildata { get; set; }
        public List<BondPerformaTransactionMasterFactor> bndcostfactor { get; set; }
        public List<pr_BondPerforma_Invoice_BondNO_Grid_Assgn_Result> bondinvcdata { get; set; }
        
        public List<pr_Ex_Bond_VT_Proforma_Invoice_BondNO_Grid_Assgn_Result> bondvtinvcdata { get; set; }
        

    }
}