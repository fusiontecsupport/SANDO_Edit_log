using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SoftDepartmentMasterController : Controller
    {
        //
        // GET: /SoftDepartmentMaster/
        SCFSERPContext context = new SCFSERPContext();

        public ActionResult Index()
        {
            return View(context.softdepartmentmasters.ToList());
        }
        public ActionResult Form(int? id = 0)
        {
            SoftDepartmentMaster tab = new SoftDepartmentMaster();

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "0", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "1", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            tab.SDPTID = 0;
            if (id != 0)
            {
                tab = context.softdepartmentmasters.Find(id);

                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 0)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "0", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "1", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
            }
            return View(tab);
        }
        public void savedata(SoftDepartmentMaster tab)
        {
            tab.PRCSDATE = DateTime.Now;
            tab.OPTNSTR = tab.SDPTNAME;

            if ((tab.SDPTID).ToString() != "0")
            {
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                context.softdepartmentmasters.Add(tab);
                context.SaveChanges();
            }
            Response.Redirect("Index");
        }
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                SoftDepartmentMaster softdepartmentmaster = context.softdepartmentmasters.Find(Convert.ToInt32(id));
                context.softdepartmentmasters.Remove(softdepartmentmaster);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//End of Delete
	}
}