using System.Collections.Generic;

namespace RazorWare.CoreDb.StorageEngine.Service {
   public interface IStorageEngine {
      string RootDirectory { get; }
      ICatalogCollection Catalogs { get; }
   }
}
