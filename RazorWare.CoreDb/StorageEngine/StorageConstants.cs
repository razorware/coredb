using System;
using System.Collections.Generic;

namespace RazorWare.CoreDb.StorageEngine {
   public static class StorageConstants {
      public const string RootDirectory = "./Data";

      public enum CatalogFormat {
         FilePerSource,
         MasterSourceFile
      }

      [Flags]
      public enum CatalogStatus {
         Unknown = 0,
         New = 1,
         Open = 2
      }

   }
}
