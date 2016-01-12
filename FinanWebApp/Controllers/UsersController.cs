using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinanWebApp.Models;
using System.Threading.Tasks;


using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace FinanWebApp.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR N1, ADMINISTRADOR N2, ADMINISTRADOR N3")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public UsersController()
        {
        }

        public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Users
        public ActionResult Index()
        {
            ListUserViewModels user;

            List<ListUserViewModels> model = new List<ListUserViewModels>();

            foreach (var item in db.Users.ToList().OrderBy(item => item.UserName))
            {
                int count = 0;
                if (db.UserSessions.Where(us => us.UserId == item.Id).ToList() != null)
                {
                    count = db.UserSessions.Where(us => us.UserId == item.Id).ToList().Count;
                }
                user = new ListUserViewModels();
                user.Id = item.Id;
                user.UserName = item.UserName;
                user.Email = item.Email;
                user.AccessFailedCount = item.AccessFailedCount;
                user.AccessDate = count > 0 ?
                    db.UserSessions.Where(us => us.UserId == item.Id).ToList()[0].AccessDate : DateTime.Now;
                user.IsOnLine = count > 0 ?
                    db.UserSessions.Where(us => us.UserId == item.Id).ToList()[0].IsOnLine : false;
                user.IsOnLineImg = count > 0 ?
                    db.UserSessions.Where(us => us.UserId == item.Id).ToList()[0].IsOnLine ? "/Content/img/online.jpg" : "/Content/img/offline.jpg" : "/Content/img/offline.jpg";
                user.IpAddress = count > 0 ?
                    db.UserSessions.Where(us => us.UserId == item.Id).ToList()[0].IpAddress : string.Empty;

                model.Add(user);
            } 

            return View(model.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            List<UserSession> usersession = db.UserSessions.Where(us => us.UserId == user.Id).ToList();

            if (user == null)
            {
                return HttpNotFound();
            }

            int count = 0;
            if (usersession.Count > 0)
            {
                count = usersession.Count;
            }
            
            ListUserViewModels model = new ListUserViewModels
            {
                
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                EmailConfirmedImg = user.EmailConfirmed ? "/Content/img/ok.png" : "/Content/img/nook.png",
                PhoneNumber = user.PhoneNumber == null ? "-" : user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                PhoneNumberConfirmedImg = user.PhoneNumberConfirmed ? "/Content/img/ok.png" : "/Content/img/nook.png",
                AccessFailedCount = user.AccessFailedCount,
                AccessDate = count > 0 ?  usersession[0].AccessDate : DateTime.Now,
                IsOnLine = count > 0 ? usersession[0].IsOnLine : false,
                IsOnLineImg = count > 0 ? usersession[0].IsOnLine ? "/Content/img/online.jpg" : "/Content/img/offline.jpg" : "/Content/img/offline.jpg",
                IpAddress = count > 0 ? usersession[0].IpAddress : string.Empty
            };

            return View(model);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateUserViewModels model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    UserName = model.UserName.ToUpper(),
                    Email = model.Email
                };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                AddErrors(result, user);
            }

            return View(model);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            EditUserViewModels model = new EditUserViewModels
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Email,PhoneNumber,UserName")] EditUserViewModels model)
        {
            if (ModelState.IsValid)
            {
                User user = UserManager.FindById(model.Id);

                user.UserName = model.UserName.ToUpper();
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                
                var result = await UserManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                AddErrors(result, user);
            }
            return View(model);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            ListUserViewModels model = new ListUserViewModels
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                EmailConfirmedImg = user.EmailConfirmed ? "/Content/img/ok.png" : "/Content/img/nook.png",
                PhoneNumber = user.PhoneNumber == null ? "-" : user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                PhoneNumberConfirmedImg = user.PhoneNumberConfirmed ? "/Content/img/ok.png" : "/Content/img/nook.png",
                AccessFailedCount = user.AccessFailedCount
            };

            return View(model);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = db.Users.Find(id);
            List<UserSession> us = db.UserSessions.Where(usw => usw.UserId == id).ToList();

            if (us.Count > 0)
            {
                foreach (var item in us)
                {
                    db.UserSessions.Remove(item);
                }
            }

            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void AddErrors(IdentityResult result, User user)
        {
            foreach (var error in result.Errors)
            {
                var name = error.IndexOf("Name"); 
                var email = error.IndexOf("Email");
                var err = string.Empty;

                if (name > -1)
                {
                    err = "El usuario " + user.UserName + " ya está en uso";
                }

                if (email > -1)
                {
                    err = "El correo electrónico " + user.Email + " ya está en uso";
                }

                ModelState.AddModelError("", err);
            }
        }
    }
}
