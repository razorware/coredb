using System;

namespace RazorWare.CoreDb.Fiction.DataObjects {
   public class ZipCode {

      public int Code { get; set; }
      public string City { get; set; }
      public string State { get; set; }

      public ZipCode( ) { }

      public static bool operator ==(ZipCode zc1, ZipCode zc2) {
         return zc1.Code == zc2.Code;
      }

      public static bool operator !=(ZipCode zc1, ZipCode zc2) {
         return zc1.Code != zc2.Code;
      }

      public override bool Equals(object obj) {
         if (!(obj is ZipCode)) {
            return false;
         }

         var other = (ZipCode)obj;

         return Code == other.Code;
      }

      public override int GetHashCode( ) {
         return Code;
      }

      public override string ToString( ) {
         return $"{Code:D5}, {City}, {State}";
      }
   }
}
