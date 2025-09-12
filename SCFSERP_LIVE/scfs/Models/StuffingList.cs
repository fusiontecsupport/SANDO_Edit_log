using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using scfs.Data;

namespace scfs_erp.Models
{
    public class StuffingList
    {
        public List<Stuffing_Json_Detail> jsondetaildata { get; set; }
        public List<z_pr_XML_Export_Stuffed_Detail_Assgn_Result> stuffeddetails { get; set; }
        public List<z_pr_XML_Stuffed_SBill_No_Detail_Assgn_Result> sbilldetails { get; set; }
    }
}