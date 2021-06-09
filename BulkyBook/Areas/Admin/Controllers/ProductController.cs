using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        //add using netcore.hosting
        //to upload images in a folder
        public readonly IWebHostEnvironment _hostEnvironment;


        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            //create product view model
            ProductViewModel productVM = new ProductViewModel()
            {
                Product = new Product(),
                //get list for a dropdown menu, only text, no model
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),

                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            //Product product = new Product();
            //if id is null, then make view to create a product
            if(id == null)
            {
                return View(productVM);
            }
            //else, make view to edit product
            //assing productVm.Product to getView of Id
            productVM.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());
            if(productVM.Product == null)
            {
                //id was not found
                return NotFound();
            }
            return View(productVM);

            
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Upsert(Product product) 
        //{
        //    //checks new data if meets requirements with model class
        //    if (ModelState.IsValid)
        //    {
        //        if(product.Id == 0)
        //        {
        //            _unitOfWork.Product.Add(product);
                   
        //        }
        //        else
        //        {
        //            _unitOfWork.Product.Update(product);
        //        }
        //        //needs to save changes
        //        _unitOfWork.Save();
        //        //redirects to index action
        //        //nameof(index) >> "Index"
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(product);
        //} 

        //JSON APIs
        //need to state #region and #endregion
        #region API CALLS 
        [HttpGet]
        public IActionResult GetAll()
        {
            //tell which foreign keys/values to populate
            var allObj = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productFromDb = _unitOfWork.Product.Get(id);
            if (productFromDb == null) return Json(new { success = false, message = "Error" });

            _unitOfWork.Product.Remove(productFromDb);
            _unitOfWork.Save();
            return Json(new {success=true, message="Product was deleted" });
        }
        #endregion 
    }
}
