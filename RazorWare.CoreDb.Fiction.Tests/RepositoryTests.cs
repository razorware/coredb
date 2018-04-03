using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RazorWare.CoreDb.Interfaces;
using RazorWare.CoreDb.Fiction.DataObjects;

namespace RazorWare.CoreDb.Fiction.Testing {
   [TestClass]
   public class RepositoryTests {
      private DataGenerator<Person>.IGenerator personGenerator;

      private int personId = 0;
      private int addressId = 0;


      [TestInitialize]
      public void InitializeTest( ) {
         personGenerator = DataGenerator<Person>.Create(null)
            .RuleFor(o => o.Id, f => personId++)
            .RuleFor(o => o.Birthdate, f => DataTests.GetBirthdate(f))
            .RuleFor(o => o.Gender, f => DataTests.GetGender(f.Random.Int(1, 3)))
            .RuleFor(o => o.Forename, (f, o) => DataTests.GetForename(f, o))
            .RuleFor(o => o.Segname, (f, o) => DataTests.GetSegname(f, o))
            .RuleFor(o => o.Surname, f => f.Name.LastName());

      }

      [TestMethod]
      public void ConstructRepository( ) {
         var repo = new Repository();

         Assert.IsNotNull(repo);
      }

      [TestMethod]
      public void AddTypeToRepository( ) {
         var repo = new Repository();
         repo.AddCache<Person>();

         Assert.IsNotNull(repo.HasCache<Person>());
      }

      [TestMethod]
      public void AddPersonToIRepo( ) {
         var repo = new Repository();
         repo.AddCache<Person>();

         var cache = (IRepository)repo;
         var person = personGenerator.Generate();

         cache.Add(person);

         var people = cache.All<Person>();

         Assert.IsTrue(people.Any(p => p.Id == person.Id));
      }
   }
}
