using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine.Testing {
   public static class TestHelpers {
      internal const int DbPageCount = 8;

      internal static readonly DirectoryInfo DataDirectory = new DirectoryInfo(@".\Data");

      internal static Stream BuilderHeaderBuffer(CatalogStatus status, CatalogFormat format, DateTime update, DateTime persist, params Page[] pages) {
         var mStream = new MemoryStream(new byte[MaxHeaderSize]);

         using (var binWriter = new BinaryWriter(mStream, Encoding, true)) {
            binWriter.Write(update.Ticks);
            binWriter.Write(persist.Ticks);
            binWriter.Write((byte)status);
            binWriter.Write((byte)format);
            binWriter.Write(pages.Length);

            if (pages != null && pages.Length > 0) {
               // with pages (Type) in reverse order create PageDescriptor array
               var pageTypes = pages.Reverse()
                  .SelectMany(p => {
                     var pgDescr = PageDescriptor.Create(new byte[]{ 1 });
                     pgDescr.Write(0, ( ) => (byte)p.Type);

                     return (byte[])pgDescr;
                  })
                  .ToArray();
               binWriter.Seek(-pageTypes.Length, SeekOrigin.End);
               binWriter.Write(pageTypes, 0, pageTypes.Length);
            }
         }

         return mStream;
      }

      internal static FileInfo MakeCatalogFile(string name, CatalogStatus status, CatalogFormat format, DateTime update, DateTime persist, params PageType[] pageContent) {
         var dbPath = new DirectoryInfo(Path.Combine(DataDirectory.FullName, name));
         dbPath.Create();

         var length = MaxHeaderSize;
         var pages = new Page[pageContent.Length];
         var i = 0;

         foreach(var pc in pageContent) {
            pages[i] = new Page(pc);
            length += pages[i].MaxSize;

            ++i;
         }

         var headerStream = BuilderHeaderBuffer(status, format, update, persist, pages);
         headerStream.Position = 0;

         var filePath = Path.Combine(dbPath.FullName, $"{name}.db");
         var file = default(FileInfo);
         using (var fStream = (file = new FileInfo(filePath)).Create()) {
            headerStream.CopyTo(fStream);

            foreach(var p in pages) {
               fStream.SetLength(fStream.Length + p.MaxSize);
            }
         }

         return file;
      }

      internal static List<Page> GetCatalogPages(Catalog catalog) {
         var catalogType = typeof(Catalog);
         var pagesField = catalogType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(f => f.Name == "pages")
            .First();
         var catalogPages = (List<Page>)pagesField.GetValue(catalog);
         return catalogPages;
      }
   }
}
