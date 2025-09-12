using scfs.Context;
using scfs_erp.Context;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Helper
{
    public class TMP_InsertPrint
    {
        public static string InsertToTMP(String table_name, String table_fld1, int table_fld2,string cusrid)
        {

            String temp = "";
            using (var context = new SCFSERPContext())
            {
               

                //........addng value to TMPRPT....//
                TMPRPT_IDS RPTIDS = new TMPRPT_IDS();
                //RPTIDS.KUSRID = Convert.ToInt32(cusrid);
                RPTIDS.KUSRID = Convert.ToString(cusrid);
                RPTIDS.OPTNSTR = table_fld1;
                RPTIDS.RPTID = Convert.ToInt32(table_fld2);

                context.TMPRPT_IDS.Add(RPTIDS);
                context.SaveChanges();//...End

                temp = "Successfully Added";
            }

            return temp;
        }

        public static string NewInsertToTMP(String table_name, String table_fld1, int table_fld2, string cusrid)
        {

            String temp = "";
            using (var context = new SCFSERPContext())
            {


                //........addng value to TMPRPT....//
                NEW_TMPRPT_IDS NEW_RPTIDS = new NEW_TMPRPT_IDS();
                NEW_RPTIDS.KUSRID = cusrid;
                NEW_RPTIDS.OPTNSTR = table_fld1;
                NEW_RPTIDS.RPTID = Convert.ToInt32(table_fld2);

                context.NEW_TMPRPT_IDS.Add(NEW_RPTIDS);
                context.SaveChanges();//...End

                temp = "Successfully Added";
            }

            return temp;
        }

        public static string Bond_NewInsertToTMP(String table_name, String table_fld1, int table_fld2, string cusrid)
        {

            String temp = "";
            using (var context = new SCFS_BondContext())
            {


                //........addng value to TMPRPT....//
                Bond_NEW_TMPRPT_IDS BOND_NEW_RPTIDS = new Bond_NEW_TMPRPT_IDS();
                BOND_NEW_RPTIDS.KUSRID = cusrid;
                BOND_NEW_RPTIDS.OPTNSTR = table_fld1;
                BOND_NEW_RPTIDS.RPTID = Convert.ToInt32(table_fld2);

                context.BOND_NEW_TMPRPT_IDS.Add(BOND_NEW_RPTIDS);
                context.SaveChanges();//...End

                temp = "Successfully Added";
            }

            return temp;
        }

        public static string In_VT_Bond_NewInsertToTMP(String table_name, String table_fld1, int table_fld2, string cusrid)
        {

            String temp = "";
            using (var context = new BondContext())
            {


                //........addng value to TMPRPT....//
                Bond_NEW_TMPRPT_IDS BOND_NEW_RPTIDS = new Bond_NEW_TMPRPT_IDS();
                BOND_NEW_RPTIDS.KUSRID = cusrid;
                BOND_NEW_RPTIDS.OPTNSTR = table_fld1;
                BOND_NEW_RPTIDS.RPTID = Convert.ToInt32(table_fld2);

                context.BOND_NEW_TMPRPT_IDS.Add(BOND_NEW_RPTIDS);
                context.SaveChanges();//...End

                temp = "Successfully Added";
            }

            return temp;
        }
    }
}