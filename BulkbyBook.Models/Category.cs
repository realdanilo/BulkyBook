using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BulkyBook.Models
{
    public class Category
    {
        //with entity framework, if Id is the name of the property, it will be binded as the primary key automatically
        //Or else you can use [Key] >> add using Data Anotation 
        public int Id { get; set; }

        [Display(Name="Category Name")]
        [Required]
        [MaxLength(50)]
        public string Name{ get; set; }


    }
}
