using System.Linq;
using System.Collections.Generic;

using Bogus;
using System;

namespace RazorWare.CoreDb.Fiction.DataObjects {
   public class Address {

      public int Id { get; set; }
      public bool IsApartment { get; set; }
      public string StreetNumber { get; set; }
      public string Street { get; set; }
      public string UnitNumber { get; set; }
      public int ZipCodeId { get; set; }

      //private int GetZipCode(Faker f) {
      //   return Binder.Single<ZipCode>(persist: true).Code;
      //}

      public override bool Equals(object obj) {
         if (!(obj is Address)) {
            return false;
         }

         var other = (Address)obj;
         return Id == other.Id;
      }

      public override int GetHashCode( ) {
         return Id;
      }

      public override string ToString( ) {
         var unit = IsApartment ? $", #{UnitNumber}" : " ";

         return $"{StreetNumber} {Street}{unit}, {ZipCodeId}";
      }
   }
}
