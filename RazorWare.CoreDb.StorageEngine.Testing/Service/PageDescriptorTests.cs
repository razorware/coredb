using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static RazorWare.CoreDb.StorageEngine.StorageConstants;

namespace RazorWare.CoreDb.StorageEngine.Testing {
   [TestClass]
   public class PageDescriptorTests {

      [TestMethod]
      public void NullPageDescriptor( ) {
         // creates a null page descriptor
         var pgDescr = new PageDescriptor();

         Assert.AreEqual(PageDescriptor.Null, pgDescr);
         Assert.AreEqual(0, PageDescriptor.Null.GetHashCode());
         Assert.AreEqual(0, pgDescr.GetHashCode());
         // null descriptor has null partition collection
         Assert.IsNull(PageDescriptor.Null.Partitions);
      }

      [TestMethod]
      public void EmptyPageDescriptor( ) {
         var expEmpty = PageDescriptor.Create(new byte[] { 0 }).Fill(255);

         Assert.AreEqual(expEmpty, PageDescriptor.Empty);
         Assert.AreEqual(-1450246159, PageDescriptor.Empty.GetHashCode());
         Assert.AreNotEqual(PageDescriptor.Null, PageDescriptor.Empty);
         // empty descriptor has 1 partition of 0 length
         Assert.AreEqual(1, PageDescriptor.Empty.Partitions.Count);
         Assert.AreEqual(0, PageDescriptor.Empty.Partitions[0]);
      }

      [TestMethod]
      public void ConfigurePartitions( ) {
         var partitions = new byte[]{ 1, sizeof(long) };
         var pgDescr = PageDescriptor.Create(partitions: partitions);

         Assert.AreEqual(2, pgDescr.Partitions.Count);
         Assert.AreEqual(1, pgDescr.Partitions[0]);
         Assert.AreEqual(sizeof(long), pgDescr.Partitions[1]);
      }

      [TestMethod]
      public void ReadWritePartition( ) {
         var expType = PageType.Master;
         var expDate = DateTime.UtcNow;

         var partitions = new byte[] { 1, sizeof(long) };
         var pgDescr = PageDescriptor.Create(partitions: partitions);

         // this will store a PageType enum value and a time stamp in partions 0, 1 respective
         pgDescr.Write(0, () => (byte)expType);
         pgDescr.Write(1, () => BitConverter.GetBytes(expDate.Ticks));

         var actType = pgDescr.Read(0, b => (PageType)b);
         var actDate = pgDescr.Read(1, b => new DateTime(BitConverter.ToInt64(b, 0)));

         Assert.AreEqual(expType, actType);
         Assert.AreEqual(expDate, actDate);
      }
   }
}
