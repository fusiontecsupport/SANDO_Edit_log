using scfs_erp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers
{
    public class deletecheckController : Controller
    {
        SCFSERPContext context = new SCFSERPContext();
        //
        // GET: /deletecheck/
        public ActionResult Index()
        {
            return View();
        }

	}
}