using System;
using System.IO;
using System.Collections.Generic;
using RazorWare.CoreDb.StorageEngine.Service;

namespace RazorWare.CoreDb.StorageEngine {
   public class ServiceBootstrap {
      public static IStorageEngine Default => StorageEngine.Instance;

      private class StorageEngine : Singleton<StorageEngine, IStorageEngine>, IStorageEngine {
         private readonly CatalogCollection catalogs;

         public string RootDirectory => catalogs.Root;
         public ICatalogCollection Catalogs => catalogs;

         protected StorageEngine( ) {
            catalogs = new CatalogCollection();
         }
      }
   }

}
