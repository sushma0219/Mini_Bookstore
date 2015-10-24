using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Core;

namespace _671Books.Utils
{
    public class MenuSecurity 
    {
        public static bool IsAdmin(HttpCookie ID)
        {
           
            try
            {
                using(var book = new BookStoreEntities())
                {
                    if (ID == null)
                    {
                        return false;
                    }

                    var userID = ID["UserID"];
                     var person = book.people.Where(x => x.cid.ToString() == userID).ToList();
                     var isAdmin = person.SingleOrDefault().admin_status.ToString();
                     if (person.Count() == 0)
                     {
                         return false;
                     }
                     else
                     {
                         if (isAdmin.ToLower() == "yes")
                         {
                             return true;
                         }
                         else
                         {
                             return false;
                         }
                     }
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public static bool IsUser(HttpCookie ID)
        {

            try
            {
                using (var book = new BookStoreEntities())
                {
                    if (ID == null)
                    {
                        return false;
                    }
                    var userID = ID["UserID"];
                    var person = book.people.Where(x => x.cid.ToString() == userID).ToList();
                    
                    if (person.Count() == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }

    
}