using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RazorWare.DbFake.ClientDatabase;

namespace RazorWare.CoreDb.Fiction.Testing {
   [TestClass]
   public class CustomersTests {

      [TestMethod]
      public void ConstructCustomersRepo( ) {
         var generator = new CustomersDataGenerator(new Repository());
         var customers = generator.Customers;

         Assert.IsNotNull(customers);
         Assert.IsTrue(customers.ZipCodes.Any());
      }

      [TestMethod]
      public void ConstructClientPeople( ) {
         var generator = new CustomersDataGenerator(new Repository());
         var customers = generator.Customers;

         // generate 1000 customers (Persons)
         for (var i = 0; i < 1000; i++) {
            generator.Generate<Person>();
         }

         Assert.AreEqual(1000, customers.People.Count());
      }
   }
}
