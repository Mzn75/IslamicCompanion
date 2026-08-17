using IslamicCompanion.Models;
using IslamicCompanion.Services;
using IslamicCompanion.Data; // Required for ApplicationDbContext
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IslamicCompanion.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly AppAuthService _authService;
        private readonly ApplicationDbContext _context; // 1. Add the database context

        // 2. Inject both the auth service and the database context
        public AccountController(AppAuthService authService, ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("Register")]
        public IActionResult Register(string username, string displayName, string password)
        {
            // 1. Create a fresh AppUser object
            var newUser = new AppUser
            {
                Username = username,
                DisplayName = displayName,
                PasswordHash = password
            };

            // 2. Try to save them to the SQL Database via the Auth Service
            bool success = _authService.RegisterUser(newUser);

            if (!success)
            {
                ViewBag.Error = "That username is already taken. Please try another one.";
                return View();
            }

            // 3. Success! Send them to the login page
            return RedirectToAction("Login");
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _authService.Authenticate(username, password);
            if (user == null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            // 1. Create the user's identity claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Name, user.DisplayName)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // 2. Set the cookie to be persistent (this makes it survive browser restarts)
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(365)
            };

            // 3. Encrypt and write the cookie to the user's browser
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            // Destroys the persistent cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpPost("FinishSession")]
        [Authorize]
        public IActionResult FinishSession()
        {
            // 1. Get the logged-in user
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var user = _authService.GetUserByUsername(username);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            // 2. Setup the dates for comparison (ignoring the exact time of day)
            DateTime today = DateTime.Now.Date;
            DateTime? lastDate = user.LastAdhkarDate?.Date;

            // 3. Always increase their total lifetime sessions
            user.TotalAdhkarSessions += 1;

            // 4. Record that they just finished a session right now
            user.LastAdhkarDate = DateTime.Now;

            // 5. Save the updated profile back to SQL Database
            _authService.UpdateUser(user);

            return Json(new { success = true });
        }

        [Authorize]
        public IActionResult Profile()
        {
            // 1. Get the currently logged-in username from the authentication cookie
            var loggedInUsername = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var todayString = DateTime.Now.ToString("yyyy-MM-dd");
            int currentScore = 0;
            int totalScore= 0;

            if (string.IsNullOrEmpty(loggedInUsername))
            {
                return RedirectToAction("Login");
            }

            // 2. Fetch the dynamic user data from your SQL service
            AppUser user = _authService.GetUserByUsername(loggedInUsername);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // 3. SQL REPLACEMENT: Count the checkmarks directly from the database!
            // Notice how much cleaner this is than reading and deserializing a file.
            currentScore = _context.Activities.Count(t =>
                t.AppUserId == user.Id &&
                t.TaskDate == todayString &&
                t.IsCompleted);

            totalScore = _context.Activities.Count(t =>
                t.AppUserId == user.Id &&
                t.IsCompleted);

            // 4. Map the real, dynamic data to the ViewModel
            var viewModel = new UserProfileViewModel
            {
                Username = user.Username,
                FullName = user.DisplayName,
                CurrentStreak = user.CurrentStreak,
                HighestStreak = user.HighestStreak,
                TotalAdhkarSessions = user.TotalAdhkarSessions,
                JoinedDate = user.JoinedDate,
                DailyTaskScore = currentScore,
                TotalDoneTasks = totalScore,
                MorningCompletionRate = 0,
                EveningCompletionRate = 0
            };

            return View(viewModel);
        }
    }
}