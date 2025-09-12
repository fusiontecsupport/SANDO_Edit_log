using scfs_erp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Helper
{
    public class GateInCheck
    {
        public static int recordCount(string voyno, string igmno, string gplno, string containerno)
        {
            SCFSERPContext context = new SCFSERPContext();
            var d = context.Database.SqlQuery<Int32>("select Count(" + containerno + ") As Rcount from  gateindetail where VOYNO= " + voyno + " and IGMNO= " + igmno + " and GPLNO= " + gplno + " and CONTNRNO=" + containerno).ToList();
            // return d.Count();
            var count = d.Count();
            if (count != 0)
            {
                return 1;
            }
            else
            {
                return 0;

            }
            //for (int i = 0; i < d.Count(); i++)
            //{
            //    var temp1 = (d[i]).ToString();
            //    TmpRCount = Convert.ToInt16(temp1);
            //    return TmpRCount;
            //}
        }
    }
}