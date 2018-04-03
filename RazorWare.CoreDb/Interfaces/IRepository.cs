using System.Collections.Generic;

namespace RazorWare.CoreDb.Interfaces {
   public interface IRepository {
      TData Add<TData>(TData data);
      IEnumerable<TData> All<TData>( );
   }
}
