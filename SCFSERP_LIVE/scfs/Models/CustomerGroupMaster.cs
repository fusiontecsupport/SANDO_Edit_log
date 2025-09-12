using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs.Models
{
    [Table("CUSTOMERGROUPMASTER")]
    public class CustomerGroupMaster
    {
        [Key]
        public int CUSTGID { get; set; }
        public string CUSTGDESC { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}