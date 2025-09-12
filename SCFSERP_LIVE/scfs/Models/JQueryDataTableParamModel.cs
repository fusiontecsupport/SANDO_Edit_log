using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    public class JQueryDataTableParamModel
    {
        public string sEcho { get; set; }

        public string sSearch { get; set; }

        public int iDisplayLength { get; set; }

        public int iDisplayStart { get; set; }
    }
}