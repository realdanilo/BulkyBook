using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BulkyBook.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }

        [Key]
        public int Id { get; set; }
        public string AplicationUserId { get; set; }
        [ForeignKey("AplicationUserId ")]
        public ApplicationUser ApplicationUser { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Range(1,1000,ErrorMessage ="Please convert value between 1 and 1000")]
        public int Count { get; set; }


        [NotMapped]
        public double Price { get; set; }
    
    }
}
