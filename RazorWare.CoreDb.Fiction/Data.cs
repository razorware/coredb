using System;
using Bogus;

namespace RazorWare.CoreDb.Fiction {
   public abstract class Data : IDataObject {
      private readonly string name;

      private IDataFake faker;


      protected IDataFake Fake => faker;

      string IDataObject.Name => name;
      

      protected Data(string dataName) {
         name = dataName;
      }

      protected Faker<TFake> MakeFake<TFake>( ) where TFake : class {
         faker = new DataFake<TFake>();

         return (Faker<TFake>)faker;
      }

      TFake IDataObject.Generate<TFake>( ) {
         var fake = (DataFake<TFake>)faker;

         return fake.Generate();
      }

      private class DataFake<TFake> : Faker<TFake>, IDataFake 
         where TFake : class {

      }

      protected interface IDataFake { }
   }

   public interface IDataObject {
      string Name { get; }

      TFake Generate<TFake>( ) where TFake : class;
   }
}
