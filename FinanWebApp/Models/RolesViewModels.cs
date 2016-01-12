using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinanWebApp.Models
{
    public class ListRoleViewModel
    {
        public string Id { get; set; }
        
        [Display(Name = "Nombre")]
        public string Name { get; set; }
    }

    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Nombre")]
        public string Name { get; set; }
    }

    public class EditRoleViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string Name { get; set; }
    }
}