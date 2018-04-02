using System;

using Bogus;

namespace RazorWare.CoreDb.Fiction.DataObjects {
   public class ZipCode {

      public int Code { get; set; }
      public string City { get; set; }
      public string State { get; set; }

      //internal ZipCode(DataBinder dataBinder) : base("ZipCode", dataBinder) {
      //   Configure<ZipCode>()
      //      .RuleFor(o => o.Code, f => GetZipCode(f))
      //      .RuleFor(o => o.City, f => f.Address.City())
      //      .RuleFor(o => o.State, f => f.Address.StateAbbr());
      //}

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
         return $"{Code}, {City}, {State}";
      }

      private int GetZipCode(Faker f) {
         var code = f.Address.ZipCode();

         if (code.Contains("-")) {
            return int.Parse(code.Split('-')[0]);
         }
         else {
            return int.Parse(code);
         }
      }
   }
}
