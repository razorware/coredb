using System;
using System.Collections;
using System.Collections.Generic;

using RazorWare.CoreDb.Interfaces;

namespace RazorWare.CoreDb.Fiction {
   internal class Repository : IRepository {
      private readonly Dictionary<Type, ICollection> cache = new Dictionary<Type, ICollection>();

      public TData Add<TData>(TData data) {
         if (!TryGetCache(out List<TData> cache)) {
            return default(TData);
         }

         cache.Add(data);

         return data;
      }

      public IEnumerable<TData> All<TData>( ) {
         if(!TryGetCache(out List<TData> cache)) {
            return null;
         }

         return cache;
      }

      internal void AddCache<TData>( ) where TData : class {
         var type = typeof(TData);

         if (!cache.TryGetValue(type, out ICollection table)) {
            cache[type] = new List<TData>();
         }
      }

      internal bool HasCache<TData>() {
         return cache.ContainsKey(typeof(TData));
      }

      private bool TryGetCache<TData>(out List<TData> cache) {
         var type = typeof(TData);
         cache = null;

         if (this.cache.TryGetValue(type, out ICollection table)) {
            cache = (List<TData>)table;
         }

         return cache != null;
      }
   }
}
