using scfs_erp.Context;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace scfs_erp.Helper
{
    public class Delete_check
    {
  
      SCFSERPContext context=new SCFSERPContext();
                
        public static string delete_check1(string fname, string pid)
        {
            var TmpPrcdStatus = "PROCEED";
            var TmpRCount = 0;
            using (var context = new SCFSERPContext())
            {
                var s = context.Database.SqlQuery<Soft_Table_Delete_Detail>("select * from SOFT_TABLE_DELETE_DETAIL where OPTNSTR= " + fname + " Order by TABDID");
                var ss = s;
                foreach (var sss in ss)
                {
                    var Tablename = sss.TABNAME;
                    //var m = Tablename;
                    //return m;
                    var fieldname = sss.PFLDNAME;
                    var condstr = sss.DCONDTNSTR + pid;
                    //var m = condstr;
                    //return m;
                    var Dispdesc = sss.DISPDESC;

                TmpRCount = recordCount(Tablename, fieldname, condstr);
                  
                    if (TmpRCount > 0)
                    {
                        TmpPrcdStatus = Dispdesc; break;
                    }
                    return TmpPrcdStatus;
                }
                return TmpPrcdStatus;
            }
        }
       
       public static int recordCount(string Tablename, string fieldname, string condstr)
        {
            SCFSERPContext context = new SCFSERPContext();
            int TmpRCount = 0;
            // var dcount=context.Database.SqlQuery<Int32>("select Count(" + fname + ") As Rcount from " + tname).ToList();

            if (condstr.Trim().Length > 0)
            {
                //return cstring;
                var d = context.Database.SqlQuery<Int32>("select Count(" + fieldname + ") As Rcount from " + Tablename + " where " + condstr + " Group by " + fieldname).ToList();
                var temp1 = (d[0]).ToString();
                

                // var aaa = context.Database.SqlQuery<Int32>(d + "Group by" + fname).ToList();
                //   var temp1 = (d[0]).ToString();
                //   return temp1;
                // var cquery = aaa;

                TmpRCount = Convert.ToInt16(temp1);
                return TmpRCount;
            }

            return 0;
     
        }
              }
              
		

                }
