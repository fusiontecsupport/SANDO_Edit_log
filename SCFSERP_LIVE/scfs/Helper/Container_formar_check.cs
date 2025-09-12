using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace scfs_erp.Helper
{
    public class Container_formar_check
    {
        public static long container_format(string container_no)
        {
            var TmpPStatus = PRJ_CONTAINER_FIRST_FOUR_DIGIT_CHK_ASSGN(container_no);
            if ((TmpPStatus == "PROCEED"))
            {
                if ((container_no.Trim().Length == 11))
                {
                    var TmpADigit = container_no.Substring((container_no.Length - 1));
                    var TmpLDigit = PRJ_CONTAINER_CHECK_DIGIT_ASSGN(container_no);
                    if ((Convert.ToInt64(TmpADigit) == TmpLDigit))
                    {
                        TmpPStatus = "PROCEED";
                    }
                    else
                    {
                        TmpPStatus = "CONT_VALID";
                    }
                }
                else
                {
                    TmpPStatus = "CONT_11DIGIT";
                }
            }
            else
            {
                TmpPStatus = ((TmpPStatus == "PROCEED") ? "CONT_VALID" : TmpPStatus);
            }
            return 0;

        }

        public static string PRJ_CONTAINER_FIRST_FOUR_DIGIT_CHK_ASSGN(string PContnrNo)
        {
            int TmpCVal;
            for (int I = 1; (I <= 4); I++)
            {
              var   TmpAStr = PContnrNo.Substring((I - 1), 1);
             // 
                if ((TmpAStr.Trim().Length > 0))
                {
                    var a = Convert.ToInt32(TmpAStr);
                    TmpCVal = a;

                  // TmpCVal =Convert.ToByte(Encoding.ASCII.GetBytes(TmpAStr));
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
            }
            for (int I = 5; (I <= 10); I++)
            {
              var  TmpAStr = PContnrNo.Substring((I - 1), 1);
                if ((TmpAStr.Trim().Length > 0))
                {
                    TmpCVal = Convert.ToByte(Encoding.ASCII.GetBytes(TmpAStr));
                }
                else
                {
                    TmpCVal = 0;
                }
                if (((TmpCVal < 48)
                            || (TmpCVal > 57)))
                {
                    return  "CONT_6DIGIT";
                    break;
                }
                else
                {
                    return  "PROCEED";
                }
            }
            return "";
        }

        public static long PRJ_CONTAINER_CHECK_DIGIT_ASSGN(string PContnrNo)
        {
            long TmpACol1;
            long TmpACol2;
            long TmpACol3;
            long TmpACol4;
            long TmpACol5;
            long TmpACol6;
            long TmpACol7;
            long TmpACol8;
            long TmpACol9;
            long TmpACol10;
            long TmpACol11;
            long TmpATCol;
            TmpATCol = 0;
            TmpACol11 = 0;
            TmpACol1 = PRJ_CONTAINER_ALPHABETS_NUMBER_ASSGN(PContnrNo.Substring(0, 1).Trim());
            TmpACol2 = PRJ_CONTAINER_ALPHABETS_NUMBER_ASSGN(PContnrNo.Substring(1, 1).Trim());
            TmpACol3 = PRJ_CONTAINER_ALPHABETS_NUMBER_ASSGN(PContnrNo.Substring(2, 1).Trim());
            TmpACol4 = PRJ_CONTAINER_ALPHABETS_NUMBER_ASSGN(PContnrNo.Substring(3, 1).Trim());
            TmpACol5 =Convert.ToInt64(PContnrNo.Substring(4, 1));
            TmpACol6 = Convert.ToInt64(PContnrNo.Substring(5, 1));
            TmpACol7 = Convert.ToInt64(PContnrNo.Substring(6, 1));
            TmpACol8 =Convert.ToInt64( PContnrNo.Substring(7, 1));
            TmpACol9 =Convert.ToInt64( PContnrNo.Substring(8, 1));
            TmpACol10 = Convert.ToInt64(PContnrNo.Substring(9, 1));
            TmpACol1 = (TmpACol1 * 1);
            TmpACol2 = (TmpACol2 * 2);
            TmpACol3 = (TmpACol3 * 4);
            TmpACol4 = (TmpACol4 * 8);
            TmpACol5 = (TmpACol5 * 16);
            TmpACol6 = (TmpACol6 * 32);
            TmpACol7 = (TmpACol7 * 64);
            TmpACol8 = (TmpACol8 * 128);
            TmpACol9 = (TmpACol9 * 256);
            TmpACol10 = (TmpACol10 * 512);
            TmpATCol = (TmpACol1
                        + (TmpACol2
                        + (TmpACol3
                        + (TmpACol4
                        + (TmpACol5
                        + (TmpACol6
                        + (TmpACol7
                        + (TmpACol8
                        + (TmpACol9 + TmpACol10)))))))));
            TmpACol11 = (TmpATCol % 11);
            if ((TmpACol11 > 9))
            {
                TmpACol11 = 0;
            }
            return TmpACol11;
        }


        private static int PRJ_CONTAINER_ALPHABETS_NUMBER_ASSGN(string PDesc)
        {
            int TmpCol1=0;
            switch (PDesc)
            {
                case "A":
                    TmpCol1 = 10;
                    break;
                case "B":
                    TmpCol1 = 12;
                    break;
                case "C":
                    TmpCol1 = 13;
                    break;
                case "D":
                    TmpCol1 = 14;
                    break;
                case "E":
                    TmpCol1 = 15;
                    break;
                case "F":
                    TmpCol1 = 16;
                    break;
                case "G":
                    TmpCol1 = 17;
                    break;
                case "H":
                    TmpCol1 = 18;
                    break;
                case "I":
                    TmpCol1 = 19;
                    break;
                case "J":
                    TmpCol1 = 20;
                    break;
                case "K":
                    TmpCol1 = 21;
                    break;
                case "L":
                    TmpCol1 = 23;
                    break;
                case "M":
                    TmpCol1 = 24;
                    break;
                case "N":
                    TmpCol1 = 25;
                    break;
                case "O":
                    TmpCol1 = 26;
                    break;
                case "P":
                    TmpCol1 = 27;
                    break;
                case "Q":
                    TmpCol1 = 28;
                    break;
                case "R":
                    TmpCol1 = 29;
                    break;
                case "S":
                    TmpCol1 = 30;
                    break;
                case "T":
                    TmpCol1 = 31;
                    break;
                case "U":
                    TmpCol1 = 32;
                    break;
                case "V":
                    TmpCol1 = 34;
                    break;
                case "W":
                    TmpCol1 = 35;
                    break;
                case "X":
                    TmpCol1 = 36;
                    break;
                case "Y":
                    TmpCol1 = 37;
                    break;
                case "Z":
                    TmpCol1 = 38;
                    break;
            }
            return TmpCol1;
        }

    }
}