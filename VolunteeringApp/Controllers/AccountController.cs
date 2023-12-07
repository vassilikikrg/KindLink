using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using VolunteeringApp.Models;

namespace VolunteeringApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppIdentityUser> userManager; //used to manage Users in Identity
        private readonly SignInManager<AppIdentityUser> signInManager; //used to perform the authentication procedure

        public AccountController(UserManager<AppIdentityUser> userMgr, SignInManager<AppIdentityUser> signinMgr)
        {
            userManager = userMgr;
            signInManager = signinMgr;
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Display registration page
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                AppIdentityUser appUser = new AppIdentityUser
                {
                    UserName = user.Name,
                    Email = user.Email
                };

                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);
                //await userManager.AddToRoleAsync(appUser, "ADMIN");
                if (result.Succeeded)
                    return RedirectToAction("Login");
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            LoginViewModel login = new LoginViewModel();
            if (returnUrl != null) login.ReturnUrl = returnUrl;
            else login.ReturnUrl = "/";
            return View(login); //pass a new login model to the view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                AppIdentityUser appUser = await userManager.FindByEmailAsync(loginModel.Email); //search for user based on email
                if (appUser != null)
                {
                    await signInManager.SignOutAsync(); //sign out any already logged in user that attempts to login
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(appUser, loginModel.Password, false, false); //attempts to sign in the user based on the info provided
                    if (result.Succeeded)
                        return Redirect(loginModel.ReturnUrl ?? "/");
                }
                ModelState.AddModelError(nameof(loginModel.Email), "Login Failed: Invalid Email or password");
            }
            return View(loginModel);
        }

        //[HttpPost]
        //[Authorize]
        //public IActionResult Logout()
        //{
        //    // Perform logout logic
        //    // Redirect to home page or login page
        //}
    }
}
