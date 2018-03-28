using System;
using System.Linq;
using System.Collections.Generic;

namespace RazorWare.CoreDb.Fiction {
   public abstract class DataBinder {
      private readonly List<Data> dataCollection = new List<Data>();
      private readonly string name;

      public string Name => name;
      public IReadOnlyList<Data> Data => dataCollection;

      protected DataBinder(string binderName) {
         name = binderName;
      }

      protected void AddDataType<TData>(TData data) where TData : Data {
         // can only have one type in collection
         if (!dataCollection.Any(t => t is TData)) {
            dataCollection.Add(data);
         }
      }

      public IEnumerable<TData> Generate<TData>(int numRecords) where TData : Data {
         var records = new List<TData>();

         if (TryGetDataType(out TData tData)) {
            while (records.Count <= numRecords) {
               records.Add(((IDataObject)tData).Generate<TData>());
            }
         }

         return records;
      }

      private bool TryGetDataType<TData>(out TData tData) where TData : Data {
         tData = null;
         var data = default(Data);

         if((data = dataCollection.FirstOrDefault(t => t is TData)) != null) {
            tData = (TData)data;
         }

         return tData != null;
      }
   }
}
