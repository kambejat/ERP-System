using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Erp.Data;
using Erp.Models;

namespace Erp.Controllers
{
    [Route("users")]
    public class UsersController : Controller
    {
        private readonly ERPDbContext _context;

        public UsersController(ERPDbContext context)
        {
            _context = context;
        }

        #region Authentication

        // GET: Users/Login
        [HttpGet("login")]
        public IActionResult Login()
        {
            ViewData["Title"] = "Login";
            return View();
        }

        // POST: Users/Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(User loginUser)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.username == loginUser.username);

            if (user == null || user.password_hash != loginUser.password_hash)
            {
                ViewData["ErrorMessage"] = "Invalid username or password.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.username),
                new Claim(ClaimTypes.Role, user.role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            TempData["SuccessMessage"] = "You have successfully logged in.";
            return RedirectToAction("Index", "Dashboard");
        }

        // POST: Users/Logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "You have successfully logged out.";
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region User Management

        // GET: Users/Create
        [HttpGet("create")]
        public IActionResult Create()
        {
            var model = new User();
            return View(model);
        }

        // POST: Users/Create
        [HttpPost("create")]
        public async Task<IActionResult> Create(User newUser)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.username == newUser.username);

                if (existingUser != null)
                {
                    ViewData["ErrorMessage"] = "Username already exists.";
                    return View(newUser);
                }

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(newUser);
        }

        // GET: Users/Details/{id}
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // GET: Users/Edit/{id}
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/{id}
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, User updatedUser)
        {
            if (id != updatedUser.user_id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _context.Entry(updatedUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = updatedUser.user_id });
            }

            return View(updatedUser);
        }

        // GET: Users/Delete/{id}
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/{id}
        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        #endregion

        #region User Search & Index

        // GET: Users/Manage
        [HttpGet("manage")]
        public async Task<IActionResult> Manage(string search)
        {
            var usersQuery = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                usersQuery = usersQuery.Where(u =>
                    u.username.Contains(search) || u.role.Contains(search));
            }

            var users = await usersQuery.ToListAsync();
            if (users == null)
            {
                users = new List<User>();
            }

            return View("Index", users);
        }

        // GET: Users/Index
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            if (users == null || !users.Any())
            {
                TempData["ErrorMessage"] = "No users found.";
            }

            return View(users);
        }

        #endregion
    }
}
