using System;
using RazorWare.CoreDb.StorageEngine.Service;

namespace RazorWare.CoreDb.StorageEngine {
   public delegate CreateCommand CreateHandler( );

   public static class StorageEngineExtensions {
      public static CreateCommand Create(this IStorageEngine storageEngine) {
         //return ( ) => new CreateCommand(storageEngine);
         return new CreateCommand(storageEngine);
      }

      public static CreateHandler Catalog(this CreateHandler handler, string name) {
         return ( ) => {
            return null;
         };
      }
   }

   public class CreateCommand {
      private readonly IStorageEngine se;

      internal CreateCommand(IStorageEngine storageEngine) {
         se = storageEngine;
      }

      public ICatalog Catalog(string name) {
         return se.Catalogs.Create(name);
      }
   }
}
