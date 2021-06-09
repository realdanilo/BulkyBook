using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.Models.ViewModels
{
    public class ProductViewModel
    {
        //main
        public Product Product { get; set; }

        //get populated list, dropdown for categorylist and covertypelist
        public IEnumerable<SelectListItem> CategoryList { get; set; }

        public IEnumerable<SelectListItem> CoverTypeList { get; set; }

    }
}
