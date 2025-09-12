using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    
    [Table("EXPORTSHIPPINGBILLMASTER")]
    public class ExportShippingBillMaster
    {
        [Key]
        public int ESBMID { get; set; }
        public int COMPYID { get; set; }
        // [Required(ErrorMessage = "Field is required")]
        public int ESBMNO { get; set; }
        public string ESBMDNO { get; set; }
        public DateTime ESBMDATE { get; set; }
        public int EXPRTID { get; set; }
        [Required(ErrorMessage = "Field is required")]
        public string EXPRTNAME { get; set; }
        public int CHAID { get; set; }
        //  [Required(ErrorMessage = "Field is required")]
        public string CHANAME { get; set; }
        // [Required(ErrorMessage = "Field is required")]
        public string ESBMREFNO { get; set; }
        public DateTime ESBMREFDATE { get; set; }
        // [Required(ErrorMessage = "Field is required")]
        public decimal ESBMREFAMT { get; set; }
        //  [Required(ErrorMessage = "Field is required")]
        public decimal ESBMFOBAMT { get; set; }
        //  [Required(ErrorMessage = "Field is required")]
        public string ESBMDPNAME { get; set; }
        //  [Required(ErrorMessage = "Field is required")]
        public int PRDTGID { get; set; }
        //  [Required(ErrorMessage = "Field is required")]
        public string PRDTDESC { get; set; }
        // [Required(ErrorMessage = "Field is required")]
        public decimal ESBMNOP { get; set; }
        // [Required(ErrorMessage = "Field is required")]
        public decimal ESBMQTY { get; set; }
        public string ESBMRMKS { get; set; }
        public short ESBMITYPE { get; set; }
        public Nullable <DateTime> ESBMIDATE { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public DateTime PRCSDATE { get; set; }
        public short DISPSTATUS { get; set; }
        public string DESTINATION { get; set; }
        
    }
}