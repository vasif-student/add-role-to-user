using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_proj.Areas.Admin.ViewModels;
using MVC_proj.DAL;
using MVC_proj.Data;
using MVC_proj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_proj.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {

        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(AppDbContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _dbContext.Users.ToListAsync();
            var userList = new List<UserViewModel>();

            foreach (var user in users)
            {
                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = (await _userManager.GetRolesAsync(user))[0]
                });
            }

            return View(userList);
        }

        //***** Add Role *****//

        public async Task<IActionResult> AddRole(string id)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if(user == null)
            {
                return NotFound();
            }

            List<string> roles = new List<string>() { RoleConstants.Admin, RoleConstants.Moderator, RoleConstants.User };

            return View(roles.ToList());

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(string id, string role)
        {
            var user = await _dbContext.Users.FindAsync(id);

            await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction(nameof(Index));

        }
    }
}
