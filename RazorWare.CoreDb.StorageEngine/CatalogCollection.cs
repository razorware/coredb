using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine {
   internal class CatalogCollection : ICatalogCollection {
      private static DirectoryInfo rootDirectory = new DirectoryInfo(RootDirectory);
      private static Dictionary<string, DirectoryInfo> catalogPaths;

      public string Root => rootDirectory.Exists ? rootDirectory.FullName : null;
      public IReadOnlyList<string> Directories => catalogPaths.Keys.ToArray();
      public int Count => catalogPaths.Count;

      internal CatalogCollection( ) {
         Sync();
      }

      internal static void Sync( ) {
         catalogPaths = GetDirectories()
            .ToDictionary(k => k.Name, v => v);
      }

      public ICatalog Create(string directoryName) {
         if(!catalogPaths.ContainsKey(directoryName)) {
            return CreateCatalog(directoryName);
         }

         // error: cannot create catalog, already exists
         throw new IOException("Cannot create catalog that already exists");
      }

      private Catalog CreateCatalog(string catalogName) {
         return Catalog.Create(catalogPaths[catalogName] = new DirectoryInfo(Path.Combine(rootDirectory.FullName, catalogName)));
      }

      private static IReadOnlyList<DirectoryInfo> GetDirectories( ) {
         return rootDirectory.GetDirectories()
            .ToArray();
      }
   }
}
