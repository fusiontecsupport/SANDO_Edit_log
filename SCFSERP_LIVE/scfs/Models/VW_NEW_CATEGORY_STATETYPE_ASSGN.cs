
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

    [Table("VW_NEW_CATEGORY_STATETYPE_ASSGN")]
    public partial class VW_NEW_CATEGORY_STATETYPE_ASSGN
    {
        [Key]
        public int CATEID { get; set; }
        public string CATENAME { get; set; }
        public short STATETYPE { get; set; }
        public int CATETID { get; set; }
        public Nullable<int> DISPSTATUS { get; set; }
    }
}