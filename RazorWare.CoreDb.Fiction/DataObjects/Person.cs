using System;
using Bogus;
using Bogus.DataSets;

namespace RazorWare.CoreDb.Fiction.DataObjects {
   public enum Gender {
      Unknown,
      Male,
      Female
   }

   public class Person : Data {
      private int personId = 0;

      public int Id { get; set; }
      public DateTime Birthdate { get; set; }
      public Gender Gender { get; set; }
      public string Forename { get; set; }
      public string Segname { get; set; }
      public string Surname { get; set; }

      public Person( ) : base("Person") {
         MakeFake<Person>()
            .RuleFor(o => o.Id, f => personId++)
            .RuleFor(o => o.Birthdate, f => GetBirthdate(f))
            .RuleFor(o => o.Gender, f => GetGender(f.Random.Int(1, 3)))
            .RuleFor(o => o.Forename, (f, o) => GetForename(f, o))
            .RuleFor(o => o.Segname, (f, o) => GetSegname(f, o))
            .RuleFor(o => o.Surname, f => f.Name.LastName());
      }

      private DateTime GetBirthdate(Faker f) {
         var earliest = new DateTime(DateTime.Now.Year - 55, 1, 31);
         var latest = new DateTime(DateTime.Now.Year - 20, 12, 31);

         return f.Date.Between(latest, earliest);
      }

      private string GetForename(Faker f, Person p) {
         var fGender = p.Gender == Gender.Male ?
            Name.Gender.Male :
            Name.Gender.Female;
         return f.Name.FirstName(fGender);
      }

      private string GetSegname(Faker f, Person p) {
         if (f.Random.Int(0, 100) < 95) {
            var fGender = p.Gender == Gender.Male ?
               Name.Gender.Male :
               Name.Gender.Female;

            return f.Name.FirstName(fGender);
         }

         return null;
      }

      private Gender GetGender(int gInt) {
         return (Gender)gInt;
      }
   }
}
