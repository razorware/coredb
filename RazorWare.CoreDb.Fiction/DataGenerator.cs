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

      public static IGenerator Create(string name = null) {
         return new Generator(name ?? typeof(TData).Name);
      }

      public class Generator : IGenerator {
         private readonly string name;
         private readonly Type type;

         private IDataFake faker;
         private bool isUnique;

         protected IDataFake Fake => faker;

         string IGenerator.Name => name;
         bool IGenerator.IsUnique => isUnique;

         internal Generator(string dataName) {
            name = dataName;
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

         IGenerator RuleFor<TProperty>(Expression<Func<TData, TProperty>> property, Func<Faker, TProperty> setter);
         IGenerator RuleFor<TProperty>(Expression<Func<TData, TProperty>> property, Func<Faker, TData, TProperty> setter);
         TData Generate( );
      }
   }

   public interface IDataGenerator {
   }
}
