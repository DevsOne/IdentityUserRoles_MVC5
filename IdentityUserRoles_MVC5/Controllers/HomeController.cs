using IdentityUserRoles_MVC5.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;
using System.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityUserRoles_MVC5.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext context;
        public UserManager<ApplicationUser> UserManager { get; set; }

        public HomeController()
        {
            context = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        }
        public ActionResult Index()
        {
            RoleViewModel model = new RoleViewModel();
            List<SelectListItem> userList = new List<SelectListItem>();
            List<SelectListItem> roleList = new List<SelectListItem>();
            var users = context.Users;
            var roles = context.Roles;
            foreach(var user in users)
            {
                userList.Add(new SelectListItem { Text=user.UserName , Value = user.UserName });
            }
            foreach(var role in roles)
            {
                roleList.Add(new SelectListItem { Text = role.Name, Value = role.Name });
            }
            model.Users = userList;
            model.Roles = roleList;
            
            return View(model);
        }

        //In PackageManagerConsole Run >>      Enable-Migrations     , then 
        //Add-Migration "Name"  , then
        //Add UseContext or UseRoleManager method to the Seed method in Migrations/Configuration.cs , then
        //In PackageManagerConsole Run >>      Update-Database

        public void UseContext()
        {
            //Create Roles
            context.Roles.AddOrUpdate(r => r.Name,
                new IdentityRole { Name = "Admin" },
                new IdentityRole { Name = "Senior" },
                new IdentityRole { Name = "Moderator" },
                new IdentityRole { Name = "Member" },
                new IdentityRole { Name = "Junior" },
                new IdentityRole { Name = "Candidate" }
                );
            context.SaveChanges();

            //Create Users
            CreateUsers();

            //Assign Role To User(s)
            UserManager.AddToRole("1", "Admin");
        }
        public void UseRoleManager()
        {
            //Create Roles
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            string[] roleNames = { "Admin", "Member", "Moderator", "Junior", "Senior", "Candidate" };
            IdentityResult roleResult;
            foreach (var roleName in roleNames)
            {
                if (!RoleManager.RoleExists(roleName))
                {
                    roleResult = RoleManager.Create(new IdentityRole(roleName));
                }
            }

            //Create Users
            CreateUsers();

            //Assign Role To User(s)
            UserManager.AddToRole("1", "Admin");
        }

        public void CreateUsers()
        {
            List<ApplicationUser> newUsers = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", Email = "one@email.com", UserName = "one" },
                new ApplicationUser { Id = "2", Email = "two@email.com", UserName = "two" },
                new ApplicationUser { Id = "3", Email = "three@email.com", UserName = "three" },
                new ApplicationUser { Id = "4", Email = "four@email.com", UserName = "four" },
                new ApplicationUser { Id = "5", Email = "five@email.com", UserName = "five" },
                new ApplicationUser { Id = "6", Email = "six@email.com", UserName = "six" },
                new ApplicationUser { Id = "7", Email = "seven@email.com", UserName = "seven" },
                new ApplicationUser { Id = "8", Email = "eight@email.com", UserName = "eight" },
                new ApplicationUser { Id = "9", Email = "nine@email.com", UserName = "nine" },
            };
            foreach (var user in newUsers)
            {
                UserManager.Create(user, "12345Aa!");
            }
        }


        //Create Role From View
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUserRole(RoleViewModel model)
        {
            //var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            //IdentityResult roleResult;
            //if (!RoleManager.RoleExists(model.RoleName))
            //{
            //    roleResult = RoleManager.Create(new IdentityRole(model.RoleName));
            //}

            //OR
            context.Roles.AddOrUpdate(r => r.Name, new IdentityRole { Name = model.RoleName });
            context.SaveChanges();

            return RedirectToAction("Index","Home");
        }


        //Assign Role From View
        public ActionResult AssignRole(RoleViewModel model)
        {
            var userId = UserManager.FindByName(model.UserName).Id;
            var roleCheck = UserManager.IsInRole(userId, model.RoleName);
            
            if (!roleCheck)
            {
                var userRoles = UserManager.GetRoles(userId);
                if (userRoles != null)
                {
                    foreach (var role in userRoles)
                    {
                        UserManager.RemoveFromRole(userId, role);
                    }
                }
                UserManager.AddToRole(userId, model.RoleName);
            }

            return RedirectToAction("Index", "Home");
        }

    }

    public class RoleViewModel
    {
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        public List<SelectListItem> Users { get; set; }
        
        public List<SelectListItem> Roles { get; set; }


    }

}