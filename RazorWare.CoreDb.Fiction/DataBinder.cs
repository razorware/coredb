using System;
using System.Linq;
using System.Collections.Generic;

namespace RazorWare.CoreDb.Fiction {
   public class DataBinder<TBinder> {
      private readonly Dictionary<Type, IDataGenerator> generators = new Dictionary<Type, IDataGenerator>();

      private readonly string name;

      public string Name => name;

      public DataBinder(string binderName = null) {
         name = binderName ?? typeof(TBinder).Name;
      }

      public DataGenerator<TData> AddGenerator<TData>( ) where TData : class {
         var type = typeof(TData);

         if (!generators.TryGetValue(type, out IDataGenerator generator)) {
            generators[type] = generator = DataGenerator<TData>.Create();
         }

         return new DataGenerator<TData>((DataGenerator<TData>.IGenerator)generator);
      }

      public TData Generate<TData>( ) where TData : class {
         var type = typeof(TData);

         if (!generators.TryGetValue(type, out IDataGenerator generator)) {
            return null;
         }

         return ((DataGenerator<TData>.IGenerator)generator).Generate();
      }
   }
}
