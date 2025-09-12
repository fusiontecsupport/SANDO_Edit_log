using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [Table("MenuRoleMaster")]
    public class MenuRoleMaster
    {
        [Key]
        public string LinkText { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string Roles { get; set; }
        public Nullable<short> MenuGId { get; set; }
        public Nullable<short> MenuGIndex { get; set; } 
    }
}