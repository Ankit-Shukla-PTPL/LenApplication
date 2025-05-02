using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication12.Models;

namespace WebApplication12.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly RoleManager<ApplicationRole> _roleManager;
		public AccountController(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			RoleManager<ApplicationRole> roleManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}


		public IActionResult Index()
		{
			return View();
		}


		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				// Copy data from RegisterViewModel to IdentityUser
				var user = new ApplicationUser
				{
					UserName = model.Email,
					Email = model.Email
				};
				// Store user data in AspNetUsers database table
				var result = await _userManager.CreateAsync(user, model.Password);
				// If user is successfully created, sign-in the user using
				// SignInManager and redirect to index action of HomeController
				if (result.Succeeded)
				{
					await _signInManager.SignInAsync(user, isPersistent: false);
					return RedirectToAction("index", "home");
				}
				// If there are any errors, add them to the ModelState object
				// which will be displayed by the validation summary tag helper
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			return View(model);
		}

		[AllowAnonymous]
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginViewModel model, string? ReturnUrl)
		{
			if (ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

				if (result.Succeeded)
				{
					// Check if the ReturnUrl is not null and is a local URL
					if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
					{
						return Redirect(ReturnUrl);
					}
					else
					{
						// Redirect to default page
						return RedirectToAction(nameof(HomeController.Index), "Home");
					}

					// Handle successful login
					return RedirectToAction(nameof(HomeController.Index), "Home");
				}
				if (result.RequiresTwoFactor)
				{
					// Handle two-factor authentication case
				}
				if (result.IsLockedOut)
				{
					// Handle lockout scenario
				}
				else
				{
					// Handle failure
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return View(model);
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("index", "home");
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult AccessDenied()
		{
			return View();
		}



	}
}
