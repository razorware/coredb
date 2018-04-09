using System;
using System.IO;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine.Testing {
   [TestClass]
   public class HeaderTests {

      [TestMethod]
      public void ConstructHeader( ) {
         var header = new Header();

         Assert.AreEqual(HeaderSize, header.Size);
      }

      [TestMethod]
      public void ReadHeader( ) {
         var expStatus = CatalogStatus.Open | CatalogStatus.Dirty;
         var expPersist = DateTime.UtcNow;
         var expUpdate = expPersist + new TimeSpan(0, 5, 38);
         Stream buffer = BuilderHeaderBuffer(CatalogStatus.Open, CatalogFormat.FilePerSource, expUpdate, expPersist);

         var header = new Header();
         header.Read(buffer);

         Assert.AreEqual(expStatus, header.Status);
         Assert.AreEqual(CatalogFormat.FilePerSource, header.Format);
         Assert.AreEqual(PageSize, header.PageSize);
         Assert.AreEqual(PageCount, header.PageCount);
      }

      [TestMethod]
      public void CommitHeader( ) {
         var expStatus = CatalogStatus.Open;
         var expPersist = DateTime.UtcNow;
         var expUpdate = expPersist + new TimeSpan(0, 5, 38);
         Stream buffer = BuilderHeaderBuffer(CatalogStatus.Open, CatalogFormat.FilePerSource, expUpdate, expPersist);

         var header = new Header();
         header.Read(buffer);

         Assert.AreEqual(expStatus | CatalogStatus.Dirty, header.Status);

         // after commit, header NotDirty
         header.Commit();

         // brute test
         Assert.AreEqual(expStatus, header.Status);
         // bitwise finesse
         Assert.IsFalse((header.Status & CatalogStatus.Dirty) == CatalogStatus.Dirty);
      }

      private Stream BuilderHeaderBuffer(CatalogStatus status, CatalogFormat format, DateTime update, DateTime persist) {
         var memStream = new MemoryStream(new byte[HeaderSize]);

         using (var binWriter = new BinaryWriter(memStream, Encoding.UTF8, true)) {
            binWriter.Write(update.Ticks);
            binWriter.Write(persist.Ticks);
            binWriter.Write((byte)status);
            binWriter.Write((byte)format);
            binWriter.Write(PageSize);
            binWriter.Write(PageCount);
         }

         return memStream;
      }
   }
}
