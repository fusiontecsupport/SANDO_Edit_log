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
    [Table("CATEGORY_ADDRESS_DETAIL")]
    public class Category_Address_Details
    {
        [Key]
        public int CATEAID { get; set; }

        public int CATEID { get; set; }

        public int STATEID { get; set; }

        public string CATEAGSTNO { get; set; }

        public string CATEAADDR1 { get; set; }

        public string CATEAADDR2 { get; set; }

        public string CATEAADDR3 { get; set; }

        public string CATEAADDR4 { get; set; }

        public string CATEATYPEDESC { get; set; }
        public string CATEAPANNO { get; set; }

    }

    public class CategoryList
    {
        public CategoryMaster CategoryMaster { get; set; }
        public List<Category_Address_Details> CategoryAddressDetails { get; set; }
        public IEnumerable<string> pofile { get; set; }
    }
}