using GuessTheDate.Models;
using GuessTheDate.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuessTheDate.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministrationController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdministrationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public ViewResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole()
                {
                    Name = model.RoleName
                };

                var result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        public ViewResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<ViewResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"No user with id = {id} can be found.";
                return View("NotFound");
            }

            var model = new EditRoleViewModel()
            {
                Id = id,
                RoleName = role.Name
            };

            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByIdAsync(model.Id);
                if (role == null)
                {
                    ViewBag.ErrorMessage = $"No user with id = {model.Id} can be found.";
                    return View("NotFound");
                }

                role.Name = model.RoleName;

                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string roleId)
        {
            ViewBag.RoleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"No user with id = {roleId} can be found.";
                return View("NotFound");
            }

            List<EditUserInRoleViewModel> model = new List<EditUserInRoleViewModel>();
            foreach (var user in userManager.Users)
            {
                EditUserInRoleViewModel userInRole = new EditUserInRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userInRole.IsSelected = true;
                }
                else
                {
                    userInRole.IsSelected = false;
                }

                model.Add(userInRole);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<EditUserInRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"No user with id = {roleId} can be found.";
                return View("NotFound");
            }

            foreach (var user in model)
            {
                var userInRole = await userManager.FindByIdAsync(user.UserId);

                IdentityResult result = null;
                if (user.IsSelected && !(await userManager.IsInRoleAsync(userInRole, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(userInRole, role.Name);
                }
                else if (!user.IsSelected && (await userManager.IsInRoleAsync(userInRole, role.Name)))
                {
                    result = await userManager.RemoveFromRoleAsync(userInRole, role.Name);
                }
                else
                {
                    continue;
                }

                if (!result.Succeeded)
                {
                    ViewBag.ErrorMessage = "There have been problembs assigning roles to users. Contact the Administrator.";
                    return View("NotFound");
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }

        public ViewResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }
    }
}
