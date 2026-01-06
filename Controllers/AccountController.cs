using HMSSystem.Data;
using HMSSystem.Models.ViewModels;
using HMSSystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using HMSSystem.Service;



namespace HMSSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;



        public AccountController(ApplicationDbContext context, 
            IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", GetControllerName());
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            if (user.IsBlocked)
            {
                ModelState.AddModelError("", "Your account has been blocked. Contact administrator.");
                return View(model);
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = model.RememberMe }
            );

            if (user.Role == "Admin")
                return RedirectToAction("Dashboard", "Admin");
            else
                return RedirectToAction("Dashboard", "User");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", GetControllerName());
            }
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                MobileNumber = model.MobileNumber,
                Role = "User",
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private string GetControllerName()
        {
            return User.IsInRole("Admin") ? "Admin" : "User";
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

        //    // SECURITY: Do not expose whether email exists
        //    if (user == null || !user.IsActive || user.IsBlocked)
        //    {
        //        TempData["Success"] = "If the email exists, a reset link has been sent.";
        //        return RedirectToAction("Login");
        //    }

        //    // 1️⃣ Generate token
        //    user.ResetToken = Guid.NewGuid().ToString();
        //    user.ResetTokenExpiry = DateTime.Now.AddMinutes(30);
        //    user.ModifiedDate = DateTime.Now;

        //    await _context.SaveChangesAsync();

        //    // 2️⃣ Generate reset URL
        //    var resetLink = Url.Action(
        //        "ResetPassword",
        //        "Account",
        //        new { token = user.ResetToken },
        //        Request.Scheme
        //    );

        //    // 3️⃣ Send email (TEMP: console)
        //    Console.WriteLine("RESET LINK: " + resetLink);

        //    // 👉 When SMTP is enabled, replace Console.WriteLine with EmailService

        //    TempData["Success"] = "Password reset link has been sent to your email.";
        //    return RedirectToAction("Login");
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            // SECURITY: Do not reveal email existence
            if (user == null || !user.IsActive || user.IsBlocked)
            {
                TempData["Success"] = "If the email exists, a reset link has been sent.";
                return RedirectToAction("Login");
            }

            // 1️⃣ Generate token
            user.ResetToken = Guid.NewGuid().ToString();
            user.ResetTokenExpiry = DateTime.Now.AddMinutes(30);
            user.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            // 2️⃣ Generate dynamic reset link
            var resetLink = Url.Action(
                "ResetPassword",
                "Account",
                new { token = user.ResetToken },
                Request.Scheme
            );

            // 3️⃣ Email body
            string body = $@"
        <p>Hello <b>{user.FullName}</b>,</p>
        <p>You requested to reset your password.</p>
        <p>
            <a href='{resetLink}'
               style='background:#0d6efd;
                      color:#fff;
                      padding:10px 15px;
                      text-decoration:none;
                      border-radius:5px;'>
               Reset Password
            </a>
        </p>
        <p>This link expires in 30 minutes.</p>
        <p>If you didn’t request this, ignore this email.</p>
        <br/>
        <b>Appointment Management System</b>";

            // 4️⃣ Send email
            await _emailService.SendEmailAsync(
                user.Email,
                "Reset Your Password",
                body
            );

            TempData["Success"] = "Password reset link has been sent to your email.";
            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest();

            return View(new ResetPasswordViewModel { Token = token });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u =>
                u.ResetToken == model.Token &&
                u.ResetTokenExpiry != null &&
                u.ResetTokenExpiry > DateTime.Now);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid or expired reset token");
                return View(model);
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;
            user.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Password reset successful. Please login.";
            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);

            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
            {
                ModelState.AddModelError("", "Current password is incorrect");
                return View(model);
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            user.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Password changed successfully";
            return RedirectToAction("Dashboard", GetControllerName());
        }





    }

}