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
    [Table("COSTFACTORMASTER")]
    public class CostFactorMaster
    {
        [Key]

        public int CFID { get; set; }


        [Required(ErrorMessage = "Field is required")]
        public int TRANTID { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Please Enter Description !!")]
        [Remote("ValidateCFDESC", "Common", AdditionalFields = "i_CFDESC", ErrorMessage = "This is already used.")]
        public string CFDESC { get; set; }
       
        [DisplayName("Accounted to")]
        [Required(ErrorMessage = "Please Enter Accounted to !!")]
        public int ACHEADID { get; set; }
        public short CFOPTN { get; set; }
        [DisplayName("MODE")]
        public short CFMODE { get; set; }
        [DisplayName("As")]
        public short CFTYPE { get; set; }
        [DisplayName("Value")]
        public decimal CFEXPR { get; set; }
        [DisplayName("On")]
        public short CFNATR { get; set; }
        [DisplayName("Belongs To")]
        public short DORDRID { get; set; }
        public int LMUSRID { get; set; }
        public string CUSRID { get; set; }
        [DisplayName("Status")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}