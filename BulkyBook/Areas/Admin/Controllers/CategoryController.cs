using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            //if id is null, then make view to create a category
            if(id == null)
            {
                return View(category);
            }
            //else, make view to edit category
            
            category = _unitOfWork.Category.Get(id.GetValueOrDefault());
            if(category == null)
            {
                //id was not found
                return NotFound();
            }
            return View(category);

            
        }

        //JSON APIs
        #region API CALLS 
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Category.GetAll();
            return Json(new { data = allObj });
        }
        #endregion 
    }
}
