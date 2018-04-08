using System;

namespace RazorWare.CoreDb.StorageEngine {
   public interface ICatalogCollection {
      int Count { get; }

      ICatalog Create(string name);
   }
}
