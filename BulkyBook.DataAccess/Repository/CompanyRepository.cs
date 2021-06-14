﻿using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DatAccess.Data;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulkyBook.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company> , ICompanyRepository
    {
       private readonly ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
        public void Update(Company company)
        {
            var objFromDb = _db.Companies.FirstOrDefault(obj => obj.Id == company.Id);
            if(objFromDb !=null)
            {

                //objFromDb.Name = company.Name;
                //objFromDb.StreetAddress = company.StreetAddress;
                //objFromDb.City = company.City;
                //objFromDb.State= company.State;
                //objFromDb.PostalCode= company.PostalCode;
                //objFromDb.PhoneNumber= company.PhoneNumber;
                //objFromDb.IsAuthorizedCompany= company.IsAuthorizedCompany;

                //also works
                _db.Update(company);

                //will be saved in controller, following same format pattern

            }

        }
    }
}
