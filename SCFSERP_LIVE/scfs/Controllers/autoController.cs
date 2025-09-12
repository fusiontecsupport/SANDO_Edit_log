using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers
{
    public class autoController : Controller
    {
        //
        // GET: /Default1/
        public ActionResult Index()
        {
            var regsid = 16;
            var btype = 1;
            return View();
        }
	}
}