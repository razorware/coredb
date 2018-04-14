using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using RazorWare.Reflection;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine {
   internal class Header {
      private readonly int maxSize;
      private CatalogFormat format;
      private int pageCount;
      private DateTime updateTS;
      private DateTime persistTS;

      public int MaxSize => maxSize;
      public CatalogStatus Status { get; internal set; }
      public CatalogFormat Format => format;
      public int PageCount => pageCount;
      public int Length => CalculateLength();
      public IEnumerable<PageType> PageContent { get; private set; }

      internal Header( ) {
         maxSize = MaxHeaderSize;
      }

      internal void Read(Stream stream) {
         stream.Seek(0, SeekOrigin.Begin);

         using (var binReader = new BinaryReader(stream, Encoding, true)) {
            updateTS = new DateTime(binReader.ReadInt64());
            persistTS = new DateTime(binReader.ReadInt64());
            Status = (CatalogStatus)binReader.ReadByte();
            format = (CatalogFormat)binReader.ReadByte();
            pageCount = binReader.ReadInt32();

            if (pageCount > 0) {
               binReader.BaseStream.Seek(-pageCount, SeekOrigin.End);
               // reading in reverse order
               var revContent = binReader.ReadBytes(pageCount);
               PageContent = revContent
                  .Reverse()
                  .Select(b => (PageType)b);
            }
         }

         if (updateTS > persistTS) {
            Status |= CatalogStatus.Dirty;
         }
      }

      internal void Write(Stream stream) {
         stream.Seek(0, SeekOrigin.Begin);

         // update
         Update();
         // auto-commit
         Commit();

         using(var binWriter = new BinaryWriter(stream, Encoding, true)) {
            binWriter.Write(updateTS.Ticks);
            binWriter.Write(persistTS.Ticks);
            binWriter.Write((byte)Status);
            binWriter.Write((byte)format);
            binWriter.Write(pageCount);
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
         
         // once committed, the Header (Catalog) is no longer New
         if ((Status & CatalogStatus.New) == CatalogStatus.New) {
            Status &= ~CatalogStatus.New;
         }
      }

      private int CalculateLength( ) {
         var length = 0;

         // updateTS:   DateTime.Ticks (long)
         length += Type.GetTypeCode(typeof(long)).Size();
         // persistTS:  DateTime.Ticks (long)
         length += Type.GetTypeCode(typeof(long)).Size();
         // Status (byte)
         ++length;
         // format (byte)
         ++length;
         // pageCount (Int32)
         length += Type.GetTypeCode(typeof(Int32)).Size();
         // pageContent (Int32)
         length += pageCount;

         return length;
      }
   }
}
