using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    // global level, only admin allowed
    [Authorize(Roles =SD.Role_Admin)]
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category) 
        {
            //checks new data if meets requirements with model class
            if (ModelState.IsValid)
            {
                if(category.Id == 0)
                {
                    _unitOfWork.Category.Add(category);
                   
                }
                else
                {
                    _unitOfWork.Category.Update(category);
                }
                //needs to save changes
                _unitOfWork.Save();
                //redirects to index action
                //nameof(index) >> "Index"
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        } 

        //JSON APIs
        //need to state #region and #endregion
        #region API CALLS 
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Category.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryFromDb = _unitOfWork.Category.Get(id);
            if (categoryFromDb == null) return Json(new { success = false, message = "Error" });

            _unitOfWork.Category.Remove(categoryFromDb);
            _unitOfWork.Save();
            return Json(new {success=true, message="Category was deleted" });
        }
        #endregion 
    }
}
