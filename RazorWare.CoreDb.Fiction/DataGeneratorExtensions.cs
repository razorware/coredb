using System;
using System.Linq;
using System.Collections.Generic;

using RazorWare.CoreDb.Interfaces;

namespace RazorWare.CoreDb.Fiction {
   public delegate TData Generate<TData>( );

   public static class DataGeneratorExtensions {

      public static Generate<TData> Unique<TData, TProperty>(this DataGenerator<TData> generator, IRepository repository, Func<TData, TProperty> qualifier) where TData : class {
         return ( ) => {
            var data = generator.Generate();
            if (!repository.All<TData>().TryGet(qualifier(data), qualifier, out TData original)) {
               original = repository.Add(data);
            }

            return original;
         };
      }

      private static bool TryGet<TData, TProperty>(this IEnumerable<TData> source, TProperty property, Func<TData, TProperty> qualifier, out TData data) where TData : class {
         if (source == null) {
            data = null;
         }
         else {
            data = source.FirstOrDefault(i => qualifier(i).Equals(property));
         }

         return data != default(TData);
      }
   }
}
