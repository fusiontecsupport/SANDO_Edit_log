using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Helper
{
    public class AmtInWrd
    {
        public static string ConvertNumbertoWords(string number)
        {
            string currency = "";               //  for adding a prefix before converted string          
            if (Convert.ToDouble(number) == 0)
            {

                currency = "ZERO";             //if user enters null value
                return currency;
            }
            if (Convert.ToDouble(number) < 0)
            {
                currency = "Invalid Value";

                return currency;
            }
            string[] no =  number.Split('.');
            string Number; string deciml; string _number; string _deciml;
            if ((no[0] != null) && (no[1] != "00"))      //spliting the texbox value to check if any decimal point is added after number  
            {
                Number = no[0]; deciml = no[1];
                _number = (NameOfNumber(Convert.ToInt32(Number))) ;
                _deciml = (NameOfNumber(Convert.ToInt32(deciml))) + "  " + "PAISE";

                currency = _number.Trim() + " AND " + _deciml.Trim() + "  " + "ONLY";
            }
            if ((no[0] != null) && (no[1] == "00"))      // check if user does not input decimal, value
            {
                Number = no[0];
                _number = (NameOfNumber(Convert.ToInt32(Number))) ;

                currency = currency + _number + "  " + "ONLY"; // showing whole  result with suffix
            }
            if ((Convert.ToDouble(no[0]) == 0) && (no[1] != null))
            {
                deciml = no[1];
                _deciml = (NameOfNumber(Convert.ToInt32(deciml))) + "  " + "PAISE";
                currency = _deciml + currency + "  " + "ONLY";

            }
            return currency;
        }
        public static string NameOfNumber(int number)
        {
            var crore = 0; var lakhs = 0;

         if (number == 0)
                return "ZERO";
            if (number < 0)
                return "minus " + NameOfNumber(Math.Abs(number));
            string words = "";

            crore = number / 10000000; number = number - crore * 10000000;
            lakhs = number / 100000; number = number - lakhs * 100000;
            if (crore > 0)
            {

                words += NameOfNumber(crore) + " CRORE ";
               
            }

            if (lakhs > 0)
            {
               
                    words += NameOfNumber(lakhs) + " LAKH ";
                
            }
            // if ((number / 1000000) > 0)
            //{
            //    words += NameOfNumber(number / 1000000) + " MILLION ";
            //    number %= 1000000;
            //}
            if ((number / 1000) > 0)
            {
                words += NameOfNumber(number / 1000) + " THOUSAND ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += NameOfNumber(number / 100) + " HUNDRED ";
                number %= 100;
            }
            if (number > 0)
            {
                if (words != "")
                    words += "AND ";
                var unitsMap = new[] { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
                var tensMap = new[] { "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }
            return words;
        
        }

    }
}