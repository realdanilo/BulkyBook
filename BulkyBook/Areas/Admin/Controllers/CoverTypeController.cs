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
    public class CoverTypeController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            CoverType coverType= new CoverType();
            //if id is null, then make view to create a category
            if(id == null)
            {
                return View(coverType);
            }
            //else, make view to edit category
            
            coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
            if(coverType == null)
            {
                //id was not found
                return NotFound();
            }
            return View(coverType);

            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType) 
        {
            //checks new data if meets requirements with model class
            if (ModelState.IsValid)
            {
                if(coverType.Id == 0)
                {
                    _unitOfWork.CoverType.Add(coverType);
                   
                }
                else
                {
                    _unitOfWork.CoverType.Update(coverType);
                }
                //needs to save changes
                _unitOfWork.Save();
                //redirects to index action
                //nameof(index) >> "Index"
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        } 

        //JSON APIs
        //need to state #region and #endregion
        
        #region API CALLS 
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.CoverType.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var coverTypeFromDb = _unitOfWork.CoverType.Get(id);
            if (coverTypeFromDb == null) return Json(new { success = false, message = "Error" });

            _unitOfWork.CoverType.Remove(coverTypeFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Category was deleted" });
        }
        #endregion 
    }
}
