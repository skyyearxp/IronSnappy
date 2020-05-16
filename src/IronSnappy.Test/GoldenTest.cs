using System;
using System.IO;
using Xunit;

namespace IronSnappy.Test
{
   public class GoldenTest
   {
      [Fact]
      public void EncodeGoldenInput()
      {
         byte[] got = Snappy.Encode(File.ReadAllBytes("TestData/Mark.Twain-Tom.Sawyer.txt"));

         byte[] want = File.ReadAllBytes("TestData/Mark.Twain-Tom.Sawyer.rawsnappy.txt");

         Assert.Equal(want.Length, got.Length);

         Assert.Equal(want, got);
      }

      [Fact]
      public void DecodeGoldenInput()
      {
         byte[] got = Snappy.Decode(File.ReadAllBytes("TestData/Mark.Twain-Tom.Sawyer.rawsnappy.txt"));

         byte[] want = File.ReadAllBytes("TestData/Mark.Twain-Tom.Sawyer.txt");

         Assert.Equal(want.Length, got.Length);

         Assert.Equal(want, got);
      }

      [Fact]
      public void RoundtripGoldenData()
      {
         byte[] goldenRaw = File.ReadAllBytes("TestData/Mark.Twain-Tom.Sawyer.txt");
         byte[] compressed = Snappy.Encode(goldenRaw);
         byte[] uncompressed = Snappy.Decode(compressed);

         Assert.Equal(goldenRaw.Length, uncompressed.Length);
         Assert.Equal(goldenRaw, uncompressed);
      }

      [Fact]
      public void RoundtripEncodeBytes()
      {
         byte[] bytes = File.ReadAllBytes("TestData/Mark.Shanghai-skyyearxp.bytes");
         byte[] wants = File.ReadAllBytes("TestData/Mark.Shanghai-skyyearxp.snappy.bytes");

         byte[] compressed = Snappy.Encode(bytes);
         Assert.Equal(wants.Length, compressed.Length);
         Assert.Equal(wants, compressed);

         byte[] uncompressed = Snappy.Decode(compressed);
         Assert.Equal(bytes.Length, uncompressed.Length);
         Assert.Equal(bytes, uncompressed);
      }
   }
}
