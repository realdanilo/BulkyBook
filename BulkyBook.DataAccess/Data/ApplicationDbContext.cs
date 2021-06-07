using BulkyBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DatAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //pipeline, getting all models
        //DbSets > gets list of models
        //ie) Categories returns a list of all category objects in db
        public DbSet<Category> Categories { get; set; }
        public DbSet<CoverType> CoverTypes{ get; set; }

    }
}
