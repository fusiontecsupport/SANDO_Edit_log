using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace scfs_erp.Models
{
    [Table("BONDVEHICLETICKETDETAIL")]
    public class ExBondVehicleTicket
    {
        [Key]
        public int VTDID { get; set; }
        public int COMPYID { get; set; }
        public int SDPTID { get; set; }
        public System.DateTime VTDATE { get; set; }
        public System.DateTime VTTIME { get; set; }
        public int VTNO { get; set; }
        public string VTDNO { get; set; }
        public string VHLNO { get; set; }
        public string DRVNAME { get; set; }
        public string VTDESC { get; set; }
        public Nullable<decimal> VTQTY { get; set; }
        public Nullable<int> VTTYPE { get; set; }
        public Nullable<int> VTSTYPE { get; set; }
        public string VTREMRKS { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public Nullable<short> DISPSTATUS { get; set; }
        public Nullable<System.DateTime> PRCSDATE { get; set; }
        public int EBNDID { get; set; }
        //public int GIDID { get; set; }
        public decimal VTAREA { get; set; }
        public Nullable<int> CONTNRSID { get; set; }
        public Nullable<decimal> EBVTNOC { get; set; }
    }
}