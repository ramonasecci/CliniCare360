using CliniCare360.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CliniCare360.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        DBContext db = new DBContext();
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Users user)
        {
            var loggedUser = db.Users.Where(u => u.Email == user.Email && u.Password == user.Password).FirstOrDefault();

            if (loggedUser == null)
            {
                TempData["ErrorLogin"] = true;
                return RedirectToAction("Login");
            }

            FormsAuthentication.SetAuthCookie(loggedUser.UserId.ToString(), true);
            //verifico il ruolo dell'utente loggato, se admin lo ridireziono nella home, altrimenti sarà un utente e lo ridirezioni al suo profilo
         
            if(loggedUser.Ruolo == "admin")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Profilo", "Users");
            }            
        }



        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}