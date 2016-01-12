using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using FinanWebApp.Models;

namespace FinanWebApp.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Su contraseña ha cambiado."
                : message == ManageMessageId.SetPasswordSuccess ? "Su contraseña ha sido configurada."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Su proveedor de autenticación ha sido configurado."
                : message == ManageMessageId.Error ? "Un error ha ocurrido."
                : message == ManageMessageId.AddPhoneSuccess ? "Su número teléfonico ha sido agregado."
                : message == ManageMessageId.RemovePhoneSuccess ? "Su número teléfonico ha sido eliminado."
                : "";
            List<Menus> defMenus = this.createDefMenus().Where(w => w.Status == "A").ToList();

            List<Menus> menus = db.Menus.Where(w => w.Status == "A").ToList();

            List<Menus> grantMenus = new List<Menus>();

            var userId = User.Identity.GetUserId();
            var userRole = UserManager.GetRoles(userId).Count == 0 ? "" : UserManager.GetRoles(userId)[0];

            foreach (var item in defMenus)
            {
                if (item.RoleName == null || item.RoleName == string.Empty)
                {
                    grantMenus.Add(item);
                }
                else
                {
                    string[] roles = item.RoleName.Split(',');

                    foreach (var roleItem in roles)
                    {
                        if (roleItem.Trim() == userRole)
                        {
                            grantMenus.Add(item);
                            break;
                        }
                    }
                }
            }

            foreach (var item in menus)
            {
                if (item.RoleName == null || item.RoleName == string.Empty)
                {
                    grantMenus.Add(item);
                }
                else
                {
                    string[] roles = item.RoleName.Split(',');

                    foreach (var roleItem in roles)
                    {
                        if (roleItem.Trim() == userRole)
                        {
                            grantMenus.Add(item);
                            break;
                        }
                    }
                }
            }

            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                menusList = grantMenus

            };
            return View(model);
        }
        
        // GET: /Manage/ConfigCount
        public async Task<ActionResult> ConfigCount(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Su contraseña ha cambiado."
                : message == ManageMessageId.SetPasswordSuccess ? "Su contraseña ha sido configurada."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Su proveedor de autenticación ha sido configurado."
                : message == ManageMessageId.Error ? "Un error ha ocurrido."
                : message == ManageMessageId.AddPhoneSuccess ? "Su número teléfonico ha sido agregado."
                : message == ManageMessageId.RemovePhoneSuccess ? "Su número teléfonico ha sido eliminado."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View(model);
        }


        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Su código de seguridad es: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // GET: /Manage/RemovePhoneNumber
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "El acceso externo ha sido removido."
                : message == ManageMessageId.Error ? "Un error ha ocurrido."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        private List<Menus> createDefMenus()
        {
            List<Menus> def = new List<Menus>();
            //Crea menu para la administración de la cuenta
            Menus menu = new Menus
            {
                Id = 1000,
                ParentId = 0,
                Name = "Configurar Cuenta",
                Title = "Menú de configuración la cuenta",
                Status = "A"
            };
            def.Add(menu);
            menu = new Menus
            {
                Id = 1001,
                ParentId = 1000,
                Name = "Cambiar contraseña",
                ActionName = "ChangePassword",
                ControllerName = "Manage",
                Title = "Cambia la contraseña del usuario",
                Status = "A"
            };
            def.Add(menu);
            //Crea el menu para la administración de Aplicación
            menu = new Menus
            {
                Id = 1100,
                ParentId = 0,
                Name = "Administrar Aplicación",
                Title = "Menús para la administración de la aplicación",
                RoleName = "ADMINISTRADOR N1,ADMINISTRADOR N2,ADMINISTRADOR N3",
                Status = "A"
            };
            def.Add(menu);
            menu = new Menus
            {
                Id = 1101,
                ParentId = 1100,
                Name = "Registrar Usuario",
                ActionName = "Register",
                ControllerName = "Account",
                Title = "Registra a un nuevo usuario",
                RoleName = "ADMINISTRADOR N1,ADMINISTRADOR N2,ADMINISTRADOR N3",
                Status = "A"
            };
            def.Add(menu);
            menu = new Menus
            {
                Id = 1102,
                ParentId = 1100,
                Name = "Usuarios",
                ActionName = "Index",
                ControllerName = "Users",
                Title = "Administra a los usuarios",
                RoleName = "ADMINISTRADOR N1,ADMINISTRADOR N2,ADMINISTRADOR N3",
                Status = "A"
            };
            def.Add(menu);
            menu = new Menus
            {
                Id = 1103,
                ParentId = 1100,
                Name = "Usuarios-Roles",
                ActionName = "Index",
                ControllerName = "UserRoles",
                Title = "Administra las relaciones de los usuarios y roles",
                RoleName = "ADMINISTRADOR N1,ADMINISTRADOR N2",
                Status = "A"
            };
            def.Add(menu);
            menu = new Menus
            {
                Id = 1104,
                ParentId = 1100,
                Name = "Roles",
                ActionName = "Index",
                ControllerName = "Roles",
                Title = "Administra los roles para los usuarios",
                RoleName = "ADMINISTRADOR N1",
                Status = "A"
            };
            def.Add(menu);
            menu = new Menus
            {
                Id = 1105,
                ParentId = 1100,
                Name = "Menús",
                ActionName = "Index",
                ControllerName = "Menus",
                Title = "Administra los menús",
                RoleName = "ADMINISTRADOR N1",
                Status = "A"
            };
            def.Add(menu);

            return def;
        }

        #endregion
    }
}