using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace _671Books.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /Account/Register


        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(FormCollection formCollection)
        {

            // Attempt to register the user
            try
            {
                using (BookStoreEntities book = new BookStoreEntities())
                {
                    string error = "";
                    var username = formCollection["UserName"];
                    var password = formCollection["Password"];
                    var phone = formCollection["Phone"];
                    var name = formCollection["Name"];
                    var address = formCollection["Address"];
                    var confirmpassword = formCollection["ConfirmPassword"];
                    if (string.IsNullOrEmpty(username))
                    {
                        error = error + "Please enter Username." + "<br>";
                    }
                    if (string.IsNullOrEmpty(password))
                    {
                        error = error + "Please enter Password." + "<br>";
                    }
                    if (string.Compare(password, confirmpassword) != 0)
                    {
                        error = error + "Password's did not match." + "<br>";
                    }
                    if (string.IsNullOrEmpty(phone))
                    {
                        error = error + "Please enter Phone." + "<br>";
                    }
                    if (string.IsNullOrEmpty(address))
                    {
                        error = error + "Please enter Address." + "<br>";
                    }
                    if (string.IsNullOrEmpty(address))
                    {
                        error = error + "Please enter Address." + "<br>";
                    }
                    if (string.IsNullOrEmpty(error))
                    {
                        var person = book.people.Where(x => x.username.ToLower() == username.ToLower()).ToList();
                        if (person.Count == 0)
                        {
                            var id = book.people.Max(b => b.cid);
                            book.people.Add(new person {cid = id+1, username = username, password = password, phone = Convert.ToInt64(phone), name = name, address = address, admin_status = "no" });
                            book.SaveChanges();
                        }
                        else
                        {
                            ViewBag.ExistingUser = "User already exists. Please select another user name";
                            return View();
                        }
                        return RedirectToAction("Login", "User");
                    }
                    else
                    {
                        ViewBag.SummaryDiv = error;
                        return View();
                    }

                }
            }
            catch (Exception ex)
            {
                
                //return to error page
            }
          

            // If we got this far, something failed, redisplay form
            return View();
        }

        public ActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection formCollection)
        {
            var username = formCollection["UserName"];
            var password = formCollection["Password"];
            
            
            string existingPassword = "";
            string adminStatus = "no";
            using(BookStoreEntities book = new BookStoreEntities())
            {
                var person = book.people.Where(x => x.username.ToLower() == username.ToLower()).ToList();
                if (person.Count == 0)
                {
                    ViewBag.InvalidUser = "Either the username or password are incorrect. Please enter correct username or password.";
                    
                    return View();
                }
                else
                {
                    existingPassword = person.SingleOrDefault(p => p.username.ToLower() == username.ToLower()).password;
                    if (string.Compare(existingPassword,password) == 0)
                    {
                        HttpCookie myCookie = new HttpCookie("UserSettings");
                        myCookie["UserID"] = person.SingleOrDefault().cid.ToString();
                        myCookie["UserName"] = person.SingleOrDefault().username.ToString();
                        
                        Response.Cookies.Add(myCookie);
                        
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        
                        ViewBag.InvalidUser = "Either the username or password are incorrect. Please enter correct username or password.";
                        return View();
                    }
                }
                
            }
            return View();
        }

        public ActionResult LogOff()
        {
            if (Request.Cookies["UserSettings"] != null)
            {
                HttpCookie myCookie = new HttpCookie("UserSettings");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);
            }
            return RedirectToAction("Login","User");
        }
    }
}
