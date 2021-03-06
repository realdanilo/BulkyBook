using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DatAccess.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            SP_Call = new SP_Class(_db);

            CoverType = new CoverTypeRepository(_db);
            Product= new ProductRepository(_db);
            Company= new CompanyRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            OrderDetails = new OrderDetailsRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
        }

        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverType { get; private set; }

        public IProductRepository Product { get; set; }
        public ICompanyRepository Company{ get; set; }

        public IApplicationUserRepository ApplicationUser { get; set; }
        public IShoppingCartRepository ShoppingCart { get; set; }
        public IOrderHeaderRepository OrderHeader{ get; set; }
        public IOrderDetailsRepository OrderDetails{ get; set; }


        public ISP_Call SP_Call{ get; private set; }


        public void Dispose()
        {
            _db.Dispose();
        }

        //save method here, because Repository class changes the set<T> but does not update to the db
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
