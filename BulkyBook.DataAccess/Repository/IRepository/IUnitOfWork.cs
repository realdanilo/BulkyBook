using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    //IDisposable >> release extensive resources used after they are used
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }

        ICoverTypeRepository CoverType{ get; }
        IProductRepository Product{ get; }
        ICompanyRepository Company { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IOrderDetailsRepository OrderDetails{ get; }
        IOrderHeaderRepository OrderHeader{ get; }
        IShoppingCartRepository ShoppingCart{ get; }

        ISP_Call SP_Call { get; }

        void Save();
    }
}
