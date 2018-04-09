using System;
using System.IO;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine {
   internal class Catalog : ICatalog {
      private readonly DirectoryInfo directory;

      private Header header;

      public string Location => directory.FullName;
      public string Name => directory.Name;
      public CatalogStatus Status => header.Status;

      private Catalog(DirectoryInfo path) {
         directory = path;
      }

      public void Open( ) {
         header.Status |= CatalogStatus.Open;
      }

      private void New( ) {
         header.Status = CatalogStatus.New;

         // create db file
         var fileName = Path.Combine(Location, $"{Name}.db");
         using(var fStream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) { }

         // set new catalogs open
         Open();
         header.Update();
      }

      internal static Catalog Create(DirectoryInfo catalogPath) {
         var catalog = new Catalog(catalogPath);
         catalog.header = new Header();

         try {
            catalogPath.Create();
            catalog.New();
         }
         catch (IOException) {
            throw;
         }

         return catalog;
      }
   }
}
