using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using scfs.Data;

namespace scfs_erp.Models
{
    public class ExportShippingAdmission
    {
        public int EXPRTID { get; set; }
        public string EXPRTNAME { get; set; }
        public int CHAID { get; set; }
        public string CHANAME { get; set; }
        
        public List<pr_Search_Export_Corderdetails_Result> details { get; set; }
        public List<ExportShippingBillDetail> sbillDetails { get; set; }
        public List<pr_Search_ExportShippingAdmission_Multiple_Result> sbillDetailsMultiple { get; set; }
        
    }

    public class ExportShippingAdmissionMD
    {
        
        public List<pr_Search_Export_Corderdetails_Result> details { get; set; }
        public List<ExportShippingBillDetail> sbillDetails { get; set; }
        public List<pr_Search_ExportShippingAdmission_Multiple_Result> sbillDetailsMultiple { get; set; }

    }
}