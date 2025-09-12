using scfs_erp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Helper
{
    public class Autonumber
    {
        public static string performaautonum(String table_name, String table_fld, string REGISID, string COMPYID, string BTYPE, int TRANREFID)
        {

            String temp = "";

            using (var context = new SCFSERPContext())
            {
                temp = "1";
                int catebilltype = 0;

                var Query = context.Database.SqlQuery<int>("select CATEBILLTYPE from  CategoryMaster where CATEID=" + TRANREFID).ToList();

                if (Query.Count() != 0) { catebilltype = Query[0]; }
                else { catebilltype = 0; }
                String btypecond = "";
                if (BTYPE == "5" || BTYPE == "6" || BTYPE == "7")
                {
                    btypecond = " AND TRANBTYPE = " + BTYPE;
                }
                if (catebilltype == 2)
                {
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID IN(1, 2, 6, 15) and COMPYID=" + COMPYID + " and SDPTID = 1 and TRANREFID =" + Convert.ToString(TRANREFID) + " " + btypecond).ToList();

                    if (autonumber[0] != null)
                        temp = (autonumber[0] + 1).ToString();
                    else
                        return temp;
                }
                else
                {
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID  IN(1, 2, 6, 15) and COMPYID=" + COMPYID + " and SDPTID = 1 " + btypecond).ToList();

                    if (autonumber[0] != null)
                        temp = (autonumber[0] + 1).ToString();
                    else
                        return temp;
                }


                //var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID  IN(1, 2, 6, 15) and COMPYID=" + COMPYID + " and SDPTID = 1").ToList();

                //if (autonumber[0] != null)
                //    temp = (autonumber[0] + 1).ToString();
                //else
                //    return temp;
            }

            return temp;

        }

        public static string autonum(String table_name, String table_fld, String record_condn)
        {

            String temp = "";

            using (var context = new SCFSERPContext())
            {
                temp = "1";

                var s = "SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where " + record_condn + "";

                var autonum = context.Database.SqlQuery<Nullable<Int32>>("SELECT   isnull(MAX(" + table_fld + "),0) as " + table_fld + " from " + table_name + " where " + record_condn).ToList();
                if (autonum[0] != null)
                {

                    //  var autonumber = context.Database.SqlQuery<Int32>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where " + record_condn).ToList();
                    temp = (autonum[0] + 1).ToString();
                }
                else
                {
                    return temp;
                }


            }


            return temp;
        }

        public static string cargoautonum(String table_name, String table_fld, string SDPTID, string COMPYID)
        {

            String temp = "";

            using (var context = new SCFSERPContext())
            {
                temp = "1";

                var autonum = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where SDPTID=" + SDPTID + " and COMPYID=" + COMPYID + "").ToList();

                //var autonum = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ")as " + table_fld + " from " + table_name + " where " + record_condn).ToList();
                if (autonum[0] != null)
                {

                    //  var autonumber = context.Database.SqlQuery<Int32>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where " + record_condn).ToList();
                    temp = (autonum[0] + 1).ToString();
                }
                else
                {
                    return temp;
                }
            }


            return temp;
        }


        public static string manualcreditnote(String table_name, String table_fld, string SDPTID, string COMPYID, string BTYPE)
        {

            String temp = "";

            //using (var context = new SCFSERPContext())
            //{
            //    temp = "1";



            //    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where SDPTID=" + SDPTID + " and COMPYID=" + COMPYID + " and TRANBTYPE=" + BTYPE).ToList();

            //    if (autonumber[0] != null)
            //        temp = (autonumber[0] + 1).ToString();
            //    else
            //        return temp;
            //}

            using (var context = new SCFSERPContext())
            {
                temp = "1";



                var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where  COMPYID=" + COMPYID).ToList();

                if (autonumber[0] != null)
                    temp = (autonumber[0] + 1).ToString();
                else
                    return temp;
            }

            return temp;

        }
    }
}