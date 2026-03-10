using Entities.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MCV.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManagerController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<User> signInManager;

        public ManagerController(UserManager<User> _userManager, RoleManager<IdentityRole> _roleManager, SignInManager<User> _signInManager)
        {
            userManager = _userManager;
            roleManager = _roleManager;
            signInManager = _signInManager;
        }
        public async Task<IActionResult> Index()
        {
                var users = userManager.Users.ToList();
                return View(users);
        }
        public async Task<IActionResult> UpdateRoles(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            
            if (user == null)
            {
                return NotFound();
            }
            var allRoles = roleManager.Roles.ToList();
            //get user roles as list of strings
            List<string> roles = new List<string>();
            foreach (var role in allRoles) 
            {
                roles.Add(role.Name);
            }          
            var userRoles = await userManager.GetRolesAsync(user);
            var nonUserRoles = roles.Except(userRoles).ToList();

            ViewBag.UserRoles = userRoles;
            ViewBag.NonUserRoles = nonUserRoles;
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoles(string id, List<string> RolesToAdd, List<string> RolesToRemove)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) {
                return NotFound();
            }
            await userManager.RemoveFromRolesAsync(user, RolesToRemove);
            await userManager.AddToRolesAsync(user, RolesToAdd);
            // print success message
            TempData["Success"] = "Roles updated successfully!";
            var logedInUser = await userManager.FindByEmailAsync(User.Identity.Name);
            if(id== logedInUser.Id) 
            {         
                await signInManager.SignInAsync(user, isPersistent: false);
            }

            return RedirectToAction("UpdateRoles", "Manager" ,new { id = id });
        }
    }
}
