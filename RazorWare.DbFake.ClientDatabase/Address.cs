namespace RazorWare.DbFake.ClientDatabase {
   public class Address {

      public int Id { get; set; }
      public bool IsApartment { get; set; }
      public string StreetNumber { get; set; }
      public string Street { get; set; }
      public string UnitNumber { get; set; }
      public int ZipCodeId { get; set; }

      public Address( ) {
         ZipCodeId = -1;
      }

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
