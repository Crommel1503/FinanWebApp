using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FinanWebApp.Models;

namespace FinanWebApp.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR N1, ADMINISTRADOR N2")]
    public class UserRolesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: UserRoles
        public ActionResult Index(UserRolesMessageId? message)
        {
            ViewBag.StatusMessage =
                message == UserRolesMessageId.CreateUserRoleFail ? "No se pudo asignar el rol, el usuario ya tiene un rol asignado." :
                message == UserRolesMessageId.CreateRoleFail ? "No se pudo asignar el rol, debe crear al menos un rol para asignar." :
                "";

            List<ListUserRoleViewModel> list = new List<ListUserRoleViewModel>();

            foreach (var item in db.IdentityUserRoles.ToList())
            {
                ListUserRoleViewModel model = new ListUserRoleViewModel
                {
                    UserId = item.UserId,
                    UserName = db.Users.Find(item.UserId).UserName,
                    RoleId = item.RoleId,
                    Name = db.Roles.Find(item.RoleId).Name
                };                
                list.Add(model);
            }
            
            return View(list.ToList().OrderBy(item => item.UserName));
        }

        // GET: UserRoles/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRoles userRoles = db.IdentityUserRoles.Find(id);
            if (userRoles == null)
            {
                return HttpNotFound();
            }
            return View(userRoles);
        }
        
        // GET: UserRoles/Create
        public ActionResult Create()
        {
            var model = new CreateUserRoleViewModel();

            model.userList = db.Users.ToList().OrderBy(item => item.UserName).Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.UserName
            });

            model.rolesList = db.Roles.ToList().OrderBy(item => item.Name).Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Name
            });

            return View(model);
        }

        // POST: UserRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,RoleId")] CreateUserRoleViewModel cUserRoles)
        {
            if (ModelState.IsValid)
            {
                if (cUserRoles.RoleId == null)
                {
                    return RedirectToAction("Index", new { Message = UserRolesMessageId.CreateRoleFail });
                }

                UserRoles ur = null;

                foreach (var item in db.IdentityUserRoles.ToList())
                {
                    if (cUserRoles.UserId == item.UserId)
                    {
                        ur = new UserRoles
                        {
                            UserId = cUserRoles.UserId,
                            RoleId = cUserRoles.RoleId
                        };
                        break;
                    }
                }

                if (ur == null)
                {
                    ur = new UserRoles
                    {
                        UserId = cUserRoles.UserId,
                        RoleId = cUserRoles.RoleId
                    };

                    db.IdentityUserRoles.Add(ur);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index", new { Message = UserRolesMessageId.CreateUserRoleFail });
            }

            return View(cUserRoles);
        }

        // GET: UserRoles/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRoles userRoles = db.IdentityUserRoles.Find(id);

            if (userRoles == null)
            {
                return HttpNotFound();
            }
            return View();
        }

        // POST: UserRoles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,RoleId")] UserRoles userRoles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userRoles).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userRoles);
        }

        // GET: UserRoles/Delete/5
        public ActionResult Delete(string uid, string rid)
        {
            
            if (uid == null || rid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRoles userRoles = db.IdentityUserRoles.Find(uid, rid);
            if (userRoles == null)
            {
                return HttpNotFound();
            }
            ListUserRoleViewModel model = new ListUserRoleViewModel
            {
                UserId = userRoles.UserId,
                UserName = db.Users.Find(userRoles.UserId).UserName,
                RoleId = userRoles.RoleId,
                Name = db.IdentityRoles.Find(userRoles.RoleId).Name
            };

            return View(model);
        }

        // POST: UserRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string uid, string rid)
        {
            UserRoles userRoles = db.IdentityUserRoles.Find(uid, rid);
            db.IdentityUserRoles.Remove(userRoles);
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

        public enum UserRolesMessageId
        {
            CreateUserRoleFail,
            CreateRoleFail
        }
    }
}
