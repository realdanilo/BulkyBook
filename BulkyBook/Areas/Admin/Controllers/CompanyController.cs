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
    //admin and employee can manage companies
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

    public class CompanyController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Company company= new Company();
            //if id is null, then make view to create a company
            if(id == null)
            {
                return View(company);
            }
            //else, make view to edit company

            company = _unitOfWork.Company.Get(id.GetValueOrDefault());
            if(company == null)
            {
                //id was not found
                return NotFound();
            }
            return View(company);

            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company) 
        {
            //checks new data if meets requirements with model class
            if (ModelState.IsValid)
            {
                if(company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                   
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                }
                //needs to save changes
                _unitOfWork.Save();
                //redirects to index action
                //nameof(index) >> "Index"
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        } 

        //JSON APIs
        //need to state #region and #endregion
        #region API CALLS 
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Company.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var companyFromDb = _unitOfWork.Company.Get(id);
            if (companyFromDb == null) return Json(new { success = false, message = "Error" });

            _unitOfWork.Company.Remove(companyFromDb);
            _unitOfWork.Save();
            return Json(new {success=true, message="Company was deleted" });
        }
        #endregion 
    }
}
