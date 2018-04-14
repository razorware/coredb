using System;
using System.IO;
using System.Linq;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine.Testing {
   public static class TestHelpers {
      internal const int DbPageCount = 8;

      internal static Stream BuilderHeaderBuffer(CatalogStatus status, CatalogFormat format, DateTime update, DateTime persist, params Page[] pages) {
         var memStream = new MemoryStream(new byte[MaxHeaderSize]);

         using (var binWriter = new BinaryWriter(memStream, Encoding, true)) {
            binWriter.Write(update.Ticks);
            binWriter.Write(persist.Ticks);
            binWriter.Write((byte)status);
            binWriter.Write((byte)format);
            binWriter.Write(DbPageCount);

            if (pages != null && pages.Length > 0) {
               // add pages (Type) in reverse order to end of header byte array
               var pageTypes = pages.Reverse()
                  .Select(p => (byte)p.Type)
                  .ToArray();
               binWriter.Seek(-pageTypes.Length, SeekOrigin.End);
               binWriter.Write(pageTypes, 0, pageTypes.Length);
            }
         }

         return memStream;
      }

   }
}
