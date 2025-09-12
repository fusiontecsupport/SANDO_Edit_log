using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Models
{
    [Table("CATEGORYMASTER")]
    public class CategoryMaster
    {
        [Key]
        public int CATEID { get; set; }
        public int CATETID { get; set; }

        [DisplayName("Name")]
        //[Required(ErrorMessage = "Field is required")]
        [Remote("ValidateCATENAME", "Common", AdditionalFields = "i_CATENAME", ErrorMessage = "This is already used.")]
        public string CATENAME { get; set; }

        [DisplayName("Printable Name")]      
        public string CATEDNAME { get; set; }

        [DisplayName("Address")]
        //[Required(ErrorMessage = "Field is required")]
        public string CATEADDR { get; set; }

        [DisplayName("PHN Code")]
        public string CATEPHNID { get; set; }

        [DisplayName("Landline1")]       
        [DataType(DataType.PhoneNumber)]     
        public string CATEPHN1 { get; set; }

        [DisplayName("Landline2")]
        [DataType(DataType.PhoneNumber)]     
        public string CATEPHN2 { get; set; }

        [DisplayName("Mobile1")]
        [DataType(DataType.PhoneNumber)]      
        public string CATEPHN3 { get; set; }

        [DisplayName("Mobile2")]
        [DataType(DataType.PhoneNumber)]
       // [RegularExpression(@"([0-9]{10,11})", ErrorMessage = "Number Invalid..Enter 10 digit")]
        public string CATEPHN4 { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("Email")]
        [MaxLength(50)]       
        public string CATEMAIL { get; set; }

        [DisplayName("Contact Person")]       
        public string CATECPRSN { get; set; }

        [DisplayName("Code")]     
        //[Remote("ValidateCATECODE", "Common", AdditionalFields = "i_CATECODE", ErrorMessage = "This is already used.")]
        public string CATECODE { get; set; }
        public int ACHEADID { get; set; }
        public int TARIFFMID { get; set; }

        //public Nullable<int> CATEBTYPE { get; set; }

        public int CATEBTYPE { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("Email1")]    
        public string CATEMAIL2 { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("Email2")]    
        public string CATEMAIL3 { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("Email3")]       
        public string CATEMAIL4 { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("Email4")]     
        public string CATEMAIL5 { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("Email5")]       
        public string CATEMAIL6 { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("Email6")]     
        public string CATEMAIL7 { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("Email7")]     
        public string CATEMAIL8 { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("Email8")]    
        public string CATEMAIL9 { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("Email9")]      
        public string CATEMAIL10 { get; set; }

        [DisplayName("TDS")]       
        public decimal CATETDSEXPRN { get; set; }

        public string CATELICNO { get; set; }

        public decimal CATEFNOD { get; set; }

        public string CATEBNAME { get; set; }

        public string CATEBADDR { get; set; }

        public string CATEBSTATENAME { get; set; }

        public string CATEBRNCHCODE { get; set; }

        public string CATEBACCTYPE { get; set; }

        public string CATEBACCTNO { get; set; }

        public string CATEBPANNO { get; set; }

        public string CATEBNEFTCODE { get; set; }

        public string CATEBRTGSCODE { get; set; }

        public string CATETDSCODE { get; set; }

        public string CATETDSRATE { get; set; }

        public string CATEBFROM { get; set; }

        public string CATEBTO { get; set; }

        public string CATEBVATNO { get; set; }

        public string CATEBCSTNO { get; set; }

        public string CATEBCARGO { get; set; }

        public string CATEBSTAXNO { get; set; }

        public int CATESTRCALCTYPE { get; set; }

        public string CATEBTANNO { get; set; }

        [DisplayName("State")]
        public int STATEID { get; set; }

        [DisplayName("HSN Code")]
        public int HSNID { get; set; }

        public string CATEBGSTNO { get; set; }
        //public string CATEGSTNO { get; set; }

        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }

        [DisplayName("Status")]      
        public int DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public Nullable<DateTime> PRCSDATE { get; set; }

        public int CATEBILLTYPE { get; set; }

        public int CUSTGID { get; set; }

    }
}