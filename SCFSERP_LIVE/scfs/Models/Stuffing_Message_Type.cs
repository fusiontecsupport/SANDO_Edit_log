using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("STUFFING_MESSAGE_TYPE")]
    public class Stuffing_Message_Type
    {
        [Key]
        public int STF_MSG_TID { get; set; }
        public string STF_MSG_TDESC { get; set; }
        public string STF_MSG_TCODE { get; set; }
        public int CUSRID { get; set; }
        public int LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}