using System;
using System.Linq.Expressions;

using Bogus;

namespace RazorWare.CoreDb.Fiction {
   public class DataGenerator<TData> where TData : class {
      private IGenerator generator;

      public DataGenerator(IGenerator dataGenerator) {
         generator = dataGenerator;
      }

      public DataGenerator<TData> RuleFor<TProperty>(Expression<Func<TData, TProperty>> property, Func<Faker, TProperty> setter) {
         generator.RuleFor(property, setter);

         return this;
      }

      public DataGenerator<TData> RuleFor<TProperty>(Expression<Func<TData, TProperty>> property, Func<Faker, TData, TProperty> setter) {
         generator.RuleFor(property, setter);

         return this;
      }

      public Func<TData> Cache( ) {
         return generator.Binder.Cache(generator.Generate);
      }

      public static IGenerator Create(DataBinder dataBinder, string name = null) {
         return new Generator(dataBinder, name ?? typeof(TData).Name);
      }

      public class Generator : IGenerator {
         private readonly string name;
         private readonly Type type;
         private readonly DataBinder binder;

         private IDataFake faker;
         private bool isUnique;

         protected IDataFake Fake => faker;

         string IGenerator.Name => name;
         bool IGenerator.IsUnique => isUnique;
         DataBinder IGenerator.Binder => binder;

         internal Generator(DataBinder dataBinder, string dataName) {
            name = dataName;
            binder = dataBinder;
         }

         public IGenerator RuleFor<TProperty>(Expression<Func<TData, TProperty>> property, Func<Faker, TProperty> setter) {
            Configure().RuleFor(property, setter);

            return this;
         }

         public IGenerator RuleFor<TProperty>(Expression<Func<TData, TProperty>> property, Func<Faker, TData, TProperty> setter) {
            Configure().RuleFor(property, setter);

            return this;
         }

         internal TData Generate( ) {
            return ((IGenerator)this).Generate();
         }

         internal Generator IsUnique(bool unique) {
            isUnique = unique;

            return this;
         }

         private Faker<TData> Configure( ) {
            if (type != null) {
               throw new InvalidOperationException("Cannot make new fake with this object");
            }

            if (faker == null) {
               faker = new DataFake<TData>();
            }

            return (Faker<TData>)faker;
         }

         public override bool Equals(object obj) {
            return obj.GetType() == type;
         }

         public override int GetHashCode( ) {
            return type.GetHashCode();
         }

         TData IGenerator.Generate( ) {
            var fake = (DataFake<TData>)faker;

            return fake.Generate();
         }

         private class DataFake<TFake> : Faker<TFake>, IDataFake
            where TFake : class {

         }

         protected interface IDataFake { }
      }

      public interface IGenerator : IDataGenerator {
         string Name { get; }
         bool IsUnique { get; }
         DataBinder Binder { get; }

         IGenerator RuleFor<TProperty>(Expression<Func<TData, TProperty>> property, Func<Faker, TProperty> setter);
         IGenerator RuleFor<TProperty>(Expression<Func<TData, TProperty>> property, Func<Faker, TData, TProperty> setter);
         TData Generate( );
      }
   }

   public interface IDataGenerator {
   }
}
