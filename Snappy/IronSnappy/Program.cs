using System;
using System.IO;

namespace IronSnappy
{
   class Program
   {
      static void Main(string[] args)
      {
         var ms = new MemoryStream();

         using(Stream w = Snappy.OpenWriter(ms))
         {
            using(FileStream src = File.OpenRead("TestData/Mark.Twain-Tom.Sawyer.txt"))
            {
               src.CopyTo(w);
            }
         }
      }
   }
}