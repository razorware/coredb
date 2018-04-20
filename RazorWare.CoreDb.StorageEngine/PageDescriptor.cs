using System;
using System.Linq;
using System.Collections.Generic;

using RazorWare.Data.Comparers;

namespace RazorWare.CoreDb.StorageEngine {
   /// <summary>
   /// If used as keys in dictionary, proceed with caution. Mutating values within the descriptor will render key unsearchable.
   /// TODO: PageDescriptorCollection
   /// </summary>
   public unsafe struct PageDescriptor : IEquatable<PageDescriptor> {
      private const int Length = 16;
      private static BufferComparer Comparer = new BufferComparer();
      private static readonly PageDescriptor @null = new PageDescriptor();
      private static readonly PageDescriptor empty = new PageDescriptor(new byte[] { 0 }).Fill(255);

      fixed byte buffer[Length];

      private readonly byte[] partIndexes;
      private readonly byte[] parts;

      public static PageDescriptor Null => @null;
      public static PageDescriptor Empty => empty;
      public IReadOnlyDictionary<byte, byte> Partitions => GetPartitions(this);

      private PageDescriptor(byte[] partitions) {
         parts = partitions;
         partIndexes = parts?.Select((p, i) => (byte)i)
            .ToArray();
      }

      public PageDescriptor Fill(byte b) {
         fixed (byte* array = buffer) {
            var idx = 0;
            while (idx < Length) {
               array[idx] = b;

               ++idx;
            }
         }

         return this;
      }

      public static PageDescriptor Create(byte[] partitions) {
         if (partitions == null) {
            return Empty;
         }
         if (partitions.Length == 0) {
            return Null;
         }

         if (partitions.Sum(b => b) > Length) {
            throw new InvalidOperationException($"Sum of partition lengths cannot exceed {Length}");
         }

         return new PageDescriptor(partitions);
      }

      public static implicit operator byte[](PageDescriptor pgDescr) {
         var array = new byte[Length];
         var idx = 0;

         while (idx < Length) {
            array[idx] = pgDescr.buffer[idx];

            ++idx;
         }


         return array;
      }

      public TValue Read<TValue>(byte index, Func<byte, TValue> converter) {
         // this method expects a single byte
         if (parts[index] != 1) {
            throw new InvalidOperationException("Expected length of 1");
         }

         return converter(Read(index)[0]);
      }

      public TValue Read<TValue>(byte index, Func<byte[], TValue> converter) {
         return converter(Read(index));
      }

      public void Write(byte index, Func<byte> converter) {
         Write(index, new byte[] { converter() });
      }

      public void Write(byte index, Func<byte[]> converter) {
         Write(index, converter());
      }

      private byte[] Read(byte index) {
         var include = parts
            .Where((p, i) => i < index)
            .ToArray();
         var start = include.Sum(i => i);

         fixed (byte* array = buffer) {
            var output = new byte[parts[index]];
            var pos = start;
            var idx = 0;

            while(idx < output.Length) {
               output[idx] = array[pos];

               ++pos;
               ++idx;
            }

            return output;
         }
      }

      private void Write(byte index, byte[] source) {
         if(parts[index] != source.Length) {
            throw new InvalidOperationException($"expected length [{parts[index]}] actual: {source.Length}");
         }

         var include = parts
            .Where((p, i) => i < index)
            .ToArray();
         var start = include.Sum(i => i);
         
         fixed (byte* array = buffer) {
            var pos = start;
            var idx = 0;
            while (idx < parts[index]) {
               array[pos] = source[idx];

               ++pos;
               ++idx;
            }
         }
      }

      private static IReadOnlyDictionary<byte, byte> GetPartitions(PageDescriptor pgDescr) {
         if (pgDescr.partIndexes == null) {
            return null;
         }

         return pgDescr.partIndexes
            .ToDictionary(key => key, val => pgDescr.parts[val]);
      }

      public bool Equals(PageDescriptor other) {
         fixed (byte* array = buffer) {
            return Comparer.Equals(array, other.buffer, Length);
         }
      }

      public override bool Equals(object obj) {
         if (!(obj is PageDescriptor)) {
            return false;
         }

         var other = (PageDescriptor)obj;

         return Equals(other);
      }

      public override int GetHashCode( ) {
         var hash = 0;

         fixed (byte* array = buffer) {
            var idx = 0;

            unchecked {
               while (idx < Length) {
                  hash += hash * 17 + array[idx];

                  ++idx;
               }
            }
         }

         return hash;
      }
   }
}
