using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinanWebApp.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FinanWebApp.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR N1")]
    public class RolesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Roles
        public ActionResult Index(RolesMessageId? message)
        {
            ViewBag.StatusMessage =
                message == RolesMessageId.CreateRoleFail ? "No se pudo crear el rol, ya existe." :
                message == RolesMessageId.EditRoleFail ? "No se pudo editar el rol, ya existe." :
                "";

            List<ListRoleViewModel> model = new List<ListRoleViewModel>();

            foreach(var item in db.IdentityRoles.ToList().OrderBy(item => item.Name))
            {
                ListRoleViewModel role = new ListRoleViewModel
                {
                    Id = item.Id,
                    Name = item.Name
                };
                model.Add(role);
            }

            return View(model.ToList());
        }

        // GET: Roles/Details/5
        public ActionResult Details(string id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roles roles = db.IdentityRoles.Find(id);
            if (roles == null)
            {
                return HttpNotFound();
            }
            ListRoleViewModel model = new ListRoleViewModel
            {
                Id = roles.Id,
                Name = roles.Name
            };

            return View(model);
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] CreateRoleViewModel rolesC)
        {
            if (ModelState.IsValid)
            {
                bool result = true;
                foreach (var item in db.Roles.ToList())
                {
                    if(rolesC.Name == item.Name)
                    {
                        result = false;
                        break;
                    }
                }

                if (result)
                {
                    Roles roles = new Roles
                    {
                        Name = rolesC.Name.ToUpper()
                    };
                    db.IdentityRoles.Add(roles);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index", new { Message = RolesMessageId.CreateRoleFail });
                }
                
            }

            return View(rolesC);
        }

        // GET: Roles/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roles roles = db.IdentityRoles.Find(id);
            if (roles == null)
            {
                return HttpNotFound();
            }

            EditRoleViewModel model = new EditRoleViewModel
            {
                Id = roles.Id,
                Name = roles.Name
            };
            
            return View(model);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Roles roles)
        {
            if (ModelState.IsValid)
            {
                
                bool result = true;
                foreach (var item in db.Roles.ToList())
                {
                    if (roles.Name == item.Name)
                    {
                        result = false;
                        break;
                    }
                }

                db = new ApplicationDbContext();
                if (result)
                {
                    db.Entry(roles).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index", new { Message = RolesMessageId.EditRoleFail });
                }

            }
            return View(roles);
        }

        // GET: Roles/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roles roles = db.IdentityRoles.Find(id);
            if (roles == null)
            {
                return HttpNotFound();
            }
            ListRoleViewModel model = new ListRoleViewModel
            {
                Id = roles.Id,
                Name = roles.Name
            };
            
            return View(model);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Roles roles = db.IdentityRoles.Find(id);
            db.IdentityRoles.Remove(roles);
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

        public enum RolesMessageId
        {
            CreateRoleFail,
            EditRoleFail
        }
    }
}
