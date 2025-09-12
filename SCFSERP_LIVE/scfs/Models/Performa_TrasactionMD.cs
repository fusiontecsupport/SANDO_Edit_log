using scfs_erp;
using scfs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    public class Performa_TransactionMD
    {
        public List<Performa_TransactionMaster> perfmasterdata { get; set; }
        public List<Performa_TransactionDetail> perfdetaildata { get; set; }
        public List<Performa_TransactionMasterFactor> perfcostfactor { get; set; }
        public List<pr_Import_Performa_Invoice_IGMNO_Grid_Assgn_Result> perfimpinvcedata { get; set; }
       
        //public List<pr_Import_Invoice_IGMNO_Grid_Assgn_A001_Result> impinvcedata_A01 { get; set; }

        //public List<pr_Import_Supp_Invoice_Grid_Mod_Assgn_Result> suppinvdata { get; set; }

        //public List<pr_LCL_Invoice_Mod_Detail_Result> lcldetaildata { get; set; }
       
        //public List<pr_Import_Manual_Invoice_Grid_Assgn_Result> impmanualviewdata { get; set; }
    }
}