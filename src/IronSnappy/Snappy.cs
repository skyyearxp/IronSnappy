using System;
using System.Buffers;
using System.IO;

namespace IronSnappy
{
   public static class Snappy
   {

      public static Stream OpenWriter(Stream destination)
      {
         return new SnappyWriter(destination);
      }

      public static byte[] Encode(ReadOnlySpan<byte> src)
      {
         int maxLen = SnappyWriter.GetMaxEncodedLen(src.Length);

         using(var dst = new RentedBuffer(maxLen))
         {
            ReadOnlySpan<byte> compressed = SnappyWriter.Encode(dst.Span, src);

            return compressed.ToArray();
         }
      }
   }
}
