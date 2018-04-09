using System;
using System.IO;
using System.Collections.Generic;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine {
   internal class Header {
      private readonly int size;
      private CatalogFormat format;
      private int pageSize;
      private int pageCount;
      private DateTime updateTS;
      private DateTime persistTS;

      public int Size => size;
      public CatalogStatus Status { get; internal set; }
      public CatalogFormat Format => format;
      public int PageSize => pageSize;
      public int PageCount => pageCount;

      internal Header( ) {
         size = HeaderSize;
      }

      internal void Read(Stream stream) {
         stream.Seek(0, SeekOrigin.Begin);

         using (var binReader = new BinaryReader(stream)) {
            updateTS = new DateTime(binReader.ReadInt64());
            persistTS = new DateTime(binReader.ReadInt64());
            Status = (CatalogStatus)binReader.ReadByte();
            format = (CatalogFormat)binReader.ReadByte();
            pageSize = binReader.ReadInt32();
            pageCount = binReader.ReadInt32();
         }

         if (updateTS > persistTS) {
            Status |= CatalogStatus.Dirty;
         }
      }

      internal void Update( ) {
         updateTS = DateTime.UtcNow;

         if (updateTS > persistTS) {
            Status |= CatalogStatus.Dirty;
         }
      }

      internal void Commit() {
         updateTS = persistTS = DateTime.UtcNow;
         Status &= ~CatalogStatus.Dirty;
      }
   }
}
