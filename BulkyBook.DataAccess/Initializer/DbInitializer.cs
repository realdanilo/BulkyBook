using BulkyBook.DatAccess.Data;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulkyBook.DataAccess.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            try { 
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    //there are pending migrations
                    _db.Database.Migrate();
                }
            }catch(Exception ex)
            {
                //err
            }
            //if there is already admin, do not create a new one
            if (_db.Roles.Any(role => role.Name == SD.Role_Admin)) return;
            //create roles
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Comp)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Indi)).GetAwaiter().GetResult();

            //create users
            _userManager.CreateAsync(new ApplicationUser { 
                UserName = "admin@test.com",
                Email = "admin@test.com",
                EmailConfirmed = true,
                Name = "Dan Admin"
            },"Password1@").GetAwaiter().GetResult();

            //get user to become admin
            ApplicationUser user = _db.ApplicationUsers.Where(u => u.Email == "admin@test.com").FirstOrDefault();

            // assing user as admin
            _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
        }
    }
}
