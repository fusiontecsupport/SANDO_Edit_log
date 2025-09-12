using scfs_erp.Context;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Helper
{
    public class CartngOrdr_Del
    {
        SCFSERPContext context = new SCFSERPContext();

        public static string delete_check1(string fname, string pid)
        {
            var TmpPrcdStatus = "PROCEED";
            var TmpRCount = 0;
            using (var context = new SCFSERPContext())
            {
                var s = context.Database.SqlQuery<Soft_Table_Delete_Detail>("select * from SOFT_TABLE_DELETE_DETAIL where OPTNSTR= '" + fname + "' Order by TABDID").ToList();
                var ss = s;
                foreach (var sss in ss)
                {
                    var Tablename = sss.TABNAME;
                    //var m = Tablename;
                    //return m;
                    var fieldname = sss.PFLDNAME;

                    var condstr = sss.DCONDTNSTR + pid;

                    var Dispdesc = sss.DISPDESC;

                    TmpRCount = recordCount(Tablename, fieldname, condstr);

                    if (TmpRCount > 0)
                    {
                        TmpPrcdStatus = Dispdesc; break;
                    }
                    //return TmpPrcdStatus;
                }
                return TmpPrcdStatus;
            }
        }



        public static int recordCount(string Tablename, string fieldname, string condstr)
        {
            SCFSERPContext context = new SCFSERPContext();
            // int TmpRCount = 0;


            if (condstr.Trim().Length > 0)
            {

                
                var d = context.Database.SqlQuery<Int32>("SELECT COUNT(" + fieldname + ") FROM STUFFINGPRODUCTDETAIL INNER JOIN STUFFINGDETAIL ON STUFFINGPRODUCTDETAIL.STFDID=STUFFINGDETAIL.STFDID INNER JOIN STUFFINGMASTER ON STUFFINGDETAIL.STFMID=STUFFINGMASTER.STFMID LEFT OUTER JOIN SHIPPINGBILLDETAIL ON SHIPPINGBILLDETAIL.SBDID=STUFFINGPRODUCTDETAIL.SBDID WHERE " + condstr + " Group by " + fieldname).ToList();
                
             
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

            return 2;

        }
    }



}
