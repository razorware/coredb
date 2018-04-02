using System;
using System.Linq;

using Bogus;
using Bogus.DataSets;

namespace RazorWare.CoreDb.Fiction.DataObjects {
   public enum Gender {
      Unknown,
      Male,
      Female
   }

   public class Person {
      private int personId = 0;

      public int Id { get; set; }
      public DateTime Birthdate { get; set; }
      public Gender Gender { get; set; }
      public string Forename { get; set; }
      public string Segname { get; set; }
      public string Surname { get; set; }
      public int AddressId { get; set; }

      public Person( ) {
         Id = -1;
         AddressId = -1;
      }

      public override bool Equals(object obj) {
         if (!(obj is Person)) {
            return false;
         }

         var other = (Person)obj;
         return Id == other.Id;
      }

      public override int GetHashCode( ) {
         return Id;
      }
   }
}
