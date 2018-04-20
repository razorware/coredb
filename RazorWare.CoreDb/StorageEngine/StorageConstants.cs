using System;
using System.Text;

namespace RazorWare.CoreDb.StorageEngine {
   public static class StorageConstants {
      public const string RootDirectory = "./Data";
      public const int Kilobytes = 1024;
      public const int MaxHeaderSize = 128;  // bytes
      public const int MaxPageSize = 8 * Kilobytes;
      public const int MasterPage = 2 * Kilobytes;
      public const int PageCount = 128;

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
         Closed = 8,
      }

      public enum PageType {
         Empty = 0,
         Master,
         Schema,
         Index
      }
   }
}
