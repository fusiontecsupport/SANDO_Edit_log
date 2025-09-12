using scfs_erp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.DAL
{
    public class CustomMembershipProvider
    {
        readonly DataContext context = new DataContext();


        public bool CreateUser(string username, string password, string email)
        {

            try
            {
                User NewUser = new User
                {
                    Username = username,
                    Password = password,
                    Email = email,
                    CreateDate = DateTime.UtcNow,
                };

                context.Users.Add(NewUser);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public  bool ValidateUser(string username, string password)
        {
            User User = null;
            User = context.Users.FirstOrDefault(Usr => Usr.Username == username && Usr.Password == password);

            if (User != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public  bool DeleteUser(string username)
        {
            User User = null;
            User = context.Users.FirstOrDefault(Usr => Usr.Username == username);
            if (User != null)
            {
                context.Users.Remove(User);
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}