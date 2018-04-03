using System;

namespace RazorWare.DbFake.ClientDatabase {
   public enum Gender {
      Unknown,
      Male,
      Female
   }

   public class Person {
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

      public override string ToString( ) {
         return $"{Surname}, {Forename} {(string.IsNullOrEmpty(Segname) ? string.Empty : $"{Segname[0]}.")}";
      }
   }
}
