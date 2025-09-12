using scfs_erp.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Helper
{
    public class autono
    {
       
        public static string autonum(String table_name, String table_fld,String compyid)
            {
            
                String temp = "";

                using (var context = new SCFSERPContext())
                {
                    temp = "1";
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where SBMID IN(SELECT SBMID FROM SHIPPINGBILLMASTER WHERE COMPYID = " + HttpContext.Current.Session["compyid"] + ")").ToList();
                    if (autonumber[0] != null)
                        temp = (autonumber[0] + 1).ToString();
                    else
                        return temp;
                }


                return temp;
               
            }
        
        }
    
}