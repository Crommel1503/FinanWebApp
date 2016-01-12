using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FinanWebApp.Models
{
    public class ListUserRoleViewModel
    {
        [Display(Name = "Usuario")]
        public string UserId { get; set; }
        public string UserName { get; set; }

        [Display(Name = "Rol")]
        public string RoleId { get; set; }
        public string Name { get; set; }

        public IEnumerable<ListUserRoleViewModel> userRoleList { get; set; }
        
    }

    public class CreateUserRoleViewModel
    {
        [Display(Name = "Usuario")]
        public string UserId { get; set; }

        [Display(Name = "Rol")]
        public string RoleId { get; set; }

        public IEnumerable<SelectListItem> userList { get; set; }

        public IEnumerable<SelectListItem> rolesList { get; set; }
    }

    
}