using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FinanWebApp.Models
{
    public class Menus
    {
        public int Id { get; set; }
        [Display(Name = "Padre")]
        public int ParentId { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(Name = "Acción")]
        public string ActionName { get; set; }

        [Display(Name = "Controlador")]
        public string ControllerName { get; set; }

        [Display(Name = "Título")]
        public string Title { get; set; }

        [Display(Name = "Rol")]
        public string RoleName { get; set; }

        [Display(Name = "Estatus")]
        public string Status { get; set; }

    }

    public class IndexMenuViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Padre")]
        public int ParentId { get; set; }
        public string ParentName { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(Name = "Acción")]
        public string ActionName { get; set; }

        [Display(Name = "Controlador")]
        public string ControllerName { get; set; }

        [Display(Name = "Título")]
        public string Title { get; set; }

        [Display(Name = "Rol")]
        public string RoleName { get; set; }

        [Display(Name = "Estatus")]
        public string Status { get; set; }
        public string StatusName { get; set; }
    }

    public class CreateMenuViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Padre")]
        public int ParentId { get; set; }

        public IEnumerable<SelectListItem> ParentIdList { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(Name = "Acción")]
        public string ActionName { get; set; }

        public IEnumerable<SelectListItem> ActionNameList { get; set; }

        [Display(Name = "Controlador")]
        public string ControllerName { get; set; }

        [Display(Name = "Título")]
        public string Title { get; set; }

        [Display(Name = "Rol")]
        public string[] RolesName { get; set; }

        public MultiSelectList RoleList { get; set; }

        [Display(Name = "Estatus")]
        public string Status { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }

    }

    public class EditMenuViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Padre")]
        public int ParentId { get; set; }
        public string ParentName { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(Name = "Acción")]
        public string ActionName { get; set; }

        [Display(Name = "Controlador")]
        public string ControllerName { get; set; }

        [Display(Name = "Título")]
        public string Title { get; set; }

        [Display(Name = "Rol")]
        public string[] RolesName { get; set; }

        public MultiSelectList RoleList { get; set; }

        [Display(Name = "Estatus")]
        public string Status { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }
    }
}