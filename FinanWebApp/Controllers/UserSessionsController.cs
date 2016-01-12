using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinanWebApp.Models;

namespace FinanWebApp.Controllers
{
    public class UserSessionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserSessions
        public ActionResult Index()
        {
            return View(db.UserSessions.ToList());
        }

        // GET: UserSessions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSession userSession = db.UserSessions.Find(id);
            if (userSession == null)
            {
                return HttpNotFound();
            }
            return View(userSession);
        }

        // GET: UserSessions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserSessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,AccessDate,IsOnLine,IpAddress")] UserSession userSession)
        {
            if (ModelState.IsValid)
            {
                db.UserSessions.Add(userSession);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userSession);
        }

        // GET: UserSessions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSession userSession = db.UserSessions.Find(id);
            if (userSession == null)
            {
                return HttpNotFound();
            }
            return View(userSession);
        }

        // POST: UserSessions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,AccessDate,IsOnLine,IpAddress")] UserSession userSession)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userSession).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userSession);
        }

        // GET: UserSessions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSession userSession = db.UserSessions.Find(id);
            if (userSession == null)
            {
                return HttpNotFound();
            }
            return View(userSession);
        }

        // POST: UserSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserSession userSession = db.UserSessions.Find(id);
            db.UserSessions.Remove(userSession);
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
    }
}
