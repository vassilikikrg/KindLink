using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using VolunteeringApp.Models.Identity;
using VolunteeringApp.ViewModels.Authentication;

namespace VolunteeringApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppIdentityUser> userManager; //used to manage Users in Identity
        private readonly UserManager<Citizen> citizenManager; //used to manage Citizens in Identity
        private readonly UserManager<Organization> organizationManager; //used to manage Citizens in Identity
        private readonly SignInManager<AppIdentityUser> signInManager; //used to perform the authentication procedure

        public AccountController(UserManager<AppIdentityUser> userMgr, UserManager<Citizen> citizenMgr, UserManager<Organization> organizationMgr, SignInManager<AppIdentityUser> signinMgr)
        {
            userManager = userMgr;
            citizenManager = citizenMgr;
            organizationManager = organizationMgr;
            signInManager = signinMgr;
        }

        [HttpGet("/Account/Register/Citizen")]
        public IActionResult RegisterCitizen()
        {
            // Display registration page
            return View();
        }

        [HttpPost("/Account/Register/Citizen")]
        public async Task<IActionResult> RegisterCitizen(CitizenRegisterViewModel citizen)
        {
            if (ModelState.IsValid)
            {
                var appUser = new Citizen
                {
                    UserName = citizen.UserName,
                    Email = citizen.Email,
                    Firstname = citizen.Firstname,
                    Lastname = citizen.Lastname
                };

                var result = await userManager.CreateAsync(appUser, citizen.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(appUser, "Citizen");
                    await signInManager.SignInAsync(appUser, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(citizen);
        }


        [HttpGet("/Account/Register/Organization")]
        public IActionResult RegisterOrganization()
        {
            // Display registration page
            return View();
        }

        [HttpPost("/Account/Register/Organization")]
        public async Task<IActionResult> RegisterOrganization(OrganizationRegisterViewModel organization)
        {
            if (ModelState.IsValid)
            {
                Organization appUser = new Organization
                {
                    UserName = organization.Name,
                    Email = organization.Email,
                    OfficialName = organization.OfficialName,
                    OrganizationType = organization.OrganizationType,
                    Phone = organization.Phone,
                    Website = organization.Website
                };

                IdentityResult resultCreation = await organizationManager.CreateAsync(appUser, organization.Password);
                IdentityResult resultRole = await userManager.AddToRoleAsync(appUser, "Organization");
                if (resultCreation.Succeeded && resultRole.Succeeded)
                    return RedirectToAction("Login");
            }
            else
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        ModelState.AddModelError("", error.ErrorMessage);
                    }
                }
            }
            return View(organization);
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

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
