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
    [Table("VEHICLEMASTER")]
    public class VehicleMaster
    {
        [Key]
        public int VHLMID { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]
        [Remote("ValidateVHLMDESC", "Common", AdditionalFields = "i_VHLMDESC", ErrorMessage = "This is already used.")]
        public string VHLMDESC { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Field is required")]

        public string AVHLMDESC { get; set; }
        [DisplayName("Driver Name")]
        [Required(ErrorMessage = "Field is required")]
        public string VHLDRVNAME { get; set; }
        public int TRNSPRTID { get; set; }
        public string CUSRID { get; set; }
        public int LMUSRID { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public short DISPSTATUS { get; set; }
        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
        [DisplayName("PNR No.")]
        public string VHLPNRNO { get; set; }
        
    }
}