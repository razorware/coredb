using System;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static RazorWare.CoreDb.StorageEngine.Testing.TestHelpers;

namespace RazorWare.CoreDb.StorageEngine.Testing {
   [TestClass]
   public class CatalogCollectionTests {

      [TestInitialize]
      public void InitializeTest( ) {
         if (!DataDirectory.Exists) {
            DataDirectory.Create();
         }
         else {
            foreach (var d in DataDirectory.GetDirectories()) {
               d.Delete(true);
            }
         }

         CatalogCollection.Sync();
      }

      [TestCleanup]
      public void CleanupTest( ) {
         if (DataDirectory.Exists) {
            // move to TestResult-{TimeStamp}
            var testPath = Path.Combine(DataDirectory.Parent.FullName, $"DataTests_{DateTime.Now.ToString("MMM_dd_HH_mm_ss_ffff")}");
            Directory.Move(DataDirectory.FullName, testPath);
         }
      }

      [ClassCleanup]
      public static void CleanupHarness( ) {
         var currDir = new DirectoryInfo(Directory.GetCurrentDirectory());
         foreach (var d in currDir.GetDirectories()) {
            if (d.Name.StartsWith("Data")) {
               d.Delete(true);
            }
         }
      }

      [TestMethod]
      public void ConstructCatalogCollection( ) {
         var catalogs = new CatalogCollection();

         Assert.IsNotNull(catalogs);
         Assert.IsNotNull(catalogs.Root);
      }

      [TestMethod]
      public void CatalogContentsMatch( ) {
         var catalogs = new CatalogCollection();
         var expAny = new DirectoryInfo(catalogs.Root).GetDirectories().Any();

         Assert.AreEqual(expAny, catalogs.Directories.Any());
      }

      [TestMethod]
      [ExpectedException(typeof(IOException))]
      public void CreateDuplicateCatalog( ) {
         var catalogs = new CatalogCollection();
         var tempCatalog = catalogs.Create("temp");

         // throw exception
         catalogs.Create("temp");
      }
   }
}
