using Microsoft.VisualStudio.TestTools.UnitTesting;

using static RazorWare.CoreDb.StorageEngine.Testing.TestHelpers;

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
   }
}
