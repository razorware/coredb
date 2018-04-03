using System;
using System.Collections.Generic;

using RazorWare.CoreDb.Interfaces;

namespace RazorWare.DbFake.ClientDatabase {
   public class Customers {
      private readonly IRepository repository;

      public IEnumerable<ZipCode> ZipCodes => repository.All<ZipCode>();
      public IEnumerable<Address> Addresses => repository.All<Address>();
      public IEnumerable<Person> People => repository.All<Person>();

      public Customers(IRepository dataRepository) {
         repository = dataRepository;
      }
   }
}
