using scfs_erp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using scfs_erp.Models;
using scfs_erp.Controllers;

namespace scfs_erp.Helper
{
    public class auto_numbr_invoice
    {
     
        public static string autonum(String table_name, String table_fld,string REGISID,string COMPYID,string BTYPE)
        {

            String temp = "";

            using (var context = new SCFSERPContext())
            {
                temp = "1";



                var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID=" + REGISID + " and COMPYID=" + COMPYID + " and TRANBTYPE=" + BTYPE).ToList();

                if (autonumber[0] != null)
                    temp = (autonumber[0] + 1).ToString();
                else
                    return temp;
            }
       
            return temp;

        }

        public static string Bondautonum(String table_name, String table_fld, string REGISID, string COMPYID, string BTYPE)
        {

            String temp = "";

            using (var context = new SCFSERPContext())
            {
                temp = "1";



                var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID=" + REGISID + " and COMPYID=" + COMPYID + " and TRANBTYPE=" + BTYPE).ToList();

                if (autonumber[0] != null)
                    temp = (autonumber[0] + 1).ToString();
                else
                    return temp;
            }

            return temp;

        }


        public static string gstautonum(String table_name, String table_fld, string REGISID, string COMPYID, string BTYPE, int TRANREFID)
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

        public static string sezgstautonum(String table_name, String table_fld, string REGISID, string COMPYID, string BTYPE, int TRANREFID)
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
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID IN(70) and COMPYID=" + COMPYID + " and SDPTID = 1 and TRANREFID =" + Convert.ToString(TRANREFID) + " " + btypecond).ToList();

                    if (autonumber[0] != null)
                        temp = (autonumber[0] + 1).ToString();
                    else
                        return temp;
                }
                else
                {
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID  IN(70) and COMPYID=" + COMPYID + " and SDPTID = 1 " + btypecond).ToList();

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

        public static string newgstautonum(String table_name, String table_fld, string REGISID, string COMPYID, string BTYPE, int TRANREFID, int CUSTGID)
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
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID IN(1, 2, 6, 15) and CUSTGID = " + CUSTGID + " AND COMPYID=" + COMPYID + " and SDPTID = 1 and TRANREFID =" + Convert.ToString(TRANREFID) + " " + btypecond).ToList();

                    if (autonumber[0] != null)
                        temp = (autonumber[0] + 1).ToString();
                    else
                        return temp;
                }
                else
                {
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID  IN(1, 2, 6, 15) and CUSTGID = " + CUSTGID + " AND COMPYID=" + COMPYID + " and SDPTID = 1 " + btypecond).ToList();

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

        public static string npnrgstautonum(String table_name, String table_fld, string REGISID, string COMPYID, string BTYPE, int TRANREFID)
        {

            String temp = "";
            
            using (var context = new SCFSERPContext())
            {
                temp = "1";
                var Query = context.Database.SqlQuery<int>("select CATEBILLTYPE from  CategoryMaster where CATEID=" + TRANREFID).ToList();
                int catebilltype = 0;
                if (Query.Count() != 0) { catebilltype = Query[0]; ; }
                else { catebilltype = 0; }

                if (catebilltype == 2)
                {
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID IN(1,2,6,15,51,52,53) and COMPYID=" + COMPYID + " and SDPTID = 9 and TRANREFID =" + Convert.ToString(TRANREFID) + "").ToList();

                    if (autonumber[0] != null)
                        temp = (autonumber[0] + 1).ToString();
                    else
                        return temp;
                }
                else
                {
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID IN(1,2,6,15,51,52,53) and COMPYID=" + COMPYID + " and SDPTID = 9").ToList();

                    if (autonumber[0] != null)
                        temp = (autonumber[0] + 1).ToString();
                    else
                        return temp;
                }


                //var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID IN(1,2,6,15,51,52,53) and COMPYID=" + COMPYID + " and SDPTID = 9").ToList();

                //if (autonumber[0] != null)
                //    temp = (autonumber[0] + 1).ToString();
                //else
                //    return temp;
            }

            return temp;

        }


        public static string bsgstautonum(String table_name, String table_fld, string REGISID, string COMPYID, string BTYPE, int TRANREFID)
        {

            String temp = "";

            using (var context = new SCFSERPContext())
            {
                temp = "1";
                
                var Query = context.Database.SqlQuery<int>("select CATEBILLTYPE from  CategoryMaster where CATEID=" + TRANREFID).ToList();
                int catebilltype = 0;
                if (Query.Count() != 0) { catebilltype = Query[0]; }
                else { catebilltype = 0; }
                String btypecond = "";
                if (BTYPE == "5" || BTYPE == "6")
                {
                    btypecond = " AND TRANBTYPE = " + BTYPE;
                }
                if (catebilltype == 2)
                {
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID IN(31,32,33,37,65) and COMPYID=" + COMPYID + " and SDPTID = 1 and TRANREFID =" + Convert.ToString(TRANREFID) + " " + btypecond).ToList();

                    if (autonumber[0] != null)
                        temp = (autonumber[0] + 1).ToString();
                    else
                        return temp;
                }
                else
                {
                    String qry = "SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID IN(31,32,33,37,65) and COMPYID=" + COMPYID + " and SDPTID = 1 " + btypecond;
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>(qry).ToList();

                    if (autonumber[0] != null)
                        temp = (autonumber[0] + 1).ToString();
                    else
                        return temp;
                }


                //var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID IN(31,32,33,37) and COMPYID=" + COMPYID + " and SDPTID = 1").ToList();

                //if (autonumber[0] != null)
                //    temp = (autonumber[0] + 1).ToString();
                //else
                //    return temp;
            }

            return temp;

        }
        public static string gstexportautonum(String table_name, String table_fld, string REGISID, string COMPYID, string BTYPE, int TRANREFID)
        {

            String temp = "";

            using (var context = new SCFSERPContext())
            {
                temp = "1";
                
                var Query = context.Database.SqlQuery<int>("select CATEBILLTYPE from  CategoryMaster where CATEID=" + TRANREFID).ToList();
                int catebilltype = 0;
                if (Query.Count() != 0) { catebilltype = Query[0];  }
                else { catebilltype = 0; }

                if (catebilltype == 2)
                {
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID IN(16,17,18, 49) and COMPYID=" + COMPYID + " and SDPTID = 2 and TRANREFID =" + Convert.ToString(TRANREFID) + "").ToList();

                    if (autonumber[0] != null)
                        temp = (autonumber[0] + 1).ToString();
                    else
                        return temp;
                }
                else
                {
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID IN(16,17,18, 49) and COMPYID=" + COMPYID + " and SDPTID = 2 ").ToList();

                    if (autonumber[0] != null)
                        temp = (autonumber[0] + 1).ToString();
                    else
                        return temp;
                }

                //var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID IN(16,17,18, 49) and COMPYID=" + COMPYID + " and SDPTID = 2 ").ToList();

                //if (autonumber[0] != null)
                //    temp = (autonumber[0] + 1).ToString();
                //else
                //    return temp;
            }

            return temp;

        }

        public static string gstBondAutonum(String table_name, String table_fld, string REGISID, string COMPYID, string BTYPE, int TRANREFID, int TRANTID, int TAXType)
        {

            String temp = "";

            using (var context = new SCFSERPContext())
            {
                temp = "1";

                var Query = context.Database.SqlQuery<int>("select CATEBILLTYPE from  CategoryMaster where CATEID=" + TRANREFID).ToList();
                int catebilltype = 0;
                if (Query.Count() != 0) { catebilltype = Query[0]; }
                else { catebilltype = 0; }

                if (catebilltype == 2)
                {
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where  COMPYID=" + COMPYID + " and SDPTID = 10 and TRANREFID =" + Convert.ToString(TRANREFID)+" and TaxType = "+ TAXType ).ToList(); //REGSTRID IN(16,17,18, 49) and + " and TRANTID = " + Convert.ToString(TRANTID) + ""

                    if (autonumber[0] != null)
                        temp = (autonumber[0] + 1).ToString();
                    else
                        return temp;
                }
                else
                {
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where  COMPYID=" + COMPYID + " and SDPTID = 10  " ).ToList(); //REGSTRID IN(16,17,18, 49) and and TRANTID = " + Convert.ToString(TRANTID) + ""

                    if (autonumber[0] != null)
                        temp = (autonumber[0] + 1).ToString();
                    else
                        return temp;
                }

                //var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID IN(16,17,18, 49) and COMPYID=" + COMPYID + " and SDPTID = 2 ").ToList();

                //if (autonumber[0] != null)
                //    temp = (autonumber[0] + 1).ToString();
                //else
                //    return temp;
            }

            return temp;

        }

        public static string GetCateBillType(int CateID)
        {
            using (var context = new SCFSERPContext())
            {
                var Query = context.Database.SqlQuery<string>("select isnull(CATECODE,'') from  CategoryMaster where CATEBILLTYPE = 2 and CATEID=" + CateID).ToList();
                string btyp = "";
                if (Query.Count() != 0) { btyp = Query[0].ToString(); }
                return btyp;
            }
        }
        //BS EXPORT

        public static string gstexportautonum_BS(String table_name, String table_fld, string REGISID, string COMPYID, string BTYPE, int TRANREFID)
        {

            String temp = "";

            using (var context = new SCFSERPContext())
            {
                temp = "1";
                 
                var Query = context.Database.SqlQuery<int>("select CATEBILLTYPE from  CategoryMaster where CATEID=" + TRANREFID).ToList();
                int catebilltype = 0;
                if (Query.Count() != 0) { catebilltype = Query[0];  }
                else { catebilltype = 0; }

                if (catebilltype == 2)
                {
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID IN(34, 35, 36, 50) and COMPYID=" + COMPYID + " and SDPTID = 2 and TRANREFID =" + Convert.ToString(TRANREFID) + "").ToList();

                    if (autonumber[0] != null)
                        temp = (autonumber[0] + 1).ToString();
                    else
                        return temp;
                }
                else
                {
                    var autonumber = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where REGSTRID IN(34, 35, 36, 50) and COMPYID=" + COMPYID + " and SDPTID = 2").ToList();

                    if (autonumber[0] != null)
                        temp = (autonumber[0] + 1).ToString();
                    else
                        return temp;
                }
            }

            return temp;

        }       

    }
}