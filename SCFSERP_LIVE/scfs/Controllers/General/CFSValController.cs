using scfs_erp.Context;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers
{
    public class CFSValController : Controller
    {
        SCFSERPContext context = new SCFSERPContext();
        //
        // GET: /CFSVal/
        public JsonResult ValidateCode(String ACHEADGDESC)
        {
            List<String> d = context.Database.SqlQuery<String>("select ACHEADGDESC from accountgroupmaster").ToList();
            if (d.Contains(ACHEADGDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
       
       
        
	}
}