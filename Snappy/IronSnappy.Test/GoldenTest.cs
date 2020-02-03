using System;
using System.IO;
using Xunit;

namespace IronSnappy.Test
{
   public class GoldenTest
   {
      [Fact]
      public void CompressGoldenData()
      {
         //compress
         var ms = new MemoryStream();
         using(Stream w = Snappy.OpenWriter(ms))
         {
            using(FileStream src = File.OpenRead("TestData/Mark.Twain-Tom.Sawyer.txt"))
            {
               src.CopyTo(w);
            }
         }
         byte[] compressedByMe = ms.ToArray();

         byte[] goldenCompressed = File.ReadAllBytes("TestData/Mark.Twain-Tom.Sawyer.rawsnappy.txt");

         Assert.Equal(goldenCompressed.Length, compressedByMe.Length);

         Assert.Equal(goldenCompressed, compressedByMe);
      }
   }
}
