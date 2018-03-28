using System;
using System.Text;
using System.Collections.Generic;
using RazorWare.CoreDb.Fiction.DataObjects;

namespace RazorWare.CoreDb.Fiction.Binders {
   public class Customers : DataBinder {

      public Customers(): base("Customers") {
         AddDataType(new Person());
      }
   }
}
