using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Web;
using System;

namespace scfs_erp
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                ExpireTimeSpan = new System.TimeSpan(1, 0, 0),
                SlidingExpiration = true,
            });

            SetPageCaching();

            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication();
        }

        public static void SetPageCaching()
        {
            //Used for setting/disabling page caching
            HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(24));
            HttpContext.Current.Response.Cache.SetValidUntilExpires(true);//false
            HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            //HttpContext.Current.Response.Cache.SetNoStore(); to disable
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Public); //nocache
            TimeSpan ts = new TimeSpan(24, 0, 0);
            HttpContext.Current.Response.Cache.SetMaxAge(ts);
            //HttpContext.Current.Response.ContentType = "image/jpeg"; // specific content
            HttpContext.Current.Response.Cache.SetSlidingExpiration(true);
        }
    }
}