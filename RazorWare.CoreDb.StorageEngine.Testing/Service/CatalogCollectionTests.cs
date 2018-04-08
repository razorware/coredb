using System;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine.Testing {
   [TestClass]
   public class CatalogCollectionTests {
      private readonly DirectoryInfo dataDir = new DirectoryInfo(@".\Data");

      [TestInitialize]
      public void InitializeTest( ) {
         if (!dataDir.Exists) {
            dataDir.Create();
         }
         else {
            foreach (var d in dataDir.GetDirectories()) {
               d.Delete(true);
            }
         }

         CatalogCollection.Sync();
      }

      [TestCleanup]
      public void CleanupTest( ) {
         if (dataDir.Exists) {
            // move to TestResult-{TimeStamp}
            var testPath = Path.Combine(dataDir.Parent.FullName, $"DataTests_{DateTime.Now.ToString("MMM_dd_HH_mm_ss_ffff")}");
            Directory.Move(dataDir.FullName, testPath);
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
      public void CreateCatalog( ) {
         var expName = "temp";
         var catalogs = new CatalogCollection();
         var tempCatalog = catalogs.Create(expName);

         var expPath = Path.Combine(dataDir.FullName, expName);

         Assert.AreEqual(expName, tempCatalog.Name);
         Assert.AreEqual(expPath, tempCatalog.Location);
      }

      [TestMethod]
      public void CatalogDirectoryCreated( ) {
         var catalogs = new CatalogCollection();
         var tempCatalog = catalogs.Create("temp");

         Assert.IsTrue(Directory.Exists(tempCatalog.Location));
      }

      [TestMethod]
      [ExpectedException(typeof(IOException))]
      public void CreateDuplicateCatalog( ) {
         var catalogs = new CatalogCollection();
         var tempCatalog = catalogs.Create("temp");

         // throw exception
         catalogs.Create("temp");
      }

      [TestMethod]
      public void CatalogStatusNew( ) {
         /* when a catalog is created:
          *    - already verified directory created (and dupes raise exception)
          *    - status: New | Open
          * ***/
         var expName = "temp";
         var expStatus = CatalogStatus.New | CatalogStatus.Open;
         var catalogs = new CatalogCollection();
         var tempCatalog = catalogs.Create(expName);

         Assert.AreEqual(expStatus, tempCatalog.Status);
      }
   }
}
