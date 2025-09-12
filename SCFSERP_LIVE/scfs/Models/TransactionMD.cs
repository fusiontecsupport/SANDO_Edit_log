using scfs_erp;
using scfs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    public class TransactionMD
    {
        public List<TransactionMaster> masterdata { get; set; }
        public List<TransactionDetail> detaildata { get; set; }
        public List<TransactionMasterFactor> costfactor { get; set; }
        public List<pr_NonPnr_Invoice_IGMNO_Grid_Assgn_Result> nonpnrinvcedata { get; set; }
        public List<pr_Import_Invoice_IGMNO_Grid_Assgn_Result> impinvcedata { get; set; }
        public List<pr_ESeal_Invoice_Grid_Assgn_Result> eslinvcedata { get; set; }
        
        public List<pr_Fetch_Orginal_Shipping_Bill_Detail_Result> orgsbdata { get; set; }
        
        public List<pr_Export_Invoice_Stuff_Flx_Assgn_Result> viewdata { get; set; }
        //public List<pr_Export_Shutout_Detail_Ctrl_Assgn_Result> shutoutviewdata { get; set; }


        //public List<pr_Import_Invoice_IGMNO_Grid_Assgn_A001_Result> impinvcedata_A01 { get; set; }

        //public List<pr_Import_Supp_Invoice_Grid_Mod_Assgn_Result> suppinvdata { get; set; }

        //public List<pr_LCL_Invoice_Mod_Detail_Result> lcldetaildata { get; set; }
        public List<pr_Export_Manual_Invoice_Ctrl_Flx_Assgn_Result> manualviewdata { get; set; }
        //public List<pr_Import_Manual_Invoice_Grid_Assgn_Result> impmanualviewdata { get; set; }
        public List<pr_Import_Manual_Invoice_Ctrl_Flx_Assgn_Result> impmanualviewdata { get; set; }

        public List<pr_Get_VT_Details_For_QRCode_Result> VTDetails_4QRC { get; set; }
    }
}