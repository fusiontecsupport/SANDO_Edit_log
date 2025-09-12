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

    [Table("EXPORT_OPERATIONTYPEMASTER")]
    public class Export_OperationTypeMaster
    {
        [Key]
        public int EOPTID { get; set; }
        [Remote("ValidateEOPTDESC", "Common", AdditionalFields = "i_EOPTDESC", ErrorMessage = "This is already used.")]
        public string EOPTDESC { get; set; }
        [Remote("ValidateEOPTCODE", "Common", AdditionalFields = "i_EOPTCODE", ErrorMessage = "This is already used.")]
        public string EOPTCODE { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}