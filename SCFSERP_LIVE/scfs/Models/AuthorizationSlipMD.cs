using System.Collections.Generic;
using scfs.Data;

namespace scfs_erp.Models
{
    public class AuthorizationSlipMD
    {
        public List<AuthorizationSlipMaster> masterdata { get; set; }

        public List<AuthorizationSlipDetail> detaildata { get; set; }
        public List<VW_EXPORT_ASL_DETAIL_CTRL_ASSGN_MODS> viewdetail { get; set; }
        //public List<VW_STUFF_GATEIN_CTRL_ASSGN> det { get; set; }
       public List<PR_IMPORT_AUTHORIZATION_DETAIL_CTRL_ASSGN_Result> destuffdata { get; set; }

        public List<PR_NONPNR_AUTHORIZATION_DETAIL_CTRL_ASSGN_Result> nonpnrdestuffdata { get; set; }
    }

    
}