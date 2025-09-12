using scfs_erp.Context;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Helper
{
    public class BOECheck
    {
        SCFSERPContext context = new SCFSERPContext();

        public static string check1(string fname, string boeno,string igmno,string lno)
        {
            var TmpPrcdStatus = "PROCEED";
            var TmpRCount = 0; var igmcount = 0;
            using (var context = new SCFSERPContext())
            {
                var s = context.Database.SqlQuery<Soft_Table_Delete_Detail>("select * from SOFT_TABLE_DELETE_DETAIL where OPTNSTR= '" + fname + "' Order by TABDID");
                var ss = s;
                foreach (var sss in ss)
                {
                    var Tablename = sss.TABNAME;
                    //var m = Tablename;
                    //return m;
                    var fieldname = sss.PFLDNAME;
                    var condstr = sss.DCONDTNSTR + boeno;
                    //var m = condstr;
                    //return m;
                    var Dispdesc = sss.DISPDESC;

                    TmpRCount = recordCount(Tablename, fieldname, condstr);

                    if (TmpRCount > 0)
                    {
                        igmcount = igmrecordCount(Tablename, fieldname, igmno, lno,boeno);
                        if(igmcount==0)
                          TmpPrcdStatus = Dispdesc;
                        else if (igmcount > 0)
                        TmpPrcdStatus="PROCEED";
                    }
                   // return TmpPrcdStatus;
                }
                return TmpPrcdStatus;
            }
        }

        public static int recordCount(string Tablename, string fieldname, string condstr)
        {
            SCFSERPContext context = new SCFSERPContext();
            int TmpRCount = 0;
            if (condstr.Trim().Length > 0)
            {var temp1="0";
                var d = context.Database.SqlQuery<Int32>("select Count(" + fieldname + ") As Rcount from " + Tablename + " where " + condstr + " Group by " + fieldname).ToList();
                if(d.Count>0)
                 temp1 = (d[0]).ToString();

                TmpRCount = Convert.ToInt16(temp1);
                return TmpRCount;
            }

            return 0;

        }
        public static int igmrecordCount(string Tablename, string fieldname, string tigmno,string tlineno,string tboe)
        {
            SCFSERPContext context = new SCFSERPContext();
            int TmpRCount = 0;
            var temp1 = "0";
                var d = context.Database.SqlQuery<Int32>("select Count(" + fieldname + ") As Rcount from " + Tablename + " where OSMIGMNO='"+tigmno+"' and OSMLNO='"+tlineno+"' and boeno='"+tboe+"' Group by " + fieldname).ToList();
                if (d.Count > 0)
                    temp1 = (d[0]).ToString();

                TmpRCount = Convert.ToInt16(temp1);
                return TmpRCount;
         

        }
    }



}
