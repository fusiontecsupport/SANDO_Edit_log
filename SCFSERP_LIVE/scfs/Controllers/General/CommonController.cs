using scfs.Data;
using scfs_erp.Context;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers
{
    public class CommonController : Controller
    {
        //
        // GET: /Common/
        SCFSERPContext context = new SCFSERPContext();



        public JsonResult ValidateACHEADGDESC(String ACHEADGDESC, String i_ACHEADGDESC)
        {

            if (ACHEADGDESC.Equals(i_ACHEADGDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select ACHEADGDESC from accountgroupmaster").ToList();
            if (d.Contains(ACHEADGDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateACHEADGCODE(String ACHEADGCODE, String i_ACHEADGCODE)
        {

            if (ACHEADGCODE.Equals(i_ACHEADGCODE))
                return Json(true, JsonRequestBehavior.AllowGet);


            List<String> d = context.Database.SqlQuery<String>("select ACHEADGCODE from accountgroupmaster").ToList();
            if (d.Contains(ACHEADGCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------


        public JsonResult ValidateGWNDESC(String GWNDESC, String i_GWNDESC)
        {

            if (GWNDESC.Equals(i_GWNDESC))
                return Json(true, JsonRequestBehavior.AllowGet);


            List<String> d = context.Database.SqlQuery<String>("select GWNDESC from bondgodownmaster (nolock)").ToList();
            if (d.Contains(GWNDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ValidateGWNCODE(String GWNCODE, String i_GWNCODE)
        {

            if (GWNCODE.Equals(i_GWNCODE))
                return Json(true, JsonRequestBehavior.AllowGet);


            List<String> d = context.Database.SqlQuery<String>("select GWNCODE from bondgodownmaster (nolock)").ToList();
            if (d.Contains(GWNCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        /*...............ENDORSEMENT CHARGE TYPE*/
        public JsonResult ValidateECTDESC(String ECTDESC, String i_ECTDESC)
        {

            if (ECTDESC.Equals(i_ECTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select ECTDESC from ENDORSEMENT_CHARGE_TYPE_MASTER").ToList();
            if (d.Contains(ECTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateECTCODE(String ECTCODE, String i_ECTCODE)
        {

            if (ECTCODE.Equals(i_ECTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);


            List<String> d = context.Database.SqlQuery<String>("select ECTCODE from ENDORSEMENT_CHARGE_TYPE_MASTER").ToList();
            if (d.Contains(ECTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        /**/

        /*...............ENDORSEMENT CHARGE */
        public JsonResult ValidateECMDESC(String ECMDESC, String i_ECMDESC)
        {
            if (ECMDESC.Equals(i_ECMDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select ECMDESC from ENDORSEMENT_CHARGE_MASTER").ToList();
            if (d.Contains(ECMDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateECMCODE(String ECMCODE, String i_ECMCODE)
        {

            if (ECMCODE.Equals(i_ECMCODE))
                return Json(true, JsonRequestBehavior.AllowGet);


            List<String> d = context.Database.SqlQuery<String>("select ECMCODE from ENDORSEMENT_CHARGE_MASTER").ToList();
            if (d.Contains(ECMCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        /**/
        /*...............ENDORSEMENT OPERATIONS */
        public JsonResult ValidateEOPTDESC(String EOPTDESC, String i_EOPTDESC)
        {
            if (EOPTDESC.Equals(i_EOPTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select EOPTDESC from EXPORT_OPERATIONTYPEMASTER").ToList();
            if (d.Contains(EOPTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateEOPTCODE(String EOPTCODE, String i_EOPTCODE)
        {

            if (EOPTCODE.Equals(i_EOPTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select EOPTCODE from EXPORT_OPERATIONTYPEMASTER").ToList();
            if (d.Contains(EOPTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        /**/
        //-----
        public JsonResult ValidateTGDESC(String TGDESC, String i_TGDESC)
        {

            if (TGDESC.Equals(i_TGDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select TGDESC from ExportTariffGroupMaster").ToList();
            if (d.Contains(TGDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateTGCODE(String TGCODE, String i_TGCODE)
        {

            if (TGCODE.Equals(i_TGCODE))
                return Json(true, JsonRequestBehavior.AllowGet);


            List<String> d = context.Database.SqlQuery<String>("select TGCODE from ExportTariffGroupMaster").ToList();
            if (d.Contains(TGCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        /*import tasriff group*/
        public JsonResult ValidateITGDESC(String TGDESC, String i_TGDESC)
        {

            if (TGDESC.Equals(i_TGDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select TGDESC from TariffGroupMaster").ToList();
            if (d.Contains(TGDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateITGCODE(String TGCODE, String i_TGCODE)
        {

            if (TGCODE.Equals(i_TGCODE))
                return Json(true, JsonRequestBehavior.AllowGet);


            List<String> d = context.Database.SqlQuery<String>("select TGCODE from TariffGroupMaster").ToList();
            if (d.Contains(TGCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateCNTDESC(String CNTDESC, String i_CNTDESC)
        {
            if (CNTDESC.Equals(i_CNTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select CNTDESC from CREDITNOTE_TYPEMASTER").ToList();
            if (d.Contains(CNTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateACHEADDESC(String ACHEADDESC, String i_ACHEADDESC)
        {
            if (ACHEADDESC.Equals(i_ACHEADDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select ACHEADDESC from accountheadmaster").ToList();
            if (d.Contains(ACHEADDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateACHEADCODE(String ACHEADCODE, String i_ACHEADCODE)
        {
            if (ACHEADCODE.Equals(i_ACHEADCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select ACHEADCODE from accountheadmaster").ToList();
            if (d.Contains(ACHEADCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        public JsonResult ValidateBANKMDESC(String BANKMDESC, String i_BANKMDESC)
        {
            if (BANKMDESC.Equals(i_BANKMDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select BANKMDESC from bankmaster").ToList();
            if (d.Contains(BANKMDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateBANKMCODE(String BANKMCODE, String i_BANKMCODE)
        {
            if (BANKMCODE.Equals(i_BANKMCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select BANKMCODE from bankmaster").ToList();
            if (d.Contains(BANKMCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        public JsonResult ValidateBOOKDESC(String BOOKDESC, String i_BOOKDESC)
        {
            if (BOOKDESC.Equals(i_BOOKDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select BOOKDESC from bookedmaster").ToList();
            if (d.Contains(BOOKDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateBOOKCODE(String BOOKCODE, String i_BOOKCODE)
        {
            if (BOOKCODE.Equals(i_BOOKCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select BOOKCODE from bookedmaster").ToList();
            if (d.Contains(BOOKCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        public JsonResult ValidateCATENAME(String CATENAME, String i_CATENAME)
        {
            if (CATENAME.Equals(i_CATENAME))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CATENAME from categorymaster").ToList();
            if (d.Contains(CATENAME))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateCATECODE(String CATECODE, String i_CATECODE)
        {
            if (CATECODE.Equals(i_CATECODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CATECODE from categorymaster").ToList();
            if (d.Contains(CATECODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        public JsonResult ValidateCATETDESC(String CATETDESC, String i_CATETDESC)
        {
            if (CATETDESC.Equals(i_CATETDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CATETDESC from categorytypemaster").ToList();
            if (d.Contains(CATETDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateCATETCODE(String CATETCODE, String i_CATETCODE)
        {
            if (CATETCODE.Equals(i_CATETCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CATETCODE from categorytypemaster").ToList();
            if (d.Contains(CATETCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        //-------------End----------------
        public JsonResult ValidateCHRGDESC(String CHRGDESC, String i_CHRGDESC)
        {
            if (CHRGDESC.Equals(i_CHRGDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CHRGDESC from chargemaster").ToList();
            if (d.Contains(CHRGDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateCHRGCODE(String CHRGCODE, String i_CHRGCODE)
        {
            if (CHRGCODE.Equals(i_CHRGCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CHRGCODE from chargemaster").ToList();
            if (d.Contains(CHRGCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        public JsonResult ValidateCONDTNDESC(String CONDTNDESC, String i_CONDTNDESC)
        {
            if (CONDTNDESC.Equals(i_CONDTNDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CONDTNDESC from conditionmaster").ToList();
            if (d.Contains(CONDTNDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateCONDTNCODE(String CONDTNCODE, String i_CONDTNCODE)
        {
            if (CONDTNCODE.Equals(i_CONDTNCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CONDTNCODE from conditionmaster").ToList();
            if (d.Contains(CONDTNCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        public JsonResult ValidateCONTNRSDESC(String CONTNRSDESC, String i_CONTNRSDESC)
        {
            if (CONTNRSDESC.Equals(i_CONTNRSDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CONTNRSDESC from containersizemaster").ToList();
            if (d.Contains(CONTNRSDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateCONTNRSCODE(String CONTNRSCODE, String i_CONTNRSCODE)
        {
            if (CONTNRSCODE.Equals(i_CONTNRSCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CONTNRSCODE from containersizemaster").ToList();
            if (d.Contains(CONTNRSCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        public JsonResult ValidateCNTNRSDESC(String CNTNRSDESC, String i_CNTNRSDESC)
        {
            if (CNTNRSDESC.Equals(i_CNTNRSDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CNTNRSDESC from containerstatusmaster").ToList();
            if (d.Contains(CNTNRSDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateCNTNRSCODE(String CNTNRSCODE, String i_CNTNRSCODE)
        {
            if (CNTNRSCODE.Equals(i_CNTNRSCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CNTNRSCODE from containerstatusmaster").ToList();
            if (d.Contains(CNTNRSCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        public JsonResult ValidateCONTNRFDESC(String CONTNRFDESC, String i_CONTNRFDESC)
        {
            if (CONTNRFDESC.Equals(i_CONTNRFDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CONTNRFDESC from containerthrumaster").ToList();
            if (d.Contains(CONTNRFDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateCONTNRFCODE(String CONTNRFCODE, String i_CONTNRFCODE)
        {
            if (CONTNRFCODE.Equals(i_CONTNRFCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CONTNRFCODE from containerthrumaster").ToList();
            if (d.Contains(CONTNRFCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        public JsonResult ValidateCONTNRTDESC(String CONTNRTDESC, String i_CONTNRTDESC)
        {
            if (CONTNRTDESC.Equals(i_CONTNRTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CONTNRTDESC from containertypemaster").ToList();
            if (d.Contains(CONTNRTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateCONTNRTCODE(String CONTNRTCODE, String i_CONTNRTCODE)
        {
            if (CONTNRTCODE.Equals(i_CONTNRTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CONTNRTCODE from containertypemaster").ToList();
            if (d.Contains(CONTNRTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        //----------------------Department----------------------------
        public JsonResult ValidateDEPTDESC(String DEPTDESC, String i_DEPTDESC)
        {
            if (DEPTDESC.Equals(i_DEPTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select DEPTDESC from departmentmaster").ToList();
            if (d.Contains(DEPTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateDEPTCODE(String DEPTCODE, String i_DEPTCODE)
        {
            if (DEPTCODE.Equals(i_DEPTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select DEPTCODE from departmentmaster").ToList();
            if (d.Contains(DEPTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //----------------------Designation----------------------------
        public JsonResult ValidateDSGNDESC(String DSGNDESC, String i_DSGNDESC)
        {
            if (DSGNDESC.Equals(i_DSGNDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select DSGNDESC from designationmaster").ToList();
            if (d.Contains(DSGNDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateDSGNCODE(String DSGNCODE, String i_DSGNCODE)
        {
            if (DSGNCODE.Equals(i_DSGNCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select DSGNCODE from designationmaster").ToList();
            if (d.Contains(DSGNCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        //-------------End----------------


        //----------------------DesignationType----------------------------
        public JsonResult ValidateDSGNTDESC(String DSGNTDESC, String i_DSGNTDESC)
        {
            if (DSGNTDESC.Equals(i_DSGNTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select DSGNTDESC from designationtypemaster").ToList();
            if (d.Contains(DSGNTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateDSGNTCODE(String DSGNTCODE, String i_DSGNTCODE)
        {
            if (DSGNTCODE.Equals(i_DSGNTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select DSGNTCODE from designationtypemaster").ToList();
            if (d.Contains(DSGNTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------


        //----------------------Display Order----------------------------
        public JsonResult ValidateDORDRDESC(String DORDRDESC, String i_DORDRDESC)
        {
            if (DORDRDESC.Equals(i_DORDRDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select DORDRDESC from displayordermaster").ToList();
            if (d.Contains(DORDRDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }


        //-------------End----------------

        //-------------------------------------Company---------
        public JsonResult ValidateCOMPNAME(String COMPNAME, String i_COMPNAME)
        {
            if (COMPNAME.Equals(i_COMPNAME))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select COMPNAME from companymaster").ToList();
            if (d.Contains(COMPNAME))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateCOMPCODE(String COMPCODE, String i_COMPCODE)
        {
            if (COMPCODE.Equals(i_COMPCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select COMPCODE from companymaster").ToList();
            if (d.Contains(COMPCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------


        //----------------------Cost Factor----------------------------
        public JsonResult ValidateCFDESC(String CFDESC, String i_CFDESC)
        {
            if (CFDESC.Equals(i_CFDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select CFDESC from costfactormaster").ToList();
            if (d.Contains(CFDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }


        //-------------End----------------


        //----------------------EmptyComponent----------------------------
        public JsonResult ValidateCOMPTDESC(String COMPTDESC, String i_COMPTDESC)
        {
            if (COMPTDESC.Equals(i_COMPTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select COMPTDESC from empty_componentcodemaster").ToList();
            if (d.Contains(COMPTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateCOMPTCODE(String COMPTCODE, String i_COMPTCODE)
        {
            if (COMPTCODE.Equals(i_COMPTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select COMPTCODE from empty_componentcodemaster").ToList();
            if (d.Contains(COMPTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //----------------------EmptyDamage----------------------------
        public JsonResult ValidateDMGDESC(String DMGDESC, String i_DMGDESC)
        {
            if (DMGDESC.Equals(i_DMGDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select DMGDESC from empty_damagecodemaster").ToList();
            if (d.Contains(DMGDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateDMGCODE(String DMGCODE, String i_DMGCODE)
        {
            if (DMGCODE.Equals(i_DMGCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select DMGCODE from empty_damagecodemaster").ToList();
            if (d.Contains(DMGCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------


        //----------------------EmptyLocation----------------------------
        public JsonResult ValidateLOCTDESC1(String LOCTDESC, String i_LOCTDESC)
        {
            if (LOCTDESC.Equals(i_LOCTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select LOCTDESC from empty_locationcodemaster").ToList();
            if (d.Contains(LOCTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateLOCTCODE1(String LOCTCODE, String i_LOCTCODE)
        {
            if (LOCTCODE.Equals(i_LOCTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select LOCTCODE from empty_locationcodemaster").ToList();
            if (d.Contains(LOCTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //----------------------EmptyRepair----------------------------
        public JsonResult ValidateRPRDESC(String RPRDESC, String i_RPRDESC)
        {
            if (RPRDESC.Equals(i_RPRDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select RPRDESC from empty_repaircodemaster").ToList();
            if (d.Contains(RPRDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateRPRCODE(String RPRCODE, String i_RPRCODE)
        {
            if (RPRCODE.Equals(i_RPRCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select RPRCODE from empty_repaircodemaster").ToList();
            if (d.Contains(RPRCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------


        //----------------------EmptySlabtype----------------------------
        public JsonResult ValidateSLABTDESC1(String SLABTDESC, String i_SLABTDESC)
        {
            if (SLABTDESC.Equals(i_SLABTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select SLABTDESC from SLABTYPEMASTER").ToList();
            if (d.Contains(SLABTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateSLABTCODE1(String SLABTCODE, String i_SLABTCODE)
        {
            if (SLABTCODE.Equals(i_SLABTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select SLABTCODE from SLABTYPEMASTER").ToList();
            if (d.Contains(SLABTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //----------------------ExportTarifftype----------------------------
        public JsonResult ValidateTARIFFTMDESC(String TARIFFTMDESC, String i_TARIFFTMDESC)
        {
            if (TARIFFTMDESC.Equals(i_TARIFFTMDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select TARIFFTMDESC from exporttarifftypemaster").ToList();
            if (d.Contains(TARIFFTMDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateTARIFFTMCODE(String TARIFFTMCODE, String i_TARIFFTMCODE)
        {
            if (TARIFFTMCODE.Equals(i_TARIFFTMCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select TARIFFTMCODE from exporttarifftypemaster").ToList();
            if (d.Contains(TARIFFTMCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //----------------------Godown----------------------------
        public JsonResult ValidateGDWNDESC(String GDWNDESC, String i_GDWNDESC)
        {
            if (GDWNDESC.Equals(i_GDWNDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select GDWNDESC from godownmaster").ToList();
            if (d.Contains(GDWNDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateGDWNCODE(String GDWNCODE, String i_GDWNCODE)
        {
            if (GDWNCODE.Equals(i_GDWNCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select GDWNCODE from godownmaster").ToList();
            if (d.Contains(GDWNCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //----------------------Grade----------------------------
        public JsonResult ValidateGRADEDESC(String GRADEDESC, String i_GRADEDESC)
        {
            if (GRADEDESC.Equals(i_GRADEDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select GRADEDESC from grademaster").ToList();
            if (d.Contains(GRADEDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateGRADECODE(String GRADECODE, String i_GRADECODE)
        {
            if (GRADECODE.Equals(i_GRADECODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select GRADECODE from grademaster").ToList();
            if (d.Contains(GRADECODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //----------------------Location----------------------------
        public JsonResult ValidateLOCTDESC(String LOCTDESC, String i_LOCTDESC)
        {
            if (LOCTDESC.Equals(i_LOCTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select LOCTDESC from locationmaster").ToList();
            if (d.Contains(LOCTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateLOCTCODE(String LOCTCODE, String i_LOCTCODE)
        {
            if (LOCTCODE.Equals(i_LOCTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select LOCTCODE from locationmaster").ToList();
            if (d.Contains(LOCTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //----------------------place----------------------------
        public JsonResult ValidatePLCDESC(String PLCDESC, String i_PLCDESC)
        {
            if (PLCDESC.Equals(i_PLCDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select PLCDESC from placemaster").ToList();
            if (d.Contains(PLCDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidatePLCCODE(String PLCCODE, String i_PLCCODE)
        {
            if (PLCCODE.Equals(i_PLCCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select PLCCODE from placemaster").ToList();
            if (d.Contains(PLCCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //-------------------------------------ProductGroup---------
        public JsonResult ValidatePRDTGDESC(String PRDTGDESC, String i_PRDTGDESC)
        {
            if (PRDTGDESC.Equals(i_PRDTGDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select PRDTGDESC from productgroupmaster").ToList();
            if (d.Contains(PRDTGDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidatePRDTGCODE(String PRDTGCODE, String i_PRDTGCODE)
        {
            if (PRDTGCODE.Equals(i_PRDTGCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select PRDTGCODE from productgroupmaster").ToList();
            if (d.Contains(PRDTGCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------


        //----------------------Row----------------------------
        public JsonResult ValidateROWDESC(String ROWDESC, String i_ROWDESC)
        {
            if (ROWDESC.Equals(i_ROWDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select ROWDESC from rowmaster").ToList();
            if (d.Contains(ROWDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateROWCODE(String ROWCODE, String i_ROWCODE)
        {
            if (ROWCODE.Equals(i_ROWCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select ROWCODE from rowmaster").ToList();
            if (d.Contains(ROWCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //----------------------slabtype----------------------------
        public JsonResult ValidateSLABTDESC(String SLABTDESC, String i_SLABTDESC)
        {
            if (SLABTDESC.Equals(i_SLABTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select SLABTDESC from slabtypemaster").ToList();
            if (d.Contains(SLABTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateSLABTCODE(String SLABTCODE, String i_SLABTCODE)
        {
            if (SLABTCODE.Equals(i_SLABTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select SLABTCODE from slabtypemaster").ToList();
            if (d.Contains(SLABTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //----------------------Bond Slabtype----------------------------
        public JsonResult ValidateBNDSLABTDESC(String SLABTDESC, String i_SLABTDESC)
        {
            if (SLABTDESC.Equals(i_SLABTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select SLABTDESC from BondSlabTypeMaster (nolock)").ToList();
            if (d.Contains(SLABTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateBNDSLABTCODE(String SLABTCODE, String i_SLABTCODE)
        {
            if (SLABTCODE.Equals(i_SLABTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select SLABTCODE from BondSlabTypeMaster (nolock)").ToList();
            if (d.Contains(SLABTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------


        //----------------------slot----------------------------
        public JsonResult ValidateSLOTDESC(String SLOTDESC, String i_SLOTDESC)
        {
            if (SLOTDESC.Equals(i_SLOTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select SLOTDESC from slotmaster").ToList();
            if (d.Contains(SLOTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateSLOTCODE(String SLOTCODE, String i_SLOTCODE)
        {
            if (SLOTCODE.Equals(i_SLOTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select SLOTCODE from slotmaster").ToList();
            if (d.Contains(SLOTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //----------------------stag----------------------------
        public JsonResult ValidateSTAGDESC(String STAGDESC, String i_STAGDESC)
        {
            if (STAGDESC.Equals(i_STAGDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select STAGDESC from stagmaster").ToList();
            if (d.Contains(STAGDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateSTAGCODE(String STAGCODE, String i_STAGCODE)
        {
            if (STAGCODE.Equals(i_STAGCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select STAGCODE from stagmaster").ToList();
            if (d.Contains(STAGCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------


        //----------------------Tariff----------------------------
        public JsonResult ValidateTARIFFMDESC(String TARIFFMDESC, String i_TARIFFMDESC)
        {
            if (TARIFFMDESC.Equals(i_TARIFFMDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select TARIFFMDESC from tariffmaster").ToList();
            if (d.Contains(TARIFFMDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateTARIFFMCODE(String TARIFFMCODE, String i_TARIFFMCODE)
        {
            if (TARIFFMCODE.Equals(i_TARIFFMCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select TARIFFMCODE from tariffmaster").ToList();
            if (d.Contains(TARIFFMCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //----------------------Unit----------------------------
        public JsonResult ValidateUNITDESC(String UNITDESC, String i_UNITDESC)
        {
            if (UNITDESC.Equals(i_UNITDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select UNITDESC from unitmaster").ToList();
            if (d.Contains(UNITDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateUNITCODE(String UNITCODE, String i_UNITCODE)
        {
            if (UNITCODE.Equals(i_UNITCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select UNITCODE from unitmaster").ToList();
            if (d.Contains(UNITCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------


        //----------------------vehicle----------------------------
        public JsonResult ValidateVHLMDESC(String VHLMDESC, String i_VHLMDESC)
        {
            if (VHLMDESC.Equals(i_VHLMDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select VHLMDESC from vehiclemaster").ToList();
            if (d.Contains(VHLMDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateVHLMCODE(String VHLMCODE, String i_VHLMCODE)
        {
            if (VHLMCODE.Equals(i_VHLMCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select VHLMCODE from vehiclemaster").ToList();
            if (d.Contains(VHLMCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //----------------------vessel----------------------------
        public JsonResult ValidateVSLDESC(String VSLDESC, String i_VSLDESC)
        {
            if (VSLDESC.Equals(i_VSLDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select VSLDESC from vesselmaster").ToList();
            if (d.Contains(VSLDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateVSLCODE(String VSLCODE, String i_VSLCODE)
        {
            if (VSLCODE.Equals(i_VSLCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select VSLCODE from vesselmaster").ToList();
            if (d.Contains(VSLCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        //----------------------yard----------------------------
        public JsonResult ValidateYRDDESC(String YRDDESC, String i_YRDDESC)
        {
            if (YRDDESC.Equals(i_YRDDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select YRDDESC from yardmaster").ToList();
            if (d.Contains(YRDDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateYRDCODE(String YRDCODE, String i_YRDCODE)
        {
            if (YRDCODE.Equals(i_YRDCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select YRDCODE from yardmaster").ToList();
            if (d.Contains(YRDCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------



        public JsonResult Validateamt(Decimal GPEAMT, Int32 GPETYPE)
        {
            //if (YRDCODE.Equals(i_YRDCODE))
            //    return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select YRDDESC from yardmaster").ToList();
            if (GPETYPE == 1 && GPEAMT == 0)
            {
                return Json("Enter Escord Amount", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            //return Json(false, JsonRequestBehavior.AllowGet);
        }


        // condin---2


        public JsonResult ValidateContainer(Decimal GPEAMT, Int32 GPETYPE)
        {
            //if (YRDCODE.Equals(i_YRDCODE))
            //    return Json(true, JsonRequestBehavior.AllowGet);
            if (GPETYPE == 1 && GPEAMT == 0)
            {
                return Json("Enter Escord Amount", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
            //return Json(false, JsonRequestBehavior.AllowGet);
        }

        //--------------export Tariff master-----------------//
        public JsonResult ValidateTARIFFMDESC1(String TARIFFMDESC, String i_TARIFFMDESC)
        {
            if (TARIFFMDESC.Equals(i_TARIFFMDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select TARIFFMDESC from exporttariffmaster").ToList();
            if (d.Contains(TARIFFMDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateTARIFFMCODE1(String TARIFFMCODE, String i_TARIFFMCODE)
        {
            if (TARIFFMCODE.Equals(i_TARIFFMCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select TARIFFMCODE from exporttariffmaster").ToList();
            if (d.Contains(TARIFFMCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateDuplicateVehicle(String id)
        {

            var Param = id.Split('~');
            var vhlno = Param[0];
            var TranID = Param[1];
            if (TranID == "0")
            {
                string qry = "select VHLSTS from VW_GEN_VHLNO_DUPLICATE_CTRL_CHK where vhlsts = 'IN' and vhlno = '" + vhlno + "'";
                List<String> d = context.Database.SqlQuery<String>(qry).ToList();
                return Json(d, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<String> d = context.Database.SqlQuery<String>("select 'OUT'").ToList();
                return Json(d, JsonRequestBehavior.AllowGet);
            }


        }
        //end




        //............................................stuffing......................................






        //................................end........................


        //......................Shut Out Cargo....................//




        //................................end........................



        //.........................carting no and ref no.............//
        public JsonResult Check(int id)
        {
            var result = context.Database.SqlQuery<int>("select SBDID from VW_STUFFING_SBILLNO_REFNO_CBX_ASSGN where SBMID=" + id).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult CostFactorDetail(int id)
        {
            var result = (from r in context.costfactormasters
                          where r.CFID.Equals(id)
                          select new { r.CFID, r.CFMODE, r.CFEXPR, r.CFTYPE, r.DORDRID, r.CFNATR }).Distinct();


            return Json(result, JsonRequestBehavior.AllowGet);

        }

        // Code added by Rajesh S on 15-Jul-2021 for Server Date Validation <Start>
        public string GetSrvCurDtTm()
        {
            var dateQuery = context.Database.SqlQuery<DateTime>("SELECT getdate()");
            DateTime serverDate = dateQuery.AsEnumerable().First();
            return (serverDate.ToString("dd/MM/yy hh:mm"));
        }




        // Code added by Rajesh S on 15-Jul-2021 for Server Date Date Validation <End>
        public string CostFactor(string term)
        {

            DbSqlQuery<CostFactorMaster> data = context.costfactormasters.SqlQuery("select * from costfactormaster  where CFID not in(" + term + "-1) order by DORDRID");

            string html = "<select id='TAX' class='TAX' name='TAX' onchange='sel_text(this,&quot;CFDESC&quot;);'  > ";

            string first = "";
            string f_order = "";
            string f_expr = "";
            string mod = "";
            string expr = "";
            string first_id = "0";

            int i = 0;

            foreach (var cost in data)
            {

                first_id = cost.CFID.ToString();

                if (i == 0)
                {
                    first = cost.CFDESC;
                    f_order = cost.DORDRID.ToString();
                    f_expr = cost.CFEXPR.ToString();
                    if (cost.CFMODE != 0)
                        mod = "selected";
                    if (cost.CFTYPE != 0)
                        expr = "selected";
                }

                html = html + "<option value='" + cost.CFID + "'>" + cost.CFDESC + "</option>";

                i++;

                //do something with cust
            }

            if (i == 1)
                html = "<input type=text name=TAX id='TAX'  class='hide TAX' value='" + first_id + "'><input type=text name=CFDESC id='CFDESC' class='CFDESC' value='" + first + "'>";
            else
                html = html + "</select><input style='display:none' type=text name=CFDESC id='CFDESC' class='CFDESC'  onchange='totalonchange(this)' value='" + first + "'>";

            html = html + "</td> <tD class='col-lg-1' > <input  type=text value='0' style='display:none' name=TMPCFVAL id='TMPCFVAL' class='TMPCFVAL' ><select id='CFTYPE' name='CFTYPE' class='CFTYPE' onchange='totalonchange(this)'> <option value='0' >Value </option><option value='1' " + expr + "  >  %</option> </select></td> <td class='col-lg-1'><input type='text' id='DEDNOS' class='DEDNOS' name='DEDNOS' value='0'  style='width:50px' onchange='totalonchange(this)'></td><td class='col-lg-1' > <input onchange='totalonchange(this)' type=text value='" + f_expr + "' class='CFEXPR' name='CFEXPR' id='CFEXPR'> </td><td><select onchange='total()' class='CFMODE' id='CFMODE' name='CFMODE'> <option value='0' >  +</option><option value='1' " + mod + " >-</option> </select><input type='text' id='DORDRID'   value='" + f_order + "' class='DORDRID' style='display:none'  name='DORDRID' >  <input  type=text value='0' style='display:none' name=TRANMFID id='TRANMFID' class='TRANMFID' >";
            html = html + "<input  type=text value='0' style='display:none' name=DEDORDR id='DEDORDR' class='DEDORDR' >";

            if (i == 0)
                html = "";

            return html;

        }
        //end




        //..................costfactor method.........................................
        //public string def_CostFactor()//DEFALUT CF
        //{
        //    string query = "select * from costfactormaster  where CFID in (2,77) order by CFID desc";
        //    IEnumerable<CostFactorMaster> data = context.Database.SqlQuery<CostFactorMaster>(query);
        //    string html = "";

        //    int i = 0; string mod = ""; var cfamt = 0;
        //    string expr = "";
        //    //   Response.Write(data.Count()); Response.End();
        //    foreach (var cost in data)
        //    {
        //       // if (i == 1) cfamt = 250;
        //        if (cost.CFMODE == 1)
        //            mod = "selected";
        //        if (cost.CFTYPE == 1)
        //            expr = "selected";
        //        html = html + "<tr><td></td><Td class=hide> <button href='#'   type='button' onclick='del_factor(this)' class='btn btn-danger btn-sm dfact'><i class='fa fa-trash-o'></i></button>  </td> <td><input type=text name=TAX id='TAX'  class='TAX form-control hide' value='" + cost.CFID + "'><input type=text name=CFDESC id='CFDESC' class='hide CFDESC form-control' value='" + cost.CFDESC + "'>" + cost.CFDESC + "";
        //        html = html + "</td> <tD class='col-lg-1' ><select id='CFTYPE' name='CFTYPE' class='CFTYPE form-control' onchange='total()'> <option value='0'>Value </option><option value='1' " + expr + "  >  %</option> </select></td> <td class='col-lg-1' > <input onchange='total()' type=text value='" + cost.CFEXPR + "' class='CFEXPR form-control' name='CFEXPR' id='CFEXPR'> </td><td><select onchange='total()' class='CFMODE' id='CFMODE' name='CFMODE'> <option value='+' >  +</option><option value='-' " + mod + " >-</option> </select><input type='text' id='DORDRID'   value='" + cost.DORDRID + "' class='DORDRID form-control' style='display:none'  name='DORDRID' >  <input  type=text value='0' style='display:none' name=TRANMFID id='TRANMFID' class='TRANMFID form-control' ><input type=text value=" + cost.CFNATR + " style='display:none' name=DEDORDR id='DEDORDR' class='DEDORDR'></td><td ><input  type=text value=" + cfamt + " name=CFAMOUNT id='CFAMOUNT' class='CFAMOUNT' readonly='readonly'> </td>  </TD></tr>";

        //        i++;
        //    }
        //    string query2 = "select * from costfactormaster  where CFID in (93) order by CFID desc";
        //    IEnumerable<CostFactorMaster> data2 = context.Database.SqlQuery<CostFactorMaster>(query2);
        //    foreach (var cost in data2)
        //    {
        //      //  if (i == 1) cfamt = 250;
        //        if (cost.CFMODE == 1)
        //            mod = "selected";
        //        if (cost.CFTYPE == 1)
        //            expr = "selected";
        //        html = html + "<tr><td></td><Td class=hide> <button href='#'   type='button' onclick='del_factor(this)' class='btn btn-danger btn-sm dfact'><i class='fa fa-trash-o'></i></button>  </td> <td><input type=text name=TAX id='TAX'  class='TAX form-control hide' value='" + cost.CFID + "'><input type=text name=CFDESC id='CFDESC' class='hide CFDESC form-control' value='" + cost.CFDESC + "'>" + cost.CFDESC + "";
        //        html = html + "</td> <tD class='col-lg-1' ><select id='CFTYPE' name='CFTYPE' class='CFTYPE form-control' onchange='total()'> <option value='0'>Value </option><option value='1' " + expr + "  >  %</option> </select></td> <td class='col-lg-1' > <input onchange='total()' type=text value='" + cost.CFEXPR + "' class='CFEXPR form-control' name='CFEXPR' id='CFEXPR'> </td><td><select onchange='total()' class='CFMODE' id='CFMODE' name='CFMODE'> <option value='+' >  +</option><option value='-' " + mod + " >-</option> </select><input type='text' id='DORDRID'   value='" + cost.DORDRID + "' class='DORDRID form-control' style='display:none'  name='DORDRID' >  <input  type=text value='0' style='display:none' name=TRANMFID id='TRANMFID' class='TRANMFID form-control' ><input type=text value="+cost.CFNATR+" style='display:none' name=DEDORDR id='DEDORDR' class='DEDORDR'></td><td ><input  type=text value=" + cfamt + " name=CFAMOUNT id='CFAMOUNT' class='CFAMOUNT' readonly='readonly'> </td>  </TD></tr>";

        //        i++;
        //    }
        //    return html;
        //}



    }
}