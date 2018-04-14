using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine.Testing {
   [TestClass]
   public class BootstrapTests {
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
      public void ConstructStorageEngineBootstrap( ) {
         var expectedRoot = Path.GetFullPath(StorageConstants.RootDirectory);
         var se = ServiceBootstrap.Default;

         Assert.IsNotNull(se);
         Assert.AreEqual(expectedRoot, se.RootDirectory);
         Assert.AreEqual(0, se.Catalogs.Count);
      }

      [TestMethod]
      public void CreateCatalog( ) {
         var expName = "master";
         var expStatus = CatalogStatus.New | CatalogStatus.Open | CatalogStatus.Dirty;
         var se = ServiceBootstrap.Default;

         ICatalog catalog = se.Create().Catalog(expName);
         Assert.AreEqual(expStatus, catalog.Status);
      }
   }
}
