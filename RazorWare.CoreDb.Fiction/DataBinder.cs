using System;
using System.Linq;
using System.Collections.Generic;
using RazorWare.CoreDb.Interfaces;

namespace RazorWare.CoreDb.Fiction {
   public class DataBinder {
      private readonly Dictionary<Type, IDataGenerator> generators = new Dictionary<Type, IDataGenerator>();
      private readonly IRepository repo;
      private readonly string name;

      public string Name => name;

      public DataBinder(IRepository dataRepo=null, string binderName=null) {
         name = binderName;
         repo = dataRepo;
      }

      public DataGenerator<TData> AddGenerator<TData>( ) where TData : class {
         var type = typeof(TData);

         if (!generators.TryGetValue(type, out IDataGenerator generator)) {
            generators[type] = generator = DataGenerator<TData>.Create(this);
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

      public Func<TData> Cache<TData>(Func<TData> generate) where TData : class {
         if (repo == null) {
            throw new InvalidOperationException("Cannot cache generator objects without a repository");
         }

         if (!generators.ContainsKey(typeof(TData))) {
            AddGenerator<TData>();
         }

         return ( ) => {
            return repo.Add(generate());
         };
      }
   }
}
