using System;
using System.IO;
using System.Collections.Generic;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine {
   internal class Catalog : ICatalog {
      private readonly DirectoryInfo directory;
      private readonly FileInfo file;
      private readonly List<Page> pages;

      private Header header;

      public string Location => directory.FullName;
      public string Name => directory.Name;
      public CatalogStatus Status => header.Status;

      private Catalog(DirectoryInfo path) {
         directory = path ?? throw new InvalidOperationException("directory path cannot be null");
         file = new FileInfo(Path.Combine(directory.FullName, $"{directory.Name}.db"));
         pages = new List<Page>();
      }

      public void Open( ) {
         header.Status |= CatalogStatus.Open;
      }

      private void New( ) {
         header.Status = CatalogStatus.New;

         // create db file
         var fileName = Path.Combine(Location, $"{Name}.db");
         using(var fStream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) { }

         // set new catalogs open
         Open();
         header.Update();
      }

      internal static Catalog Create(DirectoryInfo catalogPath) {
         var catalog = new Catalog(catalogPath);
         catalog.header = new Header();

         try {
            catalogPath.Create();
            catalog.New();
         }
         catch (IOException) {
            throw;
         }

         return catalog;
      }

      internal static Catalog Load(DirectoryInfo catalogPath) {
         var catalog = new Catalog(catalogPath);

         try {
            catalog.Load();
         }
         catch (IOException) {
            throw;
         }

         return catalog;
      }

      private void Load( ) {
         using (var fStream = file.OpenRead()) {
            header = Header.FromStream(fStream);

            foreach(var pgType in header.PageContent) {
               var page = new Page(pgType);
               try {
                  page.Load(fStream);
               }
               catch (IOException) {
                  throw;
               }

               pages.Add(page);
            }
         }

      }
   }
}
