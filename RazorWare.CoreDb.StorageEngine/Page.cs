using System.IO;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine {
   internal class Page {
      private readonly int maxSize;

      private PageType type;
      private long location;
      private MemoryStream mStream;

      public PageType Type => type;
      public int MaxSize => maxSize;
      public long Location => location;

      internal Page( ) : this(PageType.Empty) { }
      internal Page(PageType pageType) {
         type = pageType;
         maxSize = pageType == PageType.Master ? MasterPage : MaxPageSize;
      }

      internal void Load(Stream stream) {
         location = stream.Position;
         mStream = new MemoryStream(new byte[maxSize]);
         stream.CopyTo(mStream);
      }
   }
}
