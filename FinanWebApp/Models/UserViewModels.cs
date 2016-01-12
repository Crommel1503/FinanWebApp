using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace FinanWebApp.Models
{
    public class ListUserViewModels
    {
        public string Id { get; set; }

        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Display(Name = "Confirmado")]
        public bool EmailConfirmed { get; set; }
        [DataType(DataType.ImageUrl)]
        public string EmailConfirmedImg { get; set; }

        [Phone]
        [Display(Name = "Teléfono")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Confirmado")]
        public bool PhoneNumberConfirmed { get; set; }
        [DataType(DataType.ImageUrl)]
        public string PhoneNumberConfirmedImg { get; set; }

        [Display(Name = "Intentos")]
        public int AccessFailedCount { get; set; }

        [Display(Name = "Fecha de Acceso")]
        [DataType(DataType.DateTime)]
        public System.DateTime AccessDate { get; set; }

        [Display(Name = "En línea")]
        public bool IsOnLine { get; set; }
        [DataType(DataType.ImageUrl)]
        public string IsOnLineImg { get; set; }

        [Display(Name = "Dirección IP")]
        public string IpAddress { get; set; }
    }

    public class CreateUserViewModels
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        public string UserName
        { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres de longitud.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }
    }

    public class EditUserViewModels
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Teléfono")]
        public string PhoneNumber { get; set; }
    }
}