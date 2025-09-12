using scfs_erp.Context;

using scfs.Data;

using scfs_erp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        ApplicationDbContext _db = new ApplicationDbContext();                

        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        IAuthenticationManager Authentication
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }


        public UserManager<ApplicationUser> UserManager { get; private set; }


        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //Session.Abandon();
            SCFSERPContext context = new SCFSERPContext(); 
            ViewBag.COMPID = new SelectList(context.companymasters, "COMPID", "COMPNAME");
            Session["LDATE"] = DateTime.Now.ToString("dd-MM-yyyy");

            //if (DateTime.Now.Month >= 4)
            //{
            //    Session["GYrDesc"] = (DateTime.Now.Year) + " - " + (DateTime.Now.Year + 1);
            //    string s = (DateTime.Now.Year) + " - " + (DateTime.Now.Year + 1);
            //    string gprx1 = s.Substring(s.Length - 9, 2);
            //    string gprx2 = s.Substring(s.Length - 4, 2);
            //    Session["GPrxDesc"] = gprx1 + gprx2;
            //}
            //else
            //{
            //    Session["GYrDesc"] = (DateTime.Now.Year - 1) + " - " + (DateTime.Now.Year);
            //    string s = (DateTime.Now.Year) + " - " + (DateTime.Now.Year + 1);
            //    string gprx1 = s.Substring(s.Length - 9, 2);
            //    string gprx2 = s.Substring(s.Length - 4, 2);
            //    Session["GPrxDesc"] = gprx1 + gprx2;
            //}
            ViewBag.COMPYID = new SelectList(context.VW_ACCOUNTING_YEAR_DETAIL_ASSGN, "COMPYID", "YRDESC");
            //ViewBag.COMPYID = new SelectList(context.Database.SqlQuery<VW_ACCOUNTING_YEAR_DETAIL_ASSGN>("select * from VW_ACCOUNTING_YEAR_DETAIL_ASSGN order by YRDESC").ToList(), "COMPYID", "YRDESC");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            SCFSERPContext context = new SCFSERPContext();
            Session["CUSRID"] = ""; Session["COMPCODE"] = "";
            ViewBag.COMPID = new SelectList(context.companymasters, "COMPID", "COMPNAME");
            ViewBag.COMPYID = new SelectList(context.VW_ACCOUNTING_YEAR_DETAIL_ASSGN, "COMPYID", "YRDESC");

            //ViewBag.COMPYID = new SelectList(context.Database.SqlQuery<VW_ACCOUNTING_YEAR_DETAIL_ASSGN>("select * from VW_ACCOUNTING_YEAR_DETAIL_ASSGN order by YRDESC").ToList(), "COMPYID", "YRDESC");

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                
                Session["CUSRID"] = model.UserName;
                Session["compyid"] = model.COMPYID;

                Session["LDATE"] = Request.Form.Get("LDATE"); var COMPID = Request.Form.Get("COMPID");
                DateTime TmpDate = Convert.ToDateTime(Request.Form.Get("LDATE")).Date;
                var LMNTH = TmpDate.Month; var LYR = TmpDate.Year; var PFYear = 0; var PTYear = 0; var PFDATE = ""; var PTDATE = ""; var GYrDesc = "";

                if (LMNTH >= 4)
                {// Response.Write(LMNTH + ".." + LYR + "..." + Session["LDATE"]); Response.End(); 
                    PFYear = LYR;
                    PTYear = LYR + 1;
                    PFDATE = "01/04/" + PFYear; PTDATE = "31/03/" + PTYear;
                    GYrDesc = PFYear + " - " + PTYear;


                }
                else
                { //Response.Write("ELSE" + LMNTH + ".." + LYR + "..." + Session["LDATE"]); Response.End(); 
                    PFYear = LYR - 1;
                    PTYear = LYR;
                    PFDATE = "01/04/" + PFYear; PTDATE = "31/03/" + PTYear;
                    GYrDesc = PFYear + " - " + PTYear;
                }

                List<int> Max_YrId = new List<int>();
                var ACCYR_QRY = context.Database.SqlQuery<PR_ACCOUNTINGYEAR_ID_CHK_Result>("PR_ACCOUNTINGYEAR_ID_CHK @PFYear=" + PFYear + ",@PTYear=" + PTYear + "").ToList();
                if (ACCYR_QRY.Count == 0)
                {
                    context.Database.ExecuteSqlCommand("INSERT INTO AccountingYear (  YrDesc, FDate, TDate, CUSRID, PRCSDATE ) VALUES  ( '" + GYrDesc + "', '" + Convert.ToDateTime(PFDATE).ToString("MM/dd/yyyy") + "', '" + Convert.ToDateTime(PTDATE).ToString("MM/dd/yyyy") + "', '" + Session["CUSRID"] + "', '" + DateTime.Now.ToString("MM-dd-yyyy") + "')");
                    Max_YrId = context.Database.SqlQuery<Int32>("SELECT MAX(YRID) FROM AccountingYear").ToList();
                }
                else
                {
                    var ROW = ACCYR_QRY.Count - 1;
                    Max_YrId.Add(ACCYR_QRY[ROW].YRID);
                }

                var GCID = context.Database.SqlQuery<int>("select MAX(COMPYID) from CompanyAccountingDetail").ToList();

                var COMPDTL_QRY = context.Database.SqlQuery<PR_COMPANYACCOUNTINGDETAIL_ID_CHK_Result>("PR_COMPANYACCOUNTINGDETAIL_ID_CHK @PCompId=" + COMPID + ",@PYrId=" + Convert.ToInt32(Max_YrId[0]) + "").ToList();
                if (COMPDTL_QRY.Count == 0)
                {
                    context.Database.ExecuteSqlCommand("INSERT INTO CompanyAccountingDetail (COMPYID, CompId, YrId,  CUSRID, PRCSDATE ) VALUES  ( " + Convert.ToInt32(GCID[0] + 1) + "," + COMPID + ", " + Convert.ToInt32(Max_YrId[0]) + ",  '" + Session["CUSRID"] + "', '" + DateTime.Now.ToString("MM-dd-yyyy") + "')");
                    // GCID = context.Database.SqlQuery<int>("select MAX(COMPYID) from CompanyAccountingDetail").ToList();
                    Session["compyid"] = Convert.ToInt32(GCID[0] + 1);
                }
                else
                {
                    Session["compyid"] = Convert.ToInt32(COMPDTL_QRY[0].COMPYID);
                }


                Session["GYrDesc"] = GYrDesc;
                string yrdesc = Session["GYrDesc"].ToString();
                string ayrdesc = yrdesc.Substring(2, 2) + yrdesc.Substring(9, 2);
                Session["GPrxDesc"] = ayrdesc;

                Session["COMPCODE"] = "SCFS"; Session["COMPNAME"] = "SANCO TRANS LIMITED"; Session["EXCLPATH"] = "F:\\SCFS\\" + Session["CUSRID"] + "\\";
                Session["Group"] = ""; Session["GDNAME"] = ""; Session["GDBNAME"] = ""; Session["GUNAME"] = ""; Session["GUPASS"] = "";

                var zcompsql = context.Database.SqlQuery<CompanyMaster>("select * from CompanyMaster").ToList();
                if (zcompsql.Count > 0)
                {
                    Session["COVIDSDATE"] = Convert.ToDateTime(zcompsql[0].COMP_COVID_SDATE).Date;
                    Session["COVIDEDATE"] = Convert.ToDateTime(zcompsql[0].COMP_COVID_EDATE).Date;
                    Session["COMPNAME"] = zcompsql[0].COMPDNAME;
                    Session["COMPADDR"] = zcompsql[0].COMPADDR;
                    Session["COMPGSTNO"] = zcompsql[0].COMPGSTNO;
                    Session["COMPMAIL"] = zcompsql[0].COMPMAIL;
                    Session["COMPPHN1"] = zcompsql[0].COMPPHN1;
                    Session["COMPPHN2"] = zcompsql[0].COMPPHN2;

                }
                

                var sql = context.Database.SqlQuery<int>("select GroupId from ApplicationUserGroups inner join AspNetUsers on AspNetUsers.Id=ApplicationUserGroups.UserId where AspNetUsers.UserName='" + model.UserName + "'").ToList();
               // var sql = context.Database.SqlQuery<string>("select cast(GroupId as varchar(10))+'~'+GroupName from aspnet_user_group where UserName='" + model.UserName + "' order by groupid").ToList();

                if (sql.Count != 0 && user != null)
                {
                    //var param = sql[0].Split('~');

                    //Session["Group"] = param[1];
                    if (sql[0].Equals(17)) { Session["Group"] = "Admin"; }
                    if (sql[0].Equals(1)) { Session["Group"] = "SuperAdmin"; }
                    if (sql[0].Equals(2)) { Session["Group"] = "GroupAdmin"; }
                    if (sql[0].Equals(4)) { Session["Group"] = "Users"; }
                    if (sql[0].Equals(3)) { Session["Group"] = "UserAdmin"; }
                    if (sql[0].Equals(5)) { Session["Group"] = "GP"; }
                    if (sql[0].Equals(6)) { Session["Group"] = "Billing"; }
                    if (sql[0].Equals(7)) { Session["Group"] = "Imports"; }
                    if (sql[0].Equals(8)) { Session["Group"] = "Exports"; }
                    if (sql[0].Equals(14)) { Session["Group"] = "Non PNR"; }
                    if (sql[0].Equals(16)) { Session["Group"] = "Accounts"; }
                    if (sql[0].Equals(18)) { Session["Group"] = "SuperAdmin"; }

                    await SignInAsync(user, model.RememberMe);

                    Session["AdminGrp"] = "N";
                    if(Session["Group"].ToString().Contains("Admin"))
                    {
                        Session["AdminGrp"] = "Y";
                    }
                        context.Database.ExecuteSqlCommand("delete from menurolemaster where Roles='" + model.UserName + "'");
                    context.Database.ExecuteSqlCommand("EXEC pr_USER_MENU_DETAIL_ASSGN @PKUSRID='" + model.UserName + "'");
                 
                    var tqry = "select count('*') as 'hndrlcnt' from AspNetUsers a(nolock) ,  AspNetUserRoles b(nolock), AspNetRoles c(nolock)" +
                        " where a.id = b.UserId " +
                        " and b.RoleId = c.id " +
                        " and a.UserName = '" + model.UserName + "' and c.Name = 'HANDLING CHARGES UPDATE'";
                    var zhandupdrolechk = context.Database.SqlQuery<int>(tqry).ToList();
                    Session["HANDLING_CHRG_UPD"] = "N";

                    
                    if (zhandupdrolechk.Count > 0)
                    {
                        if (zhandupdrolechk[0] > 0)
                        {
                            Session["HANDLING_CHRG_UPD"] = "Y";
                        }
                            
                    }

                    Session["ImportDBRoleChk"] = "N";
                    
                    var tqry1 = "select count('*') as 'hndrlcnt' from AspNetUsers a(nolock) ,  AspNetUserRoles b(nolock), AspNetRoles c(nolock)" +
                        " where a.id = b.UserId " +
                        " and b.RoleId = c.id " +
                        " and a.UserName = '" + model.UserName + "' and c.Name = 'ImportDashboardIndex'";
                    var zhandupdrolechk1 = context.Database.SqlQuery<int>(tqry1).ToList();

                    if (zhandupdrolechk1.Count > 0)
                    {
                        if (zhandupdrolechk1[0] > 0)
                        {
                            Session["ImportDBRoleChk"] = "Y";
                        }

                    }

                    Session["ExportDBRoleChk"] = "N";

                    var tqry2 = "select count('*') as 'hndrlcnt' from AspNetUsers a(nolock) ,  AspNetUserRoles b(nolock), AspNetRoles c(nolock)" +
                        " where a.id = b.UserId " +
                        " and b.RoleId = c.id " +
                        " and a.UserName = '" + model.UserName + "' and c.Name = 'ExportDashboardIndex'";
                    var zhandupdrolechk2 = context.Database.SqlQuery<int>(tqry2).ToList();

                    if (zhandupdrolechk2.Count > 0)
                    {
                        if (zhandupdrolechk2[0] > 0)
                        {
                            Session["ExportDBRoleChk"] = "Y";
                        }

                    }

                    var tqry3 = "select count('*') as 'hndrlcnt' from AspNetUsers a(nolock) ,  AspNetUserRoles b(nolock), AspNetRoles c(nolock)" +
                       " where a.id = b.UserId " +
                       " and b.RoleId = c.id " +
                       " and a.UserName = '" + model.UserName + "' and c.Name = 'ShippingBillUpdate'";
                    var zhandupdrolechk3 = context.Database.SqlQuery<int>(tqry3).ToList();

                    if (zhandupdrolechk3.Count > 0)
                    {
                        if (zhandupdrolechk3[0] > 0)
                        {
                            Session["ShippingBillUpdate"] = "Y";
                        }
                    }
                    else
                    {
                        Session["ShippingBillUpdate"] = "N";
                    }

                    return RedirectToLocal(returnUrl);
                    //return RedirectToAction("Index", "Home");
                }
                else { ModelState.AddModelError("", "Invalid username or password."); }




                //var servrdetl = context.Database.SqlQuery<ServerDetail>("select * from ServerDetail").ToList();
                //if (servrdetl.Count > 0)
                //{
                //    Session["GDNAME"] = servrdetl[0].DNAME; Session["GDBNAME"] = servrdetl[0].DBNAME; Session["GUNAME"] = servrdetl[0].UNAME; Session["GUPASS"] = servrdetl[0].UPASS;
                //}
                //if (user != null)
                //{

                //    context.Database.ExecuteSqlCommand("delete from menurolemaster where Roles='" + model.UserName + "'");
                //    context.Database.ExecuteSqlCommand("EXEC pr_USER_MENU_DETAIL_ASSGN @PKUSRID='" + model.UserName + "'");
                //    await SignInAsync(user, model.RememberMe);
                //    return RedirectToLocal(returnUrl);
                //}

                //ModelState.AddModelError("", "Invalid username or password.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            Session["AppStart"] = 1;
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPassword(string username)
        {
            var user = UserManager.FindByName(username);
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        public enum ResetPasswordMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            Session["CUSRID"] = null;
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }


        //[Authorize(Roles = "SuperAdmin, Admin, CanEditUser")]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "SuperAdmin, Admin, CanEditUser")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

            if (ModelState.IsValid)
            {
                var user = model.GetUser();
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Account");
                }

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //[Authorize(Roles = "SuperAdmin, Admin, CanEditUser")]
        public ActionResult UserGroups(string id)
        {
            var user = _db.Users.First(u => u.UserName == id);
            var model = new SelectUserGroupsViewModel(user);
            return View(model);
        }


        [HttpPost]
        //[Authorize(Roles = "SuperAdmin, Admin, CanEditUser")]
        //[ValidateAntiForgeryToken]
        public ActionResult UserGroups(SelectUserGroupsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var user = _db.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserGroups(user.Id);
                foreach (var group in model.Groups)
                {
                    if (group.Selected)
                    {
                        idManager.AddUserToGroup(user.Id, group.GroupId);
                    }
                }
                return RedirectToAction("index");
            }
            return View();
        }


        //[Authorize(Roles = "SuperAdmin, Admin, CanEditRole, CanEditGroup, User")]
        public ActionResult UserPermissions(string id)
        {
            var user = _db.Users.First(u => u.UserName == id);
            var model = new UserPermissionsViewModel(user);
            return View(model);
        }


        //[Authorize(Roles = "SuperAdmin, Admin, CanEditUser")]        
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "SuperAdmin, Admin, CanEditUser")]       
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }

                //if (ModelState.IsValid)
                //{
                //    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                //    if (result.Succeeded)
                //    {
                //        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                //    }
                //    else
                //    {
                //        AddErrors(result);
                //    }
                //}
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ResetPassword(ResetPasswordMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ResetPasswordMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ResetPasswordMessageId.SetPasswordSuccess ? "Your password has been reset successfully."
                : message == ResetPasswordMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ResetPasswordMessageId.Error ? "An error has occurred."
                : "";
            var username = Request.Url.Segments.Last();// Request.QueryString["id"];
            var user = UserManager.FindByName(username);
            ViewBag.HasLocalPassword = HasPassword(username);
            ViewBag.ReturnUrl = Url.Action("ResetPassword");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin, SuperAdmin, CanEditUser")]
        public async Task<ActionResult> ResetPassword(ResetPasswordUserViewModel model)
        {

            // string token = await UserManager.GeneratePasswordResetTokenAsync(user);
            //    return await userManager.ResetPasswordAsync(user, token, password);
            if (ModelState.IsValid)
            {
                var username = Request.Url.Segments.Last();// Request.QueryString["id"];

                var user = UserManager.FindByName(username);

                await UserManager.RemovePasswordAsync(user.Id);
                model.NPassword = model.NewPassword;
                await UserManager.AddPasswordAsync(user.Id, model.NewPassword);
                return RedirectToAction("ResetPassword", new { Message = ResetPasswordMessageId.SetPasswordSuccess });
            }
            return View(model);
        }

        //[Authorize(Roles = "SuperAdmin, Admin, CanEditGroup, CanEditUser")]       
        public ActionResult Index()
        {
            var users = _db.Users;
            var model = new List<EditUserViewModel>();
            foreach (var user in users)
            {
                var u = new EditUserViewModel(user);
                model.Add(u);
            }
            return View(model);
        }

        //[Authorize(Roles = "Admin, CanEditUser")]
        public ActionResult Edit(string id, ManageMessageId? Message = null)
        {
            var user = _db.Users.First(u => u.UserName == id);            
            
            var model = new EditUserViewModel(user);
            
            ViewBag.MessageId = Message;
            return View(model);
        }

        [HttpPost]
        //[Authorize(Roles = "SuperAdmin, Admin, CanEditUser")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var user = _db.Users.First(u => u.UserName == model.UserName);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                _db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //[Authorize(Roles = "SuperAdmin, Admin, CanEditUser")]
        public ActionResult Delete(string id, ManageMessageId? Message = null)
        {
            var user = _db.Users.First(u => u.UserName == id);
            var model = new EditUserViewModel(user);
            ViewBag.MessageId = Message;
            return View(model);
        }
       
        //[Authorize(Roles = "SuperAdmin, Admin, CanEditUser")]
        public ActionResult Delete(string id = null)
        {
            var user = _db.Users.First(u => u.UserName == id);
            var model = new EditUserViewModel(user);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "SuperAdmin, Admin, CanEditUser")]
        public ActionResult DeleteConfirmed(string id)
        {
            var user = _db.Users.First(u => u.UserName == id);
            _db.Users.Remove(user);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }     


        
    }
}