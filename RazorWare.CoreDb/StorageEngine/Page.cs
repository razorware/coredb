using System;
using System.Text;
using System.Collections.Generic;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine {
   internal class Page {
      private readonly int maxSize;

      private PageType type;

      public PageType Type => type;
      public int MaxSize => maxSize;

      internal Page( ) : this(PageType.Empty) { }
      internal Page(PageType pageType) {
         type = pageType;
         maxSize = pageType == PageType.Master ? MasterPage : MaxPageSize;
      }
   }
}
