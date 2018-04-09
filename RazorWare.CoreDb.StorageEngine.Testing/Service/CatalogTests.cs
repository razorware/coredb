using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine.Testing {
   [TestClass]
   public class CatalogTests {
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
      public void ConstructCatalog( ) {
         var expName = "master";
         var expPath = Path.Combine(dataDir.FullName, expName);
         var masterDir = new DirectoryInfo(expPath);
         var catalog = Catalog.Create(masterDir);

         Assert.AreEqual(expName, catalog.Name);
         Assert.AreEqual(expPath, catalog.Location);
      }

      [TestMethod]
      public void CatalogDirectoryCreated( ) {
         var expName = "master";
         var expPath = Path.Combine(dataDir.FullName, expName);
         var masterDir = new DirectoryInfo(expPath);
         var catalog = Catalog.Create(masterDir);

         Assert.IsTrue(Directory.Exists(catalog.Location));
      }

      [TestMethod]
      public void CatalogStatusNewOpen( ) {
         /* when a catalog is created:
          *    - already verified directory created (and dupes raise exception)
          *    - status: New | Open
          * ***/
         var expStatus = CatalogStatus.New | CatalogStatus.Open | CatalogStatus.Dirty;
         var expName = "master";
         var expPath = Path.Combine(dataDir.FullName, expName);
         var masterDir = new DirectoryInfo(expPath);
         var catalog = Catalog.Create(masterDir);

         Assert.AreEqual(expStatus, catalog.Status);
      }
   }
}
