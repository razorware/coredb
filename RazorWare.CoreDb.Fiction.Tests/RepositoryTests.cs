using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RazorWare.CoreDb.Fiction.Testing {
   [TestClass]
   public class RepositoryTests {

      [TestMethod]
      public void ConstructRepository( ) {
         var repo = new Repository();

         Assert.IsNotNull(repo);
      }
   }
}
