using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DatAccess.Data;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    //admin and employees can manage users
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

    public class UserController : Controller
    {
        public readonly ApplicationDbContext _db;
        //in real project, only use one patter, (dapper, repository pattern, appDbContext)

        public UserController(ApplicationDbContext db)
        {
            _db = db;

        }
        public IActionResult Index()
        {
            return View();
        }

       
       
        //JSON APIs
        //need to state #region and #endregion
        #region API CALLS 
        [HttpGet]
        public IActionResult GetAll()
        {
            //Include (using entity framework) allows to populate (foreign variables) company field
           //List of users
            var userList = _db.ApplicationUsers.Include(u => u.Company).ToList();
            //list of userId and RoleId
            var userRole = _db.UserRoles.ToList();
            //list of roles
            var roles = _db.Roles.ToList();

            foreach(var user in userList)
            {
                //using [Not Mapped] from ApplicationUser
                //find userId, then get roleId
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                //find name of assigned role
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

                //to avoid errors
                if(user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }

            }

            return Json(new { data = userList });
        }

       [HttpPost]
       public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDm = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if(objFromDm == null)
            {
                return Json(new { success = false, message = "Error, user not found" });
            }
            if (objFromDm.LockoutEnd != null && objFromDm.LockoutEnd > DateTime.Now)
            {
                //user is locked
                //unlock them
                objFromDm.LockoutEnd = DateTime.Now;
            }
            else
            {
                //user is not locked, lock him
                objFromDm.LockoutEnd = DateTime.Now.AddDays(10);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Successful" });

        }
        #endregion 
    }
}
