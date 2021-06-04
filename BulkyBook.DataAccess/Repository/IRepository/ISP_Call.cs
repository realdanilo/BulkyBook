using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface ISP_Call: IDisposable
    {
        //return single INTEGER or BOOLEAN value, ie) count
        T Single<T>(string procedureName, DynamicParameters param = null);

        //ie) execute something from the db (add,delete) but do not want to retrieve anything
        void Execute(string procedureName, DynamicParameters param = null);
    
        //retrieve one complete record
        T OneRecord<T>(string procedureName, DynamicParameters param = null);
    
        //returns all complete records
        IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null);

        //Tuple >> combines multiple data/class elements
        //returns two complete tables
        Tuple<IEnumerable<T1>,IEnumerable<T2>> List<T1,T2>(string procedureName, DynamicParameters param = null);

    }
}
