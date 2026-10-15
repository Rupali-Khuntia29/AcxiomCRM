using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly SignInManager<ApplicationUser> _signIn;
    public AccountController(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn)
    {
        _users = users;
        _signIn = signIn;
    }

    public class LoginViewModel
    {
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required, DataType(DataType.Password)] public string Password { get; set; } = "";
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required, StringLength(120)] public string FullName { get; set; } = "";
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required, StringLength(100, MinimumLength = 8), DataType(DataType.Password)]
        public string Password { get; set; } = "";
        [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }

    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        if (!ModelState.IsValid) return View(model);
        var user = await _users.FindByEmailAsync(model.Email);
        if (user == null || !user.IsActive)
        {
            ModelState.AddModelError("", "Invalid login attempt or account is inactive.");
            return View(model);
        }
        var result = await _signIn.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Dashboard");
        }
        ModelState.AddModelError("", result.IsLockedOut
            ? "Your account is temporarily locked due to repeated failed attempts."
            : "Invalid login attempt.");
        return View(model);
    }

    [AllowAnonymous]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            IsActive = true
        };
        var result = await _users.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _users.AddToRoleAsync(user, "SalesExecutive");
            await _signIn.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Dashboard");
        }
        foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
        return View(model);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }
}
