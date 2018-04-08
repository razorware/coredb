using System;
using System.IO;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine {
   internal class Catalog : ICatalog {
      private readonly DirectoryInfo directory;

      private CatalogStatus status;

      public string Location => directory.FullName;
      public string Name => directory.Name;
      public CatalogStatus Status => status;

      private Catalog(DirectoryInfo path) {
         directory = path;
      }

      public void Open( ) {
         status |= CatalogStatus.Open;
      }

      private void New( ) {
         status = CatalogStatus.New;

         // create db file
         var fileName = Path.Combine(Location, $"{Name}.db");
         using(var fStream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) { }

         // set new catalogs open
         Open();
      }

      internal static Catalog Create(DirectoryInfo catalogPath) {
         var catalog = new Catalog(catalogPath);

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
