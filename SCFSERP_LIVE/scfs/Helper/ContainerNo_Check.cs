using scfs_erp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using scfs_erp.Models;
using scfs.Data;

namespace scfs_erp.Helper
{
    public class ContainerNo_Check
    {
        SCFSERPContext context = new SCFSERPContext();
        public static string recordCount(string Voyno, string Igmno, string Gplno, string Contnrno)
        {

            SCFSERPContext context = new SCFSERPContext();

            int count = 0;
            string squery = "SELECT *From GATEINDETAIL where SDPTID=1 AND VOYNO = '" + Voyno + "' AND IGMNO = '" + Igmno + "' AND GPLNO = '" + Gplno + "' AND CONTNRNO = '" + Contnrno + "'";

            var s = context.Database.SqlQuery<GateInDetail>(squery).ToList();
            count = s.Count;

            if (count == 0)
            {
                return "PROCEED";
            }
            else
                return "Container number already exists";

        }

        public static string nonpnrrecordCount(string Voyno, string Igmno, string Gplno, string Contnrno)
        {

            SCFSERPContext context = new SCFSERPContext();

            int count = 0;

            string squery = "SELECT *From GATEINDETAIL where SDPTID=9 AND VOYNO = '" + Voyno + "' AND IGMNO = '" + Igmno + "' AND GPLNO = '" + Gplno + "' AND CONTNRNO = '" + Contnrno + "'";

            var s = context.Database.SqlQuery<GateInDetail>(squery).ToList();

            //var s = context.Database.SqlQuery<Int32>("SELECT COUNT(CONTNRNO) from GATEINDETAIL where SDPTID=9 AND VOYNO = '" + Voyno + "' AND IGMNO = '" + Igmno + "' AND GPLNO = '" + Gplno + "' AND CONTNRNO = '" + Contnrno + "' ").ToList();
            count = s.Count;

            if (count == 0)
            {
                return "PROCEED";
            }
            else
                return "Container number already exists";

        }

        public static string esrecordCount(string Contnrno)
        {

            SCFSERPContext context = new SCFSERPContext();

            int count = 0;
            //string squery = "SELECT *From GATEINDETAIL where SDPTID = 11 AND CONTNRNO = '" + Contnrno + "'";

            //var s = context.Database.SqlQuery<GateInDetail>(squery).ToList();
            var s = context.Database.SqlQuery<PR_ES_GATEIN_CONTAINER_CHK_ASSGN_001_Result>("EXEC PR_ES_GATEIN_CONTAINER_CHK_ASSGN_001 @PCONTNRNO = '" + Contnrno + "'").ToList();
            count = s.Count;

            if (count == 0)
            {
                return "PROCEED";
            }
            else
                return "Container number already exists";

        }
    }

    public class ExBondNo_Check
    {
        SCFSERPContext context = new SCFSERPContext();
        public static string recordCount(string BNDDNO)
        {

            SCFSERPContext context = new SCFSERPContext();

            int count = 0;
            string squery = "SELECT *From ExBondMaster (nolock) where SDPTID=10 AND BNDDNO = '" + BNDDNO + "'";

            var s = context.Database.SqlQuery<Ex_BondMaster>(squery).ToList();
            count = s.Count;

            if (count == 0)
            {
                return "PROCEED";
            }
            else
                return "Ex Bond No number already exists";

        }


    }
    public class BondNo_Check
    {
        SCFSERPContext context = new SCFSERPContext();
        public static string recordCount(string BNDDNO)
        {

            SCFSERPContext context = new SCFSERPContext();

            int count = 0;
            string squery = "SELECT *From BondMaster (nolock) where SDPTID=10 AND BNDDNO = '" + BNDDNO + "'";

            var s = context.Database.SqlQuery<BondMaster>(squery).ToList();
            count = s.Count;

            if (count == 0)
            {
                return "PROCEED";
            }
            else
                return "Bond No number already exists";

        }

        
    }
}