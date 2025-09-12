
using scfs.Data;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System;
using System.Configuration;

namespace scfs_erp.Controllers
{
   
    public class CompanyMasterController : Controller
    {
        SCFSERPContext context = new SCFSERPContext();
        //
        // GET: /Company/
        [Authorize(Roles="CompanyMasterIndex")]
        public ActionResult Index()
        {

            return View(context.companymasters.ToList());//Loading Grid
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_SearchCompanyMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                var aaData = data.Select(d => new string[] { d.COMPCODE, d.COMPNAME, d.COMPADDR, d.COMPPHN1, d.COMPCPRSN, d.COMPMAIL, d.COMPID.ToString() }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "CompanyMasterEdit")]
        public void Edit(int id)
        {
            var strPath = ConfigurationManager.AppSettings["BaseURL"];

            Response.Redirect("" + strPath + "/CompanyMaster/Form/" + id);

            //Response.Redirect("/CompanyMaster/Form/" + id);
        }

        [Authorize(Roles = "CompanyMasterCreate")]
        public ActionResult Form(int id = 0)
        {
            CompanyMaster tab = new CompanyMaster();

            ViewBag.STATEID = new SelectList(context.statemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.STATEDESC), "STATEID", "STATEDESC");
            ViewBag.HSNID = new SelectList(context.HSNCodeMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.HSNCODE), "HSNID", "HSNCODE");

            tab.COMPCOFFTIME = DateTime.Now;
            if (id != 0)
            {
                tab = context.companymasters.Find(id);

                ViewBag.STATEID = new SelectList(context.statemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.STATEDESC), "STATEID", "STATEDESC", tab.STATEID);
                ViewBag.HSNID = new SelectList(context.HSNCodeMasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.HSNCODE), "HSNID", "HSNCODE", tab.HSNID);

            }
            return View(tab);
        }//End

        public void savedata(CompanyMaster tab)
        {


            tab.COMPPHNID = "0"; tab.CUSRID = Session["CUSRID"].ToString();
            tab.PRCSDATE = DateTime.Now;
            tab.COMPCOFFTIME = Convert.ToDateTime(tab.COMPCOFFTIME).Date;
            tab.LMUSRID = 1;
            tab.DISPSTATUS = 0;
            if (tab.COMPID.ToString() != "0" || tab.COMPID != 0)
            {

                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
              
                context.companymasters.Add(tab);
                context.SaveChanges();
            }
            Response.Redirect("Index");
        }
        
        
        //End of savedata
        //------------------------Delete Record----------//
         [Authorize(Roles = "CompanyMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);

            if (temp.Equals("PROCEED"))
            {
                CompanyMaster companymasters = context.companymasters.Find(Convert.ToInt32(id));
                context.companymasters.Remove(companymasters);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//End of Delete
    }//End of Class
 }//End of namespace
