using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecureShop.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureShop.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        
        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Visa lista över användare
        public async Task<IActionResult> UserList()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new Dictionary<string, IList<string>>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles;

            }
            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        // Visa och hantera roller
        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ManageRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = await _userManager.GetRolesAsync(user)
            };

            ViewBag.AllRoles = _roleManager.Roles;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoles(ManageRolesViewModel model, string[] roles)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = roles.Except(userRoles);
            var rolesToRemove = userRoles.Except(roles).ToList();

            await _userManager.AddToRolesAsync(user, rolesToAdd);
            
            foreach (var role in rolesToRemove)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }


            return RedirectToAction("UserList");
        }
    }
}