# Data Generation  
This was a fun walk through fake data generation. Here is the short journal of discovery:

  Looking into: https://github.com/bchavez/Bogus

  Bogus is actually pretty cool. So I designed the `Fiction` project to have a database feel to it. It will act just like a data store.

The `CustomersDataGenerator` class lets me create a data container like a database:  
``` c#
   public class CustomersDataGenerator {
      private static readonly List<(string num, string str)> aptAddress = new List<(string num, string str)>();

      private static int personId = 0;
      private static int addressId = 0;
      
      private readonly Customers customers;
      private readonly DataBinder binder;
      private readonly Dictionary<Type, Delegate> generators = new Dictionary<Type, Delegate>();

      public Customers Customers => customers;

      public CustomersDataGenerator(IRepository repository) {
         customers = new Customers(repository);
         binder = new DataBinder(repository, "Customers");

         generators[typeof(ZipCode)] = binder.AddGenerator<ZipCode>()
            .RuleFor(o => o.Code, f => GetZipCode(f))
            .RuleFor(o => o.City, f => f.Address.City())
            .RuleFor(o => o.State, f => f.Address.StateAbbr())
            .Unique(repository, z => z.Code);

         generators[typeof(Address)] = binder.AddGenerator<Address>()
            .RuleFor(o => o.Id, f => addressId++)
            .RuleFor(o => o.StreetNumber, f => GetStreetNumber(f))
            .RuleFor(o => o.Street, f => GetStreet(f))
            .RuleFor(o => o.IsApartment, (f, o) => GetIsApartment(f, o))
            .RuleFor(o => o.UnitNumber, (f, o) => GetUnitNumber(f, o))
            .RuleFor(o => o.ZipCodeId, f => GetZipCode(f))
            .Cache();

         generators[typeof(Person)] = binder.AddGenerator<Person>()
            .RuleFor(o => o.Id, f => personId++)
            .RuleFor(o => o.Birthdate, f => GetBirthdate(f))
            .RuleFor(o => o.Gender, f => GetGender(f.Random.Int(1, 3)))
            .RuleFor(o => o.Forename, (f, o) => GetForename(f, o))
            .RuleFor(o => o.Segname, (f, o) => GetSegname(f, o))
            .RuleFor(o => o.Surname, f => f.Name.LastName())
            .RuleFor(o => o.AddressId, f => Generate<Address>().Id)
            .Cache();

         // prime 1000 ZipCodes
         for (var i = 0; i < 1000; i++) {
            Generate<ZipCode>();
         }
      }

      public TData Generate<TData>() {
         var tData = typeof(TData);

         if (!generators.TryGetValue(tData, out Delegate genDelegate)) {
            throw new InvalidOperationException($"{tData.FullName} not recognized");
         }

         var generate = (Generate<TData>)genDelegate;
         return generate();
      }
      
      private static string GetStreetNumber(Bogus.Faker f) {
         return f.Random.Int(1, 15000).ToString();
      }

      private static string GetStreet(Bogus.Faker f) {
         return $"{f.Address.StreetName()} {f.Address.StreetSuffix()}";
      }

      private static bool GetIsApartment(Bogus.Faker f, Address a) {
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

      private static string GetUnitNumber(Bogus.Faker f, Address a) {
         if (a.IsApartment) {
            return f.Random.UInt(100, 500).ToString();
         }

         return null;
      }

      private static DateTime GetBirthdate(Bogus.Faker f) {
         var earliest = new DateTime(DateTime.Now.Year - 55, 1, 31);
         var latest = new DateTime(DateTime.Now.Year - 20, 12, 31);

         return f.Date.Between(latest, earliest);
      }

      private static string GetForename(Bogus.Faker f, Person p) {
         var fGender = p.Gender == Gender.Male ?
            Bogus.DataSets.Name.Gender.Male :
            Bogus.DataSets.Name.Gender.Female;
         return f.Name.FirstName(fGender);
      }

      private static string GetSegname(Bogus.Faker f, Person p) {
         if (f.Random.Int(0, 100) < 95) {
            var fGender = p.Gender == Gender.Male ?
               Bogus.DataSets.Name.Gender.Male :
               Bogus.DataSets.Name.Gender.Female;

            return f.Name.FirstName(fGender);
         }

         return null;
      }

      private static int GetZipCode(Bogus.Faker f) {
         var code = f.Address.ZipCode();

         if (code.Contains("-")) {
            return int.Parse(code.Split('-')[0]);
         }
         else {
            return int.Parse(code);
         }
      }

      private static Gender GetGender(int gInt) {
         return (Gender)gInt;
      }
   }
```

Usage (see unit test):
``` c#
   var generator = new CustomersDataGenerator(new Repository());
   var customers = generator.Customers;

   // generate 1000 customers (Persons)
   for (var i = 0; i < 1000; i++) {
      generator.Generate<Person>();
   }
```

The above will generate 1000 Person objects with all the rules fulfilled in the `CustomersDataGenerator`. Because the rules are amended with either `Cache()` or `Unique(keyProperty)`, generated 
results are automatically inserted into the `Customer.IRepository`.

NOTE: ...now I have to wonder if there is a way to use an ORM configuration to generate relational pseudo-tables with generated data. _But this is a problem for another time._  
