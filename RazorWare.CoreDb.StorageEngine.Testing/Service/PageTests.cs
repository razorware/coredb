using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine.Testing {
   [TestClass]
   public class PageTests {

      [TestMethod]
      public void ConstructDefaultPage( ) {
         var page = new Page(PageType.Schema);

         Assert.AreEqual(PageType.Schema, page.Type);
         Assert.AreEqual(MaxPageSize, page.MaxSize);
      }

      [TestMethod]
      public void ConstructMasterPage( ) {
         var expPageSize = MasterPage;
         var page = new Page(PageType.Master);

         Assert.AreEqual(expPageSize, page.MaxSize);
      }

      [TestMethod]
      public void PageLoadFromStream( ) {
         var buffer = new byte[MaxHeaderSize + MasterPage];
         var stream = new MemoryStream(buffer);
         stream.Seek(MaxHeaderSize, SeekOrigin.Begin);

         var page = new Page(PageType.Master);
         page.Load(stream);

         Assert.AreEqual(stream.Length, stream.Position);
         Assert.AreEqual(MaxHeaderSize, page.Location);
      }
   }
}
