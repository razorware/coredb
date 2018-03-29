# coredb
A foray into finding out what it takes to make a database. Unabashedly different with a new take on a query language.

## StorageEngine
The first part to research is the storage engine. Studies of MySQL have multiple storage engines - MyISAM, Federated and InnoDB. SQL Server has a very complex system that minimizes the number of files created.

My initial thought had been a primary file with database information. Databases might be organized by directory so that a 'Customers' database would exist under a 'Customers' directory. Several files may exist 
such as a database schema, table files, indexes, etc. The exact nature and organization of the file system is yet undetermined. I am looking to design a hybrid between the MySQL default _file per table_ and the 
default SQL Server _all in one_ architectures.

Requirements:
* Data generation tools to create tons of data.  
  Looking into: https://github.com/bchavez/Bogus

  Bogus is actually pretty cool. So I designed the `Fiction` project to have a database feel to it. It will act just like a data store.

The `DataBinder` abstract class let me create a data container like a database:  
``` c#
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
   }
```