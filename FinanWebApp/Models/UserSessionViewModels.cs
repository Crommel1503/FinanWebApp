using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinanWebApp.Models
{
    public class UserSession
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Fecha de Acceso")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime AccessDate { get; set; }

        [Display(Name = "En línea")]
        public bool IsOnLine { get; set; }

        [Display(Name = "Dirección IP")]
        public string IpAddress { get; set; }
    }
}