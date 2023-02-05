using Form.ViewModels;
using Form.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Form.Controllers
{
    public class AccountController : Controller
    {
        #region AppUser
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        #endregion

        #region Register
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }
            if (registerVM.Password != registerVM.ConfirmPassword)
            {
                ModelState.AddModelError("", "Password and Confirm Password do not match.");
                return View(registerVM);
            }
            AppUser appUser = new AppUser
            {
                UserName = registerVM.Username,
                Name = registerVM.Name,
                Surname = registerVM.Surname,
                Email = registerVM.Email
            };
            if (!IsValidUsername(appUser.UserName))
            {
                ModelState.AddModelError("", "Username is invalid, can only contain letters or digits.");
                return View(registerVM);
            }
            IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerVM.Password);
            bool IsValidUsername(string username)
            {
                return Regex.IsMatch(username, @"^[a-zA-Z0-9]+$");
            }
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(registerVM);
            }
            await _signInManager.SignInAsync(appUser, true);
            return RedirectToAction("Index", "Home");


        }
        #endregion

        #region Login
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM) 
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            AppUser appUser = await _userManager.FindByEmailAsync(loginVM.Email);

            if (appUser == null)
            {
                ModelState.AddModelError("", "The email or password you entered is incorrect");
                return View();
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, true, true);
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "You have been blocked for 1 minute because of several typos.");
                return View();
            }
            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "The email or password you entered is incorrect!");
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

    }
}
