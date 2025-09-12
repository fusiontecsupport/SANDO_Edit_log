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

    [Table("DEPARTMENTMASTER")]
    public class DepartmentMaster
    {
        [Key]
        public int DEPTID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Please Enter Description !!")]
        [Remote("ValidateDEPTDESC", "Common", AdditionalFields = "i_DEPTDESC", ErrorMessage = "This is already used.")]
        public string DEPTDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Please Enter Code !!")]
        [Remote("ValidateDEPTCODE", "Common", AdditionalFields = "i_DEPTCODE", ErrorMessage = "This is already used.")]
        public string DEPTCODE { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Please Select Status !!")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}