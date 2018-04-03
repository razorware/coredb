using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RazorWare.CoreDb.Fiction.DataObjects;


namespace RazorWare.CoreDb.Fiction.Testing {
   [TestClass]
   public class DataTests {
      private static readonly List<(string num, string str)> aptAddress = new List<(string num, string str)>();

      private int personId = 0;
      private int addressId = 0;

      [TestInitialize]
      public void InitializeTest( ) {
         personId = 0;
         addressId = 0;
      }

      [TestMethod]
      public void ConstructDataGenerator( ) {
         var personGenerator = DataGenerator<Person>.Create(null);

         // instance of IGenerator
         Assert.IsInstanceOfType(personGenerator, typeof(IDataGenerator));

         // default naming
         Assert.AreEqual("Person", personGenerator.Name);

         var expName = "TestPersonGenerator";
         personGenerator = DataGenerator<Person>.Create(null, expName);

         Assert.AreEqual(expName, personGenerator.Name);

         // default properties
         Assert.AreEqual(false, personGenerator.IsUnique);
      }

      [TestMethod]
      public void ConfigureDataGenerator( ) {
         var personGenerator = DataGenerator<Person>.Create(null)
            .RuleFor(o => o.Id, f => personId++)
            .RuleFor(o => o.Birthdate, f => GetBirthdate(f))
            .RuleFor(o => o.Gender, f => GetGender(f.Random.Int(1, 3)))
            .RuleFor(o => o.Forename, (f, o) => GetForename(f, o))
            .RuleFor(o => o.Segname, (f, o) => GetSegname(f, o))
            .RuleFor(o => o.Surname, f => f.Name.LastName());

         var person0 = personGenerator.Generate();
         var person1 = personGenerator.Generate();

         Assert.AreEqual(0, person0.Id);
         Assert.AreEqual(1, person1.Id);
      }

      [TestMethod]
      public void UseGeneratorDependencies( ) {
         var addressGenerator = DataGenerator<Address>.Create(null)
            .RuleFor(o => o.Id, f => addressId++)
            .RuleFor(o => o.StreetNumber, f => GetStreetNumber(f))
            .RuleFor(o => o.Street, f => GetStreet(f))
            .RuleFor(o => o.IsApartment, (f, o) => GetIsApartment(f, o))
            .RuleFor(o => o.UnitNumber, (f, o) => GetUnitNumber(f, o));
         //.RuleFor(o => o.ZipCodeId, f => GetZipCode(f));
         var personGenerator = DataGenerator<Person>.Create(null)
            .RuleFor(o => o.Id, f => personId++)
            .RuleFor(o => o.Birthdate, f => GetBirthdate(f))
            .RuleFor(o => o.Gender, f => GetGender(f.Random.Int(1, 3)))
            .RuleFor(o => o.Forename, (f, o) => GetForename(f, o))
            .RuleFor(o => o.Segname, (f, o) => GetSegname(f, o))
            .RuleFor(o => o.Surname, f => f.Name.LastName())
            .RuleFor(o => o.AddressId, f => addressGenerator.Generate().Id);

         var person = personGenerator.Generate();

         Assert.IsTrue(person.AddressId != -1);
      }

      [TestMethod]
      public void ConstructDataBinder( ) {
         var customers = new DataBinder(binderName:"Customers");

         // default name
         Assert.AreEqual("Customers", customers.Name);
      }

      [TestMethod]
      public void AddGeneratorToBinder( ) {
         var customers = new DataBinder(binderName:"Customers");
         customers.AddGenerator<Person>()
            .RuleFor(o => o.Id, f => personId++)
            .RuleFor(o => o.Birthdate, f => GetBirthdate(f))
            .RuleFor(o => o.Gender, f => GetGender(f.Random.Int(1, 3)))
            .RuleFor(o => o.Forename, (f, o) => GetForename(f, o))
            .RuleFor(o => o.Segname, (f, o) => GetSegname(f, o))
            .RuleFor(o => o.Surname, f => f.Name.LastName());

         var person0 = customers.Generate<Person>();
         var person1 = customers.Generate<Person>();

         Assert.AreEqual(0, person0.Id);
         Assert.AreEqual(1, person1.Id);
      }

      [TestMethod]
      public void AddGeneratorDependencyToBinder( ) {
         var customers = new DataBinder(binderName: "Customers");
         customers.AddGenerator<Address>()
            .RuleFor(o => o.Id, f => addressId++)
            .RuleFor(o => o.StreetNumber, f => GetStreetNumber(f))
            .RuleFor(o => o.Street, f => GetStreet(f))
            .RuleFor(o => o.IsApartment, (f, o) => GetIsApartment(f, o))
            .RuleFor(o => o.UnitNumber, (f, o) => GetUnitNumber(f, o));
         customers.AddGenerator<Person>()
            .RuleFor(o => o.Id, f => personId++)
            .RuleFor(o => o.Birthdate, f => GetBirthdate(f))
            .RuleFor(o => o.Gender, f => GetGender(f.Random.Int(1, 3)))
            .RuleFor(o => o.Forename, (f, o) => GetForename(f, o))
            .RuleFor(o => o.Segname, (f, o) => GetSegname(f, o))
            .RuleFor(o => o.Surname, f => f.Name.LastName())
            .RuleFor(o => o.AddressId, f => customers.Generate<Address>().Id);

         var person = customers.Generate<Person>();

         Assert.IsTrue(person.AddressId != -1);
      }

      [TestMethod]
      public void CacheBinderData( ) {
         var repository = new Repository();
         repository.AddCache<Address>();
         repository.AddCache<Person>();

         var customers = new DataBinder(dataRepo:repository, binderName: "Customers");
         customers.AddGenerator<Address>()
            .RuleFor(o => o.Id, f => addressId++)
            .RuleFor(o => o.StreetNumber, f => GetStreetNumber(f))
            .RuleFor(o => o.Street, f => GetStreet(f))
            .RuleFor(o => o.IsApartment, (f, o) => GetIsApartment(f, o))
            .RuleFor(o => o.UnitNumber, (f, o) => GetUnitNumber(f, o));
         customers.AddGenerator<Person>()
            .RuleFor(o => o.Id, f => personId++)
            .RuleFor(o => o.Birthdate, f => GetBirthdate(f))
            .RuleFor(o => o.Gender, f => GetGender(f.Random.Int(1, 3)))
            .RuleFor(o => o.Forename, (f, o) => GetForename(f, o))
            .RuleFor(o => o.Segname, (f, o) => GetSegname(f, o))
            .RuleFor(o => o.Surname, f => f.Name.LastName())
            .RuleFor(o => o.AddressId, f => customers.Generate<Address>().Id);

         // returns a function
         var genPerson = customers.Cache(customers.Generate<Person>);

         var expPeople = new List<Person>();
         while (expPeople.Count < 5) {
            expPeople.Add(genPerson());
         }

         CollectionAssert.AreEqual(expPeople, repository.All<Person>().ToList());
         // NOTE: we did not cache any of the generated addresses
         Assert.IsFalse(repository.All<Address>().Any());
      }

      [TestMethod]
      public void CacheDependencyBinderData( ) {
         var repository = new Repository();
         repository.AddCache<Address>();
         repository.AddCache<Person>();

         var customers = new DataBinder(dataRepo: repository, binderName: "Customers");
         var addrGenerator = customers.AddGenerator<Address>()
            .RuleFor(o => o.Id, f => addressId++)
            .RuleFor(o => o.StreetNumber, f => GetStreetNumber(f))
            .RuleFor(o => o.Street, f => GetStreet(f))
            .RuleFor(o => o.IsApartment, (f, o) => GetIsApartment(f, o))
            .RuleFor(o => o.UnitNumber, (f, o) => GetUnitNumber(f, o))
            .Cache();
         customers.AddGenerator<Person>()
            .RuleFor(o => o.Id, f => personId++)
            .RuleFor(o => o.Birthdate, f => GetBirthdate(f))
            .RuleFor(o => o.Gender, f => GetGender(f.Random.Int(1, 3)))
            .RuleFor(o => o.Forename, (f, o) => GetForename(f, o))
            .RuleFor(o => o.Segname, (f, o) => GetSegname(f, o))
            .RuleFor(o => o.Surname, f => f.Name.LastName())
            .RuleFor(o => o.AddressId, f => addrGenerator().Id);

         var genPerson = customers.Cache(customers.Generate<Person>);

         var expPeople = new List<Person>();
         var expAddresses = new List<Address>();
         while (expPeople.Count < 5) {
            var p = genPerson();
            expPeople.Add(p);
            expAddresses.Add(repository.All<Address>().First(addr => addr.Id == p.AddressId));
         }

         CollectionAssert.AreEqual(expPeople, repository.All<Person>().ToList());
         CollectionAssert.AreEqual(expAddresses, repository.All<Address>().ToList());
      }

      [TestMethod]
      public void CacheUniqueData( ) {
         var repository = new Repository();
         repository.AddCache<ZipCode>();

         var customers = new DataBinder(dataRepo: repository, binderName: "Customers");
         // Unique executes Cache automatically
         var zipGenerator = customers.AddGenerator<ZipCode>()
            .RuleFor(o => o.Code, f => GetZipCode(f))
            .RuleFor(o => o.City, f => f.Address.City())
            .RuleFor(o => o.State, f => f.Address.StateAbbr())
            .Unique(repository, z => z.Code);

         // generate until duplicate code is encountered
         var zipCodes = new Dictionary<int, List<ZipCode>>();
         var hasDupe = false;
         var key = -1;

         while (!hasDupe) {
            var data = zipGenerator();
            if(zipCodes.TryGetValue(data.Code, out List<ZipCode> cache)) {
               cache.Add(data);
               key = data.Code;
            }
            else {
               zipCodes[data.Code] = cache = new List<ZipCode>();
               cache.Add(data);
            }

            hasDupe = key != -1;
         }

         Assert.IsTrue(zipCodes[key].Count == 2);

         var zc0 = zipCodes[key][0];
         var zc1 = zipCodes[key][1];

         Assert.AreEqual(zc0.City, zc1.City);
         Assert.AreEqual(zc0.State, zc1.State);
      }

      internal static string GetStreetNumber(Bogus.Faker f) {
         return f.Random.Int(1, 15000).ToString();
      }

      internal static string GetStreet(Bogus.Faker f) {
         return $"{f.Address.StreetName()} {f.Address.StreetSuffix()}";
      }

      internal static bool GetIsApartment(Bogus.Faker f, Address a) {
         // is address already listed as apt?
         var isApt = aptAddress.Any(apt => apt.num == a.StreetNumber && apt.str == a.Street);

         if (!isApt) {
            // otherwise, randomize result
            isApt = f.Random.Int(0, 101) > 75 ? true : false;

            if (isApt) {
               aptAddress.Add((a.StreetNumber, a.Street));
            }
         }

         return isApt;
      }

      internal static string GetUnitNumber(Bogus.Faker f, Address a) {
         if (a.IsApartment) {
            return f.Random.UInt(100, 500).ToString();
         }

         return null;
      }

      internal static DateTime GetBirthdate(Bogus.Faker f) {
         var earliest = new DateTime(DateTime.Now.Year - 55, 1, 31);
         var latest = new DateTime(DateTime.Now.Year - 20, 12, 31);

         return f.Date.Between(latest, earliest);
      }

      internal static string GetForename(Bogus.Faker f, Person p) {
         var fGender = p.Gender == Gender.Male ?
            Bogus.DataSets.Name.Gender.Male :
            Bogus.DataSets.Name.Gender.Female;
         return f.Name.FirstName(fGender);
      }

      internal static string GetSegname(Bogus.Faker f, Person p) {
         if (f.Random.Int(0, 100) < 95) {
            var fGender = p.Gender == Gender.Male ?
               Bogus.DataSets.Name.Gender.Male :
               Bogus.DataSets.Name.Gender.Female;

            return f.Name.FirstName(fGender);
         }

         return null;
      }

      internal static int GetZipCode(Bogus.Faker f) {
         var code = f.Address.ZipCode();

         if (code.Contains("-")) {
            return int.Parse(code.Split('-')[0]);
         }
         else {
            return int.Parse(code);
         }
      }

      internal static Gender GetGender(int gInt) {
         return (Gender)gInt;
      }

      private bool DateBetween(int earliest, int latest, DateTime birthdate) {
         var target = birthdate.Ticks;
         var youngest = new DateTime(DateTime.Now.Year - earliest, DateTime.Now.Month, DateTime.Now.Day);
         var oldest = new DateTime(DateTime.Now.Year - latest, DateTime.Now.Month, DateTime.Now.Day);

         return target > oldest.Ticks && target < youngest.Ticks;
      }
   }
}
