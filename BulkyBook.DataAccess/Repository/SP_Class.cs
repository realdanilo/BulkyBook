using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DatAccess.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulkyBook.DataAccess.Repository
{
    public class SP_Class : ISP_Call
    {
        private readonly ApplicationDbContext _db;
        private static string ConnectionString = "";

        public SP_Class(ApplicationDbContext db)
        {
            _db = db;
            //get string connection to make sql store procedures
            ConnectionString = db.Database.GetDbConnection().ConnectionString;
        }
        public void Dispose()
        {
            _db.Dispose();
        }

        //to Update
        public void Execute(string procedureName, DynamicParameters param = null)
        {
            using SqlConnection sqlCon = new SqlConnection(ConnectionString);
            sqlCon.Open();
            sqlCon.Execute(procedureName, param, commandType: System.Data.CommandType.StoredProcedure);

        }


        //ie) retrieve all Categories
        public IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null)
        {
            using SqlConnection sqlCon = new SqlConnection(ConnectionString);
            sqlCon.Open();
            return sqlCon.Query<T>(procedureName, param, commandType: System.Data.CommandType.StoredProcedure);

        }


        //retrieve two tables
        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null)
        {
            using SqlConnection sqlCon = new SqlConnection(ConnectionString);
            sqlCon.Open();
            var result = SqlMapper.QueryMultiple(sqlCon, procedureName, param, commandType: System.Data.CommandType.StoredProcedure);
            var item1 = result.Read<T1>().ToList();
            var item2 = result.Read<T2>().ToList();
            if(item1 != null && item2 != null)
            {
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
            }
            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(), new List<T2>());
        }

        //one complete record
        public T OneRecord<T>(string procedureName, DynamicParameters param = null)
        {
            using SqlConnection sqlCon = new SqlConnection(ConnectionString);
            sqlCon.Open();
            var value =  sqlCon.Query<T>(procedureName, param, commandType: System.Data.CommandType.StoredProcedure);
            return (T)Convert.ChangeType(value.FirstOrDefault(), typeof(T));
        }

        public T Single<T>(string procedureName, DynamicParameters param = null)
        {
            using SqlConnection sqlCon = new SqlConnection(ConnectionString);
            sqlCon.Open();
            return (T)Convert.ChangeType(sqlCon.ExecuteScalar<T>(procedureName, param, commandType: System.Data.CommandType.StoredProcedure), typeof(T));

        }
    }
}
