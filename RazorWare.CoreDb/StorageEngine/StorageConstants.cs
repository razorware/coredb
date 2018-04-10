using System;
using System.Text;
using System.Collections.Generic;

namespace RazorWare.CoreDb.StorageEngine {
   public static class StorageConstants {
      public const string RootDirectory = "./Data";
      public const int Kilobytes = 1024;
      public const int MaxHeaderSize = 128;  // bytes
      public const int PageSize = 8 * Kilobytes;
      public const int PageCount = 16;

      public static Encoding Encoding => Encoding.UTF8;

      public enum CatalogFormat {
         FilePerSource,
         MasterSourceFile
      }

      [Flags]
      public enum CatalogStatus {
         Unknown = 0,
         New = 1,
         Open = 2,
         Dirty = 4,
      }
   }
}
