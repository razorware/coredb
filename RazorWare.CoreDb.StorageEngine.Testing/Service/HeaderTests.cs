using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static RazorWare.CoreDb.StorageEngine.Testing.TestHelpers;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine.Testing {
   [TestClass]
   public class HeaderTests {
      [TestMethod]
      public void ConstructHeader( ) {
         var header = new Header();

         Assert.AreEqual(MaxHeaderSize, header.MaxSize);
      }

      [TestMethod]
      public void ReadHeaderBuffer( ) {
         var expStatus = CatalogStatus.Open | CatalogStatus.Dirty;
         var expPersist = DateTime.UtcNow;
         var expUpdate = expPersist + new TimeSpan(0, 5, 38);
         Stream buffer = BuilderHeaderBuffer(CatalogStatus.Open, CatalogFormat.FilePerSource, expUpdate, expPersist);

         var header = new Header();
         header.Read(buffer);

         Assert.AreEqual(expStatus, header.Status);
         Assert.AreEqual(CatalogFormat.FilePerSource, header.Format);
         Assert.AreEqual(DbPageCount, header.PageCount);
         Assert.IsTrue(header.Length < MaxHeaderSize);
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

         Assert.IsTrue(header.Length < MaxHeaderSize);
      }

      [TestMethod]
      public void WriteHeaderBuffer( ) {
         var expPersist = DateTime.UtcNow;
         var expStatus = CatalogStatus.New | CatalogStatus.Open;
         var expFormat = CatalogFormat.MasterSourceFile;
         var expUpdate = expPersist + new TimeSpan(0, 5, 38);
         var expStream = BuilderHeaderBuffer(expStatus, expFormat, expUpdate, expPersist);
         var expBuffer = new byte[MaxHeaderSize];
         var stream = new MemoryStream(expBuffer);

         var header = new Header();
         header.Read(expStream);
         // automatically executes Update() and Commit()
         header.Write(stream);
         expStatus &= ~CatalogStatus.New;

         Assert.AreEqual(expStatus, header.Status);

         using (var binReader = new BinaryReader(stream)) {
            stream.Seek(0, SeekOrigin.Begin);

            Assert.AreEqual(binReader.ReadInt64(), binReader.ReadInt64());
            Assert.AreEqual(expStatus, (CatalogStatus)binReader.ReadByte());
            Assert.AreEqual(expFormat, (CatalogFormat)binReader.ReadByte());
            Assert.AreEqual(DbPageCount, binReader.ReadInt32());
         }
      }

      [TestMethod]
      public void HeaderReadPageContent( ) {
         var pages = new List<Page>() {
            new Page(PageType.Master)
         };
         // add the rest of the default pages
         while(pages.Count < DbPageCount) {
            pages.Add(new Page());
         }
         var persist = DateTime.UtcNow;
         var status = CatalogStatus.New | CatalogStatus.Open;
         var format = CatalogFormat.MasterSourceFile;
         var update = persist + new TimeSpan(0, 5, 38);
         var stream = BuilderHeaderBuffer(status, format, update, persist, pages.ToArray());

         var expPageContent = pages
            .Select(p => p.Type)
            .ToList();

         var header = new Header();
         header.Read(stream);

         CollectionAssert.AreEqual(expPageContent, header.PageContent.ToList());
      }
   }
}
