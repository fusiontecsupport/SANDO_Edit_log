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
    [Table("EXPORTTARIFFOPERATIONMASTER")]
    public class ExportTariffOperationMaster
    {
        [Key]
        public int ETID { get; set; }
        public int TARIFFMID { get; set; }
        public int EOPTID { get; set; }
    }
}