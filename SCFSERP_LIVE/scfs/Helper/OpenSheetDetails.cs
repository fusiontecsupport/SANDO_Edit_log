using scfs_erp.Context;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Helper
{
    public class OpenSheetDetails
    {
        public static string Detail(string id)
        {
            string tmp = "Date Not Found";


            using (var context = new SCFSERPContext())
            {
                var Date = context.Database.SqlQuery<string>("SELECT CONVERT(VARCHAR(19), SEALMDATE, 120) from import_seal_master where SEALMID='" + Convert.ToInt32(id) + "'").ToList();

                foreach (var dd in Date)
                {
                    var DEL_DATE = dd;
                    return DEL_DATE.ToString();
                }
               
          
             }

            return tmp;
        }
        
    }
}