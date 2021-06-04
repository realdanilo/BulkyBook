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
            db = _db;
            Category = new CategoryRepository(_db);
            SP_Call = new SP_Class(_db);
        }

        public ICategoryRepository Category { get; private set; }

        public ISP_Call SP_Call{ get; private set; }

        public ISP_Call Sp_Call => throw new NotImplementedException();

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
