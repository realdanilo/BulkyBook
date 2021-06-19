using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DatAccess.Data;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
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

       
        #endregion 
    }
}
