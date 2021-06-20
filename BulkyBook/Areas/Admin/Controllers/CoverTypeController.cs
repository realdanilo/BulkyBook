using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

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

            //coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
             coverType = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);
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
                var parameter = new DynamicParameters();
                //as client validation, there always be a name input
                parameter.Add("@Name", coverType.Name);
                if(coverType.Id == 0)
                {
                    _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Create, parameter);
                   
                }
                else
                {
                    parameter.Add("@Id", coverType.Id);
                    _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Update, parameter);

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
        //
        //****
        //USING STORE PROCEDURES *****
        //****
        //

        #region API CALLS 
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.SP_Call.List<CoverType>(SD.Proc_CoverType_GetAll,null); ;
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            //SD.Proc_CoverType... is a static name to keep clean code, can be replaced with string
            
            //get one category based on id
            var parameter = new DynamicParameters();
            //defines whats in store procedure statement
            parameter.Add("@Id", id);
            //first get the cover from db 
            var coverTypeFromDb = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get,parameter);

            if (coverTypeFromDb == null) return Json(new { success = false, message = "Error" });

            //No remove method, execute a statement instead
            _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Delete, parameter);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Category was deleted" });
        }
        #endregion 
    }
}
