using System.IO;

namespace IronSnappy
{
   public static class Snappy
   {
      public static Stream OpenWriter(Stream destination)
      {
         return new SnappyWriter(destination);
      }
   }
}
