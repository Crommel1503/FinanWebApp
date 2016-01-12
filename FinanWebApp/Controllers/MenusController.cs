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
    [Authorize(Roles = "ADMINISTRADOR N1")]
    public class MenusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string defFather = "Sin padre";
        // GET: Menus
        public ActionResult Index()
        {
            List<IndexMenuViewModel> model = new List<IndexMenuViewModel>();
            
            foreach (var item in db.Menus.ToList())
            {
                IndexMenuViewModel imvm = new IndexMenuViewModel()
                {
                    Id = item.Id,
                    ParentId = item.ParentId,
                    ParentName = item.ParentId == 0 ? defFather : db.Menus.Find(item.ParentId) == null ? defFather : db.Menus.Find(item.ParentId).Name,
                    Name = item.Name,
                    ActionName = item.ActionName,
                    ControllerName = item.ControllerName,
                    RoleName = item.RoleName,
                    Title = item.Title,
                    Status = item.Status,
                    StatusName = item.Status == "A" ? "ACTIVO" : "INACTIVO"
                };

                model.Add(imvm);
            }

            return View(model.ToList());
        }

        // GET: Menus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menus menus = db.Menus.Find(id);

            IndexMenuViewModel model = new IndexMenuViewModel()
            {
                Id = menus.Id,
                ParentId = menus.ParentId,
                ParentName = menus.ParentId == 0 ? defFather : db.Menus.Find(menus.ParentId).Name,
                Name = menus.Name,
                ActionName = menus.ActionName,
                ControllerName = menus.ControllerName,
                RoleName = menus.RoleName,
                Title = menus.Title,
                Status = menus.Status,
                StatusName = menus.Status == "A" ? "Activo" : "INACTIVO"
            };

            if (menus == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: Menus/Create
        public ActionResult Create()
        {
            CreateMenuViewModel model = new CreateMenuViewModel();

            var parents = new List<SelectListItem>();
            parents.Add(new SelectListItem() {Text = defFather, Value = "0"});

            parents.AddRange(db.Menus.ToList().Where(w => w.ParentId == 0).OrderBy(o => o.Name).Select(s => 
            new SelectListItem { Value = s.Id.ToString(), Text = s.Name}));

            model.ParentIdList = parents;

            var action = new List<SelectListItem>();
            action.Add(new SelectListItem() { Text = "Sin Acción", Value = "" });
            action.Add(new SelectListItem() { Text = "Create", Value = "Create" });
            action.Add(new SelectListItem() { Text = "Delete", Value = "Delete" });
            action.Add(new SelectListItem() { Text = "Details", Value = "Details" });
            action.Add(new SelectListItem() { Text = "Edit", Value = "Edit" });
            action.Add(new SelectListItem() { Text = "Index", Value = "Index" });

            model.ActionNameList = action;

            var roles = db.Roles.Select(s => new {
                Value = s.Name,
                Text = s.Name
            }).ToList().OrderBy(o => o.Text);

            model.RoleList = new MultiSelectList(roles, "Value", "Text");

            var status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Activo", Value = "A" });
            status.Add(new SelectListItem() { Text = "Inactivo", Value = "I" });

            model.StatusList = status;

            return View(model);
        }

        // POST: Menus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ParentId,Name,ActionName,ControllerName,Title,RolesName,Status")] CreateMenuViewModel menusC)
        {
            if (ModelState.IsValid)
            {
                string roleName = string.Empty;
                if (menusC.RolesName != null)
                {
                    string sep = string.Empty;
                    foreach (var rn in menusC.RolesName)
                    {
                        roleName += sep + rn;
                        sep = ",";
                    }
                }
                Menus menus = new Menus()
                {
                    ParentId = menusC.ParentId,
                    Name = menusC.Name,
                    ActionName = menusC.ActionName,
                    ControllerName = menusC.ControllerName,
                    Title = menusC.Title,
                    RoleName = roleName,
                    Status = menusC.Status.Substring(0,1)
                };
                db.Menus.Add(menus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(menusC);
        }

        // GET: Menus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menus menus = db.Menus.Find(id);

            EditMenuViewModel model = new EditMenuViewModel()
            {
                Id = menus.Id,
                ParentId = menus.ParentId,
                ParentName = menus.ParentId == 0 ? defFather : db.Menus.Find(menus.ParentId).Name,
                Name = menus.Name,
                ActionName = menus.ActionName,
                Title = menus.Title,
                ControllerName = menus.ControllerName,
                RolesName = menus.RoleName.Split(','),
                Status = menus.Status
            };

            var roles = db.Roles.Select(s => new {
                Value = s.Name,
                Text = s.Name
            }).ToList().OrderBy(o => o.Text);

            model.RoleList = new MultiSelectList(roles, "Value", "Text");
            
            var status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Activo", Value = "A" });
            status.Add(new SelectListItem() { Text = "Inactivo", Value = "I" });

            model.StatusList = status;

            if (menus == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Menus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ParentId,Name,ActionName,ControllerName,Title,RolesName,Status")]EditMenuViewModel menusE)
        {
            if (ModelState.IsValid)
            {
                string roleName = string.Empty;
                if (menusE.RolesName != null)
                {
                    string sep = string.Empty;
                    foreach (var rn in menusE.RolesName)
                    {
                        roleName += sep + rn;
                        sep = ",";
                    }
                }

                Menus menus = new Menus()
                {
                    Id = menusE.Id,
                    ParentId = menusE.ParentId,
                    Name = menusE.Name,
                    ActionName = menusE.ActionName,
                    ControllerName = menusE.ControllerName,
                    Title = menusE.Title,
                    RoleName = roleName,
                    Status = menusE.Status.Substring(0, 1)
                };

                db.Entry(menus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menusE);
        }

        // GET: Menus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menus menus = db.Menus.Find(id);
            if (menus == null)
            {
                return HttpNotFound();
            }

            IndexMenuViewModel model = new IndexMenuViewModel()
            {
                Id = menus.Id,
                ParentId = menus.ParentId,
                ParentName = menus.ParentId == 0 ? defFather : db.Menus.Find(menus.ParentId).Name,
                Name = menus.Name,
                ActionName = menus.ActionName,
                ControllerName = menus.ControllerName,
                Title = menus.Title,
                RoleName = menus.RoleName,
                Status = menus.Status == "A" ? "Activo" : "Inactivo"
            };

            return View(model);
        }

        // POST: Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Menus menus = db.Menus.Find(id);
            var submenus = db.Menus.ToList().Where(s => s.ParentId == menus.Id);

            foreach (Menus item in submenus)
            {
                db.Menus.Remove(item);
            }

            db.Menus.Remove(menus);
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
