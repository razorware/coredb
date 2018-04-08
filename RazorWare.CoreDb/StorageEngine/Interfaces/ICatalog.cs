using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine {
   public interface ICatalog {
      CatalogStatus Status { get; }
      string Name { get; }
      string Location { get; }
   }
}
