using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace scfs_erp.Models
{
    [Table("XMLwithOpenXML")]
    public class XMLwithOpenXML
    {
        [Key]
        public int XMLId { get; set; }
        public string XMLPath { get; set; }
        public string XMLFileName { get; set; }
        public string XMLData { get; set; }
        public DateTime LoadedDateTime { get; set; }
        public int XMLType { get; set; }
        public int XMLStatus { get; set; }
    }
}