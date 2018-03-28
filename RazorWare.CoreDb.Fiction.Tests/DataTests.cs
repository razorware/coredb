using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RazorWare.CoreDb.Fiction.Binders;
using RazorWare.CoreDb.Fiction.DataObjects;

namespace RazorWare.CoreDb.Fiction.Testing {
   [TestClass]
   public class DataTests {

      [TestMethod]
      public void CustomersValid( ) {
         var dataBinder = (DataBinder)new Customers();

         Assert.AreEqual("Customers", dataBinder.Name);
         // Data is not null
         Assert.IsNotNull(dataBinder.Data);
      }

      [TestMethod]
      public void CreatePersonDataObject( ) {
         var customers = new Customers();
         var genPeople = customers.Generate<Person>(1);
         
         Assert.IsTrue(genPeople.Any());

         var person = genPeople.First();

         // first record created
         Assert.AreEqual(0, person.Id);
         // birthdate 20 - 55 years ago
         Assert.IsTrue(DateBetween(20, 55, person.Birthdate));
         // has a gender
         Assert.AreNotEqual(Gender.Unknown, person.Gender);
         // has a forename
         Assert.IsNotNull(person.Forename);
         // has a surname
         Assert.IsNotNull(person.Surname);
      }

      private bool DateBetween(int earliest, int latest, DateTime birthdate) {
         var target = birthdate.Ticks;
         var youngest = new DateTime(DateTime.Now.Year - earliest, DateTime.Now.Month, DateTime.Now.Day);
         var oldest = new DateTime(DateTime.Now.Year - latest, DateTime.Now.Month, DateTime.Now.Day);

         return target > oldest.Ticks && target < youngest.Ticks;
      }
   }
}
