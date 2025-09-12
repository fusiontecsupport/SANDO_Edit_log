using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs.Models
{
    [Table("DEBITNOTE_TYPEMASTER")]
    public class DebitNote_TypeMaster
    {
        [Key]
        public int DNTID { get; set; }

        public string DNTDESC { get; set; }
    }
}