using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static RazorWare.CoreDb.StorageEngine.Testing.TestHelpers;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine.Testing {
   [TestClass]
   public class CatalogTests {

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
      public void ConstructCatalog( ) {
         var expName = "master";
         var expPath = Path.Combine(DataDirectory.FullName, expName);
         var masterDir = new DirectoryInfo(expPath);
         var catalog = Catalog.Create(masterDir);

         Assert.AreEqual(expName, catalog.Name);
         Assert.AreEqual(expPath, catalog.Location);
      }

      [TestMethod]
      public void CatalogDirectoryCreated( ) {
         var expName = "master";
         var expPath = Path.Combine(DataDirectory.FullName, expName);
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
         var expPath = Path.Combine(DataDirectory.FullName, expName);
         var masterDir = new DirectoryInfo(expPath);
         var catalog = Catalog.Create(masterDir);

         Assert.AreEqual(expStatus, catalog.Status);
      }

      [TestMethod]
      public void CatalogFromFile( ) {
         // create a test catalog
         var persist = DateTime.UtcNow;
         var update = persist + new TimeSpan(0, 5, 38);
         var expContent = new PageType[]{
            PageType.Master, PageType.Schema, PageType.Empty, PageType.Index
         };
         var expSize = (long)MaxHeaderSize;
         foreach (var pc in expContent) {
            if (pc == PageType.Master) {
               expSize += MasterPage;
               continue;
            }

            expSize += MaxPageSize;
         }
         var dbFile = MakeCatalogFile("test", CatalogStatus.Closed, CatalogFormat.MasterSourceFile, update, persist, expContent);

         Assert.IsTrue(dbFile.Exists);
         Assert.AreEqual(expSize, dbFile.Length);

         var catalog = Catalog.Load(dbFile.Directory);

         Assert.AreEqual(CatalogStatus.Closed, catalog.Status);

         var catalogPages = GetCatalogPages(catalog);

         Assert.IsNotNull(catalogPages);
      }
   }
}
