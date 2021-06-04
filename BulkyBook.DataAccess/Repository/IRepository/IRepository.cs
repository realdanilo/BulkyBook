using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    //make interface public
    //<T> means that accepts a generic class, needs to specify "where T:class"
    public interface IRepository<T> where T : class
    {
        //finds one object by id, in T Class
        T Get(int id);

        //Gets a list of T objects
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>,IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null
            );

        IEnumerable<T> GetFirstOrDefault(
         Expression<Func<T, bool>> filter = null,
         string includeProperties = null
         );

        //entity >> object that has an id, is real(true) and needs to be created
        void Add(T entity);

        //remove by id
        void Remove(int id);

        //Remove by entity, also same as remove by id
        void Remove(T Entity);

        //remove a list/range entities
        void RemoveRange(IEnumerable<T> entity);
    }
}
