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
    public class ExportTariffOPDetails
    {
        //public ExportTariffMaster tariff { get; set; }
        public List<ExportTariffOperationMaster> tariffoperation { get; set; }
    }
}