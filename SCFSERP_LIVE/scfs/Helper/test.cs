using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace scfs_erp.Helper
{
    public class test
    {

        public string PRJ_CONTAINER_FIRST_FOUR_DIGIT_CHK_ASSGN(string PContnrNo)
        {
            int TmpCVal;
            for (int I = 1; (I <= 4); I++)
            {
               var TmpAStr = PContnrNo.Substring((I - 1), 1);
                if ((TmpAStr.Trim().Length > 0))
                {
                    TmpCVal = Convert.ToInt32(TmpAStr);
                }
                else
                {
                    TmpCVal = 0;
                }
                if (((TmpCVal < 65)
                            || (TmpCVal > 97)))
                {
                    return "CONT_4DIGIT";
                    break;
                }
                else
                {
                    return "PROCEED";
                }
                break;
            }
           
            for (int I = 5; (I <= 10); I++)
            {
               var TmpAStr = PContnrNo.Substring((I - 1), 1);
                if ((TmpAStr.Trim().Length > 0))
                {
                    TmpCVal = Convert.ToInt32(TmpAStr);
                }
                else
                {
                    TmpCVal = 0;
                }
                if (((TmpCVal < 48)
                            || (TmpCVal > 57)))
                {
                    return "CONT_6DIGIT";
                    break;
                }
                else
                {
                    return "PROCEED";
                }
                break;
            }
            return "";
        }

         
    }
      
       
}
