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

    [Table("Export_StuffTypeMaster")]
    public class Export_StuffTypeMaster
    {
        [Key]
        public int ESID { get; set; }

        public int ESTID { get; set; }

        public string ESTDESC { get; set; }
    }
}